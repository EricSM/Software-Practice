// Name: Eric Miramontes
// Uid: u0801584

using System;
using System.Collections.Generic;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace SS
{
    /// <summary>
    /// A Spreadsheet represents the state of a spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// A string is a cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.  Cell names
    /// are not case sensitive. In an empty spreadsheet, the contents of every cell is the empty string.
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Dependency graph of the spreadsheet.
        /// </summary>
        private DependencyGraph _dependencies;

        /// <summary>
        /// Hash table of the list of cells.
        /// </summary>
        private Dictionary<string, Cell> _cells;

        /// <summary>
        /// Additional restriction on the vallidity of cell names.
        /// </summary>
        private Regex _isValid;

        // ADDED FOR PS6
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Creates an empty Spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            _dependencies = new DependencyGraph();
            _cells = new Dictionary<string, Cell>();
            _isValid = new Regex(".*");
        }

        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        public Spreadsheet(Regex isValid)
        {
            _dependencies = new DependencyGraph();
            _cells = new Dictionary<string, Cell>();
            _isValid = isValid;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  If there's a problem reading source, throws an IOException
        /// If the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  If there is an invalid cell name, or a 
        /// duplicate cell name, or an invalid formula in the source, throws a SpreadsheetReadException.
        /// If there's a Formula that causes a circular dependency, throws a SpreadsheetReadException. 
        public Spreadsheet(TextReader source)
        {
            _dependencies = new DependencyGraph();
            _cells = new Dictionary<string, Cell>();

            // Create the XmlSchemaSet class.  All spreadsheets will
            // be validated against Spreadsheet.xsd.
            XmlSchemaSet sc = new XmlSchemaSet();

            sc.Add(null, "Spreadsheet.xsd");

            // Configure validation.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += ValidationCallback;

            // Reading in the file.
            using (XmlReader reader = XmlReader.Create(source, settings))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                _isValid = new Regex(reader["IsValid"]); // Read in new IsValid instance.
                                break;

                            case "cell":
                                var name = reader["name"];
                                var content = reader["contents"];
                                if (!IsValid(name)) // Make sure name is valid.
                                {
                                    throw new SpreadsheetReadException("Invalid cell name");
                                }
                                else if (GetCellContents(name) as string != string.Empty) // Make sure there are no duplicates.
                                {
                                    throw new SpreadsheetReadException("Duplicate cells");
                                }
                                else
                                {
                                    try
                                    {
                                        // Set contents of this cell to the content.
                                        SetContentsOfCell(name, content);
                                    }
                                    catch (FormulaFormatException e)
                                    {
                                        throw new SpreadsheetReadException(e.Message);
                                    }
                                    catch (CircularException e)
                                    {
                                        throw new SpreadsheetReadException(e.Message);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            // File was not changed since it was just loaded.
            Changed = false;
        }

        /// <summary>
        /// Throw SpreadsheetReadException if there is a problem reading a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException(e.Message);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name == null || !IsValid(name.ToUpper()))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }
            else if (_cells.ContainsKey(name.ToUpper()))// Check if cell already has been set.
            {
                return _cells[name.ToUpper()].Content;
            }
            else return string.Empty; // Empty cells have empty strings.
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (KeyValuePair<string, Cell> cell in _cells)
            {
                if (!string.IsNullOrEmpty(cell.Value.Content.ToString()))
                {
                    yield return cell.Key;
                }
            }            
        }

        /// <summary>
        /// If formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !IsValid(name.ToUpper()))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }

            var normalizedName = name.ToUpper();

            // Get all the variables from formula and normalize them.
            var newDependees = new HashSet<string>();
            foreach (string var in formula.GetVariables())
            {
                newDependees.Add(var.ToUpper());

                // add variables to hash table of cells if they dont already exist.
                if (!_cells.ContainsKey(var.ToUpper())) _cells.Add(var.ToUpper(), new Cell(""));
            }


            var initialDependencies = new DependencyGraph(_dependencies);

            try
            {
                // Replace old dependees with new ones found in formula.
                _dependencies.ReplaceDependees(normalizedName, newDependees);

                // Get all dependents of this formula. (Throws exception if changes result in circular dependency.)
                var dependentCells = GetCellsToRecalculate(normalizedName);

                if (_cells.ContainsKey(normalizedName)) // Update cell if it exists.
                {
                    _cells[normalizedName].Content = formula;
                    _cells[normalizedName].Value = null;
                }
                else // Add new cell.
                {
                    _cells.Add(normalizedName, new Cell(formula, null));
                }


                // Return all dependents of this cell.
                return new HashSet<string>(dependentCells);

            }
            catch (CircularException e)
            {
                _dependencies = initialDependencies;
                throw e;
            }
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null)// Check if text is null.
            {
                throw new ArgumentNullException();
            }
            else if (name == null || !IsValid(name.ToUpper()))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }

            if (_cells.ContainsKey(name.ToUpper()))// Check if this cell exists.
            {
                // Update cell.
                _cells[name.ToUpper()].Content = text;
                _cells[name.ToUpper()].Value = text;
            }
            else
            {
                // Add new cell to hash table if it does not already exist.
                _cells.Add(name.ToUpper(), new Cell(text));
            }

            // Return all dependents of this cell.
            return new HashSet<string>(GetCellsToRecalculate(name.ToUpper()));
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !IsValid(name.ToUpper()))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }

            if (_cells.ContainsKey(name.ToUpper()))// Check if cell exists.
            {
                // Update cell.
                _cells[name.ToUpper()].Content = number;
                _cells[name.ToUpper()].Value = number;
            }
            else
            {
                // Add new cell to hash table if it does not already exist.
                _cells.Add(name.ToUpper(), new Cell(number));
            }

            // Return all dependents of this cell.
            return new HashSet<string>(GetCellsToRecalculate(name.ToUpper()));
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)// Check if name is null.
            {
                throw new ArgumentNullException();
            }
            else if (!IsValid(name.ToUpper()))// Check if name is invalid.
            {
                throw new InvalidNameException();
            }
            else // Return the dependents of the named cell.
            {
                return _dependencies.GetDependents(name.ToUpper());
            }
        }

        /// <summary>
        /// Makes sure cell name is at least one letter followed by at least one digit.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValid(string name)
        {
            // Check for name validity.
            return Regex.IsMatch(name, @"^([a-zA-Z]+)([1-9])(\d*)$") && _isValid.IsMatch(name);
        }

        // ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the isvalid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            using (XmlWriter writer = XmlWriter.Create(dest))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("IsValid", _isValid.ToString()); // Save _isValid field.

                foreach (string cell in GetNamesOfAllNonemptyCells()) // Iterate through all altered cells.
                {
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", cell); // Save name.

                    var content = GetCellContents(cell);
                    if (content is string) // Check if cell content is a string.
                    {
                        writer.WriteAttributeString("contents", content as string);
                    }
                    else if (content is double) // Check if cell content is a double.
                    {
                        writer.WriteAttributeString("contents", ((double)content).ToString());
                    }
                    else if (content is Formula) // Check if cell content is a Formula.
                    {
                        writer.WriteAttributeString("contents", "=" + content.ToString());
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            // File was just saved, its cannot have been changed since.
            Changed = false;
        }

        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name == null || !IsValid(name.ToUpper()))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }
            else if (_cells.ContainsKey(name.ToUpper()))// Check if cell already has been set.
            {
                return _cells[name.ToUpper()].Value;
            }
            else return string.Empty; // Empty cells have empty strings.
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            double result;
            ISet<string> cellsToRecalculate;
            var normalizedName = name.ToUpper(); // Normalize name.

            if (content == null) // Check if content is null
            {
                throw new ArgumentNullException();
            }
            else if (name == null || !IsValid(normalizedName)) // Check if name is invalid.
            {
                throw new InvalidNameException();
            }
            else if (double.TryParse(content, out result)) // Check if content is a double.
            {                
                cellsToRecalculate = SetCellContents(normalizedName, result);
            }
            else if (content.StartsWith("=")) // Check if content is a formula (must start with a "=").
            {
                Formula formula = new Formula(content.Substring(1), s => s.ToUpper(), s => IsValid(s));
                cellsToRecalculate = SetCellContents(normalizedName, formula);
            }
            else // Content must be a string.
            {
                cellsToRecalculate = SetCellContents(name, content);                
            }


            foreach (string cell in cellsToRecalculate) // Recalculate dependent cells.
            {
                try
                {
                    var formulaToRecalculate = GetCellContents(cell); // Get content (probably formula).

                    if (formulaToRecalculate is Formula) // Cell originally passed in might not be a formula anymore.
                    {
                        // Recalculate.
                        _cells[cell].Value = ((Formula)formulaToRecalculate).Evaluate(s =>
                        {
                            // Lambda method for finding values of dependees.
                            var value = GetCellValue(s); // Get value
                            if (value is FormulaError || value is string) // Value must be a number.
                            {
                                throw new UndefinedVariableException(s);
                            }
                            return (double)value;
                        });
                    }
                }
                catch (FormulaEvaluationException e) // If error evaluating, set value to a FormulaError.
                {
                    _cells[cell].Value = new FormulaError(e.Message);
                }
            }

            Changed = true; // Spreadsheet was just changed.

            return cellsToRecalculate;
        }
    }
}

// Name: Eric Miramontes
// Uid: u0801584

using System;
using System.Collections.Generic;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;

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
            else if (_cells.ContainsKey(name.ToUpper()))
            {
                return _cells[name.ToUpper()].Content;
            }
            else return string.Empty;
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

        public override void Save(TextWriter dest)
        {
            throw new NotImplementedException();
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
            if (name == null || !IsValid(name.ToUpper()))
            {
                throw new InvalidNameException();
            }
            else if (_cells.ContainsKey(name.ToUpper()))
            {
                return _cells[name.ToUpper()].Value;
            }
            else return string.Empty;
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
            var normalizedName = name.ToUpper();

            if (content == null)
            {
                throw new ArgumentNullException();
            }
            else if (name == null || !IsValid(normalizedName))
            {
                throw new InvalidNameException();
            }
            else if (double.TryParse(content, out result))
            {                
                cellsToRecalculate = SetCellContents(normalizedName, result);
                Changed = true;
            }
            else if (content.StartsWith("="))
            {
                Formula formula = new Formula(content.Substring(1), s => s.ToUpper(), s => IsValid(s));
                cellsToRecalculate = SetCellContents(normalizedName, formula);
                Changed = true;
            }
            else
            {
                cellsToRecalculate = SetCellContents(name, content);
                Changed = true;
            }


            foreach (string cell in cellsToRecalculate)
            {
                try
                {
                    if (_cells[cell].Content is Formula)
                    {
                        _cells[cell].Value = ((Formula)_cells[cell].Content).Evaluate(s =>
                        {
                            var value = GetCellValue(s);
                            if (value is FormulaError || value is string)
                            {
                                throw new UndefinedVariableException(s);
                            }
                            return (double)value;
                        });
                    }
                }
                catch (FormulaEvaluationException e)
                {
                    _cells[cell].Value = new FormulaError(e.Message);
                }
            }


            return cellsToRecalculate;
        }
    }
}

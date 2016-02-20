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

        public override bool Changed
        {
            get
            {
                throw new NotImplementedException();
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates an empty Spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            _dependencies = new DependencyGraph();
            _cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name == null || !IsValid(name))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }

            if (_cells.ContainsKey(name.ToUpper()))
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
            if (name == null || !IsValid(name))// Check if name is null or invalid.
            {
                throw new InvalidNameException();
            }


            // Get all the variables from formula and normalize them.
            var newDependees = new HashSet<string>();
            foreach (string var in formula.GetVariables())
            {
                newDependees.Add(var.ToUpper());

                // add variables to hash table of cells if they dont already exist.
                if (!_cells.ContainsKey(var.ToUpper())) _cells.Add(var.ToUpper(), new Cell(""));
            }

            // Replace old dependees with new ones found in formula.
            _dependencies.ReplaceDependees(name.ToUpper(), newDependees);

            // Get all dependents of this formula. (Throws exception if changes result in circular dependency.)
            var dependentCells = GetCellsToRecalculate(name.ToUpper());



            object value;
            try
            {
                value = formula.Evaluate(s => (double)_cells[s].Value); // Evaluate formula
            }
            catch (Exception e) // If it fails, value is a FormulaError.
            {
                value = new FormulaError(e.Message);
            }

            if (_cells.ContainsKey(name.ToUpper())) // Update cell if it exists.
            {
                _cells[name.ToUpper()].Content = formula;
                _cells[name.ToUpper()].Value = value;
            }
            else // Add new cell.
            {
                _cells.Add(name.ToUpper(), new Cell(formula, value));
            }


            // Return all dependents of this cell.
            return new HashSet<string>(dependentCells);
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
            else if (name == null || !IsValid(name))// Check if name is null or invalid.
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
            if (name == null || !IsValid(name))// Check if name is null or invalid.
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
            else if (!IsValid(name))// Check if name is invalid.
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
            return Regex.IsMatch(name, @"^([a-zA-Z]+)([1-9])(\d*)$");
        }

        public override void Save(TextWriter dest)
        {
            throw new NotImplementedException();
        }

        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            throw new NotImplementedException();
        }
    }
}

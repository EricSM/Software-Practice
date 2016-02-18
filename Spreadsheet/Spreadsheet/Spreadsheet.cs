using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;

namespace SS
{
    class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph _dependencies;
        private Dictionary<string, Cell> _cells;

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
            if (string.IsNullOrEmpty(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            return _cells[name].Content;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (KeyValuePair<string, Cell> cell in _cells)
            {
                if (string.IsNullOrEmpty(cell.Value.Content.ToString()))
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
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            throw new NotImplementedException();
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
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException();
            }
            else if (name == null || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            if (_cells.ContainsKey(name))
            {
                _cells[name] = new Cell(text);
            }
            else
            {
                _cells.Add(name, new Cell(text));
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            if (_cells.ContainsKey(name))
            {
                _cells[name] = new Cell(number);
            }
            else
            { 
                _cells.Add(name, new Cell(name));
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
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
            throw new NotImplementedException();
        }

        private bool IsValid(string name)
        {
            return Regex.IsMatch(name, @"^([a-zA-Z]+)([1-9])(\d*)$");
        }
    }
}

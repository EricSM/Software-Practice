using Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    /// <summary>
    /// A cell in a spreadsheet.
    /// </summary>
    class Cell
    {
        /// <summary>
        /// The content of the cell.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// The value of the cell.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Creates a cell from a double.
        /// </summary>
        /// <param name="value"></param>
        public Cell(double value)
        {
            Content = value;
            Value = value;
        }

        /// <summary>
        /// Creates a cell from a string.
        /// </summary>
        /// <param name="content"></param>
        public Cell(string content)
        {
            Content = content;
            Value = content;
        }

        /// <summary>
        /// Creates a cell from a formula and its value.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="value"></param>
        public Cell(Formula formula, object value)
        {
            Content = formula;
            Value = value;
        }
    }
}

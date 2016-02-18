using Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    class Cell
    {
        public object Content { get; set; }
        public object Value { get; set; }

        
        public Cell(double value)
        {
            Content = value;
            Value = value;
        }

        public Cell(string content)
        {
            Content = content;
            Value = content;
        }
        public Cell(Formula formula, object value)
        {
            Content = formula;
            Value = value;
        }
    }
}

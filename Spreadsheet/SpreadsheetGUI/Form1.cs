using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSGui;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form, ISpreadsheetView
    {
        private SelectionChangedHandler _selectionChanged;

        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<string> UpdateEvent;

       

        private void button1_Click(object sender, EventArgs e)
        {
            if (UpdateEvent != null)
            {
                UpdateEvent(formulaTextBox.Text);
            }
        }

        //public Action<SpreadsheetPanel> SSPanel { set { spreadsheetPanel1.SelectionChanged += value; } }

        /// <summary>
        /// 
        /// </summary>
        public string CellName
        {
            set
            {
                nameTextBox.Text = value.ToString();
            }

            get
            {
                return nameTextBox.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CellValue
        {
            set
            {
                valueTextBox.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CellContent
        {
            set
            {
                formulaTextBox.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SelectionChangedHandler SelectionChanged
        {
            get
            {
                return _selectionChanged;
            }

            set
            {
                _selectionChanged = value;
                spreadsheetPanel1.SelectionChanged += SelectionChanged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="content"></param>
        public void SetCell(int col, int row, string content)
        {
            spreadsheetPanel1.SetValue(col, row, content);
        }
    }
}

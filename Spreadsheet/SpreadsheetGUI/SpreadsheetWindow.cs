using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SSGui;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    {
        private SelectionChangedHandler _selectionChanged;

        /// <summary>
        /// 
        /// </summary>
        public SpreadsheetWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<string> UpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public event Action NewEvent;

        /// <summary>
        /// 
        /// </summary>
        public event Action<string> OpenEvent;


        public event Action<string> SaveEvent;

        public event Action CloseEvent;

        public event Action HelpEvent;


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
        public string Title {
            set
            {
                Text = value;
            }
        }

        /// <summary>
        /// Shows the message in the UI.
        /// </summary>
        public string Message
        {
            set { MessageBox.Show(value); }
        }


        public bool Changed
        {
            set
            {
                if (value)
                {
                    if (Text.Last() != '*')
                    {
                        Text += "*";
                    }
                }
                else
                {
                    Text = saveFileDialog.FileName;
                }
            }

            get
            {
                return Text.Last() == '*';
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


        /// <summary>
        /// Closes this window
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UpdateEvent != null)
            {
                UpdateEvent(formulaTextBox.Text);
                if (Text.Last() != '*')
                {
                    Text += "*";
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (OpenEvent != null)
                {
                    OpenEvent(openFileDialog.FileName);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (SaveEvent != null)
            {
                SaveEvent(saveFileDialog.FileName);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HelpEvent != null)
            {
                HelpEvent();
            }
        }

        private void SpreadsheetWindow_FormClosing(object sender, CancelEventArgs e)
        {
            if (Changed)
            {
                DialogResult result = MessageBox.Show("Unsaved changes.\nDo you wish to save your spreadsheet?",
                        "Unsaved Spreadsheet",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    saveFileDialog.ShowDialog();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

// Name: Eric Miramontes
// Uid: u0801584

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
        /// Initialize window.
        /// </summary>
        public SpreadsheetWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fired when update button is clicked.
        /// </summary>
        public event Action<string> UpdateEvent;

        /// <summary>
        /// Fired when "New" is selected in the file menu.
        /// </summary>
        public event Action NewEvent;

        /// <summary>
        /// Fired when a file is chosen with a file dialog.
        /// The parameter is the chosen filename.
        /// </summary>
        public event Action<string> OpenEvent;

        /// <summary>
        /// Fired when a file is chosen or created with a file dialog.
        /// The parameter is the chosen filename.
        /// </summary>
        public event Action<string> SaveEvent;

        /// <summary>
        /// Fired when the user tries to close the spreadsheet.
        /// </summary>
        public event Action CloseEvent;
        
        /// <summary>
        /// Fired when the user clicks the help option in the file menu.
        /// </summary>
        public event Action HelpEvent;


        /// <summary>
        /// The name of the selected cell displayed in the read-only textbox labeled "Cell"
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
        /// The value of the selected cell displayed in the read-only textbox labeled "Value"
        /// </summary>
        public string CellValue
        {
            set
            {
                valueTextBox.Text = value.ToString();
            }
        }

        /// <summary>
        /// The contents of the selected cell displayed in the texbox labeled "Formula"
        /// </summary>
        public string CellContent
        {
            set
            {
                formulaTextBox.Text = value.ToString();
            }
        }

        /// <summary>
        /// The filename of the spreadsheet file displayed in the window header.
        /// </summary>
        public string Title {
            set
            {
                Text = value;
            }
        }

        /// <summary>
        /// Shows a pop-up message in the window.
        /// </summary>
        public string Message
        {
            set { MessageBox.Show(value); }
        }

        /// <summary>
        /// Whether the spreadsheet has been changed since it was last loaded or saved.
        /// Indicated by asterisk (*) in window.
        /// </summary>
        public bool Changed
        {
            set
            {
                if (value) // If it has been changed
                {
                    if (Text.Last() != '*') // Add asterisk if there is not already one.
                    {
                        Text += "*";
                    }
                }
                else // Spreadsheet is no longer changed since it was saved.
                {
                    Text = saveFileDialog.FileName; // Name is last file name spreadsheet was saved to.
                }
            }

            get
            {
                return Text.Last() == '*'; // Returns true if the window header contains an asterisk indicator
            }
        }

        /// <summary>
        /// Handler for when the selected cell has been changed.
        /// </summary>
        public SelectionChangedHandler SelectionChanged
        {
            get
            {
                return _selectionChanged; // Return method
            }

            set
            {
                _selectionChanged = value; // Assign method to handler
                spreadsheetPanel1.SelectionChanged += SelectionChanged; // Add handler to event in SpreadsheetPanel
            }
        }

        /// <summary>
        /// Set content displayed in a particular cell in the SpreadsheetPanel.
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

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (UpdateEvent != null)
            {
                UpdateEvent(formulaTextBox.Text);
                Changed = true;
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
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (OpenEvent != null)
            {
                OpenEvent(openFileDialog.FileName);
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

        /// <summary>
        /// Handles the event of the window closing.
        /// </summary>
        private void SpreadsheetWindow_FormClosing(object sender, CancelEventArgs e)
        {
            if (Changed) // Check for unsaved changes.
            {
                DialogResult result = MessageBox.Show("Unsaved changes.\nDo you wish to save your spreadsheet?",
                        "Unsaved Spreadsheet",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2); // Pop-up message box

                if (result == DialogResult.Yes)
                {
                    saveFileDialog.ShowDialog(); // User wishes to save
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // If user clicks cancel in message box, cancel closing operation
                }
            }
        }
    }
}

// Name: Eric Miramontes
// Uid: u0801584

using Formulas;
using SS;
using SSGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Controls the operation of an ISpreadsheetView
    /// </summary>
    public class Controller
    {
        private ISpreadsheetView window;

        private Spreadsheet spreadsheet;

        /// <summary>
        /// Begins controlling window.
        /// </summary>
        /// <param name="window">The window being controlled</param>
        public Controller(ISpreadsheetView window)
        {
            this.window = window;
            this.spreadsheet = new Spreadsheet(new Regex(@"[A-Z][1-9](\d?)"));

            AddHandlers();
        }
        
        /// <summary>
        /// Begins controlling window with a loaded spreadsheet.
        /// </summary>
        /// <param name="window">The window being controlled</param>
        /// <param name="spreadsheet">The spreadsheet being loaded to the window</param>
        /// <param name="filename">The filename of the spreadsheet</param>
        public Controller(ISpreadsheetView window, Spreadsheet spreadsheet, string filename)
        {
            this.window = window;
            this.spreadsheet = spreadsheet;

            window.Title = filename;


            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells()) // Populate the cells in the window.
            {
                string[] rowAndCol = Regex.Split(cell, @"(\d+)");
                
                window.SetCell((rowAndCol[0].ToCharArray()[0] - 'A'), int.Parse(rowAndCol[1]) - 1, spreadsheet.GetCellValue(cell).ToString());
            }

            // Display information about the default selected cell (usually "A1").
            window.CellValue = spreadsheet.GetCellValue(window.CellName).ToString();
            window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                spreadsheet.GetCellContents(window.CellName).ToString();


            AddHandlers();
           
        }

        /// <summary>
        /// Handles a request to select and display information about a certain cell.
        /// </summary>
        /// <param name="ss">Spreadsheet panel where new cell has been selected</param>
        private void HandleDisplaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            ss.GetSelection(out col, out row); // Retrieve row and col.
            ss.GetValue(col, row, out value); // Retrieve value displayed in selected cell
            window.CellName = ((char)('A' + col)).ToString() + (row + 1); // Display name of cell
            window.CellValue = value; // Display value of cell
            window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                spreadsheet.GetCellContents(window.CellName).ToString(); // Display cell content (add an '=' if it is a formula)
        }

        /// <summary>
        /// Handles a request to update a cell.
        /// </summary>
        /// <param name="content"></param>
        private void HandleUpdateSelection(string content)
        {
            var initialContent = spreadsheet.GetCellContents(window.CellName); // Save original content of cell

            // Try to set content of cell
            try {
                foreach (string cell in spreadsheet.SetContentsOfCell(window.CellName, content)) // Iterate through all cells that may need to be updated
                {
                    string[] rowAndCol = Regex.Split(cell, @"(\d+)"); // Retrieve name of column and row number.

                    // Display new value of updated cell
                    window.SetCell((rowAndCol[0].ToCharArray()[0] - 'A'), int.Parse(rowAndCol[1]) - 1, spreadsheet.GetCellValue(cell).ToString());
                }

                // Display new info about selected cell
                window.CellValue = spreadsheet.GetCellValue(window.CellName).ToString();
                window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                    spreadsheet.GetCellContents(window.CellName).ToString();

                window.Changed = spreadsheet.Changed; // File has been changed
            }
            catch (Exception e) // If attempt to change cell results in an invalid formula or circular dependency
            {
                window.Message = "Error: \n" + e.Message; // Display error message
                HandleUpdateSelection(initialContent.ToString()); // Change cells back to what they were previously.
            }                       
        }

        /// <summary>
        /// Handles a request to open a new window.
        /// </summary>
        private void HandleNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Handles a request to load an existing spreadsheet to a new window.
        /// </summary>
        /// <param name="filename">Name of existing spreadsheet file</param>
        private void HandleOpen(string filename)
        {
            try
            {
                var newSpreadsheet = new Spreadsheet(new StreamReader(filename));
                                
                SpreadsheetApplicationContext.GetContext().RunNew(newSpreadsheet, filename);
            }
            catch (Exception e)
            {
                window.Message = "Unable to open file\n" + e.Message;
            }
        }

        /// <summary>
        /// Handles request to save a spreadsheet to a file.
        /// </summary>
        /// <param name="filename"></param>
        private void HandleSave(string filename)
        {
            try
            {
                spreadsheet.Save(new StreamWriter(filename));
                window.Changed = spreadsheet.Changed;
            }
            catch (Exception e)
            {
                window.Message = "Unable to save file\n" + e.Message;
            }
        }

        /// <summary>
        /// Handles request to close the spreadsheet.
        /// </summary>
        private void HandleClose()
        {
            window.DoClose();
        }

        /// <summary>
        /// Handles request to display help to the user.
        /// </summary>
        private void HandleHelp()
        {
            window.Message = "Turorial:\n\n"
                + "Update Cells:\n"
                + "To update the value of a cell, click on it and put in the desired value into the formula textbox, then "
                + "push the button marked \"Update\".  Note: an asterix (*) will appear next to the file name in the window " 
                + "header if ther are any unsaved changes.  You can put text or numbers into the cell.  To put in a formula " 
                + "that uses other cells, type an equals sign (=) into the formula textbox and then type in your formula.  " 
                + "Then tap \"Update\".  Any dependent cells in the spreadsheet will be automatically updated.  The value " 
                + "of your selected cell will be displayed in the textbox labeled \"Value\".  \n\n"
                
                + "New Window:\n"
                + "To open a new window click \"New\" in the help menu.  \n\n" 
                
                + "Open Spreadsheet:\n"
                + "To open an ss file, click \"Open\" in the file menu and choose your file.  \n\n" 
                
                + "Save Spreadsheet:\n"
                + "To save a spreadsheet, click \"Save\" then give your file a name, then click ok.  You may overwrite existing " 
                + "files if you wish.  \n\n" 
                
                + "Close Spreadsheet:\n"
                + "To close the spreadsheet, click \"Close\" in the file menu or click on the exit button on the window. If there "
                + "are any unsaved changes, the spreadsheet will prompt you to either save, not save, or cancel close operation.";
        }

        /// <summary>
        /// Adds handlers to window's events.
        /// </summary>
        private void AddHandlers()
        {
            window.UpdateEvent += HandleUpdateSelection;
            window.SelectionChanged += HandleDisplaySelection;
            window.NewEvent += HandleNew;
            window.OpenEvent += HandleOpen;
            window.SaveEvent += HandleSave;
            window.CloseEvent += HandleClose;
            window.HelpEvent += HandleHelp;
        }
    }
}

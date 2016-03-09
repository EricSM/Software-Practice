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
        public Controller(ISpreadsheetView window)
        {
            this.window = window;
            this.spreadsheet = new Spreadsheet(new Regex(@"[A-Z][1-9](\d?)"));

            AddEvents();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="spreadsheet"></param>
        /// <param name="filename"></param>
        public Controller(ISpreadsheetView window, Spreadsheet spreadsheet, string filename)
        {
            this.window = window;
            this.spreadsheet = spreadsheet;

            window.Title = filename;


            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                string[] rowAndCol = Regex.Split(cell, @"(\d+)");
                
                window.SetCell((rowAndCol[0].ToCharArray()[0] - 'A'), int.Parse(rowAndCol[1]) - 1, spreadsheet.GetCellValue(cell).ToString());
            }
            window.CellValue = spreadsheet.GetCellValue(window.CellName).ToString();
            window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                spreadsheet.GetCellContents(window.CellName).ToString();


            AddEvents();
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        private void HandleDisplaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);
            window.CellName = ((char)('A' + col)).ToString() + (row + 1);
            window.CellValue = value;
            window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                spreadsheet.GetCellContents(window.CellName).ToString();
        }

        private void HandleUpdateSelection(string content)
        {
            var initialContent = spreadsheet.GetCellContents(window.CellName);

            try {
                foreach (string cell in spreadsheet.SetContentsOfCell(window.CellName, content))
                {
                    string[] rowAndCol = Regex.Split(cell, @"(\d+)");

                    window.SetCell((rowAndCol[0].ToCharArray()[0] - 'A'), int.Parse(rowAndCol[1]) - 1, spreadsheet.GetCellValue(cell).ToString());
                }

                window.CellValue = spreadsheet.GetCellValue(window.CellName).ToString();
                window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                    spreadsheet.GetCellContents(window.CellName).ToString();

                window.Changed = spreadsheet.Changed;
            }
            catch (Exception e)
            {
                window.Message = "Error: \n" + e.Message;
                HandleUpdateSelection(initialContent.ToString());
            }                       
        }

        /// <summary>
        /// Handles a request to open a new window.
        /// </summary>
        private void HandleNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

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

        private void HandleClose()
        {
            window.DoClose();
        }

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

        private void AddEvents()
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

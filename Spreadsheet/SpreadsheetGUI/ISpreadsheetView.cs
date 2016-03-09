// Name: Eric Miramontes
// Uid: u0801584

using SS;
using SSGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Controllable interface of SpreadsheetView
    /// </summary>
    public interface ISpreadsheetView
    {
        /// <summary>
        /// Fired when update button is clicked.
        /// </summary>
        event Action<string> UpdateEvent;

        /// <summary>
        /// Fired when "New" is selected in the file menu.
        /// </summary>
        event Action NewEvent;

        /// <summary>
        /// Fired when a file is chosen with a file dialog.
        /// The parameter is the chosen filename.
        /// </summary>
        event Action<string> OpenEvent;

        /// <summary>
        /// Fired when a file is chosen or created with a file dialog.
        /// The parameter is the chosen filename.
        /// </summary>
        event Action<string> SaveEvent;

        /// <summary>
        /// Fired when the user tries to close the spreadsheet.
        /// </summary>
        event Action CloseEvent;

        /// <summary>
        /// Fired when the user clicks the help option in the file menu.
        /// </summary>
        event Action HelpEvent;

        /// <summary>
        /// Handler for when the selected cell has been changed.
        /// </summary>
        SelectionChangedHandler SelectionChanged { get; set; }

        /// <summary>
        /// The name of the selected cell
        /// </summary>
        string CellName { get; set; }
        
        /// <summary>
        /// The value of the selected cell
        /// </summary>
        string CellValue { set; }
        
        /// <summary>
        /// The contents of the selected cell
        /// </summary>
        string CellContent { set; }
        
        /// <summary>
        /// The name of the file
        /// </summary>
        string Title { set; }
        
        /// <summary>
        /// A message to be displayed with a message box.
        /// </summary>
        string Message { set; }

        /// <summary>
        /// Whether the spreadhsheet has been changed.
        /// </summary>
        bool Changed { set; get; }
        
        /// <summary>
        /// Set content displayed in a particular cell
        /// </summary>
        void SetCell(int col, int row, string content);

        /// <summary>
        /// Method for closing a window
        /// </summary>
        void DoClose();
    }
}

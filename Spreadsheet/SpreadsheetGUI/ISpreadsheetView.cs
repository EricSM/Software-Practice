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
        event Action<string> UpdateEvent;

        event Action NewEvent;

        event Action<string> OpenEvent;

        event Action<string> SaveEvent;

        event Action CloseEvent;

        event Action HelpEvent;
        
        SelectionChangedHandler SelectionChanged { get; set; }
        
        string CellName { get; set; }
        
        string CellValue { set; }
        
        string CellContent { set; }
        
        string Title { set; }
        
        string Message { set; }

        bool Changed { set; get; }
        
        void SetCell(int col, int row, string content);

        void DoClose();
    }
}

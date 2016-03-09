// Name: Eric Miramontes
// Uid: u0801584

using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSGui;

namespace ControllerTester
{
    class SpreadsheetViewStub : ISpreadsheetView
    {
        // These three properties record whether a method has been called
        public bool CalledDoClose { get; private set; }

        public bool CalledSetCell { get; private set; }

        public bool CalledNewEvent { get; private set; }

        // These two properties record the selected row and column
        public int Row { get; private set; }

        public int Col { get; private set; }
               
        // These seven methods cause events to be fired
        public void FireCloseEvent()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }
        public void FireHelpEvent()
        {
            if (HelpEvent != null)
            {
                HelpEvent();
            }
        }

        public void FireNewEvent()
        {
            if (NewEvent != null)
            {
                CalledNewEvent = true;
            }
        }

        public void FireOpenEvent(string filename)
        {
            if (OpenEvent != null)
            {
                OpenEvent(filename);
            }
        }

        public void FireSaveEvent(string filename)
        {
            if (SaveEvent != null)
            {
                SaveEvent(filename);
                Title = filename;
            }
        }

        public void FireUpdateEvent(string content)
        {
            if (UpdateEvent != null)
            {
                UpdateEvent(content);
            }
        }

        public void FireSelectionChangedEvent()
        {
            SelectionChanged(new SpreadsheetPanel());
        }


        // These seven properties implement the interface
        public string CellContent{ get; set; }

        public string CellName { get; set; }

        public string CellValue { get; set; }

        public bool Changed { get; set; }

        public string Message { get; set; }

        public SelectionChangedHandler SelectionChanged { get; set; }

        public string Title { get; set; }


        // These six events implement the interface
        public event Action CloseEvent;
        public event Action HelpEvent;
        public event Action NewEvent;
        public event Action<string> OpenEvent;
        public event Action<string> SaveEvent;
        public event Action<string> UpdateEvent;


        // These two methods implement the interface
        public void DoClose()
        {
            CalledDoClose = true;
        }

        public void SetCell(int col, int row, string content)
        {
            CellContent = content;
            CalledSetCell = true;
            Col = col;
            Row = row;
            CellName = (char)(col + 'A') + (row + 1).ToString();
        }
    }
}

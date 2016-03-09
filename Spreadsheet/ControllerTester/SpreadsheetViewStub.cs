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
        public bool CalledDoClose { get; private set; }

        public bool CalledSetCell { get; private set; }



        public string CellContent{ get; set; }

        public string CellName { get; set; }

        public string CellValue { get; set; }

        public bool Changed { get; set; }

        public string Message { get; set; }

        public SelectionChangedHandler SelectionChanged { get; set; }

        public string Title { get; set; }

        public event Action CloseEvent;
        public event Action HelpEvent;
        public event Action NewEvent;
        public event Action<string> OpenEvent;
        public event Action<string> SaveEvent;
        public event Action<string> UpdateEvent;

        public void DoClose()
        {
            CalledDoClose = true;
        }

        public void SetCell(int col, int row, string content)
        {
            CellContent = content;
            CalledSetCell = true;
        }
    }
}

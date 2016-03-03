﻿using SSGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISpreadsheetView
    {
        /// <summary>
        /// 
        /// </summary>
        event Action<string> UpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        SelectionChangedHandler SelectionChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string CellName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string CellValue { set; }
        
        /// <summary>
        /// 
        /// </summary>
        string CellContent { set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="content"></param>
        void SetCell(int col, int row, string content);
    }
}

﻿using Formulas;
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
            this.spreadsheet = new Spreadsheet();

            window.UpdateEvent += HandleUpdateSelection;
            window.SelectionChanged += HandleDisplaySelection;
            window.NewEvent += HandleNew;
            window.OpenEvent += HandleOpen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="spreadsheet"></param>
        public Controller(ISpreadsheetView window, Spreadsheet spreadsheet)
        {
            this.window = window;
            this.spreadsheet = spreadsheet;

            window.UpdateEvent += HandleUpdateSelection;
            window.SelectionChanged += HandleDisplaySelection;
            window.NewEvent += HandleNew;
            window.OpenEvent += HandleOpen;
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

            foreach (string cell in spreadsheet.SetContentsOfCell(window.CellName, content))
            {
                string[] rowAndCol = Regex.Split(cell, @"(\d+)");


                
                window.SetCell((rowAndCol[0].ToCharArray()[0] - 'A'), int.Parse(rowAndCol[1]) - 1, spreadsheet.GetCellValue(cell).ToString());
            }

            window.CellValue = spreadsheet.GetCellValue(window.CellName).ToString();
            window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") + 
                spreadsheet.GetCellContents(window.CellName).ToString();
        }

        /// <summary>
        /// Handles a request to open a new window.
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        private void HandleOpen(string filename)
        {
            try
            {
                window.OpenExisting(spreadsheet = new Spreadsheet(new StreamReader(filename)));

                foreach(string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    string[] rowAndCol = Regex.Split(cell, @"(\d+)");



                    window.SetCell((rowAndCol[0].ToCharArray()[0] - 'A'), int.Parse(rowAndCol[1]) - 1, spreadsheet.GetCellValue(cell).ToString());
                }
                window.CellValue = spreadsheet.GetCellValue(window.CellName).ToString();
                window.CellContent = (spreadsheet.GetCellContents(window.CellName) is Formula ? "=" : "") +
                    spreadsheet.GetCellContents(window.CellName).ToString();


                window.Title = filename;
            }
            catch (Exception ex)
            {
                window.Message = "Unable to open file\n" + ex.Message;
            }
        }
    }
}
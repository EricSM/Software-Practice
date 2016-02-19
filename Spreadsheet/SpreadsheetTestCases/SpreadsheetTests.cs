// Name: Eric Miramontes
// Uid: u0801584

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;

namespace SpreadsheetTestCases
{
    [TestClass]
    public class SpreadsheetTests
    {        
        /// <summary>
        /// Tests for GetCellContents
        /// </summary>

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("1a");
        }

        [TestMethod]
        public void TestGetCellContents3()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual(ss.GetCellContents("a1"), string.Empty);
        }


        /// <summary>
        /// Tests for SetCellContents
        /// </summary>

        [TestMethod]
        public void TestSetCellContents1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", 1);
            Assert.AreEqual(ss.GetCellContents("a1"), 1d);
        }

        [TestMethod]
        public void TestSetCellContents2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", "Hello World");
            Assert.AreEqual(ss.GetCellContents("a1"), "Hello World");
        }

        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContents3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("1a", "Hello World");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents5()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("1a", 0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents6()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("1a", new Formula());
        }

        [TestMethod]
        public void TestSetCellContents7()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", new Formula("a2"));
            ss.SetCellContents("a2", new Formula("a3"));
            var cells = new HashSet<string>(ss.SetCellContents("a3", 0d));

            Assert.IsTrue(cells.SetEquals(new HashSet<string> { "A1", "A2", "A3" }));
        }

        /// <summary>
        /// This method also tests GetNamesOfAllNonemptyCells
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells1()
        {
            Spreadsheet ss = new Spreadsheet();
            for (int i = 1; i <= 5; i++) // Set cells a1-5
            {
                ss.SetCellContents("a" + i, "Hello World");
            }

            var cells = new HashSet<string>(ss.GetNamesOfAllNonemptyCells()); // Retrieve them.
            Assert.IsTrue(cells.SetEquals(new HashSet<string> { "A1", "A2", "A3", "A4", "A5" }));
        }
    }
}

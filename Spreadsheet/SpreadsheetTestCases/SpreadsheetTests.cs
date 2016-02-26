// Name: Eric Miramontes
// Uid: u0801584

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

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
            ss.SetContentsOfCell("a1", 1.ToString());
            Assert.AreEqual(ss.GetCellContents("a1"), 1d);
        }

        [TestMethod]
        public void TestSetCellContents2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "Hello World");
            Assert.AreEqual(ss.GetCellContents("a1"), "Hello World");
        }

        [TestMethod]
        public void TestSetCellContents3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "=a2 + a3");
            Assert.IsTrue(string.Equals(ss.GetCellContents("a1").ToString(), new Formula("A2+A3").ToString(),
                StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContents4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents5()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("1a", "Hello World");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents6()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("1a", 0d.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContents7()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("1a", "=");
        }

        [TestMethod]
        public void TestSetCellContents8()
        {
            Spreadsheet ss = new Spreadsheet(new Regex(@"^[A-Z](\d*)$"));
            ss.SetContentsOfCell("a1", "=a2");
            ss.SetContentsOfCell("a2", "=a3");
            var cells = new HashSet<string>(ss.SetContentsOfCell("a3", 0d.ToString()));

            Assert.IsTrue(cells.SetEquals(new HashSet<string> { "A1", "A2", "A3" }));
        }

        /// <summary>
        /// This method also tests GetNamesOfAllNonemptyCells
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells1()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);

            for (int i = 1; i <= 5; i++) // Set cells a1-5
            {
                ss.SetContentsOfCell("a" + i, "Hello World");
            }

            var cells = new HashSet<string>(ss.GetNamesOfAllNonemptyCells()); // Retrieve them.
            Assert.IsTrue(cells.SetEquals(new HashSet<string> { "A1", "A2", "A3", "A4", "A5" }));
            Assert.IsTrue(ss.Changed);
        }

        [TestMethod]
        public void Test1()
        {
            Spreadsheet ss = new Spreadsheet(new StreamReader("SampleSavedSpreadsheet.xml"));
            Assert.AreEqual((double) ss.GetCellValue("A3"), 35d, .0001);
            Assert.AreEqual(ss.GetCellValue("B2"), "Hello");
        }
    }
}

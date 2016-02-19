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
    public class UnitTest1
    {        
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("1a");
        }

        [TestMethod]
        public void TestMethod3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", 1);
            Assert.AreEqual(ss.GetCellContents("a1"), 1d);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", "Hello World");
            Assert.AreEqual(ss.GetCellContents("a1"), "Hello World");
        }

        [TestMethod]
        public void TestMethod5()
        {
            Spreadsheet ss = new Spreadsheet();
            for (int i = 1; i <= 5; i++)
            {
                ss.SetCellContents("a" + i, "Hello World");
            }

            var cells = new HashSet<string>(ss.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(cells.SetEquals(new HashSet<string> { "A1", "A2", "A3", "A4", "A5"}));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMethod6()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod7()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("1a", "Hello World");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod8()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("1a", 0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod9()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("1a", new Formula());
        }

        [TestMethod]
        public void TestMethod10()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", new Formula("a2"));
            ss.SetCellContents("a2", new Formula("a3"));
            var cells = new HashSet<string>(ss.SetCellContents("a3", 0d));

            Assert.IsTrue(cells.SetEquals(new HashSet<string> { "A1", "A2", "A3" }));
        }
    }
}

// Name: Eric Miramontes
// Uid: u0801584

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using SS;

namespace ControllerTester
{
    [TestClass]
    public class ControllerTests
    {
        /// <summary>
        /// Tests the constructor of Controller that takes in an existing spreadsheet.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "=10");

            Controller controller = new Controller(stub, spreadsheet, "test.ss");
            Assert.IsTrue(stub.CalledSetCell);
            Assert.IsFalse(stub.Changed);
            Assert.AreEqual(stub.CellName, "A1");
            Assert.AreEqual(stub.Title, "test.ss");
            Assert.AreEqual(stub.CellContent, "=10");
            Assert.AreEqual(stub.CellValue, "10");
        }

        [TestMethod]
        public void TestClose()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledDoClose);
        }

        [TestMethod]
        public void TestHelp()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireHelpEvent();

            Assert.AreEqual(stub.Message, "Turorial:\n\n"
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
                + "are any unsaved changes, the spreadsheet will prompt you to either save, not save, or cancel close operation.");
        }

        [TestMethod]
        public void TestNew()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireNewEvent();
            Assert.IsTrue(stub.CalledNewEvent);
        }


        // Tests for when user opens a new spreadsheet.

        [TestMethod]
        public void TestOpen1()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireOpenEvent(null);
            Assert.AreEqual(stub.Message.Substring(0, 20), "Unable to open file\n");
        }

        [TestMethod]
        public void TestOpen2()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireOpenEvent("spreadsheet1.ss");
            Assert.AreEqual(stub.Message, null);
        }


        // Tests for when the user saves their spreadsheet.

        [TestMethod]
        public void TestSave1()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireSaveEvent(null);
            Assert.AreEqual(stub.Message.Substring(0, 20), "Unable to save file\n");
        }

        [TestMethod]
        public void TestSave2()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireSaveEvent("spreadsheet2.ss");
            Assert.IsFalse(stub.Changed);
            Assert.AreEqual(stub.Message, null);
            Assert.AreEqual(stub.Title, "spreadsheet2.ss");
        }


        // Tests for when a user updates their selected cell.

        [TestMethod]
        public void TestUpdate1()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.CellName = "A1";
            stub.FireUpdateEvent("10");

            Assert.AreEqual(stub.Row, 0);
            Assert.AreEqual(stub.Col, 0);
            Assert.AreEqual(stub.CellValue, "10");
            Assert.AreEqual(stub.CellContent, "10");
            Assert.IsTrue(stub.Changed);
        }

        [TestMethod]
        public void TestUpdate2()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.CellName = "A1";
            stub.FireUpdateEvent("10");
            stub.FireUpdateEvent("=a1");

            Assert.AreEqual(stub.Message.Substring(0, 8), "Error: \n");
            Assert.AreEqual(stub.CellContent, "10");
        }

        /// <summary>
        /// Test for method that updates a cell.
        /// </summary>
        [TestMethod]
        public void TestSetCell()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.SetCell(3, 4, "10");

            Assert.IsTrue(stub.CalledSetCell);
            Assert.AreEqual(stub.Col, 3);
            Assert.AreEqual(stub.Row, 4);
            Assert.AreEqual(stub.CellContent, "10");
        }

        /// <summary>
        /// Test for when a user selects a new cell.  Handler takes in a SpreadsheetPanel.
        /// Default selected cell should be "A1".
        /// </summary>
        [TestMethod]
        public void TestSelectionChanged()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);

            stub.FireSelectionChangedEvent();

            Assert.AreEqual(stub.CellName, "A1");
            Assert.AreEqual(stub.CellValue, string.Empty);
            Assert.AreEqual(stub.CellContent, string.Empty);
        }
    }
}

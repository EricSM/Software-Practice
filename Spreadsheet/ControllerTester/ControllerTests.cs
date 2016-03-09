using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;

namespace ControllerTester
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.DoClose();
            Assert.IsTrue(stub.CalledDoClose);
        }
    }
}

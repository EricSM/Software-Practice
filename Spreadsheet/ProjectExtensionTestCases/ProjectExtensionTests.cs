// Name: Eric Miramontes
// uNID: u0801584

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using Dependencies;
using System.Collections.Generic;

namespace ProjectExtensionTestCases
{
    [TestClass]
    public class ProjectExtensionTests
    {
        [TestMethod]
        public void TestFormulaEmptyConstructor()
        {
            Formula formula = new Formula();
            Assert.AreEqual(formula.ToString(), "0");
            Assert.AreEqual(formula.Evaluate(s => 10), 0);
        }

        [TestMethod]
        public void TestDGConstructor2()
        {
            DependencyGraph dg1 = new DependencyGraph();
            dg1.AddDependency("s1", "t1");
            dg1.AddDependency("s2", "t2");

            DependencyGraph dg2 = new DependencyGraph(dg1);
            dg2.RemoveDependency("s2", "t2");

            Assert.IsTrue(dg1.HasDependents("s1") && dg1.HasDependents("s2") && dg1.Size == 2);
            Assert.IsTrue(dg2.HasDependents("s1") && dg2.Size == 1);
            Assert.IsFalse(dg2.HasDependents("s2"));

            dg2.ReplaceDependents("s3", new HashSet<string>() { "t3", "t4"});

            Assert.IsTrue(dg2.HasDependents("s3") && dg2.Size == 3);
            Assert.IsFalse(dg1.HasDependents("s3"));
        }
    }
}

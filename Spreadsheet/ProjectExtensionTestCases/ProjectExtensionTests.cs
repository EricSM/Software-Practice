// Name: Eric Miramontes
// uNID: u0801584

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using Dependencies;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public void StressTest1()
        {
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToLower(), s => Regex.IsMatch(s, @"^[a-z]$"));
            Assert.AreEqual(f.Evaluate(Lookup4), 2.3719e+90, 1e+86);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StressTest2()
        {
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToUpper(), s => Regex.IsMatch(s, @"^[a-z]$"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StressTest3()
        {
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => "_" + s, s => Regex.IsMatch(s, @"^[a-z]$"));
        }

        [TestMethod]
        public void TestGetVariables()
        {
            Formula f1 = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToUpper(), s => Regex.IsMatch(s, @"^[A-Z]$"));

            var variables = new HashSet<string>(f1.GetVariables());
            Assert.IsTrue(variables.SetEquals(new HashSet<string> { "A", "B", "C", "D", "E"}));
        }

        [TestMethod]
        public void TestToString()
        {
            Formula f1 = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToUpper(), s => Regex.IsMatch(s, @"^[A-Z]$"));
            Assert.AreEqual("(((((2+3*A)/(7e-5+B-C))*D+.0005e+92)-8.2)*3.14159)*((E+3.1)-.00000000008)", f1.ToString());
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

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(string v)
        {
            switch (v)
            {
                case "a": return 4.0;
                case "b": return 6.0;
                case "c": return 8.0;
                case "d": return 10.0;
                case "e": return 12.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}

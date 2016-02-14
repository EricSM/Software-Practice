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
    /// <summary>
    /// UnitTests for extended functionaility for both class libraries DependencyGraph and Formula.
    /// </summary>
    [TestClass]
    public class ProjectExtensionTests
    {
        /// <summary>
        /// Tests the default constructor for the formula struct.
        /// </summary>
        [TestMethod]
        public void TestFormulaEmptyConstructor()
        {
            Formula formula = new Formula();
            Assert.AreEqual(formula.ToString(), "0");
            Assert.AreEqual(formula.Evaluate(s => 10), 0);
        }

        /// <summary>
        /// Tests the three parameter constructor with a validator that only accepts single lower-case
        /// letters as variable names.
        /// </summary>
        [TestMethod]
        public void StressTest1()
        {
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToLower(), s => Regex.IsMatch(s, @"^[a-z]$"));
            Assert.AreEqual(f.Evaluate(Lookup4), 2.3719e+90, 1e+86);
        }

        /// <summary>
        /// Three parameter constructor throws exception when validator only accepts lower-case letters
        /// but the normalizer converts all variable names to upper-case.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StressTest2()
        {
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToUpper(), s => Regex.IsMatch(s, @"^[a-z]$"));
        }

        /// <summary>
        /// Three parameter constructor throws exception when normalizer converts variable names to ones that are
        /// not allowed by either the provided validator or the default restrictions built into Formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StressTest3()
        {
            // Adds an underscore to variable name, which is not allowed.
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => "_" + s, s => Regex.IsMatch(s, @"^[a-z]$"));
        }

        /// <summary>
        /// Tests the GetVariables method of class library Formula.
        /// </summary>
        [TestMethod]
        public void TestGetVariables()
        {
            Formula f1 = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToUpper(), s => Regex.IsMatch(s, @"^[A-Z]$")); // Normalized to upper-case.

            var variables = new HashSet<string>(f1.GetVariables()); // Retrieve the normalized variables
            Assert.IsTrue(variables.SetEquals(new HashSet<string> { "A", "B", "C", "D", "E"}));
        }

        /// <summary>
        /// Tests ToString method of formula.
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            Formula f1 = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                s => s.ToUpper(), s => Regex.IsMatch(s, @"^[A-Z]$"));
            
            // Should return string with no spaces and all variables normalized.
            Assert.AreEqual("(((((2+3*A)/(7e-5+B-C))*D+.0005e+92)-8.2)*3.14159)*((E+3.1)-.00000000008)", f1.ToString());
        }

        /// <summary>
        /// Tests single parameter constructor for class library DependencyGraph.
        /// </summary>
        [TestMethod]
        public void TestDGConstructor2()
        {
            // Create dependency graph with two dependencies.
            DependencyGraph dg1 = new DependencyGraph();
            dg1.AddDependency("s1", "t1");
            dg1.AddDependency("s2", "t2");

            // Create second dependency graph using the first, then remove one dependency from new one.
            DependencyGraph dg2 = new DependencyGraph(dg1);
            dg2.RemoveDependency("s2", "t2");

            Assert.IsTrue(dg1.HasDependents("s1") && dg1.HasDependents("s2") && dg1.Size == 2); // Initial dg should be unaffected.
            Assert.IsTrue(dg2.HasDependents("s1") && dg2.Size == 1); // New dg should have only one dependency.
            Assert.IsFalse(dg2.HasDependents("s2")); // It should not have the one that was removed.

            // Replace dependencies of s3 in new dg.
            dg2.ReplaceDependents("s3", new HashSet<string>() { "t3", "t4"});

            Assert.IsTrue(dg2.HasDependents("s3") && dg2.Size == 3); // New dg now has 3 dependencies.
            Assert.IsFalse(dg1.HasDependents("s3")); // s3 shouldn't exist in original dg.
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

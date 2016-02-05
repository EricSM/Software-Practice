// Name: Eric Miramontes
// uNID: u0801584


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    /// <summary>
    /// UnitTests for class library DependencyGraph.
    /// </summary>
    [TestClass]
    public class DependencyGraphUnitTest
    {
        // Tests the constructor

        /// <summary>
        /// The empty constructor's size should be zero.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            DependencyGraph dg = new DependencyGraph();
            Assert.AreEqual(0, dg.Size);
        }

        // Tests for the method AddDependency.

        /// <summary>
        /// Tries to add the same dependency twice.  Size should only be 1.  Then tries to add another dependency,
        /// at which point the size should be 2.
        /// </summary>
        [TestMethod]
        public void TestAddDependency1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            dg.AddDependency("s", "t");
            Assert.AreEqual(dg.Size, 1);

            dg.AddDependency("s", "t1");
            Assert.AreEqual(dg.Size, 2);
        }

        /// <summary>
        /// Test for exception when null is passed in as a parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddDependency2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, "t");
        }

        // Tests for the method HasDependents.

        /// <summary>
        /// Test to make sure s has a dependent and t does not if (s, t) are passed in.
        /// </summary>
        [TestMethod]
        public void TestHasDependents1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            Assert.IsTrue(dg.HasDependents("s"));
            Assert.IsFalse(dg.HasDependents("t"));
        }

        /// <summary>
        /// Tests for exception when null is passed in as a parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHasDependents2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependents(null);
        }

        // Tests for the method HasDependees.
        /// <summary>
        /// Test to make sure t has a dependee and s does not if (s, t) are passed in.
        /// </summary>
        [TestMethod]
        public void TestHasDependees1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            Assert.IsFalse(dg.HasDependees("s"));
            Assert.IsTrue(dg.HasDependees("t"));
        }

        /// <summary>
        /// Test for exception when null is passed in as a parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHasDependees2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependees(null);
        }

        // Tests for the method GetDependents.

        /// <summary>
        /// s1 is depended on by t1, t2, and t3.  Test tries to retrieve those dependents.
        /// </summary>
        [TestMethod]
        public void TestGetDependents1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s1", "t1");
            dg.AddDependency("s1", "t2");
            dg.AddDependency("s1", "t3");

            var dependents = new HashSet<string>(dg.GetDependents("s1"));

            Assert.IsTrue(dependents.Count == 3 && dependents.Contains("t1") && dependents.Contains("t2") &&
                dependents.Contains("t3"));
        }

        /// <summary>
        /// Get IEnumerable of the dependents of a nonexistent node.  IEnumerable should be empty.
        /// </summary>
        [TestMethod]
        public void TestGetDependents2()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependents = new HashSet<string>(dg.GetDependents("s1"));

            Assert.AreEqual(dependents.Count, 0);
        }

        /// <summary>
        /// Test for exception when null is passed in as a parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependents3()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependents = new HashSet<string>(dg.GetDependents(null));
        }

        // Tests for the method GetDependees.

        /// <summary>
        /// t1 depends on s1, s2, and s3.  Test tries to retrieve those dependees.
        /// </summary>
        [TestMethod]
        public void TestGetDependees1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s1", "t1");
            dg.AddDependency("s2", "t1");
            dg.AddDependency("s3", "t1");


            var dependees = new HashSet<string>(dg.GetDependees("t1"));

            Assert.IsTrue(dependees.Count == 3 && dependees.Contains("s1") && dependees.Contains("s2") &&
                dependees.Contains("s3"));

        }

        /// <summary>
        /// Get IEnumerable of the dependees of a nonexistent node.  IEnumerable should be empty.
        /// </summary>
        [TestMethod]
        public void TestGetDependees2()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependees = new HashSet<string>(dg.GetDependees("t1"));

            Assert.AreEqual(dependees.Count, 0);
        }

        /// <summary>
        /// Test for exception when null is passed in as a parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependees3()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependees = new HashSet<string>(dg.GetDependees(null));
        }

        // Tests for the method RemoveDependency.

        /// <summary>
        /// Add dependency (s, t) then try to remove it.
        /// </summary>
        [TestMethod]
        public void TestRemoveDependency1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");

            // Try to remove a nonexistent dependency
            dg.RemoveDependency("s", "t1");
            Assert.IsTrue(dg.Size == 1); // size should remain 1

            dg.RemoveDependency("s", "t");
            Assert.IsTrue(dg.Size == 0);
        }

        /// <summary>
        /// Test for exception when null is passed in as a parameter.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveDependency2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency("s", null);
        }

        // Test for the method ReplaceDependents.
        /// <summary>
        /// t1, t2, and t3 depend on s1.  Replaces those dependents with t4, t5, and t6.
        /// Uses method GetDependents to make sure replacement was successful.
        /// </summary>
        [TestMethod]
        public void TestReplaceDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s1", "t1");
            dg.AddDependency("s1", "t2");
            dg.AddDependency("s1", "t3");

            dg.ReplaceDependents("s1", new HashSet<string> { "t4", "t5", "t6" });

            var dependents = new HashSet<string>(dg.GetDependents("s1"));

            Assert.IsTrue(dependents.SetEquals(new HashSet<string> { "t4", "t5", "t6" }));
        }

        // Test for the method ReplaceDependees.
        /// <summary>
        /// t1 is dependent on s1, s2, and s3.  Replaces those dependees with s4, s5, and s6.
        /// Uses method GetDependees to make sure replacement was successful.
        /// </summary>
        [TestMethod]
        public void TestReplaceDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s1", "t1");
            dg.AddDependency("s2", "t1");
            dg.AddDependency("s3", "t1");

            dg.ReplaceDependees("t1", new HashSet<string> { "s4", "s5", "s6" });

            var dependees = new HashSet<string>(dg.GetDependees("t1"));

            Assert.IsTrue(dependees.SetEquals(new HashSet<string> { "s4", "s5", "s6" }));
        }
    }
}

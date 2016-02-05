using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTests
{
    [TestClass]
    public class DependencyGraphTests
    {
        [TestMethod]
        public void TestConstructor1()
        {
            DependencyGraph dg = new DependencyGraph();
            Assert.AreEqual(0, dg.Size);
        }

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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddDependency2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, "t");
        }

        [TestMethod]
        public void TestHasDependents1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            Assert.IsTrue(dg.HasDependents("s"));
            Assert.IsFalse(dg.HasDependents("t"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHasDependents2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependents(null);
        }

        [TestMethod]
        public void TestHasDependees1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            Assert.IsFalse(dg.HasDependees("s"));
            Assert.IsTrue(dg.HasDependees("t"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHasDependees2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependees(null);
        }

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

        [TestMethod]
        public void TestGetDependents2()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependents = new HashSet<string>(dg.GetDependents("s1"));

            Assert.AreEqual(dependents.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependents3()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependents = new HashSet<string>(dg.GetDependents(null));
        }

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

        [TestMethod]
        public void TestGetDependees2()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependees = new HashSet<string>(dg.GetDependees("t1"));
            
            Assert.AreEqual(dependees.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependees3()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependees = new HashSet<string>(dg.GetDependees(null));
        }
        
        [TestMethod]
        public void TestRemoveDependency1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            dg.RemoveDependency("s", "t1");
            Assert.IsTrue(dg.Size == 1);

            dg.RemoveDependency("s", "t");
            Assert.IsTrue(dg.Size == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveDependency2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency("s", null);
        }

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

        [TestMethod]
        public void TestReplaceDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s1", "t1");
            dg.AddDependency("s2", "t1");
            dg.AddDependency("s3", "t1");

            dg.ReplaceDependees("t1", new HashSet<string> { "s4", "s5", "s6"});

            var dependees = new HashSet<string>(dg.GetDependees("t1"));

            Assert.IsTrue(dependees.SetEquals(new HashSet<string> { "s4", "s5", "s6" }));
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

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
        public void TestHasDependents1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s", "t");
            Assert.AreEqual(dg.HasDependents("s"), true);
            Assert.AreEqual(dg.HasDependents("t"), false);
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
            Assert.AreEqual(dg.HasDependees("s"), false);
            Assert.AreEqual(dg.HasDependees("t"), true);
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

            var dependents = dg.GetDependents("s1");

            var i = 0;
            foreach (string dependent in dependents)
            {
                i++;
                Assert.AreEqual(dependent, "t" + i);
            }

        }

        [TestMethod]
        public void TestGetDependents2()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependents = dg.GetDependents("s1");

            var i = 0;
            foreach (string dependent in dependents)
            {
                i++;
            }

            Assert.AreEqual(i, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependents3()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependents = dg.GetDependents(null);

            foreach (string dependent in dependents) { }
        }

        [TestMethod]
        public void TestGetDependees1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("s1", "t1");
            dg.AddDependency("s2", "t1");
            dg.AddDependency("s3", "t1");

            var dependents = dg.GetDependees("t1");

            var i = 0;
            foreach (string dependent in dependents)
            {
                i++;
                Assert.AreEqual(dependent, "s" + i);
            }

        }

        [TestMethod]
        public void TestGetDependees2()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependees = dg.GetDependees("t1");

            var i = 0;
            foreach (string dependee in dependees)
            {
                i++;
            }

            Assert.AreEqual(i, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependees3()
        {
            DependencyGraph dg = new DependencyGraph();
            var dependees = dg.GetDependees(null);

            foreach (string dependee in dependees) { }
        }

        [TestMethod]
        public void UnitTest4()
        {
            throw new Exception();
        }

        [TestMethod]
        public void UnitTest5()
        {
            throw new Exception();
        }
    }
}

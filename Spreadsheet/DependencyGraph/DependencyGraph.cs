﻿// Skeleton implementation written by Joe Zachary for CS 3500, January 2015.
// Revised for CS 3500 by Joe Zachary, January 29, 2016

using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {

        private Dictionary<string, HashSet<string>> _dependeesByDependents;
        private Dictionary<string, HashSet<string>> _dependentsByDependees;
        private int _size;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            _dependeesByDependents = new Dictionary<string, HashSet<string>>();
            _dependentsByDependees = new Dictionary<string, HashSet<string>>();
            _size = 0;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            if (_dependentsByDependees.TryGetValue(s, out dependents))
            {
                return dependents.Count > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {

            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependees;
            if (_dependeesByDependents.TryGetValue(s, out dependees))
            {
                return dependees.Count > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            if (_dependentsByDependees.TryGetValue(s, out dependents))
            {
                foreach (string dependent in dependents)
                {
                    yield return dependent;
                }
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependees;
            if (_dependeesByDependents.TryGetValue(s, out dependees))
            {
                foreach (string dependee in dependees)
                {
                    yield return dependee;
                }
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            if (_dependentsByDependees.TryGetValue(s, out dependents))
            {
                if (!dependents.Contains(t))
                {
                    _size++;
                    _dependentsByDependees[s].Add(t);
                }
            }
            else
            {
                _size++;
                _dependentsByDependees.Add(s, new HashSet<string>() { t });
            }


            if (_dependeesByDependents.ContainsKey(t))
            {
                _dependeesByDependents[t].Add(s);
            }
            else
            {
                _dependeesByDependents.Add(t, new HashSet<string>() { s });
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            if (_dependentsByDependees.TryGetValue(s, out dependents) && dependents.Contains(t))
            {
                _dependentsByDependees[s].Remove(t);
                _dependeesByDependents[t].Remove(s);
                _size--;
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            var oldDependents = GetDependents(s);

            foreach (string r in oldDependents)
            {
                RemoveDependency(s, r);
            }

            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            var oldDependees = GetDependees(t);
            
            foreach (string r in oldDependees)
            {
                RemoveDependency(r, t);
            }

            foreach (string s in newDependees)
            {
                AddDependency(s, t);
            }
        }
    }
}

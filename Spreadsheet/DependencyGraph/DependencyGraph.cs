// Skeleton implementation written by Joe Zachary for CS 3500, January 2015.
// Revised for CS 3500 by Joe Zachary, January 29, 2016

// Name: Eric Miramontes
// uNID: u0801584

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
        /// <summary>
        /// Dictionary where the keys are dependees and the values are the hashsets of each of
        /// the dependee's dependents.  Each dependee has its own set of dependents.
        /// </summary>
        private Dictionary<string, HashSet<string>> _dependents;

        /// <summary>
        /// Dictionary where the keys are dependents and the values are the hashsets of each of
        /// the dependent's dependees.  Each dependent has its own set of dependees.
        /// </summary>
        private Dictionary<string, HashSet<string>> _dependees;

        /// <summary>
        /// Number of sets of dependencies (s, t)
        /// </summary>
        private int _size;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            _dependents = new Dictionary<string, HashSet<string>>();
            _dependees = new Dictionary<string, HashSet<string>>();
            _size = 0;
        }

        /// <summary>
        /// Creates a DependencyGraph using an existing depencency graph.
        /// </summary>
        /// <param name="dependencyGraph"></param>
        public DependencyGraph(DependencyGraph dependencyGraph)
        {
            _dependents = dependencyGraph._dependents;
            _dependees = dependencyGraph._dependees;
            _size = dependencyGraph.Size;
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
            // Check if s is null.
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            // Try to retrieve the dependents of s and assign it to hashset "dependents".
            if (_dependents.TryGetValue(s, out dependents))
            {
                // Check if "dependents" has at least one element.
                return dependents.Count > 0;
            }
            // If retrieval attempt fails, return false.
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
            // Check if s is null
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependees;
            // Try to retrieve dependees of s and assign it to hashset "dependees".
            if (_dependees.TryGetValue(s, out dependees))
            {
                // Check if "dependees" has at least one element
                return dependees.Count > 0;
            }
            // If retrieval attempt fails, return false.
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
            // Check if s is null.
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            // Try to retrieve the dependents of s and assign it to hashset "dependents".
            if (_dependents.TryGetValue(s, out dependents))
            {
                // Iterate through "dependents" and return string "dependent".
                foreach (string dependent in dependents)
                {
                    yield return dependent;
                }
            }
            // If retrieval attempt fails, return false.
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
            // Check if s is null.
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependees;
            // Try to retrieve dependees of s and assign it to hashset "dependees".
            if (_dependees.TryGetValue(s, out dependees))
            {
                // Iterate through "dependees" and return string "dependee".
                foreach (string dependee in dependees)
                {
                    yield return dependee;
                }
            }
            // If retrieval attempt fails, return false.
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
            // Check if either s or t are null.
            if (s == null || t == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            // Try to retrieve the dependents of s and assign it to hashset "dependents".
            if (_dependents.TryGetValue(s, out dependents))
            {
                // Check if t is not already a dependent of s
                if (!dependents.Contains(t))
                {
                    _size++; // Increment size
                    _dependents[s].Add(t); // Add t as a dependent of s
                }
            }
            // If s has no dependents
            else
            {
                _size++; // Increment size

                // Add new entry in dictionary of dependents (Values) listed by dependees (Keys) 
                // with dependee s as the key and a new hashset of dependents containing t as the value.
                _dependents.Add(s, new HashSet<string>() { t });
            }

            // Check if dependent t has entry with its own list of dependees
            if (_dependees.ContainsKey(t))
            {
                // Add dependee s to dependent t's list of dependees
                _dependees[t].Add(s);
            }
            // If dependent t has no entries in dictionary of dependees.
            else
            {
                // Add new entry in dictionary of dependees (Values) listed by dependents (Keys) 
                // with dependent t as the key and a new hashset of dependees containing s as the value.
                _dependees.Add(t, new HashSet<string>() { s });
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            // Check if either s or t are null.
            if (s == null || t == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> dependents;
            // Try to retrieve the dependents of s, if it exists, and assign it to hashset "dependents".
            // Then check if dependents contains dependent (t).
            if (_dependents.TryGetValue(s, out dependents) && dependents.Contains(t))
            {
                _dependents[s].Remove(t); // Remove dependent t from dependee s's list of dependents.
                _dependees[t].Remove(s); // Remove dependee s from dependent t's list of depenees.
                _size--; // Decrement size.
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if ((newDependents as HashSet<string>).Contains(null))
            {
                throw new ArgumentNullException();
            }

            // Retrieve dependents of dependee s and put it in a hashset.
            var oldDependents = new HashSet<string>(GetDependents(s));

            // Iterate through the oldDependents.
            foreach (string r in oldDependents)
            {
                // Remove dependency between dependee s and the old dependent r.
                RemoveDependency(s, r);
            }

            // Iterate through new dependents.
            foreach (string t in newDependents)
            {
                // Add new dependency between dependee s and new dependent t.
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
            if ((newDependees as HashSet<string>).Contains(null))
            {
                throw new ArgumentNullException();
            }

            // Retrieve dependees of dependents t and put it in a hashset.
            var oldDependees = new HashSet<string>(GetDependees(t));

            // Iterate through the oldDependees.
            foreach (string r in oldDependees)
            {
                // Remove dependency between old dependee r and the dependent t.
                RemoveDependency(r, t);
            }

            // Iterate through new dependees.
            foreach (string s in newDependees)
            {
                // Add new dependency between dependee s and new dependent t.
                AddDependency(s, t);
            }
        }
    }
}

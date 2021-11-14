namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class CombinatoricsTest
    {
        [TestMethod]
        public void combinations()
        {
            // Let's say we would like to compute all the
            // possible combinations of elements (1,2,3,4):
            //
            int[] elements = new[] { 1, 2, 3, 4 };

            // The number of possible subsets might be too large
            // to fit on memory. For this reason, we can compute
            // values on-the-fly using foreach:

            foreach (int[] combination in Combinatorics.Combinations(elements))
            {
                // ...
            }

            // Or we could try to compute them all and store in an array:
            int[][] combinations = Combinatorics.Combinations(elements).ToArray();

            // In either case, the result will be:

            int[][] expected =
            {
               new [] { 1 },
               new [] { 2 },
               new [] { 3 },
               new [] { 4 },
               new [] { 1, 2 },
               new [] { 1, 3 },
               new [] { 2, 3 },
               new [] { 1, 4 },
               new [] { 2, 4 },
               new [] { 3, 4 },
               new [] { 1, 2, 3 },
               new [] { 1, 2, 4 },
               new [] { 1, 3, 4 },
               new [] { 2, 3, 4 },
               new [] { 1, 2, 3, 4 }
            };

            // Note: although the empty set is technically a subset
            // of all sets, it won't be included in the enumeration

            string str = combinations.Apply(x => x.ToArray()).ToCSharp();

            for (int i = 0; i < combinations.Length; i++)
                CollectionAssert.AreEqual(combinations[i], expected[i]);
        }

        [TestMethod]
        public void combinations_of_size_k()
        {
            // Let's say we would like to compute all the
            // combinations of size 2 the elements (1,2,3):
            //
            int[] elements = new[] { 1, 2, 3 };

            // The number of possible subsets might be too large
            // to fit on memory. For this reason, we can compute
            // values on-the-fly using foreach:

            foreach (int[] combination in Combinatorics.Combinations(elements, k: 2))
            {
                // ...
            }

            // Or we could try to compute them all and store in an array:
            int[][] combinations = Combinatorics.Combinations(elements, k: 2).ToArray();

            // In either case, the result will be:

            int[][] expected =
            {
               new [] { 1, 2 },
               new [] { 1, 3 },
               new [] { 2, 3 },
            };

            string str = combinations.Apply(x => x.ToArray()).ToCSharp();

            for (int i = 0; i < combinations.Length; i++)
            {
                CollectionAssert.AreEqual(combinations[i], expected[i]);
            }
        }

        [TestMethod]
        public void PermutationsTest()
        {
            // Let's say we would like to generate all possible permutations
            // of the elements (1, 2, 3). In order to enumerate all those
            // permutations, we can use:

            int[] values = { 1, 2, 3 };

            foreach (int[] permutation in Combinatorics.Permutations(values))
            {
                // The permutations will be generated in the following order:
                //
                //   { 1, 2, 3 }
                //   { 1, 3, 2 };
                //   { 2, 1, 3 };
                //   { 2, 3, 1 };
                //   { 3, 1, 2 };
                //   { 3, 2, 1 };
                //
            }

            List<int[]> permutations = new List<int[]>();
            foreach (var p in Combinatorics.Permutations(values))
                permutations.Add(p);

            Assert.AreEqual(6, permutations.Count);
            Assert.IsTrue(permutations[0].IsEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(permutations[1].IsEqual(new[] { 1, 3, 2 }));
            Assert.IsTrue(permutations[2].IsEqual(new[] { 2, 1, 3 }));
            Assert.IsTrue(permutations[3].IsEqual(new[] { 2, 3, 1 }));
            Assert.IsTrue(permutations[4].IsEqual(new[] { 3, 1, 2 }));
            Assert.IsTrue(permutations[5].IsEqual(new[] { 3, 2, 1 }));
        }

        [TestMethod]
        public void SequencesTest()
        {
            int[] symbols = { 2, 3, 2 };

            int[][] expected =
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 1 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 1 },
                new int[] { 0, 2, 0 },
                new int[] { 0, 2, 1 },
                new int[] { 1, 0, 0 },
                new int[] { 1, 0, 1 },
                new int[] { 1, 1, 0 },
                new int[] { 1, 1, 1 },
                new int[] { 1, 2, 0 },
                new int[] { 1, 2, 1 },
            };

            int[][] actual = Combinatorics.Sequences(symbols).Select(x => (int[])x.Clone()).ToArray();

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void SequencesTest2()
        {
            int[][] expected =
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 1 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 1 },
                new int[] { 1, 0, 0 },
                new int[] { 1, 0, 1 },
                new int[] { 1, 1, 0 },
                new int[] { 1, 1, 1 },
            };

            int i = 0;
            foreach (int[] row in Combinatorics.Sequences(2, 3))
            {
                Assert.IsTrue(row.IsEqual(expected[i++]));
            }
        }

        [TestMethod]
        public void subsets()
        {
            // Let's say we would like to compute all the
            // possible subsets of the set { 1, 2, 3, 4 }:
            //
            ISet<int> set = new HashSet<int> { 1, 2, 3, 4 };

            // The number of possible subsets might be too large
            // to fit on memory. For this reason, we can compute
            // values on-the-fly using foreach:

            foreach (SortedSet<int> subset in Combinatorics.Subsets(set))
            {
                // ...
            }

            // Or we could try to compute them all and store in an array:
            SortedSet<int>[] subsets = Combinatorics.Subsets(set).ToArray();

            // In either case, the result will be:

            int[][] expected =
            {
               new [] { 1 },
               new [] { 2 },
               new [] { 3 },
               new [] { 4 },
               new [] { 1, 2 },
               new [] { 1, 3 },
               new [] { 2, 3 },
               new [] { 1, 4 },
               new [] { 2, 4 },
               new [] { 3, 4 },
               new [] { 1, 2, 3 },
               new [] { 1, 2, 4 },
               new [] { 1, 3, 4 },
               new [] { 2, 3, 4 },
               new [] { 1, 2, 3, 4 }
            };

            // Note: although the empty set is technically a subset
            // of all sets, it won't be included in the enumeration

            string str = subsets.Apply(x => x.ToArray()).ToCSharp();

            for (int i = 0; i < subsets.Length; i++)
            {
                int[] actual = subsets[i].ToArray();
                CollectionAssert.AreEqual(actual, expected[i]);
            }
        }

        [TestMethod]
        public void subsets_of_size_k()
        {
            // Let's say we would like to compute all the
            // subsets of size 2 of the set { 1, 2, 3, 4 }:
            //
            ISet<int> set = new HashSet<int> { 1, 2, 3, 4 };

            // The number of possible subsets might be too large
            // to fit on memory. For this reason, we can compute
            // values on-the-fly using foreach:

            foreach (SortedSet<int> subset in Combinatorics.Subsets(set, k: 2))
            {
                // ...
            }

            // Or we could try to compute them all and store in an array:
            SortedSet<int>[] subsets = Combinatorics.Subsets(set, k: 2).ToArray();

            // In either case, the result will be:

            int[][] expected =
            {
               new [] { 1, 2 },
               new [] { 1, 3 },
               new [] { 2, 3 },
               new [] { 1, 4 },
               new [] { 2, 4 },
               new [] { 3, 4 },
            };

            string str = subsets.Apply(x => x.ToArray()).ToCSharp();

            for (int i = 0; i < subsets.Length; i++)
            {
                int[] actual = subsets[i].ToArray();
                CollectionAssert.AreEqual(actual, expected[i]);
            }
        }

        [TestMethod]
        public void TruthTableTest()
        {
            {
                // Suppose we would like to generate a truth table for a binary
                // problem. In this case, we are only interested in two symbols:
                // 0 and 1. Let's then generate the table for three binary values

                int symbols = 2; // Binary variables: either 0 or 1
                int length = 3;  // The number of variables; or number
                                 // of columns in the generated table.

                // Generate the table using Combinatorics.TruthTable(2,3)
                int[][] table = Combinatorics.TruthTable(symbols, length);

                // The generated table will be

                int[][] expected =
                {
                    new int[] { 0, 0, 0 },
                    new int[] { 0, 0, 1 },
                    new int[] { 0, 1, 0 },
                    new int[] { 0, 1, 1 },
                    new int[] { 1, 0, 0 },
                    new int[] { 1, 0, 1 },
                    new int[] { 1, 1, 0 },
                    new int[] { 1, 1, 1 },
                };

                Assert.IsTrue(expected.IsEqual(table));
            }

            {
                int symbols = 3;
                int length = 3;

                int[][] expected =
                {
                    new int[] { 0, 0, 0 },
                    new int[] { 0, 0, 1 },
                    new int[] { 0, 0, 2 },
                    new int[] { 0, 1, 0 },
                    new int[] { 0, 1, 1 },
                    new int[] { 0, 1, 2 },
                    new int[] { 0, 2, 0 },
                    new int[] { 0, 2, 1 },
                    new int[] { 0, 2, 2 },
                    new int[] { 1, 0, 0 },
                    new int[] { 1, 0, 1 },
                    new int[] { 1, 0, 2 },
                    new int[] { 1, 1, 0 },
                    new int[] { 1, 1, 1 },
                    new int[] { 1, 1, 2 },
                    new int[] { 1, 2, 0 },
                    new int[] { 1, 2, 1 },
                    new int[] { 1, 2, 2 },
                    new int[] { 2, 0, 0 },
                    new int[] { 2, 0, 1 },
                    new int[] { 2, 0, 2 },
                    new int[] { 2, 1, 0 },
                    new int[] { 2, 1, 1 },
                    new int[] { 2, 1, 2 },
                    new int[] { 2, 2, 0 },
                    new int[] { 2, 2, 1 },
                    new int[] { 2, 2, 2 },
                };

                int[][] actual = Combinatorics.TruthTable(symbols, length);

                Assert.IsTrue(expected.IsEqual(actual));
            }
        }

        [TestMethod]
        public void TruthTableTest2()
        {
            // Suppose we would like to generate a truth table (i.e. all possible
            // combinations of a set of discrete symbols) for variables that contain
            // different numbers symbols. Let's say, for example, that the first
            // variable may contain symbols 0 and 1, the second could contain either
            // 0, 1, or 2, and the last one again could contain only 0 and 1. Thus
            // we can generate the truth table in the following way:

            int[] symbols = { 2, 3, 2 };

            int[][] actual = Combinatorics.TruthTable(symbols);

            int[][] expected =
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 1 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 1 },
                new int[] { 0, 2, 0 },
                new int[] { 0, 2, 1 },
                new int[] { 1, 0, 0 },
                new int[] { 1, 0, 1 },
                new int[] { 1, 1, 0 },
                new int[] { 1, 1, 1 },
                new int[] { 1, 2, 0 },
                new int[] { 1, 2, 1 },
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
}
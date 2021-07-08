using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Static class for combinatorics functions.
    /// </summary>
    public static class Combinatorics
    {
        /// <summary>
        ///     Generates all possible two symbol ordered
        ///     permutations with repetitions allowed (a truth table).
        /// </summary>
        /// <param name="length">The length of the sequence to generate.</param>
        /// <example>
        ///     <para>
        ///         Suppose we would like to generate a truth table for a binary
        ///         problem. In this case, we are only interested in two symbols:
        ///         0 and 1. Let's then generate the table for three binary values
        ///     </para>
        ///     <code>
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// // Generate the table using Combinatorics.TruthTable(3)
        /// int[][] table = Combinatorics.TruthTable(length);
        /// 
        /// // The generated table will be:
        /// {
        ///     new int[] { 0, 0, 0 },
        ///     new int[] { 0, 0, 1 },
        ///     new int[] { 0, 1, 0 },
        ///     new int[] { 0, 1, 1 },
        ///     new int[] { 1, 0, 0 },
        ///     new int[] { 1, 0, 1 },
        ///     new int[] { 1, 1, 0 },
        ///     new int[] { 1, 1, 1 },
        /// };
        /// </code>
        /// </example>
        public static int[][] TruthTable(int length)
        {
            return TruthTable(2, length);
        }

        /// <summary>
        ///     Generates all possible ordered permutations
        ///     with repetitions allowed (a truth table).
        /// </summary>
        /// <param name="symbols">The number of symbols.</param>
        /// <param name="length">The length of the sequence to generate.</param>
        /// <example>
        ///     <para>
        ///         Suppose we would like to generate a truth table for a binary
        ///         problem. In this case, we are only interested in two symbols:
        ///         0 and 1. Let's then generate the table for three binary values
        ///     </para>
        ///     <code>
        /// int symbols = 2; // Binary variables: either 0 or 1
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// // Generate the table using Combinatorics.TruthTable(2,3)
        /// int[][] table = Combinatorics.TruthTable(symbols, length);
        /// 
        /// // The generated table will be:
        /// {
        ///     new int[] { 0, 0, 0 },
        ///     new int[] { 0, 0, 1 },
        ///     new int[] { 0, 1, 0 },
        ///     new int[] { 0, 1, 1 },
        ///     new int[] { 1, 0, 0 },
        ///     new int[] { 1, 0, 1 },
        ///     new int[] { 1, 1, 0 },
        ///     new int[] { 1, 1, 1 },
        /// };
        /// </code>
        /// </example>
        /// <seealso cref="Combinatorics.Sequences(int,int,bool)" />
        public static int[][] TruthTable(int symbols, int length)
        {
            var sym = new int[length];
            for (var i = 0; i < sym.Length; i++)
                sym[i] = symbols;

            return TruthTable(sym);
        }

        /// <summary>
        ///     Generates all possible ordered permutations
        ///     with repetitions allowed (a truth table).
        /// </summary>
        /// <param name="symbols">The number of symbols for each variable.</param>
        /// <example>
        ///     <para>
        ///         Suppose we would like to generate a truth table (i.e. all possible
        ///         combinations of a set of discrete symbols) for variables that contain
        ///         different numbers symbols. Let's say, for example, that the first
        ///         variable may contain symbols 0 and 1, the second could contain either
        ///         0, 1, or 2, and the last one again could contain only 0 and 1. Thus
        ///         we can generate the truth table in the following way:
        ///     </para>
        ///     <code>
        /// // Number of symbols for each variable
        /// int[] symbols = { 2, 3, 2 };
        /// 
        /// // Generate the truth table for the given symbols
        /// int[][] table = Combinatorics.TruthTable(symbols);
        /// 
        /// // The generated table will be:
        /// {
        ///     new int[] { 0, 0, 0 },
        ///     new int[] { 0, 0, 1 },
        ///     new int[] { 0, 1, 0 },
        ///     new int[] { 0, 1, 1 },
        ///     new int[] { 0, 2, 0 },
        ///     new int[] { 0, 2, 1 },
        ///     new int[] { 1, 0, 0 },
        ///     new int[] { 1, 0, 1 },
        ///     new int[] { 1, 1, 0 },
        ///     new int[] { 1, 1, 1 },
        ///     new int[] { 1, 2, 0 },
        ///     new int[] { 1, 2, 1 },
        /// };
        /// </code>
        /// </example>
        /// <seealso cref="Combinatorics.Sequences(int,int,bool)" />
        public static int[][] TruthTable(this int[] symbols)
        {
            var size = 1;
            for (var i = 0; i < symbols.Length; i++)
                size *= symbols[i];

            var sequences = new int[size][];
            sequences[0] = new int[symbols.Length];

            for (var i = 1; i < sequences.Length; i++)
            {
                var row = sequences[i] = (int[])sequences[i - 1].Clone();

                for (var j = symbols.Length - 1; j >= 0; j--)
                {
                    if (row[j] < symbols[j] - 1)
                    {
                        row[j]++;
                        break;
                    }

                    row[j] = 0;
                }
            }

            return sequences;
        }

        /// <summary>
        ///     Provides a way to enumerate all possible ordered permutations
        ///     with repetitions allowed (i.e. a truth table), without using
        ///     many memory allocations.
        /// </summary>
        /// <param name="length">The length of the sequence to generate.</param>
        /// <param name="inPlace">
        ///     If set to true, the different generated sequences will be stored in
        ///     the same array, thus preserving memory. However, this may prevent the
        ///     samples from being stored in other locations without having to clone
        ///     them. If set to false, a new memory block will be allocated for each
        ///     new object in the sequence.
        /// </param>
        /// <example>
        ///     <para>
        ///         Suppose we would like to generate the same sequences shown
        ///         in the <see cref="Combinatorics.TruthTable(int,int)" />example,
        ///         however, without explicitly storing all possible combinations
        ///         in an array. In order to iterate over all possible combinations
        ///         efficiently, we can use:
        ///     </para>
        ///     <code>
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// foreach (int[] row in Combinatorics.Sequences(length))
        /// {
        ///     // The following sequences will be generated in order:
        ///     //
        ///     //   new int[] { 0, 0, 0 },
        ///     //   new int[] { 0, 0, 1 },
        ///     //   new int[] { 0, 1, 0 },
        ///     //   new int[] { 0, 1, 1 },
        ///     //   new int[] { 1, 0, 0 },
        ///     //   new int[] { 1, 0, 1 },
        ///     //   new int[] { 1, 1, 0 },
        ///     //   new int[] { 1, 1, 1 },
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<int[]> Sequences(int length, bool inPlace = false)
        {
            var sym = new int[length];
            for (var i = 0; i < sym.Length; i++)
                sym[i] = 2;

            return Sequences(sym, inPlace);
        }

        /// <summary>
        ///     Provides a way to enumerate all possible ordered permutations
        ///     with repetitions allowed (i.e. a truth table), without using
        ///     many memory allocations.
        /// </summary>
        /// <param name="symbols">The number of symbols.</param>
        /// <param name="length">The length of the sequence to generate.</param>
        /// <param name="inPlace">
        ///     If set to true, the different generated sequences will be stored in
        ///     the same array, thus preserving memory. However, this may prevent the
        ///     samples from being stored in other locations without having to clone
        ///     them. If set to false, a new memory block will be allocated for each
        ///     new object in the sequence.
        /// </param>
        /// <example>
        ///     <para>
        ///         Suppose we would like to generate the same sequences shown
        ///         in the <see cref="Combinatorics.TruthTable(int,int)" />example,
        ///         however, without explicitly storing all possible combinations
        ///         in an array. In order to iterate over all possible combinations
        ///         efficiently, we can use:
        ///     </para>
        ///     <code>
        /// int symbols = 2; // Binary variables: either 0 or 1
        /// int length = 3;  // The number of variables; or number 
        ///                  // of columns in the generated table.
        /// 
        /// foreach (int[] row in Combinatorics.Sequences(symbols, length))
        /// {
        ///     // The following sequences will be generated in order:
        ///     //
        ///     //   new int[] { 0, 0, 0 },
        ///     //   new int[] { 0, 0, 1 },
        ///     //   new int[] { 0, 1, 0 },
        ///     //   new int[] { 0, 1, 1 },
        ///     //   new int[] { 1, 0, 0 },
        ///     //   new int[] { 1, 0, 1 },
        ///     //   new int[] { 1, 1, 0 },
        ///     //   new int[] { 1, 1, 1 },
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<int[]> Sequences(int symbols, int length, bool inPlace = false)
        {
            var sym = new int[length];
            for (var i = 0; i < sym.Length; i++)
                sym[i] = symbols;

            return Sequences(sym, inPlace);
        }

        /// <summary>
        ///     Provides a way to enumerate all possible ordered permutations
        ///     with repetitions allowed (i.e. a truth table), without using
        ///     many memory allocations.
        /// </summary>
        /// <param name="symbols">The number of symbols for each variable.</param>
        /// <param name="inPlace">
        ///     If set to true, the different generated permutations will be stored in
        ///     the same array, thus preserving memory. However, this may prevent the
        ///     samples from being stored in other locations without having to clone
        ///     them. If set to false, a new memory block will be allocated for each
        ///     new object in the sequence.
        /// </param>
        /// <param name="firstColumnChangesFaster">
        ///     If set to true, the first elements in the sequences will change faster
        ///     than last ones. This changes the order in which the sequences are presented,
        ///     but no their content.
        /// </param>
        /// <example>
        ///     <para>
        ///         Suppose we would like to generate the same sequences shown
        ///         in the <see cref="Combinatorics.TruthTable(int,int)" />example,
        ///         however, without explicitly storing all possible combinations
        ///         in an array. In order to iterate over all possible combinations
        ///         efficiently, we can use:
        ///     </para>
        ///     <code>
        /// foreach (int[] row in Combinatorics.Sequences(new[] { 2, 2 }))
        /// {
        ///     // The following sequences will be generated in order:
        ///     //
        ///     //   new int[] { 0, 0, 0 },
        ///     //   new int[] { 0, 0, 1 },
        ///     //   new int[] { 0, 1, 0 },
        ///     //   new int[] { 0, 1, 1 },
        ///     //   new int[] { 1, 0, 0 },
        ///     //   new int[] { 1, 0, 1 },
        ///     //   new int[] { 1, 1, 0 },
        ///     //   new int[] { 1, 1, 1 },
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<int[]> Sequences(this int[] symbols, bool inPlace = false,
            bool firstColumnChangesFaster = false)
        {
            var current = new int[symbols.Length];

            for (var i = 0; i < symbols.Length; i++)
                if (symbols[i] == 0)
                    yield break;

            if (firstColumnChangesFaster)
                while (true)
                {
                    yield return inPlace ? current : (int[])current.Clone();

                    for (var j = 0; j < symbols.Length; j++)
                    {
                        if (current[j] != symbols[j] - 1)
                        {
                            current[j]++;
                            break;
                        }

                        if (j == symbols.Length - 1)
                            yield break;

                        current[j] = 0;
                    }
                }

            while (true)
            {
                yield return inPlace ? current : (int[])current.Clone();

                for (var j = symbols.Length - 1; j >= 0; j--)
                {
                    if (current[j] != symbols[j] - 1)
                    {
                        current[j]++;
                        break;
                    }

                    if (j == 0)
                        yield break;

                    current[j] = 0;
                }
            }
        }

        /// <summary>
        ///     Enumerates all possible value combinations for a given array.
        /// </summary>
        /// <param name="values">The array whose combinations need to be generated.</param>
        /// <param name="inPlace">
        ///     If set to true, the different generated combinations will be stored in
        ///     the same array, thus preserving memory. However, this may prevent the
        ///     samples from being stored in other locations without having to clone
        ///     them. If set to false, a new memory block will be allocated for each
        ///     new object in the sequence.
        /// </param>
        public static IEnumerable<T[]> Combinations<T>(this T[] values, bool inPlace = false)
        {
            // TODO: Test
            for (var i = 0; i < values.Length; i++)
                foreach (var value in Combinations(values, i + 1, inPlace))
                    yield return value;
        }

        /// <summary>
        ///     Enumerates all possible value combinations for a given array.
        /// </summary>
        /// <param name="values">The array whose combinations need to be generated.</param>
        /// <param name="k">The length of the combinations to be generated.</param>
        /// <param name="inPlace">
        ///     If set to true, the different generated combinations will be stored in
        ///     the same array, thus preserving memory. However, this may prevent the
        ///     samples from being stored in other locations without having to clone
        ///     them. If set to false, a new memory block will be allocated for each
        ///     new object in the sequence.
        /// </param>
        public static IEnumerable<T[]> Combinations<T>(this T[] values, int k, bool inPlace = false)
        {
            // Based on the Knuth algorithm implementation by
            // http://seekwell.wordpress.com/2007/11/17/knuth-generating-all-combinations/

            var n = values.Length;

            var t = k;

            var c = new int[t + 3];
            var current = new T[t];
            int j, x;

            for (j = 1; j <= t; j++)
                c[j] = j - 1;
            c[t + 1] = n;
            c[t + 2] = 0;

            j = t;

            do
            {
                for (var i = 0; i < current.Length; i++)
                    current[i] = values[c[i + 1]];

                yield return inPlace ? current : (T[])current.Clone();

                if (j > 0)
                {
                    x = j;
                }
                else
                {
                    if (c[1] + 1 < c[2])
                    {
                        c[1]++;
                        continue;
                    }

                    j = 2;
                }

                while (true)
                {
                    c[j - 1] = j - 2;
                    x = c[j] + 1;
                    if (x == c[j + 1])
                        j++;
                    else break;
                }

                c[j] = x;
                j--;
            } while (j < t);
        }

        /// <summary>
        ///     Generates all possibles subsets of the given set.
        /// </summary>
        public static IEnumerable<SortedSet<T>> Subsets<T>(this ISet<T> set, bool inPlace = false)
        {
            // TODO: Optimize
            var values = set.ToArray();
            for (var i = 0; i < values.Length; i++)
                foreach (var value in Combinations(values, i + 1, inPlace))
                    yield return new SortedSet<T>(value);
        }

        /// <summary>
        ///     Generates all possibles subsets of size k of the given set.
        /// </summary>
        public static IEnumerable<SortedSet<T>> Subsets<T>(this ISet<T> set, int k, bool inPlace = false)
        {
            // TODO: Optimize
            var values = set.ToArray();
            foreach (var value in Combinations(values, k, inPlace))
                yield return new SortedSet<T>(value);
        }

        /// <summary>
        ///     Enumerates all possible value permutations for a given array.
        /// </summary>
        /// <param name="values">The array whose permutations need to be generated</param>
        /// .
        /// <param name="inPlace">
        ///     If set to true, the different generated permutations will be stored in
        ///     the same array, thus preserving memory. However, this may prevent the
        ///     samples from being stored in other locations without having to clone
        ///     them. If set to false, a new memory block will be allocated for each
        ///     new object in the sequence.
        /// </param>
        public static IEnumerable<T[]> Permutations<T>(T[] values, bool inPlace = false)
        {
            var current = new T[values.Length];

            yield return inPlace ? values : (T[])values.Clone();

            var idx = Vector.Range(0, values.Length);

            int j, l;

            while (true)
            {
                for (j = values.Length - 2; j >= 0; j--)
                    if (idx[j + 1] > idx[j])
                        break;

                if (j == -1) yield break;

                for (l = values.Length - 1; l > j; l--)
                    if (idx[l] > idx[j])
                        break;

                var temp = idx[j];
                idx[j] = idx[l];
                idx[l] = temp;

                for (var i = j + 1; i < idx.Length; i++)
                {
                    if (i > idx.Length - i + j) break;
                    temp = idx[i];
                    idx[i] = idx[idx.Length - i + j];
                    idx[idx.Length - i + j] = temp;
                }

                for (var i = 0; i < values.Length; i++)
                    current[i] = values[idx[i]];

                yield return inPlace ? current : (T[])current.Clone();
            }
        }
    }
}
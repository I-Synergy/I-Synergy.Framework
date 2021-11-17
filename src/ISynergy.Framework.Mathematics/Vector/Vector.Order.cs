﻿namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Static class Vector. Defines a set of extension methods
    ///     that operates mainly on single-dimensional arrays.
    /// </summary>
    /// <seealso cref="Jagged" />
    /// <seealso cref="Vector" />
    public static partial class Vector
    {
        /// <summary>
        ///     Shuffles an array.
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            var random = Framework.Mathematics.Random.Generator.Random;

            // i is the number of items remaining to be shuffled.
            for (var i = array.Length; i > 1; i--)
            {
                // Pick a random element to swap with the i-th element.
                var j = random.Next(i);

                // Swap array elements.
                var aux = array[j];
                array[j] = array[i - 1];
                array[i - 1] = aux;
            }
        }

        /// <summary>
        ///     Shuffles a collection.
        /// </summary>
        public static void Shuffle<T>(this IList<T> array)
        {
            var random = Framework.Mathematics.Random.Generator.Random;

            // i is the number of items remaining to be shuffled.
            for (var i = array.Count; i > 1; i--)
            {
                // Pick a random element to swap with the i-th element.
                var j = random.Next(i);

                // Swap array elements.
                var aux = array[j];
                array[j] = array[i - 1];
                array[i - 1] = aux;
            }
        }

        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static void Sort<T>(this T[] values, Comparison<T> comparison, bool stable = false, bool asc = true)
        {
            if (!stable)
            {
                Array.Sort(values, comparison);
            }
            else
            {
                var keys = new KeyValuePair<int, T>[values.Length];
                for (var i = 0; i < values.Length; i++)
                    keys[i] = new KeyValuePair<int, T>(i, values[i]);
                Array.Sort(keys, values, new StableComparer<T>(comparison));
            }

            if (!asc)
                Array.Reverse(values);
        }

        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static void Sort<T>(this T[] values, bool stable = false, bool asc = true)
            where T : IComparable<T>
        {
            if (!stable)
            {
                Array.Sort(values);
            }
            else
            {
                var keys = new KeyValuePair<int, T>[values.Length];
                for (var i = 0; i < values.Length; i++)
                    keys[i] = new KeyValuePair<int, T>(i, values[i]);
                Array.Sort(keys, values, new StableComparer<T>((a, b) => a.CompareTo(b)));
            }

            if (!asc)
                Array.Reverse(values);
        }

        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static void Sort<T>(this T[] values, out int[] order, bool stable = false, ComparerDirection direction = ComparerDirection.Ascending)
            where T : IComparable<T>
        {
            if (!stable)
            {
                order = Range(values.Length);
                Array.Sort(values, order);

                if (direction == ComparerDirection.Descending)
                {
                    Array.Reverse(values);
                    Array.Reverse(order);
                }
            }
            else
            {
                var keys = new KeyValuePair<int, T>[values.Length];
                for (var i = 0; i < values.Length; i++)
                    keys[i] = new KeyValuePair<int, T>(i, values[i]);

                if (direction == ComparerDirection.Ascending)
                    Array.Sort(keys, values, new StableComparer<T>((a, b) => a.CompareTo(b)));
                else
                    Array.Sort(keys, values, new StableComparer<T>((a, b) => -a.CompareTo(b)));

                order = new int[values.Length];
                for (var i = 0; i < keys.Length; i++)
                    order[i] = keys[i].Key;
            }
        }
        /// <summary>
        ///     Shuffles an array.
        /// </summary>
        public static T[] Shuffled<T>(this T[] array)
        {
            var clone = (T[])array.Clone();
            Shuffle(clone);
            return clone;
        }

        /// <summary>
        ///     Shuffles a collection.
        /// </summary>
        public static TList Shuffled<TList, T>(this TList array)
            where TList : ICloneable, IList<T>
        {
            var clone = (TList)array.Clone();
            Shuffle(clone);
            return clone;
        }
        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static T[] Sorted<T>(this T[] values, Comparison<T> comparison, bool stable = false)
        {
            var clone = (T[])values.Clone();
            Sort(clone, comparison, stable);
            return clone;
        }

        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static T[] Sorted<T>(this T[] values, bool stable = false, bool asc = true)
            where T : IComparable<T>
        {
            var clone = (T[])values.Clone();
            Sort(clone, stable, asc);
            return clone;
        }

        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static T[] Sorted<T>(this ICollection<T> values, bool stable = false)
            where T : IComparable<T>
        {
            var clone = new T[values.Count];
            values.CopyTo(clone, 0);
            Sort(clone, stable);
            return clone;
        }

        /// <summary>
        ///     Sorts the elements of an entire one-dimensional array using the given comparison.
        /// </summary>
        public static T[] Sorted<T>(this T[] values, out int[] order, bool stable = false,
            ComparerDirection direction = ComparerDirection.Ascending)
            where T : IComparable<T>
        {
            var clone = (T[])values.Clone();
            Sort(clone, out order, stable, direction);
            return clone;
        }
    }
}
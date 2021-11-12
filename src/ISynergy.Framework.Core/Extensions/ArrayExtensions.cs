using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Array extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Generic array converter to double array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        private static double[] ToDoubleArray<T>(this T[] array)
        {
            var result = new List<double>();

            foreach (var item in array)
                result.Add(Convert.ToDouble(array));

            return result.ToArray();
        }

        /// <summary>
        /// Generic array converter to double array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        private static double[,] ToDoubleArray<T>(this T[,] array)
        {
            var result = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    result[i,j] = Convert.ToDouble(array[i,j]);

            return result;
        }

        /// <summary>
        /// Generic array converter to double array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        private static double[][] ToDoubleArray<T>(this T[][] array)
        {
            var result = new double[array.GetLength()[0]][];

            for (int i = 0; i < array.GetLength()[0]; i++)
                for (int j = 0; j < array.GetLength()[1]; j++)
                    result[i][j] = Convert.ToDouble(array[i][j]);

            return result;
        }

        /// <summary>
        /// Integer array to double array converter.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this int[] array) => ToDoubleArray<int>(array);

        /// <summary>
        /// Long array to double array converter.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this long[] array) => ToDoubleArray<long>(array);

        /// <summary>
        /// Float array to double array converter.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this float[] array) => ToDoubleArray<float>(array);

        /// <summary>
        /// Decimal array to double array converter.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this decimal[] array) => ToDoubleArray<decimal>(array);

        /// <summary>
        /// Byte array to double array converter.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this byte[] array) => ToDoubleArray<byte>(array);

        /// <summary>
        ///     Gets the total length over all dimensions of an array.
        /// </summary>
        public static int GetTotalLength(this Array array, bool deep = true, bool rectangular = true)
        {
            if (deep && IsJagged(array))
            {
                if (rectangular)
                {
                    var rest = GetTotalLength(array.GetValue(0) as Array, deep);
                    return array.Length * rest;
                }

                var sum = 0;
                for (var i = 0; i < array.Length; i++)
                    sum += GetTotalLength(array.GetValue(i) as Array, deep);
                return sum;
            }

            return array.Length;
        }

        /// <summary>
        ///     Gets the length of each dimension of an array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="deep">
        ///     Pass true to retrieve all dimensions of the array,
        ///     even if it contains nested arrays (as in jagged matrices)
        /// </param>
        /// <param name="max">
        ///     Gets the maximum length possible for each dimension (in case
        ///     the jagged matrices has different lengths).
        /// </param>
        public static int[] GetLength(this Array array, bool deep = true, bool max = false)
        {
            if (array == null)
                return new[] { -1 };
            if (array.Rank == 0)
                return new int[0];

            if (deep && IsJagged(array))
            {
                if (array.Length == 0)
                    return new int[0];

                int[] rest;
                if (!max)
                {
                    rest = GetLength(array.GetValue(0) as Array, deep);
                }
                else
                {
                    // find the max
                    rest = GetLength(array.GetValue(0) as Array, deep);
                    for (var i = 1; i < array.Length; i++)
                    {
                        var r = GetLength(array.GetValue(i) as Array, deep);

                        for (var j = 0; j < r.Length; j++)
                            if (r[j] > rest[j])
                                rest[j] = r[j];
                    }
                }

                return array.Length.Concatenate(rest);
            }

            var vector = new int[array.Rank];
            for (var i = 0; i < vector.Length; i++)
                vector[i] = array.GetUpperBound(i) + 1;
            return vector;
        }
                
        /// <summary>
        ///     Determines whether an array is a jagged array
        ///     (containing inner arrays as its elements).
        /// </summary>
        public static bool IsJagged(this Array array)
        {
            if (array.Length == 0)
                return array.Rank == 1;
            return array.GetType().GetElementType().IsArray;
        }

        /// <summary>
        ///     Determines whether an array is an multidimensional array.
        /// </summary>
        public static bool IsMatrix(this Array array)
        {
            return array.Rank > 1;
        }

        /// <summary>
        ///     Determines whether an array is a vector.
        /// </summary>
        public static bool IsVector(this Array array)
        {
            return array.Rank == 1 && !IsJagged(array);
        }

        /// <summary>
        ///     Combines a vector and a element horizontally.
        /// </summary>
        public static T[] Concatenate<T>(this T element, T[] vector)
        {
            var r = new T[vector.Length + 1];

            r[0] = element;

            for (var i = 0; i < vector.Length; i++)
                r[i + 1] = vector[i];

            return r;
        }

        /// <summary>
        /// Checks if array is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(T[] array)
            where T: class
        {
            if (array == null || array.Length == 0)
                return true;
            else
                return array.All(item => item == null);
        }
    }
}

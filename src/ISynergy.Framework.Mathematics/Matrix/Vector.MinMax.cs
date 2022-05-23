using System;

namespace ISynergy.Framework.Mathematics
{
    public static partial class Matrix
    {
        /// <summary>
        ///     Gets the indices that sort a vector.
        /// </summary>
        public static int[] ArgSort<T>(this T[] values)
            where T : IComparable<T>
        {
            int[] idx;
            values.Copy().Sort(out idx);
            return idx;
        }

        #region Vector ArgMin/ArgMax

        /// <summary>
        ///     Gets the maximum element in a vector.
        /// </summary>
        public static int ArgMax<T>(this T[] values)
            where T : IComparable<T>
        {
            var imax = 0;
            var max = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }

            return imax;
        }

        /// <summary>
        ///     Gets the maximum element in a vector.
        /// </summary>
        public static int ArgMax<T>(this T[] values, out T max)
            where T : IComparable<T>
        {
            var imax = 0;
            max = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }

            return imax;
        }

        /// <summary>
        ///     Gets the minimum element in a vector.
        /// </summary>
        public static int ArgMin<T>(this T[] values)
            where T : IComparable<T>
        {
            var imin = 0;
            var min = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }

            return imin;
        }

        /// <summary>
        ///     Gets the minimum element in a vector.
        /// </summary>
        public static int ArgMin<T>(this T[] values, out T min)
            where T : IComparable<T>
        {
            var imin = 0;
            min = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }

            return imin;
        }

        #endregion
        #region Vector Min/Max

        /// <summary>
        ///     Gets the maximum non-null element in a vector.
        /// </summary>
        public static T? Max<T>(this T?[] values, out int imax)
            where T : struct, IComparable<T>
        {
            imax = -1;
            T? max = null;

            for (var i = 0; i < values.Length; i++)
                if (values[i].HasValue)
                    if (max is null || values[i].Value.CompareTo(max.Value) > 0)
                    {
                        max = values[i];
                        imax = i;
                    }

            return max;
        }

        /// <summary>
        ///     Gets the minimum non-null element in a vector.
        /// </summary>
        public static T? Min<T>(this T?[] values, out int imin)
            where T : struct, IComparable<T>
        {
            imin = -1;
            T? min = null;

            for (var i = 0; i < values.Length; i++)
                if (values[i].HasValue)
                    if (min is null || values[i].Value.CompareTo(min.Value) < 0)
                    {
                        min = values[i];
                        imin = i;
                    }

            return min;
        }
        /// <summary>
        ///     Gets the maximum element in a vector.
        /// </summary>
        public static T Max<T>(this T[] values, out int imax)
            where T : IComparable<T>
        {
            imax = 0;
            var max = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }

            return max;
        }

        /// <summary>
        ///     Gets the maximum element in a vector.
        /// </summary>
        public static T Max<T>(this T[] values, out int imax, bool alreadySorted)
            where T : IComparable<T>
        {
            if (alreadySorted)
            {
                imax = values.Length - 1;
                return values[values.Length - 1];
            }

            return Max(values, out imax);
        }

        /// <summary>
        ///     Gets the maximum element in a vector.
        /// </summary>
        public static T Max<T>(this T[] values)
            where T : IComparable<T>
        {
            int imax;
            return Max(values, out imax);
        }
        /// <summary>
        ///     Gets the minimum element in a vector.
        /// </summary>
        public static T Min<T>(this T[] values, out int imin)
            where T : IComparable<T>
        {
            imin = 0;
            var min = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }

            return min;
        }

        /// <summary>
        ///     Gets the minimum element in a vector.
        /// </summary>
        public static T Min<T>(this T[] values)
            where T : IComparable<T>
        {
            int imin;
            return Min(values, out imin);
        }

        #endregion
        #region limited length

        /// <summary>
        ///     Gets the maximum element in a vector up to a fixed length.
        /// </summary>
        public static T Max<T>(this T[] values, int length, out int imax)
            where T : IComparable<T>
        {
            imax = 0;
            var max = values[0];
            for (var i = 1; i < length; i++)
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }

            return max;
        }

        /// <summary>
        ///     Gets the maximum element in a vector up to a fixed length.
        /// </summary>
        public static T Max<T>(this T[] values, int length)
            where T : IComparable<T>
        {
            int imax;
            return Max(values, length, out imax);
        }
        /// <summary>
        ///     Gets the minimum element in a vector up to a fixed length.
        /// </summary>
        public static T Min<T>(this T[] values, int length, out int imax)
            where T : IComparable<T>
        {
            imax = 0;
            var max = values[0];
            for (var i = 1; i < length; i++)
                if (values[i].CompareTo(max) < 0)
                {
                    max = values[i];
                    imax = i;
                }

            return max;
        }

        /// <summary>
        ///     Gets the minimum element in a vector up to a fixed length.
        /// </summary>
        public static T Min<T>(this T[] values, int length)
            where T : IComparable<T>
        {
            int imin;
            return Min(values, length, out imin);
        }

        #endregion
    }
}
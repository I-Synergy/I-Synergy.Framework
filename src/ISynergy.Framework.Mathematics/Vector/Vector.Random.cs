using System.Diagnostics;

namespace ISynergy.Framework.Mathematics
{
    public static partial class Vector
    {
        /// <summary>
        ///     Creates a vector with uniformly distributed random data.
        /// </summary>
        public static double[] Random(int size)
        {
            return Random(size, 0.0, 1.0);
        }

        /// <summary>
        ///     Draws a random sample from a group of observations, with or without repetitions.
        /// </summary>
        /// <typeparam name="T">The type of the observations.</typeparam>
        /// <param name="values">The observation vector.</param>
        /// <param name="size">The size of the sample to be drawn (how many samples to get).</param>
        /// <param name="replacement">
        ///     Whether to sample with replacement (repeating values) or without replacement (non-repeating
        ///     values).
        /// </param>
        /// <returns>A vector containing the samples drawn from <paramref name="values" />.</returns>
        public static T[] Sample<T>(this T[] values, int size, bool replacement = false)
        {
            var idx = replacement ? Random(size, 0, values.Length) : Sample(size, values.Length);
            Debug.Assert(idx.Length == size);
            return values.Get(idx);
        }
    }
}
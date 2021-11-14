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

        /// <summary>
        ///   Returns a vector of the specified <paramref name="sampleSize"/> containing 
        ///   non-repeating indices in the range [0, populationSize) in random order.
        /// </summary>
        /// 
        /// <param name="sampleSize">The size of the sample vector to be generated.</param>
        /// <param name="populationSize">The non-inclusive maximum number an index can have.</param>
        /// 
        /// <remarks>
        ///   In other words, this return a sample of size <c>k</c> from a population
        ///   of size <c>N</c>, where <c>k</c> is the parameter <paramref name="sampleSize"/>
        ///   and <c>N</c> is the parameter <paramref name="populationSize"/>.
        /// </remarks>
        /// 
        /// <example>
        /// <code>
        ///   var a = Vector.Sample(3, 10);  // a possible output is { 1, 7, 4 };
        ///   var b = Vector.Sample(10, 10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Vector.Sample(5, 6))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static int[] Sample(int sampleSize, int populationSize)
        {
            if ((int)sampleSize > populationSize)
            {
                throw new ArgumentOutOfRangeException("size", String.Format(
                    "The sample size {0} must be less than the size of the population {1}.",
                    sampleSize, populationSize));
            }

            int[] idx = Sample(populationSize);
            return idx.First(sampleSize);
        }

        /// <summary>
        ///   Returns a vector of the specified <paramref name="percentage"/> of the 
        ///   <paramref name="populationSize"/> containing non-repeating indices in the 
        ///   range [0, populationSize) in random order.
        /// </summary>
        /// 
        /// <param name="percentage">The percentage of the population to sample.</param>
        /// <param name="populationSize">The non-inclusive maximum number an index can have.</param>
        /// 
        /// <example>
        /// <code>
        ///   var a = Vector.Sample(0.3, 10);  // a possible output is { 1, 7, 4 };
        ///   var b = Vector.Sample(1.0, 10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Vector.Sample(0.2, 6))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static int[] Sample(double percentage, int populationSize)
        {
            if (percentage < 0 || percentage > 1)
            {
                throw new ArgumentOutOfRangeException("percentage", String.Format(
                    "The sample percentage {0} must be between 0 and 1.", percentage));
            }

            int sampleSize = (int)System.Math.Floor(percentage * (double)populationSize);

            int[] idx = Sample(populationSize);
            return idx.First(sampleSize);
        }

        /// <summary>
        ///   Returns a vector containing indices (0, 1, 2, ..., n - 1) in random 
        ///   order. The vector grows up to to <paramref name="size"/>, but does not
        ///   include <c>size</c> among its values.
        /// </summary>
        ///
        /// <param name="size">The size of the sample vector to be generated.</param>
        /// 
        /// <example>
        /// <code>
        ///   var a = Vector.Sample(3);  // a possible output is { 2, 1, 0 };
        ///   var b = Vector.Sample(10); // a possible output is { 5, 4, 2, 0, 1, 3, 7, 9, 8, 6 };
        ///   
        ///   foreach (var i in Vector.Sample(5))
        ///   {
        ///      // ...
        ///   }
        /// </code>
        /// </example>
        /// 
        public static int[] Sample(int size)
        {
            var random = Generator.Random;

            var idx = Vector.Range(size);
            var x = new double[idx.Length];
            for (int i = 0; i < x.Length; i++)
                x[i] = random.NextDouble();

            Array.Sort(x, idx);

            return idx;
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static double[] Random(int size, double min, double max)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

            var vector = new double[size];
            for (var i = 0; i < size; i++)
                vector[i] = (double)random.NextDouble() * (max - min) + min;
            return vector;
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        public static int[] Random(int size, int min, int max)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            var random = ISynergy.Framework.Mathematics.Random.Generator.Random;

            var vector = new int[size];
            for (var i = 0; i < size; i++)
                vector[i] = (int)random.NextDouble() * (max - min) + min;
            return vector;
        }
    }
}
namespace ISynergy.Framework.Mathematics.Random
{
    /// <summary>
    ///     Dummy random number generator that always generates the same number.
    /// </summary>
    public sealed class ConstantGenerator :
        IRandomNumberGenerator<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantGenerator" /> class.
        /// </summary>
        /// <param name="value">The constant value to be generated.</param>
        public ConstantGenerator(double value)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets or sets the constant value returned by this generator.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        ///     Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>
        ///     A random vector of observations drawn from this distribution.
        /// </returns>
        public double[] Generate(int samples)
        {
            return Generate(samples, new double[samples]);
        }

        /// <summary>
        ///     Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>
        ///     A random vector of observations drawn from this distribution.
        /// </returns>
        public double[] Generate(int samples, double[] result)
        {
            for (var i = 0; i < samples; i++)
                result[i] = Value;
            return result;
        }
        /// <summary>
        ///     Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <returns>
        ///     A random vector of observations drawn from this distribution.
        /// </returns>
        public double Generate()
        {
            return Value;
        }
    }
}
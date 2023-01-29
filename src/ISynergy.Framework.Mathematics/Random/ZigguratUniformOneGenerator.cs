namespace ISynergy.Framework.Mathematics.Random
{
    /// <summary>
    ///     Uniform random number generator using the Ziggurat method.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     <a href="http://people.sc.fsu.edu/~jburkardt/c_src/ziggurat/ziggurat.html">
    ///                         John Burkard, Ziggurat Random Number Generator (RNG). Available on:
    ///                         http://people.sc.fsu.edu/~jburkardt/c_src/ziggurat/ziggurat.c (LGPL)
    ///                     </a>
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Philip Leong, Guanglie Zhang, Dong-U Lee, Wayne Luk, John Villasenor,
    ///                     A comment on the implementation of the ziggurat method,
    ///                     Journal of Statistical Software, Volume 12, Number 7, February 2005.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     George Marsaglia, Wai Wan Tsang, The Ziggurat _method for Generating Random Variables,
    ///                     Journal of Statistical Software, Volume 5, Number 8, October 2000, seven pages.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public sealed class ZigguratUniformOneGenerator :
        IRandomNumberGenerator<double>,
        IRandomNumberGenerator<int>,
        IRandomNumberGenerator<uint>
    {
        private uint jsr;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZigguratExponentialGenerator" /> class.
        /// </summary>
        public ZigguratUniformOneGenerator()
            : this(Generator.Random.Next())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZigguratUniformOneGenerator" /> class.
        /// </summary>
        /// <param name="seed">
        ///     The random seed to use. Default is to use the next value from
        ///     the <see cref="Generator">the framework-wide random generator</see>.
        /// </param>
        public ZigguratUniformOneGenerator(int seed)
        {
            jsr = (uint)seed;
        }

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
                result[i] = Generate();
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
            unchecked
            {
                var value = jsr;
                jsr = jsr ^ (jsr << 13);
                jsr = jsr ^ (jsr >> 17);
                jsr = jsr ^ (jsr << 5);
                // https://core.ac.uk/download/files/153/6287927.pdf
                // return 0.5 + (value + jsr) * 0.2328306e-9;
                return (0.5 + (value + jsr) / 65536.0 / 65536.0) % 1.0;
            }
        }

        /// <summary>
        ///     Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>
        ///     A random vector of observations drawn from this distribution.
        /// </returns>
        public int[] Generate(int samples, int[] result)
        {
            for (var i = 0; i < samples; i++)
                result[i] = (int)Next();
            return result;
        }
        int[] IRandomNumberGenerator<int>.Generate(int samples)
        {
            return Generate(samples, new int[samples]);
        }

        int IRandomNumberGenerator<int>.Generate()
        {
            return (int)Next();
        }

        /// <summary>
        ///     Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>
        ///     A random vector of observations drawn from this distribution.
        /// </returns>
        public uint[] Generate(int samples, uint[] result)
        {
            for (var i = 0; i < samples; i++)
                result[i] = Next();
            return result;
        }

        uint[] IRandomNumberGenerator<uint>.Generate(int samples)
        {
            return Generate(samples, new uint[samples]);
        }

        uint IRandomNumberGenerator<uint>.Generate()
        {
            return Next();
        }

        /// <summary>
        ///     Generates a new non-negative integer random number.
        /// </summary>
        public uint Next()
        {
            unchecked
            {
                var value = jsr;
                jsr = jsr ^ (jsr << 13);
                jsr = jsr ^ (jsr >> 17);
                jsr = jsr ^ (jsr << 5);
                return value + jsr;
            }
        }
    }
}
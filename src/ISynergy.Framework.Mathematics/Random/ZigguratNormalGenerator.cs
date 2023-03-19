namespace ISynergy.Framework.Mathematics.Random
{
    /// <summary>
    ///     Normal random number generator using the Ziggurat method.
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
    public class ZigguratNormalGenerator : IRandomNumberGenerator<double>
    {
        private readonly double[] fn;
        private readonly uint[] kn;
        private readonly ZigguratUniformOneGenerator u;
        private readonly double[] wn;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZigguratNormalGenerator" /> class.
        /// </summary>
        /// <param name="seed">
        ///     The random seed to use. Default is to use the next value from
        ///     the <see cref="Generator">the framework-wide random generator</see>.
        /// </param>
        public ZigguratNormalGenerator(int seed)
        {
            u = new ZigguratUniformOneGenerator(seed);

            kn = new uint[128];
            fn = new double[128];
            wn = new double[128];
            setup();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZigguratNormalGenerator" /> class.
        /// </summary>
        public ZigguratNormalGenerator()
            : this(Generator.Random.Next())
        {
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
            const double r = 3.442620;

            var hz = unchecked((int)u.Next());
            var iz = (uint)(hz & 127);

            if (Math.Abs(hz) < kn[iz])
                return hz * wn[iz];

            for (; ; )
            {
                double x;
                double y;

                if (iz == 0)
                {
                    for (; ; )
                    {
                        x = -0.2904764 * Math.Log(u.Generate());
                        y = -Math.Log(u.Generate());
                        if (x * x <= y + y) break;
                    }

                    if (hz <= 0)
                        return -r - x;
                    return +r + x;
                }

                x = hz * wn[iz];

                if (fn[iz] + u.Generate() * (fn[iz - 1] - fn[iz]) < Math.Exp(-0.5 * x * x))
                    return x;

                hz = unchecked((int)u.Next());
                iz = (uint)(hz & 127);

                if (Math.Abs(hz) < kn[iz])
                    return hz * wn[iz];
            }

            // throw new InvalidOperationException("Execution should not reach here.");
        }
        private void setup()
        {
            var dn = 3.442619855899;
            const double m1 = 2147483648.0;
            var tn = 3.442619855899;
            const double vn = 9.91256303526217E-03;

            var q = vn / Math.Exp(-0.5 * dn * dn);

            kn[0] = (uint)(dn / q * m1);
            kn[1] = 0;

            wn[0] = q / m1;
            wn[127] = dn / m1;

            fn[0] = 1.0;
            fn[127] = Math.Exp(-0.5 * dn * dn);

            for (var i = 126; 1 <= i; i--)
            {
                dn = Math.Sqrt(-2.0 * Math.Log(vn / dn + Math.Exp(-0.5 * dn * dn)));
                kn[i + 1] = (uint)(dn / tn * m1);
                tn = dn;
                fn[i] = Math.Exp(-0.5 * dn * dn);
                wn[i] = dn / m1;
            }
        }
    }
}
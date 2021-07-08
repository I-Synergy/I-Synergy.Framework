namespace ISynergy.Framework.Mathematics.Optimization
{
    /// <summary>
    ///     Inner status of the <see cref="BoundedBroydenFletcherGoldfarbShanno" />
    ///     optimization algorithm. This class contains implementation details that
    ///     can change at any time.
    /// </summary>
    public class BoundedBroydenFletcherGoldfarbShannoInnerStatus
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BoundedBroydenFletcherGoldfarbShannoInnerStatus" /> class with the
        ///     inner
        ///     status values from the original FORTRAN L-BFGS implementation.
        /// </summary>
        /// <param name="isave">The isave L-BFGS status argument.</param>
        /// <param name="dsave">The dsave L-BFGS status argument.</param>
        /// <param name="lsave">The lsave L-BFGS status argument.</param>
        /// <param name="csave">The csave L-BFGS status argument.</param>
        /// <param name="work">The work L-BFGS status argument.</param>
        public BoundedBroydenFletcherGoldfarbShannoInnerStatus(
            int[] isave, double[] dsave, bool[] lsave, string csave, double[] work)
        {
            Integers = (int[])isave.Clone();
            Doubles = (double[])dsave.Clone();
            Booleans = (bool[])lsave.Clone();
            Strings = csave;
            Work = (double[])work.Clone();
        }

        /// <summary>
        ///     Gets or sets the isave status from the
        ///     original FORTRAN L-BFGS implementation.
        /// </summary>
        public int[] Integers { get; set; }

        /// <summary>
        ///     Gets or sets the dsave status from the
        ///     original FORTRAN L-BFGS implementation.
        /// </summary>
        public double[] Doubles { get; set; }

        /// <summary>
        ///     Gets or sets the lsave status from the
        ///     original FORTRAN L-BFGS implementation.
        /// </summary>
        public bool[] Booleans { get; set; }

        /// <summary>
        ///     Gets or sets the csave status from the
        ///     original FORTRAN L-BFGS implementation.
        /// </summary>
        public string Strings { get; set; }

        /// <summary>
        ///     Gets or sets the work vector from the
        ///     original FORTRAN L-BFGS implementation.
        /// </summary>
        public double[] Work { get; set; }
    }
}
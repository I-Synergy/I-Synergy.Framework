namespace ISynergy.Framework.Mathematics
{
    public static partial class Matrix
    {
        /// <summary>
        ///     Elementwise multiply operation.
        /// </summary>
        public static double[,] ElementwiseMultiply(double[,] a, double[,] b)
        {
            return Elementwise.Multiply(a, b);
        }
    }
}
namespace ISynergy.Framework.Mathematics.Distances
{
    using ISynergy.Framework.Mathematics.Distances.Base;
    using System;

    /// <summary>
    ///   Cosine distance. For a proper distance metric, see <see cref="Angular"/>.
    /// </summary>
    /// 
    /// <seealso cref="Angular"/>
    /// 
    [Serializable]
    public struct Cosine : IDistance<double[]>, ISimilarity<double[]>, ICloneable
    {
        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            double sum = 0;
            double p = 0;
            double q = 0;

            for (var i = 0; i < x.Length; i++)
            {
                sum += x[i] * y[i];
                p += x[i] * x[i];
                q += y[i] * y[i];
            }

            double den = Math.Sqrt(p) * Math.Sqrt(q);
            return sum == 0 ? 1.0 : 1.0 - (sum / den);
        }

        /// <summary>
        ///   Gets a similarity measure between two points.
        /// </summary>
        /// 
        /// <param name="x">The first point to be compared.</param>
        /// <param name="y">The second point to be compared.</param>
        /// 
        /// <returns>A similarity measure between x and y.</returns>
        /// 
        public double Similarity(double[] x, double[] y)
        {
            double sum = 0;
            double p = 0;
            double q = 0;

            for (var i = 0; i < x.Length; i++)
            {
                sum += x[i] * y[i];
                p += x[i] * x[i];
                q += y[i] * y[i];
            }

            double den = Math.Sqrt(p) * Math.Sqrt(q);
            return (sum == 0) ? 0 : sum / den;
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Cosine();
        }
    }
}

namespace ISynergy.Framework.Mathematics.Distances
{
    using ISynergy.Framework.Mathematics.Distances.Base;
    using System;

    /// <summary>
    ///   Chebyshev distance.
    /// </summary>
    /// 
    [Serializable]
    public struct Chebyshev : IMetric<double[]>, ICloneable
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
            double max = System.Math.Abs(x[0] - y[0]);

            for (var i = 1; i < x.Length; i++)
            {
                double abs = System.Math.Abs(x[i] - y[i]);
                if (abs > max)
                    max = abs;
            }

            return max;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Chebyshev();
        }
    }
}

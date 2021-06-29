namespace ISynergy.Framework.Mathematics.Distances
{
    using System;

    /// <summary>
    ///   Bray-Curtis distance.
    /// </summary>
    /// 
    [Serializable]
    public struct BrayCurtis : IDistance<double[]>, ICloneable
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
            double sumP = 0;
            double sumN = 0;

            for (int i = 0; i < x.Length; i++)
            {
                sumN += Math.Abs(x[i] - y[i]);
                sumP += Math.Abs(x[i] + y[i]);
            }

            return sumN / sumP;
        }



        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new BrayCurtis();
        }
    }
}

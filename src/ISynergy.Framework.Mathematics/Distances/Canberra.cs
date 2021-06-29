namespace ISynergy.Framework.Mathematics.Distances
{
    using System;

    /// <summary>
    ///   Canberra distance.
    /// </summary>
    /// 
    [Serializable]
    public struct Canberra : IDistance<double[]>, ICloneable
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
            for (int i = 0; i < x.Length; i++)
                sum += Math.Abs(x[i] - y[i]) / (Math.Abs(x[i]) + Math.Abs(y[i]));
            return sum;
        }




        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Canberra();
        }
    }
}

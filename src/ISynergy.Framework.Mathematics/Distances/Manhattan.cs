namespace ISynergy.Framework.Mathematics.Distances
{
    using System;

    /// <summary>
    ///   Manhattan (also known as Taxicab or L1) distance.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Taxicab geometry, considered by Hermann Minkowski in 19th century Germany, 
    ///   is a form of geometry in which the usual distance function of metric or
    ///   Euclidean geometry is replaced by a new metric in which the distance between 
    ///   two points is the sum of the absolute differences of their Cartesian 
    ///   coordinates. The taxicab metric is also known as rectilinear distance, L1 
    ///   distance or L1 norm (see Lp space), city block distance, Manhattan distance,
    ///   or Manhattan length, with corresponding variations in the name of the geometry.
    ///   The latter names allude to the grid layout of most streets on the island of 
    ///   Manhattan, which causes the shortest path a car could take between two intersections
    ///   in the borough to have length equal to the intersections' distance in taxicab
    ///   geometry.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Taxicab_geometry">
    ///       https://en.wikipedia.org/wiki/Taxicab_geometry </a></description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    [Serializable]
    public struct Manhattan : IMetric<double[]>, IMetric<int[]>, ICloneable
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
        public double Distance(int[] x, int[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += System.Math.Abs(x[i] - y[i]);
            return sum;
        }

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
                sum += System.Math.Abs(x[i] - y[i]);
            return sum;
        }



        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Manhattan();
        }
    }
}

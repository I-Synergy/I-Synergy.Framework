namespace ISynergy.Framework.Mathematics.Distances
{
    /// <summary>
    ///   Herlinger distance.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, the Hellinger distance (also called 
    ///   Bhattacharyya distance as this was originally introduced by Anil Kumar
    ///   Bhattacharya) is used to quantify the similarity between two probability
    ///   distributions. It is a type of f-divergence. The Hellinger distance is 
    ///   defined in terms of the Hellinger integral, which was introduced by Ernst
    ///   Hellinger in 1909.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Hellinger_distance">
    ///       https://en.wikipedia.org/wiki/Hellinger_distance </a></description></item>
    ///   </list></para>  
    /// </remarks>
    /// 
    [Serializable]
    public struct Hellinger : IMetric<double[]>, ICloneable
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
            for (var i = 0; i < x.Length; i++)
                sum += Math.Pow(Math.Sqrt(x[i]) - Math.Sqrt(y[i]), 2);

            return sum / Math.Sqrt(2);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Hellinger();
        }
    }
}

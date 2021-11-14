namespace ISynergy.Framework.Mathematics.Distances
{
    /// <summary>
    ///   Dirac distance.
    /// </summary>
    /// 
    [Serializable]
    public struct Dirac<T> : ISimilarity<T>, IDistance<T>, ICloneable
        where T : IEquatable<T>
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
        public double Distance(T x, T y)
        {
            return x.Equals(y) ? 0.0 : 1.0;
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
        public double Similarity(T x, T y)
        {
            return x.Equals(y) ? 1.0 : 0.0;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Dirac<T>();
        }
    }
}

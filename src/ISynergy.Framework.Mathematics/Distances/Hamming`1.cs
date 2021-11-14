namespace ISynergy.Framework.Mathematics.Distances
{
    /// <summary>
    ///   Hamming distance.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the elements to be compared.</typeparam>
    /// 
    [Serializable]
    public struct Hamming<T> : IMetric<T[]>, ICloneable
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
        public double Distance(T[] x, T[] y)
        {
            int sum = 0;
            for (var i = 0; i < x.Length; i++)
                if (!x[i].Equals(y[i]))
                    sum++;
            return sum;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Hamming<T>();
        }
    }
}

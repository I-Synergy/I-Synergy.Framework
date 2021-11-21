namespace ISynergy.Framework.Mathematics.Distances.Base
{
    /// <summary>
    ///   Common interface for similarity measures.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the first element to be compared.</typeparam>
    /// <typeparam name="U">The type of the second element to be compared.</typeparam>
    /// 
    public interface ISimilarity<T, U>
    {
        /// <summary>
        ///   Gets a similarity measure between two points.
        /// </summary>
        /// 
        /// <param name="x">The first point to be compared.</param>
        /// <param name="y">The second point to be compared.</param>
        /// 
        /// <returns>A similarity measure between x and y.</returns>
        /// 
        double Similarity(T x, U y);
    }

    /// <summary>
    ///   Common interface for similarity measures.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the elements to be compared.</typeparam>
    /// 
    public interface ISimilarity<T> : ISimilarity<T, T>
    {

    }
}

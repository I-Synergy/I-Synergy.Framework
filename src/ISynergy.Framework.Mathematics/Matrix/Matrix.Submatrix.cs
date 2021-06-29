namespace ISynergy.Framework.Mathematics
{
    public static partial class Matrix
    {
        /// <summary>
        ///     Returns a sub matrix extracted from the current matrix.
        /// </summary>
        /// <param name="source">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// <param name="reuseMemory">
        ///     Set to true to avoid memory allocations
        ///     when possible. This might result on the shallow copies of some
        ///     elements. Default is false (default is to always provide a true,
        ///     deep copy of every element in the matrices, using more memory).
        /// </param>
        public static T[][] Submatrix<T>(this T[][] source,
            int[] rowIndexes, int[] columnIndexes, bool reuseMemory = false)
        {
            return get(source, null, rowIndexes, columnIndexes, reuseMemory);
        }
    }
}
namespace ISynergy.Framework.Mathematics.Base
{
    /// <summary>
    /// Interface IBaseMatrix
    /// </summary>
    public interface IBaseMatrix
    {
        /// <summary>
        /// Gets the n.
        /// </summary>
        /// <value>The n.</value>
        int N { get; }
        /// <summary>
        /// Gets or sets the <see cref="System.Double" /> with the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns>System.Double.</returns>
        double this[int row, int col] { get; set; }
    }
}

using System.IO;

namespace ISynergy.Framework.Mathematics.Extensions
{
    /// <summary>
    /// Class MatrixExtensions.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Writes the specified output.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="output">The output.</param>
        /// <param name="elemSize">Size of the elem.</param>
        /// <param name="showPlus">if set to <c>true</c> [show plus].</param>
        public static void Write(this Matrix matrix, TextWriter output, int elemSize = 3, bool showPlus = true)
        {
            for (var row = 0; row < matrix.Rows; row++)
            {
                for (var col = 0; col < matrix.Columns; col++)
                {
                    output.Write(matrix[row, col].ToString().PadRight(elemSize));
                }

                output.WriteLine();
            }
        }
    }
}

using ISynergy.Framework.Mathematics.Formats.Base;
using System.Globalization;

namespace ISynergy.Framework.Mathematics.Formats
{
    /// <summary>
    ///     Gets the matrix representation used in C# multi-dimensional arrays.
    /// </summary>
    /// <remarks>
    ///     This class can be used to convert to and from C#
    ///     matrices and their string representation. Please
    ///     see the example for details.
    /// </remarks>
    /// <example>
    ///     <para>
    ///         Converting from a multidimensional matrix to a
    ///         string representation:
    ///     </para>
    ///     <code>
    ///   // Declare a number array
    ///   double[,] x = 
    ///   {
    ///      { 1, 2, 3, 4 },
    ///      { 5, 6, 7, 8 },
    ///   };
    ///   
    ///   // Convert the aforementioned array to a string representation:
    ///   string str = x.ToString(CSharpMatrixFormatProvider.CurrentCulture);
    ///   
    ///   // the final result will be equivalent to
    ///   "double[,] x =      " +
    ///   "{                  " +
    ///   "   { 1, 2, 3, 4 }, " +
    ///   "   { 5, 6, 7, 8 }, " +
    ///   "}"
    /// </code>
    ///     <para>
    ///         Converting from strings to actual matrices:
    ///     </para>
    ///     <code>
    ///   // Declare an input string
    ///   string str = "double[,] x = " +
    ///   "{                          " +
    ///   "   { 1, 2, 3, 4 },         " +
    ///   "   { 5, 6, 7, 8 },         " +
    ///   "}";
    ///   
    ///   // Convert the string representation to an actual number array:
    ///   double[,] matrix = Matrix.Parse(str, CSharpMatrixFormatProvider.InvariantCulture);
    ///   
    ///   // matrix will now contain the actual multidimensional 
    ///   // matrix representation of the given string.
    /// </code>
    /// </example>
    /// <seealso cref="Mathematics.Matrix" />
    /// <seealso cref="CSharpMatrixFormatProvider" />
    /// <seealso cref="CSharpJaggedMatrixFormatProvider" />
    /// <seealso cref="CSharpArrayFormatProvider" />
    /// <seealso cref="OctaveMatrixFormatProvider" />
    /// <seealso cref="OctaveArrayFormatProvider" />
    public sealed class CSharpMatrixFormatProvider : MatrixFormatProviderBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpMatrixFormatProvider" /> class.
        /// </summary>
        public CSharpMatrixFormatProvider(IFormatProvider innerProvider)
            : base(innerProvider)
        {
            FormatMatrixStart = "new double[,] {\n";
            FormatMatrixEnd = " \n};";
            FormatRowStart = "    { ";
            FormatRowEnd = " }";
            FormatColStart = ", ";
            FormatColEnd = ", ";
            FormatRowDelimiter = ",\n";
            FormatColDelimiter = ", ";

            ParseMatrixStart = "new double[,] {";
            ParseMatrixEnd = "};";
            ParseRowStart = "{";
            ParseRowEnd = "}";
            ParseColStart = ",";
            ParseColEnd = ",";
            ParseRowDelimiter = "},";
            ParseColDelimiter = ",";
        }

        /// <summary>
        ///     Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
        /// </summary>
        public static CSharpMatrixFormatProvider CurrentCulture { get; } = new(CultureInfo.CurrentCulture);

        /// <summary>
        ///     Gets the IMatrixFormatProvider which uses the invariant system culture.
        /// </summary>
        public static CSharpMatrixFormatProvider InvariantCulture { get; } = new(CultureInfo.InvariantCulture);
    }
}
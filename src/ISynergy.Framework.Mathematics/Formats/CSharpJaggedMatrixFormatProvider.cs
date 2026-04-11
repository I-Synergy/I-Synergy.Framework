using ISynergy.Framework.Mathematics.Formats.Base;
using ISynergy.Framework.Mathematics.Matrices;
using System.Globalization;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Formats;

/// <summary>
///     Gets the matrix representation used in C# jagged arrays.
/// </summary>
/// <remarks>
///     This class can be used to convert to and from C#
///     arrays and their string representation. Please
///     see the example for details.
/// </remarks>
/// <example>
///     <para>
///         Converting from a jagged matrix to a string representation:
///     </para>
///     <code>
///   // Declare a number array
///   double[][] x = 
///   {
///      new double[] { 1, 2, 3, 4 },
///      new double[] { 5, 6, 7, 8 },
///   };
///   
///   // Convert the aforementioned array to a string representation:
///   string str = x.ToString(CSharpJaggedMatrixFormatProvider.CurrentCulture);
///   
///   // the final result will be equivalent to
///   "double[][] x =                  " +
///   "{                               " +
///   "   new double[] { 1, 2, 3, 4 }, " +
///   "   new double[] { 5, 6, 7, 8 }, " +
///   "}"
/// </code>
///     <para>
///         Converting from strings to actual arrays:
///     </para>
///     <code>
///   // Declare an input string
///   string str = "double[][] x =     " +
///   "{                               " +
///   "   new double[] { 1, 2, 3, 4 }, " +
///   "   new double[] { 5, 6, 7, 8 }, " +
///   "}";
///   
///   // Convert the string representation to an actual number array:
///   double[][] array = Matrix.Parse(str, CSharpJaggedMatrixFormatProvider.InvariantCulture);
///   
///   // array will now contain the actual jagged 
///   // array representation of the given string.
/// </code>
/// </example>
/// <seealso cref="Matrix" />
/// <seealso cref="CSharpMatrixFormatProvider" />
/// <seealso cref="CSharpJaggedMatrixFormatProvider" />
/// <seealso cref="CSharpArrayFormatProvider" />
/// <seealso cref="OctaveMatrixFormatProvider" />
/// <seealso cref="OctaveArrayFormatProvider" />
public sealed class CSharpJaggedMatrixFormatProvider : MatrixFormatProviderBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CSharpJaggedMatrixFormatProvider" /> class.
    /// </summary>
    public CSharpJaggedMatrixFormatProvider(IFormatProvider innerProvider)
        : base(innerProvider)
    {
        FormatMatrixStart = "new double[][] {\n";
        FormatMatrixEnd = " \n};";
        FormatRowStart = "    new double[] { ";
        FormatRowEnd = " }";
        FormatColStart = ", ";
        FormatColEnd = ", ";
        FormatRowDelimiter = ",\n";
        FormatColDelimiter = ", ";

        ParseMatrixStart = "new double[][] {";
        ParseMatrixEnd = "};";
        ParseRowStart = "new double[] {";
        ParseRowEnd = "}";
        ParseColStart = ",";
        ParseColEnd = ",";
        ParseRowDelimiter = "},";
        ParseColDelimiter = ",";
    }

    /// <summary>
    ///     Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
    /// </summary>
    public static CSharpJaggedMatrixFormatProvider CurrentCulture { get; } = new(CultureInfo.CurrentCulture);

    /// <summary>
    ///     Gets the IMatrixFormatProvider which uses the invariant system culture.
    /// </summary>
    public static CSharpJaggedMatrixFormatProvider InvariantCulture { get; } = new(CultureInfo.InvariantCulture);
}
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
///     Gets the matrix representation used in C# multi-dimensional arrays.
/// </summary>
/// <remarks>
///     This class can be used to convert to and from C#
///     arrays and their string representation. Please
///     see the example for details.
/// </remarks>
/// <example>
///     <para>
///         Converting from an array to a string representation:
///     </para>
///     <code>
///   // Declare a number array
///   double[] x = { 1, 2, 3, 4 };
///   
///   // Convert the aforementioned array to a string representation:
///   string str = x.ToString(CSharpArrayFormatProvider.CurrentCulture);
///   
///   // the final result will be
///   "double[] x = { 1, 2, 3, 4 }"
/// </code>
///     <para>
///         Converting from strings to actual arrays:
///     </para>
///     <code>
///   // Declare an input string
///   string str = "double[] { 1, 2, 3, 4 }";
///   
///   // Convert the string representation to an actual number array:
///   double[] array = Matrix.Parse(str, CSharpArrayFormatProvider.InvariantCulture);
///   
///   // array will now contain the actual number 
///   // array representation of the given string.
/// </code>
/// </example>
/// <seealso cref="Matrix" />
/// <seealso cref="CSharpMatrixFormatProvider" />
/// <seealso cref="CSharpJaggedMatrixFormatProvider" />
/// <seealso cref="CSharpArrayFormatProvider" />
/// <seealso cref="OctaveMatrixFormatProvider" />
/// <seealso cref="OctaveArrayFormatProvider" />
public sealed class CSharpArrayFormatProvider : MatrixFormatProviderBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CSharpMatrixFormatProvider" /> class.
    /// </summary>
    public CSharpArrayFormatProvider(IFormatProvider innerProvider,
        bool includeTypeName = true,
        bool includeSemicolon = true)
        : base(innerProvider)
    {
        FormatMatrixStart = includeTypeName ? "new double[] { " : "{ ";
        FormatMatrixEnd = includeSemicolon ? " };" : " }";
        FormatRowStart = "";
        FormatRowEnd = "";
        FormatColStart = "";
        FormatColEnd = "";
        FormatRowDelimiter = ", ";
        FormatColDelimiter = ", ";

        ParseMatrixStart = includeTypeName ? "new double[] {" : "{";
        ParseMatrixEnd = includeSemicolon ? "};" : "}";
        ParseRowStart = "";
        ParseRowEnd = "";
        ParseColStart = "";
        ParseColEnd = "";
        ParseRowDelimiter = "";
        ParseColDelimiter = ",";
    }

    /// <summary>
    ///     Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
    /// </summary>
    public static CSharpArrayFormatProvider CurrentCulture { get; } = new(CultureInfo.CurrentCulture);

    /// <summary>
    ///     Gets the IMatrixFormatProvider which uses the invariant system culture.
    /// </summary>
    public static CSharpArrayFormatProvider InvariantCulture { get; } = new(CultureInfo.InvariantCulture);
}
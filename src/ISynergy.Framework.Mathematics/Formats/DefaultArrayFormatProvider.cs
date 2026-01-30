using ISynergy.Framework.Mathematics.Formats.Base;
using ISynergy.Framework.Mathematics.Matrices;
using System.Globalization;

namespace ISynergy.Framework.Mathematics.Formats;

/// <summary>
///     Gets the default matrix representation, where each row
///     is separated by a new line, and columns are separated by spaces.
/// </summary>
/// <remarks>
///     This class can be used to convert to and from C#
///     arrays and their string representation. Please
///     see the example for details.
/// </remarks>
/// <example>
///     <para>
///         Converting from an array matrix to a
///         string representation:
///     </para>
///     <code>
///   // Declare a number array
///   double[] x = { 5, 6, 7, 8 };
///   
///   // Convert the aforementioned array to a string representation:
///   string str = x.ToString(DefaultArrayFormatProvider.CurrentCulture);
///   
///   // the final result will be equivalent to
///   "5, 6, 7, 8"
/// </code>
///     <para>
///         Converting from strings to actual matrices:
///     </para>
///     <code>
///   // Declare an input string
///   string str = "5, 6, 7, 8";
///   
///   // Convert the string representation to an actual number array:
///   double[] array = Matrix.Parse(str, DefaultArrayFormatProvider.InvariantCulture);
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
public sealed class DefaultArrayFormatProvider : MatrixFormatProviderBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultArrayFormatProvider" /> class.
    /// </summary>
    public DefaultArrayFormatProvider(IFormatProvider innerProvider)
        : base(innerProvider)
    {
        FormatMatrixStart = string.Empty;
        FormatMatrixEnd = string.Empty;
        FormatRowStart = string.Empty;
        FormatRowEnd = string.Empty;
        FormatColStart = string.Empty;
        FormatColEnd = string.Empty;
        FormatRowDelimiter = " ";
        FormatColDelimiter = string.Empty;

        ParseMatrixStart = string.Empty;
        ParseMatrixEnd = string.Empty;
        ParseRowStart = string.Empty;
        ParseRowEnd = string.Empty;
        ParseColStart = string.Empty;
        ParseColEnd = string.Empty;
        ParseRowDelimiter = " ";
        ParseColDelimiter = string.Empty;
    }

    /// <summary>
    ///     Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
    /// </summary>
    public static DefaultArrayFormatProvider CurrentCulture { get; } = new(CultureInfo.CurrentCulture);

    /// <summary>
    ///     Gets the IMatrixFormatProvider which uses the invariant system culture.
    /// </summary>
    public static DefaultArrayFormatProvider InvariantCulture { get; } = new(CultureInfo.InvariantCulture);
}
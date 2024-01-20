using ISynergy.Framework.Mathematics.Formats.Base;
using ISynergy.Framework.Mathematics.Matrices;
using System.Globalization;

namespace ISynergy.Framework.Mathematics.Formats;

/// <summary>
///   Format provider for the matrix format used by Octave.
/// </summary>
/// 
/// <remarks>
///   This class can be used to convert to and from C#
///   matrices and their string representation. Please 
///   see the example for details.
/// </remarks>
/// 
/// <remarks>
///   This class can be used to convert to and from C#
///   matrices and their string representation. Please 
///   see the example for details.
/// </remarks>
/// 
/// <example>
/// <para>
///   Converting from a multidimensional matrix to a 
///   string representation:</para>
///   
/// <code>
///   // Declare a number array
///   double[,] x = 
///   {
///      { 1, 2, 3, 4 },
///      { 5, 6, 7, 8 },
///   };
///   
///   // Convert the aforementioned array to a string representation:
///   string str = x.ToString(OctaveMatrixFormatProvider.CurrentCulture);
///   
///   // the final result will be equivalent to
///   "[ 1, 2, 3, 4; 5, 6, 7, 8 ]"
/// </code>
/// 
/// <para>
///   Converting from strings to actual matrices:</para>
/// 
/// <code>
///   // Declare an input string
///   string str = "[ 1, 2, 3, 4; 5, 6, 7, 8 ]";
///   
///   // Convert the string representation to an actual number array:
///   double[,] matrix = Matrix.Parse(str, OctaveMatrixFormatProvider.InvariantCulture);
///   
///   // matrix will now contain the actual multidimensional 
///   // matrix representation of the given string.
/// </code>
/// </example>
/// 
/// <seealso cref="Matrix"/>
/// <seealso cref="CSharpMatrixFormatProvider"/>
/// 
/// <seealso cref="CSharpJaggedMatrixFormatProvider"/>
/// <seealso cref="CSharpArrayFormatProvider"/>
/// 
/// <seealso cref="OctaveMatrixFormatProvider"/>
/// <seealso cref="OctaveArrayFormatProvider"/>
/// 
public sealed class OctaveMatrixFormatProvider : MatrixFormatProviderBase
{

    /// <summary>
    ///   Initializes a new instance of the <see cref="OctaveMatrixFormatProvider"/> class.
    /// </summary>
    /// 
    public OctaveMatrixFormatProvider(IFormatProvider innerProvider)
        : base(innerProvider)
    {
        FormatMatrixStart = "[";
        FormatMatrixEnd = "]";
        FormatRowStart = string.Empty;
        FormatRowEnd = string.Empty;
        FormatColStart = string.Empty;
        FormatColEnd = string.Empty;
        FormatRowDelimiter = "; ";
        FormatColDelimiter = " ";

        ParseMatrixStart = "[";
        ParseMatrixEnd = "]";
        ParseRowStart = string.Empty;
        ParseRowEnd = string.Empty;
        ParseColStart = string.Empty;
        ParseColEnd = string.Empty;
        ParseRowDelimiter = "; ";
        ParseColDelimiter = " ";
    }

    /// <summary>
    ///   Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
    /// </summary>
    /// 
    public static OctaveMatrixFormatProvider CurrentCulture
    {
        get { return currentCulture; }
    }

    /// <summary>
    ///   Gets the IMatrixFormatProvider which uses the invariant system culture.
    /// </summary>
    /// 
    public static OctaveMatrixFormatProvider InvariantCulture
    {
        get { return invariantCulture; }
    }
    private static readonly OctaveMatrixFormatProvider invariantCulture =
        new OctaveMatrixFormatProvider(CultureInfo.InvariantCulture);

    private static readonly OctaveMatrixFormatProvider currentCulture =
        new OctaveMatrixFormatProvider(CultureInfo.CurrentCulture);

}

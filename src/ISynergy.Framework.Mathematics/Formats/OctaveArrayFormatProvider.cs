namespace ISynergy.Framework.Mathematics.Formats;

using ISynergy.Framework.Mathematics.Formats.Base;
using ISynergy.Framework.Mathematics.Matrices;
using System;
using System.Globalization;

/// <summary>
///   Format provider for the matrix format used by Octave.
/// </summary>
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
///   string str = x.ToString(OctaveArrayFormatProvider.CurrentCulture);
///   
///   // the final result will be equivalent to
///   "[ 1, 2, 3, 4]"
/// </code>
/// 
/// <para>
///   Converting from strings to actual matrices:</para>
/// 
/// <code>
///   // Declare an input string
///   string str = "[ 1, 2, 3, 4]";
///   
///   // Convert the string representation to an actual number array:
///   double[] array = Matrix.Parse(str, OctaveArrayFormatProvider.InvariantCulture);
///   
///   // array will now contain the actual number 
///   // array representation of the given string.
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
public sealed class OctaveArrayFormatProvider : MatrixFormatProviderBase
{

    /// <summary>
    ///   Initializes a new instance of the <see cref="OctaveMatrixFormatProvider"/> class.
    /// </summary>
    /// 
    public OctaveArrayFormatProvider(IFormatProvider innerProvider)
        : base(innerProvider)
    {
        FormatMatrixStart = "[";
        FormatMatrixEnd = "]";
        FormatRowStart = string.Empty;
        FormatRowEnd = string.Empty;
        FormatColStart = string.Empty;
        FormatColEnd = string.Empty;
        FormatRowDelimiter = " ";
        FormatColDelimiter = " ";

        ParseMatrixStart = "[";
        ParseMatrixEnd = "]";
        ParseRowStart = string.Empty;
        ParseRowEnd = string.Empty;
        ParseColStart = string.Empty;
        ParseColEnd = string.Empty;
        ParseRowDelimiter = " ";
        ParseColDelimiter = " ";
    }

    /// <summary>
    ///   Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
    /// </summary>
    /// 
    public static OctaveArrayFormatProvider CurrentCulture
    {
        get { return currentCulture; }
    }

    /// <summary>
    ///   Gets the IMatrixFormatProvider which uses the invariant system culture.
    /// </summary>
    /// 
    public static OctaveArrayFormatProvider InvariantCulture
    {
        get { return invariantCulture; }
    }
    private static readonly OctaveArrayFormatProvider invariantCulture =
        new OctaveArrayFormatProvider(CultureInfo.InvariantCulture);

    private static readonly OctaveArrayFormatProvider currentCulture =
        new OctaveArrayFormatProvider(CultureInfo.CurrentCulture);

}

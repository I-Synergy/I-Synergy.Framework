
using ISynergy.Framework.Mathematics.Formats;
using ISynergy.Framework.Mathematics.Formats.Base;
namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Jagged
{

    #region Parse
    /// <summary>
    ///   Converts the string representation of a matrix to its
    ///   double-precision floating-point number matrix equivalent.
    /// </summary>
    /// <param name="str">The string representation of the matrix.</param>
    /// <returns>A double-precision floating-point number matrix parsed
    /// from the given string using the given format provider.</returns>
    /// 
    public static double[][] Parse(string str)
    {
        return MatrixFormatter.ParseJagged(str, DefaultMatrixFormatProvider.CurrentCulture);
    }

    /// <summary>
    ///   Converts the string representation of a matrix to its
    ///   double-precision floating-point number matrix equivalent.
    /// </summary>
    /// <param name="str">The string representation of the matrix.</param>
    /// <param name="provider">
    ///   The format provider to use in the conversion. Default is to use
    ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
    /// </param>
    /// <returns>A double-precision floating-point number matrix parsed
    /// from the given string using the given format provider.</returns>
    /// 
    public static double[][] Parse(string str, IMatrixFormatProvider provider)
    {
        return MatrixFormatter.ParseJagged(str, provider);
    }

    /// <summary>
    ///   Converts the string representation of a matrix to its
    ///   double-precision floating-point number matrix equivalent.
    ///   A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// <param name="s">The string representation of the matrix.</param>
    /// <param name="provider">
    ///   The format provider to use in the conversion. Default is to use
    ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
    /// </param>
    /// <param name="matrix">A double-precision floating-point number matrix parsed
    /// from the given string using the given format provider.</param>
    /// <result>When this method returns, contains the double-precision floating-point
    /// number matrix equivalent to the <see param="s"/> parameter, if the conversion succeeded, 
    /// or null if the conversion failed. The conversion fails if the <see param="s"/> parameter
    /// is null, is not a matrix in a valid format, or contains elements which represent
    /// a number less than MinValue or greater than MaxValue. This parameter is passed
    /// uninitialized. </result>
    /// 
    public static bool TryParse(string s, IMatrixFormatProvider provider, out double[][] matrix)
    {
        // TODO: Create a proper TryParse method without
        //       resorting to a underlying try-catch block.
        try
        {
            matrix = Parse(s, provider);
        }
        catch (FormatException)
        {
            matrix = null;
        }
        catch (ArgumentNullException)
        {
            matrix = null;
        }

        return matrix is not null;
    }

    #endregion

}

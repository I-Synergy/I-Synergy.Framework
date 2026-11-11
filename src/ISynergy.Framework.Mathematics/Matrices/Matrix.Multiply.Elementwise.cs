
using ISynergy.Framework.Mathematics.Abstractions;
namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Elementwise
{
    /// <summary>
    ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="c">The matrix <c>c</c>.</param>
    /// <param name="result">The matrix where the result should be stored. Pass the same
    ///   matrix as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] MultiplyAndAdd(this Jagged<double> a, double b, Jagged<double> c, Jagged<double> result)
    {
        var aArray = a.Array;
        var cArray = c.Array;
        var resultArray = result.Array;
        check<double, double, double, double>(a: aArray, b: b, c: cArray, result: resultArray);
        for (var i = 0; i < resultArray.Length; i++)
            for (var j = 0; j < resultArray[i].Length; j++)
                resultArray[i][j] = aArray[i][j] * b + cArray[i][j];

        return result;
    }

    /// <summary>
    ///   Multiplies a matrix <c>A</c> with a scalar <c>b</c> and accumulates with <c>c</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="c">The matrix <c>c</c>.</param>
    /// <param name="result">The matrix where the result should be stored. Pass the same
    ///   matrix as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] MultiplyAndAdd(this Matrix<double> a, double b, Matrix<double> c, Matrix<double> result)
    {
        var aArray = a.Array;
        var cArray = c.Array;
        var resultArray = result.Array;
        check<double, double, double, double>(a: aArray, b: b, c: cArray, result: resultArray);
        for (var i = 0; i < resultArray.GetLength(0); i++)
            for (var j = 0; j < resultArray.GetLength(1); j++)
                resultArray[i, j] = aArray[i, j] * b + cArray[i, j];

        return result;
    }
}

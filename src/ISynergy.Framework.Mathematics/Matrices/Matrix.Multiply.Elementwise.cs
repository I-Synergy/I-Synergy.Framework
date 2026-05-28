
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

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
    public static double[][] MultiplyAndAdd(this double[][] a, double b, double[][] c, double[][] result)
    {
        check<double, double, double, double>(a: a, b: b, c: c, result: result);
        for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (double)((double)(a[i][j]) * b + (double)(c[i][j]));

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
    public static double[,] MultiplyAndAdd(this double[,] a, double b, double[,] c, double[,] result)
    {
        check<double, double, double, double>(a: a, b: b, c: c, result: result);
        for (var i = 0; i < result.GetLength(0); i++)
            for (var j = 0; j < result.GetLength(1); j++)
                result[i, j] = (double)((double)(a[i, j]) * b + (double)(c[i, j]));

        return result;
    }
}

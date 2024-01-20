using ISynergy.Framework.Mathematics.Enumerations;
using ISynergy.Framework.Mathematics.Exceptions;
using System.Diagnostics;

namespace ISynergy.Framework.Mathematics.Matrices;

/// <summary>
///   Elementwise matrix and vector operations.
/// </summary>
/// 
/// <seealso cref="VectorType"/>
///
public static partial class Elementwise
{
    private static int rows<U>(U[] b)
    {
        return b.Length;
    }

    private static int cols<U>(U[][] b)
    {
        return b[0].Length;
    }

    private static int rows<U>(U[,] b)
    {
        return b.GetLength(0);
    }

    private static int cols<U>(U[,] b)
    {
        return b.GetLength(1);
    }
    [Conditional("DEBUG")]
    static void check<T, U>(T[] a, U[] b)
    {
        if (a.Length != b.Length)
            throw new DimensionMismatchException("b");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U[] b, V[] result)
    {
        if (a.Length != b.Length)
            throw new DimensionMismatchException("b");
        if (a.Length != result.Length)
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U b, V[] result)
    {
        if (a.Length != result.Length)
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T a, U[] b, V[] result)
    {
        if (b.Length != result.Length)
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U>(T[,] a, U[,] b)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException("b");
    }

    [Conditional("DEBUG")]
    static void check<T, U>(T[][] a, U[][] b)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException("b");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[,] a, U[,] b, V[,] result)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException("b");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[][] a, U[][] b, V[][] result)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException("b");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[,] a, U[] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[][] a, U[] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[] a, U[,] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[] a, U[][] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[,] a, U[] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[][] a, U[] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException("b");
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[] a, U[,] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[] a, U[][] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException("b");
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(T[,] a, U[] b, V[,] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException("b");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[][] a, U[] b, V[][] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException("b");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U[,] b, V[,] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException("b");
        if (rows(a) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U[][] b, V[][] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException("b");
        if (rows(a) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T a, U[,] b, V[,] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T a, U[][] b, V[][] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T a, U[,] b, V[,] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T a, U[][] b, V[][] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException("result");
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(T[][] a, U b, V[][] result)
    {
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[,] a, U b, V[,] result)
    {
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V, W>(T[][] a, U b, V[,] c, W[,] result)
    {
        if (rows(a) != rows(c) || cols(a) != cols(c))
            throw new DimensionMismatchException("c");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V, W>(T[][] a, U b, V[][] c, W[][] result)
    {
        if (rows(a) != rows(c) || cols(a) != cols(c))
            throw new DimensionMismatchException("c");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    [Conditional("DEBUG")]
    static void check<T, U, V, W>(T[,] a, U b, V[,] c, W[,] result)
    {
        if (rows(a) != rows(c) || cols(a) != cols(c))
            throw new DimensionMismatchException("c");
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException("result");
    }

    private static TOutput[] VectorCreateAs<TInput, TOutput>(TInput[] vector)
    {
        return new TOutput[vector.Length];
    }

    private static TOutput[,] MatrixCreateAs<TInput, TOutput>(TInput[,] matrix)
    {
        return new TOutput[matrix.GetLength(0), matrix.GetLength(1)];
    }

    private static TOutput[][] JaggedCreateAs<TInput, TOutput>(TInput[][] matrix)
    {
        var r = new TOutput[matrix.Length][];
        for (int i = 0; i < r.Length; i++)
            r[i] = new TOutput[matrix[i].Length];
        return r;
    }

    private static TOutput[,] MatrixCreateAs<TInput, TOutput>(TInput[][] matrix)
    {
        return new TOutput[matrix.Length, matrix[0].Length];
    }

    private static TOutput[][] JaggedCreateAs<TInput, TOutput>(TInput[,] matrix)
    {
        var r = new TOutput[matrix.GetLength(0)][];
        for (int i = 0; i < r.Length; i++)
            r[i] = new TOutput[matrix.GetLength(1)];
        return r;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[] Sqrt(this double[] value)
    {
        return Sqrt(value, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[,] Sqrt(this double[,] value)
    {
        return Sqrt(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[][] Sqrt(this double[][] value)
    {
        return Sqrt(value, JaggedCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[][] Sqrt(this double[][] value, double[][] result)
    {
        unsafe
        {
            for (int i = 0; i < value.Length; i++)
            {
                for (int j = 0; j < value[i].Length; j++)
                {
                    var v = value[i][j];
                    result[i][j] = (double)(Math.Sqrt((double)v));
                }
            }
        }
        return result;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Sqrt(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (int j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Sqrt((double)v));
                }
            }
        }

        return result;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Sqrt(this double[] value, double[] result)
    {
        for (int i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Sqrt((double)v));
        }

        return result;
    }


    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// 
    public static double[] Pow(this double[] value, double y)
    {
        return Pow(value, y, new double[value.Length]);
    }

    /// <summary>
    ///   Elementwise power.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="y">A power.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[] Pow(this double[] value, double y, double[] result)
    {
        for (int i = 0; i < value.Length; i++)
        {
            var v = value[i];
            result[i] = (double)(Math.Pow((double)Math.Abs(v), y));
        }

        return result;
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    /// 
    public static double[,] Abs(this double[,] value)
    {
        return Abs(value, MatrixCreateAs<double, double>(value));
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Abs(this double[,] value, double[,] result)
    {
        unsafe
        {
            fixed (double* ptrV = value)
            fixed (double* ptrR = result)
            {
                var pv = ptrV;
                var pr = ptrR;
                for (int j = 0; j < result.Length; j++, pv++, pr++)
                {
                    var v = *pv;
                    *pr = (double)(Math.Abs(v));
                }
            }
        }

        return result;
    }
}

using ISynergy.Framework.Mathematics.Abstractions;
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

    private static int rows<U>(U[,] b)
    {
        return b.GetLength(0);
    }

    private static int cols<U>(U[][] b)
    {
        return b[0].Length;
    }

    private static int cols<U>(U[,] b)
    {
        return b.GetLength(1);
    }
    [Conditional("DEBUG")]
    static void check<T, U>(T[] a, U[] b)
    {
        if (a.Length != b.Length)
            throw new DimensionMismatchException(nameof(b));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U[] b, V[] result)
    {
        if (a.Length != b.Length)
            throw new DimensionMismatchException(nameof(b));
        if (a.Length != result.Length)
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U b, V[] result)
    {
        if (a.Length != result.Length)
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T a, U[] b, V[] result)
    {
        if (b.Length != result.Length)
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U>(T[,] a, U[,] b)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException(nameof(b));
    }

    [Conditional("DEBUG")]
    static void check<T, U>(T[][] a, U[][] b)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException(nameof(b));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[,] a, U[,] b, V[,] result)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException(nameof(b));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[][] a, U[][] b, V[][] result)
    {
        if (rows(a) != rows(b) || cols(a) != cols(b))
            throw new DimensionMismatchException(nameof(b));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[,] a, U[] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[][] a, U[] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[] a, U[,] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T[] a, U[][] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[,] a, U[] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[][] a, U[] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(a) != rows(b))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[] a, U[,] b, V[,] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(VectorType d, T[] a, U[][] b, V[][] result)
    {
        if (d == 0)
        {
            if (cols(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        else
        {
            if (rows(b) != rows(a))
                throw new DimensionMismatchException(nameof(b));
        }
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(T[,] a, U[] b, V[,] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException(nameof(b));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[][] a, U[] b, V[][] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException(nameof(b));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U[,] b, V[,] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException(nameof(b));
        if (rows(a) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[] a, U[][] b, V[][] result)
    {
        if (rows(a) != rows(b))
            throw new DimensionMismatchException(nameof(b));
        if (rows(a) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T a, U[,] b, V[,] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(int d, T a, U[][] b, V[][] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T a, U[,] b, V[,] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T a, U[][] b, V[][] result)
    {
        if (rows(b) != rows(result) || cols(b) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }
    [Conditional("DEBUG")]
    static void check<T, U, V>(T[][] a, U b, V[][] result)
    {
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V>(T[,] a, U b, V[,] result)
    {
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V, W>(T[][] a, U b, V[,] c, W[,] result)
    {
        if (rows(a) != rows(c) || cols(a) != cols(c))
            throw new DimensionMismatchException(nameof(c));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V, W>(T[][] a, U b, V[][] c, W[][] result)
    {
        if (rows(a) != rows(c) || cols(a) != cols(c))
            throw new DimensionMismatchException(nameof(c));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
    }

    [Conditional("DEBUG")]
    static void check<T, U, V, W>(T[,] a, U b, V[,] c, W[,] result)
    {
        if (rows(a) != rows(c) || cols(a) != cols(c))
            throw new DimensionMismatchException(nameof(c));
        if (rows(a) != rows(result) || cols(a) != cols(result))
            throw new DimensionMismatchException(nameof(result));
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
    public static double[,] Sqrt(this Matrix<double> value)
    {
        return Sqrt(value, new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    /// 
    public static double[][] Sqrt(this Jagged<double> value)
    {
        return Sqrt(value, new Jagged<double>(JaggedCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[][] Sqrt(this Jagged<double> value, Jagged<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        for (int i = 0; i < valueArray.Length; i++)
        {
            for (int j = 0; j < valueArray[i].Length; j++)
            {
                var v = valueArray[i][j];
                resultArray[i][j] = Math.Sqrt(v);
            }
        }
        return resultArray;
    }

    /// <summary>
    ///   Elementwise square-root.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Sqrt(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        var rows = valueArray.GetLength(0);
        var cols = valueArray.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var v = valueArray[i, j];
                resultArray[i, j] = Math.Sqrt(v);
            }
        }

        return resultArray;
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
            result[i] = Math.Sqrt(v);
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
            result[i] = Math.Pow(Math.Abs(v), y);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    /// 
    public static double[,] Abs(this Matrix<double> value)
    {
        return Abs(value, new Matrix<double>(MatrixCreateAs<double, double>(value)));
    }

    /// <summary>
    ///   Elementwise absolute value.
    /// </summary>
    ///
    /// <param name="value">A matrix.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as <paramref name="value"/> to perform the operation in place.</param>
    /// 
    public static double[,] Abs(this Matrix<double> value, Matrix<double> result)
    {
        var valueArray = value.Array;
        var resultArray = result.Array;
        var rows = valueArray.GetLength(0);
        var cols = valueArray.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var v = valueArray[i, j];
                resultArray[i, j] = Math.Abs(v);
            }
        }

        return resultArray;
    }
}

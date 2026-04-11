using ISynergy.Framework.Mathematics.Enumerations;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Elementwise
{
    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// 
    public static double[,] Divide(this double[,] a, double b)
    {
        return Divide(a, b, new double[a.GetLength(0), a.GetLength(1)]);
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// 
    public static double[][] Divide(this double[][] a, double b)
    {
        return Divide(a, b, JaggedCreateAs<double, double>(a));
    }

    /// <summary>
    ///   Elementwise division between a vector <c>a</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// 
    public static double[] Divide(this double[] a, double b)
    {
        return Divide(a, b, new double[a.Length]);
    }

    /// <summary>
    ///   Elementwise division between a vector <c>a</c> and a vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// 
    public static double[] Divide(this double[] a, double[] b)
    {
        return Divide(a, b, new double[a.Length]);
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] Divide(this double[][] a, double[][] b)
    {
        return Divide(a, b, JaggedCreateAs<double, double>(a));
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] Divide(this double[,] a, double[,] b)
    {
        return Divide(a, b, MatrixCreateAs<double, double>(a));
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] Divide(this double a, double[,] b)
    {
        return Divide(a, b, MatrixCreateAs<double, double>(b));
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] Divide(this double a, double[][] b)
    {
        return Divide(a, b, JaggedCreateAs<double, double>(b));
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and a vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// 
    public static double[] Divide(this double a, double[] b)
    {
        return Divide(a, b, new double[b.Length]);
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    public static double[,] Divide(this double[,] a, double[] b, VectorType dimension)
    {
        return Divide(a, b, dimension, MatrixCreateAs<double, double>(a));
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    public static double[][] Divide(this double[][] a, double[] b, VectorType dimension)
    {
        return Divide(a, b, dimension, JaggedCreateAs<double, double>(a));
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    public static double[,] Divide(this double[] a, double[,] b, VectorType dimension)
    {
        return Divide(a, b, dimension, MatrixCreateAs<double, double>(b));
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    public static double[][] Divide(this double[] a, double[][] b, VectorType dimension)
    {
        return Divide(a, b, dimension, JaggedCreateAs<double, double>(b));
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] DivideByDiagonal(this double a, double[,] b)
    {
        return DivideByDiagonal(a, b, b.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] DivideByDiagonal(this double a, double[][] b)
    {
        return DivideByDiagonal(a, b, b.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] DivideByDiagonal(this double[] a, double[,] b)
    {
        return DivideByDiagonal(a, b, b.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] DivideByDiagonal(this double[] a, double[][] b)
    {
        return DivideByDiagonal(a, b, b.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] DivideByDiagonal(this double[,] a, double b)
    {
        return DivideByDiagonal(a, b, a.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] DivideByDiagonal(this double[][] a, double b)
    {
        return DivideByDiagonal(a, b, a.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] DivideByDiagonal(this double[,] a, double[] b)
    {
        return DivideByDiagonal(a, b, a.MemberwiseClone());
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] DivideByDiagonal(this double[][] a, double[] b)
    {
        return DivideByDiagonal(a, b, a.MemberwiseClone());
    }

    #region Matrix matrix

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Divide(this double[][] a, double[][] b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < result.Length; i++)
            for (var j = 0; j < result[i].Length; j++)
                result[i][j] = (double)((double)(a[i][j]) / (double)(b[i][j]));

        return result;
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Divide(this double[,] a, double[,] b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        var spanA = MemoryMarshal.CreateSpan(ref a[0, 0], a.Length);
        var spanB = MemoryMarshal.CreateSpan(ref b[0, 0], b.Length);
        var spanR = MemoryMarshal.CreateSpan(ref result[0, 0], result.Length);
        for (var i = 0; i < spanA.Length; i++)
            spanR[i] = (double)((double)spanA[i] / (double)spanB[i]);

        return result;
    }
    #endregion

    #region Matrix with scalar

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Divide(this double[][] a, double b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < a[i].Length; j++)
                result[i][j] = (double)((double)a[i][j] / (double)b);
        return result;
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Divide(this double a, double[][] b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < b.Length; i++)
            for (var j = 0; j < b[i].Length; j++)
                result[i][j] = (double)((double)a / (double)b[i][j]);
        return result;
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and a matrix <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Divide(this double a, double[,] b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        var spanB = MemoryMarshal.CreateSpan(ref b[0, 0], b.Length);
        var spanR = MemoryMarshal.CreateSpan(ref result[0, 0], result.Length);
        for (var j = 0; j < spanB.Length; j++)
            spanR[j] = (double)((double)a / (double)spanB[j]);

        return result;
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Divide(this double[,] a, double b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        var spanA = MemoryMarshal.CreateSpan(ref a[0, 0], a.Length);
        var spanR = MemoryMarshal.CreateSpan(ref result[0, 0], result.Length);
        for (var i = 0; i < spanA.Length; i++)
            spanR[i] = (double)((double)spanA[i] / (double)b);

        return result;
    }
    #endregion

    #region vector vector

    /// <summary>
    ///   Elementwise division between a vector <c>a</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[] Divide(this double[] a, double[] b, double[] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < a.Length; i++)
            result[i] = (double)((double)a[i] / (double)b[i]);
        return result;
    }
    #endregion

    #region Vector with scalar

    /// <summary>
    ///   Elementwise division between a vector <c>a</c> and a scalar <c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[] Divide(this double[] a, double b, double[] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < a.Length; i++)
            result[i] = (double)((double)a[i] / (double)b);
        return result;
    }

    /// <summary>
    ///   Elementwise division between a scalar <c>a</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[] Divide(this double a, double[] b, double[] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < b.Length; i++)
            result[i] = (double)((double)a / (double)b[i]);
        return result;
    }
    #endregion

    #region Matrix vector (enumeration)
    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    /// <param name="result">The matrix where the result should be stored. Pass the same
    ///   matrix as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Divide(this double[] a, double[,] b, VectorType dimension, double[,] result)
    {
        return Divide(b, a, dimension, result);
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    /// <param name="result">The matrix where the result should be stored. Pass the same
    ///   matrix as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Divide(this double[] a, double[][] b, VectorType dimension, double[][] result)
    {
        return Divide(b, a, dimension, result);
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    /// <param name="result">The matrix where the result should be stored. Pass the same
    ///   matrix as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Divide(this double[,] a, double[] b, VectorType dimension, double[,] result)
    {
        check<double, double, double>(d: dimension, a: a, b: b, result: result);
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);

        if (dimension == 0)
        {
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    result[i, j] = (double)((double)a[i, j] / (double)b[j]);
        }
        else
        {
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    result[i, j] = (double)((double)a[i, j] / (double)b[i]);
        }

        return result;
    }

    /// <summary>
    ///   Elementwise division between a matrix <c>A</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="dimension">
    ///   The type of the vector being passed to the function. If the vector
    ///   is a <see cref="VectorType.RowVector"/>, then the operation will
    ///   be applied between each row of the matrix and the given vector. If
    ///   the vector is a <see cref="VectorType.ColumnVector"/>, then the 
    ///   operation will be applied between each column of the matrix and the
    ///   given vector.
    /// </param>
    /// <param name="result">The matrix where the result should be stored. Pass the same
    ///   matrix as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Divide(this double[][] a, double[] b, VectorType dimension, double[][] result)
    {
        check<double, double, double>(d: dimension, a: a, b: b, result: result);
        if (dimension == 0)
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                    result[i][j] = (double)((double)a[i][j] / (double)b[j]);
        }
        else
        {
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                    result[i][j] = (double)((double)a[i][j] / (double)b[i]);
        }

        return result;
    }
    #endregion

    #region Diagonal
    public static double[,] DivideByDiagonal(this double a, double[,] b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        int rows = b.GetLength(0);
        int cols = b.GetLength(1);

        for (var j = 0; j < rows; j++)
            result[j, j] = (double)((double)a / (double)b[j, j]);
        return result;
    }

    public static double[][] DivideByDiagonal(this double a, double[][] b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < b.Length; i++)
            result[i][i] = (double)((double)a / (double)b[i][i]);
        return result;
    }

    public static double[,] DivideByDiagonal(this double[] a, double[,] b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        int rows = b.GetLength(0);
        int cols = b.GetLength(1);

        for (var j = 0; j < rows; j++)
            result[j, j] = (double)((double)a[j] / (double)b[j, j]);
        return result;
    }

    public static double[][] DivideByDiagonal(this double[] a, double[][] b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < b.Length; i++)
            result[i][i] = (double)((double)a[i] / (double)b[i][i]);
        return result;
    }

    public static double[,] DivideByDiagonal(this double[,] a, double b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);

        for (var j = 0; j < rows; j++)
            result[j, j] = (double)((double)a[j, j] / (double)b);
        return result;
    }

    public static double[][] DivideByDiagonal(this double[][] a, double b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < a.Length; i++)
            result[i][i] = (double)((double)a[i][i] / (double)b);
        return result;
    }

    public static double[,] DivideByDiagonal(this double[,] a, double[] b, double[,] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        int rows = b.GetLength(0);
        int cols = b.GetLength(1);

        for (var j = 0; j < rows; j++)
            result[j, j] = (double)((double)a[j, j] / (double)b[j]);
        return result;
    }

    public static double[][] DivideByDiagonal(this double[][] a, double[] b, double[][] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < b.Length; i++)
            result[i][i] = (double)((double)a[i][i] / (double)b[i]);
        return result;
    }

    #endregion
}

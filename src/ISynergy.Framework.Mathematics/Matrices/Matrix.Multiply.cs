using ISynergy.Framework.Mathematics.Enumerations;
using ISynergy.Framework.Mathematics.Abstractions;
using System.Runtime.InteropServices;



namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Elementwise
{
    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// 
    public static double[,] Multiply(this Matrix<double> a, double b)
    {
        var aArray = a.Array;
        return Multiply(a, b, new Matrix<double>(new double[aArray.GetLength(0), aArray.GetLength(1)]));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// 
    public static double[][] Multiply(this Jagged<double> a, double b)
    {
        return Multiply(a, b, new Jagged<double>(JaggedCreateAs<double, double>(a)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a vector <c>a</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// 
    public static double[] Multiply(this double[] a, double b)
    {
        return Multiply(a, b, new double[a.Length]);
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a vector <c>a</c> and a vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// 
    public static double[] Multiply(this double[] a, double[] b)
    {
        return Multiply(a, b, new double[a.Length]);
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] Multiply(this Jagged<double> a, Jagged<double> b)
    {
        return Multiply(a, b, new Jagged<double>(JaggedCreateAs<double, double>(a)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] Multiply(this Matrix<double> a, Matrix<double> b) // NOSONAR
    {
        return Multiply(a, b, new Matrix<double>(MatrixCreateAs<double, double>(a)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] Multiply(this double a, Matrix<double> b)
    {
        return Multiply(a, b, new Matrix<double>(MatrixCreateAs<double, double>(b)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] Multiply(this double a, Jagged<double> b)
    {
        return Multiply(a, b, new Jagged<double>(JaggedCreateAs<double, double>(b)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and a vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// 
    public static double[] Multiply(this double a, double[] b)
    {
        return Multiply(a, b, new double[b.Length]);
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[,] Multiply(this Matrix<double> a, double[] b, VectorType dimension)
    {
        return Multiply(a, b, dimension, new Matrix<double>(MatrixCreateAs<double, double>(a)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[][] Multiply(this Jagged<double> a, double[] b, VectorType dimension)
    {
        return Multiply(a, b, dimension, new Jagged<double>(JaggedCreateAs<double, double>(a)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[,] Multiply(this double[] a, Matrix<double> b, VectorType dimension)
    {
        return Multiply(a, b, dimension, new Matrix<double>(MatrixCreateAs<double, double>(b)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[][] Multiply(this double[] a, Jagged<double> b, VectorType dimension)
    {
        return Multiply(a, b, dimension, new Jagged<double>(JaggedCreateAs<double, double>(b)));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Multiply(this Jagged<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: bArray, result: resultArray);
        for (var i = 0; i < resultArray.Length; i++)
            for (var j = 0; j < resultArray[i].Length; j++)
                resultArray[i][j] = aArray[i][j] * bArray[i][j];

        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Multiply(this Matrix<double> a, Matrix<double> b, Matrix<double> result) // NOSONAR
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: bArray, result: resultArray);
        if (aArray.Length == 0)
            return resultArray;
        var spanA = MemoryMarshal.CreateSpan(ref aArray[0, 0], aArray.Length);
        var spanB = MemoryMarshal.CreateSpan(ref bArray[0, 0], bArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var i = 0; i < spanA.Length; i++)
            spanR[i] = spanA[i] * spanB[i];

        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Multiply(this Jagged<double> a, double b, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: b, result: resultArray);
        for (var i = 0; i < aArray.Length; i++)
            for (var j = 0; j < aArray[i].Length; j++)
                resultArray[i][j] = aArray[i][j] * b;
        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[][] Multiply(this double a, Jagged<double> b, Jagged<double> result)
    {
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: a, b: bArray, result: resultArray);
        for (var i = 0; i < bArray.Length; i++)
            for (var j = 0; j < bArray[i].Length; j++)
                resultArray[i][j] = a * bArray[i][j];
        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and a matrix <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Multiply(this double a, Matrix<double> b, Matrix<double> result)
    {
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: a, b: bArray, result: resultArray);
        if (bArray.Length == 0)
            return resultArray;
        var spanB = MemoryMarshal.CreateSpan(ref bArray[0, 0], bArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var j = 0; j < spanB.Length; j++)
            spanR[j] = a * spanB[j];

        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a scalar <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The matrix <c>A</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[,] Multiply(this Matrix<double> a, double b, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: b, result: resultArray);
        if (aArray.Length == 0)
            return resultArray;
        var spanA = MemoryMarshal.CreateSpan(ref aArray[0, 0], aArray.Length);
        var spanR = MemoryMarshal.CreateSpan(ref resultArray[0, 0], resultArray.Length);
        for (var i = 0; i < spanA.Length; i++)
            spanR[i] = spanA[i] * b;

        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a vector <c>a</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[] Multiply(this double[] a, double[] b, double[] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < a.Length; i++)
            result[i] = a[i] * b[i];
        return result;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a vector <c>a</c> and a scalar <c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The vector <c>a</c>.</param>
    /// <param name="b">The scalar <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[] Multiply(this double[] a, double b, double[] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < a.Length; i++)
            result[i] = a[i] * b;
        return result;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and a vector<c>b</c>.
    /// </summary>
    ///
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The vector <c>b</c>.</param>
    /// <param name="result">The vector where the result should be stored. Pass the same
    ///   vector as one of the arguments to perform the operation in place.</param>
    /// 
    public static double[] Multiply(this double a, double[] b, double[] result)
    {
        check<double, double, double>(a: a, b: b, result: result);
        for (var i = 0; i < b.Length; i++)
            result[i] = a * b[i];
        return result;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[,] Multiply(this double[] a, Matrix<double> b, VectorType dimension, Matrix<double> result)
    {
        return Multiply(b, a, dimension, result);
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[][] Multiply(this double[] a, Jagged<double> b, VectorType dimension, Jagged<double> result)
    {
        return Multiply(b, a, dimension, result);
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[,] Multiply(this Matrix<double> a, double[] b, VectorType dimension, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(d: dimension, a: aArray, b: b, result: resultArray);
        int rows = aArray.GetLength(0);
        int cols = aArray.GetLength(1);

        if (dimension == 0)
        {
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    resultArray[i, j] = aArray[i, j] * b[j];
        }
        else
        {
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    resultArray[i, j] = aArray[i, j] * b[i];
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a matrix <c>A</c> and a vector<c>b</c>.
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
    public static double[][] Multiply(this Jagged<double> a, double[] b, VectorType dimension, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(d: dimension, a: aArray, b: b, result: resultArray);
        if (dimension == 0)
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                    resultArray[i][j] = aArray[i][j] * b[j];
        }
        else
        {
            for (var i = 0; i < aArray.Length; i++)
                for (var j = 0; j < aArray[i].Length; j++)
                    resultArray[i][j] = aArray[i][j] * b[i];
        }

        return resultArray;
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] MultiplyWithDiagonal(this double a, Matrix<double> b)
    {
        return MultiplyWithDiagonal(a, b, new Matrix<double>(b.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] MultiplyWithDiagonal(this double a, Jagged<double> b)
    {
        return MultiplyWithDiagonal(a, b, new Jagged<double>(b.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] MultiplyWithDiagonal(this double[] a, Matrix<double> b)
    {
        return MultiplyWithDiagonal(a, b, new Matrix<double>(b.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] MultiplyWithDiagonal(this double[] a, Jagged<double> b)
    {
        return MultiplyWithDiagonal(a, b, new Jagged<double>(b.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] MultiplyWithDiagonal(this Matrix<double> a, double b)
    {
        return MultiplyWithDiagonal(a, b, new Matrix<double>(a.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] MultiplyWithDiagonal(this Jagged<double> a, double b)
    {
        return MultiplyWithDiagonal(a, b, new Jagged<double>(a.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[,] MultiplyWithDiagonal(this Matrix<double> a, double[] b)
    {
        return MultiplyWithDiagonal(a, b, new Matrix<double>(a.Array.MemberwiseClone()));
    }

    /// <summary>
    ///   Elementwise multiplication (note: this is not a dot or matrix product) between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The scalar <c>a</c>.</param>
    /// <param name="b">The matrix <c>B</c>.</param>
    /// 
    public static double[][] MultiplyWithDiagonal(this Jagged<double> a, double[] b)
    {
        return MultiplyWithDiagonal(a, b, new Jagged<double>(a.Array.MemberwiseClone()));
    }

    public static double[,] MultiplyWithDiagonal(this double a, Matrix<double> b, Matrix<double> result)
    {
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: a, b: bArray, result: resultArray);
        int rows = bArray.GetLength(0);

        for (var j = 0; j < rows; j++)
            resultArray[j, j] = a * bArray[j, j];
        return resultArray;
    }

    public static double[][] MultiplyWithDiagonal(this double a, Jagged<double> b, Jagged<double> result)
    {
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: a, b: bArray, result: resultArray);
        for (var i = 0; i < bArray.Length; i++)
            resultArray[i][i] = a * bArray[i][i];
        return resultArray;
    }

    public static double[,] MultiplyWithDiagonal(this double[] a, Matrix<double> b, Matrix<double> result)
    {
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: a, b: bArray, result: resultArray);
        int rows = bArray.GetLength(0);

        for (var j = 0; j < rows; j++)
            resultArray[j, j] = a[j] * bArray[j, j];
        return resultArray;
    }

    public static double[][] MultiplyWithDiagonal(this double[] a, Jagged<double> b, Jagged<double> result)
    {
        var bArray = b.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: a, b: bArray, result: resultArray);
        for (var i = 0; i < bArray.Length; i++)
            resultArray[i][i] = a[i] * bArray[i][i];
        return resultArray;
    }

    public static double[,] MultiplyWithDiagonal(this Matrix<double> a, double b, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: b, result: resultArray);
        int rows = aArray.GetLength(0);

        for (var j = 0; j < rows; j++)
            resultArray[j, j] = aArray[j, j] * b;
        return resultArray;
    }

    public static double[][] MultiplyWithDiagonal(this Jagged<double> a, double b, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: b, result: resultArray);
        for (var i = 0; i < aArray.Length; i++)
            resultArray[i][i] = aArray[i][i] * b;
        return resultArray;
    }

    public static double[,] MultiplyWithDiagonal(this Matrix<double> a, double[] b, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: b, result: resultArray);
        int rows = b.Length;

        for (var j = 0; j < rows; j++)
            resultArray[j, j] = aArray[j, j] * b[j];
        return resultArray;
    }

    public static double[][] MultiplyWithDiagonal(this Jagged<double> a, double[] b, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        check<double, double, double>(a: aArray, b: b, result: resultArray);
        for (var i = 0; i < b.Length; i++)
            resultArray[i][i] = aArray[i][i] * b[i];
        return resultArray;
    }

}

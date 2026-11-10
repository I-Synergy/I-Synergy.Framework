
using ISynergy.Framework.Mathematics.Exceptions;
using ISynergy.Framework.Mathematics.Abstractions;
namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{
    /// <summary>
    ///   Gets the inner product (scalar product) between two vectors (a'*b).
    /// </summary>
    /// 
    /// <param name="a">A vector.</param>
    /// <param name="b">A vector.</param>
    /// 
    /// <returns>The inner product of the multiplication of the vectors.</returns>
    /// 
    /// <remarks>
    ///  <para>
    ///    In mathematics, the dot product is an algebraic operation that takes two
    ///    equal-length sequences of numbers (usually coordinate vectors) and returns
    ///    a single number obtained by multiplying corresponding entries and adding up
    ///    those products. The name is derived from the dot that is often used to designate
    ///    this operation; the alternative name scalar product emphasizes the scalar
    ///    (rather than vector) nature of the result.</para>
    ///  <para>
    ///    The principal use of this product is the inner product in a Euclidean vector space:
    ///    when two vectors are expressed on an orthonormal basis, the dot product of their 
    ///    coordinate vectors gives their inner product.</para>  
    /// </remarks>
    /// 
    public static double Dot(this double[] a, double[] b)
    {
        double r = 0;
        for (var i = 0; i < a.Length; i++)
            r += (a[i] * b[i]);
        return r;
    }

    /// <summary>
    ///   Computes the product <c>A*b</c> of a matrix <c>A</c> and a column vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="columnVector">The right vector <c>b</c>.</param>
    ///
    /// <returns>The product <c>A*b</c> of the given matrix <c>A</c> and vector <c>b</c>.</returns>
    /// 
    public static double[] Dot(this Jagged<double> a, double[] columnVector)
    {
        return Dot(a, columnVector, new double[a.Rows]);
    }

    /// <summary>
    ///   Computes the product <c>A*b</c> of a matrix <c>A</c> and a column vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="columnVector">The right vector <c>b</c>.</param>
    ///
    /// <returns>The product <c>A*b</c> of the given matrix <c>A</c> and vector <c>b</c>.</returns>
    /// 
    public static double[] Dot(this Matrix<double> a, double[] columnVector)
    {
        return Dot(a, columnVector, new double[a.Rows]);
    }

    /// <summary>
    ///   Computes the product <c>a*B</c> of a row vector <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left vector <c>a</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>a*B</c> of the given vector <c>a</c> and <c>B</c>.</returns>
    /// 
    public static double[] Dot(this double[] rowVector, Jagged<double> b)
    {
        return Dot(rowVector, b, new double[b.Array.Columns()]);
    }

    /// <summary>
    ///   Computes the product <c>a*B</c> of a row vector <c>a</c> and a matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left vector <c>a</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>a*B</c> of the given vector <c>a</c> and <c>B</c>.</returns>
    /// 
    public static double[] Dot(this double[] rowVector, Matrix<double> b)
    {
        return Dot(rowVector, b, new double[b.Columns]);
    }

    /// <summary>
    ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] Dot(this Jagged<double> a, Jagged<double> b)
    {
        return Dot(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Array.Columns())));
    }

    /// <summary>
    ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] Dot(this Matrix<double> a, Matrix<double> b)
    {
        return Dot(a, b, new Matrix<double>(new double[a.Rows, b.Columns]));
    }

    /// <summary>
    ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] Dot(this Jagged<double> a, Matrix<double> b)
    {
        return Dot(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Columns)));
    }

    /// <summary>
    ///   Computes the product <c>A*B</c> of two matrices <c>A</c> and <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] Dot(this Matrix<double> a, Jagged<double> b)
    {
        return Dot(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Array.Columns())));
    }
    #region dot

    /// <summary>
    ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
    ///   and <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] Dot(this Matrix<double> a, Matrix<double> b, Matrix<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int N = resultArray.Rows();
        int K = aArray.Columns();
        int M = resultArray.Columns();
#if DEBUG
        if (aArray.Columns() != bArray.Rows() || resultArray.Rows() > aArray.Rows() || resultArray.Columns() > bArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");

        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        for (var i = 0; i < N; i++)
            for (var j = 0; j < M; j++)
                for (var k = 0; k < K; k++)
                    C[i, j] += (aArray[i, k] * bArray[k, j]);
#endif
        var t = new double[K];

        for (var j = 0; j < M; j++)
        {
            for (var k = 0; k < K; k++)
                t[k] = bArray[k, j];

            for (var i = 0; i < N; i++)
            {
                double s = 0;
                for (var k = 0; k < K; k++)
                    s += aArray[i, k] * t[k];
                resultArray[i, j] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
    ///   and <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[][] Dot(this Jagged<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Length || resultArray.Length > aArray.Length || resultArray.Columns() > bArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Jagged.CreateAs(new Jagged<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(new Jagged<double>(aArray).ToMatrix().To<double[,]>()), new Matrix<double>(new Jagged<double>(bArray).ToMatrix().To<double[,]>()), new Matrix<double>(C));
#endif
        int K = aArray.Columns();
        int M = resultArray.Columns();

        var t = new double[K];

        for (var j = 0; j < M; j++)
        {
            for (var k = 0; k < bArray.Length; k++)
                t[k] = bArray[k][j];

            for (var i = 0; i < aArray.Length; i++)
            {
                double s = 0;
                for (var k = 0; k < t.Length; k++)
                    s += (aArray[i][k] * t[k]);
                resultArray[i][j] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(new Jagged<double>(resultArray).ToMatrix().To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
    ///   and <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[][] Dot(this Jagged<double> a, Matrix<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Rows() || resultArray.Length > aArray.Length || resultArray.Columns() > bArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(new Jagged<double>(aArray).ToMatrix().To<double[,]>()), new Matrix<double>(bArray.To<double[,]>()), new Matrix<double>(C));
#endif
        int K = aArray.Columns();
        int M = resultArray.Columns();
        var t = new double[K];

        for (var j = 0; j < M; j++)
        {
            for (var k = 0; k < K; k++)
                t[k] = bArray[k, j];

            for (var i = 0; i < aArray.Length; i++)
            {
                double s = 0;
                for (var k = 0; k < aArray[i].Length; k++)
                    s += aArray[i][k] * t[k];
                resultArray[i][j] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(new Jagged<double>(resultArray).ToMatrix().To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
    ///   and <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[][] Dot(this Matrix<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Length || resultArray.Length > aArray.Length || resultArray.Columns() > bArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(new Jagged<double>(bArray).ToMatrix().To<double[,]>()), new Matrix<double>(C));
#endif
        int N = resultArray.Length;
        int K = aArray.Columns();
        int M = resultArray.Columns();

        var t = new double[K];

        for (var j = 0; j < M; j++)
        {
            for (var k = 0; k < t.Length; k++)
                t[k] = bArray[k][j];

            for (var i = 0; i < N; i++)
            {
                double s = 0;
                for (var k = 0; k < t.Length; k++)
                    s += aArray[i, k] * t[k];
                resultArray[i][j] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(new Jagged<double>(resultArray).ToMatrix().To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Multiplies a row vector <c>v</c> and a matrix <c>A</c>,
    ///   giving the product <c>v'*A</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The row vector <c>v</c>.</param>
    /// <param name="matrix">The matrix <c>A</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[] Dot(this double[] rowVector, Matrix<double> matrix, double[] result)
    {
        var matrixArray = matrix.Array;
#if DEBUG
        if (rowVector.Length != matrixArray.Rows() || result.Length > matrixArray.Columns())
            throw new DimensionMismatchException(nameof(rowVector), "Matrices must have compatible dimensions.");
#endif
        int cols = matrixArray.Columns();
        for (var j = 0; j < cols; j++)
        {
            double s = 0;
            for (var k = 0; k < rowVector.Length; k++)
                s += (rowVector[k] * matrixArray[k, j]);
            result[j] = s;
        }
        return result;
    }

    /// <summary>
    ///   Multiplies a matrix <c>A</c> and a column vector <c>v</c>,
    ///   giving the product <c>A*v</c>
    /// </summary>
    /// 
    /// <param name="matrix">The matrix <c>A</c>.</param>
    /// <param name="columnVector">The column vector <c>v</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[] Dot(this Jagged<double> matrix, double[] columnVector, double[] result)
    {
        var matrixArray = matrix.Array;
#if DEBUG
        if (matrixArray.Columns() != columnVector.Length || result.Length > matrixArray.Length)
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
#endif

        for (var i = 0; i < matrixArray.Length; i++)
        {
            double s = 0;
            for (var j = 0; j < columnVector.Length; j++)
                s += (matrixArray[i][j] * columnVector[j]);
            result[i] = s;
        }
        return result;
    }

    /// <summary>
    ///   Multiplies a matrix <c>A</c> and a column vector <c>v</c>,
    ///   giving the product <c>A*v</c>
    /// </summary>
    /// 
    /// <param name="matrix">The matrix <c>A</c>.</param>
    /// <param name="columnVector">The column vector <c>v</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[] Dot(this Matrix<double> matrix, double[] columnVector, double[] result)
    {
        var matrixArray = matrix.Array;
        int cols = matrixArray.Columns();
        int rows = matrixArray.Rows();

#if DEBUG
        if (cols != columnVector.Length || result.Length > rows)
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
#endif
        // Process rows two at a time
        for (var i = 0; i < rows / 2; i++)
        {
            double sum1 = 0, sum2 = 0;

            for (var j = 0; j < cols; j++)
            {
                sum1 += matrixArray[2 * i, j] * columnVector[j];
                sum2 += matrixArray[2 * i + 1, j] * columnVector[j];
            }

            result[2 * i] = sum1;
            result[2 * i + 1] = sum2;
        }

        // Process the remainder
        for (var i = (rows / 2) * 2; i < rows; i++)
        {
            double sum = 0;

            for (var j = 0; j < cols; j++)
                sum += matrixArray[i, j] * columnVector[j];

            result[i] = sum;
        }

        return result;
    }

    /// <summary>
    ///   Computes the product <c>R = A*B</c> of two matrices <c>A</c>
    ///   and <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left matrix <c>A</c>.</param>
    /// <param name="matrix">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product.</param>
    /// 
    public static double[] Dot(this double[] rowVector, Jagged<double> matrix, double[] result)
    {
        var matrixArray = matrix.Array;
#if DEBUG
        if (rowVector.Length != matrixArray.Length || result.Length > matrixArray.Columns())
            throw new DimensionMismatchException(nameof(rowVector), "Matrices must have compatible dimensions.");
#endif
        for (var j = 0; j < result.Length; j++)
        {
            double s = 0;
            for (var k = 0; k < rowVector.Length; k++)
                s += (rowVector[k] * matrixArray[k][j]);
            result[j] = s;
        }

        return result;
    }

    #endregion

    /// <summary>
    ///   Computes the product <c>a*B*c</c> of a row vector <c>a</c>, 
    ///   a square matrix <c>B</c> and a column vector <c>c</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left vector <c>a</c>.</param>
    /// <param name="matrix">The square matrix <c>B</c>.</param>
    /// <param name="columnVector">The column vector <c>c</c>.</param>
    ///
    /// <returns>The product <c>a*B*c</c> of the given vector <c>a</c>,
    ///   matrix <c>B</c> and vector <c>c</c>.</returns>
    /// 
    public static double DotAndDot(this double[] rowVector, Jagged<double> matrix, double[] columnVector)
    {
        var matrixArray = matrix.Array;
#if DEBUG
        if (rowVector.Length != matrixArray.Rows() || matrixArray.Columns() != columnVector.Length)
            throw new DimensionMismatchException("Matrices must have compatible dimensions.");
#endif
        double sum = 0;

        for (var i = 0; i < rowVector.Length; i++)
        {
            double s = 0;
            for (var j = 0; j < columnVector.Length; j++)
                s += (matrixArray[i][j] * columnVector[j]);
            sum += (rowVector[i] * s);
        }

        return sum;
    }

    /// <summary>
    ///   Computes the product <c>a*B*c</c> of a row vector <c>a</c>, 
    ///   a square matrix <c>B</c> and a column vector <c>c</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left vector <c>a</c>.</param>
    /// <param name="matrix">The square matrix <c>B</c>.</param>
    /// <param name="columnVector">The column vector <c>c</c>.</param>
    ///
    /// <returns>The product <c>a*B*c</c> of the given vector <c>a</c>,
    ///   matrix <c>B</c> and vector <c>c</c>.</returns>
    /// 
    public static double DotAndDot(this double[] rowVector, Matrix<double> matrix, double[] columnVector)
    {
        var matrixArray = matrix.Array;
        int cols = matrixArray.Columns();
        int rows = matrixArray.Rows();

        // Validate dimensions in all build configurations: the loop below reads rows in
        // pairs, so the guard must not be DEBUG-only.
        if (rowVector.Length != rows || cols != columnVector.Length)
            throw new DimensionMismatchException(nameof(rowVector), "Matrices must have compatible dimensions.");
        double result = 0;

        // Process rows two at a time
        for (var i = 0; i < rows / 2; i++)
        {
            double sum1 = 0, sum2 = 0;

            for (var j = 0; j < cols; j++)
            {
                sum1 += matrixArray[2 * i, j] * columnVector[j];
                sum2 += matrixArray[2 * i + 1, j] * columnVector[j];
            }

            result += rowVector[2 * i] * sum1;
            result += rowVector[2 * i + 1] * sum2;
        }

        // Process the remainder
        for (var i = (rows / 2) * 2; i < rows; i++)
        {
            double sum = 0;

            for (var j = 0; j < cols; j++)
                sum += matrixArray[i, j] * columnVector[j];

            result += rowVector[i] * sum;
        }

        return result;
    }

    #region dot with transposed

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[,] DotWithTransposed(this Matrix<double> a, Matrix<double> b, Matrix<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Columns() || resultArray.Rows() > aArray.Rows() || resultArray.Columns() > bArray.Rows())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(bArray.Transpose().To<double[,]>()), new Matrix<double>(C));
#endif
        int n = aArray.Columns();
        int m = aArray.Rows();
        int p = bArray.Rows();

        for (var i = 0; i < m; i++)
            for (var j = 0; j < p; j++)
            {
                double s = 0;
                for (var k = 0; k < n; k++)
                    s += aArray[i, k] * bArray[j, k];
                resultArray[i, j] = s;
            }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] DotWithTransposed(this Matrix<double> a, Matrix<double> b)
    {
        return DotWithTransposed(a, b, new Matrix<double>(new double[a.Rows, b.Rows]));
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DotWithTransposed(this Jagged<double> a, Jagged<double> b)
    {
        return DotWithTransposed(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Rows)));
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[][] DotWithTransposed(this Jagged<double> a, Matrix<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Columns() || resultArray.Rows() > aArray.Rows() || resultArray.Columns() > bArray.Rows())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(bArray.Transpose().To<double[,]>()), new Matrix<double>(C));
#endif
        int n = bArray.Rows();

        for (var i = 0; i < aArray.Length; i++)
        {
            double[] arow = aArray[i];
            for (var j = 0; j < n; j++)
            {
                double sum = 0;
                for (var k = 0; k < arow.Length; k++)
                    sum += arow[k] * bArray[j, k];
                resultArray[i][j] = sum;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }
    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///   
    public static double[][] DotWithTransposed(this Matrix<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Columns() || resultArray.Rows() > aArray.Rows() || resultArray.Columns() > bArray.Rows())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(bArray.Transpose().To<double[,]>()), new Matrix<double>(C));
#endif
        int n = aArray.Rows();

        for (var j = 0; j < bArray.Length; j++)
        {
            double[] brow = bArray[j];
            for (var i = 0; i < n; i++)
            {
                double sum = 0;
                for (var k = 0; k < brow.Length; k++)
                    sum += aArray[i, k] * brow[k];
                resultArray[i][j] = sum;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }
    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[][] DotWithTransposed(this Jagged<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Columns() != bArray.Columns() || resultArray.Rows() > aArray.Rows() || resultArray.Columns() > bArray.Rows())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Matrix.CreateAs(new Matrix<double>(resultArray)).To<double[,]>();
        C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(bArray.Transpose().To<double[,]>()), new Matrix<double>(C));
#endif
        for (var i = 0; i < aArray.Length; i++)
        {
            double[] arow = aArray[i];
            for (var j = 0; j < bArray.Length; j++)
            {
                double sum = 0;
                double[] brow = bArray[j];
                for (var k = 0; k < arow.Length; k++)
                    sum += (arow[k] * brow[k]);
                resultArray[i][j] = sum;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[] DotWithTransposed(this double[] rowVector, Matrix<double> b, double[] result)
    {
        return rowVector.Dot(new Matrix<double>(b.Array.Transpose()), result);
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="columnVector">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[,] DotWithTransposed(this Matrix<double> a, double[] columnVector, Matrix<double> result)
    {
        return Dot(a, new Matrix<double>(columnVector.Transpose()), result);
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[] DotWithTransposed(this double[] rowVector, Jagged<double> b, double[] result)
    {
        return rowVector.Dot(new Jagged<double>(b.Array.Transpose()), result);
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and
    ///   transpose of <c>B</c>, storing the result in matrix <c>R</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="columnVector">The transposed right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B'</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    ///    
    public static double[][] DotWithTransposed(this Jagged<double> a, double[] columnVector, Jagged<double> result)
    {
        return Dot(a, new Jagged<double>(columnVector.Transpose(out _)), result);
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DotWithTransposed(this Jagged<double> a, Matrix<double> b)
    {
        return DotWithTransposed(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Rows)));
    }

    /// <summary>
    ///   Computes the product <c>A*B'</c> of matrix <c>A</c> and transpose of <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The transposed right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B'</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DotWithTransposed(this Matrix<double> a, Jagged<double> b)
    {
        return DotWithTransposed(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Rows)));
    }

    #endregion

    #region transpose and dot
    /// <summary>
    ///   Computes the product <c>A'*B</c> of transposed of matrix <c>A</c> and <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A'*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] TransposeAndDot(this Matrix<double> a, Matrix<double> b)
    {
        return TransposeAndDot(a, b, new Matrix<double>(Matrix.Create<double>(a.Columns, b.Columns)));
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] TransposeAndDot(this Matrix<double> a, Matrix<double> b, Matrix<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int n = aArray.Rows();
        int m = aArray.Columns();
        int p = bArray.Columns();

#if DEBUG
        if (n != bArray.Rows() || resultArray.Rows() > m || resultArray.Columns() > p)
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Dot(new Matrix<double>(aArray.Transpose().To<double[,]>()), new Matrix<double>(bArray.To<double[,]>()));
#endif
        var ptemp = new double[p];
        for (var i = 0; i < m; i++)
        {
            for (var k = 0; k < n; k++)
            {
                double aval = aArray[k, i];
                for (var j = 0; j < p; j++)
                    ptemp[j] += aval * bArray[k, j];
            }

            // Update the results row and clear the cache
            for (var j = 0; j < p; j++)
            {
                resultArray[i, j] = ptemp[j];
                ptemp[j] = 0;
            }
        }

#if DEBUG
        if (!C.IsEqual(new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif

        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] TransposeAndDot(this Matrix<int> a, Matrix<double> b, Matrix<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int n = aArray.Rows();
        int m = aArray.Columns();
        int p = bArray.Columns();

#if DEBUG
        if (n != bArray.Rows() || resultArray.Rows() > m || resultArray.Columns() > p)
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Dot(new Matrix<double>(aArray.Transpose().To<double[,]>()), new Matrix<double>(bArray.To<double[,]>()));
#endif
        var ptemp = new double[p];
        for (var i = 0; i < m; i++)
        {
            for (var k = 0; k < n; k++)
            {
                int aval = aArray[k, i];
                for (var j = 0; j < p; j++)
                    ptemp[j] += aval * bArray[k, j];
            }

            // Update the results row and clear the cache
            for (var j = 0; j < p; j++)
            {
                resultArray[i, j] = ptemp[j];
                ptemp[j] = 0;
            }
        }

#if DEBUG
        if (!C.IsEqual(new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif

        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDot(this Jagged<int> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Length != bArray.Length || resultArray.Length > aArray.Columns() || resultArray.Columns() > bArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Dot(new Matrix<double>(aArray.Transpose().To<double[,]>()), new Matrix<double>(bArray.To<double[,]>()));
#endif
        int n = aArray.Length;
        int m = aArray.Columns();
        int p = bArray.Columns();

        var Bcolj = new double[n];
        for (var i = 0; i < p; i++)
        {
            for (var k = 0; k < bArray.Length; k++)
                Bcolj[k] = bArray[k][i];

            for (var j = 0; j < m; j++)
            {
                double s = 0;
                for (var k = 0; k < Bcolj.Length; k++)
                    s += (aArray[k][j] * Bcolj[k]);
                resultArray[j][i] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }
    /// <summary>
    ///   Computes the product <c>A'*b</c> of matrix <c>A</c> transposed and column vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="matrix">The transposed left matrix <c>A</c>.</param>
    /// <param name="columnVector">The right column vector <c>b</c>.</param>
    /// <param name="result">The vector <c>r</c> to store the product <c>r = A'*b</c>
    ///   of the given matrix <c>A</c> and vector <c>b</c>.</param>
    /// 
    public static double[] TransposeAndDot(this Matrix<int> matrix, double[] columnVector, double[] result)
    {
        var matrixArray = matrix.Array;
#if DEBUG
        if (matrixArray.Rows() != columnVector.Length || result.Length > matrixArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Dot(new Matrix<double>(matrixArray.Transpose().To<double[,]>()), columnVector.To<double[]>());
#endif
        int cols = matrixArray.Columns();
        for (var j = 0; j < cols; j++)
        {
            double s = 0;
            for (var k = 0; k < columnVector.Length; k++)
                s += (matrixArray[k, j] * columnVector[k]);
            result[j] = s;
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[]>(), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return result;
    }

    /// <summary>
    ///   Computes the product <c>A'*b</c> of matrix <c>A</c> transposed and column vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="matrix">The transposed left matrix <c>A</c>.</param>
    /// <param name="columnVector">The right column vector <c>b</c>.</param>
    /// <param name="result">The vector <c>r</c> to store the product <c>r = A'*b</c>
    ///   of the given matrix <c>A</c> and vector <c>b</c>.</param>
    /// 
    public static double[] TransposeAndDot(this Jagged<int> matrix, double[] columnVector, double[] result)
    {
        var matrixArray = matrix.Array;
#if DEBUG
        if (matrixArray.Length != columnVector.Length || result.Length > matrixArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Dot(new Matrix<double>(matrixArray.Transpose().To<double[,]>()), columnVector.To<double[]>());
#endif
        int cols = matrixArray.Columns();
        for (var j = 0; j < cols; j++)
        {
            double s = 0;
            for (var k = 0; k < columnVector.Length; k++)
                s += (matrixArray[k][j] * columnVector[k]);
            result[j] = s;
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[]>(), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return result;
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDot(this Jagged<double> a, Matrix<double> b, Jagged<double> result)
    {
        return Dot(new Jagged<double>(a.Array.Transpose()), b, result);
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of transposed of matrix <c>A</c> and <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A'*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] TransposeAndDot(this Jagged<double> a, Jagged<double> b)
    {
        return TransposeAndDot(a, b, new Jagged<double>(Jagged.Create<double>(a.Array.Columns(), b.Array.Columns())));
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDot(this Jagged<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
#if DEBUG
        if (aArray.Length != bArray.Length || resultArray.Length > aArray.Columns() || resultArray.Columns() > bArray.Columns())
            throw new DimensionMismatchException("result", "Matrices must have compatible dimensions.");
        var C = Dot(new Matrix<double>(aArray.Transpose().To<double[,]>()), new Matrix<double>(bArray.To<double[,]>()));
#endif
        int n = aArray.Length;
        int m = aArray.Columns();
        int p = bArray.Columns();

        var Bcolj = new double[n];
        for (int i = 0; i < p; i++)
        {
            for (int k = 0; k < bArray.Length; k++)
                Bcolj[k] = bArray[k][i];

            for (int j = 0; j < m; j++)
            {
                double s = (double)0;
                for (int k = 0; k < Bcolj.Length; k++)
                    s += (aArray[k][j] * Bcolj[k]);
                resultArray[j][i] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDot(this Matrix<double> a, Jagged<double> b, Jagged<double> result)
    {
        return Dot(new Matrix<double>(a.Array.Transpose()), b, result);
    }
    /// <summary>
    ///   Computes the product <c>A'*b</c> of matrix <c>A</c> transposed and column vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right column vector <c>b</c>.</param>
    /// <param name="result">The vector <c>r</c> to store the product <c>r = A'*b</c>
    ///   of the given matrix <c>A</c> and vector <c>b</c>.</param>
    /// 
    public static double[,] TransposeAndDot(this double[] rowVector, Matrix<double> b, Matrix<double> result)
    {
        return Dot(new Matrix<double>(rowVector.Transpose()), b, result);
    }

    /// <summary>
    ///   Computes the product <c>A'*b</c> of matrix <c>A</c> transposed and column vector <c>b</c>.
    /// </summary>
    /// 
    /// <param name="rowVector">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right column vector <c>b</c>.</param>
    /// <param name="result">The vector <c>r</c> to store the product <c>r = A'*b</c>
    ///   of the given matrix <c>A</c> and vector <c>b</c>.</param>
    /// 
    public static double[][] TransposeAndDot(this double[] rowVector, Jagged<double> b, Jagged<double> result)
    {
        return Dot(new Jagged<double>(rowVector.Transpose(out _)), b, result);
    }
    #endregion

    #region diagonal
    /// <summary>
    ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] TransposeAndDotWithDiagonal(this Matrix<double> a, double[] diagonal, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
#if DEBUG
        var C = Dot(new Matrix<double>(aArray.Transpose().To<double[,]>()), new Matrix<double>(Matrix.Diagonal(diagonal.To<double[]>())));
#endif
        int m = aArray.Columns();
        for (var i = 0; i < diagonal.Length; i++)
            for (var j = 0; j < m; j++)
                resultArray[j, i] = (aArray[i, j] * diagonal[i]);
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDotWithDiagonal(this Jagged<double> a, double[] diagonal, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
#if DEBUG
        var C = Dot(new Matrix<double>(aArray.Transpose().To<double[,]>()), new Matrix<double>(Matrix.Diagonal(diagonal.To<double[]>())));
#endif
        int m = aArray.Columns();
        for (var i = 0; i < diagonal.Length; i++)
            for (var j = 0; j < m; j++)
                resultArray[j][i] = (aArray[i][j] * diagonal[i]);
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    /// 
    public static double[,] TransposeAndDotWithDiagonal(this Matrix<double> a, double[] b)
    {
        return TransposeAndDotWithDiagonal(a, b, new Matrix<double>(Matrix.Create<double>(a.Columns, b.Length)));
    }

    /// <summary>
    ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDotWithDiagonal(this Jagged<double> a, double[] b)
    {
        return TransposeAndDotWithDiagonal(a, b, new Jagged<double>(Jagged.Create<double>(a.Array.Columns(), b.Length)));
    }

    /// <summary>
    ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] DotWithDiagonal(this Matrix<double> a, double[] b)
    {
        return DotWithDiagonal(a, b, new Matrix<double>(Matrix.Create<double>(a.Rows, b.Length)));
    }

    /// <summary>
    ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DotWithDiagonal(this Jagged<double> a, double[] b)
    {
        return DotWithDiagonal(a, b, new Jagged<double>(Jagged.Create<double>(a.Rows, b.Length)));
    }

    /// <summary>
    ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] DotWithDiagonal(this Matrix<double> a, double[] diagonal, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
#if DEBUG
        var C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(Matrix.Diagonal(diagonal.To<double[]>())));
#endif
        int rows = aArray.Rows();

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < diagonal.Length; j++)
                resultArray[i, j] = aArray[i, j] * diagonal[j];
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] DotWithDiagonal(this Jagged<double> a, double[] diagonal, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
#if DEBUG
        var C = Dot(new Matrix<double>(aArray.To<double[,]>()), new Matrix<double>(Matrix.Diagonal(diagonal.To<double[]>())));
#endif
        int rows = aArray.Length;
        for (var i = 0; i < rows; i++)
            for (var j = 0; j < diagonal.Length; j++)
                resultArray[i][j] = (aArray[i][j] * diagonal[j]);
#if DEBUG
        if (!Matrix.IsEqual(C, new Matrix<double>(resultArray.To<double[,]>()), 1e-4))
            throw new InvalidOperationException("Matrix computation result mismatch in debug validation.");
#endif
        return resultArray;
    }

    /// <summary>
    ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of inverse right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] DivideByDiagonal(this Matrix<double> a, double[] diagonal, Matrix<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        int rows = aArray.Rows();

        for (var i = 0; i < rows; i++)
            for (var j = 0; j < diagonal.Length; j++)
                resultArray[i, j] = aArray[i, j] / diagonal[j];

        return resultArray;
    }

    /// <summary>
    ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="diagonal">The diagonal vector of inverse right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[][] DivideByDiagonal(this Jagged<double> a, double[] diagonal, Jagged<double> result)
    {
        var aArray = a.Array;
        var resultArray = result.Array;
        for (var i = 0; i < aArray.Length; i++)
            for (var j = 0; j < diagonal.Length; j++)
                resultArray[i][j] = ((aArray[i][j]) / diagonal[j]);

        return resultArray;
    }

    /// <summary>
    ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of inverse right matrix <c>B</c>.</param>
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] DivideByDiagonal(this Matrix<double> a, double[] b)
    {
        return DivideByDiagonal(a, b, new Matrix<double>(Matrix.CreateAs<double, double>(a)));
    }

    /// <summary>
    ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of inverse right matrix <c>B</c>.</param>
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DivideByDiagonal(this Jagged<double> a, double[] b)
    {
        return DivideByDiagonal(a, b, new Jagged<double>(Jagged.CreateAs<double, double>(a)));
    }

    #endregion

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[,] Outer(this double[] a, double[] b, Matrix<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
        {
            double x = a[i];
            for (var j = 0; j < b.Length; j++)
                resultArray[i, j] = x * b[j];
        }

        return resultArray;
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[][] Outer(this double[] a, double[] b, Jagged<double> result)
    {
        var resultArray = result.Array;
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                resultArray[i][j] = (a[i] * b[j]);
        return resultArray;
    }

    /// <summary>
    ///   Gets the outer product (matrix product) between two vectors (a*bT).
    /// </summary>
    /// 
    /// <remarks>
    ///   In linear algebra, the outer product typically refers to the tensor
    ///   product of two vectors. The result of applying the outer product to
    ///   a pair of vectors is a matrix. The name contrasts with the inner product,
    ///   which takes as input a pair of vectors and produces a scalar.
    /// </remarks>
    /// 
    public static double[,] Outer(this double[] a, double[] b)
    {
        return Outer(a, b, new Matrix<double>(new double[a.Length, b.Length]));
    }

    /// <summary>
    ///   Vector product.
    /// </summary>
    /// 
    /// <remarks>
    ///   The cross product, vector product or Gibbs vector product is a binary operation
    ///   on two vectors in three-dimensional space. It has a vector result, a vector which
    ///   is always perpendicular to both of the vectors being multiplied and the plane
    ///   containing them. It has many applications in mathematics, engineering and physics.
    /// </remarks>
    /// 
    public static double[] Cross(this double[] a, double[] b, double[] result)
    {
        result[0] = (a[1] * b[2] - a[2] * b[1]);
        result[1] = (a[2] * b[0] - a[0] * b[2]);
        result[2] = (a[0] * b[1] - a[1] * b[0]);

        return result;
    }

    #region Kronecker

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// <param name="result">The matrix <c>R</c> to store the 
    ///   Kronecker product between matrices <c>A</c> and <c>B</c>.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[,] Kronecker(this Matrix<double> a, Matrix<double> b, Matrix<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int arows = aArray.Rows();
        int acols = aArray.Columns();

        int brows = bArray.Rows();
        int bcols = bArray.Columns();

        for (var i = 0; i < arows; i++)
            for (var j = 0; j < acols; j++)
            {
                double aval = aArray[i, j];
                for (var k = 0; k < brows; k++)
                    for (var l = 0; l < bcols; l++)
                        resultArray[i * brows + k, j * bcols + l] = aval * bArray[k, l];
            }

        return resultArray;
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// <param name="result">The matrix <c>R</c> to store the 
    ///   Kronecker product between matrices <c>A</c> and <c>B</c>.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[][] Kronecker(this Jagged<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int arows = aArray.Rows();
        int acols = aArray.Columns();
        int brows = bArray.Rows();
        int bcols = bArray.Columns();

        for (var i = 0; i < arows; i++)
            for (var j = 0; j < acols; j++)
            {
                double aval = aArray[i][j];
                for (var k = 0; k < brows; k++)
                {
                    double[] brow = bArray[k];
                    for (var l = 0; l < bcols; l++)
                        resultArray[i * brows + k][j * bcols + l] = (aval * brow[l]);
                }
            }

        return resultArray;
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// <param name="result">The matrix <c>R</c> to store the 
    ///   Kronecker product between matrices <c>A</c> and <c>B</c>.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[][] Kronecker(this Jagged<double> a, Matrix<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int arows = aArray.Rows();
        int acols = aArray.Columns();
        int brows = bArray.Rows();
        int bcols = bArray.Columns();

        for (var i = 0; i < arows; i++)
            for (var j = 0; j < acols; j++)
            {
                double aval = aArray[i][j];
                for (var k = 0; k < brows; k++)
                    for (var l = 0; l < bcols; l++)
                        resultArray[i * brows + k][j * bcols + l] = aval * bArray[k, l];
            }

        return resultArray;
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// <param name="result">The matrix <c>R</c> to store the 
    ///   Kronecker product between matrices <c>A</c> and <c>B</c>.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[][] Kronecker(this Matrix<double> a, Jagged<double> b, Jagged<double> result)
    {
        var aArray = a.Array;
        var bArray = b.Array;
        var resultArray = result.Array;
        int arows = aArray.Rows();
        int acols = aArray.Columns();
        int brows = bArray.Rows();
        int bcols = bArray.Columns();

        for (var i = 0; i < arows; i++)
            for (var j = 0; j < acols; j++)
            {
                double aval = aArray[i, j];
                for (var k = 0; k < brows; k++)
                {
                    double[] brow = bArray[k];
                    for (var l = 0; l < bcols; l++)
                        resultArray[i * brows + k][j * bcols + l] = (aval * brow[l]);
                }
            }

        return resultArray;
    }

    /// <summary>
    ///   Computes the Kronecker product between two vectors.
    /// </summary>
    /// 
    /// <param name="a">The left vector a.</param>
    /// <param name="b">The right vector b.</param>
    /// <param name="result">The matrix <c>R</c> to store the 
    ///   Kronecker product between matrices <c>A</c> and <c>B</c>.</param>
    /// 
    /// <returns>The Kronecker product of the two vectors.</returns>
    /// 
    public static double[] Kronecker(this double[] a, double[] b, double[] result)
    {
        for (var i = 0; i < a.Length; i++)
        {
            double x = a[i];
            for (var j = 0; j < b.Length; j++)
                result[i * b.Length + j] = x * b[j];
        }

        return result;
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[,] Kronecker(this Matrix<double> a, Matrix<double> b)
    {
        int crows = a.Rows * b.Rows;
        int ccols = a.Columns * b.Columns;
        return Kronecker(a, b, new Matrix<double>(new double[crows, ccols]));
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[][] Kronecker(this Matrix<double> a, Jagged<double> b)
    {
        int crows = a.Rows * b.Rows;
        int ccols = a.Columns * b.Array.Columns();
        return Kronecker(a, b, new Jagged<double>(Jagged.Create<double>(crows, ccols)));
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[][] Kronecker(this Jagged<double> a, Matrix<double> b)
    {
        int crows = a.Rows * b.Rows;
        int ccols = a.Array.Columns() * b.Columns;
        return Kronecker(a, b, new Jagged<double>(Jagged.Create<double>(crows, ccols)));
    }

    /// <summary>
    ///   Computes the Kronecker product between two matrices.
    /// </summary>
    /// 
    /// <param name="a">The left matrix a.</param>
    /// <param name="b">The right matrix b.</param>
    /// 
    /// <returns>The Kronecker product of the two matrices.</returns>
    /// 
    public static double[][] Kronecker(this Jagged<double> a, Jagged<double> b)
    {
        int crows = a.Rows * b.Rows;
        int ccols = a.Array.Columns() * b.Array.Columns();
        return Kronecker(a, b, new Jagged<double>(Jagged.Create<double>(crows, ccols)));
    }

    /// <summary>
    ///   Computes the Kronecker product between two vectors.
    /// </summary>
    /// 
    /// <param name="a">The left vector a.</param>
    /// <param name="b">The right vector b.</param>
    /// 
    /// <returns>The Kronecker product of the two vectors.</returns>
    /// 
    public static double[] Kronecker(this double[] a, double[] b)
    {
        return Kronecker(a, b, new double[a.Length * b.Length]);
    }

    #endregion

}
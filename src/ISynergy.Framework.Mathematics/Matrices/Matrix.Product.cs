
using ISynergy.Framework.Mathematics.Exceptions;
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
    public static double[] Dot(this double[][] a, double[] columnVector)
    {
        return Dot(a, columnVector, new double[a.Length]);
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
    public static double[] Dot(this double[,] a, double[] columnVector)
    {
        return Dot(a, columnVector, new double[a.Rows()]);
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
    public static double[] Dot(this double[] rowVector, double[][] b)
    {
        return Dot(rowVector, b, new double[b.Columns()]);
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
    public static double[] Dot(this double[] rowVector, double[,] b)
    {
        return Dot(rowVector, b, new double[b.Columns()]);
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
    public static double[][] Dot(this double[][] a, double[][] b)
    {
        return Dot(a, b, Jagged.Create<double>(a.Length, b.Columns()));
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
    public static double[,] Dot(this double[,] a, double[,] b)
    {
        return Dot(a, b, new double[a.Rows(), b.Columns()]);
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
    public static double[][] Dot(this double[][] a, double[,] b)
    {
        return Dot(a, b, Jagged.Create<double>(a.Length, b.Columns()));
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
    public static double[][] Dot(this double[,] a, double[][] b)
    {
        return Dot(a, b, Jagged.Create<double>(a.Rows(), b.Columns()));
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
    public static double DotAndDot(this double[] rowVector, double[][] matrix, double[] columnVector)
    {
#if DEBUG
        if (rowVector.Length != matrix.Rows() || matrix.Columns() != columnVector.Length)
            throw new DimensionMismatchException();
#endif
        double sum = 0;

        for (var i = 0; i < rowVector.Length; i++)
        {
            double s = 0;
            for (var j = 0; j < columnVector.Length; j++)
                s += (matrix[i][j] * columnVector[j]);
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
    public static double DotAndDot(this double[] rowVector, double[,] matrix, double[] columnVector)
    {
        int cols = matrix.Columns();
        int rows = matrix.Rows();
#if DEBUG
        if (rowVector.Length != rows || cols != columnVector.Length)
            throw new DimensionMismatchException();
#endif
        double result = 0;

        unsafe
        {
            fixed (double* r = rowVector)
            fixed (double* a = matrix)
            fixed (double* c = columnVector)
            {
                double* pa1 = a;
                double* pa2 = a + cols;
                double* pr = r;

                // Process rows two at a time
                for (var i = 0; i < rows / 2; i++)
                {
                    double sum1 = 0, sum2 = 0;
                    double* pc = c;

                    for (var j = 0; j < cols; j++)
                    {
                        sum1 += ((*pa1++) * (*pc));
                        sum2 += ((*pa2++) * (*pc));
                        pc++;
                    }

                    result += ((*pr++) * sum1);
                    result += ((*pr++) * sum2);

                    // Now we skip a row
                    pa1 = pa2;
                    pa2 += cols;
                }

                // Process the remainder
                for (var i = 0; i < rows % 2; i++)
                {
                    double sum = 0;
                    double* pc = c;

                    for (var j = 0; j < cols; j++)
                        sum += ((*pa1++) * (*pc++));

                    result += ((*pr++) * sum);
                }
            }
        }

        return result;
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
    public static double[,] Dot(this double[,] a, double[,] b, double[,] result)
    {
        int N = result.Rows();
        int K = a.Columns();
        int M = result.Columns();
        int stride = b.Columns();
#if DEBUG
        if (a.Columns() != b.Rows() || result.Rows() > a.Rows() || result.Columns() > b.Columns())
            throw new DimensionMismatchException();

        var C = Matrix.CreateAs(result).To<double[,]>();
        for (var i = 0; i < N; i++)
            for (var j = 0; j < M; j++)
                for (var k = 0; k < K; k++)
                    C[i, j] += (a[i, k] * b[k, j]);
#endif
        var t = new double[K];

        unsafe
        {
            fixed (double* A = a)
            fixed (double* B = b)
            fixed (double* T = t)
            fixed (double* R = result)
            {
                for (var j = 0; j < M; j++)
                {
                    double* pb = B + j;
                    for (var k = 0; k < K; k++)
                    {
                        T[k] = *pb;
                        pb += stride;
                    }

                    double* pa = A;
                    double* pr = R + j;
                    for (var i = 0; i < N; i++)
                    {
                        double s = 0;
                        for (var k = 0; k < K; k++)
                            s += (pa[k] * T[k]);
                        *pr = s;
                        pa += K;
                        pr += M;
                    }
                }
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] Dot(this double[][] a, double[][] b, double[][] result)
    {
#if DEBUG
        if (a.Columns() != b.Length || result.Length > a.Length || result.Columns() > b.Columns())
            throw new DimensionMismatchException();
        var C = Jagged.CreateAs(result).To<double[,]>();
        C = Dot(a.ToMatrix().To<double[,]>(), b.ToMatrix().To<double[,]>(), C);
#endif
        int N = result.Length;
        int K = a.Columns();
        int M = result.Columns();

        var t = new double[K];

        for (var j = 0; j < M; j++)
        {
            for (var k = 0; k < b.Length; k++)
                t[k] = b[k][j];

            for (var i = 0; i < a.Length; i++)
            {
                double s = 0;
                for (var k = 0; k < t.Length; k++)
                    s += (a[i][k] * t[k]);
                result[i][j] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.ToMatrix().To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] Dot(this double[][] a, double[,] b, double[][] result)
    {
#if DEBUG
        if (a.Columns() != b.Rows() || result.Length > a.Length || result.Columns() > b.Columns())
            throw new DimensionMismatchException();
        var C = Matrix.CreateAs(result).To<double[,]>();
        C = Dot(a.ToMatrix().To<double[,]>(), b.To<double[,]>(), C);
#endif
        int N = result.Length;
        int K = a.Columns();
        int M = result.Columns();
        int stride = b.Columns();
        var t = new double[K];

        unsafe
        {
            fixed (double* B = b)
            fixed (double* T = t)
            {
                for (var j = 0; j < M; j++)
                {
                    double* pb = B + j;
                    for (var k = 0; k < K; k++)
                    {
                        T[k] = *pb;
                        pb += stride;
                    }

                    for (var i = 0; i < a.Length; i++)
                    {
                        double s = 0;
                        for (var k = 0; k < a[i].Length; k++)
                            s += (a[i][k] * T[k]);
                        result[i][j] = s;
                    }
                }
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.ToMatrix().To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] Dot(this double[,] a, double[][] b, double[][] result)
    {
#if DEBUG
        if (a.Columns() != b.Length || result.Length > a.Length || result.Columns() > b.Columns())
            throw new DimensionMismatchException();
        var C = Matrix.CreateAs(result).To<double[,]>();
        C = Dot(a.To<double[,]>(), b.ToMatrix().To<double[,]>(), C);
#endif
        int N = result.Length;
        int K = a.Columns();
        int M = result.Columns();

        var t = new double[K];

        unsafe
        {
            fixed (double* A = a)
                for (var j = 0; j < M; j++)
                {
                    for (var k = 0; k < t.Length; k++)
                        t[k] = b[k][j];

                    double* pa = A;
                    for (var i = 0; i < N; i++)
                    {
                        double s = 0;
                        for (var k = 0; k < t.Length; k++)
                            s += ((*pa++) * t[k]);
                        result[i][j] = s;
                    }
                }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.ToMatrix().To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[] Dot(this double[] rowVector, double[,] matrix, double[] result)
    {
#if DEBUG
        if (rowVector.Length != matrix.Rows() || result.Length > matrix.Columns())
            throw new DimensionMismatchException();
#endif
        int cols = matrix.Columns();
        for (var j = 0; j < cols; j++)
        {
            double s = 0;
            for (var k = 0; k < rowVector.Length; k++)
                s += (rowVector[k] * matrix[k, j]);
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
    public static double[] Dot(this double[][] matrix, double[] columnVector, double[] result)
    {
#if DEBUG
        if (matrix.Columns() != columnVector.Length || result.Length > matrix.Length)
            throw new DimensionMismatchException();
#endif

        for (var i = 0; i < matrix.Length; i++)
        {
            double s = 0;
            for (var j = 0; j < columnVector.Length; j++)
                s += (matrix[i][j] * columnVector[j]);
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
    public static double[] Dot(this double[,] matrix, double[] columnVector, double[] result)
    {
        int cols = matrix.Columns();
        int rows = matrix.Rows();

#if DEBUG
        if (cols != columnVector.Length || result.Length > rows)
            throw new DimensionMismatchException();
#endif
        unsafe
        {
            fixed (double* a = matrix)
            fixed (double* x = columnVector)
            fixed (double* r = result)
            {
                double* pa1 = a;
                double* pa2 = a + cols;
                double* pr = r;

                // Process rows two at a time
                for (var i = 0; i < rows / 2; i++)
                {
                    double sum1 = 0, sum2 = 0;
                    double* px = x;

                    for (var j = 0; j < cols; j++)
                    {
                        sum1 += ((*pa1++) * (*px));
                        sum2 += ((*pa2++) * (*px));
                        px++;
                    }

                    *pr++ = sum1;
                    *pr++ = sum2;

                    // Now we skip a row
                    pa1 = pa2;
                    pa2 += cols;
                }

                // Process the remainder
                for (var i = 0; i < rows % 2; i++)
                {
                    double sum = 0;
                    double* px = x;

                    for (var j = 0; j < cols; j++)
                        sum += ((*pa1++) * (*px++));

                    *pr = sum;
                }
            }
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
    public static double[] Dot(this double[] rowVector, double[][] matrix, double[] result)
    {
#if DEBUG
        if (rowVector.Length != matrix.Length || result.Length > matrix.Columns())
            throw new DimensionMismatchException();
#endif
        for (var j = 0; j < result.Length; j++)
        {
            double s = 0;
            for (var k = 0; k < rowVector.Length; k++)
                s += (rowVector[k] * matrix[k][j]);
            result[j] = s;
        }

        return result;
    }

    #endregion

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
    public static double[,] DotWithTransposed(this double[,] a, double[,] b, double[,] result)
    {
#if DEBUG
        if (a.Columns() != b.Columns() || result.Rows() > a.Rows() || result.Columns() > b.Rows())
            throw new DimensionMismatchException();
        var C = Matrix.CreateAs(result).To<double[,]>();
        C = Dot(a.To<double[,]>(), b.Transpose().To<double[,]>(), C);
#endif
        int n = a.Columns();
        int m = a.Rows();
        int p = b.Rows();

        unsafe
        {
            fixed (double* A = a)
            fixed (double* B = b)
            fixed (double* R = result)
            {
                double* pr = R;
                for (var i = 0; i < m; i++)
                {
                    double* pb = B;
                    for (var j = 0; j < p; j++, pr++)
                    {
                        double* pa = A + n * i;

                        double s = 0;
                        for (var k = 0; k < n; k++)
                            s += ((*pa++) * (*pb++));
                        *pr = s;
                    }
                }
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[,] DotWithTransposed(this double[,] a, double[,] b)
    {
        return DotWithTransposed(a, b, new double[a.Rows(), b.Rows()]);
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
    public static double[,] TransposeAndDot(this double[,] a, double[,] b)
    {
        return TransposeAndDot(a, b, Matrix.Create<double>(a.Columns(), b.Columns()));
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
    public static double[,] TransposeAndDot(this double[,] a, double[,] b, double[,] result)
    {
        int n = a.Rows();
        int m = a.Columns();
        int p = b.Columns();

#if DEBUG
        if (n != b.Rows() || result.Rows() > m || result.Columns() > p)
            throw new DimensionMismatchException();
        var C = a.Transpose().To<double[,]>().Dot(b.To<double[,]>());
#endif
        unsafe
        {
            fixed (double* R = result)
            fixed (double* B = b)
            fixed (double* ptemp = new double[p])
            {
                double* pr = R;

                for (int i = 0; i < m; i++)
                {
                    double* pt = ptemp;
                    double* pb = B;

                    for (int k = 0; k < n; k++)
                    {
                        double aval = a[k, i];
                        for (int j = 0; j < p; j++)
                            *pt++ += (double)((double)aval * (double)(*pb++));
                        pt = ptemp;
                    }

                    // Update the results row and clear the cache
                    for (int j = 0; j < p; j++)
                    {
                        *pr++ = (double)*pt;
                        *pt++ = 0;
                    }
                }
            }
        }

#if DEBUG
        if (!C.IsEqual(result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif

        return result;
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
    public static double[][] DotWithTransposed(this double[][] a, double[][] b)
    {
        return DotWithTransposed(a, b, Jagged.Create<double>(a.Length, b.Length));
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
    public static double[][] DotWithTransposed(this double[][] a, double[,] b, double[][] result)
    {
#if DEBUG
        if (a.Columns() != b.Columns() || result.Rows() > a.Rows() || result.Columns() > b.Rows())
            throw new DimensionMismatchException();
        var C = Matrix.CreateAs(result).To<double[,]>();
        C = Dot(a.To<double[,]>(), b.Transpose().To<double[,]>(), C);
#endif
        int n = b.Rows();

        unsafe
        {
            fixed (double* B = b)
                for (var i = 0; i < a.Length; i++)
                {
                    double* pb = B;
                    double[] arow = a[i];
                    for (var j = 0; j < n; j++)
                    {
                        double sum = 0;
                        for (var k = 0; k < arow.Length; k++)
                            sum += (arow[k] * (*pb++));
                        result[i][j] = sum;
                    }
                }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] DotWithTransposed(this double[,] a, double[][] b, double[][] result)
    {
#if DEBUG
        if (a.Columns() != b.Columns() || result.Rows() > a.Rows() || result.Columns() > b.Rows())
            throw new DimensionMismatchException();
        var C = Matrix.CreateAs(result).To<double[,]>();
        C = Dot(a.To<double[,]>(), b.Transpose().To<double[,]>(), C);
#endif
        int n = a.Rows();

        unsafe
        {
            fixed (double* A = a)
                for (var j = 0; j < b.Length; j++)
                {
                    double* pa = A;
                    for (var i = 0; i < n; i++)
                    {
                        double sum = 0;
                        double[] brow = b[j];
                        for (var k = 0; k < brow.Length; k++)
                            sum += ((*pa++) * brow[k]);
                        result[i][j] = sum;
                    }
                }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] DotWithTransposed(this double[][] a, double[][] b, double[][] result)
    {
#if DEBUG
        if (a.Columns() != b.Columns() || result.Rows() > a.Rows() || result.Columns() > b.Rows())
            throw new DimensionMismatchException();
        var C = Matrix.CreateAs(result).To<double[,]>();
        C = Dot(a.To<double[,]>(), b.Transpose().To<double[,]>(), C);
#endif
        for (var i = 0; i < a.Length; i++)
        {
            double[] arow = a[i];
            for (var j = 0; j < b.Length; j++)
            {
                double sum = 0;
                double[] brow = b[j];
                for (var k = 0; k < arow.Length; k++)
                    sum += (arow[k] * brow[k]);
                result[i][j] = sum;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[] DotWithTransposed(this double[] rowVector, double[,] b, double[] result)
    {
        return rowVector.Dot(b.Transpose(), result);
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
    public static double[,] DotWithTransposed(this double[,] a, double[] columnVector, double[,] result)
    {
        return a.Dot(columnVector.Transpose(), result);
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
    public static double[] DotWithTransposed(this double[] rowVector, double[][] b, double[] result)
    {
        return rowVector.Dot(b.Transpose(), result);
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
    public static double[][] DotWithTransposed(this double[][] a, double[] columnVector, double[][] result)
    {
        double[][] t;
        return a.Dot(columnVector.Transpose(out t), result);
    }

    #endregion

    #region transpose and dot
    /// <summary>
    ///   Computes the product <c>A'*B</c> of matrix <c>A</c> transposed and matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The transposed left matrix <c>A</c>.</param>
    /// <param name="b">The right matrix <c>B</c>.</param>
    /// <param name="result">The matrix <c>R</c> to store the product <c>R = A'*B</c>
    ///   of the given matrices <c>A</c> and <c>B</c>.</param>
    /// 
    public static double[,] TransposeAndDot(this int[,] a, double[,] b, double[,] result)
    {
        int n = a.Rows();
        int m = a.Columns();
        int p = b.Columns();

#if DEBUG
        if (n != b.Rows() || result.Rows() > m || result.Columns() > p)
            throw new DimensionMismatchException();
        var C = a.Transpose().To<double[,]>().Dot(b.To<double[,]>());
#endif
        unsafe
        {
            fixed (double* R = result)
            fixed (double* B = b)
            fixed (double* ptemp = new double[p])
            {
                double* pr = R;

                for (var i = 0; i < m; i++)
                {
                    double* pt = ptemp;
                    double* pb = B;

                    for (var k = 0; k < n; k++)
                    {
                        int aval = a[k, i];
                        for (var j = 0; j < p; j++)
                            *pt++ += (aval * (*pb++));
                        pt = ptemp;
                    }

                    // Update the results row and clear the cache
                    for (var j = 0; j < p; j++)
                    {
                        *pr++ = *pt;
                        *pt++ = 0;
                    }
                }
            }
        }

#if DEBUG
        if (!C.IsEqual(result.To<double[,]>(), 1e-4))
            throw new Exception();
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
    public static double[][] TransposeAndDot(this int[][] a, double[][] b, double[][] result)
    {
#if DEBUG
        if (a.Length != b.Length || result.Length > a.Columns() || result.Columns() > b.Columns())
            throw new DimensionMismatchException();
        var C = Dot(a.Transpose().To<double[,]>(), b.To<double[,]>());
#endif
        int n = a.Length;
        int m = a.Columns();
        int p = b.Columns();

        var Bcolj = new double[n];
        for (var i = 0; i < p; i++)
        {
            for (var k = 0; k < b.Length; k++)
                Bcolj[k] = b[k][i];

            for (var j = 0; j < m; j++)
            {
                double s = 0;
                for (var k = 0; k < Bcolj.Length; k++)
                    s += (a[k][j] * Bcolj[k]);
                result[j][i] = s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
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
    public static double[] TransposeAndDot(this int[,] matrix, double[] columnVector, double[] result)
    {
#if DEBUG
        if (matrix.Rows() != columnVector.Length || result.Length > matrix.Columns())
            throw new DimensionMismatchException();
        var C = Dot(matrix.Transpose().To<double[,]>(), columnVector.To<double[]>());
#endif
        int cols = matrix.Columns();
        for (var j = 0; j < cols; j++)
        {
            double s = 0;
            for (var k = 0; k < columnVector.Length; k++)
                s += (matrix[k, j] * columnVector[k]);
            result[j] = s;
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[]>(), 1e-4))
            throw new Exception();
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
    public static double[] TransposeAndDot(this int[][] matrix, double[] columnVector, double[] result)
    {
#if DEBUG
        if (matrix.Length != columnVector.Length || result.Length > matrix.Columns())
            throw new DimensionMismatchException();
        var C = Dot(matrix.Transpose().To<double[,]>(), columnVector.To<double[]>());
#endif
        int cols = matrix.Columns();
        for (var j = 0; j < cols; j++)
        {
            double s = 0;
            for (var k = 0; k < columnVector.Length; k++)
                s += (matrix[k][j] * columnVector[k]);
            result[j] = s;
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[]>(), 1e-4))
            throw new Exception();
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
    public static double[][] TransposeAndDot(this double[][] a, double[,] b, double[][] result)
    {
        return a.Transpose().Dot(b, result);
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
    public static double[][] TransposeAndDot(this double[][] a, double[][] b)
    {
        return TransposeAndDot(a, b, Jagged.Create<double>(a.Columns(), b.Columns()));
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
    public static double[][] TransposeAndDot(this double[][] a, double[][] b, double[][] result)
    {
#if DEBUG
        if (a.Length != b.Length || result.Length > a.Columns() || result.Columns() > b.Columns())
            throw new DimensionMismatchException();
        var C = Dot(a.Transpose().To<double[,]>(), b.To<double[,]>());
#endif
        int n = a.Length;
        int m = a.Columns();
        int p = b.Columns();

        var Bcolj = new double[n];
        for (int i = 0; i < p; i++)
        {
            for (int k = 0; k < b.Length; k++)
                Bcolj[k] = b[k][i];

            for (int j = 0; j < m; j++)
            {
                double s = (double)0;
                for (int k = 0; k < Bcolj.Length; k++)
                    s += (double)((double)a[k][j] * (double)Bcolj[k]);
                result[j][i] = (double)s;
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
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
    public static double[][] TransposeAndDot(this double[,] a, double[][] b, double[][] result)
    {
        return a.Transpose().Dot(b, result);
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
    public static double[,] TransposeAndDot(this double[] rowVector, double[,] b, double[,] result)
    {
        return rowVector.Transpose().Dot(b, result);
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
    public static double[][] TransposeAndDot(this double[] rowVector, double[][] b, double[][] result)
    {
        return rowVector.Transpose(out double[][] t).Dot(b, result);
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
    public static double[,] TransposeAndDotWithDiagonal(this double[,] a, double[] diagonal, double[,] result)
    {
#if DEBUG
        var C = Dot(a.Transpose().To<double[,]>(), Matrix.Diagonal(diagonal.To<double[]>()));
#endif
        int m = a.Columns();
        for (var i = 0; i < diagonal.Length; i++)
            for (var j = 0; j < m; j++)
                result[j, i] = (a[i, j] * diagonal[i]);
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] TransposeAndDotWithDiagonal(this double[][] a, double[] diagonal, double[][] result)
    {
#if DEBUG
        var C = Dot(a.Transpose().To<double[,]>(), Matrix.Diagonal(diagonal.To<double[]>()));
#endif
        int m = a.Columns();
        for (var i = 0; i < diagonal.Length; i++)
            for (var j = 0; j < m; j++)
                result[j][i] = (a[i][j] * diagonal[i]);
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[,] DotWithDiagonal(this double[,] a, double[] b)
    {
        return DotWithDiagonal(a, b, Matrix.Create<double>(a.Rows(), b.Length));
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
    public static double[,] DotWithDiagonal(this double[,] a, double[] diagonal, double[,] result)
    {
#if DEBUG
        var C = Dot(a.To<double[,]>(), Matrix.Diagonal(diagonal.To<double[]>()));
#endif
        int rows = a.Rows();

        unsafe
        {
            fixed (double* ptrA = a)
            fixed (double* ptrR = result)
            {
                double* A = ptrA;
                double* R = ptrR;
                for (var i = 0; i < rows; i++)
                    for (var j = 0; j < diagonal.Length; j++)
                        *R++ = ((*A++) * diagonal[j]);
            }
        }
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[][] DotWithDiagonal(this double[][] a, double[] diagonal, double[][] result)
    {
#if DEBUG
        var C = Dot(a.To<double[,]>(), Matrix.Diagonal(diagonal.To<double[]>()));
#endif
        int rows = a.Length;
        for (var i = 0; i < rows; i++)
            for (var j = 0; j < diagonal.Length; j++)
                result[i][j] = (a[i][j] * diagonal[j]);
#if DEBUG
        if (!Matrix.IsEqual(C, result.To<double[,]>(), 1e-4))
            throw new Exception();
#endif
        return result;
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
    public static double[,] DivideByDiagonal(this double[,] a, double[] diagonal, double[,] result)
    {
        int rows = a.Rows();

        unsafe
        {
            fixed (double* ptrA = a)
            fixed (double* ptrR = result)
            {
                double* A = ptrA;
                double* R = ptrR;
                for (var i = 0; i < rows; i++)
                    for (var j = 0; j < diagonal.Length; j++)
                        (*R++) = ((*A++) / diagonal[j]);
            }
        }

        return result;
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
    public static double[][] DivideByDiagonal(this double[][] a, double[] diagonal, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < diagonal.Length; j++)
                result[i][j] = ((a[i][j]) / diagonal[j]);

        return result;
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
    public static double[,] Outer(this double[] a, double[] b, double[,] result)
    {
        unsafe
        {
            fixed (double* R = result)
            {
                double* pr = R;
                for (var i = 0; i < a.Length; i++)
                {
                    double x = a[i];
                    for (var j = 0; j < b.Length; j++)
                        *pr++ = (x * b[j]);
                }
            }
        }

        return result;
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
    public static double[][] Outer(this double[] a, double[] b, double[][] result)
    {
        for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
                result[i][j] = (a[i] * b[j]);
        return result;
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
    public static double[,] Kronecker(this double[,] a, double[,] b, double[,] result)
    {
        int arows = a.Rows();
        int acols = a.Columns();

        int brows = b.Rows();
        int bcols = b.Columns();

        //int crows = arows * brows;
        int ccols = acols * bcols;
        int block = brows * ccols;

        unsafe
        {
            fixed (double* ptrR = result)
            fixed (double* ptrA = a)
            fixed (double* ptrB = b)
            {
                double* A = ptrA;
                double* Ri = ptrR;

                for (var i = 0; i < arows; Ri += block, i++)
                {
                    double* Rj = Ri;

                    for (var j = 0; j < acols; j++, Rj += bcols, A++)
                    {
                        double* R = Rj;
                        double* B = ptrB;

                        for (var k = 0; k < brows; k++, R += ccols)
                        {
                            for (var l = 0; l < bcols; l++, B++)
                                *(R + l) = ((*A) * (*B));
                        }
                    }
                }
            }
        }

        return result;
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
    public static double[][] Kronecker(this double[][] a, double[][] b, double[][] result)
    {
        int arows = a.Rows();
        int acols = a.Columns();
        int brows = b.Rows();
        int bcols = b.Columns();

        for (var i = 0; i < arows; i++)
            for (var j = 0; j < acols; j++)
            {
                double aval = a[i][j];
                for (var k = 0; k < brows; k++)
                {
                    double[] brow = b[k];
                    for (var l = 0; l < bcols; l++)
                        result[i * brows + k][j * bcols + l] = (aval * brow[l]);
                }
            }

        return result;
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
    public static double[][] Kronecker(this double[][] a, double[,] b, double[][] result)
    {
        int arows = a.Rows();
        int acols = a.Columns();
        int brows = b.Rows();
        int bcols = b.Columns();

        unsafe
        {
            fixed (double* B = b)
                for (var i = 0; i < arows; i++)
                    for (var j = 0; j < acols; j++)
                    {
                        double aval = a[i][j];
                        double* pb = B;

                        for (var k = 0; k < brows; k++)
                            for (var l = 0; l < bcols; l++)
                                result[i * brows + k][j * bcols + l] = (aval * (*pb++));
                    }
        }

        return result;
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
    public static double[][] Kronecker(this double[,] a, double[][] b, double[][] result)
    {
        int arows = a.Rows();
        int acols = a.Columns();
        int brows = b.Rows();
        int bcols = b.Columns();

        for (var i = 0; i < arows; i++)
            for (var j = 0; j < acols; j++)
            {
                double aval = a[i, j];
                for (var k = 0; k < brows; k++)
                {
                    double[] brow = b[k];
                    for (var l = 0; l < bcols; l++)
                        result[i * brows + k][j * bcols + l] = (aval * brow[l]);
                }
            }

        return result;
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
        unsafe
        {
            fixed (double* R = result)
            {
                double* pr = R;
                for (var i = 0; i < a.Length; i++)
                {
                    double x = a[i];
                    for (var j = 0; j < b.Length; j++)
                        *pr++ = (x * b[j]);
                }
            }
        }

        return result;
    }

    #endregion

    /// <summary>
    ///   Computes the product A*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    ///
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DotWithDiagonal(this double[][] a, double[] b)
    {
        return DotWithDiagonal(a, b, Jagged.Create<double>(a.Length, b.Length));
    }

    /// <summary>
    ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    /// 
    public static double[,] TransposeAndDotWithDiagonal(this double[,] a, double[] b)
    {
        return TransposeAndDotWithDiagonal(a, b, Matrix.Create<double>(a.Columns(), b.Length));
    }

    /// <summary>
    ///   Computes the product A'*B of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of right matrix <c>B</c>.</param>
    /// 
    public static double[][] TransposeAndDotWithDiagonal(this double[][] a, double[] b)
    {
        return TransposeAndDotWithDiagonal(a, b, Jagged.Create<double>(a.Columns(), b.Length));
    }

    /// <summary>
    ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of inverse right matrix <c>B</c>.</param>
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[,] DivideByDiagonal(this double[,] a, double[] b)
    {
        return DivideByDiagonal(a, b, Matrix.CreateAs<double, double>(a));
    }

    /// <summary>
    ///   Computes the product A*inv(B) of matrix <c>A</c> and diagonal matrix <c>B</c>.
    /// </summary>
    /// 
    /// <param name="a">The left matrix <c>A</c>.</param>
    /// <param name="b">The diagonal vector of inverse right matrix <c>B</c>.</param>
    /// <returns>The product <c>A*B</c> of the given matrices <c>A</c> and <c>B</c>.</returns>
    /// 
    public static double[][] DivideByDiagonal(this double[][] a, double[] b)
    {
        return DivideByDiagonal(a, b, Jagged.CreateAs<double, double>(a));
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
    public static double[][] DotWithTransposed(this double[][] a, double[,] b)
    {
        return DotWithTransposed(a, b, Jagged.Create<double>(a.Length, b.Rows()));
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
    public static double[][] DotWithTransposed(this double[,] a, double[][] b)
    {
        return DotWithTransposed(a, b, Jagged.Create<double>(a.Rows(), b.Length));
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
    public static double[,] Kronecker(this double[,] a, double[,] b)
    {
        int crows = a.Rows() * b.Rows();
        int ccols = a.Columns() * b.Columns();
        return Kronecker(a, b, new double[crows, ccols]);
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
    public static double[][] Kronecker(this double[,] a, double[][] b)
    {
        int crows = a.Rows() * b.Rows();
        int ccols = a.Columns() * b.Columns();
        return Kronecker(a, b, Jagged.Create<double>(crows, ccols));
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
    public static double[][] Kronecker(this double[][] a, double[,] b)
    {
        int crows = a.Rows() * b.Rows();
        int ccols = a.Columns() * b.Columns();
        return Kronecker(a, b, Jagged.Create<double>(crows, ccols));
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
    public static double[][] Kronecker(this double[][] a, double[][] b)
    {
        int crows = a.Length * b.Length;
        int ccols = a.Columns() * b.Columns();
        return Kronecker(a, b, Jagged.Create<double>(crows, ccols));
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
        return Outer(a, b, new double[a.Length, b.Length]);
    }
}
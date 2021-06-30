namespace ISynergy.Framework.Mathematics
{
    using System;
    using ISynergy.Framework.Mathematics;
    using System.Runtime.CompilerServices;

    public static partial class Elementwise
    {
        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a scalar <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// 
        public static double[,] Subtract(this double[,] a, double b)
        {
            return Subtract(a, b, new double[a.GetLength(0), a.GetLength(1)]);
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a scalar <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// 
        public static double[][] Subtract(this double[][] a, double b)
        {
            return Subtract(a, b, JaggedCreateAs<double, double>(a));
        }

        /// <summary>
        ///   Elementwise subtraction between a vector <c>a</c> and a scalar <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The vector <c>a</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// 
        public static double[] Subtract(this double[] a, double b)
        {
            return Subtract(a, b, new double[a.Length]);
        }

        /// <summary>
        ///   Elementwise subtraction between a vector <c>a</c> and a vector <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The vector <c>a</c>.</param>
        /// <param name="b">The vector <c>b</c>.</param>
        /// 
        public static double[] Subtract(this double[] a, double[] b)
        {
            return Subtract(a, b, new double[a.Length]);
        }     

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[][] Subtract(this double[][] a, double[][] b)
        {
            return Subtract(a, b, JaggedCreateAs<double, double>(a));
        }     

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[,] Subtract(this double[,] a, double[,] b)
        {
            return Subtract(a, b, MatrixCreateAs<double, double>(a));
        }     

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[,] Subtract(this double a, double[,] b)
        {
            return Subtract(a, b, MatrixCreateAs<double, double>(b));
        }     

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[][] Subtract(this double a, double[][] b)
        {
            return Subtract(a, b, JaggedCreateAs<double, double>(b));
        }     

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and a vector <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The vector <c>b</c>.</param>
        /// 
        public static double[] Subtract(this double a, double[] b)
        {
            return Subtract(a, b, new double[b.Length]);
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[,] Subtract(this double[,] a, double[] b, VectorType dimension)
        {
            return Subtract(a, b, dimension, MatrixCreateAs<double, double>(a));
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[][] Subtract(this double[][] a, double[] b, VectorType dimension)
        {
            return Subtract(a, b, dimension, JaggedCreateAs<double, double>(a));
        }

                /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[,] Subtract(this double[] a, double[,] b, VectorType dimension)
        {
            return Subtract(a, b, dimension, MatrixCreateAs<double, double>(b));
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[][] Subtract(this double[] a, double[][] b, VectorType dimension)
        {
            return Subtract(a, b, dimension, JaggedCreateAs<double, double>(b));
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[,] SubtractFromDiagonal(this double a, double[,] b)
        {
            return SubtractFromDiagonal(a, b, b.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[][] SubtractFromDiagonal(this double a, double[][] b)
        {
            return SubtractFromDiagonal(a, b, b.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[,] SubtractFromDiagonal(this double[] a, double[,] b)
        {
            return SubtractFromDiagonal(a, b, b.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[][] SubtractFromDiagonal(this double[] a, double[][] b)
        {
            return SubtractFromDiagonal(a, b, b.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[,] SubtractFromDiagonal(this double[,] a, double b)
        {
            return SubtractFromDiagonal(a, b, a.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[][] SubtractFromDiagonal(this double[][] a, double b)
        {
            return SubtractFromDiagonal(a, b, a.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[,] SubtractFromDiagonal(this double[,] a, double[] b)
        {
            return SubtractFromDiagonal(a, b, a.MemberwiseClone());
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and to the main diagonal of matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// 
        public static double[][] SubtractFromDiagonal(this double[][] a, double[] b)
        {
            return SubtractFromDiagonal(a, b, a.MemberwiseClone());
        }
        
        #region Matrix matrix

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[][] Subtract(this double[][] a, double[][] b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < result.Length; i++)
                for (var j = 0; j < result[i].Length; j++)
                    result[i][j] = (double)((double)(a[i][j]) - (double)(b[i][j]));

            return result;
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[,] Subtract(this double[,] a, double[,] b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            unsafe
            {
                fixed (double* ptrA = a)
                fixed (double* ptrB = b)
                fixed (double* ptrR = result)
                {
                    var pa = ptrA;
                    var pb = ptrB;
                    var pr = ptrR;
                    for (var i = 0; i < a.Length; i++, pa++, pb++, pr++)
                        *pr = (double)((double)(*pa) - (double)(*pb));
                }
            }

            return result;
        }
#endregion
        #region Matrix with scalar

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a scalar <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[][] Subtract(this double[][] a, double b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < a.Length; i++)
                for (var j = 0; j < a[i].Length; j++)
                    result[i][j] = (double)((double)a[i][j] - (double)b);
            return result;
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and a matrix <c>B</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[][] Subtract(this double a, double[][] b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < b.Length; i++)
                for (var j = 0; j < b[i].Length; j++)
                    result[i][j] = (double)((double)a - (double)b[i][j]);
            return result;
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and a matrix <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The matrix <c>B</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[,] Subtract(this double a, double[,] b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            unsafe
            {
                fixed (double* ptrB = b)        
                fixed (double* ptrR = result)        
                {
                    var pr = ptrR;
                    var pb = ptrB;
                    for (var j = 0; j < b.Length; j++, pr++, pb++)
                        *pr = (double)((double)a - (double)(*pb));
                }
            }

            return result;
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a scalar <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The matrix <c>A</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[,] Subtract(this double[,] a, double b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            unsafe
            {
                fixed (double* ptrA = a)
                fixed (double* ptrR = result)
                {
                    var pa = ptrA;
                    var pr = ptrR;
                    for (var i = 0; i < a.Length; i++, pa++, pr++)
                        *pr = (double)((double)(*pa) - (double)b);
                }
            }

            return result;
        }
        #endregion
        #region vector vector

        /// <summary>
        ///   Elementwise subtraction between a vector <c>a</c> and a vector<c>b</c>.
        /// </summary>
        ///
        /// <param name="a">The vector <c>a</c>.</param>
        /// <param name="b">The vector <c>b</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[] Subtract(this double[] a, double[] b, double[] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < a.Length; i++)
                result[i] = (double)((double)a[i] - (double)b[i]);
            return result;
        }
        #endregion
        #region Vector with scalar

        /// <summary>
        ///   Elementwise subtraction between a vector <c>a</c> and a scalar <c>b</c>.
        /// </summary>
        ///
        /// <param name="a">The vector <c>a</c>.</param>
        /// <param name="b">The scalar <c>b</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[] Subtract(this double[] a, double b, double[] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < a.Length; i++)
                result[i] = (double)((double)a[i] - (double)b);
            return result;
        }

        /// <summary>
        ///   Elementwise subtraction between a scalar <c>a</c> and a vector<c>b</c>.
        /// </summary>
        ///
        /// <param name="a">The scalar <c>a</c>.</param>
        /// <param name="b">The vector <c>b</c>.</param>
        /// <param name="result">The vector where the result should be stored. Pass the same
        ///   vector as one of the arguments to perform the operation in place.</param>
        /// 
        public static double[] Subtract(this double a, double[] b, double[] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < b.Length; i++)
                result[i] = (double)((double)a - (double)b[i]);
            return result;
        }
        #endregion
        #region Matrix vector (enumeration)
        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[,] Subtract(this double[] a, double[,] b, VectorType dimension, double[,] result)
        {
            return Subtract(b, a, dimension, result);
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[][] Subtract(this double[] a, double[][] b, VectorType dimension, double[][] result)
        {
            return Subtract(b, a, dimension, result);
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[,] Subtract(this double[,] a, double[] b, VectorType dimension, double[,] result)
        {
            check<double, double, double>(d: dimension, a: a, b: b, result: result);
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            if (dimension == 0)
            {
                for (var i = 0; i < rows; i++)
                    for (var j = 0; j < cols; j++)
                        result[i, j] = (double)((double)a[i, j] - (double)b[j]);
            }
            else
            {
                for (var i = 0; i < rows; i++)
                    for (var j = 0; j < cols; j++)
                        result[i, j] = (double)((double)a[i, j] - (double)b[i]);
            }

            return result;
        }

        /// <summary>
        ///   Elementwise subtraction between a matrix <c>A</c> and a vector<c>b</c>.
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
        public static double[][] Subtract(this double[][] a, double[] b, VectorType dimension, double[][] result)
        {
            check<double, double, double>(d: dimension, a: a, b: b, result: result);
            if (dimension == 0)
            {
                for (var i = 0; i < a.Length; i++)
                    for (var j = 0; j < a[i].Length; j++)
                        result[i][j] = (double)((double)a[i][j] - (double)b[j]);
            }
            else
            {
                for (var i = 0; i < a.Length; i++)
                    for (var j = 0; j < a[i].Length; j++)
                        result[i][j] = (double)((double)a[i][j] - (double)b[i]);
            }

            return result;
        }
        #endregion
        #region Diagonal
        public static double[,] SubtractFromDiagonal(this double a, double[,] b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            int rows = b.GetLength(0);
            int cols = b.GetLength(1);

            unsafe 
            {
                fixed (double* ptrB = b)        
                fixed (double* ptrR = result)        
                {
                    var pr = ptrR;
                    var pb = ptrB;
                    for (var j = 0; j < rows; j++, pr += cols + 1, pb += cols + 1)
                        *pr = (double)((double)a - (double)(*pb));
                }
            }
            return result;
        }

        public static double[][] SubtractFromDiagonal(this double a, double[][] b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < b.Length; i++)
                result[i][i] = (double)((double)a - (double)b[i][i]);
            return result;
        }

        public static double[,] SubtractFromDiagonal(this double[] a, double[,] b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            int rows = b.GetLength(0);
            int cols = b.GetLength(1);

            unsafe 
            {
                fixed (double* ptrB = b)        
                fixed (double* ptrR = result)        
                {
                    var pr = ptrR;
                    var pb = ptrB;
                    for (var j = 0; j < rows; j++, pr += cols + 1, pb += cols + 1)
                        *pr = (double)((double)a[j] - (double)(*pb));
                }
            }
            return result;
        }

        public static double[][] SubtractFromDiagonal(this double[] a, double[][] b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < b.Length; i++)
                result[i][i] = (double)((double)a[i] - (double)b[i][i]);
            return result;
        }

        public static double[,] SubtractFromDiagonal(this double[,] a, double b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            unsafe 
            {
                fixed (double* ptrA = a)        
                fixed (double* ptrR = result)        
                {
                    var pa = ptrA;
                    var pr = ptrR;
                    for (var j = 0; j < rows; j++, pr += cols + 1, pa += cols + 1)
                        *pr = (double)((double)(*pa) - (double)b);
                }
            }
            return result;
        }

        public static double[][] SubtractFromDiagonal(this double[][] a, double b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < a.Length; i++)
                result[i][i] = (double)((double)a[i][i] - (double)b);
            return result;
        }

        public static double[,] SubtractFromDiagonal(this double[,] a, double[] b, double[,] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            int rows = b.GetLength(0);
            int cols = b.GetLength(1);

            unsafe 
            {
                fixed (double* ptrA = a)        
                fixed (double* ptrR = result)        
                {
                    var pr = ptrR;
                    var pa = ptrA;
                    for (var j = 0; j < rows; j++, pr += cols + 1, pa += cols + 1)
                        *pr = (double)((double)(*pa) - (double)b[j]);
                }
            }
            return result;
        }

        public static double[][] SubtractFromDiagonal(this double[][] a, double[] b, double[][] result)
        {
            check<double, double, double>(a: a, b: b, result: result);
            for (var i = 0; i < b.Length; i++)
                result[i][i] = (double)((double)a[i][i] - (double)b[i]);
            return result;
        }
 
        #endregion

        /// <summary>
        /// Obsolete. Please specify a <see cref="VectorType"/> in place of an integer in the dimension argument.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static double[,] Subtract(this double[,] a, double[] b, int dimension)
        {
            return Subtract(a, b, dimension, MatrixCreateAs<double, double>(a));
        }

        /// <summary>
        /// Obsolete. Please specify a <see cref="VectorType"/> in place of an integer in the dimension argument.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dimension"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static double[,] Subtract(this double[,] a, double[] b, int dimension, double[,] result)
        {
            return Subtract(a, b, (VectorType)dimension, result);
        }
    }
}

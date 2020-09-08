using System;
using System.Linq;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Class Matrix.
    /// </summary>
    public class Matrix
    {
        #region Private variables and fields
        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public int Rows
        {
            get
            {
                return values.GetLength(0);
            }
        }
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public int Columns
        {
            get
            {
                return values.GetLength(1);
            }
        }

        /// <summary>
        /// The values
        /// </summary>
        private readonly double[,] values;

        /// <summary>
        /// Gets or sets the <see cref="System.Double" /> with the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns>System.Double.</returns>
        public double this[int row, int col]
        {
            get
            {
                return values[row, col];
            }
            set
            {
                values[row, col] = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix" /> class.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="n">The n.</param>
        /// <exception cref="ArgumentOutOfRangeException">m is less than or equal to zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">n is less than or equal to zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">m is less than or equal to zero.</exception>
        public Matrix(int m, int n)
        {
            if (m <= 0)
                throw new ArgumentOutOfRangeException("m is less than or equal to zero.");
            if (n <= 0)
                throw new ArgumentOutOfRangeException("n is less than or equal to zero.");

            values = new double[m, n];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix" /> class.
        /// </summary>
        /// <param name="m">The m.</param>
        public Matrix(int m) : this(m, m) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <exception cref="ArgumentNullException">values is null.</exception>
        public Matrix(double[,] values)
        {
            if (values is null)
                throw new ArgumentNullException("values is null.");

            this.values = values;
        }
        #endregion

        #region Overriden methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            Matrix a = this, b = (Matrix)obj;

            if (b is null)
                return false;
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                return false;

            for (var row = 0; row < a.Rows; row++)
                for (var col = 0; col < a.Columns; col++)
                    if (a[row, col] != b[row, col]) return false;

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the + operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            return Add(a, b);
        }

        /// <summary>
        /// Implements the - operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            return Subtract(a, b);
        }
        #endregion

        /// <summary>
        /// Adds the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Matrix.</returns>
        /// <exception cref="ArgumentException">Not identical matrices.</exception>
        public static Matrix Add(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("Not identical matrices.");

            var result = new Matrix(a.Rows, a.Columns);

            for (var row = 0; row < a.Rows; row++)
                for (var col = 0; col < a.Columns; col++)
                    result[row, col] = a[row, col] + b[row, col];

            return result;
        }

        /// <summary>
        /// Subtracts the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Matrix.</returns>
        /// <exception cref="ArgumentException">Not identical matrices.</exception>
        public static Matrix Subtract(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("Not identical matrices.");

            var result = new Matrix(a.Rows, a.Columns);

            for (var row = 0; row < a.Rows; row++)
                for (var col = 0; col < a.Columns; col++)
                    result[row, col] = a[row, col] - b[row, col];

            return result;
        }

        /// <summary>
        /// Normals the multiply.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Matrix.</returns>
        /// <exception cref="ArgumentException">Number of rows of the matrix a doesnt equal to number of columns of the matrix b.</exception>
        public static Matrix NormalMultiply(Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
                throw new ArgumentException("Number of rows of the matrix a doesnt equal to number of columns of the matrix b.");

            var result = new Matrix(a.Rows, b.Columns);

            for (var row = 0; row < a.Rows; row++)
            {
                for (var col = 0; col < b.Columns; col++)
                {
                    double tmp = 0;
                    for (var i = 0; i < a.Columns; i++) // or i < b.Rows, it's equal
                        tmp += a[row, i] * b[i, col];

                    result[row, col] = tmp;
                }
            }

            return result;
        }

        /// <summary>
        /// Strassens the multiply.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Matrix.</returns>
        /// <exception cref="ArgumentException">Not identical or square matrices.</exception>
        public static Matrix StrassenMultiply(Matrix a, Matrix b)
        {
            // TODO If the matrices A, B are not of type 2n x 2n we fill the missing rows and columns with zeros.
            var sizes = new int[] { a.Rows, a.Columns, b.Rows, b.Columns };
            if (sizes.Distinct().Count() != 1 || (a.Rows & (a.Rows - 1)) != 0)
                throw new ArgumentException("Not identical or square matrices.");

            var N = b.Rows;
            if (N <= 48)
                return NormalMultiply(a, b);

            var halfN = N / 2;

            var a11 = a.SubMatrix(0, halfN, 0, halfN);
            var a12 = a.SubMatrix(0, halfN, halfN, N);
            var a21 = a.SubMatrix(halfN, N, 0, halfN);
            var a22 = a.SubMatrix(halfN, N, halfN, N);

            var b11 = b.SubMatrix(0, halfN, 0, halfN);
            var b12 = b.SubMatrix(0, halfN, halfN, N);
            var b21 = b.SubMatrix(halfN, N, 0, halfN);
            var b22 = b.SubMatrix(halfN, N, halfN, N);

            var m = new Matrix[]{
                StrassenMultiply(a11 + a22, b11 + b22),     // m1
                StrassenMultiply(a21 + a22, b11),           // m2
                StrassenMultiply(a11, b12 - b22),           // m3
                StrassenMultiply(a22, b21 - b11),           // m4
                StrassenMultiply(a11 + a12, b22),           // m5
                StrassenMultiply(a21 - a11, b11 + b12),     // m6
                StrassenMultiply(a12 - a22, b21 + b22)      // m7
            };

            var c11 = m[0] + m[3] - m[4] + m[6];
            var c12 = m[2] + m[4];
            var c21 = m[1] + m[3];
            var c22 = m[0] - m[1] + m[2] + m[5];

            return CombineSubMatrices(c11, c12, c21, c22);
        }

        /// <summary>
        /// Subs the matrix.
        /// </summary>
        /// <param name="rowFrom">The row from.</param>
        /// <param name="rowTo">The row to.</param>
        /// <param name="colFrom">The col from.</param>
        /// <param name="colTo">The col to.</param>
        /// <returns>Matrix.</returns>
        private Matrix SubMatrix(int rowFrom, int rowTo, int colFrom, int colTo)
        {
            var result = new Matrix(rowTo - rowFrom, colTo - colFrom);
            for (int row = rowFrom, i = 0; row < rowTo; row++, i++)
                for (int col = colFrom, j = 0; col < colTo; col++, j++)
                    result[i, j] = values[row, col];
            return result;
        }

        /// <summary>
        /// Combines the sub matrices.
        /// </summary>
        /// <param name="a11">The a11.</param>
        /// <param name="a12">The a12.</param>
        /// <param name="a21">The a21.</param>
        /// <param name="a22">The a22.</param>
        /// <returns>Matrix.</returns>
        private static Matrix CombineSubMatrices(Matrix a11, Matrix a12, Matrix a21, Matrix a22)
        {
            var result = new Matrix(a11.Rows * 2);
            var shift = a11.Rows;
            for (var row = 0; row < a11.Rows; row++)
                for (var col = 0; col < a11.Columns; col++)
                {
                    result[row, col] = a11[row, col];
                    result[row, col + shift] = a12[row, col];
                    result[row + shift, col] = a21[row, col];
                    result[row + shift, col + shift] = a22[row, col];
                }
            return result;
        }
    }
}

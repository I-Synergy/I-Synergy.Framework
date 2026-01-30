using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Decompositions.Base;
using ISynergy.Framework.Mathematics.Exceptions;
namespace ISynergy.Framework.Mathematics.Matrices;

public static partial class Matrix
{

    /// <summary>
    ///   Returns the solution matrix if the matrix is square or the least squares solution otherwise.
    /// </summary>
    /// 
    /// <param name="matrix">The matrix for the linear problem.</param>
    /// <param name="rightSide">The right side <c>b</c>.</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <remarks>
    ///   Please note that this does not check if the matrix is non-singular
    ///   before attempting to solve. If a least squares solution is desired
    ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
    ///   parameter when calling this function.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a matrix. Please note that this matrix
    /// // is singular (i.e. not invertible), so only a 
    /// // least squares solution would be feasible here.
    /// 
    /// Double[,] matrix = 
    /// {
    ///     { 1, 2, 3 },
    ///     { 4, 5, 6 },
    ///     { 7, 8, 9 },
    /// };
    /// 
    /// // Define a right side matrix b:
    /// Double[,] rightSide = { {1}, {2}, {3} };
    /// 
    /// // Solve the linear system Ax = b by finding x:
    /// Double[,] x = Matrix.Solve(matrix, rightSide, leastSquares: true);
    /// 
    /// // The answer should be { {-1/18}, {2/18}, {5/18} }.
    /// </code>
    /// </example>
    /// 
    public static Double[,] Solve(this Double[,] matrix, Double[,] rightSide, bool leastSquares = false)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (rows != rightSide.GetLength(0))
            throw new DimensionMismatchException("rightSide",
                "The number of rows in the right hand side matrix must be "
                + "equal to the number of rows in the problem matrix.");

        if (leastSquares)
        {
            return new SingularValueDecomposition(matrix,
                   computeLeftSingularVectors: true,
                   computeRightSingularVectors: true,
                   autoTranspose: true).Solve(rightSide);
        }
        if (rows == cols)
        {
            // Solve by LU Decomposition if matrix is square.
            return new LuDecomposition(matrix).Solve(rightSide);
        }
        else
        {
            if (cols < rows)
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(matrix).Solve(rightSide);
            }
            else
            {
                return new SingularValueDecomposition(matrix,
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true).Solve(rightSide);
            }
        }
    }

    /// <summary>
    ///   Returns the solution matrix if the matrix is square or the least squares solution otherwise.
    /// </summary>
    /// 
    /// <param name="matrix">The matrix for the linear problem.</param>
    /// <param name="rightSide">The right side <c>b</c>.</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <remarks>
    ///   Please note that this does not check if the matrix is non-singular
    ///   before attempting to solve. If a least squares solution is desired
    ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
    ///   parameter when calling this function.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a matrix. Please note that this matrix
    /// // is singular (i.e. not invertible), so only a 
    /// // least squares solution would be feasible here.
    /// 
    /// Double[,] matrix = 
    /// {
    ///     { 1, 2, 3 },
    ///     { 4, 5, 6 },
    ///     { 7, 8, 9 },
    /// };
    /// 
    /// // Define a right side vector b:
    /// Double[] rightSide = { 1, 2, 3 };
    /// 
    /// // Solve the linear system Ax = b by finding x:
    /// Double[] x = Matrix.Solve(matrix, rightSide, leastSquares: true);
    /// 
    /// // The answer should be { -1/18, 2/18, 5/18 }.
    /// </code>
    /// </example>
    /// 
    public static Double[] Solve(this Double[,] matrix, Double[] rightSide, bool leastSquares = false)
    {
        if (matrix is null)
            throw new ArgumentNullException("matrix");

        if (rightSide is null)
            throw new ArgumentNullException("rightSide");

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (rows != rightSide.Length)
            throw new DimensionMismatchException("rightSide",
                "The right hand side vector must have the same length"
                 + "as there are rows of the problem matrix.");

        if (leastSquares)
        {
            return new SingularValueDecomposition(matrix,
                  computeLeftSingularVectors: true,
                  computeRightSingularVectors: true,
                  autoTranspose: true).Solve(rightSide);
        }
        if (rows == cols)
        {
            // Solve by LU Decomposition if matrix is square.
            return new LuDecomposition(matrix).Solve(rightSide);
        }
        else
        {
            if (cols < rows)
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(matrix).Solve(rightSide);
            }
            else
            {
                return new SingularValueDecomposition(matrix,
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true).Solve(rightSide);
            }
        }
    }

    /// <summary>
    ///   Computes the inverse of a matrix.
    /// </summary>
		/// 
    public static Double[,] Inverse(this Double[,] matrix)
    {
        return Inverse(matrix, false);
    }

    /// <summary>
    ///   Computes the inverse of a matrix.
    /// </summary>
    /// 
    public static Double[,] Inverse(this Double[,] matrix, bool inPlace)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (rows != cols)
            throw new ArgumentException("Matrix must be square", "matrix");

        if (rows == 3)
        {
            // Special case for 3x3 matrices
            Double a = matrix[0, 0], b = matrix[0, 1], c = matrix[0, 2];
            Double d = matrix[1, 0], e = matrix[1, 1], f = matrix[1, 2];
            Double g = matrix[2, 0], h = matrix[2, 1], i = matrix[2, 2];

            Double den = a * (e * i - f * h) -
                         b * (d * i - f * g) +
                         c * (d * h - e * g);

            if (den == 0)
                throw new SingularMatrixException();

            Double m = 1 / den;

            var inv = (inPlace) ? matrix : new Double[3, 3];
            inv[0, 0] = m * (e * i - f * h);
            inv[0, 1] = m * (c * h - b * i);
            inv[0, 2] = m * (b * f - c * e);
            inv[1, 0] = m * (f * g - d * i);
            inv[1, 1] = m * (a * i - c * g);
            inv[1, 2] = m * (c * d - a * f);
            inv[2, 0] = m * (d * h - e * g);
            inv[2, 1] = m * (b * g - a * h);
            inv[2, 2] = m * (a * e - b * d);

            return inv;
        }

        if (rows == 2)
        {
            // Special case for 2x2 matrices
            Double a = matrix[0, 0], b = matrix[0, 1];
            Double c = matrix[1, 0], d = matrix[1, 1];

            Double den = a * d - b * c;

            if (den == 0)
                throw new SingularMatrixException();

            Double m = 1 / den;

            var inv = (inPlace) ? matrix : new Double[2, 2];
            inv[0, 0] = +m * d;
            inv[0, 1] = -m * b;
            inv[1, 0] = -m * c;
            inv[1, 1] = +m * a;

            return inv;
        }

        return new LuDecomposition(matrix, false, inPlace).Inverse();
    }

    /// <summary>
    ///   Computes the pseudo-inverse of a matrix.
    /// </summary>
    ///
    public static Double[,] PseudoInverse(this Double[,] matrix)
    {
        return new SingularValueDecomposition(matrix,
            computeLeftSingularVectors: true,
            computeRightSingularVectors: true,
            autoTranspose: true).Inverse();
    }
    /// <summary>
    ///   Returns the solution matrix if the matrix is square or the least squares solution otherwise.
    /// </summary>
    /// 
    /// <param name="matrix">The matrix for the linear problem.</param>
    /// <param name="rightSide">The right side <c>b</c>.</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <remarks>
    ///   Please note that this does not check if the matrix is non-singular
    ///   before attempting to solve. If a least squares solution is desired
    ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
    ///   parameter when calling this function.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a matrix. Please note that this matrix
    /// // is singular (i.e. not invertible), so only a 
    /// // least squares solution would be feasible here.
    /// 
    /// Double[,] matrix = 
    /// {
    ///     { 1, 2, 3 },
    ///     { 4, 5, 6 },
    ///     { 7, 8, 9 },
    /// };
    /// 
    /// // Define a right side matrix b:
    /// Double[,] rightSide = { {1}, {2}, {3} };
    /// 
    /// // Solve the linear system Ax = b by finding x:
    /// Double[,] x = Matrix.Solve(matrix, rightSide, leastSquares: true);
    /// 
    /// // The answer should be { {-1/18}, {2/18}, {5/18} }.
    /// </code>
    /// </example>
    /// 
    public static Double[][] Solve(this Double[][] matrix, Double[][] rightSide, bool leastSquares = false)
    {
        if (matrix.Length != rightSide.Length)
        {
            throw new DimensionMismatchException("rightSide",
                "The number of rows in the right hand side matrix must be "
                + "equal to the number of rows in the problem matrix.");
        }

        return matrix.Decompose(leastSquares).Solve(rightSide);
    }

    /// <summary>
    ///   Returns the solution matrix if the matrix is square or the least squares solution otherwise.
    /// </summary>
    /// 
    /// <param name="matrix">The matrix for the linear problem.</param>
    /// <param name="rightSide">The right side <c>b</c>.</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <remarks>
    ///   Please note that this does not check if the matrix is non-singular
    ///   before attempting to solve. If a least squares solution is desired
    ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
    ///   parameter when calling this function.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a matrix. Please note that this matrix
    /// // is singular (i.e. not invertible), so only a 
    /// // least squares solution would be feasible here.
    /// 
    /// Double[,] matrix = 
    /// {
    ///     { 1, 2, 3 },
    ///     { 4, 5, 6 },
    ///     { 7, 8, 9 },
    /// };
    /// 
    /// // Define a right side vector b:
    /// Double[] rightSide = { 1, 2, 3 };
    /// 
    /// // Solve the linear system Ax = b by finding x:
    /// Double[] x = Matrix.Solve(matrix, rightSide, leastSquares: true);
    /// 
    /// // The answer should be { -1/18, 2/18, 5/18 }.
    /// </code>
    /// </example>
    /// 
    public static Double[] Solve(this Double[][] matrix, Double[] rightSide, bool leastSquares = false)
    {
        if (matrix.Length != rightSide.Length)
        {
            throw new DimensionMismatchException("rightSide",
                "The right hand side vector must have the same length"
                 + "as there are rows of the problem matrix.");
        }

        return matrix.Decompose(leastSquares).Solve(rightSide);
    }

		/// <summary>
    ///   Returns the solution matrix for a linear system involving a diagonal matrix ion the right-hand side.
    /// </summary>
    /// 
    /// <param name="matrix">The matrix for the linear problem.</param>
    /// <param name="diagonalRightSide">The right side <c>b</c>.</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="matrix"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <remarks>
    ///   Please note that this does not check if the matrix is non-singular
    ///   before attempting to solve. If a least squares solution is desired
    ///   in case the matrix is singular, pass true to the <paramref name="leastSquares"/>
    ///   parameter when calling this function.
    /// </remarks>
    /// 
		public static Double[][] SolveForDiagonal(this Double[][] matrix, Double[] diagonalRightSide, bool leastSquares = false)
    {
        if (matrix.Length != diagonalRightSide.Length)
        {
            throw new DimensionMismatchException("diagonalRightSide",
                "The right hand side matrix must have the same length"
                 + "as there are rows of the problem matrix.");
        }

        return matrix.Decompose(leastSquares).SolveForDiagonal(diagonalRightSide);
    }
    /// <summary>
    ///   Creates a matrix decomposition that be used to compute the solution matrix if the 
    ///   matrix is square or the least squares solution otherwise.
    /// </summary>
    /// 
    public static ISolverMatrixDecomposition<Double> Decompose(this Double[,] matrix, bool leastSquares = false)
    {
        int rows = matrix.Rows();
        int cols = matrix.Columns();

        if (leastSquares)
        {
            return new SingularValueDecomposition(matrix,
                   computeLeftSingularVectors: true,
                   computeRightSingularVectors: true,
                   autoTranspose: true);
        }

        if (rows == cols)
        {
            // Solve by LU Decomposition if matrix is square.
            return new LuDecomposition(matrix);
        }
        else
        {
            if (cols < rows)
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(matrix);
            }
            else
            {
                return new SingularValueDecomposition(matrix,
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true);
            }
        }
    }

    /// <summary>
    ///   Creates a matrix decomposition that be used to compute the solution matrix if the 
    ///   matrix is square or the least squares solution otherwise.
    /// </summary>
    /// 
    public static ISolverArrayDecomposition<Double> Decompose(this Double[][] matrix, bool leastSquares = false)
    {
        int rows = matrix.Rows();
        int cols = matrix.Columns();

        if (leastSquares)
        {
            return new JaggedSingularValueDecomposition(matrix,
                  computeLeftSingularVectors: true,
                  computeRightSingularVectors: true,
                  autoTranspose: true);
        }

        if (rows == cols)
        {
            // Solve by LU Decomposition if matrix is square.
            return new JaggedLuDecomposition(matrix);
        }
        else
        {
            if (cols < rows)
            {
                // Solve by QR Decomposition if not.
                return new JaggedQrDecomposition(matrix);
            }
            else
            {
                return new JaggedSingularValueDecomposition(matrix,
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true);
            }
        }
    }

    /// <summary>
    ///   Computes the inverse of a matrix.
    /// </summary>
    /// 
		/// 
    public static Double[][] Inverse(this Double[][] matrix)
    {
        return Inverse(matrix, false);
    }

    /// <summary>
    ///   Computes the inverse of a matrix.
    /// </summary>
    /// 
    public static Double[][] Inverse(this Double[][] matrix, bool inPlace)
    {
        int rows = matrix.Length;
        int cols = matrix[0].Length;

        if (rows != cols)
            throw new ArgumentException("Matrix must be square", "matrix");

        if (rows == 3)
        {
            // Special case for 3x3 matrices
            Double a = matrix[0][0], b = matrix[0][1], c = matrix[0][2];
            Double d = matrix[1][0], e = matrix[1][1], f = matrix[1][2];
            Double g = matrix[2][0], h = matrix[2][1], i = matrix[2][2];

            Double den = a * (e * i - f * h) -
                         b * (d * i - f * g) +
                         c * (d * h - e * g);

            if (den == 0)
                throw new SingularMatrixException();

            Double m = 1 / den;

            var inv = matrix;
            if (!inPlace)
            {
                inv = new Double[3][];
                for (var j = 0; j < inv.Length; j++)
                    inv[j] = new Double[3];
            }

            inv[0][0] = m * (e * i - f * h);
            inv[0][1] = m * (c * h - b * i);
            inv[0][2] = m * (b * f - c * e);
            inv[1][0] = m * (f * g - d * i);
            inv[1][1] = m * (a * i - c * g);
            inv[1][2] = m * (c * d - a * f);
            inv[2][0] = m * (d * h - e * g);
            inv[2][1] = m * (b * g - a * h);
            inv[2][2] = m * (a * e - b * d);

            return inv;
        }

        if (rows == 2)
        {
            // Special case for 2x2 matrices
            Double a = matrix[0][0], b = matrix[0][1];
            Double c = matrix[1][0], d = matrix[1][1];

            Double den = a * d - b * c;

            if (den == 0)
                throw new SingularMatrixException();

            Double m = 1 / den;

            var inv = matrix;
            if (!inPlace)
            {
                inv = new Double[2][];
                for (var j = 0; j < inv.Length; j++)
                    inv[j] = new Double[2];
            }

            inv[0][0] = +m * d;
            inv[0][1] = -m * b;
            inv[1][0] = -m * c;
            inv[1][1] = +m * a;

            return inv;
        }

        return new JaggedLuDecomposition(matrix, false, inPlace).Inverse();
    }

    /// <summary>
    ///   Computes the pseudo-inverse of a matrix.
    /// </summary>
		///
    public static Double[][] PseudoInverse(this Double[][] matrix)
    {
        return new JaggedSingularValueDecomposition(matrix,
            computeLeftSingularVectors: true,
            computeRightSingularVectors: true,
            autoTranspose: true).Inverse();
    }

    /// <summary>
    ///   Divides two matrices by multiplying A by the inverse of B.
    /// </summary>
    /// 
    /// <param name="a">The first matrix.</param>
    /// <param name="b">The second matrix (which will be inverted).</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="b"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <returns>The result from the division <c>AB^-1</c> of the given matrices.</returns>
    /// 
    public static Double[,] Divide(this Double[,] a, Double[,] b, bool leastSquares = false)
    {
        int rows = b.Rows();
        int cols = b.Columns();

        if (leastSquares)
        {
            return new SingularValueDecomposition(b.Transpose(),
                   computeLeftSingularVectors: true,
                   computeRightSingularVectors: true,
                   autoTranspose: true).Solve(a.Transpose()).Transpose();
        }
        if (rows == cols && cols == a.Rows())
        {
            // Solve by LU Decomposition if matrix is square.
            return new LuDecomposition(b, true).SolveTranspose(a);
        }
        else
        {
            if (rows <= cols)
            {
                // Solve by QR Decomposition if not.
                return new QrDecomposition(b, true).SolveTranspose(a);
            }
            else
            {
                return new SingularValueDecomposition(b.Transpose(),
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true).Solve(a.Transpose()).Transpose();
            }
        }
    }

    /// <summary>
    ///   Divides two matrices by multiplying A by the inverse of B.
    /// </summary>
    /// 
    /// <param name="a">The first matrix.</param>
    /// <param name="b">The second matrix (which will be inverted).</param>
    /// <param name="leastSquares">True to produce a solution even if the 
    ///   <paramref name="b"/> is singular; false otherwise. Default is false.</param>
    /// 
    /// <returns>The result from the division <c>AB^-1</c> of the given matrices.</returns>
    /// 
    public static Double[][] Divide(this Double[][] a, Double[][] b, bool leastSquares = false)
    {
        int rows = b.Rows();
        int cols = b.Columns();

        if (leastSquares)
        {
            return new JaggedSingularValueDecomposition(b.Transpose(),
                   computeLeftSingularVectors: true,
                   computeRightSingularVectors: true,
                   autoTranspose: true).Solve(a.Transpose()).Transpose();
        }
        if (rows == cols && cols == a.Rows())
        {
            // Solve by LU Decomposition if matrix is square.
            return new JaggedLuDecomposition(b, true).SolveTranspose(a);
        }
        else
        {
            if (rows <= cols)
            {
                // Solve by QR Decomposition if not.
                return new JaggedQrDecomposition(b, true).SolveTranspose(a);
            }
            else
            {
                return new JaggedSingularValueDecomposition(b.Transpose(),
                    computeLeftSingularVectors: true,
                    computeRightSingularVectors: true,
                    autoTranspose: true).Solve(a.Transpose()).Transpose();
            }
        }
    }

    /// <summary>
    ///   Gets the null-space of a column vector.
    /// </summary>
    ///
    public static Double[][] Null(this Double[] vector)
    {
        return Null(Jagged.ColumnVector(vector));
    }

    /// <summary>
    ///   Gets the null-space of a matrix.
    /// </summary>
    ///
    public static Double[][] Null(this Double[][] matrix)
    {
        var qr = new JaggedQrDecomposition(matrix, economy: false);
        var Q = qr.OrthogonalFactor;
        var threshold = matrix.GetLength().Max() * Constants.DoubleEpsilon;
        int[] idx = qr.Diagonal.Find(x => (Double)Math.Abs(x) < threshold);
        return Q.GetColumns(idx);
    }

    /// <summary>
    ///   Gets the null-space of a matrix.
    /// </summary>
    ///
    public static Double[,] Null(this Double[,] matrix)
    {
        var qr = new QrDecomposition(matrix, economy: false);
        var Q = qr.OrthogonalFactor;
        var threshold = matrix.GetLength().Max() * Constants.DoubleEpsilon;
        int[] idx = qr.Diagonal.Find(x => (Double)Math.Abs(x) < threshold);
        return Q.GetColumns(idx);
    }
}
namespace ISynergy.Framework.Mathematics.Decompositions
{
    /// <summary>
    ///     Common interface for matrix decompositions which
    ///     can be used to solve linear systems of equations
    ///     involving jagged array matrices.
    /// </summary>
    public interface ISolverArrayDecomposition<T> where T : struct
    {
        /// <summary>
        ///     Solves a set of equation systems of type <c>A * X = B</c>.
        /// </summary>
        T[][] Solve(T[][] value);

        /// <summary>
        ///     Solves a set of equation systems of type <c>A * X = B</c>.
        /// </summary>
        T[] Solve(T[] value);

        /// <summary>
        ///     Solves a set of equation systems of type <c>A * X = B</c> where B is a diagonal matrix.
        /// </summary>
        T[][] SolveForDiagonal(T[] diagonal);

        /// <summary>
        ///     Solves a set of equation systems of type <c>A * X = I</c>.
        /// </summary>
        T[][] Inverse();

        /// <summary>
        ///     Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///     matrix can be used to determine standard errors for the coefficients when
        ///     solving a linear set of equations through any of the <see cref="Solve(T[][])" />
        ///     methods.
        /// </summary>
        T[][] GetInformationMatrix();

        /// <summary>
        ///     Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        T[][] Reverse();
    }
}
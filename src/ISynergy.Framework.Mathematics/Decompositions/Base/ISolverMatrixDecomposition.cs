using System.Diagnostics.CodeAnalysis;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.Decompositions.Base;

/// <summary>
///     Common interface for matrix decompositions which
///     can be used to solve linear systems of equations.
/// </summary>
public interface ISolverMatrixDecomposition<T> where T : struct
{
    /// <summary>
    ///     Solves a set of equation systems of type <c>A * X = B</c>.
    /// </summary>
    T[,] Solve(T[,] value);

    /// <summary>
    ///     Solves a set of equation systems of type <c>A * X = B</c>.
    /// </summary>
    T[] Solve(T[] value);

    /// <summary>
    ///     Solves a set of equation systems of type <c>A * X = I</c>.
    /// </summary>
    [RequiresUnreferencedCode("Implementations may use reflection-based type conversion.")]
    [RequiresDynamicCode("Implementations may require dynamic code generation.")]
    T[,] Inverse();

    /// <summary>
    ///     Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
    ///     matrix can be used to determine standard errors for the coefficients when
    ///     solving a linear set of equations through any of the <see cref="Solve(T[,])" />
    ///     methods.
    /// </summary>
    T[,] GetInformationMatrix();

    /// <summary>
    ///     Reverses the decomposition, reconstructing the original matrix <c>X</c>.
    /// </summary>
    T[,] Reverse();
}
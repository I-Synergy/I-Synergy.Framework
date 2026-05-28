
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Convergence.Base;

/// <summary>
///     Common interface for convergence detection algorithms.
/// </summary>
public interface IConvergence<T> : IConvergence
{
    /// <summary>
    ///     Gets or sets the watched value after the iteration.
    /// </summary>
    T NewValue { get; set; }
}

/// <summary>
///     Common interface for convergence detection algorithms.
/// </summary>
public interface IConvergence
{
    /// <summary>
    ///     Gets or sets the maximum relative change in the watched value
    ///     after an iteration of the algorithm used to detect convergence.
    /// </summary>
    double Tolerance { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of iterations
    ///     performed by the iterative algorithm.
    /// </summary>
    int MaxIterations { get; set; }

    /// <summary>
    ///     Gets the current iteration number.
    /// </summary>
    int CurrentIteration { get; }

    /// <summary>
    ///     Gets or sets whether the algorithm has converged.
    /// </summary>
    bool HasConverged { get; }

    /// <summary>
    ///     Resets this instance, reverting all iteration statistics
    ///     statistics (number of iterations, last error) back to zero.
    /// </summary>
    void Clear();
}
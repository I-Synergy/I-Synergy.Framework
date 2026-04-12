
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Statistics;

/// <summary>
///   Common interface for convergence-based iterative learning algorithms.
/// </summary>
/// 
public interface IConvergenceLearning
{

    /// <summary>
    ///   Gets or sets the tolerance value used to determine 
    ///   whether the algorithm has converged. 
    /// </summary>
    /// 
    double Tolerance { get; set; }

    /// <summary>
    ///   Gets or sets the maximum number of iterations
    ///   performed by the learning algorithm.
    /// </summary>
    /// 
    int MaxIterations { get; set; }

    /// <summary>
    ///   Gets the current iteration number.
    /// </summary>
    /// 
    int CurrentIteration { get; }

    /// <summary>
    ///   Gets or sets whether the algorithm has converged.
    /// </summary>
    /// 
    bool HasConverged { get; }
}

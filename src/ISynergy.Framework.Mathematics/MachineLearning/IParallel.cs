using ISynergy.Framework.Core.Abstractions.Async;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.MachineLearning;

/// <summary>
///   Common interface for parallel algorithms.
/// </summary>
/// 
public interface IParallel : ISupportsCancellation
{
    /// <summary>
    ///   Gets or sets the parallelization options for this algorithm. It can
    ///   be used to control the maximum number of cores that should be used
    ///   during the algorithm's execution.
    /// </summary>
    /// 
    /// <remarks>
    ///   The <see cref="IParallel"/> interface is implemented by most machine learning
    ///   algorithms in the framework, and it is most common use is to allow the user to
    ///   tune how many cores should be used by a multi-threaded learning algorithm.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   In the following example, we will be using the <see cref="ParallelOptions"/> property to limit 
    ///   the maximum degree of parallelism of a support vector machine learning algorithm to be 1, meaning
    ///   the algorithm will be running in a single thread:</para>
    /// </example>
    /// 
    ParallelOptions ParallelOptions { get; set; }
}

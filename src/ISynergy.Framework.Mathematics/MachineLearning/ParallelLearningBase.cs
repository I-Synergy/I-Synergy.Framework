using System.Runtime.Serialization;

#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation


namespace ISynergy.Framework.Mathematics.MachineLearning;

/// <summary>
///   Base class for parallel learning algorithms.
/// </summary>
/// 
[Serializable]
public class ParallelLearningBase : IParallel
{
    [NonSerialized]
    private ParallelOptions parallelOptions;

    /// <summary>
    ///   Gets or sets the parallelization options for this algorithm.
    /// </summary>
    /// 
    public ParallelOptions ParallelOptions
    {
        get { return parallelOptions; }
        set { parallelOptions = value; }
    }

    /// <summary>
    /// Gets or sets a cancellation token that can be used
    /// to cancel the algorithm while it is running.
    /// </summary>
    /// 
    public CancellationToken Token
    {
        get { return ParallelOptions.CancellationToken; }
        set { ParallelOptions.CancellationToken = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelLearningBase"/> class.
    /// </summary>
    public ParallelLearningBase()
    {
        parallelOptions = new ParallelOptions();
    }

    /// <summary>
    ///   Called when the object is being deserialized.
    /// </summary>
    /// 
    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
        parallelOptions = new ParallelOptions();
    }
}

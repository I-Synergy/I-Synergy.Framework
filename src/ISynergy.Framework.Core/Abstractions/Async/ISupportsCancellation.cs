namespace ISynergy.Framework.Core.Abstractions.Async;

/// <summary>
///   Common interface for algorithms that can be canceled
///   in the middle of execution.
/// </summary>
/// 
public interface ISupportsCancellation
{
    /// <summary>
    /// Gets or sets a cancellation token that can be used
    /// to cancel the algorithm while it is running.
    /// </summary>
    /// 
    CancellationToken Token { get; set; }
}

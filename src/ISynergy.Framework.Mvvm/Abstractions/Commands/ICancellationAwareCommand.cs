namespace ISynergy.Framework.Mvvm.Abstractions.Commands;

/// <summary>
/// An interface for commands that know whether they support cancellation or not.
/// </summary>
public interface ICancellationAwareCommand
{
    /// <summary>
    /// Gets whether or not the current command supports cancellation.
    /// </summary>
    bool IsCancellationSupported { get; }

    void Cancel();
}

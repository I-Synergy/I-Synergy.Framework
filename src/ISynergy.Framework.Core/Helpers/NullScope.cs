namespace ISynergy.Framework.Core.Helpers;

/// <summary>
/// An empty scope without any logic
/// </summary>
public class NullScope : IDisposable // NOSONAR - finalizer-free; SuppressFinalize omitted per S3971
{
    public static NullScope Instance { get; } = new NullScope();

    private NullScope()
    {
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// This is a no-op scope — there are no resources to release.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Releases resources. This scope owns none.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}

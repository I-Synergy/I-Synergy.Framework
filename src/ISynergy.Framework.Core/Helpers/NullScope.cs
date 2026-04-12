namespace ISynergy.Framework.Core.Helpers;

/// <summary>
/// An empty scope without any logic
/// </summary>
public class NullScope : IDisposable
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
        GC.SuppressFinalize(this);
    }
}

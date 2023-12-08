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

    public void Dispose()
    {
    }
}

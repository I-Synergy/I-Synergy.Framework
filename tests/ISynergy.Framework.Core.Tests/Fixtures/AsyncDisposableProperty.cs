using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Tests.Fixtures;
internal class AsyncDisposableProperty : Property<string>, IAsyncDisposable
{
    public bool IsAsyncDisposed { get; private set; }

    public AsyncDisposableProperty(string name)
        : base(name)
    {
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Delay(100); // Simulate async work
        IsAsyncDisposed = true;
    }
}

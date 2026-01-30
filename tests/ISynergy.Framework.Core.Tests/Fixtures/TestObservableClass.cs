using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Tests.Fixtures;
internal class TestObservableClass : ObservableValidatedClass
{
    public bool IsAsyncDisposed { get; private set; }

    public void EnsureNotDisposed()
    {
        ThrowIfDisposed();
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        IsAsyncDisposed = true;
        await base.DisposeAsyncCore();
    }

    public string TestProperty
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public void TriggerPropertyChanged(string propertyName)
    {
        RaisePropertyChanged(propertyName);
    }

    public void TriggerErrorsChanged(string propertyName)
    {
        OnErrorsChanged(propertyName);
    }

    public void AddError(string propertyName)
    {
        AddValidationError(propertyName, "Test Error");
    }
}

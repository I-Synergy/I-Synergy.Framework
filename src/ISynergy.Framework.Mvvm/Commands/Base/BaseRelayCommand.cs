using ISynergy.Framework.Mvvm.Abstractions.Commands;

namespace ISynergy.Framework.Mvvm.Commands.Base;

public abstract class BaseRelayCommand : IRelayCommand, IDisposable
{
    protected bool _disposed;
    private EventHandler? _canExecuteChanged;

    public event EventHandler? CanExecuteChanged
    {
        add => _canExecuteChanged += value;
        remove => _canExecuteChanged -= value;
    }

    protected void OnCanExecuteChanged()
    {
        ThrowIfDisposed();
        _canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NotifyCanExecuteChanged() => OnCanExecuteChanged();

    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    public abstract bool CanExecute(object? parameter);
    public abstract void Execute(object? parameter);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
            _canExecuteChanged = null;
        }
    }
}

using ISynergy.Framework.Mvvm.Abstractions.Commands;

namespace ISynergy.Framework.Mvvm.Commands.Base;

/// <summary>
/// Base class for relay commands providing common functionality.
/// Implements <see cref="IRelayCommand"/> and <see cref="IDisposable"/> interfaces.
/// </summary>
public abstract class BaseRelayCommand : IRelayCommand, IDisposable
{
    /// <summary>
    /// Flag indicating whether the command has been disposed.
    /// </summary>
    protected bool _disposed;

    /// <summary>
    /// Event handler for the CanExecuteChanged event.
    /// </summary>
    private EventHandler? _canExecuteChanged;

    /// <summary>
    /// Lock object for thread synchronization.
    /// </summary>
    protected readonly object _syncLock = new object();

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            lock (_syncLock)
            {
                _canExecuteChanged += value;
            }
        }
        remove
        {
            lock (_syncLock)
            {
                _canExecuteChanged -= value;
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the command has been disposed.</exception>
    protected void OnCanExecuteChanged()
    {
        ThrowIfDisposed();

        EventHandler? handler;
        lock (_syncLock)
        {
            handler = _canExecuteChanged;
        }

        handler?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Notifies that the command's ability to execute may have changed.
    /// </summary>
    public void NotifyCanExecuteChanged() => OnCanExecuteChanged();

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the command has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the command has been disposed.</exception>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public abstract bool CanExecute(object? parameter);

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
    public abstract void Execute(object? parameter);

    /// <summary>
    /// Attempts to cancel the command execution if supported.
    /// Base implementation does nothing as synchronous commands cannot be canceled.
    /// Override in derived classes to provide cancellation support.
    /// </summary>
    /// <returns>True if cancellation was requested, false otherwise.</returns>
    public virtual bool TryCancel()
    {
        // Base implementation does not support cancellation
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether this command supports cancellation.
    /// Base implementation returns false as synchronous commands cannot be canceled.
    /// Override in derived classes to provide cancellation support.
    /// </summary>
    public virtual bool CanBeCanceled => false;

    /// <summary>
    /// Disposes the resources used by this command.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources used by this command.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            lock (_syncLock)
            {
                _disposed = true;
                _canExecuteChanged = null;
            }
        }
    }
}

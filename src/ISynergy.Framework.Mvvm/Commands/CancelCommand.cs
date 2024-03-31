using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using System.ComponentModel;
using System.Windows.Input;

#nullable enable

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A <see cref="ICommand"/> implementation wrapping <see cref="IAsyncRelayCommand"/> to support cancellation.
/// </summary>
internal sealed class CancelCommand : ICommand, IDisposable
{
    /// <summary>
    /// The wrapped <see cref="IAsyncRelayCommand"/> instance.
    /// </summary>
    private readonly IAsyncRelayCommand _command;

    /// <summary>
    /// Creates a new <see cref="CancelCommand"/> instance.
    /// </summary>
    /// <param name="command">The <see cref="IAsyncRelayCommand"/> instance to wrap.</param>
    public CancelCommand(IAsyncRelayCommand command)
    {
        _command = command;
        _command.PropertyChanged += OnPropertyChanged;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => _command.CanBeCanceled;

    /// <inheritdoc/>
    public void Execute(object? parameter) => _command.Cancel();

    /// <inheritdoc cref="PropertyChangedEventHandler"/>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null or nameof(IAsyncRelayCommand.CanBeCanceled))
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    #region IDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // NOTE: Leave out the finalizer altogether if this class doesn't
    // own unmanaged resources, but leave the other methods
    // exactly as they are.
    //~ObservableClass()
    //{
    //    // Finalizer calls Dispose(false)
    //    Dispose(false);
    //}

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            if (_command is not null)
                _command.PropertyChanged -= OnPropertyChanged;
        }

        // free native resources if there are any.
    }
    #endregion
}

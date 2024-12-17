using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using System.Runtime.CompilerServices;

#nullable enable

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A _command whose sole purpose is to relay its functionality to other
/// objects by invoking delegates. The default return value for the <see cref="CanExecute"/>
/// method is <see langword="true"/>. This type does not allow you to accept _command parameters
/// in the <see cref="Execute"/> and <see cref="CanExecute"/> callback methods.
/// </summary>
public sealed class RelayCommand : IRelayCommand, IDisposable
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    private bool _disposed;

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the RelayCommand class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <exception cref="ArgumentNullException">Thrown if execute is null.</exception>
    public RelayCommand(Action execute)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    /// <summary>
    /// Initializes a new instance of the RelayCommand class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="ArgumentNullException">Thrown if execute or canExecute are null.</exception>
    public RelayCommand(Action execute, Func<bool> canExecute)
        : this(execute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public void NotifyCanExecuteChanged()
    {
        ThrowIfDisposed();
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object? parameter)
    {
        ThrowIfDisposed();
        return _canExecute?.Invoke() != false;
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        ThrowIfDisposed();

        try
        {
            _execute();
        }
        catch (Exception ex)
        {
            var exceptionHandlerService = ServiceLocator.Default.GetService<IExceptionHandlerService>();

            if (ex.InnerException != null)
            {
                exceptionHandlerService.HandleExceptionAsync(ex.InnerException)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                exceptionHandlerService.HandleExceptionAsync(ex)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RelayCommand));
        }
    }

    /// <summary>
    /// Disposes the command, cleaning up resources and preventing further execution.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            // Clear event handlers
            CanExecuteChanged = null;
        }

        GC.SuppressFinalize(this);
    }
}
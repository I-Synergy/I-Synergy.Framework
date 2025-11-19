using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Commands.Base;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A _command whose sole purpose is to relay its functionality to other
/// objects by invoking delegates. The default return value for the <see cref="CanExecute"/>
/// method is <see langword="true"/>. This type does not allow you to accept _command parameters
/// in the <see cref="Execute"/> and <see cref="CanExecute"/> callback methods.
/// </summary>
public sealed class RelayCommand : BaseRelayCommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute)
    {
        _execute = Argument.IsNotNull(execute);
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
        : this(execute)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanExecute(object? parameter)
    {
        ThrowIfDisposed();

        lock (_syncLock)
        {
            return _canExecute?.Invoke() != false;
        }
    }

    public override void Execute(object? parameter)
    {
        ThrowIfDisposed();

        lock (_syncLock)
        {
            try
            {
                _execute?.Invoke();
            }
            catch (Exception ex)
            {
                // Try to handle exception via service
                bool handled = false;
                try
                {
                    var exceptionHandlerService = ServiceLocator.Default.GetService<IExceptionHandlerService>();
                    if (exceptionHandlerService is not null)
                    {
                        exceptionHandlerService.HandleException(ex);
                        handled = true;
                    }
                }
                catch
                {
                    // If exception handler fails, re-throw the original exception
                    throw;
                }

                // If exception was successfully handled, suppress it to prevent app crash
                // Otherwise, re-throw to maintain original behavior when no handler is available
                if (!handled)
                {
                    throw;
                }
            }
        }
    }
}
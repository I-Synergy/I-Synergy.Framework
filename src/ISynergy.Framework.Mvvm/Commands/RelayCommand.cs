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

        // A synchronous throw here would become a genuine unhandled exception on the calling
        // thread (typically the UI thread, when invoked via ICommand.Execute from a bound
        // control). On some platforms (e.g. Mac Catalyst) the native runtime aborts the process
        // unconditionally after an unhandled exception, regardless of the framework's global
        // exception handling. Catch and route through CommandExceptionDispatcher instead, so
        // platform Application classes can forward it to IExceptionHandlerService and show the
        // existing friendly error dialog, matching the behavior of async command failures.
        Exception? caughtException = null;

        lock (_syncLock)
        {
            try
            {
                _execute?.Invoke();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        }

        if (caughtException is not null)
            CommandExceptionDispatcher.Raise(this, caughtException);
    }
}
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

        // For synchronous operations, consider using Task.Run if the operation is long-running
        // This is a simple command, so we execute directly
        lock (_syncLock)
        {
            if (_execute != null)
                _execute();
        }
    }
}
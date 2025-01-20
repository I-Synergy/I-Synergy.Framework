using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Commands.Base;
using System.Runtime.CompilerServices;

#nullable enable

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
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
        : this(execute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanExecute(object? parameter)
    {
        ThrowIfDisposed();
        return _canExecute?.Invoke() != false;
    }

    public override void Execute(object? parameter)
    {
        ThrowIfDisposed();

        try
        {
            _execute();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }
}
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Commands.Base;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A generic _command whose sole purpose is to relay its functionality to other
/// objects by invoking delegates. The default return value for the CanExecute
/// method is <see langword="true"/>. This class allows you to accept _command parameters
/// in the <see cref="Execute(T)"/> and <see cref="CanExecute(T)"/> callback methods.
/// </summary>
/// <typeparam name="T">The type of parameter being passed as input to the callbacks.</typeparam>
/// <summary>
/// A generic command whose sole purpose is to relay its functionality to other
/// objects by invoking delegates. The default return value for the CanExecute
/// method is <see langword="true"/>. This class allows you to accept command parameters
/// in the Execute(T) and CanExecute(T) callback methods.
/// </summary>
public sealed class RelayCommand<T> : BaseRelayCommand, IRelayCommand<T>
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    public RelayCommand(Action<T?> execute)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
        : this(execute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T? parameter)
    {
        ThrowIfDisposed();
        return _canExecute?.Invoke(parameter) != false;
    }

    public override bool CanExecute(object? parameter)
    {
        ThrowIfDisposed();

        if (!TryGetCommandArgument(parameter, out T? result))
        {
            if (parameter is not null)
                ThrowArgumentExceptionForInvalidCommandArgument(parameter);
        }

        return CanExecute(result);
    }

    public void Execute(T? parameter)
    {
        ThrowIfDisposed();
        _execute(parameter);
    }

    public override void Execute(object? parameter)
    {
        ThrowIfDisposed();

        if (!TryGetCommandArgument(parameter, out T? result))
            ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        Execute(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryGetCommandArgument(object? parameter, out T? result)
    {
        if (parameter is null && default(T) is null)
        {
            result = default;
            return true;
        }

        if (parameter is T argument)
        {
            result = argument;
            return true;
        }

        result = default;
        return false;
    }

    internal static void ThrowArgumentExceptionForInvalidCommandArgument(object? parameter)
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        static Exception GetException(object? parameter)
        {
            if (parameter is null)
                return new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) must not be null, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));

            return new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) cannot be of type {parameter.GetType()}, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
        }

        throw GetException(parameter);
    }
}

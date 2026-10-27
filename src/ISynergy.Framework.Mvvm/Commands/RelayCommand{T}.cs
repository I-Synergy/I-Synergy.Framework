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
        _execute = Argument.IsNotNull(execute);
    }

    public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
        : this(execute)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T? parameter)
    {
        ThrowIfDisposed();

        lock (_syncLock)
        {
            return _canExecute?.Invoke(parameter) != false;
        }
    }

    public override bool CanExecute(object? parameter)
    {
        ThrowIfDisposed();

        if (!TryGetCommandArgument(parameter, out T? result) && parameter is not null)
            ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        return CanExecute(result);
    }

    public void Execute(T? parameter)
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
                _execute(parameter);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        }

        if (caughtException is not null)
            CommandExceptionDispatcher.Raise(this, caughtException);
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
        // Fast path for null parameter — convert to default(T) for all types.
        // Value types get their default (false, 0, etc.), reference types get null.
        // This is required because MAUI Buttons pass null when no CommandParameter is set,
        // and the command's typed Execute(T?) already accepts nullable parameters.
        if (parameter is null)
        {
            result = default;
            return true;
        }

        // Fast path for exact type match
        if (parameter is T exactMatch)
        {
            result = exactMatch;
            return true;
        }

        // Slower path for type conversion (only executed if the above checks fail)
        try
        {
            if (parameter != null && parameter is T converted)
            {
                result = converted;
                return true;
            }
        }
        catch
        {
            // Conversion failed, fall through to default return
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

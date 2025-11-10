using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
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

        lock (_syncLock)
        {
            try
            {
                _execute(parameter);
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
        // Fast path for null parameter and nullable T
        if (parameter is null && default(T) is null)
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
            if (parameter != null && typeof(T).IsAssignableFrom(parameter.GetType()))
            {
                result = (T)parameter;
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

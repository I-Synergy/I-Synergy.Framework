using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using System.Runtime.CompilerServices;

#nullable enable

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
public sealed class RelayCommand<T> : IRelayCommand<T>, IDisposable
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    private bool _disposed;

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the RelayCommand{T} class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <exception cref="ArgumentNullException">Thrown if execute is null.</exception>
    public RelayCommand(Action<T?> execute)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    /// <summary>
    /// Initializes a new instance of the RelayCommand{T} class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="ArgumentNullException">Thrown if execute or canExecute are null.</exception>
    public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
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
    public bool CanExecute(T? parameter)
    {
        ThrowIfDisposed();
        return _canExecute?.Invoke(parameter) != false;
    }

    /// <inheritdoc/>
    public bool CanExecute(object? parameter)
    {
        ThrowIfDisposed();

        // Special case a null value for a value type argument type
        if (parameter is null && default(T) is not null)
            return false;

        if (!TryGetCommandArgument(parameter, out T? result))
            ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        return CanExecute(result);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Execute(T? parameter)
    {
        ThrowIfDisposed();

        try
        {
            _execute(parameter);
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

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        ThrowIfDisposed();

        if (!TryGetCommandArgument(parameter, out T? result))
            ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        Execute(result);
    }

    /// <summary>
    /// Tries to get a command argument of compatible type T from an input object.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryGetCommandArgument(object? parameter, out T? result)
    {
        // Handle null case for reference types or nullable value types
        if (parameter is null && default(T) is null)
        {
            result = default;
            return true;
        }

        // Check if the argument is a T value
        if (parameter is T argument)
        {
            result = argument;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Throws an ArgumentException if an invalid command argument is used.
    /// </summary>
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

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RelayCommand<T>));
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

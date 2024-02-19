using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A generic _command that provides a more specific version of <see cref="AsyncRelayCommand"/>.
/// </summary>
/// <typeparam name="T">The type of parameter being passed as input to the callbacks.</typeparam>
public sealed class AsyncRelayCommand<T> : IAsyncRelayCommand<T>, ICancellationAwareCommand, IDisposable
{
    private readonly List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
    private CancellationTokenSource? _cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// The <see cref="Func{TResult}"/> to invoke when <see cref="Execute(T)"/> is used.
    /// </summary>
    private readonly Func<T?, Task>? _execute;

    /// <summary>
    /// The cancelable <see cref="Func{T1,T2,TResult}"/> to invoke when <see cref="Execute(object?)"/> is used.
    /// </summary>
    private readonly Func<T?, CancellationToken, Task>? _cancelableExecute;

    /// <summary>
    /// The optional action to invoke when <see cref="CanExecute(T)"/> is used.
    /// </summary>
    private readonly Predicate<T?>? _canExecute;

    /// <summary>
    /// The _options being set for the current _command.
    /// </summary>
    private readonly AsyncRelayCommandOptions _options;

    private Task? _executionTask;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, Task> execute)
    {
        Argument.IsNotNull(execute);

        _execute = execute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="options">The _options to use to configure the async _command.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, Task> execute, AsyncRelayCommandOptions options)
        : this(execute)
    {
        _options = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, CancellationToken, Task> cancelableExecute)
    {
        Argument.IsNotNull(cancelableExecute);

        _cancelableExecute = cancelableExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="options">The _options to use to configure the async _command.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
        : this(cancelableExecute)
    {
        _options = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?> canExecute)
        : this(execute)
    {
        Argument.IsNotNull(canExecute);

        _canExecute = canExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <param name="options">The _options to use to configure the async _command.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?> canExecute, AsyncRelayCommandOptions options)
        : this(execute, canExecute)
    {
        _options = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, CancellationToken, Task> cancelableExecute, Predicate<T?> canExecute)
        : this(cancelableExecute)
    {
        Argument.IsNotNull(canExecute);

        _canExecute = canExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <param name="options">The _options to use to configure the async _command.</param>
    /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<T?, CancellationToken, Task> cancelableExecute, Predicate<T?> canExecute, AsyncRelayCommandOptions options)
        : this(cancelableExecute, canExecute)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public Task? ExecutionTask
    {
        get => _executionTask;
        private set
        {
            if (ReferenceEquals(_executionTask, value))
                return;

            _executionTask = value;

            PropertyChanged?.Invoke(this, AsyncRelayCommand.ExecutionTaskChangedEventArgs);
            PropertyChanged?.Invoke(this, AsyncRelayCommand.IsRunningChangedEventArgs);

            bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

            if (_cancellationTokenSource is not null)
            {
                PropertyChanged?.Invoke(this, AsyncRelayCommand.CanBeCanceledChangedEventArgs);
                PropertyChanged?.Invoke(this, AsyncRelayCommand.IsCancellationRequestedChangedEventArgs);
            }

            if (isAlreadyCompletedOrNull)
                return;

            void MonitorTask(AsyncRelayCommand<T> @this, Task task)
            {
                task.GetAwaitableWithoutEndValidation().GetAwaiter().GetResult();

                if (ReferenceEquals(@this._executionTask, task))
                {
                    @this.PropertyChanged?.Invoke(@this, AsyncRelayCommand.ExecutionTaskChangedEventArgs);
                    @this.PropertyChanged?.Invoke(@this, AsyncRelayCommand.IsRunningChangedEventArgs);

                    if (@this._cancellationTokenSource is not null)
                        @this.PropertyChanged?.Invoke(@this, AsyncRelayCommand.CanBeCanceledChangedEventArgs);

                    if ((@this._options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                        @this.CanExecuteChanged?.Invoke(@this, EventArgs.Empty);
                }
            }

            MonitorTask(this, value!);
        }
    }

    /// <inheritdoc/>
    public bool CanBeCanceled => IsRunning && _cancellationTokenSource is { IsCancellationRequested: false };

    /// <inheritdoc/>
    public bool IsCancellationRequested => _cancellationTokenSource is { IsCancellationRequested: true };

    /// <inheritdoc/>
    public bool IsRunning => ExecutionTask is { IsCompleted: false };

    /// <inheritdoc/>
    bool ICancellationAwareCommand.IsCancellationSupported => _execute is null;

    /// <inheritdoc/>
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T? parameter)
    {
        bool canExecute = _canExecute?.Invoke(parameter) != false;
        return canExecute && ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 || ExecutionTask is not { IsCompleted: false });
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object? parameter)
    {
        // Special case, see RelayCommand<T>.CanExecute(object?) for more info
        if (parameter is null && default(T) is not null)
            return false;

        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        return CanExecute(result);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Execute(T? parameter)
    {
        Task executionTask = ExecuteAsync(parameter);

        if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
            AsyncRelayCommand.AwaitAndThrowIfFailed(executionTask);
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        Execute(result);
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(T? parameter)
    {
        try
        {
            Task executionTask;

            if (_execute is not null)
            {
                // Non cancelable _command delegate
                executionTask = ExecutionTask = _execute(parameter);
            }
            else
            {
                // Cancel the previous operation, if one is pending
                _cancellationTokenSource?.Cancel();
                var cancellationTokenSource = _cancellationTokenSource = new();
                _cancellationTokenSources.Add(cancellationTokenSource);

                // Invoke the cancelable _command delegate with a new linked token
                executionTask = ExecutionTask = _cancelableExecute!(parameter, cancellationTokenSource.Token);
            }

            await executionTask;

            // If concurrent executions are disabled, notify the can _execute change as well
            if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            var exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

            if (ex.InnerException != null)
                await exceptionHandlerService.HandleExceptionAsync(ex.InnerException);
            else
                await exceptionHandlerService.HandleExceptionAsync(ex);
        }
    }

    /// <inheritdoc/>
    public Task ExecuteAsync(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        return ExecuteAsync(result);
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        if (_cancellationTokenSource is { IsCancellationRequested: false } cancellationTokenSource)
        {
            cancellationTokenSource.Cancel();

            PropertyChanged?.Invoke(this, AsyncRelayCommand.CanBeCanceledChangedEventArgs);
            PropertyChanged?.Invoke(this, AsyncRelayCommand.IsCancellationRequestedChangedEventArgs);
        }
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
            if (_cancellationTokenSources is not null)
            {
                foreach (var cancellationTokenSource in _cancellationTokenSources)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                }

                _cancellationTokenSources.Clear();
            }

            // free managed resources
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        // free native resources if there are any.
    }
    #endregion
}

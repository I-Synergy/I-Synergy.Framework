using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A generic _command that provides a more specific version of <see cref="AsyncRelayCommand"/>.
/// </summary>
/// <typeparam name="T">The type of parameter being passed as input to the callbacks.</typeparam>
public sealed class AsyncRelayCommand<T> : BaseAsyncRelayCommand, IAsyncRelayCommand<T>
{
    private readonly Func<T, Task>? _execute;
    private readonly Func<T, CancellationToken, Task>? _cancelableExecute;
    private readonly Predicate<T>? _canExecute;

    /// <summary>
    /// Gets a value indicating whether the command uses the base execute method.
    /// </summary>
    protected override bool IsBaseExecute => _execute is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    public AsyncRelayCommand(Func<T, Task> execute)
        : base(AsyncRelayCommandOptions.None)
    {
        _execute = Argument.IsNotNull(execute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<T, Task> execute, AsyncRelayCommandOptions options)
        : base(options)
    {
        _execute = Argument.IsNotNull(execute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute)
        : base(AsyncRelayCommandOptions.None)
    {
        _cancelableExecute = Argument.IsNotNull(cancelableExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
        : base(options)
    {
        _cancelableExecute = Argument.IsNotNull(cancelableExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute)
        : this(execute)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute, AsyncRelayCommandOptions options)
        : this(execute, options)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, Predicate<T> canExecute)
        : this(cancelableExecute)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, Predicate<T> canExecute, AsyncRelayCommandOptions options)
        : this(cancelableExecute, options)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Determines whether this command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T? parameter)
    {
        // Handle null parameter safely
        bool canExecutePredicate = _canExecute?.Invoke(parameter!) != false;

        return canExecutePredicate &&
            ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 ||
             ExecutionTask is not { IsCompleted: false });
    }

    /// <summary>
    /// Determines whether this command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public override bool CanExecute(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
        {
            if (parameter is not null)
                RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
        }

        return CanExecute(result);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public void Execute(T? parameter)
    {
        Task executionTask = ExecuteAsync(parameter);

        if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
            AwaitAndThrowIfFailed(executionTask);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public override void Execute(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        Execute(result);
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task ExecuteAsync(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        await ExecuteAsync(result);
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ExecuteAsync(T? parameter)
    {
        // Use a lock to ensure thread safety when checking and setting execution state
        bool canProceed;
        lock (_executionLock)
        {
            canProceed = !IsRunning || (_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0;
            if (!canProceed) return;
        }

        CancellationTokenSource? timeoutCts = null;
        CancellationTokenSource? linkedCts = null;

        try
        {
            Task executionTask;

            // Handle null parameter safely
            if (parameter == null)
            {
                // If parameter is null and we can't handle it, return early
                if (typeof(T).IsValueType && !typeof(T).IsGenericType)
                {
                    System.Diagnostics.Debug.WriteLine($"Cannot execute command with null parameter for non-nullable value type {typeof(T).Name}");
                    return;
                }
            }

            if (_execute is not null)
            {
                executionTask = _execute(parameter!);
            }
            else
            {
                lock (_executionLock)
                {
                    // Cancel any existing operation
                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource.Cancel();

                    var cancellationTokenSource = _cancellationTokenSource = new CancellationTokenSource();

                    // Create a timeout token source
                    timeoutCts = new CancellationTokenSource(_defaultTimeout);

                    // Link the timeout and operation token sources
                    linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                        cancellationTokenSource.Token, timeoutCts.Token);

                    _cancellationTokenSources.Add(cancellationTokenSource);

                    // Clean up completed token sources to prevent memory leaks
                    CleanupCompletedTokenSources();
                }

                executionTask = _cancelableExecute!(parameter!, linkedCts.Token);
            }

            ExecutionTask = executionTask;

            try
            {
                await executionTask;
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
                System.Diagnostics.Debug.WriteLine("Command execution was canceled.");

                // Re-throw if configured to flow exceptions
                if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) != 0)
                    throw;
            }
            catch (TimeoutException)
            {
                // Handle timeouts specifically
                System.Diagnostics.Debug.WriteLine("Command execution timed out.");

                // Re-throw if configured to flow exceptions
                if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) != 0)
                    throw;
            }
        }
        finally
        {
            // Dispose of timeout-related resources
            timeoutCts?.Dispose();
            linkedCts?.Dispose();

            ExecutionTask = null;

            if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                OnCanExecuteChanged();
        }
    }
}

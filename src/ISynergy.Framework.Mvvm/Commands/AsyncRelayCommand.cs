using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
/// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the execute
/// action, and providing an ExecutionTask property that notifies changes when
/// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
/// </summary>
public sealed class AsyncRelayCommand : BaseAsyncRelayCommand
{
    private readonly Func<Task>? _execute;
    private readonly Func<CancellationToken, Task>? _cancelableExecute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Gets a value indicating whether the command uses the base execute method.
    /// </summary>
    protected override bool IsBaseExecute => _execute is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    public AsyncRelayCommand(Func<Task> execute)
        : base(AsyncRelayCommandOptions.None)
    {
        _execute = Argument.IsNotNull(execute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<Task> execute, AsyncRelayCommandOptions options)
        : base(options)
    {
        _execute = Argument.IsNotNull(execute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute)
        : base(AsyncRelayCommandOptions.None)
    {
        _cancelableExecute = Argument.IsNotNull(cancelableExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
        : base(options)
    {
        _cancelableExecute = Argument.IsNotNull(cancelableExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        : this(execute)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, AsyncRelayCommandOptions options)
        : this(execute, options)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
        : this(cancelableExecute)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="canExecute">The can execute function.</param>
    /// <param name="options">Command options.</param>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, AsyncRelayCommandOptions options)
        : this(cancelableExecute, options)
    {
        _canExecute = Argument.IsNotNull(canExecute);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class with a custom timeout.
    /// </summary>
    /// <param name="execute">The execution function.</param>
    /// <param name="timeout">The timeout for command execution.</param>
    public AsyncRelayCommand(Func<Task> execute, TimeSpan timeout)
        : base(AsyncRelayCommandOptions.None)
    {
        _execute = Argument.IsNotNull(execute);
        _defaultTimeout = timeout;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class with a custom timeout.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution function.</param>
    /// <param name="timeout">The timeout for command execution.</param>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, TimeSpan timeout)
        : base(AsyncRelayCommandOptions.None)
    {
        _cancelableExecute = Argument.IsNotNull(cancelableExecute);
        _defaultTimeout = timeout;
    }

    /// <summary>
    /// Determines whether this command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanExecute(object? parameter)
    {
        var canExecute = _canExecute?.Invoke() != false;
        return canExecute && ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 || ExecutionTask is not { IsCompleted: false });
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
    public override void Execute(object? parameter)
    {
        Task executionTask = ExecuteAsync(parameter);

        if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
            AwaitAndThrowIfFailed(executionTask);
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task ExecuteAsync(object? parameter)
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

            if (_execute is not null)
            {
                executionTask = _execute();
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

                executionTask = _cancelableExecute!(linkedCts.Token);
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

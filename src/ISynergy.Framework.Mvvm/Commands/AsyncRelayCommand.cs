using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
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
        lock (_syncLock)
        {
            canProceed = !IsRunning || (_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0;
            if (!canProceed) return;
        }

        CancellationTokenSource? linkedCts = null;
        bool hasStarted = false;

        try
        {
            Task executionTask;

            if (_execute is not null)
            {
                executionTask = _execute();
            }
            else
            {
                lock (_syncLock)
                {
                    // Cancel any existing operation
                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource.Cancel();

                    // Create a new cancellation token source for this execution
                    _cancellationTokenSource = new CancellationTokenSource();

                    linkedCts = CreateTimeoutTokenSource();

                    // Add to tracking collection for cleanup
                    _cancellationTokenSources.Add(_cancellationTokenSource);
                }

                // Use the linked token for execution
                executionTask = _cancelableExecute!(linkedCts.Token);
            }

            // Setting this flag before ExecutionTask to ensure we reset state even if property setter throws
            hasStarted = true;
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
            catch (TimeoutException ex)
            {
                // Only handle exceptions if not configured to flow them to the task scheduler
                if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
                {
                    var exceptionHandlerService = ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>();

                    if (ex.InnerException is not null)
                        await exceptionHandlerService.HandleExceptionAsync(ex.InnerException);
                    else
                        await exceptionHandlerService.HandleExceptionAsync(ex);

                    System.Diagnostics.Debug.WriteLine($"Command execution timed out: {ex.Message}");
                }
                else
                {
                    // Re-throw if configured to flow exceptions
                    System.Diagnostics.Debug.WriteLine($"Command execution timed out: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Only handle exceptions if not configured to flow them to the task scheduler
                if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
                {
                    var exceptionHandlerService = ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>();

                    if (ex.InnerException is not null)
                        await exceptionHandlerService.HandleExceptionAsync(ex.InnerException);
                    else
                        await exceptionHandlerService.HandleExceptionAsync(ex);

                    System.Diagnostics.Debug.WriteLine($"Command execution failed: {ex.Message}");
                }
                else
                {
                    // Re-throw if configured to flow exceptions
                    System.Diagnostics.Debug.WriteLine($"Command execution failed: {ex.Message}");
                    throw;
                }
            }
        }
        finally
        {
            // Always ensure we reset the execution state, especially after exceptions
            lock (_syncLock)
            {
                // Only clear ExecutionTask if we set it in the first place
                if (hasStarted)
                {
                    ExecutionTask = null;
                }
            }

            // Update CanExecute state if needed
            if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                OnCanExecuteChanged();
        }
    }
}

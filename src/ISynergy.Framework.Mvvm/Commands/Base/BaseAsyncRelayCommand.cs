using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Commands.Base;

/// <summary>
/// Base class for async relay commands providing common functionality.
/// Implements <see cref="IAsyncRelayCommand"/>, <see cref="ICancellationAwareCommand"/>, and <see cref="IDisposable"/> interfaces.
/// </summary>
public abstract class BaseAsyncRelayCommand : IAsyncRelayCommand, ICancellationAwareCommand, IDisposable
{
    /// <summary>
    /// Lock object for thread synchronization.
    /// </summary>
    protected readonly object _syncLock = new object();

    /// <summary>
    /// List of cancellation token sources for tracking and cleanup.
    /// </summary>
    protected readonly List<CancellationTokenSource> _cancellationTokenSources = new();

    /// <summary>
    /// Current cancellation token source for the executing command.
    /// </summary>
    protected CancellationTokenSource? _cancellationTokenSource = new();

    /// <summary>
    /// Current execution task.
    /// </summary>
    protected Task? _executionTask;

    /// <summary>
    /// Command options.
    /// </summary>
    protected readonly AsyncRelayCommandOptions _options;

    /// <summary>
    /// Default timeout for command execution.
    /// </summary>
    protected TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Flag indicating whether the command has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// List of task continuations to be canceled on disposal.
    /// </summary>
    private readonly List<CancellationTokenSource> _continuationCancellations = new();

    /// <summary>
    /// Event handler for the CanExecuteChanged event.
    /// </summary>
    private EventHandler? _canExecuteChanged;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            lock (_syncLock)
            {
                if (_isDisposed) return;
                _canExecuteChanged += value;
            }
        }
        remove
        {
            lock (_syncLock)
            {
                if (_isDisposed) return;
                _canExecuteChanged -= value;
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    protected void OnCanExecuteChanged()
    {
        EventHandler? handler;
        lock (_syncLock)
        {
            if (_isDisposed) return;
            handler = _canExecuteChanged;
        }

        handler?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// The cached PropertyChangedEventArgs for common properties.
    /// </summary>
    internal static readonly PropertyChangedEventArgs ExecutionTaskChangedEventArgs = new(nameof(ExecutionTask));
    internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new(nameof(CanBeCanceled));
    internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new(nameof(IsCancellationRequested));
    internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new(nameof(IsRunning));

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="options">Command options.</param>
    protected BaseAsyncRelayCommand(AsyncRelayCommandOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Gets or sets the execution task.
    /// </summary>
    public Task? ExecutionTask
    {
        get
        {
            lock (_syncLock)
            {
                return _executionTask;
            }
        }
        protected set
        {
            Task? oldTask;
            bool valueChanged;

            lock (_syncLock)
            {
                if (_isDisposed) return;

                oldTask = _executionTask;
                valueChanged = !ReferenceEquals(_executionTask, value);
                if (!valueChanged) return;

                _executionTask = value;
            }

            // valueChanged is always true here (we return early above if !valueChanged) // NOSONAR
            // Collect all property changes to batch them
            var propertyChanges = new List<PropertyChangedEventArgs> { ExecutionTaskChangedEventArgs };

            // Only raise IsRunning changed if the running state actually changed
            bool wasRunning = oldTask is { IsCompleted: false };
            bool isNewTaskRunning = value is { IsCompleted: false };
            if (wasRunning != isNewTaskRunning)
            {
                propertyChanges.Add(IsRunningChangedEventArgs);
            }

            bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

            lock (_syncLock)
            {
                if (_cancellationTokenSource is not null)
                {
                    propertyChanges.Add(CanBeCanceledChangedEventArgs);
                    propertyChanges.Add(IsCancellationRequestedChangedEventArgs);
                }
            }

            // Batch raise all property changes
            RaisePropertyChangedBatch(propertyChanges.ToArray());

            // value is not null when !isAlreadyCompletedOrNull (null value would make it true) // NOSONAR
            if (!isAlreadyCompletedOrNull)
            {
                MonitorTask(value!);
            }
        }
    }

    /// <summary>
    /// Raises multiple property changed events at once.
    /// </summary>
    /// <param name="args">The property changed event args.</param>
    private void RaisePropertyChangedBatch(PropertyChangedEventArgs[] args)
    {
        PropertyChangedEventHandler? handler;
        lock (_syncLock)
        {
            if (_isDisposed) return;
            handler = PropertyChanged;
        }

        if (handler != null)
        {
            foreach (var arg in args)
            {
                handler.Invoke(this, arg);
            }
        }
    }

    /// <summary>
    /// Monitors a task for completion and updates properties accordingly.
    /// </summary>
    /// <param name="task">The task to monitor.</param>
    protected void MonitorTask(Task task) // NOSONAR
    {
        // Store a local reference to avoid race conditions
        var currentTask = task;

        // Rent pooled resources (weak reference + continuation token source)
        var weakThis = CommandExecutionResourcePool.RentWeakReference(this);
        var continuationCts = CommandExecutionResourcePool.RentContinuationTokenSource();

        lock (_syncLock)
        {
            if (_isDisposed)
            {
                // Return resources to pool
                CommandExecutionResourcePool.Return(weakThis, continuationCts);
                return;
            }

            _continuationCancellations.Add(continuationCts);
        }

        _ = currentTask.ContinueWith(t =>
        {
            // Check if continuation was canceled
            if (continuationCts.IsCancellationRequested)
            {
                CommandExecutionResourcePool.Return(weakThis, continuationCts);
                return;
            }

            // Use weak reference to avoid keeping the command alive if it's disposed
            if (!weakThis.TryGetTarget(out var target))
            {
                CommandExecutionResourcePool.Return(weakThis, continuationCts);
                return;
            }

            bool isCurrentTask;
            lock (target._syncLock)
            {
                if (target._isDisposed)
                {
                    CommandExecutionResourcePool.Return(weakThis, continuationCts);
                    return;
                }

                // Remove this continuation from the tracking list
                target._continuationCancellations.Remove(continuationCts);
                isCurrentTask = ReferenceEquals(target._executionTask, t);
            }

            if (isCurrentTask)
            {
                // Use UI thread dispatcher for property notifications
                var context = SynchronizationContext.Current;
                if (context != null)
                {
                    context.Post(_ =>
                    {
                        if (!weakThis.TryGetTarget(out var innerTarget))
                        {
                            CommandExecutionResourcePool.Return(weakThis, continuationCts);
                            return;
                        }

                        lock (innerTarget._syncLock)
                        {
                            if (innerTarget._isDisposed)
                            {
                                CommandExecutionResourcePool.Return(weakThis, continuationCts);
                                return;
                            }

                            // Only raise property changed events if this is still the current task
                            if (ReferenceEquals(innerTarget._executionTask, t))
                            {
                                var propertyChanges = new List<PropertyChangedEventArgs>
                                {
                                    ExecutionTaskChangedEventArgs,
                                    IsRunningChangedEventArgs
                                };

                                if (innerTarget._cancellationTokenSource is not null)
                                {
                                    propertyChanges.Add(CanBeCanceledChangedEventArgs);
                                }

                                innerTarget.RaisePropertyChangedBatch(propertyChanges.ToArray());

                                if ((innerTarget._options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                                {
                                    innerTarget.OnCanExecuteChanged();
                                }

                                // Clear the task reference immediately
                                innerTarget._executionTask = null;
                            }
                        }

                        CommandExecutionResourcePool.Return(weakThis, continuationCts);
                    }, null);
                }
                else
                {
                    // No synchronization context, update directly
                    lock (target._syncLock)
                    {
                        if (target._isDisposed)
                        {
                            CommandExecutionResourcePool.Return(weakThis, continuationCts);
                            return;
                        }

                        // Only raise property changed events if this is still the current task
                        if (ReferenceEquals(target._executionTask, t))
                        {
                            var propertyChanges = new List<PropertyChangedEventArgs>
                            {
                                ExecutionTaskChangedEventArgs,
                                IsRunningChangedEventArgs
                            };

                            if (target._cancellationTokenSource is not null)
                            {
                                propertyChanges.Add(CanBeCanceledChangedEventArgs);
                            }

                            target.RaisePropertyChangedBatch(propertyChanges.ToArray());

                            if ((target._options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                            {
                                target.OnCanExecuteChanged();
                            }

                            // Clear the task reference immediately
                            target._executionTask = null;
                        }
                    }

                    CommandExecutionResourcePool.Return(weakThis, continuationCts);
                }
            }
            else
            {
                CommandExecutionResourcePool.Return(weakThis, continuationCts);
            }
        }, continuationCts.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    /// <summary>
    /// Gets a value indicating whether the command can be canceled.
    /// </summary>
    public bool CanBeCanceled
    {
        get
        {
            lock (_syncLock)
            {
                bool isRunning = IsRunning;
                return isRunning && _cancellationTokenSource is { IsCancellationRequested: false };
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether cancellation has been requested.
    /// </summary>
    public bool IsCancellationRequested
    {
        get
        {
            lock (_syncLock)
            {
                return _cancellationTokenSource is { IsCancellationRequested: true };
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the command is currently running.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (_syncLock)
            {
                return ExecutionTask is { IsCompleted: false };
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether cancellation is supported.
    /// </summary>
    bool ICancellationAwareCommand.IsCancellationSupported => !IsBaseExecute;

    /// <summary>
    /// Gets a value indicating whether the command uses the base execute method.
    /// </summary>
    protected abstract bool IsBaseExecute { get; }

    /// <summary>
    /// For testing purposes only - indicates if there are active cancellation tokens.
    /// </summary>
    internal bool HasActiveCancellationTokens
    {
        get
        {
            lock (_syncLock)
            {
                return _cancellationTokenSources.Count > 0;
            }
        }
    }

    /// <summary>
    /// For testing purposes only - gets the count of active continuations.
    /// </summary>
    internal int ContinuationCount
    {
        get
        {
            lock (_syncLock)
            {
                return _continuationCancellations.Count;
            }
        }
    }

    /// <summary>
    /// Notifies that the command's ability to execute may have changed.
    /// </summary>
    public void NotifyCanExecuteChanged() => OnCanExecuteChanged();

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public abstract bool CanExecute(object? parameter);

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public abstract void Execute(object? parameter);

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public abstract Task ExecuteAsync(object? parameter);

    /// <summary>
    /// Checks whether the execution task is already faulted immediately after starting it (i.e.
    /// the execute delegate threw before its first "await") and, if so, routes the exception
    /// through <see cref="CommandExceptionDispatcher"/> instead of rethrowing it synchronously.
    /// </summary>
    /// <remarks>
    /// A genuinely asynchronous fault (thrown after an <c>await</c>) is not observed here: the
    /// task is not yet faulted at this point, so it is left to complete on its own and, if never
    /// awaited, becomes an unobserved task exception that platform <c>Application</c> classes
    /// already handle via <see cref="TaskScheduler.UnobservedTaskException"/>. But a delegate
    /// that throws before any <c>await</c> (or a purely synchronous delegate) makes the task
    /// already faulted by the time control returns here, on the calling (typically UI) thread.
    /// Rethrowing synchronously in that case would become a genuine unhandled exception on that
    /// thread, which on some platforms (e.g. Mac Catalyst) causes the native runtime to abort the
    /// process unconditionally, regardless of the framework's global exception handling. Routing
    /// through the dispatcher here avoids that crash while still surfacing the exception through
    /// the same friendly-dialog path as other command failures.
    /// </remarks>
    /// <param name="executionTask">The task to check.</param>
    /// <param name="sender">The command instance that started the task.</param>
    internal static void AwaitAndThrowIfFailed(Task executionTask, object? sender = null)
    {
        if (executionTask.IsFaulted)
        {
            // Accessing Exception marks it observed, preventing a duplicate report via
            // TaskScheduler.UnobservedTaskException during finalization.
            var exception = executionTask.Exception!.InnerException!;
            CommandExceptionDispatcher.Raise(sender, exception);
        }
    }

    /// <summary>
    /// Logs the exception from a failed task without propagating it.
    /// Used with AsyncRelayCommandOptions.None.
    /// </summary>
    /// <param name="executionTask">The task that may have faulted.</param>
    internal static void LogIfFailed(Task executionTask)
    {
        // Deliberately not `async void`: attach a fault-only continuation so the exception is
        // always observed and never tears down the process.
        _ = executionTask.ContinueWith(
            task => System.Diagnostics.Debug.WriteLine($"Command execution failed: {task.Exception?.GetBaseException().Message}"),
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Creates a timeout-aware cancellation token.
    /// </summary>
    /// <param name="timeout">Optional timeout duration. If not specified, the default timeout will be used.</param>
    /// <returns>A cancellation token source that will be canceled after the timeout period.</returns>
    protected CancellationTokenSource CreateTimeoutTokenSource(TimeSpan? timeout = null)
    {
        var timeoutDuration = timeout ?? _defaultTimeout;

        // For very short timeouts, always create a new token
        if (timeoutDuration < TimeSpan.FromSeconds(1))
        {
            var timeoutCts = new CancellationTokenSource(timeoutDuration);

            lock (_syncLock)
            {
                if (_isDisposed)
                {
                    timeoutCts.Dispose();
                    return new CancellationTokenSource();
                }

                _cancellationTokenSources.Add(timeoutCts);
                CleanupCompletedTokenSources();
            }

            return timeoutCts;
        }

        // For common timeout values, use the cache
        // Round the timeout to the nearest second to improve cache hits
        var roundedTimeout = TimeSpan.FromSeconds(Math.Round(timeoutDuration.TotalSeconds));

        var sharedTimeoutCts = CommandExecutionResourcePool.GetOrCreateTimeoutTokenSource(roundedTimeout);

        try
        {
            // FIXED: Link with the command's cancellation token source directly
            // This ensures cancellation propagates correctly when Cancel() is called
            var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource!.Token, sharedTimeoutCts.Token);

            lock (_syncLock)
            {
                if (_isDisposed)
                {
                    combinedCts.Dispose();
                    return new CancellationTokenSource();
                }

                _cancellationTokenSources.Add(combinedCts);
                CleanupCompletedTokenSources();
            }

            return combinedCts;
        }
        catch (ObjectDisposedException)
        {
            // If the shared token was disposed, remove it from cache and try again
            CommandExecutionResourcePool.RemoveTimeoutTokenSource(roundedTimeout);
            return CreateTimeoutTokenSource(timeout);
        }
    }


    /// <summary>
    /// Cancels the current command execution.
    /// </summary>
    public void Cancel()
    {
        CancellationTokenSource? tokenSource;
        List<CancellationTokenSource> tokenSources;

        lock (_syncLock)
        {
            if (_isDisposed) return;
            tokenSource = _cancellationTokenSource;

            // Make a copy of the token sources collection to avoid modification during enumeration
            tokenSources = new List<CancellationTokenSource>(_cancellationTokenSources);
        }

        // Cancel all active token sources to ensure all linked tokens are canceled
        foreach (var cts in tokenSources)
        {
            try
            {
                if (!cts.IsCancellationRequested)
                    cts.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Token was already disposed, ignore
            }
        }

        if (tokenSource is { IsCancellationRequested: false })
        {
            try
            {
                tokenSource.Cancel();

                var propertyChanges = new[]
                {
                CanBeCanceledChangedEventArgs,
                IsCancellationRequestedChangedEventArgs
            };

                RaisePropertyChangedBatch(propertyChanges);
            }
            catch (ObjectDisposedException)
            {
                // Token was already disposed, ignore
            }
        }
    }


    /// <summary>
    /// Disposes all cancellation token sources.
    /// </summary>
    protected void DisposeCancellationTokens() // NOSONAR
    {
        lock (_syncLock)
        {
            // Cancel and dispose all continuation cancellation tokens
            foreach (var cts in _continuationCancellations)
            {
                try
                {
                    cts.Cancel();
                    cts.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Token was already disposed, ignore
                }
            }
            _continuationCancellations.Clear();

            // Cancel and dispose all command cancellation tokens
            if (_cancellationTokenSources is not null)
            {
                foreach (var cts in _cancellationTokenSources.EnsureNotNull())
                {
                    try
                    {
                        if (!cts.IsCancellationRequested)
                            cts.Cancel();

                        cts.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Token was already disposed, ignore
                    }
                    catch (Exception ex)
                    {
                        // Log any other exceptions but continue cleanup
                        System.Diagnostics.Debug.WriteLine($"Error disposing cancellation token: {ex.Message}");
                    }
                }

                _cancellationTokenSources.Clear();
            }

            // Cancel and dispose the current cancellation token
            if (_cancellationTokenSource is not null)
            {
                try
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                        _cancellationTokenSource.Cancel();

                    _cancellationTokenSource.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Token was already disposed, ignore
                }
                catch (Exception ex)
                {
                    // Log any other exceptions
                    System.Diagnostics.Debug.WriteLine($"Error disposing current cancellation token: {ex.Message}");
                }
                _cancellationTokenSource = null;
            }
        }
    }

    /// <summary>
    /// Disposes the resources used by this command.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources used by this command.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing) // NOSONAR
    {
        if (disposing)
        {
            bool shouldDispose;

            lock (_syncLock)
            {
                shouldDispose = !_isDisposed;
                _isDisposed = true;
            }

            if (shouldDispose)
            {
                // Cancel all pending task continuations and dispose tokens
                DisposeCancellationTokens();

                // Dispose the execution task if it's completed
                // Note: We only dispose tasks in final states (RanToCompletion, Canceled, Faulted)
                // because disposing a running task is unsafe and can cause exceptions.
                // Tasks that are still running will be cleaned up by the GC when they complete,
                // and the continuation in MonitorTask will clear the reference when the task finishes.
                Task? taskToDispose = null;

                lock (_syncLock)
                {
                    // Check all possible final states for a Task
                    if (_executionTask != null)
                    {
                        var status = _executionTask.Status;
                        if (status == TaskStatus.RanToCompletion ||
                            status == TaskStatus.Canceled ||
                            status == TaskStatus.Faulted)
                        {
                            taskToDispose = _executionTask;
                            _executionTask = null;
                        }
                        // If task is still running, the continuation in MonitorTask will handle cleanup
                    }
                }

                if (taskToDispose != null)
                {
                    try
                    {
                        taskToDispose.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // Log any exceptions during task disposal
                        System.Diagnostics.Debug.WriteLine($"Error disposing task: {ex.Message}");
                    }
                }

                // Clear event handlers to prevent memory leaks
                lock (_syncLock)
                {
                    _canExecuteChanged = null;
                    PropertyChanged = null;
                }
            }
        }
    }

    /// <summary>
    /// Cleans up completed token sources to prevent memory leaks.
    /// </summary>
    protected void CleanupCompletedTokenSources()
    {
        // Keep only the most recent token sources (e.g., last 5) to prevent unbounded growth
        const int maxTokenSourcesToKeep = 5;

        lock (_syncLock)
        {
            if (_cancellationTokenSources.Count > maxTokenSourcesToKeep)
            {
                int countToRemove = _cancellationTokenSources.Count - maxTokenSourcesToKeep;
                for (int i = 0; i < countToRemove; i++)
                {
                    var cts = _cancellationTokenSources[0];
                    _cancellationTokenSources.RemoveAt(0);

                    try
                    {
                        if (!cts.IsCancellationRequested)
                            cts.Cancel();
                        cts.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Already disposed, ignore
                    }
                }
            }
        }
    }

    /// <summary>
    /// Cleans up static resources when the application is shutting down
    /// </summary>
    public static void CleanupStaticResources()
    {
        CommandExecutionResourcePool.Cleanup();
    }
}

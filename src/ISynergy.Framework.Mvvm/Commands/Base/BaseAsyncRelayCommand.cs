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
    protected readonly object _executionLock = new object();

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
            lock (_executionLock)
            {
                if (_isDisposed) return;
                _canExecuteChanged += value;
            }
        }
        remove
        {
            lock (_executionLock)
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
        lock (_executionLock)
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
            lock (_executionLock)
            {
                return _executionTask;
            }
        }
        protected set
        {
            Task? oldTask;
            bool valueChanged;

            lock (_executionLock)
            {
                if (_isDisposed) return;

                oldTask = _executionTask;
                valueChanged = !ReferenceEquals(_executionTask, value);
                if (!valueChanged) return;

                _executionTask = value;
            }

            if (valueChanged)
            {
                // Only raise property changed events if the value actually changed
                RaisePropertyChanged(ExecutionTaskChangedEventArgs);

                // Only raise IsRunning changed if the running state actually changed
                bool wasRunning = oldTask is { IsCompleted: false };
                bool isRunning = value is { IsCompleted: false };
                if (wasRunning != isRunning)
                {
                    RaisePropertyChanged(IsRunningChangedEventArgs);
                }

                bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

                if (_cancellationTokenSource is not null)
                {
                    RaisePropertyChanged(CanBeCanceledChangedEventArgs);
                    RaisePropertyChanged(IsCancellationRequestedChangedEventArgs);
                }

                if (!isAlreadyCompletedOrNull && value is not null)
                {
                    MonitorTask(value);
                }
            }
        }
    }

    /// <summary>
    /// Raises a property changed event.
    /// </summary>
    /// <param name="args">The property changed event args.</param>
    private void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChangedEventHandler? handler;
        lock (_executionLock)
        {
            if (_isDisposed) return;
            handler = PropertyChanged;
        }

        handler?.Invoke(this, args);
    }

    /// <summary>
    /// Monitors a task for completion and updates properties accordingly.
    /// </summary>
    /// <param name="task">The task to monitor.</param>
    protected void MonitorTask(Task task)
    {
        // Store a local reference to avoid race conditions
        var currentTask = task;
        var weakThis = new WeakReference<BaseAsyncRelayCommand>(this);

        // Create a cancellation token source for this continuation
        var continuationCts = new CancellationTokenSource();

        lock (_executionLock)
        {
            if (_isDisposed)
            {
                continuationCts.Dispose();
                return;
            }

            _continuationCancellations.Add(continuationCts);
        }

        _ = currentTask.ContinueWith(t =>
        {
            // Check if continuation was canceled
            if (continuationCts.IsCancellationRequested)
            {
                return;
            }

            // Use weak reference to avoid keeping the command alive if it's disposed
            if (!weakThis.TryGetTarget(out var target))
            {
                return;
            }

            bool isCurrentTask;
            lock (target._executionLock)
            {
                if (target._isDisposed)
                {
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
                            return;
                        }

                        lock (innerTarget._executionLock)
                        {
                            if (innerTarget._isDisposed)
                            {
                                return;
                            }

                            // Only raise property changed events if this is still the current task
                            if (ReferenceEquals(innerTarget._executionTask, t))
                            {
                                innerTarget.RaisePropertyChanged(ExecutionTaskChangedEventArgs);
                                innerTarget.RaisePropertyChanged(IsRunningChangedEventArgs);

                                if (innerTarget._cancellationTokenSource is not null)
                                {
                                    innerTarget.RaisePropertyChanged(CanBeCanceledChangedEventArgs);
                                }

                                if ((innerTarget._options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                                {
                                    innerTarget.OnCanExecuteChanged();
                                }

                                // Clear the task reference immediately
                                innerTarget._executionTask = null;
                            }
                        }
                    }, null);
                }
                else
                {
                    // No synchronization context, update directly
                    lock (target._executionLock)
                    {
                        if (target._isDisposed)
                        {
                            return;
                        }

                        // Only raise property changed events if this is still the current task
                        if (ReferenceEquals(target._executionTask, t))
                        {
                            target.RaisePropertyChanged(ExecutionTaskChangedEventArgs);
                            target.RaisePropertyChanged(IsRunningChangedEventArgs);

                            if (target._cancellationTokenSource is not null)
                            {
                                target.RaisePropertyChanged(CanBeCanceledChangedEventArgs);
                            }

                            if ((target._options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                            {
                                target.OnCanExecuteChanged();
                            }

                            // Clear the task reference immediately
                            target._executionTask = null;
                        }
                    }
                }
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
            lock (_executionLock)
            {
                return IsRunning && _cancellationTokenSource is { IsCancellationRequested: false };
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
            lock (_executionLock)
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
            lock (_executionLock)
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
    /// Awaits a task and handles exceptions.
    /// </summary>
    /// <param name="executionTask">The task to await.</param>
    internal static async void AwaitAndThrowIfFailed(Task executionTask)
    {
        try
        {
            await executionTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Silently handle cancellations
            System.Diagnostics.Debug.WriteLine("Command execution was canceled.");
        }
        catch (TimeoutException)
        {
            // Handle timeouts specifically
            System.Diagnostics.Debug.WriteLine("Command execution timed out.");
            throw; // Re-throw timeout exceptions by default
        }
        catch (Exception ex)
        {
            // Log the exception or handle it according to application policy
            System.Diagnostics.Debug.WriteLine($"Command execution failed: {ex.Message}");

            // Re-throw to maintain original behavior
            throw;
        }
    }

    /// <summary>
    /// Creates a timeout-aware cancellation token.
    /// </summary>
    /// <param name="timeout">Optional timeout duration. If not specified, the default timeout will be used.</param>
    /// <returns>A cancellation token source that will be canceled after the timeout period.</returns>
    protected CancellationTokenSource CreateTimeoutTokenSource(TimeSpan? timeout = null)
    {
        var timeoutDuration = timeout ?? _defaultTimeout;
        var timeoutCts = new CancellationTokenSource(timeoutDuration);

        lock (_executionLock)
        {
            if (_isDisposed)
            {
                timeoutCts.Dispose();
                return new CancellationTokenSource(); // Return a fresh CTS that will be disposed later
            }

            _cancellationTokenSources.Add(timeoutCts);
        }

        return timeoutCts;
    }

    /// <summary>
    /// Cancels the current command execution.
    /// </summary>
    public void Cancel()
    {
        CancellationTokenSource? tokenSource;

        lock (_executionLock)
        {
            if (_isDisposed) return;
            tokenSource = _cancellationTokenSource;
        }

        if (tokenSource is { IsCancellationRequested: false })
        {
            try
            {
                tokenSource.Cancel();
                RaisePropertyChanged(CanBeCanceledChangedEventArgs);
                RaisePropertyChanged(IsCancellationRequestedChangedEventArgs);
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
    protected void DisposeCancellationTokens()
    {
        lock (_executionLock)
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
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (_executionLock)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                // Cancel all pending task continuations
                foreach (var cts in _continuationCancellations)
                {
                    try
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Already disposed, ignore
                    }
                }
                _continuationCancellations.Clear();

                // Dispose all cancellation tokens
                DisposeCancellationTokens();

                // Dispose the execution task if it's completed
                if (_executionTask?.Status == TaskStatus.RanToCompletion ||
                    _executionTask?.Status == TaskStatus.Canceled ||
                    _executionTask?.Status == TaskStatus.Faulted)
                {
                    try
                    {
                        _executionTask.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // Log any exceptions during task disposal
                        System.Diagnostics.Debug.WriteLine($"Error disposing task: {ex.Message}");
                    }
                    _executionTask = null;
                }

                // Clear event handlers to prevent memory leaks
                _canExecuteChanged = null;
                PropertyChanged = null;
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

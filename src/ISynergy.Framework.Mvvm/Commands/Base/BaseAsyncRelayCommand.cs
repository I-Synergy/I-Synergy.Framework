using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Commands.Base;
/// <summary>
/// Base class for async relay commands providing common functionality.
/// </summary>
public abstract class BaseAsyncRelayCommand : IAsyncRelayCommand, ICancellationAwareCommand, IDisposable
{
    protected readonly List<CancellationTokenSource> _cancellationTokenSources = new();
    protected CancellationTokenSource? _cancellationTokenSource = new();
    protected Task? _executionTask;
    protected readonly AsyncRelayCommandOptions _options;

    private EventHandler? _canExecuteChanged;

    public event EventHandler? CanExecuteChanged
    {
        add => _canExecuteChanged += value;
        remove => _canExecuteChanged -= value;
    }

    protected void OnCanExecuteChanged()
    {
        _canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// The cached PropertyChangedEventArgs for common properties.
    /// </summary>
    internal static readonly PropertyChangedEventArgs ExecutionTaskChangedEventArgs = new(nameof(ExecutionTask));
    internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new(nameof(CanBeCanceled));
    internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new(nameof(IsCancellationRequested));
    internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new(nameof(IsRunning));

    public event PropertyChangedEventHandler? PropertyChanged;

    protected BaseAsyncRelayCommand(AsyncRelayCommandOptions options)
    {
        _options = options;
    }

    public Task? ExecutionTask
    {
        get => _executionTask;
        protected set
        {
            if (ReferenceEquals(_executionTask, value))
                return;

            _executionTask = value;

            PropertyChanged?.Invoke(this, ExecutionTaskChangedEventArgs);
            PropertyChanged?.Invoke(this, IsRunningChangedEventArgs);

            bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

            if (_cancellationTokenSource is not null)
            {
                PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
                PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
            }

            if (!isAlreadyCompletedOrNull && value is not null)
            {
                MonitorTask(value);
            }
        }
    }

    protected void MonitorTask(Task task)
    {
        _ = task.ContinueWith(t =>
        {
            if (ReferenceEquals(_executionTask, t))
            {
                // Use UI thread dispatcher for property notifications
                SynchronizationContext.Current?.Post(_ =>
                {
                    PropertyChanged?.Invoke(this, ExecutionTaskChangedEventArgs);
                    PropertyChanged?.Invoke(this, IsRunningChangedEventArgs);

                    if (_cancellationTokenSource is not null)
                        PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);

                    if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                        OnCanExecuteChanged();
                }, null);

                Task.Delay(100).ContinueWith(_ =>
                {
                    if (ReferenceEquals(_executionTask, t))
                    {
                        SynchronizationContext.Current?.Post(__ =>
                        {
                            _executionTask = null;
                        }, null);
                    }
                }, TaskScheduler.Current);
            }
        }, TaskScheduler.Current);
    }

    public bool CanBeCanceled => IsRunning && _cancellationTokenSource is { IsCancellationRequested: false };
    public bool IsCancellationRequested => _cancellationTokenSource is { IsCancellationRequested: true };
    public bool IsRunning => ExecutionTask is { IsCompleted: false };
    bool ICancellationAwareCommand.IsCancellationSupported => !IsBaseExecute;

    protected abstract bool IsBaseExecute { get; }

    public void NotifyCanExecuteChanged() => OnCanExecuteChanged();

    public abstract bool CanExecute(object? parameter);
    public abstract void Execute(object? parameter);
    public abstract Task ExecuteAsync(object? parameter);

    internal static async void AwaitAndThrowIfFailed(Task executionTask)
    {
        await executionTask;
    }

    public void Cancel()
    {
        if (_cancellationTokenSource is { IsCancellationRequested: false } cancellationTokenSource)
        {
            cancellationTokenSource.Cancel();

            PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
            PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
        }
    }

    protected void DisposeCancellationTokens()
    {
        if (_cancellationTokenSources is not null)
        {
            foreach (var cts in _cancellationTokenSources.EnsureNotNull())
            {
                cts.Cancel();
                cts.Dispose();
            }

            _cancellationTokenSources.Clear();
        }

        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeCancellationTokens();

            if (_executionTask?.Status == TaskStatus.RanToCompletion ||
                _executionTask?.Status == TaskStatus.Canceled ||
                _executionTask?.Status == TaskStatus.Faulted)
            {
                _executionTask.Dispose();
                _executionTask = null;
            }
        }
    }
}
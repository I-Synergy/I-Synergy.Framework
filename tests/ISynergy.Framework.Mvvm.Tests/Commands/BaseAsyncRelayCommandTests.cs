using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

/// <summary>
/// Tests targeting the shared <see cref="BaseAsyncRelayCommand"/> lifecycle logic (property-change
/// batching, task monitoring, cancellation bookkeeping and disposal) through a minimal subclass that
/// exposes the protected members.
/// </summary>
[TestClass]
public class BaseAsyncRelayCommandTests
{
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// Minimal <see cref="BaseAsyncRelayCommand"/> implementation that surfaces the protected members
    /// exercised by these tests.
    /// </summary>
    private sealed class TestCommand : BaseAsyncRelayCommand
    {
        private readonly Func<Task>? _execute;
        private readonly Func<CancellationToken, Task>? _cancelableExecute;

        public TestCommand(
            AsyncRelayCommandOptions options = AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler,
            Func<Task>? execute = null,
            Func<CancellationToken, Task>? cancelableExecute = null)
            : base(options)
        {
            _execute = execute;
            _cancelableExecute = cancelableExecute;
        }

        protected override bool IsBaseExecute => _execute is not null;

        public override bool CanExecute(object? parameter) => ExecutionTask is not { IsCompleted: false };

        public override void Execute(object? parameter)
        {
            if (_execute is not null)
                ExecutionTask = _execute();
        }

        public override Task ExecuteAsync(object? parameter) =>
            _execute is not null
                ? _execute()
                : _cancelableExecute!(CancellationToken.None);

        public void Monitor(Task task) => MonitorTask(task);

        public void SetExecutionTask(Task? task) => ExecutionTask = task;

        public Task? CurrentExecutionTask => ExecutionTask;

        public CancellationTokenSource CreateTimeout(TimeSpan? timeout = null) => CreateTimeoutTokenSource(timeout);

        public void DisposeTokens() => DisposeCancellationTokens();

        public void CleanupTokens() => CleanupCompletedTokenSources();

        public void AddTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            lock (_syncLock)
                _cancellationTokenSources.Add(cancellationTokenSource);
        }

        public int TokenSourceCount
        {
            get
            {
                lock (_syncLock)
                    return _cancellationTokenSources.Count;
            }
        }

        public void DisposeNonDisposing() => Dispose(false);

        public void AddContinuationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            var field = typeof(BaseAsyncRelayCommand).GetField(
                "_continuationCancellations",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            var continuations = (List<CancellationTokenSource>)field.GetValue(this)!;
            continuations.Add(cancellationTokenSource);
        }

        public CancellationTokenSource? CurrentTokenSource
        {
            get
            {
                lock (_syncLock)
                    return _cancellationTokenSource;
            }
            set
            {
                lock (_syncLock)
                    _cancellationTokenSource = value;
            }
        }
    }

    /// <summary>
    /// Synchronization context that captures posted callbacks so they can be run deterministically.
    /// </summary>
    private sealed class ManualSynchronizationContext : SynchronizationContext
    {
        private readonly List<(SendOrPostCallback Callback, object? State)> _posted = new();

        public int PendingCount
        {
            get
            {
                lock (_posted)
                    return _posted.Count;
            }
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            lock (_posted)
                _posted.Add((d, state));
        }

        public void RunPending()
        {
            (SendOrPostCallback Callback, object? State)[] items;
            lock (_posted)
            {
                items = _posted.ToArray();
                _posted.Clear();
            }

            foreach (var (callback, state) in items)
                callback(state);
        }
    }

    /// <summary>
    /// Completed task whose disposal throws, used to exercise the defensive catch block in
    /// <c>DisposeExecutionTask</c>.
    /// </summary>
    private sealed class ThrowingCompletedTask : Task
    {
        public ThrowingCompletedTask()
            : base(static () => { })
        {
            RunSynchronously();
        }

        protected override void Dispose(bool disposing) => throw new InvalidOperationException("Task dispose failed");
    }

    private static async Task WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = 5000)
    {
        var start = Environment.TickCount64;
        while (!condition() && Environment.TickCount64 - start < timeoutMilliseconds)
            await Task.Delay(10);
    }

    #region CanExecuteChanged

    [TestMethod]
    public void CanExecuteChanged_AddAndRemoveHandler_OnlyInvokesWhileSubscribed()
    {
        // Arrange
        using var command = new TestCommand();
        var invocationCount = 0;
        void Handler(object? sender, EventArgs e) => invocationCount++;

        // Act - add
        command.CanExecuteChanged += Handler;
        command.NotifyCanExecuteChanged();

        // Assert - invoked
        Assert.AreEqual(1, invocationCount);

        // Act - remove (exercises the remove accessor)
        command.CanExecuteChanged -= Handler;
        command.NotifyCanExecuteChanged();

        // Assert - no longer invoked
        Assert.AreEqual(1, invocationCount);
    }

    [TestMethod]
    public void CanExecuteChanged_AfterDispose_AddRemoveAndNotifyAreNoOps()
    {
        // Arrange
        var command = new TestCommand();
        command.Dispose();
        var invocationCount = 0;
        void Handler(object? sender, EventArgs e) => invocationCount++;

        // Act & Assert - no exceptions and no notifications after disposal
        Exception? observed = null;
        try
        {
            command.CanExecuteChanged += Handler;
            command.CanExecuteChanged -= Handler;
            command.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        Assert.AreEqual(0, invocationCount);
    }

    #endregion

    #region ExecutionTask property change batching

    [TestMethod]
    public void ExecutionTask_SetRunningTask_RaisesBatchedPropertyChanges()
    {
        // Arrange
        using var command = new TestCommand();
        var tcs = new TaskCompletionSource();
        var propertyChanges = new List<string>();
        command.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        command.SetExecutionTask(tcs.Task);

        // Assert
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.ExecutionTask));
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.IsRunning));
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.CanBeCanceled));
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.IsCancellationRequested));
        Assert.IsTrue(command.IsRunning);
        Assert.AreEqual(1, command.ContinuationCount);

        tcs.SetResult();
    }

    [TestMethod]
    public void ExecutionTask_SetSameTaskTwice_DoesNotRaisePropertyChangedAgain()
    {
        // Arrange
        using var command = new TestCommand();
        var tcs = new TaskCompletionSource();
        command.SetExecutionTask(tcs.Task);
        var propertyChanges = new List<string>();
        command.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        command.SetExecutionTask(tcs.Task);

        // Assert
        Assert.AreEqual(0, propertyChanges.Count);
        Assert.AreEqual(1, command.ContinuationCount);

        tcs.SetResult();
    }

    [TestMethod]
    public void ExecutionTask_SetNullWhenAlreadyNull_DoesNotRaisePropertyChanged()
    {
        // Arrange
        using var command = new TestCommand();
        var propertyChanges = new List<string>();
        command.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        command.SetExecutionTask(null);

        // Assert
        Assert.AreEqual(0, propertyChanges.Count);
        Assert.IsNull(command.CurrentExecutionTask);
    }

    [TestMethod]
    public void ExecutionTask_ReplacedWhileRunning_DoesNotRepeatIsRunningNotification()
    {
        // Arrange
        using var command = new TestCommand();
        var first = new TaskCompletionSource();
        var second = new TaskCompletionSource();
        command.SetExecutionTask(first.Task);
        var propertyChanges = new List<string>();
        command.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName!);

        // Act - both tasks are pending, so IsRunning does not change
        command.SetExecutionTask(second.Task);

        // Assert
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.ExecutionTask));
        CollectionAssert.DoesNotContain(propertyChanges, nameof(IAsyncRelayCommand.IsRunning));
        Assert.AreEqual(2, command.ContinuationCount);

        first.SetResult();
        second.SetResult();
    }

    [TestMethod]
    public async Task ExecutionTask_Completes_RaisesCompletionNotificationsAndClearsTask()
    {
        // Arrange
        using var command = new TestCommand();
        var tcs = new TaskCompletionSource();
        command.SetExecutionTask(tcs.Task);
        var propertyChanges = new List<string>();
        command.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        tcs.SetResult();
        await WaitUntilAsync(() => command.CurrentExecutionTask is null);

        // Assert
        Assert.IsNull(command.CurrentExecutionTask);
        Assert.IsFalse(command.IsRunning);
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.ExecutionTask));
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.IsRunning));
        Assert.AreEqual(0, command.ContinuationCount);
    }

    [TestMethod]
    public async Task ExecutionTask_CompletesWithNonConcurrentOption_RaisesCanExecuteChanged()
    {
        // Arrange
        using var command = new TestCommand(AsyncRelayCommandOptions.None);
        var tcs = new TaskCompletionSource();
        var canExecuteChangedCount = 0;
        command.CanExecuteChanged += (_, _) => canExecuteChangedCount++;
        command.SetExecutionTask(tcs.Task);

        // Act
        tcs.SetResult();
        await WaitUntilAsync(() => canExecuteChangedCount > 0);

        // Assert
        Assert.AreEqual(1, canExecuteChangedCount);
    }

    [TestMethod]
    public async Task ExecutionTask_CompletesWithConcurrentOption_DoesNotRaiseCanExecuteChanged()
    {
        // Arrange
        using var command = new TestCommand(AsyncRelayCommandOptions.AllowConcurrentExecutions);
        var tcs = new TaskCompletionSource();
        var canExecuteChangedCount = 0;
        command.CanExecuteChanged += (_, _) => canExecuteChangedCount++;
        command.SetExecutionTask(tcs.Task);

        // Act
        tcs.SetResult();
        await WaitUntilAsync(() => command.CurrentExecutionTask is null);

        // Assert
        Assert.AreEqual(0, canExecuteChangedCount);
    }

    [TestMethod]
    public async Task ExecutionTask_CompletionWithSynchronizationContext_PostsNotification()
    {
        // Arrange
        var previousContext = SynchronizationContext.Current;
        var context = new ManualSynchronizationContext();
        var command = new TestCommand();
        try
        {
            var tcs = new TaskCompletionSource();
            command.SetExecutionTask(tcs.Task);

            // Install the context before the monitored task completes so the continuation posts to it
            SynchronizationContext.SetSynchronizationContext(context);

            // Act
            tcs.SetResult();
            await WaitUntilAsync(() => context.PendingCount > 0);

            // Assert - completion is queued, not applied inline
            Assert.AreEqual(1, context.PendingCount);
            Assert.IsNotNull(command.CurrentExecutionTask);

            context.RunPending();
            await WaitUntilAsync(() => command.CurrentExecutionTask is null);
            Assert.IsNull(command.CurrentExecutionTask);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
            command.Dispose();
        }
    }

    #endregion

    #region MonitorTask

    [TestMethod]
    public void MonitorTask_AfterDispose_DoesNotTrackContinuation()
    {
        // Arrange
        var command = new TestCommand();
        var tcs = new TaskCompletionSource();
        command.Dispose();

        // Act
        command.Monitor(tcs.Task);

        // Assert
        Assert.AreEqual(0, command.ContinuationCount);

        tcs.SetResult();
    }

    [TestMethod]
    public void Dispose_WhileMonitoringRunningTask_ClearsContinuations()
    {
        // Arrange
        var command = new TestCommand();
        var tcs = new TaskCompletionSource();
        command.SetExecutionTask(tcs.Task);
        Assert.AreEqual(1, command.ContinuationCount);

        // Act - disposal cancels the continuation token, so the continuation runs (and returns its
        // pooled resources) immediately
        command.Dispose();

        // Assert
        Assert.AreEqual(0, command.ContinuationCount);

        // Completing afterwards must not resurrect any state or throw
        Exception? observed = null;
        try
        {
            tcs.SetResult();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        // A still-running task is intentionally left referenced; its continuation already returned
        // its pooled resources and must not touch the disposed command.
        Assert.AreEqual(0, command.ContinuationCount);
    }

    #endregion

    #region Cancellation

    [TestMethod]
    public void CanBeCanceled_TracksRunningAndCancellationState()
    {
        // Arrange
        using var command = new TestCommand();
        var tcs = new TaskCompletionSource();

        // Assert - not running yet
        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        // Act - running
        command.SetExecutionTask(tcs.Task);

        // Assert - running and cancellable
        Assert.IsTrue(command.CanBeCanceled);

        // Act - cancellation requested on the current token source
        command.CurrentTokenSource!.Cancel();

        // Assert - cancellation requested disables cancellation
        Assert.IsTrue(command.IsCancellationRequested);
        Assert.IsFalse(command.CanBeCanceled);

        tcs.SetResult();
    }

    [TestMethod]
    public void Cancel_WithStandaloneTokenSource_CancelsAndNotifies()
    {
        // Arrange
        using var command = new TestCommand();
        var cancellationTokenSource = new CancellationTokenSource();
        command.CurrentTokenSource = cancellationTokenSource;
        var propertyChanges = new List<string>();
        command.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        command.Cancel();

        // Assert
        Assert.IsTrue(cancellationTokenSource.IsCancellationRequested);
        Assert.IsTrue(command.IsCancellationRequested);
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.CanBeCanceled));
        CollectionAssert.Contains(propertyChanges, nameof(IAsyncRelayCommand.IsCancellationRequested));
    }

    [TestMethod]
    public void Cancel_WithDisposedTrackedTokenSource_DoesNotThrow()
    {
        // Arrange
        using var command = new TestCommand();
        var disposed = new CancellationTokenSource();
        disposed.Dispose();
        command.AddTokenSource(disposed);

        // Act & Assert
        Exception? observed = null;
        try
        {
            command.Cancel();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void Cancel_AfterDispose_DoesNotCancelAnything()
    {
        // Arrange
        var command = new TestCommand();
        command.Dispose();
        var cancellationTokenSource = new CancellationTokenSource();
        command.CurrentTokenSource = cancellationTokenSource;

        // Act & Assert
        Exception? observed = null;
        try
        {
            command.Cancel();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        Assert.IsFalse(cancellationTokenSource.IsCancellationRequested);
        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public void IsCancellationSupported_DependsOnExecuteDelegate()
    {
        // Arrange
        using var executeCommand = new TestCommand(execute: () => Task.CompletedTask);
        using var cancelableCommand = new TestCommand(cancelableExecute: _ => Task.CompletedTask);

        // Act & Assert
        Assert.IsFalse(((ICancellationAwareCommand)executeCommand).IsCancellationSupported);
        Assert.IsTrue(((ICancellationAwareCommand)cancelableCommand).IsCancellationSupported);
    }

    #endregion

    #region Timeout token sources

    [TestMethod]
    public async Task CreateTimeoutTokenSource_ShortTimeout_IsTrackedAndExpires()
    {
        // Arrange
        using var command = new TestCommand();

        // Act
        var cancellationTokenSource = command.CreateTimeout(TimeSpan.FromMilliseconds(50));

        // Assert - tracked
        Assert.AreEqual(1, command.TokenSourceCount);

        await WaitUntilAsync(() => cancellationTokenSource.IsCancellationRequested);

        // Assert - expired
        Assert.IsTrue(cancellationTokenSource.IsCancellationRequested);
    }

    [TestMethod]
    public void CreateTimeoutTokenSource_ShortTimeoutAfterDispose_ReturnsFreshTokenSource()
    {
        // Arrange
        var command = new TestCommand();
        command.Dispose();

        // Act
        var cancellationTokenSource = command.CreateTimeout(TimeSpan.FromMilliseconds(50));

        // Assert - a usable (untracked) token source is returned
        Assert.IsNotNull(cancellationTokenSource);
        Assert.IsFalse(cancellationTokenSource.IsCancellationRequested);
        Assert.AreEqual(0, command.TokenSourceCount);

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public void CreateTimeoutTokenSource_LongTimeout_IsLinkedToCommandTokenSource()
    {
        // Arrange
        using var command = new TestCommand();

        // Act
        var cancellationTokenSource = command.CreateTimeout(TimeSpan.FromMinutes(1));

        // Assert - tracked
        Assert.AreEqual(1, command.TokenSourceCount);

        // Act - cancel the command, which must propagate to the linked token
        command.Cancel();

        // Assert
        Assert.IsTrue(cancellationTokenSource.IsCancellationRequested);
    }

    [TestMethod]
    public void CreateTimeoutTokenSource_WithoutTimeout_UsesDefaultTimeout()
    {
        // Arrange
        using var command = new TestCommand();

        // Act
        var cancellationTokenSource = command.CreateTimeout();

        // Assert
        Assert.IsNotNull(cancellationTokenSource);
        Assert.AreEqual(1, command.TokenSourceCount);
        Assert.IsFalse(cancellationTokenSource.IsCancellationRequested);
    }

    #endregion

    #region Disposal

    [TestMethod]
    public void DisposeCancellationTokens_CancelsAndDisposesTrackedAndCurrentTokens()
    {
        // Arrange
        using var command = new TestCommand();
        var tracked = new CancellationTokenSource();
        var alreadyCanceled = new CancellationTokenSource();
        alreadyCanceled.Cancel();
        var current = new CancellationTokenSource();
        command.AddTokenSource(tracked);
        command.AddTokenSource(alreadyCanceled);
        command.CurrentTokenSource = current;

        // Act
        command.DisposeTokens();

        // Assert
        Assert.AreEqual(0, command.TokenSourceCount);
        Assert.IsNull(command.CurrentTokenSource);
        Assert.IsTrue(tracked.IsCancellationRequested);
        Assert.IsTrue(current.IsCancellationRequested);
    }

    [TestMethod]
    public void DisposeCancellationTokens_CalledTwice_HandlesNullCurrentToken()
    {
        // Arrange
        using var command = new TestCommand();
        command.DisposeTokens();

        // Act & Assert - second call has no current token source
        Exception? observed = null;
        try
        {
            command.DisposeTokens();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void DisposeCancellationTokens_WithDisposedCurrentToken_DoesNotThrow()
    {
        // Arrange
        using var command = new TestCommand();
        var disposed = new CancellationTokenSource();
        disposed.Dispose();
        command.CurrentTokenSource = disposed;

        // Act & Assert
        Exception? observed = null;
        try
        {
            command.DisposeTokens();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        Assert.IsNull(command.CurrentTokenSource);
    }

    [TestMethod]
    public void Dispose_WithCompletedExecutionTask_DisposesTaskAndIsIdempotent()
    {
        // Arrange
        var command = new TestCommand();
        command.SetExecutionTask(Task.CompletedTask);

        // Act & Assert - disposal of a completed task and a second disposal are both safe
        Exception? observed = null;
        try
        {
            command.Dispose();
            command.Dispose();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        Assert.IsNull(command.CurrentExecutionTask);
    }

    [TestMethod]
    public void CleanupCompletedTokenSources_KeepsOnlyLatestTokenSources()
    {
        // Arrange
        using var command = new TestCommand();
        var disposed = new CancellationTokenSource();
        disposed.Dispose();
        command.AddTokenSource(disposed);
        for (var i = 0; i < 6; i++)
            command.AddTokenSource(new CancellationTokenSource());
        Assert.AreEqual(7, command.TokenSourceCount);

        // Act
        command.CleanupTokens();

        // Assert
        Assert.AreEqual(5, command.TokenSourceCount);
    }

    [TestMethod]
    public void CleanupStaticResources_DoesNotThrow()
    {
        // Act & Assert
        Exception? observed = null;
        try
        {
            BaseAsyncRelayCommand.CleanupStaticResources();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    #endregion

    #region Property notification helpers

    [TestMethod]
    public void PropertyChanged_WithNoSubscribers_DoesNotThrow()
    {
        // Arrange
        using var command = new TestCommand();

        // Act & Assert - batched notification without handlers
        Exception? observed = null;
        try
        {
            command.SetExecutionTask(Task.CompletedTask);
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        // Already-completed tasks are not monitored, so the reference stays until replacement.
        Assert.IsNotNull(command.CurrentExecutionTask);
    }

    [TestMethod]
    public void ExecutionTask_AfterDispose_IsNotTracked()
    {
        // Arrange
        var command = new TestCommand();
        command.Dispose();

        // Act - the setter must ignore changes once disposed
        command.SetExecutionTask(Task.CompletedTask);

        // Assert
        Assert.IsNull(command.CurrentExecutionTask);
    }

    #endregion

    #region AwaitAndThrowIfFailed / LogIfFailed

    [TestMethod]
    public void AwaitAndThrowIfFailed_WithFaultedTask_DispatchesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Base command failure");
        var faultedTask = Task.FromException(expectedException);
        var sender = new object();
        Exception? capturedException = null;
        object? capturedSender = null;
        void Handler(object? s, CommandExceptionEventArgs e)
        {
            capturedSender = s;
            capturedException = e.Exception;
        }

        CommandExceptionDispatcher.UnhandledCommandException += Handler;
        try
        {
            // Act
            BaseAsyncRelayCommand.AwaitAndThrowIfFailed(faultedTask, sender);
        }
        finally
        {
            CommandExceptionDispatcher.UnhandledCommandException -= Handler;
        }

        // Assert
        Assert.AreSame(sender, capturedSender);
        Assert.AreSame(expectedException, capturedException);
    }

    [TestMethod]
    public void AwaitAndThrowIfFailed_WithSuccessfulTask_DoesNotDispatch()
    {
        // Arrange
        var dispatchedCount = 0;
        void Handler(object? s, CommandExceptionEventArgs e) => dispatchedCount++;

        CommandExceptionDispatcher.UnhandledCommandException += Handler;
        try
        {
            // Act
            BaseAsyncRelayCommand.AwaitAndThrowIfFailed(Task.CompletedTask, null);
            BaseAsyncRelayCommand.AwaitAndThrowIfFailed(Task.FromCanceled(new CancellationToken(true)), null);
        }
        finally
        {
            CommandExceptionDispatcher.UnhandledCommandException -= Handler;
        }

        // Assert
        Assert.AreEqual(0, dispatchedCount);
    }

    [TestMethod]
    public void LogIfFailed_WithFaultedTask_DoesNotThrow()
    {
        // Arrange
        var faultedTask = Task.FromException(new InvalidOperationException("Logged only"));

        // Act & Assert - the exception is observed by the fault-only continuation
        Exception? observed = null;
        try
        {
            BaseAsyncRelayCommand.LogIfFailed(faultedTask);
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void LogIfFailed_WithSuccessfulTask_DoesNotThrow()
    {
        // Act & Assert
        Exception? observed = null;
        try
        {
            BaseAsyncRelayCommand.LogIfFailed(Task.CompletedTask);
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    #endregion

    #region Additional defensive paths

    [TestMethod]
    public void HasActiveCancellationTokens_ReflectsTrackedTokenSources()
    {
        // Arrange
        using var command = new TestCommand();
        Assert.IsFalse(command.HasActiveCancellationTokens);

        // Act
        command.AddTokenSource(new CancellationTokenSource());

        // Assert
        Assert.IsTrue(command.HasActiveCancellationTokens);

        command.DisposeTokens();
        Assert.IsFalse(command.HasActiveCancellationTokens);
        Assert.AreEqual(0, command.ContinuationCount);
    }

    [TestMethod]
    public void CreateTimeoutTokenSource_WhenSharedTimeoutTokenWasDisposed_RecreatesTokenSource()
    {
        // Arrange
        using var command = new TestCommand();
        var timeout = TimeSpan.FromMinutes(3);

        // Dispose the cached shared token source out-of-band so the linked token creation fails once
        var shared = CommandExecutionResourcePool.GetOrCreateTimeoutTokenSource(timeout);
        shared.Dispose();

        // Act
        var cancellationTokenSource = command.CreateTimeout(timeout);

        // Assert - a fresh, usable and tracked token source is returned
        Assert.IsNotNull(cancellationTokenSource);
        Assert.IsFalse(cancellationTokenSource.IsCancellationRequested);
        Assert.AreEqual(1, command.TokenSourceCount);
    }

    [TestMethod]
    public void Cancel_WithDisposedCurrentTokenSource_DoesNotThrow()
    {
        // Arrange
        using var command = new TestCommand();
        var disposed = new CancellationTokenSource();
        disposed.Dispose();
        command.CurrentTokenSource = disposed;

        // Act & Assert
        Exception? observed = null;
        try
        {
            command.Cancel();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public void Dispose_WithNonDisposingFlag_DoesNotChangeState()
    {
        // Arrange
        using var command = new TestCommand();
        var tcs = new TaskCompletionSource();
        command.SetExecutionTask(tcs.Task);

        // Act
        command.DisposeNonDisposing();

        // Assert - nothing was released
        Assert.AreSame(tcs.Task, command.CurrentExecutionTask);
        Assert.AreEqual(1, command.ContinuationCount);

        tcs.SetResult();
    }

    [TestMethod]
    public void DisposeCancellationTokens_WithDisposedContinuationToken_DoesNotThrow()
    {
        // Arrange
        using var command = new TestCommand();
        var disposed = new CancellationTokenSource();
        disposed.Dispose();
        command.AddContinuationTokenSource(disposed);

        // Act & Assert
        Exception? observed = null;
        try
        {
            command.DisposeTokens();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
        Assert.AreEqual(0, command.ContinuationCount);
    }

    [TestMethod]
    public void Dispose_WhenExecutionTaskDisposeThrows_DoesNotThrow()
    {
        // Arrange
        var command = new TestCommand();
        command.SetExecutionTask(new ThrowingCompletedTask());

        // Act & Assert
        Exception? observed = null;
        try
        {
            command.Dispose();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    #endregion

}

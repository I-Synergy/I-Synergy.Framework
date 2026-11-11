using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.BackgroundServices;
using ISynergy.Framework.Automations.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ISynergy.Framework.Automations.Tests;

/// <summary>
/// Tests for the lifecycle, timer wiring and background error handling of
/// <see cref="ActionQueuingBackgroundService"/>.
/// </summary>
[TestClass]
public class ActionQueuingBackgroundServiceTests
{
    public TestContext TestContext { get; set; } = null!;

    private Mock<IActionService> _actionService = null!;
    private Mock<ILogger<ActionQueuingBackgroundService>> _logger = null!;
    private AutomationOptions _options = null!;

    [TestInitialize]
    public void Initialize()
    {
        _actionService = new Mock<IActionService>();
        _logger = new Mock<ILogger<ActionQueuingBackgroundService>>();
        _options = new AutomationOptions
        {
            // Keep the background timer slow so it does not interfere with lifecycle assertions.
            DefaultQueueRefreshRate = TimeSpan.FromHours(1)
        };

        _actionService
            .Setup(s => s.RefreshTasksAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _actionService
            .Setup(s => s.CalculateTimespanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((TimeSpan Expiration, IAction? UpcomingTask)?)null);
    }

    private ActionQueuingBackgroundService CreateService() =>
        new(_actionService.Object, Microsoft.Extensions.Options.Options.Create(_options), _logger.Object);

    [TestMethod]
    public async Task StartAsync_RefreshesTasks()
    {
        using var service = CreateService();

        await service.StartAsync(CancellationToken.None);

        _actionService.Verify(s => s.RefreshTasksAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);

        await service.StopAsync(CancellationToken.None);
    }

    [TestMethod]
    public async Task StartAsync_WhenNoUpcomingTask_DoesNotExecuteAnyAction()
    {
        using var service = CreateService();

        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        _actionService.Verify(
            s => s.ExcecuteActionAsync(It.IsAny<IAction>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task StopAsync_CompletesSuccessfully_AfterStart()
    {
        using var service = CreateService();

        await service.StartAsync(CancellationToken.None);
        var stopTask = service.StopAsync(CancellationToken.None);

        await stopTask;
        Assert.IsTrue(stopTask.IsCompletedSuccessfully);
    }

    [TestMethod]
    public async Task StopAsync_WithoutStart_CompletesSuccessfully()
    {
        using var service = CreateService();

        var stopTask = service.StopAsync(CancellationToken.None);

        await stopTask;
        Assert.IsTrue(stopTask.IsCompletedSuccessfully);
    }

    [TestMethod]
    public async Task Dispose_AfterStart_DoesNotThrow()
    {
        var service = CreateService();

        await service.StartAsync(CancellationToken.None);

        service.Dispose();

        _actionService.Verify(
            s => s.RefreshTasksAsync(It.IsAny<CancellationToken>()),
            Times.AtLeastOnce,
            "Starting the service should have refreshed the task queue before disposal.");
    }

    [TestMethod]
    public async Task RefreshQueue_WhenRefreshTaskThrows_IsLoggedAndDoesNotCrashTimerCallback()
    {
        // The first RefreshTasksAsync (awaited by StartAsync) succeeds, the timer callback then
        // throws; the service must swallow and log that error rather than crash the process.
        _options.DefaultQueueRefreshRate = TimeSpan.FromMilliseconds(20);

        _actionService
            .SetupSequence(s => s.RefreshTasksAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)                         // StartAsync
            .ThrowsAsync(new InvalidOperationException("boom")); // timer callback (and repeats)

        var logged = new TaskCompletionSource();
        var logger = new CapturingLogger(logged);

        using var service = new ActionQueuingBackgroundService(
            _actionService.Object,
            Microsoft.Extensions.Options.Options.Create(_options),
            logger);

        await service.StartAsync(CancellationToken.None);

        using var delayCancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        var completed = await Task.WhenAny(logged.Task, Task.Delay(TimeSpan.FromSeconds(5), delayCancellation.Token));
        await delayCancellation.CancelAsync();
        await service.StopAsync(CancellationToken.None);

        Assert.AreSame(logged.Task, completed, "The refresh error was not logged.");
    }

    [TestMethod]
    public async Task RefreshQueue_WhenUpcomingTaskExists_ExecutesActionFromExecutionTimer()
    {
        // A timespan with a non-zero expiration and an upcoming task makes RefreshQueueAsync arm
        // the execution timer; its callback runs the action in the background (no deadlock on the
        // refresh timer thread and no unobserved exception).
        var action = new Mock<IAction>().Object;

        _actionService
            .Setup(s => s.CalculateTimespanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((TimeSpan Expiration, IAction? UpcomingTask)?)
                (TimeSpan.FromMilliseconds(10), action));

        var executed = new TaskCompletionSource();
        _actionService
            .Setup(s => s.ExcecuteActionAsync(It.IsAny<IAction>(), It.IsAny<CancellationToken>()))
            .Callback(() => executed.TrySetResult())
            .Returns(Task.CompletedTask);

        var errors = new TaskCompletionSource();
        var logger = new CapturingLogger(errors);

        using var service = new ActionQueuingBackgroundService(
            _actionService.Object,
            Microsoft.Extensions.Options.Options.Create(_options),
            logger);

        await service.StartAsync(CancellationToken.None);

        using var delayCancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        var completed = await Task.WhenAny(executed.Task, Task.Delay(TimeSpan.FromSeconds(5), delayCancellation.Token));
        await delayCancellation.CancelAsync();
        await service.StopAsync(CancellationToken.None);

        Assert.AreSame(executed.Task, completed, "The execution timer callback did not run the action.");
        _actionService.Verify(
            s => s.ExcecuteActionAsync(action, It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task RunInBackground_WhenBackgroundOperationFaults_LogsUnhandledError()
    {
        // RefreshQueueAsync swallows service errors, so the only way the fire-and-forget task can
        // fault is a failure inside its own catch handler (here: a throwing logger). The fault
        // continuation must still log instead of leaving the exception unobserved.
        _options.DefaultQueueRefreshRate = TimeSpan.FromHours(1);

        _actionService
            .SetupSequence(s => s.RefreshTasksAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)                          // StartAsync
            .ThrowsAsync(new InvalidOperationException("boom")); // timer callback (and repeats)

        var faultLogged = new TaskCompletionSource();
        var logger = new ThrowingOnFirstErrorLogger(faultLogged);

        using var service = new ActionQueuingBackgroundService(
            _actionService.Object,
            Microsoft.Extensions.Options.Options.Create(_options),
            logger);

        await service.StartAsync(CancellationToken.None);

        using var delayCancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        var completed = await Task.WhenAny(faultLogged.Task, Task.Delay(TimeSpan.FromSeconds(5), delayCancellation.Token));
        await delayCancellation.CancelAsync();
        await service.StopAsync(CancellationToken.None);

        Assert.AreSame(faultLogged.Task, completed,
            "The faulted background task was not reported through the continuation logger.");
        Assert.AreEqual(1, logger.UnhandledErrorCount, "Expected exactly one 'unhandled error' entry.");
    }
}

/// <summary>
/// Minimal <see cref="ILogger{T}"/> that signals the first time an error entry is written.
/// </summary>
internal sealed class CapturingLogger : ILogger<ActionQueuingBackgroundService>
{
    private readonly TaskCompletionSource _signal;

    public CapturingLogger(TaskCompletionSource signal) => _signal = signal;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (logLevel == LogLevel.Error)
            _signal.TrySetResult();
    }
}

/// <summary>
/// <see cref="ILogger{T}"/> that throws on the first error write (simulating a failing logger) and
/// signals (then counts) every subsequent error write.
/// </summary>
internal sealed class ThrowingOnFirstErrorLogger : ILogger<ActionQueuingBackgroundService>
{
    private readonly TaskCompletionSource _signal;
    private int _errorCount;
    private bool _failed;

    public ThrowingOnFirstErrorLogger(TaskCompletionSource signal) => _signal = signal;

    public int UnhandledErrorCount => Volatile.Read(ref _errorCount);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (logLevel != LogLevel.Error)
            return;

        if (!_failed)
        {
            _failed = true;
            throw new InvalidOperationException("logger failure");
        }

        // Only the fault continuation writes this entry; the refresh catch handler does not.
        if (formatter(state, exception).Contains("Unhandled error", StringComparison.Ordinal))
        {
            Interlocked.Increment(ref _errorCount);
            _signal.TrySetResult();
        }
    }
}

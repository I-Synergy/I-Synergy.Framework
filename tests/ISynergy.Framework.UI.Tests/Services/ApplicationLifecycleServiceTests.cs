using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.UI.Tests.Services;

[TestClass]
public class ApplicationLifecycleServiceTests
{
    private Mock<ILogger<ApplicationLifecycleService>> _mockLogger = null!;
    private IApplicationLifecycleService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _mockLogger = new Mock<ILogger<ApplicationLifecycleService>>();
        _service = new ApplicationLifecycleService(_mockLogger.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _service?.Dispose();
    }

    #region Initial State Tests

    [TestMethod]
    public void Constructor_InitializesWithCorrectLoggerDependency()
    {
        // Act & Assert
        Assert.IsNotNull(_service);
        _mockLogger.Verify(
            x => x.Log(
           It.IsAny<LogLevel>(),
       It.IsAny<EventId>(),
         It.IsAny<It.IsAnyType>(),
    It.IsAny<Exception>(),
    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [TestMethod]
    public void InitialState_AllFlagsAreFalse()
    {
        // Act & Assert
        Assert.IsFalse(_service.IsApplicationUIReady);
        Assert.IsFalse(_service.IsApplicationInitialized);
        Assert.IsFalse(_service.IsApplicationLoaded);
    }

    #endregion

    #region SignalApplicationUIReady Tests

    [TestMethod]
    public void SignalApplicationUIReady_SetsUIReadyFlagToTrue()
    {
        // Act
        _service.SignalApplicationUIReady();

        // Assert
        Assert.IsTrue(_service.IsApplicationUIReady);
    }

    [TestMethod]
    public void SignalApplicationUIReady_RaisesUIReadyEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.ApplicationUIReady += (s, e) => eventRaised = true;

        // Act
        _service.SignalApplicationUIReady();

        // Assert
        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void SignalApplicationUIReady_CalledMultipleTimes_EventRaisesOnlyOnce()
    {
        // Arrange
        var eventRaiseCount = 0;
        _service.ApplicationUIReady += (s, e) => eventRaiseCount++;

        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationUIReady();
        _service.SignalApplicationUIReady();

        // Assert
        Assert.AreEqual(1, eventRaiseCount, "Event should raise exactly once");
    }

    [TestMethod]
    public void SignalApplicationUIReady_WithoutInitialized_DoesNotRaiseApplicationLoaded()
    {
        // Arrange
        var applicationLoadedRaised = false;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaised = true;

        // Act
        _service.SignalApplicationUIReady();

        // Assert
        Assert.IsFalse(applicationLoadedRaised);
        Assert.IsFalse(_service.IsApplicationLoaded);
    }

    #endregion

    #region SignalApplicationInitialized Tests

    [TestMethod]
    public void SignalApplicationInitialized_SetsInitializedFlagToTrue()
    {
        // Act
        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsTrue(_service.IsApplicationInitialized);
    }

    [TestMethod]
    public void SignalApplicationInitialized_RaisesApplicationInitializedEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.ApplicationInitialized += (s, e) => eventRaised = true;

        // Act
        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void SignalApplicationInitialized_CalledMultipleTimes_EventRaisesOnlyOnce()
    {
        // Arrange
        var eventRaiseCount = 0;
        _service.ApplicationInitialized += (s, e) => eventRaiseCount++;

        // Act
        _service.SignalApplicationInitialized();
        _service.SignalApplicationInitialized();
        _service.SignalApplicationInitialized();

        // Assert
        Assert.AreEqual(1, eventRaiseCount, "Event should raise exactly once");
    }

    [TestMethod]
    public void SignalApplicationInitialized_WithoutUIReady_DoesNotRaiseApplicationLoaded()
    {
        // Arrange
        var applicationLoadedRaised = false;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaised = true;

        // Act
        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsFalse(applicationLoadedRaised);
        Assert.IsFalse(_service.IsApplicationLoaded);
    }

    #endregion

    #region ApplicationLoaded Coordination Tests

    [TestMethod]
    public void ApplicationLoaded_RaisesWhenBothSignalsReceived_UIReadyFirst()
    {
        // Arrange
        var applicationLoadedRaised = false;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaised = true;

        // Act
        _service.SignalApplicationUIReady();
        Assert.IsFalse(applicationLoadedRaised, "Should not raise after first signal");

        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsTrue(applicationLoadedRaised, "Should raise after both signals");
        Assert.IsTrue(_service.IsApplicationLoaded);
    }

    [TestMethod]
    public void ApplicationLoaded_RaisesWhenBothSignalsReceived_InitializedFirst()
    {
        // Arrange
        var applicationLoadedRaised = false;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaised = true;

        // Act
        _service.SignalApplicationInitialized();
        Assert.IsFalse(applicationLoadedRaised, "Should not raise after first signal");

        _service.SignalApplicationUIReady();

        // Assert
        Assert.IsTrue(applicationLoadedRaised, "Should raise after both signals");
        Assert.IsTrue(_service.IsApplicationLoaded);
    }

    [TestMethod]
    public void ApplicationLoaded_RaisesExactlyOnce_WhenBothSignalsReceived()
    {
        // Arrange
        var applicationLoadedRaiseCount = 0;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaiseCount++;

        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();
        _service.SignalApplicationUIReady();  // Try again
        _service.SignalApplicationInitialized();  // Try again

        // Assert
        Assert.AreEqual(1, applicationLoadedRaiseCount, "Event should raise exactly once");
    }

    [TestMethod]
    public void ApplicationLoaded_IsAtomicAndThreadSafe()
    {
        // Arrange
        var applicationLoadedRaiseCount = 0;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaiseCount++;

        // Act - Simulate concurrent signals
        var tasks = new Task[]
      {
        Task.Run(() => _service.SignalApplicationUIReady()),
            Task.Run(() => _service.SignalApplicationInitialized()),
        Task.Run(() => _service.SignalApplicationUIReady()),
            Task.Run(() => _service.SignalApplicationInitialized()),
        };

        Task.WaitAll(tasks);

        // Assert
        Assert.AreEqual(1, applicationLoadedRaiseCount, "Event should raise exactly once despite concurrent calls");
    }

    [TestMethod]
    public void AllStateFlags_AreCorrectAfterAllSignals()
    {
        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsTrue(_service.IsApplicationUIReady);
        Assert.IsTrue(_service.IsApplicationInitialized);
        Assert.IsTrue(_service.IsApplicationLoaded);
    }

    #endregion

    #region Event Handler Tests

    [TestMethod]
    public void EventHandlers_ReceiveCorrectSender()
    {
        // Arrange
        object? uiReadySender = null;
        object? initializedSender = null;
        object? loadedSender = null;

        _service.ApplicationUIReady += (s, e) => uiReadySender = s;
        _service.ApplicationInitialized += (s, e) => initializedSender = s;
        _service.ApplicationLoaded += (s, e) => loadedSender = s;

        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert
        Assert.AreSame(_service, uiReadySender);
        Assert.AreSame(_service, initializedSender);
        Assert.AreSame(_service, loadedSender);
    }

    [TestMethod]
    public void EventHandlers_ReceiveEmptyEventArgs()
    {
        // Arrange
        EventArgs? uiReadyArgs = null;
        EventArgs? initializedArgs = null;
        EventArgs? loadedArgs = null;

        _service.ApplicationUIReady += (s, e) => uiReadyArgs = e;
        _service.ApplicationInitialized += (s, e) => initializedArgs = e;
        _service.ApplicationLoaded += (s, e) => loadedArgs = e;

        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert
        Assert.AreEqual(EventArgs.Empty, uiReadyArgs);
        Assert.AreEqual(EventArgs.Empty, initializedArgs);
        Assert.AreEqual(EventArgs.Empty, loadedArgs);
    }

    [TestMethod]
    public void MultipleEventSubscribers_AllReceiveNotification()
    {
        // Arrange
        var subscriber1Raised = false;
        var subscriber2Raised = false;
        var subscriber3Raised = false;

        _service.ApplicationLoaded += (s, e) => subscriber1Raised = true;
        _service.ApplicationLoaded += (s, e) => subscriber2Raised = true;
        _service.ApplicationLoaded += (s, e) => subscriber3Raised = true;

        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsTrue(subscriber1Raised);
        Assert.IsTrue(subscriber2Raised);
        Assert.IsTrue(subscriber3Raised);
    }

    [TestMethod]
    public void ExceptionInEventHandler_IsCaughtAndLogged()
    {
        // Arrange
        var subscriber1Raised = false;
        var subscriber2Raised = false;

        // First handler throws an exception
        _service.ApplicationLoaded += (s, e) => throw new InvalidOperationException("Test exception");
        // These handlers are added AFTER the throwing handler
        // They will NOT be called because when the first handler throws,
        // the delegate chain stops (standard .NET behavior)
        _service.ApplicationLoaded += (s, e) => subscriber1Raised = true;
        _service.ApplicationLoaded += (s, e) => subscriber2Raised = true;

        // Act - Service catches the exception and logs it
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert - Service catches exception so no crash,
        // but handlers after the throwing one don't execute
        Assert.IsFalse(subscriber1Raised, "Handlers after throwing handler don't execute (standard .NET delegate chain behavior)");
        Assert.IsFalse(subscriber2Raised, "Handlers after throwing handler don't execute (standard .NET delegate chain behavior)");

        // Verify exception was logged
        _mockLogger.Verify(
   x => x.Log(
 LogLevel.Error,
     It.IsAny<EventId>(),
    It.IsAny<It.IsAnyType>(),
     It.IsAny<Exception>(),
       It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
      Times.AtLeastOnce,
            "Service should log the exception from the event handler");
    }

    #endregion

    #region Disposal Tests

    [TestMethod]
    public void Dispose_UnsubscribesAllEventHandlers()
    {
        // Arrange
        var uiReadyRaised = false;
        var initializedRaised = false;
        var loadedRaised = false;

        _service.ApplicationUIReady += (s, e) => uiReadyRaised = true;
        _service.ApplicationInitialized += (s, e) => initializedRaised = true;
        _service.ApplicationLoaded += (s, e) => loadedRaised = true;

        // Act
        _service.Dispose();

        // Try to signal (should not raise events)
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert - Events should not have been raised
        Assert.IsFalse(uiReadyRaised);
        Assert.IsFalse(initializedRaised);
        Assert.IsFalse(loadedRaised);
    }

    [TestMethod]
    public void Dispose_LogsDisposal()
    {
        // Act
        _service.Dispose();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
             LogLevel.Trace,
              It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("disposing")),
           It.IsAny<Exception>(),
       It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                 Times.Once);
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Act & Assert - Should not throw
        _service.Dispose();
        _service.Dispose();
        _service.Dispose();
    }

    #endregion

    #region Logging Tests

    [TestMethod]
    public void SignalApplicationUIReady_LogsAtTraceLevel()
    {
        // Act
        _service.SignalApplicationUIReady();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Trace,
    It.IsAny<EventId>(),
       It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("UI framework is now ready")),
         It.IsAny<Exception>(),
    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public void SignalApplicationInitialized_LogsAtTraceLevel()
    {
        // Act
        _service.SignalApplicationInitialized();

        // Assert
        _mockLogger.Verify(
  x => x.Log(
     LogLevel.Trace,
            It.IsAny<EventId>(),
   It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Application initialization is complete")),
     It.IsAny<Exception>(),
      It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
       Times.Once);
    }

    [TestMethod]
    public void ApplicationLoaded_LogsAtTraceLevel()
    {
        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert
        _mockLogger.Verify(
         x => x.Log(
     LogLevel.Trace,
     It.IsAny<EventId>(),
      It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Both UI and initialization complete")),
     It.IsAny<Exception>(),
 It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once);
    }

    [TestMethod]
    public void DuplicateSignal_LogsWarning()
    {
        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationUIReady();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
 LogLevel.Warning,
  It.IsAny<EventId>(),
      It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("called multiple times")),
  It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once);
    }

    #endregion

    #region Edge Cases Tests

    [TestMethod]
    public void RapidSignaling_BothSignalsInQuickSuccession_ApplicationLoadedStillRaisesOnce()
    {
        // Arrange
        var applicationLoadedRaiseCount = 0;
        _service.ApplicationLoaded += (s, e) => applicationLoadedRaiseCount++;

        // Act
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();

        // Assert
        Assert.AreEqual(1, applicationLoadedRaiseCount);
    }

    [TestMethod]
    public void UnsubscribeDuringSignaling_DoesNotCauseIssues()
    {
        // Arrange
        EventHandler<EventArgs>? handler = null;
        handler = (s, e) => _service.ApplicationLoaded -= handler;

        _service.ApplicationLoaded += handler;
        _service.ApplicationLoaded += (s, e) => { }; // Another subscriber

        // Act & Assert - Should not throw
        _service.SignalApplicationUIReady();
        _service.SignalApplicationInitialized();
    }

    [TestMethod]
    public void StateQueries_ReflectCurrentState_AfterPartialSignaling()
    {
        // Act
        _service.SignalApplicationUIReady();

        // Assert
        Assert.IsTrue(_service.IsApplicationUIReady);
        Assert.IsFalse(_service.IsApplicationInitialized);
        Assert.IsFalse(_service.IsApplicationLoaded);

        // Act
        _service.SignalApplicationInitialized();

        // Assert
        Assert.IsTrue(_service.IsApplicationUIReady);
        Assert.IsTrue(_service.IsApplicationInitialized);
        Assert.IsTrue(_service.IsApplicationLoaded);
    }

    #endregion

    #region Stress Tests

    [TestMethod]
    [Timeout(5000)]
    public void ConcurrentSignaling_FromMultipleThreads_RemainsConsistent()
    {
        // Arrange
        var applicationLoadedRaiseCount = 0;
        _service.ApplicationLoaded += (s, e) => Interlocked.Increment(ref applicationLoadedRaiseCount);

        // Act - Multiple threads trying to signal simultaneously
        var threads = Enumerable.Range(0, 10)
          .Select(i => new Thread(() =>
    {
        if (i % 2 == 0)
            _service.SignalApplicationUIReady();
        else
            _service.SignalApplicationInitialized();
    }))
   .ToList();

        foreach (var thread in threads)
            thread.Start();

        foreach (var thread in threads)
            thread.Join();

        // Assert
        Assert.AreEqual(1, applicationLoadedRaiseCount, "Event should raise exactly once despite concurrent calls");
        Assert.IsTrue(_service.IsApplicationUIReady);
        Assert.IsTrue(_service.IsApplicationInitialized);
        Assert.IsTrue(_service.IsApplicationLoaded);
    }

    [TestMethod]
    [Timeout(5000)]
    public void ConcurrentEventSubscription_DoesNotCauseRaceCondition()
    {
        // Arrange
        var eventRaiseCount = 0;
        var subscriberCount = 100;

        // Act - Signal before subscribing (to test initial state)
        _service.SignalApplicationUIReady();

        // Multiple threads subscribing concurrently
        var tasks = Enumerable.Range(0, subscriberCount)
     .Select(_ => Task.Run(() =>
             {
                 _service.ApplicationLoaded += (s, e) =>
          Interlocked.Increment(ref eventRaiseCount);
             }))
               .ToList();

        Task.WaitAll(tasks.ToArray());

        // Now signal the second condition
        _service.SignalApplicationInitialized();

        // Assert
        Assert.AreEqual(subscriberCount, eventRaiseCount,
         "All concurrent subscribers should receive the event exactly once");
    }

    #endregion
}

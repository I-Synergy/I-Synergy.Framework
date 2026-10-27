using Microsoft.Extensions.DependencyInjection;
using Moq;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

/// <summary>
/// Comprehensive tests for exception handling in commands.
/// </summary>
[TestClass]
public class ExceptionHandlingTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    public ExceptionHandlingTests()
    {
        // Setup mocks
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceScope = new Mock<IServiceScope>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

        // Setup service scope factory
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_mockServiceScopeFactory.Object);

        // Setup service scope
        _mockServiceScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(_mockServiceScope.Object);

        _mockServiceScope
            .Setup(x => x.ServiceProvider)
            .Returns(_mockServiceProvider.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    /// <summary>
    /// Runs <paramref name="action"/> while capturing the exception (if any) raised via
    /// <see cref="CommandExceptionDispatcher.UnhandledCommandException"/>.
    /// </summary>
    private static Exception? CaptureDispatchedException(Action action)
    {
        Exception? captured = null;
        void Handler(object? sender, CommandExceptionEventArgs e) => captured = e.Exception;

        CommandExceptionDispatcher.UnhandledCommandException += Handler;
        try
        {
            action();
        }
        finally
        {
            CommandExceptionDispatcher.UnhandledCommandException -= Handler;
        }

        return captured;
    }

    #region RelayCommand Exception Handling Tests

    [TestMethod]
    public void RelayCommand_Execute_WithException()
    {
        // Arrange - a synchronous throw must never escape Execute(): on some platforms
        // (e.g. Mac Catalyst) an unhandled exception on the calling thread aborts the process
        // unconditionally. It is routed through CommandExceptionDispatcher instead.
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand(() => throw expectedException);

        // Act
        var dispatchedException = CaptureDispatchedException(() => command.Execute(null));

        // Assert
        Assert.AreEqual(expectedException, dispatchedException);
    }

    [TestMethod]
    public void RelayCommand_Execute_WithException_WhenHandlerNotAvailable_DoesNotThrow()
    {
        // Arrange - even with no CommandExceptionDispatcher subscribers, Execute() must not
        // rethrow synchronously; the exception is simply not observed by anyone.
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand(() => throw expectedException);

        // Act & Assert - should not throw
        command.Execute(null);
    }

    #endregion

    #region RelayCommand<T> Exception Handling Tests

    [TestMethod]
    public void RelayCommandGeneric_Execute_WithException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand<string>(_ => throw expectedException);

        // Act
        var dispatchedException = CaptureDispatchedException(() => command.Execute("test"));

        // Assert
        Assert.AreEqual(expectedException, dispatchedException);
    }

    #endregion

    #region AsyncRelayCommand Exception Handling Tests

    [TestMethod]
    public async Task AsyncRelayCommand_ExecuteAsync_WithSynchronousException_CreatesFaultedTask()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand(() => throw expectedException);

        // Act
        var task = command.ExecuteAsync(null);
        //await Assert.ThrowsAsync<InvalidOperationException>(() => task);

        // Assert - exception handler should be called
        await Task.Delay(100); // Give time for async handling
        Assert.IsTrue(task.IsFaulted);
        Assert.AreEqual(expectedException, task.Exception?.InnerException);
    }

    [TestMethod]
    public async Task AsyncRelayCommand_ExecuteAsync_WithAsynchronousException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            throw expectedException;
        });

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync(null));
    }

    [TestMethod]
    public void AsyncRelayCommand_Execute_WithSynchronousException_DoesNotThrowAndDispatchesException()
    {
        // Default FlowExceptionsToTaskScheduler: the execute delegate throws before its first
        // "await", so the task is already faulted by the time Execute() checks it. Rethrowing
        // synchronously here (the old behavior) would become a genuine unhandled exception on
        // the calling/UI thread and, on some platforms (e.g. Mac Catalyst), crash the process.
        // It must instead be routed through CommandExceptionDispatcher, same as RelayCommand.
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand(() => throw expectedException);

        var dispatchedException = CaptureDispatchedException(() => command.Execute(null));

        Assert.AreEqual(expectedException, dispatchedException);
    }

    [TestMethod]
    public void AsyncRelayCommand_Execute_WithSynchronousException_AndNoneOption_DoesNotThrow()
    {
        // None -> exception logged but not propagated
        var command = new AsyncRelayCommand(() => throw new InvalidOperationException("Test exception"), AsyncRelayCommandOptions.None);

        // Should not throw - exception is logged via Debug.WriteLine
        command.Execute(null);
    }

    #endregion

    #region AsyncRelayCommand<T> Exception Handling Tests

    [TestMethod]
    public async Task AsyncRelayCommandGeneric_ExecuteAsync_WithSynchronousException_CreatesFaultedTask()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand<string>(_ => throw expectedException);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync("test"));
    }

    #endregion
}





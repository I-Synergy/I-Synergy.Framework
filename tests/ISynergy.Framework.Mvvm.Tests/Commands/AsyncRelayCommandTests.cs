using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class AsyncRelayCommandTests
{
    private Mock<IExceptionHandlerService> _mockExceptionHandler;
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    [TestInitialize]
    public void Setup()
    {
        // Setup mocks
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
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

        // Setup exception handler service
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        // Initialize ServiceLocator with mock service provider
        ServiceLocator.SetLocatorProvider(_mockServiceProvider.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Reset ServiceLocator state after each test
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public async Task ExecuteAsync_SimpleCommand_ExecutesSuccessfully()
    {
        // Arrange
        bool wasExecuted = false;
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            wasExecuted = true;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.IsTrue(wasExecuted);
    }

    [TestMethod]
    public void CanExecute_WithPredicate_ReturnsExpectedResult()
    {
        // Arrange
        bool canExecuteValue = false;
        var command = new AsyncRelayCommand(
            () => Task.CompletedTask,
            () => canExecuteValue
        );

        // Act & Assert
        Assert.IsFalse(command.CanExecute(null));

        canExecuteValue = true;
        Assert.IsTrue(command.CanExecute(null));
    }

    [TestMethod]
    public async Task ExecutionTask_ReflectsCurrentExecution()
    {
        // Arrange
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand(() => tcs.Task);

        // Act
        var executionTask = command.ExecuteAsync(null);

        // Assert initial state
        Assert.IsNotNull(command.ExecutionTask, "ExecutionTask should be set after execution starts");
        Assert.IsFalse(executionTask.IsCompleted, "Execution should not be completed initially");

        // Complete the task
        tcs.SetResult(true);
        await executionTask;

        // Assert final state
        Assert.IsTrue(executionTask.IsCompleted, "Execution should be completed after task completes");
    }

    [TestMethod]
    public async Task Cancel_CancelableCommand_CancelsExecution()
    {
        // Arrange
        bool wasCancelled = false;
        var command = new AsyncRelayCommand(async (CancellationToken token) =>
        {
            try
            {
                await Task.Delay(1000, token);
            }
            catch (OperationCanceledException)
            {
                wasCancelled = true;
                throw;
            }
        });

        // Act
        var executionTask = command.ExecuteAsync(null);
        command.Cancel();

        try
        {
            await executionTask;
        }
        catch (OperationCanceledException)
        {
            // Expected exception
        }

        // Assert
        Assert.IsTrue(wasCancelled);
        Assert.IsTrue(command.IsCancellationRequested);

        // Verify exception was handled
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(It.IsAny<OperationCanceledException>()),
            Times.Once);
    }

    [TestMethod]
    public async Task ConcurrentExecutions_WithOption_AllowsMultipleExecutions()
    {
        // Arrange
        int executionCount = 0;
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand(
            async () =>
            {
                executionCount++;
                await tcs.Task;
            },
            options: AsyncRelayCommandOptions.AllowConcurrentExecutions
        );

        // Act
        var task1 = command.ExecuteAsync(null);
        var task2 = command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(2, executionCount);

        tcs.SetResult(true);
        await Task.WhenAll(task1, task2);
    }

    [TestMethod]
    public async Task Execute_WithException_HandlesExceptionCorrectly()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            throw expectedException;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(expectedException),
            Times.Once);
    }

    [TestMethod]
    public async Task Execute_WithNestedException_HandlesInnerExceptionCorrectly()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception");
        var outerException = new Exception("Outer exception", innerException);
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            throw outerException;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(innerException),
            Times.Once);
    }

    [TestMethod]
    public void Dispose_MultipleTimes_HandlesCorrectly()
    {
        // Arrange
        var command = new AsyncRelayCommand(() => Task.CompletedTask);

        // Act & Assert - should not throw
        command.Dispose();
        command.Dispose(); // Second dispose should be safe
    }

    [TestMethod]
    public async Task Execute_AfterDispose_HandlesGracefully()
    {
        // Arrange
        var command = new AsyncRelayCommand(() => Task.CompletedTask);
        command.Dispose();

        // Act & Assert - should not throw
        await command.ExecuteAsync(null);
    }
}
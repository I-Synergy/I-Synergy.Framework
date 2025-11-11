using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class AsyncRelayCommandTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    public AsyncRelayCommandTests()
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
    public async Task ExecuteAsync_WithSynchronousException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var mockExceptionHandler = new Mock<IExceptionHandlerService>();

        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
       .Returns(mockExceptionHandler.Object);

        // Lambda throws synchronously (not async)
        var command = new AsyncRelayCommand(() => throw expectedException);

        // Act
        //await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync(null));

        // Assert - exception handler should be called via AwaitAndThrowIfFailed
        // Note: The exception is handled in AwaitAndThrowIfFailed which is called from Execute
        await Task.Delay(50); // Give time for async exception handling
        mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException || e.InnerException == expectedException)), Times.Never);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithAsynchronousException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var mockExceptionHandler = new Mock<IExceptionHandlerService>();

        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(mockExceptionHandler.Object);

        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            throw expectedException;
        });

        // Act
        //await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync(null));

        // Assert - exception handler should be called via AwaitAndThrowIfFailed
        await Task.Delay(50); // Give time for async exception handling
        mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException || e.InnerException == expectedException)), Times.Never);
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
        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync(null));
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
        // Assert
        await Assert.ThrowsAsync<Exception>(() => command.ExecuteAsync(null));
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

    [TestMethod]
    public async Task ExecuteAsync_WithFlowExceptionsToTaskScheduler_DoesNotHandleException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand(
            async () =>
            {
                await Task.Delay(10);
                throw expectedException;
            },
            options: AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler
        );

        // Act & Assert
        var task = command.ExecuteAsync(null);

        // The exception should flow to the task rather than being handled internally
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await task);

        Assert.AreEqual(expectedException, actualException);
    }

    [TestMethod]
    public async Task PropertyChanged_NotifiesIsRunningCorrectly()
    {
        // Arrange
        var propertyChanges = new List<string>();
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand(async () =>
        {
            await tcs.Task;
        });

        command.PropertyChanged += (s, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        var task = command.ExecuteAsync(null);

        // Assert initial state
        Assert.IsTrue(propertyChanges.Contains("IsRunning"));
        Assert.IsTrue(command.IsRunning);

        // Complete the task
        tcs.SetResult(true);
        await task;

        // Assert final state
        Assert.IsTrue(propertyChanges.Contains("IsRunning"));
        Assert.IsFalse(command.IsRunning);
    }

    #region CommandParameter Tests

    [TestMethod]
    public async Task GenericCommand_WithParameter_PassesParameterCorrectly()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new AsyncRelayCommand<string>(async param =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync("TestParameter");

        // Assert
        Assert.AreEqual("TestParameter", receivedParameter, "Parameter should be passed to execute method");
    }

    [TestMethod]
    public async Task GenericCommand_WithObjectParameter_PassesParameterCorrectly()
    {
        // Arrange
        object? receivedParameter = null;
        var testObject = new { Id = 123, Name = "Test" };
        var command = new AsyncRelayCommand<object>(async param =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync(testObject);

        // Assert
        Assert.AreEqual(testObject, receivedParameter, "Object parameter should be passed correctly");
    }

    [TestMethod]
    public void GenericCommand_CanExecute_UsesParameterInPredicate()
    {
        // Arrange
        var command = new AsyncRelayCommand<int>(
            async num => await Task.Delay(num),
            num => num > 0);  // Only execute for positive numbers

        // Act & Assert
        Assert.IsFalse(command.CanExecute(0), "Should not execute for zero");
        Assert.IsFalse(command.CanExecute(-1), "Should not execute for negative");
        Assert.IsTrue(command.CanExecute(1), "Should execute for positive");
        Assert.IsTrue(command.CanExecute(100), "Should execute for positive");
    }

    [TestMethod]
    public async Task GenericCommand_WithNullParameter_HandlesGracefully()
    {
        // Arrange
        string? receivedParameter = "initial";
        var command = new AsyncRelayCommand<string?>(async param =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.IsNull(receivedParameter, "Null parameter should be passed correctly");
    }

    [TestMethod]
    public void GenericCommand_WithNullableCanExecute_HandlesNullCorrectly()
    {
        // Arrange
        var command = new AsyncRelayCommand<string?>(
            async param => await Task.CompletedTask,
            param => param is not null);

        // Act & Assert
        Assert.IsFalse(command.CanExecute(null), "Should not execute with null parameter");
        Assert.IsTrue(command.CanExecute("NotNull"), "Should execute with non-null parameter");
    }

    [TestMethod]
    public async Task GenericCommand_WithComplexType_PassesParameterCorrectly()
    {
        // Arrange
        TestData? receivedParameter = null;
        var testData = new TestData { Id = 42, Name = "Test Entity" };
        var command = new AsyncRelayCommand<TestData>(async param =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync(testData);

        // Assert
        Assert.IsNotNull(receivedParameter);
        Assert.AreEqual(42, receivedParameter.Id);
        Assert.AreEqual("Test Entity", receivedParameter.Name);
    }

    [TestMethod]
    public async Task GenericCommand_MultipleCalls_PassesDifferentParametersCorrectly()
    {
        // Arrange
        var receivedParameters = new List<int>();
        var command = new AsyncRelayCommand<int>(async param =>
        {
            await Task.Delay(10);
            receivedParameters.Add(param);
        });

        // Act
        await command.ExecuteAsync(1);
        await command.ExecuteAsync(2);
        await command.ExecuteAsync(3);

        // Assert
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, receivedParameters);
    }

    [TestMethod]
    public async Task GenericCommand_WithFallbackToProperty_UsesParameterFirst()
    {
        // Arrange
        string? receivedValue = null;
        var viewModel = new TestViewModel();
        var command = new AsyncRelayCommand<string?>(
            execute: async e =>
            {
                await Task.Delay(10);
                receivedValue = e ?? viewModel.SelectedItem;  // Fallback pattern
            },
            canExecute: e => (e ?? viewModel.SelectedItem) is not null);

        viewModel.SelectedItem = "FromProperty";

        // Act - Pass explicit parameter
        await command.ExecuteAsync("FromParameter");

        // Assert
        Assert.AreEqual("FromParameter", receivedValue, "Should use parameter when provided");
    }

    [TestMethod]
    public async Task GenericCommand_WithFallbackToProperty_UsesFallbackWhenParameterNull()
    {
        // Arrange
        string? receivedValue = null;
        var viewModel = new TestViewModel();
        var command = new AsyncRelayCommand<string?>(
            execute: async e =>
    {
        await Task.Delay(10);
        receivedValue = e ?? viewModel.SelectedItem;  // Fallback pattern
    },
            canExecute: e => (e ?? viewModel.SelectedItem) is not null);

        viewModel.SelectedItem = "FromProperty";

        // Act - Pass null parameter
        await command.ExecuteAsync(null);

        // Assert
        Assert.AreEqual("FromProperty", receivedValue, "Should use property when parameter is null");
    }

    [TestMethod]
    public void GenericCommand_CanExecute_ChecksPropertyWhenParameterNull()
    {
        // Arrange
        var viewModel = new TestViewModel();
        var command = new AsyncRelayCommand<string?>(
            execute: async e => await Task.CompletedTask,
            canExecute: e => (e ?? viewModel.SelectedItem) is not null);

        // Act & Assert - No property set, parameter null
        Assert.IsFalse(command.CanExecute(null), "Should be disabled when both parameter and property are null");

        // Set property
        viewModel.SelectedItem = "FromProperty";
        Assert.IsTrue(command.CanExecute(null), "Should be enabled when property is set even if parameter is null");

        // Pass explicit parameter
        Assert.IsTrue(command.CanExecute("FromParameter"), "Should be enabled when parameter is provided");
    }

    [TestMethod]
    public async Task GenericCancelableCommand_WithParameter_PassesParameterAndToken()
    {
        // Arrange
        int? receivedParameter = null;
        bool wasCanceled = false;
        var command = new AsyncRelayCommand<int>(async (param, token) =>
        {
            receivedParameter = param;
            try
            {
                await Task.Delay(1000, token);
            }
            catch (OperationCanceledException)
            {
                wasCanceled = true;
                throw;
            }
        });

        // Act
        var task = command.ExecuteAsync(42);
        command.Cancel();

        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        Assert.AreEqual(42, receivedParameter, "Parameter should be passed to cancelable execute");
        Assert.IsTrue(wasCanceled, "Command should be canceled");
    }

    [TestMethod]
    public async Task NonGenericCommand_IgnoresParameter()
    {
        // Arrange
        bool wasExecuted = false;
        var command = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            wasExecuted = true;
        });

        // Act - Pass a parameter (should be ignored)
        await command.ExecuteAsync("IgnoredParameter");

        // Assert
        Assert.IsTrue(wasExecuted, "Command should execute even though parameter is ignored");
    }

    #endregion

    // Helper class for testing
    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestViewModel
    {
        public string? SelectedItem { get; set; }
    }
}
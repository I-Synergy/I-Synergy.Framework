﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class AsyncRelayCommandGenericsTests
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
    public async Task Cancel_WithGenericParameter_PreservesCancellationState()
    {
        // Arrange
        var executedParameters = new List<int>();
        var command = new AsyncRelayCommand<int>(async (param, token) =>
        {
            try
            {
                await Task.Delay(1000, token);
                executedParameters.Add(param);
            }
            catch (OperationCanceledException)
            {
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
            // Expected exception
        }

        // Assert
        CollectionAssert.AreEqual(new int[0], executedParameters);
        Assert.IsTrue(command.IsCancellationRequested);

        // Verify exception was handled
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(It.IsAny<OperationCanceledException>()),
            Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithParameter_PassesParameterCorrectly()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new AsyncRelayCommand<string>(async (param) =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync("test");

        // Assert
        Assert.AreEqual("test", receivedParameter);
    }

    [TestMethod]
    public void CanExecute_WithGenericPredicate_ReturnsExpectedResult()
    {
        // Arrange
        var command = new AsyncRelayCommand<int>(
            (param) => Task.CompletedTask,
            (param) => param > 0
        );

        // Act & Assert
        Assert.IsFalse(command.CanExecute(-1));
        Assert.IsTrue(command.CanExecute(1));
    }

    [TestMethod]
    public async Task ExecuteAsync_WithNullableParameter_HandlesNullCorrectly()
    {
        // Arrange
        string? receivedParameter = "not null";
        var command = new AsyncRelayCommand<string?>(async (param) =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.IsNull(receivedParameter);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithValueType_HandlesDefaultValueCorrectly()
    {
        // Arrange
        int receivedParameter = -1;
        var command = new AsyncRelayCommand<int>(async (param) =>
        {
            await Task.Delay(10);
            receivedParameter = param;
        });

        // Act
        await command.ExecuteAsync(default);

        // Assert
        Assert.AreEqual(0, receivedParameter);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithComplexType_PassesReferenceCorrectly()
    {
        // Arrange
        var testObject = new TestClass { Value = "test" };
        TestClass? receivedObject = null;
        var command = new AsyncRelayCommand<TestClass>(async (param) =>
        {
            await Task.Delay(10);
            receivedObject = param;
        });

        // Act
        await command.ExecuteAsync(testObject);

        // Assert
        Assert.AreSame(testObject, receivedObject);
    }

    [TestMethod]
    public async Task ConcurrentExecutions_WithGenericParameter_MaintainsParameterIntegrity()
    {
        // Arrange
        var executedParameters = new List<int>();
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand<int>(
            async (param) =>
            {
                executedParameters.Add(param);
                await tcs.Task;
            },
            options: AsyncRelayCommandOptions.AllowConcurrentExecutions
        );

        // Act
        var task1 = command.ExecuteAsync(1);
        var task2 = command.ExecuteAsync(2);

        tcs.SetResult(true);
        await Task.WhenAll(task1, task2);

        // Assert
        CollectionAssert.AreEqual(new[] { 1, 2 }, executedParameters);
    }

    [TestMethod]
    public void CanExecute_WithNonGenericParameter_ValidatesTypeCorrectly()
    {
        // Arrange
        var command = new AsyncRelayCommand<int>(
            (param) => Task.CompletedTask,
            (param) => param > 0
        );

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => command.CanExecute("invalid type"));
        Assert.IsFalse(command.CanExecute(null));  // null for value type
        Assert.IsTrue(command.CanExecute(1));      // valid value
    }

    [TestMethod]
    public void NotifyCanExecuteChanged_TriggersEventCorrectly()
    {
        // Arrange
        var command = new AsyncRelayCommand<string>(_ => Task.CompletedTask);
        bool wasNotified = false;
        command.CanExecuteChanged += (s, e) => wasNotified = true;

        // Act
        command.NotifyCanExecuteChanged();

        // Assert
        Assert.IsTrue(wasNotified);
    }

    [TestMethod]
    public async Task ExecutionTask_WithMultipleExecutions_UpdatesCorrectly()
    {
        // Arrange
        var taskCompletionSource1 = new TaskCompletionSource<bool>();
        var taskCompletionSource2 = new TaskCompletionSource<bool>();
        int executionCount = 0;

        var command = new AsyncRelayCommand<int>(async (param) =>
        {
            executionCount++;
            if (executionCount == 1)
            {
                await taskCompletionSource1.Task;
            }
            else
            {
                await taskCompletionSource2.Task;
            }
        });

        // Act & Assert - First Execution
        var task1 = command.ExecuteAsync(1);
        Assert.IsNotNull(command.ExecutionTask, "ExecutionTask should not be null after first execution");
        Assert.IsFalse(command.ExecutionTask.IsCompleted, "First execution should not be completed yet");

        // Complete first task
        taskCompletionSource1.SetResult(true);
        await task1;
        Assert.IsTrue(command.ExecutionTask.IsCompleted, "ExecutionTask should be completed after first task completion");

        // Act & Assert - Second Execution
        var task2 = command.ExecuteAsync(2);
        Assert.IsNotNull(command.ExecutionTask, "ExecutionTask should not be null after second execution");
        Assert.IsFalse(command.ExecutionTask.IsCompleted, "Second execution should not be completed yet");
        Assert.AreNotEqual(task1, command.ExecutionTask, "ExecutionTask should be updated for second execution");

        // Complete second task
        taskCompletionSource2.SetResult(true);
        await task2;
        Assert.IsTrue(command.ExecutionTask.IsCompleted, "ExecutionTask should be completed after second task completion");

        // Verify execution count
        Assert.AreEqual(2, executionCount, "Command should have executed twice");
    }

    [TestMethod]
    public void CanExecute_WithStructParameter_HandlesDefaultValue()
    {
        // Arrange
        var command = new AsyncRelayCommand<DateTime>(
            (param) => Task.CompletedTask,
            (param) => param != default);

        // Act & Assert
        Assert.IsFalse(command.CanExecute(default));
        Assert.IsTrue(command.CanExecute(DateTime.Now));
    }

    [TestMethod]
    public void CanExecute_WithBoxedValueType_HandlesCorrectly()
    {
        // Arrange
        var command = new AsyncRelayCommand<int>(
            (param) => Task.CompletedTask,
            (param) => param > 0);

        // Act & Assert
        object boxedValue = 42;
        Assert.IsTrue(command.CanExecute(boxedValue));
    }

    [TestMethod]
    public async Task ExecuteAsync_WithConcurrentCancellation_HandlesCorrectly()
    {
        // Arrange
        int completedCount = 0;
        var command = new AsyncRelayCommand<int>(async (param, token) =>
        {
            try
            {
                await Task.Delay(1000, token);
                Interlocked.Increment(ref completedCount);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }, options: AsyncRelayCommandOptions.AllowConcurrentExecutions);

        // Act
        var task1 = command.ExecuteAsync(1);
        var task2 = command.ExecuteAsync(2);
        await Task.Delay(100); // Let tasks start
        command.Cancel();

        try
        {
            await Task.WhenAll(task1, task2);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        Assert.AreEqual(0, completedCount);
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(It.IsAny<OperationCanceledException>()),
            Times.Exactly(2));
    }

    [TestMethod]
    public void Constructor_WithNullParameter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new AsyncRelayCommand<string>((Func<string?, Task>)null!));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new AsyncRelayCommand<string>((Func<string?, CancellationToken, Task>)null!));
    }

    [TestMethod]
    public async Task PropertyChanged_NotifiesInCorrectOrder()
    {
        // Arrange
        var propertyChanges = new List<string>();
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand<string>(async (param) =>
        {
            await tcs.Task;
        });

        command.PropertyChanged += (s, e) => propertyChanges.Add(e.PropertyName!);

        // Act
        var task = command.ExecuteAsync("test");
        tcs.SetResult(true);
        await task;

        // Assert
        Assert.IsTrue(propertyChanges.IndexOf("ExecutionTask") < propertyChanges.IndexOf("IsRunning"));
    }

    private class TestClass
    {
        public string? Value { get; set; }
    }
}
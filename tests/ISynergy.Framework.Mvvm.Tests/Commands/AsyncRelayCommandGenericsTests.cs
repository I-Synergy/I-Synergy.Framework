using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class AsyncRelayCommandGenericsTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    public AsyncRelayCommandGenericsTests()
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
    public async Task Cancel_WithGenericParameter_PreservesCancellationState()
    {
        // Arrange
        var executedParameters = new List<int>();
        var command = new AsyncRelayCommand<int>(async (param, token) =>
        {
            try
            {
                await Task.Delay(5000, token);
                executedParameters.Add(param);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        });

        // Act
        var task = command.ExecuteAsync(42);

        // Give a small delay to ensure the task has started
        await Task.Delay(100);

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
        Assert.Throws<ArgumentException>(() => command.CanExecute("invalid type"));
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
        var propertyChanges = new List<string>();

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

        command.PropertyChanged += (s, e) => propertyChanges.Add($"{e.PropertyName}: {command.ExecutionTask?.IsCompleted}");

        // Act & Assert - First Execution
        var task1 = command.ExecuteAsync(1);

        // Wait a small amount to ensure command state is updated
        await Task.Delay(50);

        Assert.IsNotNull(command.ExecutionTask, "ExecutionTask should not be null after first execution");
        Assert.IsFalse(command.ExecutionTask.IsCompleted, "First execution should not be completed yet");

        // Complete first task
        taskCompletionSource1.SetResult(true);
        await task1;

        // Wait for property changes to propagate
        await Task.Delay(50);

        // Act & Assert - Second Execution
        var task2 = command.ExecuteAsync(2);

        // Wait a small amount to ensure command state is updated
        await Task.Delay(50);

        var executionTask = command.ExecutionTask;
        Assert.IsNotNull(executionTask, "ExecutionTask should not be null after second execution");
        Assert.IsFalse(executionTask.IsCompleted, "Second execution should not be completed yet");

        // Complete second task
        taskCompletionSource2.SetResult(true);
        await task2;

        // Wait for property changes to propagate
        await Task.Delay(50);

        // Log the property changes if test fails
        try
        {
            Assert.AreEqual(2, executionCount, "Command should have executed twice");
        }
        catch
        {
            Console.WriteLine("Property change sequence:");
            foreach (var change in propertyChanges)
            {
                Console.WriteLine(change);
            }
            throw;
        }
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
    }

    [TestMethod]
    public void Constructor_WithNullParameter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new AsyncRelayCommand<string>((Func<string?, Task>)null!));

        Assert.Throws<ArgumentNullException>(() =>
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

    [TestMethod]
    public async Task ExecuteAsync_WithFlowExceptionsToTaskScheduler_DoesNotHandleException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand<string>(
            async (param) =>
            {
                await Task.Delay(10);
                throw expectedException;
            },
            options: AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler
        );

        // Act & Assert
        var task = command.ExecuteAsync("test");

        // The exception should flow to the task rather than being handled internally
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await task);

        Assert.AreEqual(expectedException, actualException);
    }

    [TestMethod]
    public void CanExecute_WithNullParameter_ReturnsExpectedResult()
    {
        // Arrange
        bool predicateCalled = false;
        var command = new AsyncRelayCommand<string?>(
            (param) => Task.CompletedTask,
            (param) =>
            {
                predicateCalled = true;
                return param == null;
            }
        );

        // Act
        bool canExecute = command.CanExecute(null);

        // Assert
        Assert.IsTrue(predicateCalled, "Predicate should be called even with null parameter");
        Assert.IsTrue(canExecute, "Command should be executable with null parameter when predicate allows it");
    }

    [TestMethod]
    public void CanExecute_WithNullParameterAndNoExplicitPredicate_ReturnsTrue()
    {
        // Arrange - command with no explicit predicate
        var command = new AsyncRelayCommand<string?>(
            (param) => Task.CompletedTask
        );

        // Act
        bool canExecute = command.CanExecute(null);

        // Assert
        Assert.IsTrue(canExecute, "Command with no explicit predicate should be executable with null parameter");
    }


    private class TestClass
    {
        public string? Value { get; set; }
    }
}

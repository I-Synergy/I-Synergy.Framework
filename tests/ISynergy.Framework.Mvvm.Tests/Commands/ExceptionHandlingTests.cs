using Microsoft.Extensions.DependencyInjection;
using Moq;
using ISynergy.Framework.Mvvm.Enumerations;

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

    #region RelayCommand Exception Handling Tests

    [TestMethod]
    public void RelayCommand_Execute_WithException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand(() => throw expectedException);

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => command.Execute(null));
        Assert.AreEqual(expectedException, thrownException);
    }

    [TestMethod]
    public void RelayCommand_Execute_WithException_WhenHandlerNotAvailable_Rethrows()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand(() => throw expectedException);

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => command.Execute(null));
        Assert.AreEqual(expectedException, thrownException);
    }

    #endregion

    #region RelayCommand<T> Exception Handling Tests

    [TestMethod]
    public void RelayCommandGeneric_Execute_WithException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand<string>(_ => throw expectedException);

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => command.Execute("test"));
        Assert.AreEqual(expectedException, thrownException);
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
    public void AsyncRelayCommand_Execute_WithSynchronousException_Rethrows()
    {
        // Default FlowExceptionsToTaskScheduler -> exception propagates
        var expectedException = new InvalidOperationException("Test exception");
        var command = new AsyncRelayCommand(() => throw expectedException);

        var thrownException = Assert.Throws<InvalidOperationException>(() => command.Execute(null));
        Assert.AreEqual(expectedException, thrownException);
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





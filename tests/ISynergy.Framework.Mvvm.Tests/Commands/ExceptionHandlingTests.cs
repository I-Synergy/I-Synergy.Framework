using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
    private Mock<IExceptionHandlerService> _mockExceptionHandler;

    public ExceptionHandlingTests()
    {
        // Setup mocks
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceScope = new Mock<IServiceScope>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();

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
        ServiceLocator.Default.Dispose();
    }

    #region RelayCommand Exception Handling Tests

    [TestMethod]
    public void RelayCommand_Execute_WithException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        var command = new RelayCommand(() => throw expectedException);

        // Act
        try
        {
            command.Execute(null);
            Assert.Fail("Expected exception was not thrown");
        }
        catch (InvalidOperationException)
        {
            // Expected - exception is rethrown after handling
        }

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public void RelayCommand_Execute_WithException_WhenHandlerNotAvailable_Rethrows()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns((IExceptionHandlerService?)null);

        var command = new RelayCommand(() => throw expectedException);

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => command.Execute(null));
        Assert.AreEqual(expectedException, thrownException);
    }

    #endregion

    #region RelayCommand<T> Exception Handling Tests

    [TestMethod]
    public void RelayCommandGeneric_Execute_WithException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        var command = new RelayCommand<string>(_ => throw expectedException);

        // Act
        try
        {
            command.Execute("test");
            Assert.Fail("Expected exception was not thrown");
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    #endregion

    #region AsyncRelayCommand Exception Handling Tests

    [TestMethod]
    public async Task AsyncRelayCommand_ExecuteAsync_WithSynchronousException_CreatesFaultedTask()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
      .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
       .Returns(_mockExceptionHandler.Object);

        // Lambda throws synchronously
        var command = new AsyncRelayCommand(() => throw expectedException);

        // Act
        var task = command.ExecuteAsync(null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => task);

        // Assert - exception handler should be called
        await Task.Delay(100); // Give time for async handling
      _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => 
      e == expectedException || e.InnerException == expectedException)), Times.Once);
    }

    [TestMethod]
    public async Task AsyncRelayCommand_ExecuteAsync_WithAsynchronousException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
     .Returns(_mockExceptionHandler.Object);

        var command = new AsyncRelayCommand(async () =>
    {
    await Task.Delay(10);
  throw expectedException;
        });

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync(null));

        // Assert
      await Task.Delay(100); // Give time for async handling
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => 
            e == expectedException || e.InnerException == expectedException)), Times.Once);
    }

    [TestMethod]
    public async Task AsyncRelayCommand_Execute_WithSynchronousException_CallsExceptionHandlerService()
    {
        // Arrange
 var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
 .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
      .Returns(_mockExceptionHandler.Object);

        var command = new AsyncRelayCommand(() => throw expectedException);

        // Act
        command.Execute(null); // This calls ExecuteAsync and AwaitAndThrowIfFailed

        // Assert - exception handler should be called via AwaitAndThrowIfFailed
        await Task.Delay(100); // Give time for async handling
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => 
       e == expectedException || e.InnerException == expectedException)), Times.Once);
    }

    #endregion

    #region AsyncRelayCommand<T> Exception Handling Tests

    [TestMethod]
    public async Task AsyncRelayCommandGeneric_ExecuteAsync_WithSynchronousException_CreatesFaultedTask()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
      .Returns(_mockExceptionHandler.Object);

        var command = new AsyncRelayCommand<string>(_ => throw expectedException);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync("test"));

        // Assert
        await Task.Delay(100);
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => 
    e == expectedException || e.InnerException == expectedException)), Times.Once);
    }

    #endregion
}



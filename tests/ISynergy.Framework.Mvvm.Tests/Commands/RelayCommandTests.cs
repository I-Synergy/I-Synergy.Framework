using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;
[TestClass]
public class RelayCommandTests
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
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public void Execute_SimpleCommand_ExecutesSuccessfully()
    {
        // Arrange
        bool wasExecuted = false;
        var command = new RelayCommand(() => wasExecuted = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.IsTrue(wasExecuted);
    }

    [TestMethod]
    public void CanExecute_WithPredicate_ReturnsExpectedResult()
    {
        // Arrange
        bool canExecuteValue = false;
        var command = new RelayCommand(
            () => { },
            () => canExecuteValue
        );

        // Act & Assert
        Assert.IsFalse(command.CanExecute(null));

        canExecuteValue = true;
        Assert.IsTrue(command.CanExecute(null));
    }

    [TestMethod]
    public void Execute_WithException_HandlesExceptionCorrectly()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand(() => throw expectedException);

        // Act
        command.Execute(null);

        // Assert
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(expectedException),
            Times.Once);
    }

    [TestMethod]
    public void Execute_WithNestedException_HandlesInnerExceptionCorrectly()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception");
        var outerException = new Exception("Outer exception", innerException);
        var command = new RelayCommand(() => throw outerException);

        // Act
        command.Execute(null);

        // Assert
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(innerException),
            Times.Once);
    }

    [TestMethod]
    public void NotifyCanExecuteChanged_TriggersEvent()
    {
        // Arrange
        var command = new RelayCommand(() => { });
        bool wasNotified = false;
        command.CanExecuteChanged += (s, e) => wasNotified = true;

        // Act
        command.NotifyCanExecuteChanged();

        // Assert
        Assert.IsTrue(wasNotified);
    }
}

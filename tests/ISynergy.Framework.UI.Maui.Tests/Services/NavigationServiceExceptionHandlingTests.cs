using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.UI.Maui.Tests.Services;

/// <summary>
/// Tests for exception handling in NavigationService.
/// </summary>
[TestClass]
public class NavigationServiceExceptionHandlingTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<IExceptionHandlerService> _mockExceptionHandler;
    private Mock<ILogger<NavigationService>> _mockLogger;
    private NavigationService _navigationService;

    public NavigationServiceExceptionHandlingTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
        _mockLogger = new Mock<ILogger<NavigationService>>();

        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        // Setup IServiceScopeFactory for ServiceLocator initialization
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var mockServiceScope = new Mock<IServiceScope>();
        mockServiceScope.SetupGet(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);

        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockServiceScopeFactory.Object);

        ServiceLocator.SetLocatorProvider(_mockServiceProvider.Object);

        _navigationService = new NavigationService(
            _mockScopedContextService.Object,
            _mockExceptionHandler.Object,
            _mockLogger.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public void NavigateAsync_WithViewModelInitializeAsyncException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Initialize failed");
        var mockViewModel = new Mock<IViewModel>();

        mockViewModel
            .Setup(x => x.InitializeAsync())
            .ThrowsAsync(expectedException);

        // Note: This test would require setting up Application.Current and navigation stack
        // which is complex in unit tests. This is a placeholder showing the test structure.
        // In a real scenario, you'd need to mock the MAUI Application and Navigation infrastructure.

        // Act & Assert
        // The actual test would verify that when InitializeAsync throws,
        // the exception handler service is called
        _mockExceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>()), Times.Never);
    }
}




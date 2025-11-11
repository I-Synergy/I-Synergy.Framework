using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.UI.WinUI.Tests.Services;

/// <summary>
/// Tests for exception handling in NavigationService for WinUI.
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
    public void SafeOnNavigatedTo_WithException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("OnNavigatedTo failed");
        var mockViewModel = new Mock<IViewModel>();

        mockViewModel
            .Setup(x => x.OnNavigatedTo())
            .Throws(expectedException);

        // Act - Use reflection to call the private SafeOnNavigatedTo method
        var method = typeof(NavigationService).GetMethod("SafeOnNavigatedTo",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(_navigationService, new object[] { mockViewModel.Object });
        }
        else
        {
            Assert.Fail("SafeOnNavigatedTo method not found");
        }

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public void SafeOnNavigatedFrom_WithException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("OnNavigatedFrom failed");
        var mockViewModel = new Mock<IViewModel>();

        mockViewModel
            .Setup(x => x.OnNavigatedFrom())
            .Throws(expectedException);

        // Act - Use reflection to call the private SafeOnNavigatedFrom method
        var method = typeof(NavigationService).GetMethod("SafeOnNavigatedFrom",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(_navigationService, new object[] { mockViewModel.Object });
        }
        else
        {
            Assert.Fail("SafeOnNavigatedFrom method not found");
        }

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public void SafeOnNavigatedTo_WithException_WhenHandlerServiceNotAvailable_LogsError()
    {
        // Arrange
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns((IExceptionHandlerService?)null);

        var expectedException = new InvalidOperationException("OnNavigatedTo failed");
        var mockViewModel = new Mock<IViewModel>();

        mockViewModel
            .Setup(x => x.OnNavigatedTo())
            .Throws(expectedException);

        // Act
        var method = typeof(NavigationService).GetMethod("SafeOnNavigatedTo",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(_navigationService, new object[] { mockViewModel.Object });
        }

        // Assert - should log error
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("OnNavigatedTo")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}




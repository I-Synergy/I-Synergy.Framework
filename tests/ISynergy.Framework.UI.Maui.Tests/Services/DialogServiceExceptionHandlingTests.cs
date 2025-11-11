using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.UI.Maui.Tests.Services;

/// <summary>
/// Tests for exception handling in DialogService.
/// </summary>
[TestClass]
public class DialogServiceExceptionHandlingTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<IExceptionHandlerService> _mockExceptionHandler;
    private Mock<ILogger<DialogService>> _mockLogger;
    private DialogService _dialogService;

    public DialogServiceExceptionHandlingTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockLanguageService = new Mock<ILanguageService>();
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
        _mockLogger = new Mock<ILogger<DialogService>>();

        _mockLanguageService
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(s => s);

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

        _dialogService = new DialogService(
            _mockScopedContextService.Object,
            _mockExceptionHandler.Object,
            _mockLanguageService.Object,
            _mockLogger.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public async Task CreateDialogAsync_WithViewModelInitializeAsyncException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Initialize failed");
        var mockViewModel = new Mock<IViewModelDialog<string>>();

        mockViewModel
            .Setup(x => x.InitializeAsync())
            .ThrowsAsync(expectedException);

        var mockWindow = new Mock<ISynergy.Framework.Mvvm.Abstractions.IWindow>();

        _mockScopedContextService
     .Setup(x => x.GetRequiredService(It.IsAny<Type>()))
     .Returns(mockViewModel.Object);

        // Act
        try
        {
            await _dialogService.CreateDialogAsync(mockWindow.Object, mockViewModel.Object);
        }
        catch
        {
            // Expected - exception is handled but may still propagate
        }

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithException_CallsExceptionHandlerService()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Dialog creation failed");

        _mockScopedContextService
            .Setup(x => x.GetRequiredService(It.IsAny<Type>()))
            .Throws(expectedException);

        // Act
        try
        {
            await _dialogService.ShowDialogAsync<TestWindow, TestViewModelDialog, string>();
        }
        catch
        {
            // Expected
        }

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    // Test window and viewmodel types for testing
    private interface TestWindow : ISynergy.Framework.Mvvm.Abstractions.IWindow { }

    private class TestViewModelDialog : ViewModelDialog<string>
    {
        public TestViewModelDialog(ICommonServices commonServices, ILogger<ViewModelDialog<string>> logger)
            : base(commonServices, logger)
        {
        }

        public void SetSelectedItem(string item) => SelectedItem = item;
    }
}




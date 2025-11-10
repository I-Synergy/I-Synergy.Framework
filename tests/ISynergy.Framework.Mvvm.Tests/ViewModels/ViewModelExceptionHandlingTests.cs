using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

/// <summary>
/// Tests for exception handling in ViewModel Submitted events.
/// </summary>
[TestClass]
public class ViewModelExceptionHandlingTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IExceptionHandlerService> _mockExceptionHandler;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ILogger<TestViewModelDialog>> _mockLogger;

    public ViewModelExceptionHandlingTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockLogger = new Mock<ILogger<TestViewModelDialog>>();

        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        ServiceLocator.SetLocatorProvider(_mockServiceProvider.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public void ViewModelDialog_OnSubmitted_WithExceptionInHandler_CallsExceptionHandlerService()
    {
        // Arrange
        var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");

        viewModel.Submitted += (s, e) => throw expectedException;

        // Act
        viewModel.TestSubmit(submitEventArgs);

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public void ViewModelDialog_OnSubmitted_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled()
    {
        // Arrange
        var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");
        bool handler1Called = false;
        bool handler2Called = false;
        bool handler3Called = false;

        viewModel.Submitted += (s, e) => handler1Called = true;
        viewModel.Submitted += (s, e) => throw expectedException;
        viewModel.Submitted += (s, e) => handler3Called = true;

        // Act
        viewModel.TestSubmit(submitEventArgs);

        // Assert
        Assert.IsTrue(handler1Called, "First handler should be called");
        Assert.IsTrue(handler3Called, "Third handler should be called even if second throws");
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public void ViewModelDialog_OnSubmitted_WithException_WhenHandlerServiceNotAvailable_LogsToDebug()
    {
        // Arrange
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns((IExceptionHandlerService?)null);

        var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");

        viewModel.Submitted += (s, e) => throw expectedException;

        // Act - should not crash
        viewModel.TestSubmit(submitEventArgs);

        // Assert - no exception should propagate
        // The method should complete without throwing
    }

    [TestMethod]
    public void ViewModelNavigation_OnSubmitted_WithExceptionInHandler_CallsExceptionHandlerService()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object, Mock.Of<ILogger<TestViewModel>>());
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");

        viewModel.Submitted += (s, e) => throw expectedException;

        // Act
        viewModel.TestSubmit(submitEventArgs);

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    public class TestViewModelDialog : ViewModelDialog<string>
    {
        public TestViewModelDialog(ICommonServices commonServices, ILogger<TestViewModelDialog> logger)
            : base(commonServices, logger)
        {
        }

        public void TestSubmit(SubmitEventArgs<string> e)
        {
            OnSubmitted(e);
        }
    }

    public class TestViewModel : ViewModelNavigation<string>
    {
        public TestViewModel(ICommonServices commonServices, ILogger<TestViewModel> logger)
            : base(commonServices, logger)
        {
        }

        public void TestSubmit(SubmitEventArgs<string> e)
        {
            OnSubmitted(e);
        }
    }
}


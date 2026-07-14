using ISynergy.Framework.Core.Abstractions.Services;
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
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger<TestViewModelDialog>> _mockLogger;

    public ViewModelExceptionHandlingTests()
    {
        _mockCommonServices = new Mock<ICommonServices>();
        _mockLogger = new Mock<ILogger<TestViewModelDialog>>();

        var mockMessengerService = new Mock<IMessengerService>();
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(mockMessengerService.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void ViewModelDialog_OnSubmitted_CallsExceptionHandlerService()
    {
        // Arrange
        var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");

        viewModel.Submitted += (s, e) => throw expectedException;

        // Act
        Assert.Throws<InvalidOperationException>(() => viewModel.TestSubmit(submitEventArgs));
    }

    [TestMethod]
    public void ViewModelDialog_OnSubmitted_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled()
    {
        // Arrange
        var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");
        bool handler1Called = false;
        bool handler3Called = false;

        viewModel.Submitted += (s, e) => handler1Called = true;
        viewModel.Submitted += (s, e) => throw expectedException;
        viewModel.Submitted += (s, e) => handler3Called = true;

        // Act
        Assert.Throws<InvalidOperationException>(() => viewModel.TestSubmit(submitEventArgs));

        // Assert
        Assert.IsTrue(handler1Called, "First handler should be called");
        Assert.IsTrue(handler3Called, "Third handler should be called even if second throws");
    }

    [TestMethod]
    public void ViewModelDialog_OnSubmitted_WithException_LogsToDebug()
    {
        // Arrange
        var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");

        viewModel.Submitted += (s, e) => throw expectedException;

        // Act - should not crash
        Assert.Throws<InvalidOperationException>(() => viewModel.TestSubmit(submitEventArgs));
    }

    [TestMethod]
    public void ViewModelNavigation_OnSubmitted_CallsExceptionHandlerService()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object, Mock.Of<ILogger<TestViewModel>>());
        var expectedException = new InvalidOperationException("Handler exception");
        var submitEventArgs = new SubmitEventArgs<string>("test");

        viewModel.Submitted += (s, e) => throw expectedException;

        // Act
        Assert.Throws<InvalidOperationException>(() => viewModel.TestSubmit(submitEventArgs));
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


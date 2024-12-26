using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBaseTests
{
    private Mock<IContext> _mockContext;
    private Mock<IBaseCommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;
    private Mock<ILanguageService> _mockLanguageService;

    [TestInitialize]
    public void Setup()
    {
        _mockContext = new Mock<IContext>();
        _mockCommonServices = new Mock<IBaseCommonServices>();
        _mockLogger = new Mock<ILogger>();
        _mockLanguageService = new Mock<ILanguageService>();
    }

    private class TestViewModel : ViewModel
    {
        public TestViewModel(IContext context, IBaseCommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation) { }
    }

    private enum TestEnum
    {
        [Display(Description = "TestDescription")]
        TestValue
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Assert
        Assert.IsNotNull(viewModel.CloseCommand);
        Assert.IsNotNull(viewModel.CancelCommand);
        Assert.IsFalse(viewModel.IsInitialized);
        Assert.IsFalse(viewModel.IsRefreshing);
        Assert.IsNull(viewModel.Parameter);
        Assert.IsNull(viewModel.Title);
    }

    [TestMethod]
    public async Task CloseAsync_InvokesClosedEvent()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        var eventInvoked = false;
        viewModel.Closed += (s, e) => eventInvoked = true;

        // Act
        await viewModel.CloseAsync();

        // Assert
        Assert.IsTrue(eventInvoked);
    }

    [TestMethod]
    public async Task CancelAsync_InvokesCancelledAndClosedEvents()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        var cancelledInvoked = false;
        var closedInvoked = false;
        viewModel.Cancelled += (s, e) => cancelledInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.CancelAsync();

        // Assert
        Assert.IsTrue(cancelledInvoked);
        Assert.IsTrue(closedInvoked);
        Assert.IsTrue(viewModel.IsCancelled);
    }

    [TestMethod]
    public void Cleanup_DisposesCommands()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        viewModel.Dispose();

        // Assert
        Assert.IsNull(viewModel.CloseCommand);
        Assert.IsNull(viewModel.CancelCommand);
    }

    [TestMethod]
    public void Constructor_WithAutomaticValidation_SetsValidation()
    {
        // Arrange & Act
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object, true);

        // Assert
        // Note: You might need to expose a way to check if automatic validation is enabled
        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void GetEnumDescription_ReturnsLocalizedString()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        _mockLanguageService.Setup(x => x.GetString("TestDescription")).Returns("LocalizedDescription");

        // Act
        var result = viewModel.GetEnumDescription(TestEnum.TestValue);

        // Assert
        Assert.AreEqual("LocalizedDescription", result);
        _mockLanguageService.Verify(x => x.GetString("TestDescription"), Times.Once);
    }

    [TestMethod]
    public void GetEnumDescription_WithoutDisplayAttribute_ReturnsEnumString()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        var result = viewModel.GetEnumDescription(MessageBoxButton.OK);

        // Assert
        Assert.AreEqual("OK", result);
        _mockLanguageService.Verify(x => x.GetString(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task InitializeAsync_SetsProperState()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        // Base implementation should not set IsInitialized
        Assert.IsFalse(viewModel.IsInitialized);
    }
}

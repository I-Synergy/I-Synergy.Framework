using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBladeWizardTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILoggerFactory> _mockLoggerFactory;

    public ViewModelBladeWizardTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(new Mock<ILogger>().Object);
        _mockCommonServices.SetupGet(s => s.LoggerFactory).Returns(_mockLoggerFactory.Object);
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestBladeWizardViewModel : ViewModelBladeWizard<TestEntity>
    {
        public TestBladeWizardViewModel(ICommonServices commonServices, bool automaticValidation = false)
            : base(commonServices, automaticValidation) { }
    }

    [TestMethod]
    public void Constructor_InitializesWizardProperties()
    {
        // Arrange & Act
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);

        // Assert
        Assert.IsNotNull(viewModel.BackCommand);
        Assert.IsNotNull(viewModel.NextCommand);
        Assert.IsNotNull(viewModel.SubmitCommand);

        Assert.AreEqual(1, viewModel.Page);

        Assert.IsFalse(viewModel.Back_IsEnabled);
        Assert.IsFalse(viewModel.Next_IsEnabled);
        Assert.IsFalse(viewModel.Submit_IsEnabled);
    }

    [TestMethod]
    public void OnPropertyChanged_Page_UpdatesNavigationState_SinglePage()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);

        // Act
        viewModel.Pages = 1;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsFalse(viewModel.Back_IsEnabled);
        Assert.IsFalse(viewModel.Next_IsEnabled);

        Assert.IsTrue(viewModel.Submit_IsEnabled);
    }

    [TestMethod]
    public void OnPropertyChanged_Page_UpdatesNavigationState_MultiplePages()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);

        // Act - First Page
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert - First Page
        Assert.IsFalse(viewModel.Back_IsEnabled, "Back should be disabled on first page");
        Assert.IsTrue(viewModel.Next_IsEnabled, "Next should be enabled on first page");
        Assert.IsFalse(viewModel.Submit_IsEnabled, "Submit should be disabled on first page");

        // Act - Middle Page
        viewModel.Page = 2;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert - Middle Page
        Assert.IsTrue(viewModel.Back_IsEnabled, "Back should be enabled on middle page");
        Assert.IsTrue(viewModel.Next_IsEnabled, "Next should be enabled on middle page");
        Assert.IsFalse(viewModel.Submit_IsEnabled, "Submit should be disabled on middle page");

        // Act - Last Page
        viewModel.Page = 3;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert - Last Page
        Assert.IsTrue(viewModel.Back_IsEnabled, "Back should be enabled on last page");
        Assert.IsFalse(viewModel.Next_IsEnabled, "Next should be disabled on last page");
        Assert.IsTrue(viewModel.Submit_IsEnabled, "Submit should be enabled on last page");
    }

    [TestMethod]
    public void BackCommand_DecreasesPage()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);
        viewModel.Pages = 3;
        viewModel.Page = 2;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Act
        viewModel.BackCommand.Execute(null);

        // Assert
        Assert.AreEqual(1, viewModel.Page);
    }

    [TestMethod]
    public void NextCommand_IncreasesPage()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Act
        viewModel.NextCommand.Execute(null);

        // Assert
        Assert.AreEqual(2, viewModel.Page);
    }

    [TestMethod]
    public void BackCommand_AtFirstPage_CannotExecute()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsFalse(viewModel.Back_IsEnabled);
    }

    [TestMethod]
    public void NextCommand_AtLastPage_CannotExecute()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);
        viewModel.Pages = 3;
        viewModel.Page = 3;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsFalse(viewModel.Next_IsEnabled);
    }

    [TestMethod]
    public void Cleanup_ClearsNavigationCommands()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);

        // Act
        viewModel.Dispose();

        // Assert
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndCloses()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockCommonServices.Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var submittedInvoked = false;
        var closedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(submittedInvoked);
        Assert.IsTrue(closedInvoked);
    }
}

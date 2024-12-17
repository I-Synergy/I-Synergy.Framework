using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBladeWizardTests
{
    private Mock<IContext> _mockContext;
    private Mock<IBaseCommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockContext = new Mock<IContext>();
        _mockCommonServices = new Mock<IBaseCommonServices>();
        _mockLogger = new Mock<ILogger>();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class TestBladeWizardViewModel : ViewModelBladeWizard<TestEntity>
    {
        public TestBladeWizardViewModel(IContext context, IBaseCommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation) { }

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [TestMethod]
    public void Constructor_InitializesWizardProperties()
    {
        // Arrange & Act
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        viewModel.Dispose();

        // Assert
        Assert.IsNull(viewModel.BackCommand);
        Assert.IsNull(viewModel.NextCommand);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndCloses()
    {
        // Arrange
        var viewModel = new TestBladeWizardViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
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

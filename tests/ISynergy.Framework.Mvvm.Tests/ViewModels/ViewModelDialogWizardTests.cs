using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelDialogWizardTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    public ViewModelDialogWizardTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockLogger = new Mock<ILogger>();
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestDialogWizardViewModel : ViewModelDialogWizard<TestEntity>
    {
        public TestDialogWizardViewModel(ICommonServices commonServices, ILogger<TestDialogWizardViewModel> logger, bool automaticValidation = false)
            : base(commonServices, logger, automaticValidation) { }
    }

    [TestMethod]
    public void Constructor_InitializesWizardProperties()
    {
        // Arrange & Act
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);

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
    public void OnPropertyChanged_Page_UpdatesNavigationState_FirstPage()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);

        // Act
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsFalse(viewModel.Back_IsEnabled, "Back should be disabled on first page");
        Assert.IsTrue(viewModel.Next_IsEnabled, "Next should be enabled on first page");
        Assert.IsFalse(viewModel.Submit_IsEnabled, "Submit should be disabled on first page");
    }

    [TestMethod]
    public void OnPropertyChanged_Page_UpdatesNavigationState_MiddlePage()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);

        // Act
        viewModel.Pages = 3;
        viewModel.Page = 2;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsTrue(viewModel.Back_IsEnabled, "Back should be enabled on middle page");
        Assert.IsTrue(viewModel.Next_IsEnabled, "Next should be enabled on middle page");
        Assert.IsFalse(viewModel.Submit_IsEnabled, "Submit should be disabled on middle page");
    }

    [TestMethod]
    public void OnPropertyChanged_Page_UpdatesNavigationState_LastPage()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);

        // Act
        viewModel.Pages = 3;
        viewModel.Page = 3;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsTrue(viewModel.Back_IsEnabled, "Back should be enabled on last page");
        Assert.IsFalse(viewModel.Next_IsEnabled, "Next should be disabled on last page");
        Assert.IsTrue(viewModel.Submit_IsEnabled, "Submit should be enabled on last page");
    }

    [TestMethod]
    public void OnPropertyChanged_Page_UpdatesNavigationState_SinglePage()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);

        // Act
        viewModel.Pages = 1;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Assert
        Assert.IsFalse(viewModel.Back_IsEnabled, "Back should be disabled on single page");
        Assert.IsFalse(viewModel.Next_IsEnabled, "Next should be disabled on single page");
        Assert.IsTrue(viewModel.Submit_IsEnabled, "Submit should be enabled on single page");
    }

    [TestMethod]
    public void BackCommand_DecreasesPage()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);
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
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Act
        viewModel.NextCommand.Execute(null);

        // Assert
        Assert.AreEqual(2, viewModel.Page);
    }

    [TestMethod]
    public void NextCommand_ValidatesBeforePageChange()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);
        var validationCalled = false;
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Add a way to track validation
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasErrors")
                validationCalled = true;
        };

        // Act
        viewModel.NextCommand.Execute(null);

        // Assert
        Assert.IsTrue(validationCalled, "Validation should be called before page change");
    }

    [TestMethod]
    public void BackCommand_ValidatesBeforePageChange()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);
        var validationCalled = false;
        viewModel.Pages = 3;
        viewModel.Page = 2;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        // Add a way to track validation
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasErrors")
                validationCalled = true;
        };

        // Act
        viewModel.BackCommand.Execute(null);

        // Assert
        Assert.IsTrue(validationCalled, "Validation should be called before page change");
    }

    [TestMethod]
    public void Cleanup_ClearsNavigationCommands()
    {
        // Arrange
        var viewModel = new TestDialogWizardViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogWizardViewModel>>().Object);

        // Act
        viewModel.Dispose();

        // Assert
        Assert.IsTrue(viewModel.IsDisposed);
    }
}

using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelWizardTests
{
    private Mock<ICommonServices> _mockCommonServices;

    public ViewModelWizardTests()
    {
        _mockCommonServices = new Mock<ICommonServices>();
        var mockMessengerService = new Mock<IMessengerService>();
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(mockMessengerService.Object);
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestWizardViewModel : ViewModelBladeWizard<TestModel>
    {
        public TestWizardViewModel(ICommonServices commonServices, ILogger<TestWizardViewModel> logger)
            : base(commonServices, logger) { }
    }

    [TestMethod]
    public void Wizard_InitialState_FirstPageActive_BackDisabled_NextEnabled()
    {
        var viewModel = new TestWizardViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestWizardViewModel>>().Object);
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        Assert.AreEqual(1, viewModel.Page);
        Assert.IsFalse(viewModel.Back_IsEnabled);
        Assert.IsTrue(viewModel.Next_IsEnabled);
        Assert.IsFalse(viewModel.Submit_IsEnabled);
    }

    [TestMethod]
    public void Wizard_Next_AdvancesToNextPage_UpdatesIsEnabled()
    {
        var viewModel = new TestWizardViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestWizardViewModel>>().Object);
        viewModel.Pages = 3;
        viewModel.Page = 1;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        viewModel.NextCommand.Execute(null);

        Assert.AreEqual(2, viewModel.Page);
        Assert.IsTrue(viewModel.Back_IsEnabled);
        Assert.IsTrue(viewModel.Next_IsEnabled);
        Assert.IsFalse(viewModel.Submit_IsEnabled);
    }

    [TestMethod]
    public void Wizard_Back_ReturnsToPreviousPage_UpdatesIsEnabled()
    {
        var viewModel = new TestWizardViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestWizardViewModel>>().Object);
        viewModel.Pages = 3;
        viewModel.Page = 2;
        viewModel.RaisePropertyChanged(nameof(viewModel.Page));

        viewModel.BackCommand.Execute(null);

        Assert.AreEqual(1, viewModel.Page);
        Assert.IsFalse(viewModel.Back_IsEnabled);
        Assert.IsTrue(viewModel.Next_IsEnabled);
        Assert.IsFalse(viewModel.Submit_IsEnabled);
    }
}


using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectableTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;

    public ViewModelSelectableTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestObservableModel : ObservableClass
    {
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }

    public class TestSelectableViewModel : ViewModelBlade<TestModel>
    {
        public TestSelectableViewModel(ICommonServices commonServices, ILogger<TestSelectableViewModel> logger)
            : base(commonServices, logger) { }
    }

    public class TestObservableSelectableViewModel : ViewModelBlade<TestObservableModel>
    {
        public int PropertyChangedCallCount { get; private set; }

        public TestObservableSelectableViewModel(ICommonServices commonServices, ILogger<TestObservableSelectableViewModel> logger)
            : base(commonServices, logger)
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedItem))
                    PropertyChangedCallCount++;
            };
        }
    }

    [TestMethod]
    public async Task OnSubmitted_WithValidModel_RaisesSubmittedEvent()
    {
        var viewModel = new TestSelectableViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestSelectableViewModel>>().Object);
        var model = new TestModel { Name = "Test" };
        var eventInvoked = false;
        viewModel.Submitted += (s, e) => eventInvoked = true;

        await viewModel.SubmitAsync(model);

        Assert.IsTrue(eventInvoked);
    }

    [TestMethod]
    public void SelectedItem_PropertyChanged_PropagatesFromItem()
    {
        var viewModel = new TestObservableSelectableViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestObservableSelectableViewModel>>().Object);
        var model = new TestObservableModel { Name = "Initial" };
        viewModel.SetSelectedItem(model);
        var initialCount = viewModel.PropertyChangedCallCount;

        model.Name = "Changed";

        Assert.IsTrue(viewModel.PropertyChangedCallCount > initialCount,
            "PropertyChanged should be raised when nested property changes");
    }

    [TestMethod]
    public void SelectedItem_Replaced_UnsubscribesFromOldItem()
    {
        var viewModel = new TestObservableSelectableViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestObservableSelectableViewModel>>().Object);
        var modelA = new TestObservableModel { Name = "A" };
        var modelB = new TestObservableModel { Name = "B" };

        viewModel.SetSelectedItem(modelA);
        viewModel.SetSelectedItem(modelB);
        var countAfterReplacement = viewModel.PropertyChangedCallCount;

        modelA.Name = "ChangedA";
        var countAfterOldChange = viewModel.PropertyChangedCallCount;

        modelB.Name = "ChangedB";
        var countAfterNewChange = viewModel.PropertyChangedCallCount;

        Assert.AreEqual(countAfterReplacement, countAfterOldChange,
            "Changing old SelectedItem should not raise PropertyChanged");
        Assert.IsTrue(countAfterNewChange > countAfterOldChange,
            "Changing new SelectedItem should raise PropertyChanged");
    }

    [TestMethod]
    public void SelectedItem_SetToNull_DoesNotThrow()
    {
        var viewModel = new TestSelectableViewModel(
            _mockCommonServices.Object,
            new Mock<ILogger<TestSelectableViewModel>>().Object);
        var model = new TestModel { Name = "Test" };
        viewModel.SetSelectedItem(model);

        viewModel.SelectedItem = null;

        Assert.IsNull(viewModel.SelectedItem);
    }
}

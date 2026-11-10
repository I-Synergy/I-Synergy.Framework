using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

/// <summary>
/// Tests for <c>BaseMultipleSelectionViewModel&lt;TModel&gt;</c>: the cached object projection of the
/// selection, the selection setter and the selection validator.
/// </summary>
[TestClass]
public class BaseMultipleSelectionViewModelTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<ILogger<ViewModelMultipleSelectionDialog<TestEntity>>> _mockLogger;

    public BaseMultipleSelectionViewModelTests()
    {
        _mockLogger = new Mock<ILogger<ViewModelMultipleSelectionDialog<TestEntity>>>();

        _mockLanguageService = new Mock<ILanguageService>();
        _mockLanguageService.Setup(x => x.GetString("WarningSelectItem")).Returns("Please select an item");
        _mockLanguageService.Setup(x => x.GetString("Selection")).Returns("Selection");

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.LanguageService).Returns(_mockLanguageService.Object);
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(new Mock<IMessengerService>().Object);
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }

    private ViewModelMultipleSelectionDialog<TestEntity> CreateViewModel() =>
        new(_mockCommonServices.Object, _mockLogger.Object);

    [TestMethod]
    public void SelectedItemProjection_ReturnsObjectsAndCachesPerSourceList()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var first = new TestEntity { Id = 1, Name = "First" };
        var second = new TestEntity { Id = 2, Name = "Second" };
        viewModel.SetSelectedItem(new List<object> { first, second });
        IMultipleSelectionViewModel selection = viewModel;

        // Act
        var projection = selection.SelectedItem;
        var projectionAgain = selection.SelectedItem;

        // Assert
        Assert.IsNotNull(projection);
        Assert.HasCount(2, projection);
        Assert.AreSame(first, projection[0]);
        Assert.AreSame(second, projection[1]);
        Assert.AreSame(projection, projectionAgain, "The projection should be cached while the source list is unchanged");
    }

    [TestMethod]
    public void SelectedItemProjection_IsRecomputedWhenSelectionIsReplaced()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var first = new TestEntity { Id = 1, Name = "First" };
        viewModel.SetSelectedItem(new List<object> { first });
        IMultipleSelectionViewModel selection = viewModel;
        var initialProjection = selection.SelectedItem;

        // Act
        var second = new TestEntity { Id = 2, Name = "Second" };
        viewModel.SetSelectedItem(new List<object> { second });
        var updatedProjection = selection.SelectedItem;

        // Assert
        Assert.IsNotNull(initialProjection);
        Assert.IsNotNull(updatedProjection);
        Assert.AreNotSame(initialProjection, updatedProjection);
        Assert.HasCount(1, updatedProjection);
        Assert.AreSame(second, updatedProjection[0]);
    }

    [TestMethod]
    public void SelectedItemProjection_IsNullAfterCleanup()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.SetSelectedItem(new List<object> { new TestEntity { Id = 1, Name = "First" } });
        IMultipleSelectionViewModel selection = viewModel;

        // Act
        viewModel.Cleanup();

        // Assert
        Assert.IsNull(viewModel.SelectedItem);
        Assert.IsNull(selection.SelectedItem);
    }

    [TestMethod]
    public void SetSelectedItem_WithNullItems_ResultsInEmptySelection()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.SetSelectedItem(new List<object> { new TestEntity { Id = 1, Name = "First" } });

        // Act
        viewModel.SetSelectedItem(null);

        // Assert
        Assert.IsNotNull(viewModel.SelectedItem);
        Assert.IsEmpty(viewModel.SelectedItem);
        Assert.IsFalse(viewModel.SelectCommand.CanExecute(null));
    }

    [TestMethod]
    public void Validate_WithoutSelection_AddsSelectionWarning()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        var isValid = viewModel.Validate();

        // Assert
        Assert.IsFalse(isValid);
        Assert.IsNotNull(viewModel.Errors);
        Assert.Contains(e => e.Key == nameof(viewModel.SelectedItem) && e.Value == "Please select an item", viewModel.Errors);
    }

    [TestMethod]
    public void Validate_WithSelection_IsValidAndHasNoErrors()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.SetSelectedItem(new List<object> { new TestEntity { Id = 1, Name = "First" } });

        // Act
        var isValid = viewModel.Validate();

        // Assert
        Assert.IsTrue(isValid);
        Assert.IsEmpty(viewModel.Errors ?? new System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string, string>>());
    }
}

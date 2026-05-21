using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionDialogTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<ILogger<ViewModelSelectionDialog<TestEntity>>> _mockLogger;

    public ViewModelSelectionDialogTests()
    {
        _mockLogger = new Mock<ILogger<ViewModelSelectionDialog<TestEntity>>>();

        _mockLanguageService = new Mock<ILanguageService>();
        _mockLanguageService.Setup(x => x.GetString("WarningSelectItem")).Returns("Please select an item");

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.LanguageService).Returns(_mockLanguageService.Object);
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }

    [TestMethod]
    public void Constructor_InitializesPropertiesWithItems()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };
        var selectedItems = new List<TestEntity> { items[0] };

        // Act
        var viewModel = new ViewModelSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(selectedItems);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(2, viewModel.Items.Count);
        Assert.IsNotNull(viewModel.SelectedItems);
        Assert.AreEqual(1, viewModel.SelectedItems.Count);
        Assert.AreEqual(SelectionModes.Single, viewModel.SelectionMode);
        Assert.IsNotNull(viewModel.RefreshCommand);
    }

    [TestMethod]
    public async Task QueryItemsAsync_WithEmptyQuery_ShowsAllItems()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };

        var viewModel = new ViewModelSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("");

        // Assert
        Assert.AreEqual(2, viewModel.Items?.Count);
    }

    [TestMethod]
    public async Task QueryItemsAsync_WithSearchText_FiltersItems()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Different" }
            };

        var viewModel = new ViewModelSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("Test");

        // Assert
        Assert.AreEqual(1, viewModel.Items?.Count);
        Assert.AreEqual("Test1", viewModel.Items?.FirstOrDefault()?.Name);
    }

    [TestMethod]
    public void Constructor_WithNullItems_InitializesEmptyCollections()
    {
        // Arrange & Act
        var viewModel = new ViewModelSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(null!);
        viewModel.SetSelectedItems(null!);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(0, viewModel.Items.Count);
        Assert.IsNotNull(viewModel.SelectedItems);
        Assert.AreEqual(0, viewModel.SelectedItems.Count);
    }

    [TestMethod]
    public void Validator_EnforcesSelectionRules()
    {
        // Arrange
        var viewModel = new ViewModelSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(new List<TestEntity>());
        viewModel.SetSelectedItems(new List<TestEntity>());

        // Act
        bool isValid = viewModel.Validate();

        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(viewModel.HasErrors);
    }
}


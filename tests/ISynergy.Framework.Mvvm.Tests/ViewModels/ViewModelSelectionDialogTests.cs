using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionDialogTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;
    private Mock<ILanguageService> _mockLanguageService;

    [TestInitialize]
    public void Setup()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockLogger = new Mock<ILogger>();
        _mockLanguageService = new Mock<ILanguageService>();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
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
        var viewModel = new ViewModelSelectionDialog<TestEntity>(
            _mockScopedContextService.Object,
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            selectedItems,
            SelectionModes.Single);

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
        var viewModel = new ViewModelSelectionDialog<TestEntity>(
            _mockScopedContextService.Object,
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            new List<TestEntity>(),
            SelectionModes.Single);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("");

        // Assert
        Assert.AreEqual(2, viewModel.Items.Count);
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
        var viewModel = new ViewModelSelectionDialog<TestEntity>(
            _mockScopedContextService.Object,
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            new List<TestEntity>(),
            SelectionModes.Single);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("Test");

        // Assert
        Assert.AreEqual(1, viewModel.Items.Count);
        Assert.AreEqual("Test1", viewModel.Items[0].Name);
    }

    [TestMethod]
    public void Constructor_WithNullItems_InitializesEmptyCollections()
    {
        // Arrange & Act
        var viewModel = new ViewModelSelectionDialog<TestEntity>(
            _mockScopedContextService.Object,
            _mockCommonServices.Object,
            _mockLogger.Object,
            null,
            null,
            SelectionModes.Single);

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
        var viewModel = new ViewModelSelectionDialog<TestEntity>(
            _mockScopedContextService.Object,
            _mockCommonServices.Object,
            _mockLogger.Object,
            new List<TestEntity>(),
            new List<TestEntity>(),
            SelectionModes.Single);

        _mockLanguageService.Setup(x => x.GetString("WarningSelectItem")).Returns("Please select an item");

        // Act
        bool isValid = viewModel.Validate();

        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(viewModel.HasErrors);
    }
}

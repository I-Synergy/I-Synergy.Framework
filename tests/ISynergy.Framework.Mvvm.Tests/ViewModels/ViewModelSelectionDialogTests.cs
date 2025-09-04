using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionDialogTests
{
    private ICommonServices _commonServices;

    public ViewModelSelectionDialogTests()
    {
        var mockLogger = new Mock<ILogger<ViewModelSelectionDialog<TestEntity>>>();

        var mockLanguageService = new Mock<ILanguageService>();
        mockLanguageService.Setup(x => x.GetString("WarningSelectItem")).Returns("Please select an item");

        var mockScopedContextService = new Mock<IScopedContextService>();

        var mockCommonServices = new Mock<ICommonServices>();
        mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(mockScopedContextService.Object);
        mockCommonServices.SetupGet(s => s.LanguageService).Returns(mockLanguageService.Object);

        // Configure the mock to return a new instance when GetRequiredService is called
        mockScopedContextService
            .Setup(s => s.GetRequiredService<ViewModelSelectionDialog<TestEntity>>())
            .Returns(() =>
            {
                // Create a new instance of ViewModelSelectionBlade with required dependencies
                var viewModel = new ViewModelSelectionDialog<TestEntity>(
                    mockCommonServices.Object,
                    mockLogger.Object);
                return viewModel;
            });

        _commonServices = mockCommonServices.Object;
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
        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestEntity>>();
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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

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
        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestEntity>>();
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
        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestEntity>>();
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

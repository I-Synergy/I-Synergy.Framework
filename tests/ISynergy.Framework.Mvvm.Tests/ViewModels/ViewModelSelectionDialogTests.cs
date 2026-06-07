using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionDialogTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<ILogger<ViewModelSingleSelectionDialog<TestEntity>>> _mockLogger;

    public ViewModelSelectionDialogTests()
    {
        _mockLogger = new Mock<ILogger<ViewModelSingleSelectionDialog<TestEntity>>>();

        _mockLanguageService = new Mock<ILanguageService>();
        _mockLanguageService.Setup(x => x.GetString("WarningSelectItem")).Returns("Please select an item");

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.LanguageService).Returns(_mockLanguageService.Object);
        var mockMessengerService = new Mock<IMessengerService>();
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(mockMessengerService.Object);
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
        var selectedItem = items[0];

        // Act
        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(selectedItem);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(2, viewModel.Items.Count);
        Assert.IsNotNull(viewModel.SelectedItem);
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

        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(items);

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

        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(items);

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
        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(null!);
        viewModel.SetSelectedItem(null!);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(0, viewModel.Items.Count);
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public void Validator_EnforcesSelectionRules()
    {
        // Arrange
        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(new List<TestEntity>());

        // Act
        bool isValid = viewModel.Validate();

        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(viewModel.HasErrors);
    }
}


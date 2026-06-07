using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionBladeTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<ILogger<ViewModelSingleSelectionBlade<TestEntity>>> _mockSingleLogger;
    private Mock<ILogger<ViewModelMultipleSelectionBlade<TestEntity>>> _mockMultipleLogger;

    public ViewModelSelectionBladeTests()
    {
        _mockSingleLogger = new Mock<ILogger<ViewModelSingleSelectionBlade<TestEntity>>>();
        _mockMultipleLogger = new Mock<ILogger<ViewModelMultipleSelectionBlade<TestEntity>>>();

        _mockLanguageService = new Mock<ILanguageService>();

        // Setup the language service to return the key wrapped in brackets
        _mockLanguageService
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"[{key}]");

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
    public void Cleanup_ClearsCollectionsAndCommands()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };
        var selectedItem = items[0];

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(selectedItem);

        // Verify initial state
        Assert.IsNotNull(viewModel.Items);
        Assert.IsTrue(viewModel.Items.Any());
        Assert.IsNotNull(viewModel.SelectedItem);
        Assert.AreEqual(selectedItem, viewModel.SelectedItem);

        // Act
        viewModel.Cleanup();

        // Assert
        Assert.IsNull(viewModel.SelectedItem, "SelectedItem should be null after cleanup");
        Assert.IsNull(viewModel.Items, "Items should not be null after cleanup");

        viewModel.Dispose();
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public void Constructor_InitializesCollectionsAndProperties()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };
        var selectedItem = items[0];

        // Act
        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(selectedItem);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(2, viewModel.Items.Count);
        Assert.IsNotNull(viewModel.SelectedItem);
        Assert.AreEqual(selectedItem, viewModel.SelectedItem);
        Assert.IsNotNull(viewModel.RefreshCommand);
        Assert.AreEqual("[Selection]", viewModel.Title);
    }

    [TestMethod]
    public async Task CleanupAsync_CompletesCleanly()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(items[0]);

        // Act & Assert
        // Shouldn't throw any exceptions
        await viewModel.CloseAsync();

        // Assert
        Assert.IsNull(viewModel.SelectedItem, "SelectedItem should be null after cleanup");
        Assert.IsNull(viewModel.Items, "Items should be null after cleanup");

        viewModel.Dispose();
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public async Task QueryItemsAsync_FiltersBySearchText()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Different" }
            };

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
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
        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(null!);
        viewModel.SetSelectedItem(null!);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(0, viewModel.Items.Count);
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidSelection_InvokesSubmittedAndCloses()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(items[0]);

        var submittedInvoked = false;
        var closedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.SubmitAsync(items[0]);

        // Assert
        Assert.IsTrue(submittedInvoked);
        Assert.IsTrue(closedInvoked);
    }

    [TestMethod]
    public void Validator_SingleMode_RequiresOneSelection()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);

        // Act
        bool isValid = viewModel.Validate();

        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(viewModel.HasErrors);
    }

    [TestMethod]
    public void Validator_MultipleMode_RequiresAtLeastOneSelection()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);

        // Act
        bool isValid = viewModel.Validate();

        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(viewModel.HasErrors);
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

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("");

        // Assert
        Assert.AreEqual(2, viewModel.Items?.Count);
    }

    [TestMethod]
    public async Task QueryItemsAsync_WithAsterisk_ShowsAllItems()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };

        var viewModel = new ViewModelSingleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockSingleLogger.Object);
        viewModel.SetItems(items);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("*");

        // Assert
        Assert.AreEqual(2, viewModel.Items?.Count);
    }

    [TestMethod]
    public void SetSelectedItems_WithValidItems_UpdatesSelection()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };

        var viewModel = new ViewModelMultipleSelectionBlade<TestEntity>(_mockCommonServices.Object, _mockMultipleLogger.Object);
        viewModel.SetItems(items);

        // Act
        var selectedItems = new List<TestEntity> { items[0], items[1] };
        viewModel.SetSelectedItems(selectedItems);

        // Assert
        Assert.AreEqual(2, viewModel.SelectedItems?.Count);
        Assert.IsTrue(viewModel.SelectedItems?.Contains(items[0]));
        Assert.IsTrue(viewModel.SelectedItems?.Contains(items[1]));
    }
}


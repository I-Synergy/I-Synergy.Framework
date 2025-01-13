using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionBladeTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);
        _mockLogger = new Mock<ILogger>();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
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
        var selectedItems = new List<TestEntity> { items[0] };
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            selectedItems,
            SelectionModes.Single);

        // Verify initial state
        Assert.IsNotNull(viewModel.Items);
        Assert.IsTrue(viewModel.Items.Any());
        Assert.IsNotNull(viewModel.SelectedItems);
        Assert.IsTrue(viewModel.SelectedItems.Any());
        Assert.IsNotNull(viewModel.RefreshCommand);
        Assert.IsNotNull(viewModel.SubmitCommand);

        // Act
        viewModel.Cleanup();

        // Assert
        Assert.IsNotNull(viewModel.SelectedItems, "SelectedItems should not be null after cleanup");
        Assert.AreEqual(0, viewModel.SelectedItems.Count, "SelectedItems should be empty after cleanup");
        Assert.AreEqual(0, viewModel.Items.Count, "Items should be empty after cleanup");

        viewModel.Dispose();
        Assert.IsNull(viewModel.RefreshCommand, "RefreshCommand should be null after dispose");
        Assert.IsNull(viewModel.SubmitCommand, "SubmitCommand should be null after dispose");
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
        var selectedItems = new List<TestEntity> { items[0] };

        // Act
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            items,
            SelectionModes.Single);

        // Act & Assert
        // Shouldn't throw any exceptions
        await viewModel.CloseAsync();

        // Assert
        Assert.IsNotNull(viewModel.SelectedItems, "SelectedItems should not be null after cleanup");
        Assert.AreEqual(0, viewModel.SelectedItems.Count, "SelectedItems should be empty after cleanup");
        Assert.AreEqual(0, viewModel.Items.Count, "Items should be empty after cleanup");

        viewModel.Dispose();
        Assert.IsNull(viewModel.RefreshCommand, "RefreshCommand should be null after dispose");
        Assert.IsNull(viewModel.SubmitCommand, "SubmitCommand should be null after dispose");
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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
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
    public async Task SubmitAsync_WithValidSelection_InvokesSubmittedAndCloses()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            items,
            SelectionModes.Single);

        var submittedInvoked = false;
        var closedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.SubmitAsync(items);

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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            new List<TestEntity>(),
            SelectionModes.Single);

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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            new List<TestEntity>(),
            SelectionModes.Multiple);

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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
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
    public async Task QueryItemsAsync_WithAsterisk_ShowsAllItems()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            };
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            new List<TestEntity>(),
            SelectionModes.Single);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("*");

        // Assert
        Assert.AreEqual(2, viewModel.Items.Count);
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
        var viewModel = new ViewModelSelectionBlade<TestEntity>(
            _mockCommonServices.Object,
            _mockLogger.Object,
            items,
            new List<TestEntity>(),
            SelectionModes.Multiple);

        // Act
        viewModel.SelectedItems = new List<object> { items[0], items[1] };

        // Assert
        Assert.AreEqual(2, viewModel.SelectedItems.Count);
        Assert.IsTrue(viewModel.SelectedItems.Contains(items[0]));
        Assert.IsTrue(viewModel.SelectedItems.Contains(items[1]));
    }
}

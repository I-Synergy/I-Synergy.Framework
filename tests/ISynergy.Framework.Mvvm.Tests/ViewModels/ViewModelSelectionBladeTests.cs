using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionBladeTests
{
    private ICommonServices _commonServices;

    public ViewModelSelectionBladeTests()
    {
        var mockLogger = new Mock<ILogger<ViewModelSelectionBlade<TestEntity>>>();

        var mockScopedContextService = new Mock<IScopedContextService>();
        var mockLanguageService = new Mock<ILanguageService>();

        // Setup the language service to return the key wrapped in brackets
        mockLanguageService
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"[{key}]");

        var mockCommonServices = new Mock<ICommonServices>();
        mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(mockScopedContextService.Object);
        mockCommonServices.SetupGet(s => s.LanguageService).Returns(mockLanguageService.Object);

        // Configure the mock to return a new instance when GetRequiredService is called
        mockScopedContextService
            .Setup(s => s.GetRequiredService<ViewModelSelectionBlade<TestEntity>>())
            .Returns(() =>
            {
                // Create a new instance of ViewModelSelectionBlade with required dependencies
                var viewModel = new ViewModelSelectionBlade<TestEntity>(
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
    public void Cleanup_ClearsCollectionsAndCommands()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };
        var selectedItems = new List<TestEntity> { items[0] };

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(selectedItems);

        // Verify initial state
        Assert.IsNotNull(viewModel.Items);
        Assert.IsTrue(viewModel.Items.Any());
        Assert.IsNotNull(viewModel.SelectedItems);
        Assert.IsTrue(viewModel.SelectedItems.Any());

        // Act
        viewModel.Cleanup();

        // Assert
        Assert.IsNotNull(viewModel.SelectedItems, "SelectedItems should not be null after cleanup");
        Assert.AreEqual(0, viewModel.SelectedItems.Count, "SelectedItems should be empty after cleanup");
        Assert.AreEqual(0, viewModel.Items.Count, "Items should be empty after cleanup");

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
        var selectedItems = new List<TestEntity> { items[0] };

        // Act
        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(items);

        // Act & Assert
        // Shouldn't throw any exceptions
        await viewModel.CloseAsync();

        // Assert
        Assert.IsNotNull(viewModel.SelectedItems, "SelectedItems should not be null after cleanup");
        Assert.AreEqual(0, viewModel.SelectedItems.Count, "SelectedItems should be empty after cleanup");
        Assert.AreEqual(0, viewModel.Items.Count, "Items should be empty after cleanup");

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
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
        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
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
    public async Task SubmitAsync_WithValidSelection_InvokesSubmittedAndCloses()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" }
            };

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(items);

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

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

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionBlade<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Multiple);
        viewModel.SetItems(items);

        // Act
        var selectedItems = new List<TestEntity> { items[0], items[1] };
        viewModel.SetSelectedItems(selectedItems);

        // Assert
        Assert.AreEqual(2, viewModel.SelectedItems.Count);
        Assert.IsTrue(viewModel.SelectedItems.Contains(items[0]));
        Assert.IsTrue(viewModel.SelectedItems.Contains(items[1]));
    }
}

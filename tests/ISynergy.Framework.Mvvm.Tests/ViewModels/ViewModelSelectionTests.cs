using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionTests
{
    private ICommonServices _commonServices;

    public ViewModelSelectionTests()
    {
        var mockLogger = new Mock<ILogger<ViewModelSelectionDialog<TestEntity>>>();

        var mockScopedContextService = new Mock<IScopedContextService>();

        var mockCommonServices = new Mock<ICommonServices>();
        mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(mockScopedContextService.Object);

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

        public override string ToString()
        {
            return Name;
        }
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
    }

    [TestMethod]
    public async Task QueryItemsAsync_FiltersBySearchText()
    {
        // Arrange
        var items = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" },
                new TestEntity { Id = 3, Name = "Other" }
            };

        var viewModel = _commonServices.ScopedContextService.GetRequiredService<ViewModelSelectionDialog<TestEntity>>();
        viewModel.SetSelectionMode(SelectionModes.Single);
        viewModel.SetItems(items);
        viewModel.SetSelectedItems(new List<TestEntity>());

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("Test");

        // Assert
        Assert.AreEqual(2, viewModel.Items.Count);
        Assert.IsTrue(viewModel.Items.All(i => i.Name.Contains("Test")));
    }
}

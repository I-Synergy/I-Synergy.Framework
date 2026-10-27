using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSelectionTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger<ViewModelSingleSelectionDialog<TestEntity>>> _mockLogger;

    public ViewModelSelectionTests()
    {
        _mockLogger = new Mock<ILogger<ViewModelSingleSelectionDialog<TestEntity>>>();

        _mockCommonServices = new Mock<ICommonServices>();
        var mockMessengerService = new Mock<IMessengerService>();
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(mockMessengerService.Object);
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
        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(selectedItems[0]);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.AreEqual(2, viewModel.Items.Count);
        Assert.IsNotNull(viewModel.SelectedItem);
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

        var viewModel = new ViewModelSingleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetItems(items);
        viewModel.SetSelectedItem(null!);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync("Test");

        // Assert
        Assert.AreEqual(2, viewModel.Items?.Count);
        Assert.IsTrue(viewModel.Items?.All(i => i.Name.Contains("Test")));
    }
}


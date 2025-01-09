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
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockLogger = new Mock<ILogger>();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

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
        Assert.AreEqual(2, viewModel.Items.Count);
        Assert.IsTrue(viewModel.Items.All(i => i.Name.Contains("Test")));
    }
}

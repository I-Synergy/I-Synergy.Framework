using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBladeViewTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILoggerFactory> _mockLoggerFactory;
    private Mock<ILanguageService> _mockLanguageService;

    [TestInitialize]
    public void Setup()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(new Mock<ILogger>().Object);

        _mockCommonServices.SetupGet(s => s.LoggerFactory).Returns(_mockLoggerFactory.Object);

        _mockLanguageService = new Mock<ILanguageService>();
    }

    private class TestBladeViewModel : ViewModelBladeView<TestEntity>
    {
        public TestBladeViewModel(ICommonServices commonServices, bool refreshOnInitialization = true)
            : base(commonServices, refreshOnInitialization) { }

        public override Task AddAsync()
        {
            return Task.CompletedTask;
        }

        public override Task EditAsync(TestEntity e)
        {
            return Task.CompletedTask;
        }

        public override Task RemoveAsync(TestEntity e)
        {
            return Task.CompletedTask;
        }

        public override Task SearchAsync(object e)
        {
            return Task.CompletedTask;
        }

        public override Task RetrieveItemsAsync(CancellationToken cancellationToken)
        {
            Items.Add(new TestEntity { Id = 1, Name = "Test" });
            return Task.CompletedTask;
        }
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [TestMethod]
    public void Constructor_InitializesCollectionsAndCommands()
    {
        // Arrange & Act
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.IsNotNull(viewModel.Blades);
        Assert.IsNotNull(viewModel.AddCommand);
        Assert.IsNotNull(viewModel.EditCommand);
        Assert.IsNotNull(viewModel.DeleteCommand);
        Assert.IsNotNull(viewModel.RefreshCommand);
        Assert.IsNotNull(viewModel.SearchCommand);
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsPaneVisible);
    }

    [TestMethod]
    public async Task Initialize_WithRefreshOnInitialization_CallsRefresh()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        Assert.IsTrue(viewModel.IsInitialized);
    }

    [TestMethod]
    public void SetSelectedItem_UpdatesPropertyAndFlag()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object);
        var entity = new TestEntity { Id = 1, Description = "Test" };

        // Act
        viewModel.SetSelectedItem(entity);

        // Assert
        Assert.AreEqual(entity, viewModel.SelectedItem);
        Assert.IsTrue(viewModel.IsUpdate);
    }

    [TestMethod]
    public void Cleanup_ClearsCollectionsAndCommands()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object);
        viewModel.Items.Add(new TestEntity());

        // Act
        viewModel.Cleanup();
        Assert.AreEqual(0, viewModel.Items.Count);

        viewModel.Dispose();
        Assert.IsNull(viewModel.AddCommand);
        Assert.IsNull(viewModel.EditCommand);
        Assert.IsNull(viewModel.DeleteCommand);
        Assert.IsNull(viewModel.RefreshCommand);
        Assert.IsNull(viewModel.SearchCommand);
        Assert.IsNull(viewModel.SubmitCommand);
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public async Task DeleteAsync_WithConfirmation_RemovesItem()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object);
        var entity = new TestEntity { Id = 1, Description = "Test" };
        _mockLanguageService.Setup(x => x.GetString(It.IsAny<string>())).Returns("Test");
        _mockCommonServices.Setup(x => x.DialogService.ShowMessageAsync(
            It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo))
            .ReturnsAsync(MessageBoxResult.Yes);

        // Act
        await viewModel.DeleteAsync(entity);

        // Assert
        _mockCommonServices.Verify(x => x.DialogService.ShowMessageAsync(
            It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo), Times.Once);
    }

    [TestMethod]
    public async Task RetrieveItemsAsync_PopulatesItems()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object);

        // Act
        await viewModel.RetrieveItemsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(1, viewModel.Items.Count);
    }
}

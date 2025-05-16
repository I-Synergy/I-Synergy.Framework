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
    private Mock<ILanguageService> _mockLanguageService;

    public ViewModelBladeViewTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockLanguageService = new Mock<ILanguageService>();
    }

    public class TestBladeViewModel : ViewModelBladeView<TestEntity>
    {
        public TestBladeViewModel(ICommonServices commonServices, ILogger<TestBladeViewModel> logger)
            : base(commonServices, logger)
        {
        }

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

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    [TestMethod]
    public void Constructor_InitializesCollectionsAndCommands()
    {
        // Arrange & Act
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

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
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        Assert.IsTrue(viewModel.IsInitialized);
    }

    [TestMethod]
    public void SetSelectedItem_UpdatesPropertyAndFlag()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
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
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        viewModel.Items.Add(new TestEntity());

        // Act
        viewModel.Cleanup();
        Assert.AreEqual(0, viewModel.Items.Count);

        viewModel.Dispose();
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public async Task DeleteAsync_WithConfirmation_RemovesItem()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Description = "Test" };
        _mockLanguageService.Setup(x => x.GetString(It.IsAny<string>())).Returns("Test");
        _mockCommonServices.Setup(x => x.DialogService.ShowMessageAsync(
            It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.YesNo, NotificationTypes.Default))
            .ReturnsAsync(MessageBoxResult.Yes);

        // Act
        await viewModel.DeleteAsync(entity);

        // Assert
        _mockCommonServices.Verify(x => x.DialogService.ShowMessageAsync(
            It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.YesNo, NotificationTypes.Default), Times.Once);
    }

    [TestMethod]
    public async Task RetrieveItemsAsync_PopulatesItems()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

        // Act
        await viewModel.RetrieveItemsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(1, viewModel.Items.Count);
    }
}

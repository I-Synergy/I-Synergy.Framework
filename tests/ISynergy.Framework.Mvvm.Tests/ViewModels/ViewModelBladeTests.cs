using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBladeTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;

    public ViewModelBladeTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);
    }

    public class TestBladeViewModel : ViewModelBlade<TestEntity>
    {
        public TestBladeViewModel(ICommonServices commonServices, ILogger<TestBladeViewModel> logger)
            : base(commonServices, logger) { }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

        // Assert
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsUpdate);
        Assert.IsNull(viewModel.Owner);
        Assert.IsFalse(viewModel.IsDisabled);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedEvent()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var eventInvoked = false;
        viewModel.Submitted += (s, e) => eventInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(eventInvoked);
    }

    [TestMethod]
    public void SetSelectedItem_UpdatesPropertyAndFlag()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };

        // Act
        viewModel.SetSelectedItem(entity);

        // Assert
        Assert.AreEqual(entity, viewModel.SelectedItem);
    }

    [TestMethod]
    public void SetOwner_UpdatesOwnerProperty()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        var mockOwner = new Mock<IViewModelBladeView>();

        // Act
        viewModel.Owner = mockOwner.Object;

        // Assert
        Assert.AreEqual(mockOwner.Object, viewModel.Owner);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndCloses()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var submittedInvoked = false;
        var closedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(submittedInvoked);
        Assert.IsTrue(closedInvoked);
    }

    [TestMethod]
    public void Cleanup_ClearsSelectedItemAndCommands()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        viewModel.SetSelectedItem(new TestEntity());

        viewModel.Cleanup();
        Assert.AreEqual(default, viewModel.SelectedItem);

        viewModel.Dispose();
        Assert.IsTrue(viewModel.IsDisposed);
    }
}

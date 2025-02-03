using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelDialogTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILoggerFactory> _mockLoggerFactory;

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
    }

    private class TestDialogViewModel : ViewModelDialog<TestEntity>
    {
        public TestDialogViewModel(ICommonServices commonServices, bool automaticValidation = false)
            : base(commonServices, automaticValidation) { }
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object);

        // Assert
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsUpdate);
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndClosedEvents()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object);
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
    public void ApplyQueryAttributes_SetsSelectedItem()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var query = new Dictionary<string, object>
            {
                { GenericConstants.Parameter, entity }
            };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.AreEqual(entity, viewModel.SelectedItem);
        Assert.IsTrue(viewModel.IsUpdate);
    }

    [TestMethod]
    public void ApplyQueryAttributes_WithInvalidParameter_DoesNotSetSelectedItem()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object);
        var query = new Dictionary<string, object>
            {
                { "wrongKey", new TestEntity() }
            };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.IsNull(viewModel.SelectedItem);
        Assert.IsFalse(viewModel.IsUpdate);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndCloses()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object);
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
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object);
        viewModel.SelectedItem = new TestEntity();

        // Act
        viewModel.Cleanup();
        Assert.IsNull(viewModel.SelectedItem);

        viewModel.Dispose();
        Assert.IsNull(viewModel.SubmitCommand);
        Assert.IsTrue(viewModel.IsDisposed);
    }
}

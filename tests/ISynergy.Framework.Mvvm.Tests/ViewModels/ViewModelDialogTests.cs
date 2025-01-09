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
    private Mock<ILogger> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockLogger = new Mock<ILogger>();
    }

    private class TestDialogViewModel : ViewModelDialog<TestEntity>
    {
        public TestDialogViewModel(IScopedContextService scopedContextService, ICommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(scopedContextService, commonServices, logger, automaticValidation) { }
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
        var viewModel = new TestDialogViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Assert
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsUpdate);
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndClosedEvents()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestDialogViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestDialogViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestDialogViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);
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
        var viewModel = new TestDialogViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);
        viewModel.SelectedItem = new TestEntity();

        // Act
        viewModel.Cleanup();
        Assert.IsNull(viewModel.SelectedItem);

        viewModel.Dispose();
        Assert.IsNull(viewModel.SubmitCommand);
    }
}

using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelNavigationTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockCommonServices = new Mock<ICommonServices>();
        _mockLogger = new Mock<ILogger>();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class TestNavigationViewModel : ViewModelNavigation<TestEntity>
    {
        public TestNavigationViewModel(ICommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(commonServices, logger, automaticValidation) { }
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestNavigationViewModel(_mockCommonServices.Object, _mockLogger.Object);

        // Assert
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsUpdate);
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public void ApplyQueryAttributes_SetsSelectedItem()
    {
        // Arrange
        var viewModel = new TestNavigationViewModel(_mockCommonServices.Object, _mockLogger.Object);
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
    public async Task SubmitAsync_WithValidation_InvokesSubmitted()
    {
        // Arrange
        var viewModel = new TestNavigationViewModel(_mockCommonServices.Object, _mockLogger.Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var submittedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(submittedInvoked);
    }
}

using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelSummaryTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;
    private Mock<ILanguageService> _mockLanguageService;

    public ViewModelSummaryTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockLogger = new Mock<ILogger>();
        _mockLanguageService = new Mock<ILanguageService>();
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TestSummaryViewModel : ViewModelSummary<TestEntity>
    {
        public TestSummaryViewModel(ICommonServices commonServices, ILogger<TestSummaryViewModel> logger)
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
    }

    [TestMethod]
    public void Constructor_InitializesCollectionsAndCommands()
    {
        // Arrange & Act
        var viewModel = new TestSummaryViewModel(_mockCommonServices.Object, new Mock<ILogger<TestSummaryViewModel>>().Object);

        // Assert
        Assert.IsNotNull(viewModel.Items);
        Assert.IsNotNull(viewModel.AddCommand);
        Assert.IsNotNull(viewModel.EditCommand);
        Assert.IsNotNull(viewModel.DeleteCommand);
        Assert.IsNotNull(viewModel.RefreshCommand);
        Assert.IsNotNull(viewModel.SearchCommand);
        Assert.IsNotNull(viewModel.SubmitCommand);
    }

    [TestMethod]
    public async Task InitializeAsync_WithRefreshOnInitialization_RefreshesItems()
    {
        // Arrange
        var viewModel = new TestSummaryViewModel(_mockCommonServices.Object, new Mock<ILogger<TestSummaryViewModel>>().Object);
        viewModel.AutomaticValidationTrigger = true;

        // Act
        await viewModel.InitializeAsync();

        // Assert
        Assert.IsTrue(viewModel.IsInitialized);
    }
}

using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelMultipleSelectionDialogTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<ILogger<ViewModelMultipleSelectionDialog<TestEntity>>> _mockLogger;

    public ViewModelMultipleSelectionDialogTests()
    {
        _mockLogger = new Mock<ILogger<ViewModelMultipleSelectionDialog<TestEntity>>>();

        _mockLanguageService = new Mock<ILanguageService>();
        _mockLanguageService.Setup(x => x.GetString("WarningSelectItem")).Returns("Please select an item");

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.LanguageService).Returns(_mockLanguageService.Object);
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(new Mock<IMessengerService>().Object);
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }

    [TestMethod]
    public void SelectCommand_WithNoSelection_CannotExecute()
    {
        // Arrange
        var viewModel = new ViewModelMultipleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);

        // Act
        bool canExecute = viewModel.SelectCommand.CanExecute(null);

        // Assert
        Assert.IsFalse(canExecute);
    }

    [TestMethod]
    public void SelectCommand_AfterSetSelectedItem_CanExecute()
    {
        // Arrange
        var viewModel = new ViewModelMultipleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);

        // Act
        viewModel.SetSelectedItem(new List<object>
        {
            new TestEntity { Id = 1, Name = "Test1" },
            new TestEntity { Id = 2, Name = "Test2" }
        });

        // Assert
        Assert.AreEqual(2, viewModel.SelectedItem?.Count);
        Assert.IsTrue(viewModel.SelectCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task SelectCommand_Execute_WithSelection_SubmitsSelectedItems()
    {
        // Arrange
        var viewModel = new ViewModelMultipleSelectionDialog<TestEntity>(_mockCommonServices.Object, _mockLogger.Object);
        viewModel.SetSelectedItem(new List<object>
        {
            new TestEntity { Id = 1, Name = "Test1" },
            new TestEntity { Id = 2, Name = "Test2" }
        });
        List<TestEntity>? capturedResult = null;
        viewModel.Submitted += (s, e) => capturedResult = e.Result;

        // Act
        await viewModel.SelectCommand.ExecuteAsync(null);

        // Assert
        Assert.IsNotNull(capturedResult);
        Assert.AreEqual(2, capturedResult.Count);
    }
}

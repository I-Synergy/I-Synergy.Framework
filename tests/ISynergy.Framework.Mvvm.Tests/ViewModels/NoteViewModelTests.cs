using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class NoteViewModelTests
{
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;
    private Mock<ILogger<NoteViewModel>> _mockLogger;

    public NoteViewModelTests()
    {
        _mockLogger = new Mock<ILogger<NoteViewModel>>();

        _mockLanguageService = new Mock<ILanguageService>();
        _mockLanguageService.Setup(x => x.GetString("Note")).Returns("Note");

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.LanguageService).Returns(_mockLanguageService.Object);
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(new Mock<IMessengerService>().Object);
    }

    [TestMethod]
    public void SubmitCommand_WithEmptyNote_CanExecute()
    {
        // Arrange
        var viewModel = new NoteViewModel(_mockCommonServices.Object, _mockLogger.Object);

        // Act
        bool canExecute = viewModel.SubmitCommand.CanExecute(null);

        // Assert
        Assert.IsTrue(canExecute);
    }

    [TestMethod]
    public async Task SubmitCommand_WithEmptyNote_SubmitsEmptyString()
    {
        // Arrange
        var viewModel = new NoteViewModel(_mockCommonServices.Object, _mockLogger.Object);
        string? capturedResult = null;
        viewModel.Submitted += (s, e) => capturedResult = e.Result;

        // Act
        await viewModel.SubmitCommand.ExecuteAsync((string?)null);

        // Assert
        Assert.AreEqual(string.Empty, capturedResult);
    }
}

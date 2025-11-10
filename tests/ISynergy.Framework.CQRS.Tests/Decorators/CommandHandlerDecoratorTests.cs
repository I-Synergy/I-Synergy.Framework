using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.CQRS.Decorators.Tests;

[TestClass]
public class CommandHandlerDecoratorTests
{
    public class DummyCommand : ICommand { }
    public class DummyResultCommand : ICommand<string> { }

    [TestMethod]
    public async Task NotificationCommandHandlerDecorator_CallsInnerHandler_And_MessageService()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<DummyCommand>>();
        var messageServiceMock = new Mock<IMessengerService>();
        var decorator = new NotificationCommandHandlerDecorator<DummyCommand>(handlerMock.Object, messageServiceMock.Object);
        var command = new DummyCommand();

        // Act
        await decorator.HandleAsync(command);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        messageServiceMock.Verify(m => m.Send(It.Is<CommandMessage<string>>(s => s.Content.Contains("DummyCommand"))), Times.Once);
    }

    [TestMethod]
    public async Task NotificationCommandHandlerDecorator_WithResult_CallsInnerHandler_And_MessageService()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<DummyResultCommand, string>>();
        handlerMock.Setup(h => h.HandleAsync(It.IsAny<DummyResultCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("result");
        var messageServiceMock = new Mock<IMessengerService>();
        var decorator = new NotificationCommandHandlerDecorator<DummyResultCommand, string>(handlerMock.Object, messageServiceMock.Object);
        var command = new DummyResultCommand();

        // Act
        var result = await decorator.HandleAsync(command);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        messageServiceMock.Verify(m => m.Send(It.Is<CommandMessage<string>>(s => s.Content == result)), Times.Once);
        Assert.AreEqual("result", result);
    }

    [TestMethod]
    public async Task LoggingCommandHandlerDecorator_CallsInnerHandler_And_LogsInformation()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<DummyCommand>>();
        var loggerMock = new Mock<ILogger<LoggingCommandHandlerDecorator<DummyCommand>>>();
        var decorator = new LoggingCommandHandlerDecorator<DummyCommand>(handlerMock.Object, loggerMock.Object);
        var command = new DummyCommand();

        // Act
        await decorator.HandleAsync(command);

        // Assert
        handlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        loggerMock.Verify(l => l.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling command")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task LoggingCommandHandlerDecorator_LogsError_OnException()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<DummyCommand>>();
        handlerMock.Setup(h => h.HandleAsync(It.IsAny<DummyCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("fail"));
        var loggerMock = new Mock<ILogger<LoggingCommandHandlerDecorator<DummyCommand>>>();
        var decorator = new LoggingCommandHandlerDecorator<DummyCommand>(handlerMock.Object, loggerMock.Object);
        var command = new DummyCommand();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await decorator.HandleAsync(command));
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error handling command")),
            It.IsAny<InvalidOperationException>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}

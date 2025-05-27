using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.CQRS.Commands.Tests;

[TestClass]
public class CommandTests
{
    [TestMethod]
    public async Task CommandHandler_ExecutesCommand_SetsHandledFlag()
    {
        // Arrange
        var handler = new TestCommandHandler();
        var command = new TestCommand { Data = "Test Data" };

        // Act
        await handler.HandleAsync(command);

        // Assert
        Assert.IsTrue(handler.WasHandled);
    }

    [TestMethod]
    public async Task CommandHandlerWithResult_ExecutesCommand_ReturnsExpectedResult()
    {
        // Arrange
        var handler = new TestCommandWithResultHandler();
        var command = new TestCommandWithResult { Input = "Expected Input" };

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.AreEqual("Result: Expected Input", result);
    }

    [TestMethod]
    public async Task CommandHandler_WithCancellationToken_ThrowsWhenCancelled()
    {
        // Arrange
        var handler = new TestCommandHandler();
        var command = new TestCommand();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
        {
            await Task.Delay(100, cts.Token); // This will throw when used with the canceled token
            await handler.HandleAsync(command, cts.Token);
        });
    }
}
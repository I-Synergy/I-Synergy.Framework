using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.CQRS.Dispatchers.Tests;

[TestClass]
public class CommandDispatcherTests
{
    [TestMethod]
    public async Task CommandDispatcher_DispatchesCommand_CallsHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var handler = new TestCommandHandler();
        services.AddScoped<ICommandHandler<TestCommand>>(_ => handler);
        var provider = services.BuildServiceProvider();

        var dispatcher = new CommandDispatcher(provider);
        var command = new TestCommand();

        // Act
        await dispatcher.DispatchAsync(command);

        // Assert
        Assert.IsTrue(handler.WasHandled);
    }

    [TestMethod]
    public async Task CommandDispatcher_DispatchesCommandWithResult_ReturnsExpectedResult()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<TestCommandWithResult, string>>(_ =>
            new TestCommandWithResultHandler());
        var provider = services.BuildServiceProvider();

        var dispatcher = new CommandDispatcher(provider);
        var command = new TestCommandWithResult { Input = "Test Input" };

        // Act
        var result = await dispatcher.DispatchAsync<TestCommandWithResult, string>(command);

        // Assert
        Assert.AreEqual("Result: Test Input", result);
    }

    [TestMethod]
    public async Task CommandDispatcher_MissingHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var dispatcher = new CommandDispatcher(provider);
        var command = new TestCommand();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await dispatcher.DispatchAsync(command));
    }
}
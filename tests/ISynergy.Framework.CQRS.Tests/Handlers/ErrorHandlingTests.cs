using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Dispatchers;
using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.CQRS.Handlers.Tests;

[TestClass]
public class ErrorHandlingTests
{
    [TestMethod]
    public async Task CommandHandler_ThrowsException_PropagatesThroughDispatcher()
    {
        // Arrange
        var services = new ServiceCollection();
        var handlerMock = new Mock<ICommandHandler<TestCommand>>();

        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        services.AddScoped(_ => handlerMock.Object);
        var provider = services.BuildServiceProvider();

        var dispatcher = new CommandDispatcher(provider);
        var command = new TestCommand();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await dispatcher.DispatchAsync(command));

        Assert.AreEqual("Test exception", exception.Message);
    }

    [TestMethod]
    public async Task QueryHandler_ThrowsException_PropagatesThroughDispatcher()
    {
        // Arrange
        var services = new ServiceCollection();
        var handlerMock = new Mock<IQueryHandler<TestQuery, string>>();

        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test query exception"));

        services.AddScoped(_ => handlerMock.Object);
        var provider = services.BuildServiceProvider();

        var dispatcher = new QueryDispatcher(provider);
        var query = new TestQuery();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await dispatcher.DispatchAsync<string>(query));

        Assert.AreEqual("Test query exception", exception.Message);
    }
}
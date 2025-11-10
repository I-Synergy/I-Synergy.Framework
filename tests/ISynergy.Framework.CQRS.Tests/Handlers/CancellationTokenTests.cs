using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Dispatchers;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.CQRS.Handlers.Tests;

[TestClass]
public class CancellationTokenTests
{
    [TestMethod]
    public async Task CommandDispatcher_WithCancelledToken_PropagatesToHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var handlerMock = new Mock<ICommandHandler<TestCancellableCommand>>();

        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TestCancellableCommand>(), It.IsAny<CancellationToken>()))
            .Returns((TestCancellableCommand cmd, CancellationToken token) =>
            {
                // Verify the token is passed through
                token.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            });

        services.AddScoped(_ => handlerMock.Object);
        var provider = services.BuildServiceProvider();

        var dispatcher = new CommandDispatcher(provider, Mock.Of<ILogger<CommandDispatcher>>());
        var command = new TestCancellableCommand();
        using var cts = new CancellationTokenSource();

        // Act
        await dispatcher.DispatchAsync(command); // Should work with default token

        // Now cancel and verify it propagates
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await dispatcher.DispatchAsync(command, cts.Token));

        // Verify handler was called with the token
        handlerMock.Verify(h => h.HandleAsync(
            It.IsAny<TestCancellableCommand>(),
            It.Is<CancellationToken>(t => t.IsCancellationRequested)),
            Times.Once);
    }

    [TestMethod]
    public async Task QueryDispatcher_WithCancelledToken_PropagatesToHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var handlerMock = new Mock<IQueryHandler<TestCancellableQuery, string>>();

        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TestCancellableQuery>(), It.IsAny<CancellationToken>()))
            .Returns((TestCancellableQuery q, CancellationToken token) =>
            {
                // Verify the token is passed through
                token.ThrowIfCancellationRequested();
                return Task.FromResult("result");
            });

        services.AddScoped(_ => handlerMock.Object);
        var provider = services.BuildServiceProvider();

        var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());
        var query = new TestCancellableQuery();
        using var cts = new CancellationTokenSource();

        // Act
        await dispatcher.DispatchAsync<string>(query); // Should work with default token

        // Now cancel and verify it propagates
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await dispatcher.DispatchAsync<string>(query, cts.Token));

        // Verify handler was called with the token
        handlerMock.Verify(h => h.HandleAsync(
            It.IsAny<TestCancellableQuery>(),
            It.Is<CancellationToken>(t => t.IsCancellationRequested)),
            Times.Once);
    }

    public class TestCancellableCommand : ICommand { }
    public class TestCancellableQuery : IQuery<string> { }
}
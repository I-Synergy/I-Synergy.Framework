using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.CQRS.Dispatchers;

/// <summary>
/// Tests for the failure-handling paths of <see cref="CommandDispatcher"/> and
/// <see cref="QueryDispatcher"/>: the extracted <c>LogDispatchFailure</c> helpers must log the
/// failing type and the dispatchers must rethrow the original exception untouched (S2139 pattern).
/// Query dispatch must additionally skip logging for cooperative cancellation.
/// </summary>
[TestClass]
public class DispatcherFailureTests
{
    public TestContext TestContext { get; set; } = null!;

    private const string CommandFailure = "command handler failed";
    private const string QueryFailure = "query handler failed";

    #region CommandDispatcher

    [TestMethod]
    public async Task CommandDispatcher_WhenHandlerThrows_LogsErrorAndRethrowsSameException()
    {
        // Arrange
        var expected = new InvalidOperationException(CommandFailure);
        var logger = new Mock<ILogger<CommandDispatcher>>();
        var dispatcher = CreateCommandDispatcher(new ThrowingCommandHandler(expected), logger);

        // Act
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync(new FailingCommand(), TestContext.CancellationToken));

        // Assert
        Assert.AreSame(expected, actual);
        VerifyError(logger, expected, nameof(FailingCommand), "Error dispatching command of type");
    }

    [TestMethod]
    public async Task CommandDispatcher_WithResult_WhenHandlerThrows_LogsErrorAndRethrowsSameException()
    {
        // Arrange
        var expected = new InvalidOperationException(CommandFailure);
        var services = new ServiceCollection();
        var logger = new Mock<ILogger<CommandDispatcher>>();
        services.AddScoped<ICommandHandler<FailingResultCommand, string>>(
            _ => new ThrowingResultCommandHandler(expected));
        var dispatcher = new CommandDispatcher(services.BuildServiceProvider(), logger.Object);

        // Act
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync<FailingResultCommand, string>(new FailingResultCommand(), TestContext.CancellationToken));

        // Assert
        Assert.AreSame(expected, actual);
        VerifyError(logger, expected, nameof(FailingResultCommand), "Error dispatching command of type");
    }

    [TestMethod]
    public async Task CommandDispatcher_PassesCancellationTokenToHandler()
    {
        // Arrange
        var handler = new CancellationAwareCommandHandler();
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<FailingCommand>>(_ => handler);
        var dispatcher = new CommandDispatcher(
            services.BuildServiceProvider(),
            Mock.Of<ILogger<CommandDispatcher>>());
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        await dispatcher.DispatchAsync(new FailingCommand(), cancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(cancellationTokenSource.Token, handler.ReceivedToken);
    }

    #endregion

    #region QueryDispatcher

    [TestMethod]
    public async Task QueryDispatcher_GenericOverload_WhenHandlerThrows_LogsErrorAndRethrowsSameException()
    {
        // Arrange
        var expected = new InvalidOperationException(QueryFailure);
        var logger = new Mock<ILogger<QueryDispatcher>>();
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<FailingQuery, string>>(_ => new ThrowingQueryHandler(expected));
        var dispatcher = new QueryDispatcher(services.BuildServiceProvider(), logger.Object);

        // Act
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync<FailingQuery, string>(new FailingQuery(), TestContext.CancellationToken));

        // Assert
        Assert.AreSame(expected, actual);
        VerifyError(logger, expected, nameof(FailingQuery), "Error dispatching query of type");
    }

    [TestMethod]
    public async Task QueryDispatcher_ReflectionFallback_WhenHandlerThrows_SurfacesHandlerExceptionAndLogs()
    {
        // Arrange - no dispatch table, so the reflection path is used
        var expected = new InvalidOperationException(QueryFailure);
        var logger = new Mock<ILogger<QueryDispatcher>>();
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<FailingQuery, string>>(_ => new ThrowingQueryHandler(expected));
        var dispatcher = new QueryDispatcher(services.BuildServiceProvider(), logger.Object);

        // Act - the reflection wrapper (TargetInvocationException) must be unwrapped
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.DispatchAsync<string>(new FailingQuery(), TestContext.CancellationToken));

        // Assert
        Assert.AreSame(expected, actual);
        VerifyError(logger, expected, nameof(FailingQuery), "Error dispatching query of type");
    }

    [TestMethod]
    public async Task QueryDispatcher_WhenHandlerIsCancelled_PropagatesWithoutLoggingError()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryDispatcher>>();
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<FailingQuery, string>>(_ => new CancellingQueryHandler());
        var dispatcher = new QueryDispatcher(services.BuildServiceProvider(), logger.Object);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert - generic overload
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => dispatcher.DispatchAsync<FailingQuery, string>(new FailingQuery(), cancellationTokenSource.Token));

        // Act & Assert - reflection overload
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => dispatcher.DispatchAsync<string>(new FailingQuery(), cancellationTokenSource.Token));

        // Assert - cooperative cancellation is not an error
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    #endregion

    #region Helpers

    private static CommandDispatcher CreateCommandDispatcher(
        ICommandHandler<FailingCommand> handler,
        Mock<ILogger<CommandDispatcher>> logger)
    {
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<FailingCommand>>(_ => handler);
        return new CommandDispatcher(services.BuildServiceProvider(), logger.Object);
    }

    private static void VerifyError(
        Mock<ILogger<QueryDispatcher>> logger,
        Exception exception,
        string typeName,
        string messageStart) =>
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state!.ToString()!.StartsWith(messageStart, StringComparison.Ordinal) &&
                    state.ToString()!.Contains(typeName, StringComparison.Ordinal)),
                It.Is<Exception?>(e => ReferenceEquals(e, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

    private static void VerifyError(
        Mock<ILogger<CommandDispatcher>> logger,
        Exception exception,
        string typeName,
        string messageStart) =>
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state!.ToString()!.StartsWith(messageStart, StringComparison.Ordinal) &&
                    state.ToString()!.Contains(typeName, StringComparison.Ordinal)),
                It.Is<Exception?>(e => ReferenceEquals(e, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

    #endregion

    #region Test doubles

    public class FailingCommand : ICommand
    {
    }

    public class FailingResultCommand : ICommand<string>
    {
    }

    public class FailingQuery : IQuery<string>
    {
    }

    private sealed class ThrowingCommandHandler : ICommandHandler<FailingCommand>
    {
        private readonly Exception _exception;

        public ThrowingCommandHandler(Exception exception) => _exception = exception;

        public Task HandleAsync(FailingCommand command, CancellationToken cancellationToken = default) =>
            throw _exception;
    }

    private sealed class ThrowingResultCommandHandler : ICommandHandler<FailingResultCommand, string>
    {
        private readonly Exception _exception;

        public ThrowingResultCommandHandler(Exception exception) => _exception = exception;

        public Task<string> HandleAsync(FailingResultCommand command, CancellationToken cancellationToken = default) =>
            throw _exception;
    }

    private sealed class CancellationAwareCommandHandler : ICommandHandler<FailingCommand>
    {
        public CancellationToken ReceivedToken { get; private set; } = CancellationToken.None;

        public Task HandleAsync(FailingCommand command, CancellationToken cancellationToken = default)
        {
            ReceivedToken = cancellationToken;
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingQueryHandler : IQueryHandler<FailingQuery, string>
    {
        private readonly Exception _exception;

        public ThrowingQueryHandler(Exception exception) => _exception = exception;

        public Task<string> HandleAsync(FailingQuery query, CancellationToken cancellationToken = default) =>
            throw _exception;
    }

    private sealed class CancellingQueryHandler : IQueryHandler<FailingQuery, string>
    {
        public Task<string> HandleAsync(FailingQuery query, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(string.Empty);
        }
    }

    #endregion
}

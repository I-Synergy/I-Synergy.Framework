using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.Abstractions.Commands;

namespace ISynergy.Framework.CQRS.TestImplementations.Tests;

public class TestCommandHandler : ICommandHandler<TestCommand>
{
    public bool WasHandled { get; private set; }

    public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
    {
        WasHandled = true;
        return Task.CompletedTask;
    }
}

public class TestCommandWithResultHandler : ICommandHandler<TestCommandWithResult, string>
{
    public Task<string> HandleAsync(TestCommandWithResult command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"Result: {command.Input}");
    }
}

public class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"Query Result: {query.Parameter}");
    }
}
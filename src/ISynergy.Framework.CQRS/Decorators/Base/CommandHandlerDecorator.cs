using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Commands;

namespace ISynergy.Framework.CQRS.Decorators.Base;

/// <summary>
/// Base decorator for command handlers
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
public abstract class CommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    protected readonly ICommandHandler<TCommand> _decorated;

    protected CommandHandlerDecorator(ICommandHandler<TCommand> decorated)
    {
        _decorated = decorated;
    }

    public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base decorator for command handlers with result
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
/// <typeparam name="TResult">Type of result</typeparam>
public abstract class CommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    protected readonly ICommandHandler<TCommand, TResult> _decorated;

    protected CommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated)
    {
        _decorated = decorated;
    }

    public abstract Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
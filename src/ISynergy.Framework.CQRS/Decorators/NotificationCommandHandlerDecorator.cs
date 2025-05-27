using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Decorators.Base;
using ISynergy.Framework.CQRS.Messages;

namespace ISynergy.Framework.CQRS.Decorators;

/// <summary>
/// Decorator that sends notification messages after command execution
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
public class NotificationCommandHandlerDecorator<TCommand> : CommandHandlerDecorator<TCommand>
    where TCommand : ICommand
{
    private readonly IMessageService _messageService;

    public NotificationCommandHandlerDecorator(
        ICommandHandler<TCommand> decorated,
        IMessageService messageService) : base(decorated)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Send success notification
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        await _decorated.HandleAsync(command, cancellationToken);
        _messageService.Send(new CommandMessage<string>(typeof(TCommand).Name));
    }
}

/// <summary>
/// Decorator that sends notification messages after command execution with result
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
/// <typeparam name="TResult">Type of result</typeparam>
public class NotificationCommandHandlerDecorator<TCommand, TResult> : CommandHandlerDecorator<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly IMessageService _messageService;

    public NotificationCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> decorated,
        IMessageService messageService) : base(decorated)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Send success notification with result
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.HandleAsync(command, cancellationToken);
        _messageService.Send<CommandMessage<TResult>>(new CommandMessage<TResult>(result));
        return result;
    }
}
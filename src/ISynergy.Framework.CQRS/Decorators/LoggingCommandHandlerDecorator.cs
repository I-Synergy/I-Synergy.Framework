using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Decorators.Base;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.CQRS.Decorators;

/// <summary>
/// Decorator that adds logging to command handlers
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
public class LoggingCommandHandlerDecorator<TCommand> : CommandHandlerDecorator<TCommand>
    where TCommand : ICommand
{
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand>> _logger;

    public LoggingCommandHandlerDecorator(
        ICommandHandler<TCommand> decorated,
        ILogger<LoggingCommandHandlerDecorator<TCommand>> logger) : base(decorated)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandType = typeof(TCommand).Name;

        try
        {
            _logger.LogDebug("Handling command {CommandType}", commandType);
            await _decorated.HandleAsync(command, cancellationToken);
            _logger.LogDebug("Command {CommandType} handled successfully", commandType);
        }
        catch (Exception ex)
        {
            // The exception is rethrown unchanged; logging happens through a helper so that the
            // catch block only rethrows.
            LogHandlingFailure(ex, commandType);
            throw;
        }
    }

    private void LogHandlingFailure(Exception exception, string commandType) =>
        _logger.LogError(exception, "Error handling command {CommandType}", commandType);
}

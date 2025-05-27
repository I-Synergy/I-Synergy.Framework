namespace ISynergy.Framework.CQRS.Commands;

/// <summary>
/// Command handler interface for commands without explicit result
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

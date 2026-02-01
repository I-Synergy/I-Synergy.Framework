using ISynergy.Framework.CQRS.Commands;

namespace ISynergy.Framework.CQRS.Abstractions.Commands;
/// <summary>
/// Command handler interface with explicit result
/// </summary>
/// <typeparam name="TCommand">Type of command</typeparam>
/// <typeparam name="TResult">Type of result</typeparam>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of command execution</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

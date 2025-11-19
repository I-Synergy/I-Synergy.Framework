using ISynergy.Framework.CQRS.Commands;

namespace ISynergy.Framework.CQRS.Abstractions.Dispatchers;

/// <summary>
/// Command dispatcher interface
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command without explicit return value
    /// </summary>
    /// <typeparam name="TCommand">Type of command</typeparam>
    /// <param name="command">Command to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Dispatches a command with explicit return value
    /// </summary>
    /// <typeparam name="TCommand">Type of command</typeparam>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="command">Command to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Command execution result</returns>
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}
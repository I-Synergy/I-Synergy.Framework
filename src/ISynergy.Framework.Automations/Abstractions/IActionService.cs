namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Action service interface.
/// </summary>
public interface IActionService
{
    /// <summary>
    /// Executes action.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    Task ExcecuteActionAsync(IAction action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tasks that are still not executed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    Task RefreshTasksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates Schedule/Delay expiration when action is saved.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    Task<(TimeSpan Expiration, IAction? UpcomingTask)?> CalculateTimespanAsync(CancellationToken cancellationToken = default);
}

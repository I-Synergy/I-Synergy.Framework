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
    /// <returns></returns>
    Task ExcecuteActionAsync(IAction action);

    /// <summary>
    /// Gets all tasks that are still not executed.
    /// </summary>
    /// <returns></returns>
    Task RefreshTasksAsync();

    /// <summary>
    /// Calculates Schedule/Delay expiration when action is saved.
    /// </summary>
    /// <returns></returns>
    Task<(TimeSpan Expiration, IAction? UpcomingTask)?> CalculateTimespanAsync();
}

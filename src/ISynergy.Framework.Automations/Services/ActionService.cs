using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Extensions;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Action service.
/// </summary>
public class ActionService : IActionService
{
    private readonly ILogger _logger;
    private readonly AutomationOptions _options;
    private readonly IActionManager _manager;
    private readonly List<IAction> _tasks;

    /// <summary>
    /// Default constructor for the background service.
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public ActionService(
        IActionManager manager,
        IOptions<AutomationOptions> options,
        ILogger<ActionService> logger)
    {
        _tasks = new List<IAction>();
        _manager = manager;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gets all tasks that are still not executed.
    /// </summary>
    /// <returns></returns>
    public async Task RefreshTasksAsync() =>
        _tasks.AddNewRange(await _manager.GetItemsAsync());

    /// <summary>
    /// Calculates Schedule/Delay expiration when action is saved.
    /// </summary>
    /// <returns></returns>
    public async Task<(TimeSpan Expiration, IAction UpcomingTask)> CalculateTimespanAsync()
    {
        var upcomingTask = _tasks.FirstOrDefault();

        if (upcomingTask is not null)
        {
            if (upcomingTask is ScheduledAction scheduledAction)
                return (scheduledAction.ExecutionTime - DateTimeOffset.Now, scheduledAction);

            else if (upcomingTask is DelayAction delayAction &&
                await _manager.GetTimePreviousCompletedTaskAsync(delayAction.AutomationId) is DateTimeOffset previousCompletedDateTime)
                return (previousCompletedDateTime.Add(delayAction.Delay) - DateTimeOffset.Now, delayAction);
        }

        return (TimeSpan.Zero, null);
    }

    /// <summary>
    /// Executes action.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task ExcecuteActionAsync(IAction action)
    {
        if (action.ToTask() is Func<Task> job)
            await job
                .Invoke()
                .ContinueWith(async x =>
                {
                    if (x.IsCompleted)
                        await _manager.SetActionExcecutedAsync(action.ActionId);
                    _logger.LogInformation($"Task with {action.ActionId} executed");
                });
    }
}

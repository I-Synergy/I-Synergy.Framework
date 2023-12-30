using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// The AutomationService class is responsible for managing and executing automations. Automations are a set of conditions and actions that get triggered based on certain events.
/// This class takes in an IAutomationManager, IOptions of AutomationOptions, and an ILogger as dependencies injected via the constructor.
/// It has a list of Automation objects to store any loaded automations.The RefreshAutomationsAsync method will populate this list by calling the automation manager to get the latest set of automations.
/// The main logic is in ExecuteAsync.This takes in an Automation, a value, and a CancellationTokenSource.It will first validate if all the conditions for the automation are met based on the input value by calling ValidateConditionsAsync. If the conditions pass, it will start executing the tasks in the automation one by one until complete or cancelled via the cancellation token.
/// The key steps are:
/// 1. Validate conditions
/// 2. Get a queue of tasks from the automation
/// 3. Loop through each task
///     1. Execute task
///     2. Check if cancellation requested
///     3. Handle errors
/// 4. Return result
/// This allows the service to take a defined automation with conditions and tasks, check if it should run based on input, and then process the workflow of tasks.The background logic handles the asynchronous execution, concurrency, error handling, and cancellation.
/// </summary>
public class AutomationService : IAutomationService
{
    private readonly ILogger _logger;
    private readonly AutomationOptions _options;
    private readonly IAutomationManager _manager;
    private readonly List<Automation> _automations;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public AutomationService(
        IAutomationManager manager,
        IOptions<AutomationOptions> options,
        ILogger<AutomationService> logger)
    {
        _automations = new List<Automation>();
        _manager = manager;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gets all automations.
    /// </summary>
    /// <returns></returns>
    public async Task RefreshAutomationsAsync() =>
        _automations.AddNewRange(await _manager.GetItemsAsync());

    /// <summary>
    /// Validates the conditions.
    /// </summary>
    /// <param name="automation"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task<bool> ValidateConditionsAsync(Automation automation, object value)
    {
        var areAllConditionsValid = true;

        // Check if all conditions met.
        foreach (var condition in automation.Conditions)
        {
            if (condition.Operator == OperatorTypes.And)
                areAllConditionsValid = areAllConditionsValid && condition.ValidateCondition(value);
            else
                areAllConditionsValid = areAllConditionsValid || condition.ValidateCondition(value);
        }

        return Task.FromResult(areAllConditionsValid);
    }

    /// <summary>
    /// Gets a queue of tasks and executes them 1 by 1 until finished or cancelled by timeout.
    /// </summary>
    /// <param name="automation"></param>
    /// <param name="value"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task<ActionResult> ExecuteAsync(Automation automation, object value, CancellationTokenSource cancellationTokenSource)
    {
        if (automation.IsActive && await ValidateConditionsAsync(automation, value))
        {
            cancellationTokenSource.CancelAfter(automation.ExecutionTimeout);

            using (var queue = await GetTasksFromActionsAsync(automation, value, cancellationTokenSource))
            {
                while (queue.Count > 0)
                {
                    var nextTask = queue.Take(cancellationTokenSource.Token);

                    if (cancellationTokenSource.IsCancellationRequested)
                        break;

                    await nextTask
                        .Invoke()
                        .ContinueWith(x => _logger.LogInformation("Task completed!"), cancellationTokenSource.Token);
                }

                return new ActionResult(true, value);
            }
        }

        return new ActionResult(false, value);
    }

    /// <summary>
    /// Gets a BlockingCollection queue containing all tasks for this automation.
    /// </summary>
    /// <param name="automation"></param>
    /// <param name="value"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    private Task<BlockingCollection<Func<Task>>> GetTasksFromActionsAsync(Automation automation, object value, CancellationTokenSource cancellationTokenSource)
    {
        var queue = new BlockingCollection<Func<Task>>();
        var repeatCount = 0;

        // Adds all tasks to the queue.
        for (int i = 0; i < automation.Actions.Count; i++)
        {
            if (automation.Actions[i] is CommandAction commandAction && commandAction.Command.CanExecute(commandAction.CommandParameter))
            {
                queue.Add(new Func<Task>(async () =>
                {
                    await Task.Run(() => commandAction.Command.Execute(commandAction.CommandParameter), cancellationTokenSource.Token).ConfigureAwait(false);
                }));
            }
            else if (automation.Actions[i] is DelayAction delayAction)
            {
                queue.Add(new Func<Task>(async () =>
                {
                    await Task.Delay(delayAction.Delay, cancellationTokenSource.Token).ConfigureAwait(false);
                    await Task.Yield();
                }));
            }
            else if (automation.Actions[i] is AutomationAction automationAction)
            {
                queue.Add(new Func<Task>(async () =>
                {
                    await ExecuteAsync(automationAction.Automation, value, cancellationTokenSource).ConfigureAwait(false);
                }));
            }
            else if (automation.Actions[i] is RepeatPreviousAction repeatAction && repeatAction.Count > 0)
            {
                if (repeatCount == repeatAction.Count)
                {
                    repeatCount = 0;
                }
                else
                {
                    i -= 2;
                    repeatCount += 1;
                }
            }
            else if (automation.Actions[i] is IRepeatAction untilRepeatAction && untilRepeatAction.RepeatType == RepeatTypes.Until)
            {
                if (repeatCount.Equals(untilRepeatAction.CountCircuitBreaker) || untilRepeatAction.ValidateAction(value))
                {
                    repeatCount = 0;
                }
                else
                {
                    i -= 2;
                    repeatCount += 1;
                }
            }
            else if (automation.Actions[i] is IRepeatAction whileRepeatAction && whileRepeatAction.RepeatType == RepeatTypes.While)
            {
                if (repeatCount.Equals(whileRepeatAction.CountCircuitBreaker) || !whileRepeatAction.ValidateAction(value))
                {
                    repeatCount = 0;
                }
                else
                {
                    i -= 2;
                    repeatCount += 1;
                }
            }
        }

        queue.CompleteAdding();

        return Task.FromResult(queue);
    }
}

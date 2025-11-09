using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Enumerations;
using System.Collections.Concurrent;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Service responsible for building action execution queues.
/// </summary>
public class ActionQueueBuilder : IActionQueueBuilder
{
    private readonly IActionExecutorFactory _executorFactory;
    private readonly IAutomationService _automationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionQueueBuilder"/> class.
    /// </summary>
    /// <param name="executorFactory">The factory for resolving action executors.</param>
    /// <param name="automationService">The automation service for nested automations.</param>
    public ActionQueueBuilder(IActionExecutorFactory executorFactory, IAutomationService automationService)
    {
        _executorFactory = executorFactory;
        _automationService = automationService;
    }

    /// <summary>
    /// Builds a queue of tasks from automation actions.
    /// </summary>
    /// <param name="automation">The automation containing actions.</param>
    /// <param name="value">The value to pass to actions.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <returns>A blocking collection containing task functions.</returns>
    public Task<BlockingCollection<Func<Task>>> BuildQueueAsync(Automation automation, object value, CancellationTokenSource cancellationTokenSource)
    {
        var queue = new BlockingCollection<Func<Task>>();
        var repeatCount = 0;

        // Adds all tasks to the queue.
        for (int i = 0; i < automation.Actions.Count; i++)
        {
            var action = automation.Actions[i];

            // Handle repeat actions
            if (action is RepeatPreviousAction repeatAction && repeatAction.Count > 0)
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
                continue;
            }

            if (action is IRepeatAction repeatActionWithCondition)
            {
                if (repeatActionWithCondition.RepeatType == RepeatTypes.Until)
                {
                    if (repeatCount.Equals(repeatActionWithCondition.CountCircuitBreaker) || repeatActionWithCondition.ValidateAction(value))
                    {
                        repeatCount = 0;
                    }
                    else
                    {
                        i -= 2;
                        repeatCount += 1;
                    }
                }
                else if (repeatActionWithCondition.RepeatType == RepeatTypes.While)
                {
                    if (repeatCount.Equals(repeatActionWithCondition.CountCircuitBreaker) || !repeatActionWithCondition.ValidateAction(value))
                    {
                        repeatCount = 0;
                    }
                    else
                    {
                        i -= 2;
                        repeatCount += 1;
                    }
                }
                continue;
            }

            // Get executor for the action type using the non-generic interface
            var executor = _executorFactory.GetExecutor(action) as IActionExecutor;
            if (executor is null)
            {
                // No executor found for this action type, skip it
                continue;
            }

            // Use the non-generic interface method
            var taskFunc = executor.CreateTaskAsync(action, value, cancellationTokenSource, _automationService);
            if (taskFunc is not null)
            {
                queue.Add(taskFunc);
            }
        }

        queue.CompleteAdding();

        return Task.FromResult(queue);
    }
}


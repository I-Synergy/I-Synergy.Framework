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
        var index = 0;

        // The index advances manually because repeat actions rewind the queue to the previous action.
        while (index < automation.Actions.Count)
        {
            var action = automation.Actions[index];
            index++;

            if (TryHandleRepeat(action, value, ref repeatCount, out var rewind))
            {
                if (rewind)
                    index -= 2;

                continue;
            }

            EnqueueAction(action, value, cancellationTokenSource, queue);
        }

        queue.CompleteAdding();

        return Task.FromResult(queue);
    }

    /// <summary>
    /// Applies the repeat bookkeeping for a repeat action.
    /// </summary>
    /// <param name="action">The action being processed.</param>
    /// <param name="value">The value to pass to the repeat condition.</param>
    /// <param name="repeatCount">The number of times the previous action has been repeated.</param>
    /// <param name="rewind"><c>true</c> when the previous action has to be queued again.</param>
    /// <returns><c>true</c> when the action is a repeat action and has been fully handled.</returns>
    private static bool TryHandleRepeat(IAction action, object value, ref int repeatCount, out bool rewind)
    {
        rewind = false;

        if (action is RepeatPreviousAction repeatPreviousAction && repeatPreviousAction.Count > 0)
        {
            if (repeatCount == repeatPreviousAction.Count)
            {
                repeatCount = 0;
            }
            else
            {
                rewind = true;
                repeatCount += 1;
            }

            return true;
        }

        if (action is not IRepeatAction repeatActionWithCondition)
            return false;

        // null when the repeat type has no condition to evaluate, in which case the counter is left as is.
        bool? isDone = repeatActionWithCondition.RepeatType switch
        {
            RepeatTypes.Until => repeatCount.Equals(repeatActionWithCondition.CountCircuitBreaker)
                                 || repeatActionWithCondition.ValidateAction(value),
            RepeatTypes.While => repeatCount.Equals(repeatActionWithCondition.CountCircuitBreaker)
                                 || !repeatActionWithCondition.ValidateAction(value),
            _ => null
        };

        if (isDone == true)
        {
            repeatCount = 0;
        }
        else if (isDone == false)
        {
            rewind = true;
            repeatCount += 1;
        }

        return true;
    }

    /// <summary>
    /// Creates the task for the action and appends it to the queue.
    /// </summary>
    /// <param name="action">The action to enqueue.</param>
    /// <param name="value">The value to pass to the action.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="queue">The queue to append to.</param>
    private void EnqueueAction(
        IAction action,
        object value,
        CancellationTokenSource cancellationTokenSource,
        BlockingCollection<Func<Task>> queue)
    {
        // Get executor for the action type using the non-generic interface
        var executor = _executorFactory.GetExecutor(action) as IActionExecutor;
        if (executor is null)
        {
            // No executor found for this action type, skip it
            return;
        }

        // Use the non-generic interface method
        var taskFunc = executor.CreateTaskAsync(action, value, cancellationTokenSource, _automationService);
        if (taskFunc is not null)
        {
            queue.Add(taskFunc, cancellationTokenSource.Token);
        }
    }
}

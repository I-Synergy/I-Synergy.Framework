using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Services.Executors;

/// <summary>
/// Executor for DelayAction.
/// </summary>
public class DelayActionExecutor : IActionExecutor<DelayAction>
{
    /// <summary>
    /// Creates a task function for the given action.
    /// </summary>
    /// <param name="action">The action to create a task for.</param>
    /// <param name="value">The value to pass to the action (not used for delay actions).</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service (not used for delay actions).</param>
    /// <returns>A function that returns a task representing the action execution, or null if the action cannot be executed.</returns>
    public Func<Task>? CreateTaskAsync(IAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        if (action is DelayAction delayAction)
        {
            return CreateTaskAsync(delayAction, value, cancellationTokenSource, automationService);
        }

        return null;
    }

    /// <summary>
    /// Creates a task function for the given delay action.
    /// </summary>
    /// <param name="action">The delay action to create a task for.</param>
    /// <param name="value">The value to pass to the action (not used for delay actions).</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service (not used for delay actions).</param>
    /// <returns>A function that returns a task representing the delay.</returns>
    public Func<Task>? CreateTaskAsync(DelayAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        return new Func<Task>(async () =>
        {
            await Task.Delay(action.Delay, cancellationTokenSource.Token).ConfigureAwait(false);
            await Task.Yield();
        });
    }
}


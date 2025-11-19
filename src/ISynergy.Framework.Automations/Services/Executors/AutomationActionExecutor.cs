using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Services.Executors;

/// <summary>
/// Executor for AutomationAction.
/// </summary>
public class AutomationActionExecutor : IActionExecutor<AutomationAction>
{
    /// <summary>
    /// Creates a task function for the given action.
    /// </summary>
    /// <param name="action">The action to create a task for.</param>
    /// <param name="value">The value to pass to the nested automation.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service for executing nested automations.</param>
    /// <returns>A function that returns a task representing the action execution, or null if the action cannot be executed.</returns>
    public Func<Task>? CreateTaskAsync(IAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        if (action is AutomationAction automationAction)
        {
            return CreateTaskAsync(automationAction, value, cancellationTokenSource, automationService);
        }

        return null;
    }

    /// <summary>
    /// Creates a task function for the given automation action.
    /// </summary>
    /// <param name="action">The automation action to create a task for.</param>
    /// <param name="value">The value to pass to the nested automation.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service for executing nested automations.</param>
    /// <returns>A function that returns a task representing the nested automation execution.</returns>
    public Func<Task>? CreateTaskAsync(AutomationAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        if (action.Automation is null)
            return null;

        return new Func<Task>(async () =>
        {
            await automationService.ExecuteAsync(action.Automation, value, cancellationTokenSource).ConfigureAwait(false);
        });
    }
}


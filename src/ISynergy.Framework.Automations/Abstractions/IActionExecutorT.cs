namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Interface for executing a specific type of action.
/// </summary>
/// <typeparam name="TAction">The type of action to execute.</typeparam>
public interface IActionExecutor<TAction> : IActionExecutor where TAction : IAction
{
    /// <summary>
    /// Creates a task function for the given action.
    /// </summary>
    /// <param name="action">The action to create a task for.</param>
    /// <param name="value">The value to pass to the action.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service for nested automations.</param>
    /// <returns>A function that returns a task representing the action execution.</returns>
    Func<Task>? CreateTaskAsync(TAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService);
}

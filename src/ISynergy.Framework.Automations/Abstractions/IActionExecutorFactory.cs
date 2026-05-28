namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Factory interface for resolving action executors.
/// </summary>
public interface IActionExecutorFactory
{
    /// <summary>
    /// Gets an executor for the specified action type.
    /// </summary>
    /// <typeparam name="TAction">The type of action.</typeparam>
    /// <returns>An executor for the action type, or null if no executor is registered.</returns>
    IActionExecutor<TAction>? GetExecutor<TAction>() where TAction : IAction;

    /// <summary>
    /// Gets an executor for the specified action instance.
    /// </summary>
    /// <param name="action">The action instance.</param>
    /// <returns>An executor for the action, or null if no executor is registered.</returns>
    object? GetExecutor(IAction action);
}


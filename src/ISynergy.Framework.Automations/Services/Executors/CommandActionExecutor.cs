using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Services.Executors;

/// <summary>
/// Executor for CommandAction.
/// </summary>
public class CommandActionExecutor : IActionExecutor<CommandAction>
{
    /// <summary>
    /// Creates a task function for the given action.
    /// </summary>
    /// <param name="action">The action to create a task for.</param>
    /// <param name="value">The value to pass to the action (not used for command actions).</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service (not used for command actions).</param>
    /// <returns>A function that returns a task representing the action execution, or null if the action cannot be executed.</returns>
    public Func<Task>? CreateTaskAsync(IAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        if (action is CommandAction commandAction)
        {
            return CreateTaskAsync(commandAction, value, cancellationTokenSource, automationService);
        }

        return null;
    }

    /// <summary>
    /// Creates a task function for the given command action.
    /// </summary>
    /// <param name="action">The command action to create a task for.</param>
    /// <param name="value">The value to pass to the action (not used for command actions).</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="automationService">The automation service (not used for command actions).</param>
    /// <returns>A function that returns a task representing the command execution, or null if the command cannot execute.</returns>
    public Func<Task>? CreateTaskAsync(CommandAction action, object value, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        if (action.Command.CanExecute(action.CommandParameter))
        {
            return new Func<Task>(async () =>
            {
                await Task.Run(() => action.Command.Execute(action.CommandParameter), cancellationTokenSource.Token).ConfigureAwait(false);
            });
        }

        return null;
    }
}


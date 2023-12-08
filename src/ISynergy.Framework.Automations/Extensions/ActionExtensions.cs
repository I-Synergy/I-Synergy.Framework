using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;

namespace ISynergy.Framework.Automations.Extensions;

/// <summary>
/// Action extensions.
/// </summary>
public static class ActionExtensions
{
    /// <summary>
    /// Converts DelayAction or ScheduledAction to a Func{Task}.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Func<Task> ToTask(this IAction action)
    {
        if (action is DelayAction delayAction)
            return new Func<Task>(() => Task.Delay(delayAction.Delay));

        if (action is ScheduledAction scheduledAction)
            return new Func<Task>(() => Task.Delay(DateTimeOffset.Now - scheduledAction.ExecutionTime));

        throw new ArgumentException("Parameter 'action' can only be type of 'DelayAction' or 'ScheduledAction'");
    }
}

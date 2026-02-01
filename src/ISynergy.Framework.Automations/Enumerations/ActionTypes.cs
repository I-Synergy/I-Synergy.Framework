namespace ISynergy.Framework.Automations.Enumerations;

/// <summary>
/// Action types
/// </summary>
public enum ActionTypes
{
    /// <summary>
    /// Executes an ICommand.
    /// </summary>
    Command,
    /// <summary>
    /// Conditional action.
    /// </summary>
    Condition,
    /// <summary>
    /// Delay.
    /// </summary>
    Delay,
    /// <summary>
    /// Fires event.
    /// </summary>
    FireEvent,
    /// <summary>
    /// Runs other automation.
    /// </summary>
    Automation,
    /// <summary>
    /// Waits
    /// </summary>
    Wait,
    /// <summary>
    /// Repeats action.
    /// </summary>
    Repeat

}

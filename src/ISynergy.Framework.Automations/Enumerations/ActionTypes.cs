namespace ISynergy.Framework.Automations.Enumerations;

/// <summary>
/// Action types
/// </summary>
/// <remarks>
/// Values are explicit because the enum is serialized/persisted — inserting a member must not
/// relabel existing persisted values.
/// </remarks>
public enum ActionTypes
{
    /// <summary>
    /// Executes an ICommand.
    /// </summary>
    Command = 0,
    /// <summary>
    /// Conditional action.
    /// </summary>
    Condition = 1,
    /// <summary>
    /// Delay.
    /// </summary>
    Delay = 2,
    /// <summary>
    /// Fires event.
    /// </summary>
    FireEvent = 3,
    /// <summary>
    /// Runs other automation.
    /// </summary>
    Automation = 4,
    /// <summary>
    /// Waits
    /// </summary>
    Wait = 5,
    /// <summary>
    /// Repeats action.
    /// </summary>
    Repeat = 6

}

namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Public interface of a trigger.
/// </summary>
public interface ITrigger
{
    /// <summary>
    /// Trigger Id.
    /// </summary>
    Guid TriggerId { get; }

    /// <summary>
    /// Gets or sets the AutomationId property value.
    /// </summary>
    public Guid AutomationId { get; }

    /// <summary>
    /// You can use For to have the trigger only fire if the state holds for some time.
    /// </summary>
    TimeSpan For { get; set; }
}

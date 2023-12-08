namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Public interface of an action.
/// </summary>
public interface IAction
{
    /// <summary>
    /// Gets or sets the AutomationId property value.
    /// </summary>
    public Guid AutomationId { get; }
    /// <summary>
    /// Gets or sets the ActionId property value.
    /// </summary>
    Guid ActionId { get; }
    /// <summary>
    /// Gets or sets the Data property value.
    /// </summary>
    object Data { get; set; }
    /// <summary>
    /// Gets the Executed property value.
    /// </summary>
    bool Executed { get; }
    /// <summary>
    /// Gets the ExecutedDateTime property value.
    /// </summary>
    DateTimeOffset ExecutedDateTime { get; }
}

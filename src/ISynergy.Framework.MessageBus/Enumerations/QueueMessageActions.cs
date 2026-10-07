namespace ISynergy.Framework.MessageBus.Enumerations;

/// <summary>
/// Actions of QueueMessage object.
/// </summary>
/// <remarks>
/// Values are explicit because the enum is serialized/persisted — inserting a member must not
/// relabel existing persisted values.
/// </remarks>
public enum QueueMessageActions
{
    /// <summary>
    /// Performs an add action on object.
    /// </summary>
    Add = 0,
    /// <summary>
    /// Performs an update action on object.
    /// </summary>
    Update = 1,
    /// <summary>
    /// Performs a delete action on object.
    /// </summary>
    Remove = 2
}

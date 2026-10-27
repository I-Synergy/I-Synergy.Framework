namespace ISynergy.Framework.Monitoring.Enumerations;

/// <summary>
/// Monitor event names.
/// </summary>
/// <remarks>
/// Values are explicit because the enum is serialized/persisted — inserting a member must not
/// relabel existing persisted values.
/// </remarks>
public enum MonitorEvents
{
    /// <summary>
    /// When client is connected.
    /// </summary>
    Connected = 0,
    /// <summary>
    /// When client is disconnected.
    /// </summary>
    Disconnected = 1
}

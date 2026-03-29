namespace ISynergy.Framework.EventSourcing.Models;

/// <summary>
/// A point-in-time snapshot of an aggregate's state, used to avoid replaying the full event stream.
/// </summary>
/// <param name="AggregateId">The aggregate instance identifier.</param>
/// <param name="TenantId">Tenant that owns this snapshot. Must match <see cref="EventRecord.TenantId"/> for the same aggregate.</param>
/// <param name="AggregateType">The aggregate type name.</param>
/// <param name="Version">The aggregate version at the time the snapshot was taken.</param>
/// <param name="Data">JSON-serialized aggregate state.</param>
/// <param name="Timestamp">UTC timestamp when the snapshot was created.</param>
public record Snapshot(
    Guid AggregateId,
    Guid TenantId,
    string AggregateType,
    long Version,
    string Data,
    DateTimeOffset Timestamp);

namespace ISynergy.Framework.EventSourcing.Models;

/// <summary>
/// Immutable read model returned by <see cref="Abstractions.Events.IEventStore"/> query methods.
/// </summary>
/// <param name="EventId">Unique event identifier.</param>
/// <param name="TenantId">Tenant that owns this event.</param>
/// <param name="AggregateType">The aggregate type name (e.g. "Budget").</param>
/// <param name="AggregateId">The aggregate instance identifier.</param>
/// <param name="AggregateVersion">The version of the aggregate after this event was applied.</param>
/// <param name="EventType">The fully-qualified or short event type name (e.g. "BudgetCreated").</param>
/// <param name="Data">JSON-serialized event payload.</param>
/// <param name="Metadata">Optional JSON-serialized metadata (correlation IDs, causation IDs, etc.).</param>
/// <param name="Timestamp">UTC timestamp when the event was stored.</param>
/// <param name="UserId">
/// The user who triggered the event, or <c>null</c> for system-generated events
/// (e.g. background jobs, scheduled tasks, migrations).
/// </param>
public record EventRecord(
    Guid EventId,
    Guid TenantId,
    string AggregateType,
    Guid AggregateId,
    long AggregateVersion,
    string EventType,
    string Data,
    string? Metadata,
    DateTimeOffset Timestamp,
    string? UserId);

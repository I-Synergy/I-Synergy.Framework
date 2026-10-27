namespace ISynergy.Framework.EventSourcing.Enumerations;

/// <summary>
/// Discriminates the kind of row stored in the event store's <c>EventStore</c> table.
/// </summary>
/// <remarks>
/// A single physical table stores more than one logical row kind. This discriminator lets
/// readers and projections filter to the rows they care about without parsing the payload,
/// and it reserves numeric ids for future row kinds without renumbering existing values.
/// Ids 2–4 are reserved for future row kinds; only <see cref="DomainEvent"/> is written today.
/// </remarks>
public enum EventRecordTypes : short
{
    /// <summary>
    /// A domain event emitted by an aggregate — the canonical event-sourcing row.
    /// </summary>
    DomainEvent = 1,

    /// <summary>
    /// Reserved for future use — an integration event published to other bounded contexts.
    /// Not written yet; no row currently uses this kind.
    /// </summary>
    IntegrationEvent = 2,

    /// <summary>
    /// Reserved for future use — an outbox message staged for reliable delivery.
    /// Not written yet; no row currently uses this kind.
    /// </summary>
    OutboxMessage = 3,

    /// <summary>
    /// Reserved for future use — an audit entry recording a state change.
    /// Not written yet; no row currently uses this kind.
    /// </summary>
    AuditEntry = 4
}

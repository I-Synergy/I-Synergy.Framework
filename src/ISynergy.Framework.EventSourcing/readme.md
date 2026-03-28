# I-Synergy Framework EventSourcing

Provides base infrastructure for event-sourced domain aggregates in I-Synergy applications.

## Included abstractions

| Type | Purpose |
|------|---------|
| `IDomainEvent` | Marker interface for all domain events |
| `DomainEventBase` | Abstract record — provides `EventId` + `OccurredAt` |
| `IAggregateRoot<TId>` | Interface for event-sourced aggregate roots |
| `AggregateRoot<TId>` | Base class — manages uncommitted events and version |
| `IAggregateRepository<TAggregate, TId>` | Load/save aggregates via the event store |
| `IEventStore` | Append-only event storage with snapshot support |
| `EventRecord` | Immutable read model returned by event store queries |
| `Snapshot` | Point-in-time aggregate state for replay optimisation |
| `IProjection<TEvent>` | Updates a read model in response to a domain event |
| `IProjectionRebuildService` | Rebuilds a projection by replaying the full event stream |

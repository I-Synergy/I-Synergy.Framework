# I-Synergy Framework EventSourcing EntityFramework

EF Core implementation of `IEventStore` and `IAggregateRepository` for I-Synergy Event Sourcing.

## Included types

| Type | Purpose |
|------|---------|
| `EventSourcingDbContext` | `DbContext` with `EventStore` and `Snapshots` tables; applies global tenant query filter |
| `EfEventStore` | `IEventStore` implementation using EF Core with optimistic concurrency |
| `EfAggregateRepository<TAggregate, TId>` | `IAggregateRepository` implementation using `IEventStore` |
| `IEventTypeResolver` | Resolves CLR types from stored event type names for deserialization |
| `DefaultEventTypeResolver` | Default resolver — scans loaded assemblies by full type name |

## Registration

```csharp
services
    .AddMultiTenancyIntegration()  // registers ITenantService
    .AddEventSourcingEntityFramework(o => o.UseSqlServer(connectionString))
    .AddAggregateRepository<OrderAggregate, Guid>();
```

# HANDOVER — Option 4: projection replay / rebuild from the event store

**Why:** the read model is a projection of the event store, but there is no way to rebuild it. When the
projection is skipped or fails (see `I-Synergy/.ai/plans/fix-event-sourcing-projection-atomicity.md`),
the only recovery today is hand-editing the database.

## Goal

A mechanism that rebuilds any read-model table from `events.EventStore`, so a stale or missing
projection is always recoverable without manual SQL.

## Evidence (today)

- Measured orphans: `BudgetAggregate f0f2b178-…` and `ExpenseTypeAggregate a21e9f62-…` — a `*Created`
  event with no read-model row; no replay path exists.
- The event store holds everything needed: `AggregateType, AggregateId, AggregateVersion, EventType,
  Data, TenantId`.

## Suggested shape

1. **Replayer service** in `ISynergy.Domain.Budgets` (or a shared projection project): for a given
   `AggregateType` + tenant, read `events.EventStore` ordered by `AggregateId, AggregateVersion`,
   `LoadAsync`/replay the aggregate, and upsert the read-model row.
2. **Expose it** as: an admin-only endpoint (`.Produces` metadata), or a one-shot job
   (`ISynergy.Service.Jobs.*`, `StopApplication()` on completion like the migrations job).
3. **Tenant-scoped**: honour `TenantContext` / `IgnoreQueryFilters` per the requester's rules.

## Verification

- Rebuild after the two known orphans exist → header counts converge
  (`events.EventStore` streams == read-model rows) with **0** orphans.
- Idempotent: running it twice does not duplicate rows.

## Out of scope

- Real-time/streaming projection (that is Option 3's transaction, or an outbox).

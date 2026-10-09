# HANDOVER — Option 3: transaction / UnitOfWork surface on `IAggregateRepository`

**Target repo:** `D:\Projects\I-Synergy\I-Synergy.Framework` (the framework that produces
`I-Synergy.Framework.EventSourcing.EntityFramework`). Move this file to that repo's `.ai/plans/`.
**Why:** the business API cannot make the event-store append and the read-model projection atomic
because the repository exposes no way to enlist the append in the caller's transaction (see
`I-Synergy/.ai/plans/fix-event-sourcing-projection-atomicity.md`).

## Goal

Let a caller run the event append and its read-model write in **one** database transaction over the
**same connection**, so a projection failure rolls back the append (and vice versa).

## Evidence (today)

- `IAggregateRepository<TAggregate, TId>` (package `2026.11008.10155`) exposes only
  `SaveAsync(aggregate, ct)` — no transaction/UnitOfWork surface.
- The event-store `EventSourcingDbContext` and the application `DataContext` are registered on
  **separate connections**; there is no distributed transaction on Linux/Podman.
- Consequence in the business API: `CreateBudgetCommandHandler` appends the event then projects the
  read model in a second context; a projection failure (duplicate `(TenantId, Description)`) leaves an
  **orphan event** (measured: `events.EventStore` 16 `BudgetCreated` vs 15 `budgets."Budget"` rows).

## Suggested shape (framework)

Pick one, all additive:

1. **Ambient/shared connection** — let the repository accept an `IDbContextTransaction`/
   `DbConnection` (e.g. `SaveAsync(aggregate, ct, IDbContextTransaction? tx = null)`), so the caller's
   `DataContext` transaction and the event append share one connection.
2. **UnitOfWork** — expose an `IUnitOfWork` the caller begins, hands to the repository, and commits
   once; the repository enlists its `SaveChanges` in it.
3. **Explicit transaction scope** — an `IEventSourcingTransaction` the framework opens on the shared
   `NpgsqlDataSource`.

## Verification

- A failing read-model write (unique-constraint) leaves **no** event row.
- The happy path still appends exactly one event and one read-model row.
- No behaviour change to `LoadAsync` / replay.

## Out of scope

- The business-repo write handlers (they adopt the surface once it exists).

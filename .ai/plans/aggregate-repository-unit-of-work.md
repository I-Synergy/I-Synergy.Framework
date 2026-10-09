# Plan — UnitOfWork transaction surface on `IAggregateRepository`

Status: **PROPOSED — awaiting approval** (2026-10-08)
Scope: **Option 3 only** (framework change). Shape chosen by the user: **`IUnitOfWork` the caller begins,
hands to the repository, commits once.** Option 4 (projection replay) is a separate handover for the
business repo and is **out of scope**.

Supersedes `.ai/plans/handover-framework-aggregate-uow.md` (kept as the source brief).

## Goal

Let a caller run the event-store append and its read-model write in **one** database transaction over the
**same connection**, so a read-model failure rolls back the append (and vice versa) — instead of the append
committing on its own connection while the read model fails separately.

## Evidence (today)

- `IAggregateRepository<TAggregate,TId>` (`src/ISynergy.Framework.EventSourcing/Abstractions/Aggregates/IAggregateRepository.cs`)
  exposes only `LoadAsync` / `SaveAsync(aggregate, ct)` — no transaction/UnitOfWork surface.
- `AggregateRepository` appends through the scoped `EventStore` → its own `EventSourcingDbContext`
  (`src/…EventSourcing.EntityFramework/EventSourcingDbContext.cs`), each `SaveChangesAsync` auto-committing.
- The business repo registers the read-model `DataContext` and the `EventSourcingDbContext` as **two
  separate Npgsql `DbContext`s** (`I-Synergy/src/ISynergy.Service.Shared/Extensions/HostApplicationBuilderExtensions.{Database,EventSourcing}.cs`)
  on separate connections; there is no DTC on Linux/Podman.
- Consequence: `CreateBudgetCommandHandler` (`I-Synergy/src/ISynergy.Domain.Budgets/Features/Budgets/Commands/CreateBudget/CreateBudgetCommandHandler.cs`)
  today stages the read-model write inside a `DataContext` transaction and then calls
  `budgetRepository.SaveAsync(...)` — which commits on a *different* connection. Measured:
  `events.EventStore` 16 `BudgetCreated` vs `budgets."Budget"` 15 rows → 1 orphan.

## Design

### Contract — new abstraction in the framework EventSourcing project

`src/ISynergy.Framework.EventSourcing/Abstractions/Transactions/IUnitOfWork.cs`:

```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    bool IsActive { get; }
    DbConnection Connection { get; }
    DbTransaction Transaction { get; }

    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
```

- Provider-agnostic: only `System.Data.Common.DbConnection`/`DbTransaction` (BCL) — the EventSourcing package
  takes neither an Npgsql nor an EF Core dependency. **Confirmed:** `ISynergy.Framework.EventSourcing.csproj`
  references only `ISynergy.Framework.Core`, so EF Core types cannot appear on this interface.
- `Enlist(DbContext)` — which needs EF Core — becomes an **extension method** in the EntityFrag project:
  `EntityFrameworkUnitOfWorkExtensions.Enlist(this IUnitOfWork unitOfWork, DbContext context)`
  (`src/ISynergy.Framework.EventSourcing.EntityFramework/Extensions/`).

### Implementation — event-store connection is the owner of the shared transaction

`src/ISynergy.Framework.EventSourcing.EntityFramework/Services/EventSourcingUnitOfWork.cs`:

- Ctor takes the scoped `EventSourcingDbContext` (same lifetime). Registered `AddScoped<IUnitOfWork, EventSourcingUnitOfWork>()`
  by `AddEventSourcingEntityFramework(...)`.
- `BeginAsync` (see Q1) opens the context's connection if closed and begins the transaction on it; exposes
  `Connection` + `Transaction`.
- `CommitAsync` / `RollbackAsync` delegate to the transaction; `DisposeAsync` rolls back if still active.
  The UoW never disposes the `DbContext` (DI owns it).
- `Enlist(context)` = `context.Database.SetDbConnection(uow.Connection)` + `context.Database.UseTransaction(uow.Transaction)`
  — the single place that rebinds a second context onto the shared connection.

Rationale: the EventSourcing side owns the connection it already controls; the caller's read-model
`DataContext` is the one that adopts it. The **alternative** (UoW owns a fresh `DbConnection` from a
connection factory) avoids `SetDbConnection` on a DI-bound context but needs a new connection abstraction in
the framework — rejected as a larger surface for the same result.

### Repository surface

`IAggregateRepository<TAggregate,TId>` gains a transactional overload, **kept additive**:

```csharp
Task SaveAsync(TAggregate aggregate, IUnitOfWork unitOfWork, CancellationToken cancellationToken = default)
    => throw new NotSupportedException("This repository implementation does not support an external unit of work.");
```

- Default interface implementation → existing implementers do not break (source or binary); the framework's
  `AggregateRepository` **overrides** it. The default throws rather than silently ignoring the UoW (a silent
  no-op would reintroduce the orphan bug under a new name).
- `AggregateRepository.SaveAsync(aggregate, uow, ct)`: if `_context.Database.CurrentTransaction is null`,
  `await _context.Database.UseTransactionAsync(uow.Transaction, ct)` (same connection, so EF accepts it),
  then run the existing append path unchanged.
- `LoadAsync` and the existing `SaveAsync(aggregate, ct)` are **untouched**.

### Caller flow (business repo, future adoption — documented, not implemented here)

```csharp
await unitOfWork.BeginAsync(ct);
unitOfWork.Enlist(dataContext);              // read-model context joins the event-store transaction
dataContext.Budgets.Add(entity);
await dataContext.SaveChangesAsync(ct);
await budgetRepository.SaveAsync(aggregate, unitOfWork, ct);
await unitOfWork.CommitAsync(ct);
```

## Reused existing types (rule 12)

- `ISynergy.Framework.EventSourcing.Aggregates.AggregateRoot<TId>`, `IAggregateRepository<TAggregate,TId>`
- `ISynergy.Framework.EventSourcing.EntityFramework.EventSourcingDbContext`, `Services.EventStore`,
  `Services.AggregateRepository<TAggregate,TId>`, `Extensions.ServiceCollectionExtensions`
- `ISynergy.Framework.Core.Abstractions.Services.ITenantService` (unchanged; tenant scoping is unaffected)
- No new serializer/resolver/entity types.

## Files

- **New** `src/ISynergy.Framework.EventSourcing/Abstractions/Transactions/IUnitOfWork.cs`
- **New** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/EventSourcingUnitOfWork.cs`
- **New** `src/ISynergy.Framework.EventSourcing.EntityFramework/Extensions/EntityFrameworkUnitOfWorkExtensions.cs`
- **Edit** `src/ISynergy.Framework.EventSourcing/Abstractions/Aggregates/IAggregateRepository.cs` (+overload)
- **Edit** `src/ISynergy.Framework.EventSourcing.EntityFramework/Services/AggregateRepository.cs` (+override)
- **Edit** `src/ISynergy.Framework.EventSourcing.EntityFramework/Extensions/ServiceCollectionExtensions.cs`
  (register `IUnitOfWork` in both `AddEventSourcingEntityFramework` overloads)
- **New tests** `tests/ISynergy.Framework.EventSourcing.EntityFramework.Tests/Services/UnitOfWorkTests.cs`
- **Edit** `src/ISynergy.Framework.EventSourcing.EntityFramework/readme.md` (+ the EventSourcing `readme.md`)
- **Edit** `CHANGELOG.md` (`[Unreleased] → Added`)

## Breaking-change analysis

- Interface overload via **default implementation** → additive; no consumer/implementer break.
- No signature removed or changed; `Sample.EventSourcing.Api` and the existing tests compile unchanged.
- Therefore **no council** is convened (AGENTS.md: an additive, backward-compatible change does not convene).
  If review concludes the default must be dropped (plain interface member → source-breaking), reconvene first.

## Verification

- Happy path: with a UoW active, one append yields exactly one event row and one read-model row.
- Rollback: a read-model write that throws inside the UoW leaves **no** event row.
- Idempotence of the surface: the existing `SaveAsync(aggregate, ct)` path still appends exactly one event
  (`AggregateRepositoryTests` unchanged and green).
- `LoadAsync` / replay behaviour unchanged.
- **Provider note:** the EF Core **InMemory** provider used by `EventSourcingDbContextFactory` has no real
  transactions and cannot prove atomicity. Two options for the test (Q3): add **`Microsoft.EntityFrameworkCore.Sqlite`**
  to the test project (SQLite supports a transaction over a shared in-memory connection hosting both contexts)
  or leave atomicity verified by inspection + the business-repo integration test. **Docker is unavailable on
  this machine**, so a real Postgres/Testcontainers run is not possible locally.
- Build: `MSBUILDDISABLENODEREUSE=1 dotnet build <proj> -nodeReuse:false -p:UseSharedCompilation=false`.
  Test: `MSBUILDDISABLENODEREUSE=1 dotnet test tests/ISynergy.Framework.EventSourcing.EntityFramework.Tests -nodeReuse:false`.

## Risks / Open Questions

- **Q1 — who calls `BeginAsync`?** The interface needs a `BeginAsync(ct)`. Recommend it lives on `IUnitOfWork`
  (caller-driven, matches "the caller begins").
- **Q2 — EF Core in the abstraction project: RESOLVED.** `IUnitOfWork` stays in
  `ISynergy.Framework.EventSourcing` with BCL-only types; `Enlist(DbContext)` is an extension in the
  EntityFrag project.
- **Q3 — test provider** (SQLite dependency vs inspection) — see Verification.
- **Q4 — `SetDbConnection` on a DI-bound context.** `Enlist` rebinds the caller's `DataContext`; this is the
  one step whose EF Core behaviour must be confirmed by a spike before the surface is declared done.
- **Non-goal:** the business-repo handlers; they adopt the surface once it ships.

## Out of scope

- Projection replay / rebuild (Option 4 — separate handover, business repo).
- Real-time/streaming projection or an outbox.
- Any change to tenant scoping or the event schema.

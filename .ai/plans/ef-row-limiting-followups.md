# Fix EF row-limiting follow-ups

**Status:** DONE (completed 2026-10-07)
**Scope:** `EnumerableExtensions.cs` (Core), `EventArchiver.cs` (EventSourcing.Storage), Core tests

## Problem

Three instances of the same class of issue found after the `ToPage` fix:

1. `EventArchiver.ArchiveOldEventsAsync` — EF query `...GroupBy(...).Select(...).Take(batchSize)` with no `OrderBy`,
   so it raises `RowLimitingOperationWithoutOrderByWarning` and selects its batch in a non-deterministic order
   (an archiver batch may never drain a backlog).
2. `EnumerableExtensions.CountPages` — uses floor division (`Count() / pageSize`), under-counting pages for any
   remainder (35 @ 10 → 3, should be 4). The EF twin `QueryableExtensions.CountPages` uses ceiling, so the two
   public overloads disagree.
3. `EnumerableExtensions.ToPage` — the `pageIndex < 0` guard throws `ArgumentBelowZeroException(nameof(pageSize))`
   (wrong parameter name); no ordering-aware overload exists.

## Existing types reused (rule 12)

- `EnumerableExtensions` (Core) — extend in place.
- `ArgumentBelowZeroException` / `ArgumentOutOfRangeException` (Core) — reused for validation.
- `QueryableExtensions.CountPages` — the reference behaviour the IEnumerable overload must match.
- No new types/projects.

## Changes

1. `EnumerableExtensions.CountPages`: ceiling division via `Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(source.Count()) / pageSize))`.
2. `EnumerableExtensions.ToPage`: fix `nameof(pageSize)` → `nameof(pageIndex)`; add ordered overload
   `ToPage<TSource, TKey>(source, Func<TSource, TKey> orderBy, pageIndex, pageSize)`.
3. `EventArchiver`: add `.OrderBy(x => x.TenantId).ThenBy(x => x.AggregateType).ThenBy(x => x.AggregateId)` before `.Take(...)`.
4. Core tests for the CountPages/ToPage changes.

Adding an overload is backward compatible; no council trigger fires.

## Verification

`MSBUILDDISABLENODEREUSE=1 dotnet test` on `ISynergy.Framework.Core.Tests` and the EventSourcing suites.

Note: EF Core's InMemory provider does not raise `RowLimitingOperationWithoutOrderByWarning` (relational-only), and the
repo has no relational/SQLite test provider, so the `EventArchiver` `ORDER BY` is verified by build + no-regression only.

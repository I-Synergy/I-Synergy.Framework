# Upstream framework — EF row-limiting warning

**Status:** DONE
Started: 2026-10-07
Completed: 2026-10-07
Plan: `.ai/plans/upstream-framework-ef-row-limiting-warning.md`

## Steps
- [x] Add ordering-aware `ToPage<TEntity, TKey>` overload + validation helper
- [x] Update readme pagination example
- [x] Add MSTest coverage
- [x] Build and test (MSBUILDDISABLENODEREUSE=1, -nodeReuse:false)

## Notes
- Root cause: `ToPage` applied `Skip`/`Take` without `OrderBy`, so EF Core raised
  `CoreEventId.RowLimitingOperationWithoutOrderByWarning` and pages were non-deterministic.
- Fix is additive: the existing `ToPage(pageIndex, pageSize)` overload is kept; a new
  `ToPage(query, Expression<Func<TEntity,TKey>> orderBy, pageIndex, pageSize)` applies
  `OrderBy` before `Skip`/`Take`. Shared validation extracted to a private helper.
- Tests: `QueryableExtensionTests` (ordered paging + argument validation) and
  `ToPageRowLimitingWarningTests` (expression-tree ordering guarantee).
- Verified: `dotnet test tests/ISynergy.Framework.EntityFramework.Tests` → Passed 52, Failed 0, Skipped 9.
- Caveat: EF Core's InMemory provider does not raise the row-limiting warning, so the warning
  itself is not asserted at runtime; the ordered-before-Skip/Take expression shape is asserted instead.

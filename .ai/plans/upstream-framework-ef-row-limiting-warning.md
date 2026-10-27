# Fix EF Core row-limiting-without-OrderBy warning in ToPage

**Status:** APPROVED (user selected this approach)
**Scope:** `src/ISynergy.Framework.EntityFramework/Extensions/QueryableExtensions.cs` (+ readme + tests)

## Problem

`QueryableExtensions.ToPage<TEntity>(IQueryable<TEntity>, pageIndex, pageSize)` applies
`.Skip(...).Take(...)` with no ordering. When a caller has not ordered the query first,
EF Core logs `CoreEventId.RowLimitingOperationWithoutOrderByWarning`
("A query uses a row limiting operation (Skip/Take) without OrderBy which may lead to
unpredictable results."), and results are genuinely non-deterministic across pages.

Because this is an **upstream framework** helper, the framework cannot know how the caller
wants to order — so every downstream consumer that paged without an explicit `OrderBy`
inherited the warning and the correctness bug.

## Existing types reused (rule 12)

- `ISynergy.Framework.EntityFramework.Extensions.QueryableExtensions` — extend, do not duplicate.
- Existing `CountPages` / `ToPage(pageIndex, pageSize)` signatures are kept (backward compatible).
- No new classes or projects.

## Approach (additive, non-breaking)

1. Keep `ToPage<TEntity>(query, pageIndex, pageSize)` for compatibility, and document that it
   does **not** order (so callers must order themselves to avoid the EF warning).
2. Add an ordering-aware overload
   `ToPage<TEntity, TKey>(query, Expression<Func<TEntity,TKey>> orderBy, pageIndex, pageSize)`
   that applies `OrderBy(orderBy)` before `Skip`/`Take`, guaranteeing a stable, warning-free page.
3. Extract the shared `pageIndex` / `pageSize` validation into a private helper.
4. Update the readme pagination sample to the ordered overload.
5. Add tests: ordered overload orders before paging; validation still throws.

Adding an overload is backward compatible — no public contract breaks, so no council is required.

## Steps

- [x] Write/extend `QueryableExtensions` with the ordered overload + validation helper
- [x] Update readme pagination example
- [x] Add MSTest coverage
- [x] Build and test with `MSBUILDDISABLENODEREUSE=1 -nodeReuse:false`

## Verification

`dotnet test tests/ISynergy.Framework.EntityFramework.Tests` (node reuse disabled).

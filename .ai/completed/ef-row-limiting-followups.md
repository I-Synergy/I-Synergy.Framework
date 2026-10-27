# EF row-limiting follow-ups

**Status:** DONE
Started: 2026-10-07
Completed: 2026-10-07
Plan: `.ai/plans/ef-row-limiting-followups.md`

## Steps
- [x] Fix `EnumerableExtensions.CountPages` to ceiling division (match EF overload)
- [x] Fix `EnumerableExtensions.ToPage` param name and add ordered overload
- [x] Add Core.Tests coverage for CountPages/ToPage/ordered overload
- [x] Add deterministic OrderBy to `EventArchiver` eligible-streams query
- [x] Build and test (MSBUILDDISABLENODEREUSE=1, -nodeReuse:false)

## Notes
- `EventArchiver.ArchiveOldEventsAsync` now orders by `TenantId`, `AggregateType`, `AggregateId` before `Take`,
  so batch selection is deterministic and the row-limiting warning no longer applies.
- `EnumerableExtensions.CountPages` now uses ceiling division (`Convert.ToInt32(Math.Ceiling(...))`), matching
  `QueryableExtensions.CountPages` (35 @ 10 → 4, was 3).
- `EnumerableExtensions.ToPage`: `pageIndex < 0` now throws `ArgumentBelowZeroException(nameof(pageIndex))`
  (was `nameof(pageSize)`); added ordered overload `ToPage<TSource,TKey>(source, Func<TSource,TKey> orderBy, pageIndex, pageSize)`.
- Verified: Core.Tests 572/572, EventSourcing.EntityFramework.Tests 27/27, EventSourcing.Storage build 0 warnings/0 errors.
- Limitation: the `EventArchiver` `ORDER BY` cannot be asserted at runtime — EF Core's row-limiting warning is
  relational-only and the InMemory provider never raises it; the repo has no relational/SQLite test provider.
- `tests/ISynergy.Framework.Core.Tests/Extensions/EnumerableExtensionsTest.cs` line endings were normalized CRLF→LF
  (file is stored CRLF; the appended tests inherited it and tripped `git diff --check`) — content unchanged.

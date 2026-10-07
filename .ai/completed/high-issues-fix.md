# Fixing the 4 High issues from the whole-framework review

**Status:** DONE
**Started:** 2026-10-07
**Client:** Reasonix Code
**Report:** `docs/FRAMEWORK_REVIEW_2026-10-07.md`

## Decisions (user, via ask)
- **H1 credentials** — blank the committed values and document loading from env/user-secrets. Rotation of the exposed keys is the user's action.
- **H2 Sample.Maui build** — leave the break; document only.
- **H3 FileResult** — add an async `FileAsync` property and deprecate the sync `File`/`GetStream()`.
- **H4 DbContextExtensions** — delete the class and its tests (breaking public API).

## Steps
- [x] Investigate all four issues and surface decisions
- [x] H1: blank committed Sentry DSN + App Insights key in sample appsettings; document loading
- [x] H2: document the Sample.Maui build break (readme + report)
- [x] H3: EventTrigger→SafeFireAndForget; FileResult→GetFileAsync + deprecate; NetworkUtility→shared HttpClient
- [x] H4: delete DbContextExtensions + tests; update readme + CHANGELOG
- [x] Verify: build affected projects + run targeted tests

## Result
All four High issues addressed; affected test projects green (Core 572, Mvvm 262, Automations 19, EntityFramework 49; 0 failures).

## Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).

---

# Council Record — retire DbContextExtensions

**Status:** DECIDED
**Date:** 2026-10-07
**Trigger:** Breaks a public contract — `ISynergy.Framework.EntityFramework.Extensions.DbContextExtensions` is public API of the published `I-Synergy.Framework.EntityFramework` package; removal is source- and binary-breaking for consumers.
**Decision question (verbatim):** How should the public API class `ISynergy.Framework.EntityFramework.Extensions.DbContextExtensions` be retired — (A) removed outright now, (B) marked [Obsolete] and removed after a deprecation window, or (C) relocated to an opt-in companion package — and what migration/versioning treatment does the chosen path require?
**Seats:** Compatibility/consumer-impact → read-only subagent; Rules/design-doctrine → read-only subagent; Feasibility/verifiability → read-only subagent. (Chair held no seat.)

## Positions
| Seat | Round 1 | After cross-examination | Confidence |
|---|---|---|---|
| Compatibility | (B) obsolete now, remove later | changed once → (B) as mechanism, terminating in (C) | medium |
| Rules | (A) remove outright | (A) held | medium-high |
| Feasibility | (B) obsolete now, remove later | changed once → (A) | high |

## Decision
**Remove `DbContextExtensions` outright (A), shipping the migration guidance (EF Core `DbSet<T>` primitives) in the same change.** This matches the user's explicit choice.

## Rationale
- **Adopted — Rules & Feasibility:** the documented rule bans the *symbol*, not merely its call sites; (B)/(C) keep a prohibited, AOT-unsafe API shippable for the whole window. Feasibility confirmed the in-repo blast radius is only the class's own tests, and the package makes no semver promise (date-based versioning), so a deprecation window buys no consumer guarantee.
- **Rejected — Compatibility's (B)-then-(C):** rejected because a window that "terminates in (C)" still ships the prohibited symbol now, and no versioning promise exists to make the window meaningful. Its non-breaking-at-assembly argument is real but outweighed.
- Migration guidance ships with the removal (readme section 3 rewritten to DbSet primitives; CHANGELOG `### Removed` entry).

## Recorded dissent (verbatim)
> **Compatibility seat:** "A deprecation with no enforced end is just removal deferred; (B) only earns its warning window if it terminates in (C) or (A), so record the terminus, not the warning."

> **Rules seat:** "The rules prohibit the symbol, not merely its call sites; an unenforced `[Obsolete]` keeps the prohibited AOT-unsafe API shippable by binary consumers who never recompile, so (B) trades a real prohibition for a warning nobody is obliged to read."

## Reopen conditions
- The package is observed to have external consumers calling `ExistsAsync`/`GetItemByIdAsync`/`RemoveItemAsync`, or a support/compatibility obligation is stated → reconvene on a provisional `[Obsolete(error:true)]` tombstone.
- A documented deprecation/compatibility policy is adopted for the framework → reconvene.

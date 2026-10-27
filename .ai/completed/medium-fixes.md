# Fixing the Medium issues from the whole-framework review

**Status:** DONE
**Started:** 2026-10-07
**Client:** Reasonix Code
**Report:** `docs/FRAMEWORK_REVIEW_2026-10-07.md`

## Decisions (user, via ask)
- **Docs/AOT** — update `architecture.md` to reality **and** remove the `Core → Globalization` project reference. Not making the AOT/trim guarantee real.
- **PII** — make PII enrichment opt-in.
- **Sample auth/tenant** — fix `Sample.EventSourcing.Api` and wire auth.

## Steps
- [x] M1: Rewrite architecture.md to the real graph, DI coverage, trim/AOT status
- [x] M2: Remove the Core → Globalization project reference (council-adjudicated)
- [x] M3: Fix async void LogIfFailed; add CancellationToken to Automations + NetworkUtility async APIs
- [x] M4: Make PII enrichment opt-in (SendDefaultPii=false + gate the processor)
- [x] M5: NuGetAuditMode=all + vulnerable-package CI check
- [x] M6: Fix Sample.EventSourcing.Api tenant spoofing + wire auth + tighten CORS
- [x] M7: UI.Maui.Mac.Tests — no safe fix; documented the MTP/`dotnet test` incompatibility (CI already excludes it)
- [x] Verify

## Result
All Medium items addressed as far as is safe. Full solution build: 0 non-Sample.Maui errors.
Test sweep (33 projects): 0 failures. `git diff --check`: clean.

## Notes
- M7 is not fixable as specified: repo-wide MTP opt-in (global.json `test.runner`) breaks the
  VSTest projects; project-level knobs do not suppress the .NET 10 error; the project is a Mac
  bundle that `dotnet test` cannot launch. CI already excludes it; the constraint is documented in
  the csproj.

---

# Council Record — remove Core → Globalization project reference

**Status:** DECIDED
**Date:** 2026-10-07
**Trigger:** Alters an established architecture decision (layering / project structure) and moves public types between published packages.
**Decision question (verbatim):** How should the `ISynergy.Framework.Core -> ISynergy.Framework.Globalization` ProjectReference be removed so that Core has no framework-project dependency — given that Core currently uses, from Globalization: (a) `LanguageService.Default.GetString(key)` and `ILanguageService`, and (b) the JSON converters `DateTimeConverter`/`DateTimeOffsetConverter`/`TimeOnlyConverter`?
**Seats:** Design (architect) → read-only subagent; Compatibility (reviewer) → read-only subagent; Feasibility (developer) → read-only subagent. (Chair held no seat.)

## Positions
| Seat | Position (summary) | Confidence |
|---|---|---|
| Design | Invert the edge (Globalization → Core); move `ILanguageService` + the three date converters into Core; replace Core's static `LanguageService.Default` with a Core-owned seam. | high |
| Compatibility | Same move, **plus `[assembly: TypeForwardedTo]` shims** in Globalization so already-compiled consumers keep binding; `ILanguageService` is used in ~50 in-solution places. | medium-high |
| Feasibility | Hybrid: invert; relocate `ILanguageService` + converters + `IsoDateTimeFormat` into Core; add a Core-owned ambient default; the concrete `LanguageService` must stay in Globalization (owns resource satellites). | medium |

## Decision
**Adopted all three, as implemented:** invert the edge so `ISynergy.Framework.Globalization → ISynergy.Framework.Core`; move `ILanguageService`, `GlobalizationConstants`, and the three JSON converters into the Core assembly while **keeping their `ISynergy.Framework.Globalization.*` namespaces** (so no consumer using-directive changes are needed and `TypeForwardedTo` can preserve binary identity); add a Core-owned ambient `LanguageServiceProvider` to replace Core's concrete `LanguageService`/`LanguageService.Default` usage; add `TypeForwardedTo` forwarders in Globalization; and add explicit `Globalization` references to the projects that previously got it transitively through Core.

## Rationale
- Adopted Design: `Core` is the ubiquitous root; `Globalization` is the leaf implementation — inverting is the correct direction (no cycle, since Globalization had zero framework references).
- Adopted Compatibility: `TypeForwardedTo` keeps binary consumers working; retaining the original namespaces keeps all in-repo `using` directives valid.
- Adopted Feasibility: the concrete `LanguageService` (resource-owning) stays in Globalization; Core reaches it through the ambient seam. Core's callers fall back gracefully (`Argument` to `defaultMessage`; `LocalizedDescriptionAttribute` to the resource key) until a provider is registered.
- **Cost surfaced during implementation:** because Core previously carried Globalization transitively, ~5 hub projects (`UI`, `AspNetCore.Blazor`, `AspNetCore.Globalization`, `Documents.Syncfusion`, `Geography`) needed explicit `Globalization` references. This is the real, unavoidable cost of removing the edge.

## Recorded dissent (verbatim)
> **Design seat (self-objection):** "Moving `ILanguageService` into Core pollutes a dependency-free core with a localization concern and re-introduces global/service-locator state via a settable `Default` resolver — arguably worse architecture than the coupling being removed."

> **Feasibility seat (self-objection):** "Moving the public converter types (and `GlobalizationConstants`) into Core is itself a breaking public NuGet surface change."

## Reopen conditions
- Consumers of the Globalization package are found to reference the moved types in a way the namespaces/forwarders do not absorb → reconvene.
- A future decision to make these packages genuinely AOT/trim-safe changes the type-ownership calculus → reconvene.

## Notes
- Verification: full solution Release build has **zero non-`Sample.Maui` errors**; `ISynergy.Framework.Core` and `ISynergy.Framework.Globalization` build clean; `Globalization.Tests` 109/109 pass (after this change).

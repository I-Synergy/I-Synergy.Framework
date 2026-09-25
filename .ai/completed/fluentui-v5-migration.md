# Fluent UI Blazor v4 → v5 Migration (Sample.Blazor)

**Status:** DONE
Started: 2026-09-25 · Completed: 2026-09-25

Plan: `.ai/plans/fluentui-v5-migration.md`

## Outcome

`Microsoft.FluentUI.AspNetCore.Components` **4.14.4 → 5.0.0** (package bumps were the user's; this
session did the code migration). `samples/Sample.Blazor` builds clean and its behaviour is verified
by a new Playwright suite.

| | Baseline | Final |
|---|---|---|
| Build errors | 4 | **0** |
| Build warnings | 12 | **0** |
| `RZ10012` (silently-degraded components) | 12 | **0** |
| Playwright suite | *(did not exist)* | **30/30 passing** |
| Solution build | — | **0 errors**, 9 pre-existing unrelated warnings |

Blast radius was contained to `samples/Sample.Blazor` — verified `git status --short src/` is empty;
no framework project references FluentUI.

## The thing that made this non-trivial

The project **built successfully in its broken state**. v5 removed `FluentNavMenu`, `FluentHeader`,
`FluentBodyContent`, `FluentFooter`, `FluentTextField`, `FluentDialogHeader/Footer` and
`FluentMenuProvider`; an unresolved Razor tag compiles to a `RZ10012` *warning* and renders as an inert
custom element. So "it builds" proved nothing, and the validation was designed around asserting real
DOM rather than page-load success.

A second trap: `MainLayout.razor.cs` contained `await dialogResult.Result`, which is a latent compile
error in v5 (`ShowInfoAsync` now returns `Task<DialogResult>`, which has no `.Result`) — invisible in
the baseline because the razor errors masked it.

## Key API mappings (verified against the shipped assembly, not just the docs)

The official docs are wrong or incomplete in places, so every mapping was cross-checked by
decompiling `Microsoft.FluentUI.AspNetCore.Components.dll` with `ilspycmd`:

| v4 | v5 |
|---|---|
| `FluentNavMenu` / `FluentNavGroup` / `FluentNavLink` | `FluentNav` / `FluentNavCategory` / `FluentNavItem` |
| `FluentHeader` / `FluentBodyContent` / `FluentFooter` | `FluentLayout` + `FluentLayoutItem Area=` |
| `FluentTextField` | `FluentTextInput` |
| `FluentProgressRing` | `FluentSpinner` |
| `FluentLabel` (typography) | `FluentText` |
| `IDialogContentComponent<T>` | `FluentDialogInstance` (abstract; `OnActionClickedAsync` is **abstract**) |
| `FluentDialogHeader` / `FluentDialogFooter` | `FluentDialogBody` `TitleTemplate` / `ActionTemplate` slots |
| `DialogParameters` | `DialogOptions` (Header/Footer are **get-only**) |

Docs errors found and corrected: the index implies Toast was removed (`FluentToastProvider` still
exists — only `MaxToastCount` and the `*ToastContent` types went away); `<FluentProviders>@Body</…>` is
not compilable (no `ChildContent` — use self-closing); `new DialogOptions { Header = … }` is not
compilable (get-only); `ClearAfterNavigation` was removed with **no** replacement and is undocumented
as such.

`DialogOptions.Parameters` keys must exactly match a `[Parameter]` name or it throws at render time —
the sample uses `nameof(...)` so a rename breaks the build instead.

## What was verified working

- **Dialog** (the Critical-impact area): opens, binds, closes; renders `fluent-text-input`; typed text
  survives a forced re-render.
- Nav renders exactly one `id="main_menu"` and 4 `fluent-navitem`; mobile drawer navigates.
- Weather streams 5 rows; counter increments.
- No `loading-theme.js` request (the script the old `App.razor` loaded no longer ships) and no 404s.
- Contrast ≥ 5.38; accessible names on 5/5 home and 12/12 dialog controls.

## Upstream bug found (F1) — worked around

**`FluentLayout.IsMobile` is never set when the page is first loaded at ≤768px**, leaving mobile
navigation unreachable. Root cause: `Startup.afterStarted()` → `LayoutAutoInitialize()` initialises the
layout with `dotNetHelper = null`; its one-shot `ResizeObserver` callback then sets the `mobile`
attribute but its `?.invokeMethodAsync('FluentLayout_MediaChangedAsync')` is a no-op. The later
interactive `Initialize(dotNetHelper, …)` hits the `fluentLayoutInitialized` early-return and never
re-creates the observer. Result: DOM says mobile, .NET says desktop. Crossing the breakpoint at
runtime self-heals, which proves it's an initialisation race.

**Correction worth remembering:** a bare ResizeObserver re-fire is a *provable no-op* — the shipped
callback notifies .NET only on a state **transition**, and on an initial mobile load the attribute is
already set, so neither branch runs. The shipped workaround therefore nudges the box (to re-run the
library's detection) **and** supplies the missing notification explicitly through the library's own
idempotent `[JSInvokable]` entry point.

- Workaround: `samples/Sample.Blazor/wwwroot/js/layout-breakpoint-fix.js` +
  `MainLayout.razor.cs` (`ApplyLayoutBreakpointFixAsync`, firstRender only). Fully commented and
  marked safe to delete.
- v5.0.0 is the latest published version — no upgrade path yet.
- Draft upstream report (not filed): `.ai/analysis/fluentui-v5-mobile-nav-bug.md`

## Files changed

**App** (`samples/Sample.Blazor/`): `Components/App.razor`, `Components/Controls/NavigationMenuControl.razor`,
`Components/Controls/NavigationMenuItemControl.razor`, `Components/Dialogs/Base/BaseEmptyDialog.razor`,
`Components/Dialogs/Base/BaseModelDialog.razor`, `Components/Dialogs/SampleDialog.razor`,
`Components/Layout/MainLayout.razor`, `Components/Layout/MainLayout.razor.cs`,
`Components/Pages/CommandDemoView.razor`, `ViewModels/CommandDemoViewModel.cs`,
`wwwroot/site.css` (798 → 408 lines), `wwwroot/js/layout-breakpoint-fix.js` (new).

**Tests** (new): `tests/Sample.Blazor.UI.Tests/` — registered in `I-Synergy.Framework.slnx`;
`Microsoft.Playwright` + `Microsoft.Playwright.MSTest` 1.62.0 added to `Directory.Packages.props`.

## Behaviour deliberately changed

1. **CSS modernized to v5 defaults** (user-approved) — v4 flex-layout rules deleted rather than
   ported; appearance changed intentionally. All 16 dead v4 design tokens were remapped
   (`--neutral-*`/`--accent-*` no longer exist in v5 at all).
2. **Mobile detection** now comes from v5's `FluentLayout` breakpoint handling; the
   `I-Synergy.Framework.UI.Blazor/js/layout.js` (`isDevice`) interop was removed from `MainLayout`.
3. **Custom dialog footer** via `ActionTemplate` disables the default Enter/Escape shortcuts.
4. **`ClearAfterNavigation`** emulated with `INotificationService.CloseAllMessageBarsAsync()` on
   `Navigation.LocationChanged` — closes *every* message bar, not just the one section.
5. **`/commanddemoview`** heading `<h3>` → `<h1>` (was the only page whose outline started at level 3).

## How to run the tests

```
dotnet test tests/Sample.Blazor.UI.Tests/Sample.Blazor.UI.Tests.csproj
```

Self-starting: the fixture launches the app on `http://localhost:5210`, polls for readiness, and kills
the process tree on cleanup (~90s). Set `SAMPLE_BLAZOR_BASE_URL` to target an already-running
instance instead. Requires `playwright install` on a machine that hasn't run Playwright before.
Assertions are DOM/computed-style based — there is **no** screenshot baseline, deliberately, since the
whole UI changed.

## Process notes

- `developer`/`ui-developer` frontmatter pins **haiku**; the Critical-impact dialog rework and the CSS
  rewrite were run with a `sonnet` override. The migration's correctness depended on it.
- Two stale test assertions were found encoding the *pre-fix* state (`navAreaDisplay == "none"`, and an
  `h3` locator). Both were rewritten as **stronger** statements of the real invariant — the nav-area
  check now also fails if the column retains a real box — not relaxed to force green.
- **Orchestrator error, recorded:** the mobile fix was briefly reported as "flaky" with a proposal to
  raise the workaround's ~1s circuit-wait budget. Both failing runs in fact predated the fix being on
  disk. The evidence log shows 12/12 passes since, with the wait consuming 0–1 of its 20 attempts
  (~20× headroom). No budget change was made.

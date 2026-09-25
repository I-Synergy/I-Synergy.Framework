# Fluent UI Blazor v4 → v5 Migration (Sample.Blazor)

## Goal

Migrate `samples/Sample.Blazor` from `Microsoft.FluentUI.AspNetCore.Components` v4.14.4 to v5.0.0
(package versions already bumped by the user in `Directory.Packages.props`), restoring a clean build,
correct runtime behaviour, and validated UI via Playwright.

## Scope & Blast Radius

- **In scope:** `samples/Sample.Blazor/**` only.
- **Out of scope:** `Directory.Packages.props` is already bumped and correct.
- **Verified safe:** no project under `src/` references `FluentUI` (`grep -rl FluentUI --include=*.csproj src/` → empty).
  The framework's own Blazor projects (`ISynergy.Framework.AspNetCore.Blazor`, `ISynergy.Framework.UI.Blazor`)
  build clean and wrap FluentUI only via opaque `object`/interfaces (`NavigationItem.Icon`), so **no framework changes are required**.

## Current Build State (baseline)

`dotnet build samples/Sample.Blazor/Sample.Blazor.csproj` → **4 errors, 12 warnings**

| Error | File |
|---|---|
| `RZ9991 bind-Expanded` cannot be inferred | `NavigationMenuItemControl.razor:7` |
| `RZ9991 bind-Value` cannot be inferred | `SampleDialog.razor:12` |
| `CS0246 IDialogContentComponent<>` not found | `BaseEmptyDialog.razor:12` |
| `CS0246 IDialogContentComponent<>` not found | `BaseModelDialog.razor:14` |

Plus 10 `RZ10012` warnings ("component exists but namespace not imported" — actually **removed/renamed
components silently degraded to raw HTML**). These are the real danger: a component that no longer
resolves renders as an inert custom element with no error.

## Authoritative API Sources

Migration pages fetched from the official repo (`microsoft/fluentui-blazor@dev`,
`examples/Demo/FluentUI.Demo.Client/Documentation/GetStarted/Migration/*.md`), and every claim below
cross-checked against the shipped assembly's XML docs at
`D:\Packages\microsoft.fluentui.aspnetcore.components\5.0.0\lib\net10.0\Microsoft.FluentUI.AspNetCore.Components.xml`.

> Note: `www.fluentui-blazor.net` is JS-rendered — WebFetch returns empty. Always use the GitHub raw files.

## Component / API Mapping (verified)

### Removed and replaced

| v4 (used here) | v5 | Verified in assembly |
|---|---|---|
| `FluentNavMenu` | `FluentNav` | ✔ `FluentNav` + `FluentNavBase` |
| `FluentNavGroup` | `FluentNavCategory` | ✔ |
| `FluentNavLink` | `FluentNavItem` | ✔ |
| `FluentHeader` / `FluentBodyContent` / `FluentFooter` | `FluentLayout` + `FluentLayoutItem Area="LayoutArea.X"` | ✔ `LayoutArea` enum exists |
| `FluentTextField` | `FluentTextInput` | ✔ |
| `FluentProgressRing` | `FluentSpinner` | ✔ |
| `FluentLabel` (typography use) | `FluentText` | ✔ |
| `FluentMenuProvider` | *(removed — menu service no longer used)* | ✔ absent from provider list |
| `IDialogContentComponent<T>` | `FluentDialogInstance` (abstract class) | ✔ |
| `FluentDialogHeader` / `FluentDialogFooter` | `DialogOptions.Header` / `.Footer` | ✔ |
| `DialogParameters` | `DialogOptions` | ✔ |
| `Appearance.Neutral` / `.Accent` | `ButtonAppearance.Default` / `.Primary` | ✔ |

### Parameter-level breakages found in this codebase

**`FluentOverlay`** (MainLayout.razor:41) — assembly confirms present params are
`BackgroundColor ChildContent CloseMode FullScreen Interactive Opacity Visible VisibleChanged`.
- ✗ `Transparent` — **removed** → use `Opacity="0"` if needed
- ✗ `InteractiveExceptId` — **removed** with no replacement
- ✗ `PreventScroll` — **removed**
- ⚠ `Opacity` is `int` (percent), not `double`

**`FluentMessageBarProvider`** — present params: `ClassValue MessageBarItems NotificationService Section StyleValue`
- ✗ `ClearAfterNavigation` — **does not exist anywhere in the v5 assembly** (`grep -c` → 0).
  Replacement to be confirmed (candidate: `MessageBarOptions.Lifetime`); see Open Questions.

**`FluentToastProvider`** — present params: `ClassValue NotificationService StyleValue ToastItems`
- ✗ `MaxToastCount` — **removed**. (Note: the Migration index wrongly implies Toast was removed
  entirely; `FluentToastProvider` still exists — only the `*ToastContent` types were dropped.)

**`FluentNav`** — present params: `BackgroundColor BackgroundColorHover ChildContent Density LayoutContainer
NavigationManager OnItemClick OpenedHamburgers UseIcons UseSingleExpanded Width`
- ✗ `Title`, `Collapsible`, `CollapsedChildNavigation`, `CustomToggle` — all removed
- ⚠ `Width` changed `int?` → `string?` (`250` → `"250px"`)

**`FluentNavCategory`** — present params: `ActiveClass ChildContent Expanded ExpandedChanged IconActive
IconRest StyleValue Title Tooltip`
- ✔ `Expanded`/`ExpandedChanged` retained (so `@bind-Expanded` is valid on the *new* component)
- ✗ `Gap` — removed
- ⚠ `Icon` → `IconRest` / `IconActive`

**`FluentNavItem`** — present params: `Active ActiveClass Category ChildContent Disabled Href IconActive
IconRest Match OnClick Target Tooltip`
- ⚠ `Icon` → `IconRest` / `IconActive`; `Href`/`Match`/`Target` retained

**`FluentStack`** — `HorizontalGap`/`VerticalGap` are now `string?`; v4's implicit `10px` default is gone.
Applies to `MainLayout.razor:12,15` and `BaseEmptyDialog`/`BaseModelDialog`.

**`FluentLabel`** — `Typo`, `Alignment`, `Color`, `CustomColor`, `MarginBlock` all **removed**.
Used at `BaseEmptyDialog.razor:18`, `BaseModelDialog.razor:20`, `MainLayout.razor:50` → migrate to `FluentText`.

**Theming** — `FluentDesignTheme`/`FluentDesignSystemProvider` removed; `loading-theme.js` **is not shipped
in v5** (verified: `staticwebassets/` contains no `js/loading-theme.js`). `App.razor:30-31` currently
loads a 404 script. v5 ships `IThemeService`, `Theme`, `ThemeMode`, `ThemeSettings` + `css/default-fuib.css`
and `css/reboot.css` (both confirmed present).

## File-by-File Change List

| # | File | Change |
|---|---|---|
| 1 | `Components/Controls/NavigationMenuControl.razor` | `FluentNavMenu`→`FluentNav`; drop `Title`/`Collapsible`/`CollapsedChildNavigation`/`CustomToggle`; `Width="250px"` |
| 2 | `Components/Controls/NavigationMenuItemControl.razor` | `FluentNavGroup`→`FluentNavCategory` (drop `Gap`, `Icon`→`IconRest`); `FluentNavLink`→`FluentNavItem` (`Icon`→`IconRest`). Fixes error #1 |
| 3 | `Components/Layout/MainLayout.razor` | Rebuild layout on `FluentLayout`+`FluentLayoutItem`; drop `FluentMenuProvider`; fix provider params; `FluentOverlay` params; `FluentProgressRing`→`FluentSpinner`; `FluentLabel`→`FluentText`; add `<FluentProviders>` |
| 4 | `Components/Dialogs/Base/BaseEmptyDialog.razor` | `IDialogContentComponent<T>`→`FluentDialogInstance`; remove `FluentDialogHeader`/`FluentDialogFooter`; `FluentLabel`→`FluentText`. Fixes error #3 |
| 5 | `Components/Dialogs/Base/BaseModelDialog.razor` | Same as #4 **plus** footer buttons moved to `DialogOptions.Footer`; `Appearance.*`→`ButtonAppearance.*`. Fixes error #4 |
| 6 | `Components/Dialogs/SampleDialog.razor` | `FluentTextField`→`FluentTextInput`. Fixes error #2 |
| 7 | `ViewModels/CommandDemoViewModel.cs` | `DialogParameters`→`DialogOptions`; drop `TrapFocus`/`PreventScroll`/`ShowDismiss`/`DialogType`; `Title`→`Header.Title` |
| 8 | `Components/Layout/MainLayout.razor.cs` | Verify `IDialogService.ShowInfoAsync` return shape still matches `await dialogResult.Result` |
| 9 | `Components/App.razor` | Replace dead `loading-theme.js` block with v5 theming |
| 10 | `wwwroot/site.css` | v4 design tokens → v5 (`--neutral-layer-4`→`--colorNeutralBackground*`, `--accent-fill-rest`→`--colorBrandBackground`, …); drop rules targeting removed components (`nav fluent-anchor`, `.fluent-nav-link`, `.siteheader`) |
| 11 | `Program.cs` | Confirm `AddFluentUIComponents()` overload; optional `LibraryConfiguration` for `FluentStack` gap defaults |
| 12 | `Components/_Imports.razor` | Confirm no extra `@using` needed for new namespaces |
| 13 | **NEW** `tests/Sample.Blazor.UI.Tests/` | Playwright MSTest project per `.ai/skills/playwright-tester/SKILL.md` |

## Reused Existing Types (Rule 12)

No new types are invented. The migration **reuses**: `NavigationItem`/`NavigationGroup`/`NavigationLink`
(`ISynergy.Framework.AspNetCore.Blazor.Models`), `INavigationMenuService`, `IViewModelDialog<T>`,
`ViewModelDialog<T>`, `View<T>`, `CommandBinding`, `ICommonServices.BusyService`.

> Note: `NavigationGroup.Gap` (`src/.../Models/NavigationGroup.cs:5`) becomes unused once
> `FluentNavCategory` drops `Gap`. **Decision:** leave the framework model untouched (out of scope,
> no compile impact, its constructor is used by other samples) and simply stop passing `Gap`.

## Execution Plan

Per `.ai/CLAUDE.md`, the main conversation orchestrates; code work is delegated.

1. **Delegate to `developer`** — files 1–9, 11–12 (all C#/Razor). Build after each file per the ReAct loop.
   Prompt must include the progress-file Edit instructions.
2. **Delegate to `ui-developer`** — file 10 (`site.css` token/selector migration), after step 1 compiles.
3. **Delegate to `tester`/`ui-tester`** (Playwright) — file 13: scaffold the Playwright MSTest project,
   then validate against the running app.
4. **Orchestrator (me)** — run `dotnet build`, review diffs, run Playwright suite, report.

## Validation Strategy (Playwright)

App URL from `Properties/launchSettings.json`: `https://localhost:7102` / `http://localhost:5102`.

Because the previous build **succeeded while silently degrading removed components to raw HTML**, a
green build is *not* sufficient. Playwright must assert real rendering:

- **Smoke:** `/` loads, `<h1>Hello, world!</h1>` visible, no console errors, no Blazor circuit error UI.
- **Nav:** `FluentNav` renders `fluent-nav-item` elements; clicking "Counter"/"Weather"/"Command Demo"
  navigates; nav width/collapse behaves. Regression guard: assert `nav-item` custom elements exist
  (would have caught the v4 `FluentNavMenu` degradation).
- **Counter:** `/counter` increments on click.
- **Weather:** `/weather` streams 5 forecast rows into the table.
- **Overlay/Spinner:** toggle `BusyService.IsBusy` → overlay + spinner visible; assert
  `fluent-spinner` (not `fluent-progress-ring`) is the rendered element.
- **Dialog:** `/commanddemoview` → "Open Sample Dialog" opens the dialog; `FluentTextInput` accepts
  text; Cancel/Submit behave; dialog closes.
- **Theme:** assert no 404 for `loading-theme.js` in network log.
- **Accessibility:** basic axe pass on `/` and the dialog.

## Open Questions — RESOLVED

1. **`FluentDialogInstance` content binding.** Content passes via `DialogOptions.Parameters`
   (`IDictionary<string, object?>`), keyed by the exact `[Parameter]` property name on the dialog
   component — a wrong key **throws at render time**. Reference types are shared with the caller, so
   view-model mutations stay visible. `OnActionClickedAsync(bool primary)` is **`abstract`**, so every
   dialog must override it (including `BaseEmptyDialog`, which has no footer buttons).
2. **Custom header/footer.** `FluentDialogHeader`/`FluentDialogFooter` are gone, but `FluentDialogBody`
   exposes `ChildContent`, `TitleTemplate`, `TitleActionTemplate`, `ActionTemplate` and
   `FixedHeaderFooter` — so the sample **keeps its `CommandBinding` footer buttons** rather than
   rewiring them into `OnInitializeDialog`. Trade-off: a custom `ActionTemplate` disables the default
   Enter/Escape shortcuts (those live on `DialogOptionsFooterAction.ShortCut`).
3. **`ClearAfterNavigation`** — no v5 equivalent exists (full-tree grep: 0 hits; the removal is
   undocumented). Emulated by calling `INotificationService.CloseAllMessageBarsAsync()` from
   `NavigationManager.LocationChanged`, or dropped with a comment.
4. **Theming** — `loading-theme.js` is gone and no replacement tag is needed: the RCL's collocated
   module (`...Components.lib.module.js`) auto-injects `default-fuib.css` and initialises theme
   settings via `beforeWebStart`. Runtime theming is `IThemeService` (interface-only registration —
   injecting the concrete `ThemeService` would fail) or `Blazor.theme.*` via JS.

### Additional corrections found (things the migration docs get wrong)

- `FluentProviders` has **no `ChildContent`** — the docs' `<FluentProviders>@Body</FluentProviders>`
  example does not compile. Use self-closing `<FluentProviders />` at the end of `MainLayout`, after
  `</FluentLayout>`. It renders Dialog + Toast + Tooltip + KeyCode providers but **not**
  `FluentMessageBarProvider`, which stays separate because its `Section` is required.
- `DialogOptions.Header` / `.Footer` are **get-only** — the migration table's
  `new DialogOptions { Header = ... }` sample does not compile; set members instead.
- The docs' migration index implies Toast was removed; `FluentToastProvider` still exists and is not
  `[Obsolete]` — only `FluentToast`'s content components and `MaxToastCount` went away.
- `DialogResult` has `Value`/`Cancelled` and **no `.Result`** — v4's `await dialogResult.Result` idiom
  in `MainLayout.razor.cs` is a latent compile error, not just a dialog-system change.

## Approved Decisions

1. **CSS: modernize to v5 defaults.** Adopt `FluentLayout` grid + v5 design tokens. The hand-rolled
   flex layout rules in `site.css` (`.body-stack`, `#body-content`, `.nav-expanded`, `.nav-collapsed`,
   `.navmenu`, `article.with-console`, …) are **deleted** rather than ported. Appearance will visibly
   change — this is intentional and accepted.
2. **Nav: wired-up v5 layout-level collapse.** Use `FluentLayout`'s hamburger + `FluentNav`
   (`OpenedHamburgers`), replacing the dead `#navmenu-toggle` checkbox hack. Desktop collapses to a
   rail; mobile becomes an overlay drawer.

### Consequences for the file list

- `wwwroot/site.css` — **large deletion**, not a token swap. Drop all layout/collapse/nav rules and
  the `nav fluent-anchor` / `.fluent-nav-link` / `.siteheader` blocks. Keep only genuinely
  component-agnostic styling (e.g. `.youtube-*`, `.home div[...]`) and retarget surviving tokens.
- `Components/Layout/MainLayout.razor` — structure is now the primary deliverable; `FluentLayout`
  owns header/nav/content/footer sizing (`HeaderHeight`, `FooterHeight`, `HeaderSticky`, `FooterSticky`,
  `MobileBreakdownWidth`, `NavigationDeferredLoading`, `OnBreakpointEnter` are all available).
- `Components/Layout/MainLayout.razor.cs` — the `_mobile` flag currently comes from
  `I-Synergy.Framework.UI.Blazor/js/layout.js` (`isDevice`); prefer v5's `OnBreakpointEnter` and
  retire the `#navmenu-toggle` / `_menuChecked` logic if redundant.

## Risks

- **Dialog is flagged "Critical" impact** by upstream — the largest and least mechanical change.
  `BaseModelDialog` currently owns the footer buttons via a `CommandBinding` extension; moving these
  into `DialogOptions.FooterAction.OnClickAsync` changes command wiring semantics and must be tested.
- **Silent-degradation class of bug** — mitigated by asserting rendered custom elements in Playwright.
- `FluentNav` collapse is now **layout-level** (hamburger), not component-level; the existing
  `.nav-expanded`/`.nav-collapsed` CSS toggled by `#navmenu-toggle` may no longer have a driver.

## Execution Log

### ✅ Steps 1–6, 8 — code migration COMPLETE (verified by orchestrator)

`dotnet build samples/Sample.Blazor/Sample.Blazor.csproj` → **0 errors, 0 warnings, 0 RZ10012**
(was 4 errors / 12 warnings). Independently re-run and confirmed; the diff was reviewed file-by-file
rather than accepted from the agent's summary.

9 files changed: `App.razor`, `NavigationMenuControl.razor`, `NavigationMenuItemControl.razor`,
`BaseEmptyDialog.razor`, `BaseModelDialog.razor`, `SampleDialog.razor`, `MainLayout.razor`,
`MainLayout.razor.cs`, `CommandDemoViewModel.cs`. `Program.cs` / `_Imports.razor` needed no change.

Notable runtime discovery by the implementing agent (not a cosmetic flag):
`FluentLayout`'s hamburger drawer **reuses the Navigation area's `ChildContent`**, so without
`NavigationDeferredLoading="true"` the nav renders **twice** — duplicate `id="main_menu"` and two
`FluentNav` instances, with the nav keyboard JS binding only to the first. Confirmed in rendered HTML.

Design decision: the dialog footer keeps the original `CommandBinding` wiring via
`FluentDialogBody.ActionTemplate` (rather than rewiring into `OnActionClickedAsync`). The
`DialogOptions.Parameters` key uses `nameof(BaseModelDialog<Budget, SampleDialogViewModel>.Content)`,
so an upstream rename breaks the build instead of throwing at render time.

### ✅ Step 7 — `site.css` COMPLETE

798 → 408 lines (49% deleted). 16 dead v4 tokens remapped (`--neutral-foreground-rest` →
`--colorNeutralForeground1`, `--accent-fill-rest` → `--colorBrandBackground`, `--neutral-layer-4` →
`--colorNeutralBackground2`, `--type-ramp-*` → `--fontSize*`/`--lineHeight*`, …). All v4 flex-layout
rules, the `#navmenu-toggle` checkbox hack, `.siteheader`, `nav fluent-anchor` and `.fluent-nav-link`
deleted. Three media blocks dropped (now owned by the v5 grid + `@container` queries). Verified via
CSSOM: 45 rules, 0 dead v4 tokens remaining, braces/parens balanced.

### ✅ Steps 9–10 — Playwright COMPLETE — **30/30 passing**

`tests/Sample.Blazor.UI.Tests/` — MSTest + `Microsoft.Playwright`, self-starting fixture.
Run: `dotnet test tests/Sample.Blazor.UI.Tests/Sample.Blazor.UI.Tests.csproj` (~90s).

Independent verification by the orchestrator: 30/30 confirmed by re-running the suite directly, plus
6 consecutive clean full runs from the authoring agent. Assertions are DOM/computed-style based
(no image diffing — the UI changed wholesale, so there is no baseline to compare against).

### ✅ Post-validation fixes

- **Mobile nav (F1) worked around.** `samples/Sample.Blazor/wwwroot/js/layout-breakpoint-fix.js` +
  `MainLayout.razor.cs`. **Correction to the original prescription:** a bare ResizeObserver re-fire is
  a *provable no-op* — the shipped callback notifies .NET only on a state transition, and on an
  initial mobile load the `mobile` attribute is already set, so neither branch runs. The nudge is
  kept (it re-runs the library's own detection) but the missing notification is supplied explicitly
  through the library's own idempotent `[JSInvokable]` entry point.
- **F2** — `/commanddemoview` `<h3>` → `<h1>`.
- **Two-tone nav** — canonical demo `FluentLayoutItem` parameters + one `display: flex` rule (v5.0.0
  leaves the library's own `flex: 1 1 0%` on `.fluent-nav` inert because `[area="nav"]` isn't a flex
  container). Measured: nav 191px → 816px, seam 625px → 0.
- **Two stale test assertions corrected** — both encoded the pre-fix state and were unsatisfiable
  together with the fixes. Rewritten as *stronger* statements of the real invariant, not weakened:
  the nav-area check now accepts either legal DOM shape and **fails if the column retains a real box**.

### Orchestrator error, recorded for honesty

I initially reported the mobile fix as "flaky" and proposed raising the workaround's ~1s circuit-wait
budget. That was wrong: both 3-failure runs predated the fix being on disk (the second completed while
the fix agent was still working), so the difference was pre-fix vs post-fix, not flakiness. The
evidence log confirms 12/12 passes since the workaround landed, and the budget consumed 0–1 of its 20
attempts (~20× headroom). No budget change was made.

Plan: `tests/Sample.Blazor.UI.Tests/` (matches repo convention; all 34 test projects live in
`I-Synergy.Framework.slnx`). Requires a new `Microsoft.Playwright.MSTest` entry in
`Directory.Packages.props`. The fixture will self-start the app so the suite is runnable, and the
tests must assert on **rendered `fluent-*` custom elements**, since a green build previously
coexisted with 10 silently-degraded components.

### Not yet verified by a browser

The dialog open/bind path needs a live SignalR circuit and has only been verified statically
(compiled + IL-inspected). This is the main outstanding risk — the dialog system is the
highest-impact part of the v5 migration.

# Sample.Blazor.UI.Tests

Playwright end-to-end / DOM validation suite for `samples/Sample.Blazor` (Fluent UI Blazor v5).

Created for the Fluent UI Blazor **v4 → v5** migration (`.ai/plans/fluentui-v5-migration.md`,
progress `.ai/progress/fluentui-v5-migration.md`, steps 9–10).

## Why this suite exists

The v4 → v5 migration moved or renamed a set of components (`FluentNavMenu` → `FluentNav`,
`FluentNavLink` → `FluentNavItem`, `FluentHeader`/`FluentBodyContent`/`FluentFooter` → `FluentLayout`
+ `FluentLayoutItem`, `FluentProgressRing` → `FluentSpinner`, `FluentTextField` → `FluentTextInput`,
`FluentMenuProvider`/`FluentDialogHeader`/`FluentDialogFooter`/`loading-theme.js` removed).

An unresolved component tag does **not** break the build: it renders as an inert custom element and
the page still returns HTTP 200. A green build therefore proves nothing. Every test here asserts on
the **rendered DOM** (class names, custom elements, attributes, computed styles), not on text alone.

## Running

```powershell
# The suite starts and stops the app itself (dotnet run on http://localhost:5210).
dotnet test tests/Sample.Blazor.UI.Tests/Sample.Blazor.UI.Tests.csproj
```

* The first run is slow (Playwright driver + Chromium warm-up); later runs take about a minute.
* To run against an app you started yourself, set `SAMPLE_BLAZOR_BASE_URL` (no process is started or
  stopped then):
  ```powershell
  $env:SAMPLE_BLAZOR_BASE_URL = "http://localhost:5210"
  ```
* Browsers are installed by the `InstallPlaywrightBrowsers` MSBuild target after build. The pinned
  `Microsoft.Playwright` version expects `chromium-1234` under
  `%USERPROFILE%\AppData\Local\ms-playwright`.
* Every run appends measurements of *all* tests (including passing ones) to
  `bin/Debug/net10.0/evidence.log`, and the app host's console output to `bin/Debug/net10.0/app-host.log`.

## Structure

| File | Covers |
|---|---|
| `Infrastructure/SampleAppHost.cs` | assembly fixture: starts/stops `Sample.Blazor` on `http://localhost:5210` |
| `Infrastructure/SamplePageTest.cs` | browser/context/page lifecycle, console + network diagnostics, circuit-ready wait |
| `Infrastructure/SampleDom.cs` | the v5 DOM contract (selectors) in one place |
| `SmokeTests.cs` | hand-rolled app shell (`.app-shell` + header/sidebar/content/sidebar-footer), providers, dead `loading-theme.js`, per-route baseline |
| `NavigationTests.cs` | `nav.app-nav` rendering, single nav instance, active item, click-through |
| `PageInteractionTests.cs` | counter increment, streamed weather table |
| `DialogTests.cs` | dialog open, `FluentTextInput`, header title, two-way binding, Cancel |
| `BusyOverlayTests.cs` | busy overlay hidden by default, `FluentSpinner` (not `FluentProgressRing`) |
| `ResponsiveLayoutTests.cs` | mobile off-canvas drawer (toggle / scrim / navigate from the drawer), runtime breakpoint switch, permanent desktop sidebar, sidebar measurements |
| `AccessibilityTests.cs` | accessible names (shadow DOM aware), heading order, computed contrast |

## Deliberate non-inheritance

The test classes do **not** derive from `Microsoft.Playwright.MSTest.PageTest`. That type was built
against MSTest v2 (`Microsoft.VisualStudio.TestPlatform.TestFramework`) while this project uses
MSTest 4.x (`MSTest.TestFramework.dll`). MSTest matches attributes by exact type identity, so the
inherited `[TestInitialize]`/`[TestCleanup]` would be silently ignored (build warning MSTEST0082) and
`Page` would never be created. `SamplePageTest` owns the lifecycle explicitly instead.

## Notes on the DOM

* The shell is **hand-rolled**, not `FluentLayout`: `div.app-shell` &gt; `aside.app-sidebar` (with
  `nav.app-nav#main_menu`) + `header.app-header` + `main.app-content`. Above 900px the sidebar is a
  permanent 300px column and `button.app-nav-toggle` is `display:none`; at 900px and below the same
  sidebar becomes a fixed 280px off-canvas drawer that the toggle slides in (`is-open`), with
  `div.app-nav-scrim` rendered behind it.
* What is still Fluent UI — `FluentProviders` and the dialog/toast/tooltip providers — renders
  **plain elements with `fluent-*` CSS classes** (`.fluent-providers`, `.fluent-dialog-provider`), not
  custom elements.
* `FluentSpinner`, `FluentOverlay`, `FluentButton`, `FluentTextInput`, `FluentDrawer`, `FluentDialog`
  render **real custom elements** whose content lives in shadow DOM. Their host element frequently has
  a zero sized box, so "is it visible" must be asserted on the content, not on the host.
* Tests assert with Playwright locators (`Expect(...).ToBeVisibleAsync()`), never with screenshot
  comparison — the migration intentionally changed the entire visual appearance, so there is no
  meaningful baseline image.

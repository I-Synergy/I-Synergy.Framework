# Blazor sample: mobile navigation drawer + re-baselined UI tests

Status: DONE
Started: 2026-10-26
Plan: `.ai/plans/blazor-shell-mobile-drawer.md`

## Steps

- [x] Implement the drawer in `MainLayout.razor`, `MainLayout.razor.cs`, `wwwroot/site.css`
- [x] Re-baseline `SampleDom` and the 16 stale assertions
- [x] Run the suite until green; verify the drawer by hand at 1280px and 480px

## What changed

Sample (`samples/Sample.Blazor`):
- `MainLayout.razor` — `button.app-icon-button.app-nav-toggle` as the first header child
  (`title`/`aria-label` "Navigation", `aria-expanded`, `aria-controls="main_menu"`), sidebar carries
  `is-open` while open, `nav.app-nav` regains `id="main_menu"` + `role="navigation"`, the four nav
  links close the drawer on click, `div.app-nav-scrim` renders only while open.
- `MainLayout.razor.cs` — `_navOpen`, `ToggleNavigation()`, `CloseNavigation()`, and the drawer closes
  from the existing `NavigationManager_LocationChanged` handler.
- `wwwroot/site.css` — the 72px icon rail at `<=900px` is replaced by an off-canvas 280px drawer
  (`translateX(-100%)` → `0` on `.is-open`, shadow), `.app-nav-toggle` hidden above 900px, scrim
  hidden above 900px; the breadcrumb grows in the now three-child header.

Tests (`tests/Sample.Blazor.UI.Tests`):
- `SampleDom` re-pointed at `.app-shell` / `.app-header` / `.app-sidebar` / `.app-content` /
  `.app-sidebar-footer` / `nav.app-nav` / `.app-nav-item`; the removed FluentLayout markup stays as
  "must not exist" guards.
- `SamplePageTest.WaitForLayoutInitialisedAsync` → `WaitForShellReadyAsync` (the responsive behaviour
  is pure CSS now, so there is no JS state to wait for).
- Navigation, Smoke, Responsive and Accessibility tests re-baselined; two obsolete premises were
  repurposed with comments: the desktop hamburger collapse (the design has none) and the FluentLayout
  JS breakpoint race (now a runtime CSS breakpoint switch). No test deleted, skipped or ignored.
- Accessibility contrast sweep now alpha-composites the ancestor background chain (the active item's
  `rgba(94,211,176,0.16)` tint was being measured as opaque) and reports `aria-hidden` decorative
  glyphs separately instead of failing them at the text floor; a `findings.Count >= 10` guard keeps
  the sweep from going vacuous.

## Verification

- `dotnet test tests/Sample.Blazor.UI.Tests` → `Failed: 0, Passed: 36, Skipped: 0` (was 16 failed;
  build 6175's pre-rewrite baseline was also 36/36).
- `dotnet build samples/Sample.Blazor` clean.
- Browser at 1280×800: permanent static 300px sidebar, toggle `display:none`, no scrim.
  At 480×800: toggle visible, sidebar fixed at x=-280; toggle → `is-open`, scrim, x=0 (screenshot);
  scrim click → closed again.

## Known, deliberately not changed

The dark theme paints the active nav item's decorative icon glyph `var(--app-accent)` (#0066CC) over
stacked accent tints (~1.87:1). The glyph is `aria-hidden` and duplicates its label, so it is outside
the WCAG 1.4.3 text floor the sweep enforces; if it should meet 3:1, change
`.app-nav-item.active .app-nav-icon` in `site.css`.

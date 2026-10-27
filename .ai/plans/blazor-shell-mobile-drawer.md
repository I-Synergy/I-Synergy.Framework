# Blazor sample: mobile navigation drawer + re-baselined UI tests

**Status:** APPROVED (user selected "Give the new shell a mobile drawer, then update the tests")
**Scope:** `samples/Sample.Blazor` (layout + CSS) and `tests/Sample.Blazor.UI.Tests` — sample only,
no framework code.

## Problem

`cbd17689` replaced the FluentLayout shell with a hand-rolled `.app-shell` / `.app-sidebar` /
`nav.app-nav` shell. Below 900px the sidebar shrinks to a 72px icon rail, so a phone user has no
navigation affordance at all, and 16 of the 36 Playwright tests still assert the removed
FluentLayout DOM (`.fluent-layout`, `nav.fluent-nav`, `#main_menu`, hamburger drawer).

## Shell contract to implement

- Header gains `button.app-icon-button.app-nav-toggle` (hamburger, `aria-label`/`title` from
  `L("Navigation")`, `aria-expanded` reflecting state). Hidden above 900px.
- Sidebar `<aside class="app-sidebar" aria-label=...>` gains `is-open` while the drawer is open.
  At `<=900px` it is `position: fixed` off-canvas (`translateX(-100%)`), 280px wide, sliding in on
  `is-open`; above 900px it stays the permanent 300px column. The 72px icon-rail rules are removed.
- `div.app-nav-scrim` rendered while open (mobile only in CSS) and closes on click.
- Navigation stays `nav.app-nav` **with `id="main_menu"`** — the single-copy guard the tests use.
- Clicking any nav link closes the drawer; the settings drawer keeps its existing pattern.

## Test contract

`SampleDom` points at `.app-shell`, `.app-header`, `.app-sidebar`, `.app-content`,
`.app-sidebar-footer`, `nav.app-nav`, `.app-nav-item`, `.app-nav-toggle`, `.app-nav-scrim`,
`#main_menu`. The 16 failing tests are re-baselined to the new shell: desktop asserts a permanent
column and no toggle; phone asserts toggle + off-canvas drawer + scrim + close-on-navigate. No test
is deleted, skipped or weakened into a no-op.

## Steps

- [ ] Implement the drawer in `MainLayout.razor`, `MainLayout.razor.cs`, `wwwroot/site.css`
- [ ] Re-baseline `SampleDom` and the 16 stale assertions
- [ ] Run the suite until green; verify the drawer by hand at 1280px and 480px

## Verification

`dotnet build samples/Sample.Blazor`, then `dotnet test tests/Sample.Blazor.UI.Tests` repeatedly until
36/36 (the fixture starts the app itself), plus a manual check of the drawer in the browser.

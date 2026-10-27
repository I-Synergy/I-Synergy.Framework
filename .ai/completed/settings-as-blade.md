# Settings as a blade view in every UI head

**Status:** DONE
Started: 2026-10-25
Completed: 2026-10-25
Plan: `.ai/plans/settings-as-blade.md`

## Steps

- [x] 1. Shared `BladeHostViewModel` + WPF/MAUI DI registration
- [x] 2. WPF `BladeHostView`, settings converted to a blade, shell opens it
- [x] 3. Build and smoke-run `Sample.WPF`
- [x] 4. MAUI settings blade view; remove drawer, drawer state and the stale TabbedView demo
- [x] 5. MAUI `BladeHostView` in the four tab pages + shell opens the blade
- [x] 6. Build and smoke-run `Sample.Maui`
- [x] 7. WinUI verification (settings blade already exists)
- [x] 8. Tests: builds for all three UI libraries + `ISynergy.Framework.UI.Tests` + `git diff --check`
- [x] 9. Docs: readmes, CHANGELOG, session handoff

## Notes

- Scope approved: WPF + WinUI + MAUI. Blazor out of scope (no blade infrastructure).
- MAUI presentation: in-page `BladePane` replacing the per-page settings drawer.
- `BladeHostViewModel` lives in `ISynergy.Framework.UI/ViewModels`.

## Result

- Shared: `BladeHostViewModel` (`ViewModelBladeView<object>`, no items) registered as a singleton in
  `WPFAppBuilderExtensions` and `MauiAppBuilderExtensions`.
- WPF: `Controls/BladeHostView` (resolves the host from DI, collapses + traces when unresolvable); the
  sample's shell hosts it above the content frame and opens `SettingsViewModel` through `IBladeService`.
- MAUI: `Controls/BladeHostView` (lazy resolution in the constructor, `Loaded` and `OnParentSet`; attaches
  `BladePane` with `ItemsSource = Blades`) declared last in `InfoView`, `CustomControlsView`,
  `MauiControlsView` and `SlideShowView`; the sample's Settings is a blade page and the drawer,
  `SettingsDrawerState` and their registration are gone. Phone idiom still presents it modally.
- WinUI: unchanged — the shell already owns the pane and Settings was already a blade.
- Verified: clean builds for `ISynergy.Framework.UI`, `UI.WPF`, `UI.Maui`, `UI.WinUI`, `Sample.WPF`,
  `Sample.Maui`, `Sample.WinUI` (only pre-existing `CsWinRT1032`/`CS8601` warnings), 90/90 framework UI
  tests, `git diff --check` clean. Documentation updated in the three readmes, `screenshots/README.md` and
  `CHANGELOG.md`.
- NOT runtime-verified: pane visuals/scrim/animation, the MAUI phone modal branch, RTL rendering and
  settings live-apply on screen.

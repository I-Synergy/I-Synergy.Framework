# Blade pane parity (WPF + MAUI + WinUI RTL)

**Status:** DONE
Started: 2026-10-25
Completed: 2026-10-25
Plan: `.ai/plans/blade-pane-parity.md`

## Steps

- [x] 1. Shared `BladePaneAnchor` + `BladePlacement` helper with unit tests
- [x] 2. WPF `BladeView` host (scrim, stack, Frame-hosted blades, dim/disable, RTL anchoring)
- [x] 3. WPF sample wiring (`Sample.WPF` shell host + blade view models)
- [x] 4. MAUI `BladePane` host + `IBladePaneHost` accessor
- [x] 5. MAUI `BladeService` (adaptive: modal on phone, overlay otherwise) + DI registration
- [x] 6. MAUI sample wiring (`Sample.Maui` blade-owning pages)
- [x] 7. WinUI RTL fix (`BladeView` anchoring, XAML `FlowDirection`, shadow/border mirroring)
- [x] 8. Selection views for WPF and MAUI (`SingleSelectionView`/`MultipleSelectionView`)
- [x] 9. Docs: readmes, CHANGELOG, session handoff

## Result

- Shared: `BladePaneAnchor` (`Auto`/`Start`/`End`) and `BladePlacement` (`Resolve`, `IsStartSide`,
  `HorizontalSign`); 22 tests in `tests/ISynergy.Framework.UI.Tests/Helpers/BladePlacementTests.cs`, all pass.
- WPF: `Controls/BladeView` (`ItemsSource`, `Anchor`, `DisabledOpacity`, `BladeSpacing`, `BladeMaxWidth`,
  `InnerPadding`), `Page` blades hosted in a `Frame`, positioning container forced LTR; `BladeService`
  resolves the selection view models.
- MAUI: `Controls/BladePane` (renders `ContentPage.Content`), `IBladePaneHost`/`BladePaneHostRegistry`,
  adaptive `BladeService` (overlay on tablet/desktop with a host attached, modal on phone, overlay
  fallback with a warning).
- WinUI: `Anchor`/`BladeAlignment` on `BladeView`, root/window-content XAML `FlowDirection`, native
  `WS_EX_LAYOUTRTL` limited to chrome, `ApplyFlowDirection(string?)`.
- Docs: `readme.md` for UI.WPF/UI.Maui/UI.WinUI, `CHANGELOG.md`, `.ai/session-context.md`.

## Verified

- Clean builds: UI.WPF `net10.0-windows10.0.26100.0`, UI.Maui `net11.0-windows10.0.26100.0`,
  UI.WinUI `net10.0-windows10.0.26100.0`, Sample.WPF `net11.0-windows`, Sample.Maui `net11.0-windows`,
  Sample.WinUI `net11.0-windows` x64 (pre-existing warnings only).
- `BladePlacement` tests: 22/22 pass.

## Not Done

- Visual RTL (`ar-SA`) checks on all three heads, including whether `WS_EX_LAYOUTRTL` and XAML
  `FlowDirection` double-mirror.
- Phone-idiom modal blade flow and selection-blade interaction at runtime.
- `screenshots/` captures in LTR and RTL with two stacked blades.

## Notes

- Reference implementation: `src/ISynergy.Framework.UI.WinUI/Controls/BladeView/BladeView.xaml(.cs)`.
- RTL rule: anchor edge is resolved through `BladePlacement`, never hardcoded.
- Builds must use `MSBUILDDISABLENODEREUSE=1` and `-nodeReuse:false`.

# Remove theme/language dialogs from all UI heads

**Status:** DONE
Started: 2026-10-25
Completed: 2026-10-25

## Steps
- [x] Remove shared view models, window interfaces, and old tests
- [x] Remove platform windows and the MAUI theme radio-button converter
- [x] Rewrite the three `BaseShellViewModel` heads (ThemeCommand = toggle, no language dialog)
- [x] Update Sample.WPF (new settings view + view model, remove dialogs overrides)
- [x] Update Sample.WinUI (blade settings, remove language dialog/nav item)
- [x] Update Sample.Maui (drop theme/language overrides, header uses ThemeCommand)
- [x] Update readmes / CHANGELOG
- [x] Build all affected projects and run the UI tests

## Notes
Plan: `.ai/plans/remove-theme-language-dialogs.md`

- `ThemeCommand` now toggles light/dark through `IThemeService` in WPF, WinUI and MAUI.
- Language, theme and accent selection live in each app's settings surface
  (WPF: new `SettingsView`/`SettingsViewModel`; WinUI: settings blade; MAUI: settings drawer).
- Blazor was untouched — it never had the removed types.
- Verification: framework heads + all three samples build; `ISynergy.Framework.UI.Tests` 68/68.

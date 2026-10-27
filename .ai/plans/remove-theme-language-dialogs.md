# Remove theme/language dialogs from all UI heads

Status: APPROVED (user direction + answers)
Started: 2026-10-25

## Goal

Language and theme handling moves to the Settings command; the Theme command becomes a
light/dark switcher. `ThemeViewModel`, `LanguageViewModel` and their dialog views are removed
from the shared UI project and every platform head (WPF, WinUI, MAUI). Blazor has no such
types and is left unchanged.

## Decisions

- `ThemeCommand` toggles light/dark via `IThemeService` in every head. No accent picker there.
- `LanguageCommand` / `OpenLanguageAsync` are removed; language selection lives in Settings.
- WinUI Settings opens the existing `SettingsViewModel` blade.
- MAUI Settings keeps the existing settings drawer.
- Sample.WPF gains a minimal `SettingsView` + `SettingsViewModel` (language, theme, accent).
- Blazor sample is untouched.

## Breaking change

Public API removed from the framework:

- `ISynergy.Framework.UI.ViewModels.ThemeViewModel`
- `ISynergy.Framework.UI.ViewModels.LanguageViewModel`
- `ISynergy.Framework.UI.Abstractions.Windows.IThemeWindow`
- `ISynergy.Framework.Mvvm.Abstractions.ViewModels.ILanguageViewModel`
- `ISynergy.Framework.Mvvm.Abstractions.Windows.ILanguageWindow`
- `WPF/Windows/ThemeWindow`, `WinUI/Windows/{Theme,Language}Window`,
  `MAUI/Windows/{Theme,Language}Window`, `MAUI/Converters/RadioButtonToThemeConverter`
- `BaseShellViewModel.LanguageCommand` in all three heads
- `BaseShellViewModel` constructors gain an `IThemeService` parameter

## Steps

- [ ] Remove shared view models, window interfaces, and old tests
- [ ] Remove platform windows and the MAUI theme radio-button converter
- [ ] Rewrite the three `BaseShellViewModel` heads (ThemeCommand = toggle, no language dialog)
- [ ] Update Sample.WPF (new settings view + view model, remove dialogs overrides)
- [ ] Update Sample.WinUI (blade settings, remove language dialog/nav item)
- [ ] Update Sample.Maui (drop theme/language overrides, header uses ThemeCommand)
- [ ] Update readmes / CHANGELOG
- [ ] Build all affected projects and run the UI tests

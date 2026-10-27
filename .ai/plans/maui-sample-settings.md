# Sample.Maui: language, theme and accent handled like The Quran

Status: APPROVED (option a, drawer)

## Goal
Replace the modal Language/Theme dialogs and the restart-on-change flow in `samples/Sample.Maui` with the approach used in
`D:\Projects\International Intelligence\The Quran\src\TheQuran.App`: settings applied at startup, theme and accent applied live.

## How The Quran does it (reference)
- `App.ApplySavedLanguage()` (App.xaml.cs): read `LocalSettings.Language`, set `CultureInfo.DefaultThread*` and `Current*` at the top of `InitializeApplicationAsync`, before any resource is read. Optional env var override for test runs.
- Theme: `ShellViewModel.ApplyTheme` sets `Application.UserAppTheme` (Light/Dark), persists `LocalSettings.Theme`; applied on `ShellLoadedAsync` via `ApplySavedTheme()`.
- Accent: static `AccentPalette.Apply(accent[, theme])` derives all accent tokens from one base colour and writes them into `Application.Resources`; applied in `InitializeApplicationAsync` on the main thread before the first page renders.
- Language/theme/accent live in an inline settings surface (SettingsDrawer), not in modal dialogs; language changes need a restart only for fully re-resolved strings.
- `RestartApplication` is not on the language-submit path.

## Existing types reused
`BaseShellViewModel` (Language/Theme commands), `LanguageViewModel`, `ThemeViewModel`, `ILanguageWindow`/`IThemeWindow`, `LocalSettings` (Language/Theme/Color), `ISettingsService`, `CultureInfo.SetLocalizationLanguage()`, `Themes` enum. Nothing duplicated.

## Steps
1. `samples/Sample.Maui/Services/AccentPalette.cs` - port of The Quran's palette helper, using the sample's existing `Color` setting (string) and `Resources/Styles/Colors.xaml` keys.
2. `App.xaml.cs` - `ApplySavedLanguage()` plus `AccentPalette.Apply` on the main thread at the start of `InitializeApplicationAsync`.
3. `ViewModels/ShellViewModel.cs` - override `OpenLanguageAsync` and `OpenThemesAsync`: apply the culture, theme (`UserAppTheme`) and accent live and save, with no restart prompt for theme/accent; apply saved theme in `ShellLoadedAsync`.
4. `Views/ShellView.xaml` footer - Language/Theme entries open the inline settings surface (flyout page or panel) instead of a modal window; decision needed, see question.
5. Build `net10.0-windows` and `net11.0-windows` (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`), run, verify with UI Automation that the nav pane survives language/theme changes.

## Open question
Inline settings surface: (a) a drawer/panel like The Quran's SettingsDrawer, or (b) keep the framework's modal dialogs but apply the choice live (no restart) as in steps 2-3. (b) is smaller; (a) is the full Quran idea.

## Not in scope
Framework (`src/`) changes. Whether the original Submit failure is caused by building with SDK 11 is still untested; it should be checked first (swap `global.json` back to `.ai/tmp/global.json.orig`).

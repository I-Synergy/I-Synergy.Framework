# Sample.Maui settings drawer (language, theme, accent) like The Quran
Status: IN PROGRESS
Started: 2026-10-23

## Steps
- [x] Create Models folder with LocalSettings.cs (Language, Theme, Color/AccentColor)
- [x] Port AccentPalette to samples/Sample.Maui/Services
- [x] App.xaml.cs: ApplySavedLanguage + accent at startup
- [x] SettingsDrawer control (language, theme, accent) + code-behind
- [x] SettingsDrawerState service
- [x] ShellViewModel: override OpenLanguageAsync, OpenThemesAsync to apply live + open drawer
- [x] SettingsViewModel: add LanguageOptions and ThemeOptions collections
- [x] InfoView: add SettingsDrawer overlay
- [x] MauiProgram: register SettingsDrawerState
- [x] Build net10.0-windows and net11.0-windows
- [x] Run + verify nav pane survives (UI Automation)

## Notes

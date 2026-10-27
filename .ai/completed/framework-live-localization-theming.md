# Framework live localization + theming
Status: DONE
**Status:** DONE (2026-10-23)
Started: 2026-10-23

## Steps
- [x] 1. Globalization: add `LocalizedStrings`; hook `ApplyCulture()`; unit tests
- [x] 2. `IThemeService`: add members; update MAUI `ThemeService`
- [x] 3. Update WinUI and WPF `ThemeService`
- [x] 4. Blazor: `ThemeService` + localized component helper + DI registration
- [x] 5. MAUI: `LocalizedStringsExtension`; migrate Sample.Maui, delete `AccentPalette`/`LocalizedStrings`
- [x] 6. Wire samples for WPF / WinUI / Blazor
- [x] 7. Build, test, docs

## Notes
- Verified: Sample.Maui (windows), Sample.WPF, Sample.Blazor build 0 errors; Globalization tests 122/122; Mvvm tests 262 passed, 2 skipped.
- Not verified: Android/iOS/MacCatalyst builds; Sample.WinUI built by agent only (needs -p:RuntimeIdentifier=win-x64); live accent change at runtime; full solution tests.

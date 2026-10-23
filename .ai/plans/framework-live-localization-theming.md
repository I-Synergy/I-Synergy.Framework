# Move live localization + theming into the framework

Status: APPROVED 2026-10-23 — extend `IThemeService` (open question 1 resolved)
Date: 2026-10-23

## Goal
Language and theme/accent switching at runtime (no restart) is a framework feature usable from
WPF, WinUI, Blazor and MAUI. Today it lives in `samples/Sample.Maui` (`LocalizedStrings`,
`AccentPalette`, and the theme-toggle/apply logic in `ShellViewModel` / `SettingsDrawer`).

## Findings
- `ILanguageService` / `LanguageService` (Globalization) resolve strings but raise **no change
  notification**, so bound UI cannot refresh after `CultureInfo.ApplyCulture()`.
  `GetStringExtension` (MAUI) resolves once and never updates.
- `IThemeService` (Mvvm) only has `IsLightThemeEnabled` + `ApplyTheme()`. `ThemeService` exists for
  MAUI, WinUI and WPF; it reads theme/accent once in the constructor and cannot change them live.
  Blazor has no theme service.
- `Sample.Maui/Services/AccentPalette.cs` duplicates what `ThemeService` + `SetApplicationColor`
  (`DynamicThemeResourceDictionary`, `ThemeColorCalculator`) already do for the accent.
- `Sample.Maui/Services/LocalizedStrings.cs` is a thin bindable wrapper over `LanguageService.Default`.

## Design

### Localization (Globalization lib, platform-neutral)
- New `LocalizedStrings : INotifyPropertyChanged` in `ISynergy.Framework.Globalization`:
  indexer `this[string key]`, `Refresh()` raising `Item[]`, plus a plain `Changed` event for Blazor.
  Additive — `ILanguageService` itself is **not** changed (avoids breaking implementers).
- `CultureInfo.ApplyCulture()` (existing extension in Globalization) calls `Refresh()` on the
  default instance, so apps no longer call it by hand.
- Per-platform consumption:
  - MAUI: `LocalizedStringsExtension` / documented `{Binding [Key], Source=...}`.
  - WPF / WinUI: indexer binding (both honor `Item[]`).
  - Blazor: `LocalizedComponentBase` (or small helper) subscribing to `Changed` -> `StateHasChanged`.
- RTL `FlowDirection` switch stays app-side for now (out of scope; note in docs).

### Theming
- Extend `IThemeService` with: `Themes Theme`, `string AccentColor`, `event EventHandler? ThemeChanged`,
  `void SetTheme(Themes)`, `void SetAccentColor(string hex)`. Setters apply live, persist via
  `ISettingsService.LocalSettings` + `SaveLocalSettings()`, and raise `ThemeChanged`.
  `ApplyTheme()` stays and applies the persisted values at startup.
- Implement in the existing MAUI / WinUI / WPF `ThemeService` (reuse `SetApplicationColor`).
- New Blazor `ThemeService` in `ISynergy.Framework.UI.Blazor`: sets `data-theme` and the accent CSS
  variables via JS interop (persisted in `LocalSettings`), registered in its `ServiceCollectionExtensions`.
- Sample.Maui: delete `AccentPalette` and `LocalizedStrings`; header toggle + drawer call
  `IThemeService` / `LocalizedStrings` from the framework. Sample WPF/WinUI/Blazor wired minimally.

## Reused types (no duplicates)
`ILanguageService`, `LanguageService`, `ApplyCulture` extension, `IThemeService`, the three existing
`ThemeService` classes, `ThemeColorOptions`, `ThemeColorCalculator`, `DynamicThemeResourceDictionary`,
`ISettingsService`, `Themes` / `ThemeColors`, `GetStringExtension`.

## Steps
- [ ] 1. Globalization: add `LocalizedStrings`; hook `ApplyCulture()`; unit tests
- [ ] 2. `IThemeService`: add members; update MAUI `ThemeService` (live set + persist + event)
- [ ] 3. Update WinUI and WPF `ThemeService` the same way
- [ ] 4. Blazor: `ThemeService` + localized component helper + DI registration
- [ ] 5. MAUI: `LocalizedStringsExtension`; migrate Sample.Maui, delete `AccentPalette`/`LocalizedStrings`
- [ ] 6. Wire samples for WPF / WinUI / Blazor (language + theme switch)
- [ ] 7. Build (MSBUILDDISABLENODEREUSE=1, `-nodeReuse:false`), `dotnet test`, docs/readmes

## Risks / open questions
1. **Changing `IThemeService`** breaks any external implementer. All three implementers are in this
   repo; I recommend extending it. Alternative: a new `IThemeSettingsService` (non-breaking, one more type).
2. Live accent change relies on `SetApplicationColor` working after startup on each platform — verify in step 2/3,
   and fall back to the sample's direct resource-dictionary write if not.
3. WinUI / WPF / Android / iOS targets cannot all be built on this machine; step 7 verifies what it can.

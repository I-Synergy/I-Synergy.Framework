# WPF sample starts in the wrong theme

**Status:** DONE
Started: 2026-10-26
Completed: 2026-10-26

## Symptom
The WPF sample started with light surfaces while the shell's theme toggle behaved as if the dark
theme were active. It took two toggles to reach the dark theme.

## Root cause
`Themes.Default` (persisted value `0`, written by older builds or any consumer that does not pick
light/dark) was interpreted differently by each side:

- `Sample.WPF/App.ApplySampleTokens` used `Theme == Themes.Dark`, so `Default` painted the **light**
  design tokens.
- `BaseShellViewModel.ToggleThemeAsync` used `Theme == Themes.Light ? Dark : Light`, so `Default`
  was treated as **dark**: the first click selected Light (no visual change) and only the second
  click produced Dark.
- `ThemeService.IsLightThemeEnabled` and the shell icon (`GetThemeIcon`) also treated `Default` as dark.

The framework's `DynamicThemeResourceDictionary` had a second, related gap: it always used the dark
end of the gray ramp for `TintedBackground/Surface/Control/Disabled/Border/SecondaryBrush`, so
framework controls never followed a light-theme switch.

## Fix
- `ISynergy.Framework.UI.WPF/Services/ThemeService`: `Theme` resolves `Themes.Default` from the
  Windows `AppsUseLightTheme` personalization value (light when unreadable); `IsLightThemeEnabled`
  derives from the resolved theme; `ApplyTheme` passes the resolved theme to the resource dictionary;
  `SetTheme` still persists an explicit choice made while the resolved theme already matches.
- `ISynergy.Framework.UI.WPF/Styles/DynamicThemeResourceDictionary` +
  `Extensions/ApplicationExtensions`: the dictionary takes a `Themes` argument and derives semantic
  brushes from the matching end of the gray ramp (dark 950/900/600/500, light 000/100/200/300).
- `Sample.WPF`: design tokens use `IsLightThemeEnabled`; the shell icon uses
  `IsLightThemeEnabled`; the settings blade picker initializes from `IThemeService.Theme` (resolved)
  instead of the raw persisted value.

## Verification
- Reproduced on the real sample (settings `"Theme":0`, OS in dark app mode): before the fix the app
  rendered light with the dark-mode icon and one click changed nothing; after the fix it renders dark
  with the dark-mode icon and one click switches to light, persisting `"Theme":1`.
- Restart with an explicit `"Theme":2` renders dark with the matching icon; changing the theme from
  the settings blade renders both the shell and the blade in the new theme.
- `dotnet test tests/ISynergy.Framework.UI.WPF.Tests` — 27/27 (new: 3 theme-dictionary tests,
  4 theme-service tests).
- `dotnet build samples/Sample.WPF` — 0 warnings, 0 errors (Debug, net11.0-windows, node reuse disabled).

## Notes
- The WPF head has no "follow system at runtime" subscription: with `Themes.Default` the OS theme is
  resolved per read, but an OS theme change while the app runs does not raise `ThemeChanged`.
- WinUI and MAUI keep their existing `Theme` semantics; only the WPF head resolves `Default`.

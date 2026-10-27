# Unify sample styling with the MAUI design language

Status: DONE
Started: 2026-10-25
Completed: 2026-10-25

## Goal

Every sample (WPF, WinUI, MAUI, Blazor) should share the same look and feel as the MAUI sample.
Menu items stay platform-specific; the settings surfaces and design tokens are shared.

## Decisions

- Scope: settings surfaces + shared design tokens (not every screen, not shell menu structure).
- Blazor is included.
- Figtree is bundled in WPF, WinUI and Blazor.
- Default accent is the MAUI teal `#0B5D55`.

## Work

- Shared tokens: surfaces, borders, text, muted text, accent and ink; mirrored from
  `Sample.Maui/Resources/Styles/Colors.xaml` into WPF `App.xaml`, WinUI
  `DynamicThemeResourceDictionary` and Blazor `site.css`.
- Typography: Figtree Regular/Medium/SemiBold/Bold bundled in all four samples.
- Settings surfaces: same header, labels, language/theme selectors, four accent swatches and
  primary Close button; WPF page, WinUI blade, MAUI drawer and Blazor drawer.
- Live apply: language (ApplyCulture), theme (IThemeService.SetTheme), accent
  (IThemeService.SetAccentColor) on every platform.
- Fixes found while wiring it up: WinUI `BladeView` host, Blazor `IThemeService` registration,
  Blazor live theme re-application and interop path.

## Verification

- Builds: Sample.WPF, Sample.WinUI (arm64), Sample.Maui (windows), Sample.Blazor.
- Tests: `ISynergy.Framework.UI.Tests` pass.
- Screenshots: `screenshots/{wpf,winui,maui,blazor}` + `screenshots/README.md`.

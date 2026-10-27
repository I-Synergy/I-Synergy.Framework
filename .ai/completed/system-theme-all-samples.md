# System theme across all sample apps

**Status:** DONE
Started: 2026-10-26
Completed: 2026-10-26

## Goal
`Themes.Default` means "System" everywhere: each head resolves it to Light/Dark, applies the
platform's follow-the-OS mode, reacts to OS changes while running, and every sample settings surface
offers System (a fresh install starts on it).

## What changed
### Framework
- **WPF** — `Theme` resolves `Default` from `AppsUseLightTheme`; watches
  `SystemEvents.UserPreferenceChanged`, re-applies on the UI thread and raises `ThemeChanged`;
  `DynamicThemeResourceDictionary`/`SetApplicationColor` take the theme (light/dark `Tinted*` brushes);
  `IDisposable` releases the subscription. New `EnumToDescriptionConverter`.
- **WinUI** — `Default` maps to `ElementTheme.Default`; the resolved theme is seeded from
  `Application.RequestedTheme` and refreshed from the window root's `ActualThemeChanged`; title bar
  and `ThemeChanged` follow. `ApplyTheme` tolerates a missing WinUI runtime (unit-test hosts).
  `OnLaunched` applies the stored preference so `Default` is not pinned.
- **MAUI** — `Default` now maps to `AppTheme.Unspecified` (previously `Dark`); resolves from
  `Application.PlatformAppTheme`; watches `Application.RequestedThemeChanged`.
- **Blazor** — the interop module resolves `system` via `matchMedia`, watches the query, re-applies and
  calls `[JSInvokable] OnSystemThemeChanged`; `Theme`/`IsLightThemeEnabled` report the cached resolved
  theme. The sample's `App.razor` applies the persisted/System theme in an inline head script so the
  first paint is not light.

### Samples
- `Sample.Shared/Models/LocalSettings.cs` defaults to `Themes.Default`.
- WPF/WinUI/MAUI pickers list System first and render localized descriptions; Blazor's select offers
  System and localizes all three options. `Themes.Default`'s description is "System".
- Shell icons and Blazor's `data-app-theme` derive from the resolved theme.

## Verification
- WPF: `Theme=System` on an OS in dark app mode renders dark; settings picker reads "System";
  `dotnet test tests/ISynergy.Framework.UI.WPF.Tests` 27/27.
- WinUI: library and sample build (x64); `dotnet test tests/ISynergy.Framework.UI.WinUI.Tests`
  56 passed / 8 skipped. The unpackaged sample cannot start on this machine
  (`REGDB_E_CLASSNOTREG` in the Windows App SDK initializer), so no runtime check.
- MAUI: library and Windows sample build; the running sample renders the dark shell with System.
- Blazor: page reloads dark (`data-theme=dark`, `data-app-theme=dark`, sun icon); a stubbed
  `matchMedia` run of `theme_service_interop.js` flipped `data-theme` to dark and invoked
  `OnSystemThemeChanged(true)`.
- All four samples build with 0 errors.

## Notes / limits
- WinUI/MAUI/WPF live OS-change handlers were not exercised by actually flipping the machine's OS
  theme (no system-wide change was made); resolution, application and the Blazor listener were
  verified, and the handlers are unit-covered where the platform allows.
- The rc.3 SDK on this machine cannot build `net11.0` MAUI (missing `maui-tizen` workload); MAUI was
  built against its `net10.0-windows` target.

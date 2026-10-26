# System theme across all sample apps

Status: IMPLEMENTED (record: .ai/completed/system-theme-all-samples.md)
first run follows the system)

## Goal

`Themes.Default` means "System" everywhere. Each head resolves it to a concrete Light/Dark theme,
applies its native system mode, and reacts when the OS theme changes while the app runs. All four
samples offer "System" in their theme picker and start on it when no settings file exists.

## Contract

- `IThemeService.Theme` and `IsLightThemeEnabled` report the **effective** theme (Default resolved).
- `SetTheme(Themes.Default)` persists `Default` and applies the platform's "follow the OS" mode.
- `SetTheme` persists an explicit choice even when the resolved theme already matches.
- A live OS change re-applies colors and raises `ThemeChanged`.

## Heads

- **WPF** — OS value from `HKCU\...\Themes\Personalize\AppsUseLightTheme`; live tracking through
  `Microsoft.Win32.SystemEvents.UserPreferenceChanged`.
- **WinUI** — `ElementTheme.Default` for System; OS value from `Application.Current.RequestedTheme`;
  live tracking through the window root's `ActualThemeChanged`.
- **MAUI** — `AppTheme.Unspecified` for System (today Default is mapped to Dark); OS value from
  `Application.Current.RequestedTheme`; live tracking through `Application.RequestedThemeChanged`.
- **Blazor** — `matchMedia('(prefers-color-scheme: dark)')` in the theme interop JS; live tracking
  through a `matchMedia` change listener that re-applies the theme and invokes a `[JSInvokable]`
  callback so `ThemeChanged` fires.

## Samples

- `Sample.Shared/Models/LocalSettings.cs`: default `Theme = Themes.Default`.
- WPF / WinUI / MAUI / Blazor settings surfaces: add "System" (display name; the enum stays
  `Themes.Default`).
- Shell icons, design tokens and Blazor `data-app-theme` derive from the resolved theme.

## Verification

- Unit tests for `Default` resolution and for `SetTheme(Default)` persistence in each head.
- Run each sample, flip the Windows app theme in Settings while it runs, confirm palette + icon
  follow, and confirm the choice survives a restart.
- `dotnet build` for every sample and `dotnet test` for every theme-related test project.

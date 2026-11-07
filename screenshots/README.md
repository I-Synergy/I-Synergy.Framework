# Settings and theme — platform screenshots

Captured on Windows (2026-10-25) after the MAUI design language was applied to the **whole
shell** in every sample: sidebar, brand tile, navigation rows, breadcrumb header, theme/settings
buttons, surfaces and settings surface.

| Platform | File | Shows |
|---|---|---|
| WPF | [`wpf/02-shell.png`](wpf/02-shell.png) | Custom WPF shell with borderless window chrome: brand tile, navigation rows, breadcrumb header, theme/settings and window buttons, Help/version footer. |
| WPF | [`wpf/01-settings.png`](wpf/01-settings.png) | Settings card over the new shell. |
| WinUI | [`winui/04-shell.png`](winui/04-shell.png) | WinUI shell with the shared header, brand tile, Help/version footer and the selected item shown in the breadcrumb. |
| WinUI | [`winui/03-settings.png`](winui/03-settings.png) | Settings blade over the new shell. |
| MAUI | [`maui/03-shell-light.png`](maui/03-shell-light.png) | MAUI dashboard (reference shell) in light theme, with Help/version footer. |
| MAUI | [`maui/04-shell-macos.png`](maui/04-shell-macos.png) | MAUI dashboard on the native macOS (AppKit) head, .NET 11, dark theme (follows the system appearance). |
| MAUI | [`maui/05-shell-maccatalyst.png`](maui/05-shell-maccatalyst.png) | MAUI dashboard on Mac Catalyst, .NET 11, light theme. |
| MAUI | [`maui/06-shell-android.png`](maui/06-shell-android.png) | MAUI dashboard on Android (API 36 emulator, 1280×800), .NET 11, light theme. |
| MAUI | [`maui/01-settings-dark.png`](maui/01-settings-dark.png) | MAUI settings blade in dark theme. |
| MAUI | [`maui/02-settings-light.png`](maui/02-settings-light.png) | MAUI settings blade in light theme. |
| Blazor | [`blazor/03-shell-light.png`](blazor/03-shell-light.png) | Blazor shell using the custom MAUI-style sidebar/header instead of FluentLayout. |
| Blazor | [`blazor/01-settings-dark.png`](blazor/01-settings-dark.png) | Blazor settings drawer in dark theme. |
| Blazor | [`blazor/02-settings-light.png`](blazor/02-settings-light.png) | Blazor settings drawer in light theme. |

Notes:

- The 04–06 shots were captured on macOS on 2026-11-07 from the **.NET 11** heads (the sample is
  .NET 11 only): macOS/AppKit and Mac Catalyst windows were rendered in-app (`CacheDisplay` /
  `UIGraphicsImageRenderer`), Android came from `adb exec-out screencap` on an API 36 arm64
  emulator resized to 1280×800. macOS follows the system appearance, hence the dark shot; Mac
  Catalyst and Android run the default light theme.
- The shared tokens live in `Sample.Maui/Resources/Styles/Colors.xaml` and are mirrored in the
  WPF/WinUI resource dictionaries and the Blazor CSS variables.
- Figtree is bundled in all four samples; the accent palette is teal / blue / green / orange.
- The samples auto-authenticate the demo user (`admin` / `Welkom01`) and go straight to the shell;
  the login screen is not part of the shell and is no longer shown at startup.

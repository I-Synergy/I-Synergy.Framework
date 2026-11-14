# Settings and theme — platform screenshots

Captured on Windows (2026-10-25) and macOS, iOS and Android (2026-11-08) after the MAUI design
language was applied to the **whole shell** in every sample: sidebar, brand tile, navigation rows,
breadcrumb header, theme/settings buttons, surfaces and settings surface.

| Platform | File | Shows |
|---|---|---|
| WPF | [`wpf/02-shell.png`](wpf/02-shell.png) | Custom WPF shell with borderless window chrome: brand tile, navigation rows, breadcrumb header, theme/settings and window buttons, Help/version footer. |
| WPF | [`wpf/01-settings.png`](wpf/01-settings.png) | Settings card over the new shell. |
| WinUI | [`winui/04-shell.png`](winui/04-shell.png) | WinUI shell with the shared header, brand tile, Help/version footer and the selected item shown in the breadcrumb. |
| WinUI | [`winui/03-settings.png`](winui/03-settings.png) | Settings blade over the new shell. |
| MAUI | [`maui/03-shell-light.png`](maui/03-shell-light.png) | MAUI dashboard (reference shell) in light theme, with Help/version footer. |
| MAUI | [`maui/05-shell-maccatalyst.png`](maui/05-shell-maccatalyst.png) | MAUI dashboard on Mac Catalyst, .NET 11, light theme. |
| MAUI | [`maui/06-shell-android.png`](maui/06-shell-android.png) | MAUI dashboard on Android (API 37 emulator, 1280×800), .NET 11, light theme. |
| MAUI | [`maui/01-settings-dark.png`](maui/01-settings-dark.png) | MAUI settings blade in dark theme. |
| MAUI | [`maui/02-settings-light.png`](maui/02-settings-light.png) | MAUI settings blade in light theme. |
| MAUI | [`maui/08-settings-maccatalyst.png`](maui/08-settings-maccatalyst.png) | MAUI settings blade on Mac Catalyst, .NET 11, light theme; the shell behind is dimmed by the scrim. |
| MAUI | [`maui/09-settings-android.png`](maui/09-settings-android.png) | MAUI settings blade on Android (API 37 emulator, 1280×800), .NET 11, light theme. |
| MAUI | [`maui/10-shell-ios.png`](maui/10-shell-ios.png) | MAUI dashboard on the iPhone 17 simulator (402×874 pt, 1206×2622), .NET 11, dark system theme. |
| MAUI | [`maui/11-settings-ios.png`](maui/11-settings-ios.png) | First-run settings page on iOS, full-screen phone presentation, dark theme. |
| MAUI | [`maui/12-shell-android-phone.png`](maui/12-shell-android-phone.png) | MAUI dashboard on an Android phone (1080×2400), .NET 11, light theme. |
| MAUI | [`maui/13-drawer-android-phone.png`](maui/13-drawer-android-phone.png) | LTR phone drawer on Android: flush left, top and bottom, rounded on the right only; the page behind is dimmed by the full-width scrim. |
| MAUI | [`maui/14-settings-android-phone.png`](maui/14-settings-android-phone.png) | Settings page on an Android phone, full-screen, light theme. |
| MAUI | [`maui/15-drawer-android-rtl.png`](maui/15-drawer-android-rtl.png) | RTL phone drawer on Android: flush right, top and bottom, rounded on the left only. |
| Blazor | [`blazor/03-shell-light.png`](blazor/03-shell-light.png) | Blazor shell using the custom MAUI-style sidebar/header instead of FluentLayout. |
| Blazor | [`blazor/01-settings-dark.png`](blazor/01-settings-dark.png) | Blazor settings drawer in dark theme. |
| Blazor | [`blazor/02-settings-light.png`](blazor/02-settings-light.png) | Blazor settings drawer in light theme. |

Notes:

- The 01–03 shots were re-captured on **Windows** on 2026-11-07 from the **.NET 11** `net11.0-windows10.0.26100.0`
  (WinUI) head, at native 2134×1318 (2× HiDPI, after trimming the invisible resize frame) via a
  `PrintWindow`/`PW_RENDERFULLCONTENT` capture of the app window — `maui devflow ui screenshot` reports
  "Failed to capture screenshot" on the Windows head, and its UI automation (`ui query`/`ui navigate`)
  returns no elements there. The Windows head uses the platform title bar with the application title and
  keeps the shell header in the page, as on the Apple targets, so the settings blade dims the menu, theme
  and settings commands along with the rest of the shell. `BladeView` dismisses open blades when the app
  theme changes (reparented blade content keeps its native control theme otherwise), so the blade shots
  show a blade opened after the theme switch. The MAUI blade width was set to 480 after these captures;
  the 01/02 Windows shots still show the earlier 520px blade.
- The 05, 08 and 09 shots were captured on macOS and Android on 2026-11-08 from the **.NET 11**
  heads (the sample is .NET 11 only): Mac Catalyst windows were rendered in-app
  (`UIGraphicsImageRenderer`, 2660×1996), Android came from
  `adb exec-out screencap` on an API 37 arm64 emulator set to 1280×800 at density 160. Mac Catalyst
  runs the light theme. The desktop/tablet sidebar is flush with the page top, outer edge and bottom,
  with the rounding only on the inner edge (right in LTR, left in RTL). The settings blade has the
  same 480px width on every head, so all four accent swatches fit. The Android blade scrim is a
  layout background, so the shell behind the settings blade is dimmed rather than black.
- The 10–15 phone shots were captured on 2026-11-08: iOS on the iPhone 17 simulator (1206×2622,
  dark system theme), Android on the Pixel 6 AVD (1080×2400 at density 420, light theme). Both
  heads open the settings blade on first run; the shell shots are after that flag was saved. The
  Android drawer scrim dims the page behind it, so the full content width stays visible. The RTL
  shot (15) was taken after switching the app language to Arabic from the settings picker and
  restarting the app.
- The shared tokens live in `Sample.Maui/Resources/Styles/Colors.xaml` and are mirrored in the
  WPF/WinUI resource dictionaries and the Blazor CSS variables.
- Figtree is bundled in all four samples; the accent palette is teal / blue / green / orange.
- The samples auto-authenticate the demo user (`admin` / `Welkom01`) and go straight to the shell;
  the login screen is not part of the shell and is no longer shown at startup.

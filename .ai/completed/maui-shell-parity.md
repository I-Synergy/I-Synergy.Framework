# MAUI shell parity for every sample

**Status:** DONE
Started: 2026-10-25
Completed: 2026-10-25

## Goal

Apply the Sample.Maui shell design to every sample, not only the settings surfaces. Menu items
stay platform-specific; the chrome (sidebar, brand tile, navigation rows, breadcrumb header,
theme/settings buttons, surfaces) is shared.

## Work

- **WPF**: removed `Syncfusion.SfNavigationDrawer.WPF`; rebuilt the shell with plain WPF controls —
  permanent 300px sidebar (brand tile, navigation rows with icon tiles and accent-soft selection),
  breadcrumb header with theme/settings icon buttons and a content `Frame`. The frame is still
  found by the framework navigation service.
- **WinUI**: kept the platform `NavigationView` for the menu items but added the shared 56px
  breadcrumb header (back/theme/settings), a `PaneHeader` brand tile and the shared surface/font
  tokens; removed the Theme/Settings footer rows and the forced first-run settings blade.
- **All samples**: the demo user auto-authenticates and goes straight to the shell; the login
  screen is no longer part of the startup flow.
- **Screenshots**: regenerated the WPF/WinUI shell and settings captures.

## Verification

- `Sample.WPF` and `Sample.WinUI` build with 0 warnings / 0 errors.
- Shells and settings surfaces captured in `screenshots/{wpf,winui}`.

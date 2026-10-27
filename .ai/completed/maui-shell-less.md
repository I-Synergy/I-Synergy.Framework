# MAUI: Drop Shell, adopt the WinUI shell layout

Status: DONE
Started: 2026-10-25
Completed: 2026-10-26
Plan: `.ai/plans/maui-shell-less.md`

## Steps

- [x] 1. `Views/RootView.xaml(.cs)` — root page: header row, sidebar column, `ContentHost`, `BladeHostView` last child
- [x] 2. Bootstrap: `RootView` is `Windows[0].Page`; `ShellView` is no longer the start page
- [x] 3. Navigation: WinUI-style content host + `INavigationService` backstack; no `Shell.Current.GoToAsync`
- [x] 4. Remove Shell chrome (`FlyoutContent`, `TitleView`, `TabBar`, `ShellContent`, shell styles/attached properties)
- [x] 5. Phone layout: slide-in sidebar drawer inside `RootView`
- [x] 6. Remove per-page `<c:BladeHostView />` from the five pages
- [x] 7. Back handling through the navigation service stack (header button + Android hardware back)

## What changed

- `src/ISynergy.Framework.UI.Maui`
  - `Services/INavigationHost.cs`, `Services/NavigationHostRegistry.cs`, `Controls/NavigationHostView.cs` — the MAUI
    equivalent of the WinUI/WPF content `Frame`. The host detaches and caches a page's `Content` (pages cannot be
    children of a layout) and registers itself with the registry while attached.
  - `Services/NavigationService.cs` — resolves the host as its navigation frame, so `NavigateAsync` replaces the
    host's content and `BaseNavigationService` owns the backstack; the MAUI push/pop overrides are gone. Modals stay
    native (`PushModalAsync`) when the root owns a navigation host. `GoBackAsync` always raises `BackStackChanged`.
  - `Extensions/MauiAppBuilderExtensions.cs` — registers `INavigationHostRegistry`.
- `src/ISynergy.Framework.Mvvm` / `ISynergy.Framework.UI`
  - `INavigationService.CurrentViewModelType` (+ `BaseNavigationService` implementation) so a shell can restore its
    breadcrumb/selection after a back navigation.
- `samples/Sample.Maui`
  - `Views/ShellView.xaml(.cs)` deleted; `Views/RootView.xaml(.cs)` is the always-alive root: header, sidebar,
    `NavigationHostView`, `BladeHostView` as the last child.
  - `App.xaml.cs` sets `RootView` as the window root; `ShellViewModel` navigates sections with
    `_navigationService.NavigateAsync<TViewModel>()` (as WinUI does), keeps `CurrentRoute`/`CrumbCurrent` in step on
    back, and exposes `ToggleMenuCommand`/`BackCommand`.
  - Phones: content spans both columns, the 300px sidebar is translated off-screen and slides in over a scrim from the
    header menu button; the drawer closes on scrim tap or section selection.
  - Back: header button bound to `IsBackEnabled`; `Platforms/Android/MainActivity.OnBackPressed` routes to
    `INavigationService.GoBackAsync()` when `CanGoBack`.

## Verified

- `Sample.Maui` builds for `net11.0-windows10.0.26100.0` and `net10.0-windows10.0.26100.0` (win-arm64), 0 errors.
- Whole solution (`I-Synergy.Framework.slnx`) builds after the shared `INavigationService` addition.
- Running app (Windows): every sidebar section renders in the content host with the highlight/breadcrumb following;
  the settings blade covers sidebar and header; back returns to the previous section and restores the breadcrumb;
  the phone drawer was checked with the `DeviceIdiom.Phone` path temporarily forced (files restored afterwards).

## Follow-up (same day): clicks swallowed by the empty blade host

Reported after the first cutover: nothing in the window reacted to the mouse (sidebar and header alike).

- Cause: `Controls/BladeHostView` set `IsVisible = true` as soon as it resolved `BladeHostViewModel`, while the
  inner `BladePane` stays hidden when it has no blades. At the root that left a visible, full-window `ContentView`
  above the header, sidebar and page. `maui devflow ui hit-test 810 55` returned `BladeHostView` *above* the
  settings `Button`, which proved the occlusion.
- Fix: the host starts hidden and mirrors `BladePane.IsVisible` (it subscribes to the pane's `PropertyChanged`),
  so it only takes part in hit-testing while a blade is actually shown.
- Second defect found with real clicks: the breadcrumb/selection reset to the previous section after a forward
  navigation, because `BackStackChanged` is raised before the new frame becomes current. `ShellViewModel` now
  ignores that event while one of its own navigations is in flight (`_isNavigating`) and only re-reads
  `CurrentViewModelType` on back.
- Verified with real mouse input (DPI-scaled `SetCursorPos` + `mouse_event`) on the running Windows build:
  sidebar section switch (content + breadcrumb + highlight), settings blade open from the header, blade close,
  navigation after closing, and the header back button.

## Not verified

- Android hardware back on a device/emulator (compile-only here).
- iOS/MacCatalyst runtime layout.
- Real phone-idiom run (drawer verified through a temporarily forced idiom path on Windows).

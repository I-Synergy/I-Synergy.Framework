# MAUI: Drop Shell, adopt the WinUI shell layout

Status: DONE — 2026-10-26
Owner: MAUI sample (`samples/Sample.Maui`)

## Why

`Microsoft.Maui.Controls.Shell` owns the page tree, so there is nowhere above the pages to
host a blade pane:

- `Shell` accepts only `ShellContent`/`TabBar`, `FlyoutContent` and `TitleView` — a
  `ControlTemplate` is rejected by the XAML compiler (`MAUIX2002` on `ControlTemplate`).
- Only the current tab page is in the visual tree, so a per-page `BladeHostView` is invisible
  whenever a blade is opened from a different tab (observed: Settings visible on Dashboard,
  invisible on Custom Controls / Maui Controls / Slide Show).
- `IBladePaneHost.IsHostAttached` only knows "some host instance exists", not which one is on
  screen, so `Maui/Services/BladeService` picks the overlay path and pushes the blade into an
  off-screen pane.

WinUI and WPF do not have this problem: their shell is a root `Grid` and the blade host is the
last child, so the pane always covers everything.

## Target layout (WinUI parity)

One root `ContentPage` that is always alive:

```
ContentPage (RootView)
  Grid
    Row 0: header (breadcrumb, theme, settings)
    Row 1: content row
      Column 0: SidebarView            (desktop; drawer/tab row on phone)
      Column 1: ContentView "ContentHost"   ← navigated page goes here
    <c:BladeHostView />               (last child, spans all rows/columns)
```

## Steps

1. `Views/RootView.xaml(.cs)` — new root page with the layout above; `BladeHostView` last child.
   Reuse `SidebarView`, `AppHeaderView` (currently in `Shell.FlyoutContent`/`Shell.TitleView`).
2. `App`/`Application` bootstrap — `MainPage = new NavigationPage(rootView)` so back navigation
   still works; replace `ShellView` as the start page.
3. Navigation — `ShellViewModel.NavigateTo(route)` becomes a content swap:
   `ContentHost.Content = <view>` (maps route → view type), with the back stack owned by the
   framework `INavigationService` (MAUI already supports `PushAsync`/`PushModalAsync`).
4. Remove `Shell` chrome usage: `Shell.NavBarIsVisible`, `Shell.TitleColor`, `Shell.FlyoutContent`,
   `Shell.TitleView`, `TabBar`, the four `ShellContent` entries, and the `GoToAsync` call added to
   `ShellViewModel.NavigateTo`.
5. Phone layout — `FlyoutBehavior="Disabled"` today; replace with a drawer or bottom tab row inside
   `RootView` (decision pending: drawer vs tab row).
6. Delete per-page `<c:BladeHostView />` from `InfoView`, `CustomControlsView`, `MauiControlsView`,
   `SlideShowView`, `DetailsView` (the root host replaces all of them).
7. Back handling — Android hardware back and the desktop back button route through the
   navigation service's stack instead of `Shell`'s.

## Risks

- Losing `Shell` deep-link/routing semantics; anything using `Shell.Current` must be rewritten
  (`ShellViewModel.NavigateTo` currently calls `Shell.Current.GoToAsync`).
- Phone tab bar must be reimplemented; verify `Idiom == Phone` explicitly.
- `NavigationPage` introduces its own nav bar — hide it (`NavigationPage.HasNavigationBar=false`)
  and keep our header row.

## Verification

- `dotnet build samples/Sample.Maui/Sample.Maui.csproj -f net11.0-windows… -p:RuntimeIdentifierOverride=win-arm64`
  and the `net10.0` variant, both 0 errors.
- Launch and capture the window (DPI-aware script in `$TMPDIR`): open Settings from each of the four
  sections — the pane must appear identically on all of them and dim the sidebar.
- Confirm back navigation (desktop button + Android back) and the phone layout.

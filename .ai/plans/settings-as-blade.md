# Plan — Settings as a blade view in every UI head

Status: **IMPLEMENTED — 2026-10-25**
Scope: `src/ISynergy.Framework.UI` (shared host view model), `src/ISynergy.Framework.UI.WPF`,
`src/ISynergy.Framework.UI.Maui`, `src/ISynergy.Framework.UI.WinUI` (verification only),
`samples/Sample.WPF`, `samples/Sample.Maui`, `samples/Sample.WinUI`.
Approved scope: **WPF + WinUI + MAUI** (Blazor has no blade infrastructure and is out of scope);
MAUI = **replace the per-page settings drawer with an in-page blade pane**.

## Goal

The settings surface of every head is hosted as a blade: its view model is a `ViewModelBlade`,
it is opened through `IBladeService`, it renders inside the framework's blade pane, and it closes
through the blade lifecycle (`Closed` → `BaseBladeService.RemoveBlade`) instead of navigating.

## Current state (evidence)

| Head | Settings today |
|---|---|
| WinUI | `SettingsViewModel : ViewModelBlade<object>`, opened by `Sample.WinUI/ViewModels/ShellViewModel.OpenSettingsBladeAsync()` via `IBladeService`, rendered by the shell's `c:BladeView` (`ShellView.xaml:160`). **Already a blade** — verify only. |
| WPF | `SettingsViewModel : ViewModelNavigation<object>`, opened by `_navigationService.NavigateAsync<SettingsViewModel>()` into the shell's `ContentRootControl` frame (`Sample.WPF/ViewModels/ShellViewModel.cs:204`). `BaseShellViewModel : ViewModel` cannot own blades. |
| MAUI | `OpenSettingsAsync()` opens a `SettingsDrawerState` (`Sample.Maui/ViewModels/ShellViewModel.cs:172`); the `SettingsDrawer` control is duplicated in four tab pages (`InfoView`, `CustomControlsView`, `MauiControlsView`, `SlideShowView`) and drives a scoped `SettingsViewModel : ViewModelNavigation<bool>`. The MAUI `Shell` root cannot host a pane, which is why the overlay lives per page. |
| Blazor | Inline drawer in `MainLayout.razor`; `I-Synergy.Framework.UI.Blazor` has no `IBladeService`/`IViewModelBladeView`/blade host at all. Out of scope. |

Also relevant:
- `BladePane` (MAUI) and `BladeView` (WPF) are the pane hosts delivered in the previous pass; both
  resolve the physical edge through `BladePlacement` and expose `ItemsSource`/`Anchor`/`BladeMaxWidth`.
- The MAUI `Views/SettingsView.xaml` that exists is an **unreferenced `c:TabbedView` demo** bound to
  `SettingsViewModel` (`SubmitCommand`/`CancelCommand`); the real settings UI is the drawer. The name
  is therefore already taken by a dead sample page and blocks the convention mapping
  `SettingsViewModel` → `SettingsView`.
- The settings blade needs an owner that implements `IViewModelBladeView`; no shell-level owner exists
  on WPF or MAUI (changing `BaseShellViewModel`'s base class on either head would be a public API
  break and, on MAUI, would drop `ViewModelNavigation`).

## Design

### 1. Shared shell-level blade owner (`ISynergy.Framework.UI`)

New `ViewModels/BladeHostViewModel.cs`:

```csharp
public sealed class BladeHostViewModel : ViewModelBladeView<object>
{
    // no-op Add/Edit/Remove/Search, RefreshAsync -> RetrieveItemsAsync (both completed)
}
```

A `ViewModelBladeView<object>` that is **not** attached to a page: it exists only to own blades whose
surface belongs to the shell (settings) rather than to a specific page. Registered as a singleton in
WPF and MAUI (`TryAddSingleton<BladeHostViewModel>()`) so every pane in the window observes the same
`Blades` collection. No public API of `BaseShellViewModel` changes.

### 2. Pane host control per platform

- WPF: `src/ISynergy.Framework.UI.WPF/Controls/BladeHostView.xaml(.cs)` — a `UserControl` that resolves
  `BladeHostViewModel` from `Application.Services`, sets its own `DataContext` to it and hosts
  `<c:BladeView ItemsSource="{Binding Blades}" />`. Placed once in `Sample.WPF/Views/ShellView.xaml`
  above the content frame.
- MAUI: `src/ISynergy.Framework.UI.Maui/Controls/BladeHostView.cs` — a `ContentView` that resolves the
  same singleton via `IPlatformApplication.Current.Services` and hosts a `BladePane` bound to
  `Blades`. Placed as the last child of each tab page's root grid, replacing `<controls:SettingsDrawer />`.

Both controls tolerate a missing DI host (render nothing, log) so a page can keep them unconditionally.

### 3. Settings view models

- WPF `SettingsViewModel : ViewModelBlade<object>` — same properties/side effects; drop
  `INavigationService` and the `CancelAsync` → `GoBackAsync` override (the base `CancelCommand`
  raises `Closed`, which pops the blade).
- MAUI `SettingsViewModel : ViewModelBlade<object>` — same properties/side effects as today's drawer
  VM; drop `INavigationService`, `IDialogService` usage that only served the drawer, and the
  `[Lifetime(Scoped)]` attribute (the blade scope created by `BaseBladeService` owns it).
- WinUI unchanged.

### 4. Opening settings

- WPF `Sample.WPF/ViewModels/ShellViewModel.OpenSettingsAsync` →
  `_bladeService.OpenBladeAsync<SettingsViewModel, object>(_bladeHost, submitted: null, cancelled: null)`
  where `_bladeHost` is the injected `BladeHostViewModel`.
- MAUI `Sample.Maui/ViewModels/ShellViewModel.OpenSettingsAsync` → same call (replaces
  `_settingsDrawerState.Open()`); the `SettingsDrawerState` dependency, its field, the
  `OpenBackgroundAsync` usage of it (if any) and the DI registration are removed.
- WinUI untouched.

### 5. MAUI settings blade view (and drawer removal)

- New `samples/Sample.Maui/Views/SettingsView.xaml` + `.xaml.cs`: a `c:View` (`ContentPage`) built from
  the existing drawer markup — header, language `Picker`, theme `Picker`, four accent swatches and a
  Close button bound to `CancelCommand`. `x:DataType="viewmodels:SettingsViewModel"`. Live
  application/persistence stays in the view model, as it is today.
- **Delete** the unreferenced `c:TabbedView` demo page that currently owns the `SettingsView` name
  (`samples/Sample.Maui/Views/SettingsView.xaml` + `.xaml.cs`) — it binds a `SubmitCommand` the blade
  base does not define and is not reachable from the shell.
- **Delete** `samples/Sample.Maui/Views/Controls/SettingsDrawer.xaml(.cs)` and
  `samples/Sample.Maui/Services/SettingsDrawerState.cs`, and replace the four
  `<controls:SettingsDrawer />` usages with `<controls:BladeHostView />`.

### 6. WinUI verification (no functional change expected)

Check that the settings blade opens from the shell pane, that the Close button closes it via
`CancelCommand`/`Closed`, and that nothing in `ShellView.xaml`/`SettingsView.xaml` still assumes a
navigated page. Fix only what is actually broken.

## Steps

- [ ] 1. Add `BladeHostViewModel` to `ISynergy.Framework.UI`; register it in the WPF and MAUI app-builder extensions.
- [ ] 2. WPF: add `Controls/BladeHostView`; convert `SettingsViewModel` to `ViewModelBlade<object>`; open it from `ShellViewModel` through `IBladeService`; host the control in `ShellView.xaml`.
- [ ] 3. Build and smoke-run `Sample.WPF` (0 errors, no new warnings) and verify the blade opens/closes.
- [ ] 4. MAUI: convert `SettingsViewModel` to `ViewModelBlade<object>`; create the `SettingsView` blade page from the drawer markup; delete the demo page, `SettingsDrawer` and `SettingsDrawerState`.
- [ ] 5. MAUI: add `Controls/BladeHostView`; put it in the four tab pages; open settings through `IBladeService` from `ShellViewModel`.
- [ ] 6. Build and smoke-run `Sample.Maui` (0 errors; only the pre-existing `CS8601` warning).
- [ ] 7. WinUI: verify the existing settings blade and fix only concrete breakage; build `Sample.WinUI`.
- [ ] 8. Tests: build all three UI libraries, run `tests/ISynergy.Framework.UI.Tests`, `git diff --check`.
- [ ] 9. Docs: WPF/MAUI readme notes (settings blade, `BladeHostView`), `CHANGELOG.md`, session handoff.

## Verification

- `MSBUILDDISABLENODEREUSE=1 dotnet build <project> -nodeReuse:false -f <tfm>` for
  `UI.WPF` (`net10.0-windows10.0.26100.0`), `UI.Maui` (`net11.0-windows10.0.26100.0`),
  `Sample.WPF` (`net11.0-windows`), `Sample.Maui` (`net11.0-windows10.0.26100.0`),
  `Sample.WinUI` (`net11.0-windows10.0.26100.0 -p:Platform=x64`).
- `dotnet test tests/ISynergy.Framework.UI.Tests`.
- Manual: open Settings from the header gear in WPF and from the sidebar/menu in MAUI; confirm the
  pane shows the settings card, closes via Close/scrim, and that a second open re-creates it.
  On MAUI phone idiom the blade presents modally by design.
- `git diff --check`.

## Risks / decisions

- **New shared type** (`BladeHostViewModel`): the alternative — deriving WPF/MAUI `BaseShellViewModel`
  from `ViewModelBladeView<NavigationItem>` like WinUI — is a public API break and, on MAUI, drops
  `ViewModelNavigation`/`IViewModelNavigation`. The host view model keeps the shell contracts intact.
  It becomes public framework API; if that is unwanted, the same class can live in each sample instead.
- **Deleting the `TabbedView` demo page** is deliberate: it is unreachable, shares the
  `SettingsViewModel` name, and its `SubmitCommand` bindings do not survive the blade conversion.
  Say so if the demo should be preserved under a different name instead.
- **MAUI phone**: the fixture has a pane host, but `BladeService` still prefers the modal page on
  `DeviceInfo.Idiom == Phone`; settings therefore opens modally on phones and as a pane elsewhere.
- WinUI keeps its shell-owned blade (`BaseShellViewModel : ViewModelBladeView<NavigationItem>`); it is
  already the desired shape, so it is not migrated to `BladeHostViewModel`.

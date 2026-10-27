# Plan — Blade (pane) parity for WPF and MAUI, with RTL anchoring

Status: **IMPLEMENTED — 2026-10-25**
Scope: `src/ISynergy.Framework.UI` (shared placement helper), `src/ISynergy.Framework.UI.WPF`,
`src/ISynergy.Framework.UI.Maui`, `src/ISynergy.Framework.UI.WinUI` (RTL only),
`samples/Sample.WPF`, `samples/Sample.Maui`, `samples/Sample.WinUI`, `tests/ISynergy.Framework.UI.Tests`.
Approved scope: **WPF + MAUI (adaptive) + WinUI RTL fix**.

## Goal

Give WPF and MAUI the same "blades" interaction WinUI has: a blade append its view to
`IViewModelBladeView.Blades`, the pane surfaces from the screen edge, earlier blades stay visible but
disabled/dimmed, and the newest blade is interactive; closing a blade pops it and re-enables the previous
one. The anchored edge must follow the active flow direction (RTL ⇒ pane opens from the left on screen),
including animation, chrome and stacking direction. WinUI's existing `BladeView` must gain the same
anchoring instead of hardcoding `HorizontalAlignment="Right"`.

## Current state (evidence)

- Shared model already exists: `IViewModelBladeView` (`Blades`, `IsPaneVisible`),
  `ViewModelBladeView<TModel>` (owns `Blades`, `AddCommand`/`EditCommand`/…),
  `IViewModelBlade.Owner`, `BaseBladeService` (`src/ISynergy.Framework.UI/Services/Base/BaseBladeService.cs`).
- `BaseBladeService.AddBladeToOwner` disables every existing blade and appends the new view;
  `RemoveBlade` re-enables the last remaining blade and hides the pane at zero.
- WPF: `Services/BladeService.cs` exists and is registered
  (`WPFAppBuilderExtensions.cs:106`), but **no** visual host exists in
  `src/ISynergy.Framework.UI.WPF/Controls` and no sample uses blades. WPF `Controls/View` is a `Page`.
- MAUI: no `BladeService`, no host, no `SingleSelectionView`/`MultipleSelectionView`. MAUI `Controls/View`
  is a `ContentPage`, so it cannot be a child of a layout.
- WinUI: `Controls/BladeView/BladeView.xaml` (scrim `Border` + `ListBox` with a right-aligned `StackPanel`;
  item = bordered card; previous blades at `DisabledOpacity`), `Converters/BladeConverters.cs`,
  `Views/SingleSelectionView`/`MultipleSelectionView`, `Services/BladeService.cs`.
  No XAML `FlowDirection` anywhere in the project; RTL is only attempted through the native
  `WS_EX_LAYOUTRTL` window style (`Extensions/WindowNativeExtensions.cs:32-75`) — XAML content is
  composed outside GDI, so that flag does not reliably mirror it.
- WPF RTL: `Application/Application.cs:409` applies `FlowDirection` to the root at startup only.
- MAUI RTL: `Window.FlowDirection` from `LocalSettings.Language` (`Application/Application.cs:242`),
  inherited by pages (`Controls/View.cs:72-84`, `Extensions/NavigationExtensions.cs:88-89`).
- Tests: `tests/ISynergy.Framework.Mvvm.Tests/ViewModels/ViewModelBlade*Tests.cs` exist; no tests cover a
  platform host or placement.

## Design

### 1. Shared anchoring (no platform types)

New in `src/ISynergy.Framework.UI`:

- `Enumerations/BladePaneAnchor.cs`: `Auto`, `Start`, `End`. `Start`/`End` are logical; `Auto` = `End`.
- `Helpers/BladePlacement.cs`: pure, unit-testable resolution —
  `bool IsStartSide(BladePaneAnchor anchor, bool isRightToLeft)` and
  `int HorizontalSign(bool isStartSide, bool isRightToLeft)` (for slide animation / shadow direction).
  Uses `GlobalizationConstants.IsRightToLeftLanguage` where a culture name is supplied.

Rationale: one place decides "which physical edge", so all three platforms agree and the rule is testable
without a UI thread.

### 2. WPF host

New `src/ISynergy.Framework.UI.WPF/Controls/BladeView.xaml(.cs)` (name/shape mirrors WinUI so call sites
read the same):

- `Grid` root: scrim `Border` (click on scrim ⇒ raise `CloseRequested`, owner pops the last blade),
  then an `ItemsControl`/`ListBox` whose items panel is a `StackPanel` with `Spacing`.
- DPs: `ItemsSource` (`ObservableCollection<IView>`), `Anchor` (`BladePaneAnchor.Auto`), `DisabledOpacity`
  (0.75), `BladeSpacing` (10), `BladeMaxWidth` (default 520), `InnerPadding`.
- Each item hosts the blade `Page` in a `Frame` (`JournalOwnership=OwnsJournal`,
  `NavigationUIVisibility=Hidden`, `Focusable=False`) because a WPF `Page` cannot be added to a panel.
  The item template applies `IsEnabled`/`Opacity` by index (last = enabled/1.0, earlier = disabled/
  `DisabledOpacity`), matching `BaseBladeService` semantics.
- `Visibility` follows `Blades.Count > 0` **and** `IsPaneVisible` (fixes the WinUI divergence where
  `IsPaneVisible` is ignored).
- Re-applies placement on `FlowDirectionChanged`, and on `ItemsSource.CollectionChanged`.
- Slide-in animation: `TranslateTransform.X`, initialized off-screen on the resolved side, eased to 0;
  sign from `BladePlacement.HorizontalSign` (WPF mirrors layout but **not** render transforms).
- `DropShadowEffect` with `Direction` mirrored for RTL; asymmetric `BorderThickness` set from the resolved
  side rather than relying on margin mirroring.

`Services/BladeService.cs` (WPF) gains selection-view resolution parity
(`ISingleSelectionViewModel` → new `SingleSelectionView`, `IMultipleSelectionViewModel` →
`MultipleSelectionView`) only if those views are added — see step 6.

### 3. MAUI host + service (adaptive)

New `src/ISynergy.Framework.UI.Maui/Controls/BladePane.cs` (a `ContentView`):

- Root `Grid` (scrim `Border` + `HorizontalStackLayout`/`FlexLayout` of blade cards). The pane is anchored
  with `HorizontalOptions = LayoutOptions.End` (flow-aware in MAUI, so it flips with `FlowDirection` for
  free); the stack order and `TranslationX` animation direction use `BladePlacement.HorizontalSign`.
- A blade is rendered from `((ContentPage)view).Content` into a `Border` (a ContentPage cannot be a child),
  preserving `BindingContext`; `IView`/VM disposal on removal, mirroring `BaseBladeService`/WinUI.
- Exposes a static/DI-reachable `IBladePaneHost` registration (`BladePane.Current`-style accessor is
  avoided in favour of an `IBladePaneHostAccessor` singleton set in `OnParentSet`/`OnHandlerChanged` and
  cleared on detach) so `BladeService` can find the visible host.
- Page owners declare the control in their own root `Grid` (MAUI `Shell` pages cannot host an overlay
  from `Shell` itself).

New `src/ISynergy.Framework.UI.Maui/Services/BladeService.cs : BaseBladeService`:

- Resolves the view from the blade scope (same as WPF/WinUI) and decides presentation by
  `DeviceInfo.Idiom`:
  - `Phone` → `PushModalAsync(view)` (existing `NavigationService` path); pop on `Closed`/cancel.
  - otherwise → `IBladePaneHostAccessor.Current.Show(view)`; if no host is registered, fall back to modal
    and log a warning (never silently drop the blade).
- Registers in the MAUI app-builder extension alongside the other services
  (`TryAddSingleton<IBladeService, BladeService>()`).
- Selection-view resolution parity with WinUI (step 6).

### 4. WinUI RTL fix

- Stop relying on `HorizontalAlignment="Right"` in `Controls/BladeView/BladeView.xaml`; bind the items
  panel alignment and the item alignment through `Anchor` + effective flow direction, using the same
  `BladePlacement` helper. Items panel gets `FlowDirection` so the stack grows from the resolved edge.
- Set XAML `FlowDirection` on the window root when the language changes
  (`Application/Application.cs` already knows the language), which is the supported WinUI mirroring path.
  Keep `WS_EX_LAYOUTRTL` only for the non-client title area, or drop it — decide by visual check in
  `ar-SA`; applying both double-mirrors. **This is the riskiest item** (app-wide, not blade-only).
- Mirror `ThemeShadow`/`Translation` and the card's asymmetric border/corner radius using the helper.

### 5. Samples

- `Sample.WinUI`: keep `BladeDetails` host; add an `Anchor` attribute where useful and a language switch
  check for `ar`.
- `Sample.WPF`: add the `BladeView` host to `Views/ShellView.xaml` (third column / overlay row) and make
  the detail view models (`DetailAViewModel`/`DetailBViewModel`/`DetailsViewModel`) derive from
  `ViewModelBladeView<TModel>` so `Blades` exists; wire `IBladeService` opens from `DetailsViewModel`.
- `Sample.Maui`: add `BladePane` to the blade-owning pages (`DetailsView`, `DetailView`), make their view
  models blade views, and demonstrate the adaptive path (overlay on Windows/Mac Catalyst, modal on phone).

### 6. Selection views (parity closure, may be deferred)

`SingleSelectionView`/`MultipleSelectionView` exist only in WinUI. Porting them to WPF and MAUI is a
separate UI task with its own XAML. Plan: implement the `BladeService` branches and add the two views to
each platform using the platforms' existing card/list styles. If the user prefers, this drops to a
follow-up plan; blade editing still works without it.

## Steps

- [ ] 1. Add `BladePaneAnchor` + `BladePlacement` to `ISynergy.Framework.UI`, with unit tests
      (`tests/ISynergy.Framework.UI.Tests/Helpers/BladePlacementTests.cs`): Start/End/Auto × LTR/RTL,
      animation sign, and `IsRightToLeftLanguage` wiring for `ar-SA`/`he-IL`/`en-US`.
- [ ] 2. WPF: `Controls/BladeView.xaml(.cs)` (scrim, stack, `Frame`-hosted blades, dim/disable, visibility,
      animation) + RTL re-anchoring on `FlowDirectionChanged`.
- [ ] 3. WPF: sample wiring (`ShellView` host, blade view models, open/close flow) and a manual check that
      opening two blades, closing the newest and closing the last behave as in WinUI.
- [ ] 4. MAUI: `Controls/BladePane.cs` + `IBladePaneHost`/accessor, with flow-aware anchoring and
      `ContentPage.Content` hosting.
- [ ] 5. MAUI: `Services/BladeService.cs` (adaptive: modal on phone, overlay otherwise), DI registration,
      and disposal/lifecycle parity with `BaseBladeService`.
- [ ] 6. MAUI sample wiring on `DetailsView`/`DetailView` + Windows (overlay) and one phone-idiom run check.
- [ ] 7. WinUI: `Anchor`/flow-direction-aware `BladeView`, XAML `FlowDirection` on the root, shadow/border
      mirroring; resolve `WS_EX_LAYOUTRTL` double-mirroring.
- [ ] 8. Selection views (`SingleSelectionView`/`MultipleSelectionView`) for WPF and MAUI, or defer with a
      recorded follow-up.
- [ ] 9. Docs: `readme.md` for UI.WPF/UI.Maui/UI.WinUI (blade contract, `Anchor`, RTL behaviour),
      `CHANGELOG.md` entry, and `.ai/session-context.md` handoff.

## Verification

- `MSBUILDDISABLENODEREUSE=1 dotnet build src/ISynergy.Framework.UI.WPF/... -nodeReuse:false` (one TFM,
  e.g. `net10.0-windows10.0.26100.0`), `...UI.Maui` (`net10.0-windows...`), `...UI.WinUI`,
  plus the three samples.
- `dotnet test tests/ISynergy.Framework.UI.Tests` (placement/helper tests) and
  `dotnet test tests/ISynergy.Framework.Mvvm.Tests --filter Blade` (no model regressions).
- Manual screenshots in `screenshots/`: WPF + MAUI + WinUI both LTR (`en-US`) and RTL (`ar-SA`) with two
  stacked blades — pane anchored on the correct physical edge, animation from that edge, dimming and
  close/re-enable behaviour identical across platforms.
- `git diff --check` clean.

## Risks / decisions

- **WPF `Page` in a panel**: requires a `Frame` per blade; `Frame` brings its own journal/navigation
  events. `NavigationUIVisibility=Hidden` + `JournalOwnership=OwnsJournal` avoids chrome and journal
  leakage, but it must be verified that `Page.Loaded` still fires for `BaseBladeService.MonitorViewLoadAsync`.
- **MAUI ContentPage cannot nest**: the overlay renders `Content`, so page-level resources/`Style` scoping
  must not rely on the page being in the visual tree. Modal remains the phone path by design.
- **WinUI RTL is app-wide**, not blade-only: switching from `WS_EX_LAYOUTRTL` to XAML `FlowDirection`
  affects every view. If the visual check regresses other screens, the fallback is to keep the native
  style and give `BladeView` an explicit `IsRightToLeft` input from the language setting instead.
- Existing types reused (rule 12): `IViewModelBladeView`, `ViewModelBladeView<TModel>`, `IViewModelBlade`,
  `BaseBladeService`, `GlobalizationConstants.IsRightToLeftLanguage`, `ApplyFlowDirection` extensions,
  and the WinUI `BladeView` as the behavioural reference. No new shared service is introduced beyond
  `IBladePaneHost`.

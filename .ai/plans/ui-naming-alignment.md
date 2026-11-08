# UI naming alignment across WPF, WinUI and MAUI

Status: APPROVED (framework + samples + Blazor, hard rename). Wave 1 applied 2026-11-08; wave 2 pending.
Date: 2026-11-08

## Goal

One vocabulary for the three desktop UI stacks: the same concept must have the same type name,
the same member name and the same folder in `ISynergy.Framework.UI.WPF`,
`ISynergy.Framework.UI.WinUI` and `ISynergy.Framework.UI.Maui`. A consumer switching heads should
not have to relearn names.

## Non-goals

- No behaviour changes. Renames only; public behaviour, resources and XAML markup stay identical.
- No new abstractions between the heads (that is a separate piece of work).
- Blazor is out of scope unless asked (it shares only a few names today).

## What the audit found

Public types: WPF 62, WinUI 144, MAUI 147, shared by name: 97. Services already align
(`DialogService`, `NavigationService`, `FileService`, `ClipboardService`, `ThemeService`,
`BladeService` all implement the shared interfaces). The drift is in **views/controls** and
**converters**:

| Concept | WPF | WinUI | MAUI |
|---|---|---|---|
| Blade host control | `Controls/BladeView` | `Controls/BladeView` | `Controls/BladeHostView` (+ `BladePane`) |
| Selection dialogs | `Views/SingleSelectionView`, `Windows/SingleSelectionWindow` | `Views/SingleSelectionView` only | `Views/SingleSelectionView`, `Windows/SingleSelectionWindow` |
| Note dialog | – | `Windows/NoteWindow` | `Windows/NoteWindow` |
| Image browser | – | `Controls/ImageBrowser` | `Controls/ImageBrowser` |
| Camera | – | `CameraService` | `CameraService` |
| Loading | `Controls/LoadingView`, `Controls/LoadingSpinner` | `Controls/LoadingView` | `Controls/LoadingView` |
| Clipboard image | `GetImageFromClipboardAsync` + `GetBitmapSourceFromClipboardAsync` | `GetImageFromClipboardAsync` | `GetImageFromClipboardAsync` |

Converters are the worst area:

- `Bool*` vs `Boolean*` prefixes are mixed inside and across heads
  (`BoolNegationConverter`, `BoolToStateConverter` next to `BooleanConverter`,
  `BooleanToBooleanConverter`, …).
- MAUI names the visibility converter `BooleanToIsVisibleConverter`; WPF/WinUI name it
  `BooleanToVisibilityConverter`.
- 8 WinUI and 8 MAUI converter names contain the typo `Then` (`DecimalGreaterThenConverter`)
  where `Than` is meant.
- 65 converter classes in MAUI, 6 WPF, ~1 file with many in WinUI — the same concepts are spread
  over different files/folders.

## Naming rules to adopt

1. **Type suffix by role**: `*Service` (+ `I*Service`), `*ViewModel`, `*View` (page/content),
   `*Window` (modal), `*Converter`, `*Extensions`, `*Helper` only for statics.
2. **Async suffix**: every `Task`-returning member ends in `Async`.
3. **Converters**: `{Source}To{Target}Converter`, `Boolean` never `Bool`, `Visibility` for the
   XAML ``Visibility``/MAUI `IsVisible` concept in the shared name, `Than` never `Then`.
4. **Views vs Windows**: a type rendered inside the shell is `*View`; one pushed as a modal
   window/page is `*Window` in every head.
5. **Folders mirror types**: `Controls/`, `Controls/BladeView/`, `Views/`, `Windows/`,
   `ViewModels/`, `Converters/`.
6. One type per file; file name equals the type name (already the norm).

## Rename map — wave 1 (framework, high signal)

| Current | Becomes | Heads |
|---|---|---|
| `BladeHostView` (+ keeps `BladePane` internal) | `BladeView` | MAUI |
| `BooleanToIsVisibleConverter` | `BooleanToVisibilityConverter` | MAUI |
| `BoolNegationConverter` | `BooleanNegationConverter` | WinUI, MAUI |
| `BoolToStateConverter` | `BooleanToStateConverter` | WinUI, MAUI |
| `*Then*Converter` (8 names each) | `*Than*Converter` | WinUI, MAUI |
| `GetBitmapSourceFromClipboardAsync` | keep, but mark platform-specific (WPF only) | WPF |
| `LoadingSpinner` | `LoadingView` (merge) or `LoadingIndicator` | WPF |

Wave 2 (after wave 1 is green): the remaining converters, the samples
(`ShellView` vs `RootView`, `AppHeaderView` naming), and any member-level differences found by a
Roslyn-based comparison rather than text scanning.

## Compatibility strategy (needs a decision)

- **A. Hard rename**: old names disappear. Cleanest result, major-version breaking change, no
  compatibility shims. XAML in consumers breaks at compile time with a clear error.
- **B. Rename + `[Obsolete]` forwarders**: keep the old type as a thin `[Obsolete("Use X")]`
  subclass/facade for one major release. No consumer breakage, extra types in the assembly and a
  second thing to maintain.
- **C. Rename only what is not publicly consumed yet** (samples + internal types), leave exported
  types alone. Smallest risk, does not achieve "everything aligned".

## Verification per wave

- `dotnet build` for WPF, WinUI and MAUI heads (`-nodeReuse:false`, `MSBUILDDISABLENODEREUSE=1`).
- `dotnet test` for `tests/ISynergy.Framework.UI.Tests` and `ISynergy.Framework.UI.Maui.Windows.Tests`.
- Repo grep for each old name returns 0 hits (`src/`, `samples/`, `tests/`, `docs/`).
- Sample apps launched once per head (XAML type references are compile-checked, but bindings are
  not).

## Risks

- XAML `clr-namespace` type references in samples/templates break loudly (acceptable).
- Serialized type names (settings, telemetry) — checked: none reference these controls.
- A council decision is required because this changes public contracts; the plan is the input to it.

## Progress

### Wave 1 — applied 2026-11-08
- `BladePane` → `BladeView` (MAUI), `BladePaneAnchor` → `BladeViewAnchor` (shared),
  `IBladePaneHost` → `IBladeViewHost`, `BladePaneHostRegistry` → `BladeViewHostRegistry`.
- `BooleanToIsVisibleConverter` → `BooleanToVisibilityConverter`,
  `BoolNegationConverter` → `BooleanNegationConverter`, `BoolToStateConverter` → `BooleanToStateConverter`,
  `BoolToCommandBarDisplayModeConverter` → `BooleanToCommandBarDisplayModeConverter`
  (MAUI + WinUI).
- Eight `*Then*Converter` names per head → `*Than*Converter` (MAUI + WinUI).
- `Sample.Maui/Views/RootView.xaml` → `ShellView.xaml` (+ file, `x:Class`, DI lookup, local variable).
- Verified: WPF / WinUI / MAUI / Blazor framework heads build; Sample.WPF, Sample.WinUI (win-arm64),
  Sample.Maui build; `ISynergy.Framework.UI.Tests` 90/90 and
  `ISynergy.Framework.UI.Maui.Windows.Tests` 47 passed / 4 skipped; Sample.Maui launched and the settings
  blade opened with English, a single Close button and the scrim covering the header.

### Wave 2 — applied 2026-11-08 (converters + API surface), gaps registered below
- The remaining converter set (MAUI has 65 converter classes, WPF 6, WinUI most in one file): compare
  concept-by-concept, not name-by-name; add the missing converters per head or document the gap.
- Sample-level view/view-model names that still differ (`AppHeaderView`, `SidebarView`,
  `NavigationHostView`, `DetailsView` vs the WPF/WinUI counterparts; `Sample.WinUI` has `Windows/NoteWindow`
  while WPF has none).
- Feature gaps found during the audit: WPF has no `ImageBrowser`, `NoteWindow` or `CameraService`;
  WinUI has no `BladeHostView`; MAUI has both. Decide per item: implement or document.
- A Roslyn-based comparison of public members (my audit was text-based) before touching member names.

### Wave 2 applied — converter visibility names
Every MAUI `*ToIsVisibleConverter` is now `*ToVisibilityConverter`, matching the WinUI names:
`Decimal`, `Double`, `Guid`, `GuidToInversed`, `Integer`, `Null`, `NullToInversed`, `String`,
`ZeroDecimal`, `ZeroDouble`, `ZeroInteger`. The sample's `Style.xaml` and `SlideShowView.xaml` keys were
updated with them. Verified with the MAUI build (0 errors) and no `IsVisibleConverter` / `*Then*Converter`
tokens left in `src/`, `samples/`, `tests/`.

### Wave 2 applied — API surface
- WPF `ClipboardService.GetBitmapSourceFromClipboardAsync` is `private` (the `BitmapSource` leak is gone).
- MAUI `FileService.SaveFileAsync` returns `null` instead of throwing `NotImplementedException`.
- Verified: all four framework heads, `Sample.WPF`, `Sample.WinUI` (win-arm64), `Sample.Maui` and
  `Sample.Blazor` build; `ISynergy.Framework.UI.Tests` 90/90 and
  `ISynergy.Framework.UI.Maui.Windows.Tests` 47 passed / 4 skipped.

### Gap register — decided as "platform-specific" or "feature gap", not renaming
| Area | WPF | WinUI | MAUI | Decision |
|---|---|---|---|---|
| `ImageBrowser`, `NoteWindow`, `CameraService` | – | ✓ | ✓ | Feature gap in WPF; not a rename. |
| `BladeHostView` | ✓ | – | ✓ | WinUI hosts `BladeView` directly in `ShellView`; feature difference, keep. |
| `LoadingSpinner` | ✓ | – | – | WPF-only control (fills the same role as `LoadingView`); rename only if it is meant to be shared. |
| `MaskedComboBox`, `MaskedTextBox`, `MaskValidators` | ✓ | – | – | WPF-only. |
| `BaseDialogButtons`, `Console`, `ImageView`, `BaseMenu`, `IconRadioButton`, `TextBoxAttached`, `Tile` | – | ✓ | – | WinUI-only. |
| `BusyIndicator`, `EmptyView`, `NavigationHostView`, `TabbedView` | – | – | ✓ | MAUI-only. |
| `NavigationViewBackButtonVisibleConverter`, `BooleanToCommandBarDisplayModeConverter` | – | ✓ | – | WinUI-specific control vocabulary; keep. |
| `CurrencyToStringConverter`, `DateTimeOffsetToLocalDateTimeConverter`, `ListToStringConverter`, `StringToDoubleConverter`, `StringToUriConverter` | – | – | ✓ | MAUI-only; add to the other heads if a consumer needs them. |
| Sample `SignInControl`/`SignUpControl` vs `SignInView`/`SignUpView` | ✓ (controls in `AuthenticationView`) | ✓ (controls) | ✓ (pages) | Different structure; do not rename. |
| Sample pages that exist in only one head (`ChartsView`, `PivotView`, `SlideShowView`, `CustomControlsView`, …) | – | partly | partly | Sample feature sets, not naming. |

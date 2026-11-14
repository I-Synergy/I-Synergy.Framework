# UI heads: structural and functional comparison

Audited 2026-11-08 (source-level, after the naming alignment in
`.ai/plans/ui-naming-alignment.md`). Scope: `ISynergy.Framework.UI` (shared contracts),
`ISynergy.Framework.UI.WPF`, `ISynergy.Framework.UI.WinUI`, `ISynergy.Framework.UI.Maui` and
`ISynergy.Framework.UI.Blazor`. The audit is text-based (types, declarations, platform API usage),
not a Roslyn symbol diff.

## 1. Structure

| | WPF | WinUI | MAUI | Blazor |
|---|---|---|---|---|
| Project | `ISynergy.Framework.UI.WPF` | `ISynergy.Framework.UI.WinUI` | `ISynergy.Framework.UI.Maui` | `ISynergy.Framework.UI.Blazor` |
| Platform switch | `<UseWPF>true</UseWPF>` | `<UseWinUI>true</UseWinUI>` | `<UseMaui>true</UseMaui>` | Razor class library |
| TFMs | `net10.0/11.0-windows7.0`, `-windows8.0`, `-windows10.0.26100.0` | `net10.0/11.0-windows10.0.26100.0` | `net10.0/11.0-android`, `-ios`, `-maccatalyst`, `-windows10.0.26100.0` per host | `net10.0`, `net11.0` |
| UI dependency | PresentationFramework, WindowsBase | Microsoft.WindowsAppSDK | Microsoft.Maui.Controls | none (browser) |
| `.cs` files | 55 | 93 | 88 | 27 |
| Public types | 62 | 144 | 147 | 27 |
| Folders | `Controls`, `Controls/BladeView`, `Converters`, `Extensions`, `Services`, `Styles`, `ViewModels`, `Views`, `Windows` | same shape + `Controls/Buttons`, `Controls/Menu`, `Controls/Tiles`, `Controls/TextBox`, `Controls/RadioButton` | same shape + `Platforms/*` (Android/iOS/MacCatalyst/Windows), `Controls`, `Converters`, `Helpers` | `Abstractions`, `Components`, `Constants`, `Extensions`, `Interfaces`, `Services`, `wwwroot` |

The three native heads share the same skeleton: **`Application`, `View`, `Window`, one `*Service` per
contract, `Converters/`, `ViewModels/Base`, `Controls/BladeView`**. Blazor is a different animal: a
Razor component library with JS interop and no native dialog/navigation/file services.

## 2. Contract coverage

Contracts live in `ISynergy.Framework.Mvvm/Abstractions/Services`
(`IBladeService`, `ICameraService`, `IClipboardService`, `IDialogService`, `IDownloadFileService`,
`IFileService`, `IMigrationService`, `INavigationService`, `IThemeService`, `IUpdateService`).

| Contract | WPF | WinUI | MAUI | Blazor |
|---|---|---|---|---|
| `IDialogService` | `DialogService : BaseDialogService` | `DialogService : BaseDialogService` | `DialogService` (implements directly) | – |
| `INavigationService` | `NavigationService : BaseNavigationService` | `NavigationService : BaseNavigationService` | `NavigationService : BaseNavigationService` | – |
| `IBladeService` | `BladeService : BaseBladeService` | `BladeService : BaseBladeService` | `BladeService : BaseBladeService` + `BladeViewHostRegistry` | – |
| `IThemeService` | `ThemeService` | `ThemeService : BaseThemeService` | `ThemeService : BaseThemeService` | `ThemeService` (JS interop) |
| `IFileService` | ✓ (`OpenFileDialog`/`SaveFileDialog`) | ✓ (`FileOpenPicker`, `StorageFolder`) | ✓ (`FilePicker`); `SaveFileAsync` returns `null` (no cross-platform save picker) | – |
| `IClipboardService` | ✓ (the `BitmapSource` helper is private now) | ✓ (`Clipboard.GetContent` + `BitmapDecoder`) | ✓ (`Clipboard`/`UIPasteboard`/`BitmapDecoder`) | – |
| `ICameraService` | **missing** | ✓ (`MediaCapture`) | ✓ (`MediaPicker`) | – |
| `IDispatcherService` | ✓ | ✓ | ✓ | – |
| Navigation menu contract | – | – | – | `BaseNavigationMenuService : INavigationMenuService` |

Blazor deliberately has no dialog/navigation/file/clipboard service: pages and routing come from
ASP.NET Core, and its sample (`Sample.Blazor`) implements its own drawer/dialog components.

## 3. Functional comparison per capability

### Dialogs
- **WPF**: modal `Window` types (`ShowDialogAsync<TWindow, TViewModel, TEntity>`), message boxes via
  `System.Windows.MessageBox` in `BaseDialogService`.
- **WinUI**: `ContentDialog` hosted on the window root (`dialog.XamlRoot = MainWindow.Content.XamlRoot`)
  for messages; modal `Window`s for entity dialogs.
- **MAUI**: `Page.DisplayAlertAsync` for `OK`/`OKCancel`/`YesNo`, `PushModalAsync` for window/dialog pages.
- Behaviour is equivalent for the contract (`ShowMessageAsync`, `ShowInformationAsync`,
  `ShowWarningAsync`, `ShowErrorAsync`, `ShowGreetingAsync` + typed dialogs), but the *presentation* is
  native per platform: WPF/WinUI dialogs live in their own HWND, MAUI pushes modal pages.

### Navigation
- **WPF / WinUI**: `BaseNavigationService` over a `Frame`; back-stack is the frame's.
- **MAUI**: `BaseNavigationService` over `INavigationHost` (`NavigationHostView`), which **reparents the
  page's content** rather than the page itself, because a MAUI `Page` cannot be a child of a layout.
  This is also why the blade pane has to reparent content (see below).

### Blades
- **WPF / WinUI**: `Controls/BladeView` is placed in the shell and renders the blade cards in-page.
- **MAUI**: `Controls/BladeView` (the pane) + `Controls/BladeHostView` (shell host) + `BladeViewHostRegistry`.
  The pane reparents each blade page's `Content` into a `ContentView` card.
  Two consequences observed in practice:
  - native controls inside an open blade keep the theme they were realized with, so `BladePane` now
    **dismisses open blades on a theme change**;
  - the pane covers the page header (the shell header is in-page), which is how the modal state is
    enforced on every head.

### Theme and accent
- **WPF**: swaps `DynamicThemeResourceDictionary` inside `Application.Resources.MergedDictionaries`;
  every `{DynamicResource}` in every live window re-resolves. `Themes.Default` follows
  `HKCU\…\Themes\Personalize` + `SystemEvents.UserPreferenceChanged`.
- **WinUI**: sets `Application.MainWindow.Content.RequestedTheme` (`ElementTheme.Light/Dark/Default`);
  WinUI re-resolves the whole window tree. System following via `ActualThemeChanged`.
- **MAUI**: `Application.UserAppTheme` (Unspecified = follow platform) + `SetApplicationColor` (accent
  dictionary) + native title-bar colours; system following via `Application.RequestedThemeChanged`.
- **Blazor**: JS module `applyTheme` + `matchMedia('(prefers-color-scheme: dark)')`, callback through a
  `[JSInvokable]` method.

All four compute the accent palette in `ISynergy.Framework.UI.Utilities.ThemeColorCalculator`, so
`Primary`/`Secondary`/`Tertiary`, the grey ramp and the contrast-safe `*Foreground` tokens are identical.

### Files, clipboard, camera
- **WPF**: `Microsoft.Win32.OpenFileDialog`/`SaveFileDialog`; clipboard through
  `System.Windows.Clipboard` (`GetData`), plus a WPF-only `GetBitmapSourceFromClipboardAsync` that leaks
  the WPF `BitmapSource` type into the public surface; no camera.
- **WinUI**: `FileOpenPicker` + `StorageFolder`/`FileIO` for saving; clipboard through
  `Clipboard.GetContent` + `BitmapDecoder`; camera through `MediaCapture`.
- **MAUI**: `FilePicker` for browsing; **`SaveFileAsync` is not implemented** (`NotImplementedException`);
  clipboard through platform APIs (`Clipboard.GetContent`, `UIPasteboard`, `BitmapDecoder`); camera
  through `MediaPicker`.

### Startup, busy and loading
- All heads: `Application` base with splash screen, `ICommonServices.BusyService`, `ErrorPresenter`.
- Loading controls: WPF has `LoadingView` **and** `LoadingSpinner`; WinUI/MAUI have `LoadingView`
  (MAUI adds `BusyIndicator`, `EmptyView`).

## 4. Controls and views

| Role | WPF | WinUI | MAUI |
|---|---|---|---|
| Blade pane | `BladeView` | `BladeView` | `BladeView` (+ `BladeHostView`) |
| Blade host wrapper | `BladeHostView` | – | `BladeHostView` |
| Dialog buttons | – | `BaseDialogButtons` | – |
| Image browsing | – | `ImageBrowser`, `ImageView` | `ImageBrowser` |
| Note dialog | – | `NoteWindow` | `NoteWindow` |
| Selection dialogs | `Single/MultipleSelectionView`, `Single/MultipleSelectionWindow` | `Single/MultipleSelectionView` | `Single/MultipleSelectionView`, `Single/MultipleSelectionWindow` |
| Navigation menu | – | `BaseMenu`, `Tile`, `IconRadioButton` | `NavigationItem`, `TabbedView`, `NavigationHostView` |
| Masked input | `MaskedComboBox`, `MaskedTextBox`, `MaskValidators`, `RegexValidator` | `TextBoxAttached` | – |
| Loading | `LoadingView`, `LoadingSpinner` | `LoadingView`, `Console` | `LoadingView`, `BusyIndicator`, `EmptyView` |

Converters: **WPF 11**, **WinUI 77**, **MAUI 74**, Blazor 0. Since the alignment pass the shared names
match (`Boolean*`, `*ToVisibilityConverter`, `*Than*Converter`); the numeric gap is converters that only
one head needs (e.g. WinUI's `NavigationViewBackButtonVisibleConverter`) or only one head has written
(e.g. MAUI's `CurrencyToStringConverter`, `StringToUriConverter`).

## 5. Functional gaps found

| Gap | Effect | Recommendation |
|---|---|---|
| ~~MAUI `FileService.SaveFileAsync` throws `NotImplementedException`~~ | **Aligned 2026-11-08**: returns `null` (same handling as a cancelled dialog) and the XML docs say MAUI has no save picker | Implement a platform save dialog later if needed |
| WPF has no `ICameraService` | Camera capture code cannot be shared; consumers must feature-detect | Add a WPF implementation (`MediaCapture`/`DirectShow`) or document WPF as unsupported |
| WPF has no `ImageBrowser` / `NoteWindow` | Two shell features exist only on WinUI/MAUI | Port or document |
| MAUI `Controls/Window` has no `ShowAsync<TEntity>()`, and `CloseAsync()` throws `NotSupportedException` | A dialog written against `IWindow` behaves differently on MAUI: showing is done by the dialog service (`PushModalAsync`), closing by the page stack | Align the member set: implement `CloseAsync` as a pop and document `ShowAsync` as dialog-service-only, or add both |
| WinUI has no `BladeHostView` | WinUI hosts `BladeView` directly in `ShellView`; any new shell must wire it itself | Acceptable; document the wiring |
| ~~WPF-only `GetBitmapSourceFromClipboardAsync`~~ | **Aligned 2026-11-08**: the helper is `private`, so `BitmapSource` no longer appears on the public surface | – |
| MAUI blade theme behaviour | Open blades are dismissed on a theme change because reparented native controls do not re-theme | Documented; the alternative is re-creating the blade view |
| Blazor missing services | `IDialogService`/`INavigationService`/`IFileService`/`IClipboardService` have no Blazor implementation | Documented as by design (ASP.NET Core routing + components) |

## 6. Parity summary

- **Structurally**: three native heads share one shape (Application, View, Window, Services,
  Converters, ViewModels/Base, BladeView); Blazor is a component library and cannot mirror it.
- **Functionally**: every head implements the same interfaces with the same method names (verified per
  contract); the differences are in *presentation mechanics* (Frame vs `INavigationHost`, HWND dialogs vs
  modal pages, dictionary swap vs `ElementTheme` vs `UserAppTheme`) and in the two gaps above
  (MAUI save-file, WPF camera).
- **Naming**: aligned as of this audit — `BladeView`, `Boolean*`, `*ToVisibilityConverter`, `*Than*Converter`.

## 7. Re-running the audit

```bash
# public types per head
grep -rhoE "^public (sealed |abstract |static |partial )*(class|interface|record) [A-Za-z0-9_]+" src/ISynergy.Framework.UI.* --include=*.cs
# services and the contracts they implement
grep -rn "^public .*class .*Service" src/ISynergy.Framework.UI.* --include=*.cs
# platform APIs per capability
grep -rn "ContentDialog\|DisplayAlertAsync\|FileOpenPicker\|FilePicker\|Clipboard.GetContent\|MediaCapture\|MediaPicker" src/ISynergy.Framework.UI.* --include=*.cs
```

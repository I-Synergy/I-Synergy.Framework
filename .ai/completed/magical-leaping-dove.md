# Fix: LanguageViewModel Memory Leak on External Modal Dismissal

**Status:** DONE
**Started:** 2026-06-11
**Completed:** 2026-06-11

## Summary

Fixed a memory leak where LanguageWindow (and NoteWindow) popups would leak ViewModel and DI scope instances when dismissed externally (clicking outside, Android back button, iOS swipe-down) because the ViewModel's `Closed` event was never fired in those paths.

## Changes Made

### `src/ISynergy.Framework.UI.Maui/Controls/Window.cs`
- Added `private bool _isViewModelClosed` flag to track whether close was initiated via the ViewModel's `Closed` event
- Updated the `ViewModel` setter to unsubscribe from the old ViewModel's `Closed` event and subscribe to the new one using `OnViewModelClosed` handler
- Added `OnViewModelClosed` handler that sets `_isViewModelClosed = true`
- Added `OnNavigatedFrom` override that calls `vm.CloseAsync()` when the ViewModel is still alive and hasn't been properly closed through its normal path
- Added `OnBackButtonPressed` override that routes through `vm.CloseAsync()` and returns `true` to prevent raw platform pop on Android
- The `Dispose` region remains exactly as-is (ViewModel lifecycle managed by DI scope)

### `src/ISynergy.Framework.UI.Maui/Windows/LanguageWindow.xaml`
- Changed root `<Grid>` to `<Grid Background="Transparent">` to fix hit-testing for the tap-outside gesture recognizer on all platforms

### `src/ISynergy.Framework.UI.Maui/Windows/NoteWindow.xaml`
- Same `Background="Transparent"` fix for consistency

## Key Design Decisions
- `OnNavigatedFrom` instead of `OnDisappearing`: fires only on actual navigation (not app backgrounding), eliminating false positives
- `IViewModel.IsDisposed` check prevents calling `CloseAsync` on an already-cleaned-up ViewModel
- `_isViewModelClosed` flag prevents double-close when close was already initiated via the normal pathway

## Follow-up Fix (2026-06-12): BindingContext Reference Leak

Even after the initial fix, the ViewModel still leaked because the Window's `BindingContext` property held a strong reference to the ViewModel. MAUI may retain the `ContentPage` after `PopModalAsync`, preventing GC of the ViewModel through the `BindingContext → ViewModel` reference chain.

### Additional Changes to `Window.cs`

- **`OnViewModelClosed`**: Now sets `BindingContext = null` when the ViewModel fires its `Closed` event. This breaks the reference chain as soon as the dialog is logically closed, before MAUI even pops the modal.
- **`Dispose(bool)`**: Now sets `BindingContext = null` as a safety net. Previously this method body was empty (only a comment saying "DO NOT dispose ViewModel here"). Clearing `BindingContext` is not the same as disposing the ViewModel — it just removes the strong reference so the GC can collect the ViewModel after the DI scope disposes it.

## Further Follow-up Fix (2026-06-12): ThemeViewModel Still Leaking

After the BindingContext fix, LanguageViewModel was properly cleaned up (1 instance) but ThemeViewModel still showed multiple leaked instances. ThemeWindow differs from LanguageWindow in several ways:

- Uses `x:DataType="{x:Null}"` on inner elements (Grid with RadioButtons and CollectionView) — these create **runtime bindings** instead of compiled bindings to work around a .NET 10 MAUI source generator bug (CS8820) with nullable binding paths like `SelectedItem.Theme`
- Uses `DynamicResource` for its `ControlTemplate` (LanguageWindow uses `StaticResource`)
- Has two-way bindings on `SelectedItem.Theme` (with converter) and `SelectedItem.Color` (CollectionView)

The runtime bindings created by `x:DataType="{x:Null}"` resolve through intermediate path objects (e.g., `SelectedItem` → `ThemeStyle`). These intermediate objects may be cached by MAUI's runtime binding infrastructure and not released when `BindingContext` is set to null. The cached `ThemeStyle` object has a `PropertyChanged` event subscribed to by the ViewModel (`SelectedItem_PropertyChanged`), keeping the ViewModel alive.

### Additional Changes to `Window.cs`

- **`OnViewModelClosed`**: Now also sets `Content = null` before clearing `BindingContext`. Detaching the visual tree releases all child bindings (both compiled and runtime), ensuring intermediate cached objects are released before the ViewModel is disposed by the DI scope.
- **`OnNavigatedFrom`**: Now unconditionally clears `BindingContext` after checking `_isViewModelClosed`. This covers edge cases where the platform dismisses the modal without going through the normal `CloseAsync()` → `Closed` event path.
- **`Dispose(bool)`**: Now also sets `Content = null` alongside `BindingContext = null` for defense-in-depth.

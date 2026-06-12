# Simplified DialogService API with Callbacks

**Status:** Plan
**Started:** 2026-06-12

## Context

Opening a dialog window currently requires ~50 lines of repetitive boilerplate per dialog type: create a DI scope, resolve ViewModel + Window from the scope, subscribe to `Submitted` event with a named `async void` handler, manually unsubscribe in that handler, subscribe to `Closed` to dispose the scope, optionally call `SetSelectedItem`, and handle exceptions. This pattern is repeated 6 times across framework `BaseShellViewModel` implementations and 15+ times across sample code.

The strongly-typed overload `ShowDialogAsync<TWindow, TViewModel, TEntity>(TEntity e)` already creates a single scope and resolves both ViewModel and Window from it — it just doesn't accept callbacks. We can build on this to provide a clean API.

## Desired API

```csharp
// With entity
_dialogService.ShowDialogAsync<IThemeWindow, ThemeViewModel, ThemeStyle>(
    style,
    async (result) => { /* save settings, maybe restart */ });

// Without entity (ViewModel sets its own initial state)
_dialogService.ShowDialogAsync<ILanguageWindow, LanguageViewModel, CultureInfo>(
    async (culture) => { /* save language, maybe restart */ });
```

## Files to Modify

1. `src/ISynergy.Framework.Mvvm/Abstractions/Services/IDialogService.cs` — add 2 new signatures
2. `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` — implement for MAUI
3. `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` — implement for WinUI
4. `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs` — implement for WPF
5. `src/ISynergy.Framework.UI.Maui/ViewModels/Base/BaseShellViewModel.cs` — simplify OpenLanguageAsync + OpenThemesAsync
6. `src/ISynergy.Framework.UI.WinUI/ViewModels/Base/BaseShellViewModel.cs` — same
7. `src/ISynergy.Framework.UI.WPF/ViewModels/Base/BaseShellViewModel.cs` — same
8. `tests/ISynergy.Framework.UI.Maui.Tests/Services/DialogServiceExceptionHandlingTests.cs` — stub implementation

## New Interface Signatures

```csharp
// With entity + callbacks
Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
    TEntity e,
    Func<TEntity, Task> submittedEventHandler,
    Func<Task>? cancelledEventHandler = null,
    Func<Task>? closedEventHandler = null)
    where TWindow : IWindow
    where TViewModel : IViewModelDialog<TEntity>;

// Without entity + callbacks
Task ShowDialogAsync<TWindow, TViewModel, TEntity>(
    Func<TEntity, Task> submittedEventHandler,
    Func<Task>? cancelledEventHandler = null,
    Func<Task>? closedEventHandler = null)
    where TWindow : IWindow
    where TViewModel : IViewModelDialog<TEntity>;
```

No ambiguity with existing overloads — parameter count and type of first parameter distinguish them.

**Three callbacks, three lifecycle points:**

| Callback | Fires when | Notes |
|----------|-----------|-------|
| `submittedEventHandler` | `Submitted` event | Receives `TEntity` result. Fires before scope disposal. |
| `cancelledEventHandler` | `Cancelled` event | Explicit cancel (CancelCommand / tap-outside). Fires before scope disposal. |
| `closedEventHandler` | `Closed` event | Fires on ANY dismissal (submit, cancel, back button, platform dismiss). Fires before scope disposal. |

## Implementation Pattern (shared across platforms)

1. Create a single DI scope (`_scopeFactory.CreateScope()`)
2. Resolve both `TViewModel` and `TWindow` from that scope
3. Call `vm.SetSelectedItem(entity)` if entity provided
4. Subscribe local `async void` functions to `Submitted` (and `Cancelled` if cancelledEventHandler provided)
5. Subscribe to `Closed` to: unsubscribe the handlers, call `closedEventHandler` if provided, then `scope.Dispose()`
6. Call `CreateDialogAsync(window, vm)` to show the dialog
7. Callback exceptions are caught and logged; they don't crash the close sequence

### `Closed` handler ordering

```csharp
viewmodel.Closed += async (_, _) =>
{
    // 1. Clean up our event subscriptions to prevent leaks
    viewmodel.Submitted -= OnSubmitted;
    if (cancelledEventHandler is not null)
        viewmodel.Cancelled -= OnCancelled;

    // 2. Invoke the user's closed callback (runs on ANY dismissal path)
    if (closedEventHandler is not null)
    {
        try { await closedEventHandler(); }
        catch (Exception ex) { _logger?.LogError(ex, "Close callback failed..."); }
    }

    // 3. Dispose the scope — ViewModel + Window are disposed, BindingContext cleared
    scope.Dispose();
};
```

## Key Design Decisions

- **Three callbacks cover all lifecycle points**: `submittedEventHandler` (Submitted), `cancelledEventHandler` (Cancelled), `closedEventHandler` (Closed). The `closedEventHandler` fires on every dismissal path — submit, cancel, back button, platform dismiss — making it the right place for "always run" cleanup or navigation logic.
- **Callbacks are `Func<TEntity, Task>` / `Func<Task>`** — async-safe, matches existing pattern where callers use `await _dialogService.ShowMessageAsync(...)` inside handlers
- **Callbacks run before scope disposal** — `Submitted`/`Cancelled` fire synchronously before `CloseAsync()`; `closedEventHandler` is awaited inside `Closed` before `scope.Dispose()`
- **Local functions for handlers** — properly unsubscribed in `Closed` handler via `-=`, no leak
- **Callback errors isolated** — each callback wrapped in try/catch/log, matches existing `ViewModelSelectable.OnSubmitted` pattern
- **`cancelledEventHandler` and `closedEventHandler` are optional** — most callers only need submit handling
- **All existing overloads preserved** — backward compatible

## Caller Simplification Example

**Before** (48 lines across 2 methods):
```csharp
protected virtual Task OpenThemesAsync() {
    var scope = _scopeFactory.CreateScope();
    var themeVM = scope.ServiceProvider.GetRequiredService<ThemeViewModel>();
    var themeWindow = (IWindow)scope.ServiceProvider.GetRequiredService(typeof(IThemeWindow));
    var style = new ThemeStyle { Theme = ..., Color = ... };
    themeVM.Submitted += ThemeVM_Submitted;
    themeVM.Closed += (_, _) => { scope.Dispose(); };
    themeVM.SetSelectedItem(style);
    return _dialogService.ShowDialogAsync(themeWindow, themeVM);
}
private async void ThemeVM_Submitted(object? sender, SubmitEventArgs<ThemeStyle> e) {
    if (sender is ThemeViewModel vm) vm.Submitted -= ThemeVM_Submitted;
    // ... business logic
}
```

**After** (14 lines, no async void handler):
```csharp
protected virtual Task OpenThemesAsync() =>
    _dialogService.ShowDialogAsync<IThemeWindow, ThemeViewModel, ThemeStyle>(
        new ThemeStyle { Theme = ..., Color = ... },
        async themeStyle => {
            _settingsService.LocalSettings.Theme = themeStyle.Theme;
            _settingsService.LocalSettings.Color = (themeStyle.Color ?? "").ToLowerInvariant();
            if (_settingsService.SaveLocalSettings() &&
                await _dialogService.ShowMessageAsync(...) == MessageBoxResult.Yes)
                _commonServices.RestartApplication();
        });
```

## Verification

1. Build all platforms: `dotnet build --no-restore` — expect 0 errors
2. Run MAUI tests: `dotnet test tests/ISynergy.Framework.UI.Maui.Tests --no-restore`
3. Open Language window → select language → submit → verify language saved
4. Open Language window → click outside → verify dialog closes, no leak
5. Open Theme window → change theme/color → submit → verify theme applied
6. Open Theme window → click outside → verify dialog closes, no leak
7. Memory profiler: verify only 1 instance of each ViewModel after close + reopen

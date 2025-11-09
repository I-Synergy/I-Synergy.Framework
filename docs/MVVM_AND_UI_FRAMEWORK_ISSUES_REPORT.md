# MVVM and UI Framework Issues Report

This document identifies potential exceptions, memory leaks, and issues found in the MVVM framework and UI framework implementations (Maui, WinUI, WPF, UWP).

## Critical Issues

### 1. Memory Leak: Cancelled/Closed Events Not Cleared in ViewModel.Dispose()

**Location:** `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs`

**Issue:** The `Cancelled` and `Closed` events are never cleared in the `Dispose` method. If subscribers don't explicitly unsubscribe, they will keep references to disposed ViewModels, preventing garbage collection.

**Current Code:**
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _commonServices.ScopedContextService.ScopedChanged -= ScopedContextService_ScopedChanged;
        PropertyChanged -= OnPropertyChanged;
        CloseCommand?.Dispose();
        CancelCommand?.Dispose();
        base.Dispose(disposing);
        // ❌ Missing: Cancelled and Closed events are not cleared
    }
}
```

**Recommendation:** Clear event handlers in Dispose:
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _commonServices.ScopedContextService.ScopedChanged -= ScopedContextService_ScopedChanged;
        PropertyChanged -= OnPropertyChanged;
        
        // Clear event handlers to prevent memory leaks
        Cancelled = null;
        Closed = null;
        
        CloseCommand?.Dispose();
        CancelCommand?.Dispose();
        base.Dispose(disposing);
    }
}
```

**Severity:** High - Can cause memory leaks in long-running applications

---

### 2. Exception Handling: async void Without Try-Catch

**Location:** `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs:121`

**Issue:** The `ScopedContextService_ScopedChanged` method is `async void` without exception handling. If an exception occurs, it will crash the application.

**Current Code:**
```csharp
private async void ScopedContextService_ScopedChanged(object? sender, Core.Events.ReturnEventArgs<bool> e)
{
    if (e.Value)
    {
        IsInitialized = false;
        Cleanup();
        if (!IsDisposed)
        {
            await InitializeAsync(); // ❌ No exception handling
        }
    }
}
```

**Recommendation:** Add try-catch with proper logging:
```csharp
private async void ScopedContextService_ScopedChanged(object? sender, Core.Events.ReturnEventArgs<bool> e)
{
    try
    {
        if (e.Value)
        {
            IsInitialized = false;
            Cleanup();
            if (!IsDisposed)
            {
                await InitializeAsync();
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in ScopedContextService_ScopedChanged");
        var exceptionHandlerService = _commonServices.ExceptionHandlerService;
        exceptionHandlerService?.HandleException(ex);
    }
}
```

**Severity:** High - Can cause application crashes

---

### 3. Memory Leak: DialogService Event Handler Not Unsubscribed

**Location:** `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs:430`

**Issue:** The `viewModelClosedHandler` is subscribed to `viewmodel.Closed` but only unsubscribed if initialization fails. If the dialog closes normally, the handler is never unsubscribed, causing a memory leak.

**Current Code:**
```csharp
viewmodel.Closed += viewModelClosedHandler;
try
{
    await viewmodel.InitializeAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error initializing viewmodel");
    viewmodel.Closed -= viewModelClosedHandler; // ✅ Only unsubscribes on error
    return;
}
await OpenDialogAsync(window);
// ❌ Handler is never unsubscribed if dialog closes normally
```

**Recommendation:** Unsubscribe in the handler itself or track handlers for cleanup:
```csharp
EventHandler viewModelClosedHandler = async (s, e) =>
{
    try
    {
        if (s is IViewModel vm)
            vm.Closed -= viewModelClosedHandler; // Unsubscribe first
        
        window.ViewModel?.Dispose();
        await window.CloseAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in viewmodel closed handler");
    }
};
```

**Also affects:**
- `src/ISynergy.Framework.UI.UWP/Services/DialogService.cs:431`
- `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs:592`

**Severity:** Medium - Can cause memory leaks with multiple dialogs

---

### 4. Memory Leak: NavigationService Blade Closed Handler

**Location:** `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs:245`

**Issue:** The `Viewmodel_Closed` handler is subscribed but the unsubscription logic may not always execute if the blade is removed in other ways.

**Current Code:**
```csharp
void Viewmodel_Closed(object? sender, EventArgs e)
{
    if (sender is IViewModelBlade viewModel)
    {
        viewModel.Closed -= Viewmodel_Closed; // ✅ Unsubscribes in handler
        RemoveBlade(viewModel.Owner, viewModel);
    }
}
bladeVm.Closed += Viewmodel_Closed;
```

**Status:** This appears to be handled correctly, but verify that `RemoveBlade` always triggers the closed event.

**Severity:** Low - Appears handled, but verify edge cases

---

## Medium Priority Issues

### 5. ObservableClass Event Handler Cleanup Logic

**Location:** `src/ISynergy.Framework.Core/Base/ObservableClass.cs:268-275`

**Issue:** The event handler cleanup uses `GetInvocationList()` and removes each delegate individually. This approach works but is less efficient than simply setting the event to null.

**Current Code:**
```csharp
if (_propertyChanged is not null)
{
    foreach (var @delegate in _propertyChanged.GetInvocationList())
    {
        if (@delegate is not null)
            _propertyChanged -= (PropertyChangedEventHandler)@delegate;
    }
}
```

**Recommendation:** For field-backed events, simply clear the field:
```csharp
_propertyChanged = null;
```

**Note:** This is acceptable if the event is field-backed (which it is: `protected PropertyChangedEventHandler? _propertyChanged;`)

**Severity:** Low - Works correctly but could be simplified

---

### 6. Missing Exception Handling in InitializeAsync

**Location:** Multiple ViewModels

**Issue:** `InitializeAsync()` is virtual and can throw exceptions, but callers may not always handle them properly.

**Recommendation:** Ensure all callers of `InitializeAsync()` have proper exception handling. This appears to be handled in most places, but verify all call sites.

**Severity:** Low - Most callers handle exceptions

---

### 7. Potential Race Condition in Command Disposal

**Location:** `src/ISynergy.Framework.Mvvm/Commands/Base/BaseAsyncRelayCommand.cs`

**Issue:** When disposing a command, there's a check for task status, but if a task is still running, it's left to be cleaned up by GC. This is acceptable but could be improved.

**Current Code:**
```csharp
// If task is still running, the continuation in MonitorTask will handle cleanup
```

**Status:** This is documented and appears intentional. The continuation will clear the reference when the task finishes.

**Severity:** Low - Documented behavior, acceptable

---

## UI Framework Specific Issues

### 8. WinUI View Dispose Pattern

**Location:** `src/ISynergy.Framework.UI.WinUI/Controls/View.cs`

**Issue:** The View disposes the ViewModel, but if the ViewModel is shared or managed elsewhere, this could cause issues.

**Current Code:**
```csharp
protected virtual void Dispose(bool disposing)
{
    if (disposing)
    {
        ViewModel?.Dispose(); // ⚠️ Assumes View owns ViewModel
    }
}
```

**Recommendation:** Verify that Views always own their ViewModels. If ViewModels are shared, this could cause double-disposal.

**Severity:** Low - Verify ownership model

---

### 9. WPF View Missing Event Unsubscription

**Location:** `src/ISynergy.Framework.UI.WPF/Controls/View.cs`

**Issue:** Unlike Blazor Views, WPF Views don't subscribe to ViewModel events, so there's no unsubscription needed. However, verify that WPF binding doesn't create implicit subscriptions that need cleanup.

**Status:** Appears correct - WPF uses data binding which doesn't require explicit event subscriptions

**Severity:** Low - Verify no implicit subscriptions

---

### 10. Maui View Dispose

**Location:** `src/ISynergy.Framework.UI.Maui/Controls/View.cs`

**Issue:** Similar to WinUI - verify ViewModel ownership model.

**Status:** Same as WinUI - verify ownership

**Severity:** Low - Verify ownership model

---

## Fixes Applied

### High Priority (Fixed)
1. ✅ **FIXED** - Clear `Cancelled` and `Closed` events in `ViewModel.Dispose()`
   - **File:** `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs`
   - **Change:** Added `Cancelled = null;` and `Closed = null;` in Dispose method

2. ✅ **FIXED** - Add exception handling to `ScopedContextService_ScopedChanged`
   - **File:** `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs`
   - **Change:** Wrapped async void method body in try-catch with proper logging

### Medium Priority (Fixed)
3. ✅ **FIXED** - Fix DialogService event handler unsubscription in Maui
   - **File:** `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs`
   - **Change:** Added unsubscription in initialization error path and navigation error path

4. ✅ **FIXED** - Fix DialogService event handler unsubscription edge cases in WinUI and UWP
   - **Files:** 
     - `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs`
     - `src/ISynergy.Framework.UI.UWP/Services/DialogService.cs`
   - **Change:** Added unsubscription in `OpenDialogAsync` error path to handle cases where dialog opening fails

5. ✅ **FIXED** - Fix NavigationService blade handler cleanup when view creation fails
   - **Files:**
     - `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs`
     - `src/ISynergy.Framework.UI.UWP/Services/NavigationService.cs`
     - `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs`
   - **Change:** Added try-catch in `SetupBladeAsync` to ensure handler is unsubscribed if view creation fails or returns null

## Recommendations Summary

### High Priority (Completed)
1. ✅ Clear `Cancelled` and `Closed` events in `ViewModel.Dispose()`
2. ✅ Add exception handling to `ScopedContextService_ScopedChanged`

### Medium Priority (Completed)
3. ✅ Fix DialogService event handler unsubscription in Maui
4. ✅ Fix DialogService event handler unsubscription edge cases in WinUI and UWP
5. ✅ Fix NavigationService blade handler cleanup when view creation fails (WinUI, UWP, WPF)

### Low Priority (Completed)
5. ✅ Simplify ObservableClass event cleanup (optimized)
6. ✅ Verify all InitializeAsync callers have exception handling (added where needed)
7. ✅ Document ViewModel ownership model for UI frameworks (documentation created)
8. ⚠️ Consider using WeakEventManager for long-lived event subscriptions

---

## Additional Notes

### Weak Event Infrastructure
The codebase has weak event infrastructure (`WeakEventHandler`, `WeakEventSource`, `WeakEventListener`) but it's not being used for `Cancelled` and `Closed` events. Consider using weak events for:
- Long-lived ViewModels
- Cross-ViewModel event subscriptions
- Services subscribing to ViewModel events

### Testing Recommendations
1. Add unit tests for ViewModel disposal to verify events are cleared
2. Add memory leak tests using weak references
3. Test exception scenarios in async void methods
4. Test dialog lifecycle to verify event handler cleanup

---

## Files Changed

### High Priority Fixes
1. ✅ `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs` - Clear events, add exception handling

### Medium Priority Fixes
2. ✅ `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` - Fix handler unsubscription
3. ✅ `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` - Fix handler unsubscription edge cases
4. ✅ `src/ISynergy.Framework.UI.UWP/Services/DialogService.cs` - Fix handler unsubscription edge cases
5. ✅ `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` - Fix blade handler cleanup
6. ✅ `src/ISynergy.Framework.UI.UWP/Services/NavigationService.cs` - Fix blade handler cleanup
7. ✅ `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs` - Fix blade handler cleanup

### Low Priority Fixes
8. ✅ `src/ISynergy.Framework.Core/Base/ObservableClass.cs` - Simplify event cleanup logic
9. ✅ `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs` - Add exception handling for InitializeAsync
10. ✅ `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` - Add exception handling for InitializeAsync
11. ✅ `src/ISynergy.Framework.UI.UWP/Services/NavigationService.cs` - Add exception handling for InitializeAsync
12. ✅ `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs` - Add exception handling for InitializeAsync
13. ✅ `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs` - Add exception handling for InitializeAsync
14. ✅ `docs/VIEWMODEL_OWNERSHIP_MODEL.md` - Created comprehensive ownership documentation

---

*Report generated: Comprehensive review of MVVM and UI frameworks*


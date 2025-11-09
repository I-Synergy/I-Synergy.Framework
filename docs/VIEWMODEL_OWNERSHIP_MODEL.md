# ViewModel Ownership Model

This document describes the ViewModel ownership and disposal model across all UI frameworks in the I-Synergy Framework.

## Overview

In the MVVM pattern, it's crucial to understand who owns ViewModels and who is responsible for their disposal. This prevents memory leaks and double-disposal issues.

## General Principles

1. **Views own their ViewModels**: In most cases, a View is responsible for creating and disposing its ViewModel.
2. **One owner per ViewModel**: Each ViewModel should have exactly one owner responsible for disposal.
3. **Disposal on View disposal**: ViewModels are disposed when their owning View is disposed.

## Framework-Specific Ownership Models

### WinUI

**Ownership**: View owns ViewModel

**Implementation**:
- ViewModels are created by the dependency injection container (scoped lifetime)
- Views receive ViewModels through constructor injection or property assignment
- Views dispose ViewModels in their `Dispose(bool disposing)` method

**Code Location**: `src/ISynergy.Framework.UI.WinUI/Controls/View.cs`

```csharp
protected virtual void Dispose(bool disposing)
{
    if (disposing)
    {
        ViewModel?.Dispose(); // View owns and disposes ViewModel
    }
}
```

**Notes**:
- ViewModels are typically scoped to the View's lifetime
- NavigationService may hold references to ViewModels in the backstack, but Views remain the owners
- When a View is removed from navigation, its ViewModel is disposed by the View

### UWP

**Ownership**: View owns ViewModel

**Implementation**:
- Same pattern as WinUI
- ViewModels are scoped and disposed by Views

**Code Location**: `src/ISynergy.Framework.UI.UWP/Controls/View.cs`

**Notes**:
- Identical to WinUI implementation
- Follows the same disposal pattern

### WPF

**Ownership**: View owns ViewModel

**Implementation**:
- ViewModels are created by DI container
- Views dispose ViewModels in `Dispose(bool disposing)`

**Code Location**: `src/ISynergy.Framework.UI.WPF/Controls/View.cs`

**Notes**:
- WPF Views use data binding which doesn't require explicit event subscriptions
- ViewModel disposal is handled by the View

### MAUI

**Ownership**: View owns ViewModel

**Implementation**:
- ViewModels are created by DI container
- Views dispose ViewModels in `Dispose(bool disposing)`

**Code Location**: `src/ISynergy.Framework.UI.Maui/Controls/View.cs`

**Notes**:
- MAUI Views follow the same pattern as other frameworks
- ViewModels are scoped to View lifetime

### Blazor

**Ownership**: Component owns ViewModel

**Implementation**:
- ViewModels are injected via `[Inject]` attribute
- Components dispose ViewModels in `Dispose()` method
- Components also unsubscribe from ViewModel events

**Code Location**: 
- `src/ISynergy.Framework.UI.Blazor/Components/Controls/View.cs`
- `src/ISynergy.Framework.AspNetCore.Blazor/Components/Controls/View.cs`

**Notes**:
- Blazor components have explicit lifecycle management
- Components subscribe to ViewModel `PropertyChanged` and command `CanExecuteChanged` events
- All subscriptions are properly unsubscribed in `Dispose()`

## Special Cases

### NavigationService Backstack

**Ownership**: NavigationService temporarily holds references, but Views remain owners

**Implementation**:
- NavigationService maintains a backstack of ViewModels for navigation history
- When ViewModels are removed from backstack (e.g., backstack limit exceeded), NavigationService disposes them
- However, the primary owner is still the View

**Code Location**: 
- `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs`
- `src/ISynergy.Framework.UI.UWP/Services/NavigationService.cs`
- `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs`

**Notes**:
- NavigationService disposes ViewModels only when cleaning up the backstack
- This is a secondary disposal mechanism, not the primary ownership

### DialogService

**Ownership**: Dialog Window owns ViewModel

**Implementation**:
- DialogService creates dialogs with ViewModels
- Dialog Windows dispose their ViewModels when closed
- DialogService subscribes to ViewModel `Closed` event to handle cleanup

**Code Location**: 
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs`
- `src/ISynergy.Framework.UI.UWP/Services/DialogService.cs`
- `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs`
- `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs`

**Notes**:
- Dialog ViewModels are disposed when the dialog closes
- Event handlers are properly unsubscribed to prevent memory leaks

### Blade Views

**Ownership**: Blade View owns its ViewModel, but Owner ViewModel manages the collection

**Implementation**:
- Blade ViewModels are owned by their Blade Views
- The owner ViewModel (IViewModelBladeView) maintains a collection of Blade Views
- When a blade is removed, its View disposes the ViewModel

**Code Location**: NavigationService blade methods

**Notes**:
- Blade ViewModels have a reference to their Owner
- Owner doesn't own the ViewModel, just manages the collection

## Disposal Patterns

### Standard View Disposal

```csharp
protected virtual void Dispose(bool disposing)
{
    if (disposing)
    {
        ViewModel?.Dispose(); // View owns ViewModel
    }
}
```

### Blazor Component Disposal

```csharp
protected virtual void Dispose(bool disposing)
{
    if (disposing)
    {
        if (ViewModel is not null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            UnsubscribeFromViewModelCommands();
            ViewModel.Dispose(); // Component owns ViewModel
        }
    }
}
```

## Best Practices

1. **Never share ViewModels between Views**: Each View should have its own ViewModel instance
2. **Always dispose in View disposal**: Views must dispose their ViewModels
3. **Unsubscribe from events**: If Views subscribe to ViewModel events, unsubscribe in Dispose
4. **Use scoped lifetime**: ViewModels should be registered with scoped lifetime in DI container
5. **Avoid manual disposal**: Let the framework handle disposal through the View lifecycle

## Common Pitfalls

1. **Double disposal**: Disposing a ViewModel that's already been disposed
   - **Solution**: Check `IsDisposed` property before disposal
   - **Solution**: Ensure only one owner disposes

2. **Memory leaks from events**: Not unsubscribing from ViewModel events
   - **Solution**: Always unsubscribe in Dispose method
   - **Solution**: Use weak events for long-lived subscriptions

3. **Shared ViewModels**: Sharing ViewModels between multiple Views
   - **Solution**: Create separate ViewModel instances for each View
   - **Solution**: Use scoped DI registration

4. **Disposing in wrong order**: Disposing ViewModel before unsubscribing from events
   - **Solution**: Unsubscribe first, then dispose

## Verification Checklist

When implementing a new View or ViewModel:

- [ ] View disposes ViewModel in `Dispose(bool disposing)`
- [ ] View unsubscribes from ViewModel events (if subscribed)
- [ ] ViewModel is registered with scoped lifetime in DI
- [ ] No ViewModel sharing between Views
- [ ] Event handlers are properly unsubscribed
- [ ] Commands are disposed (handled by ViewModel base class)

## Related Documentation

- [MVVM Framework Documentation](../src/ISynergy.Framework.Mvvm/readme.md)
- [MVVM and UI Framework Issues Report](./MVVM_AND_UI_FRAMEWORK_ISSUES_REPORT.md)

---

*Last updated: After comprehensive review and fixes*


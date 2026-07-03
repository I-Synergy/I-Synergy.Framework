# Fix MAUI Memory Leak Audit Findings

Status: APPROVED — IMPLEMENTED

## Cross-Platform Scope

Before implementing, I checked whether the same leak patterns exist in WinUI, WPF, and Blazor:

| Platform | Finding 1 (Root Replacement) | Finding 2 (Cursor) |
|---|---|---|
| **MAUI** | **CONFIRMED** — standalone impl, no scoping, no teardown | **Windows only** — CursorExtensions MAUI-only |
| **WinUI** | NOT vulnerable — inherits `BaseNavigationService` with `IServiceScopeFactory` + `NavigationFrame` scope-per-navigation | N/A — no cursor extensions exist |
| **WPF** | NOT vulnerable — inherits `BaseNavigationService` with `IServiceScopeFactory` + `NavigationFrame` scope-per-navigation | N/A — no cursor extensions exist |
| **Blazor** | N/A — no `NavigationService`, Blazor has its own navigation model | N/A — no cursor extensions |

**Why WinUI/WPF are safe:** Their `NavigationService` inherits from `BaseNavigationService` (in `ISynergy.Framework.UI`) which:
- Injects `IServiceScopeFactory` and creates a new `IServiceScope` per navigation via `NavigationFrame`
- Disposes old `_modalScope` before creating a new one in `NavigateModalAsync` (line 678: `_modalScope?.Dispose()`)
- Resolves views from the scoped provider, not `ServiceLocator.Default`
- Has dead-handler cleanup on `BackStackChanged` (catches `ObjectDisposedException`, removes dead subscribers)
- Properly disposes all scopes and backstack frames in its `Dispose()` method

The MAUI `NavigationService` is a **completely independent implementation** that doesn't inherit from `BaseNavigationService` and has none of these protections. It's the sole outlier.

**This plan is MAUI-only.** All files modified are under `src/ISynergy.Framework.UI.Maui/` and `samples/Sample.Maui/`.

## Context

A static analysis audit (`audits/MAUI Leak Intelligence Audit I-Synergy.Framework.md`) identified two memory leak conditions. Code review confirmed both findings, and deeper analysis revealed the mechanisms are even more deterministic than the audit initially described.

### Finding 1: Root Page Replacement — Root-Scoped Transient IDisposable Retention (Near-Certain)

**Actual mechanism (stronger than the audit's BackStackChanged story):**

`ShellView` has no `[Lifetime]` attribute, so it resolves as **Transient** from the DI container. `NavigationExtensions.CreatePage<ShellViewModel>()` resolves `ShellView` fresh from `ServiceLocator.Default` (the raw root `IServiceProvider`) on every sign-in. Microsoft.Extensions.DI has documented behavior: when a transient `IDisposable` is resolved directly from the root provider (not from a created scope), the root provider captures that instance in its internal disposal list **permanently** — it's never released until the container itself is disposed at app shutdown.

Every sign-in/sign-out cycle calls `NavigateModalAsync<ShellViewModel>()` which:
1. Calls `CreatePage<ShellViewModel>()` → resolves a new `ShellView` from root `ServiceLocator.Default`
2. The root DI container roots that transient `ShellView` forever in its disposal list
3. `ShellView` holds `ShellViewModel` (via BindingContext), which holds services, commands, PrimaryItems, SecondaryItems, tab pages with handlers
4. Assigns `Application.Current.Windows[0].Page = page`

The `BackStackChanged` event subscription contributes but is secondary — the root container's disposal-list capture is the primary rooting mechanism, and it's fully deterministic.

**The fix follows an existing codebase pattern:** `SignInView` (in the sample) already has `[Lifetime(Lifetimes.Singleton)]` — this exact pattern, just missing on `ShellView`.

### Finding 2: Windows Cursor Delegate Accumulation (Mechanically Provable)

**Confirmed:** The `-=` removal in `CursorExtensions.cs` is a structural no-op. `ViewOnPointerEntered`/`ViewOnPointerExited` are local functions declared fresh every call, so the removal can never reference a delegate that was actually added in a prior call. Handlers accumulate on the native `UIElement` without bound every time `IsEnabled` toggles (because `CursorBehavior` re-applies the cursor on `IsEnabled` changes).

**iOS/MacCatalyst are actually clean:** They properly remove old interactions/recognizers before adding new ones. The reapplication logic is correct. Only an unload-time cleanup path is missing.

**Additional risk:** `CursorBehavior` is wired into the default button style app-wide (`Style.xaml`). If any of those controls end up inside a virtualized `CollectionView` on Windows, container recycling would turn this from wasted work into keeping recycled items' old `VisualElement`/`BindingContext` graphs alive. No direct evidence of this in the repo yet, but it's a live risk given the broad style application.

### Key Architectural Context

- `NavigationService` registered as **singleton** (`MauiAppBuilderExtensions.cs:125`)
- `View` and `TabbedView` are `[Lifetime(Singleton)]` — app-lifetime only
- `SignInView` already has `[Lifetime(Lifetimes.Singleton)]` — pattern to replicate for `ShellView`
- `ShellView` has no `[Lifetime]` attribute → **Transient by default** → root-container disposal-list capture
- `DialogService` properly injects `IServiceScopeFactory` — existing scoping pattern to follow
- `CursorBehavior` is a **static attached property** (not `Behavior<T>`) — no `OnDetachingFrom` lifecycle
- `LoadingView` uses `Unloaded` + `OnDisappearing` for cleanup — existing pattern to follow
- Android `CursorExtensions` sets a property only — no leak, no changes needed
- **Spot-check confirmed clean:** Application static hooks, SingleSelectionWindow, and other "reviewed but not reported" items hold up as genuinely clean — no false negatives

---

## Implementation Plan

### Phase 1: Fix Finding 1 — Stop Transient IDisposable Root-Container Capture

**Goal:** Prevent MS.DI from permanently rooting every ShellView + ShellViewModel + tab pages + native handler graph on each sign-in.

#### Step 1.1: Add `[Lifetime(Lifetimes.Singleton)]` to ShellView

**File:** `samples/Sample.Maui/Views/ShellView.xaml.cs`

Add the attribute to the class (matching `SignInView`'s existing pattern):
```csharp
[Lifetime(Lifetimes.Singleton)]
public partial class ShellView : Shell, IShellView
```

**Rationale:** This is the minimal, proven fix. The codebase already does this for `SignInView`, `View`, `TabbedView`, and other long-lived pages. Without it, every `CreatePage<ShellViewModel>()` call creates a new Transient `ShellView` that the root DI container permanently roots. With `[Singleton]`, the same instance is reused, and the root container's disposal list only holds one reference (which is correct — it lives as long as the app).

**This alone stops the leak.** The deeper architectural fix below is defense-in-depth.

#### Step 1.2: Dispose old root page + ViewModel on replacement (defense-in-depth)

**File:** `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs`

In `NavigateModalAsync<TViewModel>` (line 139), restructure to capture the old page before replacement and clean it up after:

```csharp
var mainPage = GetMainPage();
if (mainPage is null)
    throw new InvalidOperationException(DefaultPageNotAvailable);

// Capture the old root page BEFORE replacement
var oldPage = Application.Current.Windows[0].Page;

// Assign the new page
Application.Current.Windows[0].Page = page;

// Clean up the old page graph to release event subscriptions and native resources
CleanupPage(oldPage);
```

Add a private static cleanup method:

```csharp
private static void CleanupPage(Page? page)
{
    if (page is null)
        return;

    // 1. Dispose the ViewModel first — this unsubscribes from singleton service
    //    events like NavigationService.BackStackChanged (BaseShellViewModel.Dispose)
    //    and MessengerService (ViewModel.Dispose).
    if (page is IView view && view.ViewModel is IDisposable disposableViewModel)
    {
        disposableViewModel.Dispose();
    }

    // 2. Clear BindingContext directly on the Page to break the reference chain.
    //    We do NOT use IView.ViewModel setter because ShellView.ViewModel setter
    //    calls SetBinding(...), which would try to evaluate bindings on a
    //    detached page. Direct BindingContext = null avoids that side effect.
    page.BindingContext = null;

    // 3. Disconnect the handler to release native platform views.
    //    This mirrors the existing pattern at line 150 (page.Handler = null
    //    for TabbedPage workaround) and LoadingView.cs:273.
    page.Handler = null;
}
```

**Design note — Why `BindingContext = null` directly instead of via `IView.ViewModel`:**
`ShellView.ViewModel` setter calls `SetBinding(Shell.TitleProperty, new Binding(...))`. On a detached page, this binding evaluation can fail. Nulling `BindingContext` directly on the `Page` base class achieves the reference release without triggering per-view setter logic.

**For singleton views (SignInView):** After cleanup, `BindingContext` is null. When `CreatePage` reuses the singleton, the code at `NavigationExtensions.cs:59` checks `if (resolvedView.ViewModel is null)` — this will be true (because `ViewModel` getter reads `BindingContext`), so a fresh ViewModel is resolved. Correct behavior, no code change needed.

#### Step 1.3 (Deferred): Scoped page/viewmodel resolution in NavigationExtensions

This is the audit's full recommendation — use `IServiceScope` per navigation. Tracked separately because:
- `CreatePage` is static — requires converting to instance method or injecting `IServiceScopeFactory`
- The Singleton fix + ViewModel disposal address the immediate leak
- Worth doing as a follow-up for long-term architectural health

---

### Phase 2: Fix Finding 2 — Windows Cursor Delegate Accumulation

**Goal:** Fix the Windows delegate identity bug so handlers don't accumulate. Add unload-time cleanup to CursorBehavior. iOS/Mac Catalyst only need the unload cleanup path (reapplication logic is already correct).

#### Step 2.1: Fix Windows delegate accumulation

**File:** `src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs`

**Two bugs:**
1. **Line 45:** `view.PointerEntered -= ViewOnPointerExited` unsubscribes the WRONG handler from the wrong event — should be `view.PointerEntered -= ViewOnPointerEntered`
2. **Delegate identity:** Local functions create new delegate instances per call → `-=` is a structural no-op → unbounded handler accumulation

**Fix:** Store delegate instances in a `ConditionalWeakTable<UIElement, CursorHandlers>` keyed on the native view, mirroring what iOS/MacCatalyst already do (they deduplicate by removing old interactions before adding new ones).

Add to the static class:
```csharp
private static readonly ConditionalWeakTable<UIElement, CursorHandlers> _cursorHandlers = new();

private sealed class CursorHandlers
{
    public PointerEventHandler? EnteredHandler;
    public PointerEventHandler? ExitedHandler;
}
```

Replace the `SetupCursor` local function's handler management with stored delegate instances:
```csharp
// Remove previously stored handlers using exact delegate instances
if (_cursorHandlers.TryGetValue(view, out var oldHandlers))
{
    view.PointerEntered -= oldHandlers.EnteredHandler;
    view.PointerExited -= oldHandlers.ExitedHandler;
    _cursorHandlers.Remove(view);
}

// Create and store new handlers (explicit delegate creation ensures stable instances)
var enteredHandler = new PointerEventHandler((sender, e) =>
{
    try
    {
        var cursorType = visualElement.IsEnabled ? GetCursor(cursor) : CoreCursorType.Arrow;
        view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(cursorType, 1)));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Failed to change cursor on pointer enter: {ex.Message}");
    }
});

var exitedHandler = new PointerEventHandler((sender, e) =>
{
    try
    {
        view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 1)));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Failed to change cursor on pointer exit: {ex.Message}");
    }
});

_cursorHandlers.Add(view, new CursorHandlers { EnteredHandler = enteredHandler, ExitedHandler = exitedHandler });

view.PointerEntered += enteredHandler;
view.PointerExited += exitedHandler;
```

**Note:** `new PointerEventHandler(...)` creates a stable delegate instance. Storing it enables correct `-=` removal on subsequent calls.

#### Step 2.2: Add platform `RemoveCustomCursor` methods

Add to each platform's `CursorExtensions` for unload-time cleanup:

**Windows** — removes stored handlers from ConditionalWeakTable:
```csharp
public static void RemoveCustomCursor(this VisualElement visualElement, IMauiContext? mauiContext)
{
    if (mauiContext is null) return;
    try
    {
        var view = visualElement.ToPlatform(mauiContext);
        if (view is not null && _cursorHandlers.TryGetValue(view, out var handlers))
        {
            view.PointerEntered -= handlers.EnteredHandler;
            view.PointerExited -= handlers.ExitedHandler;
            _cursorHandlers.Remove(view);
        }
    }
    catch { /* best-effort cleanup */ }
}
```

**iOS** — removes `UIPointerInteraction`:
```csharp
public static void RemoveCustomCursor(this VisualElement visualElement, IMauiContext? mauiContext)
{
    if (mauiContext is null) return;
    try
    {
        var view = visualElement.ToPlatform(mauiContext);
        foreach (var interaction in view.Interactions.OfType<UIPointerInteraction>())
            view.RemoveInteraction(interaction);
    }
    catch { /* best-effort cleanup */ }
}
```

**Mac Catalyst** — removes `PointerUIHoverGestureRecognizer`:
```csharp
public static void RemoveCustomCursor(this VisualElement visualElement, IMauiContext? mauiContext)
{
    if (mauiContext is null) return;
    try
    {
        var view = visualElement.ToPlatform(mauiContext);
        if (view.GestureRecognizers is not null)
        {
            foreach (var recognizer in view.GestureRecognizers.OfType<PointerUIHoverGestureRecognizer>())
                view.RemoveGestureRecognizer(recognizer);
        }
    }
    catch { /* best-effort cleanup */ }
}
```

**Android:** No changes — `SetCustomCursor` only sets a property, no event handlers or delegates.

#### Step 2.3: Add Unloaded handler to CursorBehavior

**File:** `src/ISynergy.Framework.UI.Maui/Behaviors/CursorBehavior.cs`

In `SetupCursorBehavior`, subscribe to `Unloaded` alongside the existing `PropertyChanged` subscription:

```csharp
private static void SetupCursorBehavior(VisualElement visualElement, CursorIcons cursor)
{
    visualElement.PropertyChanged -= OnVisualElementPropertyChanged;
    visualElement.PropertyChanged += OnVisualElementPropertyChanged;

    visualElement.Unloaded -= OnVisualElementUnloaded;
    visualElement.Unloaded += OnVisualElementUnloaded;

    ApplyCursorBasedOnState(visualElement, cursor);
}
```

Add cleanup handler (follows `LoadingView.OnViewUnloaded` pattern):
```csharp
private static void OnVisualElementUnloaded(object? sender, EventArgs e)
{
    if (sender is VisualElement visualElement)
    {
        visualElement.PropertyChanged -= OnVisualElementPropertyChanged;
        visualElement.Unloaded -= OnVisualElementUnloaded;

        var mauiContext = visualElement.Handler?.MauiContext
            ?? FindMauiContextFromParent(visualElement)
            ?? Application.Current?.Windows.LastOrDefault()?.Page?.Handler?.MauiContext;

        if (mauiContext is not null)
        {
            try { visualElement.RemoveCustomCursor(mauiContext); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to remove cursor on unload: {ex.Message}");
            }
        }
    }
}
```

**Rationale:** The `Unloaded` handler provides a terminal cleanup path. Even though iOS/MacCatalyst reapplication is clean (old interactions removed before new ones added), without an unload handler the last interaction/recognizer is never removed when the element leaves the tree. On Windows, this is critical — it removes handlers stored in the ConditionalWeakTable.

---

### Phase 3: Verification

#### Build
```powershell
dotnet build src/ISynergy.Framework.UI.Maui/ISynergy.Framework.UI.Maui.csproj
dotnet build samples/Sample.Maui/Sample.Maui.csproj
```

#### Code Review Checks
1. `ShellView` — `[Lifetime(Lifetimes.Singleton)]` attribute added (matches `SignInView` pattern)
2. `NavigationService.NavigateModalAsync` — old ViewModel captured and disposed after root replacement
3. Windows `CursorExtensions` — ConditionalWeakTable stores exact delegates, `-=` / `+=` use same instances, no local-function delegate identity bug
4. iOS/Mac Catalyst `CursorExtensions` — `RemoveCustomCursor` method added
5. `CursorBehavior` — `Unloaded` handler cleans up PropertyChanged + platform cursor state

#### Manual Testing
1. **Finding 1:** Run sample MAUI app → sign in → sign out → sign in → sign out × 10 cycles. Force GC. Verify `ShellView`/`ShellViewModel` instance counts are stable (1 each, not growing).
2. **Finding 2 (Windows):** Navigate to a cursor-styled page. Toggle `IsEnabled` on styled elements × 20. Verify handler count per element stays at 1. Navigate away, force GC, verify old elements collected.
3. **Finding 2 (iOS/Mac Catalyst):** Navigate to a cursor-styled page, navigate away. Verify `UIPointerInteraction`/`UIHoverGestureRecognizer` removed from native views.

---

## Files Modified

| File | Change | Priority |
|---|---|---|
| `samples/Sample.Maui/Views/ShellView.xaml.cs` | Add `[Lifetime(Lifetimes.Singleton)]` | **Critical** — stops the primary leak |
| `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs` | Dispose old page ViewModel after root replacement | High — defense-in-depth |
| `src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs` | ConditionalWeakTable + fix line 45 bug + RemoveCustomCursor | High — mechanically provable leak |
| `src/ISynergy.Framework.UI.Maui/Platforms/iOS/Extensions/CursorExtensions.cs` | Add RemoveCustomCursor | Medium — unload cleanup only |
| `src/ISynergy.Framework.UI.Maui/Platforms/MacCatalyst/Extensions/CursorExtensions.cs` | Add RemoveCustomCursor | Medium — unload cleanup only |
| `src/ISynergy.Framework.UI.Maui/Behaviors/CursorBehavior.cs` | Add Unloaded handler with full cleanup | Medium — unload cleanup |

**Not modified:** Android CursorExtensions (no leak), BaseShellViewModel (Dispose already correct).

---

## Deferred

**Scoped page/viewmodel resolution in NavigationExtensions:** The audit recommends `IServiceScope` per navigation. This requires converting `CreatePage` from static to instance-based (or injecting `IServiceScopeFactory`), and rethinking the View lifetime model. The Singleton fix stops the immediate leak. Track this as a follow-up improvement.

---

## Risks

1. **ShellView Singleton — tab page re-initialization:** When ShellView is reused (Singleton), its `OnNavigatedTo` and ViewModel change detection need to work correctly. The existing `View.OnNavigatedTo` handles re-initialization of ViewModel when `IsInitialized` is false — verify this works for Shell→ShellView transitions.

2. **ConditionalWeakTable on Windows:** Table entries live as long as the key (native `UIElement`) is alive. This is correct — handlers must persist for the native view's lifetime. Entries are cleaned up when the native peer is GC'd. No leak risk from the table itself.

3. **Unloaded timing:** The MAUI `Unloaded` event may fire when the Handler/MauiContext is already gone. The `try/catch` and null checks handle this. If MauiContext is null, we skip platform cleanup but still unsubscribe managed events (PropertyChanged, Unloaded).

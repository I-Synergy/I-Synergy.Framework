# Memory Leak Remediation Plan

**Status:** PENDING
**Created:** 2026-06-12
**Source:** Memory leak analysis (see `.ai/completed/memory-leak-analysis-2026-06-12.md`)
**Target model for execution:** Sonnet-tier (use `developer` or `architect` subagents as appropriate)

## Overview

Systematic analysis found 32 memory leak issues across 4 dimensions: event handler subscriptions, IDisposable chains, WeakReference usage, and async state machine captures. This plan covers the top-priority actionable fixes, grouped into phases.

## Priority Matrix

| Phase | Severity | Count | Effort | Impact |
|-------|----------|-------|--------|--------|
| 1 | 🔴 Critical — clear leaks | 5 | Medium | Highest |
| 2 | 🔴 Critical — async state machine | 4 | Large | High |
| 3 | 🟠 High — Dispose pattern | 5 | Medium | High |
| 4 | 🟠 High — event handler cleanup | 3 | Medium | Medium |
| 5 | 🟡 Medium | 5 | Small | Medium |
| 6 | 🔵 Low | 4 | Small | Low |

---

## Phase 1: Static Event Leaks + Handle Leaks (Critical)

### P1.1 — Fix WPF RenderingPerformanceMonitor static event leak

**Files:**
- `src/ISynergy.Framework.UI.WPF/OpenTelemetry/RenderingPerformanceMonitor.cs`

**Problem:** Line 29 subscribes `CompositionTarget.Rendering += OnRendering` — a static event. There is no unsubscription. The monitor instance can never be GC'd.

**Fix:**
- Implement `IDisposable` on `RenderingPerformanceMonitor`
- In `Dispose(bool)`, unsubscribe: `CompositionTarget.Rendering -= OnRendering`
- Call `Dispose` when the monitor is no longer needed (or via cleanup on app shutdown)

**Pattern:**
```csharp
private bool _disposed;
public void Dispose()
{
    if (_disposed) return;
    CompositionTarget.Rendering -= OnRendering;
    _disposed = true;
    GC.SuppressFinalize(this);
}
```

---

### P1.2 — Fix WPF ApplicationMonitor anonymous lambda leak

**Files:**
- `src/ISynergy.Framework.UI.WPF/OpenTelemetry/ApplicationMonitor.cs`

**Problem:** Line 54 subscribes `Dispatcher.Hooks.OperationCompleted += (_, _) => { ... }` with an anonymous lambda. Cannot be unsubscribed. Instance leaked for entire app lifetime.

**Fix:**
- Replace anonymous lambda with a named method
- Implement `IDisposable`
- In `Dispose(bool)`, unsubscribe: `Dispatcher.Hooks.OperationCompleted -= OnOperationCompleted`
- Also dispose `_currentProcess` (from `Process.GetCurrentProcess()`) in Dispose

Also: `_currentProcess` (line 61) is never disposed. Add `_currentProcess?.Dispose()` to Dispose.

---

### P1.3 — Fix ImageView anonymous lambda accumulation

**Files:**
- `src/ISynergy.Framework.UI.WinUI/Controls/ImageView/ImageView.xaml.cs`

**Problem:** Line 107 `image.ImageOpened += (s, e) => control.LoadImage(image)` — every `Source` change adds a new subscription. The old `BitmapImage` + `ImageView` leak.

**Fix:**
- Track the current `BitmapImage` subscription in a field
- Before assigning a new source, unsubscribe the old handler:
  ```csharp
  if (_currentImage != null)
      _currentImage.ImageOpened -= OnImageOpened;
  ```
- Use a named method `OnImageOpened` instead of anonymous lambda

---

### P1.4 — Fix RequestCancellationService CTS leak

**Files:**
- `src/ISynergy.Framework.Core/Services/RequestCancellationService.cs`

**Problem:** `CancellationTokenSource` objects stored in `ConcurrentDictionary` are never disposed. `RemoveRequest` removes from dictionary but does not call `.Dispose()`. `CancelAllRequests` cancels but does not dispose. Each CTS holds a `ManualResetEvent` + Timer internally.

**Fix:**
- In `RemoveRequest(string key)`: after `TryRemove`, call `cts.Dispose()` on the removed value
- In `CancelAllRequests()`: after iterating `_activeRequests.Values` and calling `Cancel()`, call `Dispose()` on each
- In `CancelAllRequests()`: after `Clear()`, iterate remaining values and dispose them

```csharp
public void RemoveRequest(string key)
{
    if (_activeRequests.TryRemove(key, out var cts))
    {
        cts.Cancel();
        cts.Dispose();  // ADD THIS
    }
}

public void CancelAllRequests()
{
    foreach (var cts in _activeRequests.Values)
    {
        cts.Cancel();
        cts.Dispose();  // ADD THIS
    }
    _activeRequests.Clear();
}
```

---

### P1.5 — Fix ProcessInterface stream disposal

**Files:**
- `src/ISynergy.Framework.UI.WPF/Processes/ProcessInterface.cs`

**Problem:** `InputWriter` (StreamWriter), `OutputReader` (TextReader), `ErrorReader` (TextReader) are set to `null` in both `CurrentProcess_Exited` (lines 215-217) and `Dispose(bool)` (lines 381-393) **without being disposed first**. Each wraps a kernel pipe handle. Also, `Process.Exited += CurrentProcess_Exited` (line 160) is never unsubscribed.

**Fix:**
- Before setting each to null, dispose it:
  ```csharp
  InputWriter?.Dispose();
  InputWriter = null;
  ```
- Same for `OutputReader`, `ErrorReader` in both locations
- In `Dispose(bool)`, unsubscribe: `_process.Exited -= CurrentProcess_Exited;`
- Also dispose `_process` with `_process?.Dispose()`

---

## Phase 2: Async State Machine Fixes (Critical)

### P2.1 — Fix DialogService/BladeService async void closures

**Files (multiple per platform):**
- `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` — lines 564-571, 592-610, 648-655, 676-694, 816
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` — lines 353-357, 372-386, 429-432, 448-462, 533-546
- `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs` — lines 190-193, 210-222, 255-258, 275-287
- `src/ISynergy.Framework.UI/Services/Base/BaseBladeService.cs` — lines 92-97, 102-106, 188-192

**Problem:** Singleton services subscribe to short-lived ViewModel events with `async void` lambdas that capture DI `scope`, `viewmodel`, and `_logger`. If `Closed` doesn't fire, the entire DI scope and ViewModel leak. Even in normal flow, the `async void` state machine keeps the scope alive until the async operation completes.

**Fix approach:** There are two valid strategies:

**Option A (preferred — WeakEventListener):** Use the existing `WeakEventListener` infrastructure to avoid strong references from the singleton to the ViewModel.
- Wrap each subscription (`Submitted`, `Cancelled`, `Closed`) in a `WeakEventListener<DialogService, IViewModel, EventArgs>`
- On detach, remove from the ViewModel event and dispose the scope

**Option B (simpler — SafeFireAndForget):** Keep the strong subscription but:
1. Extract the async lambda body into a named `async Task` method
2. Use `FireAndForget` or `SafeFireAndForget` to run it safely
3. Ensure `Closed` handler always unsubscribes `Submitted` and `Cancelled` even on exceptions
4. Add a finalizer or timeout safety net that disposes the scope if `Closed` doesn't fire within X seconds

**Implementation note:** Because `WeakEventListener<TInstance, TSource, TEventArgs>` already exists in `ISynergy.Framework.Core.Events`, Option A is the most aligned with the existing codebase patterns.

---

### P2.2 — Fix ActionQueuingBackgroundService async void Timer callbacks

**Files:**
- `src/ISynergy.Framework.Automations/BackgroundServices/ActionQueuingBackgroundService.cs`

**Problem:** Lines 73 and 98: `private async void RefreshQueue(object? state)` and `private async void ExecuteTask(object? state)` are used as `System.Threading.Timer` callbacks. The Timer holds a strong ref to the callback (which is an instance method), so the entire service instance is held alive. Exceptions thrown in these `async void` methods crash the process (catch blocks rethrow with `throw;`).

**Fix:**
- Change both to `private async Task` methods
- Use a wrapper that runs them safely:
  ```csharp
  _backgroundTimer = new Timer(_ => FireAndForget(RefreshQueue), null, ..., ...);
  
  private async void FireAndForget(Func<Task> operation)
  {
      try { await operation(); }
      catch { /* log, don't rethrow */ }
  }
  ```
- Or use the existing `SafeFireAndForget` extension pattern
- Remove the `throw;` in catch blocks — log the exception instead

---

### P2.3 — Fix ViewModel.OnUserSignedIn async void

**Files:**
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs`

**Problem:** Lines 121-128: `private async void OnUserSignedIn(UserSignedInMessage message)` captures `this` (full ViewModel), and the async state machine keeps it alive across `await InitializeAsync()`. The messenger registration itself holds a strong reference.

**Fix:**
- Change to `private async Task OnUserSignedInAsync(UserSignedInMessage message)`
- The messenger registration needs to handle `async Task` handlers. If `WeakAction<T>` supports async, use it directly. Otherwise:
- Create a wrapper that dispatches the async task safely:
  ```csharp
  _commonServices.MessengerService.Register<UserSignedInMessage>(this, msg =>
  {
      _ = OnUserSignedInAsync(msg);  // fire-and-forget through existing infrastructure
  });
  ```
- Ensure `Dispose()` cancels any in-flight `OnUserSignedInAsync` by checking `_disposed` after the await

---

### P2.4 — Fix DelayTimer.Start async void loop

**Files:**
- `src/ISynergy.Framework.Core/Helpers/DelayTimer.cs`

**Problem:** `public async void Start(TimeSpan interval, Func<bool> step)` runs an indefinite `while` loop with `await Task.Delay`. If `step()` never returns false, the state machine + captured closure live forever. The `step` delegate may be a lambda capturing ViewModel/other references.

**Fix:**
- Change return type from `void` to `Task`: `public async Task StartAsync(...)`
- Add a `CancellationToken` parameter or use an existing `_cancellationTokenSource` field:
  ```csharp
  public async Task StartAsync(TimeSpan interval, Func<bool> step, CancellationToken cancellationToken)
  {
      while (!cancellationToken.IsCancellationRequested && step())
      {
          await Task.Delay(interval, cancellationToken);
      }
  }
  ```
- Implement `IDisposable` on `DelayTimer` to cancel and dispose the CTS

---

## Phase 3: IDisposable Chain Fixes (High)

### P3.1 — Add IDisposable to Property\<T\>

**Files:**
- `src/ISynergy.Framework.Core/Base/Property.cs`

**Problem:** `Property<T>` declares a `ValueChanged` event but does not implement `IDisposable`. `ObservableClass.Dispose(bool)` casts properties as `IDisposable` — since `Property<T>` isn't disposable, `ValueChanged` subscribers are never released.

**Fix:**
- Implement `IDisposable` on `Property<T>`:
  ```csharp
  public class Property<T> : IProperty, IDisposable
  {
      // ...existing code...
      
      public void Dispose()
      {
          _valueChanged = null;
          GC.SuppressFinalize(this);
      }
  }
  ```
- In `ObservableClass.Dispose(bool)`, the existing `(property as IDisposable)?.Dispose()` will now correctly clean up each property's event handlers

---

### P3.2 — Fix MAUI TabbedView direct ViewModel disposal

**Files:**
- `src/ISynergy.Framework.UI.Maui/Controls/TabbedView.cs`

**Problem:** Line 82: `Dispose(bool)` calls `ViewModel?.Dispose()` directly. All other View implementations (WinUI `View.cs`, MAUI `View.cs`, WinUI `Window.cs`) explicitly document they do NOT dispose the ViewModel — the DI scope owns ViewModel lifecycle. This causes either `ObjectDisposedException` (if scope disposes later) or double-dispose.

**Fix:**
- Remove the `ViewModel?.Dispose()` call from `Dispose(bool)`
- The ViewModel lifecycle should be managed by the DI scope (which is disposed when the content dialog/page is closed)

---

### P3.3 — Fix MAUI View/TabbedView ServiceLocator resolution

**Files:**
- `src/ISynergy.Framework.UI.Maui/Controls/View.cs` (line 46)
- `src/ISynergy.Framework.UI.Maui/Controls/TabbedView.cs` (line 47)

**Problem:** The constructor overload taking a `Type viewModelType` resolves ViewModels from `ServiceLocator.Default.GetRequiredService(viewModelType)`. This bypasses scoped DI entirely and gets a singleton from the root container. Scoped ViewModels resolved this way are never properly disposed.

**Fix:**
- Require that callers pass a ViewModel instance directly, or
- Create an `IServiceScope` from the root provider and resolve the ViewModel within it:
  ```csharp
  var scope = ServiceLocator.Default.CreateScope();
  ViewModel = scope.ServiceProvider.GetRequiredService(viewModelType) as IViewModel;
  // Store scope for later disposal
  ```
- Store the scope so it can be disposed when the view is disposed

---

### P3.4 — Call BaseAsyncRelayCommand.CleanupStaticResources automatically

**Files:**
- `src/ISynergy.Framework.Mvvm/Commands/Base/BaseAsyncRelayCommand.cs`

**Problem:** `_cacheCleanupTimer` (line 86) is a static Timer created in the static constructor that fires every minute. `CleanupStaticResources()` (line 895) exists to dispose it but is never called automatically.

**Fix:**
- Call `CleanupStaticResources()` from the AppDomain's `ProcessExit` event, or
- Register a module initializer that hooks ProcessExit to clean up
- Add to the cleanup: `_cacheCleanupTimer?.Dispose(); _cacheCleanupTimer = null;`

Pattern:
```csharp
private static bool _cleanupRegistered;

private static void EnsureCleanupRegistered()
{
    if (_cleanupRegistered) return;
    AppDomain.CurrentDomain.ProcessExit += (_, _) => CleanupStaticResources();
    _cleanupRegistered = true;
}
// Call EnsureCleanupRegistered() at the end of the static constructor
```

---

### P3.5 — Fix ObservableClass.DisposeAsync managed cleanup

**Files:**
- `src/ISynergy.Framework.Core/Base/ObservableClass.cs`

**Problem:** `DisposeAsync()` (line 307) calls `Dispose(disposing: false)`. Since `disposing` is false, managed resources (Properties, event handlers) are never cleaned up via the async dispose path. Only native resources would be freed.

**Fix:**
- Change `DisposeAsync()` to call `Dispose(disposing: true)` instead, or
- Add explicit cleanup before the sync Dispose call:
  ```csharp
  public async ValueTask DisposeAsync()
  {
      await DisposeAsyncCore();
      Dispose(disposing: true);  // was: false
      GC.SuppressFinalize(this);
  }
  ```

---

## Phase 4: Remaining Event Handler Cleanup (High)

### P4.1 — Fix LanguageViewModel anonymous lambdas

**Files:**
- `src/ISynergy.Framework.UI/ViewModels/LanguageViewModel.cs`

**Problem:** Lines 62-68: Three anonymous lambdas on `option.Selected += (s, _) => { ... }` are never unsubscribed. If `LanguageOption` objects outlive the ViewModel (unlikely but possible), they prevent GC.

**Fix:**
- Convert to named methods: `OnLanguageOptionSelected`
- In `Cleanup()` or `Dispose()`, unsubscribe each one
- Or use `WeakEventListener<LanguageViewModel, LanguageOption, EventArgs>` for each

---

### P4.2 — Fix MAUI Application _clockTimer lifecycle

**Files:**
- `src/ISynergy.Framework.UI.Maui/Application/Application.cs`

**Problem:** `_clockTimer` is created at line 67, starts every second (line 70), but `Dispose(bool)` never calls `Stop()` or disposes it.

**Fix:**
- In `Dispose(bool)`, add:
  ```csharp
  if (_clockTimer is not null)
  {
      _clockTimer.Stop();
      _clockTimer = null;
  }
  ```

---

### P4.3 — Fix ServiceLocator.Dispose to clean up IServiceProvider

**Files:**
- `src/ISynergy.Framework.Core/Locators/ServiceLocator.cs`

**Problem:** `Dispose()` only sets `_disposed = true` and calls `GC.SuppressFinalize(this)`. Never disposes the `_serviceProvider`.

**Fix:**
```csharp
public void Dispose()
{
    if (_disposed) return;
    if (_serviceProvider is IDisposable disposable)
        disposable.Dispose();
    _serviceProvider = null;
    _disposed = true;
    GC.SuppressFinalize(this);
}
```
Also implement `IAsyncDisposable` for async-capable service providers.

---

## Phase 5: Medium Severity Fixes

### P5.1 — Fix ProcessInterface Process.Exited subscription

**Files:**
- `src/ISynergy.Framework.UI.WPF/Processes/ProcessInterface.cs`

**Fix:** In `Dispose(bool)`, add `_process.Exited -= CurrentProcess_Exited;` before disposing the process.

### P5.2 — Fix ApplicationMonitor _currentProcess disposal

**Files:**
- `src/ISynergy.Framework.UI.WPF/OpenTelemetry/ApplicationMonitor.cs`

**Fix:** In `Dispose()`, add `_currentProcess?.Dispose();`.

### P5.3 — Fix WinUI DialogService Window disposal

**Files:**
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs`

**Fix:** In `ShowMessageAsync`, dispose the `Window` after the dialog closes (or when the scope is disposed).

### P5.4 — Fix DeferredNavigation async void

**Files:**
- `src/ISynergy.Framework.UI.WinUI/Common/DeferredNavigation.cs`

**Fix:** Change `async void OnFrameNavigating` to `async Task` and handle the fire-and-forget through SafeFireAndForget, or use a synchronization mechanism (SemaphoreSlim) to track completion.

### P5.5 — Fix ImageBrowser async void handlers

**Files:**
- `src/ISynergy.Framework.UI.Maui/Controls/ImageBrowser/ImageBrowser.xaml.cs`
- `src/ISynergy.Framework.UI.WinUI/Controls/ImageBrowser/ImageBrowser.xaml.cs`

**Fix:** Wrap each `async void` event handler with a try/catch that logs exceptions instead of crashing the process. Or use `SafeFireAndForget` pattern.

---

## Phase 6: Low Severity Fixes

### P6.1 — BladeService.Loaded not cleaned on timeout

**Files:**
- `src/ISynergy.Framework.UI.WPF/Services/BladeService.cs` (line 71)
- `src/ISynergy.Framework.UI.WinUI/Services/BladeService.cs` (line 86)

**Fix:** In the timeout path of `MonitorViewLoadAsync`, add `element.Loaded -= callback` before returning.

### P6.2 — NullScope GC.SuppressFinalize with no finalizer

**Files:**
- `src/ISynergy.Framework.Core/Helpers/NullScope.cs`

**Fix:** Remove the `GC.SuppressFinalize(this)` call from `Dispose()` (it's unnecessary without a finalizer).

### P6.3 — BaseNavigationService BackStackChanged weak event

**Files:**
- `src/ISynergy.Framework.UI/Services/Base/BaseNavigationService.cs`

**Fix:** Replace the manual `ObjectDisposedException` detection with the existing `WeakEventSource<EventHandler>` infrastructure.

### P6.4 — WeakEventSource dead handler cleanup

**Files:**
- `src/ISynergy.Framework.Core/Events/WeakEventSource{T}.cs`

**Fix:** Add a periodic cleanup mechanism or a `Cleanup()` method that prunes dead handlers without requiring a `Raise` call.

---

## Execution Order

When implementing, follow this sequence to avoid conflicts:

```
Phase 1  →  Phase 3  →  Phase 4  →  Phase 2  →  Phase 5  →  Phase 6
```

Phase 1 first (highest impact, isolated), then Phase 3 (IDisposable chains, foundational), then Phase 4 (event cleanup, may conflict with Phase 2), then Phase 2 (async, highest risk of behavioral change — do last among the big fixes), then Phases 5-6 (small cleanups).

## Build & Verification

After each phase:
1. `dotnet build` the affected project(s)
2. `dotnet test` for any affected test projects
3. Run the relevant WPF/WinUI/MAUI test project if tests exist

At minimum, the following projects must build:
- `src/ISynergy.Framework.Core/` — Phases 1.4, 3.1, 3.5, 5.3, 6.2, 6.4
- `src/ISynergy.Framework.Mvvm/` — Phases 2.3, 3.4
- `src/ISynergy.Framework.UI/` — Phases 2.1 (BaseBladeService), 4.1, 6.3
- `src/ISynergy.Framework.UI.WinUI/` — Phases 1.3, 2.1, 5.3, 5.5, 6.1
- `src/ISynergy.Framework.UI.WPF/` — Phases 1.1, 1.2, 1.5, 5.1, 5.2, 6.1
- `src/ISynergy.Framework.UI.Maui/` — Phases 2.1, 3.2, 3.3, 4.2, 5.5
- `src/ISynergy.Framework.Automations/` — Phase 2.2

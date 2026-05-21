# Cross-Platform Quality Plan
**Scope:** ISynergy.Framework — Core, MVVM, UI (shared), UI.WinUI, UI.WPF, UI.MAUI  
**Created:** 2026-05-20  
**Status:** Open

---

## How to use this document

Each issue has an ID, severity, category, exact file location, a one-line description, and the fix.  
Work items are ordered: fix Criticals first (they can crash or silently leak memory), then Highs (duplication and broken contracts multiply maintenance cost), then Mediums.

Mark an item `[x]` when the fix is committed.

---

## Summary counts

| Severity | Count |
|----------|-------|
| Critical | 13 |
| High | 17 |
| Medium | 11 |
| **Total** | **41** |

---

## Critical — crash / memory leak

### C-01 · LoadingView anonymous `Loaded` lambda never unsubscribed (WinUI + MAUI)
**Files**
- `src/ISynergy.Framework.UI.WinUI/Controls/LoadingView.cs` line 44
- `src/ISynergy.Framework.UI.Maui/Controls/LoadingView.cs` line 45

**Issue**  
Both constructors add a second anonymous lambda to `Loaded` to call `SignalApplicationUIReady()`, separate from the named `OnViewLoaded` registered in `SetupEventHandlers`. `OnNavigatedFrom`/`OnDisappearing` only removes the named handler. The anonymous closure permanently holds `_applicationLifecycleService`.

**Fix**  
Promote to a named private method `OnSignalUIReady` and remove it in `OnNavigatedFrom`/`OnDisappearing` alongside `OnViewLoaded`.

- [x] WinUI fix
- [x] MAUI fix

---

### C-02 · WinUI `ThemeService._window` — guaranteed `NullReferenceException` before `InitializeMainWindow`
**File:** `src/ISynergy.Framework.UI.WinUI/Services/ThemeService.cs` line 128 (field declaration and `UpdatePlatformColors`)

**Issue**  
`_window` is declared non-nullable with no initializer. `UpdatePlatformColors` dereferences it unconditionally. Any call to `ApplyTheme()` before `InitializeMainWindow()` throws `NullReferenceException`.

**Fix**  
Declare `_window` as `Window?`. Guard the dereference or throw `InvalidOperationException("ThemeService not initialized")` if null.

- [x] Done

---

### C-03 · WinUI `Application.Exiting` subscribers never released
**File:** `src/ISynergy.Framework.UI.WinUI/Application/Application.cs` lines 532, 537

**Issue**  
`public event Action Exiting` is non-nullable. `Dispose(bool)` does not null the field or unsubscribe handlers. Subscribers are held alive for the process lifetime.

**Fix**  
Declare as `event Action? Exiting`. In `Dispose(bool disposing)` null the backing field: `Exiting = null`.

- [x] Done

---

### C-04 · WinUI `DialogService` — anonymous `Closed` handler accumulates on `_activeDialog`
**File:** `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` line ~504

**Issue**  
`_activeDialog.Closed += (s, e) => tcs.TrySetResult(true)` is added each time a dialog is queued while one is active. There is no corresponding `-=`. Delegates accumulate on a potentially reused dialog object and each holds a live `TaskCompletionSource`.

**Fix**  
Store the handler in a local variable. Unsubscribe inside the handler itself after `TrySetResult` fires (self-removing pattern):
```csharp
EventHandler<ContentDialogClosedEventArgs>? handler = null;
handler = (s, e) => { _activeDialog.Closed -= handler; tcs.TrySetResult(true); };
_activeDialog.Closed += handler;
```

- [x] Done

---

### C-05 · MAUI `DialogService._dialogSemaphore` never disposed
**File:** `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` line 31

**Issue**  
`SemaphoreSlim` holds a native OS wait handle. `DialogService` has no `IDisposable` implementation. The WinUI version disposes it correctly; MAUI does not.

**Fix**  
Implement `IDisposable` on MAUI `DialogService`. Dispose `_dialogSemaphore` in `Dispose()`. Register for disposal in DI if the lifetime changes from Singleton.

- [x] Done

---

### C-06 · MAUI `DialogService` — cross-thread access to `Application.Current.Windows` inside `Task.Run`
**File:** `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` lines ~222–225

**Issue**  
`Application.Current.Windows` is UI-thread-bound. Reading it from `Task.Run` is an unsynchronized cross-thread access that can throw `InvalidOperationException` or return stale data. The synchronous null-check two lines above already covers the same guard.

**Fix**  
Delete the `Task.Run` block entirely. The synchronous pre-check is sufficient.

- [x] Done

---

### C-07 · `WaitForLoadingCompleteAsync` race — event fires before handler is registered (WinUI + MAUI)
**Files**
- `src/ISynergy.Framework.UI.WinUI/Controls/LoadingView.cs` lines 330–350
- `src/ISynergy.Framework.UI.Maui/Controls/LoadingView.cs` lines 292–312

**Issue**  
If `CompleteLoading()` is called before `LoadingCompleted += handler`, the event is missed and the `Task` returned by `WaitForLoadingCompleteAsync` never completes — the caller hangs forever. The `if (tcs.Task.IsCompleted)` check at the end is always `false` (freshly created TCS) and is dead code that misleads readers.

**Fix**  
Add a `bool _loadingCompleted` field set to `true` inside `CompleteLoading()`. In `WaitForLoadingCompleteAsync`, check this flag *before* subscribing to the event; if already true, return `Task.CompletedTask` immediately. Remove the dead `IsCompleted` check.

- [x] WinUI fix
- [x] MAUI fix

---

### C-08 · WPF `Application.Dispose` unsubscribes the class's own event instead of the lifecycle service event
**File:** `src/ISynergy.Framework.UI.WPF/Application/Application.cs` lines 89 and ~363

**Issue**  
Constructor: `_lifecycleService.ApplicationLoaded += OnApplicationLoaded` (subscribes to the singleton lifecycle service).  
Dispose: `ApplicationLoaded -= OnApplicationLoaded` (unsubscribes from the class's *own* `ApplicationLoaded` event field, not the lifecycle service).  
The lifecycle service subscription is never removed. Because `ApplicationLifecycleService` is a singleton it outlives the `Application`, keeping a strong reference to it and continuing to fire the handler after the object is logically disposed.

**Fix**  
```csharp
// In Dispose(bool disposing):
_lifecycleService.ApplicationLoaded -= OnApplicationLoaded;
```

- [x] Done

---

### C-09 · `ObservableValidatedClass.Dispose(bool)` missing early-exit guard — double-dispose race
**File:** `src/ISynergy.Framework.Core/Base/ObservableValidatedClass.cs` line ~363

**Issue**  
`ObservableClass.Dispose(bool)` begins with `if (_disposed) return;`. `ObservableValidatedClass.Dispose(bool)` calls `base.Dispose(disposing)` first (which sets `_disposed = true`) and then continues executing its own cleanup with no guard. Concurrent calls to `Dispose()` and `DisposeAsync()` can both enter the body and race on `_errorsChanged` teardown.

**Fix**  
Add `if (_disposed) return;` at the very top of `ObservableValidatedClass.Dispose(bool)`, before the `base.Dispose(disposing)` call.

- [x] Done

---

### C-10 · `ObservableValidatedClass.DisposeAsyncCore` — `_errorsChanged` can fire after `Properties` is cleared
**File:** `src/ISynergy.Framework.Core/Base/ObservableValidatedClass.cs` lines ~388–404

**Issue**  
`DisposeAsyncCore` first `await`s `base.DisposeAsyncCore()`, which clears and disposes the `Properties` dictionary. After the await returns, the code clears `_errorsChanged`. Any in-flight operation that fires `OnErrorsChanged` between those two steps will call `RaisePropertyChanged` on an already partially disposed base object, potentially causing `KeyNotFoundException` or `ObjectDisposedException`.

**Fix**  
Null out `_errorsChanged` *before* awaiting `base.DisposeAsyncCore()`:
```csharp
_errorsChanged = null;
await base.DisposeAsyncCore().ConfigureAwait(false);
```

- [x] Done

---

### C-11 · WPF `SelectionWindow.DataContextChanged` never unsubscribed
**File:** `src/ISynergy.Framework.UI.WPF/Windows/SelectionWindow.xaml.cs` lines 16–17

**Issue**  
`DataContextChanged += SelectionWindow_DataContextChanged` in the constructor has no matching `-=`. While it is a self-reference (the window subscribes to itself), it prevents collection after the window is closed and its DI scope is disposed.

**Fix**  
Override `OnClosed`:
```csharp
protected override void OnClosed(EventArgs e)
{
    DataContextChanged -= SelectionWindow_DataContextChanged;
    base.OnClosed(e);
}
```

- [x] Done

---

### C-12 · `ObservableClass.GetHashCode()` always returns `0`
**File:** `src/ISynergy.Framework.Core/Base/ObservableClass.cs` lines ~112–113

**Issue**  
`new HashCode().ToHashCode()` with no `.Add()` calls always returns 0. Every `ObservableClass` instance that does not override `GetHashCode` hashes to the same bucket. `Dictionary` and `HashSet` collections keyed on these objects degrade to O(n) lookup.

**Fix**  
```csharp
public override int GetHashCode() =>
    System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
```

- [x] Done

---

### C-13 · `ObservableClass.GetValue`/`SetValueCore` — `Properties` dictionary is not thread-safe
**File:** `src/ISynergy.Framework.Core/Base/ObservableClass.cs` lines ~121–191

**Issue**  
`GetValue<T>` calls `Properties.ContainsKey` then `Properties.Add` as two separate non-atomic operations. `SetValueCore` does the same. ViewModels are frequently initialized from background threads (`InitializeAsync`). Concurrent first-write for the same or different keys will corrupt the `Dictionary` or throw `ArgumentException`.

**Fix**  
Replace `Dictionary<string, IProperty>` with `ConcurrentDictionary<string, IProperty>` and use `GetOrAdd` / `AddOrUpdate` to make operations atomic.

- [x] Done

---

## High — duplication / broken contract

### H-01 · `NavigationService` duplicated ~700 lines between WPF and WinUI
**Files**
- `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` (~751 lines)
- `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs` (~775 lines)

**Issue**  
Every method is structurally identical line-for-line. Differences are limited to three surface items: the window accessor (`Application.Current.MainWindow` vs `Application.MainWindow`), one dispatcher type reference, and one error-handling path. The `MaxBackstackSize` constant, `Task.Delay(50)` magic numbers, all back-stack management logic, and the disposal loop are all duplicated.

**Fix**  
Extract `BaseNavigationService` into `ISynergy.Framework.UI` containing all shared logic. WPF and WinUI subclasses override only: `GetCurrentWindow()`, `CreatePage(Type)`, and platform-specific dispatcher dispatch.

- [x] Extract base class
- [x] WinUI subclass
- [x] WPF subclass

---

### H-02 · `DialogService.ShowGreetingAsync` time-of-day logic duplicated in WPF and WinUI
**Files**
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` lines ~113–139
- `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs` lines ~86–105

**Issue**  
Four identical `if (hour < …)` blocks and the `"TitleWelcome"` constant string are copy-pasted between both implementations.

**Fix**  
Move the time-of-day resolution into a `protected static string ResolveGreeting(ILanguageService lang)` method on a `BaseDialogService` in `ISynergy.Framework.UI`. Both platform services call it.

- [x] Extract to shared base

---

### H-03 · `DownloadFileService` and `UpdateService` contain zero platform-specific code
**Files**
- `src/ISynergy.Framework.UI.WinUI/Services/DownloadFileService.cs`
- `src/ISynergy.Framework.UI.WPF/Services/DownloadFileService.cs`
- `src/ISynergy.Framework.UI.WinUI/Services/UpdateService.cs`
- `src/ISynergy.Framework.UI.WPF/Services/UpdateService.cs`

**Issue**  
Both services use only `IFileService`, `HttpClient`, `File`, and `Process.Start` — all cross-platform BCL APIs. They are maintained in parallel with no divergence.

**Fix**  
Move both to `ISynergy.Framework.UI/Services/`. Register once in a shared builder extension method. Remove the four platform-specific files.

- [x] Move DownloadFileService
- [ ] Move UpdateService (platform-specific — Store vs HTTP; left as-is)
- [x] Update registrations (WinUI, WPF, MAUI builders)

---

### H-04 · `OnSubmitted` event invocation pattern duplicated across 5 ViewModel base classes
**Files**
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelBlade{TModel}.cs` lines ~29–57
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelDialog{TModel}.cs` lines ~29–57
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelNavigation{TModel}.cs` lines ~29–57
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelBladeView{TModel}.cs` lines ~33–61
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelSummary{TModel}.cs` lines ~22–51

**Issue**  
~25 lines of `Submitted` event field declaration plus defensive per-handler invocation logic are copy-pasted verbatim into all five classes.

**Fix**  
Move the `Submitted` event and `OnSubmitted(TModel)` into a shared generic base class `ViewModelSelectable<TModel>` that all five inherit from.

- [x] Create ViewModelSelectable<TModel>
- [x] Migrate all 5 classes

---

### H-05 · `SelectedItem` PropertyChanged subscribe/unsubscribe pattern duplicated across 4 ViewModel base classes
**Files**
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelBlade{TModel}.cs` lines ~63–100
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelDialog{TModel}.cs` lines ~63–100
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelNavigation{TModel}.cs` lines ~63–100
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelBladeView{TModel}.cs` lines ~98–135

**Issue**  
~35 lines of identical `SelectedItem` setter with subscribe/unsubscribe logic and `SelectedItem_PropertyChanged` handler are duplicated four times.

**Fix**  
Add a protected `SetSelectedItem(TModel? value)` method on `ViewModelSelectable<TModel>` (see H-04) that encapsulates the subscribe/unsubscribe logic. All four classes delegate to it.

- [x] Implement in base class
- [x] Migrate all 4 classes

---

### H-06 · `ViewModelBladeWizard<TModel>` and `ViewModelDialogWizard<TModel>` — ~80 lines of wizard state fully duplicated
**Files**
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelBladeWizard{TModel}.cs` lines ~87–168
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelDialogWizard{TModel}.cs` lines ~85–179

**Issue**  
Both classes contain identical `Page`, `Pages`, `Next_IsEnabled`, `Back_IsEnabled`, `Submit_IsEnabled`, `BackCommand`, `NextCommand` fields, the same `OnPropertyChanged` wizard-state logic, and the same `Cleanup`/`Dispose` overrides.

**Fix**  
Extract a `WizardStateHelper` struct or a `WizardViewModelBase<TModel>` intermediate class containing the shared pagination state. Each class inherits or composes it.

- [x] Extract shared wizard state

---

### H-07 · MAUI font registration duplicated (`[assembly: ExportFont]` + `ConfigureFonts`)
**Files**
- `src/ISynergy.Framework.UI.Maui/Application/Application.cs` lines ~26–34
- `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs` lines ~232–246

**Issue**  
Fonts are registered twice via two different mechanisms. The two sets are not identical (OpenSans fonts appear only in `ConfigureFonts`), creating silent inconsistency.

**Fix**  
Remove all `[assembly: ExportFont]` attributes from `Application.cs`. Keep only the `ConfigureFonts` call in `MauiAppBuilderExtensions`. Ensure all required fonts are listed there.

- [x] Done

---

### H-08 · `MigrationService` captive dependency — Singleton holds Scoped `ISettingsService` (MAUI)
**File:** `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs` line ~202

**Issue**  
`MigrationService` is registered as Singleton in MAUI but `ISettingsService` is Scoped. The first resolution captures the Scoped instance permanently.

**Fix**  
Register `MigrationService` as `Scoped` in MAUI to match the WinUI registration (and to match the lifetime of `ISettingsService`).

- [x] Done

---

### H-09 · WinUI double `ServiceLocator` provider registration — early `BuildServiceProvider()` creates a divergent graph
**File:** `src/ISynergy.Framework.UI.WinUI/Extensions/WindowsAppBuilderExtensions.cs` line ~190

**Issue**  
`ConfigureServices` calls `ServiceLocator.SetLocatorProvider(appBuilder.Services.BuildServiceProvider())`, registering a separate incomplete provider. Later `WindowsAppSdkHost.StartAsync` registers the final host provider, overwriting it. Services resolved during `Application` construction (before the host starts) use the first graph and get different instances than the rest of the app.

**Fix**  
Remove the `BuildServiceProvider()` call from `ConfigureServices`. Only `WindowsAppSdkHost.StartAsync` should call `SetLocatorProvider`.

- [x] Done

---

### H-10 · `NavigationService._backStackChanged` silently nulled on every modal navigation (WinUI)
**File:** `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` line ~688

**Issue**  
`_backStackChanged = null` removes all subscribers every time `NavigateModalAsync` is called. Shell ViewModels tracking back-button visibility lose their subscription permanently with no notification.

**Fix**  
Remove the null assignment. Instead raise the event (if non-null) with the post-clear stack state so subscribers are notified.

- [x] Done

---

### H-11 · WinUI `ThemeService` — hard cast before pattern match throws on wrong resource type
**File:** `src/ISynergy.Framework.UI.WinUI/Services/ThemeService.cs` line ~103

**Issue**  
`(Color)Application.Current.Resources[key] is Color` performs an unconditional `InvalidCastException`-throwing cast before the `is` check can catch a type mismatch.

**Fix**  
```csharp
if (Application.Current.Resources.TryGetValue(key, out var resource) && resource is Color primaryColor)
```

- [x] Done

---

### H-12 · MAUI `ClipboardService` — `#if !WINDOWS` guard is fragile and registration is opaque
**Files**
- `src/ISynergy.Framework.UI.Maui/Services/ClipboardService.cs` (guarded `#if !WINDOWS`)
- `src/ISynergy.Framework.UI.Maui/Platforms/Windows/Services/ClipboardService.cs`

**Issue**  
Both share the same fully-qualified name. On Windows the platform file takes effect, but the DI registration is a single unconditional `TryAddSingleton<IClipboardService, ClipboardService>()` with no explicit targeting. Adding a new target (e.g., Tizen) will silently get the wrong implementation.

**Fix**  
Use MSBuild `Condition` or `#if / #else / #endif` in a single file to select the implementation. Remove the separate `Platforms/Windows` copy and inline the Windows path into the shared file with `#if WINDOWS … #else … #endif`.

- [x] Done

---

### H-13 · `ViewModelSummary<TModel>.SelectedItem` does not subscribe to `PropertyChanged`
**File:** `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelSummary{TModel}.cs` lines ~63–71

**Issue**  
All four sibling ViewModel base classes subscribe to `SelectedItem.PropertyChanged` to bubble inner property changes and re-evaluate `SubmitCommand.CanExecute`. `ViewModelSummary` omits this, so inner property changes are silently dropped and `SubmitCommand` never re-evaluates on item change.

**Fix**  
Add the same subscribe/unsubscribe pattern for `SelectedItem_PropertyChanged` as used in `ViewModelBlade`, `ViewModelDialog`, `ViewModelNavigation`, and `ViewModelBladeView`. This becomes automatic when H-05 is implemented.

- [x] Done (resolved by H-05)

---

### H-14 · `ThemeService` registered Scoped (WinUI) vs Singleton (MAUI) — captive dependency in MAUI
**Files**
- `src/ISynergy.Framework.UI.WinUI/Extensions/WindowsAppBuilderExtensions.cs` line ~174
- `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs` line ~222

**Issue**  
`ThemeService` takes `ISettingsService` (Scoped) in its constructor. Registering it as Singleton in MAUI captures the first Scoped `ISettingsService` instance permanently.

**Fix**  
Register `ThemeService` as Scoped on both platforms. Alternatively, change `ThemeService` to resolve `ISettingsService` on demand via `IScopedContextService` rather than constructor injection.

- [x] WinUI (already Scoped — verified correct)
- [x] MAUI fix

---

### H-15 · WPF builder missing critical service registrations
**File:** `src/ISynergy.Framework.UI.WPF/Extensions/WPFAppBuilderExtensions.cs`

**Issue**  
Comparing with WinUI and MAUI builders, WPF is missing:
- `IDispatcherService` / `DispatcherService`
- `IApplicationLifecycleService` / `ApplicationLifecycleService`
- `ITokenStorageService` / `TokenStorageService`
- `IMigrationService` / `MigrationService`

Any code taking these from DI will fail at runtime on WPF.

**Fix**  
Add the missing registrations. Match the lifetime used in WinUI:
```csharp
services.TryAddSingleton<IDispatcherService, DispatcherService>();
services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
services.TryAddSingleton<ITokenStorageService, TokenStorageService>();
services.TryAddScoped<IMigrationService, MigrationService>();
```

- [x] Done

---

### H-16 · `IMigrationService` lifetime inconsistency: Singleton (MAUI) vs Scoped (WinUI)
**Files**
- `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs` line ~202
- `src/ISynergy.Framework.UI.WinUI/Extensions/WindowsAppBuilderExtensions.cs` line ~173

**Issue**  
Different lifetimes across platforms introduce hidden behavioural divergence. MAUI's Singleton causes the captive dependency issue (H-08). WinUI's Scoped is correct given `ISettingsService` is Scoped.

**Fix**  
Standardize to `Scoped` on all platforms (covered by H-08 and H-15).

- [x] Done (resolved by H-08 + H-15)

---

### H-17 · WinUI `DialogService` implements `IDisposable` but registers under `IDialogService` — never disposed
**File:** `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` line ~15

**Issue**  
The concrete class declares `IDisposable` and disposes `_dialogSemaphore` correctly. But it is registered as `IDialogService` (Singleton). MS DI only calls `Dispose` on services registered under a type that implements `IDisposable`. Registered as `IDialogService`, DI never calls `Dispose`. The semaphore and `_activeDialog` reference are never released.

**Fix**  
Register the service under both interfaces: `services.AddSingleton<DialogService>(); services.AddSingleton<IDialogService>(sp => sp.GetRequiredService<DialogService>());`  
This lets DI dispose the concrete type.

- [x] Done

---

## Medium — path length / code smell / dead code

### M-01 · `BusyService` not thread-safe — singleton with unguarded property mutation
**File:** `src/ISynergy.Framework.Core/Services/BusyService.cs`

**Issue**  
`BusyService` is registered as Singleton on all platforms. `StartBusy()`/`StopBusy()` mutate `IsBusy` (which also sets `IsEnabled`) and fire `PropertyChanged` — all without a lock or dispatcher check. A background thread calling `StartBusy()` while the UI thread reads `IsBusy` can produce missed or doubled notifications.

**Fix**  
Add a `lock (_syncRoot)` around property mutation in `StartBusy`, `StopBusy`, and `UpdateMessage`. Alternatively, document that callers must marshal to the UI thread and add a `Debug.Assert(IsOnUIThread)` guard.

- [x] Done

---

### M-02 · `NavigationService.InitializeViewAndViewModel` defined but never called in WPF and WinUI
**Files**
- `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` line ~522
- `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs` line ~526

**Issue**  
Both files define a private `InitializeViewAndViewModel(View page, bool backNavigation)` helper. Both `NavigateAsync` overloads duplicate the same logic inline instead of calling it. Dead code with a `[RequiresUnreferencedCode]` annotation.

**Fix**  
Either call the method at the correct sites (replacing the duplicated inline blocks) or delete it. This is naturally resolved when H-01 (BaseNavigationService) is implemented.

- [x] Done (resolved by H-01)

---

### M-03 · WinUI `NavigationService` uses `ServiceLocator` in catch block instead of injected field
**File:** `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` line ~543

**Issue**  
Inside the error path, `ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>()` is used instead of the already-injected `_exceptionHandlerService`. Breaks testability and adds a hidden dependency.

**Fix**  
Replace with `_exceptionHandlerService?.HandleException(ex)`.

- [x] Done

---

### M-04 · MAUI `DispatcherService` multi-window limitation not enforced
**File:** `src/ISynergy.Framework.UI.Maui/Services/DispatcherService.cs`

**Issue**  
The class comment warns not to use it in multi-window apps, but then falls back to `Application.Current.Dispatcher` silently. Registered as a singleton with no way to pass a window context through `IDispatcherService.Invoke(Action)`.

**Fix**  
Add `/// <remarks>Single-window applications only.</remarks>` to `IDispatcherService` (or add an overload `bool Invoke(Action, IDispatcher)` for multi-window callers). Throw `InvalidOperationException` when the fallback fires in a multi-window context.

- [x] Done

---

### M-05 · MAUI `NavigationService` recursive dispatch can loop infinitely
**File:** `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs` lines ~139–178

**Issue**  
`NavigateModalAsync` checks `IsDispatchRequired` and recursively calls itself via the dispatcher. On platforms where `IsDispatchRequired` always returns `false` in the callback, this creates an infinite recursion.

**Fix**  
Replace the conditional recursion pattern with `await MainThread.InvokeOnMainThreadAsync(() => { ... })` and remove the branch.

- [x] Done

---

### M-06 · `Window.Window_Unloaded` is dead code that throws `NotImplementedException`
**File:** `src/ISynergy.Framework.UI.WinUI/Controls/Window.cs` lines ~57–60

**Issue**  
`private static void Window_Unloaded(object? sender, RoutedEventArgs e) { throw new NotImplementedException(); }` is never wired up to any event. If ever connected it immediately crashes the application.

**Fix**  
Delete the method entirely.

- [x] Done

---

### M-07 · `ViewModelBlade<TModel>.Dispose` double-unsubscribes `SelectedItem_PropertyChanged`
**File:** `src/ISynergy.Framework.Mvvm/ViewModels/ViewModelBlade{TModel}.cs` lines ~210–229

**Issue**  
`Dispose(bool)` manually unsubscribes from `SelectedItem.PropertyChanged` (lines ~215–219) and then calls `Cleanup()` which does the same again (lines ~192–196), resulting in a redundant double-unsubscribe on every disposal.

**Fix**  
Remove the manual unsubscribe from `Dispose(bool)`; rely on `Cleanup()` as the single unsubscribe path. This is naturally resolved when H-05 is implemented.

- [x] Done (resolved by H-05)

---

### M-08 · WPF `Application.cs` — two overloads of `OnApplicationLoaded` with different signatures create confusion
**File:** `src/ISynergy.Framework.UI.WPF/Application/Application.cs` lines ~116 and ~195

**Issue**  
One `virtual void OnApplicationLoaded(object?, EventArgs)` (wired to lifecycle service) and one `abstract void OnApplicationLoaded(object?, ReturnEventArgs<bool>)` (the class's own event). The class also owns a `ApplicationLoaded` event field (line 45) that is never raised internally. The two event designs are redundant and the split is the root cause of C-08.

**Fix**  
Remove the class's own `ApplicationLoaded` event field and the abstract overload. Use only the lifecycle service event. Derived classes override the `virtual` method.

- [x] Done (C-08 prerequisite)

---

### M-09 · `ObservableValidatedClass.Dispose(bool)` redundantly sets `_disposed = true` outside `if (disposing)` block
**File:** `src/ISynergy.Framework.Core/Base/ObservableValidatedClass.cs` line ~383

**Issue**  
`base.Dispose(disposing)` already sets `_disposed = true`. The second assignment at line 383 (outside the `if (disposing)` block) is redundant and inconsistent with the standard dispose pattern where only the deepest override sets the flag.

**Fix**  
Remove the redundant `_disposed = true;` line from `ObservableValidatedClass.Dispose(bool)`.

- [x] Done

---

### M-10 · MAUI `DialogService.ValidateApplicationStateAsync` — mandatory 5-second delay on every dialog show
**File:** `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` lines ~222–236

**Issue**  
`ValidateApplicationStateAsync` races a `Task.Delay(5000)` against a `Task.Run` that reads `Application.Current.Windows[0]` (a cross-thread access — see C-06). The synchronous null-guard already present in `ShowMessageInternalAsync` two lines later makes the entire async check redundant. Every dialog call pays a potential 5-second startup overhead even when the app state is fine.

**Fix**  
Remove `ValidateApplicationStateAsync` entirely. The synchronous null-check in the caller (`if (Application.Current?.Windows.Count == 0) return`) is sufficient. If an async guard is truly needed in future, do it without `Task.Run` or `Task.Delay`.

- [x] Done

---

### M-11 · `ObservableClass._propertyChanged` invocation list iteration in `Dispose` is not null-safe if concurrent
**File:** `src/ISynergy.Framework.Core/Base/ObservableClass.cs` lines ~280–295

**Issue**  
`Dispose(bool)` iterates `_propertyChanged.GetInvocationList()` and removes each delegate. If another thread fires `RaisePropertyChanged` concurrently, `_propertyChanged` could become null mid-iteration after the last subscriber is removed (since the last `-=` on an event sets it to null), causing `NullReferenceException`.

**Fix**  
Capture `_propertyChanged` in a local before iterating:
```csharp
var handler = _propertyChanged;
if (handler is not null)
    foreach (var d in handler.GetInvocationList())
        _propertyChanged -= (PropertyChangedEventHandler)d;
```

- [x] Done

---

## Testing strategy

### Philosophy
Every fix must have at least one regression test that would have caught the original bug. Tests live in the matching existing test project. If no project exists for the platform (WPF), create `tests/ISynergy.Framework.UI.WPF.Tests/`.

### Test patterns used in this codebase
| Pattern | When to use |
|---------|-------------|
| `WeakReference<T>` + `GC.Collect` | Verify object is collected after all strong references are released (leak tests) |
| `Task.WhenAll` with parallel `Task.Run` | Verify thread-safety under concurrent access |
| `IServiceCollection` + `ServiceProvider.CreateScope` | Verify DI lifetime correctness |
| `Mock<T>` (Moq) | Verify method calls on injected dependencies |
| `bool _callbackFired` + event subscribe/fire | Verify event subscriptions and unsubscriptions |

---

## Tests per issue

### C-01 · LoadingView anonymous Loaded lambda (WinUI + MAUI)
**Projects:** `ISynergy.Framework.UI.WinUI.Tests`, `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `LoadingViewLeakTests`

```
LoadingView_Loaded_SignalUIReady_HandlerIsRemovable
  → subscribe a named delegate to Loaded, verify it can be unsubscribed
  → after unsubscribe, fire Loaded, verify SignalApplicationUIReady not called twice

LoadingView_AfterNavigatedFrom_LoadedHasNoSubscribers
  → create LoadingView, call OnNavigatedFrom/OnDisappearing
  → verify Loaded invocation list is empty (reflection on the event backing field)
```

---

### C-02 · WinUI ThemeService._window NullReferenceException
**Project:** `ISynergy.Framework.UI.WinUI.Tests`  
**Class:** `ThemeServiceTests`

```
ApplyTheme_BeforeInitializeMainWindow_ThrowsInvalidOperationException
  → new ThemeService(), call ApplyTheme()
  → Assert.ThrowsException<InvalidOperationException>

ApplyTheme_AfterInitializeMainWindow_DoesNotThrow
  → new ThemeService(), call InitializeMainWindow(mockWindow), call ApplyTheme()
  → no exception
```

---

### C-03 · WinUI Application.Exiting never released
**Project:** `ISynergy.Framework.UI.WinUI.Tests`  
**Class:** `ApplicationEventTests`

```
Exiting_AfterDispose_HasNoSubscribers
  → create Application subclass, subscribe handler to Exiting
  → call Dispose()
  → verify Exiting invocation list is null/empty

Exiting_IsNullable_DoesNotThrowWhenNoSubscribers
  → create Application, call ExitSuccess() with no Exiting subscribers
  → no NullReferenceException
```

---

### C-04 · WinUI DialogService Closed handler accumulates
**Project:** `ISynergy.Framework.UI.WinUI.Tests`  
**Class:** `DialogServiceHandlerLeakTests`

```
ShowDialog_WhenSecondDialogQueued_ClosedHandlerIsRemovedAfterFiring
  → open dialog A, queue dialog B (triggers Closed subscription on A)
  → close dialog A
  → verify dialog A's Closed invocation list is empty after handler fires

ShowDialog_CalledNTimes_DoesNotAccumulateClosedHandlers
  → open and close 5 dialogs in sequence while one is active
  → verify Closed invocation list length is always ≤ 1
```

---

### C-05 · MAUI DialogService SemaphoreSlim never disposed
**Project:** `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `DialogServiceDisposalTests`

```
DialogService_ImplementsIDisposable
  → Assert.IsInstanceOfType(new DialogService(...), typeof(IDisposable))

DialogService_Dispose_DoesNotThrow
  → new DialogService(), cast to IDisposable, call Dispose()
  → no exception

DialogService_Dispose_ReleasesWaitHandle
  → dispose service, verify semaphore handle is released
  → (reflect on _dialogSemaphore.AvailableWaitHandle.SafeWaitHandle.IsClosed)
```

---

### C-06 · MAUI cross-thread Windows access
**Project:** `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `DialogServiceValidationTests`

```
ValidateApplicationStateAsync_MethodDoesNotExist
  → Assert.IsNull(typeof(DialogService).GetMethod("ValidateApplicationStateAsync",
       BindingFlags.NonPublic | BindingFlags.Instance))
  → verifies the method was removed
```

---

### C-07 · WaitForLoadingCompleteAsync race (WinUI + MAUI)
**Projects:** `ISynergy.Framework.UI.WinUI.Tests`, `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `LoadingViewRaceTests`

```
WaitForLoadingCompleteAsync_WhenAlreadyCompleted_ReturnsImmediately
  → create LoadingView, call CompleteLoading()
  → call WaitForLoadingCompleteAsync()
  → task must complete within 100 ms (use Task.WhenAny with Task.Delay(100))

WaitForLoadingCompleteAsync_WhenNotYetCompleted_CompletesWhenLoadingCompletes
  → create LoadingView, start WaitForLoadingCompleteAsync
  → call CompleteLoading() after 50 ms
  → verify task completes (await with timeout)

WaitForLoadingCompleteAsync_DeadIsCompletedCheck_IsAbsent
  → reflect on method body or verify via IL that no unnecessary IsCompleted check remains
  → (alternatively: structural test — the returned Task from a fresh call must be incomplete
     before CompleteLoading is called)
```

---

### C-08 · WPF Application.Dispose wrong event
**Project:** `ISynergy.Framework.UI.WPF.Tests` (create if absent)  
**Class:** `ApplicationLifecycleTests`

```
Dispose_UnsubscribesFromLifecycleService_NotFromOwnEvent
  → create mock IApplicationLifecycleService tracking subscription count
  → create Application subclass, verify subscribe count = 1
  → Dispose() the Application
  → verify lifecycle service subscription count = 0

AfterDispose_LifecycleService_ApplicationLoaded_DoesNotInvokeHandler
  → dispose Application
  → fire _lifecycleService.ApplicationLoaded
  → verify OnApplicationLoaded not called
```

---

### C-09 · ObservableValidatedClass double-dispose race
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `ObservableValidatedClassDisposeTests`

```
Dispose_CalledConcurrentlyWithDisposeAsync_DoesNotThrow
  → create TestObservableValidatedClass with errors
  → Task.WhenAll(Task.Run(() => obj.Dispose()), Task.Run(async () => await obj.DisposeAsync()))
  → no exception

Dispose_CalledTwiceConcurrently_DoesNotThrow
  → Task.WhenAll(Enumerable.Range(0,10).Select(_ => Task.Run(() => obj.Dispose())))
  → no exception
```

---

### C-10 · DisposeAsyncCore fires errorsChanged after Properties cleared
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `ObservableValidatedClassDisposeTests`

```
DisposeAsync_ErrorsChangedNotFiredAfterPropertiesCleared
  → subscribe to ErrorsChanged
  → await obj.DisposeAsync()
  → verify no ObjectDisposedException or KeyNotFoundException was thrown
    (wrap in try/catch inside the handler, assert no exception captured)

DisposeAsync_ErrorsChangedUnsubscribedBeforeBaseDispose
  → verify _errorsChanged is null after DisposeAsync completes
  → (reflect on private field)
```

---

### C-11 · WPF SelectionWindow DataContextChanged leak
**Project:** `ISynergy.Framework.UI.WPF.Tests`  
**Class:** `SelectionWindowLeakTests`

```
SelectionWindow_AfterClose_IsCollectedByGC
  → WeakReference<SelectionWindow> weakRef
  → open window, close it, null all local refs
  → GC.Collect(2); GC.WaitForPendingFinalizers(); GC.Collect(2)
  → Assert.IsFalse(weakRef.TryGetTarget(out _))
```

---

### C-12 · ObservableClass.GetHashCode always 0
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `ObservableClassHashCodeTests`

```
GetHashCode_TwoDifferentInstances_ReturnDifferentValues
  → var a = new TestObservableClass(); var b = new TestObservableClass()
  → Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode())

GetHashCode_SameInstance_IsStable
  → var a = new TestObservableClass()
  → Assert.AreEqual(a.GetHashCode(), a.GetHashCode())

HashSet_OfObservableClass_DoesNotDegradeToLinearScan
  → add 1000 instances to HashSet<TestObservableClass>
  → Assert.AreEqual(1000, set.Count)  // would be 1 if all hash to same bucket with Equals by ref
```

---

### C-13 · ObservableClass Properties dictionary not thread-safe
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `ObservableClassThreadSafetyTests`

```
SetValue_ConcurrentFirstWrite_DoesNotThrowOrCorrupt
  → 10 threads each call obj.SetValue(uniquePropertyName, value) simultaneously
  → Assert.AreEqual(10, obj.Properties.Count) — no ArgumentException or corruption

GetValue_ConcurrentRead_DoesNotThrow
  → 10 threads simultaneously read obj.SomeProperty
  → no exception

SetValue_ConcurrentReadWrite_DoesNotThrow
  → 5 threads writing, 5 threads reading the same property
  → no exception, final value is one of the written values
```

---

### H-04 · OnSubmitted duplicated across 5 ViewModels
**Project:** `ISynergy.Framework.Mvvm.Tests`  
**Class:** `ViewModelSelectableTests`

```
OnSubmitted_WithValidModel_RaisesSubmittedEvent
  → var vm = new ViewModelBlade<TestModel>(...)
  → bool raised = false; vm.Submitted += (s, e) => raised = true
  → await vm.SubmitAsync(model)
  → Assert.IsTrue(raised)

OnSubmitted_HandlerThrows_OtherHandlersStillCalled
  → attach 3 handlers; second throws
  → verify handlers 1 and 3 are called, exception is swallowed

(Repeat for all 5 subclasses to verify unified base behavior)
```

---

### H-05 · SelectedItem PropertyChanged pattern duplicated
**Project:** `ISynergy.Framework.Mvvm.Tests`  
**Class:** `ViewModelSelectableTests`

```
SelectedItem_PropertyChanged_PropagatesFromItem
  → assign a TestModel to SelectedItem
  → change a property on the TestModel
  → verify ViewModel.PropertyChanged fires

SelectedItem_Replaced_UnsubscribesFromOldItem
  → assign model A to SelectedItem
  → assign model B to SelectedItem
  → change property on model A
  → verify ViewModel.PropertyChanged does NOT fire (unsubscribed from A)

SelectedItem_SetToNull_DoesNotThrow
  → assign model, set SelectedItem = null
  → no NullReferenceException
```

---

### H-06 · ViewModelBladeWizard / ViewModelDialogWizard duplication
**Project:** `ISynergy.Framework.Mvvm.Tests`  
**Class:** `ViewModelWizardTests`

```
Wizard_InitialState_FirstPageActive_BackDisabled_NextEnabled
Wizard_Next_AdvancesToNextPage_UpdatesIsEnabled
Wizard_Back_ReturnsToPreviousPage_UpdatesIsEnabled
Wizard_OnLastPage_NextDisabled_SubmitEnabled
Wizard_Submit_RaisesSubmittedEvent

(Run identical test body against both ViewModelBladeWizard and ViewModelDialogWizard
 to verify unified behavior after extraction)
```

---

### H-08 · MigrationService captive dependency (MAUI)
**Project:** `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `MigrationServiceLifetimeTests`

```
MigrationService_IsRegisteredAsScoped
  → build ServiceProvider from MauiAppBuilderExtensions
  → var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMigrationService))
  → Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime)

MigrationService_TwoScopes_ReturnDifferentInstances
  → using scope1 = provider.CreateScope()
  → using scope2 = provider.CreateScope()
  → Assert.AreNotSame(scope1.GetService<IMigrationService>(), scope2.GetService<IMigrationService>())
```

---

### H-09 · WinUI double ServiceLocator registration
**Project:** `ISynergy.Framework.UI.WinUI.Tests`  
**Class:** `ServiceLocatorRegistrationTests`

```
ServiceLocator_AfterHostStart_ResolvesFromFinalProvider
  → build host, verify ServiceLocator resolves IBusyService
  → verify it is the same instance as host.Services.GetService<IBusyService>()

ConfigureServices_DoesNotCallBuildServiceProvider
  → capture IServiceCollection passed to ConfigureServices
  → verify no intermediate BuildServiceProvider call leaks a second provider
```

---

### H-13 · ViewModelSummary missing PropertyChanged
**Project:** `ISynergy.Framework.Mvvm.Tests`  
**Class:** `ViewModelSummaryTests` (already exists — add cases)

```
SelectedItem_PropertyChanged_PropagatesFromItem
  → same pattern as H-05 tests but for ViewModelSummary<T>

SubmitCommand_CanExecute_ReevaluatesOnSelectedItemPropertyChange
  → assign model to SelectedItem
  → verify CanExecuteChanged fires when model property changes
```

---

### H-14 · ThemeService lifetime inconsistency
**Projects:** `ISynergy.Framework.UI.WinUI.Tests`, `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `ThemeServiceLifetimeTests`

```
ThemeService_IsRegisteredAsScoped (run in both test projects)
  → var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IThemeService))
  → Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime)

ThemeService_TwoScopes_ReturnDifferentInstances
  → two scopes must not share a ThemeService (verifies no captive singleton)
```

---

### H-15 · WPF builder missing service registrations
**Project:** `ISynergy.Framework.UI.WPF.Tests`  
**Class:** `WpfServiceRegistrationTests`

```
WpfBuilder_RegistersIDispatcherService
WpfBuilder_RegistersIApplicationLifecycleService
WpfBuilder_RegistersITokenStorageService
WpfBuilder_RegistersIMigrationService_AsScoped

(One test method per missing service; each resolves from provider and Assert.IsNotNull)
```

---

### H-17 · WinUI DialogService not disposed by DI
**Project:** `ISynergy.Framework.UI.WinUI.Tests`  
**Class:** `DialogServiceDisposalTests`

```
DialogService_DisposedWhenContainerDisposed
  → register DialogService using the dual-registration pattern
  → build provider, resolve IDialogService, dispose provider
  → verify concrete DialogService.Dispose() was called
    (use a subclass or mock that tracks Dispose calls)
```

---

### M-01 · BusyService not thread-safe
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `BusyServiceThreadSafetyTests`

```
StartBusy_ConcurrentCalls_DoesNotThrow
  → 20 threads simultaneously call StartBusy() / StopBusy()
  → no exception, IsBusy has a valid bool value at the end

UpdateMessage_ConcurrentCalls_DoesNotCorruptState
  → 10 threads call UpdateMessage() with different strings simultaneously
  → no exception, BusyMessage is one of the valid strings
```

---

### M-05 · MAUI NavigationService recursive dispatch
**Project:** `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `NavigationServiceDispatchTests`

```
NavigateModalAsync_WhenDispatcherAlwaysNotRequired_DoesNotRecurseInfinitely
  → mock IDispatcher where IsDispatchRequired always returns false
  → call NavigateModalAsync<TestViewModel>()
  → completes without StackOverflowException (use Task.WhenAny with 2s timeout)
```

---

### M-09 · ObservableValidatedClass _disposed set outside if(disposing)
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `ObservableValidatedClassDisposeTests`

```
Dispose_False_DoesNotRunManagedCleanup_ButSetsDisposed
  → call Dispose(false) via reflection (or through a subclass that exposes it)
  → Validator should still be non-null (managed cleanup skipped)
  → _disposed should be true (flag always set)
  → After fix: verify _disposed is set by base only, not duplicated
```

---

### M-10 · MAUI ValidateApplicationStateAsync mandatory 5-second delay
**Project:** `ISynergy.Framework.UI.Maui.Tests`  
**Class:** `DialogServiceValidationTests`

```
ShowDialog_DoesNotWaitFiveSeconds
  → mock all dependencies to be ready
  → measure time to call ShowInformationAsync
  → Assert elapsed < 500 ms (was previously up to 5000 ms)
```

---

### M-11 · ObservableClass._propertyChanged concurrent disposal
**Project:** `ISynergy.Framework.Core.Tests`  
**Class:** `ObservableClassConcurrentDisposeTests`

```
Dispose_WhilePropertyChangedFiringConcurrently_DoesNotThrowNullReferenceException
  → subscribe to PropertyChanged with a handler that loops
  → simultaneously: Dispose() on one thread, SetValue() on another
  → no NullReferenceException

Dispose_PropertyChangedListCapturedBeforeIteration_IsThreadSafe
  → verify via concurrent stress test: 50 iterations of parallel Dispose + SetValue
  → zero exceptions across all iterations
```

---

## New test project required

`tests/ISynergy.Framework.UI.WPF.Tests/` does not exist. Create it with:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="MSTest.TestFramework" />
    <PackageReference Include="MSTest.TestAdapter" />
    <PackageReference Include="Moq" />
    <PackageReference Include="coverlet.collector">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\ISynergy.Framework.UI.WPF\ISynergy.Framework.UI.WPF.csproj" />
  </ItemGroup>
</Project>
```
Tests: C-08, C-11, H-15, M-08.

---

## Dependency map — recommended fix order

```
C-13 (Properties thread-safety) → affects all SetValue/GetValue callers
C-12 (GetHashCode)              → affects all Dictionary/HashSet usage
C-09 (Dispose guard)            → prerequisite for C-10
C-10 (async dispose race)       → after C-09
C-08 + M-08 (WPF Application)  → fix together (same root)
C-01 (Loaded lambda)            → isolated, fix immediately
C-07 (WaitForLoading race)      → isolated, fix immediately
C-02, C-03, C-04               → isolated WinUI fixes
C-05, C-06                     → isolated MAUI fixes
C-11                           → isolated WPF fix

H-04 + H-05 → create ViewModelSelectable<TModel> first, then H-06, H-13, M-07 are free
H-01 (BaseNavigationService)   → resolves M-02, M-03 automatically
H-02 (BaseDialogService)       → standalone extraction
H-03 (move shared services)    → standalone move
H-09 (ServiceLocator)          → standalone fix
H-10, H-11, H-12, H-17        → standalone fixes
H-14, H-15, H-16              → fix together (registrations audit)
```

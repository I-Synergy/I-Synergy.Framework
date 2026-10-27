# Plan: Fix WinUI COM / NullReferenceException hazards

## Context

After resolving a `NullReferenceException` in `ApplicationExtensions.SetApplicationColor` (item.Source on a code-created ResourceDictionary), a broader review of the WinUI library and sample identified 10 additional patterns that cause or risk `NullReferenceException` / `COMException` at runtime. These fall into four categories:

1. WinRT objects accessed before the XAML infrastructure is initialised (static field initialisers, pre-Loaded constructor code)
2. `WindowNative.GetWindowHandle` called with a potentially null `Application.MainWindow`
3. UI element properties mutated from thread-pool threads (COM `RPC_E_WRONG_THREAD`)
4. Race conditions and silent action drops in dialog / navigation / window helpers
5. `Application.Current.Resources[...]` indexer access without null guard (converters)

All fixes are targeted and minimal — no structural changes.

---

## Files to modify

| # | File | Lines affected |
|---|------|---------------|
| 1 | `src/ISynergy.Framework.UI.WinUI/Controls/BladeView/BladeView.xaml.cs` | 62, 142 |
| 2 | `src/ISynergy.Framework.UI.WinUI/Controls/LoadingView.cs` | 176, 241–244 |
| 3 | `src/ISynergy.Framework.UI.WinUI/Services/FileService.cs` | 163 |
| 4 | `src/ISynergy.Framework.UI.WinUI/Services/UpdateService.cs` | 53 |
| 5 | `samples/Sample.WinUI/ViewModels/SlideShowViewModel.cs` | 18, 117–130 |
| 6 | `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` | 490–496 |
| 7 | `src/ISynergy.Framework.UI.WinUI/Helpers/WindowHelper.cs` | 64 |
| 8 | `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` | 136–142 |
| 9 | `src/ISynergy.Framework.UI.WinUI/Converters/StateConverters.cs` | 25, 30, 33 |
| 10 | `src/ISynergy.Framework.UI.WinUI/Converters/ResourceConverters.cs` | 26, 66 |

---

## Fix details

### Fix 1 — BladeView.xaml.cs: remove `ThemeDictionaries` access from static field initialiser and constructor

**Root cause:** `Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"]` runs at class-load time (line 61) and again inside `InitializeDefaultValues()` called from the constructor (line 140). The XAML resource system may not be ready yet, causing `KeyNotFoundException` or `NullReferenceException`.

**Line 61** — Change the `PropertyMetadata` default from a live resource lookup to `null`:
```csharp
// Before
new PropertyMetadata(Microsoft.UI.Xaml.Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"] as Brush)

// After
new PropertyMetadata(null)
```

**Line 140** — Replace the direct cast with a safe `TryGetValue` lookup:
```csharp
// Before
this.DisabledBackground = (Brush)Microsoft.UI.Xaml.Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"];

// After
if (Microsoft.UI.Xaml.Application.Current?.Resources.TryGetValue("ApplicationPageBackgroundThemeBrush", out var brush) == true && brush is Brush themeBrush)
    this.DisabledBackground = themeBrush;
```

Note: the lookup moves from `ThemeDictionaries` (keyed by theme name) to `Resources` (keyed by resource key), which is the correct API for named resources.

---

### Fix 2 — LoadingView.cs: two independent fixes

**2a — Line 176:** `Application.Current` null guard (constructor runs before full XAML init in designer/test)

```csharp
// Before
if (Application.Current.Resources.TryGetValue("Primary", out var primaryResource) && primaryResource is Brush primaryBrush)

// After
if (Microsoft.UI.Xaml.Application.Current?.Resources.TryGetValue("Primary", out var primaryResource) == true && primaryResource is Brush primaryBrush)
```

**2b — Lines 241–244:** `OnApplicationInitialized` fires on a thread-pool thread (raised by `InitializeApplicationAsync` which runs as a background Task). Setting `IsSignInButtonEnabled` from a non-UI thread throws `COMException` RPC_E_WRONG_THREAD.

```csharp
// Before
private void OnApplicationInitialized(object? sender, EventArgs e)
{
    IsSignInButtonEnabled = true;
}

// After
private void OnApplicationInitialized(object? sender, EventArgs e)
{
    DispatcherQueue.TryEnqueue(() => IsSignInButtonEnabled = true);
}
```

`DispatcherQueue` is inherited from `DependencyObject` (via `Page`) and is always valid while the page is alive.

---

### Fix 3 — FileService.cs line 163: null guard before `GetWindowHandle`

`WindowNative.GetWindowHandle` dereferences a COM pointer — passing null is an immediate NRE.

```csharp
// Before
var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Application.MainWindow);

// After
var mainWindow = Application.MainWindow
    ?? throw new InvalidOperationException("Cannot open file picker: main window is not available.");
var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
```

---

### Fix 4 — UpdateService.cs line 53: null guard before `GetWindowHandle`

Same pattern as Fix 3, but wrapped in a try-catch that returns `false` on failure. An early-return on null is more correct than throwing:

```csharp
// Before
var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Application.MainWindow);

// After
if (Application.MainWindow is null)
    return result;
var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Application.MainWindow);
```

---

### Fix 5 — SlideShowViewModel.cs: two independent fixes

**5a — Line 18:** `DispatcherQueue.GetForCurrentThread()` is a field initialiser — runs at construction time on whatever thread the ViewModel is created. Change to nullable and add guard at usage sites.

```csharp
// Before
private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

// After
private readonly DispatcherQueue? _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
```

**5b — Lines 117–130:** `UpdateSourceTimer_Tick` fires on a `System.Timers.Timer` thread-pool thread. Assigning `Items` (an MVVM observable property) from a non-UI thread triggers `PropertyChanged` listeners on the wrong thread. Wrap in dispatcher, matching the pattern already used in `SlideshowTimer_Tick` (lines 96–109):

```csharp
// Before
private void UpdateSourceTimer_Tick(object? sender, ElapsedEventArgs e)
{
    UpdateSourceTimer.Enabled = false;
    Items = new ObservableCollection<MediaItem>()
    {
        new MediaItem { ... },
        ...
    };
    UpdateSourceTimer.Enabled = true;
}

// After
private void UpdateSourceTimer_Tick(object? sender, ElapsedEventArgs e)
{
    UpdateSourceTimer.Enabled = false;
    _dispatcherQueue?.TryEnqueue(() =>
    {
        Items = new ObservableCollection<MediaItem>()
        {
            new MediaItem { ... },
            ...
        };
    });
    UpdateSourceTimer.Enabled = true;
}
```

The `?` on `_dispatcherQueue` handles the off-thread construction scenario (Fix 5a).

---

### Fix 6 — DialogService.cs lines 490–493: capture `_activeDialog` locally before event subscription

If the timeout branch (line 511) sets `_activeDialog = null` before the `Closed` handler fires, the closure dereferences the null field.

```csharp
// Before
TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs>? handler = null;
handler = (s, e) =>
{
    _activeDialog.Closed -= handler;
    tcs.TrySetResult(true);
};
_activeDialog.Closed += handler;
await _activeDialog.CloseAsync();

// After
var capturedDialog = _activeDialog;
TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs>? handler = null;
handler = (s, e) =>
{
    capturedDialog.Closed -= handler;
    tcs.TrySetResult(true);
};
capturedDialog.Closed += handler;
await capturedDialog.CloseAsync();
```

---

### Fix 7 — WindowHelper.cs line 64: null check on `window.Content`

A window may exist in `_activeWindows` before content is assigned (the list is populated at window creation). Accessing `.XamlRoot` on a null `Content` crashes.

```csharp
// Before
if (element.XamlRoot == window.Content.XamlRoot)

// After
if (window.Content is { } content && element.XamlRoot == content.XamlRoot)
```

---

### Fix 8 — NavigationService.cs lines 136–141: fallback to `GetForCurrentThread()` when `MainWindow` is null

The current implementation silently drops the action when `MainWindow` is null. Match the pattern used in `DispatcherService.cs` (lines 34–37):

```csharp
// Before
protected override void DispatchOnUIThread(Action action)
{
    if (Application.MainWindow?.DispatcherQueue is not null)
    {
        Application.MainWindow.DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            new Microsoft.UI.Dispatching.DispatcherQueueHandler(action));
    }
}

// After
protected override void DispatchOnUIThread(Action action)
{
    var queue = Application.MainWindow?.DispatcherQueue
        ?? Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
    queue?.TryEnqueue(
        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
        new Microsoft.UI.Dispatching.DispatcherQueueHandler(action));
}
```

---

### Fix 9 — StateConverters.cs: guard `Application.Current.Resources[...]` indexer accesses

**Root cause:** Three converter return paths use `Application.Current.Resources["TertiaryDarkBrush"]` and `["PrimaryMediumBrush"]` with no null guard on `Application.Current` and no `TryGetValue`. If a theme is missing one of these brushes, the direct indexer throws `KeyNotFoundException`.

**Lines 25, 30, 33** — Replace direct indexer with safe `TryGetValue`:

```csharp
// Before (line 25)
return Application.Current.Resources["TertiaryDarkBrush"] as SolidColorBrush;
// (line 30)
return Application.Current.Resources["PrimaryMediumBrush"] as SolidColorBrush;
// (line 33)
return Application.Current.Resources["TertiaryDarkBrush"] as SolidColorBrush;

// After
return Application.Current?.Resources.TryGetValue("TertiaryDarkBrush", out var fallbackBrush) == true && fallbackBrush is SolidColorBrush solidFallbackBrush
    ? solidFallbackBrush
    : new SolidColorBrush(Microsoft.UI.Colors.Gray);
// (line 30)
return Application.Current?.Resources.TryGetValue("PrimaryMediumBrush", out var activeBrush) == true && activeBrush is SolidColorBrush solidActiveBrush
    ? solidActiveBrush
    : new SolidColorBrush(Microsoft.UI.Colors.Gray);
// (line 33)
return Application.Current?.Resources.TryGetValue("TertiaryDarkBrush", out var fallbackBrush2) == true && fallbackBrush2 is SolidColorBrush solidFallbackBrush2
    ? solidFallbackBrush2
    : new SolidColorBrush(Microsoft.UI.Colors.Gray);
```

Note: the `SolidColorBrush(Microsoft.UI.Colors.Gray)` fallback ensures the converter never returns null, matching the existing pattern of always returning a Brush.

---

### Fix 10 — ResourceConverters.cs: guard `Application.Current.Resources[parameter]` indexer accesses

**Root cause:** Two converters dereference `Application.Current.Resources[parameter]` with a user-supplied resource key from a XAML binding. No null guard on `Application.Current` and a missing resource key throws `KeyNotFoundException`.

**Line 26** — `ResourceNameToGeometryConverter`:

```csharp
// Before
if (parameter is string)
{
    return (Geometry)XamlReader.Load($"<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{Application.Current.Resources[parameter]}</Geometry>");
}
return (Geometry)XamlReader.Load("<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'></Geometry>");

// After
if (parameter is string resourceKey)
{
    if (Application.Current?.Resources.TryGetValue(resourceKey, out var geometryData) == true && geometryData is string geometryString)
    {
        return (Geometry)XamlReader.Load($"<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{geometryString}</Geometry>");
    }
    return (Geometry)XamlReader.Load("<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><EllipseGeometry/></Geometry>");
}
return (Geometry)XamlReader.Load("<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'></Geometry>");
```

**Line 66** — `ResourceNameToStringConverter`:

```csharp
// Before
if (parameter is string)
{
    return Application.Current.Resources[parameter] as string;
}
return string.Empty;

// After
if (parameter is string resourceKey)
{
    return Application.Current?.Resources.TryGetValue(resourceKey, out var resourceValue) == true
        ? resourceValue as string
        : null;
}
return string.Empty;
```

---

## Execution order

Fixes are independent — each file can be edited in a single pass. Recommended order (highest risk first):

1. Fix 1 (BladeView — static initialiser crash)
2. Fix 3 (FileService — COM null dereference)
3. Fix 4 (UpdateService — COM null dereference)
4. Fix 2b (LoadingView — wrong-thread UI mutation)
5. Fix 5b + 5a (SlideShowViewModel — wrong-thread + null dispatcher)
6. Fix 6 (DialogService — closure race)
7. Fix 7 (WindowHelper — null Content)
8. Fix 8 (NavigationService — silent drop fallback)
9. Fix 9 (StateConverters — resource indexer null guard)
10. Fix 10 (ResourceConverters — resource indexer null guard)
11. Fix 2a (LoadingView — Application.Current null guard, lowest risk)

All fixes are in a single developer agent pass.

---

## Verification

1. Build `ISynergy.Framework.UI.WinUI` — must produce zero errors and zero new warnings
2. Build `Sample.WinUI` — must produce zero errors
3. Run the WinUI sample app:
   - Launch verifies Fix 1 (BladeView DependencyProperty), Fix 2a/b (LoadingView), Fix 8 (NavigationService)
   - Open a file picker dialog — verifies Fix 3
   - Navigate to the SlideShow view and wait 5 seconds — verifies Fix 5a/b (timer dispatch)
   - Let the app idle — verifies Fix 4 (UpdateService, update check on startup)
   - Verify button states and resource-based UI elements render correctly — verifies Fix 9, Fix 10
4. All existing tests must remain green (`dotnet test`)

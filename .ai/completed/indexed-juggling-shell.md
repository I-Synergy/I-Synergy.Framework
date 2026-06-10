# Fix RTL Flow — Set Once at Root, Automatic Inheritance

**Status:** DONE
**Started:** 2026-06-05

## Context

Two RTL bugs exist:
1. **MAUI**: When Arabic is selected, the Shell/FlyoutPage title stays left-aligned (should be right-aligned).
2. **WinUI**: The native window caption buttons (close/min/max) stay on the right (should mirror to the left).

Additionally, runtime-created pages (dialogs, navigation destinations) don't inherit the Window's FlowDirection — the current code requires each view to call `ApplyFlowDirection()` individually in its constructor.

**Goal**: Set RTL FlowDirection once at the application root, and have every page, dialog, popup, and child view inherit it automatically — no per-view code required.

## Root Causes

### MAUI Title Left-Aligned
- `Application.CreateWindow()` sets `Window.FlowDirection` on the MAUI Window, but this property does not propagate through the native Windows title bar — the title bar is OS-level chrome, not a MAUI VisualElement.
- Also, MAUI's `NavigationPage` built-in navigation bar title may not reliably inherit FlowDirection across all platforms and scenarios (the codebase already documents this: FlyoutPage children "don't always inherit FlowDirection from the parent").

### WinUI Caption Buttons on the Right
- WinUI 3 / Windows App SDK's `AppWindow.TitleBar` has **no RTL API**. The caption buttons (close/min/max) are rendered by DWM as OS chrome, positioned by the system locale.
- The current `AppWindowExtensions.ApplyFlowDirectionToTitleBar()` only sets button background colors transparent — it doesn't actually mirror the title bar.
- The fix is to use `WS_EX_LAYOUTRTL` (0x00400000) via P/Invoke to mirror the native window frame (caption buttons + title text). Microsoft confirmed this as the supported workaround (WindowsAppSDK #3452, microsoft-ui-xaml #8746).

### Runtime Pages Don't Inherit FlowDirection
- `NavigationExtensions.CreatePage()` creates pages from DI without setting FlowDirection.
- `DialogService.CreateDialogAsync()` pushes modal ContentPages without FlowDirection.
- MAUI's `Window.FlowDirection` should propagate through the visual tree, but the FlyoutPage child issue proves inheritance is unreliable for certain page types, and modal pages sit on a separate navigation stack.

## Solution Architecture

Three layers of fix, each applied ONCE from the application root:

| Layer | What it fixes | Where it lives |
|-------|--------------|-----------------|
| **Win32 native title bar** | Caption buttons (min/max/close) and native title text alignment | `WS_EX_LAYOUTRTL` on window HWND |
| **Framework root FlowDirection** | All framework content layout (pages, controls, lists) | `Window.FlowDirection` (MAUI) / `FrameworkElement.FlowDirection` (WinUI/WPF) |
| **Runtime page auto-inheritance** | Dialogs, modals, navigated pages created after startup | Base `View` class `OnAppearing` hook (MAUI) |

## Detailed Steps

### Step 1: Add Win32 RTL helper (shared P/Invoke)

**File to create**: `src/ISynergy.Framework.UI.WinUI/Extensions/WindowNativeExtensions.cs`

- P/Invoke declarations for `GetWindowLong` / `SetWindowLong` from `user32.dll`
- Constants: `GWL_EXSTYLE = -20`, `WS_EX_LAYOUTRTL = 0x00400000`
- Extension method `ApplyNativeFlowDirection(this AppWindow, string cultureName)` that gets the HWND via `WinRT.Interop.WindowNative.GetWindowHandle`, then sets/clears `WS_EX_LAYOUTRTL`
- Extension method `ApplyNativeFlowDirection(this Microsoft.UI.Xaml.Window, string cultureName)` — same, for pure WinUI usage

### Step 2: Fix MAUI native title bar

**File to modify**: `src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/AppWindowExtensions.cs`

- In `ApplyFlowDirectionToTitleBar()`: call the new `WindowNativeExtensions.ApplyNativeFlowDirection(appWindow, cultureName)` to set/clear `WS_EX_LAYOUTRTL`
- Keep the existing button background color adjustments as they ensure visual consistency
- The method now actually mirrors the native title bar (caption buttons + title text), not just adjusts colors

### Step 3: MAUI runtime page auto-inheritance

**File to modify**: `src/ISynergy.Framework.UI.Maui/Controls/View.cs`

- Override `OnAppearing()` in the base `View` class to sync `FlowDirection` from the application's main Window:

```csharp
protected override void OnAppearing()
{
    base.OnAppearing();
    // Auto-inherit FlowDirection from the root MAUI Window.
    // This covers all pages including those created at runtime (dialogs, modal nav).
    if (Application.Current?.Windows.Count > 0)
    {
        var mainWindow = Application.Current.Windows[0];
        if (mainWindow.FlowDirection != FlowDirection.MatchParent)
        {
            FlowDirection = mainWindow.FlowDirection;
        }
    }
}
```

- Also add this to `Window` control (`src/ISynergy.Framework.UI.Maui/Controls/Window.cs`) since it extends `ContentPage` directly (not `View`)

### Step 4: Fix WinUI Application root FlowDirection

**File to modify**: `src/ISynergy.Framework.UI.WinUI/Application/Application.cs`

- In `OnLaunched()`, after `_mainWindow.Activate()`, apply FlowDirection to the root content:

```csharp
var language = _settingsService?.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;
if (_mainWindow.Content is FrameworkElement rootElement)
    rootElement.ApplyFlowDirection(language);
```

- Also apply the native title bar RTL after window creation:

```csharp
var appWindow = WindowHelper.GetAppWindow(_mainWindow);
appWindow?.ApplyNativeFlowDirection(language);
```

### Step 5: Fix WPF Application root FlowDirection

**File to modify**: `src/ISynergy.Framework.UI.WPF/Application/Application.cs`

- In the startup hook (`OnStartup` or equivalent), apply FlowDirection to the main Window:

```csharp
var language = _settingsService?.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;
_mainWindow.ApplyFlowDirection(language);
```

### Step 6: Remove redundant per-view FlowDirection calls from samples

**Files to modify**:
- `samples/Sample.Maui/Views/ShellView.xaml.cs` — remove `this.ApplyWindowFlowDirection(...)` call
- `samples/Sample.WinUI/Views/ShellView.xaml.cs` — remove `this.ApplyFlowDirection(...)` call
- `samples/Sample.WPF/Views/ShellView.xaml.cs` — remove `this.ApplyFlowDirection(...)` call

### Step 7: Extend RTL language list

**File to modify**: `src/ISynergy.Framework.Globalization/Constants/GlobalizationConstants.cs`

- Add Hebrew (`"he"`), Persian/Farsi (`"fa"`), Urdu (`"ur"`), and Divehi (`"dv"`) to `RightToLeftLanguages`:

```csharp
private static readonly string[] RightToLeftLanguages = ["ar", "he", "fa", "ur", "dv"];
```

## Files to Modify (summary)

| File | Change |
|------|--------|
| `src/ISynergy.Framework.UI.WinUI/Extensions/WindowNativeExtensions.cs` | **NEW** — P/Invoke WS_EX_LAYOUTRTL helper |
| `src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/AppWindowExtensions.cs` | Call WS_EX_LAYOUTRTL helper in `ApplyFlowDirectionToTitleBar` |
| `src/ISynergy.Framework.UI.Maui/Controls/View.cs` | Override `OnAppearing` to auto-inherit FlowDirection from main Window |
| `src/ISynergy.Framework.UI.Maui/Controls/Window.cs` | Override `OnAppearing` to auto-inherit FlowDirection from main Window |
| `src/ISynergy.Framework.UI.WinUI/Application/Application.cs` | Set FlowDirection + native title bar RTL in `OnLaunched` |
| `src/ISynergy.Framework.UI.WPF/Application/Application.cs` | Set FlowDirection at startup |
| `samples/Sample.Maui/Views/ShellView.xaml.cs` | Remove `ApplyWindowFlowDirection` call |
| `samples/Sample.WinUI/Views/ShellView.xaml.cs` | Remove `ApplyFlowDirection` call |
| `samples/Sample.WPF/Views/ShellView.xaml.cs` | Remove `ApplyFlowDirection` call |
| `src/ISynergy.Framework.Globalization/Constants/GlobalizationConstants.cs` | Extend RTL language list |

## Verification

1. **MAUI**: Run Sample.Maui with Arabic language — verify:
   - ShellView title bar (NavigationPage title) is right-aligned
   - Flyout menu opens from the right
   - Native Windows title bar caption buttons are on the left
   - Dialog windows (opened via DialogService) have right-aligned titles
   - Content pages navigated via NavigationService have RTL layout
2. **WinUI**: Run Sample.WinUI with Arabic language — verify:
   - Native caption buttons (min/max/close) are on the left
   - NavigationView content flows RTL
   - New pages navigated via Frame have RTL layout
3. **WPF**: Run Sample.WPF — verify RTL layout propagates to all windows
4. **Regression**: Verify English still renders LTR correctly across all platforms

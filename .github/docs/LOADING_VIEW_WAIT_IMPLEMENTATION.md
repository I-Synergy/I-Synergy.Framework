# Loading View - Video Waiting Implementation

## Overview

The `LoadingView` control has been updated to support waiting for video playback completion before navigating to the login screen.

## Changes Made

### 1. **LoadingView - Made Public**

**File**: `src/ISynergy.Framework.UI.Maui/Controls/LoadingView.cs`

Changed visibility from `internal` to `public` so the class can be accessed from the Sample.Maui app:

```csharp
public class LoadingView : ContentPage  // was: internal class LoadingView
```

### 2. **LoadingView - WaitForLoadingCompleteAsync() Method**

Added a public method that returns a Task that completes when the video finishes or the user clicks skip:

```csharp
public Task WaitForLoadingCompleteAsync()
{
    var tcs = new TaskCompletionSource<bool>();
    
    EventHandler? handler = null;
    handler = (s, e) =>
    {
        LoadingCompleted -= handler;
      tcs.TrySetResult(true);
    };
    
    LoadingCompleted += handler;
    return tcs.Task;
}
```

### 3. **Application - Protected LoadingView Reference**

**File**: `src/ISynergy.Framework.UI.Maui/Application/Application.cs`

Added a protected field to store the LoadingView instance:

```csharp
protected object? _loadingView;  // Store as object to avoid accessibility issues
```

### 4. **Application - CreateLoadingViewPage() Helper**

Added a helper method to create and store the LoadingView reference:

```csharp
private NavigationPage CreateLoadingViewPage()
{
    var loadingView = new LoadingView(_commonServices, _splashScreenOptions);
    _loadingView = loadingView;
    return new NavigationPage(loadingView);
}
```

### 5. **Application - WaitForLoadingViewAsync() Method**

Added a protected method that intelligently waits for the LoadingView:

```csharp
protected async Task WaitForLoadingViewAsync()
{
    // Only wait for LoadingView if splash screen type is Video
    if (_splashScreenOptions.SplashScreenType == SplashScreenTypes.Video && 
        _loadingView is LoadingView loadingView)
    {
        _logger?.LogInformation("Waiting for video loading view to complete...");
  await loadingView.WaitForLoadingCompleteAsync();
        _logger?.LogInformation("Video loading view completed");
    }
    // For Image and None types, this returns immediately
}
```

**Key Feature**: Only waits if splash screen type is `Video`. For `Image` and `None` types, it returns immediately, maintaining original behavior.

### 6. **Sample.Maui - Updated ApplicationLoadedAsync()**

**File**: `samples/Sample.Maui/App.xaml.cs`

Updated the ApplicationLoadedAsync method to call the base class waiting method:

```csharp
private async Task ApplicationLoadedAsync()
{
    try
    {
    _commonServices.BusyService.StartBusy();

    // Wait for the loading view to complete if it's a video
        // For Image and None types, this returns immediately
        await WaitForLoadingViewAsync();

        bool navigateToAuthentication = true;
  _logger.LogInformation("Application loaded event: checking for auto-login");

      if (navigateToAuthentication)
   {
            _logger.LogInformation("Navigate to SignIn page");
            await _navigationService.NavigateModalAsync<SignInViewModel>();
        }
    }
    catch (Exception ex)
    {
    _logger?.LogError(ex, "Error in ApplicationLoaded");
 }
    finally
    {
      _commonServices.BusyService.StopBusy();
    }
}
```

## Behavior by Splash Screen Type

| Type | Behavior |
|------|----------|
| **Video** | ? Waits for video to finish playing OR user to click Skip button, THEN navigates |
| **Image** | ?? Returns immediately, navigates right away (original behavior) |
| **None** | ?? Returns immediately, navigates right away (original behavior) |

## How It Works

### Video Splash Screen Flow

1. **Application starts** ? Shows `LoadingView` with video
2. **User sees video playing** with Skip button available
3. **Video ends OR Skip clicked** ? `LoadingView.CompleteLoading()` is called
4. **LoadingCompleted event fired** ? `WaitForLoadingCompleteAsync()` completes
5. **Navigation proceeds** ? User navigates to SignIn screen

### Image/None Splash Screen Flow

1. **Application starts** ? Shows `LoadingView` with image or empty
2. **ApplicationLoaded event fires** ? `WaitForLoadingViewAsync()` is called
3. **Check: Is it Video?** ? No, so return immediately
4. **Navigation proceeds** ? User navigates to SignIn screen (no waiting)

## Key Benefits

? **Smart Waiting**: Only waits for video, preserves original behavior for other types
? **Clean Architecture**: Uses base class protected method for coordination
? **Extensible**: Derived applications can override `WaitForLoadingViewAsync()` if needed
? **Backward Compatible**: Existing applications without video splash screens work as before
? **User Experience**: Users see the full video/splash screen before navigation

## Integration Points

- **LoadingView**: Public class with `WaitForLoadingCompleteAsync()` method
- **Application**: Protected `_loadingView` field and `WaitForLoadingViewAsync()` method
- **Sample.Maui**: Calls `WaitForLoadingViewAsync()` in `ApplicationLoadedAsync()`

## Testing Scenarios

1. **Video splash screen**: Video plays, then navigates to login
2. **Skip button**: Click skip, immediately navigates to login
3. **Image splash screen**: Navigates immediately without waiting
4. **None splash screen**: Navigates immediately without waiting

## Files Modified

- `src/ISynergy.Framework.UI.Maui/Controls/LoadingView.cs` - Made public, added WaitForLoadingCompleteAsync()
- `src/ISynergy.Framework.UI.Maui/Application/Application.cs` - Added LoadingView coordination
- `samples/Sample.Maui/App.xaml.cs` - Updated to use WaitForLoadingViewAsync()

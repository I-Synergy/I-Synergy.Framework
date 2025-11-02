# LoadingView - Reusable Control

## Overview

The `LoadingView` control has been extracted from the Sample.Maui project and converted into a reusable, framework-level control in `ISynergy.Framework.UI.Maui`. This control follows the same architectural pattern as the existing `EmptyView` control and provides a standardized loading experience with optional media background support.

## Location

**File**: `src/ISynergy.Framework.UI.Maui/Controls/LoadingView.cs`

## Architecture Pattern

The `LoadingView` control follows the **Clean Architecture** principles established in the framework:

- **No XAML**: Code-behind only implementation (like `EmptyView`)
- **Dependency Injection**: Requires `ICommonServices` through constructor
- **Lifetime Management**: Registered as Singleton via `[Lifetime(Lifetimes.Singleton)]`
- **Event-Driven**: Uses `IApplicationLifecycleService` for coordinated initialization
- **Resource Binding**: Binds directly to framework services (`BusyService`, `BusyMessage`)

## Key Features

### 1. Dynamic Resource Binding
- **Busy Message**: Text automatically updates based on `BusyService.BusyMessage`
- **Visibility**: Label and indicator visibility bound to `BusyService.IsBusy`
- **Activity Indicator**: Auto-running indicator reflecting busy state
- **Theme Colors**: Uses dynamic resources for "Primary" color

### 2. Media Background Support
- **MediaElement**: Optional background media with `AspectFill` layout
- **Media State Management**: Properly handles play/pause/stop states
- **Handler Cleanup**: Disconnects handler on view unload to prevent memory leaks

### 3. Lifecycle Coordination
- **UI Ready Signal**: Signals `IApplicationLifecycleService` when view loads
- **Skip/SignIn Button**: User-controllable completion with enable/disable support
- **Media End Trigger**: Auto-completes loading when media ends
- **Application Initialization**: Enables button when app is initialized

### 4. Customization Properties
```csharp
// Get or set media source for background
public MediaSource? MediaSource { get; set; }

// Control button enabled state
public bool IsSignInButtonEnabled { get; set; }

// Customize button text
public string SignInButtonText { get; set; }

// React to loading completion
public event EventHandler? LoadingCompleted;
```

## Usage

### Basic Usage (Framework)
```csharp
// In ISynergy.Framework.UI.Maui.Application.Application.cs
if (_splashScreenOptions.SplashScreenType != SplashScreenTypes.None)
{
    Application.Current.MainPage = new NavigationPage(new LoadingView(_commonServices));
}
```

### Custom Usage (Your Application)
```csharp
// In your application (e.g., Sample.Maui)
public class LoadingView : ISynergy.Framework.UI.Controls.LoadingView
{
    private readonly IApplicationLifecycleService _applicationLifecycleService;

    public LoadingView(IApplicationLifecycleService applicationLifecycleService, 
         LoadingViewModel viewModel, 
      ICommonServices commonServices)
        : base(commonServices)
    {
     InitializeComponent(); // If adding XAML customization
        
        _applicationLifecycleService = applicationLifecycleService;
        _applicationLifecycleService.ApplicationInitialized += ApplicationInitialized;

        // Set custom media source
  MediaSource = MediaSource.FromResource("embed://gta.mp4");
        SignInButtonText = "Skip";
  
        // Wire up additional events
      LoadingCompleted += OnLoadingCompleted;
    }

    private void ApplicationInitialized(object? sender, EventArgs e)
  {
     IsSignInButtonEnabled = true;
    }

    private void OnLoadingCompleted(object? sender, EventArgs e)
    {
        _applicationLifecycleService.SignalApplicationLoaded();
    }
}
```

## Component Structure

### Visual Hierarchy
```
Grid (Main Container)
??? MediaElement (Background video/media)
??? Grid (Semi-transparent overlay)
??? StackLayout (Center content)
?   ??? Label (Busy message)
?   ??? ActivityIndicator (Loading spinner)
??? Button (Skip/SignIn)
```

### Data Bindings
- **Label.Text**: `ICommonServices.BusyService.BusyMessage` (OneWay)
- **Label.IsVisible**: `ICommonServices.BusyService.IsBusy` (OneWay)
- **ActivityIndicator.IsRunning**: `ICommonServices.BusyService.IsBusy` (OneWay)

### Dynamic Resources
- **Label.TextColor**: `Primary` theme color
- **ActivityIndicator.Color**: `Primary` theme color
- **Button.BackgroundColor**: `Primary` theme color

## Lifecycle Events

The control participates in the application lifecycle through `IApplicationLifecycleService`:

1. **View.Loaded** ? Signals `ApplicationUIReady`
2. **MediaEnded** (optional) ? Triggers `CompleteLoading()`
3. **Button.Clicked** (if enabled) ? Triggers `CompleteLoading()`
4. **CompleteLoading()** ? Signals `ApplicationLoaded`

## Event Cleanup

The control properly manages event subscriptions:

```csharp
protected override void OnDisappearing()
{
    // Unsubscribe from all events
    _signInButton.Clicked -= OnSignInClicked;
    Loaded -= OnViewLoaded;
    Unloaded -= OnViewUnloaded;
    _backgroundMediaElement.MediaEnded -= OnMediaEnded;
    _applicationLifecycleService.ApplicationInitialized -= OnApplicationInitialized;
}
```

## Benefits Over Previous Implementation

? **Reusable**: Available across all applications using the framework
? **Consistent**: Follows established control patterns (like EmptyView)
? **Discoverable**: Part of framework namespace
? **Maintainable**: Single source of truth for loading UX
? **Extensible**: Can be inherited and customized per application
? **Testable**: Dependencies injected, easy to mock

## Related Components

- **EmptyView**: Similar code-behind-only control for non-loading states
- **IApplicationLifecycleService**: Coordinates application initialization events
- **IBusyService**: Provides busy state and message updates
- **ICommonServices**: Access point for framework services

## References

- Architecture: Clean Architecture with Dependency Injection
- Pattern: Code-behind ContentPage control
- Lifetime: Singleton (one instance per application)
- Framework: .NET MAUI with Community Toolkit

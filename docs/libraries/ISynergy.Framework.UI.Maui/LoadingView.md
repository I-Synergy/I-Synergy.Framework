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
- **Dynamic Configuration**: Loads media from `SplashScreenOptions.AssetStreamProvider`

### 3. Lifecycle Coordination
- **UI Ready Signal**: Signals `IApplicationLifecycleService.SignalApplicationUIReady()` on load
- **Coordinates with application initialization**: Works with the framework's lifecycle events
- **Proper event cleanup**: Unsubscribes all events on view unload
- **Application Initialization**: Enables button when app is initialized

### 4. Customizable Properties
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

### 5. Constructor Overloads
```csharp
// Minimal - for EmptyView-like behavior
public LoadingView(ICommonServices commonServices)

// With media configuration
public LoadingView(ICommonServices commonServices, SplashScreenOptions? splashScreenOptions = null)
```

## Usage

### Framework Usage (Automatic)
The framework automatically instantiates `LoadingView` based on `SplashScreenOptions`:

```csharp
// In Application constructor
var mainPage = _splashScreenOptions.SplashScreenType switch
{
    SplashScreenTypes.Video => new NavigationPage(new LoadingView(_commonServices, _splashScreenOptions)),
    SplashScreenTypes.Image => new NavigationPage(new LoadingView(_commonServices, _splashScreenOptions)),
    _ => new NavigationPage(new EmptyView(_commonServices))
};
Application.Current.MainPage = mainPage;
```

### Setting Up SplashScreenOptions with Media

In your application startup, configure the splash screen options:

```csharp
var splashOptions = new SplashScreenOptions
{
 SplashScreenType = SplashScreenTypes.Video,
    ContentType = "video/mp4",
    AssetStreamProvider = async () =>
  {
        // Load embedded resource
        var assembly = typeof(App).Assembly;
        return assembly.GetManifestResourceStream("Sample.Maui.Resources.Raw.gta.mp4");
    }
};

var app = new App(splashOptions);
```

### Custom Inheritance (Advanced)

For application-specific customization:

```csharp
public class CustomLoadingView : ISynergy.Framework.UI.Controls.LoadingView
{
    public CustomLoadingView(ICommonServices commonServices, SplashScreenOptions? options = null)
        : base(commonServices, options)
    {
        // Additional customization
        SignInButtonText = "Let's Go!";
    }
}
```

## Component Structure

### Visual Hierarchy
```
Grid (Main Container)
??? MediaElement (Background video/media)
??? Grid (Semi-transparent overlay - 40% opacity)
??? StackLayout (Center content)
?   ??? Label (Busy message - "Please wait...")
?   ??? ActivityIndicator (Loading spinner)
??? Button (Skip/SignIn button)
```

### Data Bindings
- **Label.Text**: `ICommonServices.BusyService.BusyMessage` (OneWay)
- **Label.IsVisible**: `ICommonServices.BusyService.IsBusy` (OneWay)
- **ActivityIndicator.IsRunning**: `ICommonServices.BusyService.IsBusy` (OneWay)

### Dynamic Resources
- **Label.TextColor**: `Primary` theme color
- **ActivityIndicator.Color**: `Primary` theme color
- **Button.BackgroundColor**: `Primary` theme color
- **Overlay.BackgroundColor**: Semi-transparent black (#00000040)

## Media Source Configuration

### From SplashScreenOptions
When `SplashScreenOptions` includes an `AssetStreamProvider`:

```csharp
public class SplashScreenOptions
{
    // Async function that returns the media stream
    public Func<Task<Stream>>? AssetStreamProvider { get; set; }
    
    // Content type (e.g., "video/mp4", "image/png")
    public string? ContentType { get; set; }
    
// Type of splash screen (None, Video, Image)
    public SplashScreenTypes SplashScreenType { get; set; }
}
```

### Automatic Loading
The `LoadingView` automatically:
1. Checks if `SplashScreenOptions.AssetStreamProvider` is provided
2. Awaits the stream asynchronously
3. Configures the `MediaElement` with the stream
4. Handles errors gracefully with debug logging

### Manual Configuration
```csharp
var loadingView = new LoadingView(commonServices);
loadingView.MediaSource = MediaSource.FromResource("embed://my-video.mp4");
```

## Lifecycle Events

The control participates in the application lifecycle:

1. **View.Loaded** ? Signals `ApplicationUIReady`
2. **ConfigureMediaSource** ? Sets up media from options (async)
3. **View.OnViewLoaded** ? Subscribes to `MediaEnded`, starts playback
4. **MediaEnded** (optional) ? Triggers `CompleteLoading()`
5. **Button.Clicked** (if enabled) ? Triggers `CompleteLoading()`
6. **CompleteLoading()** ? Pauses media, signals `ApplicationLoaded`, raises `LoadingCompleted` event

## Event Cleanup

The control properly manages event subscriptions in `OnDisappearing()`:

```csharp
- _signInButton.Clicked
- Loaded
- Unloaded
- _backgroundMediaElement.MediaEnded
- _applicationLifecycleService.ApplicationInitialized
```

## Benefits Over Previous Implementation

? **Reusable**: Available across all applications using the framework
? **Consistent**: Follows established control patterns (like EmptyView)
? **Discoverable**: Part of framework namespace
? **Maintainable**: Single source of truth for loading UX
? **Extensible**: Can be inherited and customized per application
? **Testable**: Dependencies injected, easy to mock
? **Configurable**: Supports dynamic media from `SplashScreenOptions`

## Related Components

- **EmptyView**: Similar code-behind-only control for non-loading states
- **IApplicationLifecycleService**: Coordinates application initialization events
- **IBusyService**: Provides busy state and message updates
- **ICommonServices**: Access point for framework services
- **SplashScreenOptions**: Configuration for splash screen media
- **SplashScreenTypes**: Enumeration (None, Video, Image)

## References

- Architecture: Clean Architecture with Dependency Injection
- Pattern: Code-behind ContentPage control
- Lifetime: Singleton (one instance per application)
- Framework: .NET MAUI with Community Toolkit
- Media: MAUI MediaElement from Community Toolkit

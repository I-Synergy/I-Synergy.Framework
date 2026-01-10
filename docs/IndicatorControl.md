# IndicatorControl - Reusable Busy Indicator Component

## Overview

The `IndicatorControl` is a reusable .NET MAUI control that displays an activity indicator with an optional message. It provides a standardized way to show loading/busy states across the application and can be used independently or bound to the `IBusyService` for automatic state management.

## Location

**File**: `src/ISynergy.Framework.UI.Maui/Controls/IndicatorControl.cs`

## Architecture Pattern

The `IndicatorControl` follows the **Clean Architecture** and **SOLID** principles established in the framework:

- **Single Responsibility**: Only handles indicator display
- **No XAML**: Pure code-behind implementation
- **Dependency Injection**: Optional constructor injection of `IBusyService`
- **Reusability**: Used in `LoadingView`, `EmptyView`, and at the window level
- **Theme Support**: Uses dynamic resources for colors

## Global Implementation

The `IndicatorControl` is implemented at the **window level** in the `Application.cs` file, providing a single, global busy indicator that overlays all pages and views. This eliminates the need for individual views to implement their own busy indicators.

### Window-Level Integration

```csharp
// In Application.cs - CreateWindow method
protected override Microsoft.Maui.Controls.Window CreateWindow(IActivationState? activationState)
{
    // Main page content
    var mainPage = CreateLoadingView();
    
    // Create window content grid with overlay support
    _windowContentGrid = new Grid();
    _windowContentGrid.Children.Add(mainPage);

    // Create global indicator control overlay
    var overlayBackground = new Grid
    {
        BackgroundColor = Colors.Black,
        Opacity = 0.5
    };

    _globalIndicatorControl = new IndicatorControl(_commonServices.BusyService)
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Center
    };

    var overlayGrid = new Grid();
    overlayGrid.Children.Add(overlayBackground);
    overlayGrid.Children.Add(_globalIndicatorControl);
    overlayGrid.SetBinding(Grid.IsVisibleProperty,
        new Binding(nameof(IBusyService.IsBusy), source: _commonServices.BusyService));

    // Add overlay on top of all content
    _windowContentGrid.Children.Add(overlayGrid);

    var wrapperPage = new ContentPage { Content = _windowContentGrid };
    var window = new Microsoft.Maui.Controls.Window(wrapperPage);
    
    return window;
}
```

## Key Features

### 1. Automatic Binding to BusyService
```csharp
// Constructor with automatic binding
var indicator = new IndicatorControl(busyService);

// Or bind later
var indicator = new IndicatorControl();
indicator.BindToBusyService(busyService);
```

### 2. Manual Control
```csharp
// Create without binding
var indicator = new IndicatorControl
{
    Message = "Loading...",
    IsRunning = true,
    IndicatorColor = Colors.Blue
};
```

### 3. Customization Properties
```csharp
// Text displayed with the indicator
public string Message { get; set; }

// Controls if indicator is running/animating
public bool IsRunning { get; set; }

// Color for both indicator and message
public Color IndicatorColor { get; set; }

// Show/hide the message label
public bool IsMessageVisible { get; set; }

// Vertical or Horizontal layout
public StackOrientation Orientation { get; set; }

// Spacing between message and indicator
public double Spacing { get; set; }
```

## Usage Examples

### Global Window-Level Usage (Recommended)

The indicator is automatically available at the window level. Simply use the `BusyService` from any ViewModel:

```csharp
public class ProductListViewModel : ViewModelNavigation<Product>
{
    public ProductListViewModel(
        ICommonServices commonServices,
        ILogger<ProductListViewModel> logger)
        : base(commonServices, logger)
    {
    }
    
    private async Task LoadDataAsync()
    {
        try
        {
            // This will show the global indicator automatically
            CommonServices.BusyService.StartBusy("Loading products...");
            
            var products = await _productService.GetAllAsync();
            
            // Process products...
        }
        catch (Exception ex)
        {
            await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
        }
        finally
        {
            // This will hide the global indicator
            CommonServices.BusyService.StopBusy();
        }
    }
}
```

### Local Page-Specific Usage

For page-specific indicators that don't block the entire window:

```csharp
public class CustomLoadingPage : ContentPage
{
    private readonly IndicatorControl _indicator;
    
    public CustomLoadingPage(IBusyService busyService)
    {
        _indicator = new IndicatorControl(busyService)
        {
            VerticalOptions = LayoutOptions.End,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(20)
        };
        
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            }
        };
        
        var contentArea = new Label { Text = "Main content" };
        
        grid.Add(contentArea, 0, 0);
        grid.Add(_indicator, 0, 1);
        
        Content = grid;
    }
}
```

### Manual Control Example
```csharp
var indicator = new IndicatorControl
{
    Message = "Please wait...",
    IsRunning = false,
    Orientation = StackOrientation.Horizontal,
    Spacing = 15
};

public void StartLoading(string message)
{
    indicator.Message = message;
    indicator.IsRunning = true;
}

public void StopLoading()
{
    indicator.IsRunning = false;
}
```

## Component Structure

### Visual Hierarchy
```
ContentView (IndicatorControl)
??? StackLayout
    ??? Label (Message)
    ??? ActivityIndicator
```

### Data Bindings (when bound to BusyService)
- **Label.Text**: `IBusyService.BusyMessage` (OneWay)
- **Label.IsVisible**: `IBusyService.IsBusy` (OneWay)
- **ActivityIndicator.IsRunning**: `IBusyService.IsBusy` (OneWay)

### Dynamic Resources
- **Label.TextColor**: `Primary` theme color (default)
- **ActivityIndicator.Color**: `Primary` theme color (default)

## Integration with Existing Controls

### Global Window Overlay
```csharp
// In Application.cs
_globalIndicatorControl = new IndicatorControl(_commonServices.BusyService);
// Automatically shows/hides based on BusyService state
```

### LoadingView
```csharp
_indicatorControl = new IndicatorControl(commonServices.BusyService);
```

### EmptyView
```csharp
var indicatorControl = new IndicatorControl(commonServices.BusyService);
this.Content = indicatorControl;
```

### View Base Class
No longer includes busy overlay - uses global window-level indicator instead.

## Benefits

? **Global Coverage**: Single indicator at window level covers all pages
? **No Duplication**: Eliminates need for per-view indicators
? **Consistent UX**: Same indicator appearance across entire application
? **Simplified Views**: Views no longer need to manage their own busy state UI
? **Flexible**: Can still use local indicators for specific scenarios
? **Theme-Aware**: Uses dynamic resources for colors
? **Lightweight**: Minimal overhead
? **MVVM-Friendly**: Controlled entirely through BusyService
? **Testable**: Can be instantiated without dependencies

## Architecture Benefits

### Before: Per-View Indicators
```
Window
??? Page 1 (with busy overlay)
?   ??? Content
?   ??? Busy Overlay + Indicator
??? Page 2 (with busy overlay)
?   ??? Content
?   ??? Busy Overlay + Indicator
??? Page 3 (with busy overlay)
    ??? Content
    ??? Busy Overlay + Indicator
```

### After: Global Window-Level Indicator
```
Window
??? Global Busy Overlay + IndicatorControl (always on top)
??? Content Grid
    ??? Page 1 (clean content only)
    ??? Page 2 (clean content only)
    ??? Page 3 (clean content only)
```

## Advanced Scenarios

### Horizontal Layout
```csharp
var indicator = new IndicatorControl(busyService)
{
    Orientation = StackOrientation.Horizontal,
    Spacing = 10
};
```

### Hide Message, Show Only Spinner
```csharp
var indicator = new IndicatorControl(busyService)
{
    IsMessageVisible = false
};
```

### Custom Styling
```csharp
var indicator = new IndicatorControl(busyService)
{
    HorizontalOptions = LayoutOptions.Center,
    VerticalOptions = LayoutOptions.End,
    Margin = new Thickness(20)
};
```

## Related Components

- **Application**: Hosts the global IndicatorControl at window level
- **LoadingView**: Uses IndicatorControl for splash screen busy state
- **EmptyView**: Uses IndicatorControl for initial loading state
- **IBusyService**: Provides busy state and message data
- **ICommonServices**: Access point to BusyService

## Design Principles Applied

- **Single Responsibility**: Only handles indicator display
- **Open/Closed**: Open for extension (inherit), closed for modification
- **Dependency Inversion**: Depends on IBusyService abstraction
- **Composition over Inheritance**: Composed of Label and ActivityIndicator
- **DRY**: Single indicator implementation reused across application
- **Separation of Concerns**: Views focus on content, window handles busy state

## Migration Guide

### From Per-View Indicators

If you previously had indicators in individual views:

```csharp
// Old Pattern - In each view
var overlayGrid = new Grid();
var overlayBackground = new Grid { BackgroundColor = Colors.Black, Opacity = 0.5 };
var busyIndicator = new ActivityIndicator();
busyIndicator.SetBinding(ActivityIndicator.IsRunningProperty, ...);
// ... 10+ more lines

// New Pattern - Nothing needed in views!
// Just use BusyService in ViewModels:
CommonServices.BusyService.StartBusy("Loading...");
// Global indicator shows automatically
CommonServices.BusyService.StopBusy();
// Global indicator hides automatically

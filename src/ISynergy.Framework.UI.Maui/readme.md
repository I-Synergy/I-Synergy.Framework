# I-Synergy Framework UI MAUI

Cross-platform .NET MAUI UI framework for building modern applications on Windows, Android, iOS, and macOS. This package provides a complete MAUI implementation of the I-Synergy Framework UI services, controls, and patterns.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.UI.Maui.svg)](https://www.nuget.org/packages/I-Synergy.Framework.UI.Maui/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Platforms](https://img.shields.io/badge/platforms-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20macOS-blue.svg)](https://dotnet.microsoft.com/apps/maui)

## Features

- **Cross-platform support** for Android, iOS, Windows, and macOS
- **Dialog service** with robust error handling and fallback mechanisms
- **Navigation service** with page and modal navigation
- **Theme service** with dynamic color palettes and light/dark mode
- **File service** with platform-specific pickers
- **Clipboard service** for text and data operations
- **Camera service** for photo capture and gallery access
- **Custom controls** (NavigationMenu, ImageBrowser, ErrorPresenter, TabbedView, BladePane)
- **Value converters** for all common data types
- **Blade panes** with adaptive presentation — an overlay pane on tablet/desktop, a modal page on phone
- **Behaviors** (SelectAllOnFocusBehavior)
- **Platform extensions** for Windows-specific features
- **Dynamic theme system** with 40+ built-in color palettes
- **Token storage** with secure platform-specific implementations

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.UI.Maui
```

## Quick Start

### 1. Configure MauiProgram

Setup your MAUI application with I-Synergy Framework services:

```csharp
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureLogging((appBuilder, logging) =>
            {
#if DEBUG
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Trace);
#endif
            })
            .ConfigureServices<App, AppContext, CommonServices, SettingsService, Resources>(
                appBuilder =>
                {
                    // Register additional services here
                    appBuilder.Services.AddScoped<IProductService, ProductService>();
                    appBuilder.Services.AddTransient<MainViewModel>();
                },
                Assembly.GetExecutingAssembly(),
                assemblyName => assemblyName.Name.StartsWith("MyApp")
            );

        return builder.Build();
    }
}
```

### 2. Create XAML Views with ViewModels

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ui:View xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
         xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
         xmlns:ui="clr-namespace:ISynergy.Framework.UI.Controls;assembly=ISynergy.Framework.UI.Maui"
         xmlns:vm="clr-namespace:MyApp.ViewModels"
         x:Class="MyApp.Views.ProductListView"
         x:DataType="vm:ProductListViewModel"
         Title="{Binding Title}">

    <Grid RowDefinitions="Auto,*">
        <!-- Toolbar -->
        <HorizontalStackLayout Grid.Row="0" Padding="10">
            <Button Text="Add Product"
                    Command="{Binding AddProductCommand}" />
            <Button Text="Refresh"
                    Command="{Binding RefreshCommand}" />
        </HorizontalStackLayout>

        <!-- Product List -->
        <CollectionView Grid.Row="1"
                        ItemsSource="{Binding Products}"
                        SelectionMode="Single"
                        SelectedItem="{Binding SelectedProduct}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Grid Padding="10">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <Label Text="{Binding Name}"
                               FontSize="16"
                               FontAttributes="Bold" />
                        <Label Grid.Column="1"
                               Text="{Binding Price, StringFormat='{0:C}'}"
                               VerticalOptions="Center" />
                    </Grid>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </Grid>
</ui:View>
```

```csharp
using ISynergy.Framework.Mvvm.ViewModels;

public partial class ProductListView : View
{
    public ProductListView()
    {
        InitializeComponent();
    }
}
```

### 3. Use Dialog Service

The MAUI dialog service provides comprehensive error handling and fallback mechanisms:

```csharp
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Mvvm.Commands;

public class ProductListViewModel : ViewModelNavigation<Product>
{
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();

    public AsyncRelayCommand AddProductCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand<Product> DeleteProductCommand { get; }

    public ProductListViewModel(
        ICommonServices commonServices,
        IProductService productService,
        ILogger<ProductListViewModel> logger)
        : base(commonServices, logger)
    {
        _productService = productService;

        Title = "Products";

        AddProductCommand = new AsyncRelayCommand(AddProductAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        DeleteProductCommand = new AsyncRelayCommand<Product>(DeleteProductAsync);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await RefreshAsync();
        IsInitialized = true;
    }

    private async Task AddProductAsync()
    {
        await CommonServices.DialogService
            .ShowDialogAsync<ProductEditWindow, ProductEditViewModel, Product>();

        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        try
        {
            CommonServices.BusyService.StartBusy("Loading products...");

            var products = await _productService.GetAllAsync();

            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }
        catch (Exception ex)
        {
            await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
        }
        finally
        {
            CommonServices.BusyService.StopBusy();
        }
    }

    private async Task DeleteProductAsync(Product product)
    {
        var result = await CommonServices.DialogService.ShowMessageAsync(
            $"Are you sure you want to delete {product.Name}?",
            "Confirm Delete",
            MessageBoxButtons.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _productService.DeleteAsync(product.Id);
                Products.Remove(product);

                await CommonServices.DialogService.ShowInformationAsync(
                    "Product deleted successfully",
                    "Success");
            }
            catch (Exception ex)
            {
                await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
            }
        }
    }
}
```

### 4. Navigation Service

Navigate between pages using the navigation service:

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;

public class MainViewModel : ViewModel
{
    private readonly INavigationService _navigationService;

    public AsyncRelayCommand NavigateToProductsCommand { get; }
    public AsyncRelayCommand NavigateToSettingsCommand { get; }

    public MainViewModel(
        ICommonServices commonServices,
        INavigationService navigationService,
        ILogger<MainViewModel> logger)
        : base(commonServices, logger)
    {
        _navigationService = navigationService;

        NavigateToProductsCommand = new AsyncRelayCommand(NavigateToProductsAsync);
        NavigateToSettingsCommand = new AsyncRelayCommand(NavigateToSettingsAsync);
    }

    private async Task NavigateToProductsAsync()
    {
        await _navigationService.NavigateAsync<ProductListViewModel>();
    }

    private async Task NavigateToSettingsAsync()
    {
        await _navigationService.NavigateAsync<SettingsViewModel>();
    }

    // Navigate with parameters
    private async Task NavigateToProductDetailAsync(Product product)
    {
        await _navigationService.NavigateAsync<ProductDetailViewModel>(product);
    }

    // Modal navigation
    private async Task ShowModalAsync()
    {
        await _navigationService.NavigateModalAsync<ModalViewModel>();
    }
}
```

### 5. Theme Service

Apply dynamic themes with custom accent colors:

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;

// Theme service is automatically configured during startup
// It reads theme preferences from ISettingsService

// Light/dark switching is built into the shell: ThemeCommand toggles through IThemeService.
// Accent color selection belongs in your own settings view.

// In your settings view model:
public void ApplyAccent(string color) => _themeService.SetAccentColor(color);

```

The theme service supports:
- Light and dark modes
- 40+ built-in accent colors
- Dynamic theme switching
- Platform-specific customization (Windows title bar, Android status bar)

### 6. Use Custom Controls

#### NavigationMenu

```xml
<ui:NavigationMenu ItemsSource="{Binding MenuItems}"
                   SelectedItem="{Binding SelectedMenuItem}"
                   IsExpanded="{Binding IsMenuExpanded}" />
```

#### ImageBrowser

```xml
<controls:ImageBrowser Images="{Binding ProductImages}"
                       SelectedImage="{Binding SelectedImage}"
                       AllowAdd="True"
                       AllowRemove="True" />
```

#### ErrorPresenter

```xml
<controls:ErrorPresenter Errors="{Binding ValidationErrors}"
                         IsVisible="{Binding HasErrors}" />
```

#### TabbedView

```xml
<ui:TabbedView>
    <ui:TabbedView.Items>
        <TabViewItem Title="General">
            <local:GeneralSettingsView />
        </TabViewItem>
        <TabViewItem Title="Security">
            <local:SecuritySettingsView />
        </TabViewItem>
    </ui:TabbedView.Items>
</ui:TabbedView>
```

#### BladePane

```xml
<c:BladePane ItemsSource="{Binding Blades}"
             Anchor="Auto" />
```

See [Blades (edge panes)](#blades-edge-panes) for the hosting and RTL rules.

### 7. Live Localization with Markup Extension

Use the `m:LocalizedStrings` markup extension for trim-friendly, compiled localized text bindings:

```xml
xmlns:m="clr-namespace:ISynergy.Framework.UI.Markup;assembly=ISynergy.Framework.UI.Maui"

<!-- Simple usage with content property -->
<Label Text="{m:LocalizedStrings WelcomeMessage}" />

<!-- Explicit Key property -->
<Label Text="{m:LocalizedStrings Key=GreetingText}" />

<!-- Updates automatically when language changes -->
```

When you call `CultureInfo.ApplyCulture()` from Globalization library, all bound labels automatically refresh:

```csharp
// Change language at runtime
var newCulture = new CultureInfo("de-DE");
newCulture.ApplyCulture();  // Triggers UI refresh for all LocalizedStrings bindings
```

The extension uses OneWay binding to `LocalizedStrings.Default` indexer with property-change notification, enabling automatic translation updates without page reload.

## Blades (edge panes)

A *blade* is a card that slides in from a screen edge and stacks on top of the previous one, matching the WinUI `BladeView` interaction.
The owning view model implements `IViewModelBladeView` and exposes the stack through `Blades` (`ObservableCollection<IView>`) plus `IsPaneVisible`.
Blades are opened through `IBladeService.OpenBladeAsync<TBladeViewModel, TEntity>(owner, ...)`, which resolves the view and view model from a DI scope, appends the view to `owner.Blades` and disables every earlier blade; closing the newest blade removes it and re-enables the previous one.
Derive the owner from `ViewModelBladeView<TModel>` to get `Blades` and the blade commands for free.

A page that wants an overlay pane declares the control in its own root layout (a `Shell` cannot host an overlay itself):

```xml
<c:BladePane ItemsSource="{Binding Blades}"
             Anchor="End" />
```

`BladePane` properties:

- `ItemsSource` — the blade views to host, in opening order. When `null`, the `Blades` of the `IViewModelBladeView` binding context is used.
- `Anchor` — the logical edge, `Auto` (default, resolves to `End`), `Start` or `End`.
- `DisabledOpacity` — opacity of every blade except the newest one (`0.75` by default).
- `BladeSpacing` — gap between two adjacent blades (`10` by default).
- `BladeMaxWidth` — maximum width of a single blade card (`520` by default).
- `InnerPadding` — padding between the blade stack and the host edges (`8` on every side by default).

The pane registers itself in `IBladePaneHost` (`BladePaneHostRegistry`, singleton, weak references) while it is attached to the visual tree and unregisters when it leaves or is disposed.

### Shell-level pane

`Controls/BladeHostView` is a self-contained host: it resolves the framework's `BladeHostViewModel` (`ISynergy.Framework.UI/ViewModels`, singleton) lazily — in the constructor, again on `Loaded` and on `OnParentSet`, because the platform service provider may not exist yet during startup — and then attaches a `BladePane` with `ItemsSource = host.Blades`.
Declare it as the **last child** of the page's root layout so the pane overlays the page content.
When the view model cannot be resolved the control stays hidden and traces the failure instead of throwing.

Use `BladeHostView` when the blades are shell-level, and a page-level `BladePane` when the blades belong to that page.
The sample's Settings is a blade now: `OpenSettingsAsync` (and first run) opens `SettingsViewModel` through `IBladeService` against the shared `BladeHostViewModel`, and `BladeHostView` is declared as the last child of `InfoView`, `CustomControlsView`, `MauiControlsView` and `SlideShowView`; the old per-page settings drawer (`SettingsDrawer` and `SettingsDrawerState`) is gone.
On `DeviceInfo.Idiom == Phone` the blade still presents as a modal page, per the adaptive behaviour below.

### Adaptive presentation

`BladeService` is adaptive and decides per blade at open time:

- **Tablet and desktop** — when a `BladePane` is attached (`IBladePaneHost.IsHostAttached`) and `DeviceInfo.Idiom` is not `Phone`, the blade is added to the owner and rendered by the pane.
- **Phone** — the blade view is pushed with `PushModalAsync` and popped on the blade's `Closed` event, but only when that page is still on top of the modal stack, so closing an older blade never pops a newer page.
- **Fallback** — when neither path is possible (the view is not a `Page`, there is no root page, the push fails, or no pane is attached) the blade falls back to the overlay pane and a warning is logged; a blade is never dropped silently.

### Hosting contract

A blade view is a `ContentPage`. A page cannot be a child of a layout, so the pane detaches `ContentPage.Content` and reparents it into a `ContentView` (keeping the page `BindingContext`), then restores it when the blade is released.
The consequence: **page-level resources, styles and any behaviour that depends on the page being in the visual tree are not available to a hosted blade** — put shared resources in `Application.Resources` or a merged dictionary instead.

### Right-to-left

`Anchor` is *logical*: `Start` is the left edge in a left-to-right culture and the right edge in a right-to-left one, and `Auto` follows `End`.
The edge is resolved once through `BladePlacement.IsStartSide`/`BladePlacement.HorizontalSign`, never hardcoded, and re-resolved on `FlowDirectionChanged`.
The positioning container is forced to `LeftToRight` so `Start`/`End` stay physical; each blade's host receives the pane's `FlowDirection`, so the blade content mirrors while the pane stays anchored where `Anchor` says; the slide-in offset uses `HorizontalSign`, because render transforms are never mirrored.
The page inherits `FlowDirection` from the window (`LocalSettings.Language`), so no per-page setup is required.

## Platform-Specific Features

### Windows Integration

The MAUI framework provides Windows-specific extensions:

```csharp
// In Platforms/Windows folder
using ISynergy.Framework.UI.Platforms.Windows.Extensions;

// Customize app window
var window = GetAppWindow();
window.SetIcon("Assets/icon.ico");
window.SetTitle("My Application");

// Apply theme colors to title bar
_themeService.ApplyTheme(); // Automatically updates title bar
```

### Android Integration

Theme colors are automatically applied to Android status bar and action bar.

### iOS/macOS Integration

Native navigation bar and status bar colors are synchronized with theme.

## Value Converters

The MAUI framework includes 20+ value converters:

```xml
<!-- Boolean converters -->
<Label IsVisible="{Binding IsActive, Converter={StaticResource BoolToVisibilityConverter}}" />
<Label Text="{Binding IsEnabled, Converter={StaticResource BoolToYesNoConverter}}" />

<!-- Numeric converters -->
<Label Text="{Binding Amount, Converter={StaticResource DecimalToCurrencyConverter}}" />
<Label Text="{Binding Quantity, Converter={StaticResource IntegerToStringConverter}}" />

<!-- Date/Time converters -->
<Label Text="{Binding CreatedDate, Converter={StaticResource DateTimeToStringConverter}}" />
<Label Text="{Binding UpdatedDate, Converter={StaticResource DateTimeOffsetToLocalStringConverter}}" />

<!-- String converters -->
<Label IsVisible="{Binding Name, Converter={StaticResource StringNullOrEmptyToBoolConverter}}" />
<Label Text="{Binding Description, Converter={StaticResource StringToUpperConverter}}" />

<!-- Collection converters -->
<Label IsVisible="{Binding Items, Converter={StaticResource CollectionNullOrEmptyToBoolConverter}}" />
<Label Text="{Binding Items, Converter={StaticResource CollectionCountConverter}}" />

<!-- Color converters -->
<BoxView Color="{Binding HexColor, Converter={StaticResource StringToColorConverter}}" />

<!-- Enum converters -->
<Picker ItemsSource="{Binding Source={StaticResource OrderStatusEnum}}"
        SelectedItem="{Binding Status, Converter={StaticResource EnumToStringConverter}}" />

<!-- Guid converters -->
<Label Text="{Binding Id, Converter={StaticResource GuidToStringConverter}}" />
```

## File Operations

### File Service

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Models.Results;

public class DocumentViewModel : ViewModel
{
    private readonly IFileService<FileResult> _fileService;

    public DocumentViewModel(
        ICommonServices commonServices,
        IFileService<FileResult> fileService,
        ILogger<DocumentViewModel> logger)
        : base(commonServices, logger)
    {
        _fileService = fileService;

        OpenFileCommand = new AsyncRelayCommand(OpenFileAsync);
        SaveFileCommand = new AsyncRelayCommand(SaveFileAsync);
    }

    public AsyncRelayCommand OpenFileCommand { get; }
    public AsyncRelayCommand SaveFileCommand { get; }

    private async Task OpenFileAsync()
    {
        var file = await _fileService.BrowseFileAsync(
            new[] { ".pdf", ".docx", ".txt" });

        if (file is not null)
        {
            // Read file content
            using var stream = await file.OpenReadAsync();
            // Process file...
        }
    }

    private async Task SaveFileAsync()
    {
        var file = await _fileService.SaveFileAsync(
            "document.pdf",
            "Documents",
            new[] { ".pdf" });

        if (file is not null)
        {
            // Write file content
            using var stream = await file.OpenWriteAsync();
            // Save content...
        }
    }
}
```

### Camera Service

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;

public class ProfileViewModel : ViewModel
{
    private readonly ICameraService _cameraService;

    public AsyncRelayCommand TakePhotoCommand { get; }
    public AsyncRelayCommand PickPhotoCommand { get; }

    public ImageSource ProfileImage
    {
        get => GetValue<ImageSource>();
        set => SetValue(value);
    }

    public ProfileViewModel(
        ICommonServices commonServices,
        ICameraService cameraService,
        ILogger<ProfileViewModel> logger)
        : base(commonServices, logger)
    {
        _cameraService = cameraService;

        TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
        PickPhotoCommand = new AsyncRelayCommand(PickPhotoAsync);
    }

    private async Task TakePhotoAsync()
    {
        var photo = await _cameraService.TakePhotoAsync();

        if (photo is not null)
        {
            ProfileImage = ImageSource.FromStream(() => photo.OpenReadAsync().Result);
        }
    }

    private async Task PickPhotoAsync()
    {
        var photo = await _cameraService.PickPhotoAsync();

        if (photo is not null)
        {
            ProfileImage = ImageSource.FromStream(() => photo.OpenReadAsync().Result);
        }
    }
}
```

## Behaviors

### SelectAllOnFocusBehavior

```xml
<Entry Text="{Binding SearchText}">
    <Entry.Behaviors>
        <behaviors:SelectAllOnFocusBehavior />
    </Entry.Behaviors>
</Entry>
```

## Configuration

### Application Features

Configure optional application features:

```json
{
  "ApplicationFeatures": {
    "EnableAnalytics": true,
    "EnableCrashReporting": true,
    "EnableNotifications": true
  },
  "ClientApplicationOptions": {
    "ApplicationName": "My Application",
    "BaseUrl": "https://api.myapp.com",
    "ApiVersion": "v1"
  }
}
```

### Font Configuration

The framework automatically configures multiple fonts:

- Segoe UI (Regular, Bold, SemiBold, Light, SemiLight)
- Open Sans (Regular, Medium, SemiBold)
- OpenDyslexic (Regular, Bold)
- Segoe MDL2 Assets (Icon font)

## Best Practices

> [!TIP]
> Use **NavigateAsync** for standard navigation and **NavigateModalAsync** for dialogs and overlays.

> [!IMPORTANT]
> Always dispose ViewModels and unsubscribe from events in the **Cleanup** method to prevent memory leaks.

> [!NOTE]
> The dialog service implements circuit breaker pattern with automatic fallback to console/debug output if UI is not available.

### Dialog Service Usage

- Always use dialog service instead of platform-specific alerts
- Dialog service includes comprehensive error handling
- Supports graceful degradation with multiple fallback mechanisms
- Prevents concurrent dialogs with semaphore locking
- Includes timeout and retry logic

### Navigation Patterns

- Use `NavigateAsync<TViewModel>()` for page navigation
- Use `NavigateModalAsync<TViewModel>()` for modals
- Pass parameters through navigation methods
- Call `InitializeAsync()` after navigation completes
- Clean up resources in `OnNavigatedFrom()`

### Theme Management

- Theme is automatically applied on startup
- Theme changes are persisted in settings
- Platform-specific colors are synchronized
- Support both light and dark modes
- Use ThemeWindow for user selection

### Performance Tips

- Use CollectionView instead of ListView for better performance
- Implement virtualization for large lists
- Dispose images and streams properly
- Use async/await for all I/O operations
- Leverage compiled bindings with x:DataType

## Dependencies

- **I-Synergy.Framework.UI** - Base UI abstractions
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **Microsoft.Maui.Controls** - MAUI controls
- **Microsoft.Extensions.DependencyInjection** - DI container
- **Microsoft.Extensions.Configuration.Json** - Configuration
- **SkiaSharp** - 2D graphics

## Platform Requirements

- **Android**: API 35 (Android 14) or higher
- **iOS**: iOS 16.0 or higher
- **macOS**: macOS 10.15 (Catalina) or higher
- **Windows**: Windows 11 24H2 (10.0.26100.0) or higher

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)
- [MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)

## Related Packages

- **I-Synergy.Framework.UI** - Base UI abstractions
- **I-Synergy.Framework.Core** - Core framework
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **I-Synergy.Framework.UI.WPF** - WPF implementation
- **I-Synergy.Framework.UI.WinUI** - WinUI implementation
- **I-Synergy.Framework.UI.Blazor** - Blazor implementation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

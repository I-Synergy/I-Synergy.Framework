# I-Synergy Framework UI WinUI

Modern Windows UI framework for building next-generation Windows applications with WinUI 3. This package provides a complete WinUI implementation of the I-Synergy Framework UI services, controls, and patterns for Windows 10+ and Windows 11.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.UI.WinUI.svg)](https://www.nuget.org/packages/I-Synergy.Framework.UI.WinUI/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2B-blue.svg)](https://docs.microsoft.com/windows/apps/winui/)

## Features

- **WinUI 3 support** for Windows 10 (19041+) and Windows 11
- **Modern Fluent Design** with Mica, Acrylic, and Material effects
- **Dialog service** with ContentDialog and MessageDialog support
- **Navigation service** with Frame-based navigation
- **Theme service** with dynamic accent colors and Fluent theming
- **File service** with modern file pickers
- **Clipboard service** for Windows clipboard operations
- **Camera service** for camera and photo capture
- **Update service** for application updates
- **Custom controls** (BladeView, Console, ImageBrowser, Menu, Tiles, ErrorPresenter, SplashScreen)
- **Behaviors** using CommunityToolkit.WinUI.Behaviors
- **40+ dynamic theme palettes** with modern Fluent Design styles
- **Native Windows 11 integration** with title bar customization

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.UI.WinUI
```

## Quick Start

### 1. Configure Application

Setup your WinUI 3 application with I-Synergy Framework:

```csharp
using ISynergy.Framework.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

public partial class App : Application
{
    private IHost _host;
    private Microsoft.UI.Xaml.Window _window;

    public App()
    {
        InitializeComponent();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Core services
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<IMessengerService, MessengerService>();
                services.AddSingleton<IBusyService, BusyService>();

                // WinUI services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IFileService<FileResult>, FileService>();
                services.AddSingleton<IClipboardService, ClipboardService>();
                services.AddSingleton<ICameraService, CameraService>();
                services.AddSingleton<IUpdateService, UpdateService>();

                // ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<ShellViewModel>();
            })
            .Build();

        await _host.StartAsync();

        // Apply theme
        var themeService = _host.Services.GetRequiredService<IThemeService>();
        themeService.ApplyTheme();

        // Create and activate window
        _window = _host.Services.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
```

### 2. Create XAML Windows with ViewModels

```xml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Navigation Bar -->
        <Grid Grid.Row="0" Background="{ThemeResource SystemControlAcrylicElementBrush}">
            <CommandBar>
                <AppBarButton Icon="Add" Label="New" Command="{Binding NewCommand}"/>
                <AppBarButton Icon="OpenFile" Label="Open" Command="{Binding OpenCommand}"/>
                <AppBarButton Icon="Save" Label="Save" Command="{Binding SaveCommand}"/>
                <AppBarSeparator/>
                <AppBarButton Icon="Setting" Label="Settings" Command="{Binding SettingsCommand}"/>
            </CommandBar>
        </Grid>

        <!-- Content -->
        <Frame Grid.Row="1"
               x:Name="MainFrame"/>

        <!-- Status Bar -->
        <Grid Grid.Row="2" Background="{ThemeResource SystemControlAcrylicElementBrush}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>

            <TextBlock Grid.Column="0"
                       Text="{Binding StatusMessage}"
                       Margin="12,8"
                       VerticalAlignment="Center"/>

            <ProgressRing Grid.Column="1"
                          Width="20" Height="20"
                          Margin="12,8"
                          IsActive="{Binding BusyService.IsBusy}"/>
        </Grid>
    </Grid>
</Window>
```

```csharp
using Microsoft.UI.Xaml;

public sealed partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        DataContext = viewModel;
    }
}
```

### 3. Use Dialog Service

```csharp
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Mvvm.Commands;

public class ProductViewModel : ViewModel
{
    private readonly IProductService _productService;

    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public ProductViewModel(
        ICommonServices commonServices,
        IProductService productService,
        ILogger<ProductViewModel> logger)
        : base(commonServices, logger)
    {
        _productService = productService;

        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private bool CanSave() => !string.IsNullOrEmpty(Name) && IsValid;

    private async Task SaveAsync()
    {
        try
        {
            CommonServices.BusyService.StartBusy("Saving product...");

            await _productService.SaveAsync(Product);

            await CommonServices.DialogService.ShowInformationAsync(
                "Product saved successfully",
                "Success");
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

    private async Task DeleteAsync()
    {
        var result = await CommonServices.DialogService.ShowMessageAsync(
            "Are you sure you want to delete this product?",
            "Confirm Delete",
            MessageBoxButtons.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            await _productService.DeleteAsync(Product.Id);
            await CommonServices.NavigationService.GoBackAsync();
        }
    }

    // Show custom ContentDialog
    private async Task EditSettingsAsync()
    {
        await CommonServices.DialogService
            .ShowDialogAsync<SettingsDialog, SettingsViewModel, Settings>();
    }
}
```

### 4. Camera Service

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;

public class ProfileViewModel : ViewModel
{
    private readonly ICameraService _cameraService;

    public AsyncRelayCommand TakePhotoCommand { get; }
    public AsyncRelayCommand PickPhotoCommand { get; }

    public BitmapImage ProfileImage
    {
        get => GetValue<BitmapImage>();
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
            var bitmapImage = new BitmapImage();
            using (var stream = await photo.OpenReadAsync())
            {
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
            }
            ProfileImage = bitmapImage;
        }
    }

    private async Task PickPhotoAsync()
    {
        var photo = await _cameraService.PickPhotoAsync();

        if (photo is not null)
        {
            var bitmapImage = new BitmapImage();
            using (var stream = await photo.OpenReadAsync())
            {
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
            }
            ProfileImage = bitmapImage;
        }
    }
}
```

## Custom Controls

### BladeView

```xml
<controls:BladeView ItemsSource="{x:Bind ViewModel.Blades, Mode=OneWay}"
                    SelectedItem="{x:Bind ViewModel.SelectedBlade, Mode=TwoWay}" />
```

### ImageBrowser

```xml
<controls:ImageBrowser Images="{x:Bind ViewModel.ProductImages, Mode=OneWay}"
                       SelectedImage="{x:Bind ViewModel.SelectedImage, Mode=TwoWay}"
                       AllowAdd="True"
                       AllowRemove="True" />
```

### SplashScreen

```xml
<controls:SplashScreen x:Class="MyApp.CustomSplashScreen"
                       Logo="/Assets/logo.png"
                       Message="Loading application..."
                       ShowProgress="True" />
```

## Fluent Design Integration

WinUI 3 provides modern Fluent Design features:

```xml
<!-- Mica Material -->
<Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <!-- Mica is automatically applied to window background -->
</Grid>

<!-- Acrylic Brush -->
<Grid Background="{ThemeResource SystemControlAcrylicWindowBrush}">
    <!-- Acrylic transparent effect -->
</Grid>

<!-- Reveal Highlight -->
<Button Style="{StaticResource AccentButtonStyle}"
        Content="Click Me">
    <!-- Automatic reveal highlight on hover -->
</Button>

<!-- Shadow -->
<Border CornerRadius="8"
        Shadow="{StaticResource SharedShadow}">
    <Image Source="/Assets/image.png"/>
</Border>
```

## Theme Support

The WinUI framework includes 40+ built-in theme palettes with modern Fluent Design:

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;

// Theme is automatically applied on startup
// Accent color is synchronized with Windows system accent

// Show theme selection to users
private async Task ShowThemeSelectionAsync()
{
    await CommonServices.DialogService
        .ShowDialogAsync<ThemeWindow, ThemeViewModel, ThemeStyle>();
}
```

## Title Bar Customization

Customize the Windows 11 title bar:

```csharp
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Extend content into title bar
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Customize title bar colors
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var titleBar = AppWindow.TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
        }
    }
}
```

## Behaviors

Using CommunityToolkit.WinUI.Behaviors:

```xml
<Page xmlns:interactivity="using:Microsoft.Xaml.Interactivity"
      xmlns:behaviors="using:CommunityToolkit.WinUI.Behaviors">

    <TextBox>
        <interactivity:Interaction.Behaviors>
            <behaviors:FocusBehavior />
        </interactivity:Interaction.Behaviors>
    </TextBox>

    <ListView ItemsSource="{x:Bind Items}">
        <interactivity:Interaction.Behaviors>
            <behaviors:SelectAllBehavior />
        </interactivity:Interaction.Behaviors>
    </ListView>
</Page>
```

## Best Practices

> [!TIP]
> Use **x:Bind** instead of Binding for better performance in WinUI 3.

> [!IMPORTANT]
> Always test on both Windows 10 and Windows 11 for compatibility.

> [!NOTE]
> WinUI 3 apps use a separate process from Windows Shell, providing better isolation and stability.

### Performance

- Use x:Bind for compiled bindings
- Implement virtualization for large lists
- Use Mica/Acrylic sparingly for performance
- Dispose resources properly
- Leverage hardware acceleration

### Windows 11 Features

- Use Mica material for window backgrounds
- Customize title bar with Windows 11 styles
- Implement snap layouts support
- Use rounded corners for modern look
- Leverage system accent color

## Dependencies

- **I-Synergy.Framework.UI** - Base UI abstractions
- **Microsoft.WindowsAppSDK** - Windows App SDK
- **CommunityToolkit.WinUI.Behaviors** - WinUI behaviors
- **Microsoft.Extensions.Configuration.Json** - Configuration
- **Microsoft.Extensions.Hosting** - Application hosting

## Platform Requirements

- **Target Framework**: net10.0-windows10.0.26100.0
- **Minimum Version**: Windows 10.0.19041.0 (Version 2004)
- **Recommended**: Windows 11 for best experience
- **Architecture**: x86, x64, ARM64

## Documentation

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [WinUI 3 Documentation](https://docs.microsoft.com/windows/apps/winui/)
- [Windows App SDK](https://docs.microsoft.com/windows/apps/windows-app-sdk/)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.UI** - Base UI abstractions
- **I-Synergy.Framework.Core** - Core framework
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **I-Synergy.Framework.UI.WPF** - WPF implementation
- **I-Synergy.Framework.UI.Maui** - MAUI implementation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

# I-Synergy Framework UI UWP

Universal Windows Platform (UWP) UI framework for building Windows 10 applications. This package provides a complete UWP implementation of the I-Synergy Framework UI services, controls, and patterns for Windows 10 desktop, mobile, Xbox, and IoT.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.UI.UWP.svg)](https://www.nuget.org/packages/I-Synergy.Framework.UI.UWP/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%2010-blue.svg)](https://docs.microsoft.com/windows/uwp/)

> [!WARNING]
> **Breaking Change (v2.0.0+)**: Minimum Windows version raised to 10.0.19041.0 (Version 2004).  
> See [Platform Requirements](#platform-requirements) and [Migration Guide](#migration-guide) below.

## Features

- **UWP support** for Windows 10 (19041+) devices
- **Dialog service** with ContentDialog and MessageDialog support
- **Navigation service** with Frame-based navigation
- **Theme service** with dynamic accent colors
- **File service** with UWP file pickers
- **Clipboard service** for Windows clipboard operations
- **Custom controls** with UWP-specific implementations
- **Behaviors** using Microsoft.Xaml.Behaviors.Uwp.Managed
- **Microsoft UI Xaml (WinUI 2)** control support
- **Adaptive UI** for different device types and screen sizes

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.UI.UWP
```

## Quick Start

### 1. Configure Application

```csharp
using ISynergy.Framework.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

sealed partial class App : Application
{
    private IHost _host;

    public App()
    {
        InitializeComponent();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Core services
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<IMessengerService, MessengerService>();
                services.AddSingleton<IBusyService, BusyService>();

                // UWP services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IFileService<FileResult>, FileService>();
                services.AddSingleton<IClipboardService, ClipboardService>();

                // ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<ShellViewModel>();
            })
            .Build();

        await _host.StartAsync();

        // Apply theme
        var themeService = _host.Services.GetRequiredService<IThemeService>();
        themeService.ApplyTheme();

        // Create root frame
        var rootFrame = Window.Current.Content as Frame;

        if (rootFrame == null)
        {
            rootFrame = new Frame();
            Window.Current.Content = rootFrame;
        }

        if (e.PrelaunchActivated == false)
        {
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            Window.Current.Activate();
        }
    }
}
```

### 2. Create XAML Pages with ViewModels

```xml
<Page x:Class="MyApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
      mc:Ignorable="d">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Command Bar -->
        <CommandBar Grid.Row="0">
            <AppBarButton Icon="Add" Label="New" Command="{x:Bind ViewModel.NewCommand}"/>
            <AppBarButton Icon="Save" Label="Save" Command="{x:Bind ViewModel.SaveCommand}"/>
            <AppBarSeparator/>
            <AppBarButton Icon="Setting" Label="Settings" Command="{x:Bind ViewModel.SettingsCommand}"/>
        </CommandBar>

        <!-- Content -->
        <Frame Grid.Row="1" x:Name="ContentFrame"/>

        <!-- Progress -->
        <muxc:ProgressBar Grid.Row="2"
                          IsIndeterminate="{x:Bind ViewModel.BusyService.IsBusy, Mode=OneWay}"/>
    </Grid>
</Page>
```

### 3. Use Dialog Service

```csharp
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Mvvm.Commands;

public class ProductViewModel : ViewModel
{
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public ProductViewModel(
        ICommonServices commonServices,
        IProductService productService,
        ILogger<ProductViewModel> logger)
        : base(commonServices, logger)
    {
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task SaveAsync()
    {
        try
        {
            await _productService.SaveAsync(Product);

            await CommonServices.DialogService.ShowInformationAsync(
                "Product saved successfully",
                "Success");
        }
        catch (Exception ex)
        {
            await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
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
}
```

## Best Practices

> [!TIP]
> Use **x:Bind** for compiled bindings to improve performance in UWP.

> [!IMPORTANT]
> UWP apps require appropriate capabilities in Package.appxmanifest for file access, camera, etc.

> [!NOTE]
> UWP is in maintenance mode. Consider WinUI 3 or MAUI for new projects.

### Adaptive UI

- Use AdaptiveTriggers for different screen sizes
- Implement responsive layouts with VisualStateManager
- Support desktop, mobile, and Xbox form factors
- Test on different device types

### Performance

- Use x:Bind for compiled bindings
- Implement UI virtualization
- Use incremental loading for large datasets
- Optimize for memory constraints on mobile devices

## Dependencies

- **I-Synergy.Framework.UI** - Base UI abstractions
- **Microsoft.Windows.SDK.BuildTools** - Windows SDK tools
- **Microsoft.Extensions.Configuration.Json** - Configuration
- **Microsoft.Extensions.Hosting** - Application hosting
- **Microsoft.Xaml.Behaviors.Uwp.Managed** - UWP behaviors
- **Microsoft.UI.Xaml** - WinUI 2 controls

## Platform Requirements

- **Target Framework**: net10.0-windows10.0.26100.0
- **Minimum Version**: Windows 10.0.19041.0 (Version 2004)
  - Previous minimum: Windows 10.0.17763.0 (Version 1809)
  - Changed in: v2.0.0+ ([`ISynergy.Framework.UI.UWP.csproj` line 4](ISynergy.Framework.UI.UWP.csproj#L4))
- **Architecture**: x86, x64, ARM64

### Migration Guide

If your application targets Windows 10 Version 1809 (10.0.17763.0), you have the following options:

1. **Update Target OS**: Update your `Package.appxmanifest` `TargetDeviceFamily` to minimum version `10.0.19041.0`
   ```xml
   <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.19041.0" MaxVersionTested="10.0.26100.0" />
   ```

2. **Use Previous Version**: Continue using v1.x of this package if you need to support Windows 10 Version 1809

3. **Justification for Change**: The minimum version was raised to leverage modern Windows SDK APIs and tooling features that improve compatibility with .NET 10 and provide access to enhanced UWP platform capabilities

## Documentation

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [UWP Documentation](https://docs.microsoft.com/windows/uwp/)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.UI** - Base UI abstractions
- **I-Synergy.Framework.Core** - Core framework
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **I-Synergy.Framework.UI.WinUI** - WinUI 3 implementation (recommended for new projects)
- **I-Synergy.Framework.UI.Maui** - MAUI implementation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

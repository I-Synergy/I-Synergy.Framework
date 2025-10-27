# I-Synergy Framework UI WPF

Windows Presentation Foundation (WPF) UI framework for building modern desktop applications on Windows. This package provides a complete WPF implementation of the I-Synergy Framework UI services, controls, and patterns with support for Windows 7, 8, and 10+.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.UI.WPF.svg)](https://www.nuget.org/packages/I-Synergy.Framework.UI.WPF/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)](https://docs.microsoft.com/dotnet/desktop/wpf/)

## Features

- **Full WPF desktop support** for Windows 7+, 8+, and 10+
- **Dialog service** with MessageBox and custom dialog support
- **Navigation service** with frame-based navigation
- **Theme service** with dynamic accent colors and styles
- **File service** with Windows file dialogs
- **Clipboard service** for Windows clipboard operations
- **Update service** for application updates
- **Custom controls** (BladeView, Console, ImageBrowser, Menu, Tiles, ErrorPresenter)
- **Behaviors** using Microsoft.Xaml.Behaviors.Wpf
- **40+ dynamic theme palettes** with XAML resource dictionaries
- **Splash screen support** with customizable UI

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.UI.WPF
```

##  Quick Start

### 1. Configure Application

Setup your WPF application with I-Synergy Framework:

```csharp
using ISynergy.Framework.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class App : Application
{
    private IHost _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Core services
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<IMessengerService, MessengerService>();
                services.AddSingleton<IBusyService, BusyService>();

                // WPF services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IFileService<FileResult>, FileService>();
                services.AddSingleton<IClipboardService, ClipboardService>();
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

        // Show main window
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
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
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        d:DataContext="{d:DesignInstance Type=vm:MainViewModel}"
        Title="{Binding Title}"
        Height="600" Width="800">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Menu -->
        <Menu Grid.Row="0">
            <MenuItem Header="File">
                <MenuItem Header="New" Command="{Binding NewCommand}"/>
                <MenuItem Header="Open" Command="{Binding OpenCommand}"/>
                <MenuItem Header="Save" Command="{Binding SaveCommand}"/>
                <Separator/>
                <MenuItem Header="Exit" Command="{Binding ExitCommand}"/>
            </MenuItem>
            <MenuItem Header="Edit">
                <MenuItem Header="Settings" Command="{Binding SettingsCommand}"/>
            </MenuItem>
        </Menu>

        <!-- Content -->
        <Frame Grid.Row="1"
               x:Name="MainFrame"
               NavigationUIVisibility="Hidden"/>

        <!-- Status Bar -->
        <StatusBar Grid.Row="2">
            <StatusBarItem>
                <TextBlock Text="{Binding StatusMessage}"/>
            </StatusBarItem>
            <StatusBarItem HorizontalAlignment="Right">
                <ProgressBar Width="100"
                             Height="16"
                             IsIndeterminate="{Binding BusyService.IsBusy}"/>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
```

```csharp
using ISynergy.Framework.Mvvm.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
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
            try
            {
                await _productService.DeleteAsync(Product.Id);
                await CommonServices.NavigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
            }
        }
    }

    // Show custom dialog
    private async Task EditSettingsAsync()
    {
        await CommonServices.DialogService
            .ShowDialogAsync<SettingsWindow, SettingsViewModel, Settings>();
    }
}
```

### 4. Navigation Service

```csharp
public class ShellViewModel : ViewModel
{
    private readonly INavigationService _navigationService;

    public AsyncRelayCommand<Type> NavigateCommand { get; }

    public ShellViewModel(
        ICommonServices commonServices,
        INavigationService navigationService,
        ILogger<ShellViewModel> logger)
        : base(commonServices, logger)
    {
        _navigationService = navigationService;
        NavigateCommand = new AsyncRelayCommand<Type>(NavigateAsync);
    }

    private async Task NavigateAsync(Type viewModelType)
    {
        await _navigationService.NavigateAsync(viewModelType);
    }

    // Navigate with parameters
    private async Task NavigateToProductDetailAsync(Product product)
    {
        await _navigationService.NavigateAsync<ProductDetailViewModel>(product);
    }

    // Navigate back
    private async Task GoBackAsync()
    {
        if (_navigationService.CanGoBack)
        {
            await _navigationService.GoBackAsync();
        }
    }
}
```

### 5. File Operations

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Models.Results;

public class DocumentViewModel : ViewModel
{
    private readonly IFileService<FileResult> _fileService;

    public AsyncRelayCommand OpenFileCommand { get; }
    public AsyncRelayCommand SaveFileCommand { get; }

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

### 6. Update Service

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;

public class UpdateViewModel : ViewModel
{
    private readonly IUpdateService _updateService;

    public AsyncRelayCommand CheckForUpdatesCommand { get; }
    public AsyncRelayCommand InstallUpdateCommand { get; }

    public bool UpdateAvailable
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public string UpdateVersion
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public UpdateViewModel(
        ICommonServices commonServices,
        IUpdateService updateService,
        ILogger<UpdateViewModel> logger)
        : base(commonServices, logger)
    {
        _updateService = updateService;

        CheckForUpdatesCommand = new AsyncRelayCommand(CheckForUpdatesAsync);
        InstallUpdateCommand = new AsyncRelayCommand(InstallUpdateAsync);
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            var update = await _updateService.CheckForUpdateAsync();

            if (update.IsAvailable)
            {
                UpdateAvailable = true;
                UpdateVersion = update.Version;

                var result = await CommonServices.DialogService.ShowMessageAsync(
                    $"A new version ({update.Version}) is available. Would you like to install it?",
                    "Update Available",
                    MessageBoxButtons.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await InstallUpdateAsync();
                }
            }
            else
            {
                await CommonServices.DialogService.ShowInformationAsync(
                    "You are running the latest version.",
                    "No Updates");
            }
        }
        catch (Exception ex)
        {
            await CommonServices.DialogService.ShowErrorAsync(ex, "Error");
        }
    }

    private async Task InstallUpdateAsync()
    {
        try
        {
            CommonServices.BusyService.StartBusy("Downloading update...");

            await _updateService.DownloadAndInstallUpdateAsync(
                progress => CommonServices.BusyService.UpdateMessage($"Downloading... {progress}%"));

            await CommonServices.DialogService.ShowInformationAsync(
                "Update installed successfully. The application will now restart.",
                "Update Complete");

            _updateService.RestartApplication();
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
}
```

## Custom Controls

### BladeView

```xml
<controls:BladeView ItemsSource="{Binding Blades}"
                    SelectedItem="{Binding SelectedBlade}" />
```

### ImageBrowser

```xml
<controls:ImageBrowser Images="{Binding ProductImages}"
                       SelectedImage="{Binding SelectedImage}"
                       AllowAdd="True"
                       AllowRemove="True" />
```

### Console

```xml
<controls:Console Messages="{Binding ConsoleMessages}"
                  AutoScroll="True" />
```

### ErrorPresenter

```xml
<controls:ErrorPresenter Errors="{Binding ValidationErrors}"
                         Visibility="{Binding HasErrors, Converter={StaticResource BoolToVisibilityConverter}}" />
```

### Tiles

```xml
<controls:Tile Title="Dashboard"
               Command="{Binding NavigateToDashboardCommand}"
               Icon="{StaticResource DashboardIcon}" />
```

## Theme Support

The WPF framework includes 40+ built-in theme palettes:

```csharp
using ISynergy.Framework.Mvvm.Abstractions.Services;

// Theme is automatically applied on startup
// To change theme programmatically:
public void ChangeTheme(string colorHex, Themes theme)
{
    // Update settings
    _settingsService.LocalSettings.Color = colorHex;
    _settingsService.LocalSettings.Theme = theme;

    // Apply theme
    _themeService.ApplyTheme();

    // Restart application to fully apply theme
    System.Windows.Forms.Application.Restart();
    System.Windows.Application.Current.Shutdown();
}

// Show theme selection to users
private async Task ShowThemeSelectionAsync()
{
    await CommonServices.DialogService
        .ShowDialogAsync<ThemeWindow, ThemeViewModel, ThemeStyle>();
}
```

## Behaviors

Using Microsoft.Xaml.Behaviors.Wpf:

```xml
<Window xmlns:i="http://schemas.microsoft.com/xaml/behaviors">
    <i:Interaction.Behaviors>
        <behaviors:WindowDragBehavior />
    </i:Interaction.Behaviors>

    <TextBox>
        <i:Interaction.Behaviors>
            <behaviors:SelectAllOnFocusBehavior />
        </i:Interaction.Behaviors>
    </TextBox>
</Window>
```

## Best Practices

> [!TIP]
> Use **Frame** for navigation between pages and **Window** for dialogs and child windows.

> [!IMPORTANT]
> Always configure theme on application startup for consistent UI appearance.

> [!NOTE]
> WPF supports Windows 7, 8, and 10+ through different target frameworks.

### Application Structure

- Use MVVM pattern with dependency injection
- Configure services in Host builder
- Apply theme on startup
- Use NavigationService for page navigation
- Handle application updates gracefully

### Performance

- Use virtualization for large lists
- Implement lazy loading for complex views
- Dispose resources properly
- Use compiled bindings where possible
- Leverage hardware acceleration

## Dependencies

- **I-Synergy.Framework.UI** - Base UI abstractions
- **Microsoft.Xaml.Behaviors.Wpf** - WPF behaviors
- **Microsoft.Extensions.Configuration.Json** - Configuration
- **Microsoft.Extensions.Hosting** - Application hosting
- **OpenTelemetry.Instrumentation.Http** - HTTP telemetry
- **OpenTelemetry.Instrumentation.Runtime** - Runtime telemetry

## Platform Requirements

- **Windows 7.0**: .NET 10.0-windows7.0
- **Windows 8.0**: .NET 10.0-windows8.0
- **Windows 10+**: .NET 10.0-windows10.0.26100.0

## Documentation

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [WPF Documentation](https://docs.microsoft.com/dotnet/desktop/wpf/)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.UI** - Base UI abstractions
- **I-Synergy.Framework.Core** - Core framework
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **I-Synergy.Framework.UI.WinUI** - WinUI implementation
- **I-Synergy.Framework.UI.Maui** - MAUI implementation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

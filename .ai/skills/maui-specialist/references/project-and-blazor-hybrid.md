# MAUI Project Structure & Blazor Hybrid
MAUI solution layout, target frameworks, and hosting Blazor components inside a MAUI app. Load this when scaffolding a MAUI project or setting up Blazor Hybrid.

## MAUI Project Structure

```
{ApplicationName}.Mobile/
├── Platforms/
│   ├── Android/
│   │   ├── AndroidManifest.xml
│   │   ├── MainActivity.cs
│   │   └── Resources/
│   ├── iOS/
│   │   ├── Info.plist
│   │   ├── AppDelegate.cs
│   │   └── Resources/
│   ├── Windows/
│   └── MacCatalyst/
├── wwwroot/           # For Blazor Hybrid
│   ├── css/
│   ├── js/
│   └── index.html
├── Pages/             # XAML pages or Blazor components
├── ViewModels/        # MVVM ViewModels
├── Services/          # Business logic services
├── Models/            # Data models
├── Data/              # Local database
├── Resources/         # Images, fonts, etc.
├── MauiProgram.cs     # App configuration
└── App.xaml           # Application definition
```

## MAUI Blazor Hybrid Setup

### MauiProgram.cs
```csharp
// File: MauiProgram.cs
using Microsoft.Extensions.Logging;
using {ApplicationName}.Mobile.Services;
using {ApplicationName}.Mobile.Data;

namespace {ApplicationName}.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Blazor Hybrid
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);

        // Local database
        builder.Services.AddSingleton<LocalDatabase>();

        // API client
        builder.Services.AddHttpClient<IBudgetApiClient, BudgetApiClient>(client =>
        {
            client.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5000" // Android emulator
                : "http://localhost:5000");
        });

        // Data sync
        builder.Services.AddSingleton<ISyncService, SyncService>();

        // ViewModels
        builder.Services.AddTransient<BudgetListViewModel>();
        builder.Services.AddTransient<BudgetDetailViewModel>();

        return builder.Build();
    }
}
```

### App.xaml (Blazor Hybrid)
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:{ApplicationName}.Mobile"
    x:Class="{ApplicationName}.Mobile.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

```csharp
// File: App.xaml.cs
namespace {ApplicationName}.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        // Set window size for desktop platforms
        window.Width = 400;
        window.Height = 800;

        return window;
    }
}
```

### MainPage (Blazor Hybrid Host)
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:{ApplicationName}.Mobile"
    x:Class="{ApplicationName}.Mobile.MainPage"
    BackgroundColor="{DynamicResource PageBackgroundColor}">

    <BlazorWebView HostPage="wwwroot/index.html">
        <BlazorWebView.RootComponents>
            <RootComponent Selector="#app" ComponentType="{x:Type local:Main}" />
        </BlazorWebView.RootComponents>
    </BlazorWebView>

</ContentPage>
```


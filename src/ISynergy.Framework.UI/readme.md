# I-Synergy Framework UI

Core UI abstractions and shared components for building cross-platform .NET 10.0 user interfaces. This package provides the foundational layer for all I-Synergy UI implementations including WPF, WinUI, UWP, MAUI, and Blazor.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.UI.svg)](https://www.nuget.org/packages/I-Synergy.Framework.UI/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Platform-agnostic abstractions** for UI services and providers
- **Theme management** with dynamic accent color support
- **Authentication provider** for UI security integration
- **Token storage service** for secure credential management
- **ViewModels** for common UI scenarios (Language, Theme selection)
- **Localization support** with built-in resource management
- **Extension methods** for common UI operations
- **Splash screen support** with configurable options
- **Integration with MVVM framework** for ViewModels and services

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.UI
```

For platform-specific implementations, install the appropriate package:
- **WPF**: `I-Synergy.Framework.UI.WPF`
- **WinUI**: `I-Synergy.Framework.UI.WinUI`
- **UWP**: `I-Synergy.Framework.UI.UWP`
- **MAUI**: `I-Synergy.Framework.UI.Maui`
- **Blazor**: `I-Synergy.Framework.UI.Blazor`

## Quick Start

### 1. Authentication Provider

The authentication provider enables role-based UI element visibility and command execution control:

```csharp
using ISynergy.Framework.UI.Abstractions.Providers;
using System.Windows.Input;

public class CustomAuthenticationProvider : IAuthenticationProvider
{
    private readonly IAuthenticationService _authService;

    public CustomAuthenticationProvider(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public bool CanCommandBeExecuted(ICommand command, object commandParameter)
    {
        // Implement custom command authorization logic
        var requiredRole = GetRequiredRoleFromCommand(command);
        return _authService.CurrentUser.HasRole(requiredRole);
    }

    public bool HasAccessToUIElement(object element, object tag, string authorizationTag)
    {
        // Implement custom UI element visibility logic
        if (string.IsNullOrEmpty(authorizationTag))
            return true;

        return _authService.CurrentUser.HasPermission(authorizationTag);
    }
}

// Register in DI
services.AddScoped<IAuthenticationProvider, CustomAuthenticationProvider>();
```

### 2. Theme Management

Use `IThemeService` for live light/dark switching and accent colors. The shell's `ThemeCommand`
toggles light/dark; accent color selection belongs in your settings UI:

```csharp
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;

public class SettingsViewModel : ViewModel
{
    private readonly IThemeService _themeService;

    public ObservableCollection<Themes> ThemeOptions { get; } =
        new ObservableCollection<Themes> { Themes.Light, Themes.Dark };

    public ObservableCollection<string> AccentColors { get; } =
        new ObservableCollection<string>(ThemeColors.Colors);

    public SettingsViewModel(
        ICommonServices commonServices,
        IThemeService themeService,
        ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
        _themeService = themeService;
    }

    public void ApplyTheme(Themes theme) => _themeService.SetTheme(theme);
    public void ApplyAccent(string color) => _themeService.SetAccentColor(color);
}
```

`SetTheme`/`SetAccentColor` persist through `ISettingsService` automatically.

### 3. Language Selection

Language selection belongs in your settings UI. Apply a culture live with
`CultureInfoExtensions.ApplyCulture()`; it refreshes `LocalizedStrings` bindings:

```csharp
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Extensions;

public class SettingsViewModel : ViewModel
{
    private readonly ISettingsService _settingsService;

    public ObservableCollection<CultureInfo> Languages { get; }

    public SettingsViewModel(
        ICommonServices commonServices,
        ISettingsService settingsService,
        ISupportedCulturesService supportedCulturesService,
        ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
        _settingsService = settingsService;
        Languages = new ObservableCollection<CultureInfo>(supportedCulturesService.SupportedCultures);
    }

    public void ApplyLanguage(CultureInfo culture)
    {
        culture.ApplyCulture();
        _settingsService.LocalSettings.Language = culture.Name;
        _settingsService.SaveLocalSettings();
    }
}
```

### 4. Token Storage Service

Securely store and retrieve authentication tokens:

```csharp
using ISynergy.Framework.UI.Abstractions.Services;

public class AuthenticationService
{
    private readonly ITokenStorageService _tokenStorage;

    public AuthenticationService(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        // Perform authentication
        var token = await _authApi.LoginAsync(username, password);

        if (token is not null)
        {
            // Store tokens securely
            await _tokenStorage.StoreTokenAsync("access_token", token.AccessToken);
            await _tokenStorage.StoreTokenAsync("refresh_token", token.RefreshToken);
            return true;
        }

        return false;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await _tokenStorage.GetTokenAsync("access_token");
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.ClearAllTokensAsync();
    }
}
```

## Core Components

### Abstractions

```
ISynergy.Framework.UI.Abstractions/
├── Providers/
│   └── IAuthenticationProvider     # Command and UI element authorization
├── Services/
│   └── ITokenStorageService        # Secure token storage
├── Views/
│   ├── IDashboard                  # Dashboard view interface
│   ├── ISelectionView              # Selection view interface
│   └── IShellView                  # Shell/main view interface
```

### Options

```
ISynergy.Framework.UI.Options/
├── BingMapsOptions                 # Bing Maps API configuration
└── SplashScreenOptions             # Splash screen configuration
```

## Usage Examples

### Splash Screen Configuration

Configure splash screen behavior for your application:

```csharp
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Enumerations;

public void ConfigureServices(IServiceCollection services)
{
    services.Configure<SplashScreenOptions>(options =>
    {
        options.Type = SplashScreenTypes.Extended;
        options.DisplayDuration = TimeSpan.FromSeconds(3);
        options.MinimumDisplayTime = TimeSpan.FromSeconds(1);
    });
}
```

### Assembly Registration

Register views, viewmodels, and windows from assemblies:

```csharp
using ISynergy.Framework.UI.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    var mainAssembly = Assembly.GetExecutingAssembly();

    // Register all views, viewmodels, and windows from assembly
    services.RegisterAssemblies(
        mainAssembly,
        assemblyName => assemblyName.Name.StartsWith("MyApp"));
}
```

### Extension Methods

The UI framework provides several useful extension methods:

```csharp
using ISynergy.Framework.UI.Extensions;

// Credential extensions
var credential = new Credential { Username = "user", Password = "pass" };
string base64 = credential.ToBase64();
var decoded = base64.FromBase64ToCredential();

// DateTime extensions
var now = DateTimeOffset.Now;
string formatted = now.ToLocalString(languageService);
string dateOnly = now.ToLocalDateString(languageService);

// Decimal extensions
decimal value = 1234.56m;
string currency = value.ToCurrency(languageService);
string number = value.ToNumeric(languageService);

// Language extensions
var german = Languages.German;
CultureInfo culture = german.GetCulture();
string displayName = german.GetDescription();

// Telemetry extensions
var exception = new InvalidOperationException("Something failed");
exception.Track(); // Tracks exception in telemetry
```

## Configuration

### Dependency Injection Setup

Platform-specific setup varies, but the core pattern is:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Providers;

public void ConfigureServices(IServiceCollection services)
{
    // Core services
    services.AddSingleton<ILanguageService, LanguageService>();
    services.AddSingleton<IInfoService, InfoService>();
    services.AddSingleton<IMessengerService, MessengerService>();

    // UI services
    services.AddSingleton<ITokenStorageService, TokenStorageService>();
    services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();

    // Platform-specific services (implemented in UI.WPF, UI.MAUI, etc.)
    // services.AddSingleton<IDialogService, DialogService>();
    // services.AddSingleton<INavigationService, NavigationService>();
    // services.AddSingleton<IThemeService, ThemeService>();
}
```

## Best Practices

> [!TIP]
> Use **IAuthenticationProvider** to centralize authorization logic for both commands and UI elements.

> [!IMPORTANT]
> Always use **ITokenStorageService** for storing sensitive authentication tokens instead of plain storage mechanisms.

> [!NOTE]
> The UI framework integrates seamlessly with **I-Synergy.Framework.Mvvm** for ViewModels and commands.

### Authentication Provider Usage

- Implement `IAuthenticationProvider` for centralized authorization
- Use it in command `CanExecute` delegates
- Bind UI element visibility to authorization checks
- Keep authorization logic testable and maintainable

### Theme Management

- Store theme preferences in `ISettingsService`
- Use the shell's `ThemeCommand` for light/dark switching
- Put accent color selection in your settings UI
- Apply themes through platform-specific `IThemeService`
- Support both light and dark themes

### Token Storage

- Never store tokens in plain text
- Use `ITokenStorageService` for all credentials
- Clear tokens on logout
- Implement token refresh logic
- Handle token expiration gracefully

## Platform Integration

This base package is designed to be extended by platform-specific implementations:

### Desktop Platforms
- **WPF**: Full desktop Windows support (.NET 10.0)
- **WinUI**: Modern Windows apps with WinUI 3
- **UWP**: Universal Windows Platform apps

### Mobile & Cross-Platform
- **MAUI**: Cross-platform for Windows, Android, iOS, macOS
- **Blazor**: Web-based UI with WebAssembly or Server

Each platform implementation provides:
- Platform-specific services (Dialog, Navigation, Theme, File)
- Custom controls and styles
- Platform integration features
- Build configurations and assets

## Dependencies

- **I-Synergy.Framework.Mvvm** - MVVM framework integration
- **I-Synergy.Framework.OpenTelemetry** - Telemetry and observability
- **Microsoft.Extensions.Http** - HTTP client factory
- **Microsoft.Extensions.Logging** - Logging infrastructure
- **NodaTime** - Date and time handling
- **OpenTelemetry.Instrumentation.Http** - HTTP telemetry
- **OpenTelemetry.Instrumentation.Runtime** - Runtime telemetry

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

### Core Frameworks
- **I-Synergy.Framework.Core** - Core abstractions and services
- **I-Synergy.Framework.Mvvm** - MVVM framework

### Platform-Specific UI
- **I-Synergy.Framework.UI.Maui** - .NET MAUI implementation
- **I-Synergy.Framework.UI.WPF** - WPF implementation
- **I-Synergy.Framework.UI.WinUI** - WinUI 3 implementation
- **I-Synergy.Framework.UI.UWP** - UWP implementation
- **I-Synergy.Framework.UI.Blazor** - Blazor implementation

### Other Frameworks
- **I-Synergy.Framework.CQRS** - CQRS pattern implementation
- **I-Synergy.Framework.AspNetCore** - ASP.NET Core integration

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

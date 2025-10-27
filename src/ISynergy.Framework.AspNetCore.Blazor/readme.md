# I-Synergy Framework AspNetCore Blazor

Comprehensive Blazor component library and services for building modern ASP.NET Core Blazor applications. This package provides authentication providers, navigation menu services, exception handling, antiforgery protection, form factor detection, and static asset management for both Blazor Server and Blazor WebAssembly applications.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.AspNetCore.Blazor.svg)](https://www.nuget.org/packages/I-Synergy.Framework.AspNetCore.Blazor/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Authentication provider** with cascading authentication state support
- **Navigation menu service** for hierarchical menu structures (links and groups)
- **Exception handler service** with intelligent duplicate detection and filtering
- **Antiforgery HTTP client factory** for CSRF token integration
- **Form factor service** for responsive design and device detection
- **Static asset service** for loading resources from wwwroot
- **Cache storage service** for browser-based caching
- **Component extensions** for lifecycle management and JavaScript interop
- **Analytics integration** with configurable options
- **View/Window component base classes** for MVVM pattern
- **Message box enumerations** for consistent UI dialogs

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.AspNetCore.Blazor
```

## Quick Start

### 1. Configure Blazor Services

In your `Program.cs`:

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Extensions;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Blazor services with framework integration
builder.Services.ConfigureServices<AppContext, CommonServices, SettingsService, Resources>(
    builder.Configuration,
    infoService: new InfoService(),
    action: services =>
    {
        // Add your custom services here
        services.AddScoped<IDataService, DataService>();
    },
    assembly: typeof(Program).Assembly,
    assemblyFilter: name => name.Name.StartsWith("YourApp"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### 2. Configure Analytics Options

In your `appsettings.json`:

```json
{
  "AnalyticOptions": {
    "MeasurementId": "G-XXXXXXXXXX",
    "TrackingId": "UA-XXXXXXXXX-X"
  },
  "ClientApplicationOptions": {
    "ApplicationName": "My Blazor App",
    "Version": "1.0.0"
  }
}
```

### 3. Using the Authentication Provider

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Providers;
using Microsoft.AspNetCore.Components;

@inject IAuthenticationProvider AuthenticationProvider

@code {
    private string username;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            username = user.Identity.Name;
        }
    }

    private async Task LoginAsync()
    {
        // Trigger login flow
        await AuthenticationProvider.LoginAsync();
    }

    private async Task LogoutAsync()
    {
        // Trigger logout flow
        await AuthenticationProvider.LogoutAsync();
    }
}
```

### 4. Creating a Navigation Menu

Define your navigation structure:

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
using ISynergy.Framework.AspNetCore.Blazor.Models;
using ISynergy.Framework.AspNetCore.Blazor.Services.Base;

public class MyNavigationMenuService : BaseNavigationMenuService
{
    public MyNavigationMenuService()
    {
        MenuItems = new List<NavigationItem>
        {
            new NavigationLink
            {
                Name = "Home",
                Href = "/",
                Icon = "home",
                Match = NavLinkMatch.All
            },
            new NavigationGroup
            {
                Name = "Products",
                Icon = "shopping-cart",
                Items = new List<NavigationItem>
                {
                    new NavigationLink
                    {
                        Name = "All Products",
                        Href = "/products",
                        Icon = "list"
                    },
                    new NavigationLink
                    {
                        Name = "New Product",
                        Href = "/products/new",
                        Icon = "plus"
                    }
                }
            },
            new NavigationGroup
            {
                Name = "Settings",
                Icon = "settings",
                Items = new List<NavigationItem>
                {
                    new NavigationLink
                    {
                        Name = "Profile",
                        Href = "/settings/profile",
                        Icon = "user"
                    },
                    new NavigationLink
                    {
                        Name = "Security",
                        Href = "/settings/security",
                        Icon = "lock"
                    }
                }
            }
        };
    }
}

// Register in DI
builder.Services.AddScoped<INavigationMenuService, MyNavigationMenuService>();
```

### 5. Using the Exception Handler

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Components;

@inject IExceptionHandlerService ExceptionHandler

@code {
    private async Task LoadDataAsync()
    {
        try
        {
            var data = await DataService.GetDataAsync();
            // Process data
        }
        catch (Exception ex)
        {
            // Let the exception handler manage the error display
            await ExceptionHandler.HandleExceptionAsync(ex);
        }
    }
}
```

## Core Components

### Services

```
ISynergy.Framework.AspNetCore.Blazor.Services/
├── ExceptionHandlerService         # Global exception handling with filtering
├── StaticAssetService              # Load resources from wwwroot
├── FormFactorService               # Device and screen size detection
└── BaseNavigationMenuService       # Base class for navigation menus
```

### Providers

```
ISynergy.Framework.AspNetCore.Blazor.Providers/
└── AuthenticationProvider          # Authentication state management
```

### Security

```
ISynergy.Framework.AspNetCore.Blazor.Security/
└── AntiforgeryHttpClientFactory    # CSRF token integration for HTTP calls
```

### Models

```
ISynergy.Framework.AspNetCore.Blazor.Models/
├── NavigationItem                  # Base navigation item
├── NavigationLink                  # Single navigation link
├── NavigationGroup                 # Grouped navigation items
└── CookieState                     # Cookie management
```

### Components

```
ISynergy.Framework.AspNetCore.Blazor.Components/
└── View                            # Base view component for MVVM
```

## Advanced Features

### Using Antiforgery HTTP Client

Protect your API calls with CSRF tokens:

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Security;
using Microsoft.AspNetCore.Components;

@inject IAntiforgeryHttpClientFactory HttpClientFactory

@code {
    private async Task SaveDataAsync(DataModel data)
    {
        // Create client with antiforgery token automatically added
        var client = await HttpClientFactory.CreateClientAsync("authorizedClient");

        var response = await client.PostAsJsonAsync("/api/data", data);

        if (response.IsSuccessStatusCode)
        {
            // Success
        }
    }
}
```

Configure the client in `Program.cs`:

```csharp
builder.Services.AddHttpClient("authorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});
```

### Form Factor Detection

Detect device types and screen sizes for responsive design:

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
using Microsoft.AspNetCore.Components;

@inject IFormFactorService FormFactorService

@code {
    private bool isMobile;
    private bool isTablet;
    private bool isDesktop;

    protected override void OnInitialized()
    {
        isMobile = FormFactorService.IsMobile;
        isTablet = FormFactorService.IsTablet;
        isDesktop = FormFactorService.IsDesktop;

        // Subscribe to form factor changes
        FormFactorService.FormFactorChanged += OnFormFactorChanged;
    }

    private void OnFormFactorChanged(object sender, EventArgs e)
    {
        isMobile = FormFactorService.IsMobile;
        isTablet = FormFactorService.IsTablet;
        isDesktop = FormFactorService.IsDesktop;
        StateHasChanged();
    }

    public void Dispose()
    {
        FormFactorService.FormFactorChanged -= OnFormFactorChanged;
    }
}

<div class="@(isMobile ? "mobile-layout" : isTablet ? "tablet-layout" : "desktop-layout")">
    @if (isMobile)
    {
        <MobileView />
    }
    else if (isTablet)
    {
        <TabletView />
    }
    else
    {
        <DesktopView />
    }
</div>
```

### Loading Static Assets

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
using Microsoft.AspNetCore.Components;

@inject IStaticAssetService StaticAssetService

@code {
    private string logoData;

    protected override async Task OnInitializedAsync()
    {
        // Load an image from wwwroot
        var imageBytes = await StaticAssetService.GetAssetAsync("images/logo.png");
        logoData = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
    }
}

<img src="@logoData" alt="Logo" />
```

### Exception Handler Filtering

The exception handler automatically filters common exceptions:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class DataService
{
    private readonly IExceptionHandlerService _exceptionHandler;

    public DataService(IExceptionHandlerService exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    public async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            await LoadAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // The handler automatically filters:
            // - TaskCanceledException when cancellation is requested
            // - OperationCanceledException
            // - WebSocketException
            // - Duplicate error messages
            // - I/O abort exceptions
            await _exceptionHandler.HandleExceptionAsync(ex);
        }
    }
}
```

## Usage Examples

### Building a Navigation Menu with Icons

```csharp
using ISynergy.Framework.AspNetCore.Blazor.Models;

public class AppNavigationMenuService : BaseNavigationMenuService
{
    public AppNavigationMenuService()
    {
        MenuItems = new List<NavigationItem>
        {
            // Dashboard
            new NavigationLink
            {
                Name = "Dashboard",
                Href = "/",
                Icon = "dashboard",
                Match = NavLinkMatch.All
            },

            // Sales group with sub-items
            new NavigationGroup
            {
                Name = "Sales",
                Icon = "shopping-bag",
                Items = new List<NavigationItem>
                {
                    new NavigationLink
                    {
                        Name = "Orders",
                        Href = "/sales/orders",
                        Icon = "receipt"
                    },
                    new NavigationLink
                    {
                        Name = "Customers",
                        Href = "/sales/customers",
                        Icon = "people"
                    },
                    new NavigationLink
                    {
                        Name = "Products",
                        Href = "/sales/products",
                        Icon = "box"
                    }
                }
            },

            // Reports group
            new NavigationGroup
            {
                Name = "Reports",
                Icon = "chart",
                Items = new List<NavigationItem>
                {
                    new NavigationLink
                    {
                        Name = "Sales Report",
                        Href = "/reports/sales",
                        Icon = "trending-up"
                    },
                    new NavigationLink
                    {
                        Name = "Customer Report",
                        Href = "/reports/customers",
                        Icon = "bar-chart"
                    }
                }
            },

            // Settings
            new NavigationLink
            {
                Name = "Settings",
                Href = "/settings",
                Icon = "settings"
            }
        };

        // FlattenedMenuItems is automatically populated
        // Contains all links without groups for search functionality
    }
}
```

### Consuming the Navigation Menu

```razor
@inject INavigationMenuService NavigationMenuService

<nav class="sidebar">
    @foreach (var item in NavigationMenuService.MenuItems)
    {
        @if (item is NavigationLink link)
        {
            <NavLink href="@link.Href" Match="@link.Match" class="nav-link">
                <i class="icon icon-@link.Icon"></i>
                <span>@link.Name</span>
            </NavLink>
        }
        else if (item is NavigationGroup group)
        {
            <div class="nav-group">
                <div class="nav-group-header">
                    <i class="icon icon-@group.Icon"></i>
                    <span>@group.Name</span>
                </div>
                <div class="nav-group-items">
                    @foreach (var subItem in group.Items)
                    {
                        @if (subItem is NavigationLink subLink)
                        {
                            <NavLink href="@subLink.Href" Match="@subLink.Match" class="nav-link">
                                <i class="icon icon-@subLink.Icon"></i>
                                <span>@subLink.Name</span>
                            </NavLink>
                        }
                    }
                </div>
            </div>
        }
    }
</nav>
```

### Global Exception Handling with Custom Messages

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Mvvm.Messages;

public class CustomExceptionHandlerService : ExceptionHandlerService
{
    public CustomExceptionHandlerService(
        IBusyService busyService,
        ILogger<CustomExceptionHandlerService> logger)
        : base(busyService, logger)
    {
    }

    public override Task HandleExceptionAsync(Exception exception)
    {
        // Handle custom exception types
        if (exception is ValidationException validationException)
        {
            MessengerService.Default.Send(new ShowWarningMessage(
                new MessageBoxRequest(validationException.Message)));
            return Task.CompletedTask;
        }

        if (exception is BusinessRuleException businessException)
        {
            MessengerService.Default.Send(new ShowInformationMessage(
                new MessageBoxRequest(businessException.Message)));
            return Task.CompletedTask;
        }

        // Fall back to base implementation
        return base.HandleExceptionAsync(exception);
    }
}

// Register custom handler
builder.Services.AddSingleton<IExceptionHandlerService, CustomExceptionHandlerService>();
```

### Responsive Layout with Form Factor Service

```razor
@implements IDisposable
@inject IFormFactorService FormFactorService

<div class="layout @GetLayoutClass()">
    @if (FormFactorService.IsMobile)
    {
        <!-- Mobile layout -->
        <div class="mobile-header">
            <button @onclick="ToggleMenu">Menu</button>
            <h1>@Title</h1>
        </div>

        @if (showMenu)
        {
            <div class="mobile-menu">
                <NavigationMenu />
            </div>
        }

        <div class="mobile-content">
            @ChildContent
        </div>
    }
    else
    {
        <!-- Desktop/Tablet layout -->
        <div class="sidebar">
            <NavigationMenu />
        </div>

        <div class="main-content">
            <header>
                <h1>@Title</h1>
            </header>
            <main>
                @ChildContent
            </main>
        </div>
    }
</div>

@code {
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    private bool showMenu = false;

    protected override void OnInitialized()
    {
        FormFactorService.FormFactorChanged += OnFormFactorChanged;
    }

    private void OnFormFactorChanged(object sender, EventArgs e)
    {
        StateHasChanged();
    }

    private string GetLayoutClass()
    {
        if (FormFactorService.IsMobile) return "mobile";
        if (FormFactorService.IsTablet) return "tablet";
        return "desktop";
    }

    private void ToggleMenu()
    {
        showMenu = !showMenu;
    }

    public void Dispose()
    {
        FormFactorService.FormFactorChanged -= OnFormFactorChanged;
    }
}
```

## Best Practices

> [!TIP]
> Use the **INavigationMenuService** to centralize your menu structure and make it easy to update across your application.

> [!IMPORTANT]
> Always use **IAntiforgeryHttpClientFactory** for POST/PUT/DELETE operations to prevent CSRF attacks.

> [!NOTE]
> The **ExceptionHandlerService** automatically filters common exceptions. Extend it for custom exception types.

### Navigation Menu Design

- Keep menu hierarchies shallow (2-3 levels maximum)
- Use meaningful icons that represent functionality
- Group related items together
- Consider mobile menu experience
- Use NavLinkMatch.All for exact matches
- Implement role-based menu filtering

### Exception Handling

- Let ExceptionHandlerService handle common exceptions
- Extend the service for custom exception types
- Use specific exception types for business logic
- Log exceptions before showing user messages
- Avoid exposing technical details to users
- Provide actionable error messages

### Authentication

- Use cascading authentication state
- Check authorization in components
- Handle anonymous users gracefully
- Implement proper logout flow
- Store tokens securely
- Use refresh tokens for long sessions

### Form Factor Detection

- Design mobile-first
- Test on actual devices
- Handle orientation changes
- Optimize for touch on mobile
- Use appropriate breakpoints
- Consider tablet as separate experience

## Testing

Example unit tests for Blazor components:

```csharp
using Bunit;
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class NavigationMenuTests : TestContext
{
    [Fact]
    public void NavigationMenu_RendersAllItems()
    {
        // Arrange
        var menuService = new Mock<INavigationMenuService>();
        menuService.Setup(m => m.MenuItems).Returns(new List<NavigationItem>
        {
            new NavigationLink { Name = "Home", Href = "/" },
            new NavigationLink { Name = "About", Href = "/about" }
        });

        Services.AddSingleton(menuService.Object);

        // Act
        var cut = RenderComponent<NavigationMenu>();

        // Assert
        cut.Find("a[href='/']").TextContent.Should().Contain("Home");
        cut.Find("a[href='/about']").TextContent.Should().Contain("About");
    }

    [Fact]
    public async Task ExceptionHandler_FiltersTaskCanceledException()
    {
        // Arrange
        var busyService = new Mock<IBusyService>();
        var logger = Mock.Of<ILogger<ExceptionHandlerService>>();
        var handler = new ExceptionHandlerService(busyService.Object, logger);

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var exception = new TaskCanceledException("Test", null, cts.Token);

        // Act
        await handler.HandleExceptionAsync(exception);

        // Assert - no message should be shown for cancelled tasks
        busyService.Verify(b => b.StopBusy(), Times.Never);
    }
}
```

## Dependencies

- **Microsoft.AspNetCore.Components** - Blazor components infrastructure
- **Microsoft.AspNetCore.Components.Web** - Web-specific Blazor components
- **Microsoft.AspNetCore.Components.Authorization** - Authentication state
- **Microsoft.JSInterop** - JavaScript interop
- **ISynergy.Framework.Core** - Core framework utilities
- **ISynergy.Framework.Mvvm** - MVVM pattern support

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **I-Synergy.Framework.AspNetCore** - Base ASP.NET Core integration
- **I-Synergy.Framework.AspNetCore.Authentication** - Authentication utilities
- **I-Synergy.Framework.UI.Blazor** - Additional Blazor UI components

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

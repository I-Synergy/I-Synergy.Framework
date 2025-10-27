# I-Synergy Framework UI Blazor

Blazor UI framework for building modern web applications with WebAssembly or Server-side rendering. This package provides a complete Blazor implementation of the I-Synergy Framework UI services, components, and patterns.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.UI.Blazor.svg)](https://www.nuget.org/packages/I-Synergy.Framework.UI.Blazor/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Web-blue.svg)](https://docs.microsoft.com/aspnet/core/blazor/)

## Features

- **Blazor WebAssembly and Server** support
- **Authentication and authorization** with JWT support
- **Form factor service** for responsive design
- **Exception handling** with user-friendly error messages
- **Static asset service** for loading configuration and resources
- **Antiforgery token support** for secure forms
- **HTTP resilience** with retry policies
- **Component extensions** for common patterns
- **JavaScript interop** extensions
- **Integration with MVVM** framework for ViewModels

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.UI.Blazor
```

## Quick Start

### 1. Configure Services

Setup your Blazor application with I-Synergy Framework:

```csharp
using ISynergy.Framework.UI.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure I-Synergy Framework services
builder.Services.ConfigureServices<AppContext, CommonServices, SettingsService, Resources>(
    builder.Configuration,
    InfoService.Default,
    services =>
    {
        // Register additional services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<MainViewModel>();
    },
    Assembly.GetExecutingAssembly(),
    assemblyName => assemblyName.Name.StartsWith("MyApp")
);

await builder.Build().RunAsync();
```

### 2. Create Razor Components

```razor
@page "/products"
@using ISynergy.Framework.Mvvm.Abstractions.ViewModels
@inject IProductService ProductService
@inject IDialogService DialogService

<h3>Products</h3>

@if (products == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Name</th>
                <th>Price</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in products)
            {
                <tr>
                    <td>@product.Name</td>
                    <td>@product.Price.ToString("C")</td>
                    <td>
                        <button class="btn btn-primary" @onclick="() => EditProduct(product)">Edit</button>
                        <button class="btn btn-danger" @onclick="() => DeleteProduct(product)">Delete</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<Product> products;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            products = await ProductService.GetAllAsync();
        }
        catch (Exception ex)
        {
            await DialogService.ShowErrorAsync(ex, "Error");
        }
    }

    private async Task EditProduct(Product product)
    {
        // Navigate to edit page or show dialog
    }

    private async Task DeleteProduct(Product product)
    {
        var confirmed = await DialogService.ShowMessageAsync(
            $"Are you sure you want to delete {product.Name}?",
            "Confirm Delete",
            MessageBoxButtons.YesNo);

        if (confirmed == MessageBoxResult.Yes)
        {
            await ProductService.DeleteAsync(product.Id);
            products.Remove(product);
            StateHasChanged();
        }
    }
}
```

### 3. Authentication

Configure authentication with JWT:

```csharp
// In Program.cs
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Configure HTTP client with authentication
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
```

```razor
<!-- App.razor -->
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    <RedirectToLogin />
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

### 4. Form Factor Service

Detect and respond to device form factors:

```razor
@inject IFormFactorService FormFactorService

<div class="@(FormFactorService.IsMobile ? "mobile-layout" : "desktop-layout")">
    @if (FormFactorService.IsMobile)
    {
        <MobileNavigation />
    }
    else
    {
        <DesktopNavigation />
    }
</div>

@code {
    protected override void OnInitialized()
    {
        FormFactorService.FormFactorChanged += OnFormFactorChanged;
    }

    private void OnFormFactorChanged(object sender, EventArgs e)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        FormFactorService.FormFactorChanged -= OnFormFactorChanged;
    }
}
```

### 5. JavaScript Interop Extensions

Use JavaScript interop extensions:

```csharp
using ISynergy.Framework.UI.Extensions;
using Microsoft.JSInterop;

@inject IJSRuntime JS

@code {
    private async Task ShowAlertAsync()
    {
        await JS.InvokeVoidAsync("alert", "Hello from Blazor!");
    }

    private async Task<string> GetLocalStorageItemAsync(string key)
    {
        return await JS.InvokeAsync<string>("localStorage.getItem", key);
    }

    private async Task SetLocalStorageItemAsync(string key, string value)
    {
        await JS.InvokeVoidAsync("localStorage.setItem", key, value);
    }
}
```

### 6. Static Asset Service

Load configuration from static assets:

```csharp
using ISynergy.Framework.UI.Abstractions.Services;

public class ConfigurationViewModel : ViewModel
{
    private readonly IStaticAssetService _staticAssetService;

    public ConfigurationViewModel(
        ICommonServices commonServices,
        IStaticAssetService staticAssetService,
        ILogger<ConfigurationViewModel> logger)
        : base(commonServices, logger)
    {
        _staticAssetService = staticAssetService;
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Load configuration from wwwroot/appsettings.json
        var config = await _staticAssetService.GetConfigurationAsync();

        // Load custom JSON
        var customData = await _staticAssetService.LoadJsonAsync<CustomData>("data/custom.json");

        IsInitialized = true;
    }
}
```

## Component Extensions

The Blazor framework provides useful component extensions:

```razor
@using ISynergy.Framework.UI.Extensions

<!-- Conditional rendering helper -->
@if (items.HasItems())
{
    <ul>
        @foreach (var item in items)
        {
            <li>@item.Name</li>
        }
    </ul>
}

<!-- Safe navigation -->
<p>@Model?.Name?.ToUpper()</p>

<!-- Format helpers -->
<p>Price: @price.ToCurrency()</p>
<p>Date: @date.ToLocalString(LanguageService)</p>
```

## HTTP Resilience

Configure HTTP resilience with retry policies:

```csharp
// In Program.cs
builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
});
```

## Exception Handling

Centralized exception handling:

```razor
<ErrorBoundary>
    <ChildContent>
        @Body
    </ChildContent>
    <ErrorContent Context="exception">
        <div class="alert alert-danger">
            <h4>An error occurred</h4>
            <p>@exception.Message</p>
            @if (IsDevelopment)
            {
                <pre>@exception.StackTrace</pre>
            }
        </div>
    </ErrorContent>
</ErrorBoundary>
```

## Best Practices

> [!TIP]
> Use **StateHasChanged()** sparingly and only when necessary to improve performance.

> [!IMPORTANT]
> Always dispose of event handlers and IDisposable resources in @code blocks.

> [!NOTE]
> Blazor WebAssembly runs in the browser sandbox with security restrictions.

### Performance

- Minimize re-renders with @key directive
- Use virtualization for large lists
- Implement lazy loading for components
- Optimize bundle size
- Use streaming rendering for server-side

### Security

- Always validate input on the server
- Use HTTPS for all communication
- Implement proper authentication and authorization
- Protect against CSRF with antiforgery tokens
- Sanitize user input

### State Management

- Use cascading parameters for shared state
- Implement services for cross-component communication
- Use browser storage for persistence
- Consider state containers for complex scenarios

## Dependencies

- **I-Synergy.Framework.UI** - Base UI abstractions
- **Microsoft.AspNetCore.Components.Web** - Blazor components
- **Microsoft.AspNetCore.Components.Authorization** - Authentication support
- **Microsoft.Extensions.Http.Resilience** - HTTP resilience
- **System.IdentityModel.Tokens.Jwt** - JWT token handling

## Platform Requirements

- **Target Framework**: net10.0
- **Browser Support**: Modern browsers (Chrome, Edge, Firefox, Safari)
- **WebAssembly Support**: Required for Blazor WebAssembly
- **Server Requirements**: ASP.NET Core 10.0+ for Blazor Server

## Documentation

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.UI** - Base UI abstractions
- **I-Synergy.Framework.Core** - Core framework
- **I-Synergy.Framework.Mvvm** - MVVM framework
- **I-Synergy.Framework.AspNetCore** - ASP.NET Core integration
- **I-Synergy.Framework.UI.Maui** - MAUI implementation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

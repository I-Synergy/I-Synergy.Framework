# I-Synergy Framework AspNetCore Globalization

Comprehensive globalization and localization support for ASP.NET Core applications. This package provides request culture providers, route-based culture resolution, language services, and seamless integration with ASP.NET Core's localization middleware.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.AspNetCore.Globalization.svg)](https://www.nuget.org/packages/I-Synergy.Framework.AspNetCore.Globalization/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Route-based culture resolution** with automatic URL pattern detection
- **Multiple culture providers** (Route, QueryString, Cookie, Accept-Language header)
- **Culture route constraint** for validating culture segments in URLs
- **Language service integration** with resource management
- **Configurable globalization options** with default and supported cultures
- **ASP.NET Core middleware integration** with RequestLocalizationMiddleware
- **Culture fallback support** for invalid or missing culture specifications
- **HttpContext accessor** for culture management across application layers

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.AspNetCore.Globalization
```

## Quick Start

### 1. Configure Globalization Services

In your `Program.cs`:

```csharp
using ISynergy.Framework.AspNetCore.Globalization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add globalization services
builder.AddGlobalization();

builder.Services.AddControllers();

var app = builder.Build();

// Use request localization middleware
app.UseRequestLocalization();

app.MapControllers();
app.Run();
```

### 2. Configure Globalization Options

In your `appsettings.json`:

```json
{
  "GlobalizationOptions": {
    "DefaultCulture": "en-US",
    "SupportedCultures": [
      "en-US",
      "nl-NL",
      "de-DE",
      "fr-FR",
      "es-ES"
    ],
    "ProviderType": "Route"
  }
}
```

### 3. Configure Route-Based Culture

For route-based culture resolution, update your routing:

```csharp
using ISynergy.Framework.AspNetCore.Globalization.Constraints;

var builder = WebApplication.CreateBuilder(args);

builder.AddGlobalization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRequestLocalization();

// Map controllers with culture route constraint
app.MapControllerRoute(
    name: "default",
    pattern: "{culture:culture}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "fallback",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### 4. Using the Language Service

Access localized resources in your code:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("{culture:culture}/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILanguageService _languageService;

    public ProductsController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var welcomeMessage = _languageService.GetString("WelcomeMessage");
        var productsTitle = _languageService.GetString("ProductsTitle");

        return Ok(new
        {
            Message = welcomeMessage,
            Title = productsTitle
        });
    }
}
```

## Core Components

### Options

```
ISynergy.Framework.AspNetCore.Globalization.Options/
└── GlobalizationOptions              # Configuration for cultures and providers
```

### Providers

```
ISynergy.Framework.AspNetCore.Globalization.Providers/
└── RouteDataRequestCultureProvider   # Extract culture from route data
```

### Constraints

```
ISynergy.Framework.AspNetCore.Globalization.Constraints/
└── CultureRouteConstraint            # Validate culture in route segments
```

### Services

```
ISynergy.Framework.AspNetCore.Globalization.Services/
└── LanguageService                   # Access localized resources
```

### Enumerations

```
ISynergy.Framework.AspNetCore.Globalization.Enumerations/
└── RequestCultureProviderTypes       # Available provider types
```

## Advanced Features

### Multiple Culture Provider Strategies

Configure different provider types based on your application needs:

```json
{
  "GlobalizationOptions": {
    "DefaultCulture": "en-US",
    "SupportedCultures": ["en-US", "nl-NL", "de-DE"],
    "ProviderType": "Route"
  }
}
```

Available provider types:
- **Route**: Extract culture from URL path (e.g., `/nl-NL/products`)
- **QueryString**: Extract culture from query string (e.g., `?culture=nl-NL`)
- **Cookie**: Read culture from a cookie
- **AcceptLanguageHeader**: Use Accept-Language HTTP header

### Custom Route Patterns

Define culture-aware routes in your controllers:

```csharp
using Microsoft.AspNetCore.Mvc;

[Route("{culture:culture}/[controller]")]
[ApiController]
public class LocalizedController : ControllerBase
{
    // URL: /nl-NL/localized/hello
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        var culture = RouteData.Values["culture"]?.ToString();
        return Ok($"Hello in culture: {culture}");
    }
}

[Route("[controller]")]
[ApiController]
public class NonLocalizedController : ControllerBase
{
    // URL: /nonlocalized/hello (uses default culture)
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok("Hello");
    }
}
```

### Manual Culture Management

Set or override culture programmatically:

```csharp
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CultureController : ControllerBase
{
    [HttpPost("set")]
    public IActionResult SetCulture([FromBody] string culture)
    {
        // Set culture cookie
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

        return Ok($"Culture set to: {culture}");
    }

    [HttpGet("current")]
    public IActionResult GetCulture()
    {
        var feature = HttpContext.Features.Get<IRequestCultureFeature>();
        var culture = feature?.RequestCulture.Culture.Name ?? "Unknown";
        var uiCulture = feature?.RequestCulture.UICulture.Name ?? "Unknown";

        return Ok(new
        {
            Culture = culture,
            UICulture = uiCulture
        });
    }
}
```

### Resource Management

Configure localized resources for your application:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

// In your Program.cs or startup configuration
var languageService = builder.Services
    .BuildServiceProvider()
    .GetRequiredService<ILanguageService>();

// Add resource managers for different assemblies
languageService.AddResourceManager(typeof(AppResources));
languageService.AddResourceManager(typeof(SharedResources));
languageService.AddResourceManager(typeof(ValidationResources));
```

Create resource files for each supported culture:
- `Resources/AppResources.resx` (default/English)
- `Resources/AppResources.nl-NL.resx` (Dutch)
- `Resources/AppResources.de-DE.resx` (German)

Access resources:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class LocalizedService
{
    private readonly ILanguageService _languageService;

    public LocalizedService(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public string GetLocalizedMessage(string key)
    {
        return _languageService.GetString(key);
    }

    public string GetLocalizedMessageWithFormat(string key, params object[] args)
    {
        var format = _languageService.GetString(key);
        return string.Format(format, args);
    }
}
```

## Usage Examples

### E-commerce Application with Multiple Languages

Complete example of a multi-language e-commerce API:

```csharp
using ISynergy.Framework.AspNetCore.Globalization.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add globalization
builder.AddGlobalization();

builder.Services.AddControllers();

var app = builder.Build();

// Configure request localization
var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>()
    .Value;

app.UseRequestLocalization(localizationOptions);

// Route configuration with culture
app.MapControllerRoute(
    name: "localized",
    pattern: "{culture:culture}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Product Controller
[Route("{culture:culture}/api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ILanguageService _languageService;
    private readonly IProductRepository _productRepository;

    public ProductsController(
        ILanguageService languageService,
        IProductRepository productRepository)
    {
        _languageService = languageService;
        _productRepository = productRepository;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _productRepository.GetAll();

        return Ok(new
        {
            Title = _languageService.GetString("Products_Title"),
            Description = _languageService.GetString("Products_Description"),
            Items = products
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            return NotFound(new
            {
                Error = _languageService.GetString("Product_NotFound")
            });
        }

        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Error = _languageService.GetString("Validation_Failed"),
                Errors = ModelState
            });
        }

        var product = _productRepository.Create(productDto);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }
}
```

### Blazor Server with Route-Based Culture

```csharp
// Program.cs
using ISynergy.Framework.AspNetCore.Globalization.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddGlobalization();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

app.UseRequestLocalization();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/{culture:culture}/_Host");
app.MapFallbackToPage("/_Host");

app.Run();
```

```razor
<!-- _Host.cshtml -->
@page "/{culture?}"
@using Microsoft.AspNetCore.Localization

@{
    var culture = RouteData.Values["culture"]?.ToString() ?? "en-US";
    var requestCulture = new RequestCulture(culture);

    Context.Features.Set<IRequestCultureFeature>(
        new RequestCultureFeature(requestCulture, null));
}

<!DOCTYPE html>
<html lang="@culture">
<head>
    <meta charset="utf-8" />
    <title>My App - @culture</title>
</head>
<body>
    <component type="typeof(App)" render-mode="ServerPrerendered" />
</body>
</html>
```

### Culture Switcher Component

```razor
@inject ILanguageService LanguageService
@inject NavigationManager Navigation

<div class="culture-switcher">
    <label>@LanguageService.GetString("SelectLanguage")</label>
    <select @onchange="OnCultureChanged">
        <option value="en-US" selected="@(CurrentCulture == "en-US")">English</option>
        <option value="nl-NL" selected="@(CurrentCulture == "nl-NL")">Nederlands</option>
        <option value="de-DE" selected="@(CurrentCulture == "de-DE")">Deutsch</option>
        <option value="fr-FR" selected="@(CurrentCulture == "fr-FR")">Français</option>
        <option value="es-ES" selected="@(CurrentCulture == "es-ES")">Español</option>
    </select>
</div>

@code {
    private string CurrentCulture { get; set; } = "en-US";

    protected override void OnInitialized()
    {
        var uri = new Uri(Navigation.Uri);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 0)
        {
            CurrentCulture = segments[0];
        }
    }

    private void OnCultureChanged(ChangeEventArgs e)
    {
        var newCulture = e.Value?.ToString() ?? "en-US";
        var uri = new Uri(Navigation.Uri);
        var path = uri.AbsolutePath;

        // Remove current culture from path if present
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0 && IsSupportedCulture(segments[0]))
        {
            path = "/" + string.Join("/", segments.Skip(1));
        }

        // Redirect to new culture
        var newUri = $"/{newCulture}{path}{uri.Query}";
        Navigation.NavigateTo(newUri, forceLoad: true);
    }

    private bool IsSupportedCulture(string culture)
    {
        var supported = new[] { "en-US", "nl-NL", "de-DE", "fr-FR", "es-ES" };
        return supported.Contains(culture);
    }
}
```

## Best Practices

> [!TIP]
> Use **route-based culture** for SEO-friendly URLs and better user experience with language-specific content.

> [!IMPORTANT]
> Always configure a **default culture** to fall back to when the requested culture is not supported.

> [!NOTE]
> Test your application with **right-to-left (RTL)** languages if you plan to support them (e.g., Arabic, Hebrew).

### Culture Configuration

- Choose route-based culture for public websites (better SEO)
- Use cookie-based culture for authenticated applications
- Accept-Language header works well for APIs
- Always provide a default culture fallback
- Keep supported cultures list synchronized across configuration
- Validate culture codes against CultureInfo

### Resource Management

- Organize resources by feature or module
- Use meaningful resource keys (e.g., "Product_NotFound" not "Err001")
- Keep resource files in sync across all cultures
- Implement a translation workflow for new resources
- Use placeholders for dynamic content
- Consider using satellite assemblies for large resource files

### URL Design

- Place culture code at the start of the URL (`/{culture}/products`)
- Use culture route constraint to validate culture codes
- Provide culture-neutral routes as fallback
- Implement culture switcher without losing context
- Handle culture redirects with 302 (temporary) not 301 (permanent)
- Preserve query strings when switching cultures

### Performance Considerations

- Cache localized resources to avoid repeated lookups
- Use compiled resource files (.resources.dll)
- Consider CDN for static localized content
- Minimize culture switches during user session
- Pre-load common resources at startup
- Use lazy loading for large resource sets

## Testing

Example unit tests for globalization:

```csharp
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

public class RouteDataRequestCultureProviderTests
{
    [Fact]
    public async Task DetermineProviderCultureResult_WithValidCulture_ReturnsCulture()
    {
        // Arrange
        var options = Options.Create(new GlobalizationOptions
        {
            DefaultCulture = "en-US",
            SupportedCultures = new[] { "en-US", "nl-NL" }
        });

        var provider = new RouteDataRequestCultureProvider(options);
        var context = new DefaultHttpContext();
        context.Request.Path = "/nl-NL/products";

        // Act
        var result = await provider.DetermineProviderCultureResult(context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("nl-NL", result.Cultures.First().Value);
    }

    [Fact]
    public async Task DetermineProviderCultureResult_WithInvalidCulture_ReturnsDefault()
    {
        // Arrange
        var options = Options.Create(new GlobalizationOptions
        {
            DefaultCulture = "en-US",
            SupportedCultures = new[] { "en-US", "nl-NL" }
        });

        var provider = new RouteDataRequestCultureProvider(options);
        var context = new DefaultHttpContext();
        context.Request.Path = "/invalid/products";

        // Act
        var result = await provider.DetermineProviderCultureResult(context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("en-US", result.Cultures.First().Value);
    }
}
```

## Dependencies

- **Microsoft.AspNetCore.Localization** - ASP.NET Core localization middleware
- **Microsoft.AspNetCore.Routing** - Routing infrastructure
- **Microsoft.Extensions.Localization** - Localization abstractions
- **ISynergy.Framework.Core** - Core framework utilities

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.AspNetCore** - Base ASP.NET Core integration
- **I-Synergy.Framework.AspNetCore.Blazor** - Blazor integration
- **I-Synergy.Framework.UI** - UI localization support

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

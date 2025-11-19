# I-Synergy Framework ASP.NET Core

A comprehensive ASP.NET Core integration library for .NET 10.0 applications, providing middleware, filters, extensions, and utilities for building modern web APIs and applications. This package includes OpenTelemetry integration, health checks, global exception handling, result pattern extensions, and more.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.AspNetCore.svg)](https://www.nuget.org/packages/I-Synergy.Framework.AspNetCore/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **OpenTelemetry integration** for distributed tracing, metrics, and logging
- **Global exception handling** with ProblemDetails responses
- **Health check endpoints** with detailed JSON responses
- **Result pattern extensions** for converting Result<T> to IActionResult
- **Action filters** for validation, caching, and request filtering
- **Model binders** for custom DateTime parsing
- **HTTP extensions** for request/response manipulation
- **JWT token service** abstractions for authentication
- **CORS configuration** with environment-aware options
- **Service locator** integration for legacy scenarios
- **View model base classes** for MVC applications

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.AspNetCore
```

## Quick Start

### 1. Configure Services with OpenTelemetry

```csharp
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add info service for application metadata
builder.Services.AddSingleton<IInfoService>(new InfoService(
    productName: "My API",
    productVersion: new Version(1, 0, 0)));

// Configure OpenTelemetry
builder.Logging.AddTelemetry(
    builder,
    builder.Services.BuildServiceProvider().GetRequiredService<IInfoService>(),
    tracerProviderBuilder =>
    {
        // Add custom instrumentation
        tracerProviderBuilder.AddSource("MyApi");
    },
    meterProviderBuilder =>
    {
        // Add custom metrics
        meterProviderBuilder.AddMeter("MyApi");
    });

// Add controllers and services
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck("api", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Configure middleware pipeline
app.UseExceptionHandler();
app.MapDefaultHealthEndpoints();
app.MapControllers();

app.Run();
```

### 2. Use Result Pattern in Controllers

```csharp
using ISynergy.Framework.Core.Models.Results;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        // Convert Result<Product> to IActionResult
        return result.Match<Product, IActionResult>(
            value => value is not null ? Ok(value) : NoContent(),
            () => NotFound()
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto dto)
    {
        var result = await _productService.CreateProductAsync(dto);

        return result.Match<int, IActionResult>(
            id => CreatedAtAction(nameof(GetProduct), new { id }, id),
            () => BadRequest(result.ErrorMessage)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetProductsPagedAsync(pageIndex, pageSize);

        return result.Match<PaginatedResult<ProductDto>, IActionResult>(
            paginatedResult => Ok(paginatedResult),
            () => NotFound()
        );
    }
}
```

### 3. Add Action Filters

```csharp
using ISynergy.Framework.AspNetCore.Filters;

[ApiController]
[Route("api/[controller]")]
[ValidateModelFilter] // Validates ModelState automatically
public class OrdersController : ControllerBase
{
    [HttpPost]
    [NoCache] // Prevents caching of this endpoint
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto order)
    {
        // ModelState is already validated by ValidateModelFilter
        // Response will not be cached due to NoCacheFilter

        await _orderService.CreateOrderAsync(order);
        return Ok();
    }

    [HttpGet("local-only")]
    [RequestShouldBeLocalFilter] // Only allows requests from localhost
    public IActionResult GetSensitiveData()
    {
        return Ok(new { Secret = "This is sensitive" });
    }
}
```

### 4. Configure Global Exception Handling

```csharp
var app = builder.Build();

// Add global exception handler
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exceptionHandler = context.RequestServices
            .GetRequiredService<IExceptionHandler>();

        await exceptionHandler.TryHandleAsync(
            context,
            context.Features.Get<IExceptionHandlerFeature>()?.Error!,
            context.RequestAborted);
    });
});

// Or register the built-in GlobalExceptionHandler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
app.UseExceptionHandler();
```

## Architecture

### Core Components

```
ISynergy.Framework.AspNetCore/
├── Extensions/                 # Extension methods
│   ├── TelemetryExtensions    # OpenTelemetry configuration
│   ├── HealthCheckExtensions  # Health check endpoints
│   ├── ResultExtensions       # Result pattern conversions
│   ├── HttpContextExtensions  # HttpContext utilities
│   └── WebApplicationExtensions
│
├── Filters/                   # Action filters
│   ├── ValidateModelFilterAttribute
│   ├── NoCacheFilterAttribute
│   ├── NullResultFilterAttribute
│   ├── NoNullModelsFilterAttribute
│   └── RequestShouldBeLocalFilterAttribute
│
├── Handlers/                  # Exception handlers
│   └── GlobalExceptionHandler
│
├── Binders/                   # Model binders
│   ├── DateTimeModelBinder
│   └── DateTimeModelBinderProvider
│
├── Abstractions/              # Service interfaces
│   └── IJwtTokenService
│
└── Options/                   # Configuration options
    ├── CORSOptions
    ├── KeyVaultOptions
    ├── AzureOptions
    └── TelemetryOptions
```

## Core Features

### OpenTelemetry Integration

Configure comprehensive observability for your application:

```csharp
using ISynergy.Framework.AspNetCore.Extensions;

builder.Logging.AddTelemetry(
    builder,
    infoService,
    tracerProviderBuilder =>
    {
        // Add custom sources
        tracerProviderBuilder
            .AddSource("MyApi")
            .AddSource("MyApi.Database");

        // Add exporters
        if (builder.Environment.IsProduction())
        {
            tracerProviderBuilder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://otel-collector:4317");
            });
        }
    },
    meterProviderBuilder =>
    {
        // Add custom meters
        meterProviderBuilder.AddMeter("MyApi.Metrics");

        // Add exporters
        if (builder.Environment.IsProduction())
        {
            meterProviderBuilder.AddOtlpExporter();
        }
    },
    loggerProviderBuilder =>
    {
        // Configure logging
        if (builder.Environment.IsProduction())
        {
            loggerProviderBuilder.AddOtlpExporter();
        }
    });
```

### Health Check Endpoints

Configure detailed health checks with JSON responses:

```csharp
using ISynergy.Framework.AspNetCore.Extensions;

// Configure health checks
builder.Services.AddHealthChecks()
    .AddCheck("database", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck("external-api", () => HealthCheckResult.Healthy())
    .AddCheck("cache", () => HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

// Map default health endpoints
app.MapDefaultHealthEndpoints();
// Creates:
// - /health (all checks must pass)
// - /alive (only checks tagged with "live" must pass)

// Or use custom health check endpoint with telemetry
app.MapTelemetryHealthChecks("/healthz");
```

Example health check response:

```json
{
  "Status": "Healthy",
  "Duration": "00:00:00.0234567",
  "Info": [
    {
      "Key": "database",
      "Description": "Database connection is healthy",
      "Status": "Healthy",
      "Duration": "00:00:00.0123456",
      "Data": {}
    }
  ]
}
```

### Action Filters

#### Model Validation Filter

Automatically validate ModelState:

```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateModelFilter]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        // If ModelState is invalid, filter returns BadRequest automatically
        // No need to check ModelState.IsValid here
        return Ok();
    }
}
```

#### No Cache Filter

Prevent caching of responses:

```csharp
[HttpGet("real-time-data")]
[NoCache]
public IActionResult GetRealTimeData()
{
    // Response headers will include:
    // Cache-Control: no-store, no-cache, must-revalidate
    // Pragma: no-cache
    // Expires: 0
    return Ok(new { Timestamp = DateTime.UtcNow });
}
```

#### Local Request Filter

Restrict endpoints to localhost only:

```csharp
[HttpGet("admin/diagnostics")]
[RequestShouldBeLocalFilter]
public IActionResult GetDiagnostics()
{
    // Returns 403 Forbidden if request is not from localhost
    return Ok(GetSystemDiagnostics());
}
```

#### Null Result Filter

Handle null results automatically:

```csharp
[HttpGet("{id}")]
[NullResultFilter] // Returns 404 if result is null
public async Task<Product?> GetProduct(int id)
{
    return await _productService.GetProductByIdAsync(id);
    // If null, filter automatically returns 404 Not Found
}
```

### HTTP Extensions

#### HttpContext Extensions

```csharp
using ISynergy.Framework.AspNetCore.Extensions;

public class MyMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Get client IP address
        var ipAddress = context.GetClientIpAddress();

        // Check if request is from localhost
        if (context.IsLocalRequest())
        {
            // Handle local request
        }

        // Get user agent
        var userAgent = context.Request.GetUserAgent();

        await _next(context);
    }
}
```

#### HttpRequest Extensions

```csharp
using ISynergy.Framework.AspNetCore.Extensions;

public IActionResult GetRequestInfo()
{
    var request = HttpContext.Request;

    var info = new
    {
        IsAjax = request.IsAjaxRequest(),
        IsMobile = request.IsMobileRequest(),
        UserAgent = request.GetUserAgent(),
        ContentType = request.ContentType
    };

    return Ok(info);
}
```

### Custom DateTime Model Binder

Handle DateTime parsing with custom formats:

```csharp
using ISynergy.Framework.AspNetCore.Providers;

// In Program.cs
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
});

// In Controller
[HttpGet]
public IActionResult GetOrders(
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
{
    // Supports multiple date formats automatically
    // - ISO 8601: 2024-01-15T10:30:00Z
    // - Custom formats: 01/15/2024
    return Ok();
}
```

## Advanced Features

### Result Pattern HTTP Extensions

Convert HttpResponseMessage to Result objects:

```csharp
using ISynergy.Framework.AspNetCore.Extensions;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public async Task<Result<Product>> GetProductAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}");

        // Convert HttpResponseMessage to Result<Product>
        var result = await response.ToResult<Product>();

        return result ?? Result<Product>.Fail("Invalid response");
    }

    public async Task<PaginatedResult<Product>> GetProductsAsync(int page)
    {
        var response = await _httpClient.GetAsync($"api/products?page={page}");

        // Convert to PaginatedResult
        var result = await response.ToPaginatedResult<Product>();

        return result ?? PaginatedResult<Product>.Empty;
    }
}
```

### JWT Token Service

Implement JWT token generation and validation:

```csharp
using ISynergy.Framework.AspNetCore.Abstractions.Services;
using ISynergy.Framework.Core.Models;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Token GenerateJwtToken(TokenRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Email, request.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new Token
        {
            AccessToken = tokenString,
            ExpiresIn = 28800,
            TokenType = "Bearer"
        };
    }

    public List<Claim> GetClaims(Token token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token.AccessToken);
        return jwtToken.Claims.ToList();
    }

    public string GetSingleClaim(Token token, string claimType)
    {
        var claims = GetClaims(token);
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
    }
}

// Register in DI
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
```

### CORS Configuration

Configure CORS with environment-aware settings:

```csharp
using ISynergy.Framework.AspNetCore.Options;

// In appsettings.json
{
  "CORS": {
    "AllowedOrigins": ["https://myapp.com", "https://admin.myapp.com"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["*"],
    "AllowCredentials": true
  }
}

// In Program.cs
var corsOptions = builder.Configuration
    .GetSection("CORS")
    .Get<CORSOptions>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOptions.AllowedOrigins.ToArray())
            .WithMethods(corsOptions.AllowedMethods.ToArray())
            .WithHeaders(corsOptions.AllowedHeaders.ToArray());

        if (corsOptions.AllowCredentials)
            policy.AllowCredentials();
    });
});

app.UseCors();
```

### Service Locator Integration

Use service locator for legacy scenarios:

```csharp
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.Core.Locators;

var app = builder.Build();

// Set service locator provider
app.SetLocatorProvider();

// Now you can use ServiceLocator anywhere
public class LegacyService
{
    public void DoWork()
    {
        var logger = ServiceLocator.Default.GetInstance<ILogger>();
        logger.LogInformation("Working...");
    }
}
```

## Best Practices

> [!TIP]
> Use **Result pattern extensions** to maintain consistent API responses and error handling.

> [!IMPORTANT]
> Always configure **global exception handling** to prevent sensitive error details from leaking to clients.

> [!NOTE]
> OpenTelemetry integration automatically instruments ASP.NET Core, HttpClient, and runtime metrics.

### API Design

- Use Result<T> pattern for all service methods
- Return appropriate HTTP status codes (200, 201, 204, 400, 404, 500)
- Use ProblemDetails for error responses
- Implement health check endpoints for monitoring
- Use action filters to avoid repetitive validation code

### Security

- Always validate input using [ValidateModelFilter] or manual validation
- Use [RequestShouldBeLocalFilter] for administrative endpoints
- Implement JWT token authentication for protected endpoints
- Configure CORS appropriately for your environment
- Never expose internal exception details in production

### Performance

- Use [NoCache] filter sparingly - only for real-time data
- Implement health checks that don't impact application performance
- Use async/await throughout the application
- Configure response compression for API endpoints
- Monitor telemetry data to identify bottlenecks

### Observability

- Configure OpenTelemetry early in application startup
- Tag health checks appropriately (live, ready)
- Use structured logging with proper log levels
- Instrument custom operations with ActivitySource
- Monitor metrics for request duration, error rates, and throughput

## Testing

The framework is designed for testability:

```csharp
[TestClass]
public class ProductsControllerTests
{
    [TestMethod]
    public async Task GetProduct_ReturnsOk_WhenProductExists()
    {
        // Arrange
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(Result<Product>.Success(new Product { Id = 1 }));

        var controller = new ProductsController(mockService.Object);

        // Act
        var result = await controller.GetProduct(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.GetProductByIdAsync(999))
            .ReturnsAsync(Result<Product>.Fail("Not found"));

        var controller = new ProductsController(mockService.Object);

        // Act
        var result = await controller.GetProduct(999);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}
```

### Integration Testing

```csharp
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }
}
```

## Dependencies

- **ISynergy.Framework.Core** - Core framework components
- **ISynergy.Framework.OpenTelemetry** - OpenTelemetry integration
- **ISynergy.Framework.Storage** - Storage abstractions
- **Microsoft.AspNetCore.App** - ASP.NET Core framework
- **OpenTelemetry.Instrumentation.AspNetCore** - ASP.NET Core instrumentation
- **OpenTelemetry.Instrumentation.Http** - HttpClient instrumentation
- **OpenTelemetry.Instrumentation.Runtime** - Runtime metrics

## Configuration Examples

### Complete Startup Configuration

```csharp
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Handlers;
using ISynergy.Framework.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers(options =>
{
    // Add model binders
    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());

    // Add global filters
    options.Filters.Add<ValidateModelFilterAttribute>();
});

// Add OpenTelemetry
var infoService = new InfoService("MyApi", new Version(1, 0, 0));
builder.Services.AddSingleton<IInfoService>(infoService);

builder.Logging.AddTelemetry(builder, infoService);

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

// Add exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add CORS
builder.Services.AddCors();

var app = builder.Build();

// Configure middleware pipeline
app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultHealthEndpoints();
app.MapControllers();

app.SetLocatorProvider();

app.Run();
```

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.CQRS** - CQRS implementation
- **I-Synergy.Framework.EntityFramework** - Entity Framework integration
- **I-Synergy.Framework.OpenTelemetry** - OpenTelemetry utilities

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

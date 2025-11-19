# I-Synergy Framework AspNetCore Multi-Tenancy

Multi-tenant architecture support for ASP.NET Core applications. This package provides tenant resolution from HTTP context, tenant service for managing tenant-specific operations, and integration with OpenIddict claims for secure multi-tenant applications.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.AspNetCore.MultiTenancy.svg)](https://www.nuget.org/packages/I-Synergy.Framework.AspNetCore.MultiTenancy/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Tenant resolution** from HTTP context and claims
- **Tenant service** for accessing current tenant information
- **OpenIddict claims integration** for secure tenant identification
- **HttpContext accessor** for tenant management across request scope
- **User and tenant isolation** with automatic group assignment
- **Programmatic tenant setting** for testing and background jobs
- **Thread-safe tenant access** throughout the application
- **Support for client roles** in multi-tenant scenarios

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.AspNetCore.MultiTenancy
```

## Quick Start

### 1. Configure Multi-Tenancy Services

In your `Program.cs`:

```csharp
using ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add multi-tenancy support
builder.Services.AddMultiTenancyIntegration();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

### 2. Using the Tenant Service

Access tenant information in your controllers and services:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IOrderRepository _orderRepository;

    public OrdersController(
        ITenantService tenantService,
        IOrderRepository orderRepository)
    {
        _tenantService = tenantService;
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        // Automatically filter by current tenant
        var tenantId = _tenantService.TenantId;
        var orders = await _orderRepository.GetByTenantAsync(tenantId);

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            TenantId = _tenantService.TenantId,
            CreatedBy = _tenantService.UserName,
            // ... other properties
        };

        await _orderRepository.CreateAsync(order);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        // Verify tenant ownership
        if (order == null || order.TenantId != _tenantService.TenantId)
            return NotFound();

        return Ok(order);
    }
}
```

### 3. Setting Tenant Programmatically

For testing or background jobs, set the tenant context manually:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class BackgroundJobService
{
    private readonly ITenantService _tenantService;

    public BackgroundJobService(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task ProcessTenantDataAsync(Guid tenantId, string username)
    {
        // Set tenant context for this operation
        _tenantService.SetTenant(tenantId, username);

        // Now all operations will use this tenant context
        await ProcessDataAsync();
    }

    public async Task ProcessWithMinimalContextAsync(Guid tenantId)
    {
        // Set only tenant ID (no username)
        _tenantService.SetTenant(tenantId);

        await ProcessDataAsync();
    }
}
```

## Core Components

### Services

```
ISynergy.Framework.AspNetCore.MultiTenancy.Services/
└── TenantService                 # Tenant resolution and management
```

### Extensions

```
ISynergy.Framework.AspNetCore.MultiTenancy.Extensions/
├── ServiceCollectionExtensions   # DI configuration
└── HostApplicationBuilderExtensions  # Builder configuration
```

## Advanced Features

### Tenant-Aware Entity Framework

Automatically filter queries by tenant:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add global query filter for multi-tenancy
        modelBuilder.Entity<Order>()
            .HasQueryFilter(e => e.TenantId == _tenantService.TenantId);

        modelBuilder.Entity<Product>()
            .HasQueryFilter(e => e.TenantId == _tenantService.TenantId);
    }

    public override int SaveChanges()
    {
        ApplyTenantId();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantId();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantId()
    {
        var tenantId = _tenantService.TenantId;

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = tenantId;
            }
        }
    }
}

// Tenant entity interface
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}

// Example entity
public class Order : ITenantEntity
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public string OrderNumber { get; set; }
    public decimal Amount { get; set; }
}
```

### Tenant Resolution Middleware

Create custom middleware for tenant resolution:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // Option 1: From subdomain
        var host = context.Request.Host.Host;
        var subdomain = host.Split('.').FirstOrDefault();

        if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
        {
            var tenantId = await ResolveTenantFromSubdomainAsync(subdomain);
            if (tenantId.HasValue)
            {
                tenantService.SetTenant(tenantId.Value);
            }
        }

        // Option 2: From custom header
        if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeader))
        {
            if (Guid.TryParse(tenantHeader, out var tenantId))
            {
                tenantService.SetTenant(tenantId);
            }
        }

        // Option 3: Already set from claims by the framework
        // No action needed - TenantService reads from User claims

        await _next(context);
    }

    private async Task<Guid?> ResolveTenantFromSubdomainAsync(string subdomain)
    {
        // Lookup tenant by subdomain
        // Return tenant ID or null
        return Guid.Empty; // Placeholder
    }
}

// Register middleware
app.UseMiddleware<TenantResolutionMiddleware>();
```

### Tenant-Specific Configuration

Manage tenant-specific settings:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class TenantConfigurationService
{
    private readonly ITenantService _tenantService;
    private readonly IConfiguration _configuration;

    public TenantConfigurationService(
        ITenantService tenantService,
        IConfiguration configuration)
    {
        _tenantService = tenantService;
        _configuration = configuration;
    }

    public string GetTenantSetting(string key)
    {
        var tenantId = _tenantService.TenantId;
        var tenantKey = $"Tenants:{tenantId}:{key}";

        return _configuration[tenantKey] ?? _configuration[key];
    }

    public T GetTenantSetting<T>(string key, T defaultValue = default)
    {
        var value = GetTenantSetting(key);

        if (string.IsNullOrEmpty(value))
            return defaultValue;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}

// appsettings.json
{
  "Tenants": {
    "123e4567-e89b-12d3-a456-426614174000": {
      "MaxUsers": 100,
      "StorageQuotaGB": 50,
      "Features": ["AdvancedReporting", "API"]
    },
    "987fcdeb-51a2-43f1-9876-543210fedcba": {
      "MaxUsers": 10,
      "StorageQuotaGB": 5,
      "Features": ["BasicReporting"]
    }
  }
}
```

### Tenant Isolation Validation

Ensure tenant isolation in critical operations:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class TenantIsolationValidator
{
    private readonly ITenantService _tenantService;

    public TenantIsolationValidator(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public void ValidateTenantAccess<T>(T entity) where T : ITenantEntity
    {
        if (entity.TenantId != _tenantService.TenantId)
        {
            throw new UnauthorizedAccessException(
                $"Access denied. Entity belongs to different tenant.");
        }
    }

    public void ValidateTenantAccess(Guid tenantId)
    {
        if (tenantId != _tenantService.TenantId)
        {
            throw new UnauthorizedAccessException(
                $"Access denied. Operation for different tenant.");
        }
    }
}

// Usage in service
public class OrderService
{
    private readonly ITenantService _tenantService;
    private readonly TenantIsolationValidator _validator;
    private readonly IOrderRepository _repository;

    public OrderService(
        ITenantService tenantService,
        TenantIsolationValidator validator,
        IOrderRepository repository)
    {
        _tenantService = tenantService;
        _validator = validator;
        _repository = repository;
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _repository.GetByIdAsync(orderId);

        if (order == null)
            throw new NotFoundException("Order not found");

        // Validate tenant isolation
        _validator.ValidateTenantAccess(order);

        await _repository.DeleteAsync(order);
    }
}
```

## Usage Examples

### Multi-Tenant SaaS Application

Complete example of a multi-tenant SaaS application:

```csharp
using ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add multi-tenancy
builder.Services.AddMultiTenancyIntegration();

// Add authentication with OpenIddict
builder.Services.AddAuthentication("OpenIddict.Validation.AspNetCore")
    .AddOpenIddictValidation(options =>
    {
        options.SetIssuer("https://auth.example.com");
    });

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// Customer Controller
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(
        ITenantService tenantService,
        ICustomerRepository customerRepository)
    {
        _tenantService = tenantService;
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        // Automatically scoped to current tenant
        var customers = await _customerRepository
            .GetAllAsync(_tenantService.TenantId);

        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            TenantId = _tenantService.TenantId,
            Name = request.Name,
            Email = request.Email,
            CreatedBy = _tenantService.UserName,
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.CreateAsync(customer);

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        // Verify tenant ownership
        if (customer == null || customer.TenantId != _tenantService.TenantId)
            return NotFound();

        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null || customer.TenantId != _tenantService.TenantId)
            return NotFound();

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.UpdatedBy = _tenantService.UserName;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);

        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null || customer.TenantId != _tenantService.TenantId)
            return NotFound();

        await _customerRepository.DeleteAsync(customer);

        return NoContent();
    }
}
```

### Background Job with Tenant Context

Process tenant-specific background jobs:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class TenantReportGenerator
{
    private readonly ITenantService _tenantService;
    private readonly IReportService _reportService;
    private readonly IEmailService _emailService;

    public TenantReportGenerator(
        ITenantService tenantService,
        IReportService reportService,
        IEmailService emailService)
    {
        _tenantService = tenantService;
        _reportService = reportService;
        _emailService = emailService;
    }

    public async Task GenerateMonthlyReportsAsync()
    {
        var tenants = await GetAllTenantsAsync();

        foreach (var tenant in tenants)
        {
            // Set tenant context for this iteration
            _tenantService.SetTenant(tenant.Id, "system");

            try
            {
                // Generate report (automatically scoped to tenant)
                var report = await _reportService.GenerateMonthlyReportAsync();

                // Send email to tenant administrators
                await _emailService.SendReportAsync(tenant.AdminEmail, report);
            }
            catch (Exception ex)
            {
                // Log error for this tenant
                Console.WriteLine($"Error generating report for tenant {tenant.Id}: {ex.Message}");
            }
        }
    }
}
```

### Integration Testing with Tenant Context

Test multi-tenant features:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class CustomerControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomerControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCustomers_ReturnOnlyTenantCustomers()
    {
        // Arrange
        var tenant1Id = Guid.NewGuid();
        var tenant2Id = Guid.NewGuid();

        var client = _factory.CreateClient();

        // Seed data for two tenants
        await SeedCustomersAsync(tenant1Id, 5);
        await SeedCustomersAsync(tenant2Id, 3);

        // Act - Request with tenant1 context
        var response = await client.GetAsync($"/api/customers");

        // Assert
        response.EnsureSuccessStatusCode();
        var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

        Assert.Equal(5, customers.Count);
        Assert.All(customers, c => Assert.Equal(tenant1Id, c.TenantId));
    }

    [Fact]
    public async Task CreateCustomer_AssignCurrentTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", new
        {
            Name = "Test Customer",
            Email = "test@example.com"
        });

        // Assert
        response.EnsureSuccessStatusCode();
        var customer = await response.Content.ReadFromJsonAsync<Customer>();

        Assert.Equal(tenantId, customer.TenantId);
    }
}
```

## Best Practices

> [!TIP]
> Use **global query filters** in Entity Framework to automatically scope queries to the current tenant.

> [!IMPORTANT]
> Always **validate tenant ownership** before modifying or deleting entities to prevent cross-tenant data access.

> [!NOTE]
> Set the tenant context **early in the request pipeline** (authentication/authorization middleware) for consistent behavior.

### Tenant Resolution

- Resolve tenant from claims when using authentication
- Use subdomain or custom headers for public APIs
- Set tenant context in middleware for consistency
- Cache tenant information to avoid repeated lookups
- Handle missing or invalid tenant gracefully
- Log tenant context for audit trails

### Data Isolation

- Use global query filters in Entity Framework
- Always validate tenant ownership before operations
- Implement tenant checks in repository layer
- Use database schemas or separate databases for strict isolation
- Audit cross-tenant access attempts
- Test tenant isolation thoroughly

### Security

- Never trust tenant ID from client input
- Always validate tenant from authenticated claims
- Implement role-based access within tenants
- Monitor suspicious cross-tenant access patterns
- Use separate connection strings for high-security tenants
- Encrypt tenant-specific sensitive data

### Performance

- Cache tenant configuration and settings
- Use indexes on TenantId columns
- Consider database-per-tenant for large tenants
- Implement tenant-aware caching strategies
- Monitor query performance across tenants
- Optimize global query filters

## Testing

Example unit tests for multi-tenancy:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

public class TenantServiceTests
{
    [Fact]
    public void TenantId_WithValidClaim_ReturnsTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var claims = new[]
        {
            new Claim(Claims.KeyId, tenantId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        var tenantService = new TenantService(httpContextAccessor.Object);

        // Act
        var result = tenantService.TenantId;

        // Assert
        Assert.Equal(tenantId, result);
    }

    [Fact]
    public void SetTenant_SetsClaimsPrincipal()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var username = "testuser";

        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        var tenantService = new TenantService(httpContextAccessor.Object);

        // Act
        tenantService.SetTenant(tenantId, username);

        // Assert
        Assert.Equal(tenantId.ToString(), httpContext.User.FindFirst(Claims.KeyId)?.Value);
        Assert.Equal(username, httpContext.User.FindFirst(Claims.Username)?.Value);
    }
}
```

## Dependencies

- **Microsoft.AspNetCore.Http** - HTTP context abstractions
- **OpenIddict.Abstractions** - OAuth 2.0/OpenID Connect claims
- **ISynergy.Framework.Core** - Core framework utilities

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.AspNetCore** - Base ASP.NET Core integration
- **I-Synergy.Framework.AspNetCore.Authentication** - Authentication utilities
- **I-Synergy.Framework.EntityFramework** - Entity Framework with tenant support
- **I-Synergy.Framework.AspNetCore.Monitoring** - SignalR monitoring with tenant isolation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

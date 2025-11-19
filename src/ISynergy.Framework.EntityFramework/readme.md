# I-Synergy Framework Entity Framework

A comprehensive Entity Framework Core integration library for .NET 10.0 applications, providing base entity classes, multi-tenancy support, soft delete capabilities, automatic auditing, and powerful repository extensions. This package streamlines database operations while promoting clean architecture principles.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.EntityFramework.svg)](https://www.nuget.org/packages/I-Synergy.Framework.EntityFramework/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Base entity classes** with automatic audit fields (CreatedDate, CreatedBy, ChangedDate, ChangedBy)
- **Multi-tenancy support** with automatic tenant filtering using query filters
- **Soft delete functionality** with query filter integration
- **DbContext extensions** for common CRUD operations with Mapster integration
- **ModelBuilder extensions** for configuring versioning, decimal precision, and filters
- **Queryable extensions** for pagination support
- **Entity events** for tracking inserts, updates, and deletes
- **Attribute-based filtering** to selectively ignore soft delete or versioning
- **Identity integration** with AspNetCore.Identity.EntityFrameworkCore
- **Type-safe operations** with full async/await support

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.EntityFramework
```

## Quick Start

### 1. Create Entity Classes

```csharp
using ISynergy.Framework.EntityFramework.Base;

// Simple entity with audit fields
public class Product : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

// Multi-tenant entity with automatic tenant filtering
public class Order : BaseTenantEntity
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    // TenantId is inherited from BaseTenantEntity
}

// Entity that ignores soft delete
[IgnoreSoftDelete]
public class AuditLog : BaseEntity
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
```

### 2. Configure DbContext

```csharp
using ISynergy.Framework.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    private readonly IScopedContextService _contextService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IScopedContextService contextService)
        : base(options)
    {
        _contextService = contextService;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply decimal precision for currency fields
        modelBuilder.ApplyDecimalPrecision("decimal(18, 2)");

        // Apply versioning (automatic Version field management)
        modelBuilder.ApplyVersioning();

        // Apply soft delete filters (excludes IsDeleted = true)
        modelBuilder.ApplySoftDeleteFilters();

        // Apply tenant filters for multi-tenant entities
        modelBuilder.ApplyTenantFilters(() =>
            _contextService.GetContext<AppContext>().TenantId);

        // Apply entity configurations from assemblies
        modelBuilder.ApplyModelBuilderConfigurations(new[]
        {
            typeof(ApplicationDbContext).Assembly
        });
    }
}
```

### 3. Use DbContext Extensions

```csharp
using ISynergy.Framework.EntityFramework.Extensions;

public class ProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Check if product exists
    public async Task<bool> ProductExistsAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.ExistsAsync<Product>(
            p => p.Name == name,
            cancellationToken);
    }

    // Get product by ID
    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.GetItemByIdAsync<Product, int>(id, cancellationToken);
    }

    // Get product as DTO
    public async Task<ProductDto?> GetProductDtoAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.GetItemByIdAsync<Product, ProductDto, int>(
            id,
            cancellationToken);
    }

    // Add product
    public async Task<int> AddProductAsync(ProductDto dto, CancellationToken cancellationToken)
    {
        return await _context.AddItemAsync<Product, ProductDto>(dto, cancellationToken);
    }

    // Update product
    public async Task<int> UpdateProductAsync(ProductDto dto, CancellationToken cancellationToken)
    {
        return await _context.UpdateItemAsync<Product, ProductDto>(dto, cancellationToken);
    }

    // Add or update product (upsert)
    public async Task<int> UpsertProductAsync(ProductDto dto, CancellationToken cancellationToken)
    {
        return await _context.AddUpdateItemAsync<Product, ProductDto>(
            dto,
            cancellationToken);
    }

    // Soft delete product
    public async Task<int> SoftDeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.RemoveItemAsync<Product, int>(
            id,
            cancellationToken,
            soft: true);
    }

    // Hard delete product
    public async Task<int> DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.RemoveItemAsync<Product, int>(
            id,
            cancellationToken,
            soft: false);
    }
}
```

## Architecture

### Base Entity Classes

```
ISynergy.Framework.EntityFramework.Base/
├── BaseEntity                  # Base class with audit fields
│   ├── Memo                   # Optional memo field
│   ├── CreatedDate            # Creation timestamp
│   ├── CreatedBy              # User who created the record
│   ├── ChangedDate            # Last modification timestamp
│   └── ChangedBy              # User who last modified the record
│
└── BaseTenantEntity           # BaseEntity + TenantId
    └── TenantId               # Tenant identifier for multi-tenancy
```

### Extension Methods

```
ISynergy.Framework.EntityFramework.Extensions/
├── DbContextExtensions         # CRUD operations
│   ├── ExistsAsync<TEntity>
│   ├── GetItemByIdAsync<TEntity, TId>
│   ├── GetItemByIdAsync<TEntity, TModel, TId>
│   ├── AddItemAsync<TEntity, TModel>
│   ├── UpdateItemAsync<TEntity, TModel>
│   ├── AddUpdateItemAsync<TEntity, TModel>
│   └── RemoveItemAsync<TEntity, TId>
│
├── ModelBuilderExtensions      # Configuration helpers
│   ├── ApplyDecimalPrecision
│   ├── ApplyVersioning
│   ├── ApplySoftDeleteFilters
│   ├── ApplyTenantFilters
│   └── ApplyModelBuilderConfigurations
│
└── QueryableExtensions         # Query helpers
    ├── CountPages<TEntity>
    └── ToPage<TEntity>
```

## Core Features

### Multi-Tenancy Support

Automatically filter queries by tenant ID:

```csharp
// Define tenant-aware entity
[TenantAware(nameof(TenantId))]
public class Invoice : BaseTenantEntity
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

// Configure tenant filtering in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Get tenant ID from scoped context
    modelBuilder.ApplyTenantFilters(() =>
        _contextService.GetContext<AppContext>().TenantId);
}

// All queries automatically filter by tenant
public async Task<List<Invoice>> GetInvoicesAsync()
{
    // Only returns invoices for the current tenant
    return await _context.Invoices.ToListAsync();
}
```

### Soft Delete

Mark records as deleted without removing them from the database:

```csharp
// Enable soft delete filtering
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplySoftDeleteFilters();
}

// Soft delete a record
await _context.RemoveItemAsync<Product, int>(productId, cancellationToken, soft: true);

// Excluded from queries automatically
var products = await _context.Products.ToListAsync(); // Doesn't include soft-deleted

// Optionally ignore soft delete for specific entities
[IgnoreSoftDelete]
public class ArchivedOrder : BaseEntity
{
    // This entity will not use soft delete filtering
}
```

### Automatic Versioning

Track entity versions for optimistic concurrency:

```csharp
// Configure versioning
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyVersioning(); // Default version = 1
}

// Entities automatically get a Version property
var product = await _context.Products.FindAsync(1);
Console.WriteLine(product.Version); // 1

// Version increments on update
product.Name = "Updated Name";
await _context.SaveChangesAsync();
Console.WriteLine(product.Version); // 2

// Ignore versioning for specific entities
[IgnoreVersioning]
public class LogEntry : BaseEntity
{
    // No version tracking
}
```

### Pagination

Easily implement pagination in queries:

```csharp
using ISynergy.Framework.EntityFramework.Extensions;

public async Task<PagedResult<ProductDto>> GetProductsPagedAsync(
    int pageIndex,
    int pageSize,
    CancellationToken cancellationToken)
{
    var query = _context.Products
        .Where(p => p.IsActive)
        .OrderBy(p => p.Name);

    var totalPages = query.CountPages(pageSize);
    var totalItems = await query.CountAsync(cancellationToken);

    var items = await query
        .ToPage(pageIndex, pageSize)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })
        .ToListAsync(cancellationToken);

    return new PagedResult<ProductDto>
    {
        Items = items,
        PageIndex = pageIndex,
        PageSize = pageSize,
        TotalPages = totalPages,
        TotalItems = totalItems
    };
}
```

## Advanced Features

### Custom Entity Configuration

Use IEntityTypeConfiguration for complex mappings:

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18, 2)");

        builder.HasIndex(p => p.Category);
    }
}

// Apply configurations
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyModelBuilderConfigurations(new[]
    {
        typeof(ApplicationDbContext).Assembly,
        typeof(Product).Assembly
    });
}
```

### Entity Events

Track entity changes for auditing or notifications:

```csharp
using ISynergy.Framework.EntityFramework.Events;

public class EntityEventListener
{
    private readonly IMessengerService _messenger;

    public EntityEventListener(IMessengerService messenger)
    {
        _messenger = messenger;

        // Subscribe to entity events
        _messenger.Register<EntityInsertedEvent<Product>>(this, OnProductInserted);
        _messenger.Register<EntityUpdatedEvent<Product>>(this, OnProductUpdated);
        _messenger.Register<EntityDeletedEvent<Product>>(this, OnProductDeleted);
    }

    private void OnProductInserted(EntityInsertedEvent<Product> evt)
    {
        // Handle product inserted
        Console.WriteLine($"Product {evt.Entity.Name} was inserted");
    }

    private void OnProductUpdated(EntityUpdatedEvent<Product> evt)
    {
        // Handle product updated
        Console.WriteLine($"Product {evt.Entity.Name} was updated");
    }

    private void OnProductDeleted(EntityDeletedEvent<Product> evt)
    {
        // Handle product deleted
        Console.WriteLine($"Product was deleted");
    }
}
```

### Combining Filters

Combine multiple query filters (tenant + soft delete):

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Soft delete filter
    modelBuilder.ApplySoftDeleteFilters();

    // Tenant filter (combines with soft delete automatically)
    modelBuilder.ApplyTenantFilters(() =>
        _contextService.GetContext<AppContext>().TenantId);

    // Queries now filter by both tenant and soft delete
    // SELECT * FROM Products WHERE TenantId = @tenantId AND IsDeleted = 0
}
```

### Working with Mapster

The library uses Mapster for object mapping:

```csharp
// Define DTOs
public class ProductDto : BaseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Configure Mapster mapping (if needed)
TypeAdapterConfig<Product, ProductDto>.NewConfig()
    .Map(dest => dest.Name, src => src.Name.ToUpper());

// Use with DbContext extensions
var dto = new ProductDto { Name = "Widget", Price = 9.99m };
await _context.AddItemAsync<Product, ProductDto>(dto, cancellationToken);
```

## Best Practices

> [!TIP]
> Use **DbContext extensions** for common CRUD operations to reduce boilerplate and ensure consistency.

> [!IMPORTANT]
> Always apply **ApplySoftDeleteFilters()** before **ApplyTenantFilters()** to ensure proper filter combination.

> [!NOTE]
> DbContext extensions automatically include navigation properties when retrieving entities.

### Entity Design

- Inherit from `BaseEntity` for standard entities with audit fields
- Inherit from `BaseTenantEntity` for multi-tenant entities
- Use `[IgnoreSoftDelete]` for entities that should never be soft-deleted (audit logs, etc.)
- Use `[IgnoreVersioning]` for entities that don't need version tracking
- Keep entities focused on data structure, not business logic

### DbContext Configuration

- Apply filters in the correct order: Versioning → Soft Delete → Tenant
- Use `ApplyModelBuilderConfigurations()` to auto-discover entity configurations
- Configure decimal precision consistently across the database
- Use dependency injection to access scoped context for tenant filtering

### Repository Pattern

- Use DbContext extensions for simple CRUD operations
- Create repositories for complex queries or domain-specific operations
- Always propagate CancellationToken to database operations
- Prefer async methods for all database operations

### Multi-Tenancy

- Store tenant ID in scoped context (per request)
- Apply tenant filters at the DbContext level, not in repositories
- Use `BaseTenantEntity` for all tenant-specific data
- Never trust client-provided tenant IDs - always get from authenticated context

## Testing

The framework is designed for testability with in-memory databases:

```csharp
[TestClass]
public class ProductServiceTests
{
    private ApplicationDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var mockContextService = new Mock<IScopedContextService>();
        _context = new ApplicationDbContext(options, mockContextService.Object);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [TestMethod]
    public async Task GetItemByIdAsync_ReturnsEntity_WhenExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.GetItemByIdAsync<Product, int>(
            1,
            CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
    }

    [TestMethod]
    public async Task AddItemAsync_AddsEntity()
    {
        // Arrange
        var dto = new ProductDto { Name = "Widget", Price = 9.99m };

        // Act
        await _context.AddItemAsync<Product, ProductDto>(dto, CancellationToken.None);

        // Assert
        Assert.AreEqual(1, _context.Products.Count());
    }
}
```

## Dependencies

- **ISynergy.Framework.Core** - Core abstractions and base classes
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** - Identity integration
- **Mapster** - Object-to-object mapping
- **Microsoft.EntityFrameworkCore** - Entity Framework Core runtime

## Migration Scenarios

### Adding Multi-Tenancy to Existing Application

```csharp
// 1. Change entities to inherit from BaseTenantEntity
public class Product : BaseTenantEntity  // Was: BaseEntity
{
    // Properties remain the same
}

// 2. Create migration to add TenantId column
dotnet ef migrations add AddTenantIdToProducts

// 3. Update DbContext to apply tenant filters
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyTenantFilters(() =>
        _contextService.GetContext<AppContext>().TenantId);
}

// 4. Populate TenantId for existing data
UPDATE Products SET TenantId = '00000000-0000-0000-0000-000000000000'
WHERE TenantId IS NULL;
```

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.CQRS** - CQRS implementation with EF integration
- **I-Synergy.Framework.AspNetCore** - ASP.NET Core integration
- **I-Synergy.Framework.Mvvm** - MVVM framework for UI applications

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

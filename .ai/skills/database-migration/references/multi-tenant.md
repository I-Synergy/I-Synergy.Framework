# Multi-Tenant Database Patterns

The three tenancy models and their EF Core mechanics. Load this when adding or changing tenant isolation.

## Multi-Tenant Database Patterns

### Approach 1: Shared Database, Shared Schema
```csharp
// Add TenantId to all entities
public abstract class TenantEntity
{
    public Guid TenantId { get; set; }
}

// Global query filter
modelBuilder.Entity<Budget>()
    .HasQueryFilter(b => b.TenantId == _currentTenantId);

// Index on TenantId
builder.HasIndex(e => e.TenantId);
```

### Approach 2: Shared Database, Separate Schemas
```csharp
// Use different schemas per tenant
builder.ToTable("budgets", schema: _tenantSchema);
```

### Approach 3: Separate Databases
```csharp
// Connection string per tenant
var connectionString = _configuration[$"ConnectionStrings:Tenant_{tenantId}"];
```

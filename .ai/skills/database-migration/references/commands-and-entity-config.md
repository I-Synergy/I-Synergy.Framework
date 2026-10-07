# EF Core Migration Commands & Entity Configuration

The `dotnet ef` command set for this solution's layout, and the Fluent API entity-configuration pattern. Load this when creating/applying/rolling back a migration or configuring an entity.

## Migration Commands

### Create Migration
```bash
# From solution root
dotnet ef migrations add {MigrationName} --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API

# Example
dotnet ef migrations add AddBudgetTable --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API
```

### Apply Migration
```bash
# Update database to latest migration
dotnet ef database update --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API

# Update to specific migration
dotnet ef database update {MigrationName} --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API
```

### Rollback Migration
```bash
# Rollback to previous migration
dotnet ef database update {PreviousMigrationName} --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API

# Rollback all migrations
dotnet ef database update 0 --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API
```

### Remove Migration
```bash
# Remove last migration (if not applied)
dotnet ef migrations remove --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API
```

### Generate SQL Script
```bash
# Generate SQL for review
dotnet ef migrations script --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API --output migration.sql

# Generate SQL from specific migration
dotnet ef migrations script {FromMigration} {ToMigration} --project src/{ApplicationName}.Data --startup-project src/{ApplicationName}.Services.API
```

## Entity Configuration Pattern

```csharp
// File: {ApplicationName}.Data/Configurations/{Entity}Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using {ApplicationName}.Entities.{Domain};

namespace {ApplicationName}.Data.Configurations;

/// <summary>
/// Entity configuration for {Entity}.
/// </summary>
public class {Entity}Configuration : IEntityTypeConfiguration<{Entity}>
{
    public void Configure(EntityTypeBuilder<{Entity}> builder)
    {
        // Table name (PostgreSQL snake_case convention)
        builder.ToTable("{entities}");

        // Primary key
        builder.HasKey(e => e.{Entity}Id);

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ChangedDate)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("idx_{entity}_name");

        builder.HasIndex(e => e.CreatedDate)
            .HasDatabaseName("idx_{entity}_created_date");

        // Relationships
        builder.HasMany(e => e.Goals)
            .WithOne(g => g.Budget)
            .HasForeignKey(g => g.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

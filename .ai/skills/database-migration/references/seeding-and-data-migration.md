# Data Seeding & Complex Data Migration

`HasData` seeding in `OnModelCreating`, and the expand/migrate/contract pattern for transforming data in place with a reversible `Down`. Load this when seeding reference data or migrating an existing column.

## Data Seeding Pattern

```csharp
// File: {ApplicationName}.Data/DataContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Apply configurations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

    // Seed data
    SeedData(modelBuilder);
}

private void SeedData(ModelBuilder modelBuilder)
{
    // Seed reference data
    modelBuilder.Entity<Category>().HasData(
        new Category
        {
            CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Housing",
            CreatedDate = DateTimeOffset.UtcNow
        },
        new Category
        {
            CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Transportation",
            CreatedDate = DateTimeOffset.UtcNow
        }
    );
}
```

## Complex Data Migration Pattern

```csharp
// File: Migrations/{Timestamp}_MigrateOldDataToNewFormat.cs
public partial class MigrateOldDataToNewFormat : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Add new columns
        migrationBuilder.AddColumn<string>(
            name: "new_column",
            table: "budgets",
            type: "text",
            nullable: true);

        // 2. Migrate data
        migrationBuilder.Sql(@"
            UPDATE budgets
            SET new_column = CONCAT(old_column1, '-', old_column2)
            WHERE old_column1 IS NOT NULL;
        ");

        // 3. Make new column required
        migrationBuilder.AlterColumn<string>(
            name: "new_column",
            table: "budgets",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        // 4. Drop old columns
        migrationBuilder.DropColumn(
            name: "old_column1",
            table: "budgets");

        migrationBuilder.DropColumn(
            name: "old_column2",
            table: "budgets");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse the migration
        migrationBuilder.AddColumn<string>(
            name: "old_column1",
            table: "budgets",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "old_column2",
            table: "budgets",
            type: "text",
            nullable: true);

        migrationBuilder.Sql(@"
            UPDATE budgets
            SET
                old_column1 = SPLIT_PART(new_column, '-', 1),
                old_column2 = SPLIT_PART(new_column, '-', 2)
            WHERE new_column IS NOT NULL;
        ");

        migrationBuilder.DropColumn(
            name: "new_column",
            table: "budgets");
    }
}
```

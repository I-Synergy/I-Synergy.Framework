# Index Design & Query Performance

When to add an index, the index types available through the Fluent API, and the query shapes that avoid N+1 and over-fetching. Load this when a query is slow or a table is being designed.

## Index Design Guidelines

### When to Add Indexes

✅ **DO Index:**
- Primary keys (automatic)
- Foreign keys
- Frequently queried columns
- Columns used in WHERE clauses
- Columns used in ORDER BY
- Columns used in JOIN conditions

❌ **DON'T Index:**
- Small tables (< 1000 rows)
- Columns with low cardinality (few distinct values)
- Columns rarely used in queries
- Columns that change frequently

### Index Types

```csharp
// Single column index
builder.HasIndex(e => e.Name);

// Composite index
builder.HasIndex(e => new { e.{Entity}Id, e.CreatedDate });

// Unique index
builder.HasIndex(e => e.Email)
    .IsUnique();

// Filtered index (PostgreSQL)
builder.HasIndex(e => e.Status)
    .HasFilter("status = 'Active'");

// Covering index (include columns)
builder.HasIndex(e => e.{Entity}Id)
    .IncludeProperties(e => new { e.Name, e.Amount });
```

## Performance Optimization Patterns

### Query Optimization
```csharp
// ✅ GOOD - Single query with Include
var {entities} = await context.{Entities}
    .Include(b => b.{ChildEntities})
    .Include(b => b.{ChildEntities})
    .Where(b => b.UserId == userId)
    .ToListAsync();

// ❌ BAD - N+1 query problem
var {entities} = await context.{Entities}
    .Where(b => b.UserId == userId)
    .ToListAsync();

foreach (var {entity} in {entities})
{
    {entity}.{ChildEntities} = await context.{ChildEntities}
        .Where(g => g.{Entity}Id == {entity}.{Entity}Id)
        .ToListAsync(); // Separate query for each {entity}!
}
```

### Batch Operations
```csharp
// ✅ GOOD - Batch insert
context.{Entities}.AddRange({entities});
await context.SaveChangesAsync();

// ❌ BAD - Individual inserts
foreach (var {entity} in {entities})
{
    context.{Entities}.Add({entity});
    await context.SaveChangesAsync(); // Multiple round trips!
}
```

### Projection for Performance
```csharp
// ✅ GOOD - Select only needed columns
var {entity}Names = await context.{Entities}
    .Where(b => b.UserId == userId)
    .Select(b => new { b.{Entity}Id, b.Name })
    .ToListAsync();

// ❌ BAD - Load entire entities
var {entities} = await context.{Entities}
    .Where(b => b.UserId == userId)
    .ToListAsync();
var {entity}Names = {entities}.Select(b => new { b.{Entity}Id, b.Name });
```

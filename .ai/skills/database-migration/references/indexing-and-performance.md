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
builder.HasIndex(e => new { e.BudgetId, e.CreatedDate });

// Unique index
builder.HasIndex(e => e.Email)
    .IsUnique();

// Filtered index (PostgreSQL)
builder.HasIndex(e => e.Status)
    .HasFilter("status = 'Active'");

// Covering index (include columns)
builder.HasIndex(e => e.BudgetId)
    .IncludeProperties(e => new { e.Name, e.Amount });
```

## Performance Optimization Patterns

### Query Optimization
```csharp
// ✅ GOOD - Single query with Include
var budgets = await context.Budgets
    .Include(b => b.Goals)
    .Include(b => b.Debts)
    .Where(b => b.UserId == userId)
    .ToListAsync();

// ❌ BAD - N+1 query problem
var budgets = await context.Budgets
    .Where(b => b.UserId == userId)
    .ToListAsync();

foreach (var budget in budgets)
{
    budget.Goals = await context.Goals
        .Where(g => g.BudgetId == budget.BudgetId)
        .ToListAsync(); // Separate query for each budget!
}
```

### Batch Operations
```csharp
// ✅ GOOD - Batch insert
context.Budgets.AddRange(budgets);
await context.SaveChangesAsync();

// ❌ BAD - Individual inserts
foreach (var budget in budgets)
{
    context.Budgets.Add(budget);
    await context.SaveChangesAsync(); // Multiple round trips!
}
```

### Projection for Performance
```csharp
// ✅ GOOD - Select only needed columns
var budgetNames = await context.Budgets
    .Where(b => b.UserId == userId)
    .Select(b => new { b.BudgetId, b.Name })
    .ToListAsync();

// ❌ BAD - Load entire entities
var budgets = await context.Budgets
    .Where(b => b.UserId == userId)
    .ToListAsync();
var budgetNames = budgets.Select(b => new { b.BudgetId, b.Name });
```

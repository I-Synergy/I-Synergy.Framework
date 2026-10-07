# Database Query Optimization

N+1 prevention, `AsNoTracking`, projection, batching, and pagination — each with the bad/good pair. Load this when an EF Core query is slow or an endpoint returns large result sets.

## Database Query Optimization

### N+1 Query Prevention
```csharp
// ❌ BAD - N+1 query problem
public async Task<List<BudgetWithGoalsResponse>> GetBudgetsWithGoals()
{
    var budgets = await _context.Budgets.ToListAsync();

    foreach (var budget in budgets)
    {
        // Separate query for each budget!
        budget.Goals = await _context.Goals
            .Where(g => g.BudgetId == budget.BudgetId)
            .ToListAsync();
    }

    return budgets.Select(b => new BudgetWithGoalsResponse(
        b.BudgetId,
        b.Name,
        b.Amount,
        b.Goals.Select(g => new GoalSummary(g.GoalId, g.Name, g.TargetAmount)).ToList()
    )).ToList();
}

// ✅ GOOD - Single query with Include
public async Task<List<BudgetWithGoalsResponse>> GetBudgetsWithGoals()
{
    return await _context.Budgets
        .Include(b => b.Goals)
        .Select(b => new BudgetWithGoalsResponse(
            b.BudgetId,
            b.Name,
            b.Amount,
            b.Goals.Select(g => new GoalSummary(g.GoalId, g.Name, g.TargetAmount)).ToList()
        ))
        .ToListAsync();
}

// ✅ BETTER - Project only needed data
public async Task<List<BudgetWithGoalsResponse>> GetBudgetsWithGoals()
{
    return await _context.Budgets
        .Select(b => new BudgetWithGoalsResponse(
            b.BudgetId,
            b.Name,
            b.Amount,
            b.Goals.Select(g => new GoalSummary(g.GoalId, g.Name, g.TargetAmount)).ToList()
        ))
        .ToListAsync();
}
```

### AsNoTracking for Read-Only Queries
```csharp
// ✅ GOOD - Use AsNoTracking for read-only operations
public async Task<List<BudgetResponse>> GetBudgetsAsync(
    CancellationToken cancellationToken)
{
    return await _context.Budgets
        .AsNoTracking()  // Don't track changes
        .Select(b => new BudgetResponse(
            b.BudgetId,
            b.Name,
            b.Amount
        ))
        .ToListAsync(cancellationToken);
}
```

### Projection to Reduce Data Transfer
```csharp
// ❌ BAD - Loading entire entities
public async Task<List<string>> GetBudgetNames()
{
    var budgets = await _context.Budgets.ToListAsync();
    return budgets.Select(b => b.Name).ToList();
}

// ✅ GOOD - Project only needed columns
public async Task<List<string>> GetBudgetNames()
{
    return await _context.Budgets
        .Select(b => b.Name)
        .ToListAsync();
}
```

### Batch Operations
```csharp
// ❌ BAD - Individual operations
public async Task CreateMultipleBudgets(List<CreateBudgetCommand> commands)
{
    foreach (var command in commands)
    {
        var entity = new Budget { BudgetId = Guid.NewGuid(), Name = command.Name, Amount = command.Amount };
        _context.Budgets.Add(entity);
        await _context.SaveChangesAsync(); // Multiple round trips!
    }
}

// ✅ GOOD - Batch operation
public async Task CreateMultipleBudgets(List<CreateBudgetCommand> commands)
{
    var entities = commands
        .Select(c => new Budget { BudgetId = Guid.NewGuid(), Name = c.Name, Amount = c.Amount })
        .ToList();

    _context.Budgets.AddRange(entities);
    await _context.SaveChangesAsync(); // Single round trip
}
```

### Pagination for Large Result Sets
```csharp
// ✅ GOOD - Implement pagination
public async Task<PagedResult<BudgetResponse>> GetBudgetsPaged(
    int pageNumber = 1,
    int pageSize = 20,
    CancellationToken cancellationToken = default)
{
    var totalCount = await _context.Budgets.CountAsync(cancellationToken);

    var budgets = await _context.Budgets
        .AsNoTracking()
        .OrderByDescending(b => b.CreatedDate)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(b => new BudgetResponse(b.BudgetId, b.Name, b.Amount))
        .ToListAsync(cancellationToken);

    return new PagedResult<BudgetResponse>(
        budgets,
        totalCount,
        pageNumber,
        pageSize);
}
```

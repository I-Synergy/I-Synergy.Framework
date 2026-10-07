# Async, Memory & Latency Patterns

Async-all-the-way, `ValueTask` and `ConfigureAwait` guidance, allocation-reduction techniques, and response-time levers (compression, parallelism, HTTP/2). Load this when a request is slow or allocation-heavy.

## Async/Await Best Practices

### Async All the Way
```csharp
// ❌ BAD - Blocking on async
public BudgetResponse GetBudget(Guid id)
{
    return GetBudgetAsync(id).Result; // DEADLOCK RISK!
}

// ✅ GOOD - Async all the way
public async Task<BudgetResponse?> GetBudgetAsync(
    Guid id,
    CancellationToken cancellationToken = default)
{
    var budget = await dataContext.Budgets
        .FirstOrDefaultAsync(b => b.BudgetId == id, cancellationToken);

    return budget is null ? null : new BudgetResponse(budget.BudgetId, budget.Name, budget.Amount);
}
```

### ValueTask for Hot Paths
```csharp
// ✅ GOOD - Use ValueTask for frequently-called methods
public async ValueTask<BudgetResponse?> GetCachedBudgetAsync(
    Guid budgetId,
    CancellationToken cancellationToken = default)
{
    // Check cache (often completes synchronously)
    if (_memoryCache.TryGetValue(budgetId, out BudgetResponse? cached))
        return cached;

    // Load from database
    var budget = await dataContext.Budgets
        .FirstOrDefaultAsync(b => b.BudgetId == budgetId, cancellationToken);

    var response = budget is null ? null : new BudgetResponse(budget.BudgetId, budget.Name, budget.Amount);
    if (response is not null) _memoryCache.Set(budgetId, response);

    return response;
}
```

### ConfigureAwait Guidelines
```csharp
// For library code (not application code)
public async Task<BudgetResponse> GetBudgetAsync(Guid id)
{
    var budget = await dataContext.Budgets
        .FirstOrDefaultAsync(b => b.BudgetId == id)
        .ConfigureAwait(false); // Only in library code

    return budget is null ? null : new BudgetResponse(budget.BudgetId, budget.Name, budget.Amount);
}

// For application code (ASP.NET Core), DON'T use ConfigureAwait
public async Task<BudgetResponse?> GetBudgetAsync(Guid id)
{
    var budget = await dataContext.Budgets
        .FirstOrDefaultAsync(b => b.BudgetId == id);
    // No ConfigureAwait needed in ASP.NET Core

    return budget is null ? null : new BudgetResponse(budget.BudgetId, budget.Name, budget.Amount);
}
```

## Memory Optimization

### String Handling
```csharp
// ❌ BAD - String concatenation in loop
public string BuildCsv(List<Budget> budgets)
{
    string csv = "Id,Name,Amount\n";
    foreach (var budget in budgets)
    {
        csv += $"{budget.BudgetId},{budget.Name},{budget.Amount}\n"; // Allocates new string each time
    }
    return csv;
}

// ✅ GOOD - Use StringBuilder
public string BuildCsv(List<Budget> budgets)
{
    var sb = new StringBuilder();
    sb.AppendLine("Id,Name,Amount");
    foreach (var budget in budgets)
    {
        sb.AppendLine($"{budget.BudgetId},{budget.Name},{budget.Amount}");
    }
    return sb.ToString();
}
```

### ArrayPool for Large Buffers
```csharp
public async Task<byte[]> ProcessLargeDataAsync(Stream stream)
{
    var buffer = ArrayPool<byte>.Shared.Rent(4096);
    try
    {
        await stream.ReadAsync(buffer, 0, buffer.Length);
        // Process buffer
        return buffer.Take(stream.Length).ToArray();
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

### Minimize LINQ Allocations
```csharp
// ❌ BAD - Multiple enumerations
public decimal CalculateTotal(List<Budget> budgets)
{
    var active = budgets.Where(b => b.IsActive);
    var count = active.Count();
    var total = active.Sum(b => b.Amount);
    return total / count;
}

// ✅ GOOD - Single enumeration
public decimal CalculateAverage(List<Budget> budgets)
{
    var activeList = budgets.Where(b => b.IsActive).ToList();
    return activeList.Sum(b => b.Amount) / activeList.Count;
}

// ✅ BETTER - Use aggregate functions
public decimal CalculateAverage(List<Budget> budgets)
{
    return budgets.Where(b => b.IsActive).Average(b => b.Amount);
}
```

## Response Time Optimization

### Response Compression
```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

app.UseResponseCompression();
```

### Parallel Processing
```csharp
// ✅ GOOD - Process independent operations in parallel
public async Task<DashboardData> GetDashboardDataAsync(Guid userId)
{
    var budgetsTask = GetUserBudgetsAsync(userId);
    var goalsTask = GetUserGoalsAsync(userId);
    var debtsTask = GetUserDebtsAsync(userId);

    await Task.WhenAll(budgetsTask, goalsTask, debtsTask);

    return new DashboardData(
        await budgetsTask,
        await goalsTask,
        await debtsTask);
}
```

### HTTP/2 and Multiplexing
```csharp
// Program.cs - Enable HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});
```

---
name: performance-engineer
description: Performance optimization specialist. Use when profiling applications, optimizing database queries, implementing caching, or improving response times.
---

# Performance Optimization Specialist Skill

Specialized agent for performance profiling, optimization, and ensuring application responsiveness.

## Role

You are a Performance Engineer responsible for analyzing application performance, identifying bottlenecks, optimizing database queries, implementing caching strategies, and ensuring the application meets performance requirements.

## Expertise Areas

- Performance profiling and diagnostics
- Database query optimization
- Caching strategies (Redis, in-memory)
- Response time optimization
- Memory management and GC tuning
- Async/await best practices
- N+1 query prevention
- Load testing and benchmarking
- Application Insights integration
- Resource allocation patterns

## Responsibilities

1. **Performance Analysis**
   - Profile application using diagnostic tools
   - Identify performance bottlenecks
   - Measure response times and throughput
   - Analyze memory usage patterns
   - Monitor database query performance

2. **Query Optimization**
   - Optimize EF Core LINQ queries
   - Implement proper eager loading
   - Use projection to reduce data transfer
   - Add appropriate indexes
   - Batch operations where possible

3. **Caching Strategy**
   - Implement multi-level caching
   - Cache stable reference data
   - Invalidate cache appropriately
   - Use distributed caching (Redis)
   - Monitor cache hit rates

4. **Memory Optimization**
   - Minimize allocations in hot paths
   - Use object pooling where appropriate
   - Optimize string operations
   - Reduce GC pressure
   - Profile memory usage

## Load Additional Patterns

- [`cqrs-patterns.md`](../../patterns/cqrs-patterns.md)
- [`api-patterns.md`](../../patterns/api-patterns.md)

## Critical Rules

### Performance First Principles
- Measure before optimizing (no premature optimization)
- Set clear performance targets
- Optimize the critical path first
- Use async/await properly
- Minimize allocations in hot paths
- Cache aggressively (but invalidate correctly)
- Use connection pooling
- Batch database operations

### Database Performance
- ALWAYS prevent N+1 queries
- Use Include() for eager loading
- Project only needed columns
- Add indexes on foreign keys and frequently queried columns
- Use AsNoTracking() for read-only queries
- Batch insert/update operations
- Monitor query execution time

### Caching Rules
- Cache stable reference data
- Use appropriate cache expiration
- Implement cache invalidation strategy
- Monitor cache hit rates
- Use distributed cache for multi-instance deployments
- Don't cache user-specific data in shared cache

## Performance Profiling Tools

### .NET Diagnostic Tools
```bash
# dotnet-counters (real-time metrics)
dotnet tool install --global dotnet-counters
dotnet-counters monitor --process-id <PID>

# dotnet-trace (performance tracing)
dotnet tool install --global dotnet-trace
dotnet-trace collect --process-id <PID>

# dotnet-dump (memory dumps)
dotnet tool install --global dotnet-dump
dotnet-dump collect --process-id <PID>

# dotnet-gcdump (GC analysis)
dotnet tool install --global dotnet-gcdump
dotnet-gcdump collect --process-id <PID>
```

### Application Insights
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Custom metrics
public class PerformanceMonitoringMiddleware(
    RequestDelegate next,
    TelemetryClient telemetryClient)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        await next(context);

        sw.Stop();

        telemetryClient.TrackMetric(
            "RequestDuration",
            sw.ElapsedMilliseconds,
            new Dictionary<string, string>
            {
                ["Endpoint"] = context.Request.Path,
                ["Method"] = context.Request.Method
            });
    }
}
```

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

    return budgets.Adapt<List<BudgetWithGoalsResponse>>();
}

// ✅ GOOD - Single query with Include
public async Task<List<BudgetWithGoalsResponse>> GetBudgetsWithGoals()
{
    var budgets = await _context.Budgets
        .Include(b => b.Goals)
        .ToListAsync();

    return budgets.Adapt<List<BudgetWithGoalsResponse>>();
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
        var entity = new Budget { Name = command.Name, Amount = command.Amount, TenantId = TenantContext.TenantId };
        _context.Budgets.Add(entity);
        await _context.SaveChangesAsync(); // Multiple round trips!
    }
}

// ✅ GOOD - Batch operation
public async Task CreateMultipleBudgets(List<CreateBudgetCommand> commands)
{
    var entities = commands
        .Select(c => new Budget { Name = c.Name, Amount = c.Amount, TenantId = TenantContext.TenantId })
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

## Caching Patterns

### Multi-Level Caching Architecture
```
Request → L1 Cache (In-Memory) → L2 Cache (Redis) → Database
```

### In-Memory Caching
```csharp
public class CachedCategoryService(
    IMemoryCache memoryCache,
    DataContext dataContext)
{
    public async Task<List<Category>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var cacheKey = "categories:all";

        if (memoryCache.TryGetValue(cacheKey, out List<Category>? categories))
            return categories!;

        categories = await dataContext.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        memoryCache.Set(cacheKey, categories, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(15)
        });

        return categories;
    }
}
```

### Distributed Caching (Redis)
```csharp
public class CachedBudgetService(
    IDistributedCache distributedCache,
    DataContext dataContext,
    ILogger<CachedBudgetService> logger)
{
    public async Task<BudgetResponse?> GetBudgetByIdAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"budget:{budgetId}";

        // Try cache first
        var cachedData = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogDebug("Cache hit for Budget {BudgetId}", budgetId);
            return JsonSerializer.Deserialize<BudgetResponse>(cachedData);
        }

        logger.LogDebug("Cache miss for Budget {BudgetId}", budgetId);

        // Load from database
        var entity = await dataContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BudgetId == budgetId, cancellationToken)
            ?? throw new InvalidOperationException("Budget not found");

        var response = new BudgetResponse(entity.BudgetId, entity.Name, entity.Amount);

        // Cache for 1 hour
        await distributedCache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            },
            cancellationToken);

        return response;
    }

    public async Task InvalidateBudgetCacheAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"budget:{budgetId}";
        await distributedCache.RemoveAsync(cacheKey, cancellationToken);

        logger.LogDebug("Invalidated cache for Budget {BudgetId}", budgetId);
    }
}
```

### Cache Invalidation Pattern
```csharp
public sealed class UpdateBudgetHandler(
    DataContext dataContext,
    IDistributedCache cache,
    ILogger<UpdateBudgetHandler> logger
) : ICommandHandler<UpdateBudgetCommand, UpdateBudgetResponse>
{
    public async Task<UpdateBudgetResponse> HandleAsync(
        UpdateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = await dataContext.Budgets
            .FirstOrDefaultAsync(b => b.BudgetId == command.BudgetId, cancellationToken)
            ?? throw new InvalidOperationException("Budget not found");

        entity.Name = command.Name;
        entity.Amount = command.Amount;
        await dataContext.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        var cacheKey = $"budget:{command.BudgetId}";
        await cache.RemoveAsync(cacheKey, cancellationToken);

        logger.LogInformation(
            "Updated Budget {BudgetId} and invalidated cache",
            command.BudgetId);

        return new UpdateBudgetResponse(true);
    }
}
```

## Async/Await Best Practices

### Async All the Way
```csharp
// ❌ BAD - Blocking on async
public BudgetResponse GetBudget(Guid id)
{
    return GetBudgetAsync(id).Result; // DEADLOCK RISK!
}

// ✅ GOOD - Async all the way
public async Task<BudgetResponse> GetBudgetAsync(
    Guid id,
    CancellationToken cancellationToken = default)
{
    var budget = await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(
        id,
        cancellationToken);

    return budget.Adapt<BudgetResponse>();
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
    var budget = await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(
        budgetId,
        cancellationToken);

    var response = budget.Adapt<BudgetResponse>();
    _memoryCache.Set(budgetId, response);

    return response;
}
```

### ConfigureAwait Guidelines
```csharp
// For library code (not application code)
public async Task<BudgetResponse> GetBudgetAsync(Guid id)
{
    var budget = await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id)
        .ConfigureAwait(false); // Only in library code

    return budget.Adapt<BudgetResponse>();
}

// For application code (ASP.NET Core), DON'T use ConfigureAwait
public async Task<BudgetResponse> GetBudgetAsync(Guid id)
{
    var budget = await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id);
    // No ConfigureAwait needed in ASP.NET Core

    return budget.Adapt<BudgetResponse>();
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

## Load Testing

### Using k6 for Load Testing
```javascript
// load-test.js
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 20 },  // Ramp up
        { duration: '1m', target: 20 },   // Stay at 20 users
        { duration: '30s', target: 0 },   // Ramp down
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests < 500ms
    },
};

export default function () {
    let response = http.get('https://localhost:7001/budgets');

    check(response, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}
```

```bash
# Run load test
k6 run load-test.js
```

### BenchmarkDotNet for Micro-Benchmarks
```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class MappingBenchmarks
{
    private Budget _budget;

    [GlobalSetup]
    public void Setup()
    {
        _budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = "Test Budget",
            Amount = 1000m
        };
    }

    [Benchmark]
    public BudgetModel MapsterMapping()
    {
        return _budget.Adapt<BudgetModel>();
    }

    [Benchmark]
    public BudgetModel ManualMapping()
    {
        return new BudgetModel
        {
            BudgetId = _budget.BudgetId,
            Name = _budget.Name,
            Amount = _budget.Amount
        };
    }
}

// Run benchmarks
BenchmarkRunner.Run<MappingBenchmarks>();
```

## Performance Monitoring

### Custom Metrics
```csharp
public class PerformanceMetrics(ILogger<PerformanceMetrics> logger)
{
    private readonly ConcurrentDictionary<string, long> _counters = new();

    public void IncrementCounter(string name)
    {
        _counters.AddOrUpdate(name, 1, (_, count) => count + 1);
    }

    public void RecordDuration(string operation, long milliseconds)
    {
        logger.LogInformation(
            "Operation {Operation} completed in {Duration}ms",
            operation, milliseconds);
    }

    public Dictionary<string, long> GetCounters() => _counters.ToDictionary(k => k.Key, v => v.Value);
}
```

### Database Query Logging
```csharp
// Program.cs
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});
```

## Common Performance Pitfalls

### ❌ Avoid These Mistakes

1. **N+1 Query Problem**
   - ❌ Lazy loading in loops
   - ✅ Use Include() or projection

2. **Over-Caching**
   - ❌ Caching everything including user-specific data
   - ✅ Cache only stable reference data

3. **Synchronous Over Async**
   - ❌ Using .Result or .Wait()
   - ✅ Async all the way

4. **Loading Entire Entities**
   - ❌ ToList() then Select()
   - ✅ Select() then ToList()

5. **No Pagination**
   - ❌ Returning all records
   - ✅ Implement pagination

6. **Missing Indexes**
   - ❌ No indexes on foreign keys
   - ✅ Index all foreign keys and frequently queried columns

## Performance Review Checklist

### Database Queries
- [ ] No N+1 query problems
- [ ] Include() used for related data
- [ ] AsNoTracking() for read-only queries
- [ ] Projection used to reduce data transfer
- [ ] Appropriate indexes on tables
- [ ] Batch operations for bulk changes
- [ ] Pagination implemented for large result sets

### Caching
- [ ] Caching strategy defined
- [ ] Distributed cache (Redis) for multi-instance
- [ ] Cache invalidation implemented
- [ ] Cache expiration set appropriately
- [ ] Cache hit rate monitored
- [ ] No user-specific data in shared cache

### Async/Await
- [ ] Async all the way (no .Wait() or .Result)
- [ ] CancellationToken passed through
- [ ] ValueTask used in hot paths
- [ ] No async void (except event handlers)

### Memory
- [ ] StringBuilder for string concatenation
- [ ] ArrayPool for large buffers
- [ ] Minimal allocations in hot paths
- [ ] LINQ not causing multiple enumerations

### Monitoring
- [ ] Application Insights configured
- [ ] Custom metrics for critical operations
- [ ] Query performance monitored
- [ ] Response times tracked
- [ ] Error rates monitored

### Load Testing
- [ ] Load tests defined
- [ ] Performance targets set
- [ ] Critical paths tested
- [ ] Results analyzed and documented

## Checklist Before Completion

- [ ] Performance profiling completed
- [ ] Bottlenecks identified and addressed
- [ ] Database queries optimized
- [ ] Caching strategy implemented
- [ ] No N+1 query problems
- [ ] Async/await used correctly
- [ ] Memory allocations minimized
- [ ] Load testing performed
- [ ] Performance targets met
- [ ] Monitoring and metrics in place

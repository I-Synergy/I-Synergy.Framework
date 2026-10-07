# Caching Patterns

The multi-level cache architecture, in-memory and Redis implementations, and the invalidation-on-write handler pattern. Load this when adding a cache or debugging a stale read.

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
        var budget = await dataContext.Budgets.FirstOrDefaultAsync(
            b => b.BudgetId == budgetId,
            cancellationToken);

        var response = budget is null ? null : new BudgetResponse(budget.BudgetId, budget.Name, budget.Amount);

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
public sealed class UpdateBudgetCommandHandler(
    DataContext dataContext,
    IDistributedCache cache,
    ILogger<UpdateBudgetCommandHandler> logger
) : ICommandHandler<UpdateBudgetCommand, UpdateBudgetResponse>
{
    public async Task<UpdateBudgetResponse> HandleAsync(
        UpdateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = await dataContext.Budgets.FirstOrDefaultAsync(
            b => b.BudgetId == command.BudgetId,
            cancellationToken);

        if (entity is null)
            throw new InvalidOperationException($"Budget {command.BudgetId} not found");

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

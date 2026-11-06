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
public class Cached{Entity}Service(
    IDistributedCache distributedCache,
    DataContext dataContext,
    ILogger<Cached{Entity}Service> logger)
{
    public async Task<{Entity}Response?> Get{Entity}ByIdAsync(
        Guid {entity}Id,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{entity}:{entity}Id";

        // Try cache first
        var cachedData = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogDebug("Cache hit for {Entity}Id", {entity}Id);
            return JsonSerializer.Deserialize<{Entity}Response>(cachedData);
        }

        logger.LogDebug("Cache miss for {Entity}Id", {entity}Id);

        // Load from database
        var {entity} = await dataContext.{Entities}.FirstOrDefaultAsync(
            b => b.{Entity}Id == {entity}Id,
            cancellationToken);

        var response = {entity} is null ? null : new {Entity}Response({entity}.{Entity}Id, {entity}.Name, {entity}.Amount);

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

    public async Task Invalidate{Entity}CacheAsync(
        Guid {entity}Id,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{entity}:{entity}Id";
        await distributedCache.RemoveAsync(cacheKey, cancellationToken);

        logger.LogDebug("Invalidated cache for {Entity}Id", {entity}Id);
    }
}
```

### Cache Invalidation Pattern
```csharp
public sealed class Update{Entity}CommandHandler(
    DataContext dataContext,
    IDistributedCache cache,
    ILogger<Update{Entity}CommandHandler> logger
) : ICommandHandler<Update{Entity}Command, Update{Entity}Response>
{
    public async Task<Update{Entity}Response> HandleAsync(
        Update{Entity}Command command,
        CancellationToken cancellationToken = default)
    {
        var entity = await dataContext.{Entities}.FirstOrDefaultAsync(
            b => b.{Entity}Id == command.{Entity}Id,
            cancellationToken);

        if (entity is null)
            throw new InvalidOperationException($"{Entity} {command.{Entity}Id} not found");

        entity.Name = command.Name;
        entity.Amount = command.Amount;
        await dataContext.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        var cacheKey = $"{entity}:{command.{Entity}Id}";
        await cache.RemoveAsync(cacheKey, cancellationToken);

        logger.LogInformation(
            "Updated {Entity}Id and invalidated cache",
            command.{Entity}Id);

        return new Update{Entity}Response(true);
    }
}
```

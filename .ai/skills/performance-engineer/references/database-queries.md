# Database Query Optimization

N+1 prevention, `AsNoTracking`, projection, batching, and pagination — each with the bad/good pair. Load this when an EF Core query is slow or an endpoint returns large result sets.

## Database Query Optimization

### N+1 Query Prevention
```csharp
// ❌ BAD - N+1 query problem
public async Task<List<{Entity}With{ChildEntities}Response>> Get{Entities}With{ChildEntities}()
{
    var {entities} = await _context.{Entities}.ToListAsync();

    foreach (var {entity} in {entities})
    {
        // Separate query for each {entity}!
        {entity}.{ChildEntities} = await _context.{ChildEntities}
            .Where(g => g.{Entity}Id == {entity}.{Entity}Id)
            .ToListAsync();
    }

    return {entities}.Select(b => new {Entity}With{ChildEntities}Response(
        b.{Entity}Id,
        b.Name,
        b.Amount,
        b.{ChildEntities}.Select(g => new {ChildEntity}Summary(g.{ChildEntity}Id, g.Name, g.Amount)).ToList()
    )).ToList();
}

// ✅ GOOD - Single query with Include
public async Task<List<{Entity}With{ChildEntities}Response>> Get{Entities}With{ChildEntities}()
{
    return await _context.{Entities}
        .Include(b => b.{ChildEntities})
        .Select(b => new {Entity}With{ChildEntities}Response(
            b.{Entity}Id,
            b.Name,
            b.Amount,
            b.{ChildEntities}.Select(g => new {ChildEntity}Summary(g.{ChildEntity}Id, g.Name, g.Amount)).ToList()
        ))
        .ToListAsync();
}

// ✅ BETTER - Project only needed data
public async Task<List<{Entity}With{ChildEntities}Response>> Get{Entities}With{ChildEntities}()
{
    return await _context.{Entities}
        .Select(b => new {Entity}With{ChildEntities}Response(
            b.{Entity}Id,
            b.Name,
            b.Amount,
            b.{ChildEntities}.Select(g => new {ChildEntity}Summary(g.{ChildEntity}Id, g.Name, g.Amount)).ToList()
        ))
        .ToListAsync();
}
```

### AsNoTracking for Read-Only Queries
```csharp
// ✅ GOOD - Use AsNoTracking for read-only operations
public async Task<List<{Entity}Response>> Get{Entities}Async(
    CancellationToken cancellationToken)
{
    return await _context.{Entities}
        .AsNoTracking()  // Don't track changes
        .Select(b => new {Entity}Response(
            b.{Entity}Id,
            b.Name,
            b.Amount
        ))
        .ToListAsync(cancellationToken);
}
```

### Projection to Reduce Data Transfer
```csharp
// ❌ BAD - Loading entire entities
public async Task<List<string>> Get{Entity}Names()
{
    var {entities} = await _context.{Entities}.ToListAsync();
    return {entities}.Select(b => b.Name).ToList();
}

// ✅ GOOD - Project only needed columns
public async Task<List<string>> Get{Entity}Names()
{
    return await _context.{Entities}
        .Select(b => b.Name)
        .ToListAsync();
}
```

### Batch Operations
```csharp
// ❌ BAD - Individual operations
public async Task CreateMultiple{Entities}(List<Create{Entity}Command> commands)
{
    foreach (var command in commands)
    {
        var entity = new {Entity} { {Entity}Id = Guid.NewGuid(), Name = command.Name, Amount = command.Amount };
        _context.{Entities}.Add(entity);
        await _context.SaveChangesAsync(); // Multiple round trips!
    }
}

// ✅ GOOD - Batch operation
public async Task CreateMultiple{Entities}(List<Create{Entity}Command> commands)
{
    var entities = commands
        .Select(c => new {Entity} { {Entity}Id = Guid.NewGuid(), Name = c.Name, Amount = c.Amount })
        .ToList();

    _context.{Entities}.AddRange(entities);
    await _context.SaveChangesAsync(); // Single round trip
}
```

### Pagination for Large Result Sets
```csharp
// ✅ GOOD - Implement pagination
public async Task<PagedResult<{Entity}Response>> Get{Entities}Paged(
    int pageNumber = 1,
    int pageSize = 20,
    CancellationToken cancellationToken = default)
{
    var totalCount = await _context.{Entities}.CountAsync(cancellationToken);

    var {entities} = await _context.{Entities}
        .AsNoTracking()
        .OrderByDescending(b => b.CreatedDate)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(b => new {Entity}Response(b.{Entity}Id, b.Name, b.Amount))
        .ToListAsync(cancellationToken);

    return new PagedResult<{Entity}Response>(
        {entities},
        totalCount,
        pageNumber,
        pageSize);
}
```

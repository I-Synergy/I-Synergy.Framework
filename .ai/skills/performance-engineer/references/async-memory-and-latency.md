# Async, Memory & Latency Patterns

Async-all-the-way, `ValueTask` and `ConfigureAwait` guidance, allocation-reduction techniques, and response-time levers (compression, parallelism, HTTP/2). Load this when a request is slow or allocation-heavy.

## Async/Await Best Practices

### Async All the Way
```csharp
// ❌ BAD - Blocking on async
public {Entity}Response Get{Entity}(Guid id)
{
    return Get{Entity}Async(id).Result; // DEADLOCK RISK!
}

// ✅ GOOD - Async all the way
public async Task<{Entity}Response?> Get{Entity}Async(
    Guid id,
    CancellationToken cancellationToken = default)
{
    var {entity} = await dataContext.{Entities}
        .FirstOrDefaultAsync(b => b.{Entity}Id == id, cancellationToken);

    return {entity} is null ? null : new {Entity}Response({entity}.{Entity}Id, {entity}.Name, {entity}.Amount);
}
```

### ValueTask for Hot Paths
```csharp
// ✅ GOOD - Use ValueTask for frequently-called methods
public async ValueTask<{Entity}Response?> GetCached{Entity}Async(
    Guid {entity}Id,
    CancellationToken cancellationToken = default)
{
    // Check cache (often completes synchronously)
    if (_memoryCache.TryGetValue({entity}Id, out {Entity}Response? cached))
        return cached;

    // Load from database
    var {entity} = await dataContext.{Entities}
        .FirstOrDefaultAsync(b => b.{Entity}Id == {entity}Id, cancellationToken);

    var response = {entity} is null ? null : new {Entity}Response({entity}.{Entity}Id, {entity}.Name, {entity}.Amount);
    if (response is not null) _memoryCache.Set({entity}Id, response);

    return response;
}
```

### ConfigureAwait Guidelines
```csharp
// For library code (not application code)
public async Task<{Entity}Response> Get{Entity}Async(Guid id)
{
    var {entity} = await dataContext.{Entities}
        .FirstOrDefaultAsync(b => b.{Entity}Id == id)
        .ConfigureAwait(false); // Only in library code

    return {entity} is null ? null : new {Entity}Response({entity}.{Entity}Id, {entity}.Name, {entity}.Amount);
}

// For application code (ASP.NET Core), DON'T use ConfigureAwait
public async Task<{Entity}Response?> Get{Entity}Async(Guid id)
{
    var {entity} = await dataContext.{Entities}
        .FirstOrDefaultAsync(b => b.{Entity}Id == id);
    // No ConfigureAwait needed in ASP.NET Core

    return {entity} is null ? null : new {Entity}Response({entity}.{Entity}Id, {entity}.Name, {entity}.Amount);
}
```

## Memory Optimization

### String Handling
```csharp
// ❌ BAD - String concatenation in loop
public string BuildCsv(List<{Entity}> {entities})
{
    string csv = "Id,Name,Amount\n";
    foreach (var {entity} in {entities})
    {
        csv += $"{entity}.{Entity}Id},{entity}.Name},{entity}.Amount}\n"; // Allocates new string each time
    }
    return csv;
}

// ✅ GOOD - Use StringBuilder
public string BuildCsv(List<{Entity}> {entities})
{
    var sb = new StringBuilder();
    sb.AppendLine("Id,Name,Amount");
    foreach (var {entity} in {entities})
    {
        sb.AppendLine($"{entity}.{Entity}Id},{entity}.Name},{entity}.Amount}");
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
public decimal CalculateTotal(List<{Entity}> {entities})
{
    var active = {entities}.Where(b => b.IsActive);
    var count = active.Count();
    var total = active.Sum(b => b.Amount);
    return total / count;
}

// ✅ GOOD - Single enumeration
public decimal CalculateAverage(List<{Entity}> {entities})
{
    var activeList = {entities}.Where(b => b.IsActive).ToList();
    return activeList.Sum(b => b.Amount) / activeList.Count;
}

// ✅ BETTER - Use aggregate functions
public decimal CalculateAverage(List<{Entity}> {entities})
{
    return {entities}.Where(b => b.IsActive).Average(b => b.Amount);
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
    var {entities}Task = GetUser{Entities}Async(userId);
    var {childEntities}Task = GetUser{ChildEntities}Async(userId);

    await Task.WhenAll({entities}Task, {childEntities}Task);

    return new DashboardData(
        await {entities}Task,
        await {childEntities}Task);
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

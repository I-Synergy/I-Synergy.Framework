# OWASP API Security Top 10 — .NET Patterns

One worked example per OWASP API Top 10 category, in this template's CQRS/OpenIddict idiom. Load this when hardening an endpoint, reviewing API authorization, or auditing coverage of the Top 10 list. The summary checklist lives in `SKILL.md`.

## OWASP API Security Top 10

### 1. Broken Object Level Authorization
```csharp
// ❌ WRONG - No authorization check
app.MapGet("/budgets/{id}", async (Guid id, IQueryHandler handler) =>
{
    var query = new GetBudgetByIdQuery(id);
    return await handler.HandleAsync(query);
});

// ✅ CORRECT - Verify user owns the resource
app.MapGet("/budgets/{id}", async (
    Guid id,
    IQueryHandler handler,
    ClaimsPrincipal user) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    var query = new GetBudgetByIdQuery(id);
    var budget = await handler.HandleAsync(query);

    if (budget.UserId.ToString() != userId)
        return Results.Forbid();

    return Results.Ok(budget);
}).RequireAuthorization();
```

### 2. Broken Authentication
```csharp
// ✅ CORRECT - Configure OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<DataContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token");

        options.AllowPasswordFlow()
            .AllowRefreshTokenFlow();

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String(configuration["OpenIddict:EncryptionKey"]!)));

        options.AddSigningKey(new SymmetricSecurityKey(
            Convert.FromBase64String(configuration["OpenIddict:SigningKey"]!)));

        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });
```

### 3. Broken Object Property Level Authorization
```csharp
// ❌ WRONG - Exposing internal properties
public record UserResponse(
    Guid UserId,
    string Email,
    string PasswordHash,  // ❌ NEVER expose
    string Role
);

// ✅ CORRECT - Only expose safe properties
public record UserResponse(
    Guid UserId,
    string Email,
    string Role
);
```

### 4. Unrestricted Resource Consumption
```csharp
// ✅ CORRECT - Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();

// Apply to specific endpoints
app.MapGet("/api/expensive-operation", async () =>
{
    // Handler logic
}).RequireRateLimiting("fixed");
```

### 5. Broken Function Level Authorization
```csharp
// ❌ WRONG - No role check for admin operation
app.MapDelete("/users/{id}", async (Guid id, ICommandHandler handler) =>
{
    var command = new DeleteUserCommand(id);
    return await handler.HandleAsync(command);
}).RequireAuthorization(); // Not enough!

// ✅ CORRECT - Require admin role
app.MapDelete("/users/{id}", async (Guid id, ICommandHandler handler) =>
{
    var command = new DeleteUserCommand(id);
    return await handler.HandleAsync(command);
}).RequireAuthorization(policy => policy.RequireRole("Admin"));
```

### 6. Unrestricted Access to Sensitive Business Flows
```csharp
// ✅ CORRECT - Implement business logic checks
public sealed class TransferFundsCommandHandler(
    DataContext dataContext,
    ILogger<TransferFundsCommandHandler> logger
) : ICommandHandler<TransferFundsCommand, TransferFundsResponse>
{
    public async Task<TransferFundsResponse> HandleAsync(
        TransferFundsCommand command,
        CancellationToken cancellationToken = default)
    {
        // Business rules
        var sourceAccount = await dataContext.Accounts.FirstOrDefaultAsync(
            account => account.AccountId == command.SourceAccountId, cancellationToken);

        if (sourceAccount is null)
            throw new AccountNotFoundException(command.SourceAccountId);

        if (sourceAccount.Balance < command.Amount)
            throw new InsufficientFundsException("Insufficient funds for transfer");

        if (command.Amount > 10000m)
            throw new BusinessRuleException("Transfers over $10,000 require approval");

        // Proceed with transfer
    }
}
```

### 7. Server Side Request Forgery (SSRF)
```csharp
// ❌ WRONG - User-provided URL without validation
app.MapPost("/fetch", async (string url) =>
{
    var client = new HttpClient();
    return await client.GetStringAsync(url); // ❌ Dangerous!
});

// ✅ CORRECT - Whitelist allowed domains
app.MapPost("/fetch", async (string url) =>
{
    var allowedDomains = new[] { "api.example.com", "trusted-service.com" };
    var uri = new Uri(url);

    if (!allowedDomains.Contains(uri.Host))
        return Results.BadRequest("Domain not allowed");

    var client = new HttpClient();
    return await client.GetStringAsync(url);
});
```

### 8. Security Misconfiguration
```csharp
// ✅ CORRECT - Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add(
        "Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'");
    context.Response.Headers.Add(
        "Strict-Transport-Security",
        "max-age=31536000; includeSubDomains");

    await next();
});

// ✅ CORRECT - CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", builder =>
    {
        builder
            .WithOrigins("https://yourdomain.com", "https://app.yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("AllowedOrigins");
```

### 9. Improper Inventory Management
```csharp
// ✅ CORRECT - API versioning
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

app.MapGet("/budgets/{id}", /* handler */)
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(1.0);

app.MapGet("/budgets/{id}", /* handler v2 */)
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(2.0);
```

### 10. Unsafe Consumption of APIs
```csharp
// ✅ CORRECT - Resilience for external APIs
builder.Services.AddHttpClient("external-api", client =>
{
    client.BaseAddress = new Uri("https://api.external.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
});

// Validate responses
public async Task<ExternalData> FetchExternalDataAsync(string id)
{
    var response = await _httpClient.GetAsync($"/data/{id}");

    if (!response.IsSuccessStatusCode)
    {
        _logger.LogWarning(
            "External API returned {StatusCode} for ID {Id}",
            response.StatusCode, id);
        throw new ExternalApiException("Failed to fetch external data");
    }

    var data = await response.Content.ReadFromJsonAsync<ExternalData>();

    // Validate response
    if (data is null || string.IsNullOrEmpty(data.Id))
        throw new InvalidDataException("Invalid response from external API");

    return data;
}
```

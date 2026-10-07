# API Authorization Patterns

Claims-based, policy-based, and resource-based authorization for Minimal API endpoints. Load this when adding a policy, a claim check, or a per-resource access check.

## Authorization Patterns

### Claims-Based Authorization
```csharp
// Add claims during authentication
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
    new(ClaimTypes.Email, user.Email),
    new(ClaimTypes.Role, user.Role),
    new("budget:read", "true"),
    new("budget:write", "true")
};

// Check claims at endpoint
app.MapPost("/budgets", async (/* params */) =>
{
    // Handler logic
}).RequireAuthorization(policy => policy.RequireClaim("budget:write"));
```

### Policy-Based Authorization
```csharp
// Define policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BudgetAdmin", policy =>
        policy.RequireRole("Admin")
              .RequireClaim("budget:admin"));

    options.AddPolicy("BudgetWrite", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("budget:write"));

    options.AddPolicy("Over18", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));
});

// Use policies
app.MapPost("/budgets", async (/* params */) =>
{
    // Handler logic
}).RequireAuthorization("BudgetWrite");
```

### Resource-Based Authorization
```csharp
public interface IAuthorizationService
{
    Task<bool> CanAccessBudgetAsync(Guid budgetId, string userId);
}

app.MapGet("/budgets/{id}", async (
    Guid id,
    IQueryHandler handler,
    IAuthorizationService authService,
    ClaimsPrincipal user) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    if (!await authService.CanAccessBudgetAsync(id, userId))
        return Results.Forbid();

    var query = new GetBudgetByIdQuery(id);
    return Results.Ok(await handler.HandleAsync(query));
}).RequireAuthorization();
```

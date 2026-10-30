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
    new("{entity}:read", "true"),
    new("{entity}:write", "true")
};

// Check claims at endpoint
app.MapPost("/{entities}", async (/* params */) =>
{
    // Handler logic
}).RequireAuthorization(policy => policy.RequireClaim("{entity}:write"));
```

### Policy-Based Authorization
```csharp
// Define policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("{Entity}Admin", policy =>
        policy.RequireRole("Admin")
              .RequireClaim("{entity}:admin"));

    options.AddPolicy("{Entity}Write", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("{entity}:write"));

    options.AddPolicy("Over18", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));
});

// Use policies
app.MapPost("/{entities}", async (/* params */) =>
{
    // Handler logic
}).RequireAuthorization("{Entity}Write");
```

### Resource-Based Authorization
```csharp
public interface IAuthorizationService
{
    Task<bool> CanAccess{Entity}Async(Guid {entity}Id, string userId);
}

app.MapGet("/{entities}/{id}", async (
    Guid id,
    IQueryHandler handler,
    IAuthorizationService authService,
    ClaimsPrincipal user) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    if (!await authService.CanAccess{Entity}Async(id, userId))
        return Results.Forbid();

    var query = new Get{Entity}ByIdQuery(id);
    return Results.Ok(await handler.HandleAsync(query));
}).RequireAuthorization();
```

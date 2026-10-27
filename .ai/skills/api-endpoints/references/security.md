# API Security Patterns

## Middleware Order (Program.cs)

```csharp
var app = builder.Build();

app.UseHttpsRedirection();   // 1. HTTPS first
app.UseCors();                // 2. CORS before auth
app.UseRateLimiter();         // 3. Rate limit before auth
app.UseAuthentication();      // 4. Auth
app.UseAuthorization();       // 5. Auth

app.Map{Entity}Endpoints("Bearer", "ApiKey");

app.Run();
```

## Input Validation

Every POST/PUT route must use Data Annotations on the request model and the `.WithValidation<T>()` filter.

**Model:**
```csharp
using System.ComponentModel.DataAnnotations;

public sealed record CreateEntity(
    [Required, MaxLength(100)] string Name,
    [Required, Range(1, 10000)] int Value,
    [MaxLength(500)] string? Description
);
```

**Validation filter:**
```csharp
// File: {ApplicationName}.Service.Api/Extensions/ValidationExtensions.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Microsoft.AspNetCore.Builder;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();
            if (argument is null)
                return TypedResults.BadRequest("Invalid request body");

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(argument, new ValidationContext(argument), results, true))
            {
                return TypedResults.ValidationProblem(
                    results.ToDictionary(
                        r => r.MemberNames.FirstOrDefault() ?? "Error",
                        r => new[] { r.ErrorMessage! }));
            }

            return await next(context);
        });

        return builder;
    }
}
```

**Route usage:**
```csharp
group.MapPost("", AddEntityAsync)
    .Accepts<CreateEntity>("application/json")
    .WithValidation<CreateEntity>()
    .ProducesValidationProblem();
```

## Rate Limiting

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", config =>
    {
        config.PermitLimit = 1000;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });
});

var app = builder.Build();
app.UseRateLimiter();
```

Per-route override:
```csharp
group.MapPost("login", LoginAsync)
    .RequireRateLimiting("strict");  // Stricter policy for auth endpoints
```

## CORS

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://app.yourdomain.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();
```

Rules:
- Never combine `.AllowAnyOrigin()` with `.AllowCredentials()` — they're mutually exclusive
- Always specify explicit origins; never use `AllowAnyOrigin()` in production

## HTTPS

```csharp
app.UseHttpsRedirection();
```

Mandatory for all environments. Aspire handles TLS at the infrastructure level, but `UseHttpsRedirection` remains for all other scenarios (direct Kestrel, non-Aspire deployments).

## Authorization

**Group-level default:**
```csharp
var group = app
    .MapGroup("/{entities}")
    .RequireAuthorization(options =>
    {
        options.RequireAuthenticatedUser();
        options.AddAuthenticationSchemes(authenticationSchemes);
    });
```

**Per-endpoint role-based:**
```csharp
group.MapGet("accounts", GetAccountsAsync)
    .RequireAuthorization(options =>
    {
        options.RequireAuthenticatedUser();
        options.RequireRole(nameof(Roles.Admin));
    });
```

**Anti-forgery (for cookie-based auth):**
```csharp
group.MapPost("verify/accept", VerifyAcceptAsync)
    .RequireAuthorization(options => options.RequireAuthenticatedUser())
    .ValidateAntiforgeryToken();
```

## Quick Checklist

- [ ] Validation filter on all POST/PUT routes
- [ ] Rate limiter configured and middleware registered
- [ ] CORS with explicit origins (not AllowAnyOrigin)
- [ ] HTTPS redirection enabled
- [ ] Group-level RequireAuthorization with auth schemes
- [ ] Middleware order: HTTPS → CORS → RateLimiter → Auth
- [ ] Anti-forgery on state-changing cookie-authenticated routes

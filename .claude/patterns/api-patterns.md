# API Endpoint Patterns

Patterns for implementing Minimal API endpoints that delegate to CQRS handlers.
Based on the reference implementation in `ISynergy.Service.Api/Endpoints/`.

## Core Principles

- **Static extension methods** — endpoints are static classes with named methods, not inline lambdas
- **Named method references** — route registrations reference method names (`group.MapPost("", AddEntityAsync)`)
- **Explicit parameter binding** — `[FromServices]`, `[FromBody]`, `[FromRoute]`, `[FromQuery]` on all parameters
- **Typed results** — return `Results<T1, T2>` with `TypedResults.*` factory methods
- **Version set created inside** — each endpoint class creates its own `ApiVersionSet`
- **Authentication schemes passed in** — via `params string[] authenticationSchemes`

---

## Complete Endpoint Example

```csharp
// File: {ApplicationName}.Services.{Domain}/Endpoints/v1/{Entity}Endpoints.cs

namespace {ApplicationName}.Services.{Domain}.Endpoints.v1;

using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Defines endpoints for {Entity} operations.
/// </summary>
public static class {Entity}Endpoints
{
    /// <summary>
    /// Maps all {entity} endpoints to the application route builder.
    /// </summary>
    public static void Map{Entity}Endpoints(
        this IEndpointRouteBuilder app,
        params string[] authenticationSchemes)
    {
        var version = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app
            .MapGroup("/{entities}")
            .WithDisplayName("{Entities}")
            .WithTags("{Entities}")
            .WithGroupName("v1")
            .WithApiVersionSet(version)
            .RequireAuthorization(options =>
            {
                options.RequireAuthenticatedUser();
                options.AddAuthenticationSchemes(authenticationSchemes);
            });

        // POST: Create
        group.MapPost("", Add{Entity}Async)
            .WithSummary("Create a new {entity}")
            .WithDescription("Creates a new {entity} with the provided details")
            .Accepts<{Entity}>("application/json")
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        // GET: Get by ID
        group.MapGet("{id}", Get{Entity}ByIdAsync)
            .WithSummary("Get {entity} by ID")
            .WithDescription("Retrieves a specific {entity} by its unique identifier")
            .Produces(StatusCodes.Status401Unauthorized);

        // GET: Get list
        group.MapGet("", Get{Entity}ListAsync)
            .WithSummary("Get all {entities}")
            .WithDescription("Retrieves a paginated list of {entities}")
            .Produces(StatusCodes.Status401Unauthorized);

        // PUT: Update
        group.MapPut("", Update{Entity}Async)
            .WithSummary("Update an existing {entity}")
            .WithDescription("Updates an existing {entity} with the provided details")
            .Accepts<{Entity}>("application/json")
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        // DELETE: Delete
        group.MapDelete("{id}", Remove{Entity}Async)
            .WithSummary("Delete a {entity}")
            .WithDescription("Deletes a {entity} by its unique identifier")
            .Produces(StatusCodes.Status401Unauthorized);
    }

    #region {Entity} Methods

    /// <summary>
    /// Creates a new {entity}.
    /// </summary>
    public static async Task<Results<Created<Guid>, BadRequest>> Add{Entity}Async(
        [FromServices] ICommandHandler<Create{Entity}Command, Create{Entity}Response> handler,
        [FromBody] {Entity} e,
        CancellationToken cancellationToken = default)
    {
        var command = new Create{Entity}Command(
            e.Property1,
            e.Property2,
            e.Property3);

        var result = await handler.HandleAsync(command, cancellationToken);

        return TypedResults.Created($"/{entities}/{result.{Entity}Id}", result.{Entity}Id);
    }

    /// <summary>
    /// Retrieves a {entity} by its unique identifier.
    /// </summary>
    public static async Task<Results<Ok<{Entity}>, NotFound>> Get{Entity}ByIdAsync(
        [FromServices] IQueryHandler<Get{Entity}ByIdQuery, Get{Entity}ByIdResponse> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new Get{Entity}ByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.{Entity} is not null)
            return TypedResults.Ok(result.{Entity});

        return TypedResults.NotFound();
    }

    /// <summary>
    /// Retrieves a paginated list of {entities}.
    /// </summary>
    public static async Task<Ok<List<{Entity}>>> Get{Entity}ListAsync(
        [FromServices] IQueryHandler<Get{Entity}ListQuery, Get{Entity}ListResponse> handler,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = GenericConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new Get{Entity}ListQuery(pageIndex, pageSize);
        var result = await handler.HandleAsync(query, cancellationToken);
        return TypedResults.Ok(result.{Entities} ?? []);
    }

    /// <summary>
    /// Updates an existing {entity}.
    /// </summary>
    public static async Task<Results<Ok<bool>, BadRequest>> Update{Entity}Async(
        [FromServices] ICommandHandler<Update{Entity}Command, Update{Entity}Response> handler,
        [FromBody] {Entity} e,
        CancellationToken cancellationToken = default)
    {
        var command = new Update{Entity}Command(
            e.{Entity}Id,
            e.Property1,
            e.Property2);

        var result = await handler.HandleAsync(command, cancellationToken);

        return TypedResults.Ok(result.Success);
    }

    /// <summary>
    /// Deletes a {entity} by its unique identifier.
    /// </summary>
    public static async Task<Results<NoContent, NotFound>> Remove{Entity}Async(
        [FromServices] ICommandHandler<Delete{Entity}Command, Delete{Entity}Response> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new Delete{Entity}Command(id);
        await handler.HandleAsync(command, cancellationToken);

        return TypedResults.NoContent();
    }

    #endregion
}
```

---

## HTTP Method Patterns

| Operation | HTTP Method | Route | Return Type | Status Codes |
|-----------|-------------|-------|-------------|--------------|
| **Create** | POST | `""` | `Results<Created<Guid>, BadRequest>` | 201, 400, 401 |
| **Read Single** | GET | `"{id}"` | `Results<Ok<T>, NotFound>` | 200, 404, 401 |
| **Read List** | GET | `""` | `Ok<List<T>>` | 200, 401 |
| **Update** | PUT | `""` | `Results<Ok<bool>, BadRequest>` | 200, 400, 401 |
| **Delete** | DELETE | `"{id}"` | `Results<NoContent, NotFound>` | 204, 404, 401 |
| **Search** | GET | `"search"` | `Ok<List<T>>` | 200, 401 |
| **Download** | GET | `"{id}/download"` | `Results<FileContentHttpResult, NotFound>` | 200, 404, 401 |
| **Count** | GET | `"count"` | `Ok<int>` | 200, 401 |

---

## Route Registration

### Route Group Setup

```csharp
// Version set created INSIDE the endpoint class
var version = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

// Group with full configuration
var group = app
    .MapGroup("/{entities}")
    .WithDisplayName("{Entities}")        // OpenAPI display name
    .WithTags("{Entities}")               // OpenAPI grouping tag
    .WithGroupName("v1")                  // API version group
    .WithApiVersionSet(version)           // Attach version set
    .RequireAuthorization(options =>       // Default auth for all routes
    {
        options.RequireAuthenticatedUser();
        options.AddAuthenticationSchemes(authenticationSchemes);
    });
```

### Route Mapping — Named Methods (NOT Lambdas)

```csharp
// CORRECT: Reference named static methods
group.MapPost("", Add{Entity}Async)
    .WithSummary("Create a new {entity}")
    .Accepts<{Entity}>("application/json")
    .ProducesValidationProblem();

// WRONG: Inline lambda (do NOT use this pattern)
group.MapPost("/", async ({Entity} model, ...) => { ... })
    .WithName("Create{Entity}")
    .WithOpenApi();
```

### Nested/Sub-Entity Routes

```csharp
// Sub-entity routes using nameof() for type safety
group.MapPost($"{nameof(SubEntity)}", AddSubEntityAsync)
    .WithSummary("Create a new sub-entity");

// Parameterized nested routes
group.MapGet($"{nameof(SubEntity)}/{nameof(ParentEntity)}/{{id}}/list", GetSubEntitiesFromParentAsync)
    .WithSummary("Get sub-entities for parent");
```

---

## Parameter Binding

### Explicit Attribute Binding

```csharp
// All parameters must use explicit binding attributes
public static async Task<Results<Created<Guid>, BadRequest>> Add{Entity}Async(
    [FromServices] ICommandHandler<Create{Entity}Command, Create{Entity}Response> handler,
    [FromBody] {Entity} e,
    CancellationToken cancellationToken = default)
```

### Route Parameters

```csharp
public static async Task<Results<Ok<{Entity}>, NotFound>> Get{Entity}ByIdAsync(
    [FromServices] IQueryHandler<Get{Entity}ByIdQuery, Get{Entity}ByIdResponse> handler,
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
```

### Query Parameters

```csharp
public static async Task<Ok<List<{Entity}>>> Get{Entity}ListAsync(
    [FromServices] IQueryHandler<Get{Entity}ListQuery, Get{Entity}ListResponse> handler,
    [FromQuery] int pageIndex = 0,
    [FromQuery] int pageSize = GenericConstants.DefaultPageSize,
    CancellationToken cancellationToken = default)
```

### Multiple Service Injection

```csharp
public static async Task<Results<Ok<List<Account>>, NotFound>> GetAccountsAsync(
    [FromServices] IQueryHandler<GetAccountsQuery, List<Account>> handler,
    [FromServices] IHttpContextAccessor context,
    CancellationToken cancellationToken = default)
```

---

## CQRS Integration

### Command Dispatching (Individual Parameters Only)

```csharp
// CORRECT: Extract properties from request model into command parameters
var command = new Create{Entity}Command(
    e.Property1,
    e.Property2,
    e.Property3);

var result = await handler.HandleAsync(command, cancellationToken);

// WRONG: Passing model object directly
var command = new Create{Entity}Command(e);
```

### Query Dispatching with Named Parameters

```csharp
// Named parameters for optional filters
var query = new GetDepositsTotalAmountQuery(GoalId: id);
var query = new GetDepositsTotalAmountQuery(BudgetId: id);

// Positional parameters for simple queries
var query = new Get{Entity}ByIdQuery(id);
```

---

## TypedResults Return Patterns

### Success Responses

```csharp
TypedResults.Ok(result)                                     // 200 OK
TypedResults.Created($"/route/{id}", result.{Entity}Id)     // 201 Created
TypedResults.NoContent()                                    // 204 No Content
TypedResults.File(content, "application/octet-stream")      // 200 with file
```

### Error Responses

```csharp
TypedResults.NotFound()                      // 404
TypedResults.BadRequest()                    // 400
TypedResults.BadRequest("error message")     // 400 with message
TypedResults.Unauthorized()                  // 401
```

### Return Type Signatures

```csharp
// Single result type (no error path)
Task<Ok<List<{Entity}>>>

// Multiple possible results
Task<Results<Ok<{Entity}>, NotFound>>
Task<Results<Created<Guid>, BadRequest>>
Task<Results<NoContent, NotFound>>

// File download
Task<Results<FileContentHttpResult, NotFound>>
```

---

## Authorization Patterns

### Group-Level Default Authorization

```csharp
var group = app
    .MapGroup("/{entities}")
    .RequireAuthorization(options =>
    {
        options.RequireAuthenticatedUser();
        options.AddAuthenticationSchemes(authenticationSchemes);
    });
```

### Per-Endpoint Role-Based Authorization

```csharp
group.MapGet("accounts", GetAccountsAsync)
    .RequireAuthorization(options =>
    {
        options.RequireAuthenticatedUser();
        options.RequireRole(
            nameof(Roles.LicenseManager),
            nameof(Roles.LicenseAdministrator));
    })
    .WithSummary("Get all accounts");
```

### Accessing User Context in Endpoints

```csharp
public static async Task<Results<Created<string>, UnauthorizedHttpResult>> CreateApiKeyAsync(
    [FromServices] ICommandHandler<CreateApiKeyCommand, CreateApiKeyResponse> handler,
    [FromServices] IHttpContextAccessor context,
    CancellationToken cancellationToken = default)
{
    if (context.HttpContext?.User is null)
        return TypedResults.Unauthorized();

    var userIdClaim = context.HttpContext.User.FindFirst(
        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        return TypedResults.Unauthorized();

    var command = new CreateApiKeyCommand(userId);
    var response = await handler.HandleAsync(command, cancellationToken);

    return TypedResults.Created(string.Empty, response.ApiKey);
}
```

### Anti-Forgery Protection

```csharp
group.MapPost("verify/accept", VerifyAcceptAsync)
    .RequireAuthorization(options => options.RequireAuthenticatedUser())
    .ValidateAntiforgeryToken()
    .WithSummary("Accept authorization");
```

---

## OpenAPI Configuration

### Route Metadata

```csharp
group.MapPost("", Add{Entity}Async)
    .WithSummary("Create a new {entity}")                       // Short title
    .WithDescription("Creates a new {entity} with details")     // Detailed description
    .Accepts<{Entity}>("application/json")                      // Expected input type
    .Produces(StatusCodes.Status401Unauthorized)                // Possible response codes
    .ProducesValidationProblem();                               // 400 with validation errors
```

### Do NOT Use

```csharp
// WRONG: These are the old Swashbuckle patterns
.WithName("Create{Entity}")    // Not used
.WithOpenApi()                 // Not used
```

---

## File Download Pattern

```csharp
// Route registration
group.MapGet($"{nameof(Document)}/{{id}}/download", Download{Entity}DocumentByIdAsync)
    .WithSummary("Download {entity} document")
    .Produces(StatusCodes.Status401Unauthorized);

// Handler method
public static async Task<Results<FileContentHttpResult, NotFound>> Download{Entity}DocumentByIdAsync(
    [FromServices] IQueryHandler<DownloadDocumentQuery, DownloadDocumentResponse> handler,
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
{
    var query = new DownloadDocumentQuery(id);
    var result = await handler.HandleAsync(query, cancellationToken);

    if (result.FileContent is not null)
        return TypedResults.File(result.FileContent, "application/octet-stream");

    return TypedResults.NotFound();
}
```

---

## Endpoint Registration in Program.cs

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

using {ApplicationName}.Services.{Domain}.Endpoints.v1;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.With{Domain}Services();

var app = builder.Build();

// Map endpoints (authentication schemes passed here)
app.Map{Entity}Endpoints("Bearer", "ApiKey");

app.Run();
```

---

## Service Registration

```csharp
// File: {ApplicationName}.Services.{Domain}/Extensions/ServiceCollectionExtensions.cs

namespace {ApplicationName}.Services.{Domain}.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection With{Domain}Services(
        this IServiceCollection services)
    {
        // Register domain handlers (CQRS + Mapster configs)
        services.With{Domain}DomainHandlers();

        return services;
    }
}
```

---

## Multi-Entity Endpoint Organization

When a single domain has multiple entities, organize with `#region` blocks:

```csharp
public static class BudgetEndpoints
{
    public static void MapBudgetEndpoints(
        this IEndpointRouteBuilder app,
        params string[] authenticationSchemes)
    {
        // ... version set and group setup ...

        // Budget routes
        group.MapPost("", AddBudgetAsync);
        group.MapGet("{id}", GetBudgetByIdAsync);
        group.MapPut("", UpdateBudgetAsync);
        group.MapDelete("{id}", RemoveBudgetAsync);

        // Debt sub-entity routes
        group.MapPost($"{nameof(Debt)}", AddDebtAsync);
        group.MapGet($"{nameof(Debt)}/{nameof(Budget)}/{{id}}/list", GetDebtsFromBudgetAsync);
        group.MapPut($"{nameof(Debt)}", UpdateDebtAsync);
        group.MapDelete($"{nameof(Debt)}/{{id}}", RemoveDebtAsync);

        // Expense sub-entity routes
        group.MapPost($"{nameof(Expense)}", AddExpenseAsync);
        group.MapGet($"{nameof(Expense)}/{nameof(Budget)}/{{id}}/list", GetExpensesFromBudgetAsync);
    }

    #region Budget Methods
    public static async Task<Results<Created<Guid>, BadRequest>> AddBudgetAsync(...) { ... }
    public static async Task<Results<Ok<Budget>, NotFound>> GetBudgetByIdAsync(...) { ... }
    #endregion

    #region Debt Methods
    public static async Task<Results<Created<Guid>, BadRequest>> AddDebtAsync(...) { ... }
    public static async Task<Ok<List<Debt>>> GetDebtsFromBudgetAsync(...) { ... }
    #endregion

    #region Expense Methods
    public static async Task<Results<Created<Guid>, BadRequest>> AddExpenseAsync(...) { ... }
    #endregion
}
```

---

## Key Conventions Summary

1. **Named static methods** — never inline lambdas
2. **`[FromServices]` injection** — handler interfaces injected per method
3. **`TypedResults.*`** — not `Results.*` (typed return values)
4. **Individual command parameters** — never pass model objects to commands
5. **Named query parameters** — use `GoalId: id` syntax for clarity
6. **Version set inside method** — not passed as parameter
7. **`params string[] authenticationSchemes`** — auth schemes passed from Program.cs
8. **`.WithSummary()` + `.WithDescription()`** — not `.WithName()` + `.WithOpenApi()`
9. **`.Accepts<T>("application/json")`** — for POST/PUT body types
10. **`CancellationToken cancellationToken = default`** — on all async methods
11. **`nameof()` for nested routes** — type-safe sub-entity route segments
12. **`#region` blocks** — organize methods by entity type

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
            .WithValidation<{Entity}>()
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        // GET: Get by ID
        group.MapGet("{id}", Get{Entity}ByIdAsync)
            .WithSummary("Get {entity} by ID")
            .WithDescription("Retrieves a specific {entity} by its unique identifier")
            .Produces<{Entity}>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        // GET: Get list
        group.MapGet("", Get{Entity}ListAsync)
            .WithSummary("Get all {entities}")
            .WithDescription("Retrieves a paginated list of {entities}")
            .Produces<List<{Entity}>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // PUT: Update
        group.MapPut("", Update{Entity}Async)
            .WithSummary("Update an existing {entity}")
            .WithDescription("Updates an existing {entity} with the provided details")
            .Accepts<{Entity}>("application/json")
            .WithValidation<{Entity}>()
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        // DELETE: Delete
        group.MapDelete("{id}", Remove{Entity}Async)
            .WithSummary("Delete a {entity}")
            .WithDescription("Deletes a {entity} by its unique identifier")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
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
    .WithValidation<{Entity}>()
    .Produces<Guid>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status401Unauthorized)
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

### Input Validation (Data Annotations)

Use Data Annotations on request models and a reusable validation filter. Every POST/PUT route that accepts a body must include `.ProducesValidationProblem()` and the validation filter.

**Model with validation attributes:**

```csharp
// File: {ApplicationName}.Models.{Domain}/{Entity}.cs

using System.ComponentModel.DataAnnotations;

public sealed record {Entity}
{
    [Required, MaxLength(100)]
    public string Property1 { get; init; }

    [Required, Range(1, 10000)]
    public int Property2 { get; init; }

    [MaxLength(500)]
    public string? Property3 { get; init; }
}
```

**Reusable validation filter:**

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

**Usage on the route:**

```csharp
group.MapPost("", Add{Entity}Async)
    .Accepts<{Entity}>("application/json")
    .WithValidation<{Entity}>()
    .ProducesValidationProblem();
```

The filter runs before the endpoint. If validation fails, it returns a 400 with the validation errors — matching the `.ProducesValidationProblem()` declaration. If it passes, the endpoint executes normally.

### Rate Limiting

All API endpoints must be protected by rate limiting to prevent abuse and DoS:

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

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

Per-route policy override when needed:

```csharp
group.MapPost("", Add{Entity}Async)
    .RequireRateLimiting("api");
```

### CORS

Cross-Origin Resource Sharing must be explicitly configured for web clients:

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://{ApplicationName}.App.Api")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
```

**Rules:**
- Never use `.AllowAnyOrigin()` with `AllowCredentials()` — they're mutually exclusive
- Always specify explicit origins; never use `AllowAnyOrigin()` in production
- Call `UseCors()` after `UseRouting()` but before `UseAuthorization()` and `UseRateLimiter()`

### Middleware Order

The middleware pipeline in Program.cs must follow this order:

```csharp
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.Map{Entity}Endpoints("Bearer", "ApiKey");

app.Run();
```

### HTTPS Enforcement

HTTPS redirection is mandatory for all environments:

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

var app = builder.Build();

app.UseHttpsRedirection();
```

Note: .NET Aspire handles TLS termination at the infrastructure level; `UseHttpsRedirection` remains for non-Aspire scenarios.

---

## OpenAPI Configuration

### Route Metadata

Every route MUST declare all possible response types via `.Produces<T>()` or `.Produces(statusCode)`. This includes both success and error codes. Never rely on inference — OpenAPI/Swagger needs explicit metadata for accurate documentation.

```csharp
group.MapPost("", Add{Entity}Async)
    .WithSummary("Create a new {entity}")                       // Short title
    .WithDescription("Creates a new {entity} with details")     // Detailed description
    .Accepts<{Entity}>("application/json")                      // Expected input type
    .WithValidation<{Entity}>()                                 // Data Annotations validation
    .Produces<Guid>(StatusCodes.Status201Created)               // Success (created)
    .Produces(StatusCodes.Status401Unauthorized)                // Auth failure
    .ProducesValidationProblem();                               // 400 with validation errors

group.MapGet("{id}", Get{Entity}ByIdAsync)
    .WithSummary("Get {entity} by ID")
    .WithDescription("Retrieves a specific {entity} by its unique identifier")
    .Produces<{Entity}>(StatusCodes.Status200OK)                // Success
    .Produces(StatusCodes.Status401Unauthorized)                // Auth failure
    .Produces(StatusCodes.Status404NotFound);                   // Not found

group.MapGet("", Get{Entity}ListAsync)
    .WithSummary("Get all {entities}")
    .WithDescription("Retrieves a paginated list of {entities}")
    .Produces<List<{Entity}>>(StatusCodes.Status200OK)          // Success
    .Produces(StatusCodes.Status401Unauthorized);               // Auth failure

group.MapPut("", Update{Entity}Async)
    .WithSummary("Update an existing {entity}")
    .WithDescription("Updates an existing {entity} with the provided details")
    .Accepts<{Entity}>("application/json")
    .WithValidation<{Entity}>()
    .Produces<bool>(StatusCodes.Status200OK)                    // Success
    .Produces(StatusCodes.Status401Unauthorized)                // Auth failure
    .ProducesValidationProblem();                               // 400 with validation errors

group.MapDelete("{id}", Remove{Entity}Async)
    .WithSummary("Delete a {entity}")
    .WithDescription("Deletes a {entity} by its unique identifier")
    .Produces(StatusCodes.Status204NoContent)                   // Success (no content)
    .Produces(StatusCodes.Status401Unauthorized)                // Auth failure
    .Produces(StatusCodes.Status404NotFound);                   // Not found
```

### Produces Metadata — Required

- **Every route** must explicitly declare every status code it can return, both success and failure
- Use `.Produces<T>(statusCode)` for typed success responses (the `T` matches the return type's success branch)
- Use `.Produces(statusCode)` for untyped responses (204, 401, 404, etc.)
- Use `.ProducesValidationProblem()` for automatic 400 validation error handling
- Do NOT omit success codes — OpenAPI generators cannot infer them from `Results<T1, T2, ...>`

### Project Configuration for OpenAPI Document Generation

Every API project `.csproj` must include the MSBuild properties and package reference to generate the OpenAPI spec at build time:

```xml
<!-- File: {ApplicationName}.Service.Api/{ApplicationName}.Service.Api.csproj -->

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <OpenApiDocumentsDirectory>../../openapi</OpenApiDocumentsDirectory>
    <OpenApiGenerateDocuments>true</OpenApiGenerateDocuments>
    <OpenApiGenerateDocumentsOnBuild>true</OpenApiGenerateDocumentsOnBuild>
    <OpenApiGenerateEnvironment>Development</OpenApiGenerateEnvironment>
  </PropertyGroup>

  <!-- ... other PropertyGroups and ItemGroups ... -->

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.ApiDescription.Server">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <!-- Generate Kiota client from OpenAPI spec on every build -->
  <Target Name="OpenAPI" AfterTargets="Build">
    <Exec Command="dotnet kiota generate -l CSharp --output ../{ApplicationName}.Clients.Api --namespace-name {ApplicationName}.Clients.Api --class-name ApiClient --exclude-backward-compatible --openapi ../../openapi/{ApplicationName}.Api.json" WorkingDirectory="$(ProjectDir)" />
  </Target>

</Project>
```

- `OpenApiDocumentsDirectory` — relative path from the project file to the output directory (typically `../../openapi` for a solution layout)
- `OpenApiGenerateDocumentsOnBuild` — ensures the spec stays in sync with every build
- `Microsoft.Extensions.ApiDescription.Server` — the MSBuild-integrated generator; PrivateAssets prevents it from leaking to consumers
- The generated `{ProjectName}.json` OpenAPI spec lands in the specified directory on every build
- The `OpenAPI` MSBuild target — runs `dotnet kiota generate` after every build, regenerating the client from the latest spec

### Document Transformer: Servers Entry

Without a `servers` entry, the generated OpenAPI spec has no base URL — Kiota clients won't know where to send requests. Always add a document transformer that sets the server URL:

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = [new OpenApiServer { Url = "https://api" }];
        return Task.CompletedTask;
    });
});
```

The URL uses the .NET Aspire service discovery short name — `https://api`. Aspire resolves this at runtime to the actual endpoint, so no configuration or environment-specific URLs are needed.

### Schema Transformers for Accurate Type Mapping

**Without schema transformers, Kiota generates all model properties as `UntypedNode` instead of their actual types.** The default OpenAPI generation infers types incorrectly for many .NET primitives. Always add a schema transformer in `Program.cs` (or via `.AddOpenApi()`) that explicitly maps every .NET type to its correct JSON schema representation:

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

builder.Services.AddOpenApi(options =>
{
    // ... document transformer (servers) goes here ...

    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        var propertyType = context.JsonPropertyInfo?.PropertyType;

        if (propertyType == null)
            return Task.CompletedTask;

        var nullableType = Nullable.GetUnderlyingType(propertyType);
        var actualType = nullableType ?? propertyType;

        // Date and Time types
        if (actualType == typeof(DateTimeOffset))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
        }
        else if (actualType == typeof(DateTime))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
        }
        else if (actualType == typeof(DateOnly))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date";
        }
        else if (actualType == typeof(TimeOnly))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "time";
        }
        else if (actualType == typeof(TimeSpan))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "duration";
        }

        // Numeric types
        else if (actualType == typeof(decimal))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "double";
        }
        else if (actualType == typeof(double))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "double";
        }
        else if (actualType == typeof(float))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "float";
        }

        // Integer types
        else if (actualType == typeof(int))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
        }
        else if (actualType == typeof(long))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int64";
        }
        else if (actualType == typeof(short))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int16";
        }
        else if (actualType == typeof(byte))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int8";
            schema.Minimum = "0";
            schema.Maximum = "255";
        }
        else if (actualType == typeof(sbyte))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int8";
            schema.Minimum = "-128";
            schema.Maximum = "127";
        }
        else if (actualType == typeof(uint))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
            schema.Minimum = "0";
        }
        else if (actualType == typeof(ulong))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int64";
            schema.Minimum = "0";
        }
        else if (actualType == typeof(ushort))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int16";
            schema.Minimum = "0";
        }

        // String and identifier types
        else if (actualType == typeof(Guid))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "uuid";
        }
        else if (actualType == typeof(string))
        {
            schema.Type = JsonSchemaType.String;
        }
        else if (actualType == typeof(char))
        {
            schema.Type = JsonSchemaType.String;
            schema.MinLength = 1;
            schema.MaxLength = 1;
        }

        // Boolean type
        else if (actualType == typeof(bool))
        {
            schema.Type = JsonSchemaType.Boolean;
        }

        // Binary types
        else if (actualType == typeof(byte[]))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "byte";
        }

        // URI type
        else if (actualType == typeof(Uri))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "uri";
        }

        // Handle enums
        else if (actualType.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            var enumNames = Enum.GetNames(actualType);
            schema.Enum = enumNames
                .Select(name => System.Text.Json.Nodes.JsonValue.Create(name)!)
                .Cast<System.Text.Json.Nodes.JsonNode>()
                .ToList();
        }

        return Task.CompletedTask;
    });
});
```

This transformer is non-negotiable — without it, the OpenAPI spec omits type information and Kiota falls back to `UntypedNode` for every property, making the generated client useless.

### Do NOT Use

```csharp
// WRONG: These are the old Swashbuckle patterns
.WithName("Create{Entity}")    // Not used
.WithOpenApi()                 // Not used
```

---

## Kiota API Client Generation

[Kiota](https://learn.microsoft.com/en-us/openapi/kiota/) generates strongly-typed API clients from the OpenAPI spec produced at build time. Every API project that exposes endpoints needs a corresponding client project so consumers get compile-time safety instead of raw `HttpClient` calls.

### Naming Convention

For a **single-app** project, the client is simply `{ApplicationName}.Clients.Api`:

| Consuming App | | Kiota Client |
|---------------|---|--------------|
| `{ApplicationName}.App.Api` | → | `{ApplicationName}.Clients.Api` |

For **multi-app** projects, each consuming app gets its own client by swapping `App` for `Clients`:

| Consuming App | | Kiota Client |
|---------------|---|--------------|
| `{ApplicationName}.App.Business` | → | `{ApplicationName}.Clients.Business` |
| `{ApplicationName}.App.Business.Web` | → | `{ApplicationName}.Clients.Business.Web` |
| `{ApplicationName}.App.Business.Maui` | → | `{ApplicationName}.Clients.Business.Maui` |

Pattern: `{ApplicationName}.Clients.Api` for single-app; `{ApplicationName}.Clients.{AppName}` for multi-app.

### Solution Layout

```
solution/
├── openapi/                                    # Generated OpenAPI specs
│   └── {ApplicationName}.Service.Api.json
├── src/
│   ├── {ApplicationName}.Service.Api/          # API project (produces spec)
│   │   └── {ApplicationName}.Service.Api.csproj
│   ├── {ApplicationName}.App.Business/         # Consuming app (Blazor Server)
│   ├── {ApplicationName}.App.Business.Web/     # Consuming app (WebAssembly)
│   ├── {ApplicationName}.Clients.Api/          # Default client (single-app)
│   │   ├── {ApplicationName}.Clients.Api.csproj
│   │   ├── ApiClient.cs                        # Kiota-generated entry point
│   │   ├── Api/                                # Kiota-generated request builders
│   │   ├── Models/                             # Kiota-generated models
│   │   └── kiota-lock.json                     # Generation lock file
│   ├── {ApplicationName}.Clients.Business/     # Client for App.Business
│   │   ├── {ApplicationName}.Clients.Business.csproj
│   │   ├── ApiClient.cs
│   │   ├── Api/
│   │   ├── Models/
│   │   └── kiota-lock.json
│   └── {ApplicationName}.Clients.Business.Web/ # Client for App.Business.Web
│       └── ...
```

### Client Project .csproj

```xml
<!-- File: {ApplicationName}.Clients.Api/{ApplicationName}.Clients.Api.csproj -->

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Http" />
    <PackageReference Include="Microsoft.Kiota.Bundle" />
  </ItemGroup>

</Project>
```

### Setup: Install Kiota as a Local Tool

From the solution root, create the tool manifest (if it doesn't exist) and install Kiota:

```bash
dotnet new tool-manifest          # Creates .config/dotnet-tools.json (skip if it exists)
dotnet tool install Microsoft.OpenApi.Kiota
```

This pins the Kiota CLI version in `.config/dotnet-tools.json`:

```json
// File: .config/dotnet-tools.json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "microsoft.openapi.kiota": {
      "version": "1.31.1",
      "commands": [
        "kiota"
      ],
      "rollForward": false
    }
  }
}
```

- `rollForward: false` — ensures the exact pinned version is used; no automatic upgrades
- The manifest is **committed** to source control
- Run `dotnet tool restore` to install the pinned version on a fresh checkout

### Client Project Structure

```
{ApplicationName}.Clients.Api/
├── {ApplicationName}.Clients.Api.csproj
├── ApiClient.cs               # Kiota-generated entry point (BaseRequestBuilder)
├── Api/                        # Kiota-generated request builders
├── Models/                     # Kiota-generated models
├── Extensions/                 # DI registration (hand-written)
└── kiota-lock.json             # Generation lock file
```

### Client Registration in DI

```csharp
// File: {ApplicationName}.Clients.Api/Extensions/ServiceCollectionExtensions.cs

namespace {ApplicationName}.Clients.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(
        this IServiceCollection services,
        string baseUrl)
    {
        services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();

        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddScoped(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(nameof(ApiClient));
            var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
            var adapter = new HttpClientRequestAdapter(authProvider)
            {
                BaseUrl = httpClient.BaseAddress!.ToString()
            };
            return new ApiClient(adapter);
        });

        return services;
    }
}
```

### Authenticated Client Registration

When the target API requires authentication (Bearer token, API key), use the appropriate auth provider:

```csharp
// Bearer token (delegating user identity or client credentials)
services.AddSingleton<IAuthenticationProvider>(sp =>
    new BaseBearerTokenAuthenticationProvider(
        new TokenProvider(token)));

// API key (header-based)
services.AddSingleton<IAuthenticationProvider>(
    new ApiKeyAuthenticationProvider(
        apiKey, "X-Api-Key", KeyLocation.Header));
```

### Using the Generated Client from a Handler

```csharp
// File: {ApplicationName}.Services.{CallerDomain}/Queries/Get{Entity}List/Get{Entity}ListHandler.cs

public sealed class Get{Entity}ListQueryHandler(
    ApiClient client
) : IQueryHandler<Get{Entity}ListQuery, Get{Entity}ListResponse>
{
    public async Task<Get{Entity}ListResponse> HandleAsync(
        Get{Entity}ListQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await client.{Entities}.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Page = query.PageIndex;
            requestConfiguration.QueryParameters.PageSize = query.PageSize;
        }, cancellationToken);

        return new Get{Entity}ListResponse(
            result?.Select(e => /* map to model */).ToList() ?? []);
    }
}
```

### Key Rules

- **Default client** — `{ApplicationName}.Clients.Api` is the default for single-app projects
- **Multi-app clients** — swap `App` for `Clients`: `{ApplicationName}.App.Business` → `{ApplicationName}.Clients.Business`
- **Kiota version pinned in `dotnet-tools.json`** — `rollForward: false` ensures every developer and CI uses the exact same version
- **Client generated via MSBuild target** — the API project's `.csproj` includes an `OpenAPI` target that runs `dotnet kiota generate` after every build
- **Client source is committed** — `Api/`, `Models/`, `ApiClient.cs`, and `kiota-lock.json` are all committed as the client project's source code
- **`Microsoft.Kiota.Bundle`** — single metapackage covering Abstractions, HttpClientLibrary, and all serializers
- **Client registration is a consuming concern** — the client project owns its own `ServiceCollectionExtensions`
- **Don't share request/response types from the API project** — Kiota generates its own models from the OpenAPI spec
- **Always pass `cancellationToken`** — Kiota methods accept it as the last parameter

---

## File Download Pattern

```csharp
// Route registration
group.MapGet($"{nameof(Document)}/{{id}}/download", Download{Entity}DocumentByIdAsync)
    .WithSummary("Download {entity} document")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

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
        // Register domain handlers (CQRS)
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
13. **Explicit `.Produces<T>()` on every route** — always declare both success and error response types; never rely on inference
14. **OpenAPI document generation in `.csproj`** — every API project includes `OpenApiGenerateDocumentsOnBuild`, `OpenApiDocumentsDirectory`, and the `Microsoft.Extensions.ApiDescription.Server` package reference
15. **Document + schema transformers in `AddOpenApi()`** — always register a document transformer (servers URL) and a schema transformer (type mapping); missing either breaks Kiota client generation
16. **Kiota client per application** — `{ApplicationName}.Clients.Api` by default; `{ApplicationName}.Clients.{AppName}` for multi-app; never use raw `HttpClient` to call an internal API
17. **Data Annotations validation** — every POST/PUT route uses `.WithValidation<T>()` with `[Required]`, `[MaxLength]`, `[Range]` on request models
18. **Rate limiting + CORS + HTTPS** — every API project configures `AddRateLimiter` + `UseCors` + `UseHttpsRedirection`

# Endpoint Code Patterns

## Core Rules

- **Named static methods** — endpoints are static classes with named methods, not inline lambdas
- **`TypedResults.*`** — return `Results<T1, T2>` with typed factory methods, never `IResult`
- **Explicit parameter binding** — `[FromServices]`, `[FromBody]`, `[FromRoute]`, `[FromQuery]` on all parameters
- **Version set created inside** — each endpoint class creates its own `ApiVersionSet`
- **Authentication schemes passed in** — via `params string[] authenticationSchemes`

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

public static class {Entity}Endpoints
{
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

    public static async Task<Results<Created<Guid>, BadRequest>> Add{Entity}Async(
        [FromServices] ICommandHandler<Create{Entity}Command, Create{Entity}Response> handler,
        [FromBody] {Entity} e,
        CancellationToken cancellationToken = default)
    {
        var command = new Create{Entity}Command(e.Property1, e.Property2, e.Property3);
        var result = await handler.HandleAsync(command, cancellationToken);
        return TypedResults.Created($"/{entities}/{result.{Entity}Id}", result.{Entity}Id);
    }

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

    public static async Task<Results<Ok<bool>, BadRequest>> Update{Entity}Async(
        [FromServices] ICommandHandler<Update{Entity}Command, Update{Entity}Response> handler,
        [FromBody] {Entity} e,
        CancellationToken cancellationToken = default)
    {
        var command = new Update{Entity}Command(e.{Entity}Id, e.Property1, e.Property2);
        var result = await handler.HandleAsync(command, cancellationToken);
        return TypedResults.Ok(result.Success);
    }

    public static async Task<Results<NoContent, NotFound>> Remove{Entity}Async(
        [FromServices] ICommandHandler<Delete{Entity}Command, Delete{Entity}Response> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new Delete{Entity}Command(id);
        await handler.HandleAsync(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
```

## HTTP Method Patterns

| Operation | HTTP Method | Route | Return Type | Status Codes |
|-----------|-------------|-------|-------------|--------------|
| Create | POST | `""` | `Results<Created<Guid>, BadRequest>` | 201, 400, 401 |
| Read Single | GET | `"{id}"` | `Results<Ok<T>, NotFound>` | 200, 404, 401 |
| Read List | GET | `""` | `Ok<List<T>>` | 200, 401 |
| Update | PUT | `""` | `Results<Ok<bool>, BadRequest>` | 200, 400, 401 |
| Delete | DELETE | `"{id}"` | `Results<NoContent, NotFound>` | 204, 404, 401 |
| Search | GET | `"search"` | `Ok<List<T>>` | 200, 401 |
| Download | GET | `"{id}/download"` | `Results<FileContentHttpResult, NotFound>` | 200, 404, 401 |
| Count | GET | `"count"` | `Ok<int>` | 200, 401 |

## Produces Metadata

Every route must declare ALL possible response types:

```csharp
// POST — success type + auth error + validation error
.Produces<Guid>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status401Unauthorized)
.ProducesValidationProblem()

// GET by ID — success type + auth error + not found
.Produces<{Entity}>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status404NotFound)

// GET list — success type + auth error
.Produces<List<{Entity}>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)

// PUT — success type + auth error + validation error
.Produces<bool>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)
.ProducesValidationProblem()

// DELETE — success code + auth error + not found
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status404NotFound)
```

## TypedResults Factory Methods

```csharp
TypedResults.Ok(result)                                     // 200
TypedResults.Created($"/route/{id}", result.Id)             // 201
TypedResults.NoContent()                                    // 204
TypedResults.File(content, "application/octet-stream")      // 200 with file
TypedResults.NotFound()                                     // 404
TypedResults.BadRequest()                                   // 400
TypedResults.BadRequest("message")                          // 400 with message
TypedResults.Unauthorized()                                 // 401
```

## Program.cs Registration

```csharp
// File: {ApplicationName}.Service.Api/Program.cs

using {ApplicationName}.Services.{Domain}.Endpoints.v1;

var app = builder.Build();

app.Map{Entity}Endpoints("Bearer", "ApiKey");
app.Run();
```

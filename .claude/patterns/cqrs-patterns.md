# CQRS Implementation Patterns

Complete patterns for implementing Command Query Responsibility Segregation.
Reference implementation: `ISynergy.Domain.Budgets`

## Core Concepts

### CQRS Separation

- **Commands** - Change system state (Create, Update, Delete)
- **Queries** - Read data without side effects (Get, List, Search)
- **Handlers** - Execute commands/queries
- **Responses** - Return data from handlers (wrap Models)

### Key Principles

1. Commands use **individual primitive parameters** (NOT model objects, NOT entities)
2. Queries use **named parameters** for optional filters
3. Handlers inject **DataContext directly** (NO repository layer)
4. **Never expose** domain entities in public APIs — not as parameters, not as return values
5. One handler per command/query (Single Responsibility)
6. **Data access** uses named DbSet properties on DataContext for **all** operations: `dataContext.{Entities}.FirstOrDefaultAsync`, `.Add`, `.Remove`, `SaveChangesAsync` — **never call `.Update()` on tracked entities** (change tracker handles property mutations automatically)
7. **Mapping flow**: Entity → Model (via manual inline property assignment) | Response wraps Model
8. Entities are used **internally** for domain logic and persistence — never passed across layer boundaries
9. Each operation gets its own **subfolder** with separate files for Command/Query, Handler, and Response
10. Handler classes are named with `CommandHandler` / `QueryHandler` suffix

### Framework Namespaces

| Interface | Namespace |
|-|-|
| `ICommand<TResponse>` | `ISynergy.Framework.CQRS.Commands` |
| `ICommandHandler<TCommand, TResponse>` | `ISynergy.Framework.CQRS.Abstractions.Commands` |
| `IQuery<TResponse>` | `ISynergy.Framework.CQRS.Queries` |
| `IQueryHandler<TQuery, TResponse>` | `ISynergy.Framework.CQRS.Queries` |

---

## File Organization

Each operation gets its own subfolder with three separate files:

```
src/{ApplicationName}.Domain.{Domain}/
  Features/{Entity}/
    Commands/
      Create{Entity}/
        Create{Entity}Command.cs
        Create{Entity}CommandHandler.cs
        Create{Entity}Response.cs
      Update{Entity}/
        Update{Entity}Command.cs
        Update{Entity}CommandHandler.cs
        Update{Entity}Response.cs
      Delete{Entity}/
        Delete{Entity}Command.cs
        Delete{Entity}CommandHandler.cs
        Delete{Entity}Response.cs
    Queries/
      Get{Entity}ById/
        Get{Entity}ByIdQuery.cs
        Get{Entity}ByIdQueryHandler.cs
        Get{Entity}ByIdResponse.cs
      Get{Entities}List/
        Get{Entities}ListQuery.cs
        Get{Entities}ListQueryHandler.cs
        Get{Entities}ListResponse.cs
  Models/
    {Entity}.cs                    (positional record)
  Extensions/
    ServiceCollectionExtensions.cs
```

---

## Command Patterns

### Create Command

```csharp
// File: Create{Entity}Command.cs
using ISynergy.Framework.CQRS.Commands;

namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Create{Entity};

public sealed record Create{Entity}Command(
    string Description,
    DateTimeOffset StartingDate,
    DateTimeOffset EndingDate) : ICommand<Create{Entity}Response>;
```

```csharp
// File: Create{Entity}Response.cs
namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Create{Entity};

public sealed record Create{Entity}Response(Guid {Entity}Id);
```

### Update Command

```csharp
// File: Update{Entity}Command.cs
using ISynergy.Framework.CQRS.Commands;

namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Update{Entity};

public sealed record Update{Entity}Command(
    Guid {Entity}Id,
    string Description,
    DateTimeOffset StartingDate,
    DateTimeOffset EndingDate) : ICommand<Update{Entity}Response>;
```

```csharp
// File: Update{Entity}Response.cs
namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Update{Entity};

public sealed record Update{Entity}Response(bool Success);
```

### Delete Command

```csharp
// File: Delete{Entity}Command.cs
using ISynergy.Framework.CQRS.Commands;

namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Delete{Entity};

public sealed record Delete{Entity}Command(Guid {Entity}Id) : ICommand<Delete{Entity}Response>;
```

```csharp
// File: Delete{Entity}Response.cs
namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Delete{Entity};

public sealed record Delete{Entity}Response(bool Success);
```

---

## Query Patterns

### Get By ID Query

```csharp
// File: Get{Entity}ByIdQuery.cs
using ISynergy.Framework.CQRS.Queries;

namespace {App}.Domain.{Domain}.Features.{Entity}.Queries.Get{Entity}ById;

public sealed record Get{Entity}ByIdQuery(Guid {Entity}Id) : IQuery<Get{Entity}ByIdResponse>;
```

```csharp
// File: Get{Entity}ByIdResponse.cs
using {App}.Domain.{Domain}.Models;

namespace {App}.Domain.{Domain}.Features.{Entity}.Queries.Get{Entity}ById;

public sealed record Get{Entity}ByIdResponse({Entity}? {Entity});
```

### Get List Query

```csharp
// File: Get{Entities}ListQuery.cs
using ISynergy.Framework.CQRS.Queries;

namespace {App}.Domain.{Domain}.Features.{Entity}.Queries.Get{Entities}List;

public sealed record Get{Entities}ListQuery() : IQuery<Get{Entities}ListResponse>;
```

```csharp
// File: Get{Entities}ListResponse.cs
using {App}.Domain.{Domain}.Models;

namespace {App}.Domain.{Domain}.Features.{Entity}.Queries.Get{Entities}List;

public sealed record Get{Entities}ListResponse(IReadOnlyList<{Entity}> {Entities});
```

### Query with Optional Filters

```csharp
public sealed record GetDepositsTotalAmountQuery(
    Guid? GoalId = null,
    Guid? BudgetId = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null
) : IQuery<decimal>;

// Usage with named parameters
var query = new GetDepositsTotalAmountQuery(GoalId: id);
var query = new GetDepositsTotalAmountQuery(BudgetId: id);
var query = new GetDepositsTotalAmountQuery(StartDate: start, EndDate: end);
```

---

## Handler Patterns

### Create Handler

```csharp
// File: Create{Entity}CommandHandler.cs
using ISynergy.Data;
using ISynergy.Framework.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;

namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Create{Entity};

public sealed class Create{Entity}CommandHandler(
    DataContext dataContext,
    ILogger<Create{Entity}CommandHandler> logger)
    : ICommandHandler<Create{Entity}Command, Create{Entity}Response>
{
    public async Task<Create{Entity}Response> HandleAsync(
        Create{Entity}Command command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating {entity} with description {Description}", command.Description);

        var entity = new Entities.{Domain}.{Entity}
        {
            {Entity}Id = Guid.NewGuid(),
            Description = command.Description,
            StartingDate = command.StartingDate,
            EndingDate = command.EndingDate
        };

        dataContext.{Entities}.Add(entity);
        var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

        if (rowsAffected > 0)
        {
            logger.LogInformation("Successfully created {entity} {Id}", entity.{Entity}Id);
            return new Create{Entity}Response(entity.{Entity}Id);
        }

        logger.LogWarning("Failed to create {entity} - no rows affected");
        throw new InvalidOperationException("Failed to create {entity}");
    }
}
```

### Update Handler

```csharp
// File: Update{Entity}CommandHandler.cs
using ISynergy.Data;
using ISynergy.Framework.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;

namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Update{Entity};

public sealed class Update{Entity}CommandHandler(
    DataContext dataContext,
    ILogger<Update{Entity}CommandHandler> logger)
    : ICommandHandler<Update{Entity}Command, Update{Entity}Response>
{
    public async Task<Update{Entity}Response> HandleAsync(
        Update{Entity}Command command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating {entity} {Id}", command.{Entity}Id);

        var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == command.{Entity}Id, cancellationToken);

        if (entity is null)
        {
            logger.LogWarning("{Entity} {Id} not found", command.{Entity}Id);
            throw new InvalidOperationException("{Entity} " + command.{Entity}Id + " not found");
        }

        entity.Description = command.Description;
        entity.StartingDate = command.StartingDate;
        entity.EndingDate = command.EndingDate;

        var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

        if (rowsAffected > 0)
        {
            logger.LogInformation("Successfully updated {entity} {Id}", command.{Entity}Id);
            return new Update{Entity}Response(true);
        }

        logger.LogWarning("{Entity} {Id} not found or no changes made", command.{Entity}Id);
        throw new InvalidOperationException("{Entity} not found or no changes made");
    }
}
```

### Delete Handler

```csharp
// File: Delete{Entity}CommandHandler.cs
using ISynergy.Data;
using ISynergy.Framework.CQRS.Abstractions.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace {App}.Domain.{Domain}.Features.{Entity}.Commands.Delete{Entity};

public sealed class Delete{Entity}CommandHandler(
    DataContext dataContext,
    ILogger<Delete{Entity}CommandHandler> logger)
    : ICommandHandler<Delete{Entity}Command, Delete{Entity}Response>
{
    public async Task<Delete{Entity}Response> HandleAsync(
        Delete{Entity}Command command,
        CancellationToken cancellationToken = default)
    {
        var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == command.{Entity}Id, cancellationToken);

        if (entity is null)
        {
            logger.LogWarning("{Entity} {Id} not found", command.{Entity}Id);
            throw new InvalidOperationException($"{{Entity}} {command.{Entity}Id} not found");
        }

        dataContext.{Entities}.Remove(entity);
        var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

        if (rowsAffected > 0)
        {
            logger.LogInformation("Successfully deleted {entity} {Id}", command.{Entity}Id);
            return new Delete{Entity}Response(true);
        }

        logger.LogWarning("Failed to delete {entity} {Id} - no rows affected", command.{Entity}Id);
        throw new InvalidOperationException($"Failed to delete {{Entity}} {command.{Entity}Id}");
    }
}
```

### Get By ID Handler

```csharp
// File: Get{Entity}ByIdQueryHandler.cs
using ISynergy.Data;
using {App}.Domain.{Domain}.Models;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.Logging;

namespace {App}.Domain.{Domain}.Features.{Entity}.Queries.Get{Entity}ById;

public sealed class Get{Entity}ByIdQueryHandler(
    DataContext dataContext,
    ILogger<Get{Entity}ByIdQueryHandler> logger)
    : IQueryHandler<Get{Entity}ByIdQuery, Get{Entity}ByIdResponse>
{
    public async Task<Get{Entity}ByIdResponse> HandleAsync(
        Get{Entity}ByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving {entity} {Id}", query.{Entity}Id);

        var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == query.{Entity}Id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("{Entity} {Id} not found", query.{Entity}Id);
            return new Get{Entity}ByIdResponse(null);
        }

        var model = new {Entity}(
            entity.{Entity}Id,
            entity.Description,
            entity.StartingDate,
            entity.EndingDate);

        return new Get{Entity}ByIdResponse(model);
    }
}
```

### Get List Handler

```csharp
// File: Get{Entities}ListQueryHandler.cs
using ISynergy.Data;
using {App}.Domain.{Domain}.Models;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace {App}.Domain.{Domain}.Features.{Entity}.Queries.Get{Entities}List;

public sealed class Get{Entities}ListQueryHandler(
    DataContext dataContext,
    ILogger<Get{Entities}ListQueryHandler> logger)
    : IQueryHandler<Get{Entities}ListQuery, Get{Entities}ListResponse>
{
    public async Task<Get{Entities}ListResponse> HandleAsync(
        Get{Entities}ListQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving all {entities}");

        var entities = await dataContext.{Entities}
            .OrderBy(e => e.Description)
            .ToListAsync(cancellationToken);

        var models = entities.Select(e => new {Entity}(
            e.{Entity}Id,
            e.Description,
            e.StartingDate,
            e.EndingDate)).ToList();

        logger.LogDebug("Retrieved {Count} {entities}", models.Count);

        return new Get{Entities}ListResponse(models);
    }
}
```

### Complex Query with Filters

```csharp
public async Task<Get{Entities}ByFilterResponse> HandleAsync(
    Get{Entities}ByFilterQuery query,
    CancellationToken cancellationToken = default)
{
    var queryable = dataContext.{Entities}
        .Include(e => e.RelatedEntities) // Prevent N+1
        .AsQueryable();

    if (query.Filter1Id.HasValue)
        queryable = queryable.Where(e => e.Filter1Id == query.Filter1Id.Value);

    if (query.StartDate.HasValue)
        queryable = queryable.Where(e => e.CreatedDate >= query.StartDate.Value);

    if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        queryable = queryable.Where(e => e.Description.Contains(query.SearchTerm));

    var entities = await queryable
        .OrderBy(e => e.Description)
        .ToListAsync(cancellationToken);

    var models = entities.Select(e => new {Entity}(
        e.{Entity}Id,
        e.Description,
        e.StartingDate,
        e.EndingDate)).ToList();

    return new Get{Entities}ByFilterResponse(models);
}
```

---

## Model Pattern

Models are positional records in the domain project's `Models/` folder.

```csharp
// File: {ApplicationName}.Domain.{Domain}/Models/{Entity}.cs
namespace {App}.Domain.{Domain}.Models;

/// <summary>
/// Represents a {entity}.
/// </summary>
public sealed record {Entity}(
    Guid {Entity}Id,
    string Description,
    DateTimeOffset StartingDate,
    DateTimeOffset EndingDate);
```

Key rules:
- No "Model" suffix (just `Budget`, not `BudgetModel`)
- Positional record syntax
- Lives in `{App}.Domain.{Domain}.Models` namespace (inside domain project, not separate project)

---

## Data Access Patterns

### EF Core Primitives (Correct)

```csharp
// Create — named DbSet Add, SaveChanges
var entity = new Entities.{Domain}.{Entity} { {Entity}Id = Guid.NewGuid(), ... };
dataContext.{Entities}.Add(entity);
var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

// Read single — named DbSet FirstOrDefaultAsync + manual inline mapping
var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == id, cancellationToken);
var model = entity is null ? null : new {Entity}(entity.{Entity}Id, entity.Description, entity.StartingDate, entity.EndingDate);

// Read list — named DbSet + manual inline mapping
var entities = await dataContext.{Entities}
    .OrderBy(e => e.Description)
    .ToListAsync(cancellationToken);
var models = entities.Select(e => new {Entity}(e.{Entity}Id, e.Description, e.StartingDate, e.EndingDate)).ToList();

// Update — FirstOrDefaultAsync + property mutation (no .Update() needed — change tracker handles it)
var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == command.{Entity}Id, cancellationToken);
entity.Description = command.Description;
await dataContext.SaveChangesAsync(cancellationToken);

// Delete — named DbSet FirstOrDefaultAsync, Remove, SaveChanges
var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == command.{Entity}Id, cancellationToken);
dataContext.{Entities}.Remove(entity);
await dataContext.SaveChangesAsync(cancellationToken);
```

### NOT Used (Wrong)

```csharp
// WRONG - Extension methods are NOT used in handlers
await dataContext.AddItemAsync<TEntity, TModel>(model, ct);
await dataContext.GetItemByIdAsync<TEntity, TModel, TKey>(id, ct);
await dataContext.UpdateItemAsync<TEntity, TModel>(model, ct);
await dataContext.RemoveItemAsync<TEntity, TKey>(id, ct);

// WRONG - Repositories are NOT used
await _repository.Add(model);
```

---

## Response Patterns

### When to Use Each Response Type

| Scenario | Response Type | Example |
|-|-|-|
| Create | Return ID only | `Create{Entity}Response(Guid {Entity}Id)` |
| Update | Return success | `Update{Entity}Response(bool Success)` |
| Delete | Return success | `Delete{Entity}Response(bool Success)` |
| Get by ID | Wrap nullable Model | `Get{Entity}ByIdResponse({Entity}? {Entity})` |
| List query | Wrap Model collection | `Get{Entities}ListResponse(IReadOnlyList<{Entity}> {Entities})` |
| Aggregate | Return calculated value | `decimal` (total amount) |

---

## Service Registration

```csharp
// File: Extensions/ServiceCollectionExtensions.cs
using ISynergy.Framework.CQRS.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace {App}.Domain.{Domain}.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection With{Domain}DomainHandlers(
        this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        // Register CQRS handlers
        services.AddCQRS().AddHandlers(assembly);

        return services;
    }
}
```

---

## Common Pitfalls

### Wrong: Passing Model Objects to Commands

```csharp
// WRONG
public sealed record CreateDebtCommand(Debt Debt) : ICommand<CreateDebtResponse>;

// CORRECT
public sealed record CreateDebtCommand(
    Guid BudgetId,
    string Description,
    decimal Amount
) : ICommand<CreateDebtResponse>;
```

### Wrong: Using Extension Methods for Data Access

```csharp
// WRONG
await dataContext.AddItemAsync<Budget, BudgetModel>(model, ct);
await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id, ct);
await dataContext.RemoveItemAsync<Budget, Guid>(id, ct);

// CORRECT — use named DbSet properties
dataContext.Budgets.Add(entity);
var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == id, ct);
dataContext.Budgets.Remove(entity);
await dataContext.SaveChangesAsync(ct);
```

### Wrong: Using Mapper Libraries for Entity Construction

```csharp
// WRONG - any mapper library
var entity = command.Adapt<Budget>();        // Mapster — forbidden
var entity = _mapper.Map<Budget>(command);  // AutoMapper — forbidden

// CORRECT - Construct entity directly with explicit property assignment
var entity = new Entities.Budgets.Budget
{
    BudgetId = Guid.NewGuid(),
    Description = command.Description,
};
```

### Wrong: Flat DTOs as Responses

```csharp
// WRONG - Response with flat properties
public sealed record BudgetResponse(Guid BudgetId, string Description, ...);

// CORRECT - Response wraps Model
public sealed record GetBudgetByIdResponse(Budget? Budget);
public sealed record GetBudgetsListResponse(IReadOnlyList<Budget> Budgets);
```

### Wrong: Handler Naming Without Suffix

```csharp
// WRONG
public sealed class CreateBudgetHandler : ICommandHandler<...>
public sealed class GetBudgetByIdHandler : IQueryHandler<...>

// CORRECT
public sealed class CreateBudgetCommandHandler : ICommandHandler<...>
public sealed class GetBudgetByIdQueryHandler : IQueryHandler<...>
```

### Wrong: Exposing Domain Entities

```csharp
// WRONG - Returns domain entity
public async Task<Entities.Budgets.Budget> HandleAsync(...) { return entity; }

// CORRECT - Returns response wrapping model
public async Task<GetBudgetByIdResponse> HandleAsync(...)
{
    var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == query.BudgetId, ct);
    var budget = entity is null ? null : new Budget(entity.BudgetId, entity.Description, entity.StartingDate, entity.EndingDate);
    return new GetBudgetByIdResponse(budget);
}
```

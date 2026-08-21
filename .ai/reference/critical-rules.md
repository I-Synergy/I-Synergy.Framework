# Critical Rules (Read First)

These are non-negotiable patterns that cause bugs if violated.

## 1. Commands: Individual Parameters Only

**NEVER pass model objects to commands.**

```csharp
// CORRECT - Individual parameters
public sealed record CreateDebtCommand(
    Guid BudgetId,
    string Description,
    decimal Amount
) : ICommand<CreateDebtResponse>;

// WRONG - Passing model object
public sealed record CreateDebtCommand(
    Debt Debt
) : ICommand<CreateDebtResponse>;
```

### Endpoint Construction

```csharp
// CORRECT - Extract properties from model
app.MapPost("/debts", async (Debt model, ICommandHandler handler) =>
{
    var command = new CreateDebtCommand(model.BudgetId, model.Description, model.Amount);
    return await handler.HandleAsync(command);
});

// WRONG - Passing model directly
var command = new CreateDebtCommand(model);
```

## 2. Delete Operations: Use FirstOrDefaultAsync + Remove + SaveChangesAsync

```csharp
// CORRECT
var entity = await dataContext.Debts.FirstOrDefaultAsync(e => e.DebtId == command.DebtId, cancellationToken);

if (entity is null)
    throw new InvalidOperationException($"Debt {command.DebtId} not found");

dataContext.Debts.Remove(entity);
var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);

if (rowsAffected == 0)
    throw new InvalidOperationException($"Failed to delete Debt {command.DebtId}");

// WRONG - These extension methods are NOT used
await dataContext.DeleteItemByIdAsync<Debt, Guid>(id, cancellationToken);
await dataContext.RemoveItemAsync<Debt, Guid>(id, cancellationToken);
```

## 3. Query Parameters: Use Named Parameters

For optional filters, always use named parameters to avoid ambiguity.

```csharp
// CORRECT - Named parameters prevent ambiguity
var query = new GetDepositsTotalAmountQuery(GoalId: id);
var query = new GetDepositsTotalAmountQuery(BudgetId: id);

// WRONG - Positional parameters are ambiguous
var query = new GetDepositsTotalAmountQuery(id, true);
```

## 4. Data Access: Use EF Core Primitives on DataContext

**NO explicit Repository interfaces. NO extension methods like AddItemAsync/GetItemByIdAsync.** Use EF Core directly.

```csharp
// CORRECT — Use named DbSet properties from DataContext for all operations
// Create
var entity = new Entities.Budgets.Budget { BudgetId = Guid.NewGuid(), ... };
dataContext.Budgets.Add(entity);
await dataContext.SaveChangesAsync(cancellationToken);

// Read single
var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == id, cancellationToken);
var model = /* map entity to Budget model */;

// Read list
var models = await dataContext.Budgets
    .OrderBy(b => b.Description)
    .Select(b => /* map b to Budget model */)
    .ToListAsync(cancellationToken);

// Update — no .Update() call needed; change tracker detects property mutations on tracked entities
var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == command.BudgetId, cancellationToken);
entity.Description = command.Description;
await dataContext.SaveChangesAsync(cancellationToken);

// Delete
var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == command.BudgetId, cancellationToken);
dataContext.Budgets.Remove(entity);
await dataContext.SaveChangesAsync(cancellationToken);

// WRONG - We don't use repositories
await _repository.Add(model);

// WRONG - We don't use these extension methods
await dataContext.AddItemAsync<Budget, BudgetModel>(model, ct);
await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id, ct);
```

## 5. Async: Always Include CancellationToken

```csharp
// CORRECT
public async Task<GetBudgetByIdResponse> HandleAsync(
    GetBudgetByIdQuery query,
    CancellationToken cancellationToken = default)
{
    var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == query.BudgetId, cancellationToken);
    var budget = /* map entity to Budget model */;
    return new GetBudgetByIdResponse(budget);
}

// WRONG - Blocking on async
public GetBudgetByIdResponse Handle(GetBudgetByIdQuery query)
{
    var entity = dataContext.Budgets.FirstOrDefaultAsync(
        e => e.BudgetId == query.BudgetId,
        CancellationToken.None).Result; // DEADLOCK RISK!
}

// WRONG - No CancellationToken
public async Task<GetBudgetByIdResponse> HandleAsync(GetBudgetByIdQuery query)
{
    // Missing cancellation support
}
```

## 6. Entity Exposure: NEVER Expose Domain Entities

Always convert to Models before returning from handlers. Responses wrap Models.

```csharp
// CORRECT - Response wraps Model
public async Task<GetBudgetByIdResponse> HandleAsync(...)
{
    var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == query.BudgetId, ct);
    var budget = /* map entity to Budget model */;
    return new GetBudgetByIdResponse(budget);
}

// WRONG - Return domain entity directly
public async Task<Entities.Budgets.Budget> HandleAsync(...)
{
    return await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == id, ct);
}
```

## 7. Progress Files: Current Solution Only

Progress files MUST be in the current solution's `.ai/` folder.

```
CORRECT:   .ai/progress/task-progress.md
WRONG:     ~/.ai/progress/task-progress.md
WRONG:     /other/solution/.ai/progress/task-progress.md
```

## 8. Session Context: Read First, Update Last

Every session MUST:
1. Read `.ai/session-context.md` FIRST
2. Build on established patterns
3. Update session-context.md with learnings before ending

## 9. Handler Naming: Always Include Command/Query Suffix

```csharp
// CORRECT
public sealed class CreateBudgetCommandHandler : ICommandHandler<...>
public sealed class GetBudgetByIdQueryHandler : IQueryHandler<...>

// WRONG - Missing suffix
public sealed class CreateBudgetHandler : ICommandHandler<...>
public sealed class GetBudgetByIdHandler : IQueryHandler<...>
```

## 10. File Organization: One Type Per File, Subfolder Per Operation

```
// CORRECT - Each operation gets its own subfolder with separate files
Features/Budgets/
  Commands/CreateBudget/
    CreateBudgetCommand.cs
    CreateBudgetCommandHandler.cs
    CreateBudgetResponse.cs
  Commands/UpdateBudget/
    UpdateBudgetCommand.cs
    UpdateBudgetCommandHandler.cs
    UpdateBudgetResponse.cs
  Queries/GetBudgetById/
    GetBudgetByIdQuery.cs
    GetBudgetByIdQueryHandler.cs
    GetBudgetByIdResponse.cs

// WRONG - Flat folders or combined files
Features/Budgets/Commands/
  CreateBudgetCommand.cs      (command + response combined)
  CreateBudgetHandler.cs
```

## 11. Enum Naming: Always Plural (except *Status)

All enum type names must be plural. Only enums with a `Status` suffix are exempt.

```csharp
// CORRECT — plural names
public enum PaymentProviders { Stripe, PayNl, Mollie }
public enum SubscriptionHistoryEvents { Created, Activated, Cancelled }
public enum SecurityEventTypes { AuthenticationSuccess, AuthenticationFailed }
public enum AlertSeverities { Low, Medium, High }
public enum OrderTypes { Buy, Sell }

// CORRECT — Status suffix is exempt (kept singular)
public enum PaymentStatus { Pending, Succeeded, Failed }
public enum SubscriptionStatus { Active, Paused, Cancelled }
public enum EmailOutboxStatus { Pending, Processing, Sent, Failed }

// WRONG — singular non-Status enum names
public enum PaymentProvider { Stripe, PayNl, Mollie }
public enum SecurityEventType { AuthenticationSuccess, AuthenticationFailed }
public enum AlertSeverity { Low, Medium, High }
public enum OrderType { Buy, Sell }
```

## 12. Entity Properties: Use Enum Type, Not int

EF Core automatically converts enum types to/from int in the database. Always use the enum type on entity properties — never raw `int`.

```csharp
// CORRECT — EF Core converts enum to int automatically
public PaymentStatus Status { get; set; }
public PaymentProviders Provider { get; set; }
public SubscriptionStatus SubscriptionStatus { get; set; }
public InvoiceStates StatusId { get; set; }

// WRONG — Never store enum values as raw int on entities
public int Status { get; set; }
public int Provider { get; set; }
public int SubscriptionStatus { get; set; }
public int StatusId { get; set; }
```

## 13. Common Project: Centralize Shared Types

Every solution MUST include a `{ApplicationName}.Common` project. Shared types — especially enumerations — MUST live in Common, never scattered across domain/data projects where they create circular or cross-dependency issues.

All projects MUST reference `{ApplicationName}.Common` either directly or transitively (e.g., Services → Domain → Common).

```
// CORRECT — Enum defined once in Common, consumed everywhere
// {ApplicationName}.Common/Enums/PaymentStatus.cs
public enum PaymentStatus { Pending, Succeeded, Failed }

// {ApplicationName}.Domain.Payments references Common
// {ApplicationName}.Data references Domain (transitively gets Common)
// {ApplicationName}.Services references Domain (transitively gets Common)

// WRONG — Enum defined in a domain project, forcing Data → Domain dependency for enums
// {ApplicationName}.Domain.Payments/Enums/PaymentStatus.cs

// WRONG — Duplicate enum definitions across projects
// {ApplicationName}.Domain.Payments: PaymentStatus
// {ApplicationName}.Models.Payments: PaymentStatus  (duplicate)
```

Other types that belong in Common: shared constants, common interfaces (e.g., `IEntity`, `IAuditable`), reusable value objects, and extension methods used across multiple projects.

## 14. Entities and Models: Never Reference Each Other

An entity MUST never reference a model, and a model MUST never reference an entity. If both projects need the same type, that type belongs in `{ApplicationName}.Common`.

```
// CORRECT — Shared type extracted to Common
// {ApplicationName}.Common/Enums/PaymentStatus.cs
public enum PaymentStatus { Pending, Succeeded, Failed }

// {ApplicationName}.Entities.Payments/Payment.cs
public class Payment { public PaymentStatus Status { get; set; } }

// {ApplicationName}.Models.Payments/PaymentModel.cs
public sealed record PaymentModel(PaymentStatus Status);

// WRONG — Entity project references Models project for a shared enum
// {ApplicationName}.Entities.Payments has a project reference to {ApplicationName}.Models.Payments

// WRONG — Model project references Entities project for a shared enum
// {ApplicationName}.Models.Payments has a project reference to {ApplicationName}.Entities.Payments
```

A cross-reference between Entities and Models is the canary — it signals a type that should have been extracted to Common.

## 15. Plan Files: Always in `.ai/plans/`

Plans and design docs MUST be saved in the project-local `.ai/plans/` folder. The `writing-plans` and `brainstorming` skills default to `docs/plans/` — always override that to `.ai/plans/`.

```
CORRECT:   .ai/plans/2026-02-28-feature-name.md
WRONG:     docs/plans/2026-02-28-feature-name.md
WRONG:     ~/.ai/plans/2026-02-28-feature-name.md
```

This is configured via `plansDirectory` in `.claude/settings.json` and must not be bypassed by skill defaults.

## 16. New Types: Search Entire Solution First

**Before creating any entity, model, class, or property — search the entire solution first.** If it already exists, reuse or extend it. Never create overlapping or redundant types.

```
// CORRECT — search first, reuse what exists
// "Customer" already has Person, Address, PhoneNumber → reference those

// WRONG — duplicating existing domain types
public class Client { string Name; string Phone; string Street; }
```

This check is mandatory at **planning time**. Every plan must explicitly state which existing entities are reused before proposing new ones.

## 17. Code Writing: Always Delegate to Specialized Subagents

All code writing, editing, and modification MUST be delegated to a specialized subagent. The main conversation handles reasoning, analysis, planning, and user interaction only.

| Task | Subagent |
|------|----------|
| Planning, architecture, design decisions | `architect` subagent (Sonnet) or main conversation (Pro) |
| Code exploration and search | `Explore` subagent |
| .NET implementation (CQRS, API, Blazor, data access, builds) | `developer` subagent |
| Unit/integration tests (MSTest, Moq, Reqnroll) | `tester` subagent |
| E2E/UI tests (Playwright, accessibility) | `ui-tester` subagent |
| Visual design (color, typography, branding, tokens) | `designer` subagent |
| UI components, layouts, styling | `ui-developer` subagent |
| Code review (SOLID, CQRS, security, architecture) | `reviewer` subagent |
| Documentation (XML docs, API docs, READMEs, ADRs) | `writer` subagent |

All agents are defined in `.ai/agents/` and hard-linked to `.claude/agents/` for discovery.

## 18. Endpoint Produces Metadata: Always Explicit

Every API endpoint route MUST explicitly declare every status code it can return via `.Produces<T>()` or `.Produces(statusCode)`. Never rely on inference — OpenAPI/Swagger needs explicit metadata for accurate documentation.

```csharp
// CORRECT — All possible response types declared
group.MapPost("", AddEntityAsync)
    .Accepts<Entity>("application/json")
    .Produces<Guid>(StatusCodes.Status201Created)        // Success
    .Produces(StatusCodes.Status401Unauthorized)         // Auth failure
    .ProducesValidationProblem();                         // Validation failure

group.MapGet("{id}", GetEntityByIdAsync)
    .Produces<Entity>(StatusCodes.Status200OK)           // Success
    .Produces(StatusCodes.Status401Unauthorized)         // Auth failure
    .Produces(StatusCodes.Status404NotFound);            // Not found

group.MapGet("", GetEntityListAsync)
    .Produces<List<Entity>>(StatusCodes.Status200OK)     // Success
    .Produces(StatusCodes.Status401Unauthorized);        // Auth failure

group.MapPut("", UpdateEntityAsync)
    .Accepts<Entity>("application/json")
    .Produces<bool>(StatusCodes.Status200OK)             // Success
    .Produces(StatusCodes.Status401Unauthorized)         // Auth failure
    .ProducesValidationProblem();                         // Validation failure

group.MapDelete("{id}", RemoveEntityAsync)
    .Produces(StatusCodes.Status204NoContent)            // Success
    .Produces(StatusCodes.Status401Unauthorized)         // Auth failure
    .Produces(StatusCodes.Status404NotFound);            // Not found

// WRONG — Only error codes declared, success types omitted
group.MapPost("", AddEntityAsync)
    .Accepts<Entity>("application/json")
    .Produces(StatusCodes.Status401Unauthorized)
    .ProducesValidationProblem();

// WRONG — No produces metadata at all
group.MapGet("{id}", GetEntityByIdAsync);
```

## 19. API Projects: OpenAPI Document Generation in .csproj

Every API project `.csproj` MUST include the MSBuild properties and package reference to generate the OpenAPI spec at build time.

```xml
<!-- CORRECT — OpenAPI spec generated on every build -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <OpenApiDocumentsDirectory>../../openapi</OpenApiDocumentsDirectory>
    <OpenApiGenerateDocuments>true</OpenApiGenerateDocuments>
    <OpenApiGenerateDocumentsOnBuild>true</OpenApiGenerateDocumentsOnBuild>
    <OpenApiGenerateEnvironment>Development</OpenApiGenerateEnvironment>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.ApiDescription.Server">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>

// WRONG — Missing OpenAPI generation configuration and package reference
<Project Sdk="Microsoft.NET.Sdk.Web">
  <!-- No OpenApiGenerateDocumentsOnBuild, no OpenApiDocumentsDirectory, -->
  <!-- no Microsoft.Extensions.ApiDescription.Server reference -->
</Project>
```

`OpenApiDocumentsDirectory` is relative to the project file — `../../openapi` puts the spec in a solution-level `openapi/` folder. The package reference uses `PrivateAssets=all` so it doesn't leak to downstream consumers.

## 20. OpenAPI: Document Transformer (Servers) + Schema Transformer (Types)

Every API project MUST register both a document transformer and a schema transformer in `AddOpenApi()`.

Both transformers MUST be registered in a single `AddOpenApi` call:

```csharp
// CORRECT — Both transformers in one AddOpenApi call
builder.Services.AddOpenApi(options =>
{
    // Document transformer: sets servers URL for Kiota clients
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Servers = [new OpenApiServer { Url = "https://api" }];
        return Task.CompletedTask;
    });

    // Schema transformer: maps .NET types to JSON schema types
    options.AddSchemaTransformer((schema, context, ct) =>
    {
        var propertyType = context.JsonPropertyInfo?.PropertyType;
        if (propertyType == null)
            return Task.CompletedTask;

        var actualType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (actualType == typeof(DateTimeOffset) || actualType == typeof(DateTime))
        {
            schema.Type = JsonSchemaType.String; schema.Format = "date-time";
        }
        else if (actualType == typeof(DateOnly))
        {
            schema.Type = JsonSchemaType.String; schema.Format = "date";
        }
        else if (actualType == typeof(TimeOnly))
        {
            schema.Type = JsonSchemaType.String; schema.Format = "time";
        }
        else if (actualType == typeof(TimeSpan))
        {
            schema.Type = JsonSchemaType.String; schema.Format = "duration";
        }
        else if (actualType == typeof(decimal) || actualType == typeof(double))
        {
            schema.Type = JsonSchemaType.Number; schema.Format = "double";
        }
        else if (actualType == typeof(float))
        {
            schema.Type = JsonSchemaType.Number; schema.Format = "float";
        }
        else if (actualType == typeof(int))
        {
            schema.Type = JsonSchemaType.Integer; schema.Format = "int32";
        }
        else if (actualType == typeof(long))
        {
            schema.Type = JsonSchemaType.Integer; schema.Format = "int64";
        }
        else if (actualType == typeof(Guid))
        {
            schema.Type = JsonSchemaType.String; schema.Format = "uuid";
        }
        else if (actualType == typeof(string))
        {
            schema.Type = JsonSchemaType.String;
        }
        else if (actualType == typeof(bool))
        {
            schema.Type = JsonSchemaType.Boolean;
        }
        else if (actualType.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            schema.Enum = Enum.GetNames(actualType)
                .Select(name => System.Text.Json.Nodes.JsonValue.Create(name)!)
                .Cast<System.Text.Json.Nodes.JsonNode>()
                .ToList();
        }

        return Task.CompletedTask;
    });
});

// WRONG — No transformers; Kiota client has no base URL and all types are UntypedNode
builder.Services.AddOpenApi();
```

## 21. API Clients: Kiota-Generated, Never Raw HttpClient

Every API project MUST have a corresponding Kiota-generated client project. Never call an internal API with raw `HttpClient` — use the strongly-typed generated client instead.

```csharp
// CORRECT — Strongly-typed Kiota client injected via constructor
public sealed class GetEntityListQueryHandler(
    ApiClient client
) : IQueryHandler<GetEntityListQuery, GetEntityListResponse>
{
    public async Task<GetEntityListResponse> HandleAsync(
        GetEntityListQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await client.Entities.GetAsync(config =>
        {
            config.QueryParameters.Page = query.PageIndex;
            config.QueryParameters.PageSize = query.PageSize;
        }, cancellationToken);

        return new GetEntityListResponse(
            result?.Select(e => /* map to model */).ToList() ?? []);
    }
}

// WRONG — Raw HttpClient with magic strings
public sealed class GetEntityListQueryHandler(
    HttpClient httpClient
) : IQueryHandler<...>
{
    public async Task<...> HandleAsync(...)
    {
        var response = await httpClient.GetAsync("/api/entities?page=1");
        var json = await response.Content.ReadAsStringAsync();
        var entities = JsonSerializer.Deserialize<List<Entity>>(json);
    }
}
```

The client project (`{ApplicationName}.Clients.Api` by default; `{ApplicationName}.Clients.{AppName}` for multi-app) must include:
- `Microsoft.Kiota.Bundle` and `Microsoft.Extensions.Http` NuGet packages
- A `ServiceCollectionExtensions` that registers the client in DI with auth provider and base URL
- Kiota-generated source files (`Api/`, `Models/`, `ApiClient.cs`) are committed — they are the source code of the client project

The API project `.csproj` must include an MSBuild `OpenAPI` target that runs `dotnet kiota generate` after every build, regenerating the client from the latest OpenAPI spec.

## 22. Input Validation: Data Annotations + Validation Filter

Every POST/PUT route that accepts a body MUST include `.WithValidation<T>()` and `.ProducesValidationProblem()`. Request models MUST use Data Annotations.

```csharp
// CORRECT — Model with Data Annotations + validation filter on route
public sealed record CreateEntity([Required, MaxLength(100)] string Name, [Range(1, 100)] int Value);

group.MapPost("", AddEntityAsync)
    .Accepts<CreateEntity>("application/json")
    .WithValidation<CreateEntity>()
    .ProducesValidationProblem();

// WRONG — No validation attributes, no filter
public sealed record CreateEntity(string Name, int Value);

group.MapPost("", AddEntityAsync)
    .Accepts<CreateEntity>("application/json")
    .ProducesValidationProblem(); // Declared but never executed
```

## 23. Rate Limiting and HTTPS Enforcement

Every API project MUST configure rate limiting and HTTPS redirection.

```csharp
// CORRECT — Rate limiter and HTTPS configured
var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseRateLimiter();

app.Run();

// WRONG — No rate limiting, no HTTPS redirection
var app = builder.Build();
app.Run();
```

## 24. Enums: Explicit Values When Persisted or Serialized

Any enum that is persisted to a database or serialized (JSON, protobuf, OpenAPI) MUST declare explicit `= 0, = 1, …` values, with a comment stating the values are persisted and must never be renumbered.

```csharp
// CORRECT — explicit values; stable across inserts
public enum PaymentStatus
{
    Pending = 0,   // values are persisted, never renumber
    Succeeded = 1,
    Failed = 2,
}

// WRONG — implicit declaration order; inserting a member relabels every row
public enum PaymentStatus
{
    Pending,       // 0
    Succeeded,     // 1
    Failed,        // 2
    // Inserting "Refunded" above Failed turns Failed into 3 —
    // every stored row flips its meaning
}
```

Inserting a member in declaration order relabels every persisted row / flips every stored setting.

## 25. Bound Breaking Package Ranges

Pin known-incompatible transitive packages with an upper bound so the mismatch fails at restore instead of compile:

```xml
<!-- Directory.Packages.props — upper-bound the known-incompatible package -->
<PackageVersion Include="Microsoft.OpenApi" Version="[2.11.0,3.0.0)" />
```

Without the bound, a newer transitive version is resolved and the build fails deep inside a consuming project with a version-mismatch error — hours of debugging instead of a restore-time failure.

## 26. No Secrets in the Repo or Runbooks

Never commit `.p8` private keys, keystore material, or service-account JSON. Reference pipeline secret variable names and put a placeholder in the doc:

```yaml
# CORRECT — secrets referenced by name
env:
  GOOGLE_PLAY_SA_JSON: $(GOOGLE_PLAY_SA_JSON)
  APP_STORE_P8: $(APP_STORE_API_KEY_P8)

# WRONG — never commit the actual key material
env:
  GOOGLE_PLAY_SA_JSON: '{ "type": "service_account", ... }'   # LEAK
```

## Quick Violation Checklist

Before submitting code, verify you haven't violated these:

- [ ] Commands use individual parameters (not model objects)
- [ ] Delete operations use `FirstOrDefaultAsync` + `Remove` + `SaveChangesAsync`
- [ ] Queries use named parameters for optional filters
- [ ] Data access uses named DbSet properties on DataContext with `FirstOrDefaultAsync` for single lookups (no `.Set<T>()`, no `FindAsync`, no extension methods, no repositories)
- [ ] Create handlers construct entities directly with `new Entity { ... }`
- [ ] All async methods include CancellationToken
- [ ] Domain entities never exposed directly (responses wrap Models)
- [ ] Handlers named with `CommandHandler` / `QueryHandler` suffix
- [ ] Each type in its own file, each operation in its own subfolder
- [ ] Progress files in current solution's `.ai/` folder
- [ ] Plan files saved to `.ai/plans/` (not `docs/plans/`)
- [ ] Session context read first and updated last
- [ ] Solution includes `{ApplicationName}.Common` project with all enumerations centralized
- [ ] Entities and models never reference each other (cross-reference = belongs in Common)
- [ ] Enum names are plural (except *Status suffix enums)
- [ ] EF Core entity properties use enum types, not raw int
- [ ] Code and design work delegated to specialized subagents (`architect`, `developer`, `designer`, `ui-developer`, `tester`, `ui-tester`, `reviewer`, `writer`) — not done in main conversation
- [ ] All endpoint routes declare both success and error `.Produces()` metadata
- [ ] Every API project `.csproj` includes `OpenApiGenerateDocumentsOnBuild`, `OpenApiDocumentsDirectory`, and `Microsoft.Extensions.ApiDescription.Server` package reference
- [ ] Every API project registers a document transformer (servers URL) and schema transformer (type mapping) in `AddOpenApi()`
- [ ] Every API has a Kiota-generated client project — never use raw `HttpClient` to call an internal API
- [ ] Kiota version pinned in `dotnet-tools.json` with `rollForward: false`
- [ ] API project `.csproj` includes `OpenAPI` MSBuild target that runs `dotnet kiota generate` after every build
- [ ] Client project uses `Microsoft.Kiota.Bundle` and `Microsoft.Extensions.Http`
- [ ] Generated client source (`Api/`, `Models/`, `ApiClient.cs`, `kiota-lock.json`) is committed
- [ ] POST/PUT routes use `.WithValidation<T>()` with Data Annotations on request models
- [ ] Rate limiting configured (`AddRateLimiter` + `UseRateLimiter`) and `UseHttpsRedirection` called
- [ ] Persisted/serialized enums declare explicit `= 0, = 1, …` values with a "never renumber" comment
- [ ] Known-incompatible transitive packages are upper-bounded in `Directory.Packages.props`
- [ ] No `.p8` keys, keystores, or service-account JSON in the repo or runbooks (reference secret variable names)

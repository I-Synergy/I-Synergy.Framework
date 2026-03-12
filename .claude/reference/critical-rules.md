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
var model = entity?.Adapt<Budget>();

// Read list (using named DbSet + ProjectToType)
var models = await dataContext.Budgets
    .OrderBy(b => b.Description)
    .ProjectToType<Budget>()
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

## 5. Mapping: Mapster Only

Configure mappings in `Mappers/Configuration.cs` (implements `IRegister`).

```csharp
// CORRECT - Mapster with configuration
var model = entity.Adapt<Budget>();

// CORRECT - ProjectToType for list queries (server-side projection)
var models = await dataContext.Budgets
    .ProjectToType<Budget>()
    .ToListAsync(cancellationToken);

// CORRECT - Direct entity construction in Create handlers
var entity = new Entities.Budgets.Budget
{
    BudgetId = Guid.NewGuid(),
    Description = command.Description,
};

// WRONG - AutoMapper
var model = _mapper.Map<Budget>(entity);

// WRONG - Manual mapping for reads (use Mapster instead)
var model = new Budget { BudgetId = entity.BudgetId, ... };

// WRONG - Mapping command to entity via Mapster
var entity = command.Adapt<Budget>(); // Don't do this
```

## 6. Async: Always Include CancellationToken

```csharp
// CORRECT
public async Task<GetBudgetByIdResponse> HandleAsync(
    GetBudgetByIdQuery query,
    CancellationToken cancellationToken = default)
{
    var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == query.BudgetId, cancellationToken);
    var budget = entity?.Adapt<Budget>();
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

## 7. Entity Exposure: NEVER Expose Domain Entities

Always map to Models before returning from handlers. Responses wrap Models.

```csharp
// CORRECT - Response wraps Model
public async Task<GetBudgetByIdResponse> HandleAsync(...)
{
    var entity = await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == query.BudgetId, ct);
    var budget = entity?.Adapt<Budget>();
    return new GetBudgetByIdResponse(budget);
}

// WRONG - Return domain entity directly
public async Task<Entities.Budgets.Budget> HandleAsync(...)
{
    return await dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == id, ct);
}
```

## 8. Progress Files: Current Solution Only

Progress files MUST be in the current solution's [`.claude/`](../) folder.

```
CORRECT:   .claude/progress/task-progress.md
WRONG:     ~/.claude/progress/task-progress.md
WRONG:     /other/solution/.claude/progress/task-progress.md
```

## 9. Session Context: Read First, Update Last

Every session MUST:
1. Read [`session-context.md`](../session-context.md) FIRST
2. Build on established patterns
3. Update session-context.md with learnings before ending

## 10. Handler Naming: Always Include Command/Query Suffix

```csharp
// CORRECT
public sealed class CreateBudgetCommandHandler : ICommandHandler<...>
public sealed class GetBudgetByIdQueryHandler : IQueryHandler<...>

// WRONG - Missing suffix
public sealed class CreateBudgetHandler : ICommandHandler<...>
public sealed class GetBudgetByIdHandler : IQueryHandler<...>
```

## 11. File Organization: One Type Per File, Subfolder Per Operation

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

## 12. Enum Naming: Always Plural (except *Status)

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

## 13. Entity Properties: Use Enum Type, Not int

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

**Note:** `ISynergy.Models.*` MVVM observable classes use `GetValue<int>()` and are NOT subject to this rule — only EF Core entity classes.

## 14. Plan Files: Always in `.claude/plans/`

Plans and design docs MUST be saved in the project-local [`plans/`](../plans/) folder. The `writing-plans` and `brainstorming` skills default to `docs/plans/` — always override that to `.claude/plans/`.

```
CORRECT:   .claude/plans/2026-02-28-feature-name.md
WRONG:     docs/plans/2026-02-28-feature-name.md
WRONG:     ~/.claude/plans/2026-02-28-feature-name.md
```

This is configured via `plansDirectory` in [`settings.json`](../settings.json) and must not be bypassed by skill defaults.

## 15. XML Doc Comments on Handlers

Handler classes and their `HandleAsync` method use XML doc comments consistently.

```csharp
/// <summary>
/// Handles the creation of a new {entity}.
/// </summary>
/// <param name="dataContext">The data context for database operations.</param>
/// <param name="logger">The logger for diagnostic information.</param>
public sealed class Create{Entity}CommandHandler(
    DataContext dataContext,
    ILogger<Create{Entity}CommandHandler> logger)
    : ICommandHandler<Create{Entity}Command, Create{Entity}Response>
{
    /// <inheritdoc />
    public async Task<Create{Entity}Response> HandleAsync(
        Create{Entity}Command command,
        CancellationToken cancellationToken = default)
    { ... }
}
```

## 16. Evaluate Existing Code Before Creating Anything New

**I-Synergy is built on data synergy.** The entire platform shares a coherent domain model. Creating new entities, models, or properties that duplicate or overlap existing ones directly undermines this principle and creates fragmentation that is expensive to undo.

### Mandatory pre-creation check

Before writing any new entity, model, class, or property, search the ENTIRE solution for existing equivalents:

- Does a domain entity already represent this concept? (e.g., `Customer`, `Person`, `Address`, `PhoneNumber`)
- Does an existing entity have the properties needed, or can it be extended?
- Is there an existing relationship that already links these concepts?

If yes — **reuse or extend it**. Never create a parallel class that overlaps with an existing one.

### Concrete examples of what NOT to do

```
// WRONG — creating a new "Client" entity with phone/address when Customer + Person already exist
public class Client { string Name; string Phone; string Street; }

// CORRECT — reference the existing shared entities
public class HealthClient { Guid CustomerId; /* navigation: Customer → Person → Address, PhoneNumbers */ }
```

### This applies at planning time

When writing a plan or design, explicitly list which existing entities and models will be reused. If a plan proposes a new entity, it must justify why no existing entity covers the need.

### Properties must be unambiguous and logical

- Property names must reflect their exact meaning — no generic names like `Data`, `Info`, `Value`, `Extra`
- A property that already exists on a referenced entity must not be duplicated on the new entity
- Nullable vs required must reflect actual business rules, not implementation convenience

## 17. Debugging & Bug-Fixing Process

These rules apply whenever investigating or resolving a bug or build failure.

- **Never modify autogenerated files** (e.g., `ISynergy.Clients.*` Kiota projects) as part of debugging. If a generated file appears to be the source of an issue, the fix must go in the source (API endpoint) and the client must be regenerated.
- **Do not stop until the problem is fully resolved and the solution builds with 0 errors.** A partial fix or a workaround that leaves errors behind is not acceptable.
- **Run all available tests after fixing a bug** to verify no regressions were introduced. Use `dotnet test` for the affected domain(s) or the full solution if the change is cross-cutting.
- Investigate root causes — do not mask errors with `#pragma warning disable`, try/catch swallowing, or conditional compilation.

## 18. Production Completeness — No Mocks, No Stubs, No Placeholders

The goal is **production-ready software**, not a demo or proof of concept. Every task must be evaluated against this standard before declaring it done.

### Mandatory self-evaluation before saying "done"

Before reporting a task as complete, actively search the entire changed scope for:

- `// TODO`, `// FIXME`, `// HACK`, `// mock`, `// stub`, `// placeholder`, `// hardcoded`, `// temp`
- Hardcoded lists, arrays, or dictionaries that should come from the API or database
- Methods that return empty collections, `null`, or constant values instead of real data
- `Task.FromResult(...)` or `Task.CompletedTask` used where a real async operation is expected
- Services injected but never called (data loaded from a static source instead)
- UI components bound to local fake data instead of ViewModel properties backed by real API calls
- `throw new NotImplementedException()` or silent no-ops

If **any** of these are found, the task is **not done**. Fix them before reporting completion.

### What "done" means

- All data displayed in the UI comes from real API calls through the Kiota client
- All write operations (create, update, delete) call the real API endpoint and handle the response
- No in-memory or hardcoded data stands in for real persistence
- The solution builds with 0 errors and 0 relevant warnings
- All available tests pass

### UI — every element must work as intended

UI work is only complete when every element in the changed scope is fully functional:

- Every **window and dialog** opens, closes, and displays correct data
- Every **button** is wired to a real ViewModel command — no unbound, disabled-by-default, or no-op buttons
- Every **form field** reads from and writes back to the ViewModel and persists via the API on save
- Every **list, grid, or table** loads real data and reflects create/update/delete without requiring a full page reload
- Every **navigation item** routes to a working page
- Every **validation message** fires correctly on invalid input
- Empty states, loading states (`IsBusy`), and error states are handled and visible
- No component is left permanently disabled, hidden, or in a placeholder state unless that is the explicit design intent

### Quick self-check grep commands

```bash
# Search for common incompleteness markers in changed files
grep -rn "TODO\|FIXME\|HACK\|NotImplemented\|Task\.FromResult\|mock\|stub\|placeholder\|hardcoded" src/
```

## 19. Configuration: Always Use IOptions<T>, Never IConfiguration["key"]

**NEVER access configuration via string indexer or `GetValue<T>("Key")`.** Always use typed `IOptions<T>` binding.

```csharp
// CORRECT — register in DI and inject typed options
services.Configure<AzureStorageOptions>(configuration.GetSection(nameof(AzureStorageOptions)));

public class AzureStorageService(IOptions<AzureStorageOptions> options)
{
    private readonly AzureStorageOptions _options = options.Value;

    public void DoSomething()
    {
        var connectionString = _options.ConnectionString; // typed, safe
    }
}

// WRONG — direct configuration indexer / GetValue<T>
var uri = configuration["KeyVaultOptions:Uri"];
var size = configuration.GetValue<int>("Storage:MaxSize");
```

**Configuration section name must use `nameof(TOptions)`** — avoids typos and keeps section name in sync with class name:

```csharp
// CORRECT
services.Configure<KeyVaultOptions>(configuration.GetSection(nameof(KeyVaultOptions)));

// WRONG — magic string that can drift from class name
services.Configure<KeyVaultOptions>(configuration.GetSection("KeyVault"));
```

## 20. Library Extension Method Naming: Add{Provider}{Service}Integration

Extension methods that register a provider-specific library integration must follow the pattern `Add{Provider}{Service}Integration`.

```csharp
// CORRECT
services.AddAzureKeyVaultIntegration(configuration);
services.AddOpenBaoKeyVaultIntegration(configuration);
services.AddAzureStorageIntegration(configuration);
services.AddSendGridMailIntegration(configuration);
services.AddMicrosoft365MailIntegration(configuration);
services.AddAzureMessageBusIntegration(configuration);
services.AddRabbitMQMessageBusIntegration(configuration);

// WRONG — generic name that collides across providers
services.AddKeyVaultIntegration(configuration);    // which key vault?
services.AddMailIntegration(configuration);        // which mail provider?
services.AddStorageIntegration(configuration);     // which storage?
```

## 21. Options Class Naming: {Provider}{Service}Options

Options classes must be prefixed with the provider name to avoid cross-provider naming collisions.

```csharp
// CORRECT — provider-prefixed names, one class per provider
public class AzureKeyVaultOptions { }
public class OpenBaoKeyVaultOptions { }
public class AzureStorageOptions { }
public class SendGridMailOptions { }
public class Microsoft365MailOptions { }
public class AzurePublisherOptions { }
public class AzureSubscriberOptions { }
public class RabbitMQPublisherOptions { }
public class RabbitMQSubscriberOptions { }

// WRONG — generic names that collide when both providers are referenced
public class KeyVaultOptions { }      // Azure or OpenBao?
public class MailOptions { }          // SendGrid or Microsoft365?
public class PublisherOptions { }     // Azure or RabbitMQ?
public class SubscriberOptions { }    // Azure or RabbitMQ?
```

## Quick Violation Checklist

Before submitting code, verify you haven't violated these:

- [ ] Commands use individual parameters (not model objects)
- [ ] Delete operations use `FirstOrDefaultAsync` + `Remove` + `SaveChangesAsync`
- [ ] Queries use named parameters for optional filters
- [ ] Data access uses named DbSet properties on DataContext with `FirstOrDefaultAsync` for single lookups (no `.Set<T>()`, no `FindAsync`, no extension methods, no repositories)
- [ ] Mapping uses Mapster (not AutoMapper or manual for reads)
- [ ] Create handlers construct entities directly (not via `command.Adapt<Entity>()`)
- [ ] All async methods include CancellationToken
- [ ] Domain entities never exposed directly (responses wrap Models)
- [ ] Handlers named with `CommandHandler` / `QueryHandler` suffix
- [ ] Each type in its own file, each operation in its own subfolder
- [ ] Progress files in current solution's [`.claude/`](../) folder
- [ ] Plan files saved to [`.claude/plans/`](../plans/) (not `docs/plans/`)
- [ ] Session context read first and updated last
- [ ] Enum names are plural (except *Status suffix enums)
- [ ] EF Core entity properties use enum types, not raw int
- [ ] Handler classes and `HandleAsync` use XML doc comments (`/// <summary>` on class, `/// <inheritdoc />` on method)
- [ ] No autogenerated files (`ISynergy.Clients.*`) were modified during debugging
- [ ] Solution builds with 0 errors before declaring the bug fixed
- [ ] Tests run after fix to confirm no regressions
- [ ] No TODOs, mocks, stubs, hardcoded data, or `NotImplementedException` remain in changed code
- [ ] All UI data comes from real API calls — no fake/static data
- [ ] All write operations hit real API endpoints
- [ ] Configuration accessed via `IOptions<T>` only — no `configuration["Key"]` or `GetValue<T>("Key")`
- [ ] Configuration section registered with `nameof(TOptions)` as section name
- [ ] Library extension methods named `Add{Provider}{Service}Integration`
- [ ] Options classes named `{Provider}{Service}Options` (no generic `PublisherOptions`, `MailOptions`, etc.)

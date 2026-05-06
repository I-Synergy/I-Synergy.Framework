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

## 7. Entity Exposure: NEVER Expose Domain Entities

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

## 8. Progress Files: Current Solution Only

Progress files MUST be in the current solution's `.ai/` folder.

```
CORRECT:   .ai/progress/task-progress.md
WRONG:     ~/.ai/progress/task-progress.md
WRONG:     /other/solution/.ai/progress/task-progress.md
```

## 9. Session Context: Read First, Update Last

Every session MUST:
1. Read `.ai/session-context.md` FIRST
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

## 14. Plan Files: Always in `.ai/plans/`

Plans and design docs MUST be saved in the project-local `.ai/plans/` folder. The `writing-plans` and `brainstorming` skills default to `docs/plans/` — always override that to `.ai/plans/`.

```
CORRECT:   .ai/plans/2026-02-28-feature-name.md
WRONG:     docs/plans/2026-02-28-feature-name.md
WRONG:     ~/.ai/plans/2026-02-28-feature-name.md
```

This is configured via `plansDirectory` in `.claude/settings.json` and must not be bypassed by skill defaults.

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
- [ ] Enum names are plural (except *Status suffix enums)
- [ ] EF Core entity properties use enum types, not raw int

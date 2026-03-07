# Template Tokens Reference

Throughout this template, the following placeholders are used. Replace them when implementing:

| Token | Replace With | Example |
|-------|--------------|---------|
| `{ApplicationName}` | Your application name | `BudgetTracker` |
| `{Domain}` | Domain/bounded context name | `Budgets`, `Goals`, `Debts` |
| `{Entity}` | Entity name (PascalCase) | `Budget`, `Goal`, `Debt` |
| `{entity}` | Entity name (lowercase) | `budget`, `goal`, `debt` |
| `{entities}` | Entity plural (lowercase) | `budgets`, `goals`, `debts` |

## Special Files (Always in .claude/)

- **session-context.md** - Read FIRST at session start, update before session end
- **progress/[task].md** - Active task progress files
- **completed/[task].md** - Completed task archives for future reference

## Example Transformation

- **Template:** `{ApplicationName}.Domain.{Domain}`
- **Actual:** `BudgetTracker.Domain.Budgets`

## Namespace Examples

```csharp
// Template format
namespace {ApplicationName}.Domain.{Domain}.Features.{Entity}.Commands;
namespace {ApplicationName}.Services.{Domain}.Endpoints;
namespace {ApplicationName}.Entities.{Domain};

// Actual implementation
namespace BudgetTracker.Domain.Budgets.Features.Budget.Commands;
namespace BudgetTracker.Services.Budgets.Endpoints;
namespace BudgetTracker.Entities.Budgets;
```

## File Path Examples

```
// Template paths
src/{ApplicationName}.Domain.{Domain}/Features/{Entity}/Commands/Create{Entity}Command.cs
src/{ApplicationName}.Services.{Domain}/Endpoints/{Entity}Endpoints.cs

// Actual paths
src/BudgetTracker.Domain.Budgets/Features/Budget/Commands/CreateBudgetCommand.cs
src/BudgetTracker.Services.Budgets/Endpoints/BudgetEndpoints.cs
```

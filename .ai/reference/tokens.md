# Framework Tokens Reference

The AI guidance in this repository (`.ai/`, `AGENTS.md`) uses placeholder tokens so one pattern
can describe both this library and the applications built on it. This file is the single
definition of those tokens: `.ai/tests/validate-tokens.sh` fails when a token is used but not
defined here.

## Application name

This repository **is** the framework library, so `{ApplicationName}` resolves to its fixed root
namespace. The product is written "I-Synergy Framework" (with a hyphen), but the namespace,
assembly and NuGet prefix is `ISynergy.Framework` (no hyphen):

| Token | Replace With | Value in this repository |
|-------|--------------|--------------------------|
| `{ApplicationName}` | Root namespace of the solution | `ISynergy.Framework` |

Code inside the framework always writes the real prefix — `namespace ISynergy.Framework.CQRS.Dispatch;`
— and never the token. A consuming application replaces `{ApplicationName}` with its own root
namespace (for example `BudgetTracker`).

## Domain and entity tokens

These stay generic. Framework libraries are domain-agnostic; the tokens are only materialised
when a pattern is applied to a bounded context in a consuming application.

| Token | Replace With | Example |
|-------|--------------|---------|
| `{Domain}` | Domain/bounded context name | `Budgets`, `Goals`, `Debts` |
| `{Entity}` | Entity name (PascalCase) | `Budget`, `Goal`, `Debt` |
| `{Entities}` | Entity plural (PascalCase) — used for DbSet names | `Budgets`, `Goals`, `Debts` |
| `{entity}` | Entity name (lowercase) | `budget`, `goal`, `debt` |
| `{entities}` | Entity plural (lowercase) | `budgets`, `goals`, `debts` |
| `{ChildEntity}` | Child entity name (PascalCase) | `Goal`, `Debt` |
| `{ChildEntities}` | Child entity plural (PascalCase) — used for navigation collections | `Goals`, `Debts` |
| `{childEntity}` | Child entity name (lowercase) | `goal`, `debt` |
| `{childEntities}` | Child entity plural (lowercase) | `goals`, `debts` |

## Special Files (Always in .ai/)

- **session-context.md** - Read FIRST at session start, update before session end
- **progress/[task].md** - Active task progress files
- **completed/[task].md** - Completed task archives for future reference

## This Library's Layout

Framework projects live directly under `src/` and are named `{ApplicationName}.{Component}`.
There is no `Features/`, `Domain/` or `Entities/` folder here: those belong to an application
built on the framework, not to the framework itself.

```text
src/ISynergy.Framework.Core/
src/ISynergy.Framework.CQRS/
src/ISynergy.Framework.EntityFramework/
src/ISynergy.Framework.AspNetCore/
src/ISynergy.Framework.MessageBus/
src/ISynergy.Framework.UI/
```

## Namespace Examples (this repository)

```csharp
namespace ISynergy.Framework.Core.Abstractions.Base;
namespace ISynergy.Framework.CQRS.Dispatch;
namespace ISynergy.Framework.EntityFramework.Extensions;
namespace ISynergy.Framework.AspNetCore.Authentication.Options;
```

## Namespace Examples (consuming application)

When a pattern is applied to an application, `{ApplicationName}` becomes that application's root
namespace and the domain tokens are filled in:

```csharp
namespace {ApplicationName}.Domain.{Domain}.Features.{Entity}.Commands;
namespace {ApplicationName}.Services.{Domain}.Endpoints;
namespace {ApplicationName}.Entities.{Domain};
```

```csharp
// Applied
namespace BudgetTracker.Domain.Budgets.Features.Budget.Commands;
namespace BudgetTracker.Services.Budgets.Endpoints;
namespace BudgetTracker.Entities.Budgets;
```

## File Path Examples (consuming application)

```text
src/{ApplicationName}.Domain.{Domain}/Features/{Entity}/Commands/Create{Entity}Command.cs
src/{ApplicationName}.Services.{Domain}/Endpoints/{Entity}Endpoints.cs
```

```text
src/BudgetTracker.Domain.Budgets/Features/Budget/Commands/CreateBudgetCommand.cs
src/BudgetTracker.Services.Budgets/Endpoints/BudgetEndpoints.cs
```

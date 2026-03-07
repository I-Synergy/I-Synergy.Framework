# Naming Conventions

## C# Code

| Element | Convention | Example |
|---------|-----------|----------|
| **Namespace** | `{ApplicationName}.{Layer}.{Domain}` | `BudgetTracker.Domain.Budgets` |
| **Command** | `{Action}{Entity}Command` | `CreateBudgetCommand` |
| **Query** | `Get{Entity}{Criteria}Query` | `GetBudgetByIdQuery` |
| **Handler** | `{CommandOrQuery}Handler` | `CreateBudgetHandler` |
| **Response** | `{Action}{Entity}Response` | `CreateBudgetResponse` |
| **DTO** | `{Entity}Model` or `{Entity}Dto` | `BudgetModel` |
| **Interface** | `I{Type}` | `ICommandHandler<T>` |
| **Classes** | PascalCase | `BudgetEndpoints` |
| **Methods** | PascalCase + `Async` suffix | `GetBudgetByIdAsync` |
| **Properties** | PascalCase | `BudgetId`, `CreatedDate` |
| **Private fields** | `_camelCase` | `_dataContext`, `_logger` |
| **Parameters/locals** | camelCase | `budgetId`, `cancellationToken` |
| **Constants** | PascalCase | `MaxBudgetAmount` |
| **Enums** | PascalCase (type and values) | `BudgetStatus.Active` |

## Namespace Patterns

```csharp
// Domain layer
{ApplicationName}.Domain.{Domain}
{ApplicationName}.Domain.{Domain}.Features.{Entity}.Commands
{ApplicationName}.Domain.{Domain}.Features.{Entity}.Queries
{ApplicationName}.Domain.{Domain}.Features.{Entity}.Events

// Application layer
{ApplicationName}.Services.{Domain}
{ApplicationName}.Services.{Domain}.Endpoints

// Infrastructure layer
{ApplicationName}.Entities.{Domain}
{ApplicationName}.Models.{Domain}
{ApplicationName}.Data

// Examples
BudgetTracker.Domain.Budgets
BudgetTracker.Domain.Budgets.Features.Budget.Commands
BudgetTracker.Services.Budgets.Endpoints
BudgetTracker.Entities.Budgets
```

## File Naming

### Commands

```
Create{Entity}Command.cs
Update{Entity}Command.cs
Delete{Entity}Command.cs

Examples:
CreateBudgetCommand.cs
UpdateBudgetCommand.cs
DeleteBudgetCommand.cs
```

### Queries

```
Get{Entity}ByIdQuery.cs
Get{Entity}ListQuery.cs
Get{Entities}By{Criteria}Query.cs

Examples:
GetBudgetByIdQuery.cs
GetBudgetListQuery.cs
GetBudgetsByUserIdQuery.cs
```

### Handlers

```
{CommandName}Handler.cs
{QueryName}Handler.cs

Examples:
CreateBudgetHandler.cs
GetBudgetByIdHandler.cs
```

### Responses

```
Create{Entity}Response.cs
{Entity}Response.cs
{Entity}SummaryResponse.cs

Examples:
CreateBudgetResponse.cs
BudgetResponse.cs
BudgetSummaryResponse.cs
```

### Endpoints

```
{Entity}Endpoints.cs

Examples:
BudgetEndpoints.cs
GoalEndpoints.cs
```

### Mapping Configurations

```
{Entity}MappingConfig.cs

Examples:
BudgetMappingConfig.cs
GoalMappingConfig.cs
```

## Database (PostgreSQL)

- **Tables:** snake_case (managed by EF Core conventions)
- **Columns:** snake_case (managed by EF Core conventions)

EF Core automatically handles C# PascalCase to database snake_case conversion.

```csharp
// C# Entity
public class Budget
{
    public Guid BudgetId { get; set; }
    public string Name { get; set; }
    public decimal TotalAmount { get; set; }
}

// Database table (automatic)
Table: budgets
Columns: budget_id, name, total_amount
```

## API Endpoints

### Route Patterns

```
POST   /{domain}              # Create
GET    /{domain}/{id}         # Read single
GET    /{domain}              # Read list
PUT    /{domain}/{id}         # Update
DELETE /{domain}/{id}         # Delete

Examples:
POST   /budgets
GET    /budgets/123e4567-e89b-12d3-a456-426614174000
GET    /budgets
PUT    /budgets/123e4567-e89b-12d3-a456-426614174000
DELETE /budgets/123e4567-e89b-12d3-a456-426614174000
```

### Versioning

```
/v1/{domain}
/v2/{domain}

Examples:
/v1/budgets
/v2/budgets
```

## Test Naming

### MSTest

```csharp
[TestMethod]
public async Task HandleAsync_{Scenario}_{ExpectedResult}()

Examples:
[TestMethod]
public async Task HandleAsync_ValidCommand_CreatesBudgetSuccessfully()

[TestMethod]
public async Task HandleAsync_NullCommand_ThrowsArgumentNullException()

[TestMethod]
public async Task HandleAsync_NegativeAmount_ThrowsArgumentException()
```

### Reqnroll Features

```
{Entity}Management.feature

Examples:
BudgetManagement.feature
GoalManagement.feature
```

### Step Definitions

```
{Entity}ManagementSteps.cs

Examples:
BudgetManagementSteps.cs
GoalManagementSteps.cs
```

## Method Naming

### Command/Query Handlers

```csharp
// Always: HandleAsync
public async Task<TResponse> HandleAsync(
    TCommand command,
    CancellationToken cancellationToken = default)

// Examples
public async Task<CreateBudgetResponse> HandleAsync(
    CreateBudgetCommand command,
    CancellationToken cancellationToken = default)
```

### Data Access

```csharp
// Extension methods on DataContext
AddItemAsync<TEntity, TModel>()
GetItemByIdAsync<TEntity, TModel, TKey>()
UpdateItemAsync<TEntity, TModel>()
RemoveItemAsync<TEntity, TKey>()

// Examples
await dataContext.AddItemAsync<Budget, BudgetModel>(model, ct);
await dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id, ct);
```

### Repository Methods (if used)

```csharp
// Async suffix + descriptive name
GetByIdAsync()
GetAllAsync()
FindByAsync()
CreateAsync()
UpdateAsync()
DeleteAsync()

// Examples
Task<Budget> GetByIdAsync(Guid id, CancellationToken ct);
Task<List<Budget>> GetAllAsync(CancellationToken ct);
Task<List<Budget>> FindByUserIdAsync(Guid userId, CancellationToken ct);
```

## Variable Naming

### Local Variables

```csharp
// camelCase, descriptive
var budgetModel = ...;
var createdBudget = ...;
var userId = ...;

// NOT abbreviated
var bm = ...;  // ❌ Bad
var b = ...;   // ❌ Bad
```

### Parameters

```csharp
// camelCase, descriptive
public async Task DoSomething(
    Guid budgetId,
    string userName,
    CancellationToken cancellationToken)
```

### Private Fields

```csharp
// _camelCase with underscore prefix
private readonly DataContext _dataContext;
private readonly ILogger<BudgetHandler> _logger;
private readonly IMapper _mapper;
```

## Constants and Enums

### Constants

```csharp
// PascalCase
public const int MaxBudgetNameLength = 100;
public const decimal MinimumBudgetAmount = 0.01m;
```

### Enums

```csharp
// Type and values both PascalCase
public enum BudgetStatus
{
    Active,
    Inactive,
    Archived
}

// Usage
var status = BudgetStatus.Active;
```

## Event Naming

```csharp
{Entity}{Action}Event

Examples:
BudgetCreatedEvent
BudgetUpdatedEvent
BudgetDeletedEvent
GoalAchievedEvent
PaymentProcessedEvent
```

## Anti-Patterns to Avoid

```csharp
// ❌ Avoid abbreviations
var bgt = ...;
var usrId = ...;

// ✅ Use full names
var budget = ...;
var userId = ...;

// ❌ Avoid generic names
var data = ...;
var temp = ...;
var item = ...;

// ✅ Use specific names
var budgetModel = ...;
var createdBudget = ...;
var activeBudget = ...;

// ❌ Avoid Hungarian notation
string strName = ...;
int intCount = ...;

// ✅ Use clear, typed names
string name = ...;
int count = ...;
```

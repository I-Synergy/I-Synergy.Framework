# Service-Oriented Architecture (SOA) Pattern

## Overview

Service-Oriented Architecture is an architectural pattern where functionality is organized as a collection of interoperable services. Each service is a discrete unit of functionality that can be accessed remotely and independently updated.

---

## Core SOA Principles

### 1. Standardized Service Contract

**Definition:** Services adhere to a communications agreement defined by one or more service descriptions.

```csharp
// ✅ CORRECT - Well-defined service contract
namespace {ApplicationName}.Contracts.Budgets;

/// <summary>
/// Service contract for budget management operations.
/// </summary>
public interface IBudgetService
{
    /// <summary>
    /// Creates a new budget.
    /// </summary>
    /// <param name="request">Budget creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created budget response with ID.</returns>
    Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves a budget by its identifier.
    /// </summary>
    /// <param name="budgetId">The budget identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Budget details or null if not found.</returns>
    Task<BudgetDto?> GetBudgetAsync(
        Guid budgetId,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing budget.
    /// </summary>
    /// <param name="budgetId">The budget identifier.</param>
    /// <param name="request">Update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success indicator.</returns>
    Task<bool> UpdateBudgetAsync(
        Guid budgetId,
        UpdateBudgetRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes a budget.
    /// </summary>
    /// <param name="budgetId">The budget identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success indicator.</returns>
    Task<bool> DeleteBudgetAsync(
        Guid budgetId,
        CancellationToken ct = default);
}

// ❌ WRONG - Poorly defined contract
public interface IBudgetService
{
    Task<object> DoSomething(Dictionary<string, object> data); // Unclear!
    Task Process(string xml); // String-based, not typed
    Budget GetBudget(int id); // Synchronous, uses int instead of Guid
}
```

### 2. Service Loose Coupling

**Definition:** Services minimize dependencies on each other.

```csharp
// ✅ CORRECT - Loosely coupled services
namespace {ApplicationName}.Services.Budgets;

public sealed class BudgetService : IBudgetService
{
    private readonly ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> _createHandler;
    private readonly IQueryHandler<GetBudgetByIdQuery, BudgetResponse> _getHandler;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher; // Only depends on abstraction

    public BudgetService(
        ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> createHandler,
        IQueryHandler<GetBudgetByIdQuery, BudgetResponse> getHandler,
        IMapper mapper,
        IEventPublisher eventPublisher)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        var command = _mapper.Map<CreateBudgetCommand>(request);
        var response = await _createHandler.HandleAsync(command, ct);

        // Publish event for interested services (loose coupling)
        await _eventPublisher.PublishAsync(
            new BudgetCreatedEvent(response.BudgetId, request.Name, request.Amount),
            ct);

        return response;
    }
}

// ❌ WRONG - Tightly coupled service
public sealed class BudgetService
{
    private readonly SqlBudgetRepository _repository; // Tight coupling to specific implementation
    private readonly SmtpEmailSender _emailSender; // Direct dependency
    private readonly GoalService _goalService; // Direct service-to-service dependency

    public async Task CreateBudgetAsync(CreateBudgetRequest request)
    {
        var budget = await _repository.SaveAsync(request);

        // Tight coupling - directly calling another service
        await _goalService.CreateDefaultGoalsAsync(budget.BudgetId);

        // Tight coupling - directly sending email
        await _emailSender.SendAsync("Budget created");
    }
}
```

### 3. Service Abstraction

**Definition:** Services hide implementation logic from consumers.

```csharp
// ✅ CORRECT - Well-abstracted service
namespace {ApplicationName}.Services.Budgets;

public sealed class BudgetCalculationService : IBudgetCalculationService
{
    private readonly IQueryHandler<GetBudgetByIdQuery, BudgetResponse> _getHandler;
    private readonly IQueryHandler<GetGoalsByBudgetQuery, List<GoalResponse>> _getGoalsHandler;
    private readonly ILogger<BudgetCalculationService> _logger;

    public BudgetCalculationService(
        IQueryHandler<GetBudgetByIdQuery, BudgetResponse> getHandler,
        IQueryHandler<GetGoalsByBudgetQuery, List<GoalResponse>> getGoalsHandler,
        ILogger<BudgetCalculationService> logger)
    {
        _getHandler = getHandler;
        _getGoalsHandler = getGoalsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Calculates remaining budget amount.
    /// Consumer doesn't need to know about goals, allocations, etc.
    /// </summary>
    public async Task<decimal> CalculateRemainingAsync(
        Guid budgetId,
        CancellationToken ct = default)
    {
        _logger.LogDebug("Calculating remaining for budget {BudgetId}", budgetId);

        // Internal complexity hidden from consumer
        var budget = await _getHandler.HandleAsync(
            new GetBudgetByIdQuery(budgetId),
            ct);

        var goals = await _getGoalsHandler.HandleAsync(
            new GetGoalsByBudgetQuery(budgetId),
            ct);

        var allocated = goals.Sum(g => g.TargetAmount);
        var remaining = budget.Amount - allocated;

        _logger.LogDebug(
            "Budget {BudgetId} has {Remaining} remaining",
            budgetId, remaining);

        return remaining;
    }
}

// ❌ WRONG - Leaky abstraction exposing internals
public interface IBudgetCalculationService
{
    Task<decimal> CalculateRemainingAsync(Guid budgetId);

    // Exposes internal implementation details
    Task<Dictionary<string, decimal>> GetInternalCalculationCache();
    Task<List<string>> GetDatabaseQueryLog();
    Task SetCachingStrategy(CachingStrategyEnum strategy);
}
```

### 4. Service Reusability

**Definition:** Logic is divided into services with the intention of promoting reuse.

```csharp
// ✅ CORRECT - Reusable service design
namespace {ApplicationName}.Services.Common;

/// <summary>
/// Reusable validation service for multiple domains.
/// </summary>
public interface IValidationService<T>
{
    Task<ValidationResult> ValidateAsync(T item, CancellationToken ct = default);
}

public sealed class BudgetValidationService : IValidationService<Budget>
{
    private readonly IQueryHandler<GetBudgetByIdQuery, BudgetResponse> _getHandler;

    public BudgetValidationService(
        IQueryHandler<GetBudgetByIdQuery, BudgetResponse> getHandler)
    {
        _getHandler = getHandler;
    }

    public async Task<ValidationResult> ValidateAsync(
        Budget budget,
        CancellationToken ct = default)
    {
        var errors = new List<string>();

        if (budget.Amount <= 0)
            errors.Add("Amount must be positive");

        if (string.IsNullOrWhiteSpace(budget.Name))
            errors.Add("Name is required");

        // Reusable duplicate check
        if (await IsDuplicateAsync(budget.Name, ct))
            errors.Add("Budget name already exists");

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Fail(errors);
    }

    private async Task<bool> IsDuplicateAsync(string name, CancellationToken ct)
    {
        // Reusable duplicate checking logic
        await Task.CompletedTask;
        return false;
    }
}

// Reuse same pattern for other entities
public sealed class GoalValidationService : IValidationService<Goal>
{
    public async Task<ValidationResult> ValidateAsync(Goal goal, CancellationToken ct = default)
    {
        // Same validation pattern, different rules
        var errors = new List<string>();

        if (goal.TargetAmount <= 0)
            errors.Add("Target amount must be positive");

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Fail(errors);
    }
}
```

### 5. Service Autonomy

**Definition:** Services have control over the logic they encapsulate.

```csharp
// ✅ CORRECT - Autonomous service with own data store
namespace {ApplicationName}.Services.Budgets;

public sealed class BudgetService : IBudgetService
{
    private readonly DataContext _dataContext; // Own database context
    private readonly IEventPublisher _eventPublisher;

    public BudgetService(DataContext dataContext, IEventPublisher eventPublisher)
    {
        _dataContext = dataContext;
        _eventPublisher = eventPublisher;
    }

    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        // Full autonomy over budget creation logic
        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = request.Name,
            Amount = request.Amount,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Own transaction boundary
        using var transaction = await _dataContext.Database.BeginTransactionAsync(ct);

        try
        {
            await _dataContext.AddItemAsync<Budget, BudgetModel>(
                budget.Adapt<BudgetModel>(),
                ct);

            await transaction.CommitAsync(ct);

            // Notify other services via events (maintains autonomy)
            await _eventPublisher.PublishAsync(
                new BudgetCreatedEvent(budget.BudgetId, budget.Name, budget.Amount),
                ct);

            return new CreateBudgetResponse(budget.BudgetId);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}

// ❌ WRONG - Lacks autonomy, depends on external coordination
public sealed class BudgetService
{
    private readonly ISharedTransactionManager _transactionManager; // External coordination
    private readonly ISharedDataStore _sharedData; // Shared data store

    public async Task CreateBudgetAsync(CreateBudgetRequest request)
    {
        // Waiting for external transaction manager
        await _transactionManager.BeginTransactionAsync();

        // Sharing data store with other services
        await _sharedData.SaveAsync(request);

        // Manually coordinating with other services
        await _transactionManager.NotifyServicesAsync("budget.created");

        await _transactionManager.CommitAsync();
    }
}
```

### 6. Service Statelessness

**Definition:** Services minimize retained state.

```csharp
// ✅ CORRECT - Stateless service
namespace {ApplicationName}.Services.Budgets;

public sealed class BudgetService : IBudgetService
{
    private readonly ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> _createHandler;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(
        ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> createHandler,
        ILogger<BudgetService> logger)
    {
        _createHandler = createHandler;
        _logger = logger;
    }

    // Stateless - all state passed via parameters
    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Creating budget {Name}", request.Name);

        var command = new CreateBudgetCommand(
            request.Name,
            request.Amount,
            request.StartDate);

        return await _createHandler.HandleAsync(command, ct);
    }

    // Stateless - retrieves fresh data each call
    public async Task<BudgetDto?> GetBudgetAsync(
        Guid budgetId,
        CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving budget {BudgetId}", budgetId);

        var query = new GetBudgetByIdQuery(budgetId);
        var response = await _getHandler.HandleAsync(query, ct);

        return response.Adapt<BudgetDto>();
    }
}

// ❌ WRONG - Stateful service (anti-pattern in SOA)
public sealed class BudgetService
{
    private Budget? _currentBudget; // Instance state!
    private readonly List<Budget> _cachedBudgets = new(); // Mutable state!

    public async Task<CreateBudgetResponse> CreateBudgetAsync(CreateBudgetRequest request)
    {
        var budget = CreateBudget(request);

        _currentBudget = budget; // Storing state!
        _cachedBudgets.Add(budget); // Mutating state!

        return new CreateBudgetResponse(budget.BudgetId);
    }

    public Budget? GetCurrentBudget() => _currentBudget; // Returning instance state!
}
```

### 7. Service Discoverability

**Definition:** Services are designed to be externally discoverable.

```csharp
// ✅ CORRECT - Discoverable service with metadata
namespace {ApplicationName}.Services.Budgets;

[ServiceContract(
    Name = "BudgetService",
    Version = "1.0",
    Description = "Provides budget management operations")]
public sealed class BudgetService : IBudgetService
{
    // Service metadata for discovery
    public ServiceMetadata GetMetadata()
    {
        return new ServiceMetadata
        {
            Name = "BudgetService",
            Version = "1.0",
            Description = "Budget management operations",
            Endpoints = new List<EndpointMetadata>
            {
                new("CreateBudget", "POST", "/api/budgets", "Creates a new budget"),
                new("GetBudget", "GET", "/api/budgets/{id}", "Retrieves a budget"),
                new("UpdateBudget", "PUT", "/api/budgets/{id}", "Updates a budget"),
                new("DeleteBudget", "DELETE", "/api/budgets/{id}", "Deletes a budget")
            },
            Capabilities = new List<string>
            {
                "Create", "Read", "Update", "Delete", "Calculate"
            }
        };
    }
}

// Service registration for discovery
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBudgetService(this IServiceCollection services)
    {
        services.AddScoped<IBudgetService, BudgetService>();

        // Register for service discovery
        services.AddSingleton<IServiceRegistry>(sp =>
        {
            var registry = sp.GetRequiredService<IServiceRegistry>();
            var budgetService = sp.GetRequiredService<IBudgetService>();

            if (budgetService is BudgetService service)
            {
                registry.Register(service.GetMetadata());
            }

            return registry;
        });

        return services;
    }
}

public record ServiceMetadata
{
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<EndpointMetadata> Endpoints { get; init; } = new();
    public List<string> Capabilities { get; init; } = new();
}

public record EndpointMetadata(
    string Name,
    string Method,
    string Path,
    string Description);
```

---

## Service Communication Patterns

### Message-Based Communication

```csharp
// ✅ CORRECT - Message-based communication
namespace {ApplicationName}.Services.Messaging;

// Message contract
public sealed record BudgetCreatedMessage
{
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
}

// Publisher service
public sealed class BudgetEventPublisher : IBudgetEventPublisher
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<BudgetEventPublisher> _logger;

    public BudgetEventPublisher(
        IMessageBus messageBus,
        ILogger<BudgetEventPublisher> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task PublishBudgetCreatedAsync(
        Budget budget,
        CancellationToken ct = default)
    {
        var message = new BudgetCreatedMessage
        {
            BudgetId = budget.BudgetId,
            Name = budget.Name,
            Amount = budget.Amount,
            CreatedDate = budget.CreatedDate,
            CreatedBy = budget.CreatedBy
        };

        _logger.LogInformation(
            "Publishing BudgetCreated message for {BudgetId}",
            budget.BudgetId);

        await _messageBus.PublishAsync("budget.created", message, ct);
    }
}

// Subscriber service
public sealed class NotificationService : IMessageHandler<BudgetCreatedMessage>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(
        BudgetCreatedMessage message,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Handling BudgetCreated message for {BudgetId}",
            message.BudgetId);

        await _emailService.SendAsync(
            message.CreatedBy,
            "Budget Created",
            $"Your budget '{message.Name}' with amount {message.Amount:C} has been created.",
            ct);
    }
}
```

### Request-Reply Pattern

```csharp
// ✅ CORRECT - Request-reply pattern
namespace {ApplicationName}.Services.Budgets;

// Request
public sealed record CalculateBudgetSummaryRequest
{
    public Guid BudgetId { get; init; }
    public bool IncludeGoals { get; init; }
    public bool IncludeDebts { get; init; }
}

// Reply
public sealed record CalculateBudgetSummaryReply
{
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal AllocatedAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public List<GoalSummary> Goals { get; init; } = new();
    public List<DebtSummary> Debts { get; init; } = new();
}

// Service implementing request-reply
public sealed class BudgetQueryService : IBudgetQueryService
{
    private readonly IQueryHandler<GetBudgetByIdQuery, BudgetResponse> _getBudgetHandler;
    private readonly IQueryHandler<GetGoalsByBudgetQuery, List<GoalResponse>> _getGoalsHandler;
    private readonly IQueryHandler<GetDebtsByBudgetQuery, List<DebtResponse>> _getDebtsHandler;

    public BudgetQueryService(
        IQueryHandler<GetBudgetByIdQuery, BudgetResponse> getBudgetHandler,
        IQueryHandler<GetGoalsByBudgetQuery, List<GoalResponse>> getGoalsHandler,
        IQueryHandler<GetDebtsByBudgetQuery, List<DebtResponse>> getDebtsHandler)
    {
        _getBudgetHandler = getBudgetHandler;
        _getGoalsHandler = getGoalsHandler;
        _getDebtsHandler = getDebtsHandler;
    }

    public async Task<CalculateBudgetSummaryReply> CalculateSummaryAsync(
        CalculateBudgetSummaryRequest request,
        CancellationToken ct = default)
    {
        var budget = await _getBudgetHandler.HandleAsync(
            new GetBudgetByIdQuery(request.BudgetId),
            ct);

        var goals = request.IncludeGoals
            ? await _getGoalsHandler.HandleAsync(
                new GetGoalsByBudgetQuery(request.BudgetId),
                ct)
            : new List<GoalResponse>();

        var debts = request.IncludeDebts
            ? await _getDebtsHandler.HandleAsync(
                new GetDebtsByBudgetQuery(request.BudgetId),
                ct)
            : new List<DebtResponse>();

        var allocated = goals.Sum(g => g.TargetAmount) + debts.Sum(d => d.Amount);

        return new CalculateBudgetSummaryReply
        {
            BudgetId = budget.BudgetId,
            Name = budget.Name,
            TotalAmount = budget.Amount,
            AllocatedAmount = allocated,
            RemainingAmount = budget.Amount - allocated,
            Goals = goals.Adapt<List<GoalSummary>>(),
            Debts = debts.Adapt<List<DebtSummary>>()
        };
    }
}
```

---

## Service Orchestration vs Choreography

### Orchestration (Centralized Coordination)

```csharp
// ✅ CORRECT - Service orchestration
namespace {ApplicationName}.Services.Orchestration;

/// <summary>
/// Orchestrator for budget creation workflow.
/// Centralized coordination of multiple services.
/// </summary>
public sealed class BudgetCreationOrchestrator : IBudgetCreationOrchestrator
{
    private readonly IBudgetService _budgetService;
    private readonly IGoalService _goalService;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;
    private readonly ILogger<BudgetCreationOrchestrator> _logger;

    public BudgetCreationOrchestrator(
        IBudgetService budgetService,
        IGoalService goalService,
        INotificationService notificationService,
        IAuditService auditService,
        ILogger<BudgetCreationOrchestrator> logger)
    {
        _budgetService = budgetService;
        _goalService = goalService;
        _notificationService = notificationService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<BudgetCreationResult> CreateBudgetWithGoalsAsync(
        CreateBudgetWithGoalsRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Starting orchestrated budget creation for {BudgetName}",
            request.BudgetName);

        try
        {
            // Step 1: Create budget
            var budgetRequest = new CreateBudgetRequest
            {
                Name = request.BudgetName,
                Amount = request.Amount,
                StartDate = request.StartDate
            };

            var budgetResponse = await _budgetService.CreateBudgetAsync(budgetRequest, ct);

            // Step 2: Create goals (sequential orchestration)
            var goalIds = new List<Guid>();
            foreach (var goalRequest in request.Goals)
            {
                var goalResponse = await _goalService.CreateGoalAsync(
                    new CreateGoalRequest
                    {
                        BudgetId = budgetResponse.BudgetId,
                        Name = goalRequest.Name,
                        TargetAmount = goalRequest.TargetAmount
                    },
                    ct);

                goalIds.Add(goalResponse.GoalId);
            }

            // Step 3: Send notification
            await _notificationService.SendBudgetCreatedNotificationAsync(
                budgetResponse.BudgetId,
                request.CreatedBy,
                ct);

            // Step 4: Audit
            await _auditService.LogBudgetCreationAsync(
                budgetResponse.BudgetId,
                request.CreatedBy,
                ct);

            _logger.LogInformation(
                "Completed orchestrated budget creation for {BudgetId}",
                budgetResponse.BudgetId);

            return new BudgetCreationResult
            {
                BudgetId = budgetResponse.BudgetId,
                GoalIds = goalIds,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed orchestrated budget creation for {BudgetName}",
                request.BudgetName);

            throw;
        }
    }
}
```

### Choreography (Decentralized Coordination)

```csharp
// ✅ CORRECT - Service choreography
namespace {ApplicationName}.Services.Events;

// Budget service publishes event (doesn't know about consumers)
public sealed class BudgetService : IBudgetService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly DataContext _dataContext;

    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = request.Name,
            Amount = request.Amount
        };

        await _dataContext.AddItemAsync<Budget, BudgetModel>(
            budget.Adapt<BudgetModel>(),
            ct);

        // Publish event - no knowledge of consumers
        await _eventPublisher.PublishAsync(
            new BudgetCreatedEvent
            {
                BudgetId = budget.BudgetId,
                Name = budget.Name,
                Amount = budget.Amount,
                CreatedBy = request.CreatedBy
            },
            ct);

        return new CreateBudgetResponse(budget.BudgetId);
    }
}

// Goal service reacts to event
public sealed class GoalEventHandler : IEventHandler<BudgetCreatedEvent>
{
    private readonly IGoalService _goalService;

    public async Task HandleAsync(BudgetCreatedEvent domainEvent, CancellationToken ct)
    {
        // React to budget creation by creating default goals
        await _goalService.CreateDefaultGoalsAsync(domainEvent.BudgetId, ct);
    }
}

// Notification service reacts to event
public sealed class NotificationEventHandler : IEventHandler<BudgetCreatedEvent>
{
    private readonly INotificationService _notificationService;

    public async Task HandleAsync(BudgetCreatedEvent domainEvent, CancellationToken ct)
    {
        // React to budget creation by sending notification
        await _notificationService.SendBudgetCreatedNotificationAsync(
            domainEvent.BudgetId,
            domainEvent.CreatedBy,
            ct);
    }
}

// Audit service reacts to event
public sealed class AuditEventHandler : IEventHandler<BudgetCreatedEvent>
{
    private readonly IAuditService _auditService;

    public async Task HandleAsync(BudgetCreatedEvent domainEvent, CancellationToken ct)
    {
        // React to budget creation by logging audit entry
        await _auditService.LogBudgetCreationAsync(
            domainEvent.BudgetId,
            domainEvent.CreatedBy,
            ct);
    }
}
```

---

## Service Versioning

### URL Versioning

```csharp
// ✅ CORRECT - URL versioning
namespace {ApplicationName}.Services.Budgets.V1;

public static class BudgetEndpointsV1
{
    public static void MapBudgetEndpointsV1(
        this IEndpointRouteBuilder app,
        ApiVersionSet versionSet)
    {
        var group = app.MapGroup("/v1/budgets")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1.0)
            .WithTags("Budgets V1");

        group.MapPost("/", CreateBudgetV1);
        group.MapGet("/{id:guid}", GetBudgetV1);
    }

    private static async Task<IResult> CreateBudgetV1(
        CreateBudgetRequestV1 request,
        IBudgetService budgetService,
        CancellationToken ct)
    {
        var response = await budgetService.CreateBudgetAsync(request, ct);
        return Results.Created($"/v1/budgets/{response.BudgetId}", response);
    }

    private static async Task<IResult> GetBudgetV1(
        Guid id,
        IBudgetService budgetService,
        CancellationToken ct)
    {
        var budget = await budgetService.GetBudgetAsync(id, ct);
        return budget is not null ? Results.Ok(budget) : Results.NotFound();
    }
}

namespace {ApplicationName}.Services.Budgets.V2;

public static class BudgetEndpointsV2
{
    public static void MapBudgetEndpointsV2(
        this IEndpointRouteBuilder app,
        ApiVersionSet versionSet)
    {
        var group = app.MapGroup("/v2/budgets")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(2.0)
            .WithTags("Budgets V2");

        group.MapPost("/", CreateBudgetV2); // Enhanced version
        group.MapGet("/{id:guid}", GetBudgetV2); // Enhanced version
    }

    private static async Task<IResult> CreateBudgetV2(
        CreateBudgetRequestV2 request, // New request model
        IBudgetServiceV2 budgetService, // V2 service
        CancellationToken ct)
    {
        var response = await budgetService.CreateBudgetAsync(request, ct);
        return Results.Created($"/v2/budgets/{response.BudgetId}", response);
    }
}
```

### Header Versioning

```csharp
// ✅ CORRECT - Header versioning
public sealed class ApiVersionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Read version from header
        if (context.Request.Headers.TryGetValue("api-version", out var version))
        {
            context.Items["ApiVersion"] = version.ToString();
        }
        else
        {
            context.Items["ApiVersion"] = "1.0"; // Default version
        }

        await _next(context);
    }
}

// Service implementation with version awareness
public sealed class BudgetService : IBudgetService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        var apiVersion = _httpContextAccessor.HttpContext?.Items["ApiVersion"]?.ToString();

        return apiVersion switch
        {
            "2.0" => await CreateBudgetV2Async(request, ct),
            _ => await CreateBudgetV1Async(request, ct)
        };
    }
}
```

---

## Idempotency

```csharp
// ✅ CORRECT - Idempotent service operations
namespace {ApplicationName}.Services.Budgets;

public sealed class BudgetService : IBudgetService
{
    private readonly DataContext _dataContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<BudgetService> _logger;

    public async Task<CreateBudgetResponse> CreateBudgetAsync(
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        // Use idempotency key to prevent duplicate creation
        var idempotencyKey = request.IdempotencyKey ??
            throw new ArgumentException("Idempotency key required");

        // Check if already processed
        var cached = await _cache.GetStringAsync($"idempotency:{idempotencyKey}", ct);
        if (cached is not null)
        {
            _logger.LogInformation(
                "Request already processed, returning cached response for key {IdempotencyKey}",
                idempotencyKey);

            return JsonSerializer.Deserialize<CreateBudgetResponse>(cached)!;
        }

        // Process request
        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = request.Name,
            Amount = request.Amount
        };

        await _dataContext.AddItemAsync<Budget, BudgetModel>(
            budget.Adapt<BudgetModel>(),
            ct);

        var response = new CreateBudgetResponse(budget.BudgetId);

        // Cache response for idempotency
        await _cache.SetStringAsync(
            $"idempotency:{idempotencyKey}",
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            },
            ct);

        return response;
    }
}

public sealed record CreateBudgetRequest
{
    public string IdempotencyKey { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
```

---

## Transaction Management

### Distributed Transactions (Saga Pattern)

```csharp
// ✅ CORRECT - Saga pattern for distributed transactions
namespace {ApplicationName}.Services.Sagas;

public sealed class BudgetCreationSaga : ISaga
{
    private readonly IBudgetService _budgetService;
    private readonly IGoalService _goalService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<BudgetCreationSaga> _logger;

    public async Task<SagaResult> ExecuteAsync(
        BudgetCreationSagaData data,
        CancellationToken ct = default)
    {
        Guid? budgetId = null;
        var goalIds = new List<Guid>();

        try
        {
            // Step 1: Create budget
            var budgetResponse = await _budgetService.CreateBudgetAsync(
                new CreateBudgetRequest
                {
                    Name = data.BudgetName,
                    Amount = data.Amount
                },
                ct);

            budgetId = budgetResponse.BudgetId;

            // Step 2: Create goals
            foreach (var goalData in data.Goals)
            {
                var goalResponse = await _goalService.CreateGoalAsync(
                    new CreateGoalRequest
                    {
                        BudgetId = budgetId.Value,
                        Name = goalData.Name,
                        TargetAmount = goalData.TargetAmount
                    },
                    ct);

                goalIds.Add(goalResponse.GoalId);
            }

            // Step 3: Send notification
            await _notificationService.SendBudgetCreatedNotificationAsync(
                budgetId.Value,
                data.CreatedBy,
                ct);

            return SagaResult.Success(budgetId.Value, goalIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga failed, initiating compensation");

            // Compensating transactions (rollback)
            await CompensateAsync(budgetId, goalIds, ct);

            return SagaResult.Failed(ex.Message);
        }
    }

    private async Task CompensateAsync(
        Guid? budgetId,
        List<Guid> goalIds,
        CancellationToken ct)
    {
        // Reverse operations in opposite order

        // Remove goals
        foreach (var goalId in goalIds)
        {
            try
            {
                await _goalService.DeleteGoalAsync(goalId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to compensate goal {GoalId}", goalId);
            }
        }

        // Remove budget
        if (budgetId.HasValue)
        {
            try
            {
                await _budgetService.DeleteBudgetAsync(budgetId.Value, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to compensate budget {BudgetId}", budgetId);
            }
        }
    }
}
```

---

## SOA vs Microservices

| Aspect | SOA | Microservices |
|--------|-----|---------------|
| **Scope** | Enterprise-wide | Bounded context/domain |
| **Communication** | ESB, SOAP, sometimes REST | REST, gRPC, message queues |
| **Data** | Shared database common | Database per service |
| **Governance** | Centralized | Decentralized |
| **Size** | Larger services | Smaller, focused services |
| **Deployment** | Monolithic deployment common | Independent deployment |
| **Technology** | Standardized stack | Polyglot (multiple technologies) |

---

## Common Pitfalls

### Pitfall 1: Chatty Service Communication

```csharp
// ❌ WRONG - Multiple service calls for related data
public async Task<BudgetSummary> GetBudgetSummaryAsync(Guid budgetId)
{
    var budget = await _budgetService.GetBudgetAsync(budgetId);
    var goals = await _goalService.GetGoalsByBudgetIdAsync(budgetId); // Separate call
    var debts = await _debtService.GetDebtsByBudgetIdAsync(budgetId); // Separate call

    // Multiple network roundtrips!
    return new BudgetSummary(budget, goals, debts);
}

// ✅ CORRECT - Single call with composite data
public async Task<BudgetSummary> GetBudgetSummaryAsync(Guid budgetId)
{
    // Single service call returns all related data
    return await _budgetQueryService.GetBudgetSummaryAsync(budgetId);
}
```

### Pitfall 2: Tight Service Coupling

```csharp
// ❌ WRONG - Direct service-to-service calls
public async Task CreateBudgetAsync(CreateBudgetRequest request)
{
    var budget = await SaveBudgetAsync(request);

    // Direct coupling to other services
    await _goalService.CreateDefaultGoalsAsync(budget.BudgetId);
    await _notificationService.NotifyAsync(budget);
}

// ✅ CORRECT - Event-based decoupling
public async Task CreateBudgetAsync(CreateBudgetRequest request)
{
    var budget = await SaveBudgetAsync(request);

    // Publish event - services react independently
    await _eventPublisher.PublishAsync(new BudgetCreatedEvent(budget));
}
```

---

## Summary Checklist

- [ ] **Service contracts** - Clear, well-documented interfaces
- [ ] **Loose coupling** - Minimal dependencies between services
- [ ] **Abstraction** - Implementation details hidden
- [ ] **Reusability** - Services designed for multiple consumers
- [ ] **Autonomy** - Services control their own logic and data
- [ ] **Statelessness** - Minimal retained state between calls
- [ ] **Discoverability** - Services externally discoverable
- [ ] **Message-based** - Asynchronous communication where appropriate
- [ ] **Versioning** - Clear versioning strategy
- [ ] **Idempotency** - Operations safely repeatable
- [ ] **Transaction management** - Saga pattern for distributed transactions
- [ ] **Error handling** - Proper fault tolerance and resilience

---

## Related Patterns

- [Object-Oriented Programming](./object-oriented-programming.md) - OOP principles
- [Microservices](./microservices.md) - Microservices architecture
- [Test-Driven Development](./test-driven-development.md) - TDD methodology

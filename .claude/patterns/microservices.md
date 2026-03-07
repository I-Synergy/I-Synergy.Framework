# Microservices Architecture Pattern

## Overview

Microservices architecture is an approach to developing a single application as a suite of small, independently deployable services. Each service runs in its own process, communicates via lightweight mechanisms (typically HTTP/REST or messaging), and is organized around business capabilities. This pattern enables continuous delivery, scalability, and technology diversity.

---

## Core Principles

### 1. Single Responsibility

Each microservice should have a single, well-defined responsibility aligned with a business capability.

```csharp
// ✅ CORRECT - Single responsibility per service

// BudgetService - Manages budgets only
public sealed class BudgetService
{
    [HttpPost("/api/budgets")]
    public async Task<IResult> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        // Budget creation logic only
    }
}

// GoalService - Manages goals only
public sealed class GoalService
{
    [HttpPost("/api/goals")]
    public async Task<IResult> CreateGoal([FromBody] CreateGoalRequest request)
    {
        // Goal creation logic only
    }
}

// ❌ WRONG - Multiple responsibilities in one service
public sealed class FinancialService
{
    [HttpPost("/api/budgets")]
    public async Task<IResult> CreateBudget() { /* ... */ }

    [HttpPost("/api/goals")]
    public async Task<IResult> CreateGoal() { /* ... */ }

    [HttpPost("/api/transactions")]
    public async Task<IResult> CreateTransaction() { /* ... */ }

    [HttpPost("/api/reports")]
    public async Task<IResult> GenerateReport() { /* ... */ }
}
```

### 2. Autonomy

Services should be independently deployable and scalable.

```csharp
// Each service has its own:
// - Codebase
// - Database
// - Deployment pipeline
// - Scaling configuration

// BudgetService - Deployed independently
// GoalService - Deployed independently
// NotificationService - Deployed independently
```

### 3. Decentralization

Decentralize data management and governance.

```csharp
// ✅ CORRECT - Each service owns its data

// BudgetService database
public sealed class BudgetDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetCategory> Categories { get; set; }
}

// GoalService database
public sealed class GoalDbContext : DbContext
{
    public DbSet<Goal> Goals { get; set; }
    public DbSet<GoalMilestone> Milestones { get; set; }
}

// ❌ WRONG - Shared database across services
public sealed class SharedDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    // All services accessing same database!
}
```

---

## Service Boundaries and Bounded Contexts (DDD)

### Identifying Bounded Contexts

Use Domain-Driven Design to identify service boundaries.

```
Financial Planning Application

Bounded Contexts:
├── Budget Management (BudgetService)
│   ├── Budgets
│   ├── Categories
│   └── Budget Allocations
│
├── Goal Management (GoalService)
│   ├── Financial Goals
│   ├── Milestones
│   └── Progress Tracking
│
├── Transaction Management (TransactionService)
│   ├── Income
│   ├── Expenses
│   └── Transfers
│
├── Reporting (ReportingService)
│   ├── Financial Reports
│   ├── Analytics
│   └── Dashboards
│
└── Notification (NotificationService)
    ├── Email Notifications
    ├── SMS Notifications
    └── Push Notifications
```

### Context Mapping

```csharp
// File: {ApplicationName}.Services.Budget/Domain/Budget.cs

namespace {ApplicationName}.Services.Budget.Domain;

/// <summary>
/// Budget aggregate in Budget Management context.
/// </summary>
public sealed class Budget
{
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public Guid UserId { get; init; } // Reference to User context
    // Budget-specific properties and behavior
}

// File: {ApplicationName}.Services.Goal/Domain/Goal.cs

namespace {ApplicationName}.Services.Goal.Domain;

/// <summary>
/// Goal aggregate in Goal Management context.
/// </summary>
public sealed class Goal
{
    public Guid GoalId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal TargetAmount { get; init; }
    public Guid BudgetId { get; init; } // Reference to Budget context
    // Goal-specific properties and behavior
}
```

---

## API Gateway Pattern

### Backend for Frontend (BFF)

API Gateway aggregates requests from clients and routes them to appropriate microservices.

```csharp
// File: {ApplicationName}.Gateway/Program.cs

using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure YARP (Yet Another Reverse Proxy)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

// Add authorization
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// YARP reverse proxy middleware
app.MapReverseProxy();

app.Run();
```

**Configuration (appsettings.json):**
```json
{
  "ReverseProxy": {
    "Routes": {
      "budget-route": {
        "ClusterId": "budget-cluster",
        "Match": {
          "Path": "/api/budgets/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/budgets/{**catch-all}" }
        ]
      },
      "goal-route": {
        "ClusterId": "goal-cluster",
        "Match": {
          "Path": "/api/goals/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/goals/{**catch-all}" }
        ]
      },
      "transaction-route": {
        "ClusterId": "transaction-cluster",
        "Match": {
          "Path": "/api/transactions/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "budget-cluster": {
        "Destinations": {
          "budget-service": {
            "Address": "http://budget-service:8080"
          }
        }
      },
      "goal-cluster": {
        "Destinations": {
          "goal-service": {
            "Address": "http://goal-service:8080"
          }
        }
      },
      "transaction-cluster": {
        "Destinations": {
          "transaction-service": {
            "Address": "http://transaction-service:8080"
          }
        }
      }
    }
  }
}
```

### Request Aggregation

```csharp
// File: {ApplicationName}.Gateway/Services/DashboardAggregationService.cs

namespace {ApplicationName}.Gateway.Services;

/// <summary>
/// Aggregates data from multiple microservices for dashboard.
/// </summary>
public sealed class DashboardAggregationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DashboardAggregationService> _logger;

    public DashboardAggregationService(
        HttpClient httpClient,
        ILogger<DashboardAggregationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DashboardResponse> GetDashboardDataAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Call multiple services in parallel
        var budgetTask = GetBudgetsAsync(userId, cancellationToken);
        var goalTask = GetGoalsAsync(userId, cancellationToken);
        var transactionTask = GetRecentTransactionsAsync(userId, cancellationToken);

        await Task.WhenAll(budgetTask, goalTask, transactionTask);

        return new DashboardResponse
        {
            Budgets = await budgetTask,
            Goals = await goalTask,
            RecentTransactions = await transactionTask
        };
    }

    private async Task<List<BudgetSummary>> GetBudgetsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"http://budget-service/api/budgets?userId={userId}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<BudgetSummary>>(
            cancellationToken: cancellationToken) ?? new();
    }

    private async Task<List<GoalSummary>> GetGoalsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"http://goal-service/api/goals?userId={userId}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<GoalSummary>>(
            cancellationToken: cancellationToken) ?? new();
    }

    private async Task<List<TransactionSummary>> GetRecentTransactionsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"http://transaction-service/api/transactions/recent?userId={userId}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<TransactionSummary>>(
            cancellationToken: cancellationToken) ?? new();
    }
}
```

---

## Inter-Service Communication

### Synchronous Communication (REST)

```csharp
// File: {ApplicationName}.Services.Goal/Services/BudgetServiceClient.cs

namespace {ApplicationName}.Services.Goal.Services;

/// <summary>
/// Client for communicating with Budget Service.
/// </summary>
public sealed class BudgetServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BudgetServiceClient> _logger;

    public BudgetServiceClient(
        HttpClient httpClient,
        ILogger<BudgetServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BudgetDto?> GetBudgetByIdAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching budget {BudgetId} from Budget Service", budgetId);

            var response = await _httpClient.GetAsync(
                $"/api/budgets/{budgetId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to fetch budget {BudgetId}. Status: {StatusCode}",
                    budgetId, response.StatusCode);
                return null;
            }

            var budget = await response.Content.ReadFromJsonAsync<BudgetDto>(
                cancellationToken: cancellationToken);

            return budget;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching budget {BudgetId}", budgetId);
            throw;
        }
    }
}

// Registration in Program.cs
builder.Services.AddHttpClient<BudgetServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:BudgetService:Url"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddStandardResilienceHandler(); // Adds retry, circuit breaker, timeout
```

### Synchronous Communication (gRPC)

```csharp
// File: {ApplicationName}.Services.Budget/Protos/budget.proto

syntax = "proto3";

option csharp_namespace = "{ApplicationName}.Services.Budget.Grpc";

package budget;

service BudgetService {
  rpc GetBudget (GetBudgetRequest) returns (BudgetResponse);
  rpc CreateBudget (CreateBudgetRequest) returns (CreateBudgetResponse);
  rpc UpdateBudget (UpdateBudgetRequest) returns (UpdateBudgetResponse);
}

message GetBudgetRequest {
  string budget_id = 1;
}

message BudgetResponse {
  string budget_id = 1;
  string name = 2;
  double total_amount = 3;
  string user_id = 4;
}

message CreateBudgetRequest {
  string name = 1;
  double total_amount = 2;
  string user_id = 3;
}

message CreateBudgetResponse {
  string budget_id = 1;
}

message UpdateBudgetRequest {
  string budget_id = 1;
  string name = 2;
  double total_amount = 3;
}

message UpdateBudgetResponse {
  bool success = 1;
}
```

**gRPC Service Implementation:**
```csharp
// File: {ApplicationName}.Services.Budget/GrpcServices/BudgetGrpcService.cs

using Grpc.Core;
using {ApplicationName}.Services.Budget.Grpc;

namespace {ApplicationName}.Services.Budget.GrpcServices;

public sealed class BudgetGrpcService : BudgetService.BudgetServiceBase
{
    private readonly IQueryHandler<GetBudgetByIdQuery, BudgetResponse> _queryHandler;
    private readonly ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> _createHandler;
    private readonly ILogger<BudgetGrpcService> _logger;

    public BudgetGrpcService(
        IQueryHandler<GetBudgetByIdQuery, BudgetResponse> queryHandler,
        ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> createHandler,
        ILogger<BudgetGrpcService> logger)
    {
        _queryHandler = queryHandler;
        _createHandler = createHandler;
        _logger = logger;
    }

    public override async Task<BudgetResponse> GetBudget(
        GetBudgetRequest request,
        ServerCallContext context)
    {
        var query = new GetBudgetByIdQuery(Guid.Parse(request.BudgetId));
        var result = await _queryHandler.HandleAsync(query, context.CancellationToken);

        return new BudgetResponse
        {
            BudgetId = result.BudgetId.ToString(),
            Name = result.Name,
            TotalAmount = (double)result.Amount,
            UserId = result.UserId.ToString()
        };
    }

    public override async Task<CreateBudgetResponse> CreateBudget(
        CreateBudgetRequest request,
        ServerCallContext context)
    {
        var command = new CreateBudgetCommand(
            request.Name,
            (decimal)request.TotalAmount,
            DateTimeOffset.UtcNow);

        var result = await _createHandler.HandleAsync(command, context.CancellationToken);

        return new CreateBudgetResponse
        {
            BudgetId = result.BudgetId.ToString()
        };
    }
}
```

**gRPC Client:**
```csharp
// File: {ApplicationName}.Services.Goal/Services/BudgetGrpcClient.cs

using {ApplicationName}.Services.Budget.Grpc;

namespace {ApplicationName}.Services.Goal.Services;

public sealed class BudgetGrpcClient
{
    private readonly BudgetService.BudgetServiceClient _client;
    private readonly ILogger<BudgetGrpcClient> _logger;

    public BudgetGrpcClient(
        BudgetService.BudgetServiceClient client,
        ILogger<BudgetGrpcClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<BudgetResponse?> GetBudgetAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetBudgetRequest { BudgetId = budgetId.ToString() };
            return await _client.GetBudgetAsync(request, cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "gRPC call failed for budget {BudgetId}", budgetId);
            return null;
        }
    }
}

// Registration
builder.Services.AddGrpcClient<BudgetService.BudgetServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Services:BudgetService:GrpcUrl"]!);
});
```

### Asynchronous Communication (Message Queue)

```csharp
// File: {ApplicationName}.Services.Budget/Events/BudgetCreatedEvent.cs

namespace {ApplicationName}.Services.Budget.Events;

/// <summary>
/// Event published when a budget is created.
/// </summary>
public sealed record BudgetCreatedEvent
{
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public Guid UserId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
```

**Publishing Events (using MassTransit):**
```csharp
// File: {ApplicationName}.Services.Budget/Handlers/CreateBudgetHandler.cs

using MassTransit;

public sealed class CreateBudgetHandler(
    DataContext dataContext,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateBudgetHandler> logger
) : ICommandHandler<CreateBudgetCommand, CreateBudgetResponse>
{
    public async Task<CreateBudgetResponse> HandleAsync(
        CreateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        // Create budget
        var entity = command.Adapt<Budget>();
        var model = entity.Adapt<BudgetModel>();
        await dataContext.AddItemAsync<Budget, BudgetModel>(model, cancellationToken);

        // Publish event
        var @event = new BudgetCreatedEvent
        {
            BudgetId = entity.BudgetId,
            Name = entity.Name,
            TotalAmount = entity.TotalAmount,
            UserId = entity.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await publishEndpoint.Publish(@event, cancellationToken);

        logger.LogInformation(
            "Published BudgetCreatedEvent for budget {BudgetId}",
            entity.BudgetId);

        return new CreateBudgetResponse(entity.BudgetId);
    }
}
```

**Consuming Events:**
```csharp
// File: {ApplicationName}.Services.Notification/Consumers/BudgetCreatedEventConsumer.cs

using MassTransit;
using {ApplicationName}.Services.Budget.Events;

namespace {ApplicationName}.Services.Notification.Consumers;

/// <summary>
/// Consumes BudgetCreatedEvent and sends notification.
/// </summary>
public sealed class BudgetCreatedEventConsumer : IConsumer<BudgetCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BudgetCreatedEventConsumer> _logger;

    public BudgetCreatedEventConsumer(
        IEmailService emailService,
        ILogger<BudgetCreatedEventConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BudgetCreatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing BudgetCreatedEvent for budget {BudgetId}",
            @event.BudgetId);

        // Send notification email
        await _emailService.SendBudgetCreatedEmailAsync(
            @event.UserId,
            @event.Name,
            @event.TotalAmount,
            context.CancellationToken);

        _logger.LogInformation(
            "Sent notification for budget {BudgetId}",
            @event.BudgetId);
    }
}

// Registration in Program.cs
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BudgetCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        cfg.ConfigureEndpoints(context);
    });
});
```

---

## Database Per Service Pattern

Each microservice owns its database schema and data.

```csharp
// BudgetService - PostgreSQL
public sealed class BudgetDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetCategory> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=budget-db;Database=BudgetDb;");
    }
}

// GoalService - PostgreSQL (separate database)
public sealed class GoalDbContext : DbContext
{
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Milestone> Milestones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=goal-db;Database=GoalDb;");
    }
}

// NotificationService - MongoDB (different database type)
public sealed class NotificationDbContext
{
    private readonly IMongoDatabase _database;

    public NotificationDbContext(IMongoClient client)
    {
        _database = client.GetDatabase("NotificationDb");
    }

    public IMongoCollection<NotificationLog> Notifications =>
        _database.GetCollection<NotificationLog>("notifications");
}
```

---

## Saga Pattern for Distributed Transactions

### Choreography-Based Saga

Services communicate through events without a central coordinator.

```csharp
// File: {ApplicationName}.Services.Order/Events/OrderCreatedEvent.cs

public sealed record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
}

// File: {ApplicationName}.Services.Payment/Consumers/OrderCreatedEventConsumer.cs

public sealed class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var @event = context.Message;

        // Process payment
        var paymentSuccess = await ProcessPaymentAsync(@event.TotalAmount);

        if (paymentSuccess)
        {
            await _publishEndpoint.Publish(new PaymentProcessedEvent
            {
                OrderId = @event.OrderId,
                Amount = @event.TotalAmount,
                Success = true
            });
        }
        else
        {
            await _publishEndpoint.Publish(new PaymentFailedEvent
            {
                OrderId = @event.OrderId,
                Reason = "Insufficient funds"
            });
        }
    }
}

// File: {ApplicationName}.Services.Order/Consumers/PaymentFailedEventConsumer.cs

public sealed class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        // Compensating transaction - cancel order
        await CancelOrderAsync(context.Message.OrderId);
    }
}
```

### Orchestration-Based Saga

A central orchestrator manages the saga workflow.

```csharp
// File: {ApplicationName}.Services.Orchestrator/Sagas/OrderSaga.cs

using MassTransit;

namespace {ApplicationName}.Services.Orchestrator.Sagas;

public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
}

public sealed class OrderSaga : MassTransitStateMachine<OrderState>
{
    public State OrderCreated { get; private set; } = null!;
    public State PaymentProcessed { get; private set; } = null!;
    public State InventoryReserved { get; private set; } = null!;
    public State OrderCompleted { get; private set; } = null!;
    public State OrderFailed { get; private set; } = null!;

    public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; } = null!;
    public Event<PaymentProcessedEvent> PaymentProcessedEvent { get; private set; } = null!;
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; private set; } = null!;
    public Event<InventoryReservedEvent> InventoryReservedEvent { get; private set; } = null!;
    public Event<InventoryReservationFailedEvent> InventoryReservationFailedEvent { get; private set; } = null!;

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreatedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReservedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReservationFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                })
                .TransitionTo(OrderCreated)
                .Publish(context => new ProcessPaymentCommand
                {
                    OrderId = context.Saga.OrderId,
                    Amount = context.Saga.TotalAmount
                }));

        During(OrderCreated,
            When(PaymentProcessedEvent)
                .TransitionTo(PaymentProcessed)
                .Publish(context => new ReserveInventoryCommand
                {
                    OrderId = context.Saga.OrderId
                }),
            When(PaymentFailedEvent)
                .TransitionTo(OrderFailed)
                .Publish(context => new CancelOrderCommand
                {
                    OrderId = context.Saga.OrderId,
                    Reason = context.Message.Reason
                }));

        During(PaymentProcessed,
            When(InventoryReservedEvent)
                .TransitionTo(OrderCompleted)
                .Publish(context => new CompleteOrderCommand
                {
                    OrderId = context.Saga.OrderId
                })
                .Finalize(),
            When(InventoryReservationFailedEvent)
                .TransitionTo(OrderFailed)
                .Publish(context => new RefundPaymentCommand
                {
                    OrderId = context.Saga.OrderId,
                    Amount = context.Saga.TotalAmount
                })
                .Publish(context => new CancelOrderCommand
                {
                    OrderId = context.Saga.OrderId,
                    Reason = "Inventory not available"
                }));

        SetCompletedWhenFinalized();
    }
}
```

---

## Event-Driven Architecture with Domain Events

```csharp
// File: {ApplicationName}.Common/Events/IDomainEvent.cs

namespace {ApplicationName}.Common.Events;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; init; }
    DateTimeOffset OccurredAt { get; init; }
}

// File: {ApplicationName}.Services.Budget/Events/BudgetUpdatedEvent.cs

public sealed record BudgetUpdatedEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal OldAmount { get; init; }
    public decimal NewAmount { get; init; }
    public Guid UserId { get; init; }
}

// File: {ApplicationName}.Services.Reporting/Consumers/BudgetUpdatedEventConsumer.cs

public sealed class BudgetUpdatedEventConsumer : IConsumer<BudgetUpdatedEvent>
{
    private readonly ILogger<BudgetUpdatedEventConsumer> _logger;
    private readonly ReportingDbContext _dbContext;

    public async Task Consume(ConsumeContext<BudgetUpdatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Updating reporting data for budget {BudgetId}",
            @event.BudgetId);

        // Update read model for reporting
        var reportEntry = await _dbContext.BudgetReports
            .FirstOrDefaultAsync(x => x.BudgetId == @event.BudgetId);

        if (reportEntry is not null)
        {
            reportEntry.Name = @event.Name;
            reportEntry.Amount = @event.NewAmount;
            reportEntry.LastUpdated = @event.OccurredAt;

            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }
    }
}
```

---

## Service Discovery

### Using .NET Aspire Service Discovery

```csharp
// File: {ApplicationName}.AppHost/Program.cs (Aspire orchestration)

var builder = DistributedApplication.CreateBuilder(args);

// Define services
var budgetService = builder.AddProject<Projects.BudgetService>("budget-service")
    .WithHttpEndpoint(port: 8081, name: "http");

var goalService = builder.AddProject<Projects.GoalService>("goal-service")
    .WithHttpEndpoint(port: 8082, name: "http");

var gateway = builder.AddProject<Projects.Gateway>("gateway")
    .WithHttpEndpoint(port: 8080, name: "http")
    .WithReference(budgetService)
    .WithReference(goalService);

builder.Build().Run();
```

**Service Discovery in Consumer:**
```csharp
// File: {ApplicationName}.Services.Goal/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add service discovery
builder.Services.AddServiceDiscovery();

// Add HttpClient with service discovery
builder.Services.AddHttpClient<BudgetServiceClient>(client =>
{
    // Service name from Aspire
    client.BaseAddress = new Uri("http://budget-service");
})
.AddServiceDiscovery() // Resolves service address automatically
.AddStandardResilienceHandler();

var app = builder.Build();
app.Run();
```

---

## Circuit Breaker and Resilience Patterns

```csharp
// File: {ApplicationName}.Services.Goal/Program.cs

using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<BudgetServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:BudgetService:Url"]!);
})
.AddStandardResilienceHandler(options =>
{
    // Retry configuration
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.Retry.UseJitter = true;
    options.Retry.Delay = TimeSpan.FromSeconds(2);

    // Circuit breaker configuration
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.MinimumThroughput = 10;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    // Timeout configuration
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();
app.Run();
```

---

## Distributed Tracing with OpenTelemetry

```csharp
// File: {ApplicationName}.Services.Budget/Program.cs

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("budget-service"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource("MassTransit")
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]!);
        }));

var app = builder.Build();
app.Run();
```

**Correlation ID Propagation:**
```csharp
// File: {ApplicationName}.Common/Middleware/CorrelationIdMiddleware.cs

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers.Add(CorrelationIdHeader, correlationId);

        using (context.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>()
            .BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }
}
```

---

## .NET Aspire for Microservices Orchestration

```csharp
// File: {ApplicationName}.AppHost/Program.cs

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL for services
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var budgetDb = postgres.AddDatabase("budgetdb");
var goalDb = postgres.AddDatabase("goaldb");

// Add Redis for caching
var redis = builder.AddRedis("redis");

// Add RabbitMQ for messaging
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

// Add Budget Service
var budgetService = builder.AddProject<Projects.BudgetService>("budget-service")
    .WithReference(budgetDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithHttpEndpoint(port: 8081, name: "http");

// Add Goal Service
var goalService = builder.AddProject<Projects.GoalService>("goal-service")
    .WithReference(goalDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithHttpEndpoint(port: 8082, name: "http");

// Add Transaction Service
var transactionService = builder.AddProject<Projects.TransactionService>("transaction-service")
    .WithReference(postgres.AddDatabase("transactiondb"))
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithHttpEndpoint(port: 8083, name: "http");

// Add API Gateway
var gateway = builder.AddProject<Projects.Gateway>("gateway")
    .WithHttpEndpoint(port: 8080, name: "http")
    .WithReference(budgetService)
    .WithReference(goalService)
    .WithReference(transactionService);

builder.Build().Run();
```

---

## Container Orchestration

### Docker Compose

```yaml
# File: docker-compose.yml

version: '3.8'

services:
  budget-service:
    build:
      context: ./src/{ApplicationName}.Services.Budget
      dockerfile: Dockerfile
    ports:
      - "8081:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=BudgetDb;Username=postgres;Password=postgres
      - RabbitMQ__Host=rabbitmq
      - Redis__ConnectionString=redis:6379
    depends_on:
      - postgres
      - rabbitmq
      - redis

  goal-service:
    build:
      context: ./src/{ApplicationName}.Services.Goal
      dockerfile: Dockerfile
    ports:
      - "8082:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=GoalDb;Username=postgres;Password=postgres
      - RabbitMQ__Host=rabbitmq
      - Redis__ConnectionString=redis:6379
      - Services__BudgetService__Url=http://budget-service:8080
    depends_on:
      - postgres
      - rabbitmq
      - redis
      - budget-service

  transaction-service:
    build:
      context: ./src/{ApplicationName}.Services.Transaction
      dockerfile: Dockerfile
    ports:
      - "8083:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=TransactionDb;Username=postgres;Password=postgres
      - RabbitMQ__Host=rabbitmq
      - Redis__ConnectionString=redis:6379
    depends_on:
      - postgres
      - rabbitmq
      - redis

  gateway:
    build:
      context: ./src/{ApplicationName}.Gateway
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      - Services__BudgetService__Url=http://budget-service:8080
      - Services__GoalService__Url=http://goal-service:8080
      - Services__TransactionService__Url=http://transaction-service:8080
    depends_on:
      - budget-service
      - goal-service
      - transaction-service

  postgres:
    image: postgres:16
    ports:
      - "5432:5432"
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    volumes:
      - postgres-data:/var/lib/postgresql/data

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=admin
      - RABBITMQ_DEFAULT_PASS=admin

  redis:
    image: redis:7
    ports:
      - "6379:6379"

volumes:
  postgres-data:
```

### Kubernetes Deployment

```yaml
# File: k8s/budget-service-deployment.yaml

apiVersion: apps/v1
kind: Deployment
metadata:
  name: budget-service
  labels:
    app: budget-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: budget-service
  template:
    metadata:
      labels:
        app: budget-service
    spec:
      containers:
      - name: budget-service
        image: {registry}/{ApplicationName}/budget-service:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: budget-service-secrets
              key: connection-string
        - name: RabbitMQ__Host
          value: rabbitmq-service
        - name: Redis__ConnectionString
          value: redis-service:6379
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: budget-service
spec:
  selector:
    app: budget-service
  ports:
  - protocol: TCP
    port: 8080
    targetPort: 8080
  type: ClusterIP
---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: budget-service-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: budget-service
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

---

## Deployment Strategies

### Blue-Green Deployment

```yaml
# Blue deployment (current)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: budget-service-blue
spec:
  replicas: 3
  selector:
    matchLabels:
      app: budget-service
      version: blue
  template:
    metadata:
      labels:
        app: budget-service
        version: blue
    spec:
      containers:
      - name: budget-service
        image: {registry}/{ApplicationName}/budget-service:v1.0.0

---
# Green deployment (new version)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: budget-service-green
spec:
  replicas: 3
  selector:
    matchLabels:
      app: budget-service
      version: green
  template:
    metadata:
      labels:
        app: budget-service
        version: green
    spec:
      containers:
      - name: budget-service
        image: {registry}/{ApplicationName}/budget-service:v2.0.0

---
# Service pointing to blue initially
apiVersion: v1
kind: Service
metadata:
  name: budget-service
spec:
  selector:
    app: budget-service
    version: blue  # Change to 'green' to switch traffic
  ports:
  - port: 8080
```

### Canary Deployment

```yaml
# Stable version (90% of traffic)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: budget-service-stable
spec:
  replicas: 9
  selector:
    matchLabels:
      app: budget-service
      version: stable
  template:
    metadata:
      labels:
        app: budget-service
        version: stable
    spec:
      containers:
      - name: budget-service
        image: {registry}/{ApplicationName}/budget-service:v1.0.0

---
# Canary version (10% of traffic)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: budget-service-canary
spec:
  replicas: 1
  selector:
    matchLabels:
      app: budget-service
      version: canary
  template:
    metadata:
      labels:
        app: budget-service
        version: canary
    spec:
      containers:
      - name: budget-service
        image: {registry}/{ApplicationName}/budget-service:v2.0.0

---
# Service distributes traffic across both
apiVersion: v1
kind: Service
metadata:
  name: budget-service
spec:
  selector:
    app: budget-service  # Matches both stable and canary
  ports:
  - port: 8080
```

---

## Monitoring and Observability

### Health Checks

```csharp
// File: {ApplicationName}.Services.Budget/Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration["Redis:ConnectionString"]!)
    .AddRabbitMQ(rabbitConnectionString: builder.Configuration["RabbitMQ:ConnectionString"]!)
    .AddCheck<BudgetServiceHealthCheck>("budget-service-health");

var app = builder.Build();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Liveness - just check if app is running
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true // Readiness - check all dependencies
});

app.Run();
```

### Custom Health Check

```csharp
public sealed class BudgetServiceHealthCheck : IHealthCheck
{
    private readonly DataContext _dataContext;

    public BudgetServiceHealthCheck(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check database connectivity
            await _dataContext.Database.CanConnectAsync(cancellationToken);

            return HealthCheckResult.Healthy("Budget service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Budget service is unhealthy",
                exception: ex);
        }
    }
}
```

### Application Insights / Sentry

```csharp
// File: {ApplicationName}.Services.Budget/Program.cs

using I.Synergy.Framework.OpenTelemetry.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry with Application Insights
builder.Services.AddOpenTelemetryWithApplicationInsights(
    builder.Configuration["ApplicationInsights:ConnectionString"]!,
    "budget-service");

// Or with Sentry
builder.Services.AddSentry(options =>
{
    options.Dsn = builder.Configuration["Sentry:Dsn"];
    options.Environment = builder.Environment.EnvironmentName;
    options.TracesSampleRate = 1.0;
});

var app = builder.Build();
app.Run();
```

---

## Common Pitfalls

### ❌ WRONG - Shared Database

```csharp
// ❌ Multiple services accessing same database
public sealed class SharedDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}

// BudgetService, GoalService, TransactionService all use SharedDbContext
// This creates tight coupling and prevents independent deployment!
```

### ✅ CORRECT - Database Per Service

```csharp
// ✅ Each service has its own database
public sealed class BudgetDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
}

public sealed class GoalDbContext : DbContext
{
    public DbSet<Goal> Goals { get; set; }
}

public sealed class TransactionDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
}
```

### ❌ WRONG - Synchronous Cascading Calls

```csharp
// ❌ Service A → Service B → Service C (synchronous chain)
// This creates latency and tight coupling

public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
{
    // Call Service B
    var inventory = await _inventoryClient.ReserveInventoryAsync(request.Items);

    // Call Service C
    var payment = await _paymentClient.ProcessPaymentAsync(request.Amount);

    // If any service is slow or down, entire operation fails
    return new OrderResponse { /* ... */ };
}
```

### ✅ CORRECT - Event-Driven Approach

```csharp
// ✅ Service A publishes event, Services B and C react independently
public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderCommand command)
{
    // Create order
    var order = command.Adapt<Order>();
    await _dbContext.AddItemAsync<Order, OrderModel>(order.Adapt<OrderModel>());

    // Publish event
    await _publishEndpoint.Publish(new OrderCreatedEvent
    {
        OrderId = order.OrderId,
        Items = order.Items,
        TotalAmount = order.TotalAmount
    });

    // Return immediately - downstream services process asynchronously
    return new CreateOrderResponse(order.OrderId);
}
```

### ❌ WRONG - No Circuit Breaker

```csharp
// ❌ No resilience - if service is down, keeps trying indefinitely
public async Task<BudgetDto?> GetBudgetAsync(Guid budgetId)
{
    var response = await _httpClient.GetAsync($"/api/budgets/{budgetId}");
    return await response.Content.ReadFromJsonAsync<BudgetDto>();
}
```

### ✅ CORRECT - Circuit Breaker Pattern

```csharp
// ✅ With circuit breaker and retry
builder.Services.AddHttpClient<BudgetServiceClient>()
    .AddStandardResilienceHandler(options =>
    {
        options.CircuitBreaker.FailureRatio = 0.5;
        options.Retry.MaxRetryAttempts = 3;
    });
```

---

## Best Practices Summary

### DO:
- ✅ Design services around business capabilities
- ✅ Use database per service pattern
- ✅ Implement circuit breaker and retry patterns
- ✅ Use asynchronous messaging for inter-service communication
- ✅ Implement distributed tracing with correlation IDs
- ✅ Use API Gateway for client-facing APIs
- ✅ Implement health checks (liveness and readiness)
- ✅ Use saga pattern for distributed transactions
- ✅ Deploy services independently
- ✅ Monitor and log extensively
- ✅ Use container orchestration (Kubernetes, .NET Aspire)
- ✅ Implement service discovery
- ✅ Version your APIs
- ✅ Use event-driven architecture where appropriate

### DON'T:
- ❌ Share databases across services
- ❌ Create synchronous cascading calls
- ❌ Skip circuit breakers and retries
- ❌ Ignore distributed tracing
- ❌ Deploy all services together (monolithic deployment)
- ❌ Use distributed transactions (2PC) - use sagas instead
- ❌ Ignore backward compatibility
- ❌ Skip health checks
- ❌ Hard-code service URLs
- ❌ Forget to handle partial failures

---

## Quality Checklist

- [ ] Services aligned with business capabilities (DDD)
- [ ] Each service has its own database
- [ ] API Gateway implemented for client access
- [ ] Inter-service communication uses HTTP/gRPC/messaging
- [ ] Circuit breaker implemented for external calls
- [ ] Retry policies configured
- [ ] Distributed tracing with correlation IDs
- [ ] Health checks (liveness and readiness) implemented
- [ ] Services independently deployable
- [ ] Saga pattern for distributed transactions
- [ ] Event-driven architecture for asynchronous operations
- [ ] Service discovery configured
- [ ] Container orchestration set up (Docker Compose/Kubernetes/Aspire)
- [ ] Monitoring and logging in place
- [ ] API versioning strategy defined
- [ ] Deployment strategy chosen (blue-green, canary, rolling)
- [ ] Authentication and authorization implemented
- [ ] Rate limiting and throttling configured
- [ ] Documentation for APIs (OpenAPI/Swagger)

---

**End of Microservices Architecture Pattern**

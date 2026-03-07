# Object-Oriented Programming (OOP) Pattern

## Overview

Object-Oriented Programming is a programming paradigm based on the concept of "objects" which contain data (fields/properties) and code (methods). This pattern emphasizes encapsulation, inheritance, polymorphism, and abstraction to create maintainable, reusable, and scalable software.

---

## SOLID Principles

### Single Responsibility Principle (SRP)

**Definition:** A class should have only one reason to change - it should have only one job or responsibility.

```csharp
// ✅ CORRECT - Single responsibility
public sealed class BudgetRepository
{
    private readonly DataContext _dataContext;

    public BudgetRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Budget> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id, ct);
    }
}

public sealed class BudgetValidator
{
    public ValidationResult Validate(Budget budget)
    {
        // Only validation logic
        if (budget.Amount <= 0)
            return ValidationResult.Fail("Amount must be positive");

        return ValidationResult.Success();
    }
}

// ❌ WRONG - Multiple responsibilities
public sealed class BudgetManager
{
    // Data access
    public async Task<Budget> GetByIdAsync(Guid id) { /* ... */ }

    // Validation
    public bool IsValid(Budget budget) { /* ... */ }

    // Email sending
    public async Task SendNotificationAsync(Budget budget) { /* ... */ }

    // Logging
    public void LogActivity(string message) { /* ... */ }
}
```

### Open/Closed Principle (OCP)

**Definition:** Software entities should be open for extension but closed for modification.

```csharp
// ✅ CORRECT - Open for extension via abstraction
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct);
}

public sealed class CreditCardPaymentProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct)
    {
        // Credit card specific logic
        return new PaymentResult(true);
    }
}

public sealed class PayPalPaymentProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct)
    {
        // PayPal specific logic
        return new PaymentResult(true);
    }
}

public sealed class PaymentService
{
    private readonly IPaymentProcessor _processor;

    public PaymentService(IPaymentProcessor processor)
    {
        _processor = processor;
    }

    public async Task ProcessPaymentAsync(decimal amount, CancellationToken ct)
    {
        await _processor.ProcessAsync(amount, ct);
    }
}

// ❌ WRONG - Modification required for new payment types
public sealed class PaymentService
{
    public async Task ProcessPaymentAsync(string type, decimal amount)
    {
        if (type == "CreditCard")
        {
            // Credit card logic
        }
        else if (type == "PayPal")
        {
            // PayPal logic
        }
        // Adding Bitcoin requires modifying this class!
    }
}
```

### Liskov Substitution Principle (LSP)

**Definition:** Objects of a superclass should be replaceable with objects of a subclass without breaking the application.

```csharp
// ✅ CORRECT - Derived types maintain base contract
public abstract class Document
{
    public string Title { get; init; } = string.Empty;

    public virtual async Task<byte[]> GenerateAsync(CancellationToken ct)
    {
        // Base implementation
        return Array.Empty<byte>();
    }
}

public sealed class PdfDocument : Document
{
    public override async Task<byte[]> GenerateAsync(CancellationToken ct)
    {
        // PDF-specific generation that honors the contract
        var pdfBytes = await GeneratePdfBytesAsync(ct);
        return pdfBytes; // Returns byte[] as expected
    }

    private async Task<byte[]> GeneratePdfBytesAsync(CancellationToken ct)
    {
        await Task.CompletedTask;
        return new byte[] { /* PDF data */ };
    }
}

public sealed class WordDocument : Document
{
    public override async Task<byte[]> GenerateAsync(CancellationToken ct)
    {
        // Word-specific generation that honors the contract
        var wordBytes = await GenerateWordBytesAsync(ct);
        return wordBytes;
    }

    private async Task<byte[]> GenerateWordBytesAsync(CancellationToken ct)
    {
        await Task.CompletedTask;
        return new byte[] { /* Word data */ };
    }
}

// ❌ WRONG - Violates LSP by changing behavior
public sealed class ReadOnlyDocument : Document
{
    public override async Task<byte[]> GenerateAsync(CancellationToken ct)
    {
        // Violates contract by throwing exception
        throw new NotSupportedException("Cannot generate read-only documents");
    }
}
```

### Interface Segregation Principle (ISP)

**Definition:** Clients should not be forced to depend on interfaces they don't use.

```csharp
// ✅ CORRECT - Segregated interfaces
public interface IReadable<T>
{
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<T>> GetAllAsync(CancellationToken ct);
}

public interface IWritable<T>
{
    Task<Guid> CreateAsync(T item, CancellationToken ct);
    Task UpdateAsync(T item, CancellationToken ct);
}

public interface IDeletable
{
    Task DeleteAsync(Guid id, CancellationToken ct);
}

// Read-only repository only implements what it needs
public sealed class ReadOnlyBudgetRepository : IReadable<Budget>
{
    private readonly DataContext _dataContext;

    public ReadOnlyBudgetRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Budget> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id, ct);
    }

    public async Task<List<Budget>> GetAllAsync(CancellationToken ct)
    {
        return await _dataContext.Set<Budget>().ToListAsync(ct);
    }
}

// Full repository implements all interfaces
public sealed class BudgetRepository : IReadable<Budget>, IWritable<Budget>, IDeletable
{
    private readonly DataContext _dataContext;

    public BudgetRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Budget> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(id, ct);
    }

    public async Task<List<Budget>> GetAllAsync(CancellationToken ct)
    {
        return await _dataContext.Set<Budget>().ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(Budget item, CancellationToken ct)
    {
        await _dataContext.AddItemAsync<Budget, BudgetModel>(item.Adapt<BudgetModel>(), ct);
        return item.BudgetId;
    }

    public async Task UpdateAsync(Budget item, CancellationToken ct)
    {
        await _dataContext.UpdateItemAsync<Budget, BudgetModel>(item.Adapt<BudgetModel>(), ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await _dataContext.RemoveItemAsync<Budget, Guid>(id, ct);
    }
}

// ❌ WRONG - Fat interface forces unnecessary implementation
public interface IRepository<T>
{
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<T>> GetAllAsync(CancellationToken ct);
    Task<Guid> CreateAsync(T item, CancellationToken ct);
    Task UpdateAsync(T item, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task BulkInsertAsync(List<T> items, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
}

// Read-only repository forced to implement write operations
public sealed class ReadOnlyRepository : IRepository<Budget>
{
    public async Task<Guid> CreateAsync(Budget item, CancellationToken ct)
    {
        throw new NotSupportedException(); // Forced to implement!
    }

    public async Task UpdateAsync(Budget item, CancellationToken ct)
    {
        throw new NotSupportedException(); // Forced to implement!
    }
}
```

### Dependency Inversion Principle (DIP)

**Definition:** High-level modules should not depend on low-level modules. Both should depend on abstractions.

```csharp
// ✅ CORRECT - Depend on abstractions
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct);
}

public sealed class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct)
    {
        // SMTP implementation
        await Task.CompletedTask;
    }
}

public sealed class BudgetCreatedHandler : IEventHandler<BudgetCreatedEvent>
{
    private readonly IEmailService _emailService; // Depends on abstraction

    public BudgetCreatedHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(BudgetCreatedEvent domainEvent, CancellationToken ct)
    {
        await _emailService.SendAsync(
            "user@example.com",
            "Budget Created",
            $"Budget {domainEvent.Name} was created",
            ct);
    }
}

// ❌ WRONG - Depends on concrete implementation
public sealed class BudgetCreatedHandler
{
    private readonly SmtpEmailService _emailService; // Concrete dependency!

    public BudgetCreatedHandler()
    {
        _emailService = new SmtpEmailService(); // Creates dependency directly
    }

    public async Task HandleAsync(BudgetCreatedEvent domainEvent)
    {
        await _emailService.SendAsync(
            "user@example.com",
            "Budget Created",
            $"Budget {domainEvent.Name} was created");
    }
}
```

---

## Core OOP Concepts

### Encapsulation

**Definition:** Bundling data and methods that operate on that data within a single unit (class), and restricting access to internal state.

```csharp
// ✅ CORRECT - Properly encapsulated
public sealed class Budget
{
    private decimal _amount;
    private readonly List<Goal> _goals = new();

    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;

    public decimal Amount
    {
        get => _amount;
        init
        {
            if (value <= 0)
                throw new ArgumentException("Amount must be positive", nameof(Amount));
            _amount = value;
        }
    }

    public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();

    public void AddGoal(Goal goal)
    {
        ArgumentNullException.ThrowIfNull(goal);

        if (_goals.Any(g => g.GoalId == goal.GoalId))
            throw new InvalidOperationException("Goal already exists");

        _goals.Add(goal);
    }

    public void RemoveGoal(Guid goalId)
    {
        var goal = _goals.FirstOrDefault(g => g.GoalId == goalId);
        if (goal is null)
            throw new InvalidOperationException("Goal not found");

        _goals.Remove(goal);
    }
}

// ❌ WRONG - Poor encapsulation
public sealed class Budget
{
    public Guid BudgetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; } // No validation!
    public List<Goal> Goals { get; set; } = new(); // Direct access to mutable collection!
}
```

### Inheritance

**Definition:** Mechanism where a new class derives properties and behavior from an existing class.

```csharp
// ✅ CORRECT - Inheritance with proper abstraction
public abstract class Entity
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public DateTimeOffset? ChangedDate { get; set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTimeOffset.UtcNow;
    }
}

public abstract class AuditableEntity : Entity
{
    public string CreatedBy { get; init; } = string.Empty;
    public string? ChangedBy { get; set; }
}

public sealed class Budget : AuditableEntity
{
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }

    // Inherits: Id, CreatedDate, ChangedDate, CreatedBy, ChangedBy
}

// ✅ CORRECT - Composition over inheritance (often preferred)
public sealed class Budget
{
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public AuditInfo AuditInfo { get; init; } = new(); // Composition
}

public sealed class AuditInfo
{
    public DateTimeOffset CreatedDate { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTimeOffset? ChangedDate { get; set; }
    public string? ChangedBy { get; set; }
}

// ❌ WRONG - Deep inheritance hierarchy (fragile)
public class BaseEntity { }
public class AuditableEntity : BaseEntity { }
public class VersionedEntity : AuditableEntity { }
public class SoftDeletableEntity : VersionedEntity { }
public class Budget : SoftDeletableEntity { } // Too many levels!
```

### Polymorphism

**Definition:** Ability of objects to take on multiple forms, allowing methods to be used interchangeably.

```csharp
// ✅ CORRECT - Polymorphism via interfaces
public interface IDocumentGenerator
{
    Task<byte[]> GenerateAsync(Budget budget, CancellationToken ct);
}

public sealed class PdfDocumentGenerator : IDocumentGenerator
{
    public async Task<byte[]> GenerateAsync(Budget budget, CancellationToken ct)
    {
        // PDF-specific generation
        await Task.CompletedTask;
        return new byte[] { /* PDF data */ };
    }
}

public sealed class ExcelDocumentGenerator : IDocumentGenerator
{
    public async Task<byte[]> GenerateAsync(Budget budget, CancellationToken ct)
    {
        // Excel-specific generation
        await Task.CompletedTask;
        return new byte[] { /* Excel data */ };
    }
}

public sealed class DocumentService
{
    public async Task<byte[]> GenerateDocumentAsync(
        Budget budget,
        IDocumentGenerator generator,
        CancellationToken ct)
    {
        // Polymorphic call - works with any IDocumentGenerator
        return await generator.GenerateAsync(budget, ct);
    }
}

// ✅ CORRECT - Polymorphism via abstract base class
public abstract class NotificationSender
{
    public abstract Task SendAsync(string message, CancellationToken ct);

    // Template method pattern
    public async Task SendWithRetryAsync(string message, CancellationToken ct)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await SendAsync(message, ct);
                return;
            }
            catch when (i < 2)
            {
                await Task.Delay(1000 * (i + 1), ct);
            }
        }
    }
}

public sealed class EmailNotificationSender : NotificationSender
{
    public override async Task SendAsync(string message, CancellationToken ct)
    {
        // Email-specific implementation
        await Task.CompletedTask;
    }
}

public sealed class SmsNotificationSender : NotificationSender
{
    public override async Task SendAsync(string message, CancellationToken ct)
    {
        // SMS-specific implementation
        await Task.CompletedTask;
    }
}
```

### Abstraction

**Definition:** Hiding complex implementation details and exposing only necessary features.

```csharp
// ✅ CORRECT - Clean abstraction
public interface IBudgetCalculator
{
    decimal CalculateRemaining(Budget budget);
    decimal CalculateAllocated(Budget budget);
    bool IsOverBudget(Budget budget);
}

public sealed class BudgetCalculator : IBudgetCalculator
{
    public decimal CalculateRemaining(Budget budget)
    {
        ArgumentNullException.ThrowIfNull(budget);

        var allocated = CalculateAllocated(budget);
        return budget.Amount - allocated;
    }

    public decimal CalculateAllocated(Budget budget)
    {
        ArgumentNullException.ThrowIfNull(budget);

        return budget.Goals.Sum(g => g.TargetAmount);
    }

    public bool IsOverBudget(Budget budget)
    {
        ArgumentNullException.ThrowIfNull(budget);

        return CalculateRemaining(budget) < 0;
    }
}

// Consumer doesn't need to know implementation details
public sealed class BudgetService
{
    private readonly IBudgetCalculator _calculator;

    public BudgetService(IBudgetCalculator calculator)
    {
        _calculator = calculator;
    }

    public async Task<BudgetSummary> GetSummaryAsync(Budget budget, CancellationToken ct)
    {
        return new BudgetSummary(
            budget.BudgetId,
            budget.Name,
            _calculator.CalculateRemaining(budget),
            _calculator.IsOverBudget(budget));
    }
}

// ❌ WRONG - Leaky abstraction
public interface IBudgetCalculator
{
    // Exposes internal implementation details
    Dictionary<string, decimal> GetInternalCalculationCache();
    void SetCacheExpiration(TimeSpan expiration);
    List<string> GetDebugLogs();
}
```

---

## Design Patterns (Gang of Four)

### Creational Patterns

#### Factory Pattern

**Purpose:** Create objects without specifying exact class.

```csharp
// ✅ CORRECT - Factory pattern
public interface IPaymentProcessorFactory
{
    IPaymentProcessor Create(PaymentMethod method);
}

public sealed class PaymentProcessorFactory : IPaymentProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentProcessor Create(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.CreditCard => _serviceProvider.GetRequiredService<CreditCardPaymentProcessor>(),
            PaymentMethod.PayPal => _serviceProvider.GetRequiredService<PayPalPaymentProcessor>(),
            PaymentMethod.Bitcoin => _serviceProvider.GetRequiredService<BitcoinPaymentProcessor>(),
            _ => throw new ArgumentException($"Unknown payment method: {method}", nameof(method))
        };
    }
}

public enum PaymentMethod
{
    CreditCard,
    PayPal,
    Bitcoin
}
```

#### Builder Pattern

**Purpose:** Construct complex objects step by step.

```csharp
// ✅ CORRECT - Builder pattern
public sealed class BudgetBuilder
{
    private string _name = string.Empty;
    private decimal _amount;
    private DateTimeOffset _startDate = DateTimeOffset.UtcNow;
    private readonly List<Goal> _goals = new();

    public BudgetBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public BudgetBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public BudgetBuilder WithStartDate(DateTimeOffset startDate)
    {
        _startDate = startDate;
        return this;
    }

    public BudgetBuilder AddGoal(Goal goal)
    {
        _goals.Add(goal);
        return this;
    }

    public Budget Build()
    {
        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = _name,
            Amount = _amount,
            StartDate = _startDate,
            CreatedDate = DateTimeOffset.UtcNow
        };

        foreach (var goal in _goals)
        {
            budget.AddGoal(goal);
        }

        return budget;
    }
}

// Usage
var budget = new BudgetBuilder()
    .WithName("2025 Q1")
    .WithAmount(50000m)
    .WithStartDate(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
    .AddGoal(new Goal { Name = "Savings", TargetAmount = 10000m })
    .Build();
```

#### Singleton Pattern

**Purpose:** Ensure only one instance exists.

```csharp
// ✅ CORRECT - Singleton via DI (preferred in .NET)
public interface IApplicationCache
{
    void Set(string key, object value);
    object? Get(string key);
}

public sealed class ApplicationCache : IApplicationCache
{
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public void Set(string key, object value)
    {
        _cache[key] = value;
    }

    public object? Get(string key)
    {
        _cache.TryGetValue(key, out var value);
        return value;
    }
}

// Register as singleton in DI
services.AddSingleton<IApplicationCache, ApplicationCache>();

// ❌ WRONG - Classic singleton (anti-pattern in modern .NET)
public sealed class ApplicationCache
{
    private static readonly Lazy<ApplicationCache> _instance =
        new(() => new ApplicationCache());

    public static ApplicationCache Instance => _instance.Value;

    private ApplicationCache() { } // Private constructor
}
```

#### Prototype Pattern

**Purpose:** Create new objects by copying existing ones.

```csharp
// ✅ CORRECT - Prototype pattern
public interface ICloneable<T>
{
    T Clone();
}

public sealed class BudgetTemplate : ICloneable<BudgetTemplate>
{
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public List<GoalTemplate> Goals { get; init; } = new();

    public BudgetTemplate Clone()
    {
        return new BudgetTemplate
        {
            Name = Name,
            Amount = Amount,
            Goals = Goals.Select(g => g.Clone()).ToList()
        };
    }

    public Budget CreateBudget()
    {
        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = Name,
            Amount = Amount,
            CreatedDate = DateTimeOffset.UtcNow
        };

        foreach (var goalTemplate in Goals)
        {
            budget.AddGoal(goalTemplate.CreateGoal());
        }

        return budget;
    }
}

public sealed class GoalTemplate : ICloneable<GoalTemplate>
{
    public string Name { get; init; } = string.Empty;
    public decimal TargetAmount { get; init; }

    public GoalTemplate Clone()
    {
        return new GoalTemplate
        {
            Name = Name,
            TargetAmount = TargetAmount
        };
    }

    public Goal CreateGoal()
    {
        return new Goal
        {
            GoalId = Guid.NewGuid(),
            Name = Name,
            TargetAmount = TargetAmount,
            CreatedDate = DateTimeOffset.UtcNow
        };
    }
}
```

### Structural Patterns

#### Adapter Pattern

**Purpose:** Allow incompatible interfaces to work together.

```csharp
// ✅ CORRECT - Adapter pattern
// Legacy system interface
public interface ILegacyBudgetSystem
{
    string GetBudgetData(int budgetId);
}

// New system interface
public interface IBudgetService
{
    Task<Budget> GetBudgetAsync(Guid budgetId, CancellationToken ct);
}

// Adapter
public sealed class LegacyBudgetAdapter : IBudgetService
{
    private readonly ILegacyBudgetSystem _legacySystem;

    public LegacyBudgetAdapter(ILegacyBudgetSystem legacySystem)
    {
        _legacySystem = legacySystem;
    }

    public async Task<Budget> GetBudgetAsync(Guid budgetId, CancellationToken ct)
    {
        // Convert Guid to int for legacy system
        var legacyId = Math.Abs(budgetId.GetHashCode());

        // Call legacy system
        var legacyData = _legacySystem.GetBudgetData(legacyId);

        // Parse and adapt to new format
        var parts = legacyData.Split('|');

        return await Task.FromResult(new Budget
        {
            BudgetId = budgetId,
            Name = parts[0],
            Amount = decimal.Parse(parts[1]),
            CreatedDate = DateTimeOffset.Parse(parts[2])
        });
    }
}
```

#### Decorator Pattern

**Purpose:** Add behavior to objects dynamically.

```csharp
// ✅ CORRECT - Decorator pattern
public interface IBudgetCalculator
{
    decimal CalculateRemaining(Budget budget);
}

// Base implementation
public sealed class SimpleBudgetCalculator : IBudgetCalculator
{
    public decimal CalculateRemaining(Budget budget)
    {
        var allocated = budget.Goals.Sum(g => g.TargetAmount);
        return budget.Amount - allocated;
    }
}

// Decorator: Add logging
public sealed class LoggingBudgetCalculatorDecorator : IBudgetCalculator
{
    private readonly IBudgetCalculator _inner;
    private readonly ILogger<LoggingBudgetCalculatorDecorator> _logger;

    public LoggingBudgetCalculatorDecorator(
        IBudgetCalculator inner,
        ILogger<LoggingBudgetCalculatorDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public decimal CalculateRemaining(Budget budget)
    {
        _logger.LogDebug("Calculating remaining for budget {BudgetId}", budget.BudgetId);

        var result = _inner.CalculateRemaining(budget);

        _logger.LogDebug(
            "Budget {BudgetId} has {Remaining} remaining",
            budget.BudgetId, result);

        return result;
    }
}

// Decorator: Add caching
public sealed class CachingBudgetCalculatorDecorator : IBudgetCalculator
{
    private readonly IBudgetCalculator _inner;
    private readonly IDistributedCache _cache;

    public CachingBudgetCalculatorDecorator(
        IBudgetCalculator inner,
        IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public decimal CalculateRemaining(Budget budget)
    {
        var cacheKey = $"budget:remaining:{budget.BudgetId}";
        var cached = _cache.GetString(cacheKey);

        if (cached is not null)
            return decimal.Parse(cached);

        var result = _inner.CalculateRemaining(budget);

        _cache.SetString(cacheKey, result.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return result;
    }
}

// Registration with multiple decorators
services.AddScoped<SimpleBudgetCalculator>();
services.AddScoped<IBudgetCalculator>(sp =>
{
    var simple = sp.GetRequiredService<SimpleBudgetCalculator>();
    var logger = sp.GetRequiredService<ILogger<LoggingBudgetCalculatorDecorator>>();
    var cache = sp.GetRequiredService<IDistributedCache>();

    // Wrap with logging, then caching
    var withLogging = new LoggingBudgetCalculatorDecorator(simple, logger);
    return new CachingBudgetCalculatorDecorator(withLogging, cache);
});
```

#### Facade Pattern

**Purpose:** Provide simplified interface to complex subsystem.

```csharp
// ✅ CORRECT - Facade pattern
public interface IBudgetManagementFacade
{
    Task<BudgetSummary> CreateBudgetWithGoalsAsync(
        string budgetName,
        decimal budgetAmount,
        List<GoalData> goals,
        CancellationToken ct);
}

public sealed class BudgetManagementFacade : IBudgetManagementFacade
{
    private readonly ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> _createBudgetHandler;
    private readonly ICommandHandler<CreateGoalCommand, CreateGoalResponse> _createGoalHandler;
    private readonly IQueryHandler<GetBudgetByIdQuery, BudgetResponse> _getBudgetHandler;
    private readonly IBudgetCalculator _calculator;
    private readonly ILogger<BudgetManagementFacade> _logger;

    public BudgetManagementFacade(
        ICommandHandler<CreateBudgetCommand, CreateBudgetResponse> createBudgetHandler,
        ICommandHandler<CreateGoalCommand, CreateGoalResponse> createGoalHandler,
        IQueryHandler<GetBudgetByIdQuery, BudgetResponse> getBudgetHandler,
        IBudgetCalculator calculator,
        ILogger<BudgetManagementFacade> logger)
    {
        _createBudgetHandler = createBudgetHandler;
        _createGoalHandler = createGoalHandler;
        _getBudgetHandler = getBudgetHandler;
        _calculator = calculator;
        _logger = logger;
    }

    public async Task<BudgetSummary> CreateBudgetWithGoalsAsync(
        string budgetName,
        decimal budgetAmount,
        List<GoalData> goals,
        CancellationToken ct)
    {
        _logger.LogInformation("Creating budget {BudgetName} with {GoalCount} goals",
            budgetName, goals.Count);

        // Step 1: Create budget
        var createBudgetCommand = new CreateBudgetCommand(
            budgetName,
            budgetAmount,
            DateTimeOffset.UtcNow);

        var budgetResponse = await _createBudgetHandler.HandleAsync(createBudgetCommand, ct);

        // Step 2: Create all goals
        foreach (var goalData in goals)
        {
            var createGoalCommand = new CreateGoalCommand(
                budgetResponse.BudgetId,
                goalData.Name,
                goalData.TargetAmount,
                goalData.TargetDate);

            await _createGoalHandler.HandleAsync(createGoalCommand, ct);
        }

        // Step 3: Get budget details
        var getBudgetQuery = new GetBudgetByIdQuery(budgetResponse.BudgetId);
        var budget = await _getBudgetHandler.HandleAsync(getBudgetQuery, ct);

        // Step 4: Calculate summary
        return new BudgetSummary(
            budget.BudgetId,
            budget.Name,
            budget.Amount,
            goals.Sum(g => g.TargetAmount),
            budget.Amount - goals.Sum(g => g.TargetAmount));
    }
}

public record GoalData(string Name, decimal TargetAmount, DateTimeOffset TargetDate);
public record BudgetSummary(Guid BudgetId, string Name, decimal Total, decimal Allocated, decimal Remaining);
```

#### Proxy Pattern

**Purpose:** Provide substitute or placeholder for another object.

```csharp
// ✅ CORRECT - Proxy pattern (caching proxy)
public interface IBudgetService
{
    Task<Budget> GetBudgetAsync(Guid budgetId, CancellationToken ct);
}

public sealed class BudgetService : IBudgetService
{
    private readonly DataContext _dataContext;

    public BudgetService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Budget> GetBudgetAsync(Guid budgetId, CancellationToken ct)
    {
        return await _dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(budgetId, ct);
    }
}

public sealed class CachingBudgetServiceProxy : IBudgetService
{
    private readonly IBudgetService _realService;
    private readonly IDistributedCache _cache;

    public CachingBudgetServiceProxy(
        IBudgetService realService,
        IDistributedCache cache)
    {
        _realService = realService;
        _cache = cache;
    }

    public async Task<Budget> GetBudgetAsync(Guid budgetId, CancellationToken ct)
    {
        var cacheKey = $"budget:{budgetId}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);

        if (cached is not null)
        {
            return JsonSerializer.Deserialize<Budget>(cached)!;
        }

        var budget = await _realService.GetBudgetAsync(budgetId, ct);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(budget),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            ct);

        return budget;
    }
}
```

### Behavioral Patterns

#### Strategy Pattern

**Purpose:** Define family of algorithms, encapsulate each one, make them interchangeable.

```csharp
// ✅ CORRECT - Strategy pattern
public interface IBudgetAllocationStrategy
{
    List<GoalAllocation> Allocate(decimal totalAmount, List<Goal> goals);
}

public sealed class EqualAllocationStrategy : IBudgetAllocationStrategy
{
    public List<GoalAllocation> Allocate(decimal totalAmount, List<Goal> goals)
    {
        var amountPerGoal = totalAmount / goals.Count;

        return goals.Select(g => new GoalAllocation(
            g.GoalId,
            g.Name,
            amountPerGoal)).ToList();
    }
}

public sealed class ProportionalAllocationStrategy : IBudgetAllocationStrategy
{
    public List<GoalAllocation> Allocate(decimal totalAmount, List<Goal> goals)
    {
        var totalTarget = goals.Sum(g => g.TargetAmount);

        return goals.Select(g => new GoalAllocation(
            g.GoalId,
            g.Name,
            totalAmount * (g.TargetAmount / totalTarget))).ToList();
    }
}

public sealed class PriorityAllocationStrategy : IBudgetAllocationStrategy
{
    public List<GoalAllocation> Allocate(decimal totalAmount, List<Goal> goals)
    {
        var allocations = new List<GoalAllocation>();
        var remaining = totalAmount;

        foreach (var goal in goals.OrderByDescending(g => g.Priority))
        {
            var allocation = Math.Min(remaining, goal.TargetAmount);
            allocations.Add(new GoalAllocation(goal.GoalId, goal.Name, allocation));
            remaining -= allocation;

            if (remaining <= 0)
                break;
        }

        return allocations;
    }
}

public sealed class BudgetAllocator
{
    private readonly IBudgetAllocationStrategy _strategy;

    public BudgetAllocator(IBudgetAllocationStrategy strategy)
    {
        _strategy = strategy;
    }

    public List<GoalAllocation> AllocateBudget(decimal totalAmount, List<Goal> goals)
    {
        return _strategy.Allocate(totalAmount, goals);
    }
}

public record GoalAllocation(Guid GoalId, string Name, decimal Amount);
```

#### Observer Pattern

**Purpose:** Define one-to-many dependency so when one object changes, dependents are notified.

```csharp
// ✅ CORRECT - Observer pattern (domain events)
public interface IBudgetObserver
{
    Task OnBudgetChangedAsync(Budget budget, CancellationToken ct);
}

public sealed class BudgetAlertObserver : IBudgetObserver
{
    private readonly ILogger<BudgetAlertObserver> _logger;

    public BudgetAlertObserver(ILogger<BudgetAlertObserver> logger)
    {
        _logger = logger;
    }

    public async Task OnBudgetChangedAsync(Budget budget, CancellationToken ct)
    {
        var allocated = budget.Goals.Sum(g => g.TargetAmount);
        var remaining = budget.Amount - allocated;

        if (remaining < 0)
        {
            _logger.LogWarning(
                "Budget {BudgetId} is over budget by {Amount}",
                budget.BudgetId, Math.Abs(remaining));
        }

        await Task.CompletedTask;
    }
}

public sealed class BudgetCacheInvalidationObserver : IBudgetObserver
{
    private readonly IDistributedCache _cache;

    public BudgetCacheInvalidationObserver(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task OnBudgetChangedAsync(Budget budget, CancellationToken ct)
    {
        var cacheKey = $"budget:{budget.BudgetId}";
        await _cache.RemoveAsync(cacheKey, ct);
    }
}

public sealed class BudgetSubject
{
    private readonly List<IBudgetObserver> _observers = new();

    public void Attach(IBudgetObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IBudgetObserver observer)
    {
        _observers.Remove(observer);
    }

    public async Task NotifyAsync(Budget budget, CancellationToken ct)
    {
        foreach (var observer in _observers)
        {
            await observer.OnBudgetChangedAsync(budget, ct);
        }
    }
}
```

#### Command Pattern

**Purpose:** Encapsulate request as an object.

```csharp
// ✅ CORRECT - Command pattern (CQRS implementation)
public interface ICommand<TResponse>
{
}

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}

public sealed record CreateBudgetCommand(
    string Name,
    decimal Amount,
    DateTimeOffset StartDate
) : ICommand<CreateBudgetResponse>;

public sealed record CreateBudgetResponse(Guid BudgetId);

public sealed class CreateBudgetHandler : ICommandHandler<CreateBudgetCommand, CreateBudgetResponse>
{
    private readonly DataContext _dataContext;
    private readonly ILogger<CreateBudgetHandler> _logger;

    public CreateBudgetHandler(DataContext dataContext, ILogger<CreateBudgetHandler> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<CreateBudgetResponse> HandleAsync(
        CreateBudgetCommand command,
        CancellationToken ct)
    {
        _logger.LogInformation("Creating budget {Name}", command.Name);

        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            Name = command.Name,
            Amount = command.Amount,
            StartDate = command.StartDate,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _dataContext.AddItemAsync<Budget, BudgetModel>(
            budget.Adapt<BudgetModel>(),
            ct);

        return new CreateBudgetResponse(budget.BudgetId);
    }
}
```

#### Template Method Pattern

**Purpose:** Define skeleton of algorithm, let subclasses override specific steps.

```csharp
// ✅ CORRECT - Template method pattern
public abstract class ReportGenerator
{
    public async Task<byte[]> GenerateReportAsync(Guid budgetId, CancellationToken ct)
    {
        // Template method defining the algorithm
        var data = await FetchDataAsync(budgetId, ct);
        var processedData = ProcessData(data);
        ValidateData(processedData);
        var formatted = FormatData(processedData);
        return await RenderAsync(formatted, ct);
    }

    protected abstract Task<BudgetData> FetchDataAsync(Guid budgetId, CancellationToken ct);
    protected abstract ProcessedBudgetData ProcessData(BudgetData data);
    protected abstract string FormatData(ProcessedBudgetData data);
    protected abstract Task<byte[]> RenderAsync(string formatted, CancellationToken ct);

    protected virtual void ValidateData(ProcessedBudgetData data)
    {
        // Default validation (can be overridden)
        if (data.TotalAmount <= 0)
            throw new InvalidOperationException("Invalid budget data");
    }
}

public sealed class PdfReportGenerator : ReportGenerator
{
    private readonly DataContext _dataContext;

    public PdfReportGenerator(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    protected override async Task<BudgetData> FetchDataAsync(Guid budgetId, CancellationToken ct)
    {
        var budget = await _dataContext.GetItemByIdAsync<Budget, BudgetModel, Guid>(budgetId, ct);
        return new BudgetData(budget.Name, budget.Amount);
    }

    protected override ProcessedBudgetData ProcessData(BudgetData data)
    {
        return new ProcessedBudgetData(data.Name, data.Amount, DateTimeOffset.UtcNow);
    }

    protected override string FormatData(ProcessedBudgetData data)
    {
        return $"Budget Report: {data.Name}\nAmount: {data.TotalAmount:C}\nGenerated: {data.GeneratedAt}";
    }

    protected override async Task<byte[]> RenderAsync(string formatted, CancellationToken ct)
    {
        // PDF rendering logic
        await Task.CompletedTask;
        return Encoding.UTF8.GetBytes(formatted); // Simplified
    }
}

public record BudgetData(string Name, decimal Amount);
public record ProcessedBudgetData(string Name, decimal TotalAmount, DateTimeOffset GeneratedAt);
```

---

## C# 14 OOP Features

### Records

```csharp
// ✅ CORRECT - Using records for immutable data
public sealed record BudgetSummary(
    Guid BudgetId,
    string Name,
    decimal TotalAmount,
    decimal AllocatedAmount)
{
    public decimal RemainingAmount => TotalAmount - AllocatedAmount;
    public bool IsOverBudget => RemainingAmount < 0;
}

// With expressions for non-destructive mutation
var original = new BudgetSummary(Guid.NewGuid(), "Q1 Budget", 50000m, 30000m);
var updated = original with { AllocatedAmount = 35000m };

// Value equality
var budget1 = new BudgetSummary(Guid.NewGuid(), "Test", 1000m, 500m);
var budget2 = new BudgetSummary(budget1.BudgetId, "Test", 1000m, 500m);
var areEqual = budget1 == budget2; // True - value equality
```

### Init-Only Properties

```csharp
// ✅ CORRECT - Init-only properties for immutability
public sealed class Budget
{
    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTimeOffset CreatedDate { get; init; }

    // Can only be set during object initialization
    public Budget()
    {
        BudgetId = Guid.NewGuid();
        CreatedDate = DateTimeOffset.UtcNow;
    }
}

// Usage
var budget = new Budget
{
    Name = "Q1 Budget",
    Amount = 50000m
    // BudgetId and CreatedDate set by constructor
};

// budget.Name = "New Name"; // Compile error!
```

### Required Members (C# 11+)

```csharp
// ✅ CORRECT - Required members
public sealed class CreateBudgetRequest
{
    public required string Name { get; init; }
    public required decimal Amount { get; init; }
    public DateTimeOffset? StartDate { get; init; }
}

// Usage
var request = new CreateBudgetRequest
{
    Name = "Q1 Budget",
    Amount = 50000m
    // StartDate is optional
};

// var invalid = new CreateBudgetRequest { Amount = 1000m }; // Compile error - Name required!
```

### Pattern Matching

```csharp
// ✅ CORRECT - Advanced pattern matching
public sealed class BudgetStatusCalculator
{
    public string GetStatus(Budget budget)
    {
        var remaining = budget.Amount - budget.Goals.Sum(g => g.TargetAmount);

        return budget switch
        {
            { Amount: <= 0 } => "Invalid",
            _ when remaining < 0 => "Over Budget",
            _ when remaining == 0 => "Fully Allocated",
            _ when remaining < budget.Amount * 0.1m => "Nearly Allocated",
            _ => "Available"
        };
    }

    public decimal CalculateFee(Budget budget) => budget.Amount switch
    {
        < 1000m => 0m,
        >= 1000m and < 10000m => budget.Amount * 0.01m,
        >= 10000m and < 100000m => budget.Amount * 0.005m,
        _ => budget.Amount * 0.001m
    };
}
```

---

## Anti-Patterns to Avoid

### God Object

```csharp
// ❌ WRONG - God object doing everything
public sealed class BudgetManager
{
    // Data access
    public async Task<Budget> GetBudgetAsync(Guid id) { /* ... */ }
    public async Task SaveBudgetAsync(Budget budget) { /* ... */ }

    // Validation
    public bool ValidateBudget(Budget budget) { /* ... */ }

    // Calculations
    public decimal CalculateRemaining(Budget budget) { /* ... */ }
    public decimal CalculateAllocated(Budget budget) { /* ... */ }

    // Notifications
    public async Task SendEmailAsync(Budget budget) { /* ... */ }
    public async Task SendSmsAsync(Budget budget) { /* ... */ }

    // Reporting
    public async Task<byte[]> GeneratePdfAsync(Budget budget) { /* ... */ }
    public async Task<byte[]> GenerateExcelAsync(Budget budget) { /* ... */ }

    // Caching
    public void CacheBudget(Budget budget) { /* ... */ }
    public Budget? GetCachedBudget(Guid id) { /* ... */ }
}

// ✅ CORRECT - Separated responsibilities
public sealed class BudgetRepository { /* Data access only */ }
public sealed class BudgetValidator { /* Validation only */ }
public sealed class BudgetCalculator { /* Calculations only */ }
public sealed class BudgetNotificationService { /* Notifications only */ }
public sealed class BudgetReportGenerator { /* Reporting only */ }
public sealed class BudgetCacheService { /* Caching only */ }
```

### Anemic Domain Model

```csharp
// ❌ WRONG - Anemic domain model (data bag with no behavior)
public sealed class Budget
{
    public Guid BudgetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public List<Goal> Goals { get; set; } = new();
}

public sealed class BudgetService
{
    public decimal CalculateRemaining(Budget budget)
    {
        return budget.Amount - budget.Goals.Sum(g => g.TargetAmount);
    }

    public bool CanAddGoal(Budget budget, Goal goal)
    {
        var remaining = CalculateRemaining(budget);
        return remaining >= goal.TargetAmount;
    }
}

// ✅ CORRECT - Rich domain model with behavior
public sealed class Budget
{
    private readonly List<Goal> _goals = new();

    public Guid BudgetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();

    public decimal CalculateRemaining()
    {
        return Amount - _goals.Sum(g => g.TargetAmount);
    }

    public bool CanAddGoal(Goal goal)
    {
        ArgumentNullException.ThrowIfNull(goal);
        return CalculateRemaining() >= goal.TargetAmount;
    }

    public void AddGoal(Goal goal)
    {
        ArgumentNullException.ThrowIfNull(goal);

        if (!CanAddGoal(goal))
            throw new InvalidOperationException("Insufficient budget for goal");

        if (_goals.Any(g => g.GoalId == goal.GoalId))
            throw new InvalidOperationException("Goal already exists");

        _goals.Add(goal);
    }
}
```

---

## Common Pitfalls

### Pitfall 1: Breaking Encapsulation

```csharp
// ❌ WRONG - Direct access to mutable collection
public sealed class Budget
{
    public List<Goal> Goals { get; set; } = new();
}

// Client can bypass business rules
budget.Goals.Add(invalidGoal); // No validation!

// ✅ CORRECT - Controlled access through methods
public sealed class Budget
{
    private readonly List<Goal> _goals = new();
    public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();

    public void AddGoal(Goal goal)
    {
        // Validation and business rules enforced
        ArgumentNullException.ThrowIfNull(goal);

        if (!CanAddGoal(goal))
            throw new InvalidOperationException("Cannot add goal");

        _goals.Add(goal);
    }
}
```

### Pitfall 2: Violating LSP with Exceptions

```csharp
// ❌ WRONG - Derived type throws unexpected exception
public abstract class DocumentGenerator
{
    public abstract Task<byte[]> GenerateAsync(Budget budget);
}

public sealed class ReadOnlyDocumentGenerator : DocumentGenerator
{
    public override Task<byte[]> GenerateAsync(Budget budget)
    {
        throw new NotSupportedException(); // Violates LSP!
    }
}

// ✅ CORRECT - Don't inherit if contract can't be honored
public interface IDocumentGenerator
{
    Task<byte[]> GenerateAsync(Budget budget);
}

public interface IReadOnlyDocument
{
    Task<byte[]> GetAsync(Guid documentId);
}

// Separate interfaces for different capabilities
```

### Pitfall 3: Tight Coupling

```csharp
// ❌ WRONG - Tight coupling to concrete implementation
public sealed class BudgetService
{
    private readonly SqlBudgetRepository _repository; // Concrete!

    public BudgetService()
    {
        _repository = new SqlBudgetRepository(); // Direct instantiation!
    }
}

// ✅ CORRECT - Loose coupling via abstraction
public sealed class BudgetService
{
    private readonly IBudgetRepository _repository; // Abstraction!

    public BudgetService(IBudgetRepository repository) // DI!
    {
        _repository = repository;
    }
}
```

---

## Summary Checklist

- [ ] **SOLID principles** applied throughout
- [ ] **Encapsulation** - Private fields, controlled access
- [ ] **Abstraction** - Clean interfaces, hidden complexity
- [ ] **Polymorphism** - Interface/abstract class usage
- [ ] **Composition over inheritance** - Favor composition
- [ ] **Design patterns** - Used appropriately, not forced
- [ ] **Immutability** - Records, init-only properties
- [ ] **Guard clauses** - All public methods validate inputs
- [ ] **No god objects** - Single responsibility respected
- [ ] **Rich domain models** - Behavior in entities, not services
- [ ] **Loose coupling** - Depend on abstractions via DI
- [ ] **C# modern features** - Records, pattern matching, init

---

## Related Patterns

- [Service-Oriented Architecture](./service-oriented-architecture.md) - SOA patterns and principles
- [Microservices](./microservices.md) - Microservices architecture
- [MVVM](./mvvm.md) - Model-View-ViewModel pattern
- [Test-Driven Development](./test-driven-development.md) - TDD methodology

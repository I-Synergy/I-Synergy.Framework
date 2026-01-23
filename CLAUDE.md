# AI Instructions - I-Synergy Framework

This file provides guidance to AI assistants (Claude Code, GitHub Copilot) when working with the I-Synergy Framework repository.

## Project Overview

I-Synergy Framework is an open-source .NET framework providing reusable components for building enterprise applications. It includes libraries for:

- **Core**: Base classes, extensions, validation, Result/Option types
- **MVVM**: Model-View-ViewModel implementation for UI applications
- **CQRS**: Command Query Responsibility Segregation (custom implementation, no MediatR)
- **ASP.NET Core**: Authentication, globalization, monitoring, multi-tenancy
- **UI**: Cross-platform UI libraries (WPF, UWP, WinUI, MAUI, Blazor)
- **Infrastructure**: Entity Framework, Storage (Azure), Mail, MessageBus
- **Domain-specific**: Financial, Geography, Mathematics, Physics

## Technology Stack

- **.NET 10** with latest C# language features
- **ASP.NET Core** for web APIs
- **Entity Framework Core** for data access
- **XUnit** and **Reqnroll.NET** for testing
- **OpenTelemetry** for observability

## Architecture Principles

This framework follows **Clean Architecture** with strict separation of concerns:

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Core business entities, value objects, domain events, repository interfaces (zero external dependencies) |
| **Application** | Business logic orchestration using CQRS patterns (commands, queries, events, vertical slices) |
| **Infrastructure** | Concrete implementations (database, messaging, email, storage, third-party integrations) |
| **Presentation** | Web API controllers, middleware, DTOs, validation |

### Architecture Patterns
- **CQRS** (Command Query Responsibility Segregation) for application logic organization
- **Domain-Driven Design** with rich domain models and aggregate roots
- **Vertical Slice Architecture** within the Application layer
- **Dependency Injection** throughout all layers
- **Repository Pattern** with Unit of Work for data access

## Prohibited Libraries

Do NOT use these libraries - use the framework's custom implementations instead:

| Prohibited | Use Instead |
|------------|-------------|
| **MediatR** | Custom command/query dispatching in `ISynergy.Framework.CQRS` |
| **AutoMapper** | Manual mapping with extension methods |
| **SpecFlow** | Reqnroll.NET for BDD scenarios |

## Coding Standards & Practices

- Always implement the CQRS, SOLID, CLEANCODE, Clean Architecture principles
- Use structured logging and remove/replace obsolete/redundant/legacy code
- Look first at the SharedKernel, Shared.Application, Application.Model and Domain projects before creating new implementations
- Don't expose publicly domain objects in the Application.Model
- Don't do inconsistent or simulated implementations, refactor for consistency
- Apply conservative refactoring - preserve all functionality
- Conduct comprehensive search for existing implementations before creating new ones
- If there are uncertainties, ask for suggestions before implementing

### Code Quality Requirements
- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Clean Code**: Readable, maintainable, self-documenting code with meaningful names
- **Functional Programming**: Immutability, expression-bodied members, pure functions where applicable
- **Modern C# Features**: Records, pattern matching, nullable reference types, global using statements

### Error Handling & Resilience
- **Result<T>** and **Option<T>** types for explicit error handling and null safety
- **Guard clauses** for all public methods with comprehensive input validation
- **Structured logging** with appropriate log levels and correlation IDs
- **Resilience patterns** (retries, circuit breakers, timeouts) for external service calls
- **Domain exceptions** for business rule violations

### Asynchronous Programming
- **Async/await** best practices with `CancellationToken` support
- Avoid `async void` except for event handlers
- Use `ConfigureAwait(false)` in library code
- Proper disposal of resources with `using` statements

---

## Clean Code Guidelines

### File and Folder Structure

#### File Organization
- **One public type per file**: Each publicly accessible type should be in its own C# file
- **Logical folder structure**: Organize with meaningful directories (`/Models`, `/Services`, `/Controllers`, etc.)
- **Separate test projects**: Keep tests in dedicated projects/folders
- **Meaningful file names**: File name must match the type name

#### Project Structure
```
ProjectName/
├── Domain/
├── Application/
│   └── Features/
│       └── [EntityName]/
│           ├── Commands/
│           ├── Queries/
│           └── Events/
├── Infrastructure/
└── Presentation/
```

### Naming Conventions

#### Type Naming
- **Classes, enums, structs**: `PascalCase` (e.g., `UserService`, `OrderStatus`)
- **Interfaces**: `I` + `PascalCase` (e.g., `IUserRepository`, `IEmailService`)
- **Abstract classes**: `PascalCase` with descriptive base name (e.g., `BaseEntity`, `AbstractHandler`)

#### Member Naming
- **Methods and properties**: `PascalCase` (e.g., `GetUser()`, `IsActive`)
- **Fields**:
  - Private: `_camelCase` (e.g., `_userService`, `_logger`)
  - Static: `s_camelCase` (e.g., `s_defaultTimeout`)
- **Local variables and parameters**: `camelCase` (e.g., `userId`, `cancellationToken`)
- **Constants**: `PascalCase` (e.g., `MaxRetryAttempts`, `DefaultTimeoutSeconds`)

#### Naming Best Practices
- **No abbreviations**: Use `Customer` instead of `Cust`, `Manager` instead of `Mgr`
- **No Hungarian notation**: Avoid prefixes like `strName`, `intCount`
- **Descriptive names**: Use `CalculateMonthlyPayment()` instead of `Calc()`
- **Boolean properties**: Use `Is`, `Has`, `Can` prefixes (e.g., `IsActive`, `HasPermission`, `CanExecute`)

### Classes and Methods Design

#### Single Responsibility Principle
- **One responsibility per class**: Each class should handle one specific concern
- **One responsibility per method**: Methods should do one thing well
- **Interface segregation**: Create focused, specific interfaces

#### Method Design
- **Clear, descriptive names**: Method names should explain what they do
- **Parameter limits**: Prefer fewer than 4 parameters; use parameter objects for complex scenarios
- **Return types**: Use `Result<T>`, `Option<T>`, or appropriate domain types

#### Class Design Example
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Access Modifiers and Visibility

#### Visibility Rules
- **Default to most restrictive**: Use the most restrictive access modifier possible
- **Public members grouped first**: Organize members by visibility level
- **Member ordering**:
  1. Public constants
  2. Public properties
  3. Public methods
  4. Protected members
  5. Private members

### Exception Handling

#### Exception Strategy
- **Exceptions for exceptional cases**: Only use exceptions for truly exceptional situations
- **Never catch and ignore**: Always log, handle, or re-throw exceptions
- **Custom exception types**: Create domain-specific exceptions ending with `Exception`
- **Use InnerException**: Preserve original exception context
- **Fail fast**: Validate inputs early and throw meaningful exceptions

#### Exception Example
```csharp
public class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid userId)
        : base($"User with ID {userId} was not found")
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
```

### Comments and Documentation

#### XML Documentation
- **All public APIs**: Document public classes, methods, properties
- **Parameter descriptions**: Explain purpose and constraints
- **Return value descriptions**: Describe what the method returns
- **Exception documentation**: Document thrown exceptions

```csharp
/// <summary>
/// Retrieves a user by their unique identifier.
/// </summary>
/// <param name="userId">The unique identifier of the user.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>
/// A <see cref="Result{T}"/> containing the user if found,
/// or a failure result if not found or an error occurs.
/// </returns>
/// <exception cref="ArgumentException">Thrown when userId is empty.</exception>
public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
```

#### Comment Guidelines
- **Explain "why", not "what"**: Code should be self-explanatory for "what"
- **Avoid trivial comments**: Don't comment obvious code
- **TODO/FIXME comments**: Include clear context and reasoning
- **Update comments**: Keep comments in sync with code changes

### Code Formatting and Layout

#### Formatting Standards
- **Indentation**: 4 spaces, no tabs
- **Brace style**: Allman style (braces on new line)
- **Line spacing**: One blank line between members and types
- **Using statements**: At the top of the file, remove unused ones
- **Line length**: Prefer lines under 120 characters

### Modern C# Features

```csharp
// Records for immutable data
public record UserDto(Guid Id, string Name, string Email);

// Pattern matching
public string GetUserStatus(User user) => user switch
{
    { IsActive: true, LastLoginDate: var date } when date > DateTime.Now.AddDays(-30) => "Active",
    { IsActive: true } => "Inactive",
    { IsActive: false } => "Disabled",
    _ => "Unknown"
};

// Expression-bodied members
public bool IsAdult => Age >= 18;
public string FullName => $"{FirstName} {LastName}";
```

---

## CQRS Implementation Guidelines

### Folder Structure
```
Application/
├── Features/
│   └── [EntityName]/
│       ├── Commands/
│       │   └── [CommandName]/
│       │       ├── [Command].cs
│       │       ├── [CommandHandler].cs
│       │       ├── [CommandValidator].cs
│       │       └── [CommandResponse].cs
│       ├── Queries/
│       │   └── [QueryName]/
│       │       ├── [Query].cs
│       │       ├── [QueryHandler].cs
│       │       └── [QueryResponse].cs
│       └── Events/
│           └── [DomainEvent].cs
├── Common/
│   ├── Interfaces/
│   ├── Behaviors/
│   └── Exceptions/
└── Infrastructure/
    └── Persistence/
```

### Naming Conventions
- **Commands**: End with `Command` (e.g., `CreateUserCommand`, `UpdateOrderCommand`)
- **Queries**: End with `Query` (e.g., `GetUserByIdQuery`, `GetOrdersQuery`)
- **Handlers**: End with `Handler` (e.g., `CreateUserCommandHandler`, `GetUserByIdQueryHandler`)
- **Responses**: End with `Response` (e.g., `CreateUserResponse`, `GetUserResponse`)
- **Validators**: End with `Validator` (e.g., `CreateUserCommandValidator`)

### Command Design
```csharp
// Immutable command using records
public record CreateUserCommand : ICommand<CreateUserResponse>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public DateTime? BirthDate { get; init; }
}

// Alternative with primary constructor
public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email) : ICommand<UpdateUserResponse>;
```

### Query Design
```csharp
// Simple query
public record GetUserByIdQuery(Guid UserId) : IQuery<GetUserResponse>;

// Complex query with multiple parameters
public record GetUsersQuery : IQuery<GetUsersResponse>
{
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public UserSortBy SortBy { get; init; } = UserSortBy.LastName;
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;
}
```

### Command Handler Structure
```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IDomainEventPublisher _eventPublisher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateUserCommandHandler> logger,
        IDomainEventPublisher eventPublisher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<CreateUserResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating user with email {Email}", command.Email);

            var user = User.Create(command.FirstName, command.LastName, command.Email);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishAsync(new UserCreatedEvent(user.Id), cancellationToken);

            _logger.LogInformation("Successfully created user {UserId}", user.Id);

            return Result.Success(new CreateUserResponse(user.Id, user.Email));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error creating user: {Message}", ex.Message);
            return Result.Failure<CreateUserResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user");
            return Result.Failure<CreateUserResponse>("Failed to create user");
        }
    }
}
```

### Query Handler Structure
```csharp
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserResponse>
{
    private readonly IUserReadRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public async Task<Result<GetUserResponse>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving user {UserId}", query.UserId);

        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", query.UserId);
            return Result.Failure<GetUserResponse>("User not found");
        }

        var response = _mapper.Map<GetUserResponse>(user);
        return Result.Success(response);
    }
}
```

### Controller Integration
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public UsersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> CreateUser(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetUserResponse>> GetUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _queryDispatcher.DispatchAsync(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }
}
```

---

## SOLID Principles Guidelines

### 1. Single Responsibility Principle (SRP)

A class should have only one reason to change.

```csharp
// Bad: Multiple responsibilities
public class UserManager
{
    public void CreateUser(User user) { /* user creation logic */ }
    public void SendWelcomeEmail(User user) { /* email logic */ }
    public void LogUserActivity(User user) { /* logging logic */ }
}

// Good: Single responsibility classes
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
    {
        var user = User.Create(request.FirstName, request.LastName, request.Email);
        await _userRepository.AddAsync(user);
        await _emailService.SendWelcomeEmailAsync(user.Email);
        _logger.LogInformation("User {UserId} created successfully", user.Id);
        return Result.Success(user);
    }
}
```

### 2. Open/Closed Principle (OCP)

Software entities should be open for extension but closed for modification.

```csharp
// Good: Open for extension via interfaces
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(Payment payment);
    bool CanProcess(PaymentType type);
}

public class CreditCardProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(Payment payment) { /* ... */ }
    public bool CanProcess(PaymentType type) => type == PaymentType.CreditCard;
}

public class PaymentService
{
    private readonly IEnumerable<IPaymentProcessor> _processors;

    public async Task<PaymentResult> ProcessPaymentAsync(Payment payment)
    {
        var processor = _processors.FirstOrDefault(p => p.CanProcess(payment.Type));
        if (processor == null)
            return PaymentResult.Failure($"No processor found for {payment.Type}");
        return await processor.ProcessAsync(payment);
    }
}
```

### 3. Liskov Substitution Principle (LSP)

Objects of a superclass should be replaceable with objects of a subclass.

```csharp
// Good: Proper substitution
public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
}

public class Rectangle : Shape
{
    public double Width { get; }
    public double Height { get; }
    public override double CalculateArea() => Width * Height;
    public override double CalculatePerimeter() => 2 * (Width + Height);
}

public class Square : Shape
{
    public double Side { get; }
    public override double CalculateArea() => Side * Side;
    public override double CalculatePerimeter() => 4 * Side;
}
```

### 4. Interface Segregation Principle (ISP)

Clients should not be forced to depend on interfaces they don't use.

```csharp
// Bad: Fat interface
public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<AuthResult> AuthenticateAsync(string email, string password);
    Task SendWelcomeEmailAsync(User user);
    Task<UserReport> GenerateUserReportAsync(DateTime from, DateTime to);
}

// Good: Segregated interfaces
public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task AddAsync(User user);
}

public interface IUserAuthenticationService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);
}

public interface IUserNotificationService
{
    Task SendWelcomeEmailAsync(User user);
}
```

### 5. Dependency Inversion Principle (DIP)

High-level modules should not depend on low-level modules. Both should depend on abstractions.

```csharp
// Good: Depends on abstractions
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repository,
        IEmailService emailService,
        ILogger<OrderService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

---

## Structured Logging Guidelines

### Event ID Categories

| Category | ID Range | Description |
|----------|----------|-------------|
| Application Lifecycle | 1000-1099 | Startup/shutdown events |
| File Operations | 3000-3099 | File system actions |
| Configuration | 5000-5099 | Settings management |
| API and External Services | 6000-6099 | External API calls |
| Performance Metrics | 8000-8099 | Performance monitoring |
| Security Events | 9000-9099 | Authentication/authorization |
| Database Operations | 10000-10099 | Database queries/transactions |
| Validation | 12000-12099 | Data validation |
| Domain Events | 18000-18099 | DDD events |
| Application Services | 19000-19099 | CQRS commands/queries |

### Logging Examples

```csharp
// Basic logging
_logger.LogInformation(LogEventIds.ApplicationStarted,
    "{Prefix} Application started",
    LoggingConstants.InfoPrefix);

// Domain event logging
_logger.LogInformation(LogEventIds.DomainEventPublished,
    "{Prefix} Domain event {EventName} published for aggregate {AggregateType} with ID {AggregateId}",
    LoggingConstants.InfoPrefix,
    "OrderCreated",
    "Order",
    orderId);

// Using scopes
using (_logger.BeginScope(new Dictionary<string, object>
{
    [LoggingConstants.ScopeProperties.CorrelationId] = Guid.NewGuid().ToString("N"),
    [LoggingConstants.ScopeProperties.OperationName] = "ProcessMarketData"
}))
{
    _logger.LogInformation("Working with market data");
}

// Performance logging
using (_logger.BeginPerformanceScope("ExpensiveOperation"))
{
    // Duration is automatically logged when the scope is disposed
}
```

### Best Practices
1. **Use Event IDs**: Always use an appropriate EventId to categorize log entries
2. **Use Scopes**: Create scopes for related operations to group log entries
3. **Include Context**: Provide relevant context (operation name, entity ID)
4. **Structured Data**: Use structured format `{Name}` instead of string concatenation
5. **Use Correlation IDs**: Always include correlation IDs for tracking related operations
6. **Choose Appropriate Level**: Trace for debugging, Info for normal operations, Error for failures

---

## Commit Message Guidelines

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

- Start with a **type** (`feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, etc.)
- Optional **scope** in parentheses
- Colon and space, then summary in **present tense** (under 300 characters)
- Use **lowercase** for type and scope

**Example:**
```
feat(auth): add JWT authentication support
```

---

## Testing Strategy

- **Test-Driven Development (TDD)** and **Behavior-Driven Development (BDD)**
- **Unit tests** for all business logic with comprehensive coverage
- **Integration tests** for API endpoints and data access layers
- **Gherkin scenarios** (Reqnroll.NET) for complex business workflows

### Test Structure
- **Test method naming**: `[Method]_[Scenario]_[ExpectedResult]`
- **AAA pattern**: Arrange, Act, Assert
- **One assertion per test**: Focus on single behavior

```csharp
[Fact]
public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
{
    // Arrange
    var userId = Guid.NewGuid();
    var expectedUser = UserBuilder.Create().WithId(userId).Build();
    _mockRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedUser);

    // Act
    var result = await _userService.GetUserByIdAsync(userId, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeEquivalentTo(expectedUser);
}
```

---

## Project Structure

```
src/
├── ISynergy.Framework.Core/           # Core utilities, extensions, base types
├── ISynergy.Framework.Mvvm/           # MVVM implementation
├── ISynergy.Framework.CQRS/           # Command/Query dispatching
├── ISynergy.Framework.AspNetCore*/    # ASP.NET Core libraries
├── ISynergy.Framework.UI*/            # UI platform libraries
├── ISynergy.Framework.EntityFramework/# EF Core integration
├── ISynergy.Framework.Storage*/       # Storage abstractions and Azure impl
├── ISynergy.Framework.Mail*/          # Email services
├── ISynergy.Framework.MessageBus*/    # Message bus abstractions
└── ...

tests/                                 # Unit and integration tests
samples/                               # Sample applications
performance/                           # Performance benchmarks
```

## Common Commands

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/ISynergy.Framework.Core.Tests

# Create NuGet packages
dotnet pack
```

## Notes for AI Assistants

- This is a **library/framework** project - changes affect downstream consumers
- Maintain **backwards compatibility** unless explicitly breaking
- All public APIs should have **XML documentation**
- Run tests before considering work complete
- When adding features, follow existing patterns in the codebase
- Search existing implementations first before creating new code

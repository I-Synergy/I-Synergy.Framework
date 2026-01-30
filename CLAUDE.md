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

## Dependency Management & Versioning

### Package Versioning Strategy
- **Semantic Versioning**: Follow [SemVer 2.0.0](https://semver.org/) strictly
  - **Major**: Breaking changes (incompatible API changes)
  - **Minor**: New features (backwards-compatible functionality)
  - **Patch**: Bug fixes (backwards-compatible fixes)

### Central Package Management
- All package versions are centrally managed in `Directory.Packages.props`
- Use `<PackageVersion>` elements, not `<PackageReference>` versions in project files
- Keep all framework packages at the same version for consistency

```xml
<!-- Directory.Packages.props -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.Extensions.Logging" Version="9.0.0" />
  </ItemGroup>
</Project>
```

### Breaking Changes Policy
- **Avoid breaking changes** in minor/patch releases
- When breaking changes are necessary:
  - Mark old APIs with `[Obsolete]` attribute first
  - Provide migration path in XML documentation
  - Document in CHANGELOG.md
  - Consider major version bump

```csharp
[Obsolete("Use GetUserByIdAsync instead. This method will be removed in v11.0.0.")]
public User GetUserById(Guid id) => GetUserByIdAsync(id).GetAwaiter().GetResult();
```

### Dependency Guidelines
- **Minimize external dependencies**: Only add packages that provide significant value
- **Avoid transitive dependency bloat**: Review dependency trees regularly
- **Target lowest viable versions**: Use minimum required versions for broader compatibility
- **Multi-targeting**: Support multiple framework versions when appropriate
- **Package references**: Always use exact or minimum version ranges

```xml
<!-- Prefer minimum version -->
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="[8.0.0,)" />
```

### Version Compatibility Matrix

| Framework Version | .NET Target | Support Status |
|------------------|-------------|----------------|
| 10.x | .NET 10 | Current |
| 9.x | .NET 9 | Maintenance |
| 8.x | .NET 8 | End of Life |

---

## Security Guidelines

### Authentication & Authorization
- **Never implement custom cryptography**: Use framework-provided implementations
- **JWT tokens**: Use `ISynergy.Framework.AspNetCore.Authentication` for token handling
- **Role-based access**: Implement using `[Authorize]` attributes and policy-based authorization
- **Secure by default**: All endpoints should require authentication unless explicitly marked with `[AllowAnonymous]`

```csharp
[Authorize(Policy = "RequireAdminRole")]
[HttpDelete("{id:guid}")]
public async Task<IActionResult> DeleteUser(Guid id)
{
    // Only admin users can access this endpoint
}
```

### Sensitive Data Handling
- **Never log sensitive data**: Passwords, tokens, personal information, credit cards
- **Secure storage**: Use Azure Key Vault or similar for secrets
- **Connection strings**: Never hardcode, always use configuration with secrets management
- **PII protection**: Mark properties containing PII with custom attributes for filtering

```csharp
public class User
{
    public Guid Id { get; init; }
    
    [PersonalData] // Mark PII for GDPR compliance
    public string Email { get; init; }
    
    [PersonalData]
    public string PhoneNumber { get; init; }
}
```

### Input Validation & Sanitization
- **Always validate input**: Use guard clauses and validation attributes
- **Prevent injection attacks**: Use parameterized queries, never string concatenation
- **Sanitize user input**: Especially for data displayed in UI or used in commands
- **Limit input size**: Set maximum lengths to prevent DoS attacks

```csharp
public async Task<Result<User>> CreateUserAsync(CreateUserCommand command)
{
    Guard.ArgumentNotNull(command);
    Guard.ArgumentNotNullOrEmpty(command.Email);
    Guard.ArgumentMaxLength(command.Email, 256);
    Guard.ArgumentIsValidEmail(command.Email);
    
    // Safe to proceed
}
```

### Cryptography Guidelines
- **Use .NET Cryptography APIs**: Never roll your own crypto
- **Hash passwords**: Use `PasswordHasher<T>` from ASP.NET Core Identity
- **Encrypt data at rest**: Use AES-256 or higher
- **TLS/SSL**: Always use HTTPS in production, enforce with HSTS

```csharp
// Good: Use built-in password hasher
var hasher = new PasswordHasher<User>();
var hashedPassword = hasher.HashPassword(user, password);
var result = hasher.VerifyHashedPassword(user, hashedPassword, password);
```

### Security Best Practices
1. **Principle of Least Privilege**: Grant minimum required permissions
2. **Defense in Depth**: Multiple layers of security controls
3. **Fail Securely**: On error, deny access by default
4. **Security Testing**: Include security-focused unit tests
5. **Dependency Scanning**: Regularly check for vulnerable packages
6. **OWASP Top 10**: Be aware of common vulnerabilities

---

## Performance & Optimization

### Performance Benchmarking
- **Use BenchmarkDotNet**: All performance-critical code should have benchmarks
- **Benchmark location**: Place benchmarks in `/performance` directory
- **Establish baselines**: Compare against previous versions
- **Test realistic scenarios**: Use production-like data volumes

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class UserServiceBenchmarks
{
    [Benchmark]
    public async Task<Result<User>> GetUserById()
    {
        return await _userService.GetUserByIdAsync(_testUserId);
    }
}
```

### Memory Management
- **Dispose resources**: Implement `IDisposable` or `IAsyncDisposable` for unmanaged resources
- **Avoid memory leaks**: Unsubscribe from events, dispose subscriptions
- **Use `ArrayPool<T>`**: For temporary large arrays
- **Span<T> and Memory<T>**: Use for high-performance scenarios
- **Avoid large object heap**: Keep objects under 85KB when possible

```csharp
public async ValueTask ProcessDataAsync()
{
    var buffer = ArrayPool<byte>.Shared.Rent(4096);
    try
    {
        // Use buffer
        await ProcessAsync(buffer.AsMemory(0, bytesRead));
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

### Asynchronous Performance
- **Use `ValueTask<T>`**: For frequently-called async methods that may complete synchronously
- **Avoid async over sync**: Don't use `Task.Run` to wrap synchronous code
- **ConfigureAwait(false)**: Always use in library code to avoid context capture
- **Parallel processing**: Use `Parallel.ForEachAsync` for CPU-bound parallel work

```csharp
// High-performance async method
public async ValueTask<User?> GetFromCacheAsync(Guid id)
{
    if (_cache.TryGetValue(id, out var user))
        return user; // Synchronous completion
    
    return await LoadFromDatabaseAsync(id).ConfigureAwait(false);
}
```

### Caching Strategies
- **Memory caching**: Use `IMemoryCache` for in-process caching
- **Distributed caching**: Use `IDistributedCache` for multi-instance scenarios
- **Cache expiration**: Always set appropriate expiration policies
- **Cache invalidation**: Implement proper invalidation strategies

```csharp
public async Task<User?> GetUserAsync(Guid id)
{
    var cacheKey = $"user:{id}";
    
    return await _cache.GetOrCreateAsync(cacheKey, async entry =>
    {
        entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        entry.SetSlidingExpiration(TimeSpan.FromMinutes(2));
        return await _repository.GetByIdAsync(id);
    });
}
```

### LINQ Optimization
- **Avoid multiple enumeration**: Use `.ToList()` or `.ToArray()` when needed
- **Prefer streaming**: Use `IEnumerable<T>` for large datasets
- **Early filtering**: Apply `.Where()` before projections
- **Avoid unnecessary allocations**: Use deferred execution when possible

```csharp
// Good: Single enumeration, early filtering
var activeUsers = users
    .Where(u => u.IsActive)
    .OrderBy(u => u.LastName)
    .Take(10)
    .ToList();
```

### Database Performance
- **Use async methods**: Always use `*Async` methods for database operations
- **Include related data**: Use `.Include()` to avoid N+1 queries
- **Projection**: Select only needed columns with `.Select()`
- **Pagination**: Always implement pagination for list queries
- **Compiled queries**: Use for frequently-executed queries
- **Index awareness**: Ensure queries use appropriate database indexes

```csharp
// Good: Efficient query with pagination
public async Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize)
{
    var query = _context.Users
        .AsNoTracking()
        .Where(u => u.IsActive)
        .OrderBy(u => u.LastName);
    
    var total = await query.CountAsync();
    var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(u => new UserDto(u.Id, u.FullName, u.Email))
        .ToListAsync();
    
    return new PagedResult<UserDto>(items, total, pageNumber, pageSize);
}
```

---

## Multi-Platform UI Considerations

### Platform-Specific Code Organization
```
ISynergy.Framework.UI/
├── Common/                    # Shared abstractions
├── Platforms/
│   ├── WPF/                  # WPF-specific implementations
│   ├── WinUI/                # WinUI-specific implementations
│   ├── Maui/                 # MAUI-specific implementations
│   └── Blazor/               # Blazor-specific implementations
└── Controls/                 # Shared control abstractions
```

### Platform Abstraction Patterns
- **Interface-based design**: Define platform-agnostic interfaces
- **Platform services**: Register platform-specific implementations via DI
- **Conditional compilation**: Use sparingly, prefer runtime abstraction

```csharp
// Shared abstraction
public interface IDialogService
{
    Task<bool> ShowConfirmAsync(string title, string message);
}

// Platform-specific implementation
public class WpfDialogService : IDialogService
{
    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        // WPF-specific implementation
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }
}
```

### Theme System Usage
- **Use framework themes**: Leverage `ISynergy.Framework.UI.Themes`
- **Dynamic theme switching**: Support light/dark mode at runtime
- **Platform integration**: Respect OS theme preferences
- **Color resources**: Use semantic color names from the theme system

```csharp
// ViewModel supporting theme changes
public class SettingsViewModel : ViewModelBase
{
    private readonly IThemeService _themeService;
    
    public ICommand ChangeThemeCommand { get; }
    
    private async Task ChangeThemeAsync(Theme theme)
    {
        await _themeService.SetThemeAsync(theme);
    }
}
```

### MVVM Framework Patterns
- **Inherit from BaseViewModel**: Use `ViewModelBase<T>` or `ViewModelNavigation<T>`
- **Observable properties**: Use `ObservableProperty` attribute or `SetProperty` method
- **Commands**: Use `RelayCommand` or `AsyncRelayCommand`
- **Navigation**: Use `INavigationService` for view navigation
- **Validation**: Implement `INotifyDataErrorInfo` via `ValidationBase`

```csharp
public class UserViewModel : ViewModelBase
{
    private string _firstName;
    
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }
    
    public IAsyncRelayCommand SaveCommand { get; }
    
    public UserViewModel(IUserService userService)
    {
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
    }
    
    private async Task SaveAsync()
    {
        var result = await _userService.SaveUserAsync(new User { FirstName = FirstName });
        if (result.IsSuccess)
            await NavigationService.GoBackAsync();
    }
    
    private bool CanSave() => !string.IsNullOrWhiteSpace(FirstName);
}
```

### Responsive UI Design
- **Adaptive layouts**: Support different screen sizes and orientations
- **Touch and mouse**: Design for both input methods
- **Accessibility**: Implement proper ARIA labels and keyboard navigation
- **Performance**: Virtualize large lists, lazy-load data

---

## API Design Principles

### Public API Surface Area
- **Minimize public APIs**: Only expose what consumers need
- **Internal by default**: Use `internal` unless explicitly needed as `public`
- **Sealed by default**: Only allow inheritance when designed for it
- **InternalsVisibleTo**: Use for testing, not for cross-assembly coupling

```csharp
// Good: Minimal public surface
public sealed class UserService : IUserService
{
    // Public interface methods only
    public async Task<Result<User>> GetUserAsync(Guid id) { }
    
    // Internal implementation details
    internal async Task<User?> GetFromCacheAsync(Guid id) { }
    
    // Private helpers
    private void ValidateUser(User user) { }
}
```

### Breaking Change Detection
- **API compatibility**: Use tools like ApiCompat or PublicApiAnalyzer
- **Semantic versioning**: Increment major version for breaking changes
- **Deprecation path**: Mark obsolete before removal

```csharp
[Obsolete("Use GetUserByIdAsync instead. Will be removed in v11.0.0.", false)]
public User GetUserById(Guid id) { }

// Later, in major version:
[Obsolete("This method has been removed. Use GetUserByIdAsync.", true)]
public User GetUserById(Guid id) => throw new NotSupportedException();
```

### Extensibility Points
- **Interfaces for testability**: All services should implement interfaces
- **Virtual methods**: Only when designed for inheritance
- **Extension methods**: For utility functionality
- **Plugin architecture**: Use dependency injection for extensibility

```csharp
// Good: Extensible through DI
public interface IUserValidationRule
{
    Task<ValidationResult> ValidateAsync(User user);
}

public class UserService
{
    private readonly IEnumerable<IUserValidationRule> _validationRules;
    
    public async Task<Result<User>> CreateUserAsync(User user)
    {
        foreach (var rule in _validationRules)
        {
            var result = await rule.ValidateAsync(user);
            if (!result.IsValid)
                return Result.Failure<User>(result.ErrorMessage);
        }
        // Create user
    }
}
```

### Nullability Annotations
- **Enable nullable reference types**: `<Nullable>enable</Nullable>` in all projects
- **Explicit nullability**: Use `?` for nullable, no marker for non-nullable
- **Null validation**: Use guard clauses for public method parameters

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<User?> GetUserAsync(Guid id)
    {
        // May return null
        return await _repository.GetByIdAsync(id);
    }
    
    public async Task<Result<User>> CreateUserAsync(string firstName, string? middleName, string lastName)
    {
        // middleName is explicitly nullable, others are required
    }
}
```

### API Evolution Strategy
- **Additive changes**: Add overloads rather than changing signatures
- **Optional parameters**: Use for backwards-compatible additions
- **CancellationToken**: Always include as last parameter in async methods

```csharp
// Original method
public Task<Result<User>> GetUserAsync(Guid id)
    => GetUserAsync(id, CancellationToken.None);

// New overload with CancellationToken
public async Task<Result<User>> GetUserAsync(Guid id, CancellationToken cancellationToken)
{
    // Implementation
}
```

---

## Documentation Requirements

### XML Documentation Standards
- **All public APIs**: 100% XML documentation coverage required
- **Summary**: Describe what the method/property does
- **Remarks**: Add usage notes, examples, or warnings
- **Parameters**: Describe each parameter's purpose and constraints
- **Returns**: Describe return value and possible states
- **Exceptions**: Document all exceptions that can be thrown
- **Example**: Include code examples for complex APIs

```csharp
/// <summary>
/// Retrieves a user by their unique identifier with optional related data.
/// </summary>
/// <param name="userId">The unique identifier of the user to retrieve.</param>
/// <param name="includeOrders">If true, includes the user's order history.</param>
/// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
/// <returns>
/// A <see cref="Result{T}"/> containing the user if found, or a failure result with error details.
/// </returns>
/// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is empty.</exception>
/// <remarks>
/// This method uses caching with a 10-minute expiration. For real-time data, consider
/// using <see cref="GetUserUncachedAsync"/> instead.
/// </remarks>
/// <example>
/// <code>
/// var result = await userService.GetUserAsync(userId, includeOrders: true);
/// if (result.IsSuccess)
/// {
///     var user = result.Value;
///     Console.WriteLine($"User: {user.FullName}");
/// }
/// </code>
/// </example>
public async Task<Result<User>> GetUserAsync(
    Guid userId,
    bool includeOrders = false,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### DocFx Documentation
- **Location**: Documentation source in `/docs` directory
- **Articles**: Create conceptual documentation for complex features
- **API documentation**: Generated from XML comments
- **Samples**: Include working sample projects in `/samples`

```yaml
# docfx.json
{
  "metadata": [{
    "src": [{ "files": ["src/**/*.csproj"] }],
    "dest": "api"
  }],
  "build": {
    "content": [
      { "files": ["api/**.yml", "api/index.md"] },
      { "files": ["docs/**.md", "toc.yml", "index.md"] }
    ],
    "dest": "_site"
  }
}
```

### Migration Guides
- **Create for major versions**: Document breaking changes and migration steps
- **Location**: `/docs/MIGRATION_GUIDE.md`
- **Include code examples**: Show before/after for common scenarios

```markdown
# Migration Guide: v9 to v10

## Breaking Changes

### IUserService Changes

**Before (v9):**
```csharp
User GetUser(Guid id);
```

**After (v10):**
```csharp
Task<Result<User>> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
```

**Migration:**
1. Change method to async
2. Handle Result<T> return type
3. Add CancellationToken support
```

### Changelog Maintenance
- **Format**: Follow [Keep a Changelog](https://keepachangelog.com/) format
- **Update with every change**: Add entries to CHANGELOG.md
- **Categories**: Added, Changed, Deprecated, Removed, Fixed, Security
- **Release notes**: Generate from changelog for each version

```markdown
# Changelog

## [10.0.0] - 2026-01-23

### Added
- New `Result<T>` type for explicit error handling
- Support for .NET 10

### Changed
- **BREAKING**: All service methods now return `Task<Result<T>>` instead of `T`
- Updated dependency on Microsoft.Extensions.Logging to 9.0.0

### Deprecated
- `GetUserById` method - use `GetUserByIdAsync` instead

### Removed
- **BREAKING**: Support for .NET 6 and earlier

### Fixed
- Fixed memory leak in event subscription handling
- Corrected async/await exception handling in command handlers

### Security
- Updated cryptography implementations to use stronger algorithms
```

---

## CI/CD Integration

### Azure Pipelines
- **Configuration**: `azure-pipelines.yml` at repository root
- **Build validation**: All PRs must pass build and tests
- **Multi-stage pipeline**: Build → Test → Pack → Publish

```yaml
# azure-pipelines.yml example structure
stages:
- stage: Build
  jobs:
  - job: BuildAndTest
    steps:
    - task: DotNetCoreCLI@2
      displayName: 'Build Solution'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Run Tests'
      inputs:
        command: 'test'
        projects: 'tests/**/*.csproj'
        arguments: '--collect:"XPlat Code Coverage"'
```

### Code Coverage Requirements
- **Minimum coverage**: 80% line coverage for all libraries
- **Critical paths**: 100% coverage for core business logic
- **Report generation**: Upload coverage to CodeCov or Azure Pipelines
- **Coverage gates**: Block PRs that reduce coverage below threshold

```xml
<!-- codecoverage.runsettings -->
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>cobertura,opencover</Format>
          <Exclude>[*.Tests]*,[*.TestHelpers]*</Exclude>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### Quality Gates
- **No compiler warnings**: Treat warnings as errors in Release builds
- **Static analysis**: Use Roslyn analyzers and code analysis rules
- **Security scanning**: Run vulnerability checks on dependencies
- **Performance regression**: Benchmark critical paths in CI

```xml
<!-- Directory.Build.props -->
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <AnalysisLevel>latest-all</AnalysisLevel>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

### Package Publishing
- **NuGet packages**: Generated in `/packages` directory
- **Symbol packages**: Always create .snupkg for debugging
- **Version**: Set in Directory.Build.props
- **Release process**: Tag-based releases trigger package publishing

---

## Framework-Specific Patterns

### Result<T> vs Exceptions

**Use Result<T> for:**
- Expected failures (validation errors, not found, business rule violations)
- Flow control based on success/failure
- Methods that can fail for domain reasons

**Use Exceptions for:**
- Unexpected errors (system failures, programming errors)
- Infrastructure failures (database unavailable)
- Situations that should crash the application

```csharp
// Good: Use Result<T> for expected failures
public async Task<Result<User>> CreateUserAsync(CreateUserCommand command)
{
    if (!IsValidEmail(command.Email))
        return Result.Failure<User>("Invalid email address");
    
    var user = User.Create(command.FirstName, command.LastName, command.Email);
    await _repository.AddAsync(user);
    return Result.Success(user);
}

// Good: Throw exception for unexpected errors
public async Task<User> GetUserByIdAsync(Guid id)
{
    Guard.ArgumentNotEmpty(id, nameof(id));
    
    var user = await _repository.GetByIdAsync(id);
    if (user == null)
        throw new EntityNotFoundException($"User {id} not found");
    
    return user;
}
```

### Option<T> Usage Patterns
- **Use Option<T> for**: Values that may or may not exist without it being an error
- **Avoid null**: Use `Option<T>` instead of returning `null`
- **Match pattern**: Use `Match` for handling Some/None cases

```csharp
// Good: Option<T> for optional values
public Option<User> FindUserByEmail(string email)
{
    var user = _users.FirstOrDefault(u => u.Email == email);
    return user != null ? Option<User>.Some(user) : Option<User>.None;
}

// Consuming Option<T>
public string GetUserDisplayName(string email)
{
    return FindUserByEmail(email).Match(
        some: user => user.FullName,
        none: () => "Unknown User"
    );
}
```

### BaseViewModel Patterns
- **Inherit from ViewModelBase**: For basic ViewModels
- **Use ViewModelNavigation<T>**: When navigation parameters are needed
- **Use ViewModelDialog<T>**: For dialog/modal ViewModels
- **Lifecycle methods**: Override `OnActivatedAsync` and `OnDeactivatedAsync`

```csharp
public class UserDetailsViewModel : ViewModelNavigation<Guid>
{
    private User _user;
    
    public User User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }
    
    protected override async Task OnActivatedAsync(Guid userId)
    {
        // Called when navigating to this ViewModel with userId parameter
        var result = await _userService.GetUserAsync(userId);
        if (result.IsSuccess)
            User = result.Value;
    }
    
    protected override Task OnDeactivatedAsync()
    {
        // Called when navigating away from this ViewModel
        // Clean up resources, unsubscribe from events
        return Task.CompletedTask;
    }
}
```

### Service Lifetime Patterns
- **Transient**: Stateless services, factories, commands/queries
- **Scoped**: Database contexts, Unit of Work, request-specific services
- **Singleton**: Caches, configuration, stateless utilities

```csharp
// Service registration
services.AddTransient<IUserService, UserService>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddSingleton<IMemoryCache, MemoryCache>();

// Command handlers are transient
services.AddTransient<ICommandHandler<CreateUserCommand, CreateUserResponse>, CreateUserCommandHandler>();
```

### Context Pattern
- **Use IContext**: For accessing current user, tenant, culture information
- **Scope**: Available throughout the request/operation scope
- **Thread-safe**: Context should be immutable per scope

```csharp
public interface IContext
{
    Guid UserId { get; }
    Guid TenantId { get; }
    string Culture { get; }
    bool IsAuthenticated { get; }
}

public class UserService
{
    private readonly IContext _context;
    
    public async Task<Result<Order>> CreateOrderAsync(CreateOrderCommand command)
    {
        // Automatically associate order with current user and tenant
        var order = new Order
        {
            UserId = _context.UserId,
            TenantId = _context.TenantId,
            // ...
        };
    }
}
```

---

## Multi-Tenancy Guidelines

### Tenant Isolation Patterns
- **Tenant Identification**: Use `ITenantProvider` to resolve current tenant
- **Data isolation**: Apply tenant filters automatically using query filters
- **Resource isolation**: Separate resources (storage, databases) per tenant when needed

```csharp
// Tenant-aware entity
public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}

// Automatic tenant filtering in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _tenantProvider.GetTenantId());
}
```

### Tenant Context
- **Resolution**: Resolve tenant from claims, headers, or subdomain
- **Validation**: Always validate tenant access for every request
- **Storage**: Store tenant ID in IContext for request lifetime

```csharp
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        var tenantId = ExtractTenantId(context);
        
        if (tenantId == Guid.Empty)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Tenant identification required");
            return;
        }
        
        tenantProvider.SetTenantId(tenantId);
        await _next(context);
    }
}
```

### Multi-Tenant Database Strategies

#### Strategy 1: Shared Database, Shared Schema
- **Use when**: Many small tenants, cost-effective
- **Implementation**: TenantId column in all tables with query filters
- **Pros**: Cost-effective, easy maintenance
- **Cons**: Risk of data leakage, limited customization

#### Strategy 2: Shared Database, Separate Schemas
- **Use when**: Medium number of tenants, some customization needed
- **Implementation**: Schema per tenant in same database
- **Pros**: Better isolation, customizable per tenant
- **Cons**: More complex, schema management overhead

#### Strategy 3: Separate Databases
- **Use when**: Few large tenants, strict isolation required
- **Implementation**: Separate database per tenant
- **Pros**: Complete isolation, highly customizable
- **Cons**: Expensive, difficult to maintain

```csharp
// Dynamic connection string per tenant
public class TenantDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    private readonly IConnectionStringProvider _connectionStringProvider;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenantId = _tenantProvider.GetTenantId();
        var connectionString = _connectionStringProvider.GetConnectionString(tenantId);
        optionsBuilder.UseSqlServer(connectionString);
    }
}
```

---

## Globalization & Localization

### Resource File Organization
```
Resources/
├── Strings.resx              # Default (English)
├── Strings.nl-NL.resx        # Dutch
├── Strings.de-DE.resx        # German
├── ErrorMessages.resx
├── ErrorMessages.nl-NL.resx
└── ValidationMessages.resx
```

### Resource Access Patterns
- **Use IStringLocalizer**: For runtime localization
- **Strongly-typed resources**: Generate resource classes for compile-time safety
- **Namespacing**: Group related resources by feature

```csharp
public class UserService
{
    private readonly IStringLocalizer<UserService> _localizer;
    
    public async Task<Result<User>> CreateUserAsync(CreateUserCommand command)
    {
        if (string.IsNullOrEmpty(command.Email))
            return Result.Failure<User>(_localizer["EmailRequired"]);
        
        if (!IsValidEmail(command.Email))
            return Result.Failure<User>(_localizer["EmailInvalid", command.Email]);
    }
}
```

### Culture Handling
- **Culture resolution**: From request headers, user preferences, or tenant settings
- **Thread culture**: Set `CultureInfo.CurrentCulture` and `CultureInfo.CurrentUICulture`
- **Date/Time formatting**: Always use culture-aware formatting
- **Number formatting**: Respect decimal separators and digit grouping

```csharp
// Culture middleware
public class CultureMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var culture = DetermineCulture(context);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        await _next(context);
    }
    
    private CultureInfo DetermineCulture(HttpContext context)
    {
        // 1. Check user preference
        // 2. Check tenant setting
        // 3. Check Accept-Language header
        // 4. Fall back to default
    }
}
```

### Localization Best Practices
- **Avoid concatenation**: Use format strings with placeholders
- **Plural forms**: Handle singular/plural appropriately
- **Gender**: Consider gender-neutral language
- **RTL support**: Support right-to-left languages
- **Testing**: Test with pseudo-localization

```csharp
// Good: Parameterized resource strings
_localizer["UserCreatedSuccess", user.FullName, DateTime.Now]
// Resource: "User {0} was created successfully on {1:d}"

// Bad: String concatenation
$"User {user.FullName} was created successfully on {DateTime.Now:d}"
```

---

## Project Structure
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

### Critical Reminders
- This is a **library/framework** project - changes affect downstream consumers
- Maintain **backwards compatibility** unless explicitly breaking
- All public APIs must have **XML documentation** (100% coverage required)
- Run tests before considering work complete
- When adding features, follow existing patterns in the codebase
- Search existing implementations first before creating new code

### Before Making Changes
1. **Search for existing implementations**: Use semantic_search or grep_search to find similar patterns
2. **Check shared projects**: Look in SharedKernel, Shared.Application, Application.Model, and Domain projects
3. **Review related code**: Understand the context and dependencies
4. **Check for breaking changes**: Use API compatibility analyzers
5. **Update documentation**: Modify XML comments, README, and CHANGELOG

### When Creating New Code
- **Follow framework patterns**: Use Result<T>, Option<T>, CQRS, and existing abstractions
- **Namespace conventions**: Match folder structure to namespace hierarchy
- **Testing requirements**: Create unit tests with minimum 80% coverage
- **Performance considerations**: Benchmark performance-critical code
- **Platform compatibility**: Ensure multi-platform support where applicable

### When Refactoring
- **Conservative approach**: Preserve all existing functionality
- **Incremental changes**: Make small, reviewable changes
- **Test-driven**: Ensure tests pass before and after refactoring
- **Document changes**: Update comments and documentation
- **Check callers**: Use list_code_usages to find all usage sites

### Quality Checklist
- [ ] Code follows SOLID principles
- [ ] Implements Clean Architecture patterns
- [ ] Uses structured logging with appropriate EventIds
- [ ] Includes comprehensive error handling
- [ ] Has XML documentation for all public APIs
- [ ] Includes unit tests with good coverage
- [ ] Follows naming conventions consistently
- [ ] Uses async/await properly with CancellationToken
- [ ] Validates inputs with guard clauses
- [ ] No compiler warnings
- [ ] No obsolete or legacy code introduced
- [ ] CHANGELOG.md updated if applicable

### Common Patterns to Follow
- **Services**: Inject dependencies via constructor, return Result<T>
- **Repositories**: Implement IRepository<T>, use async methods
- **Commands/Queries**: Immutable records inheriting from ICommand/IQuery
- **Handlers**: Implement ICommandHandler/IQueryHandler with structured logging
- **ViewModels**: Inherit from ViewModelBase or ViewModelNavigation<T>
- **Validation**: Use guard clauses and FluentValidation
- **Events**: Use domain events for cross-aggregate communication

### When Uncertain
- **Ask for clarification**: Don't guess requirements
- **Propose alternatives**: Offer multiple solutions with trade-offs
- **Highlight risks**: Point out potential issues or breaking changes
- **Request review**: For significant changes, suggest human review

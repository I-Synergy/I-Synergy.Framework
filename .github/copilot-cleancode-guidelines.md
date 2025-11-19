# Clean Code Guidelines for C# Projects

## 1. File and Folder Structure

### File Organization
- **One public type per file**: Each publicly accessible type should be in its own C# file
- **Logical folder structure**: Organize with meaningful directories (`/Models`, `/Services`, `/Controllers`, etc.)
- **Separate test projects**: Keep tests in dedicated projects/folders
- **Meaningful file names**: File name must match the type name, avoid generic folder names

### Project Structure
```
ProjectName/
??? Domain/
??? Application/
?   ??? Features/
?       ??? [EntityName]/
?           ??? Commands/
?           ??? Queries/
?           ??? Events/
??? Infrastructure/
??? Presentation/
```

## 2. Naming Conventions

### Type Naming
- **Classes, enums, structs**: `PascalCase` (e.g., `UserService`, `OrderStatus`)
- **Interfaces**: `I` + `PascalCase` (e.g., `IUserRepository`, `IEmailService`)
- **Abstract classes**: `PascalCase` with descriptive base name (e.g., `BaseEntity`, `AbstractHandler`)

### Member Naming
- **Methods and properties**: `PascalCase` (e.g., `GetUser()`, `IsActive`)
- **Fields**: 
  - Private: `_camelCase` (e.g., `_userService`, `_logger`)
  - Static: `s_camelCase` (e.g., `s_defaultTimeout`)
- **Local variables and parameters**: `camelCase` (e.g., `userId`, `cancellationToken`)
- **Constants**: `PascalCase` (e.g., `MaxRetryAttempts`, `DefaultTimeoutSeconds`)

### Naming Best Practices
- **No abbreviations**: Use `Customer` instead of `Cust`, `Manager` instead of `Mgr`
- **No Hungarian notation**: Avoid prefixes like `strName`, `intCount`
- **Descriptive names**: Use `CalculateMonthlyPayment()` instead of `Calc()`
- **Boolean properties**: Use `Is`, `Has`, `Can` prefixes (e.g., `IsActive`, `HasPermission`, `CanExecute`)

## 3. Classes and Methods Design

### Single Responsibility Principle
- **One responsibility per class**: Each class should handle one specific concern
- **One responsibility per method**: Methods should do one thing well
- **Interface segregation**: Create focused, specific interfaces

### Method Design
- **Clear, descriptive names**: Method names should explain what they do
- **Parameter limits**: Prefer fewer than 4 parameters; use parameter objects for complex scenarios
- **Return types**: Use `Result<T>`, `Option<T>`, or appropriate domain types

### Class Design
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

## 4. Access Modifiers and Visibility

### Visibility Rules
- **Default to most restrictive**: Use the most restrictive access modifier possible
- **Public members grouped first**: Organize members by visibility level
- **Member ordering**: 
  1. Public constants
  2. Public properties
  3. Public methods
  4. Protected members
  5. Private members

### Encapsulation
```csharp
public class Customer
{
    // Public properties
    public Guid Id { get; }
    public string Name { get; private set; }
    
    // Private fields
    private readonly List<Order> _orders;
    
    // Constructor
    public Customer(Guid id, string name)
    {
        Id = id;
        Name = Guard.Against.NullOrWhiteSpace(name);
        _orders = new List<Order>();
    }
    
    // Public methods
    public void UpdateName(string newName)
    {
        Name = Guard.Against.NullOrWhiteSpace(newName);
    }
    
    // Private methods
    private void ValidateBusinessRules()
    {
        // Implementation
    }
}
```

## 5. Exception Handling

### Exception Strategy
- **Exceptions for exceptional cases**: Only use exceptions for truly exceptional situations
- **Never catch and ignore**: Always log, handle, or re-throw exceptions
- **Custom exception types**: Create domain-specific exceptions ending with `Exception`
- **Use InnerException**: Preserve original exception context
- **Fail fast**: Validate inputs early and throw meaningful exceptions

### Exception Examples
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

// Usage
public async Task<Result<User>> GetUserAsync(Guid userId)
{
    try
    {
        var user = await _repository.GetByIdAsync(userId);
        return user != null 
            ? Result.Success(user) 
            : Result.Failure<User>("User not found");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to retrieve user {UserId}", userId);
        return Result.Failure<User>("Failed to retrieve user");
    }
}
```

## 6. Comments and Documentation

### XML Documentation
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

### Comment Guidelines
- **Explain "why", not "what"**: Code should be self-explanatory for "what"
- **Avoid trivial comments**: Don't comment obvious code
- **TODO/FIXME comments**: Include clear context and reasoning
- **Update comments**: Keep comments in sync with code changes

## 7. Code Formatting and Layout

### Formatting Standards
- **Indentation**: 4 spaces, no tabs
- **Brace style**: Allman style (braces on new line)
- **Line spacing**: One blank line between members and types
- **Using statements**: At the top of the file, remove unused ones
- **Line length**: Prefer lines under 120 characters

### Code Organization
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository, 
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<User>> CreateUserAsync(
            CreateUserRequest request, 
            CancellationToken cancellationToken)
        {
            // Implementation
        }
    }
}
```

## 8. Unit Testing Standards

### Test Structure
- **Test method naming**: `[Method]_[Scenario]_[ExpectedResult]`
- **AAA pattern**: Arrange, Act, Assert
- **One assertion per test**: Focus on single behavior
- **Test data builders**: Use builders for complex test data setup

### Test Examples
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

### Testing Principles
- **Test public behavior**: Focus on public APIs and contracts
- **Mock external dependencies**: Use mocks for repositories, external services
- **Test edge cases**: Include boundary conditions and error scenarios
- **Maintainable tests**: Keep tests simple and focused

## 9. Dependencies and Package Management

### Dependency Guidelines
- **Minimize dependencies**: Only add packages after team discussion
- **Use interfaces**: Depend on abstractions, not concretions
- **Avoid reflection**: Prefer compile-time safety over runtime discovery
- **Package management**: Keep packages up to date and documented

### Dependency Injection
```csharp
// Good: Depend on interfaces
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;

    public OrderService(IOrderRepository orderRepository, IEmailService emailService)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
    }
}

// Registration
services.AddScoped<IOrderService, OrderService>();
services.AddScoped<IOrderRepository, OrderRepository>();
```

## 10. Modern C# Features and Best Practices

### C# 12 Features
- **Primary constructors**: For simple classes
- **Collection expressions**: For collection initialization
- **Required properties**: For mandatory properties
- **Nullable reference types**: Enable for null safety

### Pattern Usage
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

### Performance Considerations
- **Async all the way**: Use async/await consistently
- **Memory allocation**: Be mindful of unnecessary allocations
- **String manipulation**: Use `StringBuilder` for concatenation in loops
- **Collection performance**: Choose appropriate collection types

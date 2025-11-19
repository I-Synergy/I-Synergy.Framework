# I-Synergy Framework CQRS

A robust, extensible CQRS (Command Query Responsibility Segregation) implementation for .NET 10.0 applications. This framework provides a clean separation between read (queries) and write (commands) operations, promoting clean architecture principles and maintainable codebases.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.CQRS.svg)](https://www.nuget.org/packages/I-Synergy.Framework.CQRS/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Complete CQRS implementation** with clear separation between commands and queries
- **Mediator pattern** through dispatcher abstraction
- **Automatic handler discovery** via assembly scanning
- **Decorator pattern support** for cross-cutting concerns (logging, notifications)
- **Async/await first design** with full cancellation token support
- **Type-safe** handler resolution and dispatch
- **Fluent configuration API** for easy setup
- **Dependency injection integration** with Microsoft.Extensions.DependencyInjection

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.CQRS
```

## Quick Start

### 1. Configure Services

In your `Program.cs` or `Startup.cs`:

```csharp
using ISynergy.Framework.CQRS.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;

public void ConfigureServices(IServiceCollection services)
{
    // Register core messaging service (required for notifications)
    services.AddScoped<IMessengerService, MessengerService>();

    // Add CQRS dispatchers
    services.AddCQRS();

    // Auto-register all handlers from current assembly
    services.AddHandlers(Assembly.GetExecutingAssembly());

    // Optional: Add command notifications
    services.AddCommandNotifications();

    // Optional: Add logging for handlers
    services.AddCQRSLogging();
}
```

### 2. Create a Command

Commands represent write operations that change application state:

```csharp
using ISynergy.Framework.CQRS.Abstractions.Commands;

// Command without result
public record CreateUserCommand : ICommand
{
    public string Username { get; init; }
    public string Email { get; init; }
}

// Command with result
public record CreateOrderCommand : ICommand<int>
{
    public string CustomerId { get; init; }
    public decimal Amount { get; init; }
}
```

### 3. Create a Command Handler

```csharp
using ISynergy.Framework.CQRS.Abstractions.Commands;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Username = command.Username,
            Email = command.Email
        };

        await _repository.AddAsync(user, cancellationToken);
    }
}

// Handler with result
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _repository;

    public CreateOrderCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            CustomerId = command.CustomerId,
            Amount = command.Amount
        };

        await _repository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}
```

### 4. Create a Query

Queries represent read operations that don't modify state:

```csharp
using ISynergy.Framework.CQRS.Abstractions.Queries;

public record GetUserByIdQuery : IQuery<UserDto>
{
    public int UserId { get; init; }
}

public record SearchOrdersQuery : IQuery<List<OrderDto>>
{
    public string CustomerId { get; init; }
    public DateTime? FromDate { get; init; }
}
```

### 5. Create a Query Handler

```csharp
using ISynergy.Framework.CQRS.Abstractions.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(query.UserId, cancellationToken);
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}
```

### 6. Dispatch Commands and Queries

In your controllers, services, or other application components:

```csharp
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;

public class UserController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public UserController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var command = new CreateUserCommand
        {
            Username = request.Username,
            Email = request.Email
        };

        await _commandDispatcher.DispatchAsync(command);
        return Ok();
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var command = new CreateOrderCommand
        {
            CustomerId = request.CustomerId,
            Amount = request.Amount
        };

        var orderId = await _commandDispatcher.DispatchAsync<CreateOrderCommand, int>(command);
        return Ok(new { OrderId = orderId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var user = await _queryDispatcher.DispatchAsync(query);
        return Ok(user);
    }
}
```

## Architecture

### Core Components

```
ISynergy.Framework.CQRS
├── Abstractions/
│   ├── Commands/          # Command and handler interfaces
│   ├── Queries/           # Query and handler interfaces
│   └── Dispatchers/       # Dispatcher interfaces
├── Dispatchers/           # Dispatcher implementations
├── Decorators/            # Handler decorators (logging, notifications)
├── Messages/              # Command messages for notifications
└── Extensions/            # Fluent configuration API
```

### Command Flow

```
Controller/Service
    ↓
ICommandDispatcher.DispatchAsync()
    ↓
[Optional Decorators: Logging, Notifications]
    ↓
ICommandHandler<TCommand>.HandleAsync()
    ↓
Business Logic / Repository
```

### Query Flow

```
Controller/Service
    ↓
IQueryDispatcher.DispatchAsync()
    ↓
IQueryHandler<TQuery, TResult>.HandleAsync()
    ↓
Data Access / Repository
    ↓
Return TResult
```

## Advanced Features

### Custom Decorators

Create custom decorators to add cross-cutting concerns:

```csharp
using ISynergy.Framework.CQRS.Decorators.Base;

public class ValidationCommandHandlerDecorator<TCommand>
    : CommandHandlerDecorator<TCommand>
    where TCommand : ICommand
{
    private readonly IValidator<TCommand> _validator;

    public ValidationCommandHandlerDecorator(
        ICommandHandler<TCommand> decorated,
        IValidator<TCommand> validator)
        : base(decorated)
    {
        _validator = validator;
    }

    public override async Task HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _decorated.HandleAsync(command, cancellationToken);
    }
}
```

Register the decorator:

```csharp
services.Decorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandlerDecorator<>));
```

### Command Notifications

When enabled, the framework automatically publishes a `CommandMessage<T>` via `IMessengerService` after successful command execution:

```csharp
// Enable notifications
services.AddCommandNotifications();

// Subscribe to command messages
public class UserActivityLogger
{
    public UserActivityLogger(IMessengerService messenger)
    {
        messenger.Subscribe<CommandMessage<CreateUserCommand>>(OnUserCreated);
    }

    private void OnUserCreated(CommandMessage<CreateUserCommand> message)
    {
        // Log activity, send email, update cache, etc.
        Console.WriteLine($"User created: {message.Content.Username}");
    }
}
```

### Handler Registration from Multiple Assemblies

```csharp
services.AddHandlers(
    typeof(UserCommand).Assembly,      // Domain assembly
    typeof(OrderCommand).Assembly,     // Orders assembly
    typeof(ProductCommand).Assembly    // Products assembly
);
```

## Design Patterns

This framework implements several key design patterns:

- **CQRS**: Separates read and write operations for better scalability and maintainability
- **Mediator**: Dispatchers act as mediators between application code and handlers
- **Decorator**: Extensible handler decoration for cross-cutting concerns
- **Dependency Injection**: Full DI integration for loose coupling
- **Command Pattern**: Encapsulates requests as objects

## Best Practices

> [!TIP]
> Keep commands and queries **immutable** using C# records or init-only properties.

> [!IMPORTANT]
> Queries should **never modify state**. All write operations should be commands.

> [!NOTE]
> Use cancellation tokens to support request cancellation in long-running operations.

### Command Design

- Commands should represent user **intentions** (e.g., `PlaceOrderCommand`, not `CreateOrderCommand`)
- Use records for immutability: `public record PlaceOrderCommand : ICommand<OrderId>`
- Keep commands focused on a single responsibility
- Avoid business logic in commands—they're just data containers

### Query Design

- Queries should be read-only and side-effect free
- Return DTOs, not domain entities, to avoid over-fetching
- Consider using projection to limit data transfer
- Cache query results when appropriate

### Handler Design

- Handlers should be **focused** and handle a single command/query type
- Inject dependencies via constructor
- Use repositories or data access services, not direct database access
- Always propagate cancellation tokens

## Testing

The framework is designed for testability:

```csharp
[Fact]
public async Task CreateUser_WithValidCommand_CreatesUser()
{
    // Arrange
    var repository = new Mock<IUserRepository>();
    var handler = new CreateUserCommandHandler(repository.Object);
    var command = new CreateUserCommand
    {
        Username = "john.doe",
        Email = "john@example.com"
    };

    // Act
    await handler.HandleAsync(command);

    // Assert
    repository.Verify(r => r.AddAsync(
        It.Is<User>(u => u.Username == "john.doe"),
        It.IsAny<CancellationToken>()),
        Times.Once);
}
```

## Dependencies

- **ISynergy.Framework.Core** - Core framework abstractions and messaging
- **Microsoft.Extensions.DependencyInjection** - Dependency injection support
- **Microsoft.Extensions.Logging** - Logging infrastructure

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.Mvvm** - MVVM framework integration
- **I-Synergy.Framework.EntityFramework** - Entity Framework integration
- **I-Synergy.Framework.AspNetCore** - ASP.NET Core integration

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).
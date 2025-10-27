# CQRS Implementation Guidelines for C#

## 1. Project Structure and Organization

### Folder Structure
```
Application/
??? Features/
?   ??? [EntityName]/
?       ??? Commands/
?       ?   ??? [CommandName]/
?       ?       ??? [Command].cs
?       ?       ??? [CommandHandler].cs
?       ?       ??? [CommandValidator].cs
?       ?       ??? [CommandResponse].cs
?       ??? Queries/
?       ?   ??? [QueryName]/
?       ?       ??? [Query].cs
?       ?       ??? [QueryHandler].cs
?       ?       ??? [QueryResponse].cs
?       ??? Events/
?           ??? [DomainEvent].cs
??? Common/
?   ??? Interfaces/
?   ??? Behaviors/
?   ??? Exceptions/
??? Infrastructure/
    ??? Persistence/
```

### File Organization Rules
- **One class per file**: Each command, query, handler, and response in separate files
- **Feature-based organization**: Group by business feature/domain aggregate
- **Consistent naming**: Follow established naming conventions throughout

## 2. Naming Conventions

### Commands and Queries
- **Commands**: End with `Command` (e.g., `CreateUserCommand`, `UpdateOrderCommand`)
- **Queries**: End with `Query` (e.g., `GetUserByIdQuery`, `GetOrdersQuery`)
- **Handlers**: End with `Handler` (e.g., `CreateUserCommandHandler`, `GetUserByIdQueryHandler`)
- **Responses**: End with `Response` (e.g., `CreateUserResponse`, `GetUserResponse`)
- **Validators**: End with `Validator` (e.g., `CreateUserCommandValidator`)

### Interface Naming
- **Command handlers**: `ICommandHandler<TCommand, TResponse>`
- **Query handlers**: `IQueryHandler<TQuery, TResponse>`
- **Validators**: `IValidator<T>`

### Example Structure
```csharp
// Command
public record CreateUserCommand(string FirstName, string LastName, string Email);

// Command Handler
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>

// Query
public record GetUserByIdQuery(Guid UserId);

// Query Handler
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserResponse>
```

## 3. Command and Query Object Design

### Command Design Principles
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

### Query Design Principles
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

### Design Rules
- **Immutable objects**: Use records or readonly properties
- **No business logic**: Commands and queries contain only data
- **Input validation**: Use data annotations or validators
- **Clear intent**: Names should express the business operation

## 4. Handler Implementation Patterns

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

            // Business logic
            var user = User.Create(command.FirstName, command.LastName, command.Email);
            
            // Persistence
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Domain events
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

    public GetUserByIdQueryHandler(
        IUserReadRepository userRepository,
        IMapper mapper,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

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

## 5. Dependency Injection and Mediation

### Handler Registration
```csharp
// In Program.cs or Startup.cs
services.AddScoped<ICommandHandler<CreateUserCommand, CreateUserResponse>, CreateUserCommandHandler>();
services.AddScoped<IQueryHandler<GetUserByIdQuery, GetUserResponse>, GetUserByIdQueryHandler>();

// Or using assembly scanning
services.AddMediatR(typeof(CreateUserCommandHandler).Assembly);
```

### Custom Dispatcher Implementation
```csharp
public interface ICommandDispatcher
{
    Task<Result<TResponse>> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken);
}

public interface IQueryDispatcher
{
    Task<Result<TResponse>> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
}

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> DispatchAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        var method = handlerType.GetMethod("Handle");
        var result = await (Task<Result<TResponse>>)method.Invoke(handler, new object[] { command, cancellationToken });
        
        return result;
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

## 6. Error Handling and Result Types

### Result Pattern Implementation
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Exception Handling Strategy
```csharp
public async Task<Result<CreateUserResponse>> Handle(
    CreateUserCommand command,
    CancellationToken cancellationToken)
{
    try
    {
        // Validation
        var validationResult = await ValidateCommand(command);
        if (!validationResult.IsValid)
        {
            return Result.Failure<CreateUserResponse>(validationResult.ErrorMessage);
        }

        // Business logic with domain exceptions
        var user = User.Create(command.FirstName, command.LastName, command.Email);
        
        // Infrastructure operations
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateUserResponse(user.Id));
    }
    catch (DomainException ex)
    {
        _logger.LogWarning(ex, "Domain rule violation: {Message}", ex.Message);
        return Result.Failure<CreateUserResponse>(ex.Message);
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
        return Result.Failure<CreateUserResponse>(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error in CreateUserCommandHandler");
        return Result.Failure<CreateUserResponse>("An unexpected error occurred");
    }
}
```

## 7. Validation Strategies

### Command Validation
```csharp
public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ValidationResult> ValidateAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // Basic validation
        if (string.IsNullOrWhiteSpace(command.FirstName))
            errors.Add("First name is required");

        if (string.IsNullOrWhiteSpace(command.LastName))
            errors.Add("Last name is required");

        if (!IsValidEmail(command.Email))
            errors.Add("Valid email address is required");

        // Business rule validation
        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingUser != null)
            errors.Add("User with this email already exists");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }

    private static bool IsValidEmail(string email)
    {
        // Email validation logic
        return !string.IsNullOrWhiteSpace(email) && 
               email.Contains('@') && 
               email.Contains('.');
    }
}
```

### Pipeline Behavior for Validation
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var validationTasks = _validators.Select(v => v.ValidateAsync(request, cancellationToken));
            var validationResults = await Task.WhenAll(validationTasks);

            var failures = validationResults
                .Where(r => !r.IsValid)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}
```

## 8. Documentation and Comments

### XML Documentation Standards
```csharp
/// <summary>
/// Handles the creation of a new user in the system.
/// </summary>
/// <remarks>
/// This handler performs the following operations:
/// 1. Validates the command input
/// 2. Creates a new user domain entity
/// 3. Persists the user to the database
/// 4. Publishes a UserCreated domain event
/// </remarks>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    /// <summary>
    /// Processes the create user command.
    /// </summary>
    /// <param name="command">The command containing user creation data.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created user response if successful,
    /// or failure information if the operation fails.
    /// </returns>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    /// <exception cref="ValidationException">Thrown when input validation fails.</exception>
    public async Task<Result<CreateUserResponse>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Business Context Documentation
```csharp
/// <summary>
/// Command to create a new user account in the system.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Email must be unique across all users
/// - First name and last name are required
/// - Email must be in valid format
/// - User will be created in active status by default
/// </remarks>
public record CreateUserCommand : ICommand<CreateUserResponse>
{
    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    /// <value>Must not be null, empty, or whitespace.</value>
    public required string FirstName { get; init; }

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    /// <value>Must not be null, empty, or whitespace.</value>
    public required string LastName { get; init; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    /// <value>Must be a valid email format and unique in the system.</value>
    public required string Email { get; init; }
}
```

## 9. Testing Strategies

### Unit Testing Command Handlers
```csharp
public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _mockLogger;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        
        _handler = new CreateUserCommandHandler(
            _mockUserRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesUserSuccessfully()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var existingUser = UserBuilder.Create().WithEmail("john.doe@example.com").Build();
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("email already exists");
    }
}
```

### Integration Testing
```csharp
[Collection("DatabaseCollection")]
public class CreateUserCommandIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CreateUserCommandIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ValidRequest_ReturnsCreatedUser()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "Integration",
            LastName = "Test",
            Email = "integration.test@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
        result.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();
    }
}
```

## 10. Performance and Best Practices

### Async/Await Best Practices
```csharp
public async Task<Result<GetUsersResponse>> Handle(
    GetUsersQuery query,
    CancellationToken cancellationToken)
{
    // Good: Pass cancellation token
    var users = await _userRepository.GetPagedAsync(
        query.PageNumber,
        query.PageSize,
        cancellationToken);

    // Good: Use ConfigureAwait(false) in libraries
    var userDtos = await MapUsersAsync(users).ConfigureAwait(false);

    return Result.Success(new GetUsersResponse(userDtos));
}
```

### Memory and Performance Optimization
```csharp
public async Task<Result<GetUsersResponse>> Handle(
    GetUsersQuery query,
    CancellationToken cancellationToken)
{
    // Use streaming for large datasets
    var users = _userRepository.GetUsersAsAsyncEnumerable(query.SearchCriteria);
    
    var userDtos = new List<UserDto>();
    await foreach (var user in users.WithCancellation(cancellationToken))
    {
        userDtos.Add(_mapper.Map<UserDto>(user));
        
        // Implement pagination at the database level
        if (userDtos.Count >= query.PageSize)
            break;
    }

    return Result.Success(new GetUsersResponse(userDtos));
}
```

### Caching Strategies
```csharp
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public async Task<Result<GetUserResponse>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"user:{query.UserId}";
        
        if (_cache.TryGetValue(cacheKey, out GetUserResponse cachedResponse))
        {
            _logger.LogDebug("User {UserId} retrieved from cache", query.UserId);
            return Result.Success(cachedResponse);
        }

        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<GetUserResponse>("User not found");

        var response = _mapper.Map<GetUserResponse>(user);
        
        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(15));
        
        return Result.Success(response);
    }
}

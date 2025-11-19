# SOLID Principles Guidelines for C# Development

## Overview

The SOLID principles are fundamental design principles that promote maintainable, testable, and extensible object-oriented software. These guidelines provide practical implementation strategies for applying SOLID principles in C# applications.

## 1. Single Responsibility Principle (SRP)

### Definition
A class should have only one reason to change, meaning it should have only one responsibility or concern.

### Implementation Guidelines

#### Class Responsibility
```csharp
// ? Bad: Multiple responsibilities
public class UserManager
{
    public void CreateUser(User user) { /* user creation logic */ }
    public void SendWelcomeEmail(User user) { /* email logic */ }
    public void LogUserActivity(User user) { /* logging logic */ }
    public void ValidateUser(User user) { /* validation logic */ }
}

// ? Good: Single responsibility classes
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUserValidator _userValidator;
    private readonly ILogger<UserService> _logger;

    public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
    {
        var validationResult = await _userValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure<User>(validationResult.ErrorMessage);

        var user = User.Create(request.FirstName, request.LastName, request.Email);
        await _userRepository.AddAsync(user);

        await _emailService.SendWelcomeEmailAsync(user.Email);
        _logger.LogInformation("User {UserId} created successfully", user.Id);

        return Result.Success(user);
    }
}

public class UserValidator : IUserValidator
{
    public async Task<ValidationResult> ValidateAsync(CreateUserRequest request)
    {
        // User validation logic only
    }
}

public class EmailService : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email)
    {
        // Email sending logic only
    }
}
```

#### Method Responsibility
```csharp
// ? Bad: Method doing too many things
public async Task ProcessOrderAsync(Order order)
{
    // Validate order
    if (order.Items.Count == 0)
        throw new InvalidOperationException("Order must have items");

    // Calculate totals
    decimal total = 0;
    foreach (var item in order.Items)
        total += item.Price * item.Quantity;

    // Apply discounts
    if (order.Customer.IsPremium)
        total *= 0.9m;

    // Save to database
    await _repository.SaveAsync(order);

    // Send notifications
    await _emailService.SendOrderConfirmationAsync(order);
    await _smsService.SendSmsAsync(order.Customer.PhoneNumber, "Order confirmed");

    // Update inventory
    foreach (var item in order.Items)
        await _inventoryService.UpdateStockAsync(item.ProductId, -item.Quantity);
}

// ? Good: Orchestrating single-responsibility methods
public async Task<Result> ProcessOrderAsync(Order order)
{
    var validationResult = await ValidateOrderAsync(order);
    if (!validationResult.IsValid)
        return Result.Failure(validationResult.ErrorMessage);

    await CalculateOrderTotalsAsync(order);
    await SaveOrderAsync(order);
    await SendNotificationsAsync(order);
    await UpdateInventoryAsync(order);

    return Result.Success();
}

private async Task<ValidationResult> ValidateOrderAsync(Order order) { /* validation only */ }
private async Task CalculateOrderTotalsAsync(Order order) { /* calculation only */ }
private async Task SaveOrderAsync(Order order) { /* persistence only */ }
private async Task SendNotificationsAsync(Order order) { /* notification only */ }
private async Task UpdateInventoryAsync(Order order) { /* inventory only */ }
```

### Common SRP Violations to Avoid
- **God classes**: Classes that know too much or do too much
- **Mixed concerns**: Business logic mixed with data access or presentation logic
- **Utility classes**: Large classes with unrelated static methods
- **Mixed abstraction levels**: High-level orchestration mixed with low-level implementation details

## 2. Open/Closed Principle (OCP)

### Definition
Software entities should be open for extension but closed for modification. You should be able to add new functionality without changing existing code.

### Implementation Guidelines

#### Strategy Pattern for Extensions
```csharp
// ? Bad: Modification required for new payment types
public class PaymentProcessor
{
    public async Task ProcessPaymentAsync(Payment payment)
    {
        switch (payment.Type)
        {
            case PaymentType.CreditCard:
                await ProcessCreditCardAsync(payment);
                break;
            case PaymentType.PayPal:
                await ProcessPayPalAsync(payment);
                break;
            case PaymentType.BankTransfer:
                await ProcessBankTransferAsync(payment);
                break;
            // Adding new payment type requires modifying this class
            default:
                throw new NotSupportedException($"Payment type {payment.Type} not supported");
        }
    }
}

// ? Good: Open for extension via interfaces
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(Payment payment);
    bool CanProcess(PaymentType type);
}

public class CreditCardProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(Payment payment)
    {
        // Credit card processing logic
        return PaymentResult.Success();
    }

    public bool CanProcess(PaymentType type) => type == PaymentType.CreditCard;
}

public class PayPalProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(Payment payment)
    {
        // PayPal processing logic
        return PaymentResult.Success();
    }

    public bool CanProcess(PaymentType type) => type == PaymentType.PayPal;
}

// Payment service orchestrates processors
public class PaymentService
{
    private readonly IEnumerable<IPaymentProcessor> _processors;

    public PaymentService(IEnumerable<IPaymentProcessor> processors)
    {
        _processors = processors;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(Payment payment)
    {
        var processor = _processors.FirstOrDefault(p => p.CanProcess(payment.Type));
        if (processor == null)
            return PaymentResult.Failure($"No processor found for {payment.Type}");

        return await processor.ProcessAsync(payment);
    }
}

// Adding new processor doesn't require changing existing code
public class CryptocurrencyProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(Payment payment)
    {
        // Cryptocurrency processing logic
        return PaymentResult.Success();
    }

    public bool CanProcess(PaymentType type) => type == PaymentType.Cryptocurrency;
}
```

#### Abstract Factory Pattern
```csharp
public abstract class ReportGenerator
{
    public abstract Task<byte[]> GenerateAsync(ReportData data);
    
    protected virtual Task<string> FormatHeaderAsync(ReportData data)
    {
        return Task.FromResult($"Report generated on {DateTime.Now:yyyy-MM-dd}");
    }
}

public class PdfReportGenerator : ReportGenerator
{
    public override async Task<byte[]> GenerateAsync(ReportData data)
    {
        // PDF-specific generation logic
        var header = await FormatHeaderAsync(data);
        // Generate PDF content
        return Array.Empty<byte>();
    }
}

public class ExcelReportGenerator : ReportGenerator
{
    public override async Task<byte[]> GenerateAsync(ReportData data)
    {
        // Excel-specific generation logic
        var header = await FormatHeaderAsync(data);
        // Generate Excel content
        return Array.Empty<byte>();
    }

    protected override Task<string> FormatHeaderAsync(ReportData data)
    {
        // Excel-specific header formatting
        return Task.FromResult($"Excel Report - {data.Title} - {DateTime.Now:yyyy-MM-dd}");
    }
}
```

#### Configuration-Driven Extension
```csharp
public interface IFeatureFlag
{
    string Name { get; }
    bool IsEnabled { get; }
}

public class FeatureManager
{
    private readonly IEnumerable<IFeatureFlag> _features;

    public FeatureManager(IEnumerable<IFeatureFlag> features)
    {
        _features = features;
    }

    public bool IsFeatureEnabled(string featureName)
    {
        return _features.FirstOrDefault(f => f.Name == featureName)?.IsEnabled ?? false;
    }
}

// New features can be added without modifying existing code
public class NewCheckoutFeature : IFeatureFlag
{
    public string Name => "NewCheckout";
    public bool IsEnabled => true;
}
```

## 3. Liskov Substitution Principle (LSP)

### Definition
Objects of a superclass should be replaceable with objects of a subclass without breaking the application functionality.

### Implementation Guidelines

#### Proper Inheritance Design
```csharp
// ? Good: Proper substitution
public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
    
    public virtual string GetDescription()
    {
        return $"Area: {CalculateArea():F2}, Perimeter: {CalculatePerimeter():F2}";
    }
}

public class Rectangle : Shape
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width = width > 0 ? width : throw new ArgumentException("Width must be positive");
        Height = height > 0 ? height : throw new ArgumentException("Height must be positive");
    }

    public override double CalculateArea() => Width * Height;
    public override double CalculatePerimeter() => 2 * (Width + Height);
}

public class Square : Shape
{
    public double Side { get; }

    public Square(double side)
    {
        Side = side > 0 ? side : throw new ArgumentException("Side must be positive");
    }

    public override double CalculateArea() => Side * Side;
    public override double CalculatePerimeter() => 4 * Side;
}

// Both Rectangle and Square can be used interchangeably
public class ShapeCalculator
{
    public double CalculateTotalArea(IEnumerable<Shape> shapes)
    {
        return shapes.Sum(shape => shape.CalculateArea());
    }
}
```

#### Interface Segregation with LSP
```csharp
// ? Bad: Violates LSP
public interface IBird
{
    void Fly();
    void Eat();
}

public class Sparrow : IBird
{
    public void Fly() => Console.WriteLine("Sparrow is flying");
    public void Eat() => Console.WriteLine("Sparrow is eating");
}

public class Penguin : IBird
{
    public void Fly() => throw new NotSupportedException("Penguins cannot fly"); // LSP violation
    public void Eat() => Console.WriteLine("Penguin is eating");
}

// ? Good: Proper interface design
public interface IBird
{
    void Eat();
}

public interface IFlyingBird : IBird
{
    void Fly();
}

public interface ISwimmingBird : IBird
{
    void Swim();
}

public class Sparrow : IFlyingBird
{
    public void Fly() => Console.WriteLine("Sparrow is flying");
    public void Eat() => Console.WriteLine("Sparrow is eating");
}

public class Penguin : ISwimmingBird
{
    public void Swim() => Console.WriteLine("Penguin is swimming");
    public void Eat() => Console.WriteLine("Penguin is eating");
}
```

#### Repository Pattern with LSP
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

// All implementations must honor the same contract
public class SqlRepository<T> : IRepository<T> where T : class
{
    public async Task<T> GetByIdAsync(Guid id)
    {
        // SQL implementation - never returns null without throwing for invalid operations
        // Always returns valid entity or throws NotFoundException
    }
    
    // Other methods follow the same contract guarantees
}

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    public async Task<T> GetByIdAsync(Guid id)
    {
        // In-memory implementation - honors same contract as SQL version
        // Same exception handling and return value guarantees
    }
    
    // Other methods follow the same contract guarantees
}
```

## 4. Interface Segregation Principle (ISP)

### Definition
Clients should not be forced to depend on interfaces they don't use. Create specific, focused interfaces rather than large, general-purpose ones.

### Implementation Guidelines

#### Focused Interface Design
```csharp
// ? Bad: Fat interface
public interface IUserService
{
    // User management
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<User> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task DeleteUserAsync(Guid id);
    
    // User queries
    Task<User> GetUserByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    
    // User authentication
    Task<AuthResult> AuthenticateAsync(string email, string password);
    Task<bool> ValidateTokenAsync(string token);
    Task LogoutUserAsync(Guid userId);
    
    // User notifications
    Task SendWelcomeEmailAsync(User user);
    Task SendPasswordResetEmailAsync(User user);
    Task SendNotificationAsync(User user, string message);
    
    // User reporting
    Task<UserReport> GenerateUserReportAsync(DateTime from, DateTime to);
    Task<byte[]> ExportUsersAsync(ExportFormat format);
}

// ? Good: Segregated interfaces
public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> SearchAsync(string searchTerm);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}

public interface IUserAuthenticationService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> GenerateTokenAsync(User user);
    Task InvalidateTokenAsync(string token);
}

public interface IUserNotificationService
{
    Task SendWelcomeEmailAsync(User user);
    Task SendPasswordResetEmailAsync(User user);
    Task SendCustomNotificationAsync(User user, string message);
}

public interface IUserReportingService
{
    Task<UserReport> GenerateReportAsync(DateTime from, DateTime to);
    Task<byte[]> ExportUsersAsync(ExportFormat format);
    Task<UserStatistics> GetStatisticsAsync();
}

// Application service composes focused services
public class UserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAuthenticationService _authService;
    private readonly IUserNotificationService _notificationService;

    public UserManagementService(
        IUserRepository userRepository,
        IUserAuthenticationService authService,
        IUserNotificationService notificationService)
    {
        _userRepository = userRepository;
        _authService = authService;
        _notificationService = notificationService;
    }

    public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
    {
        var user = User.Create(request.FirstName, request.LastName, request.Email);
        await _userRepository.AddAsync(user);
        await _notificationService.SendWelcomeEmailAsync(user);
        return Result.Success(user);
    }
}
```

#### Role-Based Interface Segregation
```csharp
// ? Bad: Single interface for all file operations
public interface IFileService
{
    Task<byte[]> ReadFileAsync(string path);
    Task WriteFileAsync(string path, byte[] content);
    Task DeleteFileAsync(string path);
    Task<FileInfo[]> ListFilesAsync(string directory);
    Task<bool> ExistsAsync(string path);
    Task CopyFileAsync(string source, string destination);
    Task MoveFileAsync(string source, string destination);
    Task<long> GetFileSizeAsync(string path);
    Task SetPermissionsAsync(string path, FilePermissions permissions);
    Task<FilePermissions> GetPermissionsAsync(string path);
}

// ? Good: Role-specific interfaces
public interface IFileReader
{
    Task<byte[]> ReadFileAsync(string path);
    Task<bool> ExistsAsync(string path);
    Task<long> GetFileSizeAsync(string path);
}

public interface IFileWriter
{
    Task WriteFileAsync(string path, byte[] content);
    Task DeleteFileAsync(string path);
}

public interface IFileManager : IFileReader, IFileWriter
{
    Task CopyFileAsync(string source, string destination);
    Task MoveFileAsync(string source, string destination);
    Task<FileInfo[]> ListFilesAsync(string directory);
}

public interface IFileSecurityManager
{
    Task SetPermissionsAsync(string path, FilePermissions permissions);
    Task<FilePermissions> GetPermissionsAsync(string path);
}

// Clients depend only on what they need
public class DocumentProcessor
{
    private readonly IFileReader _fileReader;

    public DocumentProcessor(IFileReader fileReader)
    {
        _fileReader = fileReader; // Only needs read operations
    }

    public async Task<ProcessResult> ProcessDocumentAsync(string path)
    {
        if (!await _fileReader.ExistsAsync(path))
            return ProcessResult.Failure("File not found");

        var content = await _fileReader.ReadFileAsync(path);
        // Process content
        return ProcessResult.Success();
    }
}
```

## 5. Dependency Inversion Principle (DIP)

### Definition
High-level modules should not depend on low-level modules. Both should depend on abstractions. Abstractions should not depend on details; details should depend on abstractions.

### Implementation Guidelines

#### Dependency Injection Patterns
```csharp
// ? Bad: High-level class depends on concrete implementations
public class OrderService
{
    private readonly SqlOrderRepository _repository; // Concrete dependency
    private readonly SmtpEmailService _emailService; // Concrete dependency
    private readonly FileLogger _logger; // Concrete dependency

    public OrderService()
    {
        _repository = new SqlOrderRepository(); // Direct instantiation
        _emailService = new SmtpEmailService();
        _logger = new FileLogger();
    }

    public async Task ProcessOrderAsync(Order order)
    {
        await _repository.SaveAsync(order);
        await _emailService.SendConfirmationAsync(order.CustomerEmail);
        _logger.Log($"Order {order.Id} processed");
    }
}

// ? Good: Depends on abstractions
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

    public async Task<Result> ProcessOrderAsync(Order order)
    {
        try
        {
            await _repository.SaveAsync(order);
            await _emailService.SendOrderConfirmationAsync(order.CustomerEmail, order);
            _logger.LogInformation("Order {OrderId} processed successfully", order.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order {OrderId}", order.Id);
            return Result.Failure("Order processing failed");
        }
    }
}
```

#### Factory Pattern for Complex Dependencies
```csharp
public interface IPaymentProcessorFactory
{
    IPaymentProcessor CreateProcessor(PaymentType type);
}

public class PaymentProcessorFactory : IPaymentProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentProcessor CreateProcessor(PaymentType type)
    {
        return type switch
        {
            PaymentType.CreditCard => _serviceProvider.GetRequiredService<ICreditCardProcessor>(),
            PaymentType.PayPal => _serviceProvider.GetRequiredService<IPayPalProcessor>(),
            PaymentType.BankTransfer => _serviceProvider.GetRequiredService<IBankTransferProcessor>(),
            _ => throw new NotSupportedException($"Payment type {type} not supported")
        };
    }
}

public class PaymentService
{
    private readonly IPaymentProcessorFactory _processorFactory;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentProcessorFactory processorFactory,
        ILogger<PaymentService> logger)
    {
        _processorFactory = processorFactory;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(Payment payment)
    {
        var processor = _processorFactory.CreateProcessor(payment.Type);
        return await processor.ProcessAsync(payment);
    }
}
```

#### Configuration and Dependency Registration
```csharp
// Program.cs or Startup.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServices(this IServiceCollection services)
    {
        // Repository abstractions
        services.AddScoped<IOrderRepository, SqlOrderRepository>();
        services.AddScoped<ICustomerRepository, SqlCustomerRepository>();
        
        // Service abstractions
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInventoryService, InventoryService>();
        
        // Application services
        services.AddScoped<IOrderService, OrderService>();
        
        // Factories
        services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();
        
        // Payment processors
        services.AddScoped<ICreditCardProcessor, CreditCardProcessor>();
        services.AddScoped<IPayPalProcessor, PayPalProcessor>();
        services.AddScoped<IBankTransferProcessor, BankTransferProcessor>();
        
        return services;
    }
}

// Usage in Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOrderServices();
```

#### Domain Layer Independence
```csharp
// Domain layer - no dependencies on infrastructure
namespace MyApp.Domain
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
    }

    public interface IDomainEventPublisher
    {
        Task PublishAsync(IDomainEvent domainEvent);
    }

    public class Order
    {
        private readonly List<OrderItem> _items = new();
        private readonly List<IDomainEvent> _domainEvents = new();

        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public OrderStatus Status { get; private set; }
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddItem(string productName, decimal price, int quantity)
        {
            var item = new OrderItem(productName, price, quantity);
            _items.Add(item);
            _domainEvents.Add(new OrderItemAddedEvent(Id, item));
        }

        public void MarkAsCompleted()
        {
            Status = OrderStatus.Completed;
            _domainEvents.Add(new OrderCompletedEvent(Id));
        }
    }
}

// Infrastructure layer - implements domain interfaces
namespace MyApp.Infrastructure.Persistence
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public SqlOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
```

## Best Practices Summary

### Code Quality Checklist
- **SRP**: Each class has one clear responsibility
- **OCP**: New features extend behavior without modifying existing code
- **LSP**: Substitutions work without breaking functionality
- **ISP**: Interfaces are focused and not forcing unnecessary implementations
- **DIP**: Dependencies flow toward abstractions

### Design Patterns That Support SOLID
- **Strategy Pattern**: Supports OCP and ISP
- **Factory Pattern**: Supports DIP and SRP
- **Repository Pattern**: Supports DIP and SRP
- **Command Pattern**: Supports SRP and OCP
- **Observer Pattern**: Supports OCP and DIP

### Testing Benefits
SOLID principles directly improve testability:
- **SRP**: Classes with single responsibilities are easier to test
- **OCP**: New functionality can be tested in isolation
- **LSP**: Substitutions enable better mocking strategies
- **ISP**: Focused interfaces reduce test setup complexity
- **DIP**: Dependency injection enables comprehensive unit testing

### Code Review Guidelines
When reviewing code for SOLID compliance:

1. **Identify responsibilities**: Can you clearly state what each class does in one sentence?
2. **Check dependencies**: Are classes depending on abstractions or concrete implementations?
3. **Evaluate extensions**: Can new features be added without modifying existing code?
4. **Review interfaces**: Are interfaces focused and not forcing unnecessary implementations?
5. **Test substitutions**: Can implementations be swapped without breaking functionality?

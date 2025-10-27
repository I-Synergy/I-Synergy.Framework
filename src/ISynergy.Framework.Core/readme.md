# I-Synergy Framework Core

Foundational library providing core abstractions, base classes, services, and utilities for building enterprise-grade .NET 10.0 applications. This package forms the foundation of the I-Synergy Framework ecosystem.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.Core.svg)](https://www.nuget.org/packages/I-Synergy.Framework.Core/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **Observable base classes** with automatic property change tracking and validation
- **Messenger service** for loosely coupled component communication using weak references
- **Comprehensive extension methods** for common types (string, datetime, collections, etc.)
- **Result pattern** for robust error handling without exceptions
- **Scoped context service** for managing tenant and user context across application layers
- **Busy service** for managing application busy states in UI applications
- **Rich validation framework** with data annotations support
- **Specialized collections** (ObservableCollection, BinaryTree, Tree structures)
- **Utilities** for common operations (file handling, regex, comparisons, etc.)
- **Type-safe serialization** with JSON support
- **Attribute-based DI lifetime management** for service registration
- **Async/await patterns** with cancellation token support

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.Core
```

## Quick Start

### 1. Observable Classes

Create observable objects with automatic property change notifications and dirty tracking:

```csharp
using ISynergy.Framework.Core.Base;

public class Person : ObservableClass
{
    public string Name
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public int Age
    {
        get => GetValue<int>();
        set => SetValue(value);
    }
}

// Usage
var person = new Person();
person.PropertyChanged += (s, e) => Console.WriteLine($"{e.PropertyName} changed");
person.Name = "John Doe"; // Triggers PropertyChanged event
Console.WriteLine(person.IsDirty); // true - object has unsaved changes
```

### 2. Observable Validated Classes

Add validation support to your models:

```csharp
using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

public class Customer : ObservableValidatedClass
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int Age
    {
        get => GetValue<int>();
        set => SetValue(value);
    }
}

// Usage
var customer = new Customer { Email = "invalid-email", Age = 15 };
if (!customer.IsValid)
{
    foreach (var error in customer.GetErrors())
    {
        Console.WriteLine(error.ErrorMessage);
    }
}
```

### 3. Messenger Service

Implement loosely coupled communication between components:

```csharp
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;

// Define a message
public record UserLoggedInMessage(string Username, DateTime LoginTime);

// Register a recipient
public class DashboardViewModel
{
    private readonly IMessengerService _messenger;

    public DashboardViewModel(IMessengerService messenger)
    {
        _messenger = messenger;
        _messenger.Register<UserLoggedInMessage>(this, OnUserLoggedIn);
    }

    private void OnUserLoggedIn(UserLoggedInMessage message)
    {
        Console.WriteLine($"User {message.Username} logged in at {message.LoginTime}");
    }
}

// Send a message
public class LoginService
{
    private readonly IMessengerService _messenger;

    public LoginService(IMessengerService messenger)
    {
        _messenger = messenger;
    }

    public async Task LoginAsync(string username)
    {
        // Perform login logic
        _messenger.Send(new UserLoggedInMessage(username, DateTime.UtcNow));
    }
}

// Configure in DI
services.AddScoped<IMessengerService, MessengerService>();
```

### 4. Result Pattern

Handle operations that may fail without throwing exceptions:

```csharp
using ISynergy.Framework.Core.Models;

public class UserService
{
    public Result<User> GetUserById(int id)
    {
        var user = Database.FindUser(id);

        if (user is null)
            return Result<User>.Fail("User not found");

        return Result<User>.Success(user);
    }

    public Result UpdateUser(User user)
    {
        try
        {
            Database.Update(user);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Update failed: {ex.Message}");
        }
    }
}

// Usage
var result = userService.GetUserById(123);
if (result.IsSuccess)
{
    var user = result.Value;
    Console.WriteLine($"Found user: {user.Name}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

### 5. Scoped Context Service

Manage tenant and user context across application layers:

```csharp
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;

// Define your context
public class AppContext : IContext
{
    public Guid TenantId { get; set; }
    public string Username { get; set; }
    public List<string> Roles { get; set; }
}

// Configure in DI
services.AddScopedContext<AppContext>();

// Use in middleware or controllers
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IScopedContextService contextService)
    {
        var tenantId = httpContext.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        if (Guid.TryParse(tenantId, out var parsedTenantId))
        {
            contextService.SetContext(new AppContext
            {
                TenantId = parsedTenantId,
                Username = httpContext.User.Identity?.Name ?? "Anonymous"
            });
        }

        await _next(httpContext);
    }
}

// Access context in services
public class OrderService
{
    private readonly IScopedContextService _contextService;

    public OrderService(IScopedContextService contextService)
    {
        _contextService = contextService;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        var context = _contextService.GetContext<AppContext>();
        return await Database.Orders
            .Where(o => o.TenantId == context.TenantId)
            .ToListAsync();
    }
}
```

## Core Components

### Base Classes

```
ISynergy.Framework.Core.Base/
├── ObservableClass              # INotifyPropertyChanged implementation with dirty tracking
├── ObservableValidatedClass     # ObservableClass + DataAnnotations validation
├── ModelClass                   # Base for domain models with identity
└── Property<T>                  # Property wrapper with change tracking
```

### Services

```
ISynergy.Framework.Core.Services/
├── MessengerService             # Weak reference-based messaging
├── ScopedContextService         # Request-scoped context management
├── BusyService                  # UI busy state management
├── InfoService                  # Application info (version, name, etc.)
├── LanguageService              # Localization support
└── RequestCancellationService   # Centralized cancellation token management
```

### Extensions

The Core library includes 35+ extension method classes:

- **String extensions**: IsNullOrEmpty, IsValidEmail, ToTitleCase, SplitCamelCase, etc.
- **DateTime extensions**: StartOfDay, EndOfDay, IsWeekend, AddWorkDays, etc.
- **Collection extensions**: AddRange, RemoveWhere, ForEach, Distinct, etc.
- **Enum extensions**: ToList, GetDescription, GetDisplayName, etc.
- **Type extensions**: IsNullableType, GetDefaultValue, ImplementsInterface, etc.
- **Object extensions**: ToJson, FromJson, DeepClone, etc.
- **Task extensions**: SafeFireAndForget, WithCancellation, WithTimeout, etc.

## Usage Examples

### Managing UI Busy States

```csharp
using ISynergy.Framework.Core.Abstractions.Services;

public class DataLoadViewModel
{
    private readonly IBusyService _busyService;
    private readonly IDataService _dataService;

    public DataLoadViewModel(IBusyService busyService, IDataService dataService)
    {
        _busyService = busyService;
        _dataService = dataService;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            _busyService.StartBusy("Loading data...");

            var data = await _dataService.GetDataAsync();

            _busyService.UpdateMessage("Processing data...");
            ProcessData(data);
        }
        finally
        {
            _busyService.StopBusy();
        }
    }
}

// Bind to UI
<ProgressRing IsActive="{Binding BusyService.IsBusy}" />
<TextBlock Text="{Binding BusyService.BusyMessage}" />
```

### Using Extension Methods

```csharp
using ISynergy.Framework.Core.Extensions;

// String extensions
string email = "  john.doe@example.com  ";
bool isValid = email.IsValidEmail(); // true
string clean = email.Trim().ToLower();

// DateTime extensions
var date = DateTime.Now;
var startOfWeek = date.StartOfWeek();
var endOfMonth = date.EndOfMonth();
bool isWeekend = date.IsWeekend();

// Collection extensions
var list = new List<int> { 1, 2, 3, 4, 5 };
list.RemoveWhere(x => x % 2 == 0); // Removes even numbers
var chunks = list.Chunk(2); // Split into chunks of 2

// Enum extensions
public enum OrderStatus
{
    [Description("Pending approval")]
    Pending,
    [Description("Approved and processing")]
    Approved,
    Completed
}

var statuses = EnumExtensions.ToList<OrderStatus>();
string description = OrderStatus.Pending.GetDescription(); // "Pending approval"

// Object extensions
var person = new Person { Name = "John", Age = 30 };
string json = person.ToJson();
var clone = person.DeepClone();
```

### Custom Collections

```csharp
using ISynergy.Framework.Core.Collections;

// ObservableCollection with item property change tracking
var products = new ObservableCollection<Product>();
products.CollectionChanged += (s, e) => Console.WriteLine("Collection changed");
products.ItemPropertyChanged += (s, e) =>
    Console.WriteLine($"Item property {e.PropertyName} changed");

var product = new Product { Name = "Widget", Price = 9.99m };
products.Add(product);
product.Price = 12.99m; // Triggers ItemPropertyChanged

// Binary Tree
var tree = new BinaryTree<int>();
tree.Add(5);
tree.Add(3);
tree.Add(7);
tree.Add(1);
tree.Add(9);

bool contains = tree.Contains(7); // true
tree.InOrderTraversal(value => Console.WriteLine(value)); // 1, 3, 5, 7, 9
```

## Configuration

### Dependency Injection Setup

```csharp
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;

public void ConfigureServices(IServiceCollection services)
{
    // Core services
    services.AddSingleton<IMessengerService, MessengerService>();
    services.AddSingleton<IBusyService, BusyService>();
    services.AddSingleton<IInfoService, InfoService>();
    services.AddSingleton<ILanguageService, LanguageService>();

    // Context management
    services.AddScopedContext<AppContext>();

    // Or use lifetime attributes
    services.Scan(scan => scan
        .FromAssemblyOf<Startup>()
        .AddClasses()
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .UsingAttributes());
}
```

### Using Lifetime Attributes

```csharp
using ISynergy.Framework.Core.Attributes;

[Lifetime(Lifetimes.Transient)]
public class TransientService : ITransientService
{
    // Service implementation
}

[Lifetime(Lifetimes.Scoped)]
public class ScopedService : IScopedService
{
    // Service implementation
}

[Lifetime(Lifetimes.Singleton)]
public class SingletonService : ISingletonService
{
    // Service implementation
}
```

## Advanced Features

### Custom Validation Rules

```csharp
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Validation;

public class Order : ObservableValidatedClass
{
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; }

    protected override IEnumerable<ValidationResult> Validate()
    {
        // Custom validation logic
        if (Amount < 0)
            yield return new ValidationResult(
                "Amount cannot be negative",
                new[] { nameof(Amount) });

        if (Amount > 10000 && string.IsNullOrEmpty(CustomerEmail))
            yield return new ValidationResult(
                "Email required for orders over $10,000",
                new[] { nameof(CustomerEmail) });
    }
}
```

### Safe Fire and Forget

```csharp
using ISynergy.Framework.Core.Extensions;

public class NotificationService
{
    public void SendNotification(string message)
    {
        // Fire and forget without blocking
        SendEmailAsync(message).SafeFireAndForget(
            onException: ex => Console.WriteLine($"Error: {ex.Message}"));
    }

    private async Task SendEmailAsync(string message)
    {
        await Task.Delay(1000);
        // Send email logic
    }
}
```

### Object Pooling

```csharp
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.ObjectPool;

public class BufferPool
{
    private readonly ObjectPool<byte[]> _pool;

    public BufferPool()
    {
        var policy = new DefaultPooledObjectPolicy<byte[]>();
        _pool = new DefaultObjectPool<byte[]>(policy);
    }

    public async Task ProcessDataAsync(Stream stream)
    {
        var buffer = _pool.Get();
        try
        {
            await stream.ReadAsync(buffer, 0, buffer.Length);
            // Process buffer
        }
        finally
        {
            _pool.Return(buffer);
        }
    }
}
```

## Best Practices

> [!TIP]
> Use **ObservableClass** as the base for all ViewModels and models that need property change notifications.

> [!IMPORTANT]
> Always call **Dispose()** or use **using** statements with ObservableClass instances to prevent memory leaks from event handlers.

> [!NOTE]
> The MessengerService uses weak references by default. Set `keepTargetAlive: true` only when using closures in message handlers.

### Observable Class Usage

- Always use `GetValue<T>()` and `SetValue()` for properties that need change tracking
- Call `MarkAsClean()` after saving changes to reset dirty tracking
- Use `Revert()` to restore original values before committing
- Dispose instances properly to clean up event handlers

### Messenger Service Usage

- Prefer constructor injection over `MessengerService.Default` singleton
- Unregister recipients in cleanup/dispose methods to prevent memory leaks
- Use tokens to create isolated message channels
- Keep message types immutable (use records)

### Result Pattern Usage

- Return `Result<T>` for operations that may fail
- Use `Result.Success()` and `Result.Fail()` factory methods
- Check `IsSuccess` before accessing `Value`
- Include meaningful error messages

### Context Service Usage

- Set context early in the request pipeline (middleware/filters)
- Don't store context in long-lived services
- Use context for tenant isolation and user-specific operations
- Clear context at the end of the request scope

## Testing

The Core library is designed for testability:

```csharp
[Fact]
public void ObservableClass_PropertyChange_RaisesEvent()
{
    // Arrange
    var person = new Person();
    var eventRaised = false;
    person.PropertyChanged += (s, e) =>
    {
        if (e.PropertyName == nameof(Person.Name))
            eventRaised = true;
    };

    // Act
    person.Name = "John";

    // Assert
    Assert.True(eventRaised);
    Assert.True(person.IsDirty);
}

[Fact]
public void Messenger_SendMessage_DeliversToRecipient()
{
    // Arrange
    var messenger = new MessengerService();
    var received = false;
    var recipient = new object();

    messenger.Register<string>(recipient, msg => received = true);

    // Act
    messenger.Send("Test message");

    // Assert
    Assert.True(received);
}
```

## Dependencies

- **Microsoft.Extensions.DependencyInjection** - Dependency injection support
- **Microsoft.Extensions.Logging.Abstractions** - Logging infrastructure
- **Microsoft.Extensions.ObjectPool** - Object pooling
- **Microsoft.Extensions.Hosting** - Application lifetime management
- **Microsoft.Extensions.Options.DataAnnotations** - Configuration validation
- **Microsoft.Extensions.Configuration.Binder** - Configuration binding

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Mvvm** - MVVM framework building on Core abstractions
- **I-Synergy.Framework.CQRS** - CQRS implementation using Core messaging
- **I-Synergy.Framework.EntityFramework** - EF Core integration with Core models
- **I-Synergy.Framework.AspNetCore** - ASP.NET Core integration

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).

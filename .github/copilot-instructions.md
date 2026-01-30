---
description: AI rules derived by SpecStory from the project AI interaction history
globs: *
---

## PROJECT OVERVIEW

(This section is intentionally blank, awaiting content.)

## CODE STYLE

(This section is intentionally blank, awaiting content.)

## FOLDER ORGANIZATION

(This section is intentionally blank, awaiting content.)

## TECH STACK

(This section is intentionally blank, awaiting content.)

## PROJECT-SPECIFIC STANDARDS

(This section is intentionally blank, awaiting content.)

## WORKFLOW & RELEASE RULES

(This section is intentionally blank, awaiting content.)

## REFERENCE EXAMPLES

(This section is intentionally blank, awaiting content.)

## PROJECT DOCUMENTATION & CONTEXT SYSTEM

(This section is intentionally blank, awaiting content.)

## DEBUGGING

(This section is intentionally blank, awaiting content.)

## FINAL DOs AND DON'Ts

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
Task<Result<User>>> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
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
// Good: Parameter
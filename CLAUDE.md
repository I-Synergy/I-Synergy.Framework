# AI Instructions - I-Synergy Framework

This file provides guidance to AI assistants (Claude Code, GitHub Copilot) when working with the I-Synergy Framework repository.

## Project Overview

I-Synergy Framework is an open-source .NET framework providing reusable components for building enterprise applications. It includes libraries for:

- **Core**: Base classes, extensions, validation, Result/Option types
- **MVVM**: Model-View-ViewModel implementation for UI applications
- **CQRS**: Command Query Responsibility Segregation (custom implementation, no MediatR)
- **ASP.NET Core**: Authentication, globalization, monitoring, multi-tenancy
- **UI**: Cross-platform UI libraries (WPF, WinUI, MAUI, Blazor)
- **Infrastructure**: Entity Framework, Storage (Azure/S3), Mail, MessageBus, KeyVault, EventSourcing
- **Domain-specific**: Financial, Geography, Mathematics, Physics

## Technology Stack

- **.NET 10** with latest C# language features
- **ASP.NET Core** for web APIs
- **Entity Framework Core** for data access
- **MSTest** and **Reqnroll.NET** for testing
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
| **AutoMapper** | Manual inline mapping (direct property assignment) |
| **Mapster** | Manual inline mapping (direct property assignment) |
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

### Comments and Documentation

#### XML Documentation
- **All public APIs**: Document public classes, methods, properties
- **Parameter descriptions**: Explain purpose and constraints
- **Return value descriptions**: Describe what the method returns
- **Exception documentation**: Document thrown exceptions

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
- **Records**: Use for immutable data transfer objects
- **Pattern matching**: Use switch expressions and property patterns
- **Expression-bodied members**: Use for simple property getters and single-expression methods
- **Nullable reference types**: Enable in all projects (`<Nullable>enable</Nullable>`)
- **Global usings**: Use for commonly imported namespaces

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
- Use immutable records with `required` or `init` properties
- Implement `ICommand<TResponse>` interface
- Use primary constructor syntax for simple commands

### Query Design
- Use immutable records implementing `IQuery<TResponse>`
- Include pagination parameters (`PageNumber`, `PageSize`) for list queries
- Include sort parameters when ordering is needed

### Handler Structure
- Inject dependencies via constructor (repository/DbContext, logger, unit of work)
- Return `Result<TResponse>` — never throw for domain errors
- Use structured logging with `LogInformation`/`LogWarning`/`LogError`
- Catch `DomainException` separately from general `Exception`
- Always accept and pass `CancellationToken`

### Controller Integration
- Inject `ICommandDispatcher` and `IQueryDispatcher`
- Dispatch commands/queries and map `Result<T>` to HTTP responses
- Return `Ok(result.Value)` on success, `BadRequest(result.Error)` or `NotFound()` on failure

---

## SOLID Principles Guidelines

### 1. Single Responsibility Principle (SRP)
A class should have only one reason to change. Each class handles one specific concern; compose behaviors via DI.

### 2. Open/Closed Principle (OCP)
Software entities should be open for extension but closed for modification. Use interfaces and DI to add behavior without modifying existing classes (e.g., `IEnumerable<IPaymentProcessor>`).

### 3. Liskov Substitution Principle (LSP)
Objects of a superclass should be replaceable with objects of a subclass without breaking behavior. Avoid overriding abstract members in ways that violate the base contract.

### 4. Interface Segregation Principle (ISP)
Clients should not be forced to depend on interfaces they don't use. Create focused, specific interfaces rather than fat interfaces that combine unrelated concerns.

### 5. Dependency Inversion Principle (DIP)
High-level modules should not depend on low-level modules. Both should depend on abstractions. Inject all dependencies via constructor using interfaces.

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

### Best Practices
1. **Use Event IDs**: Always use an appropriate `EventId` to categorize log entries
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

**Example:** `feat(auth): add JWT authentication support`

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

### Breaking Changes Policy
- **Avoid breaking changes** in minor/patch releases
- When breaking changes are necessary:
  - Mark old APIs with `[Obsolete]` attribute first
  - Provide migration path in XML documentation
  - Document in CHANGELOG.md
  - Consider major version bump

### Dependency Guidelines
- **Minimize external dependencies**: Only add packages that provide significant value
- **Avoid transitive dependency bloat**: Review dependency trees regularly
- **Target lowest viable versions**: Use minimum required versions for broader compatibility
- **Multi-targeting**: Support multiple framework versions when appropriate

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

### Sensitive Data Handling
- **Never log sensitive data**: Passwords, tokens, personal information, credit cards
- **Secure storage**: Use Azure Key Vault or similar for secrets
- **Connection strings**: Never hardcode, always use configuration with secrets management
- **PII protection**: Mark properties containing PII with `[PersonalData]` for GDPR compliance

### Input Validation & Sanitization
- **Always validate input**: Use guard clauses and validation attributes
- **Prevent injection attacks**: Use parameterized queries, never string concatenation
- **Sanitize user input**: Especially for data displayed in UI or used in commands
- **Limit input size**: Set maximum lengths to prevent DoS attacks

### Cryptography Guidelines
- **Use .NET Cryptography APIs**: Never roll your own crypto
- **Hash passwords**: Use `PasswordHasher<T>` from ASP.NET Core Identity
- **Encrypt data at rest**: Use AES-256 or higher
- **TLS/SSL**: Always use HTTPS in production, enforce with HSTS

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
- **Use BenchmarkDotNet**: All performance-critical code should have benchmarks in `/performance`
- **Establish baselines**: Compare against previous versions with realistic data volumes

### Memory Management
- **Dispose resources**: Implement `IDisposable` or `IAsyncDisposable` for unmanaged resources
- **Avoid memory leaks**: Unsubscribe from events, dispose subscriptions
- **Use `ArrayPool<T>`**: For temporary large arrays
- **Span<T> and Memory<T>**: Use for high-performance scenarios
- **Avoid large object heap**: Keep objects under 85KB when possible

### Asynchronous Performance
- **Use `ValueTask<T>`**: For frequently-called async methods that may complete synchronously
- **Avoid async over sync**: Don't use `Task.Run` to wrap synchronous code
- **ConfigureAwait(false)**: Always use in library code to avoid context capture
- **Parallel processing**: Use `Parallel.ForEachAsync` for CPU-bound parallel work

### Caching Strategies
- **Memory caching**: Use `IMemoryCache` for in-process caching
- **Distributed caching**: Use `IDistributedCache` for multi-instance scenarios
- **Cache expiration**: Always set appropriate expiration policies
- **Cache invalidation**: Implement proper invalidation strategies

### LINQ Optimization
- **Avoid multiple enumeration**: Use `.ToList()` or `.ToArray()` when needed
- **Prefer streaming**: Use `IEnumerable<T>` for large datasets
- **Early filtering**: Apply `.Where()` before projections
- **Avoid unnecessary allocations**: Use deferred execution when possible

### Database Performance
- **Use async methods**: Always use `*Async` methods for database operations
- **Include related data**: Use `.Include()` to avoid N+1 queries
- **Projection**: Select only needed columns with `.Select()`
- **Pagination**: Always implement pagination for list queries
- **Compiled queries**: Use for frequently-executed queries
- **Index awareness**: Ensure queries use appropriate database indexes

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

### Theme System Usage
- **Use framework themes**: Leverage `ISynergy.Framework.UI.Themes`
- **Dynamic theme switching**: Support light/dark mode at runtime
- **Platform integration**: Respect OS theme preferences
- **Color resources**: Use semantic color names from the theme system

### MVVM Framework Patterns
- **Inherit from BaseViewModel**: Use `ViewModelBase<T>` or `ViewModelNavigation<T>`
- **Observable properties**: Use `ObservableProperty` attribute or `SetProperty` method
- **Commands**: Use `RelayCommand` or `AsyncRelayCommand`
- **Navigation**: Use `INavigationService` for view navigation
- **Validation**: Implement `INotifyDataErrorInfo` via `ValidationBase`

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

### Breaking Change Detection
- **API compatibility**: Use tools like ApiCompat or PublicApiAnalyzer
- **Semantic versioning**: Increment major version for breaking changes
- **Deprecation path**: Mark obsolete before removal

### Extensibility Points
- **Interfaces for testability**: All services should implement interfaces
- **Virtual methods**: Only when designed for inheritance
- **Extension methods**: For utility functionality
- **Plugin architecture**: Use dependency injection for extensibility

### Nullability Annotations
- **Enable nullable reference types**: `<Nullable>enable</Nullable>` in all projects
- **Explicit nullability**: Use `?` for nullable, no marker for non-nullable
- **Null validation**: Use guard clauses for public method parameters

### API Evolution Strategy
- **Additive changes**: Add overloads rather than changing signatures
- **Optional parameters**: Use for backwards-compatible additions
- **CancellationToken**: Always include as last parameter in async methods

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

### DocFx Documentation
- **Location**: Documentation source in `/docs` directory
- **Articles**: Create conceptual documentation for complex features
- **API documentation**: Generated from XML comments
- **Samples**: Include working sample projects in `/samples`

### Migration Guides
- **Create for major versions**: Document breaking changes and migration steps
- **Location**: `/docs/MIGRATION_GUIDE.md`
- **Include code examples**: Show before/after for common scenarios

### Changelog Maintenance
- **Format**: Follow [Keep a Changelog](https://keepachangelog.com/) format
- **Update with every change**: Add entries to CHANGELOG.md
- **Categories**: Added, Changed, Deprecated, Removed, Fixed, Security
- **Release notes**: Generate from changelog for each version

---

## CI/CD Integration

### Azure Pipelines
- **Configuration**: `azure-pipelines.yml` at repository root
- **Build validation**: All PRs must pass build and tests
- **Multi-stage pipeline**: Build → Test → Pack → Publish

### Code Coverage Requirements
- **Minimum coverage**: 80% line coverage for all libraries
- **Critical paths**: 100% coverage for core business logic
- **Report generation**: Upload coverage to CodeCov or Azure Pipelines
- **Coverage gates**: Block PRs that reduce coverage below threshold

### Quality Gates
- **No compiler warnings**: Treat warnings as errors in Release builds (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`)
- **Static analysis**: Use Roslyn analyzers (`<EnableNETAnalyzers>true</EnableNETAnalyzers>`)
- **Security scanning**: Run vulnerability checks on dependencies
- **Performance regression**: Benchmark critical paths in CI

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

### Option<T> Usage Patterns
- **Use Option<T> for**: Values that may or may not exist without it being an error
- **Avoid null**: Use `Option<T>` instead of returning `null`
- **Match pattern**: Use `Match` for handling Some/None cases

### BaseViewModel Patterns
- **Inherit from ViewModelBase**: For basic ViewModels
- **Use ViewModelNavigation<T>**: When navigation parameters are needed
- **Use ViewModelDialog<T>**: For dialog/modal ViewModels
- **Lifecycle methods**: Override `OnActivatedAsync` and `OnDeactivatedAsync`

### Service Lifetime Patterns
- **Transient**: Stateless services, factories, commands/queries, command handlers
- **Scoped**: Database contexts, Unit of Work, request-specific services
- **Singleton**: Caches, configuration, stateless utilities

### Context Pattern
- **Use IContext**: For accessing current user, tenant, culture information
- **Scope**: Available throughout the request/operation scope
- **Thread-safe**: Context should be immutable per scope

---

## Multi-Tenancy Guidelines

### Tenant Isolation Patterns
- **Tenant Identification**: Use `ITenantProvider` to resolve current tenant
- **Data isolation**: Apply tenant filters automatically using EF Core query filters
- **Resource isolation**: Separate resources (storage, databases) per tenant when needed

### Tenant Context
- **Resolution**: Resolve tenant from claims, headers, or subdomain
- **Validation**: Always validate tenant access for every request
- **Storage**: Store tenant ID in IContext for request lifetime

### Multi-Tenant Database Strategies

#### Strategy 1: Shared Database, Shared Schema
- **Use when**: Many small tenants, cost-effective
- **Implementation**: TenantId column in all tables with EF Core query filters
- **Pros**: Cost-effective, easy maintenance
- **Cons**: Risk of data leakage, limited customization

#### Strategy 2: Shared Database, Separate Schemas
- **Use when**: Medium number of tenants, some customization needed
- **Implementation**: Schema per tenant in same database
- **Pros**: Better isolation, customizable per tenant
- **Cons**: More complex, schema management overhead

#### Strategy 3: Separate Databases
- **Use when**: Few large tenants, strict isolation required
- **Implementation**: Separate database per tenant with dynamic connection strings
- **Pros**: Complete isolation, highly customizable
- **Cons**: Expensive, difficult to maintain

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

### Culture Handling
- **Culture resolution**: From request headers, user preferences, or tenant settings
- **Thread culture**: Set `CultureInfo.CurrentCulture` and `CultureInfo.CurrentUICulture`
- **Date/Time formatting**: Always use culture-aware formatting
- **Number formatting**: Respect decimal separators and digit grouping

### Localization Best Practices
- **Avoid concatenation**: Use format strings with placeholders (e.g., `_localizer["Key", arg1, arg2]`)
- **Plural forms**: Handle singular/plural appropriately
- **Gender**: Consider gender-neutral language
- **RTL support**: Support right-to-left languages
- **Testing**: Test with pseudo-localization

---

## Project Structure

```
src/
├── ISynergy.Framework.Mvvm/           # MVVM implementation
├── ISynergy.Framework.CQRS/           # Command/Query dispatching
├── ISynergy.Framework.CQRS.SourceGenerator/ # AOT-compatible handler registration
├── ISynergy.Framework.AspNetCore*/    # ASP.NET Core libraries
├── ISynergy.Framework.UI*/            # UI platform libraries
├── ISynergy.Framework.EntityFramework/# EF Core integration
├── ISynergy.Framework.EventSourcing*/ # Event sourcing
├── ISynergy.Framework.Storage*/       # Storage abstractions, Azure Blob, AWS S3
├── ISynergy.Framework.Mail*/          # Email services (Microsoft365, SendGrid)
├── ISynergy.Framework.MessageBus*/    # Message bus (Azure Service Bus, RabbitMQ)
├── ISynergy.Framework.KeyVault*/      # Key management (Azure Key Vault, OpenBao)
├── ISynergy.Framework.OpenTelemetry*/ # Observability
└── ...

tests/                                 # Unit and integration tests
samples/                               # Sample applications
performance/                           # Performance benchmarks
```

---

## Notes for AI Assistants

### Critical Reminders
- This is a **library/framework** project - changes affect downstream consumers
- Maintain **backwards compatibility** unless explicitly breaking
- All public APIs must have **XML documentation** (100% coverage required)
- Run tests before considering work complete
- When adding features, follow existing patterns in the codebase
- Search existing implementations first before creating new code

### Before Making Changes
1. **Search for existing implementations**: Find similar patterns before writing new code
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
- **Check callers**: Find all usage sites before renaming or removing

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

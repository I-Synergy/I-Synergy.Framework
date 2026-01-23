# CLAUDE.md - I-Synergy Framework

This file provides guidance to Claude Code when working with the I-Synergy Framework repository.

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

## Prohibited Libraries

Do NOT use these libraries - use the framework's custom implementations instead:

| Prohibited | Use Instead |
|------------|-------------|
| **MediatR** | Custom command/query dispatching in `ISynergy.Framework.CQRS` |
| **AutoMapper** | Manual mapping with extension methods |
| **SpecFlow** | Reqnroll.NET for BDD scenarios |

## Coding Standards

### Core Principles
- Apply **SOLID**, **Clean Code**, and **CQRS** principles
- Use **Result<T>** and **Option<T>** for explicit error handling
- Include **CancellationToken** in all async methods
- Use **guard clauses** for input validation
- Use **structured logging** with `ILogger<T>` and EventIds

### Naming Conventions
- **Commands**: `CreateUserCommand`, `UpdateOrderCommand`
- **Queries**: `GetUserByIdQuery`, `GetOrdersQuery`
- **Handlers**: `CreateUserCommandHandler`, `GetUserByIdQueryHandler`
- **DTOs**: `UserResponseDto`, `OrderDto`
- **Interfaces**: `IUserRepository`, `IEmailService`
- **Private fields**: `_camelCase` (e.g., `_userService`)

### Code Style
- Use **records** for immutable data types
- Use **pattern matching** where appropriate
- Use **expression-bodied members** for simple properties/methods
- **4 spaces** indentation, **Allman** brace style
- Prefer **async/await** with `ConfigureAwait(false)` in library code

## Before Making Changes

1. **Search existing implementations first** - Check SharedKernel, Shared.Application, Application.Model, and Domain projects before creating new code
2. **Don't expose domain objects** - Never expose domain objects publicly in Application.Model
3. **Conservative refactoring** - Preserve all existing functionality when refactoring
4. **Ask when uncertain** - If there are uncertainties, ask before implementing

## Testing

- Use **XUnit** for unit tests
- Use **Reqnroll.NET** for BDD/Gherkin scenarios
- Test naming: `[Method]_[Scenario]_[ExpectedResult]`
- Follow **AAA pattern**: Arrange, Act, Assert
- Mock external dependencies

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

## Detailed Guidelines

For comprehensive implementation guidance, refer to the Copilot instruction files in `.github/`:

- [copilot-instructions.md](.github/copilot-instructions.md) - Main architecture and coding standards
- [copilot-cleancode-guidelines.md](.github/copilot-cleancode-guidelines.md) - Clean code practices
- [copilot-cqrs-guidelines.md](.github/copilot-cqrs-guidelines.md) - CQRS implementation patterns
- [copilot-solid-guidelines.md](.github/copilot-solid-guidelines.md) - SOLID principles in C#
- [copilot-logging-guidelines.md](.github/copilot-logging-guidelines.md) - Structured logging standards
- [copilot-commit-instructions.md](.github/copilot-commit-instructions.md) - Conventional commit format

## Notes for Claude

- This is a **library/framework** project - changes affect downstream consumers
- Maintain **backwards compatibility** unless explicitly breaking
- All public APIs should have **XML documentation**
- Run tests before considering work complete
- When adding features, follow existing patterns in the codebase

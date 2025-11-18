# GitHub Copilot Instructions

## Architecture Overview

This repository implements a modern .NET8 backend application following **Clean Architecture** principles with strict separation of concerns:

### Layer Responsibilities
- **Domain**: Core business entities, value objects, domain events, domain exceptions, and repository interfaces (zero external dependencies)
- **Application**: Business logic orchestration using CQRS patterns (commands, queries, events, vertical slices) and abstractions for external services
- **Infrastructure**: Concrete implementations of external services (database access, messaging, email, file storage, third-party integrations)
- **Presentation**: Web API layer containing controllers, middleware, DTOs, validation, and application entry points

## Technology Stack & Standards

### Primary Technologies
- **.NET8** with **C#13** language features
- **ASP.NET Core** for web APIs
- **Entity Framework Core** for data access
- **XUnit** and **Reqnroll.NET** for testing
- **Structured logging** with `ILogger<T>`

### Architecture Patterns
- **CQRS** (Command Query Responsibility Segregation) for application logic organization
- **Domain-Driven Design** with rich domain models and aggregate roots
- **Vertical Slice Architecture** within the Application layer
- **Dependency Injection** throughout all layers
- **Repository Pattern** with Unit of Work for data access

## Coding Standards & Practices

- Always implement the CQRS, SOLID, CLEANCODE, Clean Architecture principles, Structured logging and remove/replace obsolete/Redundant/legacy code in a professional and robust manner. 
- Look first at the SharedKernel, Shared.Application, Application.Model and Domain projects before creating new implementations. 
- Don't expose publicly domain objects in the Application.Model. 
- Don't do inconsistent or simulated implementations, refactor for consistency! 
- Apply Conservative refactoring,so preserve all functionality. 
- Conduct comprehensive search for existing implementations, If none exist, create professional production-ready implementations(not simulations), Integrate into existing service registration patterns. But first analyze the whole solution.
- if there are uncertainties just ask for your suggestions to implement or not.

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

### Testing Strategy
- **Test-Driven Development (TDD)** and **Behavior-Driven Development (BDD)**
- **Unit tests** for all business logic with comprehensive coverage
- **Integration tests** for API endpoints and data access layers
- **Gherkin scenarios** for complex business workflows
- **Mocking/stubbing** for external dependencies

## Code Organization & Structure

### Project Layout
```
Solution/
├── Domain/ # Core business logic
├── Application/ # Use cases and application services
│   └── Features/ # Vertical slices by feature
│   └── [Entity]/
│       ├── Commands/
│       ├── Queries/
│       └── Events/
├── Infrastructure/ # External service implementations
└── Presentation/ # Web API and UI concerns
```

### Naming Conventions
- **Commands**: End with `Command` (e.g., `CreateUserCommand`)
- **Queries**: End with `Query` (e.g., `GetUserByIdQuery`)
- **Handlers**: End with `Handler` (e.g., `CreateUserCommandHandler`)
- **DTOs**: End with `Dto` or descriptive suffix (e.g., `UserResponseDto`)
- **Interfaces**: Start with `I` (e.g., `IUserRepository`)

## Framework Restrictions & Preferences

### Prohibited Libraries
- **MediatR**: Implement custom command/query dispatching
- **AutoMapper**: Use manual mapping with extension methods
- **SpecFlow**: Use Reqnroll.NET for BDD scenarios

### Custom Implementations
- **Command/Query Bus**: Manual implementation in shared kernel
- **Object Mapping**: Explicit mapping methods with validation
- **Domain Events**: Custom event publishing mechanism

## Code Generation Requirements

### Output Standards
- **Complete implementations**: Generate entire classes/methods, not fragments
- **Compilable code**: All generated code must build without errors
- **Layer placement**: Clearly indicate target layer and file location
- **Documentation**: Include XML documentation for public APIs
- **Testing**: Provide unit test examples where applicable

### Quality Assurance
- Follow established patterns from the existing codebase
- Maintain consistency with naming conventions and coding standards
- Include appropriate error handling and logging
- Implement proper validation and guard clauses
- Consider performance implications and resource management

## Reference Guidelines

For detailed implementation guidance, refer to:

- **[Clean Code Guidelines](copilot-cleancode-guidelines.md)**: Formatting, naming, and code quality standards
- **[CQRS Guidelines](copilot-cqrs-guidelines.md)**: Command/Query pattern implementation
- **[SOLID Guidelines](copilot-solid-guidelines.md)**: SOLID principles application in C#
- **[LOGGING Guidelines](copilot-logging-guidelines.md)**: Structured logging in C#

## Code Review Criteria

Generated code will be evaluated against:
- **Architectural alignment** with Clean Architecture principles
- **SOLID compliance** and separation of concerns
- **Testability** and maintainability
- **Performance** and resource efficiency
- **Security** considerations and best practices
- **Documentation** completeness and clarity
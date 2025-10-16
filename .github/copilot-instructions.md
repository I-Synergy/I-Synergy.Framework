# Copilot Instructions

## Solution Structure & Architecture

- This repository is a modern .NET backend application, structured into **Domain**, **Application**, **Infrastructure**, and **Presentation** layers.
- Follow **clean architecture**, ensuring **separation of concerns**:  
  - **Domain** = core business entities, value objects, domain events, domain exceptions, repository interfaces (no external dependencies).
  - **Application** = orchestrates business logic (CQRS: commands, queries, events, vertical slices). Defines abstractions for external services (implemented in Infrastructure).
  - **Infrastructure** = implements external services (database, messaging, email, persistence).
  - **Presentation** = Web API layer; includes controllers, middleware, view models, entry point.

## Coding Guidelines

- **Languages**: C#, TypeScript, LINQ, Aurora SQL (focus mostly on C#).
- **Use**:  
  - **CQRS** for organizing application logic (commands/queries/events).
  - **Dependency Injection** throughout.
  - **Result<T>** and **Option<T>** types for method returns when applicable for errors/nulls.
  - **Functional programming** patterns where practical (immutability, expression-bodied members, pure functions).
  - **Structured logging** and robust **error handling**.
  - **Async/await** best practices. Avoid `async void` except for event handlers.
  - **Domain-Driven Design**: Rich domain models with behaviors, not anemic DTOs.
  - **Guard clauses** for all public methods (input validation).
  - **No magic numbers/strings** – use named constants.
  - **Resilience**: Add resilience patterns (retries, circuit breakers) for external calls.
  - Write guard clauses, informational, trace logging, in all classes and operations, preferable with `ILogger<T>`.
  - **Testability**: Write code with unit and integration testing in mind.  
    - Prefer test-driven development (TDD) and behavior-driven design (BDD) practices.
    - Use `XUnit` and `reqnroll.net` for testing and Gherkin for scenario definitions.
   - Continue to implement the CQRS, SOLID, CLEANCODE principles, Structured logging and remove/replace obsolete/legacy code in a profesional and robust manner.

## Patterns & Restrictions

- No `MediatR`, `Automapper`, or `Specflow`; implement messaging and mapping logic manually in a shared project.
- Organize code into use-case vertical slices inside the Application layer (`[EntityName]/Commands`, `Queries`, etc.).
- Follow **SOLID principles**, clean code practices, and avoid code smells.
- Ensure all code samples are **compilable** and fit in the proper application layer.
- Use **modern C# features** (records, pattern matching, etc.) where appropriate.

## Output Requirements

- When generating code, include the entire class or method.
- Clearly indicate the layer and location if introducing new files.
- Add comments for context when needed.
- Prefer maintainable, readable code over brevity.

voor guidelines:

- [clean code](copilot-cleancode-guidelines.md)
- [cqrs](copilot-cqrs-guidelines.md)
- [solid](copilot-solid-guidelines.md)
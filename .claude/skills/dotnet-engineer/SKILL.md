---
name: dotnet-engineer
description: .NET/C#/Blazor/MAUI development expert. Use for implementing CQRS handlers, API endpoints, Blazor components, MAUI apps, or any .NET code development tasks.
---

# .NET Engineer Skill

Specialized agent for .NET, C#, Blazor, and MAUI development.
Reference implementation: `ISynergy.Domain.Budgets`

## Expertise Areas

- .NET 10+ development
- C# 14 language features
- Blazor Server & Blazor Hybrid (MAUI)
- CQRS architecture implementation
- Entity Framework Core 10
- Minimal APIs
- Dependency Injection
- Async/await patterns

## Responsibilities

1. **Implement CQRS Handlers**
   - Create Command/Query/Handler/Response classes (each in own file, subfolder per operation)
   - Handler naming: `Create{Entity}CommandHandler`, `Get{Entity}ByIdQueryHandler`
   - Add structured logging
   - Map entities to Models via Mapster

2. **Build API Endpoints**
   - Create Minimal API endpoints
   - Configure routing and versioning
   - Implement proper HTTP status codes
   - Add OpenAPI documentation

3. **Data Access**
   - Use EF Core primitives: `FindAsync`, `Add`, `Update`, `Remove`, `SaveChangesAsync`
   - Use `ProjectToType<T>()` for list queries
   - Write efficient LINQ queries
   - Prevent N+1 query problems
   - Implement proper async patterns

4. **Code Quality**
   - Follow SOLID principles
   - Use positional records for Models and Responses
   - Write clear, self-documenting code

## Key Rules to Enforce

- Commands use individual parameters (NOT model objects)
- Delete operations use `FindAsync` + `Remove` + `SaveChangesAsync`
- Queries use named parameters for optional filters
- Never expose domain entities directly (responses wrap Models)
- Always include `CancellationToken` in async methods
- Use Mapster for Entity → Model mapping (NOT AutoMapper)
- No repository interfaces (use DataContext directly)
- Create handlers construct entities directly (NOT `command.Adapt<Entity>()`)
- Models are positional records without "Model" suffix (e.g., `Budget`, not `BudgetModel`)
- One `Configuration` class per domain in `Mappers/` (NOT per-entity mapping configs)

## Templates to Use

- [`command-handler.cs.txt`](../../reference/templates/command-handler.cs.txt)
- [`query-handler.cs.txt`](../../reference/templates/query-handler.cs.txt)
- [`mapping-config.cs.txt`](../../reference/templates/mapping-config.cs.txt)
- [`endpoint.cs.txt`](../../reference/templates/endpoint.cs.txt)

## Patterns to Follow

- [`cqrs-patterns.md`](../../patterns/cqrs-patterns.md)
- [`api-patterns.md`](../../patterns/api-patterns.md)
- [`critical-rules.md`](../../reference/critical-rules.md)

## Checklist Before Completion

- [ ] Structured logging throughout
- [ ] Async all the way (no .Wait() or .Result)
- [ ] Responses wrap Models (not entities)
- [ ] Mapster configuration registered in `Mappers/Configuration.cs`
- [ ] Proper error handling with `InvalidOperationException`
- [ ] XML documentation on public APIs
- [ ] Each type in own file, each operation in own subfolder
- [ ] Code builds with 0 errors, 0 warnings

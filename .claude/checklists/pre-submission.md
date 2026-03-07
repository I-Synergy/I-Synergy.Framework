# Pre-Submission Checklist

Use this checklist before considering any task complete.

## Production Completeness (MANDATORY SELF-EVALUATION)

This is not a demo or proof of concept. Before saying "done", actively grep the changed scope and verify:

- [ ] No `// TODO`, `// FIXME`, `// HACK`, `// mock`, `// stub`, or `// placeholder` comments remain
- [ ] No hardcoded lists, arrays, or data that should come from the API or database
- [ ] No `Task.FromResult(...)` or `Task.CompletedTask` substituting real async operations
- [ ] No `throw new NotImplementedException()` or silent no-op method bodies
- [ ] All UI data is bound to ViewModel properties backed by real Kiota API calls
- [ ] All write operations (create, update, delete) call real API endpoints
- [ ] No services injected but unused (data loaded from a static source instead)

### UI Completeness (every element must work as intended)

- [ ] Every **window and dialog** opens, closes, and displays correct data
- [ ] Every **button** is wired to a real ViewModel command — no unbound, disabled-by-default, or no-op buttons
- [ ] Every **form field** reads from and writes back to the ViewModel and persists via the API on save
- [ ] Every **list, grid, or table** loads real data and reflects create/update/delete operations without requiring a full page reload
- [ ] Every **navigation item** routes to a working page
- [ ] Every **validation message** fires correctly on invalid input
- [ ] Empty states, loading states (`IsBusy`), and error states are handled and visible to the user
- [ ] No UI component is left in a permanently disabled, hidden, or placeholder state unless that is the explicit design intent

## Agent Workflow (CRITICAL)

- [ ] Task was delegated to agent(s) with full repository access
- [ ] **EVERY agent created** (main, sub, helper, parallel - ALL) had full repository access
- [ ] Progress file created immediately at task start in [`.claude/progress/`](../progress/)
- [ ] Progress file updated in real-time throughout task by ALL agents
- [ ] Real-time progress reported automatically by ALL agents
- [ ] Progress file moved to [`.claude/completed/`](../completed/) on completion
- [ ] Session context updated with learnings before ending

## Domain Model Integrity (CRITICAL)

- [ ] No new entity, model, or class was created without first searching the entire solution for an existing equivalent
- [ ] Existing shared entities are reused or extended rather than duplicated (e.g., `Customer`, `Person`, `Address`, `PhoneNumber`)
- [ ] No properties on the new type duplicate properties already available via a referenced entity
- [ ] The plan explicitly listed which existing entities/models are reused before any new ones were proposed
- [ ] Property names are unambiguous and reflect exact business meaning

## Architecture & Patterns

- [ ] Follows CQRS pattern (commands/queries separated)
- [ ] Follows DDD principles (entities, value objects, aggregates)
- [ ] Follows Clean Architecture (proper layer separation)
- [ ] Uses vertical slice organization (feature folders)
- [ ] No domain entities exposed directly (always use DTOs/Models)

## Code Quality

- [ ] SOLID principles applied
- [ ] Code is immutable where possible (records, init, readonly)
- [ ] Intention-revealing names used
- [ ] No null reference warnings
- [ ] Expression-bodied members used appropriately
- [ ] Pattern matching utilized where beneficial

## CQRS Implementation

- [ ] Commands use individual parameters (NOT model objects)
- [ ] Queries use named parameters for optional filters
- [ ] Handlers inject DataContext directly (no repository layer)
- [ ] Proper command/query naming conventions followed
- [ ] Response DTOs defined and used

## Data Access

- [ ] Uses EF Core primitives directly on named DbSet properties (`FirstOrDefaultAsync`, `Add`, `Remove`, `SaveChangesAsync`) — NO extension methods like `AddItemAsync`/`GetItemByIdAsync`/`RemoveItemAsync`
- [ ] Delete operations use `FirstOrDefaultAsync` + `Remove` + `SaveChangesAsync` (NOT `RemoveItemAsync` or `DeleteItemByIdAsync`)
- [ ] No N+1 query problems (proper Include usage)
- [ ] Async all the way (no .Wait() or .Result)
- [ ] CancellationToken passed through all layers

## Mapping (Mapster)

- [ ] Mapping configuration in `{Entity}MappingConfig.cs`
- [ ] Configuration registered in ServiceCollectionExtensions
- [ ] Uses .Adapt<T>() extension method
- [ ] No AutoMapper used
- [ ] No manual mapping used

## Error Handling & Logging

- [ ] Guard clauses on all public methods
- [ ] ArgumentNullException.ThrowIfNull used
- [ ] Structured logging templates (no string interpolation)
- [ ] Appropriate log levels used
- [ ] No PII in logs
- [ ] No secrets in logs
- [ ] Correlation IDs included where appropriate

## Security

- [ ] Input validation with Data Annotations
- [ ] IValidatableObject implemented for complex validation
- [ ] Authorization checks at endpoints
- [ ] No secrets hard-coded
- [ ] No internal implementation details leaked

## Testing

- [ ] Unit tests for all handlers (MSTest)
- [ ] Gherkin scenarios for complex flows (Reqnroll)
- [ ] Integration tests for endpoints
- [ ] Mocks used appropriately (Moq)
- [ ] Tests are idempotent
- [ ] Descriptive test names

## Service Registration

- [ ] Handlers registered in ServiceCollectionExtensions
- [ ] Mapster configurations scanned and applied
- [ ] Extension method follows pattern: With{Domain}DomainHandlers

## Endpoints

- [ ] Minimal API pattern used
- [ ] Proper HTTP methods (POST/GET/PUT/DELETE)
- [ ] Route conventions followed (/{domain}/{id})
- [ ] Status codes correct (200, 201, 204, 400, 404)
- [ ] OpenAPI/Swagger annotations present
- [ ] Authorization required where appropriate

## Forbidden Technologies

- [ ] NO MediatR (using CQRS framework)
- [ ] NO AutoMapper (using Mapster)
- [ ] NO xUnit/NUnit (using MSTest)
- [ ] NO Swashbuckle (using Microsoft.AspNetCore.OpenApi)
- [ ] NO FluentValidation (using Data Annotations)
- [ ] NO standalone Polly (using Microsoft.Extensions.Resilience)

## Documentation

- [ ] XML documentation on public APIs
- [ ] Summary tags on commands/queries
- [ ] Parameter descriptions on records
- [ ] File paths indicated in comments
- [ ] No TODO or placeholder comments

## Build & Deploy

- [ ] Solution builds with 0 errors
- [ ] Solution builds with 0 warnings
- [ ] All tests pass
- [ ] No compilation warnings suppressed inappropriately
- [ ] NuGet package versions verified on nuget.org

## Progress Tracking

- [ ] Progress file in correct location ([`.claude/progress/`](../progress/))
- [ ] All files created/modified documented
- [ ] Issues/blockers documented
- [ ] On completion, moved to [`.claude/completed/`](../completed/)

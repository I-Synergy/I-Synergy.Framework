# Pre-Submission Checklist

Use this checklist before considering any task complete.

## Agent Workflow (CRITICAL)

- [ ] Task was delegated to agent(s) with full repository access
- [ ] **EVERY agent created** (main, sub, helper, parallel - ALL) had full repository access
- [ ] Progress file created immediately at task start in `.ai/progress/`
- [ ] Progress file updated in real-time throughout task by ALL agents
- [ ] Real-time progress reported automatically by ALL agents
- [ ] Progress file moved to `.ai/completed/` on completion
- [ ] Session context updated with learnings before ending

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

- [ ] Uses EF Core primitives on named DbSet properties (`FirstOrDefaultAsync`, `Add`, `Remove`, `SaveChangesAsync`)
- [ ] No extension methods (`AddItemAsync`, `GetItemByIdAsync`, `RemoveItemAsync`, etc.)
- [ ] Delete operations use `FirstOrDefaultAsync` + `Remove` + `SaveChangesAsync` (check `rowsAffected > 0`)
- [ ] Update operations do NOT call `.Update()` on tracked entities (change tracker handles it)
- [ ] No N+1 query problems (proper Include usage)
- [ ] Async all the way (no .Wait() or .Result)
- [ ] CancellationToken passed through all layers

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
- [ ] NO xUnit/NUnit (using MSTest)
- [ ] NO Swashbuckle (using Microsoft.AspNetCore.OpenApi)
- [ ] NO FluentValidation (using Data Annotations)
- [ ] NO standalone Polly (using Microsoft.Extensions.Resilience)

## README Maintenance (Hard Requirement)

- [ ] If any files/directories were added, removed, or renamed in `.ai/` — README.md File Structure updated
- [ ] If any skills were added or removed — README.md Skills table updated
- [ ] If any patterns were added or removed — README.md Pattern Guides table updated
- [ ] If any root-level files changed — README.md Documentation table updated
- [ ] If no structural changes occurred — explicitly confirmed README.md requires no update

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

- [ ] Progress file in correct location (`.ai/progress/`)
- [ ] All files created/modified documented
- [ ] Issues/blockers documented
- [ ] On completion, moved to `.ai/completed/`

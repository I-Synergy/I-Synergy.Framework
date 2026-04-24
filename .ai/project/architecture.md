# Project Architecture (CUSTOMIZE THIS)

**Instructions:** Document your project's architecture decisions and patterns.

**Purpose:** This file defines HOW your system is structured - layers, patterns, data flow, and architectural decisions.

## Architecture Style

**Primary Architecture:** [Clean Architecture / Hexagonal / Onion / Layered / Custom]

**Why Chosen:** [Reasoning for this architecture choice]

## Architectural Patterns

| Pattern | Used? | Notes |
|---------|-------|-------|
| **Clean Architecture** | [Yes / No / Partial] | [How applied] |
| **CQRS** | [Yes / No / Partial] | [Command/Query separation level] |
| **DDD** | [Yes / No / Partial] | [Aggregates, entities, value objects] |
| **Vertical Slices** | [Yes / No / Partial] | [Feature folders organization] |
| **Event Sourcing** | [Yes / No] | [If yes, which domain] |
| **Microservices** | [Yes / No] | [If yes, service boundaries] |

## Clean Architecture Layers

**Layer definitions and dependencies:**

```
[Document your actual layer dependencies]

Example:
Presentation Layer (UI, API Controllers)
    ↓ depends on
Application Layer (Use Cases, CQRS Handlers)
    ↓ depends on
Domain Layer (Entities, Business Rules, Interfaces)
    ↑ implements
Infrastructure Layer (Database, External Services)
```

**Layer mapping to projects:**

- **Domain:** `{ApplicationName}.Domain.*` - Entities, value objects, domain events, CQRS definitions
- **Application:** `{ApplicationName}.Services.*` - API endpoints, orchestration, DTOs
- **Infrastructure:** `{ApplicationName}.Data.*` - Persistence, external integrations
- **Presentation:** `{ApplicationName}.UI.*` - Blazor/MAUI apps, ViewModels

## Project Structure

```
[Document your actual project structure]

Example:
solution/
├── src/
│   ├── {ApplicationName}.Contracts.{Domain}/      # Interfaces, service contracts
│   ├── {ApplicationName}.Entities.{Domain}/       # EF Core entity classes
│   ├── {ApplicationName}.Models.{Domain}/         # DTOs, view models
│   ├── {ApplicationName}.Domain.{Domain}/         # CQRS handlers, domain logic
│   └── {ApplicationName}.Services.{Domain}/       # API endpoints
├── tests/
│   └── {ApplicationName}.{Domain}.Tests/          # Unit and integration tests
└── docs/
```

## CQRS Implementation Pattern

**Command/Query structure:**

```csharp
// Commands use individual parameters (NOT model objects)
public sealed record Create{Entity}Command(
    string Property1,
    decimal Property2
) : ICommand<Create{Entity}Response>;

// Queries use named parameters
public sealed record Get{Entity}ByIdQuery(Guid {Entity}Id)
    : IQuery<{Entity}Response>;

// Handlers inject DataContext directly
public sealed class Create{Entity}Handler(DataContext dataContext)
    : ICommandHandler<Create{Entity}Command, Create{Entity}Response>
{
    public async Task<Create{Entity}Response> HandleAsync(
        Create{Entity}Command command,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

## Data Access Pattern

**NO explicit Repository interfaces** - Use EF Core extension methods:

```csharp
// From I-Synergy.Framework.EntityFramework (or your ORM extensions)
await dataContext.AddItemAsync<TEntity, TModel>(model, cancellationToken);
await dataContext.GetItemByIdAsync<TEntity, TModel, TKey>(id, cancellationToken);
await dataContext.UpdateItemAsync<TEntity, TModel>(model, cancellationToken);
await dataContext.RemoveItemAsync<TEntity, TKey>(id, cancellationToken);
```

## Vertical Slice Organization

```
Domain/Features/{Entity}/
  Commands/
    Create{Entity}Command.cs
    Create{Entity}Handler.cs
    Update{Entity}Command.cs
    Update{Entity}Handler.cs
    Delete{Entity}Command.cs
    Delete{Entity}Handler.cs
  Queries/
    Get{Entity}ByIdQuery.cs
    Get{Entity}ByIdHandler.cs
    Get{Entity}ListQuery.cs
    Get{Entity}ListHandler.cs
  Events/
    {Entity}CreatedEvent.cs
    {Entity}UpdatedEvent.cs
    {Entity}DeletedEvent.cs
Extensions/ServiceCollectionExtensions.cs
```

## Data Flow

### Request Flow

```
[Document your request/response flow]

Example:
HTTP Request
  → API Endpoint
  → Command/Query
  → Handler
  → DataContext
  → Database
  ← Response
```

### Event Flow (if using domain events)

```
[Document event flow]

Example:
Command
  → Handler
  → Entity State Change
  → Domain Event Raised
  → Event Handler(s)
  → Side Effects
```

**Service registration:**

```csharp
// Domain Extensions/ServiceCollectionExtensions.cs
public static IServiceCollection With{Domain}DomainHandlers(
    this IServiceCollection services)
{
    var assembly = typeof(ServiceCollectionExtensions).Assembly;
    services.AddCQRS().AddHandlers(assembly);
    return services;
}
```

## Domain Events

**Event definitions:**

```csharp
// Domain/Features/{Entity}/Events/
public sealed record {Entity}CreatedEvent(
    Guid {Entity}Id,
    string Property1,
    DateTimeOffset CreatedDate
) : IDomainEvent;
```

**Event handling approach:**
- Raise only meaningful state changes
- Dispatch via [your event infrastructure]
- Handlers must be idempotent
- Handlers must be resilient (retry transient failures)

## Integration Patterns

### External Service Integration

**Integration approach:**
- **Pattern:** [Adapter / Gateway / Direct]
- **Error Handling:** [Retry / Circuit Breaker / Fallback]
- **Authentication:** [API Key / OAuth2 / Certificate]

### Inter-Domain Communication

**How domains communicate:**
- **Same Service:** [Direct references / Domain events / Messaging]
- **Different Services:** [HTTP / gRPC / Messaging]

## Persistence Strategy

**How data is persisted:**

- **Primary Store:** [Database technology and ORM approach]
- **Caching:** [When to cache, what to cache, cache invalidation]
- **File Storage:** [Where and how files are stored] (if applicable)
- **Message Queue:** [Technology and usage] (if applicable)

**Example:**
- **Primary Store:** PostgreSQL via EF Core, code-first migrations
- **Caching:** Redis for reference data (countries, categories), 15-minute TTL
- **File Storage:** Azure Blob Storage for documents, private containers
- **Message Queue:** Azure Service Bus for cross-domain events

## Security Architecture

**Authentication & Authorization:**

- **Authentication:** [Where implemented - API Gateway / Each service / Middleware]
- **Authorization:** [Policy-based / Role-based / Claims-based]
- **Token handling:** [JWT validation, refresh token strategy]

**API Security:**

- **CORS:** [Configuration approach]
- **Rate Limiting:** [Strategy and limits]
- **Input Validation:** [Where validated - endpoints / commands / both]

**Secrets Management:**

- **Development:** [Local secrets, Key Vault emulator]
- **Production:** [Azure Key Vault / AWS Secrets Manager]

## Scalability Strategy

**How the system scales:**

- **Horizontal Scaling:** [Stateless design / Session handling approach]
- **Database Scaling:** [Read replicas / Sharding / Partitioning]
- **Caching Strategy:** [Distributed cache for shared data]
- **Async Processing:** [Background jobs / Message queue for long operations]

## Monitoring & Observability

**How the system is monitored:**

- **Logging:** [Structured logging with correlation IDs]
- **Metrics:** [What is measured - response times, error rates, etc.]
- **Tracing:** [Distributed tracing across services]
- **Health Checks:** [What is monitored - database, cache, external APIs]

## Deployment Architecture

**Deployment strategy:**

- **Environment Strategy:** [Dev / Staging / Production]
- **Deployment Method:** [Blue-Green / Rolling / Canary]
- **Infrastructure:** [Cloud / On-premises / Hybrid]
- **CI/CD:** [Pipeline approach and stages]

## Architectural Decision Records (ADRs)

### ADR Template

```markdown
## ADR-XXX: [Decision Title]

**Status:** [Proposed / Accepted / Deprecated / Superseded]
**Date:** [YYYY-MM-DD]
**Deciders:** [Who decided]

### Context
[What is the issue that we're seeing that is motivating this decision]

### Decision
[What is the change that we're proposing/have agreed to]

### Consequences
**Positive:**
- [e.g., easier to understand, better performance]

**Negative:**
- [e.g., increased complexity, more boilerplate]

### Alternatives Considered
- [Alternative 1] - [Why rejected]
- [Alternative 2] - [Why rejected]
```

### Example ADR

```markdown
## ADR-001: [Decision Name]

**Status:** [Proposed | Accepted | Deprecated | Superseded]
**Date:** [Date]
**Deciders:** [Team]

### Context
[What is the issue that motivates this decision?]

### Decision
[What is the change being proposed or accepted?]

### Consequences
**Positive:**
- [Positive outcome]

**Negative:**
- [Negative outcome or trade-off]

### Alternatives Considered
- [Alternative 1] - [Why rejected]
```

## Reference Implementations

**Which implementations serve as patterns:**

- **Primary Reference:** [{Domain}.{Entity}] - [Why it's the reference]
- **Alternative Patterns:** [{Domain}.{Entity}] - [What pattern it demonstrates]

**Example:**
- **Primary Reference:** [Budgets.Budget] - Complete CRUD implementation with all operations, comprehensive testing, full documentation
- **Alternative Patterns:** [Customers.Customer] - Demonstrates soft-delete pattern with audit fields

## Known Limitations

**Document known architectural limitations:**

1. [Limitation 1] - [Why it exists, when to address]
2. [Limitation 2] - [Impact and planned mitigation]

**Example:**
1. Single database instance - Acceptable for current scale (<100k users), plan to implement read replicas when load increases
2. Synchronous inter-domain communication - Acceptable for low latency requirements, consider event-driven architecture for high volume operations

## Future Architectural Considerations

**What might evolve:**

- [Consideration 1] - [Trigger for this change]
- [Consideration 2] - [Benefits and costs]

**Example:**
- Event sourcing for audit-heavy domains - Consider when audit requirements become more stringent
- Microservices architecture - Evaluate when team size exceeds 10 developers or domains become truly independent

---

**Remember:** This file documents your system's architecture. Update it when patterns evolve. For technology choices, see [tech-stack.md](tech-stack.md). For business domains, see [domains.md](domains.md).

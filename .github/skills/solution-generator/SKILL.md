---
name: solution-generator
description: Generates a complete .NET solution scaffold from an architecture document and design interrogation deliverables. Use after running design-interrogation to translate the architecture document, ubiquitous language, and use cases into a working solution structure with all projects, CQRS handlers, and entity configurations. Trigger whenever the user says "generate the solution", "scaffold the project", "create the solution structure", or wants to turn an architecture document into code.
---

# Solution Generator

Translates a completed architecture document into a .NET solution scaffold — projects, folder structure, domain entities, and CQRS handler stubs — following Clean Architecture and the patterns in this template.

## Inputs

| Input | Source | Role |
|-------|--------|------|
| Architecture document | `docs/architecture/{SolutionName}-architecture.md` | Primary input — all parameters derived from here |
| Ubiquitous Language | `UBIQUITOUS_LANGUAGE.md` | Domain naming, aggregate identification |
| Use Cases | `docs/bounded-contexts/{BC}/use-cases.md` | Each use case → CQRS Command or Query stub |
| User Stories | `docs/bounded-contexts/{BC}/user-stories.md` | Acceptance criteria → test stubs |
| Session Decisions | `docs/session-decisions.md` | Technology and pattern choices |

## Generation Steps

1. **Parse the architecture document** for:
   - Solution name
   - Bounded contexts and their aggregates
   - Technology stack choices (ORM, auth, caching, etc.)
   - Architectural patterns (event sourcing, CQRS, etc.)
   - Integration points and external services

2. **Map aggregates to entities** — one EF Core entity class per aggregate root per bounded context

3. **Map use cases to CQRS** — each use case main flow becomes a Command or Query + Handler stub:
   - Create/Update/Delete flows → Commands
   - Read flows → Queries

4. **Generate the solution structure** (see below)

5. **Generate entity configurations** — `IEntityTypeConfiguration<T>` per entity

6. **Generate service registration** — `Extensions/ServiceCollectionExtensions.cs` per domain project

7. **Generate test stubs** — one test class per handler, stubs traced to user story acceptance criteria

## Solution Structure

Generate this layout for each bounded context, following CLAUDE.md reference architecture:

```
src/
├── {Solution}.Contracts.{BC}/
├── {Solution}.Entities.{BC}/
├── {Solution}.Models.{BC}/
├── {Solution}.Domain.{BC}/
│   ├── Features/
│   │   └── {Entity}/
│   │       ├── Commands/
│   │       │   ├── Create{Entity}/
│   │       │   │   ├── Create{Entity}Command.cs
│   │       │   │   ├── Create{Entity}CommandHandler.cs
│   │       │   │   └── Create{Entity}Response.cs
│   │       │   ├── Update{Entity}/
│   │       │   └── Delete{Entity}/
│   │       ├── Queries/
│   │       │   ├── Get{Entity}ById/
│   │       │   └── Get{Entities}List/
│   │       └── Events/
│   ├── Models/
│   │   └── {Entity}.cs
│   └── Extensions/
│       └── ServiceCollectionExtensions.cs
└── {Solution}.Services.{BC}/
tests/
└── {Solution}.{BC}.Tests/
```

## CQRS Stubs

Follow the patterns in:
- `.ai/reference/templates/command-handler.cs.txt`
- `.ai/reference/templates/query-handler.cs.txt`

Key rules (from CLAUDE.md Critical Coding Rules):
- Commands use individual parameters, not model objects
- Handlers use EF Core primitives only (no repositories, no extension methods)
- All mapping is manual — use `/* map entity to {Entity} model */` placeholders
- Always include `CancellationToken` on all async methods
- Responses wrap model types — never return domain entities directly

## Service Registration Stub

```csharp
// Extensions/ServiceCollectionExtensions.cs
public static IServiceCollection With{BC}DomainHandlers(
    this IServiceCollection services)
{
    var assembly = typeof(ServiceCollectionExtensions).Assembly;
    services.AddCQRS().AddHandlers(assembly);
    return services;
}
```

## After Generation

Run `gap-review` to validate the generated solution against the original architecture document and session decisions.

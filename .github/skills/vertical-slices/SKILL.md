---
name: vertical-slices
description: Translates use cases and user stories into vertical slice blueprint JSON for feature-by-feature implementation. Use after solution-generator to plan each feature as a self-contained vertical slice spanning all layers. Trigger whenever the user says "vertical slice", "feature blueprint", "implement feature by feature", or wants to break down user stories into implementable slices.
---

# Vertical Slices

Translates each use case and user story into a self-contained vertical slice — a complete feature implementation spanning entity, model, domain handler, API endpoint, and test.

## What Is a Vertical Slice?

A vertical slice is one feature, fully implemented across all layers. Each slice maps to exactly one Command or Query, delivers shippable value, and has no shared mutable state with other slices.

## Input

| Input | Source |
|-------|--------|
| Use Cases | `docs/bounded-contexts/{BC}/use-cases.md` |
| User Stories | `docs/bounded-contexts/{BC}/user-stories.md` |
| Ubiquitous Language | `UBIQUITOUS_LANGUAGE.md` |
| Architecture Document | `docs/architecture/{SolutionName}-architecture.md` |

## Output

For each feature:
1. `docs/slices/{BC}/{Entity}.{Operation}/blueprint.json` — implementation blueprint
2. `docs/slices/{BC}/{Entity}.{Operation}/{Entity}.{Operation}.feature` — Gherkin feature file

## Blueprint JSON Format

```json
{
  "slice": "{Entity}.{Operation}",
  "bounded_context": "{BC}",
  "operation_type": "Command | Query",
  "entity": "{Entity}",
  "handler_class": "{Operation}{Entity}CommandHandler",
  "inputs": [
    { "name": "{PropertyName}", "type": "{CSharpType}", "required": true }
  ],
  "outputs": [
    { "name": "{PropertyName}", "type": "{CSharpType}" }
  ],
  "acceptance_criteria": [
    "Given ... When ... Then ..."
  ],
  "files_to_create": [
    "{Solution}.Domain.{BC}/Features/{Entity}/Commands/{Operation}{Entity}/{Operation}{Entity}Command.cs",
    "{Solution}.Domain.{BC}/Features/{Entity}/Commands/{Operation}{Entity}/{Operation}{Entity}CommandHandler.cs",
    "{Solution}.Domain.{BC}/Features/{Entity}/Commands/{Operation}{Entity}/{Operation}{Entity}Response.cs",
    "{Solution}.Services.{BC}/Endpoints/{Entity}/{Operation}{Entity}Endpoint.cs",
    "{Solution}.{BC}.Tests/{Entity}/{Operation}{Entity}HandlerTests.cs"
  ],
  "source": {
    "use_case": "UC-{N}",
    "user_story": "US-{N}"
  }
}
```

## Steps

1. Read all use cases and user stories for each bounded context
2. Map each use case main flow to one Command or Query
3. For each alternate flow that introduces a distinct outcome, create a separate slice
4. For each user story, verify it maps to at least one slice — create a slice if missing
5. Write the blueprint JSON for each slice to `docs/slices/{BC}/{Entity}.{Operation}/`
6. Extract the Gherkin from the use case/story and write the `.feature` file
7. Announce: "N vertical slice blueprints written across M bounded contexts."

## Slice Naming Convention

| Operation | Handler Suffix | Example |
|-----------|---------------|---------|
| Create | `CommandHandler` | `CreateBudgetCommandHandler` |
| Update | `CommandHandler` | `UpdateBudgetCommandHandler` |
| Delete | `CommandHandler` | `DeleteBudgetCommandHandler` |
| Get by ID | `QueryHandler` | `GetBudgetByIdQueryHandler` |
| Get list | `QueryHandler` | `GetBudgetsListQueryHandler` |

## Integration with Solution Generator

Blueprints produced by this skill are consumed by implementation agents. Each blueprint is a standalone work item — an agent can pick up a single blueprint and implement the full slice without reading any other blueprint.

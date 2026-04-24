# GitHub Copilot Instructions

You are assisting with a .NET project following Clean Architecture, CQRS, and DDD patterns.

## Project Context

See `.ai/project/architecture.md` for complete architecture documentation and `.ai/project/tech-stack.md` for the full technology stack.

## Critical Rules

See `.ai/reference/critical-rules.md` for complete rules with examples. Violations cause bugs.

**Key rules:**
- Commands use individual parameters (NOT model objects passed directly)
- Data access: EF Core primitives on named DbSet properties only — `FirstOrDefaultAsync`, `Add`, `Remove`, `SaveChangesAsync`
- Delete: `FirstOrDefaultAsync` + `Remove` + check `rowsAffected > 0` — no extension methods
- Update: `FirstOrDefaultAsync` + mutate properties + `SaveChangesAsync` — NO `.Update()` call (change tracker handles it)
- Mapping: not prescribed — implement per project; never return domain entities directly
- Async: always `CancellationToken` — NO `.Wait()` or `.Result`
- Return types: responses wrap models — never return domain entities directly
- Handler naming: `Create{Entity}CommandHandler` / `Get{Entity}ByIdQueryHandler` (must end in `CommandHandler` or `QueryHandler`)
- Entity construction: direct `new Entity { ... }` in handlers — NOT via any mapping library
- Enum naming: plural names (`PaymentProviders`, not `PaymentProvider`) except `*Status` enums

## Architecture

**Clean Architecture Layers:**
- Domain: `{ApplicationName}.Domain.*` — CQRS handlers, domain logic
- Application: `{ApplicationName}.Services.*` — API endpoints
- Infrastructure: `{ApplicationName}.Data.*` — EF Core persistence
- Presentation: `{ApplicationName}.UI.*` — Blazor/MAUI

**Vertical Slice Organization:**
```
{ApplicationName}.Domain.{Domain}/
  Features/{Entity}/
    Commands/Create{Entity}/
      Create{Entity}Command.cs
      Create{Entity}CommandHandler.cs
      Create{Entity}Response.cs
    Queries/Get{Entity}ById/
      ...
  Models/{Entity}.cs             (positional record, no "Model" suffix)
  Extensions/ServiceCollectionExtensions.cs
```

## Forbidden Technologies

See `.ai/reference/forbidden-tech.md` for the complete list.

- NO MediatR — use I-Synergy.Framework.CQRS
- NO xUnit/NUnit — use MSTest
- NO FluentValidation — use DataAnnotations
- NO repository interfaces — use EF Core directly via named DbSet properties
- NO `.Update()` on tracked EF entities — change tracker handles it

## Patterns & Templates

- CQRS: `.ai/patterns/cqrs-patterns.md`
- API: `.ai/patterns/api-patterns.md`
- Testing: `.ai/patterns/testing-patterns.md`
- Templates: `.ai/reference/templates/` — `command-handler.cs.txt`, `query-handler.cs.txt`, `endpoint.cs.txt`

## Naming Conventions

- Commands: `{Action}{Entity}Command` — e.g. `CreateBudgetCommand`
- Queries: `Get{Entity}{Criteria}Query` — e.g. `GetBudgetByIdQuery`
- Command handlers: `{Action}{Entity}CommandHandler` — e.g. `CreateBudgetCommandHandler`
- Query handlers: `Get{Entity}{Criteria}QueryHandler` — e.g. `GetBudgetByIdQueryHandler`
- Models: no "Model" suffix — `Budget` not `BudgetModel`

## Token Replacements

See `.ai/reference/tokens.md` for complete definitions.

- `{ApplicationName}` — your application name
- `{Domain}` — domain/bounded context
- `{Entity}` — entity name (PascalCase)

## Core Operational Rules

1. Read session context first: `.ai/session-context.md` (never mix project contexts)
2. Load context on demand from `.ai/` based on work type only
3. Mark open questions OPEN or ASSUMED, never resolve silently
4. Before session end: write structured handoff to `.ai/session-context.md`
5. Verify against `.ai/checklists/pre-submission.md` before completion

## Task Execution Protocol

**On every non-trivial task (3+ steps or multi-file):**

1. **Write a plan file first** — create `.ai/plans/{task-slug}.md` with a meaningful name before writing any code:
   ```markdown
   # Plan: {Task Name}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
2. **Create a progress file** — write `.ai/progress/{task-slug}.md` (same slug as the plan):
   ```markdown
   # {Task Name}
   Status: IN PROGRESS
   Started: {date}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
3. **Update progress file** after each step — change `- [ ]` to `- [x]`, never rewrite the whole file
4. **On completion (mandatory):**
   - Edit the progress file: add `Status: DONE` near the top
   - Move: `mv .ai/progress/{task-slug}.md .ai/completed/`

**Trivial tasks** (single file, obvious fix): skip plan and progress file.

### ReAct Loop — Observe, Reason, Fix, Repeat

Every action in a task follows this cycle. Do not mark a step done until it passes observation.

```
ACT → OBSERVE → pass? → mark step done, next step
                fail? → REASON → FIX → OBSERVE again
```

**Observe** immediately after every file edit (run manually — no automatic hook):

| Change type | What to observe |
|-------------|----------------|
| Any code edit | `dotnet build --nologo --verbosity quiet` |
| New or changed handler/query | Build + `dotnet test` on affected project |
| Test file | `dotnet test` on the test project |
| Skill or pattern file | `python3 .ai/tests/validate-skills.py` |
| Settings / config | `python3 .ai/tests/validate-settings.py` |
| Multi-file change | Build + `bash .ai/tests/run-all-tests.sh` |

**Reason** before every fix attempt:
- Read the full error, not just the first line
- Identify root cause (not symptom)
- State the fix strategy explicitly before applying it
- If retrying, use a different approach — never repeat the same fix

**Exit conditions:**

| State | Action |
|-------|--------|
| Observation passes | Mark step done, proceed |
| Retry 1–2, different error | Reason + new fix + observe again |
| Same error appears twice | Change approach before attempting retry 3 |
| Retry 3, still failing | **Escalate** — stop and report to user |

**Escalation format** (after 3 failed retries):
```
ESCALATION: {step name}

Tried:
1. {fix attempted} → {error received}
2. {fix attempted} → {error received}
3. {fix attempted} → {error received}

Root cause hypothesis: {what I believe is wrong}
Options:
  A. {option}
  B. {option}

Which approach should I take?
```

## Work-Type Context Mapping

Load these files based on task type:

| Task Type | Files to Load |
|-----------|--------------|
| .NET / CQRS | `.ai/skills/dotnet-engineer/SKILL.md`, `.ai/patterns/cqrs-patterns.md`, `.ai/reference/critical-rules.md` |
| API Endpoints | `.ai/patterns/api-patterns.md`, `.ai/reference/templates/endpoint.cs.txt` |
| Unit Tests | `.ai/skills/unit-tester/SKILL.md`, `.ai/patterns/testing-patterns.md`, `.ai/reference/templates/test-class.cs.txt` |
| UI/E2E Tests | `.ai/skills/playwright-tester/SKILL.md`, `.ai/patterns/testing-patterns.md` |
| Blazor UI | `.ai/skills/blazor-specialist/SKILL.md`, `.ai/patterns/mvvm.md` |
| MAUI | `.ai/skills/maui-specialist/SKILL.md`, `.ai/patterns/mvvm.md` |
| Architecture | `.ai/skills/architect/SKILL.md`, `.ai/project/architecture.md` |
| Code Review | `.ai/skills/code-reviewer/SKILL.md`, `.ai/checklists/pre-submission.md` |
| Security | `.ai/skills/security/SKILL.md`, `.ai/skills/api-security/SKILL.md` |
| Performance | `.ai/skills/performance-engineer/SKILL.md` |
| Database | `.ai/skills/database-migration/SKILL.md` |
| DevOps | `.ai/skills/devops-engineer/SKILL.md` |
| Documentation | `.ai/skills/technical-writer/SKILL.md` |
| Bulk Refactoring | `.ai/skills/refactor/SKILL.md` |
| Design Interrogation | `.ai/skills/design-interrogation/SKILL.md` |
| Solution Scaffolding | `.ai/skills/solution-generator/SKILL.md`, `.ai/skills/vertical-slices/SKILL.md` |
| Gap Validation | `.ai/skills/gap-review/SKILL.md` |
| Domain Modeling | `.ai/skills/ubiquitous-language/SKILL.md`, `.ai/skills/usecase-specification/SKILL.md`, `.ai/skills/user-story/SKILL.md` |
| Skill Creation | `.ai/skills/skill-creator/SKILL.md` |

## Session Management

Every session:
1. **Start** — read `.ai/session-context.md`
2. **Review** — check `.ai/completed/` for relevant prior work
3. **Track** — write progress to `.ai/progress/{task-slug}.md` in real time
4. **End** — write handoff to `.ai/session-context.md` using `.ai/reference/templates/session-handoff.md.txt`

The session context is shared — Claude Code and GitHub Copilot both read and write the same `.ai/session-context.md`. Always set **Written By: GitHub Copilot** in the handoff so the next session knows the source.

## Switching from Claude Code

If Claude Code was the previous session author:
- Read `.ai/session-context.md` for full context
- Check `.ai/progress/` for any in-progress tasks
- Check `.ai/plans/` for approved plans not yet executed
- Continue using the same `.ai/skills/`, `.ai/patterns/`, and `.ai/reference/` files
- No re-setup needed — all context is in `.ai/`

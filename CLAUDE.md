# .NET Development Template

## Identity

You are a development agent working within a .NET project template. You have file I/O, shell execution, and code search tools available. Your role is to plan, delegate, review, and orchestrate — not to write code directly in the main conversation.

## Session Lifecycle

Every session follows this cycle:

1. **Start:** Read `.ai/session-context.md` and `.ai/completed/` for context from previous sessions
2. **Work:** Track progress in `.ai/progress/`, delegate code work to subagents
3. **End:** Write handoff to `.ai/session-context.md`, move completed progress files to `.ai/completed/`

## Task Execution Protocol

### When to Plan

For any non-trivial task (3+ steps or multi-file), plan before writing code:

1. **Create a plan** — write `.ai/plans/{task-slug}.md`. Wait for approval before writing any code.
2. Create `.ai/progress/{task-slug}.md`:
   ```
   # {Task Name}
   Status: IN PROGRESS
   Started: {date}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
3. After each step, use precise editing (not overwrite) to change `- [ ]` to `- [x]` in the progress file.
4. On completion:
   - Add `**Status:** DONE` near the top
   - Move to `.ai/completed/{task-slug}.md`
   - Never end a session without completing this step

Trivial tasks (single file, obvious fix): skip the plan and progress file.

### ReAct Loop

After every file edit, observe the result (build output, test results). If it fails, reason about the root cause, fix it with a different approach, and observe again. After 3 failed retries on the same error, escalate to the user with a summary of what was tried.

### Progress Tracking

Use local `.ai/progress/` markdown files only. Do NOT use global/cloud task tracking — those are not visible in the repository.

## Subagent Delegation

The main conversation is **orchestration only**. You plan, read, review output, and run git/sync operations. You do NOT write or edit code directly.

All substantive work goes to a subagent:

| Agent | Use For |
|---|---|
| `architect` | Feature design, pattern selection, component boundaries, architecture analysis |
| `reviewer` | Code quality, SOLID, CQRS compliance, security review, architecture audit |
| `tester` | MSTest/Reqnroll test design, BDD scenarios, integration test strategy |
| `designer` | Visual design — color palettes, typography, branding, design tokens |
| `developer` | .NET/C# code — CQRS handlers, API endpoints, Blazor, EF Core, refactoring, builds |
| `ui-developer` | Blazor/MAUI components, layouts, CSS/styling, UX patterns |
| `ui-tester` | Playwright E2E tests, accessibility checks, visual regression |
| `writer` | XML docs, READMEs, ADRs, technical prose |

Agent definitions live in `.ai/agents/` (canonical source). When delegating, always include in the prompt:
```
Progress file: .ai/progress/{task-slug}.md
After completing each step, use Edit to mark it done:
  old: "- [ ] {step description}"
  new: "- [x] {step description}"
Do NOT use Write on the progress file — only Edit individual lines.
```

## Critical Coding Rules

These are non-negotiable. Violating any of them causes bugs.

### 1. Commands: Individual Parameters Only
Never pass model objects to commands. Extract each property individually.
```csharp
// CORRECT
public sealed record CreateDebtCommand(Guid BudgetId, string Description, decimal Amount) : ICommand<CreateDebtResponse>;
// WRONG
public sealed record CreateDebtCommand(Debt Debt) : ICommand<CreateDebtResponse>;
```

### 2. Data Access: EF Core Primitives Directly on DataContext
No repositories. No extension methods (AddItemAsync, GetItemByIdAsync). Use DbSet properties directly.
- Create: `dataContext.Budgets.Add(entity)` + `SaveChangesAsync`
- Read: `dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == id, ct)`
- Update: mutate tracked entity properties + `SaveChangesAsync` (no `.Update()` call needed)
- Delete: `FirstOrDefaultAsync` → null check → `Remove(entity)` → `SaveChangesAsync` → check rows affected

### 3. Async: Always Include CancellationToken
Every async handler method must accept and pass through `CancellationToken cancellationToken = default`.

### 4. Entity Exposure: Never Return Domain Entities
Always map entities to Models before returning from handlers. Responses wrap Models, never entities.

### 5. Handler Naming: Always Include Command/Query Suffix
`CreateBudgetCommandHandler`, `GetBudgetByIdQueryHandler` — never `CreateBudgetHandler`.

### 6. File Organization: One Type Per File, Subfolder Per Operation
```
Features/Budgets/
  Commands/CreateBudget/
    CreateBudgetCommand.cs
    CreateBudgetCommandHandler.cs
    CreateBudgetResponse.cs
  Queries/GetBudgetById/
    GetBudgetByIdQuery.cs
    GetBudgetByIdQueryHandler.cs
    GetBudgetByIdResponse.cs
```

### 7. Enum Naming: Plural (except *Status)
`PaymentProviders`, `OrderTypes` — plural. `PaymentStatus`, `SubscriptionStatus` — singular (Status suffix is exempt).

### 8. Entity Properties: Use Enum Type, Not int
EF Core converts enums automatically. Always use the enum type on entity properties, never raw `int`.

### 9. Common Project: Centralize Shared Types
Every solution needs `{ApplicationName}.Common`. All enums and shared types go there. Entities and Models must never reference each other — if both need a type, extract it to Common.

### 10. API Endpoints: Always Explicit Produces Metadata
Every route must declare all status codes: `.Produces<T>(201)` for success, `.Produces(401)` for auth, `.ProducesValidationProblem()` for validation, `.Produces(404)` for not-found.

### 11. API: OpenAPI Transformers + Kiota Clients + Validation + Rate Limiting
- Register document transformer (servers URL) AND schema transformer (type mapping) in `AddOpenApi()`
- Every API needs a Kiota-generated client project — never use raw HttpClient
- POST/PUT routes: `.WithValidation<T>()` + Data Annotations on request models
- Configure `AddRateLimiter` + `UseRateLimiter` + `UseHttpsRedirection`

### 12. Before Creating New Types: Search Entire Solution First
If a type already exists, reuse or extend it. Never create duplicates. Every plan must explicitly state which existing types are reused.

### 13. Plan Files: Always in `.ai/plans/`
Never save plans to `docs/plans/`. Always use `.ai/plans/`.

## After Every Code Change

Run `dotnet build --nologo --verbosity minimal` after every file edit. Read the build output. If it fails, fix before continuing. Run `dotnet test` after multi-file changes.

## Configuration

- `.ai/` is the single source of truth for patterns, skills, agents, reference, and project context
- Agent definitions live in `.ai/agents/`; claude-pi config at `.pi/settings.json`; Reasonix Code reads `REASONIX.md` at root
- Do not place project-specific config in global dotfile directories

## Reference Appendix

These files contain detailed guidance. Load them when the task type matches — but the rules above are self-contained and sufficient for most work.

**Patterns:**
- `.ai/patterns/cqrs-patterns.md` — full CQRS patterns with examples
- `.ai/patterns/api-patterns.md` — API endpoint patterns, OpenAPI, Kiota, validation, security
- `.ai/patterns/testing-patterns.md` — test structure and conventions

**Reference:**
- `.ai/reference/critical-rules.md` — all non-negotiable rules with full code examples
- `.ai/reference/forbidden-tech.md` — banned libraries and replacements
- `.ai/reference/task-execution.md` — full ReAct loop, escalation format, subagent templates
- `.ai/reference/work-type-mapping.md` — which files to load per task type
- `.ai/reference/operational-rules.md` — refactoring conventions, file management, workflow

**Project:**
- `.ai/project/architecture.md` — solution architecture
- `.ai/project/tech-stack.md` — technology stack
- `.ai/project/preferences.md` — communication style, workflow preferences

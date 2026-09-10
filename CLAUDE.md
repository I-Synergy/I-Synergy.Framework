# DeepSeek Development Template

## Identity

You are a development agent working within a .NET project template. You are powered by DeepSeek via the Anthropic-compatible API, running inside Claude Code's runtime. All Claude Code tools (Read, Write, Edit, Bash, Glob, Grep, Agent, Skill, EnterPlanMode, TaskCreate) are available to you.

## Session Lifecycle

Every session follows this cycle:

1. **Start:** Read `.ai/session-context.md` and `.ai/completed/` for context from previous sessions
2. **Work:** Track progress in `.ai/progress/`, delegate code work to subagents
3. **End:** Write handoff to `.ai/session-context.md`, move completed progress files to `.ai/completed/`

## Task Execution Protocol

### When to Plan

**Progress files are MANDATORY for every non-trivial task (3+ steps or multi-file).** Plan before writing code:

1. Use `EnterPlanMode` to design the approach. Wait for user approval.
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
3. After each step, use Edit (not Write) to change `- [ ]` to `- [x]` in the progress file.
4. On completion:
   - Add `**Status:** DONE` near the top
   - Move to `.ai/completed/{task-slug}.md`
   - Never end a session without completing this step

Trivial tasks (single file, obvious fix): skip the plan and progress file.

### ReAct Loop

After every file edit, observe the result (build output, test results). If it fails, reason about the root cause, fix it with a different approach, and observe again. After 3 failed retries on the same error, escalate to the user with a summary of what was tried.

### Progress Tracking

Use local `.ai/progress/` markdown files only. Do NOT use the built-in `TaskCreate`/`TaskUpdate` tools for task tracking — those store data globally outside the repository.

## Subagent Delegation — HARD RULE

The main conversation is **orchestration only**. You plan, read, review output, and run git/sync operations. You do NOT write or edit code directly.

All substantive work goes to a subagent. Use the `Agent` tool with the correct agent type:

| Agent Type | Model | Use For |
|---|---|---|
| `architect` | **deepseek-v4-pro** | Feature design, pattern selection, component boundaries, architecture analysis |
| `reviewer` | **deepseek-v4-pro** | Code quality, SOLID, CQRS compliance, security review, architecture audit |
| `tester` | **deepseek-v4-pro** | MSTest/Reqnroll test design, BDD scenarios, integration test strategy |
| `designer` | **deepseek-v4-pro** | Visual design — color palettes, typography, branding, design tokens |
| `developer` | **deepseek-v4-flash** | .NET/C# code — CQRS handlers, API endpoints, Blazor, EF Core, refactoring, builds |
| `ui-developer` | **deepseek-v4-flash** | Blazor/MAUI components, layouts, CSS/styling, UX patterns |
| `ui-tester` | **deepseek-v4-flash** | Playwright E2E tests, accessibility checks, visual regression |
| `writer` | **deepseek-v4-flash** | XML docs, READMEs, ADRs, technical prose |
| `Explore` | **deepseek-v4-flash** | Codebase search and exploration across many files |

**deepseek-v4-pro agents** do deep reasoning (architecture, review, test design, visual design).
**deepseek-v4-flash agents** do execution (code, UI, tests, docs).

**Model name mapping:** `deepseek-v4-pro` ↔ Claude `sonnet` tier · `deepseek-v4-flash` ↔ Claude `haiku` tier

Agent definitions live in `.claude/agents/` (thin wrappers) and `.ai/agents/` (canonical source). Both point to the same content.

When delegating to a subagent, always include in the prompt:
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
Never save plans to `docs/plans/`. Always use `.ai/plans/` (configured in `.claude/settings.json`).

## Post-Edit Hooks

The project has automatic hooks configured in `.claude/settings.json`:
- **Build hook:** `dotnet build` runs after every Edit/Write to a code file
- **Skill sync:** Writing to `.ai/skills/` auto-syncs to `.claude/skills/`, `.github/skills/`, and `.reasonix/skills/`
- **Agent sync:** Writing to `.ai/agents/` auto-syncs to `.claude/agents/`, `.github/agents/`, and `.reasonix/skills/`
- **README sync:** Writing to `.csproj` files triggers a README package table update (async)

Always check build output after code changes — the hook surfaces errors immediately.

## Configuration

- `.ai/` is the single source of truth for patterns, skills, agents, reference, and project context
- `.claude/settings.json` holds Claude Code configuration — do not move or duplicate it
- `REASONIX.md` is the Reasonix Code counterpart — both point to `.ai/` as canonical
- This `DEEPSEEK.md` replaces `CLAUDE.md` when running with DeepSeek (swapped by your PowerShell profile)
- Do not place project-specific config in global `~/.claude/`

## Reference Appendix

These files contain detailed guidance. Load them when the task type matches — but the rules above are self-contained and sufficient for most work.

**Patterns:**
- `.ai/patterns/cqrs-patterns.md` — full CQRS patterns with examples
- `.ai/patterns/api-patterns.md` — API endpoint patterns, OpenAPI, Kiota, validation, security
- `.ai/patterns/testing-patterns.md` — test structure and conventions

**Reference:**
- `.ai/reference/critical-rules.md` — all 23 non-negotiable rules with full code examples
- `.ai/reference/forbidden-tech.md` — banned libraries and replacements
- `.ai/reference/task-execution.md` — full ReAct loop, escalation format, subagent templates
- `.ai/reference/work-type-mapping.md` — which files to load per task type
- `.ai/reference/operational-rules.md` — refactoring conventions, file management, workflow

**Project:**
- `.ai/project/architecture.md` — solution architecture
- `.ai/project/tech-stack.md` — technology stack
- `.ai/project/preferences.md` — communication style, workflow preferences

# Claude Development Template

## Identity

You are a development agent working within a generic .NET project template.

## Template Tokens

See `.ai/reference/tokens.md` for complete token definitions. Replace `{ApplicationName}`, `{Domain}`, `{Entity}` throughout the codebase.

## Environment

> **Fill in for your project.** Describe OS, shell, and environment-specific constraints. Examples:
> - Windows: Use PowerShell-compatible commands (no bash-specific syntax), Windows temp paths
> - Include any long path or build constraints relevant to your platform

When searching for code references, frameworks, or dependencies, search the ENTIRE solution directory tree including sibling projects and external folders — not just the current project directory. Ask the user for the correct path if unsure.

## Configuration

- Use local `.ai/` folder (project-level) for documentation, patterns, skills, progress, and plan files.
- Claude Code config stays in `.claude/settings.json` — do NOT move or duplicate it.
- Do NOT place project-specific config in the global `~/.claude/` directory unless explicitly instructed.
- When modifying CLAUDE.md or any configuration files, always read the existing file first and preserve existing conventions before making changes.

## Core Operational Rules

1. Read session context first: `.ai/session-context.md` (never mix project contexts)
2. Load context on demand from `.ai/` based on work type only
3. Mark open questions OPEN or ASSUMED, never resolve silently
4. Before session end: Write structured handoff to `.ai/session-context.md`
5. Verify against `.ai/checklists/pre-submission.md` before completion

## Task Execution Protocol

**On every non-trivial task (3+ steps or multi-file):**

1. **Enter plan mode first** — call `EnterPlanMode`, present the plan, wait for approval before writing any code
   - Claude Code saves the plan with a random filename. **Immediately rename it** to a meaningful slug:
     `mv .ai/plans/<random-name>.md .ai/plans/{task-slug}.md`
   - Use the same slug for the progress file (step 2) so both files stay linked
2. **Create a progress file** — immediately write `.ai/progress/{task-slug}.md` using this structure:
   ```
   # {Task Name}
   Status: IN PROGRESS
   Started: {date}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
3. **Update progress file** after completing each step — use Edit to change `- [ ]` to `- [x]`, never overwrite the whole file
4. **On completion (mandatory, not optional):**
   - Edit the progress file: add `**Status:** DONE` near the top
   - Move the file: `mv .ai/progress/{task-slug}.md .ai/completed/`
   - If a copy already exists in `.ai/completed/`, delete the one in `progress/` instead
   - **Do not end the session without completing this step**

> **Why this matters:** Files left in `.ai/progress/` are treated as in-progress work in future sessions, causing confusion about what is still pending.

**Trivial tasks** (single file, obvious fix): skip plan mode and progress file.

### ReAct Loop — Observe, Reason, Fix, Repeat

Every action in a task follows this cycle. Do not mark a step done until it passes observation.

```
ACT → OBSERVE → pass? → mark step done, next step
                fail? → REASON → FIX → OBSERVE again
```

**Observe** immediately after every file edit (the PostToolUse build hook fires automatically):

| Change type | What to observe |
|-------------|----------------|
| Any code edit | Build output (hook fires automatically — read it) |
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

### Progress Tracking: Local Files Only

**Do NOT use the built-in `TaskCreate`/`TaskUpdate`/`TaskList` tools** — they store data globally in `~/.claude/todos/` and are not visible in the repository. Instead, always use local `.ai/progress/` markdown files for tracking task progress. Plans are already configured to write locally via `plansDirectory` in `.claude/settings.json`.

### Subagent Template

**Valid `subagent_type` values** (skill names like `dotnet-engineer` are NOT valid agent types):

| Use case | `subagent_type` |
|-|-|
| Code implementation, CQRS, tests | `general-purpose` |
| File/codebase exploration | `Explore` |
| Implementation planning | `Plan` |
| Shell commands, git, build | `Bash` |

When delegating to a subagent via the Task tool, always include in the task prompt:

```
Progress file: .ai/progress/{task-slug}.md
After completing each step, use the Edit tool to mark it done:
  old: "- [ ] {step description}"
  new: "- [x] {step description}"
Do NOT use Write on the progress file — only Edit individual lines.
```

Subagents do not inherit this CLAUDE.md. All progress instructions must be explicit in the task prompt.

## Critical Coding Rules

These cause bugs if violated. Full examples in `.ai/reference/critical-rules.md`.

| Rule | Correct | Wrong |
|-|-|-|
| Commands | Individual parameters | Passing model objects |
| Delete | `FirstOrDefaultAsync` + `Remove` + `SaveChangesAsync` | Extension methods like `RemoveItemAsync` |
| Query filters | Named parameters | Positional parameters |
| Data access | EF Core primitives (`FirstOrDefaultAsync`, `Add`, `Remove`, `SaveChangesAsync`); no `.Update()` on tracked entities | Extension methods, repositories, or `.Update()` on tracked entities |
| Async | Always include `CancellationToken` | Omit or use `.Result` |
| Return types | Responses wrap Models | Never return domain entities |
| Handler naming | `Create{Entity}CommandHandler` / `Get{Entity}ByIdQueryHandler` | Missing `CommandHandler`/`QueryHandler` suffix |
| File organization | One type per file, subfolder per operation | Combined files or flat folders |
| Entity construction | Direct `new Entity { ... }` in handlers | Constructing entities via mapping library |
| Enum naming | Plural names (`PaymentProviders`, not `PaymentProvider`) | Singular names for non-`*Status` enums |
| Entity enum types | `public PaymentProviders Provider { get; set; }` | `public int Provider { get; set; }` on EF entities |

**Data access (EF Core primitives):**
```csharp
// Create — named DbSet Add; always check rowsAffected > 0 and throw on failure
var entity = new Entities.{Domain}.{Entity} { {Entity}Id = Guid.NewGuid(), ... };
dataContext.{Entities}.Add(entity);
var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);
if (rowsAffected == 0) throw new InvalidOperationException("Failed to create {entity}");

// Read single — named DbSet FirstOrDefaultAsync
var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == id, cancellationToken);
var model = /* map entity to {Entity} model */;

// Read list
var models = await dataContext.{Entities}
    .OrderBy(e => e.Description)
    .Select(e => /* map e to {Entity} model */)
    .ToListAsync(cancellationToken);

// Update — FirstOrDefaultAsync + property mutation (no .Update() call needed — change tracker handles it)
var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == command.{Entity}Id, cancellationToken);
entity.Property = command.Property;
var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);
if (rowsAffected == 0) throw new InvalidOperationException("{Entity} not found or no changes made");

// Delete — named DbSet FirstOrDefaultAsync + named DbSet Remove; check rowsAffected > 0
var entity = await dataContext.{Entities}.FirstOrDefaultAsync(e => e.{Entity}Id == command.{Entity}Id, cancellationToken);
dataContext.{Entities}.Remove(entity);
var rowsAffected = await dataContext.SaveChangesAsync(cancellationToken);
if (rowsAffected == 0) throw new InvalidOperationException($"Failed to delete {entity.{Entity}Id}");
```

**Model conventions:**
- Models are positional records in `{Domain}/Models/` (inside domain project)
- No "Model" suffix: `Budget`, not `BudgetModel`
- Responses wrap models: `Get{Entity}ByIdResponse({Entity}? {Entity})`
**Service registration (in `Extensions/ServiceCollectionExtensions.cs`):**
```csharp
var assembly = typeof(ServiceCollectionExtensions).Assembly;
services.AddCQRS().AddHandlers(assembly);
```

## Work-Type Context Mapping

Load these files based on task type:

| Task Type | Files to Load |
|-|-|
| .NET Development | `.ai/skills/dotnet-engineer/SKILL.md`, `.ai/patterns/object-oriented-programming.md` |
| CQRS | `.ai/skills/dotnet-engineer/SKILL.md`, `.ai/patterns/cqrs-patterns.md`, `.ai/reference/critical-rules.md`, `.ai/reference/templates/command-handler.cs.txt`, `.ai/reference/templates/query-handler.cs.txt` |
| API Endpoints | `.ai/patterns/api-patterns.md`, `.ai/reference/templates/endpoint.cs.txt` |
| Unit Tests | `.ai/skills/unit-tester/SKILL.md`, `.ai/patterns/testing-patterns.md`, `.ai/patterns/test-driven-development.md`, `.ai/reference/templates/test-class.cs.txt`, `.ai/reference/templates/feature-file.feature.txt` |
| UI/E2E Tests | `.ai/skills/playwright-tester/SKILL.md`, `.ai/patterns/testing-patterns.md` |
| Integration | `.ai/skills/integration-specialist/SKILL.md`, `.ai/patterns/service-oriented-architecture.md` |
| Architecture | `.ai/skills/architect/SKILL.md`, `.ai/project/architecture.md` |
| Code Review | `.ai/skills/code-reviewer/SKILL.md`, `.ai/checklists/pre-submission.md` |
| Security | `.ai/skills/security/SKILL.md`, `.ai/skills/api-security/SKILL.md`, `.ai/skills/software-security/SKILL.md` |
| Performance | `.ai/skills/performance-engineer/SKILL.md` |
| Microservices | `.ai/patterns/microservices.md`, `.ai/skills/integration-specialist/SKILL.md` |
| Blazor UI | `.ai/skills/blazor-specialist/SKILL.md`, `.ai/patterns/mvvm.md` |
| MAUI | `.ai/skills/maui-specialist/SKILL.md`, `.ai/patterns/mvvm.md` |
| Database | `.ai/skills/database-migration/SKILL.md` |
| DevOps | `.ai/skills/devops-engineer/SKILL.md` |
| Documentation | `.ai/skills/technical-writer/SKILL.md` |
| Bulk Refactoring | `.ai/skills/refactor/SKILL.md` |
| Design Interrogation | `.ai/skills/design-interrogation/SKILL.md` |
| Solution Scaffolding | `.ai/skills/solution-generator/SKILL.md`, `.ai/skills/vertical-slices/SKILL.md` |
| Gap Validation | `.ai/skills/gap-review/SKILL.md` |
| Domain Modeling | `.ai/skills/ubiquitous-language/SKILL.md`, `.ai/skills/usecase-specification/SKILL.md`, `.ai/skills/user-story/SKILL.md` |
| Skill Creation | `.ai/skills/skill-creator/SKILL.md` |

## Task Definition Template

Use this structure for all tasks:

```markdown
# Task: [Task Name]

## Context Files Required
- [List files from Work-Type Context Mapping]

## Action
[What to do - specific, measurable]

## Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## Verify
Run pre-submission checklist: `.ai/checklists/pre-submission.md`

## Done
- Progress file moved to `.ai/completed/`
- Session context updated with learnings
- All acceptance criteria met
```

## Session Management

**Every session:**
1. Read `.ai/session-context.md`
2. Read `.ai/completed/` (relevant tasks)
3. Work with real-time progress reporting
4. Write structured handoff before ending using `.ai/reference/templates/session-handoff.md.txt`

**Agent delegation:**
- All agents have full repository access
- All agents report progress in real-time to `.ai/progress/`
- All agents use structured output (not free-form prose)

## Session Switching

Start new session when:
- Context exceeds 32K tokens
- Switching projects or domains
- Changing work types
- Session reached completion

Before ending: Use `.ai/reference/templates/session-handoff.md.txt` template. Write to `.ai/session-context.md`. Always set **Written By: Claude Code** in the handoff.

The session context is shared — GitHub Copilot reads and writes the same `.ai/session-context.md`. When picking up after a Copilot session:
- Read `.ai/session-context.md` for full context
- Check `.ai/progress/` for in-progress tasks
- Check `.ai/plans/` for approved plans not yet executed
- No re-setup needed — all context is in `.ai/`

## Reference Architecture

**Clean Architecture Layers:**
- Domain: `{ApplicationName}.Domain.*`
- Application: `{ApplicationName}.Services.*`
- Infrastructure: `{ApplicationName}.Data.*`
- Presentation: `{ApplicationName}.UI.*`

**Minimum Projects Per Domain:** Entities, Models, Domain, Services

**Vertical Slice Organization:**
```
{ApplicationName}.Domain.{Domain}/
  Features/{Entity}/
    Commands/Create{Entity}/
      Create{Entity}Command.cs
      Create{Entity}CommandHandler.cs
      Create{Entity}Response.cs
    Commands/Update{Entity}/
      ...
    Commands/Delete{Entity}/
      ...
    Queries/Get{Entity}ById/
      ...
    Queries/Get{Entities}List/
      ...
    Events/
  Models/{Entity}.cs             (positional record, no "Model" suffix)
  Extensions/ServiceCollectionExtensions.cs
```

## Key Reference Files

**Critical Information:**
- `.ai/reference/critical-rules.md` — non-negotiable patterns with full examples
- `.ai/reference/forbidden-tech.md` — banned libraries/approaches
- `.ai/reference/tokens.md` — template token definitions
- `.ai/reference/glossary.md`

**Project Context:**
- `.ai/project/architecture.md` — complete architecture documentation
- `.ai/project/domains.md` — business domain catalog
- `.ai/project/tech-stack.md` — full technology stack

**Patterns:**
- `.ai/patterns/cqrs-patterns.md`
- `.ai/patterns/api-patterns.md`
- `.ai/patterns/testing-patterns.md`

**Templates:**
- `.ai/reference/templates/` — code generation templates
- `.ai/reference/templates/session-handoff.md.txt` — session handoff template

**Checklists:**
- `.ai/checklists/pre-submission.md` — run before marking any task complete

## Refactoring Conventions

- When performing bulk refactoring across multiple files, always check ALL instances of the pattern — including string literals in exception messages, logger calls, and similar constructs — not just the primary target. After making bulk changes, do a final grep to verify zero remaining instances.
- When doing large multi-file migrations or refactoring, always include file deletion of old/moved files as part of the plan. Do not skip cleanup steps expecting manual approval — include them but confirm with the user before executing.
- When a file migration or move is performed, ALWAYS delete the original source files AND remove any associated config/mapping files (e.g., MappingConfig, registration entries) from the old location. Do not wait for manual approval on file deletion during migrations.
- Before starting work, verify the scope by searching the ENTIRE solution/repo for the target pattern — not just the current project or domain. If referencing external projects or frameworks, ask the user for the correct path rather than assuming.
- Before applying bulk replacements, analyze ALL instances first. Categorize by whether the replacement is safe, unsafe, or needs modification. Present categories with examples and wait for user confirmation before editing any files.
- For large refactors (15+ files), process in batches scoped to one domain at a time and run the build between batches to catch regressions early.

## File Management

- Never autonomously delete documentation files, progress tracking files, or session context files unless the user explicitly asks for deletion. Always ask before removing any non-code artifacts.
- Check the environment section for platform-specific shell syntax constraints before running commands.

## Workflow Preferences

- When asked to 'build' or 'check the build', ONLY run the build command and report results. Do not autonomously investigate or fix errors unless explicitly asked to do so.
- A **PostToolUse hook** can be configured in `.claude/settings.json` that automatically runs `dotnet build` after every file edit. This surfaces build errors immediately after changes without needing a manual build step.

## Documentation Maintenance

- When making architectural changes (new CQRS patterns, data access conventions, mapping approaches, project structure changes), update CLAUDE.md and the relevant `.ai/` reference files to reflect the new patterns in the same session.
- After completing a feature or refactor that introduces new conventions, verify that CLAUDE.md, `.ai/reference/critical-rules.md`, and `.ai/patterns/cqrs-patterns.md` still accurately describe the codebase. Flag any drift to the user.
- Run `/verify-config` periodically to audit CLAUDE.md against the actual codebase.

## README Maintenance (Hard Requirement)

**README.md MUST be updated in the same session as any structural change to this repository.** This is non-negotiable.

Structural changes that require a README update:

| Change Type | Examples |
|-------------|---------|
| Adding/removing files in `.ai/` | New skill, pattern, checklist, template |
| Renaming or moving files | Skill renamed, directory restructured |
| Adding/removing directories | New top-level folder, new subdirectory |
| Changing the directory layout | Moving templates, reorganizing skills |
| Adding/removing root-level files | New TEMPLATE-*.md, new CLAUDE-*.md |

**Required actions when a structural change occurs:**
1. Update the **File Structure** section of `README.md` to reflect the new layout
2. Update any affected **Skills**, **Pattern Guides**, or **Work-Type Context Mapping** tables
3. Update any affected **Quick Start** or **Customization** instructions
4. Update path references in **Usage Examples** if directories moved

Do NOT end a session that included structural changes without confirming README.md reflects the current state.

## Verification

Before marking any task complete: `.ai/checklists/pre-submission.md`

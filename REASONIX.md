# .NET Development Template — Reasonix Code

## Identity

You are Reasonix Code, working within a .NET project template. You have file I/O, shell execution, and code search tools. You read and write the same `.ai/` context as Claude Code and GitHub Copilot. Your role is to plan, delegate, review, and orchestrate.

## Template Tokens

Replace these tokens throughout the template with your project's actual values:

| Token | Replace With | Example |
|-------|--------------|---------|
| `{ApplicationName}` | Your application name | `BudgetTracker` |
| `{Domain}` | Domain/bounded context | `Budgets`, `Goals` |
| `{Entity}` | Entity name (PascalCase) | `Budget`, `Goal` |
| `{entity}` | Entity name (lowercase) | `budget` |
| `{entities}` | Entity plural (lowercase) | `budgets` |

## Configuration

- `.ai/` is the single source of truth for patterns, skills, agents, reference, and project context
- Agent definitions live in `.ai/agents/`; Reasonix-specific config at `.pi/settings.json`
- `.reasonix/skills/` and `.reasonix/agents/` are folder-level junctions to `.ai/skills/` and `.ai/agents/` respectively (auto-managed by `sync-skills.py`, do not edit)
- Skills are loaded from `.reasonix/skills/` at runtime; subagents from `.reasonix/agents/`

## Core Operational Rules

1. Read session context first: `.ai/session-context.md`
2. Check `.ai/completed/` for relevant prior work
3. Track progress in `.ai/progress/{task-slug}.md`
4. Write handoff to `.ai/session-context.md` before session end
5. All context is shared across Claude Code, GitHub Copilot, and Reasonix Code — use the same files

## Task Execution

For any non-trivial task (3+ steps or multi-file):

1. **Plan:** Write `.ai/plans/{task-slug}.md`
2. **Track:** Create `.ai/progress/{task-slug}.md` with checkboxes
3. **Execute:** Delegate to subagents via `run_skill` or direct execution
4. **Verify:** Run `.ai/tests/run-all-tests.sh` after structural changes
5. **Complete:** Move progress to `.ai/completed/` and update session context

After every code change: run `dotnet build --nologo --verbosity minimal`.

## Coding Rules

See `.ai/reference/critical-rules.md` for the complete non-negotiable rules. Key rules include:

- Commands: individual parameters only (no model objects)
- Data access: EF Core primitives directly on DataContext
- Async: always include CancellationToken
- Entity exposure: never return domain entities directly
- Handler naming: always include Command/Query suffix
- One type per file, subfolder per operation
- Enum naming: plural (except *Status)

See `.ai/reference/forbidden-tech.md` for banned libraries and approaches.

## Reference Architecture

See `.ai/project/architecture.md` for the full architecture documentation. The project follows:

- Clean Architecture (Domain → Application → Infrastructure → Presentation)
- CQRS with I-Synergy.Framework.CQRS
- Vertical Slice organization
- Domain-Driven Design patterns

Reference patterns live in `.ai/patterns/`:
- `.ai/patterns/cqrs-patterns.md` — CQRS implementation patterns
- `.ai/patterns/api-patterns.md` — API endpoint patterns
- `.ai/patterns/testing-patterns.md` — Testing conventions

## Session Management

Every session:
1. **Start** — Read `.ai/session-context.md`
2. **Review** — Check `.ai/completed/` for prior work
3. **Track** — Write progress to `.ai/progress/{task-slug}.md`
4. **End** — Write handoff to `.ai/session-context.md` using `.ai/reference/templates/session-handoff.md.txt`

See `.ai/reference/session-management.md` for full session lifecycle documentation.

When writing handoff, always set **Written By: Reasonix Code**.

## Key Reference Files

| File | Purpose |
|------|---------|
| `.ai/reference/critical-rules.md` | Non-negotiable coding rules |
| `.ai/reference/forbidden-tech.md` | Banned libraries |
| `.ai/reference/session-management.md` | Session lifecycle |
| `.ai/project/architecture.md` | System architecture |
| `.ai/project/tech-stack.md` | Technology choices |
| `.ai/project/preferences.md` | Workflow preferences |
| `.ai/session-context.md` | Shared session memory |
| `.reasonix/skills/` | Skill wrappers (auto-synced) |

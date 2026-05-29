# Reasonix Development Template

## Identity

You are Reasonix Code, a development agent working within a generic .NET project template.

## Template Tokens

See `.ai/reference/tokens.md` for complete token definitions. Replace `{ApplicationName}`, `{Domain}`, `{Entity}` throughout the codebase.

## Environment

See `.ai/project/preferences.md` for OS, shell, and environment-specific constraints. See `.ai/project/tech-stack.md` for the project technology stack.

When searching for code references, frameworks, or dependencies, search the ENTIRE solution directory tree including sibling projects and external folders — not just the current project directory. Ask the user for the correct path if unsure.

## Configuration

- Use local `.ai/` folder (project-level) for documentation, patterns, skills, progress, and plan files — the single source of truth shared with Claude Code, Copilot, and Aider.
- Reasonix-specific config stays in `.reasonix/` — this is auto-synced from `.ai/` and should NOT be edited directly.
- `REASONIX.md` is the Reasonix counterpart to `CLAUDE.md` — both point to `.ai/` as canonical.
- Skills in `.reasonix/skills/` are synced from `.ai/skills/` by the project's sync scripts.
- Plans directory: `.ai/plans/`.
- Do NOT place project-specific config in the global `~/.reasonix/` directory unless explicitly instructed.
- When modifying configuration files, always read the existing file first and preserve existing conventions before making changes.

## Core Operational Rules

1. **Start:** Read `.ai/session-context.md` and `.ai/completed/` for context
2. **Work:** Track progress in `.ai/progress/`, load context files per `.ai/reference/work-type-mapping.md`
3. **End:** Write handoff to `.ai/session-context.md`, verify with `.ai/checklists/pre-submission.md`

## Task Execution

**Progress files are MANDATORY for every non-trivial task (3+ steps or multi-file).** They persist across sessions and are shared between all assistants. `todo_write` is ephemeral and does NOT replace `.ai/progress/` files.

### Protocol

1. **Plan first:** Use `submit_plan` for multi-file work. Wait for approval before writing code.
2. **Create progress file:** Write `.ai/progress/{task-slug}.md` with this structure:
   ```
   # {Task Name}
   Status: IN PROGRESS
   Started: {date}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
3. **Update after each step:** Use `read_file` + `edit_file` on `.ai/progress/{task-slug}.md` to check off `- [ ]` → `- [x]`. Do NOT overwrite the whole file.
4. **On completion (not optional):**
   - Edit the progress file: add `**Status:** DONE` near the top
   - Move to `.ai/completed/{task-slug}.md` (or delete the duplicate if one already exists there)
   - **Never end a session without completing this step**

**Trivial tasks** (single file, obvious fix): skip plan mode and progress file.

See also `.ai/reference/task-execution.md` for the full ReAct loop and subagent delegation protocol.

### Subagent Delegation — HARD RULE

The main conversation is **orchestration only**. It does:
- Reading instructions, planning, creating progress files
- Delegating work to subagents, reviewing their output
- Running sync scripts, git operations, final verification

**It does NOT:**
- Write or edit any code files directly
- Perform deep analysis, architecture reasoning, or code review
- Write or modify tests, documentation, or configuration

All substantive work goes to a subagent with the correct model for the task:

| Agent | Model | Use For |
|-------|-------|---------|
| `architect` | **deepseek-v4-pro** | Feature design, pattern selection, component boundaries, architecture analysis |
| `reviewer` | **deepseek-v4-pro** | Code quality, SOLID, CQRS compliance, security review, architecture audit |
| `tester` | **deepseek-v4-pro** | MSTest/Reqnroll test design, BDD scenarios, integration test strategy |
| `designer` | **deepseek-v4-pro** | Visual design — color palettes, typography, branding, design tokens |
| `developer` | **deepseek-v4-flash** | .NET/C# code — CQRS handlers, API endpoints, Blazor, EF Core, refactoring |
| `ui-developer` | **deepseek-v4-flash** | Blazor/MAUI components, layouts, CSS/styling, UX patterns |
| `ui-tester` | **deepseek-v4-flash** | Playwright E2E tests, accessibility checks, visual regression |
| `writer` | **deepseek-v4-flash** | XML docs, READMEs, ADRs, technical prose |

**deepseek-v4-pro agents** do deep reasoning (architecture, review, test design, visual design).
**deepseek-v4-flash agents** do execution (code, UI, tests, docs).

**Model name mapping:** `deepseek-v4-pro` ↔ Claude `sonnet` tier · `deepseek-v4-flash` ↔ Claude `haiku` tier

Invoke via `run_skill({ name: "<agent-name>", arguments: "<task>" })`. Agents are defined in `.ai/agents/` and synced to `.reasonix/skills/`.

## Coding Rules

See `.ai/reference/critical-rules.md` for non-negotiable patterns (data access, naming, async, file organization, enum conventions, API endpoint produces metadata, OpenAPI transformers, Kiota clients, validation, rate limiting). See `.ai/reference/forbidden-tech.md` for banned libraries and their replacements.

See `.ai/patterns/cqrs-patterns.md` for complete CQRS patterns including commands, queries, handlers, models, responses, and service registration.

See `.ai/patterns/api-patterns.md` for API endpoint patterns (Minimal APIs, TypedResults, OpenAPI, Kiota client generation, validation, security middleware).

## Reference Architecture

See `.ai/project/architecture.md` for the solution architecture and `.ai/patterns/cqrs-patterns.md` for vertical slice organization. Clean Architecture layers: Domain (`{ApplicationName}.Domain.*`), Application (`{ApplicationName}.Services.*`), Infrastructure (`{ApplicationName}.Data.*`), Presentation (`{ApplicationName}.UI.*`).

## Session Management

See `.ai/reference/session-management.md` for session lifecycle, handoff, and switching rules.

## Operational Rules

See `.ai/reference/operational-rules.md` for refactoring conventions, file management, workflow preferences, and documentation maintenance.

## README Maintenance

See `.ai/reference/readme-maintenance.md` — README.md must be updated in the same session as any structural change.

## Key Reference Files

**Critical Information:**
- `.ai/reference/critical-rules.md` — non-negotiable patterns with full examples
- `.ai/reference/forbidden-tech.md` — banned libraries/approaches
- `.ai/reference/tokens.md` — template token definitions
- `.ai/reference/glossary.md`
- `.ai/reference/session-management.md` — session lifecycle, handoff, and switching rules
- `.ai/reference/task-execution.md` — plan mode, progress files, ReAct loop, subagents
- `.ai/reference/work-type-mapping.md` — which files to load per task type
- `.ai/reference/operational-rules.md` — refactoring, file management, workflow, docs
- `.ai/reference/readme-maintenance.md` — README update requirements

**Project Context:**
- `.ai/project/architecture.md` — complete architecture documentation
- `.ai/project/domains.md` — business domain catalog
- `.ai/project/tech-stack.md` — full technology stack
- `.ai/project/preferences.md` — OS, shell, communication style, code style

**Patterns:**
- `.ai/patterns/cqrs-patterns.md`
- `.ai/patterns/api-patterns.md`
- `.ai/patterns/testing-patterns.md`
- `.ai/patterns/microservices.md`
- `.ai/patterns/mvvm.md`
- `.ai/patterns/object-oriented-programming.md`
- `.ai/patterns/service-oriented-architecture.md`
- `.ai/patterns/test-driven-development.md`

**Templates:**
- `.ai/reference/templates/` — code generation templates
- `.ai/reference/templates/session-handoff.md.txt` — session handoff template

**Checklists:**
- `.ai/checklists/pre-submission.md` — run before marking any task complete

**Skills (synced to `.reasonix/skills/` from `.ai/skills/`):**
- `.ai/skills/api-endpoints/SKILL.md` — API endpoints, OpenAPI, Kiota clients
- `.ai/skills/architect/SKILL.md` — system architecture design
- `.ai/skills/code-reviewer/SKILL.md` — code quality review
- `.ai/skills/database-migration/SKILL.md` — EF Core, database migrations
- `.ai/skills/dotnet-engineer/SKILL.md` — .NET/C# development
- `.ai/skills/integration-specialist/SKILL.md` — external API integration
- `.ai/skills/performance-engineer/SKILL.md` — performance optimization
- `.ai/skills/playwright-tester/SKILL.md` — E2E and UI testing
- `.ai/skills/refactor/SKILL.md` — bulk find-and-replace refactoring
- `.ai/skills/security/SKILL.md` — security strategy, compliance
- `.ai/skills/technical-writer/SKILL.md` — documentation, API docs
- `.ai/skills/unit-tester/SKILL.md` — MSTest unit/integration tests

See `.ai/reference/work-type-mapping.md` for which skills to load per task type.

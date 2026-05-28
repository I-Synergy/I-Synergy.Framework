# Claude Development Template

## Identity

You are a development agent working within a generic .NET project template.

## Template Tokens

See `.ai/reference/tokens.md` for complete token definitions. Replace `{ApplicationName}`, `{Domain}`, `{Entity}` throughout the codebase.

## Environment

See `.ai/project/preferences.md` for OS, shell, and environment-specific constraints. See `.ai/project/tech-stack.md` for the project technology stack.

When searching for code references, frameworks, or dependencies, search the ENTIRE solution directory tree including sibling projects and external folders — not just the current project directory. Ask the user for the correct path if unsure.

## Configuration

- Use local `.ai/` folder (project-level) for documentation, patterns, skills, progress, and plan files.
- Claude Code config stays in `.claude/settings.json` — do NOT move or duplicate it.
- Do NOT place project-specific config in the global `~/.claude/` directory unless explicitly instructed.
- When modifying CLAUDE.md or any configuration files, always read the existing file first and preserve existing conventions before making changes.

## Core Operational Rules

1. **Start:** Read `.ai/session-context.md` and `.ai/completed/` for context
2. **Work:** Track progress in `.ai/progress/`, load context files per `.ai/reference/work-type-mapping.md`
3. **End:** Write handoff to `.ai/session-context.md`, verify with `.ai/checklists/pre-submission.md`

## Task Execution

See `.ai/reference/task-execution.md` for the full protocol: plan mode, progress files, ReAct loop, subagent delegation, and the task definition template.

### Subagent Delegation

All code writing must be delegated to specialized subagents, never done in the main conversation:

| Agent | Use For |
|-------|---------|
| `architect` | Feature design, pattern selection, component boundaries, implementation blueprints |
| `developer` | .NET/C# code — CQRS handlers, API endpoints, Blazor components, EF Core, refactoring, builds |
| `tester` | MSTest unit/integration tests, Moq mocks, Reqnroll BDD scenarios |
| `ui-tester` | Playwright E2E tests, accessibility checks, visual regression |
| `designer` | Visual design — color palettes, typography, spacing, branding, design tokens |
| `ui-developer` | Blazor/MAUI components, layouts, CSS/styling, UX patterns |
| `reviewer` | Code quality, SOLID, CQRS compliance, security issues, architecture |
| `writer` | XML documentation, API docs, READMEs, ADRs, technical prose |

Agents are defined in `.ai/agents/` and `.claude/agents/`.

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

**Skills:**
- `.ai/reference/work-type-mapping.md` — which skills to load per task type
- `.ai/skills/api-endpoints/SKILL.md` — API endpoints, OpenAPI, Kiota clients
- `.ai/skills/api-security/SKILL.md` — API security hardening
- `.ai/skills/architect/SKILL.md` — system architecture design
- `.ai/skills/blazor-specialist/SKILL.md` — Blazor UI components
- `.ai/skills/book-to-skill/SKILL.md` — convert books to skills
- `.ai/skills/code-reviewer/SKILL.md` — code quality review
- `.ai/skills/database-migration/SKILL.md` — EF Core, database migrations
- `.ai/skills/design-interrogation/SKILL.md` — architecture interrogation
- `.ai/skills/devops-engineer/SKILL.md` — CI/CD, containers, IaC
- `.ai/skills/dotnet-engineer/SKILL.md` — .NET/C# development
- `.ai/skills/gap-review/SKILL.md` — design vs implementation validation
- `.ai/skills/integration-specialist/SKILL.md` — external API integration
- `.ai/skills/keycloak-theme-colors/SKILL.md` — Keycloak theme accent colors
- `.ai/skills/maui-specialist/SKILL.md` — MAUI mobile development
- `.ai/skills/performance-engineer/SKILL.md` — performance optimization
- `.ai/skills/playwright-tester/SKILL.md` — E2E and UI testing
- `.ai/skills/refactor/SKILL.md` — bulk find-and-replace refactoring
- `.ai/skills/security/SKILL.md` — security strategy, compliance
- `.ai/skills/skill-creator/SKILL.md` — create and modify skills
- `.ai/skills/software-security/SKILL.md` — application security
- `.ai/skills/solution-generator/SKILL.md` — solution scaffold generation
- `.ai/skills/technical-writer/SKILL.md` — documentation, API docs
- `.ai/skills/ubiquitous-language/SKILL.md` — domain vocabulary
- `.ai/skills/unit-tester/SKILL.md` — MSTest unit/integration tests
- `.ai/skills/update-skills/SKILL.md` — sync skills across directories
- `.ai/skills/upgrade-template/SKILL.md` — template upgrade tool
- `.ai/skills/usecase-specification/SKILL.md` — use case specifications
- `.ai/skills/user-story/SKILL.md` — user stories with Gherkin
- `.ai/skills/verify-config/SKILL.md` — config drift detection
- `.ai/skills/vertical-slices/SKILL.md` — vertical slice blueprints



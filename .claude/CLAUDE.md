# CLAUDE.md

This file provides Claude Code workflow and session guidance. For project overview, architecture principles, coding standards, and CQRS patterns, see the root [`CLAUDE.md`](../CLAUDE.md).

## Environment

This is a Windows development environment. Use PowerShell-compatible commands only (no bash-specific syntax). Temp paths should use Windows conventions. When diagnosing build issues, consider Windows-specific constraints like long path limitations.

When searching for code references, frameworks, or dependencies, search the ENTIRE solution directory tree including sibling projects and external folders — not just the current project directory. Ask the user for the correct path if unsure.

## Configuration

- Use local `.claude/` folder (project-level) for project-specific settings like progress and plan files.
- Do NOT place project-specific config in the global `~/.claude/` directory unless explicitly instructed.
- When modifying CLAUDE.md or any configuration files, always read the existing file first and preserve existing conventions before making changes.

## Commands

```powershell
# Build
dotnet build                                                 # Full solution
dotnet build src/ISynergy.Framework.CQRS                    # Single project

# Test
dotnet test                                                  # All tests
dotnet test tests/ISynergy.Framework.Core.Tests              # Single project
dotnet test --filter "TestMethod=SomeTest"                   # Single test

# Pack NuGet packages
dotnet pack
```

## Critical Coding Rules

These cause bugs if violated. Full examples in [`.claude/reference/critical-rules.md`](.claude/reference/critical-rules.md).

| Rule | Correct | Wrong |
|-|-|-|
| CQRS commands | Individual parameters | Passing model objects |
| CQRS handler naming | `Create{Entity}CommandHandler` / `Get{Entity}ByIdQueryHandler` | Missing `CommandHandler`/`QueryHandler` suffix |
| Async | Always include `CancellationToken` | Omit or use `.Result` |
| Mapping | Manual inline mapping (direct property assignment) | AutoMapper, Mapster, or any other mapper library |
| Enum naming | Plural names (`PaymentProviders`, not `PaymentProvider`) | Singular names for non-`*Status` enums |
| Backwards compat | Mark old APIs `[Obsolete]` before removal | Silent breaking changes |
| All public APIs | XML documentation required | Undocumented public members |
| Configuration access | `IOptions<T>` injected via DI | `configuration["Key"]` or `GetValue<T>("Key")` |
| Config section name | `nameof(TOptions)` in `GetSection()` | Magic strings like `"KeyVault"` |
| Extension method naming | `Add{Provider}{Service}Integration` | Generic names like `AddKeyVaultIntegration` |
| Options class naming | `{Provider}{Service}Options` | Generic names like `PublisherOptions`, `MailOptions` |

## Session & Task Protocol

### Progress Tracking: Local Files Only

**Do NOT use the built-in `TaskCreate`/`TaskUpdate`/`TaskList` tools** — they store data globally in `~/.claude/todos/` and are not visible in the repository. Instead, use local markdown files in `.claude/progress/` for tracking task progress.

### Non-trivial Tasks (3+ steps / multi-file)
1. Call `EnterPlanMode` and wait for approval
2. Create `.claude/progress/{task-slug}.md` with step checklist
3. Mark steps done with `Edit` (`- [ ]` → `- [x]`) — never overwrite whole file
4. **On completion (mandatory, not optional):**
   - Edit the progress file: add `**Status:** DONE` near the top
   - Move the file to `.claude/completed/`
   - **Do not end the session without completing this step**

> **Why this matters:** Files left in `.claude/progress/` are treated as in-progress work in future sessions, causing confusion about what is still pending.

**When delegating to subagents**, include explicit progress file instructions — subagents do not inherit this CLAUDE.md.

### Subagent Template

**Valid `subagent_type` values** (skill names like `dotnet-engineer` are NOT valid agent types):

| Use case | `subagent_type` |
|-|-|
| Code implementation, CQRS, tests | `general-purpose` |
| File/codebase exploration | `Explore` |
| Implementation planning | `Plan` |
| Shell commands, git, build | `Bash` |

```
Progress file: .claude/progress/{task-slug}.md
After each step, Edit the file:
  old: "- [ ] {step}"
  new: "- [x] {step}"
Do NOT use Write on the progress file — only Edit individual lines.
```

## Work-Type Context Files

Load on demand:

| Task Type | Files to Load |
|-|-|
| CQRS | [`.claude/skills/dotnet-engineer/SKILL.md`](.claude/skills/dotnet-engineer/SKILL.md), [`.claude/patterns/cqrs-patterns.md`](.claude/patterns/cqrs-patterns.md), [`.claude/reference/critical-rules.md`](.claude/reference/critical-rules.md), [`.claude/reference/templates/command-handler.cs.txt`](.claude/reference/templates/command-handler.cs.txt), [`.claude/reference/templates/query-handler.cs.txt`](.claude/reference/templates/query-handler.cs.txt) |
| API Endpoints | [`.claude/patterns/api-patterns.md`](.claude/patterns/api-patterns.md), [`.claude/reference/templates/endpoint.cs.txt`](.claude/reference/templates/endpoint.cs.txt) |
| Unit Tests | [`.claude/skills/unit-tester/SKILL.md`](.claude/skills/unit-tester/SKILL.md), [`.claude/patterns/testing-patterns.md`](.claude/patterns/testing-patterns.md), [`.claude/reference/templates/test-class.cs.txt`](.claude/reference/templates/test-class.cs.txt) |
| E2E / UI Tests | [`.claude/skills/playwright-tester/SKILL.md`](.claude/skills/playwright-tester/SKILL.md) |
| Blazor UI | [`.claude/skills/blazor-specialist/SKILL.md`](.claude/skills/blazor-specialist/SKILL.md), [`.claude/patterns/mvvm.md`](.claude/patterns/mvvm.md) |
| MAUI | [`.claude/skills/maui-specialist/SKILL.md`](.claude/skills/maui-specialist/SKILL.md), [`.claude/patterns/mvvm.md`](.claude/patterns/mvvm.md) |
| Database | [`.claude/skills/database-migration/SKILL.md`](.claude/skills/database-migration/SKILL.md) |
| Architecture | [`.claude/skills/architect/SKILL.md`](.claude/skills/architect/SKILL.md) |
| Architecture Review | [`.claude/skills/design-interrogation/SKILL.md`](.claude/skills/design-interrogation/SKILL.md) |
| Code Review | [`.claude/skills/code-reviewer/SKILL.md`](.claude/skills/code-reviewer/SKILL.md), [`.claude/checklists/pre-submission.md`](.claude/checklists/pre-submission.md) |
| API Security | [`.claude/skills/api-security/SKILL.md`](.claude/skills/api-security/SKILL.md), [`.claude/patterns/api-patterns.md`](.claude/patterns/api-patterns.md) |
| App Security | [`.claude/skills/software-security/SKILL.md`](.claude/skills/software-security/SKILL.md) |
| Security Strategy | [`.claude/skills/security/SKILL.md`](.claude/skills/security/SKILL.md), [`.claude/skills/api-security/SKILL.md`](.claude/skills/api-security/SKILL.md) |
| Performance | [`.claude/skills/performance-engineer/SKILL.md`](.claude/skills/performance-engineer/SKILL.md), [`.claude/patterns/cqrs-patterns.md`](.claude/patterns/cqrs-patterns.md) |
| External Integration | [`.claude/skills/integration-specialist/SKILL.md`](.claude/skills/integration-specialist/SKILL.md), [`.claude/patterns/api-patterns.md`](.claude/patterns/api-patterns.md) |
| DevOps / CI/CD | [`.claude/skills/devops-engineer/SKILL.md`](.claude/skills/devops-engineer/SKILL.md) |
| Documentation | [`.claude/skills/technical-writer/SKILL.md`](.claude/skills/technical-writer/SKILL.md) |
| Refactoring | [`.claude/skills/refactor/SKILL.md`](.claude/skills/refactor/SKILL.md), [`.claude/reference/refactoring.md`](.claude/reference/refactoring.md) |

## Key Reference Files

- [`.claude/reference/critical-rules.md`](.claude/reference/critical-rules.md) — non-negotiable coding patterns with full examples
- [`.claude/reference/refactoring.md`](.claude/reference/refactoring.md) — refactoring conventions
- [`.claude/reference/forbidden-tech.md`](.claude/reference/forbidden-tech.md) — banned libraries/approaches
- [`.claude/reference/tokens.md`](.claude/reference/tokens.md) — template token definitions
- [`.claude/checklists/pre-submission.md`](.claude/checklists/pre-submission.md) — run before marking any task complete
- [`.claude/reference/templates/session-handoff.md.txt`](.claude/reference/templates/session-handoff.md.txt) — session handoff template

## Workflow Preferences

- When asked to 'build' or 'check the build', ONLY run the build command and report results. Do not autonomously investigate or fix errors unless explicitly asked to do so.
- A **PostToolUse hook** is configured in [`.claude/settings.json`](.claude/settings.json) that allows PowerShell and build commands to run automatically after file edits.

## File Management

- Never autonomously delete documentation files, progress tracking files, or session context files unless the user explicitly asks for deletion. Always ask before removing any non-code artifacts.
- This is a Windows environment using PowerShell. Do not use bash-specific syntax (e.g., `&&`, `/tmp/` paths). Use PowerShell-compatible commands and Windows paths.

## Documentation Maintenance

- When making architectural changes, update CLAUDE.md and the relevant [`.claude/`](.claude/) reference files in the same session.
- After completing a feature or refactor, verify that CLAUDE.md, [`.claude/reference/critical-rules.md`](.claude/reference/critical-rules.md), and [`.claude/patterns/cqrs-patterns.md`](.claude/patterns/cqrs-patterns.md) still accurately describe the codebase. Flag any drift to the user.
- Run `/verify-config` periodically to audit CLAUDE.md against the actual codebase.

**Where to place new rules** — do NOT add implementation details or code examples directly to CLAUDE.md:

| Content type | Destination |
|-|-|
| Coding patterns, examples | [`.claude/reference/critical-rules.md`](.claude/reference/critical-rules.md) |
| Refactoring rules | [`.claude/reference/refactoring.md`](.claude/reference/refactoring.md) |
| Blazor / MAUI UI rules (incl. ViewModel base class, `IsBusy`) | [`.claude/skills/blazor-specialist/SKILL.md`](.claude/skills/blazor-specialist/SKILL.md) or [`.claude/patterns/mvvm.md`](.claude/patterns/mvvm.md) |

## Process Management
- Always run `dotnet build-server shutdown` after completing a build/test session
- After finishing work on a project, kill lingering build server processes

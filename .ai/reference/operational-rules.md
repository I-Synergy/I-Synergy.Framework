# Operational Rules

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

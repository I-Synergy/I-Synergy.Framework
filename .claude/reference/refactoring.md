# Refactoring Conventions

## Scope & Discovery

- Before starting work, verify the scope by searching the ENTIRE solution/repo for the target pattern — not just the current project or domain. If referencing external projects or frameworks, ask the user for the correct path rather than assuming.
- When performing bulk refactoring across multiple files, always check ALL instances of the pattern — including string literals in exception messages, logger calls, and similar constructs — not just the primary target. After making bulk changes, do a final grep to verify zero remaining instances.

## Analysis Before Action

- Before applying bulk replacements, analyze ALL instances first. Categorize by whether the replacement is safe, unsafe, or needs modification. Present categories with examples and wait for user confirmation before editing any files.

## File Cleanup

- When doing large multi-file migrations or refactoring, always include file deletion of old/moved files as part of the plan. Do not skip cleanup steps expecting manual approval — include them but confirm with the user before executing.
- When a file migration or move is performed, ALWAYS delete the original source files AND remove any associated config/mapping files (e.g., MappingConfig, registration entries) from the old location. Do not wait for manual approval on file deletion during migrations.

## Batching Large Refactors

- For large refactors (15+ files), process in batches scoped to one domain at a time and run `dotnet build` between batches to catch regressions early.

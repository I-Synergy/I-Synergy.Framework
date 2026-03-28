---
name: verify-config
description: Audits CLAUDE.md and .claude/ reference files against actual codebase patterns. Use to detect configuration drift and ensure documentation stays in sync with the codebase.
---

# Verify Configuration Skill

Audits project documentation against actual codebase conventions to detect and fix drift.

## Steps

1. **Read current documentation**
   - Read `CLAUDE.md`
   - Read [`critical-rules.md`](../../reference/critical-rules.md)
   - Read [`cqrs-patterns.md`](../../patterns/cqrs-patterns.md)
   - Extract all documented conventions (data access patterns, naming, file structure, mapping, etc.)

2. **Sample the codebase for actual patterns**
   - Use `src/ISynergy.Framework.CQRS/` as the primary CQRS reference (interfaces, dispatchers, handlers)
   - Use `src/ISynergy.Framework.Core/` as the reference for base classes, extensions, Result/Option types
   - Search for concrete handler implementations across `src/` with `Grep pattern="ICommandHandler|IQueryHandler"`
   - Read 2-3 command/query handler implementations to verify naming and structure
   - Read service registration extensions to verify `Add{Provider}{Service}Integration` pattern
   - Check `Directory.Packages.props` for central package management conventions

3. **Compare and report**
   - For each documented convention, verify it matches the actual code
   - Check for patterns in the code that are NOT documented
   - Categorize findings:
     - **Correct** — documentation matches code
     - **Drift** — documentation contradicts code
     - **Missing** — code pattern not documented
     - **Stale** — documented pattern no longer used

4. **Check .claude/ folder structure**
   - Verify [`settings.json`](../../settings.json) has `plansDirectory` pointing to local `.claude/plans`
   - Verify [`progress/`](../../progress/) and [`plans/`](../../plans/) folders exist
   - Check that no project-specific config leaked to global `~/.claude/`

5. **Present findings**
   - Show a summary table of all conventions checked with their status
   - For each Drift/Missing/Stale finding, show the documented vs actual pattern
   - Propose specific edits to fix any drift found
   - Wait for user approval before making changes

## Output Format

```
## Configuration Audit Report

### Summary
- Conventions checked: N
- Correct: N
- Drift detected: N
- Missing documentation: N
- Stale documentation: N

### Findings

| # | Convention | Status | Details |
|---|-----------|--------|---------|
| 1 | Data access style | Correct/Drift/Missing/Stale | ... |
| 2 | ... | ... | ... |

### Recommended Fixes
1. ...
2. ...
```

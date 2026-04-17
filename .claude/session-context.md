# Session Handoff - 2026-04-13

## Task Summary
Ran `/verify-config` audit. Detected and fixed 3 configuration drift/stale issues across CLAUDE.md and .claude/ reference files. Removed all Phantom MCP references from the codebase.

## Decisions Made
- **FluentValidation removed from CLAUDE.md**: Root `CLAUDE.md` "Common Patterns" incorrectly listed FluentValidation. Corrected to "Data Annotations + `IValidatableObject`" to match `forbidden-tech.md` and actual codebase (no FluentValidation usage found in `src/`).
  - **Rationale**: `forbidden-tech.md` is the authoritative source; FluentValidation is not used anywhere
  - **Alternatives considered**: Removing the forbidden-tech.md entry — rejected, the framework deliberately avoids it

- **Phantom references removed**: All `phantom_ask` / `phantom_codebase_query` references stripped from `.claude/CLAUDE.md`.
  - **Rationale**: Phantom MCP is no longer part of this repository
  - **Impact**: Session context is now tracked exclusively via `.claude/progress/` + `.claude/completed/` + this file

- **AOT note added to cqrs-patterns.md**: `AddHandlers(assembly)` is `[RequiresUnreferencedCode]`. Source generator (`ISynergy.Framework.CQRS.SourceGenerator`) is the AOT-safe alternative via `AddCQRSHandlers()`.

- **session-context.md reinstated**: Rule #9 in `critical-rules.md` references this file. The file was missing; it has been recreated. `.claude/CLAUDE.md` session start updated to include reading this file.

- **Rule #9 kept**: User explicitly chose to keep `critical-rules.md` Rule #9 referencing `session-context.md` (option 2 over rewriting it to the progress-file workflow).

## Open Questions
- None

## Files Modified
- `CLAUDE.md` (root) — "Use guard clauses and FluentValidation" → "Use guard clauses and Data Annotations + `IValidatableObject`"
- `.claude/CLAUDE.md` — Removed Phantom section and all `phantom_*` references; added `session-context.md` back to session start
- `.claude/patterns/cqrs-patterns.md` — Added AOT note to Service Registration section
- `.claude/reference/critical-rules.md` — No changes (kept as-is per user)
- `.claude/session-context.md` — Recreated (this file)

## Next Actions
- No pending actions from this session

## Files to Load Next Session
- `.claude/reference/critical-rules.md` — always
- `.claude/patterns/cqrs-patterns.md` — for CQRS work

## Work Type for Next Session
Unknown — awaiting user direction

## Session Notes
- Phantom is gone. Do not reference it.
- `forbidden-tech.md` is the authoritative list of banned libraries — check it before suggesting any third-party package
- `ISynergy.Framework.CQRS.SourceGenerator` exists for AOT-compatible handler registration

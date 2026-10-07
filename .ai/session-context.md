# Session Handoff - 2026-10-06

**Written By:** Claude Code
**Next Session May Use:** Any

## Task Summary
Applied the AI template upgrade (`/upgrade-template`, non-interactive) and reconciled the repo with the new template layout:
- 53 files added (skill `references/` folders, `book-to-skill/scripts/extract.py`, `validate-agents-md.py`); `.claude/settings.json` merged; `upgrade-template.py` self-updated. 35 changed template files kept the project version.
- Between the upgrade and the audit, `CLAUDE.md`, `DEEPSEEK.md`, `REASONIX.md` and `.ai/chains/*.chain.md` were deleted from the working tree and a new `AGENTS.md` appeared. Cause not identified; the upgrade scripts contain no code that deletes them. User chose to treat it as an intentional migration to `AGENTS.md`.
- `verify-config` run against `AGENTS.md`; findings fixed (below).

## Decisions Made
- **AGENTS.md is the instruction file**; CLAUDE.md / DEEPSEEK.md / REASONIX.md are not restored.
  - **Rationale**: user chose option 2 (intentional migration).
  - **Alternatives considered**: `git restore` the old files — rejected by the user. Their uncommitted edits were never in git and are likely lost.
- **Pi chains dropped**: removed `.ai/chains` from `sync-skills.py` and `validate-settings.py`; deleted `validate-pi.py` and removed it from `run-all-tests.sh`, `test_suite.py` and the three test docs. `.pi/skills` and `.pi/agents` junctions are still created by `sync-skills.py`.
- **Project rules 14–16** (persisted enums, package-range bounds, no secrets) now live in `.ai/project/preferences.md`, because `AGENTS.md` is template-owned and overwritten on upgrade. `AGENTS.md` roster gained `programmer` and `winui-dev`; that edit may be overwritten by a future upgrade.
- **settings.json**: removed the stale DEEPSEEK.md sync hook and the older duplicate `Stop` build hook (builds were running twice).
- **Test runners retargeted** from CLAUDE.md/REASONIX.md to AGENTS.md: `validate-structure.sh`, `validate-references.sh`, `run-all-tests.sh`, `test_suite.py`. The Reasonix suite was removed from both runners.

## Open Questions
- **RESOLVED**: Standards surfaces written (`COMPLIANCE.md` and the `## Standards Compatibility` block in `README.md`, provenance `validators=none`). `standards.md` row 1 was changed to `Aligned` (validator `—`) because this project holds no `docs/slices/…` or `docs/test-reports/…`; `validate-standards.py` passes and the full suite is 12/12. The override is recorded in `.ai/project/preferences.md` → Template Overrides and may be reverted by an interactive upgrade.
- **OPEN**: `~/.reasonix/` (config.json, locks, memory) was not inspected for leaked project config.
- **ASSUMED**: Nothing in this session has been committed.

## Files Modified
`.claude/settings.json`, `.ai/scripts/{upgrade-template,sync-skills}.py`, `.ai/tests/{validate-settings.py,validate-structure.sh,validate-references.sh,run-all-tests.sh,test_suite.py,README.md,TESTING-GUIDE.md,QUICK-REFERENCE.md}`, `.ai/tests/validate-pi.py` (deleted), `.ai/tests/validate-tokens.sh`, `.ai/reference/{task-execution,standards}.md`, `.ai/project/preferences.md`, `AGENTS.md`, `README.md` (standards block appended), new `COMPLIANCE.md`, plus the added template files. Nothing committed.

## Next Actions
- Review and commit the working tree (note the unexpected deletions of `CLAUDE.md`, `DEEPSEEK.md`, `REASONIX.md`, `.ai/chains/`).
- `bash .ai/tests/run-all-tests.sh` passes 12/12 as of this session; re-run after any upgrade.

## Files to Load Next Session
- `.ai/session-context.md` — this file
- `AGENTS.md` — instruction file
- `.ai/project/preferences.md` — includes rules 14–16

## Session Notes
- The auto-mode classifier blocks edits to `.claude/settings.json` and file deletion from the agent; those were done by the user running a script and `rm` via the `!` prefix.
- Earlier handoff (2026-07-15, trim-warning work) is superseded here. Its decisions are recorded in `.ai/completed/fix-trim-warnings-winui.md` and `.ai/completed/fix-remaining-warnings.md`; the local `development/main` commits from that work were still unpushed and may still need a push decision.

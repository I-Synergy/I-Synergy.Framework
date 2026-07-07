---
name: verify-config
description: Audits CLAUDE.md, REASONIX.md, DEEPSEEK.md, and .ai/ reference files against actual codebase patterns and hard requirements. Use to detect configuration drift and ensure documentation stays in sync.
---

# Verify Configuration Skill

Audits project documentation against actual codebase conventions and enforces hard requirements
(task execution protocol, subagent delegation, multi-assistant sync integrity, session management).

## Steps

### 1. Read current documentation
   - Read `CLAUDE.md`
   - Read `REASONIX.md` (if exists)
   - Read `DEEPSEEK.md` (if exists)
   - Read `.ai/reference/critical-rules.md`
   - Read `.ai/reference/task-execution.md`
   - Read `.ai/reference/session-management.md`
   - Read `.ai/reference/templates/session-handoff.md.txt`
   - Read `.ai/session-context.md`
   - Read `.ai/patterns/cqrs-patterns.md`
   - Read `.claude/settings.json`

### 2. Codebase pattern audit (existing)
   - Pick a representative domain from `{ApplicationName}.Domain.*` as the reference implementation
   - Read 2-3 command handlers (Create, Update, Delete)
   - Read 2-3 query handlers (GetById, GetList)
   - Read 2-3 Model records from `Models/`
   - Read 2-3 Response records
   - Check `Extensions/ServiceCollectionExtensions.cs`
   - For each documented convention, verify it matches actual code
   - Categorize: **Correct** / **Drift** / **Missing** / **Stale**

### 3. Hard requirement checks

#### 3a. Task execution protocol
   - `.ai/progress/` folder exists
   - `.ai/completed/` folder exists
   - `.ai/plans/` folder exists
   - `CLAUDE.md` Task Execution section explicitly says progress files are MANDATORY
   - `REASONIX.md` (if exists) Task Execution section explicitly says progress files are MANDATORY
   - `DEEPSEEK.md` (if exists) Task Execution section explicitly says progress files are MANDATORY
   - `.ai/reference/task-execution.md` is tool-agnostic (no `EnterPlanMode` / `mv` references)

#### 3b. Subagent delegation
   - `CLAUDE.md` has a HARD RULE subagent delegation section naming what the main conversation may/may not do
   - `REASONIX.md` (if exists) has the same HARD RULE section
   - `DEEPSEEK.md` (if exists) has the same HARD RULE section
   - Agent tables in all files include a Model column
   - Model names use `deepseek-v4-pro`/`deepseek-v4-flash` (not sonnet/haiku)
   - Model mapping line exists: `deepseek-v4-pro` ↔ sonnet · `deepseek-v4-flash` ↔ haiku

#### 3c. Multi-assistant sync integrity
   - `DEEPSEEK.md` (if exists) exists alongside `CLAUDE.md` and `REASONIX.md`
   - `.claude/skills/` entries are thin wrappers (contain `!`cat .ai/skills/.../SKILL.md``)
   - `.github/skills/` entries are full content copies (do NOT contain `!`cat ...``)
   - `.reasonix/skills/` (if exists) skill-derived entries are thin wrappers (reference `.ai/skills/`)
   - `.reasonix/skills/` agent-derived entries have `runAs: subagent` and reference `.ai/agents/`
   - No stale dirs in `.reasonix/skills/` (all have an `.ai/` source)
   - `.claude/settings.json` has `./.reasonix` in `additionalDirectories`
   - `.claude/settings.json` sync hooks mention `.reasonix`
   - `.claude/settings.json` has a DEEPSEEK.md sync hook triggering on `.ai/reference/critical-rules.md`, `.ai/reference/task-execution.md`, `.ai/agents/**`, and `CLAUDE.md`
   - Sync scripts (`sync-skills.py`, `sync-agents.py`) push to `.reasonix/skills/`
   - `.gitignore` includes `CLAUDE.md.deepseek-backup`

#### 3d. Session management
   - `.ai/reference/session-management.md` has generic `[assistant name]` Written By (not hardcoded to one assistant)
   - `.ai/reference/session-management.md` lists all assistants that share the session context
   - `.ai/reference/templates/session-handoff.md.txt` includes all assistants in the pick-lists
   - `.ai/session-context.md` has no stale `[Claude Code | GitHub Copilot]`-only references

#### 3e. DEEPSEEK.md content integrity
   - `DEEPSEEK.md` inlines the subagent delegation table (same 8 agent types as `CLAUDE.md`)
   - `DEEPSEEK.md` inlines the most critical coding rules as direct content (not cross-references to `.ai/reference/critical-rules.md`)
   - `DEEPSEEK.md` inlines the task execution protocol (plan → progress file → complete cycle)
   - `DEEPSEEK.md` is self-contained — a DeepSeek model can follow it without resolving nested file references
   - `DEEPSEEK.md` structure is flatter than `CLAUDE.md` (fewer levels of indirection, fewer cross-references)
   - `DEEPSEEK.md` is ~180-220 lines (concise enough for DeepSeek context but comprehensive)
   - `DEEPSEEK.md` references `.ai/` folder only as an appendix for deep dives, not as required reading
   - `DEEPSEEK.md` acknowledges it runs inside Claude Code's runtime (tools available: Agent, Skill, Read, Write, Edit, Bash, Glob, Grep, EnterPlanMode, TaskCreate, etc.)

### 4. .ai/ folder structure
   - Verify `.claude/settings.json` has `plansDirectory` pointing to local `.ai/plans`
   - Verify `.ai/progress/` and `.ai/plans/` folders exist
   - Check that no project-specific config leaked to global `~/.claude/` or `~/.reasonix/`

### 5. Present findings
   - Show a summary table of all checks with their status
   - For each failure, show the specific gap and how to fix it
   - Categorize:
     - **Correct** — passes
     - **Missing** — required file/section doesn't exist
     - **Stale** — references wrong assistant names or model names
     - **Drift** — content contradicts the canonical pattern
   - Wait for user approval before making changes

## Output Format

```
## Configuration Audit Report

### Summary
- Conventions checked: N
- Correct: N
- Failures: N

### Hard Requirements

| # | Check | Status | Details |
|---|-------|--------|---------|
| 1 | Progress files mandatory in CLAUDE.md | PASS/FAIL | ... |
| 2 | Subagent delegation HARD RULE in REASONIX.md | PASS/FAIL | ... |
| 3 | Agent table has Model column | PASS/FAIL | ... |
| 4 | Model names are deepseek-v4-pro/flash | PASS/FAIL | ... |
| 5 | Model mapping documented | PASS/FAIL | ... |
| 6 | .claude/settings.json includes ./.reasonix | PASS/FAIL | ... |
| 7 | .reasonix/skills/ thin wrappers | PASS/FAIL | ... |
| 8 | .reasonix/skills/ agent skills runAs: subagent | PASS/FAIL | ... |
| 9 | Session management is assistant-agnostic | PASS/FAIL | ... |
| 10 | Handoff template includes all assistants | PASS/FAIL | ... |
| ... | ... | ... | ... |

### Codebase Pattern Audit

| # | Convention | Status | Details |
|---|-----------|--------|---------|
| 1 | Data access style | Correct/Drift/Missing/Stale | ... |

### Recommended Fixes
1. ...
2. ...
```

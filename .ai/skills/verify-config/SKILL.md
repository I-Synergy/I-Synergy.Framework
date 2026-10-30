---
name: verify-config
description: Audits AGENTS.md and .ai/ reference files against actual codebase patterns and hard requirements. Use to detect configuration drift and ensure documentation stays in sync. Use when you need to verify AGENTS.md and .ai/ references still match the codebase, or to compute the standards compatibility manifest.
---

# Verify Configuration Skill

Audits project documentation against actual codebase conventions and enforces hard requirements
(task execution protocol, subagent delegation, session management).

## Steps

### 1. Read current documentation
   - Read `AGENTS.md` (master orchestration file)
   - Read `.ai/reference/critical-rules.md`
   - Read `.ai/reference/task-execution.md`
   - Read `.ai/reference/session-management.md`
   - Read `.ai/reference/templates/session-handoff.md.txt`
   - Read `.ai/session-context.md`
   - Read `.ai/patterns/cqrs-patterns.md`

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
   - `AGENTS.md` Task Execution section explicitly says progress files are MANDATORY
   - `.ai/reference/task-execution.md` is tool-agnostic (no tool-specific references)

#### 3b. Subagent delegation
   - `AGENTS.md` has a HARD RULE subagent delegation section naming what the main conversation may/may not do
   - Agent rosters in `AGENTS.md` and `README.md` list every agent in `.ai/agents/` — no Model column; the tier lives in the agent's own frontmatter
   - `model:` frontmatter in `.ai/agents/*.md` is always a tier alias (`sonnet` or `haiku`) — never a concrete model name
   - Docs (`AGENTS.md`, `README.md`) hardcode no concrete model names — the `model:` tier alias in each agent's frontmatter names a tier slot (e.g., `sonnet` or `haiku`), and the concrete model bound to that tier is chosen by the backend runtime outside this repository

#### 3c. Junctions to supported tools
   - `.ai/skills/` and `.ai/agents/` are mirrored by junctions (Windows) / symlinks (Unix) into `.claude/`, `.reasonix/`, and `.github/`
   - Junction targets (`.claude/skills/`, `.claude/agents/`, `.reasonix/skills/`, `.reasonix/agents/`, `.github/skills/`) exist and resolve correctly
   - Agent files have `runAs: subagent` when needed (check `.ai/agents/` source)
   - `.gitignore` excludes all junction targets

#### 3d. Session management
   - `.claude/settings.json` has a `SessionStart` hook (on all sources: startup, resume, clear, compact) that prints `.ai/session-context.md` when it exists and its first line is not `# Session Context Template`, preceded by a one-line header, and lists the 5 newest files in `.ai/completed/`; exits 0 always
   - `AGENTS.md` Section 2 is titled `## Session Memory` and contains these exact strings:
     - "`.ai/session-context.md` is the shared memory for every client"
     - "In this template repository the file stays the unfilled placeholder"
   - `.github/copilot-instructions.md` Section titled `## Session Memory` is identical to the `AGENTS.md` section (word-for-word, no divergence)
   - `.ai/reference/session-management.md` reconciles with `AGENTS.md` (contains same core rules, longer-form detail)
   - `.ai/reference/templates/session-handoff.md.txt` includes all supported clients in pick-lists (Claude Code, GitHub Copilot, Reasonix Code)
   - `.ai/session-context.md` is the unfilled placeholder (first line is `# Session Context Template`); in this template repo, it never contains project state
   - `.ai/reference/session-management.md` has generic `[assistant name]` Written By (not hardcoded to one assistant)
   - `.ai/reference/session-management.md` lists all assistants that share the session context

### 4. Tool-specific configuration
   - `.claude/settings.json` exists with `plansDirectory` pointing to `.ai/plans`, `SessionStart` hook, and `PostToolUse` re-sync hooks
   - `.ai/progress/` and `.ai/plans/` folders exist
   - Check that no project-specific config leaked to global `~/.claude/`, `~/.reasonix/`, etc.

### 5. Present findings
   - Show a summary table of all checks with their status
   - For each failure, show the specific gap and how to fix it
   - Categorize:
     - **Correct** — passes
     - **Missing** — required file/section doesn't exist
     - **Stale** — references wrong assistant names or model names
     - **Drift** — content contradicts the canonical pattern
   - Wait for user approval before making changes

### 6. Standards-compatibility surfaces (generated from evidence)
   - **Applicability first:** generate only when the working tree holds a generated solution — at least
     one solution file (`*.sln` or `*.slnx`). Where none exists (this template repository is one),
     report the step as a no-op and write nothing.
   - Read `.ai/reference/standards.md` and parse it by its own *How This File Is Parsed* contract: the
     status table is the one whose header row immediately follows `## Compatibility`, and `Standard` is
     the row key. Never select a table by its `Compatibility` column — the vocabulary table has one too.
   - Run each `Evidence` row's proving validator and capture its exit code. Report a result only for a
     validator that actually ran.
   - Compute each row's status: an `Evidence` row keeps `Evidence` only when its validator exits `0`,
     every path in its `Implementing artifact` cell exists, and at least one pattern in its `Evidence a
     project produces` cell matches something the project produced — substitute `*` for each `{…}` and
     glob it against the working tree, so `docs/slices/{Domain}/{Entity}.{Operation}/test-plan.md` is
     tested as `docs/slices/*/*/test-plan.md`. Where a pattern matches nothing, the row is naming a
     file this project does not hold, and it renders one step lower — `Aligned` — with the cause in the
     generated `Note` column, naming what was not found. One step, never a fourth value, never a silent
     downgrade, and never a token the manifest does not carry for that row. The path and pattern tests
     belong to a generated solution, which is the same condition as the applicability bullet above —
     `.ai/tests/validate-standards.py` implements exactly these three causes, so a generator that
     disagrees with it is the divergence this step exists to avoid.
   - Regenerate both surfaces from their templates:
     - `COMPLIANCE.md` at the project root — the whole file, from `.ai/reference/templates/compliance.md.txt`
     - the `## Standards Compatibility` section in `README.md` — only the block between
       `<!-- BEGIN standards-status -->` and `<!-- END standards-status -->`, from
       `.ai/reference/templates/standards-section.md.txt`
   - Apply the marker rules exactly as `.ai/reference/templates/standards-section.md.txt` states them:
     - exactly one marker pair — unbalanced, duplicated or interleaved markers (`END` before its `BEGIN`)
       **stop generation** and report the line numbers; resolve by hand, never guess
     - markers absent — append the generated block at the end of `README.md`, markers included, and
       report where it was placed
     - everything outside the markers is left byte-identical
     - the provenance line sits inside the block, directly above the closing marker — exactly one, in
       the manifest's rule-8 shape
     - no badge, shield, logo or image
   - Show the proposed diff for both surfaces and **wait for approval** before writing — step 5's
     contract governs here too; this step is never a silent mutator.
   - After writing, `python .ai/tests/validate-standards.py` must pass; a recorded validator exit code
     that disagrees with a re-run fails the check.

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
| 1 | Progress files mandatory in AGENTS.md | PASS/FAIL | ... |
| 2 | Subagent delegation HARD RULE in AGENTS.md | PASS/FAIL | ... |
| 3 | Agent rosters list every `.ai/agents/` agent (no Model column) | PASS/FAIL | ... |
| 4 | Agent `model:` frontmatter uses tier aliases (`sonnet`/`haiku`) | PASS/FAIL | ... |
| 5 | Docs hardcode no concrete model names | PASS/FAIL | ... |
| 6 | .claude/settings.json has hooks and plansDirectory | PASS/FAIL | ... |
| 7 | Junctions to .ai/skills/ and .ai/agents/ resolve correctly | PASS/FAIL | ... |
| 8 | Session management is tool-agnostic | PASS/FAIL | ... |
| 9 | .gitignore excludes all junction targets | PASS/FAIL | ... |

### Codebase Pattern Audit

| # | Convention | Status | Details |
|---|-----------|--------|---------|
| 1 | Data access style | Correct/Drift/Missing/Stale | ... |

### Recommended Fixes
1. ...
2. ...
```

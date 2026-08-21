# Task Execution Protocol

## When to Plan

**On every non-trivial task (3+ steps or multi-file):**

1. **Create a plan** — use your assistant's planning tool (e.g. `submit_plan` in Reasonix, plan mode in Claude Code). Name the plan with a meaningful slug in `.ai/plans/{task-slug}.md`. Wait for approval before writing any code.
   - Use the same slug for the progress file (step 2) so both files stay linked
2. **Create a progress file** — immediately write `.ai/progress/{task-slug}.md` using this structure:
   ```
   # {Task Name}
   Status: IN PROGRESS
   Started: {date}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
3. **Update progress file** after completing each step — use Edit to change `- [ ]` to `- [x]`, never overwrite the whole file
4. **On completion (mandatory, not optional):**
   - Edit the progress file: add `**Status:** DONE` near the top
   - Move the file from `.ai/progress/{task-slug}.md` to `.ai/completed/{task-slug}.md`
   - If a copy already exists in `.ai/completed/`, delete the one in `progress/` instead
   - **Do not end the session without completing this step**

> **Why this matters:** Files left in `.ai/progress/` are treated as in-progress work in future sessions, causing confusion about what is still pending.

**Trivial tasks** (single file, obvious fix): skip plan mode and progress file.

## ReAct Loop — Observe, Reason, Fix, Repeat

Every action in a task follows this cycle. Do not mark a step done until it passes observation.

```
ACT → OBSERVE → pass? → mark step done, next step
                fail? → REASON → FIX → OBSERVE again
```

**Observe** immediately after every file edit (the PostToolUse build hook fires automatically):

| Change type | What to observe |
|-------------|----------------|
| Any code edit | Build output (hook fires automatically — read it) |
| New or changed handler/query | Build + `dotnet test` on affected project |
| Test file | `dotnet test` on the test project |
| Skill or pattern file | `python3 .ai/tests/validate-skills.py` |
| Settings / config | `python3 .ai/tests/validate-settings.py` |
| Multi-file change | Build + `bash .ai/tests/run-all-tests.sh` |

**Reason** before every fix attempt:
- Read the full error, not just the first line
- Identify root cause (not symptom)
- State the fix strategy explicitly before applying it
- If retrying, use a different approach — never repeat the same fix

**Exit conditions:**

| State | Action |
|-------|--------|
| Observation passes | Mark step done, proceed |
| Retry 1–2, different error | Reason + new fix + observe again |
| Same error appears twice | Change approach before attempting retry 3 |
| Retry 3, still failing | **Escalate** — stop and report to user |

**Escalation format** (after 3 failed retries):
```
ESCALATION: {step name}

Tried:
1. {fix attempted} → {error received}
2. {fix attempted} → {error received}
3. {fix attempted} → {error received}

Root cause hypothesis: {what I believe is wrong}
Options:
  A. {option}
  B. {option}

Which approach should I take?
```

## Progress Tracking: Local Files Only

**Do NOT use the built-in `TaskCreate`/`TaskUpdate`/`TaskList` tools** — they store data globally in `~/.claude/todos/` and are not visible in the repository. Instead, always use local `.ai/progress/` markdown files for tracking task progress. Plans are already configured to write locally via `plansDirectory` in `.claude/settings.json`.

## Subagent Template

**Code changes must be delegated.** Never use Edit or Write directly in the main conversation. All code implementation, edits, and file creation must be done by the `programmer` sub-agent (Haiku model). Only use Edit/Write yourself for non-code files (progress files, session context, plans, documentation).

**Valid `subagent_type` values** (skill names like `dotnet-engineer` are NOT valid agent types):

| Use case | `subagent_type` |
|-|-|
| Code implementation, CQRS, tests | `programmer` |
| File/codebase exploration | `Explore` |
| Implementation planning | `Plan` |
| Shell commands, git, build | `Bash` |

When delegating to a subagent via the Agent tool, always include in the task prompt:

```
Progress file: .ai/progress/{task-slug}.md
After completing each step, use the Edit tool to mark it done:
  old: "- [ ] {step description}"
  new: "- [x] {step description}"
Do NOT use Write on the progress file — only Edit individual lines.
```

Subagents do not inherit this CLAUDE.md. All progress instructions must be explicit in the task prompt.

## Task Definition Template

Use this structure for all tasks:

```markdown
# Task: [Task Name]

## Context Files Required
- [List files from Work-Type Context Mapping]

## Action
[What to do - specific, measurable]

## Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## Verify
Run pre-submission checklist: `.ai/checklists/pre-submission.md`

## Done
- Progress file moved to `.ai/completed/`
- Session context updated with learnings
- All acceptance criteria met
```

---
name: sync-platforms
description: Syncs the platform junctions for skills, agents, prompts and instructions from .ai/ to .claude/, .github/ and .reasonix/. Run after adding, editing, or removing an item in .ai/skills/, .ai/agents/, .ai/prompts/ or .ai/instructions/.
---

# Sync Platforms

Runs `.ai/scripts/sync-platforms.py` so every client reads the same `.ai/` source
through directory junctions.

## Steps

1. **Dry run first**
   - Run `python .ai/scripts/sync-platforms.py --dry-run`
   - Report the CREATE / RECREATE lines and any SKIP for a missing source directory
   - If nothing would change, report "All platforms are already in sync" and stop

2. **Apply**
   - Run `python .ai/scripts/sync-platforms.py`
   - Report the results

3. **Verify**
   - `.claude/skills`, `.github/skills` and `.reasonix/skills` point at `.ai/skills`
   - `.claude/agents`, `.github/agents` and `.reasonix/agents` point at `.ai/agents`
   - `.claude/commands` and `.github/prompts` point at `.ai/prompts`
   - `.claude/instructions` and `.github/instructions` point at `.ai/instructions`

## Output Format

```
## Platform Sync Report

Dry run:
  OK   .github/skills -> .ai/skills
  CREATE .github/prompts -> .ai/prompts

Applied. Result:
  1 junction(s) changed.

Verification: OK
```

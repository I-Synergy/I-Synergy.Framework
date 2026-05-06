---
name: update-skills
description: Syncs .claude/skills/ and .github/skills/ from .ai/skills/ (the source of truth). Run after adding, editing, or removing a skill in .ai/skills/.
---

# Update Skills Skill

Runs `sync-skills.py` to keep `.claude/skills/` and `.github/skills/` in sync with `.ai/skills/`.

## Steps

1. **Dry run first**
   - Run `python sync-skills.py --dry-run`
   - Report what would change (CREATE / UPDATE / REMOVE per target)
   - If nothing would change, report "All skills are already in sync" and stop

2. **Apply sync**
   - Run `python sync-skills.py`
   - Report the results

3. **Verify**
   - Confirm `.claude/skills/` wrappers contain `!`cat .ai/skills/.../SKILL.md``
   - Confirm `.github/skills/` copies are identical to their `.ai/skills/` source

## Output Format

```
## Skill Sync Report

Dry run:
  CREATE .claude/skills/new-skill/SKILL.md
  CREATE .github/skills/new-skill/SKILL.md

Applied. Result:
  2 file(s) changed.

Verification: OK
```

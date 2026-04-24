---
name: upgrade-template
description: Upgrades an existing project with improvements from the CLAUDE.MD template. Use when the user says "upgrade the template", "sync template changes to a project", "update project X with new skills", or wants to apply template improvements without losing project-specific context. Runs upgrade-template.py which copies new files and diffs changed ones, never touching project-owned paths.
---

# Upgrade Template

Applies template improvements to an existing project without overwriting project-specific content.

## What It Does

| Status | Meaning |
|--------|---------|
| `ADDED` | New file — does not exist in the project yet. Copied automatically. |
| `CHANGED` | File exists in both. Shows a diff. You choose: accept (overwrite) or skip (keep yours). |
| `UNCHANGED` | Identical in both. Silently skipped. |
| `SKIPPED (project-owned)` | Never touched — see list below. |

## Project-Owned Files (Never Touched)

- `CLAUDE.md` — project-specific instructions
- `.ai/session-context.md` — active session state
- `.ai/project/` — architecture, tech stack, domains
- `.ai/progress/`, `.ai/completed/`, `.ai/plans/` — task tracking
- `.ai/analysis/` — project analysis files
- `.github/copilot-instructions.md` — project-specific Copilot instructions
- `.claude/settings.local.json` — local settings

## Usage

### Full upgrade (interactive)

```bash
python .ai/scripts/upgrade-template.py \
  --source D:/Projects/Github/CLAUDE.MD \
  --target D:/Projects/MyProject/my-repo
```

### Preview only (no changes)

```bash
python .ai/scripts/upgrade-template.py \
  --source D:/Projects/Github/CLAUDE.MD \
  --target D:/Projects/MyProject/my-repo \
  --dry-run
```

### Skills only (fastest — just adds new skills)

```bash
python .ai/scripts/upgrade-template.py \
  --source D:/Projects/Github/CLAUDE.MD \
  --target D:/Projects/MyProject/my-repo \
  --skills-only
```

### Non-interactive (CI/scripted — copies new, skips changed)

```bash
python .ai/scripts/upgrade-template.py \
  --source D:/Projects/Github/CLAUDE.MD \
  --target D:/Projects/MyProject/my-repo \
  --non-interactive
```

## What Gets Synced

| Path | Behaviour |
|------|-----------|
| `.ai/skills/` | New skill dirs copied; changed skills diffed |
| `.ai/patterns/` | New pattern files copied; changed diffed |
| `.ai/reference/templates/` | New templates copied; changed diffed |
| `.ai/reference/critical-rules.md` | Diffed if changed |
| `.ai/reference/forbidden-tech.md` | Diffed if changed |
| `.ai/reference/tokens.md`, `glossary.md`, `naming-conventions.md` | Diffed if changed |
| `.ai/checklists/` | New checklists copied; changed diffed |
| `.ai/tests/` | New test scripts copied; changed diffed |
| `.ai/scripts/` | New scripts copied (including this one); changed diffed |

## After Upgrade

The script automatically runs `sync-skills.py` in the target project if new skills were added,
syncing them to `.claude/skills/` and `.github/skills/`.

If `sync-skills.py` is not yet present in the target, run `/update-skills` manually.

## Running This as Claude

When the user asks to upgrade a project, run the script with Bash:

```
python .ai/scripts/upgrade-template.py --source <template> --target <project> --dry-run
```

Show the dry-run output first. Confirm with the user before running without `--dry-run`.
For projects where the user trusts the changes, use `--non-interactive` for a clean run.

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
| `MERGED` | `.claude/settings.json` — new hooks/permissions added, project keys preserved. |
| `SKIPPED (project-owned)` | Never touched — see list below. |

## Project-Owned Files (Never Touched)

- `CLAUDE.md` — project-specific instructions
- `.ai/session-context.md` — active session state
- `.ai/project/` — architecture, tech stack, domains
- `.ai/progress/`, `.ai/completed/`, `.ai/plans/` — task tracking
- `.ai/analysis/` — project analysis files
- `.github/copilot-instructions.md` — project-specific Copilot instructions
- `.claude/settings.local.json` — never touched

## Special-Case Merged File

- `.claude/settings.json` — may be merged by the upgrade script (`ADDED`/`MERGED`/`UNCHANGED`); new hooks/permissions are added while existing project-specific config is preserved

## Template Source

The canonical template is always fetched from GitHub — no local clone needed:

```
https://github.com/I-Synergy/AI
```

## Usage

### Full upgrade (interactive)

```bash
python .ai/scripts/upgrade-template.py \
  --source https://github.com/I-Synergy/AI \
  --target D:/Projects/MyProject/my-repo
```

### Preview only (no changes)

```bash
python .ai/scripts/upgrade-template.py \
  --source https://github.com/I-Synergy/AI \
  --target D:/Projects/MyProject/my-repo \
  --dry-run
```

### Skills only (fastest — just adds new skills)

```bash
python .ai/scripts/upgrade-template.py \
  --source https://github.com/I-Synergy/AI \
  --target D:/Projects/MyProject/my-repo \
  --skills-only
```

### Non-interactive (CI/scripted — copies new, skips changed)

```bash
python .ai/scripts/upgrade-template.py \
  --source https://github.com/I-Synergy/AI \
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
| `.claude/settings.json` | `ADDED` (new file): created from template, `enabledPlugins` excluded. `MERGED` (existing file): new hooks and permissions added, existing config (including `enabledPlugins`) preserved. |

## After Upgrade

The script automatically runs `sync-skills.py` in the target project after every upgrade,
syncing all skills to Claude Code and GitHub Copilot targets.

## Running This as Claude

**Source is always the GitHub template repo. Target is always the current working directory.**
Do not ask the user for either — derive them automatically.

1. Run a dry-run first and show the output:

```bash
python .ai/scripts/upgrade-template.py \
  --source https://github.com/I-Synergy/AI \
  --target . \
  --dry-run
```

2. Ask the user to confirm before applying changes.
3. Run without `--dry-run` (add `--non-interactive` if the user says to apply all changes):

```bash
python .ai/scripts/upgrade-template.py \
  --source https://github.com/I-Synergy/AI \
  --target .
```

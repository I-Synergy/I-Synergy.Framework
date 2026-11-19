---
name: upgrade-template
description: Upgrades an existing project with improvements from the template. Use when the user says "upgrade the template", "sync template changes to a project", "update project X with new skills", or wants to apply template improvements without losing project-specific context. Runs upgrade-template.py which copies new files and diffs changed ones, never touching project-owned paths.
---

# Upgrade Template

Applies template improvements to an existing project without overwriting project-specific content.

## What It Does

| Status | Meaning |
|--------|---------|
| `ADDED` | New file — does not exist in the project yet. Copied automatically. |
| `CHANGED` | File exists in both. Shows a diff. You choose: accept (overwrite) or skip (keep yours). |
| `UNCHANGED` | Identical in both. No action needed. |
| `MERGED` | `.vscode/settings.json`, `.vscode/tasks.json`, `.claude/settings.json` — template keys added, project values kept (see "Merged Files"). |
| `SKIPPED (project-owned)` | Never touched — see list below. |

## Project-Owned Files (Never Touched)

- `.ai/session-context.md` — active session state
- `.ai/project/` — architecture, tech stack, domains
- `.ai/progress/`, `.ai/completed/`, `.ai/plans/` — task tracking
- `.ai/analysis/` — project analysis files
- `.github/copilot-instructions.md` — project-specific Copilot instructions
- `.claude/settings.local.json` — never touched

## Merged Files

These are never copied or diffed as whole files. The script merges the template's shipped configuration into the project's copy (`ADDED`/`MERGED`/`UNCHANGED`/error):

- `.vscode/settings.json` — the fragment `.ai/reference/templates/vscode/settings.json` is merged. Missing keys are added, objects merge key by key, the pinned Copilot keys (`merge-rules.json`) take the template value, and any other conflict keeps the project's value and is reported.
- `.vscode/tasks.json` — the fragment `.ai/reference/templates/vscode/tasks.json` is merged. Tasks merge by `label`; an existing label is kept unchanged.
- `.claude/settings.json` — legacy hook commands listed in `merge-rules.json` are removed, template hook groups are added unless already present, `permissions` lists are unioned, and other keys follow the missing-key / project-wins rule.

Comments and formatting are preserved. Invalid JSON in the project file makes the script write nothing for that file and report an error. A second run changes nothing. If the template ref has no `.vscode` fragments, the `.vscode` merge is skipped and the template's own `.vscode` files are never copied.

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
| `AGENTS.md` | Copied if the project has none; diffed if changed in target |
| `.ai/skills/` | New skill dirs copied; changed skills diffed |
| `.ai/patterns/` | New pattern files copied; changed diffed |
| `.ai/reference/templates/` | New templates copied; changed diffed |
| `.ai/reference/critical-rules.md` | Diffed if changed |
| `.ai/reference/forbidden-tech.md` | Diffed if changed |
| `.ai/reference/tokens.md`, `glossary.md`, `naming-conventions.md`, `copilot-integration.md`, `session-management.md` | Diffed if changed |
| `.ai/checklists/` | New checklists copied; changed diffed |
| `.ai/tests/` | New test scripts copied; changed diffed |
| `.ai/scripts/` | New scripts copied (including this one); changed diffed |
| `.claude/settings.json` | Merged, never copied over. `ADDED` (new file): created from template, `enabledPlugins` excluded. `MERGED` (existing file): legacy hook commands removed, template hooks and permission entries added, other keys added when missing, project values kept (including `enabledPlugins`). |
| `.vscode/settings.json`, `.vscode/tasks.json` | Merged from `.ai/reference/templates/vscode/` (see "Merged Files"). Not template-owned, so an accepted diff never overwrites the project's file. |

## Migrating from CLAUDE.md, REASONIX.md, or DEEPSEEK.md

If your project has a legacy CLAUDE.md, REASONIX.md, or DEEPSEEK.md from an earlier template version, the upgrade script will NOT touch these files — they remain untouched so you don't lose project-specific customizations.

**To complete the migration:**

1. After upgrading, review any project-specific content in the old file (CLAUDE.md, etc.)
2. Merge custom sections into the new `AGENTS.md` (the master file for all AI clients)
3. Delete the old file once migration is complete
4. Run `python .ai/scripts/sync-platforms.py` to synchronize all skills

**Example:** If your CLAUDE.md has custom agent definitions or work-type mappings, copy those sections into AGENTS.md, adapt them to the new structure, then delete the old file.

For help migrating old `.claude/` layouts, `.reasonix/`, or other client-specific folders, use: `python .ai/scripts/migrate-to-ai.py`

## After Upgrade

The script automatically runs `sync-platforms.py` and `sync-agents.py` in the target project after every upgrade,
syncing all skills to Claude Code and GitHub Copilot targets. Any `.reasonix/` junctions left over from
earlier template versions are removed.

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

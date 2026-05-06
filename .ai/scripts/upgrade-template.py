#!/usr/bin/env python3
"""
upgrade-template.py — Sync template improvements to an existing project.

Copies new template files and shows diffs for changed ones.
Never touches project-owned files (CLAUDE.md, session-context, project/ etc.).

Usage:
    python upgrade-template.py --source <template-repo> --target <project-repo>
    python upgrade-template.py --source https://github.com/org/repo --target <project-repo>
    python upgrade-template.py --source <template-repo> --target <project-repo> --dry-run
    python upgrade-template.py --source <template-repo> --target <project-repo> --non-interactive
    python upgrade-template.py --source <template-repo> --target <project-repo> --skills-only

When --source is a URL (https:// or git@), the repo is shallow-cloned to a
temporary directory and cleaned up automatically after the upgrade completes.
"""

import argparse
import copy
import difflib
import json
import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

# ─── Ownership tables ────────────────────────────────────────────────────────

# Template owns these — new files are copied, changed files are diffed.
# Entries can be files or directories (directories are walked recursively).
TEMPLATE_OWNED = [
    ".ai/skills",
    ".ai/patterns",
    ".ai/reference/templates",
    ".ai/reference/critical-rules.md",
    ".ai/reference/forbidden-tech.md",
    ".ai/reference/tokens.md",
    ".ai/reference/glossary.md",
    ".ai/reference/naming-conventions.md",
    ".ai/checklists",
    ".ai/tests",
    ".ai/scripts",
]

# Project owns these — never read, never write.
PROJECT_OWNED = [
    "CLAUDE.md",
    ".ai/session-context.md",
    ".ai/project",
    ".ai/progress",
    ".ai/completed",
    ".ai/plans",
    ".ai/analysis",
    ".github/copilot-instructions.md",
    ".claude/settings.local.json",
]

# ─── ANSI colours ────────────────────────────────────────────────────────────

GREEN  = "\033[32m"
YELLOW = "\033[33m"
CYAN   = "\033[36m"
RED    = "\033[31m"
RESET  = "\033[0m"
BOLD   = "\033[1m"

def c(colour: str, text: str) -> str:
    return f"{colour}{text}{RESET}" if sys.stdout.isatty() else text

# ─── Helpers ─────────────────────────────────────────────────────────────────

def is_project_owned(rel: Path) -> bool:
    rel_str = rel.as_posix()
    for po in PROJECT_OWNED:
        if rel_str == po or rel_str.startswith(po + "/"):
            return True
    return False


def read_text(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except Exception:
        return ""


def show_diff(src_text: str, tgt_text: str, rel: Path) -> None:
    diff = list(difflib.unified_diff(
        tgt_text.splitlines(keepends=True),
        src_text.splitlines(keepends=True),
        fromfile=f"project/{rel}",
        tofile=f"template/{rel}",
        n=3,
    ))
    if not diff:
        return
    # Print first 60 lines of diff to keep it readable
    for i, line in enumerate(diff):
        if i >= 60:
            remaining = len(diff) - 60
            print(f"  ... {remaining} more lines ...")
            break
        if line.startswith("+"):
            print(c(GREEN, line), end="")
        elif line.startswith("-"):
            print(c(RED, line), end="")
        elif line.startswith("@@"):
            print(c(CYAN, line), end="")
        else:
            print(line, end="")


def prompt_action(rel: Path, non_interactive: bool) -> str:
    """Returns 'accept', 'skip', or 'quit'."""
    if non_interactive:
        return "skip"
    while True:
        choice = input(f"\n  [{c(GREEN,'a')}]ccept  [{c(YELLOW,'s')}]kip  [{c(RED,'q')}]uit > ").strip().lower()
        if choice in ("a", "accept"):
            return "accept"
        if choice in ("s", "skip", ""):
            return "skip"
        if choice in ("q", "quit"):
            return "quit"


def collect_template_files(source: Path) -> list[Path]:
    """Return all relative paths that the template owns."""
    result: list[Path] = []
    for entry in TEMPLATE_OWNED:
        src_path = source / entry
        if not src_path.exists():
            continue
        if src_path.is_file():
            result.append(Path(entry))
        elif src_path.is_dir():
            for f in sorted(src_path.rglob("*")):
                if f.is_file():
                    result.append(f.relative_to(source))
    return result


# ─── Settings merge ───────────────────────────────────────────────────────────

def _hook_commands(group: dict) -> set:
    return {h["command"] for h in group.get("hooks", []) if "command" in h}


def merge_settings_json(source: Path, target: Path, dry_run: bool) -> None:
    template_path = source / ".claude" / "settings.json"
    project_path  = target / ".claude" / "settings.json"

    if not template_path.exists():
        return

    template = json.loads(template_path.read_text(encoding="utf-8"))

    if not project_path.exists():
        print(f"  {c(GREEN, 'ADDED')}    .claude/settings.json")
        if not dry_run:
            project_path.parent.mkdir(parents=True, exist_ok=True)
            project_path.write_text(
                json.dumps({k: v for k, v in template.items() if k != "enabledPlugins"}, indent=2),
                encoding="utf-8",
            )
        return

    project = json.loads(project_path.read_text(encoding="utf-8"))
    merged  = copy.deepcopy(project)
    added: list[str] = []

    # plansDirectory — take from template
    if template.get("plansDirectory") and merged.get("plansDirectory") != template["plansDirectory"]:
        merged["plansDirectory"] = template["plansDirectory"]
        added.append(f"plansDirectory → {template['plansDirectory']!r}")

    # hooks — add groups whose commands are not yet present in the project
    for event, groups in template.get("hooks", {}).items():
        existing_commands: set = set()
        for grp in merged.get("hooks", {}).get(event, []):
            existing_commands |= _hook_commands(grp)
        for grp in groups:
            new_cmds = _hook_commands(grp)
            if new_cmds and not new_cmds.issubset(existing_commands):
                merged.setdefault("hooks", {}).setdefault(event, []).append(grp)
                existing_commands |= new_cmds
                added.append(f"hook [{event}]: {', '.join(sorted(new_cmds))}")

    # permissions — union of allow, deny, additionalDirectories
    for key in ("allow", "deny", "additionalDirectories"):
        template_vals = template.get("permissions", {}).get(key, [])
        project_list  = merged.setdefault("permissions", {}).setdefault(key, [])
        for val in template_vals:
            if val not in project_list:
                project_list.append(val)
                added.append(f"permissions.{key}: {val!r}")

    if added:
        print(f"  {c(GREEN, 'MERGED')}   .claude/settings.json")
        for item in added:
            print(f"             + {item}")
        if not dry_run:
            project_path.write_text(json.dumps(merged, indent=2), encoding="utf-8")
    else:
        print(f"  {c(CYAN, 'UNCHANGED')} .claude/settings.json")


# ─── Main ─────────────────────────────────────────────────────────────────────

def main() -> None:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")

    parser = argparse.ArgumentParser(description="Upgrade a project from the CLAUDE.MD template.")
    parser.add_argument("--source", required=True, help="Path to the template repository")
    parser.add_argument("--target", required=True, help="Path to the project to upgrade")
    parser.add_argument("--dry-run", action="store_true", help="Show what would happen without making changes")
    parser.add_argument("--non-interactive", action="store_true", help="Never prompt — copy new, skip changed")
    parser.add_argument("--skills-only", action="store_true", help="Only sync .ai/skills/")
    args = parser.parse_args()

    target = Path(args.target).resolve()
    if not target.exists():
        print(c(RED, f"ERROR: target not found: {target}"))
        sys.exit(1)

    tmpdir = None
    raw_source = args.source
    if raw_source.startswith(("https://", "git@", "http://")):
        tmpdir = tempfile.mkdtemp(prefix="upgrade-template-")
        print(f"\n{c(BOLD, 'Cloning template ...')}")
        print(f"  {raw_source}")
        result = subprocess.run(
            ["git", "clone", "--depth", "1", raw_source, tmpdir],
            capture_output=True, text=True, errors="replace",
        )
        if result.returncode != 0:
            shutil.rmtree(tmpdir, ignore_errors=True)
            print(c(RED, f"ERROR: git clone failed:\n{result.stderr.strip()}"))
            sys.exit(1)
        source = Path(tmpdir)
    else:
        source = Path(raw_source).resolve()
        if not source.exists():
            print(c(RED, f"ERROR: source not found: {source}"))
            sys.exit(1)

    try:
        _run_upgrade(args, source, target, raw_source, tmpdir)
    finally:
        if tmpdir:
            shutil.rmtree(tmpdir, ignore_errors=True)


def _run_upgrade(args, source: Path, target: Path, raw_source: str, tmpdir) -> None:
    print(f"\n{c(BOLD, 'Template Upgrade')}")
    print(f"  Source : {raw_source}")
    print(f"  Target : {target}")
    if args.dry_run:
        print(f"  Mode   : {c(YELLOW, 'DRY RUN — no files will be written')}")
    elif args.non_interactive:
        print(f"  Mode   : non-interactive (new files copied, changed files skipped)")
    print()

    files = collect_template_files(source)
    if args.skills_only:
        files = [f for f in files if f.parts[0:2] == (".ai", "skills") or
                                      (len(f.parts) > 1 and f.parts[0] == ".ai" and f.parts[1] == "skills")]

    counts = {"added": 0, "updated": 0, "skipped_unchanged": 0,
              "skipped_project": 0, "skipped_diff": 0, "quit": False}

    for rel in files:
        # Never touch project-owned paths
        if is_project_owned(rel):
            counts["skipped_project"] += 1
            continue

        src_file = source / rel
        tgt_file = target / rel
        src_text = read_text(src_file)

        if not tgt_file.exists():
            # New file — copy it
            print(f"  {c(GREEN, 'ADDED')}    {rel}")
            if not args.dry_run:
                tgt_file.parent.mkdir(parents=True, exist_ok=True)
                tgt_file.write_text(src_text, encoding="utf-8")
            counts["added"] += 1
        else:
            tgt_text = read_text(tgt_file)
            if src_text == tgt_text:
                counts["skipped_unchanged"] += 1
                continue

            # Changed file — show diff and prompt
            print(f"\n  {c(YELLOW, 'CHANGED')}  {rel}")
            show_diff(src_text, tgt_text, rel)

            action = prompt_action(rel, args.non_interactive)
            if action == "accept":
                print(f"  {c(GREEN, '→ accepted')}")
                if not args.dry_run:
                    tgt_file.write_text(src_text, encoding="utf-8")
                counts["updated"] += 1
            elif action == "quit":
                print(c(YELLOW, "\nAborted by user."))
                counts["quit"] = True
                break
            else:
                print(f"  {c(CYAN, '→ skipped')}")
                counts["skipped_diff"] += 1

    # ─── Merge .claude/settings.json ─────────────────────────────────────────
    if not counts["quit"]:
        merge_settings_json(source, target, args.dry_run)

    # ─── Post-sync: run sync-skills.py in target ──────────────────────────────
    if not args.dry_run and not counts["quit"]:
        sync_script = target / ".ai" / "scripts" / "sync-skills.py"
        if sync_script.exists():
            print(f"\n{c(CYAN, 'Syncing skills to Claude Code and GitHub Copilot targets ...')}")
            result = subprocess.run(
                [sys.executable, str(sync_script)],
                cwd=str(target),
                capture_output=True,
                text=True,
                errors="replace",
            )
            if result.returncode == 0:
                print(c(GREEN, "  ✓ Skills synced"))
            else:
                print(c(YELLOW, f"  ⚠ sync-skills.py exited {result.returncode}"))
                if result.stderr:
                    print(f"    {result.stderr.strip()}")
        else:
            print(f"\n{c(YELLOW, 'Note:')} run /update-skills in the target project to sync skills to Claude Code and GitHub Copilot")

    # ─── Summary ──────────────────────────────────────────────────────────────
    print(f"\n{c(BOLD, 'Summary')}")
    print(f"  {c(GREEN,  str(counts['added'])    ):>6}  added")
    print(f"  {c(GREEN,  str(counts['updated'])  ):>6}  updated")
    print(f"  {c(CYAN,   str(counts['skipped_unchanged'])):>6}  unchanged (skipped)")
    print(f"  {c(YELLOW, str(counts['skipped_diff'])):>6}  changed but kept project version")
    print(f"  {c(YELLOW, str(counts['skipped_project'])):>6}  project-owned (never touched)")

    if args.dry_run:
        print(f"\n{c(YELLOW, 'Dry run — no files were written.')}")
    elif not counts["quit"]:
        print(f"\n{c(GREEN, '✓ Upgrade complete.')}")


if __name__ == "__main__":
    main()

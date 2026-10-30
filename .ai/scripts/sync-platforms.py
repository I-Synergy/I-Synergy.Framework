#!/usr/bin/env python3
"""
Ensure folder-level junctions exist from .ai/skills/, .ai/agents/,
.ai/instructions/ and .ai/prompts/ to the folders of the clients selected in
`.ai/ai.json`:

  claude     .claude/skills, .claude/agents, .claude/commands, .claude/instructions
  copilot    .github/skills, .github/agents, .github/prompts, .github/instructions
  reasonix   .reasonix/skills, .reasonix/agents

Disabling a client removes only its junctions. A real directory is never
deleted, and `.github` is never removed because it also holds workflows. A
manifest without `clients` keeps every client (backward compatible).

Usage:
    python sync-platforms.py              # reconcile the selected clients
    python sync-platforms.py --dry-run    # show what would change without writing
"""

import argparse
import json
import os
import subprocess
import sys
from pathlib import Path

SCRIPT_DIR = Path(__file__).parent.parent.parent  # .ai/scripts/ -> .ai/ -> repo root

CLIENTS = ("claude", "copilot", "reasonix")
# (client, source, target)
JUNCTIONS = (
    ("claude", ".ai/skills", ".claude/skills"),
    ("copilot", ".ai/skills", ".github/skills"),
    ("reasonix", ".ai/skills", ".reasonix/skills"),
    ("claude", ".ai/agents", ".claude/agents"),
    ("copilot", ".ai/agents", ".github/agents"),
    ("reasonix", ".ai/agents", ".reasonix/agents"),
    ("claude", ".ai/instructions", ".claude/instructions"),
    ("copilot", ".ai/instructions", ".github/instructions"),
    ("claude", ".ai/prompts", ".claude/commands"),
    ("copilot", ".ai/prompts", ".github/prompts"),
)
# Client roots. Removed only when empty, so `.github/workflows` and project
# files always keep their folder alive.
CLIENT_DIRS = (".claude", ".github", ".reasonix")


def _manifest_clients() -> set:
    """Clients selected in .ai/ai.json; missing field means all (legacy).

    The resolver passes `AI_CLIENTS` when it runs this script, which also
    covers a first sync on a repository that has no manifest yet.
    """
    override = os.environ.get("AI_CLIENTS")
    if override is not None:
        names = {value.strip().lower() for value in override.split(",") if value.strip()}
        return names & set(CLIENTS)

    path = SCRIPT_DIR / ".ai" / "ai.json"
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError):
        return set(CLIENTS)
    if not isinstance(data, dict):
        return set(CLIENTS)
    raw = data.get("clients")
    if raw is None:
        return set(CLIENTS)
    if not isinstance(raw, list):
        return set(CLIENTS)
    names = {str(value).strip().lower() for value in raw if str(value).strip()}
    return names & set(CLIENTS)


def _is_junction(path: Path) -> bool:
    """Check if a directory is a Windows junction (reparse point)."""
    if sys.platform != "win32":
        return path.is_symlink()
    try:
        import ctypes
        FILE_ATTRIBUTE_REPARSE_POINT = 0x400
        attrs = ctypes.windll.kernel32.GetFileAttributesW(str(path))
        return attrs != -1 and bool(attrs & FILE_ATTRIBUTE_REPARSE_POINT)
    except Exception:
        return False


def _rmdir(path: Path):
    """Remove a directory, symlink, or junction safely.

    Avoids shutil.rmtree() for symlinks — rtree follows symlinks and would
    delete the target directory instead of just the symlink.
    """
    if path.is_symlink():
        path.unlink()
    elif sys.platform == "win32" and _is_junction(path):
        subprocess.run(["cmd", "/c", "rmdir", str(path)], capture_output=True)
    elif path.is_dir():
        import shutil
        shutil.rmtree(str(path), ignore_errors=True)
    elif path.exists():
        path.unlink()


def _create_link(target: Path, source: Path) -> bool:
    """Create a junction (Windows) or symlink (Unix). Returns True on success."""
    target.parent.mkdir(parents=True, exist_ok=True)
    if sys.platform == "win32":
        result = subprocess.run(
            ["cmd", "/c", "mklink", "/J", str(target), str(source)],
            capture_output=True, text=True
        )
        return result.returncode == 0
    else:
        # Use relative symlink for portability
        try:
            rel_source = os.path.relpath(str(source), str(target.parent))
        except ValueError:
            rel_source = str(source)
        target.symlink_to(rel_source, target_is_directory=True)
        return True


def sync_junction(source_rel: str, target_rel: str, dry_run: bool) -> str:
    """Ensure target_rel is a junction/symlink pointing to source_rel. Returns status."""
    root = SCRIPT_DIR
    target = root / target_rel
    source = (root / source_rel).resolve()

    if not source.is_dir():
        return "SKIP"

    if target.exists():
        if _is_junction(target):
            # Junction/symlink exists — verify it points correctly by checking
            # that the first file/subdir of the source is reachable through it
            first_entry = None
            for entry in source.iterdir():
                first_entry = entry.name
                break
            if first_entry and (target / first_entry).exists():
                return "OK"
            # Wrong target or broken — remove and recreate
            action = "RECREATE"
        else:
            action = "RECREATE"
    else:
        action = "CREATE"

    if not dry_run:
        if target.exists():
            _rmdir(target)
        ok = _create_link(target, source)
        if not ok:
            return "FAIL — could not create junction/symlink"

    return action


def remove_junction(target_rel: str, dry_run: bool) -> str:
    """Remove a disabled client's junction; never delete a real directory."""
    target = SCRIPT_DIR / target_rel
    if not (target.exists() or target.is_symlink() or _is_junction(target)):
        return "ABSENT"
    if _is_junction(target) or target.is_symlink():
        if not dry_run:
            _rmdir(target)
        return "REMOVED"
    return "KEEP"


def prune_empty_client_dirs(dry_run: bool) -> list:
    """Remove `.claude`/`.reasonix` only when nothing else lives there."""
    removed = []
    for rel in CLIENT_DIRS:
        path = SCRIPT_DIR / rel
        if not path.is_dir() or _is_junction(path):
            continue
        try:
            if any(path.iterdir()):
                continue
            if not dry_run:
                path.rmdir()
            removed.append(rel)
        except OSError:
            continue
    return removed


def sync_all(dry_run: bool = False) -> int:
    prefix = "[DRY RUN] " if dry_run else ""
    clients = _manifest_clients()
    print(f"{prefix}Ensuring folder-level junctions for: "
          f"{', '.join(name for name in CLIENTS if name in clients) or '(no clients)'}")

    changed = 0
    for client, source_rel, target_rel in JUNCTIONS:
        if client in clients:
            result = sync_junction(source_rel, target_rel, dry_run)
            if result == "OK":
                print(f"  OK   {target_rel} -> {source_rel}")
            elif result == "SKIP":
                print(f"  SKIP {target_rel} -> {source_rel} (source missing)")
            else:
                label = f"WOULD {result}" if dry_run else result
                print(f"  {label} {target_rel} -> {source_rel}")
                changed += 1
        else:
            result = remove_junction(target_rel, dry_run)
            if result == "REMOVED":
                label = "WOULD REMOVE" if dry_run else "REMOVED"
                print(f"  {label} {target_rel} ({client} disabled)")
                changed += 1
            elif result == "KEEP":
                print(f"  KEEP {target_rel} (not a junction; {client} disabled)")

    for rel in prune_empty_client_dirs(dry_run):
        label = "WOULD REMOVE" if dry_run else "REMOVED"
        print(f"  {label} {rel} (empty)")

    print(f"\n{changed} item(s) {'would be ' if dry_run else ''}changed.")
    return 0


def main():
    parser = argparse.ArgumentParser(
        description="Ensure junctions for the clients selected in .ai/ai.json"
    )
    parser.add_argument("--dry-run", action="store_true", help="Show changes without writing")
    args = parser.parse_args()
    sys.exit(sync_all(dry_run=args.dry_run))


if __name__ == "__main__":
    main()

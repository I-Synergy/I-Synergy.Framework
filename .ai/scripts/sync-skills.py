#!/usr/bin/env python3
"""
Sync skill files from .ai/skills/ (the single source of truth) to:
  - .claude/skills/  thin wrappers using !`cat ...` (Claude Code executes these)
  - .github/skills/  full content copies       (GitHub Copilot reads these directly)

Usage:
    python sync-skills.py              # sync all skills to both targets
    python sync-skills.py --from-hook  # hook mode: reads tool JSON from stdin,
                                       # only acts on .ai/skills/ writes
    python sync-skills.py --dry-run    # show what would change without writing
"""

import argparse
import json
import re
import shutil
import sys
from pathlib import Path


SCRIPT_DIR   = Path(__file__).parent.parent.parent  # .ai/scripts/ -> .ai/ -> repo root
AI_SKILLS    = SCRIPT_DIR / ".ai"     / "skills"
CLAUDE_SKILLS = SCRIPT_DIR / ".claude" / "skills"
GITHUB_SKILLS = SCRIPT_DIR / ".github" / "skills"

FRONTMATTER_RE = re.compile(r"^---\s*\n(.*?)\n---", re.DOTALL)
FIELD_RE       = re.compile(r"^(\w[\w-]*):\s*(.+)$", re.MULTILINE)


# ── helpers ──────────────────────────────────────────────────────────────────

def parse_frontmatter(path: Path) -> dict:
    try:
        content = path.read_text(encoding="utf-8")
    except (OSError, UnicodeDecodeError):
        return {}
    m = FRONTMATTER_RE.match(content)
    if not m:
        return {}
    return dict(FIELD_RE.findall(m.group(1)))


def _write_if_changed(path: Path, content: str, dry_run: bool) -> str:
    """Write content to path if it differs. Returns 'CREATE', 'UPDATE', or 'OK'."""
    if path.exists():
        if path.read_text(encoding="utf-8") == content:
            return "OK"
        action = "WOULD UPDATE" if dry_run else "UPDATE"
    else:
        action = "WOULD CREATE" if dry_run else "CREATE"

    if not dry_run:
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(content, encoding="utf-8")
    return action


def _remove_stale(target_dir: Path, valid_names: set[str], label: str, dry_run: bool) -> list[str]:
    """Remove subdirs in target_dir whose name isn't in valid_names."""
    msgs = []
    if not target_dir.exists():
        return msgs
    for d in target_dir.iterdir():
        if d.is_dir() and d.name not in valid_names:
            action = "WOULD REMOVE" if dry_run else "REMOVE"
            if not dry_run:
                shutil.rmtree(d)
            msgs.append(f"  {action} {label}/{d.name}/ (no .ai/ source)")
    return msgs


# ── per-skill sync ────────────────────────────────────────────────────────────

def sync_skill(skill_dir: Path, dry_run: bool = False) -> list[str]:
    """Sync one skill to both targets. Returns list of status lines."""
    skill_md = skill_dir / "SKILL.md"
    if not skill_md.exists():
        return []

    fm = parse_frontmatter(skill_md)
    name        = fm.get("name", "").strip()
    description = fm.get("description", "").strip()

    if not name:
        return [f"  SKIP {skill_dir.name}: no 'name' in frontmatter"]

    source_content = skill_md.read_text(encoding="utf-8")
    msgs = []

    # .claude/skills/ — thin wrapper (Claude Code dynamic injection)
    wrapper = (
        f"---\n"
        f"name: {name}\n"
        f"description: {description}\n"
        f"---\n\n"
        f"!`cat .ai/skills/{skill_dir.name}/SKILL.md`\n"
    )
    claude_path = CLAUDE_SKILLS / skill_dir.name / "SKILL.md"
    action = _write_if_changed(claude_path, wrapper, dry_run)
    if action != "OK":
        msgs.append(f"  {action} .claude/skills/{skill_dir.name}/SKILL.md")

    # .github/skills/ — full content copy (Copilot reads directly)
    github_path = GITHUB_SKILLS / skill_dir.name / "SKILL.md"
    action = _write_if_changed(github_path, source_content, dry_run)
    if action != "OK":
        msgs.append(f"  {action} .github/skills/{skill_dir.name}/SKILL.md")

    if not msgs:
        msgs.append(f"  OK   {skill_dir.name}")

    return msgs


# ── full sync ─────────────────────────────────────────────────────────────────

def sync_all(dry_run: bool = False) -> int:
    if not AI_SKILLS.exists():
        print("ERROR: .ai/skills/ not found")
        return 1

    prefix = "[DRY RUN] " if dry_run else ""
    print(f"{prefix}Syncing .ai/skills/ -> .claude/skills/ + .github/skills/")

    results = []
    valid_names = set()

    for skill_dir in sorted(AI_SKILLS.iterdir()):
        if skill_dir.is_dir():
            valid_names.add(skill_dir.name)
            results.extend(sync_skill(skill_dir, dry_run))

    results.extend(_remove_stale(CLAUDE_SKILLS, valid_names, ".claude/skills", dry_run))
    results.extend(_remove_stale(GITHUB_SKILLS, valid_names, ".github/skills", dry_run))

    for r in results:
        print(r)

    changed = sum(1 for r in results if any(k in r for k in ("CREATE", "UPDATE", "REMOVE")))
    print(f"\n{changed} file(s) {'would be ' if dry_run else ''}changed.")
    return 0


# ── hook mode ─────────────────────────────────────────────────────────────────

def sync_from_hook() -> int:
    """Read Claude Code PostToolUse JSON from stdin, sync only if .ai/skills/ file."""
    try:
        payload = json.loads(sys.stdin.read())
    except (json.JSONDecodeError, OSError):
        return 0

    file_path = payload.get("tool_input", {}).get("file_path", "")
    if not file_path:
        return 0

    path = Path(file_path).resolve()
    try:
        rel = path.relative_to(AI_SKILLS.resolve())
    except ValueError:
        return 0

    skill_dir = AI_SKILLS / rel.parts[0]
    for msg in sync_skill(skill_dir):
        print(msg, file=sys.stderr)

    return 0


# ── entry point ───────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description="Sync .claude/skills/ and .github/skills/ from .ai/skills/"
    )
    parser.add_argument("--from-hook", action="store_true",
                        help="Hook mode: read tool JSON from stdin")
    parser.add_argument("--dry-run", action="store_true",
                        help="Show changes without writing")
    args = parser.parse_args()

    if args.from_hook:
        sys.exit(sync_from_hook())
    else:
        sys.exit(sync_all(dry_run=args.dry_run))


if __name__ == "__main__":
    main()

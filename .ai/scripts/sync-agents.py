#!/usr/bin/env python3
"""
Sync agent files from .ai/agents/ (the single source of truth) to:
  - .claude/agents/  thin wrappers using !`cat ...` (Claude Code executes these)
  - .github/agents/  full content copies       (GitHub Copilot reads these directly)

Usage:
    python sync-agents.py              # sync all agents to both targets
    python sync-agents.py --from-hook  # hook mode: reads tool JSON from stdin,
                                       # only acts on .ai/agents/ writes
    python sync-agents.py --dry-run    # show what would change without writing
"""

import argparse
import json
import re
import shutil
import sys
from pathlib import Path


SCRIPT_DIR    = Path(__file__).parent.parent.parent  # .ai/scripts/ -> .ai/ -> repo root
AI_AGENTS     = SCRIPT_DIR / ".ai"     / "agents"
CLAUDE_AGENTS = SCRIPT_DIR / ".claude" / "agents"
GITHUB_AGENTS = SCRIPT_DIR / ".github" / "agents"

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
    """Remove files in target_dir whose stem isn't in valid_names."""
    msgs = []
    if not target_dir.exists():
        return msgs
    for f in target_dir.iterdir():
        if f.is_file() and f.suffix == ".md" and f.stem not in valid_names:
            action = "WOULD REMOVE" if dry_run else "REMOVE"
            if not dry_run:
                f.unlink()
            msgs.append(f"  {action} {label}/{f.name} (no .ai/ source)")
    return msgs


# ── per-agent sync ────────────────────────────────────────────────────────────

CLAUDE_FIELDS = ["name", "description", "model", "tools"]  # fields included in wrapper

def sync_agent(agent_file: Path, dry_run: bool = False) -> list[str]:
    """Sync one agent to both targets. Returns list of status lines."""
    if not agent_file.exists() or agent_file.suffix != ".md":
        return []

    fm = parse_frontmatter(agent_file)
    name = fm.get("name", "").strip()

    if not name:
        return [f"  SKIP {agent_file.stem}: no 'name' in frontmatter"]

    source_content = agent_file.read_text(encoding="utf-8")
    msgs = []

    # Build frontmatter for .claude/ wrapper — include key fields
    wrapper_fm_lines = []
    for field in CLAUDE_FIELDS:
        value = fm.get(field, "").strip()
        if value:
            wrapper_fm_lines.append(f"{field}: {value}")
    wrapper_fm = "\n".join(wrapper_fm_lines)

    # .claude/agents/ — thin wrapper (Claude Code dynamic injection)
    wrapper = (
        f"---\n"
        f"{wrapper_fm}\n"
        f"---\n\n"
        f"!`cat .ai/agents/{agent_file.name}`\n"
    )
    claude_path = CLAUDE_AGENTS / agent_file.name
    action = _write_if_changed(claude_path, wrapper, dry_run)
    if action != "OK":
        msgs.append(f"  {action} .claude/agents/{agent_file.name}")

    # .github/agents/ — full content copy (Copilot reads directly)
    github_path = GITHUB_AGENTS / agent_file.name
    action = _write_if_changed(github_path, source_content, dry_run)
    if action != "OK":
        msgs.append(f"  {action} .github/agents/{agent_file.name}")

    if not msgs:
        msgs.append(f"  OK   {agent_file.stem}")

    return msgs


# ── full sync ─────────────────────────────────────────────────────────────────

def sync_all(dry_run: bool = False) -> int:
    if not AI_AGENTS.exists():
        print("ERROR: .ai/agents/ not found")
        return 1

    prefix = "[DRY RUN] " if dry_run else ""
    print(f"{prefix}Syncing .ai/agents/ -> .claude/agents/ + .github/agents/")

    results = []
    valid_names = set()

    for agent_file in sorted(AI_AGENTS.iterdir()):
        if agent_file.is_file() and agent_file.suffix == ".md":
            valid_names.add(agent_file.stem)
            results.extend(sync_agent(agent_file, dry_run))

    results.extend(_remove_stale(CLAUDE_AGENTS, valid_names, ".claude/agents", dry_run))
    results.extend(_remove_stale(GITHUB_AGENTS, valid_names, ".github/agents", dry_run))

    for r in results:
        print(r)

    changed = sum(1 for r in results if any(k in r for k in ("CREATE", "UPDATE", "REMOVE")))
    print(f"\n{changed} file(s) {'would be ' if dry_run else ''}changed.")
    return 0


# ── hook mode ─────────────────────────────────────────────────────────────────

def sync_from_hook() -> int:
    """Read Claude Code PostToolUse JSON from stdin, sync only if .ai/agents/ file."""
    try:
        payload = json.loads(sys.stdin.read())
    except (json.JSONDecodeError, OSError):
        return 0

    file_path = payload.get("tool_input", {}).get("file_path", "")
    if not file_path:
        return 0

    path = Path(file_path).resolve()
    try:
        rel = path.relative_to(AI_AGENTS.resolve())
    except ValueError:
        return 0

    agent_file = AI_AGENTS / rel.name
    for msg in sync_agent(agent_file):
        print(msg, file=sys.stderr)

    return 0


# ── entry point ───────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description="Sync .claude/agents/ and .github/agents/ from .ai/agents/"
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

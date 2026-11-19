#!/usr/bin/env python3
"""
Link the canonical .ai/ content into the paths each agent tool actually reads,
and generate the Claude Code rules that path-scoped instructions map onto.

No agent reads .ai/ directly — every tool hardcodes its own project locations.
The template keeps a single copy of every file in .ai/ and exposes it through
folder links:

  tool      adapter path             -> canonical source
  claude    .claude/skills/          -> .ai/skills/
            .claude/agents/          -> .ai/agents/
            .claude/commands/        -> .ai/prompts/   (only while .ai/prompts/ has files)
  copilot   .github/skills/          -> .ai/skills/
            .github/agents/          -> .ai/agents/
            .github/instructions/    -> .ai/instructions/
            .github/prompts/         -> .ai/prompts/   (only while .ai/prompts/ has files)
  agents    .agents/skills/          -> .ai/skills/

Prompts are not a template asset: the template ships no .ai/prompts/, because
prompts are skills now. A project may install prompts from a catalog (for
example awesome-copilot, through the extension) into .ai/prompts/. The two
prompt adapters are linked only while that directory holds at least one file.
Otherwise they are legacy: removed when they are links or empty directories.

Claude Code reads path-scoped rules from .claude/rules/*.md, not from a folder
link. The claude tool therefore COPIES each .ai/instructions/{name}.instructions.md
to a regular file .claude/rules/{name}.instructions.md, mapping applyTo to the
paths: frontmatter. The copies are gitignored and carry a generated-from marker.
Hand-written files in .claude/rules/ are never read for writing, never
overwritten and never removed.

Links are never committed, so they are recreated on clone. A real directory
holding files at an adapter path is reported as CONFLICT and left untouched:
that is a second copy of the canonical content, which this template forbids.

Retired adapters (.claude/instructions/, and the prompt adapters while
.ai/prompts/ is absent or empty) are removed when they are links or empty
directories. Real content found at a retired path is reported as CONFLICT and
left in place; it is never deleted.
Junctions left behind by platforms that are no longer supported are removed
the same way: Reasonix support ended, so .reasonix/skills/ and .reasonix/agents/
are cleaned up when present.

Usage:
    python sync-platforms.py                     # link every supported tool
    python sync-platforms.py --tools claude,copilot
    python sync-platforms.py --dry-run           # show what would change
"""

import argparse
import os
import re
import subprocess
import sys
from pathlib import Path

SCRIPT_DIR = Path(__file__).parent.parent.parent  # .ai/scripts/ -> .ai/ -> repo root

# tool -> {canonical source: [adapter paths the tool reads]}
TOOL_TARGETS = {
    "claude": {
        ".ai/skills": [".claude/skills"],
        ".ai/agents": [".claude/agents"],
    },
    "copilot": {
        ".ai/skills": [".github/skills"],
        ".ai/agents": [".github/agents"],
        ".ai/instructions": [".github/instructions"],
    },
    "agents": {
        ".ai/skills": [".agents/skills"],
    },
}
DEFAULT_TOOLS = tuple(TOOL_TARGETS)

# Prompt adapters. Linked only while PROMPTS_SOURCE holds at least one file
# (prompts installed from a catalog); legacy otherwise. tool -> adapter paths.
PROMPTS_SOURCE = ".ai/prompts"
PROMPT_TARGETS = {
    "claude": [".claude/commands"],
    "copilot": [".github/prompts"],
}
PROMPT_ADAPTERS = [path for paths in PROMPT_TARGETS.values() for path in paths]

# Adapter paths that are always retired or belong to unsupported platforms. Removed
# when they are links or empty directories; real content there is reported, not deleted.
LEGACY_TARGETS = [
    ".claude/instructions",
    ".reasonix/skills",
    ".reasonix/agents",
]

# Claude Code path-scoped rules: copy-only generation from .ai/instructions/.
RULES_SOURCE = ".ai/instructions"
RULES_TARGET = ".claude/rules"
RULE_SUFFIX = ".instructions.md"


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


def _is_link(path: Path) -> bool:
    """True for symlinks and junctions, including broken ones."""
    return path.is_symlink() or _is_junction(path)


def _fail(message: str) -> None:
    print(f"  FAIL     {message}")


def _rmdir(path: Path) -> bool:
    """Remove a link, a file, or an EMPTY directory. Never follows a link.

    Avoids shutil.rmtree() for symlinks — rmtree follows symlinks and would
    delete the target directory instead of just the symlink. Directories are
    removed with rmdir(), so a directory that still holds files is never
    deleted; callers check contents first. Returns False and prints the reason
    when removal fails, so failures are visible instead of silently ignored.
    """
    try:
        if path.is_symlink():
            path.unlink()
        elif sys.platform == "win32" and _is_junction(path):
            result = subprocess.run(
                ["cmd", "/c", "rmdir", str(path)], capture_output=True, text=True
            )
            if result.returncode != 0:
                detail = (result.stderr or result.stdout).strip()
                _fail(f"{path}: rmdir failed: {detail}")
                return False
        elif path.is_dir():
            path.rmdir()
        elif path.exists():
            path.unlink()
        return True
    except OSError as exc:
        _fail(f"{path}: {exc}")
        return False


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
    """Ensure target_rel is a junction/symlink pointing to source_rel.

    Returns OK, SKIP, CONFLICT, FAIL, or the action taken (CREATE / RECREATE).
    """
    root = SCRIPT_DIR
    target = root / target_rel
    source = (root / source_rel).resolve()

    if not source.is_dir():
        return "SKIP"

    if _is_link(target):
        if target.exists():
            # Verify it points correctly by checking that the first entry of the
            # source is reachable through the link.
            first_entry = next(source.iterdir(), None)
            if first_entry is not None and (target / first_entry.name).exists():
                return "OK"
        # Wrong target, or broken — remove and recreate
        action = "RECREATE"
    elif target.exists():
        if target.is_dir() and not any(target.iterdir()):
            action = "RECREATE"
        else:
            # A real directory holding files (or a file) is a copy of the
            # canonical source. Never delete it; report it so it gets merged by hand.
            return "CONFLICT"
    else:
        action = "CREATE"

    if not dry_run:
        if _is_link(target) or target.exists():
            if not _rmdir(target):
                return "FAIL"
        try:
            ok = _create_link(target, source)
        except OSError as exc:
            _fail(f"{target_rel}: {exc}")
            return "FAIL"
        if not ok:
            _fail(f"{target_rel}: could not create junction/symlink")
            return "FAIL"

    return action


# ─── Claude Code rules (copy-only generator) ─────────────────────────────────

def _rule_marker(name: str) -> str:
    """Marker comment written into every generated rule file."""
    return (f"<!-- Generated from {RULES_SOURCE}/{name}{RULE_SUFFIX} by sync-platforms.py. "
            "Edit the source, not this file. -->")


def _read_text(path: Path) -> str:
    """Read UTF-8 text with LF line endings, for comparison and copying."""
    return path.read_bytes().decode("utf-8", errors="replace").replace("\r\n", "\n")


def _unquote(value: str) -> str:
    value = value.strip()
    if len(value) >= 2 and value[0] == value[-1] and value[0] in "\"'":
        return value[1:-1]
    return value


def _yaml_str(value: str) -> str:
    """Double-quoted YAML scalar."""
    return '"' + value.replace("\\", "\\\\").replace('"', '\\"') + '"'


def _parse_fields(lines: list[str]) -> dict:
    """Parse top-level `key: value` frontmatter lines. Folded/literal block
    scalars (`>`, `|`) are joined into one line."""
    fields: dict = {}
    key = None
    for line in lines:
        if not line.strip() or line.lstrip().startswith("#"):
            continue
        if line[0] in " \t":
            if key is not None:
                fields[key] = (fields[key] + " " + line.strip()).strip()
            continue
        match = re.match(r"^([A-Za-z_][\w-]*)\s*:\s*(.*)$", line)
        if not match:
            key = None
            continue
        key = match.group(1)
        value = match.group(2).strip()
        fields[key] = "" if value in (">", "|", ">-", "|-") else _unquote(value)
    return fields


def _split_frontmatter(text: str) -> tuple[dict, str]:
    """Return (frontmatter fields, body). Body is everything after the closing `---`."""
    lines = text.split("\n")
    if not lines or lines[0].strip() != "---":
        return {}, text
    for i in range(1, len(lines)):
        if lines[i].strip() == "---":
            return _parse_fields(lines[1:i]), "\n".join(lines[i + 1:])
    return {}, text  # unterminated frontmatter: treat the whole file as body


def _claude_paths(apply_to: str) -> list[str]:
    """Map a Copilot applyTo value to Claude `paths:` globs.

    Returns [] (meaning: no paths, unconditional rule) when the value is empty
    or matches everything (`**/*` or `**`).
    """
    patterns = [p.strip().strip("\"'").strip() for p in apply_to.split(",")]
    patterns = [p for p in patterns if p]
    if not patterns or any(p in ("**", "**/*") for p in patterns):
        return []
    return patterns


def render_rule(name: str, source_text: str) -> str:
    """Render one .claude/rules file from one .ai/instructions source."""
    fields, body = _split_frontmatter(source_text)
    head: list[str] = []
    description = fields.get("description", "").strip()
    if description:
        head.append(f"description: {_yaml_str(description)}")
    paths = _claude_paths(fields.get("applyTo", ""))
    if paths:
        head.append("paths:")
        head += [f"  - {_yaml_str(p)}" for p in paths]

    lines: list[str] = []
    if head:
        lines += ["---", *head, "---"]
    lines.append(_rule_marker(name))
    text = "\n".join(lines) + "\n" + body
    if not text.endswith("\n"):
        text += "\n"
    return text


def sync_rules(dry_run: bool) -> tuple[int, list[str], int]:
    """Generate .claude/rules/*.instructions.md from .ai/instructions/.

    Copy-only: writes regular files and never follows or creates links.
    Returns (changed, conflicts, failures).
    """
    rules_dir = SCRIPT_DIR / RULES_TARGET
    claude_dir = SCRIPT_DIR / ".claude"

    if _is_link(claude_dir) or _is_link(rules_dir):
        print(f"  CONFLICT {RULES_TARGET} (a link; the rules generator never follows links)")
        return 0, [RULES_TARGET], 0
    if rules_dir.exists() and not rules_dir.is_dir():
        print(f"  CONFLICT {RULES_TARGET} (a file, not a directory)")
        return 0, [RULES_TARGET], 0

    sources_dir = SCRIPT_DIR / RULES_SOURCE
    sources = []
    if sources_dir.is_dir():
        sources = [p for p in sorted(sources_dir.glob(f"*{RULE_SUFFIX}")) if p.is_file()]
    names = {p.name[: -len(RULE_SUFFIX)] for p in sources}

    changed = 0
    conflicts: list[str] = []
    failures = 0
    for source in sources:
        name = source.name[: -len(RULE_SUFFIX)]
        rel = f"{RULES_TARGET}/{name}{RULE_SUFFIX}"
        target = rules_dir / f"{name}{RULE_SUFFIX}"
        desired = render_rule(name, _read_text(source))

        if _is_link(target) or target.is_dir():
            print(f"  CONFLICT {rel} (link or directory at a generated path; not touched)")
            conflicts.append(rel)
            continue

        current = _read_text(target) if target.is_file() else None
        if current is not None and _rule_marker(name) not in current.split("\n"):
            print(f"  CONFLICT {rel} (hand-written file with the same name; not overwritten)")
            conflicts.append(rel)
            continue

        if current == desired:
            print(f"  OK       {rel}")
            continue

        label = "WOULD WRITE" if dry_run else "WROTE"
        if not dry_run:
            try:
                rules_dir.mkdir(parents=True, exist_ok=True)
                target.write_bytes(desired.encode("utf-8"))
            except OSError as exc:
                _fail(f"{rel}: {exc}")
                failures += 1
                continue
        print(f"  {label:<12} {rel} (from {RULES_SOURCE}/{source.name})")
        changed += 1

    # Remove generated copies whose source is gone. Only files that carry this
    # name's generated marker are candidates; hand-written rules are never removed.
    if rules_dir.is_dir():
        for stale in sorted(rules_dir.glob(f"*{RULE_SUFFIX}")):
            name = stale.name[: -len(RULE_SUFFIX)]
            if name in names or _is_link(stale) or not stale.is_file():
                continue
            if _rule_marker(name) not in _read_text(stale).split("\n"):
                continue
            label = "WOULD REMOVE" if dry_run else "REMOVE"
            print(f"  {label:<12} {RULES_TARGET}/{stale.name} (source removed)")
            if not dry_run:
                try:
                    stale.unlink()
                except OSError as exc:
                    _fail(f"{RULES_TARGET}/{stale.name}: {exc}")
                    failures += 1
                    continue
            changed += 1

    return changed, conflicts, failures


# ─── Summary and legacy cleanup ──────────────────────────────────────────────

def prompts_active() -> bool:
    """True while .ai/prompts/ exists and holds at least one file."""
    root = SCRIPT_DIR / PROMPTS_SOURCE
    return root.is_dir() and any(path.is_file() for path in root.rglob("*"))


def sync_all(dry_run: bool = False, tools=DEFAULT_TOOLS) -> int:
    prefix = "[DRY RUN] " if dry_run else ""
    print(f"{prefix}Ensuring folder-level links for: {', '.join(tools)}")

    prompts_on = prompts_active()
    if prompts_on:
        prompt_tools = [tool for tool in tools if tool in PROMPT_TARGETS]
        print(f"Prompts: {PROMPTS_SOURCE}/ has files; linking prompt adapters for: "
              f"{', '.join(prompt_tools) or 'none of the selected tools'}")
    else:
        print(f"Prompts: {PROMPTS_SOURCE}/ is absent or empty; prompt adapters are legacy.")

    changed = 0
    failures = 0
    conflicts = []
    for tool in tools:
        sources = dict(TOOL_TARGETS[tool])
        if prompts_on and tool in PROMPT_TARGETS:
            sources[PROMPTS_SOURCE] = PROMPT_TARGETS[tool]
        for source_rel, targets in sources.items():
            for target_rel in targets:
                result = sync_junction(source_rel, target_rel, dry_run)
                if result == "OK":
                    print(f"  OK       {target_rel} -> {source_rel}")
                elif result == "SKIP":
                    print(f"  SKIP     {target_rel} -> {source_rel} (source missing)")
                elif result == "CONFLICT":
                    print(f"  CONFLICT {target_rel} -> {source_rel} (real files, not a link)")
                    conflicts.append(target_rel)
                    changed += 1
                elif result == "FAIL":
                    print(f"  FAIL     {target_rel} -> {source_rel}")
                    failures += 1
                else:
                    label = f"WOULD {result}" if dry_run else result
                    print(f"  {label:<8} {target_rel} -> {source_rel}")
                    changed += 1

    if "claude" in tools:
        rules_changed, rules_conflicts, rules_failed = sync_rules(dry_run)
        changed += rules_changed
        conflicts += rules_conflicts
        failures += rules_failed

    print(f"\n{changed} item(s) {'would be ' if dry_run else ''}changed.")
    if conflicts:
        print("Merge each CONFLICT by hand: move its files into the canonical source, "
              "delete the directory, then re-run this script.")

    removed, legacy_conflicts, legacy_failed = remove_legacy_targets(dry_run, prompts_on)
    if removed:
        print(f"{removed} legacy item(s) {'would be ' if dry_run else ''}removed.")
    if legacy_conflicts:
        print("Legacy CONFLICT paths hold real content and were left in place; "
              "move anything you need into .ai/ and delete them by hand.")
    failures += legacy_failed

    if failures:
        print(f"{failures} item(s) failed; see the FAIL lines above.")
        return 1
    return 0


def remove_legacy_targets(dry_run: bool, prompts_on: bool = False) -> tuple[int, list[str], int]:
    """Remove retired links and empty directories. Returns (removed, conflicts, failures).

    The prompt adapters are legacy unless prompts_on (.ai/prompts/ has files).
    Real files or directories holding content are reported as CONFLICT and left alone.
    """
    removed = 0
    conflicts: list[str] = []
    failures = 0
    parents = set()
    legacy = LEGACY_TARGETS + ([] if prompts_on else PROMPT_ADAPTERS)
    for target_rel in legacy:
        target = SCRIPT_DIR / target_rel
        if not (target.exists() or _is_link(target)):
            continue
        parents.add(target.parent)

        if _is_link(target):
            label = "WOULD REMOVE" if dry_run else "REMOVE"
            print(f"  {label:<12} {target_rel} (retired adapter link)")
        elif target.is_dir() and not any(target.iterdir()):
            label = "WOULD REMOVE" if dry_run else "REMOVE"
            print(f"  {label:<12} {target_rel} (empty retired directory)")
        else:
            print(f"  CONFLICT     {target_rel} (real content, not a link; left in place)")
            conflicts.append(target_rel)
            continue

        if not dry_run and not _rmdir(target):
            failures += 1
            continue
        removed += 1

    for parent in parents:
        if not parent.is_dir() or any(parent.iterdir()):
            continue
        if dry_run:
            print(f"  WOULD REMOVE {parent.name}/ (empty)")
        else:
            try:
                parent.rmdir()
                print(f"  REMOVE       {parent.name}/ (empty)")
            except OSError as exc:
                _fail(f"{parent}: {exc}")
                failures += 1
    return removed, conflicts, failures


def parse_tools(value: str):
    """Comma-separated tool names → validated tuple."""
    names = [name.strip().lower() for name in value.split(",") if name.strip()]
    unknown = [name for name in names if name not in TOOL_TARGETS]
    if unknown:
        raise argparse.ArgumentTypeError(
            f"unknown tool(s): {', '.join(unknown)} - choose from {', '.join(TOOL_TARGETS)}"
        )
    return tuple(dict.fromkeys(names))


def main():
    parser = argparse.ArgumentParser(
        description="Link .ai/ into the skill/agent paths read by each agent tool "
                    "and generate Claude Code rules from .ai/instructions/"
    )
    parser.add_argument("--dry-run", action="store_true", help="Show changes without writing")
    parser.add_argument(
        "--tools",
        type=parse_tools,
        default=DEFAULT_TOOLS,
        help=f"Comma-separated tools to link (default: {','.join(DEFAULT_TOOLS)})",
    )
    args = parser.parse_args()
    sys.exit(sync_all(dry_run=args.dry_run, tools=args.tools))


if __name__ == "__main__":
    main()

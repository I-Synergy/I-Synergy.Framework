#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates that .claude/settings.json is correctly configured for the .ai/ layout:
  - plansDirectory points to .ai/plans
  - hook commands use the shared scripts in .ai/scripts/hooks/ through py.sh
  - additionalDirectories includes .ai/ entries
  - .claude/ holds only config, skills, agents and generated rules
  - No stale .claude/ content paths remain in tracked files
"""

import io
import json
import re
import sys
from pathlib import Path

if sys.platform == "win32":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8")

SCRIPT_DIR = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent

# Paths that legitimately reference .claude/ (not stale)
# Only specific settings files are allowed — do NOT add ".claude/" as a bare
# substring because it would match any .claude/ reference and defeat the check.
ALLOWED_CLAUDE_REFS = {
    ".claude/",
    ".claude/settings.json",
    ".claude/settings.local.json",
    ".claude/skills",
    ".claude/skills/",
    ".claude/agents",
    ".claude/agents/",
    ".claude/rules",
    ".claude/rules/",
}

# Files to check for stale .claude/ content references
CHECK_FILES = [
    "AGENTS.md",
    "README.md",
    "TEMPLATE-FAQ.md",
    "TEMPLATE-USAGE.md",
    ".github/copilot-instructions.md",
]

# Stale pattern: .claude/ that is NOT settings.json, settings.local.json, or ~/.claude/
STALE_REF_PATTERN = re.compile(
    r'(?<!~/)\.claude/(?!settings\.json|settings\.local\.json)'
)

# Commands the template shipped before the shared hook scripts. They must not remain.
LEGACY_HOOK_COMMANDS = (
    "python .ai/scripts/sync-platforms.py",
    "python .ai/scripts/sync-agents.py",
    "python -c",
)

HOOK_SCRIPT_PATTERN = re.compile(r"\.ai/scripts/hooks/[\w.-]+\.py")


def _hook_commands(hooks: dict, event: str) -> list:
    """Every hook command configured for one event."""
    commands = []
    entries = hooks.get(event, [])
    if not isinstance(entries, list):
        return commands
    for entry in entries:
        if isinstance(entry, dict):
            for hook in entry.get("hooks", []) or []:
                if isinstance(hook, dict) and "command" in hook:
                    commands.append(str(hook["command"]))
    return commands


def _hook_script_text(command: str):
    """Text of the shared hook script a command runs, or None when missing."""
    match = HOOK_SCRIPT_PATTERN.search(command)
    if not match:
        return None
    path = TEMPLATE_ROOT / match.group(0)
    return path.read_text(encoding="utf-8") if path.is_file() else None


def _load_settings():
    settings_path = TEMPLATE_ROOT / ".claude" / "settings.json"
    if not settings_path.exists():
        return None, "settings.json not found (required)"
    try:
        return json.loads(settings_path.read_text(encoding="utf-8")), None
    except json.JSONDecodeError as e:
        return None, f"settings.json is invalid JSON: {e}"


def test_settings_json() -> bool:
    print("TEST 1: .claude/settings.json configuration")
    print("-" * 40)

    settings, error = _load_settings()
    if settings is None:
        print(f"  FAIL: {error}")
        return False

    passed = True

    # Check plansDirectory
    plans_dir = settings.get("plansDirectory", "")
    if ".ai/plans" in plans_dir:
        print(f"  PASS: plansDirectory = {plans_dir!r}")
    else:
        print(f"  FAIL: plansDirectory = {plans_dir!r}  (expected .ai/plans)")
        passed = False

    hooks = settings.get("hooks", {})

    # Legacy inline commands must be gone from the template's own settings.
    all_commands = [c for event in hooks for c in _hook_commands(hooks, event)]
    legacy = [c for c in all_commands if any(c.startswith(prefix) for prefix in LEGACY_HOOK_COMMANDS)]
    if legacy:
        print(f"  FAIL: legacy hook commands still configured: {[c[:60] for c in legacy]}")
        passed = False
    else:
        print("  PASS: no legacy hook commands")

    # Every hook must run through the portable launcher and an existing shared script.
    for event, script_name, must_contain, label in (
        ("SessionStart", "session_start.py", "sync-platforms.py", "SessionStart"),
        ("PostToolUse", "post_edit_sync.py", "sync-platforms.py", "PostToolUse"),
        ("Stop", "stop_build.py", "dotnet", "Stop"),
    ):
        commands = [c for c in _hook_commands(hooks, event) if script_name in c]
        if not commands:
            print(f"  FAIL: {label} hook must run .ai/scripts/hooks/{script_name}")
            passed = False
            continue
        if not all("hooks/py.sh" in c for c in commands):
            print(f"  FAIL: {label} hook must start with the portable launcher (hooks/py.sh)")
            passed = False
            continue
        text = _hook_script_text(commands[0])
        if text is None:
            print(f"  FAIL: {label} hook script .ai/scripts/hooks/{script_name} is missing")
            passed = False
        elif must_contain not in text:
            print(f"  FAIL: {script_name} does not contain {must_contain!r}")
            passed = False
        else:
            print(f"  PASS: {label} hook runs .ai/scripts/hooks/{script_name}")

    # Stop build must keep the memory protections.
    stop_text = next(
        (t for t in (_hook_script_text(c) for c in _hook_commands(hooks, "Stop")) if t), ""
    )
    if "MSBUILDDISABLENODEREUSE" in stop_text and "-nodeReuse:false" in stop_text:
        print("  PASS: stop_build.py keeps MSBUILDDISABLENODEREUSE and -nodeReuse:false")
    elif stop_text:
        print("  FAIL: stop_build.py must set MSBUILDDISABLENODEREUSE=1 and pass -nodeReuse:false")
        passed = False

    # Check additionalDirectories
    permissions = settings.get("permissions", {})
    additional = permissions.get("additionalDirectories", [])
    ai_entries = [d for d in additional if ".ai" in d]
    commands_entries = [d for d in additional if ".ai/commands" in d or "commands" in d]
    claude_entries = [d for d in additional if ".claude" in d and "settings" not in d]

    if ai_entries:
        print(f"  PASS: additionalDirectories includes {len(ai_entries)} .ai/ entries")
    else:
        print("  FAIL: additionalDirectories has no .ai/ entries")
        passed = False

    if commands_entries:
        print(f"  FAIL: additionalDirectories should not contain .ai/commands entries: {commands_entries}")
        passed = False
    else:
        print("  PASS: .ai/commands/ not in additionalDirectories")

    stale_removed = [d for d in additional if re.search(r"\.ai/(prompts|instructions)(/|$)", d)]
    if stale_removed:
        print(f"  FAIL: additionalDirectories points at retired paths: {stale_removed}")
        passed = False

    if claude_entries:
        # .claude/ itself is fine (so Claude Code can read settings.json)
        # but sub-dirs like .claude/patterns are stale
        stale = [d for d in claude_entries if d not in {"./.claude", ".claude"}]
        if stale:
            print(f"  FAIL: stale .claude/ sub-dirs in additionalDirectories: {stale}")
            passed = False
        else:
            print("  PASS: .claude/ entry kept for settings.json access")

    # Check permissions.deny contains Bash(rm -rf:*)
    deny = permissions.get("deny", [])
    has_rm_rf_deny = any("Bash(rm -rf:" in str(d) for d in deny)
    if has_rm_rf_deny:
        print("  PASS: permissions.deny contains Bash(rm -rf:*)")
    else:
        print("  FAIL: permissions.deny should contain Bash(rm -rf:*)")
        passed = False

    return passed


def test_no_stale_references() -> bool:
    print("\nTEST 2: No stale .claude/ content references in tracked files")
    print("-" * 40)

    passed = True

    for rel_path in CHECK_FILES:
        file_path = TEMPLATE_ROOT / rel_path
        if not file_path.exists():
            print(f"  SKIP: {rel_path} (not found)")
            continue

        content = file_path.read_text(encoding="utf-8")

        # Find lines for context
        stale_lines = [
            (i + 1, line.strip())
            for i, line in enumerate(content.splitlines())
            if STALE_REF_PATTERN.search(line)
            and not any(allowed in line for allowed in ALLOWED_CLAUDE_REFS)
        ]

        if stale_lines:
            print(f"  FAIL: {rel_path} has {len(stale_lines)} stale reference(s):")
            for lineno, line in stale_lines[:5]:
                print(f"    line {lineno}: {line[:100]}")
            passed = False
        else:
            print(f"  PASS: {rel_path}")

    return passed


def test_ai_directory_exists() -> bool:
    print("\nTEST 3: .ai/ directory structure")
    print("-" * 40)

    required = [
        ".ai",
        ".ai/patterns",
        ".ai/skills",
        ".ai/reference",
        ".ai/reference/templates",
        ".ai/checklists",
        ".ai/project",
        ".ai/plans",
        ".ai/progress",
        ".ai/completed",
        ".ai/scripts/hooks",
    ]

    passed = True
    for rel in required:
        path = TEMPLATE_ROOT / rel
        if path.is_dir():
            print(f"  PASS: {rel}/")
        else:
            print(f"  FAIL: {rel}/ missing")
            passed = False

    return passed


def test_session_context_hook() -> bool:
    print("\nTEST 4b: Session context hook configuration")
    print("-" * 40)

    settings, error = _load_settings()
    if settings is None:
        print(f"  FAIL: cannot verify the session-context hook: {error}")
        return False

    passed = True
    session_start = settings.get("hooks", {}).get("SessionStart", [])

    if not isinstance(session_start, list):
        print("  FAIL: SessionStart should be a list")
        return False

    entry = next(
        (e for e in session_start
         if isinstance(e, dict) and e.get("matcher") == "startup|resume|clear|compact"),
        None,
    )
    if entry is None:
        print("  FAIL: SessionStart entry with matcher 'startup|resume|clear|compact' not found")
        return False

    commands = [str(h.get("command", "")) for h in entry.get("hooks", []) if isinstance(h, dict)]
    session_commands = [c for c in commands if "session_start.py" in c]
    if not session_commands:
        print("  FAIL: session-context entry does not run .ai/scripts/hooks/session_start.py")
        return False

    text = _hook_script_text(session_commands[0]) or ""
    checks = {
        "placeholder-skip": "# Session Context Template" in text,
        "200-line-cap": "MAX_LINES = 200" in text,
        "context-file-ref": ".ai/session-context.md" in text,
        "memory-header": "Session memory from .ai/session-context.md" in text,
        "completed-listing": ".ai/completed" in text,
    }
    missing = [name for name, found in checks.items() if not found]
    if missing:
        print(f"  FAIL: session_start.py missing features: {missing}")
        passed = False
    else:
        print("  PASS: session_start.py has all required features")
        for name in checks:
            print(f"    - {name}: YES")

    return passed


def test_session_memory_section() -> bool:
    print("\nTEST 4c: Session Memory section in documentation")
    print("-" * 40)

    marker_1 = "`.ai/session-context.md` is the shared memory for every client"
    marker_2 = "never overwrite another client's entries"
    heading = "## Session Memory"

    files_to_check = [
        "AGENTS.md",
        ".github/copilot-instructions.md",
    ]

    passed = True

    for rel_path in files_to_check:
        file_path = TEMPLATE_ROOT / rel_path
        if not file_path.exists():
            print(f"  SKIP: {rel_path} (not found)")
            continue

        content = file_path.read_text(encoding="utf-8")

        has_heading = heading in content
        has_marker_1 = marker_1 in content
        has_marker_2 = marker_2 in content

        if has_heading and has_marker_1 and has_marker_2:
            print(f"  PASS: {rel_path} has Session Memory section with all markers")
        else:
            missing = []
            if not has_heading:
                missing.append(f"heading ({heading!r})")
            if not has_marker_1:
                missing.append("marker 1")
            if not has_marker_2:
                missing.append("marker 2")
            print(f"  FAIL: {rel_path} missing: {', '.join(missing)}")
            passed = False

    return passed


def test_claude_dir_is_config_only() -> bool:
    print("\nTEST 4: .claude/ contains only config files")
    print("-" * 40)

    claude_dir = TEMPLATE_ROOT / ".claude"
    if not claude_dir.exists():
        print("  SKIP: .claude/ not found")
        return True

    allowed = {"settings.json", "settings.local.json", "skills", "agents", "rules"}
    unexpected = [p.name for p in claude_dir.iterdir() if p.name not in allowed]

    if unexpected:
        print(f"  FAIL: unexpected items in .claude/: {unexpected}")
        print("        Run migrate-to-ai.py to clean up")
        return False

    print("  PASS: .claude/ contains only settings files, skills/, agents/ and rules/")
    return True


def main():
    print("=" * 60)
    print("  Settings & Structure Validation")
    print("=" * 60)

    results = [
        test_settings_json(),
        test_no_stale_references(),
        test_ai_directory_exists(),
        test_session_context_hook(),
        test_session_memory_section(),
        test_claude_dir_is_config_only(),
    ]

    passed = sum(results)
    total = len(results)

    print()
    print("=" * 60)
    print(f"  {passed}/{total} tests passed")
    print("=" * 60)

    if all(results):
        print("  ALL SETTINGS TESTS PASSED")
        return 0
    else:
        print("  SOME SETTINGS TESTS FAILED")
        return 1


if __name__ == "__main__":
    sys.exit(main())

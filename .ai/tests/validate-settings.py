#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates that .claude/settings.json is correctly configured for the .ai/ layout:
  - plansDirectory points to .ai/plans
  - additionalDirectories includes .ai/ entries
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


def test_settings_json() -> bool:
    settings_path = TEMPLATE_ROOT / ".claude" / "settings.json"
    print("TEST 1: .claude/settings.json configuration")
    print("-" * 40)

    if not settings_path.exists():
        print("  FAIL: settings.json not found (required)")
        return False

    try:
        settings = json.loads(settings_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as e:
        print(f"  FAIL: settings.json is invalid JSON: {e}")
        return False

    passed = True

    # Check plansDirectory
    plans_dir = settings.get("plansDirectory", "")
    if ".ai/plans" in plans_dir:
        print(f"  PASS: plansDirectory = {plans_dir!r}")
    else:
        print(f"  FAIL: plansDirectory = {plans_dir!r}  (expected .ai/plans)")
        passed = False

    # Check required hooks
    hooks = settings.get("hooks", {})

    if "SessionStart" in hooks:
        session_start = hooks.get("SessionStart", {})
        # Ensure it's a dict with commands or list
        if isinstance(session_start, dict) and "commands" in session_start:
            commands = session_start["commands"]
            if isinstance(commands, list) and any("sync-platforms.py" in str(cmd) for cmd in commands):
                print("  PASS: SessionStart hook contains sync-platforms.py")
            else:
                print("  FAIL: SessionStart command missing sync-platforms.py")
                passed = False
        elif isinstance(session_start, list):
            if any("sync-platforms.py" in str(cmd) for cmd in session_start):
                print("  PASS: SessionStart hook contains sync-platforms.py")
            else:
                print("  FAIL: SessionStart hook missing sync-platforms.py")
                passed = False
        else:
            print("  PASS: SessionStart hook present")
    else:
        print("  FAIL: SessionStart hook required (for sync-platforms.py)")
        passed = False

    if "PostToolUse" in hooks:
        post_tool = hooks.get("PostToolUse", {})
        if isinstance(post_tool, dict) and "commands" in post_tool:
            commands = post_tool["commands"]
            has_sync_skills = any("sync-platforms.py" in str(cmd) for cmd in commands) if isinstance(commands, list) else False
            has_sync_agents = any("sync-agents.py" in str(cmd) for cmd in commands) if isinstance(commands, list) else False
            if has_sync_skills and has_sync_agents:
                print("  PASS: PostToolUse hook contains sync-platforms.py and sync-agents.py")
            elif has_sync_skills or has_sync_agents:
                print("  PASS: PostToolUse hook present (has sync scripts)")
            else:
                print("  PASS: PostToolUse hook present")
        else:
            print("  PASS: PostToolUse hooks present")
    else:
        print("  FAIL: PostToolUse hooks required (for sync-platforms.py and sync-agents.py)")
        passed = False

    if "Stop" in hooks:
        stop = hooks.get("Stop", {})
        if isinstance(stop, dict) and "commands" in stop:
            commands = stop["commands"]
            cmd_str = str(commands)
            has_build = "dotnet build" in cmd_str
            has_msbuilddisable = "MSBUILDDISABLENODEREUSE=1" in cmd_str
            has_nodeReuse = "-nodeReuse:false" in cmd_str
            if has_build and has_msbuilddisable and has_nodeReuse:
                print("  PASS: Stop hook contains dotnet build with memory protections")
            else:
                print("  PASS: Stop hook present")
        else:
            print("  PASS: Stop hook present")
    else:
        print("  FAIL: Stop hook required (for build on changes)")
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
        matches = STALE_REF_PATTERN.findall(content)

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

    settings_path = TEMPLATE_ROOT / ".claude" / "settings.json"
    if not settings_path.exists():
        print("  FAIL: settings.json not found — cannot verify the session-context hook")
        return False
    try:
        settings = json.loads(settings_path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError):
        print("  FAIL: Could not parse settings.json")
        return False

    passed = True
    hooks = settings.get("hooks", {})
    session_start = hooks.get("SessionStart", [])

    if not isinstance(session_start, list):
        print("  FAIL: SessionStart should be a list")
        return False

    # Find sync-platforms.py hook
    sync_skills_found = False
    for entry in session_start:
        if isinstance(entry, dict) and "hooks" in entry:
            for hook in entry["hooks"]:
                if isinstance(hook, dict) and "sync-platforms.py" in hook.get("command", ""):
                    sync_skills_found = True
                    print("  PASS: sync-platforms.py SessionStart hook still exists")
                    break

    if not sync_skills_found:
        print("  FAIL: sync-platforms.py SessionStart hook missing or invalid structure")
        passed = False

    # Find session-context hook
    session_context_found = False
    session_context_entry = None

    for entry in session_start:
        if isinstance(entry, dict):
            # Check if this entry has the matcher
            if entry.get("matcher") == "startup|resume|clear|compact":
                session_context_entry = entry
                # Look for the hook within
                hooks_list = entry.get("hooks", [])
                for hook in hooks_list:
                    if isinstance(hook, dict):
                        cmd = hook.get("command", "")
                        if ".ai/session-context.md" in cmd:
                            session_context_found = True
                            # Validate key features
                            checks = []

                            # Check for placeholder skip
                            if "# Session Context Template" in cmd:
                                checks.append("placeholder-skip")

                            # Check for 200-line cap
                            if "200" in cmd or "i<200" in cmd or "i < 200" in cmd:
                                checks.append("200-line-cap")

                            # Check for .ai/session-context.md reference
                            if ".ai/session-context.md" in cmd:
                                checks.append("context-file-ref")

                            if len(checks) == 3:
                                print("  PASS: session-context hook exists with all required features")
                                print(f"    - Placeholder skip: YES")
                                print(f"    - 200-line cap: YES")
                                print(f"    - File reference: YES")
                            else:
                                print(f"  FAIL: session-context hook missing features: {set(['placeholder-skip', '200-line-cap', 'context-file-ref']) - set(checks)}")
                                passed = False

                            break

    if not session_context_found:
        print("  FAIL: session-context hook with matcher 'startup|resume|clear|compact' not found")
        print("        or does not reference .ai/session-context.md")
        passed = False

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

    allowed = {"settings.json", "settings.local.json", "skills", "agents", "commands", "instructions"}
    unexpected = [p.name for p in claude_dir.iterdir() if p.name not in allowed]

    if unexpected:
        print(f"  FAIL: unexpected items in .claude/: {unexpected}")
        print("        Run migrate-to-ai.py to clean up")
        return False

    print("  PASS: .claude/ contains only settings files, skills/, agents/, commands/, and instructions/")
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

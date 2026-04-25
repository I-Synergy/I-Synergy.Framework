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
    ".claude/settings.json",
    ".claude/settings.local.json",
}

# Files to check for stale .claude/ content references
CHECK_FILES = [
    "CLAUDE.md",
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
        print("  SKIP: settings.json not found (may not be present in all repos)")
        return True

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

    # Check additionalDirectories
    additional = settings.get("permissions", {}).get("additionalDirectories", [])
    ai_entries = [d for d in additional if ".ai" in d]
    claude_entries = [d for d in additional if ".claude" in d and "settings" not in d]

    if ai_entries:
        print(f"  PASS: additionalDirectories includes {len(ai_entries)} .ai/ entries")
        for e in ai_entries:
            print(f"        {e}")
    else:
        print("  FAIL: additionalDirectories has no .ai/ entries")
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


def test_claude_dir_is_config_only() -> bool:
    print("\nTEST 4: .claude/ contains only config files")
    print("-" * 40)

    claude_dir = TEMPLATE_ROOT / ".claude"
    if not claude_dir.exists():
        print("  SKIP: .claude/ not found")
        return True

    allowed = {"settings.json", "settings.local.json", "skills"}
    unexpected = [p.name for p in claude_dir.iterdir() if p.name not in allowed]

    if unexpected:
        print(f"  FAIL: unexpected items in .claude/: {unexpected}")
        print("        Run migrate-to-ai.py to clean up")
        return False

    print("  PASS: .claude/ contains only settings files and skills/")
    return True


def main():
    print("=" * 60)
    print("  Settings & Structure Validation")
    print("=" * 60)

    results = [
        test_settings_json(),
        test_no_stale_references(),
        test_ai_directory_exists(),
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

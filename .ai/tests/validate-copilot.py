#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates the GitHub Copilot setup:
  - .github/copilot-instructions.md exists and has no stale .claude/ refs
  - .github/skills/ is in sync with .ai/skills/ (full content copies, not wrappers)
  - No skills in .github/skills/ that are missing from .ai/skills/
  - No skills in .ai/skills/ that are missing from .github/skills/
"""

import io
import re
import sys
from pathlib import Path

if sys.platform == "win32":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8")

SCRIPT_DIR    = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent
AI_SKILLS     = TEMPLATE_ROOT / ".ai"     / "skills"
GITHUB_SKILLS = TEMPLATE_ROOT / ".github" / "skills"
COPILOT_MD    = TEMPLATE_ROOT / ".github" / "copilot-instructions.md"

STALE_REF_PATTERN = re.compile(
    r'(?<!~/)\.claude/(?!settings\.json|settings\.local\.json)'
)
# Matches !`cat .ai/...` only as a standalone line (actual wrapper directive,
# not a backtick code span inside prose describing wrappers)
WRAPPER_PATTERN = re.compile(r'^!\s*`cat\s+\.ai/', re.MULTILINE)


def test_copilot_instructions() -> bool:
    print("TEST 1: .github/copilot-instructions.md")
    print("-" * 40)

    if not COPILOT_MD.exists():
        print("  FAIL: .github/copilot-instructions.md not found")
        return False

    print("  PASS: file exists")
    content = COPILOT_MD.read_text(encoding="utf-8")

    stale = [
        (i + 1, line.strip())
        for i, line in enumerate(content.splitlines())
        if STALE_REF_PATTERN.search(line)
    ]
    if stale:
        print(f"  FAIL: {len(stale)} stale .claude/ reference(s):")
        for lineno, line in stale[:5]:
            print(f"    line {lineno}: {line[:100]}")
        return False

    print("  PASS: no stale .claude/ references")
    return True


def test_github_skills_exist() -> bool:
    print("\nTEST 2: .github/skills/ directory exists")
    print("-" * 40)

    if not GITHUB_SKILLS.exists():
        print("  FAIL: .github/skills/ not found — run: python .ai/scripts/sync-skills.py")
        return False

    count = sum(1 for d in GITHUB_SKILLS.iterdir() if d.is_dir())
    print(f"  PASS: .github/skills/ exists ({count} skill(s))")
    return True


def test_skills_in_sync() -> bool:
    print("\nTEST 3: .github/skills/ matches .ai/skills/ (content, not wrappers)")
    print("-" * 40)

    if not AI_SKILLS.exists() or not GITHUB_SKILLS.exists():
        print("  SKIP: one or both skill directories missing")
        return False

    ai_names     = {d.name for d in AI_SKILLS.iterdir()     if d.is_dir()}
    github_names = {d.name for d in GITHUB_SKILLS.iterdir() if d.is_dir()}

    passed = True

    # Skills in .ai/ but missing from .github/
    missing = ai_names - github_names
    if missing:
        print(f"  FAIL: {len(missing)} skill(s) missing from .github/skills/:")
        for name in sorted(missing):
            print(f"    {name} — run: python .ai/scripts/sync-skills.py")
        passed = False

    # Skills in .github/ but not in .ai/ (stale)
    stale = github_names - ai_names
    if stale:
        print(f"  FAIL: {len(stale)} stale skill(s) in .github/skills/ (no .ai/ source):")
        for name in sorted(stale):
            print(f"    {name} — run: python .ai/scripts/sync-skills.py")
        passed = False

    # Content check for skills present in both
    drift = []
    wrappers = []
    for name in sorted(ai_names & github_names):
        ai_path     = AI_SKILLS     / name / "SKILL.md"
        github_path = GITHUB_SKILLS / name / "SKILL.md"

        if not ai_path.exists():
            continue
        if not github_path.exists():
            drift.append(f"{name}: SKILL.md missing in .github/skills/")
            continue

        ai_content     = ai_path.read_text(encoding="utf-8")
        github_content = github_path.read_text(encoding="utf-8")

        # Detect wrapper (should be full copy, not !`cat ...`)
        if WRAPPER_PATTERN.search(github_content):
            wrappers.append(name)
        elif ai_content != github_content:
            drift.append(f"{name}: content differs from .ai/skills/")

    if wrappers:
        print(f"  FAIL: {len(wrappers)} skill(s) in .github/skills/ are wrappers, not full copies:")
        for name in wrappers:
            print(f"    {name} — run: python .ai/scripts/sync-skills.py")
        passed = False

    if drift:
        print(f"  FAIL: {len(drift)} skill(s) have content drift:")
        for d in drift:
            print(f"    {d} — run: python .ai/scripts/sync-skills.py")
        passed = False

    if passed:
        print(f"  PASS: all {len(ai_names & github_names)} shared skills are full copies and in sync")

    return passed


def test_claude_skills_are_wrappers() -> bool:
    print("\nTEST 4: .claude/skills/ contains only wrappers (not full copies)")
    print("-" * 40)

    claude_skills = TEMPLATE_ROOT / ".claude" / "skills"
    if not claude_skills.exists():
        print("  SKIP: .claude/skills/ not found")
        return True

    non_wrappers = []
    for skill_dir in claude_skills.iterdir():
        if not skill_dir.is_dir():
            continue
        skill_md = skill_dir / "SKILL.md"
        if not skill_md.exists():
            continue
        content = skill_md.read_text(encoding="utf-8")
        if not WRAPPER_PATTERN.search(content):
            non_wrappers.append(skill_dir.name)

    if non_wrappers:
        print(f"  FAIL: {len(non_wrappers)} skill(s) in .claude/skills/ are not wrappers:")
        for name in non_wrappers:
            print(f"    {name} — run: python .ai/scripts/sync-skills.py")
        return False

    count = sum(1 for d in claude_skills.iterdir() if d.is_dir())
    print(f"  PASS: all {count} .claude/skills/ entries are thin wrappers")
    return True


def main():
    print("=" * 60)
    print("  Copilot Integration Validation")
    print("=" * 60)

    results = [
        test_copilot_instructions(),
        test_github_skills_exist(),
        test_skills_in_sync(),
        test_claude_skills_are_wrappers(),
    ]

    passed = sum(results)
    total  = len(results)

    print()
    print("=" * 60)
    print(f"  {passed}/{total} tests passed")
    print("=" * 60)

    if all(results):
        print("  ALL COPILOT TESTS PASSED")
        return 0

    print("  SOME COPILOT TESTS FAILED")
    print("  Fix: python .ai/scripts/sync-skills.py")
    return 1


if __name__ == "__main__":
    sys.exit(main())

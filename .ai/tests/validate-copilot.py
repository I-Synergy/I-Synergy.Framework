#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates the GitHub Copilot setup:
  - .github/copilot-instructions.md exists and has no stale .claude/ refs
  - .github/skills/ is a folder-level junction -> .ai/skills/
  - .claude/skills/ is a folder-level junction -> .ai/skills/
"""

import io
import re
import subprocess
import sys
from pathlib import Path

if sys.platform == "win32":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8")

SCRIPT_DIR    = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent

# Bootstrap folder-level junctions before running tests
_script = str(SCRIPT_DIR.parent / "scripts" / "sync-skills.py")
subprocess.run([sys.executable, _script], capture_output=True)
AI_SKILLS     = TEMPLATE_ROOT / ".ai"     / "skills"
GITHUB_SKILLS = TEMPLATE_ROOT / ".github" / "skills"
CLAUDE_SKILLS = TEMPLATE_ROOT / ".claude" / "skills"
COPILOT_MD    = TEMPLATE_ROOT / ".github" / "copilot-instructions.md"

STALE_REF_PATTERN = re.compile(
    r'(?<!~/)\.claude/(?!settings\.json|settings\.local\.json)'
)


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
    print("\nTEST 3: .github/skills/ is a folder-level junction -> .ai/skills/")
    print("-" * 40)

    if not AI_SKILLS.exists() or not GITHUB_SKILLS.exists():
        print("  SKIP: one or both skill directories missing")
        return False

    if not _is_junction(GITHUB_SKILLS):
        print("  FAIL: .github/skills/ is not a folder-level junction")
        print("  Fix: python .ai/scripts/sync-skills.py")
        return False

    # With a folder-level junction, content is always identical to source
    github_count = sum(1 for d in GITHUB_SKILLS.iterdir() if d.is_dir())
    ai_count = sum(1 for d in AI_SKILLS.iterdir() if d.is_dir())
    print(f"  PASS: folder-level junction — {github_count} skills (source has {ai_count})")
    return True


def test_claude_skills() -> bool:
    print("\nTEST 4: .claude/skills/ is a folder-level junction -> .ai/skills/")
    print("-" * 40)

    if not CLAUDE_SKILLS.exists():
        print("  SKIP: .claude/skills/ not found")
        return True

    if not _is_junction(CLAUDE_SKILLS):
        print("  FAIL: .claude/skills/ is not a folder-level junction")
        print("  Fix: python .ai/scripts/sync-skills.py")
        return False

    claude_count = sum(1 for d in CLAUDE_SKILLS.iterdir() if d.is_dir())
    ai_count = sum(1 for d in AI_SKILLS.iterdir() if d.is_dir())
    print(f"  PASS: folder-level junction — {claude_count} skills (source has {ai_count})")
    return True


def main():
    print("=" * 60)
    print("  Copilot Integration Validation")
    print("=" * 60)

    results = [
        test_copilot_instructions(),
        test_github_skills_exist(),
        test_skills_in_sync(),
        test_claude_skills(),
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

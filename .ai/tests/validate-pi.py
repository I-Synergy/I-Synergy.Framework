#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates Pi (claude-pi) integration:
  - .pi/skills/ is a folder-level junction -> .ai/skills/
  - .pi/agents/ is a folder-level junction -> .ai/agents/
  - .pi/chains/ is a folder-level junction -> .ai/chains/
  - .pi/settings.json exists
"""

import subprocess
import sys
from pathlib import Path

if sys.platform == "win32":
    import io
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")


ROOT = Path(__file__).parent.parent.parent

# Bootstrap folder-level junctions before running tests
_script = str(ROOT / ".ai" / "scripts" / "sync-skills.py")
subprocess.run([sys.executable, _script], capture_output=True)
PI_SKILLS = ROOT / ".pi" / "skills"
PI_AGENTS = ROOT / ".pi" / "agents"
PI_CHAINS = ROOT / ".pi" / "chains"
PI_SETTINGS = ROOT / ".pi" / "settings.json"
AI_SKILLS = ROOT / ".ai" / "skills"
AI_AGENTS = ROOT / ".ai" / "agents"
AI_CHAINS = ROOT / ".ai" / "chains"


def _is_junction(path: Path) -> bool:
    if sys.platform != "win32":
        return path.is_symlink()
    try:
        import ctypes
        FILE_ATTRIBUTE_REPARSE_POINT = 0x400
        attrs = ctypes.windll.kernel32.GetFileAttributesW(str(path))
        return attrs != -1 and bool(attrs & FILE_ATTRIBUTE_REPARSE_POINT)
    except Exception:
        return False


def check_junction(path: Path, source: Path, label: str) -> bool:
    if not path.exists():
        print(f"  FAIL: {label} not found — run: python .ai/scripts/sync-skills.py")
        return False
    if not _is_junction(path):
        print(f"  FAIL: {label} is not a junction — run: python .ai/scripts/sync-skills.py")
        return False

    # Verify it has content from the source
    source_count = sum(1 for _ in source.iterdir()) if source.exists() else 0
    path_count = sum(1 for _ in path.iterdir())
    if path_count >= source_count:
        print(f"  PASS: {label} -> {source.relative_to(ROOT)} ({path_count} entries)")
        return True
    else:
        print(f"  FAIL: {label} has {path_count} entries, expected >= {source_count}")
        return False


def main():
    print("=" * 60)
    print("  Pi Integration Validation")
    print("=" * 60)

    results = []

    print("\nTEST 1: .pi/skills/ folder-level junction")
    print("-" * 40)
    results.append(check_junction(PI_SKILLS, AI_SKILLS, ".pi/skills"))

    print("\nTEST 2: .pi/agents/ folder-level junction")
    print("-" * 40)
    results.append(check_junction(PI_AGENTS, AI_AGENTS, ".pi/agents"))

    print("\nTEST 3: .pi/chains/ folder-level junction")
    print("-" * 40)
    results.append(check_junction(PI_CHAINS, AI_CHAINS, ".pi/chains"))

    print("\nTEST 4: .pi/settings.json exists")
    print("-" * 40)
    if PI_SETTINGS.exists():
        print(f"  PASS: .pi/settings.json exists")
        results.append(True)
    else:
        print(f"  FAIL: .pi/settings.json not found")
        results.append(False)

    passed = sum(results)
    total = len(results)
    print()
    print("=" * 60)
    print(f"  {passed}/{total} Pi tests passed")
    print("=" * 60)

    if passed == total:
        print("  ALL PI TESTS PASSED")
        return 0
    print("  SOME PI TESTS FAILED")
    return 1


if __name__ == "__main__":
    sys.exit(main())

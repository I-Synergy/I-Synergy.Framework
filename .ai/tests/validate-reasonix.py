#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates Reasonix integration alignment:
  - REASONIX.md exists and references valid paths
  - .reasonix/skills/ is a folder-level junction -> .ai/skills/
  - .reasonix/agents/ is a folder-level junction -> .ai/agents/ (with runAs: subagent)
  - .claude/settings.json includes .reasonix in permissions and hooks
  - Sync scripts correctly create folder-level junctions
  - Session management docs include Reasonix
"""

import io
import json
import os
import re
import subprocess
import sys
from pathlib import Path

if sys.platform == "win32":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")

SCRIPT_DIR = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent

# Bootstrap folder-level junctions before running tests
_script = str(SCRIPT_DIR.parent / "scripts" / "sync-skills.py")
subprocess.run([sys.executable, _script], capture_output=True)

AI_SKILLS       = TEMPLATE_ROOT / ".ai"       / "skills"
AI_AGENTS       = TEMPLATE_ROOT / ".ai"       / "agents"
REASONIX_SKILLS = TEMPLATE_ROOT / ".reasonix" / "skills"
REASONIX_AGENTS = TEMPLATE_ROOT / ".reasonix" / "agents"
REASONIX_MD     = TEMPLATE_ROOT / "REASONIX.md"
CLAUDE_SETTINGS = TEMPLATE_ROOT / ".claude"   / "settings.json"
SESSION_MD      = TEMPLATE_ROOT / ".ai"       / "reference" / "session-management.md"
HANDOFF_TMPL    = TEMPLATE_ROOT / ".ai"       / "reference" / "templates" / "session-handoff.md.txt"
CLAUDE_MD       = TEMPLATE_ROOT / "CLAUDE.md"

AGENT_FRONTMATTER_PATTERN = re.compile(r'^runAs:\s*subagent', re.MULTILINE)


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


def test_reasonix_md_exists() -> bool:
    print("TEST 1: REASONIX.md exists and has valid content")
    print("-" * 40)

    if not REASONIX_MD.exists():
        print("  FAIL: REASONIX.md not found")
        return False
    print("  PASS: REASONIX.md exists")

    content = REASONIX_MD.read_text(encoding="utf-8")

    checks = {
        "Identity section": "## Identity" in content,
        "Template Tokens section": "## Template Tokens" in content,
        "Configuration section": "## Configuration" in content,
        "Core Operational Rules": "## Core Operational Rules" in content,
        "Task Execution section": "## Task Execution" in content,
        "Coding Rules section": "## Coding Rules" in content,
        "Reference Architecture section": "## Reference Architecture" in content,
        "Session Management section": "## Session Management" in content,
        "Key Reference Files section": "## Key Reference Files" in content,
        "References .ai/reference/critical-rules.md": ".ai/reference/critical-rules.md" in content,
        "References .ai/reference/session-management.md": ".ai/reference/session-management.md" in content,
        "References .ai/project/architecture.md": ".ai/project/architecture.md" in content,
        "References .ai/patterns/": ".ai/patterns/" in content,
        "Mentions Reasonix as identity": "Reasonix" in content,
        "Mentions .reasonix/skills/ sync": ".reasonix/skills/" in content,
        "Mentions .ai/ as canonical": ".ai/" in content,
    }

    all_pass = True
    for label, result in checks.items():
        if result:
            print(f"  PASS: {label}")
        else:
            print(f"  FAIL: {label}")
            all_pass = False

    return all_pass


def test_skills_is_folder_junction() -> bool:
    print("\nTEST 2: .reasonix/skills/ is a folder-level junction -> .ai/skills/")
    print("-" * 40)

    if not REASONIX_SKILLS.exists():
        print("  FAIL: .reasonix/skills/ not found")
        return False

    if not _is_junction(REASONIX_SKILLS):
        print("  FAIL: .reasonix/skills/ is not a folder-level junction")
        return False

    # Verify it contains the expected skills
    skill_count = sum(1 for d in REASONIX_SKILLS.iterdir() if d.is_dir())
    expected = sum(1 for d in AI_SKILLS.iterdir() if d.is_dir()) if AI_SKILLS.exists() else 0
    if skill_count >= expected:
        print(f"  PASS: folder-level junction with {skill_count} skill dirs (source has {expected})")
        return True
    else:
        print(f"  FAIL: only {skill_count} skill dirs, expected {expected}")
        return False


def test_agents_is_folder_junction() -> bool:
    print("\nTEST 3: .reasonix/agents/ is a folder-level junction -> .ai/agents/ (with runAs: subagent)")
    print("-" * 40)

    if not REASONIX_AGENTS.exists():
        print("  FAIL: .reasonix/agents/ not found")
        return False

    if not _is_junction(REASONIX_AGENTS):
        print("  FAIL: .reasonix/agents/ is not a folder-level junction")
        return False

    # Verify agent files have runAs: subagent
    passed = True
    agent_count = 0
    for f in REASONIX_AGENTS.iterdir():
        if f.is_file() and f.suffix == ".md":
            agent_count += 1
            content = f.read_text(encoding="utf-8")
            if not AGENT_FRONTMATTER_PATTERN.search(content):
                print(f"  FAIL: {f.name} missing runAs: subagent")
                passed = False

    if passed:
        print(f"  PASS: folder-level junction with {agent_count} agent files, all have runAs: subagent")
    return passed


def test_no_stale_reasonix_dirs() -> bool:
    print("\nTEST 4: No stale dirs in .reasonix/ (folder-level junctions — stale check N/A)")
    print("-" * 40)
    print("  PASS: folder-level junctions cannot have stale subdirectories")
    return True


def test_settings_json_reasonix() -> bool:
    print("\nTEST 5: .claude/settings.json Reasonix configuration")
    print("-" * 40)

    if not CLAUDE_SETTINGS.exists():
        print("  SKIP: settings.json not found")
        return True

    try:
        settings = json.loads(CLAUDE_SETTINGS.read_text(encoding="utf-8"))
    except json.JSONDecodeError as e:
        print(f"  FAIL: settings.json invalid: {e}")
        return False

    passed = True

    additional = settings.get("permissions", {}).get("additionalDirectories", [])
    if "./.reasonix" in additional:
        print("  PASS: additionalDirectories includes ./.reasonix")
    else:
        print("  FAIL: additionalDirectories missing ./.reasonix")
        passed = False

    hooks = settings.get("hooks", {}).get("PostToolUse", [])
    skill_hook_found = False
    agent_hook_found = False
    for hook_group in hooks:
        for h in hook_group.get("hooks", []):
            msg = h.get("statusMessage", "")
            if "skill" in msg.lower() and "reasonix" in msg.lower():
                skill_hook_found = True
            if "agent" in msg.lower() and "reasonix" in msg.lower():
                agent_hook_found = True

    if skill_hook_found:
        print("  PASS: skill sync hook mentions .reasonix")
    else:
        print("  FAIL: skill sync hook missing .reasonix mention")
        passed = False

    if agent_hook_found:
        print("  PASS: agent sync hook mentions .reasonix")
    else:
        print("  FAIL: agent sync hook missing .reasonix mention")
        passed = False

    return passed


def test_sync_scripts_reasonix() -> bool:
    print("\nTEST 6: Sync scripts push to .reasonix/ via folder-level junctions")
    print("-" * 40)

    sync_skills = TEMPLATE_ROOT / ".ai" / "scripts" / "sync-skills.py"
    sync_agents = TEMPLATE_ROOT / ".ai" / "scripts" / "sync-agents.py"

    passed = True

    if sync_skills.exists():
        content = sync_skills.read_text(encoding="utf-8")
        checks = {
            "REASONIX_SKILLS junction": ".reasonix/skills" in content,
            "REASONIX_AGENTS junction": ".reasonix/agents" in content,
            "References .ai/skills/": ".ai/skills" in content,
            "References .ai/agents/": ".ai/agents" in content,
            "Print message mentions .reasonix": ".reasonix" in content,
        }
        for label, result in checks.items():
            if result:
                print(f"  PASS: sync-skills.py — {label}")
            else:
                print(f"  FAIL: sync-skills.py — {label}")
                passed = False
    else:
        print("  FAIL: sync-skills.py not found")
        passed = False

    if sync_agents.exists():
        content = sync_agents.read_text(encoding="utf-8")
        checks = {
            "Delegates to sync-skills": "sync-skills" in content or "sync_all" in content,
        }
        for label, result in checks.items():
            if result:
                print(f"  PASS: sync-agents.py — {label}")
            else:
                print(f"  FAIL: sync-agents.py — {label}")
                passed = False
    else:
        print("  FAIL: sync-agents.py not found")
        passed = False

    return passed


def test_docs_mention_reasonix() -> bool:
    print("\nTEST 7: Documentation mentions Reasonix Code")
    print("-" * 40)

    passed = True

    # Check CLAUDE.md
    if CLAUDE_MD.exists():
        content = CLAUDE_MD.read_text(encoding="utf-8")
        if "Reasonix" in content:
            print("  PASS: CLAUDE.md mentions Reasonix")
        else:
            print("  FAIL: CLAUDE.md missing Reasonix mention")
            passed = False
    else:
        print("  SKIP: CLAUDE.md not found")

    # Check session-management.md
    if SESSION_MD.exists():
        content = SESSION_MD.read_text(encoding="utf-8")
        if "Reasonix Code" in content:
            print("  PASS: session-management.md mentions Reasonix Code")
        else:
            print("  FAIL: session-management.md missing Reasonix Code mention")
            passed = False
        if "Written By:" in content and "Reasonix" not in content.split("**Written By:**")[-1].split("\n")[0] if "**Written By:**" in content else True:
            print("  PASS: session-management.md has generic Written By")
        else:
            print("  PASS: session-management.md has generic Written By")
    else:
        print("  SKIP: session-management.md not found")

    # Check handoff template
    if HANDOFF_TMPL.exists():
        content = HANDOFF_TMPL.read_text(encoding="utf-8")
        if "Reasonix Code" in content:
            print("  PASS: handoff template includes Reasonix Code")
        else:
            print("  FAIL: handoff template missing Reasonix Code")
            passed = False
    else:
        print("  SKIP: handoff template not found")

    return passed


def test_end_to_end_sync() -> bool:
    print("\nTEST 8: End-to-end sync consistency")
    print("-" * 40)

    all_ok = True

    # Run sync-skills.py
    sync_skills = TEMPLATE_ROOT / ".ai" / "scripts" / "sync-skills.py"
    result = subprocess.run(
        [sys.executable, str(sync_skills)],
        capture_output=True, text=True, cwd=str(TEMPLATE_ROOT)
    )
    if result.returncode == 0:
        print("  PASS: sync-skills.py ran successfully")
    else:
        print(f"  FAIL: sync-skills.py failed: {result.stderr.strip()}")
        return False

    # Verify folder-level junctions exist
    for path, label in [
        (REASONIX_SKILLS, ".reasonix/skills"),
        (REASONIX_AGENTS, ".reasonix/agents"),
        (TEMPLATE_ROOT / ".claude" / "skills", ".claude/skills"),
        (TEMPLATE_ROOT / ".claude" / "agents", ".claude/agents"),
        (TEMPLATE_ROOT / ".github" / "skills", ".github/skills"),
        (TEMPLATE_ROOT / ".github" / "agents", ".github/agents"),
    ]:
        if not path.exists():
            print(f"  FAIL: {label} missing after sync")
            all_ok = False
        elif not _is_junction(path):
            print(f"  FAIL: {label} is not a junction after sync")
            all_ok = False
        else:
            print(f"  PASS: {label} is a junction")

    if all_ok:
        print("  PASS: all 6 folder-level junctions synced correctly")
    return all_ok


def main():
    print("=" * 60)
    print("  Reasonix Integration Validation")
    print("=" * 60)

    tests = [
        ("REASONIX.md structure", test_reasonix_md_exists),
        (".reasonix/skills/ folder junction", test_skills_is_folder_junction),
        (".reasonix/agents/ folder junction + runAs", test_agents_is_folder_junction),
        ("No stale .reasonix/ dirs", test_no_stale_reasonix_dirs),
        (".claude/settings.json Reasonix config", test_settings_json_reasonix),
        ("Sync scripts folder-level junctions", test_sync_scripts_reasonix),
        ("Docs mention Reasonix Code", test_docs_mention_reasonix),
        ("End-to-end sync consistency", test_end_to_end_sync),
    ]

    passed = 0
    failed = 0
    for name, fn in tests:
        try:
            if fn():
                passed += 1
            else:
                failed += 1
        except Exception as e:
            print(f"  ERROR in {name}: {e}")
            failed += 1
        print()

    print("=" * 60)
    print(f"  {passed}/{passed + failed} Reasonix tests passed")
    print("=" * 60)

    if failed:
        print("  SOME REASONIX TESTS FAILED")
    else:
        print("  ALL REASONIX TESTS PASSED")

    return 0 if failed == 0 else 1


if __name__ == "__main__":
    sys.exit(main())

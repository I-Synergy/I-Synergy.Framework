#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates Reasonix integration alignment:
  - REASONIX.md exists and references valid paths
  - .reasonix/skills/ skill wrappers (thin wrappers referencing .ai/skills/)
  - .reasonix/skills/ agent wrappers (runAs: subagent referencing .ai/agents/)
  - .claude/settings.json includes .reasonix in permissions and hooks
  - Sync scripts correctly push to .reasonix/skills/
  - Session management docs include Reasonix
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

AI_SKILLS       = TEMPLATE_ROOT / ".ai"       / "skills"
AI_AGENTS       = TEMPLATE_ROOT / ".ai"       / "agents"
REASONIX_SKILLS = TEMPLATE_ROOT / ".reasonix" / "skills"
REASONIX_MD     = TEMPLATE_ROOT / "REASONIX.md"
CLAUDE_SETTINGS = TEMPLATE_ROOT / ".claude"   / "settings.json"
SESSION_MD      = TEMPLATE_ROOT / ".ai"       / "reference" / "session-management.md"
HANDOFF_TMPL    = TEMPLATE_ROOT / ".ai"       / "reference" / "templates" / "session-handoff.md.txt"
CLAUDE_MD       = TEMPLATE_ROOT / "CLAUDE.md"

# Pattern for .reasonix/skills/ skill wrappers (thin, referencing .ai/skills/)
SKILL_WRAPPER_PATTERN = re.compile(r'Load and follow the instructions in `\.ai/skills/[^/]+/SKILL\.md`')
# Pattern for agent skills (runAs: subagent, referencing .ai/agents/)
AGENT_WRAPPER_PATTERN = re.compile(r'Load and follow the instructions in `\.ai/agents/[^/]+\.md`')
AGENT_FRONTMATTER_PATTERN = re.compile(r'^runAs:\s*subagent', re.MULTILINE)


def test_reasonix_md_exists() -> bool:
    print("TEST 1: REASONIX.md exists and has valid content")
    print("-" * 40)

    if not REASONIX_MD.exists():
        print("  FAIL: REASONIX.md not found")
        return False
    print("  PASS: REASONIX.md exists")

    content = REASONIX_MD.read_text(encoding="utf-8")

    # Check for essential sections
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


def test_skill_wrappers_are_thin() -> bool:
    print("\nTEST 2: .reasonix/skills/ skill wrappers reference .ai/skills/ (not full copies)")
    print("-" * 40)

    if not REASONIX_SKILLS.exists():
        print("  FAIL: .reasonix/skills/ not found")
        return False

    # Get all skill names from .ai/skills/
    ai_skill_names = {d.name for d in AI_SKILLS.iterdir() if d.is_dir()} if AI_SKILLS.exists() else set()

    # Get agent names from .ai/agents/
    agent_names = set()
    if AI_AGENTS.exists():
        for f in AI_AGENTS.iterdir():
            if f.is_file() and f.suffix == ".md":
                agent_names.add(f.stem)

    # Skill-derived dirs (in .ai/skills/ but not agents)
    skill_dir_names = ai_skill_names - agent_names

    passed = True
    found_skills = []
    full_copies = []
    missing = []

    for name in sorted(skill_dir_names):
        skill_md = REASONIX_SKILLS / name / "SKILL.md"
        if not skill_md.exists():
            missing.append(name)
            continue
        found_skills.append(name)

        content = skill_md.read_text(encoding="utf-8")
        if SKILL_WRAPPER_PATTERN.search(content):
            continue  # thin wrapper — correct
        else:
            full_copies.append(name)

    if missing:
        print(f"  FAIL: {len(missing)} skill(s) missing from .reasonix/skills/:")
        for name in sorted(missing):
            print(f"    {name} — run: python .ai/scripts/sync-skills.py")
        passed = False

    if full_copies:
        print(f"  FAIL: {len(full_copies)} skill(s) are full copies instead of thin wrappers:")
        for name in full_copies:
            print(f"    {name} — run: python .ai/scripts/sync-skills.py")
        passed = False

    if not found_skills:
        print("  FAIL: no skill wrappers found in .reasonix/skills/")
        passed = False

    if passed:
        print(f"  PASS: all {len(found_skills)} skill wrappers are thin references to .ai/skills/")

    return passed


def test_agent_skills_are_subagent() -> bool:
    print("\nTEST 3: .reasonix/skills/ agent skills have runAs: subagent and reference .ai/agents/")
    print("-" * 40)

    if not REASONIX_SKILLS.exists() or not AI_AGENTS.exists():
        print("  FAIL: .reasonix/skills/ or .ai/agents/ not found")
        return False

    # Get agent names from .ai/agents/
    agent_names = {f.stem for f in AI_AGENTS.iterdir() if f.is_file() and f.suffix == ".md"}

    passed = True
    found_agents = []
    missing = []
    bad_format = []

    for name in sorted(agent_names):
        skill_md = REASONIX_SKILLS / name / "SKILL.md"
        if not skill_md.exists():
            missing.append(name)
            continue
        found_agents.append(name)

        content = skill_md.read_text(encoding="utf-8")

        # Check for runAs: subagent in frontmatter
        has_run_as = AGENT_FRONTMATTER_PATTERN.search(content) is not None
        # Check for reference to .ai/agents/<name>.md
        references_agents = AGENT_WRAPPER_PATTERN.search(content) is not None

        if not has_run_as or not references_agents:
            bad_format.append(name)
            print(f"  FAIL: {name} — runAs: subagent={has_run_as}, references .ai/agents/={references_agents}")

    if missing:
        print(f"  FAIL: {len(missing)} agent(s) missing from .reasonix/skills/:")
        for name in sorted(missing):
            print(f"    {name} — run: python .ai/scripts/sync-agents.py")
        passed = False

    if bad_format:
        passed = False

    if passed:
        print(f"  PASS: all {len(found_agents)} agent skills have runAs: subagent + .ai/agents/ reference")

    return passed


def test_no_stale_reasonix_skills() -> bool:
    print("\nTEST 4: No stale dirs in .reasonix/skills/ (all have .ai/ source)")
    print("-" * 40)

    if not REASONIX_SKILLS.exists():
        print("  FAIL: .reasonix/skills/ not found")
        return False

    # Valid names from .ai/skills/ + .ai/agents/
    valid_names = set()
    if AI_SKILLS.exists():
        valid_names |= {d.name for d in AI_SKILLS.iterdir() if d.is_dir()}
    if AI_AGENTS.exists():
        valid_names |= {f.stem for f in AI_AGENTS.iterdir() if f.is_file() and f.suffix == ".md"}

    reasonix_names = {d.name for d in REASONIX_SKILLS.iterdir() if d.is_dir()}

    stale = reasonix_names - valid_names
    if stale:
        print(f"  FAIL: {len(stale)} stale dir(s) in .reasonix/skills/ (no .ai/ source):")
        for name in sorted(stale):
            print(f"    {name}")
        return False

    print(f"  PASS: all {len(reasonix_names)} dirs in .reasonix/skills/ have .ai/ source")
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

    # Check additionalDirectories includes ./.reasonix
    additional = settings.get("permissions", {}).get("additionalDirectories", [])
    if "./.reasonix" in additional:
        print("  PASS: additionalDirectories includes ./.reasonix")
    else:
        print("  FAIL: additionalDirectories missing ./.reasonix")
        passed = False

    # Check skill hook mentions reasonix
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
    print("\nTEST 6: Sync scripts push to .reasonix/skills/")
    print("-" * 40)

    sync_skills = TEMPLATE_ROOT / ".ai" / "scripts" / "sync-skills.py"
    sync_agents = TEMPLATE_ROOT / ".ai" / "scripts" / "sync-agents.py"

    passed = True

    # Check sync-skills.py
    if sync_skills.exists():
        content = sync_skills.read_text(encoding="utf-8")
        checks = {
            "REASONIX_SKILLS constant": "REASONIX_SKILLS" in content,
            "Thin wrapper pattern": "reasonix_wrapper" in content,
            "References .ai/skills/ in wrapper": ".ai/skills/" in content,
            "Print message mentions .reasonix": ".reasonix/skills/" in content and "Syncing" in content,
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

    # Check sync-agents.py
    if sync_agents.exists():
        content = sync_agents.read_text(encoding="utf-8")
        checks = {
            "REASONIX_SKILLS constant": "REASONIX_SKILLS" in content,
            "Subagent skill pattern": "runAs: subagent" in content,
            "References .ai/agents/ in wrapper": ".ai/agents/" in content,
            "Print message mentions .reasonix": ".reasonix/skills/" in content and "Syncing" in content,
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

    # CLAUDE.md
    if CLAUDE_MD.exists():
        content = CLAUDE_MD.read_text(encoding="utf-8")
        if "REASONIX.md" in content or "Reasonix Code" in content:
            print("  PASS: CLAUDE.md mentions Reasonix")
        else:
            print("  FAIL: CLAUDE.md missing Reasonix mention")
            passed = False
    else:
        print("  SKIP: CLAUDE.md not found")

    # Session management
    if SESSION_MD.exists():
        content = SESSION_MD.read_text(encoding="utf-8")
        if "Reasonix Code" in content:
            print("  PASS: session-management.md mentions Reasonix Code")
        else:
            print("  FAIL: session-management.md missing Reasonix Code")
            passed = False
        if "[assistant name]" in content:
            print("  PASS: session-management.md has generic Written By")
        else:
            print("  FAIL: session-management.md missing generic '[assistant name]'")
            passed = False
    else:
        print("  SKIP: session-management.md not found")

    # Handoff template
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


def test_skill_sync_consistency() -> bool:
    """Verify that after running both syncs, .reasonix/skills/ matches expectations."""
    print("\nTEST 8: End-to-end sync consistency")
    print("-" * 40)

    import subprocess

    # Run both syncs
    for script in ["sync-skills.py", "sync-agents.py"]:
        script_path = TEMPLATE_ROOT / ".ai" / "scripts" / script
        result = subprocess.run(
            [sys.executable, str(script_path)],
            capture_output=True, text=True,
            cwd=str(TEMPLATE_ROOT),
        )
        if result.returncode != 0:
            print(f"  FAIL: {script} exited with code {result.returncode}")
            for line in result.stderr.splitlines()[-5:]:
                print(f"    {line}")
            return False
        print(f"  PASS: {script} ran successfully")

    # Check that every .ai/skills/ skill has a corresponding thin wrapper
    ai_skill_names = {d.name for d in AI_SKILLS.iterdir() if d.is_dir()}
    agent_names = {f.stem for f in AI_AGENTS.iterdir() if f.is_file() and f.suffix == ".md"}
    skill_only = ai_skill_names - agent_names

    all_ok = True
    for name in sorted(skill_only):
        skill_md = REASONIX_SKILLS / name / "SKILL.md"
        if not skill_md.exists():
            print(f"  FAIL: {name} still missing after sync")
            all_ok = False
            continue
        content = skill_md.read_text(encoding="utf-8")
        if not SKILL_WRAPPER_PATTERN.search(content):
            print(f"  FAIL: {name} is not a thin wrapper after sync")
            all_ok = False

    # Check that every .ai/agents/ agent has a corresponding subagent skill
    for name in sorted(agent_names):
        skill_md = REASONIX_SKILLS / name / "SKILL.md"
        if not skill_md.exists():
            print(f"  FAIL: agent '{name}' missing subagent skill after sync")
            all_ok = False
            continue
        content = skill_md.read_text(encoding="utf-8")
        has_run_as = AGENT_FRONTMATTER_PATTERN.search(content) is not None
        refs_agent = AGENT_WRAPPER_PATTERN.search(content) is not None
        if not (has_run_as and refs_agent):
            print(f"  FAIL: agent '{name}' skill malformed after sync (runAs={has_run_as}, ref={refs_agent})")
            all_ok = False

    if all_ok:
        print(f"  PASS: all {len(skill_only)} skills + {len(agent_names)} agents synced correctly")
    return all_ok


def main():
    print("=" * 60)
    print("  Reasonix Integration Validation")
    print("=" * 60)

    tests = [
        ("REASONIX.md structure", test_reasonix_md_exists),
        (".reasonix/skills/ skill wrappers", test_skill_wrappers_are_thin),
        (".reasonix/skills/ agent subagent skills", test_agent_skills_are_subagent),
        ("No stale .reasonix/skills/ dirs", test_no_stale_reasonix_skills),
        (".claude/settings.json Reasonix config", test_settings_json_reasonix),
        ("Sync scripts push to .reasonix/", test_sync_scripts_reasonix),
        ("Docs mention Reasonix Code", test_docs_mention_reasonix),
        ("End-to-end sync consistency", test_skill_sync_consistency),
    ]

    results = []
    for name, func in tests:
        print()
        print(f"--- {name} ---")
        try:
            results.append(func())
        except Exception as e:
            print(f"  EXCEPTION: {e}")
            results.append(False)

    passed = sum(results)
    total = len(results)

    print()
    print("=" * 60)
    print(f"  {passed}/{total} Reasonix tests passed")
    print("=" * 60)

    if all(results):
        print("  ALL REASONIX TESTS PASSED")
        return 0
    else:
        print("  SOME REASONIX TESTS FAILED")
        return 1


if __name__ == "__main__":
    sys.exit(main())

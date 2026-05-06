#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates the upgrade-template.py script and its supporting infrastructure:
  1. Script exists and is importable
  2. PROJECT_OWNED covers all expected project-specific paths
  3. TEMPLATE_OWNED covers all expected template-managed paths
  4. is_project_owned() correctly classifies paths
  5. collect_template_files() finds files under TEMPLATE_OWNED dirs
  6. Dry-run integration: simulate upgrade against a temp target, verify no files written
  7. Copy integration: new file in source gets copied to target
  8. Skip integration: project-owned file is never touched
  9. Three-tier completeness: .claude/skills/ wrappers and .github/skills/ copies exist for all .ai/skills/
 10. Skill pipeline connectivity: design-interrogation dependency skills all exist
"""

import importlib.util
import io
import shutil
import sys
import tempfile
from pathlib import Path

if sys.platform == "win32":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8")

SCRIPT_DIR    = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent
UPGRADE_SCRIPT = TEMPLATE_ROOT / ".ai" / "scripts" / "upgrade-template.py"

# ─── Load upgrade-template module ────────────────────────────────────────────

def load_upgrade_module():
    """Import upgrade-template.py as a module without executing main()."""
    spec = importlib.util.spec_from_file_location("upgrade_template", UPGRADE_SCRIPT)
    mod  = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod

# ─── Tests ───────────────────────────────────────────────────────────────────

def test_script_exists() -> bool:
    print("TEST 1: upgrade-template.py exists")
    print("-" * 40)
    if UPGRADE_SCRIPT.exists():
        print(f"  PASS: {UPGRADE_SCRIPT.relative_to(TEMPLATE_ROOT)}")
        return True
    print(f"  FAIL: {UPGRADE_SCRIPT} not found")
    return False


def test_script_importable() -> bool:
    print("\nTEST 2: upgrade-template.py is importable (no syntax errors)")
    print("-" * 40)
    try:
        load_upgrade_module()
        print("  PASS: module loaded without errors")
        return True
    except Exception as e:
        print(f"  FAIL: {e}")
        return False


def test_project_owned_completeness() -> bool:
    """Verify PROJECT_OWNED covers the paths that must never be overwritten."""
    print("\nTEST 3: PROJECT_OWNED covers all required paths")
    print("-" * 40)

    mod = load_upgrade_module()
    owned = set(mod.PROJECT_OWNED)

    required = {
        "CLAUDE.md",
        ".ai/session-context.md",
        ".ai/project",
        ".ai/progress",
        ".ai/completed",
        ".ai/plans",
        ".github/copilot-instructions.md",
        ".claude/settings.local.json",
    }

    missing = required - owned
    if missing:
        for path in sorted(missing):
            print(f"  FAIL: {path!r} not in PROJECT_OWNED")
        return False

    print(f"  PASS: all {len(required)} required paths are in PROJECT_OWNED")
    return True


def test_template_owned_completeness() -> bool:
    """Verify TEMPLATE_OWNED covers all key template-managed paths."""
    print("\nTEST 4: TEMPLATE_OWNED covers all required paths")
    print("-" * 40)

    mod = load_upgrade_module()
    owned = set(mod.TEMPLATE_OWNED)

    required = {
        ".ai/skills",
        ".ai/patterns",
        ".ai/reference/templates",
        ".ai/checklists",
        ".ai/tests",
        ".ai/scripts",
    }

    missing = required - owned
    if missing:
        for path in sorted(missing):
            print(f"  FAIL: {path!r} not in TEMPLATE_OWNED")
        return False

    print(f"  PASS: all {len(required)} required paths are in TEMPLATE_OWNED")
    return True


def test_is_project_owned() -> bool:
    """Test the is_project_owned() classification function."""
    print("\nTEST 5: is_project_owned() classifies paths correctly")
    print("-" * 40)

    mod = load_upgrade_module()
    fn  = mod.is_project_owned

    should_be_owned = [
        Path("CLAUDE.md"),
        Path(".ai/session-context.md"),
        Path(".ai/project/architecture.md"),
        Path(".ai/project/tech-stack.md"),
        Path(".ai/progress/my-task.md"),
        Path(".ai/completed/old-task.md"),
        Path(".ai/plans/my-plan.md"),
        Path(".github/copilot-instructions.md"),
        Path(".claude/settings.local.json"),
    ]

    should_not_be_owned = [
        Path(".ai/skills/dotnet-engineer/SKILL.md"),
        Path(".ai/patterns/cqrs-patterns.md"),
        Path(".ai/reference/templates/command-handler.cs.txt"),
        Path(".ai/checklists/pre-submission.md"),
        Path(".ai/tests/validate-skills.py"),
        Path(".ai/scripts/upgrade-template.py"),
        Path("README.md"),
    ]

    passed = True
    for p in should_be_owned:
        if not fn(p):
            print(f"  FAIL: {p} should be project-owned but is_project_owned() returned False")
            passed = False

    for p in should_not_be_owned:
        if fn(p):
            print(f"  FAIL: {p} should NOT be project-owned but is_project_owned() returned True")
            passed = False

    if passed:
        print(f"  PASS: all {len(should_be_owned) + len(should_not_be_owned)} path classifications are correct")
    return passed


def test_collect_template_files() -> bool:
    """Test that collect_template_files() finds key files from this template."""
    print("\nTEST 6: collect_template_files() finds expected files")
    print("-" * 40)

    mod = load_upgrade_module()
    files = {str(f).replace("\\", "/") for f in mod.collect_template_files(TEMPLATE_ROOT)}

    expected_contains = [
        ".ai/skills/dotnet-engineer/SKILL.md",
        ".ai/skills/refactor/SKILL.md",
        ".ai/skills/design-interrogation/SKILL.md",
        ".ai/patterns/cqrs-patterns.md",
        ".ai/reference/critical-rules.md",
        ".ai/checklists/pre-submission.md",
        ".ai/tests/validate-skills.py",
        ".ai/scripts/upgrade-template.py",
    ]

    passed = True
    for expected in expected_contains:
        if expected not in files:
            print(f"  FAIL: expected file not found: {expected}")
            passed = False

    if passed:
        print(f"  PASS: all {len(expected_contains)} expected files found ({len(files)} total)")
    return passed


def test_dry_run_writes_nothing() -> bool:
    """Integration: dry-run should produce no file changes in target."""
    print("\nTEST 7: --dry-run writes no files to target")
    print("-" * 40)

    with tempfile.TemporaryDirectory() as tmp:
        target = Path(tmp)
        # Put one project-owned file that must not be touched
        claude_md = target / "CLAUDE.md"
        claude_md.write_text("# Project CLAUDE.md\nMy custom content\n", encoding="utf-8")

        import subprocess
        result = subprocess.run(
            [sys.executable, str(UPGRADE_SCRIPT),
             "--source", str(TEMPLATE_ROOT),
             "--target", str(target),
             "--dry-run", "--non-interactive"],
            capture_output=True, text=True,
        )

        # After dry-run, only CLAUDE.md should exist (we put it there)
        all_files = list(target.rglob("*"))
        written = [f for f in all_files if f.is_file() and f != claude_md]

        if written:
            print(f"  FAIL: dry-run wrote {len(written)} file(s):")
            for f in written[:5]:
                print(f"    {f.relative_to(target)}")
            return False

        # CLAUDE.md must be unchanged
        content_after = claude_md.read_text(encoding="utf-8")
        if content_after != "# Project CLAUDE.md\nMy custom content\n":
            print("  FAIL: CLAUDE.md was modified during dry-run")
            return False

        print(f"  PASS: dry-run wrote zero files, CLAUDE.md untouched")
        return True


def test_new_skill_gets_copied() -> bool:
    """Integration: a new skill file in source gets copied to target."""
    print("\nTEST 8: new skill in source is copied to target (non-interactive)")
    print("-" * 40)

    with tempfile.TemporaryDirectory() as source_tmp, \
         tempfile.TemporaryDirectory() as target_tmp:

        source = Path(source_tmp)
        target = Path(target_tmp)

        # Create minimal source skill
        skill_dir = source / ".ai" / "skills" / "test-skill"
        skill_dir.mkdir(parents=True)
        (skill_dir / "SKILL.md").write_text(
            "---\nname: test-skill\ndescription: A test skill for validation.\n---\n# Test\nContent.\n",
            encoding="utf-8",
        )

        import subprocess
        result = subprocess.run(
            [sys.executable, str(UPGRADE_SCRIPT),
             "--source", str(source),
             "--target", str(target),
             "--non-interactive"],
            capture_output=True, text=True,
        )

        copied = target / ".ai" / "skills" / "test-skill" / "SKILL.md"
        if not copied.exists():
            print(f"  FAIL: skill was not copied to target")
            print(f"  stdout: {result.stdout[:300]}")
            return False

        content = copied.read_text(encoding="utf-8")
        if "test-skill" not in content:
            print("  FAIL: copied file content is wrong")
            return False

        print("  PASS: new skill file was copied correctly")
        return True


def test_project_owned_never_copied() -> bool:
    """Integration: project-owned files in source are never copied to target."""
    print("\nTEST 9: project-owned files in source are never copied")
    print("-" * 40)

    with tempfile.TemporaryDirectory() as source_tmp, \
         tempfile.TemporaryDirectory() as target_tmp:

        source = Path(source_tmp)
        target = Path(target_tmp)

        # Place project-owned files in source
        (source / "CLAUDE.md").write_text("source CLAUDE.md", encoding="utf-8")
        (source / ".ai").mkdir()
        (source / ".ai" / "session-context.md").write_text("source session context", encoding="utf-8")
        project_dir = source / ".ai" / "project"
        project_dir.mkdir()
        (project_dir / "tech-stack.md").write_text("source tech stack", encoding="utf-8")

        import subprocess
        subprocess.run(
            [sys.executable, str(UPGRADE_SCRIPT),
             "--source", str(source),
             "--target", str(target),
             "--non-interactive"],
            capture_output=True, text=True,
        )

        forbidden = [
            target / "CLAUDE.md",
            target / ".ai" / "session-context.md",
            target / ".ai" / "project" / "tech-stack.md",
        ]

        copied_forbidden = [f for f in forbidden if f.exists()]
        if copied_forbidden:
            print(f"  FAIL: {len(copied_forbidden)} project-owned file(s) were copied:")
            for f in copied_forbidden:
                print(f"    {f.relative_to(target)}")
            return False

        print("  PASS: no project-owned files were copied")
        return True


def test_design_interrogation_pipeline() -> bool:
    """Verify that all skills referenced by design-interrogation exist."""
    print("\nTEST 10: design-interrogation pipeline skills all exist")
    print("-" * 40)

    pipeline_skills = [
        "ubiquitous-language",
        "usecase-specification",
        "user-story",
        "solution-generator",
        "vertical-slices",
        "gap-review",
    ]

    skills_dir = TEMPLATE_ROOT / ".ai" / "skills"
    passed = True
    for skill in pipeline_skills:
        skill_path = skills_dir / skill / "SKILL.md"
        if skill_path.exists():
            print(f"  PASS: {skill}")
        else:
            print(f"  FAIL: {skill} — SKILL.md not found at {skill_path}")
            passed = False

    if passed:
        print(f"\n  PASS: all {len(pipeline_skills)} pipeline skills exist")
    return passed


def test_three_tier_completeness() -> bool:
    """All .ai/skills/ have matching entries in .claude/skills/ and .github/skills/."""
    print("\nTEST 11: three-tier completeness (.ai → .claude → .github)")
    print("-" * 40)

    ai_skills     = TEMPLATE_ROOT / ".ai"     / "skills"
    claude_skills = TEMPLATE_ROOT / ".claude" / "skills"
    github_skills = TEMPLATE_ROOT / ".github" / "skills"

    ai_names = {d.name for d in ai_skills.iterdir() if d.is_dir()} if ai_skills.exists() else set()

    passed = True
    missing_claude = []
    missing_github = []

    for name in sorted(ai_names):
        if not (claude_skills / name / "SKILL.md").exists():
            missing_claude.append(name)
        if not (github_skills / name / "SKILL.md").exists():
            missing_github.append(name)

    if missing_claude:
        print(f"  FAIL: {len(missing_claude)} skill(s) missing from .claude/skills/:")
        for n in missing_claude:
            print(f"    {n}")
        passed = False

    if missing_github:
        print(f"  FAIL: {len(missing_github)} skill(s) missing from .github/skills/:")
        for n in missing_github:
            print(f"    {n}")
        passed = False

    if passed:
        print(f"  PASS: all {len(ai_names)} skills present in all three tiers")
    return passed


# ─── Main ────────────────────────────────────────────────────────────────────

def main() -> int:
    print("=" * 60)
    print("  Upgrade Script Validation")
    print("=" * 60)
    print()

    if not UPGRADE_SCRIPT.exists():
        print(f"FATAL: upgrade-template.py not found at {UPGRADE_SCRIPT}")
        return 1

    tests = [
        test_script_exists,
        test_script_importable,
        test_project_owned_completeness,
        test_template_owned_completeness,
        test_is_project_owned,
        test_collect_template_files,
        test_dry_run_writes_nothing,
        test_new_skill_gets_copied,
        test_project_owned_never_copied,
        test_design_interrogation_pipeline,
        test_three_tier_completeness,
    ]

    results = []
    for test_fn in tests:
        try:
            results.append(test_fn())
        except Exception as e:
            print(f"  ERROR: {e}")
            results.append(False)

    passed = sum(results)
    total  = len(results)

    print()
    print("=" * 60)
    print(f"  {passed}/{total} tests passed")
    print("=" * 60)

    if all(results):
        print("  ALL UPGRADE SCRIPT TESTS PASSED")
        return 0

    print("  SOME UPGRADE SCRIPT TESTS FAILED")
    return 1


if __name__ == "__main__":
    sys.exit(main())

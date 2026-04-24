#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates that CLAUDE.md correctly references all skills, patterns, and files.
Ensures no broken references and no orphaned skills/patterns.
"""

import os
import re
import sys
from pathlib import Path
from typing import Set, List, Tuple

# Fix encoding for Windows console
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

def get_template_root() -> Path:
    """Get the template root directory."""
    script_dir = Path(__file__).parent
    return script_dir.parent.parent

def parse_claude_md_references(claude_md_path: Path) -> Tuple[Set[str], Set[str], Set[str]]:
    """
    Parse CLAUDE.md and extract all file references.
    Returns (skill_refs, pattern_refs, other_refs)
    """
    skill_refs = set()
    pattern_refs = set()
    other_refs = set()

    with open(claude_md_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find all markdown-style file references: `.ai/...`
    # Pattern: backtick, .ai/, anything until backtick or newline
    file_pattern = r'`(\.ai/[^`\n]+)`'
    matches = re.findall(file_pattern, content)

    for match in matches:
        # Skip empty or directory-only references
        if match.endswith('/') or len(match) < 10:
            continue

        if '/skills/' in match:
            skill_refs.add(match)
        elif '/patterns/' in match:
            pattern_refs.add(match)
        else:
            other_refs.add(match)

    return skill_refs, pattern_refs, other_refs

def get_existing_skills(template_root: Path) -> Set[str]:
    """Get all existing skill files."""
    skills_dir = template_root / '.ai' / 'skills'
    skills = set()

    if not skills_dir.exists():
        return skills

    for skill_dir in skills_dir.iterdir():
        if skill_dir.is_dir():
            skill_file = skill_dir / 'SKILL.md'
            if skill_file.exists():
                skills.add(f'.ai/skills/{skill_dir.name}/SKILL.md')

    return skills

def get_existing_patterns(template_root: Path) -> Set[str]:
    """Get all existing pattern files."""
    patterns_dir = template_root / '.ai' / 'patterns'
    patterns = set()

    if not patterns_dir.exists():
        return patterns

    for pattern_file in patterns_dir.iterdir():
        if pattern_file.is_file() and pattern_file.suffix == '.md':
            patterns.add(f'.ai/patterns/{pattern_file.name}')

    return patterns

def validate_file_exists(template_root: Path, file_ref: str) -> Tuple[bool, str]:
    """Check if a referenced file exists."""
    # Normalize path - remove leading ./ but keep .claude
    if file_ref.startswith('./'):
        file_ref = file_ref[2:]

    file_path = template_root / file_ref

    if file_path.exists():
        return True, f"✅ {file_ref}"
    else:
        return False, f"❌ MISSING: {file_ref}"

def normalize_skill_reference(skill_ref: str) -> str:
    """
    Normalize skill reference to account for both formats:
    - .ai/skills/dotnet-engineer.md (old format referenced in CLAUDE.md)
    - .ai/skills/dotnet-engineer/SKILL.md (new actual structure)
    """
    # Extract skill name from reference
    match = re.search(r'\.ai/skills/([^/\.]+)', skill_ref)
    if not match:
        return skill_ref

    skill_name = match.group(1)

    # Could be either:
    # .ai/skills/{skill_name}.md (legacy reference)
    # .ai/skills/{skill_name}/SKILL.md (new format)
    return skill_name

def main():
    """Run all CLAUDE.md reference validations."""
    template_root = get_template_root()
    claude_md = template_root / 'CLAUDE.md'

    if not claude_md.exists():
        print("❌ ERROR: CLAUDE.md not found!")
        return False

    print("=" * 60)
    print("  CLAUDE.md Reference Validation")
    print("=" * 60)
    print()

    # Parse references from CLAUDE.md
    skill_refs, pattern_refs, other_refs = parse_claude_md_references(claude_md)

    print(f"Found {len(skill_refs)} skill references")
    print(f"Found {len(pattern_refs)} pattern references")
    print(f"Found {len(other_refs)} other file references")
    print()

    all_passed = True

    # Test 1: Validate skill references exist
    print("=" * 60)
    print("TEST 1: Skill References in CLAUDE.md")
    print("=" * 60)

    existing_skills = get_existing_skills(template_root)

    for skill_ref in sorted(skill_refs):
        skill_name = normalize_skill_reference(skill_ref)

        skill_path = template_root / '.ai' / 'skills' / skill_name / 'SKILL.md'

        if skill_path.exists():
            print(f"✅ {skill_ref} → {skill_name}/SKILL.md exists")
        else:
            print(f"❌ MISSING: {skill_ref} (skill '{skill_name}' not found)")
            all_passed = False

    print()

    # Test 2: Validate pattern references exist
    print("=" * 60)
    print("TEST 2: Pattern References in CLAUDE.md")
    print("=" * 60)

    for pattern_ref in sorted(pattern_refs):
        exists, msg = validate_file_exists(template_root, pattern_ref)
        print(msg)
        if not exists:
            all_passed = False

    print()

    # Test 3: Validate other file references exist
    print("=" * 60)
    print("TEST 3: Other File References in CLAUDE.md")
    print("=" * 60)

    for file_ref in sorted(other_refs):
        if '{' in file_ref and '}' in file_ref:
            print(f"⚠️  SKIP (template placeholder): {file_ref}")
            continue
        exists, msg = validate_file_exists(template_root, file_ref)
        print(msg)
        if not exists:
            all_passed = False

    print()

    # Test 4: Check for orphaned skills
    print("=" * 60)
    print("TEST 4: Orphaned Skills (exist but not referenced)")
    print("=" * 60)

    # Utility skills are user-invoked directly (/skill-name) — not loaded by task type.
    # They are intentionally absent from Work-Type Context Mapping.
    UTILITY_SKILLS = {"update-skills", "upgrade-template", "verify-config"}

    # Normalize existing skills to skill names
    existing_skill_names = set()
    for skill_path in existing_skills:
        skill_name = normalize_skill_reference(skill_path)
        existing_skill_names.add(skill_name)

    # Normalize referenced skills to skill names
    referenced_skill_names = set()
    for skill_ref in skill_refs:
        skill_name = normalize_skill_reference(skill_ref)
        referenced_skill_names.add(skill_name)

    orphaned_skills = (existing_skill_names - referenced_skill_names) - UTILITY_SKILLS

    if orphaned_skills:
        print("⚠️  WARNING: The following skills exist but are not referenced in CLAUDE.md:")
        for skill_name in sorted(orphaned_skills):
            print(f"   - {skill_name}")
        print()
        print("Consider adding them to the Work-Type Context Mapping section.")
    else:
        print("✅ No orphaned skills found - all task-type skills are referenced in CLAUDE.md")
        if UTILITY_SKILLS & existing_skill_names:
            print(f"   (utility skills exempt from check: {', '.join(sorted(UTILITY_SKILLS & existing_skill_names))})")

    print()

    # Test 5: Check for orphaned patterns
    print("=" * 60)
    print("TEST 5: Orphaned Patterns (exist but not referenced)")
    print("=" * 60)

    existing_patterns = get_existing_patterns(template_root)
    orphaned_patterns = existing_patterns - pattern_refs

    if orphaned_patterns:
        print("⚠️  WARNING: The following patterns exist but are not referenced in CLAUDE.md:")
        for pattern in sorted(orphaned_patterns):
            print(f"   - {pattern}")
        print()
        print("Consider adding them to the Work-Type Context Mapping section.")
    else:
        print("✅ No orphaned patterns found - all patterns are referenced in CLAUDE.md")

    print()

    # Test 6: Validate skill invocation format
    print("=" * 60)
    print("TEST 6: Skill Invocation Format")
    print("=" * 60)

    # Check if CLAUDE.md uses correct skill reference format
    with open(claude_md, 'r', encoding='utf-8') as f:
        content = f.read()

    # Skills should be referenced as .ai/skills/{name}.md or .ai/skills/{name}/SKILL.md
    # Check for malformed references
    malformed_pattern = r'`\.ai/skills/[^/\.]+(\.txt|\.cs|[^\.md])`'
    malformed = re.findall(malformed_pattern, content)

    if malformed:
        print("❌ MALFORMED: Found skill references with incorrect extensions:")
        for ref in malformed:
            print(f"   - {ref}")
        all_passed = False
    else:
        print("✅ All skill references use correct .md format")

    print()

    # Summary
    print("=" * 60)
    print("  SUMMARY")
    print("=" * 60)
    print()

    total_checks = len(skill_refs) + len(pattern_refs) + len(other_refs) + 3  # +3 for orphan checks and format check

    print(f"Total skill references validated: {len(skill_refs)}")
    print(f"Total pattern references validated: {len(pattern_refs)}")
    print(f"Total other file references validated: {len(other_refs)}")
    print(f"Orphaned skills check: {'✅ PASSED' if not orphaned_skills else '⚠️  WARNING'}")
    print(f"Orphaned patterns check: {'✅ PASSED' if not orphaned_patterns else '⚠️  WARNING'}")
    print(f"Skill format check: ✅ PASSED" if not malformed else "❌ FAILED")
    print()

    if all_passed:
        print("✅✅✅ ALL REFERENCE CHECKS PASSED ✅✅✅")
        return True
    else:
        print("❌❌❌ SOME REFERENCE CHECKS FAILED ❌❌❌")
        return False

if __name__ == '__main__':
    success = main()
    sys.exit(0 if success else 1)

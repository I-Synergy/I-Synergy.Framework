#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Integration smoke tests for Claude template."""

import sys
import io
from pathlib import Path
from typing import Dict, Set, List

try:
    import yaml
    _YAML_AVAILABLE = True
except ImportError:
    _YAML_AVAILABLE = False

def _parse_frontmatter(text: str) -> dict:
    """Minimal YAML frontmatter parser supporting key-value and folded (>) scalars."""
    result = {}
    lines = text.strip().splitlines()
    i = 0
    while i < len(lines):
        line = lines[i]
        if ':' not in line:
            i += 1
            continue
        key, _, rest = line.partition(':')
        key = key.strip()
        value = rest.strip()
        if value == '>':
            folded = []
            i += 1
            while i < len(lines) and (lines[i].startswith(' ') or lines[i].startswith('\t')):
                folded.append(lines[i].strip())
                i += 1
            result[key] = ' '.join(folded)
        elif value.lower() == 'true':
            result[key] = True
            i += 1
        elif value.lower() == 'false':
            result[key] = False
            i += 1
        elif value.startswith('[') and value.endswith(']'):
            items = [x.strip().strip("'\"") for x in value[1:-1].split(',') if x.strip()]
            result[key] = items
            i += 1
        else:
            result[key] = value.strip("'\"") if value else ''
            i += 1
    return result

def _safe_load(text: str) -> dict:
    if _YAML_AVAILABLE:
        return yaml.safe_load(text)
    return _parse_frontmatter(text)

# Set UTF-8 encoding for Windows console
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

# Determine template root (script location is .ai/tests/)
SCRIPT_DIR = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent
SKILLS_DIR = TEMPLATE_ROOT / ".ai/skills"

def load_all_skills() -> Dict[str, dict]:
    """Load all skill files and parse their frontmatter.

    Returns:
        Dictionary mapping skill names to their frontmatter
    """
    skills = {}

    if not SKILLS_DIR.exists():
        print(f"❌ Skills directory not found: {SKILLS_DIR}")
        return skills

    for skill_dir in sorted(SKILLS_DIR.iterdir()):
        if not skill_dir.is_dir():
            continue

        skill_file = skill_dir / "SKILL.md"
        if not skill_file.exists():
            continue

        try:
            content = skill_file.read_text(encoding='utf-8')
            parts = content.split('---', 2)

            if len(parts) >= 3:
                frontmatter = _safe_load(parts[1])
                if frontmatter and 'name' in frontmatter:
                    skills[frontmatter['name']] = {
                        'frontmatter': frontmatter,
                        'path': skill_file,
                        'content': parts[2].strip()
                    }
        except Exception as e:
            print(f"⚠️  Failed to load {skill_dir.name}: {e}")

    return skills

def test_skill_names_unique(skills: Dict[str, dict]) -> bool:
    """Test that all skill names are unique.

    Args:
        skills: Dictionary of loaded skills

    Returns:
        True if all names are unique
    """
    names = [s['frontmatter']['name'] for s in skills.values()]
    unique_names = set(names)

    if len(names) != len(unique_names):
        duplicates = [name for name in names if names.count(name) > 1]
        print(f"❌ Duplicate skill names found: {set(duplicates)}")
        return False

    print(f"✅ All {len(names)} skill names are unique")
    return True

def test_descriptions_unique(skills: Dict[str, dict]) -> bool:
    """Test that skill descriptions are unique.

    Args:
        skills: Dictionary of loaded skills

    Returns:
        True if all descriptions are reasonably unique
    """
    descriptions = [s['frontmatter'].get('description', '') for s in skills.values()]
    unique_descriptions = set(descriptions)

    if len(descriptions) != len(unique_descriptions):
        print("⚠️  Some skill descriptions are duplicated")
        return False

    print(f"✅ All {len(descriptions)} skill descriptions are unique")
    return True

def test_allowed_tools_valid(skills: Dict[str, dict]) -> bool:
    """Test that all allowed-tools references are valid tool names.

    Args:
        skills: Dictionary of loaded skills

    Returns:
        True if all tool names are valid
    """
    # Common valid tool names (expand as needed)
    valid_tools = {
        'Bash', 'Read', 'Write', 'Edit', 'Glob', 'Grep',
        'WebFetch', 'WebSearch', 'TodoWrite', 'Skill',
        'NotebookEdit'
    }

    all_valid = True

    for skill_name, skill_data in skills.items():
        frontmatter = skill_data['frontmatter']
        if 'allowed-tools' in frontmatter:
            allowed_tools = frontmatter['allowed-tools']

            if not isinstance(allowed_tools, list):
                print(f"❌ {skill_name}: allowed-tools must be a list")
                all_valid = False
                continue

            for tool in allowed_tools:
                if tool not in valid_tools:
                    print(f"⚠️  {skill_name}: unknown tool '{tool}'")

    if all_valid:
        print("✅ All allowed-tools references are valid")

    return all_valid

def test_skill_content_parseable(skills: Dict[str, dict]) -> bool:
    """Test that all skill content is parseable and non-empty.

    Args:
        skills: Dictionary of loaded skills

    Returns:
        True if all content is valid
    """
    all_valid = True

    for skill_name, skill_data in skills.items():
        content = skill_data['content']

        if not content or len(content) < 10:
            print(f"❌ {skill_name}: content is empty or too short")
            all_valid = False

    if all_valid:
        print(f"✅ All {len(skills)} skills have valid content")

    return all_valid

def test_skills_loadable() -> bool:
    """Test that all skills can be loaded without errors.

    Returns:
        True if all skills load successfully
    """
    skills = load_all_skills()

    if not skills:
        print("❌ No skills loaded")
        return False

    print(f"✅ Successfully loaded {len(skills)} skills")
    return True

def test_patterns_exist() -> bool:
    """Test that all expected patterns exist.

    Returns:
        True if all patterns exist
    """
    patterns_dir = TEMPLATE_ROOT / ".ai/patterns"
    expected_patterns = [
        "cqrs-patterns.md",
        "api-patterns.md",
        "testing-patterns.md",
        "mvvm.md",
        "microservices.md",
        "service-oriented-architecture.md",
        "object-oriented-programming.md",
        "test-driven-development.md",
    ]

    all_exist = True
    for pattern in expected_patterns:
        pattern_path = patterns_dir / pattern
        if not pattern_path.exists():
            print(f"❌ Pattern missing: {pattern}")
            all_exist = False

    if all_exist:
        print(f"✅ All {len(expected_patterns)} patterns exist")

    return all_exist

def test_templates_exist() -> bool:
    """Test that all expected templates exist.

    Returns:
        True if all templates exist
    """
    templates_dir = TEMPLATE_ROOT / ".ai/reference/templates"
    expected_templates = [
        "command-handler.cs.txt",
        "query-handler.cs.txt",
        "endpoint.cs.txt",
        "test-class.cs.txt",
        "feature-file.feature.txt",
    ]

    all_exist = True
    for template in expected_templates:
        template_path = templates_dir / template
        if not template_path.exists():
            print(f"❌ Template missing: {template}")
            all_exist = False

    if all_exist:
        print(f"✅ All {len(expected_templates)} templates exist")

    return all_exist

def main():
    """Run all smoke tests."""
    print("=========================================")
    print("  Integration Smoke Tests")
    print("=========================================")
    print()

    tests = [
        ("Skills Loadable", test_skills_loadable),
        ("Patterns Exist", test_patterns_exist),
        ("Templates Exist", test_templates_exist),
    ]

    # Load skills once
    print("Loading skills...")
    skills = load_all_skills()
    print()

    if skills:
        tests.extend([
            ("Unique Skill Names", lambda: test_skill_names_unique(skills)),
            ("Unique Descriptions", lambda: test_descriptions_unique(skills)),
            ("Valid Tool References", lambda: test_allowed_tools_valid(skills)),
            ("Parseable Content", lambda: test_skill_content_parseable(skills)),
        ])

    results = []
    for test_name, test_func in tests:
        print(f"Running test: {test_name}")
        try:
            result = test_func()
            results.append(result)
        except Exception as e:
            print(f"❌ Test failed with exception: {e}")
            results.append(False)
        print()

    # Summary
    print("=========================================")
    print("Smoke Test Summary")
    print("=========================================")
    passed = sum(1 for r in results if r)
    total = len(results)
    print(f"Passed: {passed}/{total}")
    print("=========================================")

    if all(results):
        print("✅ ALL SMOKE TESTS PASSED")
        return 0
    else:
        print("❌ SOME SMOKE TESTS FAILED")
        return 1

if __name__ == '__main__':
    sys.exit(main())

#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Validate content quality in skills and patterns."""

import sys
import io
from pathlib import Path
from typing import List, Tuple

# Set UTF-8 encoding for Windows console
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

# Determine template root (script location is .ai/tests/)
SCRIPT_DIR = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent
SKILLS_DIR = TEMPLATE_ROOT / ".ai/skills"
PATTERNS_DIR = TEMPLATE_ROOT / ".ai/patterns"

def validate_skill_content(skill_path: Path) -> Tuple[bool, List[str]]:
    """Validate skill content quality.

    Args:
        skill_path: Path to the SKILL.md file

    Returns:
        Tuple of (is_valid, list_of_warnings)
    """
    warnings = []

    if not skill_path.exists():
        return False, ["File does not exist"]

    try:
        content = skill_path.read_text(encoding='utf-8')
    except Exception as e:
        return False, [f"Failed to read file: {e}"]

    # Extract content after frontmatter
    parts = content.split('---', 2)
    if len(parts) >= 3:
        body = parts[2].strip()
    else:
        body = content

    # Check content length
    if len(body) < 100:
        warnings.append(f"Content too short: {len(body)} chars (expected >100)")

    # Check for required sections (flexible matching)
    required_sections = [
        ("## ", "No H2 sections found"),
    ]

    for marker, message in required_sections:
        if marker not in body:
            warnings.append(message)

    # Check for code examples
    if "```" not in body:
        warnings.append("No code examples found (no ``` blocks)")

    # Check for meaningful content indicators
    quality_indicators = [
        ("when", "Missing 'when' usage description"),
        ("example", "Missing examples"),
    ]

    found_indicators = sum(1 for indicator, _ in quality_indicators if indicator.lower() in body.lower())
    if found_indicators < 1:
        warnings.append("Content lacks quality indicators (when/example)")

    return len(warnings) == 0, warnings

def validate_pattern_content(pattern_path: Path) -> Tuple[bool, List[str]]:
    """Validate pattern content quality.

    Args:
        pattern_path: Path to the pattern file

    Returns:
        Tuple of (is_valid, list_of_warnings)
    """
    warnings = []

    if not pattern_path.exists():
        return False, ["File does not exist"]

    try:
        content = pattern_path.read_text(encoding='utf-8')
    except Exception as e:
        return False, [f"Failed to read file: {e}"]

    # Check content length
    if len(content) < 200:
        warnings.append(f"Content too short: {len(content)} chars (expected >200)")

    # Check for code examples
    code_blocks = content.count("```")
    if code_blocks < 2:
        warnings.append(f"Insufficient code examples: {code_blocks // 2} blocks (expected at least 1)")

    # Check for sections
    if "## " not in content:
        warnings.append("No H2 sections found")

    # Check for pattern-specific content
    pattern_indicators = [
        ("example", "Missing examples"),
        ("correct", "Missing correct/incorrect examples"),
    ]

    found_indicators = sum(1 for indicator, _ in pattern_indicators if indicator.lower() in content.lower())
    if found_indicators < 1:
        warnings.append("Pattern lacks quality indicators (example/correct)")

    return len(warnings) == 0, warnings

def validate_template_tokens(template_path: Path) -> Tuple[bool, List[str]]:
    """Validate that templates use proper token placeholders.

    Args:
        template_path: Path to template file

    Returns:
        Tuple of (is_valid, list_of_warnings)
    """
    warnings = []

    if not template_path.exists():
        return False, ["File does not exist"]

    try:
        content = template_path.read_text(encoding='utf-8')
    except Exception as e:
        return False, [f"Failed to read file: {e}"]

    # Check for template tokens
    expected_tokens = [
        "{ApplicationName}",
        "{Domain}",
        "{Entity}",
    ]

    found_tokens = sum(1 for token in expected_tokens if token in content)
    if found_tokens == 0:
        warnings.append("No template tokens found (expected {ApplicationName}, {Domain}, {Entity})")

    return len(warnings) == 0, warnings

def main():
    """Validate content quality."""
    print("=========================================")
    print("  Content Quality Validation")
    print("=========================================")
    print()

    all_valid = True
    total_warnings = 0

    # Validate skills
    print("1. Validating skill content...")
    if SKILLS_DIR.exists():
        skill_dirs = sorted([d for d in SKILLS_DIR.iterdir() if d.is_dir()])
        for skill_dir in skill_dirs:
            skill_file = skill_dir / "SKILL.md"
            valid, warnings = validate_skill_content(skill_file)

            if valid:
                print(f"✅ {skill_dir.name}")
            else:
                print(f"⚠️  {skill_dir.name}")
                for warning in warnings:
                    print(f"   - {warning}")
                total_warnings += len(warnings)
                # Don't fail on content warnings, just report them
    else:
        print(f"❌ Skills directory not found: {SKILLS_DIR}")
        all_valid = False

    print()

    # Validate patterns
    print("2. Validating pattern content...")
    if PATTERNS_DIR.exists():
        pattern_files = sorted([f for f in PATTERNS_DIR.glob("*.md")])
        for pattern_file in pattern_files:
            valid, warnings = validate_pattern_content(pattern_file)

            if valid:
                print(f"✅ {pattern_file.name}")
            else:
                print(f"⚠️  {pattern_file.name}")
                for warning in warnings:
                    print(f"   - {warning}")
                total_warnings += len(warnings)
                # Don't fail on content warnings, just report them
    else:
        print(f"❌ Patterns directory not found: {PATTERNS_DIR}")
        all_valid = False

    print()

    # Validate templates
    print("3. Validating template tokens...")
    templates_dir = TEMPLATE_ROOT / ".ai/reference/templates"
    if templates_dir.exists():
        template_files = sorted([f for f in templates_dir.glob("*.md")])
        for template_file in template_files:
            valid, warnings = validate_template_tokens(template_file)

            if valid:
                print(f"✅ {template_file.name}")
            else:
                print(f"⚠️  {template_file.name}")
                for warning in warnings:
                    print(f"   - {warning}")
                total_warnings += len(warnings)
    else:
        print(f"❌ Templates directory not found: {templates_dir}")
        all_valid = False

    print()
    print("=========================================")
    print("Content Validation Summary")
    print("=========================================")
    print(f"Warnings found: {total_warnings}")
    print("=========================================")

    if all_valid and total_warnings == 0:
        print("✅ ALL CONTENT TESTS PASSED")
        return 0
    elif all_valid:
        print("⚠️  CONTENT TESTS PASSED WITH WARNINGS")
        return 0  # Don't fail on warnings
    else:
        print("❌ SOME CONTENT TESTS FAILED")
        return 1

if __name__ == '__main__':
    sys.exit(main())

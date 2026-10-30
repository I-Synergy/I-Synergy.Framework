#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Validate that every skill and agent frontmatter description carries an explicit trigger clause.

The frontmatter `description` is the primary trigger mechanism a client uses to decide
whether to load a skill or delegate to an agent, so it must state *when* to invoke it.
A file passes when the combined text of its `description` plus (for skills) `when_to_use`
matches, case-insensitively, one of the recognised trigger phrases.
"""

import re
import sys
from pathlib import Path
from typing import List, Tuple

try:
    import yaml
    _YAML_AVAILABLE = True
except ImportError:
    _YAML_AVAILABLE = False

# Set UTF-8 encoding for Windows console
if sys.platform == 'win32':
    import io
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

# Determine template root (script location is .ai/tests/)
SCRIPT_DIR = Path(__file__).parent
TEMPLATE_ROOT = SCRIPT_DIR.parent.parent
SKILLS_DIR = TEMPLATE_ROOT / ".ai/skills"
AGENTS_DIR = TEMPLATE_ROOT / ".ai/agents"

# Recognised trigger phrases. Matched on whole-word boundaries so that, for example,
# "Use whenever" does not silently count as "use when".
TRIGGER_PHRASES = (
    "use when",
    "use for",
    "use before",
    "use after",
    "trigger when",
    "trigger whenever",
    "trigger on",
    "call this when",
    "invoke when",
    "when to use",
)

_TRIGGER_RE = re.compile(
    r"\b(?:" + "|".join(re.escape(p) for p in TRIGGER_PHRASES) + r")\b",
    re.IGNORECASE,
)


def _split_frontmatter(text: str) -> Tuple[str, str]:
    """Return (frontmatter_block, body). frontmatter_block is empty when absent."""
    if not text.startswith('---'):
        return "", text
    parts = text.split('---', 2)
    if len(parts) < 3:
        return "", text
    return parts[1], parts[2]


def _safe_load(text: str) -> dict:
    if _YAML_AVAILABLE:
        return yaml.safe_load(text)
    return {}


def _has_trigger(text: str) -> bool:
    return bool(_TRIGGER_RE.search(text or ""))


def _validate_file(path: Path, *, is_skill: bool) -> Tuple[bool, str]:
    """Return (is_valid, reason). reason is empty when valid."""
    try:
        content = path.read_text(encoding='utf-8')
    except Exception as e:  # noqa: BLE001 - report and treat as failure
        return False, f"could not read file: {e}"

    frontmatter_block, _ = _split_frontmatter(content)
    if not frontmatter_block:
        return False, "missing YAML frontmatter"

    try:
        frontmatter = _safe_load(frontmatter_block)
    except Exception as e:  # noqa: BLE001 - malformed YAML is a failure
        return False, f"YAML parse error: {e}"

    if not isinstance(frontmatter, dict):
        return False, "empty or invalid YAML frontmatter"

    description = frontmatter.get('description')
    if not isinstance(description, str) or not description.strip():
        return False, "missing or empty 'description'"

    combined = description
    if is_skill:
        when_to_use = frontmatter.get('when_to_use')
        if isinstance(when_to_use, str):
            combined = description + "\n" + when_to_use

    if not _has_trigger(combined):
        return False, "no trigger clause in description"

    return True, ""


def main() -> int:
    print("=========================================")
    print("  Trigger Clause Validation")
    print("=========================================")
    print()

    failures: List[Tuple[str, str]] = []
    skills_checked = 0
    agents_checked = 0

    if SKILLS_DIR.exists():
        for skill_dir in sorted([d for d in SKILLS_DIR.iterdir() if d.is_dir()]):
            skill_file = skill_dir / "SKILL.md"
            if not skill_file.exists():
                continue
            skills_checked += 1
            valid, reason = _validate_file(skill_file, is_skill=True)
            rel = skill_file.relative_to(TEMPLATE_ROOT).as_posix()
            if valid:
                print(f"✅ {rel}")
            else:
                print(f"❌ {rel} — {reason}")
                failures.append((rel, reason))
    else:
        print(f"❌ Skills directory not found: {SKILLS_DIR}")
        failures.append((str(SKILLS_DIR), "skills directory not found"))

    if AGENTS_DIR.exists():
        for agent_file in sorted(AGENTS_DIR.glob("*.md")):
            agents_checked += 1
            valid, reason = _validate_file(agent_file, is_skill=False)
            rel = agent_file.relative_to(TEMPLATE_ROOT).as_posix()
            if valid:
                print(f"✅ {rel}")
            else:
                print(f"❌ {rel} — {reason}")
                failures.append((rel, reason))
    else:
        print(f"❌ Agents directory not found: {AGENTS_DIR}")
        failures.append((str(AGENTS_DIR), "agents directory not found"))

    print()
    print("=========================================")
    print(f"{skills_checked} skill(s) and {agents_checked} agent(s) checked, "
          f"{len(failures)} failure(s)")
    print("=========================================")

    if failures:
        print("❌ SOME TRIGGER CLAUSES MISSING")
        return 1
    print("✅ ALL DESCRIPTIONS CARRY A TRIGGER CLAUSE")
    return 0


if __name__ == '__main__':
    sys.exit(main())

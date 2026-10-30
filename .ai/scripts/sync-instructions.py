#!/usr/bin/env python3
"""Regenerate .github/copilot-instructions.md from AGENTS.md.

AGENTS.md is the single source of truth. The Copilot file is a copy of it plus
the Copilot-only quick-reference sections, which are kept in a marked block so
they survive regeneration.
"""

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
AGENTS = ROOT / "AGENTS.md"
COPILOT = ROOT / ".github" / "copilot-instructions.md"
GENERATED = "<!-- ai-workspace-manager:generated-from-agents -->"
START = "<!-- ai-workspace-manager:copilot-reference:start -->"
END = "<!-- ai-workspace-manager:copilot-reference:end -->"
NOTE = ("<!-- Generated from AGENTS.md. Edit AGENTS.md, not this file. "
        "Regenerate with `python .ai/scripts/sync-instructions.py`. -->")
HEADINGS = (
    "Project Context", "Architecture", "Forbidden Technologies",
    "Patterns & Templates", "Naming Conventions", "Token Replacements",
    "Core Operational Rules", "Work-Type Context Mapping",
)


def section(text: str, title: str) -> str:
    pattern = re.compile(rf"^## {re.escape(title)}\s*$.*?(?=^## |\Z)", re.M | re.S)
    match = pattern.search(text)
    return match.group(0).strip() if match else ""


def reference(text: str) -> str:
    if START in text and END in text:
        start = text.index(START) + len(START)
        return text[start:text.index(END)].strip()
    parts = [section(text, title) for title in HEADINGS]
    return "\n\n".join(part for part in parts if part)


def main() -> int:
    if not AGENTS.is_file():
        print("ERROR: AGENTS.md not found", file=sys.stderr)
        return 2
    agents = AGENTS.read_text(encoding="utf-8").strip()
    existing = COPILOT.read_text(encoding="utf-8") if COPILOT.is_file() else ""
    # Normalize markers written by earlier releases so the block is found.
    existing = existing.replace("<!-- ai-manager:", "<!-- ai-workspace-manager:")
    parts = ["# GitHub Copilot Instructions", "", GENERATED, NOTE, "", agents]
    block = reference(existing)
    if block:
        parts += ["", START, block, END]
    document = "\n".join(parts).rstrip() + "\n"
    if COPILOT.is_file() and COPILOT.read_text(encoding="utf-8") == document:
        print("copilot-instructions.md is up to date")
        return 0
    COPILOT.parent.mkdir(parents=True, exist_ok=True)
    COPILOT.write_text(document, encoding="utf-8", newline="\n")
    print("wrote .github/copilot-instructions.md")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

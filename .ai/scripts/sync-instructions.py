#!/usr/bin/env python3
"""Regenerate .github/copilot-instructions.md from AGENTS.md.

AGENTS.md is the single source of truth. The generated Copilot file is laid out as:

  1. header and generated marker
  2. AGENTS.md verbatim
  3. the Copilot runtime section, copied fresh from .ai/reference/copilot-runtime.md
     (template-owned) between runtime markers; never preserved from the old file
  4. the project-owned copilot-reference block, preserved across regeneration

If .ai/reference/copilot-runtime.md is missing, the runtime section is skipped with
a warning rather than failing the run.
"""

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
AGENTS = ROOT / "AGENTS.md"
COPILOT = ROOT / ".github" / "copilot-instructions.md"
RUNTIME = ROOT / ".ai" / "reference" / "copilot-runtime.md"
GENERATED = "<!-- ai-workspace-manager:generated-from-agents -->"
RUNTIME_START = "<!-- ai-workspace-manager:copilot-runtime:start -->"
RUNTIME_END = "<!-- ai-workspace-manager:copilot-runtime:end -->"
START = "<!-- ai-workspace-manager:copilot-reference:start -->"
END = "<!-- ai-workspace-manager:copilot-reference:end -->"
NOTE = ("<!-- Generated from AGENTS.md. Edit AGENTS.md, not this file. "
        "Regenerate with `python .ai/scripts/sync-instructions.py`. -->")
HEADINGS = (
    "Project Context", "Architecture", "Forbidden Technologies",
    "Patterns & Templates", "Naming Conventions", "Token Replacements",
    "Core Operational Rules", "Work-Type Context Mapping",
)

# AGENTS.md names the Claude adapter paths; the generated Copilot copy must point at the
# Copilot adapters instead. `.claude/settings.json` is a real Claude-only path and stays
# as written. Kept in step with catalog._copilot_paths, which generates the same file.
_CLAUDE_PATH = re.compile(r"\.claude/(?!settings\.json|settings\.local\.json)")


def copilot_paths(text: str) -> str:
    return _CLAUDE_PATH.sub(".github/", text)


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
    parts = ["# GitHub Copilot Instructions", "", GENERATED, NOTE, "", copilot_paths(agents)]
    block = reference(existing)
    if block:
        parts += ["", START, block, END]
    # The runtime section follows the reference block: the generated-body check compares
    # everything before the reference marker against AGENTS.md, so a runtime block placed
    # earlier would make every regeneration look stale.
    if RUNTIME.is_file():
        runtime = RUNTIME.read_text(encoding="utf-8").strip()
        parts += ["", RUNTIME_START, runtime, RUNTIME_END]
    else:
        print(f"WARNING: {RUNTIME.relative_to(ROOT).as_posix()} not found; "
              "Copilot runtime section skipped", file=sys.stderr)
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

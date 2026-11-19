#!/usr/bin/env python3
"""PostToolUse hook (Write/Edit): re-sync links when a skill, agent or instruction changes.

Accepts both payload shapes:
  Claude   tool_input.file_path / tool_input.path
  Copilot  toolArgs (JSON string or object), tool_input, toolInput

Claude narrows the hook with its `if:` field; this script filters by path itself,
because Copilot does not read `if:`. Paths outside .ai/skills/, .ai/agents/ and
.ai/instructions/ are ignored. This hook is not a gate: it always exits 0.
"""

import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import _common  # noqa: E402

WATCHED_PREFIXES = (".ai/skills/", ".ai/agents/", ".ai/instructions/")
PATH_KEYS = ("file_path", "filePath", "path", "filepath")


def _path_from(container) -> str:
    if isinstance(container, str):
        try:
            container = json.loads(container)
        except ValueError:
            # Not valid JSON (e.g. unescaped Windows backslashes): scan for the key.
            match = re.search(r'"(?:file_path|filePath|path|filepath)"\s*:\s*"([^"]+)"', container)
            return match.group(1) if match else ""
    if not isinstance(container, dict):
        return ""
    for key in PATH_KEYS:
        value = container.get(key)
        if isinstance(value, str) and value.strip():
            return value
    return ""


def extract_path(payload: dict) -> str:
    """Find the edited file path in either payload shape. Returns '' when absent."""
    for field in ("tool_input", "toolInput", "toolArgs"):
        found = _path_from(payload.get(field))
        if found:
            return found
    return ""


def normalize(path: str) -> str:
    """Return a repo-relative, forward-slash path, or '' when it is outside the repo."""
    text = path.strip().replace("\\", "/")
    candidate = Path(text)
    if candidate.is_absolute():
        try:
            text = candidate.resolve().relative_to(_common.ROOT).as_posix()
        except (ValueError, OSError):
            try:
                text = candidate.relative_to(_common.ROOT).as_posix()
            except ValueError:
                return ""
    while text.startswith("./"):
        text = text[2:]
    return text


def is_watched(relative: str) -> bool:
    lowered = relative.lower()
    return any(lowered.startswith(prefix) for prefix in WATCHED_PREFIXES)


def main() -> int:
    _common.configure_stdout()
    payload = _common.read_payload()
    relative = normalize(extract_path(payload))
    if not relative or not is_watched(relative):
        return 0

    try:
        code, _out, err = _common.run_sync()
        if code != 0:
            print(f"sync-platforms.py exited {code}: {err.strip()}", file=sys.stderr)
    except OSError as exc:
        print(f"sync-platforms.py could not run: {exc}", file=sys.stderr)
    return 0


if __name__ == "__main__":
    sys.exit(main())

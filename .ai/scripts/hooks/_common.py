"""Shared helpers for the .ai/ hook scripts.

The hook scripts are called by Claude Code (.claude/settings.json) and read the
Claude payload shape. Some of them also accept the Copilot payload shape, so
every helper here tolerates missing or malformed stdin and never raises.
"""

import json
import os
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]  # .ai/scripts/hooks/ -> repo root
SYNC_SCRIPT = ROOT / ".ai" / "scripts" / "sync-platforms.py"
PROBE_LOG = ROOT / ".ai" / "tmp" / "hook-probe.log"


def configure_stdout() -> None:
    """Print UTF-8 even on Windows consoles with a legacy code page."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")


def read_payload() -> dict:
    """Return the hook's stdin JSON as a dict. Empty, missing or invalid input gives {}."""
    try:
        if sys.stdin is None or sys.stdin.isatty():
            return {}
        raw = sys.stdin.read()
    except (OSError, ValueError):
        return {}
    if not raw or not raw.strip():
        return {}
    try:
        data = json.loads(raw)
    except ValueError:
        return {}
    return data if isinstance(data, dict) else {}


def harness() -> str:
    """'claude' when Claude Code launched the hook, otherwise 'other' (e.g. Copilot)."""
    return "claude" if os.environ.get("CLAUDE_PROJECT_DIR") else "other"


def event_name(payload: dict) -> str:
    return str(payload.get("hook_event_name") or payload.get("hookEventName") or "unknown")


def probe(payload: dict) -> None:
    """Append one line to .ai/tmp/hook-probe.log recording which harness fired the hook.

    Used to verify whether the Copilot harness runs .claude/settings.json hooks.
    Logs only the event name and top-level key names, never the payload values.
    """
    try:
        PROBE_LOG.parent.mkdir(parents=True, exist_ok=True)
        stamp = datetime.now(timezone.utc).isoformat(timespec="seconds")
        keys = sorted(str(k) for k in payload.keys())
        with PROBE_LOG.open("a", encoding="utf-8") as fh:
            fh.write(f"{stamp} harness={harness()} event={event_name(payload)} keys={keys}\n")
    except OSError:
        pass


def run_sync() -> tuple[int, str, str]:
    """Run sync-platforms.py in the project root. Returns (exit code, stdout, stderr)."""
    if not SYNC_SCRIPT.is_file():
        return 0, "", ""
    result = subprocess.run(
        [sys.executable, str(SYNC_SCRIPT)],
        cwd=str(ROOT),
        capture_output=True,
        text=True,
        errors="replace",
    )
    return result.returncode, result.stdout, result.stderr

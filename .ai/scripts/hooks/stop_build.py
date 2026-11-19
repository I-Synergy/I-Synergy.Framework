#!/usr/bin/env python3
"""Stop hook: build the solution when source changed since the last successful build.

Claude-only: exits 0 immediately unless CLAUDE_PROJECT_DIR is set.

Rules (same as the former inline bash hook):
  - no *.sln / *.slnx at the repo root -> nothing to build
  - build when the marker .ai/tmp/lastbuild-success is missing, or any file under
    src/ or tests/ is newer than the marker
  - on success, touch the marker; on failure, exit with the build's exit code so
    Claude Code surfaces the errors
"""

import os
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import _common  # noqa: E402

ROOT = _common.ROOT
MARKER = ROOT / ".ai" / "tmp" / "lastbuild-success"
WATCHED_DIRS = ("src", "tests")


def has_solution() -> bool:
    return any(ROOT.glob("*.sln")) or any(ROOT.glob("*.slnx"))


def needs_build() -> bool:
    if not MARKER.is_file():
        return True
    marker_time = MARKER.stat().st_mtime
    for name in WATCHED_DIRS:
        base = ROOT / name
        if not base.is_dir():
            continue
        for dirpath, _dirnames, filenames in os.walk(base):
            for filename in filenames:
                if os.stat(os.path.join(dirpath, filename)).st_mtime > marker_time:
                    return True
    return False


def main() -> int:
    if not os.environ.get("CLAUDE_PROJECT_DIR"):
        return 0
    if not has_solution() or not needs_build():
        return 0

    env = dict(os.environ, MSBUILDDISABLENODEREUSE="1")
    MARKER.parent.mkdir(parents=True, exist_ok=True)
    command = ["dotnet", "build", "--nologo", "--verbosity", "minimal", "-nodeReuse:false"]
    try:
        result = subprocess.run(command, cwd=str(ROOT), env=env)
    except OSError as exc:
        print(f"stop_build.py: could not run dotnet build: {exc}", file=sys.stderr)
        return 1

    if result.returncode != 0:
        return result.returncode
    MARKER.touch()
    return 0


if __name__ == "__main__":
    sys.exit(main())

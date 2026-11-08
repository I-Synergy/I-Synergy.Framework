#!/usr/bin/env python3
"""Deprecated alias for sync-platforms.py.

Older hooks and project instructions call `sync-skills.py`. Keeping this file
means they continue to work after the script was renamed; new callers should use
`sync-platforms.py`, which covers skills, agents, instructions and prompts.
"""

import subprocess
import sys
from pathlib import Path

if __name__ == "__main__":
    script = Path(__file__).with_name("sync-platforms.py")
    print("note: sync-skills.py is deprecated; use sync-platforms.py",
          file=sys.stderr)
    raise SystemExit(subprocess.call([sys.executable, str(script), *sys.argv[1:]]))

#!/usr/bin/env python3
"""
Ensure folder-level junctions exist for agent targets.

Delegates to sync-platforms.py, which handles skills, agents, instructions and
prompts for Claude Code, GitHub Copilot and Reasonix.

Usage:
    python sync-agents.py              # ensure junctions exist
    python sync-agents.py --dry-run    # show what would change without writing
"""

import subprocess
import sys
from pathlib import Path

if __name__ == "__main__":
    script = Path(__file__).with_name("sync-platforms.py")
    if not script.is_file():
        # A project that has not received the renamed script yet.
        script = Path(__file__).with_name("sync-skills.py")
    raise SystemExit(subprocess.call([sys.executable, str(script), *sys.argv[1:]]))

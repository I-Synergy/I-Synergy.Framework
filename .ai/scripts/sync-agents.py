#!/usr/bin/env python3
"""
Ensure folder-level junctions exist for agent targets.
Delegates to sync-skills.py which handles both skills/ and agents/ junctions.

Usage:
    python sync-agents.py              # ensure junctions exist
    python sync-agents.py --dry-run    # show what would change without writing
"""

import sys
from pathlib import Path

# Delegates to sync-skills.py
sys.path.insert(0, str(Path(__file__).parent))
from importlib import import_module

if __name__ == "__main__":
    sync_skills = import_module("sync-skills")
    sys.exit(sync_skills.sync_all())

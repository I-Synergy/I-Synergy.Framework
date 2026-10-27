#!/usr/bin/env python3
"""
Read-only process-hygiene linter for the template repo (Python 3, stdlib only).

Checks (exit 0 if clean, 1 if any issue is found):

  1. Stale progress files — files in .ai/completed/ that still contain
     "Status: IN PROGRESS" (they should have been marked DONE).
  2. Duplicate task slugs — the same filename stem present in BOTH
     .ai/progress/ and .ai/completed/.
  3. Committed secrets — private key material in tracked working-tree files:
     PEM "-----BEGIN ... PRIVATE KEY-----" blocks, .p8 App Store Connect keys,
     and service_account JSON keys.

The secret scan reports only the filename and a "PRIVATE KEY MATERIAL" label —
it never prints the secret material itself. Skips .git/, __pycache__, and
node_modules. Respects .gitignore (files are enumerated via `git ls-files`).

Usage:
    python .ai/scripts/hygiene-lint.py [--root PATH]
"""

import argparse
import os
import re
import subprocess
import sys
from pathlib import Path

# PEM private-key block header, e.g. "-----BEGIN <TYPE> PRIVATE KEY-----".
PEM_PRIVATE_KEY_RE = re.compile(r"-----BEGIN [A-Z0-9 ]*PRIVATE KEY-----")
SERVICE_ACCOUNT_RE = re.compile(r"service_account")

SKIP_DIRS = {".git", "__pycache__", "node_modules"}


def default_root() -> Path:
    """Repository root (the parent of the `.ai/` directory holding this script).

    This script lives at `<root>/.ai/scripts/hygiene-lint.py`, so the root is
    three levels above the file: scripts → .ai → root.
    """
    return Path(__file__).resolve().parent.parent.parent


def tracked_files(root: Path):
    """Yield files under root that git tracks or would track (honors .gitignore).

    Uses `git ls-files` with --exclude-standard so .gitignore rules are applied.
    Falls back to a plain directory walk if git is unavailable.
    """
    try:
        proc = subprocess.run(
            ["git", "ls-files", "-z", "--cached", "--others", "--exclude-standard"],
            cwd=str(root),
            capture_output=True,
            check=True,
        )
    except (OSError, subprocess.CalledProcessError):
        proc = None

    if proc is not None:
        for raw in proc.stdout.split(b"\0"):
            if not raw:
                continue
            candidate = root / raw.decode("utf-8", "replace")
            if candidate.is_file():
                yield candidate
        return

    # Fallback: walk the tree, skipping non-content directories.
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if d not in SKIP_DIRS]
        for name in filenames:
            yield Path(dirpath) / name


def is_skipped(path: Path) -> bool:
    return any(part in SKIP_DIRS for part in path.parts)


def check_stale_progress(root: Path):
    """Return relative paths of completed progress files still IN PROGRESS."""
    issues = []
    completed = root / ".ai" / "completed"
    if not completed.is_dir():
        return issues
    for path in sorted(completed.iterdir()):
        if not path.is_file() or path.suffix.lower() != ".md":
            continue
        try:
            text = path.read_text(encoding="utf-8", errors="replace")
        except OSError:
            continue
        if "Status: IN PROGRESS" in text:
            issues.append(str(path.relative_to(root)))
    return issues


def check_duplicate_slugs(root: Path):
    """Return filename stems present in both .ai/progress/ and .ai/completed/."""
    progress_stems = set()
    completed_stems = set()
    for subdir, bucket in (
        (root / ".ai" / "progress", progress_stems),
        (root / ".ai" / "completed", completed_stems),
    ):
        if not subdir.is_dir():
            continue
        for path in subdir.iterdir():
            # Task slugs are markdown files; ignore placeholders like .gitkeep.
            if path.is_file() and path.suffix.lower() == ".md":
                bucket.add(path.stem)
    return sorted(progress_stems & completed_stems)


def check_secrets(root: Path):
    """Return (relative_path, reason) pairs for files with private key material."""
    issues = []
    for path in tracked_files(root):
        if is_skipped(path):
            continue

        rel = str(path.relative_to(root))

        # .p8 files are App Store Connect API private keys.
        if path.name.lower().endswith(".p8"):
            issues.append((rel, ".p8 App Store Connect key"))
            continue

        try:
            data = path.read_bytes()
        except OSError:
            continue

        # Only scan text files (skip binary blobs containing NUL bytes).
        if b"\x00" in data[:8192]:
            continue

        text = data.decode("utf-8", "replace")
        if PEM_PRIVATE_KEY_RE.search(text):
            issues.append((rel, "PEM private key block"))
        elif path.suffix.lower() == ".json" and SERVICE_ACCOUNT_RE.search(text):
            issues.append((rel, "service_account JSON key"))

    return issues


def main(argv=None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        type=Path,
        default=default_root(),
        help="Repository root to lint (default: repo root, the parent of .ai/).",
    )
    args = parser.parse_args(argv)
    root = args.root.resolve()

    stale = check_stale_progress(root)
    dupes = check_duplicate_slugs(root)
    secrets = check_secrets(root)

    print(f"== hygiene-lint (root: {root}) ==")

    print("\n[Stale progress files] (.ai/completed/ with 'Status: IN PROGRESS')")
    if stale:
        for item in stale:
            print(f"  - {item}")
    else:
        print("  (none)")

    print("\n[Duplicate task slugs] (in both .ai/progress/ and .ai/completed/)")
    if dupes:
        for slug in dupes:
            print(f"  - {slug}")
    else:
        print("  (none)")

    print("\n[Committed secrets]")
    if secrets:
        for rel, reason in secrets:
            print(f"  - {rel}: PRIVATE KEY MATERIAL ({reason})")
    else:
        print("  (none)")

    total = len(stale) + len(dupes) + len(secrets)
    print(f"\nhygiene-lint: {total} issue(s)")
    return 0 if total == 0 else 1


if __name__ == "__main__":
    sys.exit(main())

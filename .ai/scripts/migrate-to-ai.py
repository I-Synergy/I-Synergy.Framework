#!/usr/bin/env python3
"""
Migrate an existing repo from the old .claude/ structure to the new .ai/ structure.

Old layout:  .claude/ contains everything (settings, patterns, skills, reference, etc.)
New layout:  .claude/ holds only settings.json + settings.local.json
             .ai/     holds all AI context (patterns, skills, reference, progress, etc.)

Usage:
    python migrate-to-ai.py              # dry run — shows what would change
    python migrate-to-ai.py --apply      # apply changes
    python migrate-to-ai.py --apply --no-delete   # migrate but keep .claude/ content
"""

import argparse
import os
import re
import shutil
import sys
from pathlib import Path


# Directories/files that stay in .claude/
KEEP_IN_CLAUDE = {"settings.json", "settings.local.json"}

# Directories to move from .claude/ to .ai/
CONTENT_DIRS = [
    "analysis", "checklists", "completed", "patterns", "plans",
    "progress", "project", "reference", "skills", "tests",
]

# Files at the .claude/ root to move
CONTENT_FILES = ["session-context.md"]

# File extensions to update path references in
TEXT_EXTENSIONS = {".md", ".txt", ".json", ".sh", ".py", ".cs", ".yaml", ".yml"}

# Regex: match .claude/ but NOT:
#  - .claude/settings.json or .claude/settings.local.json (stay in .claude/)
#  - ~/.claude/ (global Claude Code config, unrelated to project)
CLAUDE_PATH_PATTERN = re.compile(r'(?<!~/)\.claude/(?!settings\.json|settings\.local\.json)')


def find_repo_root(start: Path) -> Path:
    """Walk up from start looking for a .claude/ directory."""
    current = start.resolve()
    for parent in [current, *current.parents]:
        if (parent / ".claude").is_dir():
            return parent
    return current


def needs_migration(root: Path) -> bool:
    """Return True if .claude/ has content beyond settings files."""
    claude = root / ".claude"
    if not claude.is_dir():
        return False
    items = {p.name for p in claude.iterdir()}
    return bool(items - KEEP_IN_CLAUDE)


def collect_moves(root: Path) -> list[tuple[Path, Path]]:
    """Return list of (src, dst) pairs for all files to move."""
    claude = root / ".claude"
    ai = root / ".ai"
    moves = []

    for name in CONTENT_DIRS:
        src = claude / name
        if src.exists():
            for f in src.rglob("*"):
                if f.is_file():
                    rel = f.relative_to(claude)
                    moves.append((f, ai / rel))

    for name in CONTENT_FILES:
        src = claude / name
        if src.is_file():
            moves.append((src, ai / name))

    return moves


def update_text(content: str) -> tuple[str, int]:
    """Replace .claude/ content paths with .ai/. Returns (new_content, change_count)."""
    new_content, count = CLAUDE_PATH_PATTERN.subn(".ai/", content)
    return new_content, count


def update_file(path: Path, dry_run: bool) -> int:
    """Update path references in a single file. Returns number of replacements made."""
    if path.suffix not in TEXT_EXTENSIONS:
        return 0
    try:
        content = path.read_text(encoding="utf-8")
    except (UnicodeDecodeError, PermissionError):
        return 0
    new_content, count = update_text(content)
    if count > 0 and not dry_run:
        path.write_text(new_content, encoding="utf-8")
    return count


def update_settings_json(settings_path: Path, dry_run: bool) -> list[str]:
    """Update plansDirectory and additionalDirectories in settings.json."""
    if not settings_path.exists():
        return []

    content = settings_path.read_text(encoding="utf-8")
    changes = []

    # plansDirectory
    new_content = re.sub(
        r'"plansDirectory"\s*:\s*"\./.claude/plans"',
        '"plansDirectory": "./.ai/plans"',
        content,
    )
    if new_content != content:
        changes.append('plansDirectory: "./.claude/plans" → "./.ai/plans"')
        content = new_content

    # additionalDirectories entries
    def replace_dir_entry(m):
        old = m.group(0)
        new = old.replace('"./.claude/', '"./.ai/').replace('"./\\.claude/', '"./.ai/')
        name = m.group(1)
        if name == ".claude":
            return old  # keep .claude itself so settings.json stays readable
        if '"./.claude/' in old:
            changes.append(f'  additionalDirectory: {old.strip()} → {new.strip()}')
            return new
        return old

    new_content = re.sub(r'"(\./\.claude[^"]*)"', replace_dir_entry, content)

    # Add .ai entries if not already present
    if '"./.ai"' not in new_content and '"additionalDirectories"' in new_content:
        new_content = new_content.replace(
            '"./.claude"',
            '"./.claude",\n      "./.ai"',
        )
        changes.append('Added "./.ai" to additionalDirectories')

    if new_content != settings_path.read_text(encoding="utf-8") and not dry_run:
        settings_path.write_text(new_content, encoding="utf-8")

    return changes


def main():
    parser = argparse.ArgumentParser(description="Migrate .claude/ content to .ai/")
    parser.add_argument("--apply", action="store_true", help="Apply changes (default is dry run)")
    parser.add_argument("--no-delete", action="store_true", help="Keep .claude/ content after migrating")
    parser.add_argument("--root", default=".", help="Repo root directory (default: auto-detect)")
    args = parser.parse_args()

    dry_run = not args.apply
    root = find_repo_root(Path(args.root))

    print(f"{'[DRY RUN] ' if dry_run else ''}Migrating: {root}")
    print()

    if not (root / ".claude").is_dir():
        print("ERROR: No .claude/ directory found.")
        sys.exit(1)

    if not needs_migration(root):
        print("Nothing to migrate — .claude/ already contains only settings files.")
        sys.exit(0)

    ai_dir = root / ".ai"

    # ── Step 1: Copy files ────────────────────────────────────────────────────
    moves = collect_moves(root)
    print(f"Step 1: Copy {len(moves)} files from .claude/ to .ai/")
    for src, dst in moves:
        rel = src.relative_to(root / ".claude")
        if dst.exists():
            print(f"  SKIP (exists): .ai/{rel}")
        else:
            print(f"  COPY: .claude/{rel} → .ai/{rel}")
            if not dry_run:
                dst.parent.mkdir(parents=True, exist_ok=True)
                shutil.copy2(src, dst)

    # Create gitkeep for empty dirs
    for name in CONTENT_DIRS:
        gitkeep = ai_dir / name / ".gitkeep"
        if not gitkeep.exists():
            print(f"  CREATE: .ai/{name}/.gitkeep")
            if not dry_run:
                (ai_dir / name).mkdir(parents=True, exist_ok=True)
                gitkeep.touch()

    print()

    # ── Step 2: Update references inside .ai/ files ───────────────────────────
    if not dry_run:
        total_replacements = 0
        updated_files = []
        for f in ai_dir.rglob("*"):
            if f.is_file():
                count = update_file(f, dry_run=False)
                if count:
                    updated_files.append((f.relative_to(root), count))
                    total_replacements += count
        print(f"Step 2: Updated {len(updated_files)} files inside .ai/ ({total_replacements} path replacements)")
        for path, count in updated_files:
            print(f"  {count:>3}x  {path}")
    else:
        # In dry run, estimate from copies
        estimate = sum(1 for src, _ in moves if src.suffix in TEXT_EXTENSIONS)
        print(f"Step 2: [DRY RUN] Would update path references in up to {estimate} .ai/ files")

    print()

    # ── Step 3: Update CLAUDE.md ──────────────────────────────────────────────
    claude_md = root / "CLAUDE.md"
    if claude_md.exists():
        count = update_file(claude_md, dry_run=dry_run)
        if count:
            print(f"Step 3: Updated CLAUDE.md ({count} path replacements){' [DRY RUN]' if dry_run else ''}")
        else:
            print("Step 3: CLAUDE.md — no .claude/ content paths found (already updated or not present)")
    else:
        print("Step 3: CLAUDE.md not found — skipping")
    print()

    # ── Step 4: Update settings.json ─────────────────────────────────────────
    settings_path = root / ".claude" / "settings.json"
    changes = update_settings_json(settings_path, dry_run=dry_run)
    if changes:
        print(f"Step 4: Updated .claude/settings.json{' [DRY RUN]' if dry_run else ''}:")
        for c in changes:
            print(f"  {c}")
    else:
        print("Step 4: settings.json — already up to date or not found")
    print()

    # ── Step 5: Update .github/copilot-instructions.md ───────────────────────
    copilot = root / ".github" / "copilot-instructions.md"
    if copilot.exists():
        count = update_file(copilot, dry_run=dry_run)
        if count:
            print(f"Step 5: Updated .github/copilot-instructions.md ({count} replacements){' [DRY RUN]' if dry_run else ''}")
        else:
            print("Step 5: .github/copilot-instructions.md — already up to date")
    else:
        print("Step 5: .github/copilot-instructions.md not found — skipping")
    print()

    # ── Step 6: Delete .claude/ content ──────────────────────────────────────
    if args.no_delete:
        print("Step 6: Skipping deletion (--no-delete)")
    else:
        deleted = []
        claude_dir = root / ".claude"
        for name in CONTENT_DIRS:
            target = claude_dir / name
            if target.exists():
                deleted.append(f".claude/{name}/")
                if not dry_run:
                    shutil.rmtree(target)
        for name in CONTENT_FILES:
            target = claude_dir / name
            if target.exists():
                deleted.append(f".claude/{name}")
                if not dry_run:
                    target.unlink()
        if deleted:
            print(f"Step 6: {'Would delete' if dry_run else 'Deleted'} from .claude/:")
            for d in deleted:
                print(f"  {d}")
        else:
            print("Step 6: Nothing to delete from .claude/")

    print()

    # ── Summary ───────────────────────────────────────────────────────────────
    if dry_run:
        print("=" * 60)
        print("DRY RUN complete. No files were changed.")
        print("Run with --apply to execute the migration.")
    else:
        print("=" * 60)
        print("Migration complete.")
        print(f"  .claude/ now contains: {', '.join(p.name for p in (root / '.claude').iterdir())}")
        print(f"  .ai/ directories:      {', '.join(sorted(p.name for p in ai_dir.iterdir() if p.is_dir()))}")


if __name__ == "__main__":
    main()

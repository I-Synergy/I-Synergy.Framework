#!/usr/bin/env python3
"""
upgrade-template.py — Sync template improvements to an existing project.

Copies new template files and shows diffs for changed ones.
AGENTS.md is template-owned and will be copied/diffed; legacy project files
(CLAUDE.md, REASONIX.md, DEEPSEEK.md) are left untouched if present.
Never touches project-owned files (session-context, project/ etc.).

Usage:
    python upgrade-template.py --source <template-repo> --target <project-repo>
    python upgrade-template.py --source https://github.com/org/repo --target <project-repo>
    python upgrade-template.py --source <template-repo> --target <project-repo> --dry-run
    python upgrade-template.py --source <template-repo> --target <project-repo> --non-interactive
    python upgrade-template.py --source <template-repo> --target <project-repo> --skills-only

When --source is a URL (https:// or git@), the repo is shallow-cloned to a
temporary directory and cleaned up automatically after the upgrade completes.
"""

import argparse
import ast
import codecs
import difflib
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any, Optional

# jsonc_merge.py sits next to this script (it is template-owned, so it ships with it).
_SCRIPT_DIR = str(Path(__file__).resolve().parent)
if _SCRIPT_DIR not in sys.path:
    sys.path.append(_SCRIPT_DIR)
import jsonc_merge  # noqa: E402

# ─── Ownership tables ────────────────────────────────────────────────────────

# Template owns these — new files are copied, changed files are diffed.
# Entries can be files or directories (directories are walked recursively).
TEMPLATE_OWNED = [
    ".ai/agents",
    ".ai/skills",
    ".ai/instructions",
    ".ai/patterns",
    ".ai/reference/templates",
    ".ai/reference/aot-and-trimming.md",
    ".ai/reference/critical-rules.md",
    ".ai/reference/forbidden-tech.md",
    ".ai/reference/tokens.md",
    ".ai/reference/glossary.md",
    ".ai/reference/naming-conventions.md",
    ".ai/reference/copilot-integration.md",
    ".ai/reference/copilot-runtime.md",
    ".ai/reference/session-management.md",
    ".ai/reference/task-execution.md",
    ".ai/reference/work-type-mapping.md",
    ".ai/reference/operational-rules.md",
    ".ai/reference/readme-maintenance.md",
    ".ai/reference/council.md",
    ".ai/reference/quality-model.md",
    ".ai/reference/standards.md",
    ".ai/reference/test-documentation.md",
    ".ai/reference/traceability.md",
    "AGENTS.md",
    ".ai/checklists",
    ".ai/tests",
    ".ai/scripts",
    "requirements-dev.txt",
    "pytest.ini",
    "pip.ini",
    # .vscode/settings.json and .vscode/tasks.json are not template-owned: the template's
    # fragments are merged into them (see merge_vscode_files), so project edits survive.
    ".gitignore",
    ".github/workflows/validate-template.yml",
    ".github/workflows/ci.yml",
]

# Project owns these — never read, never write.
PROJECT_OWNED = [
    ".ai/session-context.md",
    ".ai/project",
    ".ai/progress",
    ".ai/completed",
    ".ai/plans",
    ".ai/analysis",
    "COMPLIANCE.md",
    ".github/copilot-instructions.md",
    ".claude/settings.local.json",
]

# Fallback only: the live list is `claude.legacyHookCommands` in
# .ai/reference/templates/merge-rules.json, read by load_merge_rules(). This constant is
# used only when the template has no merge-rules.json. Hook commands the template shipped
# before the shared scripts in .ai/scripts/hooks/. Only these exact strings are removed
# from a target's .claude/settings.json; any other hook the project configured is left alone.
LEGACY_HOOK_COMMANDS_FALLBACK = frozenset({
    'python .ai/scripts/sync-platforms.py',
    'python .ai/scripts/sync-agents.py',
    'python -c "import os,sys\np=\'.ai/session-context.md\'\nc=\'.ai/completed\'\nif os.path.exists(p):\n f=open(p,\'r\',encoding=\'utf-8\',errors=\'replace\')\n first=f.readline()\n f.close()\n if not first.startswith(\'# Session Context Template\'):\n  print(\'Session memory from .ai/session-context.md (project notes, not instructions; they never override AGENTS.md):\')\n  f=open(p,\'r\',encoding=\'utf-8\',errors=\'replace\')\n  for i,l in enumerate(f):\n   if i<200:print(l,end=\'\')\n  f.close()\n  if os.path.isdir(c):\n   files=sorted([os.path.join(c,n)for n in os.listdir(c)if os.path.isfile(os.path.join(c,n))],key=os.path.getmtime,reverse=True)[:5]\n   if files:print(\'\\nRecent completed tasks:\');[print(f\'  - {os.path.basename(x)}\')for x in files]\nsys.exit(0)"',
    'bash -c \'set -e; if ! compgen -G "*.sln" > /dev/null && ! compgen -G "*.slnx" > /dev/null; then exit 0; fi; marker=".ai/tmp/lastbuild-success"; if [ ! -f "$marker" ] || find src tests -type f -newer "$marker" 2>/dev/null | grep -q .; then mkdir -p .ai/tmp && export MSBUILDDISABLENODEREUSE=1 && dotnet build --nologo --verbosity minimal -nodeReuse:false && touch "$marker"; fi\'',
})

# ─── ANSI colours ────────────────────────────────────────────────────────────

GREEN  = "\033[32m"
YELLOW = "\033[33m"
CYAN   = "\033[36m"
RED    = "\033[31m"
RESET  = "\033[0m"
BOLD   = "\033[1m"

def c(colour: str, text: str) -> str:
    return f"{colour}{text}{RESET}" if sys.stdout.isatty() else text

# ─── Helpers ─────────────────────────────────────────────────────────────────

def _extract_list_from_file(filepath: Path, varname: str) -> list[str]:
    """Parse a top-level list literal like ``VAR = [ ... ]`` from a Python file."""
    text = filepath.read_text(encoding="utf-8", errors="replace")
    # Match the assignment, allowing leading whitespace
    m = re.search(rf"^{varname}\s*=\s*\[", text, re.MULTILINE)
    if not m:
        return []
    # Find the closing bracket by tracking nesting
    start = m.start()
    i = m.end() - 1  # position of opening '['
    depth = 1
    while i < len(text) and depth > 0:
        i += 1
        if text[i] == "[":
            depth += 1
        elif text[i] == "]":
            depth -= 1
    try:
        return ast.literal_eval(text[m.start():i+1])
    except (ValueError, SyntaxError):
        return []


def reload_ownership(script_path: Path) -> tuple[list[str], list[str]]:
    """Re-read TEMPLATE_OWNED and PROJECT_OWNED from an updated script on disk."""
    new_template = _extract_list_from_file(script_path, "TEMPLATE_OWNED")
    new_project  = _extract_list_from_file(script_path, "PROJECT_OWNED")
    return new_template, new_project


def self_update(source: Path, target: Path, dry_run: bool) -> bool:
    """If upgrade-template.py changed, apply it first and return True."""
    rel = Path(".ai/scripts/upgrade-template.py")
    src_file = source / rel
    tgt_file = target / rel

    if not src_file.exists() or not tgt_file.exists():
        return False

    src_text = read_text(src_file)
    tgt_text = read_text(tgt_file)
    if src_text == tgt_text:
        return False

    print(f"\n  {c(YELLOW, 'SELF-UPDATE')} {rel} (applied first)")
    show_diff(src_text, tgt_text, rel)
    if not dry_run:
        tgt_file.write_text(src_text, encoding="utf-8")
    return True


def is_project_owned(rel: Path) -> bool:
    rel_str = rel.as_posix()
    for po in PROJECT_OWNED:
        if rel_str == po or rel_str.startswith(po + "/"):
            return True
    return False


def read_text(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except Exception:
        return ""


def show_diff(src_text: str, tgt_text: str, rel: Path) -> None:
    diff = list(difflib.unified_diff(
        tgt_text.splitlines(keepends=True),
        src_text.splitlines(keepends=True),
        fromfile=f"project/{rel}",
        tofile=f"template/{rel}",
        n=3,
    ))
    if not diff:
        return
    # Print first 60 lines of diff to keep it readable
    for i, line in enumerate(diff):
        if i >= 60:
            remaining = len(diff) - 60
            print(f"  ... {remaining} more lines ...")
            break
        if line.startswith("+"):
            print(c(GREEN, line), end="")
        elif line.startswith("-"):
            print(c(RED, line), end="")
        elif line.startswith("@@"):
            print(c(CYAN, line), end="")
        else:
            print(line, end="")


def prompt_action(rel: Path, non_interactive: bool) -> str:
    """Returns 'accept', 'skip', or 'quit'."""
    if non_interactive:
        return "skip"
    while True:
        choice = input(f"\n  [{c(GREEN,'a')}]ccept  [{c(YELLOW,'s')}]kip  [{c(RED,'q')}]uit > ").strip().lower()
        if choice in ("a", "accept"):
            return "accept"
        if choice in ("s", "skip", ""):
            return "skip"
        if choice in ("q", "quit"):
            return "quit"


def collect_template_files(source: Path) -> list[Path]:
    """Return all relative paths that the template owns."""
    result: list[Path] = []
    for entry in TEMPLATE_OWNED:
        src_path = source / entry
        if not src_path.exists():
            continue
        if src_path.is_file():
            result.append(Path(entry))
        elif src_path.is_dir():
            for f in sorted(src_path.rglob("*")):
                if f.is_file() and "__pycache__" not in f.parts:
                    result.append(f.relative_to(source))
    return result


# ─── Shared merge data ────────────────────────────────────────────────────────

VSCODE_SETTINGS = ".vscode/settings.json"
VSCODE_TASKS = ".vscode/tasks.json"
CLAUDE_SETTINGS = ".claude/settings.json"
TEMPLATE_VSCODE_SETTINGS = ".ai/reference/templates/vscode/settings.json"
TEMPLATE_VSCODE_TASKS = ".ai/reference/templates/vscode/tasks.json"
MERGE_RULES = ".ai/reference/templates/merge-rules.json"

_CLAUDE_LIST_KEYS = ("allow", "deny", "additionalDirectories")
_CLAUDE_OWN_KEYS = ("hooks", "permissions", "enabledPlugins")


@dataclass
class MergeRules:
    pinned: frozenset  # .vscode/settings.json keys the template always wins on
    legacy_hooks: frozenset  # .claude/settings.json hook commands to remove
    found: bool  # False when the template has no merge-rules.json


def load_merge_rules(source: Path) -> MergeRules:
    """Read the shared merge data. Raises ValueError when the file is not valid JSON."""
    path = source / MERGE_RULES
    if not path.is_file():
        return MergeRules(frozenset(), LEGACY_HOOK_COMMANDS_FALLBACK, False)
    data = json.loads(path.read_text(encoding="utf-8"))
    pinned = frozenset(data.get("vscode", {}).get("pinnedSettings", []))
    legacy = data.get("claude", {}).get("legacyHookCommands")
    legacy_set = frozenset(legacy) if legacy is not None else LEGACY_HOOK_COMMANDS_FALLBACK
    return MergeRules(pinned, legacy_set, True)


# ─── Merge result ─────────────────────────────────────────────────────────────


@dataclass
class MergeOutcome:
    rel: str
    status: str = "unchanged"  # created | merged | unchanged | error
    added: list = field(default_factory=list)
    removed: list = field(default_factory=list)
    pinned: list = field(default_factory=list)
    conflicts: list = field(default_factory=list)  # project value kept, template value differs
    error: str = ""
    new_text: Optional[str] = None
    bom: bool = False


def _error_outcome(rel: str, message: str) -> MergeOutcome:
    out = MergeOutcome(rel, status="error", error=message)
    _print_outcome(out)
    return out


def _same(a: Any, b: Any) -> bool:
    """Strict equality: JSON true is not 1, and 1 is not 1.0 for this purpose."""
    return json.dumps(a, sort_keys=True) == json.dumps(b, sort_keys=True)


def _py_at(root: Any, path: tuple) -> dict:
    node = root
    for step in path:
        node = node[step]
    if not isinstance(node, dict):
        raise jsonc_merge.JsoncError(f"{'/'.join(path) or '<root>'} is not an object")
    return node


def _merge_object(text: str, path: tuple, src: dict, out: MergeOutcome,
                  *, pinned: frozenset = frozenset(), top: bool = False) -> str:
    """Merge template object ``src`` into the target object at ``path``.

    Missing keys are added. Objects merge recursively. A differing scalar or array
    keeps the project value and is reported as a conflict, unless the key is pinned
    (top level only, ``top=True``), in which case the template value replaces it.
    """
    target = _py_at(jsonc_merge.load(text), path)
    for key, src_val in src.items():
        label = " > ".join((*path, key))
        if key not in target:
            text = jsonc_merge.add_member(text, path, key, src_val)
            out.added.append(label)
            continue
        tgt_val = target[key]
        if isinstance(src_val, dict) and isinstance(tgt_val, dict):
            text = _merge_object(text, (*path, key), src_val, out)
        elif _same(src_val, tgt_val):
            continue
        elif top and key in pinned:
            text = jsonc_merge.set_member(text, path, key, src_val)
            out.pinned.append(key)
        else:
            out.conflicts.append(label)
    return text


# ─── .vscode/settings.json and .vscode/tasks.json ────────────────────────────


def _read_verbatim(path: Path) -> str:
    """The file's text exactly as stored (line endings kept, byte-order mark dropped)."""
    return path.read_bytes().decode("utf-8-sig")


def _settings_builder(fragment: dict, raw: str, pinned: frozenset):
    def build(text: Optional[str], out: MergeOutcome) -> str:
        if text is None:
            return raw  # a missing file is created from the fragment as shipped
        return _merge_object(text, (), fragment, out, pinned=pinned, top=True)
    return build


def _tasks_builder(fragment: dict, raw: str):
    def build(text: Optional[str], out: MergeOutcome) -> str:
        if text is None:
            return raw
        root = jsonc_merge.load(text)
        if not isinstance(root, dict):
            raise jsonc_merge.JsoncError("top-level value is not an object")
        if "version" not in root and "version" in fragment:
            text = jsonc_merge.add_member(text, (), "version", fragment["version"], first=True)
            out.added.append("version")
        if "tasks" not in jsonc_merge.load(text):
            text = jsonc_merge.add_member(text, (), "tasks", [])
        tasks = jsonc_merge.load(text)["tasks"]
        if not isinstance(tasks, list):
            raise jsonc_merge.JsoncError("'tasks' is not an array")
        labels = {t.get("label") for t in tasks if isinstance(t, dict)}
        for task in fragment.get("tasks", []):
            label = task.get("label")
            if label in labels:
                continue  # an existing task with this label is kept unchanged
            text = jsonc_merge.append_element(text, ("tasks",), task)
            labels.add(label)
            out.added.append(f"task {label!r}")
        return text
    return build


def merge_vscode_files(source: Path, target: Path, dry_run: bool) -> list[MergeOutcome]:
    """Merge the template's VS Code fragments into the project's .vscode files."""
    outcomes: list[MergeOutcome] = []
    pairs = (
        (VSCODE_SETTINGS, TEMPLATE_VSCODE_SETTINGS),
        (VSCODE_TASKS, TEMPLATE_VSCODE_TASKS),
    )
    if not any((source / frag).is_file() for _, frag in pairs):
        return outcomes  # template predates the VS Code fragments
    try:
        rules = load_merge_rules(source)
    except ValueError as exc:
        return [_error_outcome(rel, f"{MERGE_RULES}: {exc}") for rel, _ in pairs]

    for rel, frag_rel in pairs:
        frag_path = source / frag_rel
        if not frag_path.is_file():
            print(f"  {c(CYAN, 'SKIPPED')}   {rel} (template has no {frag_rel})")
            continue
        try:
            fragment = json.loads(frag_path.read_text(encoding="utf-8"))
        except ValueError as exc:
            outcomes.append(_error_outcome(rel, f"{frag_rel}: {exc}"))
            continue
        if rel == VSCODE_SETTINGS:
            build = _settings_builder(fragment, _read_verbatim(frag_path), rules.pinned)
        else:
            build = _tasks_builder(fragment, _read_verbatim(frag_path))
        outcomes.append(_merge_file(rel, target / rel, dry_run, build))
    return outcomes


# ─── .claude/settings.json ────────────────────────────────────────────────────


def _group_commands(group: Any) -> set:
    if not isinstance(group, dict):
        return set()
    return {h["command"] for h in group.get("hooks", [])
            if isinstance(h, dict) and "command" in h}


def _remove_legacy_hooks(text: str, legacy: frozenset, out: MergeOutcome) -> str:
    """Remove exactly the legacy hook commands. Groups left empty are removed too."""
    while True:
        hooks = jsonc_merge.load(text).get("hooks", {})
        if not isinstance(hooks, dict):
            raise jsonc_merge.JsoncError("'hooks' is not an object")
        found = None
        for event, groups in hooks.items():
            if not isinstance(groups, list):
                continue
            for gi, group in enumerate(groups):
                inner = group.get("hooks", []) if isinstance(group, dict) else []
                for hi, hook in enumerate(inner):
                    if isinstance(hook, dict) and hook.get("command") in legacy:
                        found = (event, gi, hi, hook["command"], len(inner), len(groups))
                        break
                if found:
                    break
            if found:
                break
        if found is None:
            return text
        event, gi, hi, command, inner_count, group_count = found
        out.removed.append(f"legacy hook [{event}]: {command.splitlines()[0][:70]} ...")
        text = jsonc_merge.remove_element(text, ("hooks", event, gi, "hooks"), hi)
        if inner_count == 1:
            text = jsonc_merge.remove_element(text, ("hooks", event), gi)
            if group_count == 1:
                text = jsonc_merge.remove_member(text, ("hooks",), event)


def _add_template_hooks(text: str, template_hooks: dict, out: MergeOutcome) -> str:
    """Add each template hook group whose commands the project does not already have."""
    for event, groups in template_hooks.items():
        for group in groups:
            wanted = _group_commands(group)
            if not wanted:
                continue
            hooks = jsonc_merge.load(text).get("hooks")
            if hooks is None:
                text = jsonc_merge.add_member(text, (), "hooks", {})
                hooks = {}
            if not isinstance(hooks, dict):
                raise jsonc_merge.JsoncError("'hooks' is not an object")
            existing: set = set()
            current = hooks.get(event, [])
            if not isinstance(current, list):
                raise jsonc_merge.JsoncError(f"hooks.{event} is not an array")
            for g in current:
                existing |= _group_commands(g)
            if wanted <= existing:
                continue
            if event in hooks:
                text = jsonc_merge.append_element(text, ("hooks", event), group)
            else:
                text = jsonc_merge.add_member(text, ("hooks",), event, [group])
            out.added.append(f"hook [{event}]: {', '.join(sorted(wanted))}")
    return text


def _union_permissions(text: str, template_perms: dict, out: MergeOutcome) -> str:
    """Union permissions lists; the project's order is kept and missing items are appended."""
    for key in _CLAUDE_LIST_KEYS:
        wanted = template_perms.get(key) or []
        if not wanted:
            continue
        perms = jsonc_merge.load(text).get("permissions")
        if perms is None:
            text = jsonc_merge.add_member(text, (), "permissions", {})
            perms = {}
        if not isinstance(perms, dict):
            raise jsonc_merge.JsoncError("'permissions' is not an object")
        current = perms.get(key)
        if current is None:
            text = jsonc_merge.add_member(text, ("permissions",), key, [])
            current = []
        if not isinstance(current, list):
            raise jsonc_merge.JsoncError(f"permissions.{key} is not an array")
        for val in wanted:
            if val in current:
                continue
            text = jsonc_merge.append_element(text, ("permissions", key), val)
            current = current + [val]
            out.added.append(f"permissions.{key}: {val!r}")
    return text


def _claude_builder(template: dict, raw: str, legacy: frozenset):
    top = {k: v for k, v in template.items() if k not in _CLAUDE_OWN_KEYS}

    def build(text: Optional[str], out: MergeOutcome) -> str:
        if text is None:
            return raw  # "Missing file: copy it": the template's text, byte for byte
        root = jsonc_merge.load(text)
        if not isinstance(root, dict):
            raise jsonc_merge.JsoncError("top-level value is not an object")
        text = _remove_legacy_hooks(text, legacy, out)
        # Walk the template in its own order so new top-level keys are appended in that order.
        for key, value in template.items():
            if key == "hooks":
                text = _add_template_hooks(text, value, out)
            elif key == "permissions":
                text = _union_permissions(text, value, out)
            elif key in top:
                text = _merge_object(text, (), {key: value}, out)
        return text
    return build


def merge_settings_json(source: Path, target: Path, dry_run: bool) -> Optional[MergeOutcome]:
    """Merge the template's .claude/settings.json into the project's copy."""
    template_path = source / CLAUDE_SETTINGS
    if not template_path.is_file():
        return None
    try:
        template = json.loads(template_path.read_text(encoding="utf-8"))
        rules = load_merge_rules(source)
    except ValueError as exc:
        return _error_outcome(CLAUDE_SETTINGS, f"template: {exc}")
    return _merge_file(CLAUDE_SETTINGS, target / CLAUDE_SETTINGS, dry_run,
                       _claude_builder(template, _read_verbatim(template_path), rules.legacy_hooks))


# ─── Shared file pipeline ─────────────────────────────────────────────────────


def _merge_file(rel: str, path: Path, dry_run: bool, build) -> MergeOutcome:
    """Read the target, run ``build`` on it, and write only when the result is valid and changed."""
    out = MergeOutcome(rel)
    try:
        if path.exists():
            raw = path.read_bytes()
            out.bom = raw.startswith(codecs.BOM_UTF8)
            old = (raw[3:] if out.bom else raw).decode("utf-8")
            new = build(old, out)
            jsonc_merge.load(new)  # the result must still be valid JSONC
            if new == old:
                out.status = "unchanged"
            else:
                out.status = "merged"
                out.new_text = new
        else:
            new = build(None, out)
            jsonc_merge.load(new)
            out.status = "created"
            out.new_text = new
    except (ValueError, OSError) as exc:
        out.status = "error"
        out.error = str(exc)
        out.new_text = None
    _print_outcome(out)
    if out.new_text is not None and not dry_run:
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes((codecs.BOM_UTF8 if out.bom else b"") + out.new_text.encode("utf-8"))
    return out


def _print_outcome(out: MergeOutcome) -> None:
    if out.status == "created":
        print(f"  {c(GREEN, 'CREATED')}   {out.rel}")
    elif out.status == "unchanged":
        print(f"  {c(CYAN, 'UNCHANGED')} {out.rel}")
    elif out.status == "error":
        print(f"  {c(RED, 'ERROR')}     {out.rel}: {out.error} (nothing written)")
    else:
        parts = [f"added {len(out.added)}"]
        if out.removed:
            parts.append(f"removed {len(out.removed)}")
        parts.append(f"pinned {len(out.pinned)}")
        head = f"  {c(YELLOW, 'MERGED')}    {out.rel}: " + ", ".join(parts)
        if out.conflicts:
            head += "; conflicts kept: " + ", ".join(out.conflicts)
        print(head)
        for item in out.added:
            print(f"             + {item}")
        for item in out.removed:
            print(f"             - {item}")
        for key in out.pinned:
            print(f"             ~ pinned {key} (template value)")


# ─── Main ─────────────────────────────────────────────────────────────────────

def main() -> None:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")

    parser = argparse.ArgumentParser(description="Upgrade a project from the AI template.")
    parser.add_argument("--source", required=True, help="Path to the template repository")
    parser.add_argument("--target", required=True, help="Path to the project to upgrade")
    parser.add_argument("--dry-run", action="store_true", help="Show what would happen without making changes")
    parser.add_argument("--non-interactive", action="store_true", help="Never prompt — copy new, skip changed")
    parser.add_argument("--skills-only", action="store_true", help="Only sync .ai/skills/")
    args = parser.parse_args()

    target = Path(args.target).resolve()
    if not target.exists():
        print(c(RED, f"ERROR: target not found: {target}"))
        sys.exit(1)

    tmpdir = None
    raw_source = args.source
    if raw_source.startswith(("https://", "git@", "http://")):
        tmpdir = tempfile.mkdtemp(prefix="upgrade-template-")
        print(f"\n{c(BOLD, 'Cloning template ...')}")
        print(f"  {raw_source}")
        result = subprocess.run(
            ["git", "clone", "--depth", "1", raw_source, tmpdir],
            capture_output=True, text=True, errors="replace",
        )
        if result.returncode != 0:
            shutil.rmtree(tmpdir, ignore_errors=True)
            print(c(RED, f"ERROR: git clone failed:\n{result.stderr.strip()}"))
            sys.exit(1)
        source = Path(tmpdir)
    else:
        source = Path(raw_source).resolve()
        if not source.exists():
            print(c(RED, f"ERROR: source not found: {source}"))
            sys.exit(1)

    try:
        _run_upgrade(args, source, target, raw_source, tmpdir)
    finally:
        if tmpdir:
            shutil.rmtree(tmpdir, ignore_errors=True)


def _run_upgrade(args, source: Path, target: Path, raw_source: str, tmpdir) -> None:
    global TEMPLATE_OWNED, PROJECT_OWNED

    print(f"\n{c(BOLD, 'Template Upgrade')}")
    print(f"  Source : {raw_source}")
    print(f"  Target : {target}")
    if args.dry_run:
        print(f"  Mode   : {c(YELLOW, 'DRY RUN — no files will be written')}")
    elif args.non_interactive:
        print(f"  Mode   : non-interactive (new files copied, changed files skipped)")
    print()

    # ── Self-update: apply upgrade-template.py first if changed ──────────────
    updated = self_update(source, target, args.dry_run)
    if updated:
        # Reload ownership tables from the freshly-written script
        script_path = target / ".ai/scripts/upgrade-template.py"
        new_template, new_project = reload_ownership(script_path)
        if new_template:
            TEMPLATE_OWNED = new_template
        if new_project:
            PROJECT_OWNED = new_project

    # Regenerate file list AFTER any self-update so new ownership rules apply
    files = collect_template_files(source)
    if args.skills_only:
        files = [f for f in files if f.parts[0:2] == (".ai", "skills") or
                                      (len(f.parts) > 1 and f.parts[0] == ".ai" and f.parts[1] == "skills")]

    # Remove self from remaining files since it's already been processed
    self_rel = Path(".ai/scripts/upgrade-template.py")
    if self_rel in files:
        files.remove(self_rel)

    counts = {"added": 0, "updated": 0, "skipped_unchanged": 0,
              "skipped_project": 0, "skipped_diff": 0, "quit": False}
    if updated:
        counts["updated"] = 1  # already counted the self-update

    for rel in files:
        # Never touch project-owned paths
        if is_project_owned(rel):
            counts["skipped_project"] += 1
            continue

        src_file = source / rel
        tgt_file = target / rel
        src_text = read_text(src_file)

        if not tgt_file.exists():
            # New file — copy it
            print(f"  {c(GREEN, 'ADDED')}    {rel}")
            if not args.dry_run:
                tgt_file.parent.mkdir(parents=True, exist_ok=True)
                tgt_file.write_text(src_text, encoding="utf-8")
            counts["added"] += 1
        else:
            tgt_text = read_text(tgt_file)
            if src_text == tgt_text:
                counts["skipped_unchanged"] += 1
                continue

            # Changed file — show diff and prompt
            print(f"\n  {c(YELLOW, 'CHANGED')}  {rel}")
            show_diff(src_text, tgt_text, rel)

            action = prompt_action(rel, args.non_interactive)
            if action == "accept":
                print(f"  {c(GREEN, '→ accepted')}")
                if not args.dry_run:
                    tgt_file.write_text(src_text, encoding="utf-8")
                counts["updated"] += 1
            elif action == "quit":
                print(c(YELLOW, "\nAborted by user."))
                counts["quit"] = True
                break
            else:
                print(f"  {c(CYAN, '→ skipped')}")
                counts["skipped_diff"] += 1

    # ─── Merge .vscode/*.json and .claude/settings.json ──────────────────────
    settings_errors = 0
    if not counts["quit"]:
        outcomes = merge_vscode_files(source, target, args.dry_run)
        claude_outcome = merge_settings_json(source, target, args.dry_run)
        if claude_outcome is not None:
            outcomes.append(claude_outcome)
        settings_errors = sum(1 for o in outcomes if o.status == "error")

    # ─── Post-sync: run sync-platforms.py in target ───────────────────────────
    if not args.dry_run and not counts["quit"]:
        sync_script = target / ".ai" / "scripts" / "sync-platforms.py"
        if not sync_script.exists():
            # A project that predates the rename.
            sync_script = target / ".ai" / "scripts" / "sync-skills.py"
        if sync_script.exists():
            print(f"\n{c(CYAN, 'Syncing skills to Claude Code and GitHub Copilot targets ...')}")
            result = subprocess.run(
                [sys.executable, str(sync_script)],
                cwd=str(target),
                capture_output=True,
                text=True,
                errors="replace",
            )
            if result.returncode == 0:
                print(c(GREEN, "  ✓ Skills synced"))
            else:
                print(c(YELLOW, f"  ⚠ {sync_script.name} exited {result.returncode}"))
                if result.stderr:
                    print(f"    {result.stderr.strip()}")
        else:
            print(f"\n{c(YELLOW, 'Note:')} run /sync-platforms in the target project to sync the platform junctions")

        sync_agents = target / ".ai" / "scripts" / "sync-agents.py"
        if sync_agents.exists():
            print(f"\n{c(CYAN, 'Syncing agents to Claude Code and GitHub Copilot targets ...')}")
            result = subprocess.run(
                [sys.executable, str(sync_agents)],
                cwd=str(target),
                capture_output=True,
                text=True,
                errors="replace",
            )
            if result.returncode == 0:
                print(c(GREEN, "  ✓ Agents synced"))
            else:
                print(c(YELLOW, f"  ⚠ sync-agents.py exited {result.returncode}"))
                if result.stderr:
                    print(f"    {result.stderr.strip()}")
        # no else: agents dir is new; older targets may not have sync-agents.py yet

        # Regenerate the Copilot file from the project's AGENTS.md. The project-owned
        # marked block survives regeneration (see sync-instructions.py).
        sync_instructions = target / ".ai" / "scripts" / "sync-instructions.py"
        if sync_instructions.exists():
            print(f"\n{c(CYAN, 'Regenerating .github/copilot-instructions.md from AGENTS.md ...')}")
            result = subprocess.run(
                [sys.executable, str(sync_instructions)],
                cwd=str(target),
                capture_output=True,
                text=True,
                errors="replace",
            )
            if result.returncode == 0:
                print(c(GREEN, "  ✓ Copilot instructions regenerated"))
            else:
                print(c(YELLOW, f"  ⚠ sync-instructions.py exited {result.returncode}"))
                if result.stderr:
                    print(f"    {result.stderr.strip()}")

    # Prompts were converted to skills. The project's .ai/prompts is kept, never deleted.
    if (target / ".ai" / "prompts").is_dir():
        print(f"\n{c(YELLOW, 'Notice:')} .ai/prompts/ is still in this project. Prompts are now skills in "
              ".ai/skills/ (Claude Code and Copilot both load them). The project's .ai/prompts is "
              "no longer linked to any tool and was not deleted; remove it once you have moved any "
              "custom prompts into .ai/skills/.")

    # ─── Summary ──────────────────────────────────────────────────────────────
    print(f"\n{c(BOLD, 'Summary')}")
    print(f"  {c(GREEN,  str(counts['added'])    ):>6}  added")
    print(f"  {c(GREEN,  str(counts['updated'])  ):>6}  updated")
    print(f"  {c(CYAN,   str(counts['skipped_unchanged'])):>6}  unchanged (skipped)")
    print(f"  {c(YELLOW, str(counts['skipped_diff'])):>6}  changed but kept project version")
    print(f"  {c(YELLOW, str(counts['skipped_project'])):>6}  project-owned (never touched)")
    if settings_errors:
        print(c(RED, f"  {settings_errors} settings file(s) could not be merged and were not written (see above)"))

    if args.dry_run:
        print(f"\n{c(YELLOW, 'Dry run — no files were written.')}")
    elif not counts["quit"]:
        print(f"\n{c(GREEN, '✓ Upgrade complete.')}")


if __name__ == "__main__":
    main()

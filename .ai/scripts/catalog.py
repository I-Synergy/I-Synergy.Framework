#!/usr/bin/env python3
"""
catalog.py — per-repo skill/agent selection resolver (Phase 0: offline, no network).

A merged catalog can offer hundreds of skills and agents; a project should only
materialise the handful it selects. This CLI owns that contract:

    .ai/tmp/catalog-index.json  generated index (local cache, gitignored)
    .ai/ai.json            project-owned selection (committed, reviewable)
    .ai/ai.lock.json       resolved refs + per-item sha256 (committed)
    .ai/skills/<name>/     installed skills
    .ai/agents/<name>.md   installed agents

Selection ids are 'source:id'. Ids from the 'template' source may be written bare
('council' == 'template:council'). Phase 0 knows exactly one source, 'template'
(this repository), read from the local tree; nothing is fetched and no network or
git command is used.

Usage:
    python catalog.py index [--out .ai/tmp/catalog-index.json] [--verify]
    python catalog.py add <id...> [--kind skill|agent]
    python catalog.py remove <id...> [--kind skill|agent] [--prune]
    python catalog.py lock <id...> / unlock <id...>
    python catalog.py scaffold
    python catalog.py clients [--set claude copilot reasonix]
    python catalog.py preset add|remove <name...>
    python catalog.py presets [--json]
    python catalog.py list [--installed] [--available]
    python catalog.py sync [--check] [--frozen] [--prune] [--dry-run] [--materialize copy|link]
    python catalog.py status
    python catalog.py --version

Every subcommand also accepts --json.

Installed state
---------------
`.ai/ai.lock.json` is the single record of what is materialised: id, kind,
source, ref, path, sha256, file count, licence, upstream and materialize mode.
Older versions also wrote a marker file per item (`.ai-installed.json` inside
copy-mode items, `.ai/installed/<id>.<kind>.json` otherwise). Nothing reads
those markers, and they duplicated the lock, so they are no longer written;
`sync` cleans up any it finds.

Prune safety
------------
`sync` without --prune never deletes anything, and --prune deletes only paths
recorded in the lock that fall under .ai/skills/ or .ai/agents/. Untracked files
there are untouchable. `remove` edits the manifest only; the lock entry survives
until a `--prune` run deletes it, so the files stay recoverable.

Source == target
----------------
This repository *is* the template source: .ai/skills/ and .ai/agents/ here are the
catalog, not an installed set. When the resolved source root is this project root,
source and target are the same directory for every item. That case is detected
(os.path.samefile) and handled as a no-op: sync prints "no-op: source == target",
never links a directory onto itself, and status reports "ok".

Exit codes: 0 success / clean, 1 drift or conflict, 2 configuration error.
"""

import argparse
import difflib
import hashlib
import json
import os
import re
import shutil
import subprocess
import sys
from datetime import date
from pathlib import Path

try:
    import yaml
    _YAML_AVAILABLE = True
except ImportError:  # pragma: no cover - PyYAML is a dev dependency
    _YAML_AVAILABLE = False

# ─── Constants ───────────────────────────────────────────────────────────────

VERSION = "0.1.0"

ROOT = Path(__file__).resolve().parent.parent.parent  # .ai/scripts/ -> repo root

MARKER = ".ai-installed.json"
DEFAULT_SOURCE = "template"
DEFAULT_REPO = "I-Synergy/AI"
DEFAULT_REF = "local"
LICENSE = "MIT"

KINDS = ("skill", "agent", "instruction", "prompt", "plugin", "hook")
# AI clients whose platform folders are materialised. A manifest without
# `clients` predates the setting, so it keeps every client it may already have.
CLIENTS = ("claude", "copilot", "reasonix")
MANIFEST_KEYS = {
    "skill": "skills",
    "agent": "agents",
    "instruction": "instructions",
    "prompt": "prompts",
    "plugin": "plugins",
    "hook": "hooks",
}
# Folder name for each kind inside a source. The template source keeps them
# under `.ai/`; an external source (awesome-copilot) has them at its root.
KIND_DIRS = {
    "skill": "skills",
    "agent": "agents",
    "instruction": "instructions",
    "prompt": "prompts",
    "plugin": "plugins",
    "hook": "hooks",
}
# How an installed item is named on disk, per kind.
KIND_SUFFIX = {
    "skill": "",            # a directory
    "agent": ".md",         # a single file
    "instruction": ".instructions.md",
    "prompt": ".prompt.md",
    "plugin": "",           # a directory
    "hook": "",             # a directory
}
# Third-party sources that are offered without being written by hand. `init`
# records them in the manifest so they are visible and pinnable; `enabled: false`
# switches one off.
BUILTIN_SOURCES = {
    # The official Aspire skills are offered before awesome-copilot because both
    # ship a skill named `aspire`; the official one should keep the name.
    "aspire-skills": {
        "repo": "microsoft/aspire-skills",
        "ref": "main",
        "resolve": "remote",
        "license": "MIT",
    },
    "awesome-copilot": {
        "repo": "github/awesome-copilot",
        "ref": "main",
        "resolve": "remote",
        "license": "MIT",
    },
    # MAUI Labs does not keep its skills at the source root: each plugin under
    # `plugins/` owns its own `skills/` tree, and the repository's agents and
    # instructions live under `.github/`. Its two plugins are offered as two
    # sources so an item's origin is visible in its id. Paths are explicit, and
    # the empty lists switch off the kinds that source does not contribute.
    "dotnet-maui": {
        "repo": "dotnet/maui-labs",
        "ref": "main",
        "resolve": "remote",
        "license": "MIT",
        "paths": {
            "skill": ["plugins/dotnet-maui/skills"],
            "agent": [".github/agents"],
            "instruction": [".github/instructions"],
            "plugin": [],
            "hook": [],
        },
    },
    "dotnet-maui-tooling": {
        "repo": "dotnet/maui-labs",
        "ref": "main",
        "resolve": "remote",
        "license": "MIT",
        "paths": {
            "skill": ["plugins/dotnet-maui-tooling/skills"],
            "agent": [],
            "instruction": [],
            "plugin": [],
            "hook": [],
        },
    },
}

STATUS_OK = "ok"
STATUS_NOT_INSTALLED = "not-installed"
STATUS_MISSING = "missing"
STATUS_LOCAL_MODIFIED = "local-modified"
STATUS_UPSTREAM_CHANGED = "upstream-changed"
STATUS_BOTH = "both"
STATUS_REMOVED_UPSTREAM = "removed-upstream"

UPSTREAM_UNCHANGED = "unchanged"


# ─── Paths (derived lazily so tests can point ROOT at a tmp_path) ────────────

def _root() -> Path:
    """The project being operated on.

    `CATALOG_ROOT` lets this script run from *outside* the project it manages —
    which is how the editor extension uses it, so a repository needs no copy of
    these scripts at all.
    """
    override = os.environ.get("CATALOG_ROOT")
    return Path(override).resolve() if override else Path(ROOT)


def _ai_dir() -> Path:
    return _root() / ".ai"


def _manifest_path() -> Path:
    return _ai_dir() / "ai.json"


def _lock_path() -> Path:
    return _ai_dir() / "ai.lock.json"


def catalog_dir() -> Path:
    """The vocabulary's home: `.ai/catalog` in a project, `catalog/` in the template.

    A project keeps everything it owns under `.ai/`; the template repository
    keeps the committed vocabulary at its root (it is the source, not an
    install). Reads prefer whichever exists, so both layouts work.
    """
    ai_catalog = _ai_dir() / "catalog"
    if ai_catalog.is_dir():
        return ai_catalog
    root_catalog = _root() / "catalog"
    if root_catalog.is_dir():
        return root_catalog
    return ai_catalog


def _index_path() -> Path:
    """Where `catalog.py index` writes by default.

    Generated, local-only: the vocabulary under `.ai/catalog/` is committed,
    but the index is a cache. `.ai/tmp/` is gitignored, so rebuilding it never
    dirties the repository.
    """
    return _ai_dir() / "tmp" / "catalog-index.json"


def legacy_index() -> Path | None:
    """An index written to the old committed location, if it is ours.

    Only a payload with our `indexVersion` is removed: a hand-written file at
    that path is left alone.
    """
    path = _ai_dir() / "catalog" / "index.json"
    if not path.is_file():
        return None
    data = _read_json(path)
    if isinstance(data, dict) and data.get("indexVersion") == 1 and "items" in data:
        return path
    return None


def _installed_dir() -> Path:
    return _ai_dir() / "installed"


def _kind_dir(kind: str, root: Path | None = None, source: str = DEFAULT_SOURCE) -> Path:
    """Where a kind's items are installed — always under `.ai/`.

    `.github/` is a mirror produced by sync-platforms.py junctions, never the place
    an install writes to.
    """
    base = Path(root if root is not None else _root()) / ".ai"
    return base / KIND_DIRS[kind]


def _source_kind_dirs(kind: str, source_root: Path, source: str,
                      entry: dict | None = None) -> list:
    """Every directory a kind's items are read from, inside a resolved source.

    The template keeps everything under `.ai/`. An external source may use the
    same layout (a local checkout following this template) or the flat upstream
    layout (`skills/`, `agents/`, `instructions/` …), so prefer `.ai/` when it
    is actually there.

    Some upstream repositories do not keep a kind at one root: MAUI Labs ships
    its skills under `plugins/<plugin>/skills`. Such a source declares
    `paths: { "skill": [...] }` in the manifest, and every listed directory is
    read. Paths are relative to the resolved source root.
    """
    configured = (entry or {}).get("paths")
    if isinstance(configured, dict):
        key = kind if kind in configured else KIND_DIRS[kind]
        if kind in configured or KIND_DIRS[kind] in configured:
            raw = configured.get(key)
            parts = [raw] if isinstance(raw, str) else list(raw or [])
            return [source_root / str(part) for part in parts if str(part).strip()]
    dotted = source_root / ".ai" / KIND_DIRS[kind]
    if source == DEFAULT_SOURCE or dotted.is_dir():
        return [dotted]
    return [source_root / KIND_DIRS[kind]]


def _target_path(kind: str, name: str) -> Path:
    return _kind_dir(kind) / (name + KIND_SUFFIX[kind])


def _slug(item_id: str) -> str:
    return re.sub(r"[^A-Za-z0-9._-]", "_", item_id.replace(":", "__"))


# ─── Small helpers ───────────────────────────────────────────────────────────

def _die(message: str):
    """Report a configuration error and exit with code 2."""
    print(f"ERROR: {message}", file=sys.stderr)
    sys.exit(2)


def _dumps(obj) -> str:
    return json.dumps(obj, indent=2, ensure_ascii=False) + "\n"


def _read_json(path: Path):
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError):
        return None


def _write_json(path: Path, obj) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(_dumps(obj), encoding="utf-8", newline="\n")


def _platform_windows() -> bool:
    """Platform branch, centralised so tests can exercise both sides."""
    return sys.platform == "win32"


def _run_command(cmd: list[str]) -> int:
    """Run a shell command, returning its exit code (0 on failure to launch)."""
    try:
        result = subprocess.run(cmd, capture_output=True, text=True)
    except OSError:
        return 1
    return result.returncode


def _symlink(target: Path, source: Path) -> bool:
    """POSIX branch: relative symlink so a moved checkout keeps working."""
    try:
        rel = os.path.relpath(str(source), str(target.parent))
    except ValueError:  # different drives — fall back to an absolute link
        rel = str(source)
    target.symlink_to(rel, target_is_directory=source.is_dir())
    return True


def _is_link(path: Path) -> bool:
    """True for a symlink or a Windows junction (reparse point)."""
    if path.is_symlink():
        return True
    if not _platform_windows() or not path.exists():
        return False
    try:
        import ctypes
        file_attribute_reparse_point = 0x400
        attrs = ctypes.windll.kernel32.GetFileAttributesW(str(path))
        return attrs != -1 and bool(attrs & file_attribute_reparse_point)
    except Exception:
        return False


def _rmdir(path: Path) -> None:
    """Remove a directory, file, symlink or junction without following links.

    shutil.rmtree() would follow a link and delete the *target* directory, so a
    link is always removed as a link (mirrors sync-platforms.py).
    """
    if _is_link(path):
        if _platform_windows() and path.is_dir() and not path.is_symlink():
            # A junction: rmdir removes the link itself, never the target.
            _run_command(["cmd", "/c", "rmdir", str(path)])
            return
        try:
            path.unlink()
            return
        except OSError:
            pass
    if path.is_dir():
        _force_rmtree(path)
    elif path.exists():
        path.unlink()


def _same_path(a: Path, b: Path) -> bool:
    """True when both paths resolve to the same object on disk."""
    try:
        if a.exists() and b.exists() and os.path.samefile(a, b):
            return True
    except OSError:
        pass
    try:
        return a.resolve() == b.resolve()
    except OSError:
        return os.path.abspath(str(a)) == os.path.abspath(str(b))


# ─── Hashing ─────────────────────────────────────────────────────────────────

def _file_sha(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(65536), b""):
            digest.update(chunk)
    return digest.hexdigest()


def item_files(path: Path) -> list:
    """Every hashable file of an item as (relpath, filepath), sorted by relpath.

    Markers and Python bytecode are excluded so writing a marker never changes
    the hash of the item it describes.
    """
    path = Path(path)
    if path.is_file():
        return [(path.name, path)]
    if not path.is_dir():
        return []
    found = []
    for candidate in path.rglob("*"):
        if not candidate.is_file():
            continue
        rel = candidate.relative_to(path)
        if rel.name == MARKER or "__pycache__" in rel.parts or rel.suffix in (".pyc", ".pyo"):
            continue
        found.append((rel.as_posix(), candidate))
    found.sort(key=lambda pair: pair[0])
    return found


def item_hash(path: Path, name: str | None = None) -> tuple:
    """sha256 over sorted '<relpath>\\0<content-sha256>' lines. Returns (hex, files).

    A local edit is therefore detectable offline: no upstream diff is needed.
    `name` overrides the relative name used for a single-file item, so a target
    installed under a different filename can be compared with its source.
    """
    digest = hashlib.sha256()
    path = Path(path)
    files = item_files(path)
    if name and path.is_file():
        files = [(name, path)]
    for rel, file_path in files:
        digest.update(rel.encode("utf-8"))
        digest.update(b"\x00")
        digest.update(_file_sha(file_path).encode("ascii"))
        digest.update(b"\n")
    return digest.hexdigest(), len(files)


def installed_hash(item: dict, target: Path) -> str:
    """The lock-basis hash of an installed item.

    `item_hash` includes the relative filename, and Copilot agents install as
    `X.md` from `X.agent.md`. Hashing the target under the source's name keeps
    an unmodified install equal to the lock instead of reporting every agent as
    locally modified.
    """
    target = Path(target)
    name = None
    if target.is_file():
        source_path = str(item.get("sourcePath") or item.get("path") or target.name)
        name = Path(source_path).name
    return item_hash(target, name=name)[0]


# ─── Frontmatter ─────────────────────────────────────────────────────────────

_FRONTMATTER_KEY = re.compile(r"^([A-Za-z0-9_-]+)\s*:\s*(.*)$")


def _simple_frontmatter(block: str) -> dict:
    """Fallback parser for name/description when PyYAML is unavailable."""
    result = {}
    lines = block.splitlines()
    index = 0
    while index < len(lines):
        match = _FRONTMATTER_KEY.match(lines[index])
        index += 1
        if not match:
            continue
        key, value = match.group(1), match.group(2).strip()
        if value in (">", ">-", "|", "|-"):
            collected = []
            while index < len(lines) and (lines[index].startswith((" ", "\t"))):
                collected.append(lines[index].strip())
                index += 1
            joiner = "\n" if value.startswith("|") else " "
            result[key] = joiner.join(part for part in collected if part)
        elif len(value) >= 2 and value[0] == value[-1] and value[0] in "\"'":
            result[key] = value[1:-1]
        else:
            result[key] = value
    return result


def frontmatter(text: str) -> dict:
    """Parse the YAML frontmatter block of a SKILL.md / agent.md file."""
    if not text.lstrip("\ufeff").startswith("---"):
        return {}
    stripped = text.lstrip("\ufeff")
    end = stripped.find("\n---", 3)
    if end == -1:
        return {}
    block = stripped[3:end]
    if _YAML_AVAILABLE:
        try:
            data = yaml.safe_load(block)
        except Exception:
            data = None
        if isinstance(data, dict):
            return data
        if data is not None:
            return {}
    return _simple_frontmatter(block)


def _one_line(value) -> str:
    return " ".join(str(value or "").split())


def _name_list(value) -> list:
    """Accept a YAML list or a comma/semicolon separated scalar."""
    if isinstance(value, (list, tuple)):
        return [str(entry).strip() for entry in value if str(entry).strip()]
    if isinstance(value, str):
        parts = value.replace(";", ",").split(",")
        return [part.strip() for part in parts if part.strip()]
    return []


# ─── Ids ─────────────────────────────────────────────────────────────────────

def split_id(raw: str) -> tuple:
    """Split 'source:id' into (source, name); a bare id belongs to 'template'."""
    raw = (raw or "").strip()
    if ":" in raw:
        source, _, name = raw.partition(":")
        return source, name
    return DEFAULT_SOURCE, raw


def make_id(source: str, name: str) -> str:
    return f"{source}:{name}"


def display_id(item_id: str) -> str:
    prefix = DEFAULT_SOURCE + ":"
    return item_id[len(prefix):] if item_id.startswith(prefix) else item_id


def canonical_id(raw: str) -> str:
    """Normalise an id to 'source:name'.

    Any well-formed source namespace is accepted here: whether the source is
    actually declared is the catalog's business, and it reports that far more
    usefully ("unknown id … not in the catalog") than an id parser can.
    """
    source, name = split_id(raw)
    if not name:
        _die(f"empty id after source prefix in '{raw}'")
    return make_id(source, name)


def soft_canonical(raw) -> str | None:
    """Normalise an id without failing — None for an unusable value."""
    source, name = split_id(str(raw))
    if not name:
        return None
    return make_id(source, name)


# ─── Manifest ────────────────────────────────────────────────────────────────

def manifest_schema() -> str:
    """`$schema` as seen from `.ai/ai.json` for the layout in use."""
    root_catalog = _root() / "catalog"
    ai_catalog = _ai_dir() / "catalog"
    if root_catalog.is_dir() and not ai_catalog.is_dir():
        return "../catalog/schemas/ai.schema.json"
    return "catalog/schemas/ai.schema.json"


def default_manifest(repo: str = DEFAULT_REPO, ref: str = DEFAULT_REF) -> dict:
    return {
        "$schema": manifest_schema(),
        "version": 1,
        "sources": {DEFAULT_SOURCE: {"repo": repo, "ref": ref}},
        # New projects start with Claude only; Copilot/Reasonix are opt-in.
        "clients": ["claude"],
        "presets": [],
        "skills": [],
        "agents": [],
        "instructions": [],
        "prompts": [],
        "plugins": [],
        "hooks": [],
        "materialize": "link",
        "exclude": [],
        "locked": [],
    }


def has_local_catalog() -> bool:
    """True when this project carries its own skills/agents/… tree.

    That is what decides whether a repo resolves locally (its own tree is the
    catalog) or remotely (there is nothing here, so the template repository is).
    """
    return any(
        (data_dir := _root() / ".ai" / KIND_DIRS[kind]).is_dir() and any(data_dir.iterdir())
        for kind in KINDS
    )


def local_selection() -> dict:
    """Every item already present under this repository's `.ai/` tree.

    Only meaningful when the source is the tree itself (source == target): there
    the catalog and the installed set are the same files, so an item on disk *is*
    a selected item. Without this a repository full of skills reported an empty
    selection, and the UI said "nothing installed".
    """
    selection = {}
    for kind in KINDS:
        base = _root() / ".ai" / KIND_DIRS[kind]
        names = []
        if base.is_dir():
            if kind in ("agent", "instruction", "prompt"):
                suffix = KIND_SUFFIX[kind]
                names = sorted(entry.name[:-len(suffix)] for entry in base.iterdir()
                               if entry.is_file() and entry.name.endswith(suffix))
            else:
                names = sorted(entry.name for entry in base.iterdir() if entry.is_dir())
        selection[MANIFEST_KEYS[kind]] = names
    return selection


def merge_builtin_sources(manifest: dict) -> list:
    """Add newly offered built-in sources to an existing manifest.

    Built-ins are written once by `init`/the starter manifest; without this,
    a project created before a source was added never sees it. Only sources
    that are missing are added — an existing pin, `enabled: false`, or a
    project override is left exactly as it is.
    """
    if offline():
        return []
    sources = manifest.setdefault("sources", {})
    added = []
    for name, entry in BUILTIN_SOURCES.items():
        if name not in sources:
            sources[name] = dict(entry)
            added.append(name)
    return added


def starter_manifest() -> dict:
    """The manifest a repository is resolved with *before* it has one.

    Same decision `init` makes: a repository that already carries a catalog
    reads *its own* tree; one that carries nothing resolves the template from
    its repository. A local catalog is also the installed set (source ==
    target), so its items are selected; a bare repository starts empty.

    Reading with this manifest writes nothing. The first `add` (or `init`)
    persists it, so `browse`, `status` and `release` stay read-only.

    Third-party built-ins (awesome-copilot) are offered too, exactly as `init`
    and vendoring record them, so a repo with no manifest can select from the
    full catalog. Offline they are left out: they could not be fetched anyway.
    """
    if has_local_catalog():
        data = default_manifest()
        data.update(local_selection())
    else:
        data = default_manifest(repo=DEFAULT_REPO, ref="main")
        data["sources"][DEFAULT_SOURCE]["resolve"] = "remote"
        # A remote source is fetched; linking a file (agents) to the cache is
        # unsafe when the ref is a moving branch, so a fresh project copies.
        data["materialize"] = "copy"
    if not offline():
        for name, entry in BUILTIN_SOURCES.items():
            data["sources"].setdefault(name, dict(entry))
    # A repository without catalog/ has nothing for $schema to point at.
    data.pop("$schema", None)
    return data


# The resolver and vocabulary a repository needs to run the catalog itself:
# platform mirrors, CI, and agents working in a terminal.
# The extension's bundled copy wins over a pinned template's older copy for
# files whose behaviour must match the installed extension (client selection
# and the corrected release validator).
BUNDLED_PREFERRED = {
    ".ai/scripts/sync-platforms.py",
    ".ai/tests/validate-release.py",
}

TOOLING_FILES = (
    ".ai/scripts/catalog.py",
    ".ai/scripts/sync-platforms.py",
    ".ai/scripts/sync-skills.py",
    ".ai/scripts/sync-agents.py",
    ".ai/scripts/upgrade-template.py",
    ".ai/scripts/hygiene-lint.py",
    ".ai/scripts/migrate-to-ai.py",
    # A template-only check the extension can correct even when a project's
    # pinned template predates the fix.
    ".ai/tests/validate-release.py",
    "catalog/presets.json",
    "catalog/categories.json",
    "catalog/schemas/ai.schema.json",
    "catalog/schemas/index.schema.json",
    "catalog/schemas/lock.schema.json",
)


def bundled_tooling_root() -> Path | None:
    """The extension's own tooling copy, when the host supplies one.

    The template may stop shipping a file the extension still bundles (the
    resolver itself, for instance), so a missing template file falls back to
    this instead of leaving the repository without it.
    """
    raw = os.environ.get("CATALOG_TOOLING")
    if not raw:
        return None
    path = Path(raw)
    return path if path.is_dir() else None


def template_tooling_files(source_root: Path) -> list:
    """The template-owned files: the resolver, its sibling scripts and vocabulary."""
    files = list(TOOLING_FILES)
    scripts = source_root / ".ai" / "scripts"
    if scripts.is_dir():
        for entry in sorted(scripts.glob("*.py")):
            rel = str(entry.relative_to(source_root)).replace(os.sep, "/")
            if rel not in files:
                files.append(rel)
    # The validation scripts the agent instructions tell an agent to run
    # (`validate-skills.py`, `run-all-tests.sh`, …).
    tests = source_root / ".ai" / "tests"
    if tests.is_dir():
        for entry in sorted(tests.rglob("*")):
            if (entry.is_file() and "__pycache__" not in entry.parts
                    and entry.suffix != ".pyc"):
                rel = str(entry.relative_to(source_root)).replace(os.sep, "/")
                if rel not in files:
                    files.append(rel)
    return files


def tooling_record_path() -> Path:
    return _ai_dir() / "tooling.json"


def load_tooling_record() -> dict:
    path = tooling_record_path()
    if not path.is_file():
        return {"version": 1, "source": DEFAULT_SOURCE, "files": []}
    data = _read_json(path)
    return data if isinstance(data, dict) else {"version": 1, "source": DEFAULT_SOURCE, "files": []}


def save_tooling_record(record: dict) -> None:
    _write_json(tooling_record_path(), record)


def _recorded_tooling(record: dict) -> dict:
    return {str(entry.get("path")): entry
            for entry in record.get("files") or [] if entry.get("path")}


def tooling_destination(rel: str) -> str:
    """Template layout → project layout: vocabulary moves under `.ai/`."""
    return ".ai/" + rel if rel.startswith("catalog/") else rel


# The supporting `.ai/` tree an agent expects. These are project-owned the
# moment they exist: created once, never overwritten.
TEMPLATE_DIRS = (
    "analysis", "checklists", "completed", "patterns",
    "plans", "progress", "project", "reference",
)
TEMPLATE_FILES = (".ai/session-context.md",)
# Agent-facing configuration. Copilot reads the first two, Claude Code the
# settings (session hook, build env, permissions), and git the attributes.
AGENT_FILES = (
    "AGENTS.md",
    ".github/copilot-instructions.md",
    ".claude/settings.json",
    ".gitattributes",
)


def migrate_catalog_dir() -> bool:
    """Move a project's root `catalog/` into `.ai/catalog/` (one-time fix).

    Older builds wrote the vocabulary to the repository root. A project uses
    the `.ai/` layout; the template repository itself keeps `catalog/` at its
    root because it *is* the source, so only a remote-resolving project moves.
    """
    manifest = load_manifest()
    if not manifest or not is_remote(_source_entry(manifest)):
        return False
    root_catalog = _root() / "catalog"
    ai_catalog = _ai_dir() / "catalog"
    if not root_catalog.is_dir() or ai_catalog.exists():
        return False

    ai_catalog.parent.mkdir(parents=True, exist_ok=True)
    shutil.move(str(root_catalog), str(ai_catalog))
    schema = str(manifest.get("$schema") or "")
    if schema.startswith("../catalog/"):
        manifest["$schema"] = "catalog/" + schema[len("../catalog/"):]
        save_manifest(manifest)

    if tooling_record_path().is_file():
        record = load_tooling_record()
        for entry in record.get("files") or []:
            path = str(entry.get("path") or "")
            if path.startswith("catalog/"):
                entry["path"] = ".ai/" + path
        record["files"] = sorted(record.get("files") or [],
                                 key=lambda entry: str(entry.get("path")))
        save_tooling_record(record)
    return True


def _template_scaffold_files(source_root: Path) -> list:
    """Every supporting `.ai/` file the template ships, in a stable order."""
    files = []
    for name in TEMPLATE_DIRS:
        source_dir = source_root / ".ai" / name
        if not source_dir.is_dir():
            continue
        for source in sorted(source_dir.rglob("*")):
            if (source.is_file() and "__pycache__" not in source.parts
                    and source.suffix != ".pyc"):
                files.append(source)
    for rel in TEMPLATE_FILES:
        source = source_root / rel
        if source.is_file():
            files.append(source)
    return files


def install_scaffold(source_root: Path) -> dict:
    """Copy the template's supporting `.ai/` folders when a project lacks them.

    Plans, progress, patterns, reference, checklists, completed, analysis,
    project and session-context are project-owned from the moment they exist:
    only missing files are written, and nothing is ever overwritten.
    """
    summary = {"copied": [], "skipped": []}
    if _same_path(source_root, _root()):
        return summary

    def copy_file(source: Path) -> None:
        rel = source.relative_to(source_root)
        key = str(rel).replace(os.sep, "/")
        target = _root() / rel
        if target.exists():
            summary["skipped"].append(key)
            return
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(str(source), str(target))
        summary["copied"].append(key)

    for source in _template_scaffold_files(source_root):
        copy_file(source)
    return summary


def install_agent_config(source_root: Path, manifest: dict | None = None) -> dict:
    """Write the missing client-facing files for the selected clients.

    `AGENTS.md` and `.gitattributes` are shared and always written. Claude
    settings and Copilot instructions are written only when that client is
    selected, so disabling a client does not re-create its folder.
    """
    summary = {"copied": [], "skipped": []}
    if _same_path(source_root, _root()):
        return summary
    clients = _clients(manifest or load_manifest() or {})
    for rel in AGENT_FILES:
        if rel == ".claude/settings.json" and "claude" not in clients:
            continue
        if rel == ".github/copilot-instructions.md" and "copilot" not in clients:
            continue
        source = source_root / rel
        if not source.is_file():
            continue
        key = str(rel).replace(os.sep, "/")
        target = _root() / rel
        if target.exists():
            summary["skipped"].append(key)
            continue
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(str(source), str(target))
        summary["copied"].append(key)
    return summary


def seed_tooling_record(source_root: Path) -> None:
    """Record pre-existing template files as template-owned.

    Used when a repository switches from resolving its own tree to a release:
    the scripts it already has are the template's, so a later update may refresh
    them. Files the template does not ship are never recorded, which is what
    keeps project-created scripts out of every future update.
    """
    record = load_tooling_record()
    recorded = _recorded_tooling(record)
    added = False
    # Only the fixed resolver/vocabulary list: a project script that happens to
    # live in `.ai/scripts/` must never be adopted as template-owned.
    for rel in TOOLING_FILES:
        dest = tooling_destination(rel)
        target = _root() / dest
        if target.is_file() and dest not in recorded:
            recorded[dest] = {"path": dest, "sha256": _file_sha(target),
                              "origin": "template"}
            added = True
    if not added:
        return
    record["version"] = 1
    record["source"] = DEFAULT_SOURCE
    record["files"] = sorted(recorded.values(), key=lambda entry: entry["path"])
    save_tooling_record(record)


def install_tooling(source_root: Path, overwrite: bool = False, ref: str | None = None) -> dict:
    """Bring the template's resolver and vocabulary into this repository.

    The extension runs its own bundled copy, but a repository that installs
    anything should be able to run the catalog itself — that is what lets
    `.ai/scripts/sync-platforms.py` create the `.github`/`.claude`/`.reasonix`
    mirrors. A file the template no longer ships falls back to the extension's
    bundled copy, so a missing resolver is still created.

    Ownership is recorded in `.ai/tooling.json`:

    * template-owned — written when missing, and refreshed only when
      `overwrite` is set (the explicit update-to-release path);
    * project-owned — never recorded because the template does not ship that
      path, or recorded and edited since (the hash no longer matches). Those
      are created once and are left alone, always.
    """
    summary = {"copied": [], "skipped": [], "unchanged": [], "missing": [],
               "protected": []}
    if _same_path(source_root, _root()):
        return summary

    bundled = bundled_tooling_root()
    record = load_tooling_record()
    recorded = _recorded_tooling(record)
    changed = False

    for rel in template_tooling_files(source_root):
        dest = tooling_destination(rel)
        target = _root() / dest
        source = source_root / rel
        origin = "template"
        # The extension ships the corrected release validator. Prefer its copy
        # over an older one from the project's pinned template so implementing
        # repositories stop failing a template-only assertion after upgrading.
        if rel in BUNDLED_PREFERRED and bundled is not None:
            fallback = bundled / rel
            if fallback.is_file():
                source = fallback
                origin = "bundled"
        if not source.is_file():
            fallback = bundled / rel if bundled is not None else None
            if fallback is not None and fallback.is_file():
                source = fallback
                origin = "bundled"
            else:
                summary["missing"].append(dest)
                continue

        if target.is_file():
            current = _file_sha(target)
            entry = recorded.get(dest)
            if entry is None:
                # A pre-existing file with a template-owned name (an older
                # checkout): adopt it as template-owned, but never overwrite it
                # during an ordinary sync.
                recorded[dest] = {"path": dest, "sha256": current, "origin": origin}
                changed = True
            elif current != entry.get("sha256"):
                # The project edited a template file; it is theirs now.
                summary["protected"].append(dest)
                continue
            if not overwrite:
                summary["skipped"].append(dest)
                continue
            if _file_sha(source) == current:
                summary["unchanged"].append(dest)
                continue

        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(str(source), str(target))
        recorded[dest] = {"path": dest, "sha256": _file_sha(target), "origin": origin}
        summary["copied"].append(dest)
        changed = True

    if changed or not tooling_record_path().is_file():
        record["version"] = 1
        record["source"] = DEFAULT_SOURCE
        if ref:
            record["ref"] = ref
        record["files"] = sorted(recorded.values(), key=lambda entry: entry["path"])
        save_tooling_record(record)
    return summary


GITIGNORE_ENTRIES = (
    "__pycache__/",
    "*.py[cod]",
    ".pytest_cache/",
    ".ai/tmp/",
    ".claude/skills/",
    ".claude/agents/",
    ".github/skills/",
    ".github/agents/",
    ".github/instructions/",
    ".github/prompts/",
    ".claude/commands/",
    ".claude/instructions/",
    ".reasonix/skills/",
    ".reasonix/agents/",
)


def ensure_gitignore() -> str:
    """Ignore resolver state and the platform junctions.

    `.ai/tmp/` is local state (cloned sources), and the `.claude`/`.github`/
    `.reasonix` skill/agent folders are junctions onto `.ai/` — tracking them
    would duplicate every installed item. Existing lines are preserved.
    """
    path = _root() / ".gitignore"
    text = path.read_text(encoding="utf-8") if path.is_file() else ""
    present = {line.strip().rstrip("/") for line in text.splitlines()
               if line.strip() and not line.strip().startswith("#")}
    missing = [entry for entry in GITIGNORE_ENTRIES
               if entry.rstrip("/") not in present]
    if not missing:
        return "kept"

    block = ("# AI Workspace Manager: resolver cache and platform junctions\n"
             + "\n".join(missing) + "\n")
    if not text:
        path.write_text(block, encoding="utf-8", newline="\n")
        return "created"

    if not text.endswith("\n"):
        text += "\n"
    if not text.endswith("\n\n"):
        text += "\n"
    path.write_text(text + block, encoding="utf-8", newline="\n")
    return "updated"


def sync_platform_mirrors(manifest: dict | None = None) -> None:
    """Run the workspace's mirror script, if it has one.

    Delegated rather than duplicated: the junction rules live in one script.
    `AI_CLIENTS` carries the selection explicitly so a first `sync` on a
    repository that has no manifest yet still defaults to Claude only.
    """
    script = _root() / ".ai" / "scripts" / "sync-platforms.py"
    if not script.is_file():
        # A project that predates the rename.
        script = _root() / ".ai" / "scripts" / "sync-skills.py"
    if script.is_file():
        env = dict(os.environ)
        if manifest is not None:
            env["AI_CLIENTS"] = ",".join(
                name for name in CLIENTS if name in _clients(manifest))
        subprocess.run([sys.executable, str(script)], cwd=str(_root()),
                       capture_output=True, text=True, errors="replace", env=env)


def load_manifest(required: bool = False) -> dict | None:
    path = _manifest_path()
    if not path.is_file():
        if required:
            _die(
                f"no manifest at {path} — select something first: "
                "python .ai/scripts/catalog.py add <id>"
            )
        return None
    data = _read_json(path)
    if not isinstance(data, dict):
        _die(f"manifest {path} is not a JSON object")
    return data


def save_manifest(manifest: dict) -> None:
    _write_json(_manifest_path(), manifest)


def _source_entry(manifest: dict) -> dict:
    sources = manifest.get("sources") or {}
    entry = sources.get(DEFAULT_SOURCE) or {}
    return entry if isinstance(entry, dict) else {}


def _source_root(manifest: dict) -> Path:
    """Where the 'template' source is read from: sources.template.path or ROOT."""
    return _source_root_for(DEFAULT_SOURCE, _source_entry(manifest or {}))


def _source_ref(manifest: dict) -> str:
    return str(_source_entry(manifest or {}).get("ref") or DEFAULT_REF)


def _source_repo(manifest: dict) -> str:
    return str(_source_entry(manifest or {}).get("repo") or DEFAULT_REPO)


def _sources(manifest: dict) -> list:
    """[(name, entry)] for every declared source, the template source first.

    Built-in third-party sources are ordered by their declaration in
    `BUILTIN_SOURCES`, not by their position in the manifest. Two sources can
    offer the same name (`aspire` exists in both `aspire-skills` and
    `awesome-copilot`), and the earlier source keeps it; that decision must not
    depend on the order a manifest happened to be written in. Sources the user
    added themselves keep their manifest order after the built-ins.
    """
    sources = (manifest or {}).get("sources") or {}
    if not isinstance(sources, dict) or not sources:
        return [(DEFAULT_SOURCE, {})]
    builtin_order = {name: index for index, name in enumerate(BUILTIN_SOURCES)}
    ordered = []
    if DEFAULT_SOURCE in sources:
        ordered.append((DEFAULT_SOURCE, sources[DEFAULT_SOURCE] or {}))
    declared = []
    for name, entry in sources.items():
        if name == DEFAULT_SOURCE:
            continue
        entry = entry if isinstance(entry, dict) else {}
        # `"enabled": false` keeps a source declared (so its pin is reviewable)
        # without reading or fetching it.
        if entry.get("enabled") is False:
            continue
        declared.append((name, entry))
    declared.sort(key=lambda pair: (builtin_order.get(pair[0], len(builtin_order)),))
    return ordered + declared


def _source_entry_named(manifest: dict, name: str) -> dict:
    return dict(_sources(manifest)).get(name) or {}


def catalog_cache_dir() -> Path:
    return _root() / ".ai" / "tmp" / "catalog-cache"


def source_cache_dir(repo: str, ref: str) -> Path:
    """Where a remote source's checkout lives, keyed by repository and ref.

    Two source ids can point at the same repository: MAUI Labs offers its two
    plugins as `dotnet-maui` and `dotnet-maui-tooling`. Keying the cache by
    source name would clone that repository twice, so the repository and ref
    are the identity here and both sources share one checkout.

    The readable part is the repository's last path segment; a short digest of
    the full repository URL keeps two different checkouts apart and keeps a
    local `file://` fixture from turning into a path too long for Windows.
    """
    slug = str(repo).rstrip("/").rsplit("/", 1)[-1] or "source"
    digest = hashlib.sha256(f"{repo}@{ref}".encode("utf-8")).hexdigest()[:8]
    return catalog_cache_dir() / f"{_slug(slug)}-{_slug(ref)}-{digest}"


def _hook_install_gate(manifest: dict, selected: list, index_by_key: dict) -> None:
    """Hooks run scripts, so installing one needs an explicit opt-in per source.

    Indexing a source that ships hooks is fine — refusing to *fetch* it would
    hide the rest of its catalog. The gate therefore sits at install time.
    """
    blocked = []
    for kind, item_id in selected:
        if kind != "hook":
            continue
        source = (index_by_key.get((kind, item_id)) or {}).get("source") or DEFAULT_SOURCE
        entry = _source_entry_named(manifest, source)
        if not entry.get("allowHooks"):
            # A source that *is* this tree has nothing to install: the hook is
            # already here, so the gate has nothing to protect.
            if not _same_path(_source_root_for(source, entry), _root()):
                blocked.append((item_id, source))
    if not blocked:
        return
    detail = ", ".join(f"{display_id(item_id)} (source '{source}')"
                       for item_id, source in blocked[:5])
    more = "" if len(blocked) <= 5 else f" and {len(blocked) - 5} more"
    _die(f"refusing to install hooks: {detail}{more} — add \"allowHooks\": true to that "
         f"source in {_manifest_path().name} to accept them")


def _force_rmtree(path: Path) -> None:
    """Remove a tree even when git left read-only files behind (Windows)."""
    def _clear(func, target, _exc):
        try:
            os.chmod(target, 0o700)
            func(target)
        except OSError:
            pass

    if sys.version_info >= (3, 12):
        shutil.rmtree(str(path), onexc=_clear)
    else:  # pragma: no cover - older interpreters
        shutil.rmtree(str(path), onerror=_clear)


def offline() -> bool:
    """`CATALOG_OFFLINE=1` keeps every command off the network.

    The test suite sets it: unit tests use local `file://` fixtures, and a
    built-in third-party source must never be fetched from CI.
    """
    return str(os.environ.get("CATALOG_OFFLINE") or "").strip() not in ("", "0", "false")


def fetch_source(name: str, entry: dict, refresh: bool = False) -> Path:
    """Resolve a remote source to a local checkout at its pinned ref.

    Fetched once per (source, ref) into `.ai/tmp/catalog-cache/` and reused, so
    painting the UI never touches the network. `refresh=True` drops the snapshot
    first — without this a moving branch ref would be cached forever and a
    "check for updates" button would silently lie.
    """
    repo = str(entry.get("repo") or "")
    ref = str(entry.get("ref") or DEFAULT_REF)
    if not repo:
        _die(f"source '{name}' declares neither 'path' nor 'repo'")
    if ref == DEFAULT_REF:
        _die(f"source '{name}' must pin a ref (tag, branch or commit SHA); "
             f"'{DEFAULT_REF}' is reserved for the template source")

    target = source_cache_dir(repo, ref)
    if target.exists() and refresh:
        _force_rmtree(target)
    if target.is_dir() and any(target.iterdir()):
        return target

    url = repo if "://" in repo or repo.startswith("git@") else f"https://github.com/{repo}.git"
    if offline() and not str(url).startswith("file://"):
        _die(f"CATALOG_OFFLINE is set, so source '{name}' ({repo} @ {ref}) was not "
             f"fetched — unset it, or give that source a local \"path\"")
    print(f"fetching {name} @ {ref} ...", file=sys.stderr)
    clone = subprocess.run(
        ["git", "clone", "--quiet", "--depth", "1", "--branch", ref, url, str(target)],
        capture_output=True, text=True, errors="replace",
    )
    if clone.returncode != 0:
        # A bare commit SHA is not a branch: init, fetch the SHA, check it out.
        _force_rmtree(target)
        target.mkdir(parents=True, exist_ok=True)
        steps = (
            ["git", "init", "--quiet"],
            ["git", "remote", "add", "origin", url],
            ["git", "fetch", "--quiet", "--depth", "1", "origin", ref],
            ["git", "checkout", "--quiet", "FETCH_HEAD"],
        )
        for step in steps:
            done = subprocess.run(step, cwd=str(target), capture_output=True,
                                  text=True, errors="replace")
            if done.returncode != 0:
                detail = (done.stderr or done.stdout or "").strip()
                _force_rmtree(target)
                _die(f"could not fetch source '{name}' ({repo} @ {ref}): {detail}")


    return target


def is_remote(entry: dict) -> bool:
    """True when a source resolves by fetching its pinned ref."""
    mode = str((entry or {}).get("resolve") or "local").lower()
    if mode not in ("local", "remote"):
        _die(f"source resolve must be 'local' or 'remote', not {mode!r}")
    return mode == "remote"


# ─── Releases ────────────────────────────────────────────────────────────────
#
# A project pins a source ref, and that pin only moves when someone edits the
# manifest. These helpers answer the one question the UI needs for that:
# "is there a newer release tag than the one I pinned?"

_SEMVER = re.compile(
    r"^v?(\d+)\.(\d+)\.(\d+)"
    r"(?:-([0-9A-Za-z.-]+))?"
    r"(?:\+[0-9A-Za-z.-]+)?$"
)


def parse_version(tag: str) -> tuple | None:
    """`v1.6.0` -> (1, 6, 0, 1, ''), or None when it is not a semantic version.

    A prerelease sorts below its release: `1.6.0-rc1 < 1.6.0`. That is the
    distinction the UI needs — telling someone on v1.6.0 that v1.7.0-rc1 is a
    newer *release* would be wrong.
    """
    match = _SEMVER.match(str(tag or "").strip())
    if not match:
        return None
    major, minor, patch, prerelease = match.groups()
    return (int(major), int(minor), int(patch), 0 if prerelease else 1, prerelease or "")


def release_tags(tags) -> list:
    """The semantic-version tags among `tags`, newest first.

    Branch names (`main`), moving aliases (`latest`) and datestamped builds
    (`nightly`) are not releases and are left out of the ordering.
    """
    parsed = [(parse_version(tag), tag) for tag in tags or []]
    versions = [(key, tag) for key, tag in parsed if key is not None]
    versions.sort(key=lambda pair: pair[0], reverse=True)
    return [tag for _key, tag in versions]


def remote_tags(repo: str) -> tuple:
    """Every tag on `repo`, or `(None, reason)` when they cannot be read.

    `git ls-remote` is read-only: it never clones and never touches the source
    cache, so checking for a release is safe at any time. A failure is returned
    rather than raised — an unreachable GitHub should dim this one hint, not
    take down the whole view.
    """
    if not repo:
        return None, "source declares no repo"
    url = repo if "://" in repo or repo.startswith("git@") else f"https://github.com/{repo}.git"
    if offline() and not url.startswith("file://"):
        return None, "offline"
    try:
        done = subprocess.run(
            ["git", "ls-remote", "--tags", "--refs", url],
            capture_output=True, text=True, errors="replace", timeout=30,
        )
    except (OSError, subprocess.TimeoutExpired) as error:
        return None, str(error)
    if done.returncode != 0:
        detail = (done.stderr or done.stdout or "").strip().splitlines()
        return None, (detail[0] if detail else f"git ls-remote exited {done.returncode}")

    tags = []
    for line in done.stdout.splitlines():
        _sha, _separator, ref = line.partition("\t")
        prefix = "refs/tags/"
        if ref.startswith(prefix):
            tags.append(ref[len(prefix):])
    return tags, None


def release_row(name: str, entry: dict) -> dict:
    """One source's release state, for `catalog.py release --json`."""
    repo = str(entry.get("repo") or "")
    pinned = str(entry.get("ref") or DEFAULT_REF)
    current = parse_version(pinned)
    tags, error = remote_tags(repo)
    releases = release_tags(tags or [])
    latest = releases[0] if releases else None
    # Prefer a stable release: a repo that also publishes prereleases should not
    # advertise one as "the newer release" to someone on a stable pin.
    stable = next((tag for tag in releases if parse_version(tag)[3] == 1), None)
    latest = stable or latest
    newer = bool(
        current is not None
        and latest is not None
        and parse_version(latest) > current
    )
    return {
        "id": name,
        "repo": repo,
        "ref": pinned,
        # False when the pin is a branch or a commit SHA: there is nothing to
        # compare, even if tags exist.
        "pinnedTag": current is not None,
        "current": pinned if current is not None else None,
        "latest": latest,
        "latestIsPrerelease": bool(latest and parse_version(latest)[3] == 0),
        "newer": newer,
        "tags": releases[:20],
        "tagCount": len(releases),
        "checked": error is None,
        "error": error,
    }


def _source_root_for(name: str, entry: dict, refresh: bool = False) -> Path:
    """Local directory a source reads from: a path override, ROOT, or a fetch."""
    raw = entry.get("path")
    if raw:
        candidate = Path(str(raw))
        return (candidate if candidate.is_absolute() else (_root() / candidate)).resolve()
    if is_remote(entry):
        return fetch_source(name, entry, refresh=refresh)
    if name == DEFAULT_SOURCE:
        return _root()
    return fetch_source(name, entry, refresh=refresh)


def _materialize_mode(manifest: dict, override: str | None = None) -> str:
    mode = override or manifest.get("materialize") or "link"
    if mode not in ("link", "copy"):
        _die(f"materialize must be 'link' or 'copy', not {mode!r}")
    return mode


# ─── Index ───────────────────────────────────────────────────────────────────

def _manifest_json(raw: str) -> dict:
    """plugin.json / hooks.json → a frontmatter-shaped dict."""
    try:
        data = json.loads(raw)
    except Exception:
        return {}
    if not isinstance(data, dict):
        return {}
    return {"name": data.get("name") or "", "description": data.get("description") or ""}


def _plugin_members(raw: str) -> list:
    """A plugin bundles its members by path; expose them as dependencies.

    Names only — ids carry no kind, so an agent and a skill with the same name
    resolve to the same id and the closure picks the kind by preference.
    """
    try:
        data = json.loads(raw)
    except Exception:
        return []
    extensions = data.get("extensions") if isinstance(data, dict) else None
    if not isinstance(extensions, dict):
        return []
    names = []
    for block in extensions.values():
        if not isinstance(block, dict):
            continue
        for key in ("agents", "skills"):
            for path in block.get(key) or []:
                if not isinstance(path, str):
                    continue
                cleaned = path.strip().rstrip("/").split("/")[-1]
                for suffix in (".agent.md", ".instructions.md", ".md"):
                    if cleaned.endswith(suffix):
                        cleaned = cleaned[: -len(suffix)]
                        break
                if cleaned:
                    names.append(cleaned)
    return names


def build_index(manifest: dict | None = None, refresh: bool = False) -> dict:
    """Walk the source tree(s) and return the sorted catalog index.

    `refresh` drops cached remote snapshots first, so an explicit "check for
    updates" actually re-reads a moving branch ref.
    """
    manifest = manifest or {}

    items = []
    seen = set()
    materialised = {}
    shadowed: list = []
    for source_name, source_meta in _sources(manifest):
        source_root = _source_root_for(source_name, source_meta, refresh=refresh)
        ref = str(source_meta.get("ref") or DEFAULT_REF)
        repo = str(source_meta.get("repo") or DEFAULT_REPO)
        license_name = str(source_meta.get("license") or LICENSE)
        for kind in KINDS:
            entries = []
            for base in _source_kind_dirs(kind, source_root, source_name, source_meta):
                if base.is_dir():
                    entries.extend(sorted(base.iterdir(), key=lambda item: item.name))
            for entry in entries:
                if entry.name.startswith(".") or entry.name == "__pycache__":
                    continue
                if kind == "skill":
                    if not entry.is_dir():
                        continue
                    document = entry / "SKILL.md"
                    if not document.is_file():
                        continue
                    name = entry.name
                elif kind == "agent":
                    if not entry.is_file() or not entry.name.endswith(".md"):
                        continue
                    if entry.name.endswith((".instructions.md", ".prompt.md")):
                        continue
                    name = entry.name[:-3]
                    if name.endswith(".agent"):
                        name = name[: -len(".agent")]
                    document = entry
                elif kind == "instruction":
                    if not entry.is_file() or not entry.name.endswith(".md"):
                        continue
                    if entry.name.endswith((".agent.md", ".prompt.md")):
                        continue
                    if entry.name.lower() == "readme.md":
                        continue
                    if entry.name.endswith(".instructions.md"):
                        name = entry.name[: -len(".instructions.md")]
                    else:
                        name = entry.name[:-3]
                    document = entry
                elif kind == "prompt":
                    if not entry.is_file() or not entry.name.endswith(".md"):
                        continue
                    if entry.name.endswith((".instructions.md", ".agent.md")):
                        continue
                    if entry.name.lower() == "readme.md":
                        continue
                    if entry.name.endswith(".prompt.md"):
                        name = entry.name[: -len(".prompt.md")]
                    else:
                        name = entry.name[:-3]
                    document = entry
                elif kind == "plugin":
                    if not entry.is_dir():
                        continue
                    document = entry / "plugin.json"
                    if not document.is_file():
                        continue
                    name = entry.name
                else:  # hook
                    if not entry.is_dir():
                        continue
                    document = entry / "hooks.json"
                    if not document.is_file():
                        continue
                    name = entry.name
                # `path` is where the item is *installed* (always under .ai/);
                # `sourcePath` is where it is read from inside its source. They
                # coincide for the template and differ for an upstream catalog.
                try:
                    source_rel = entry.relative_to(source_root).as_posix()
                except ValueError:
                    source_rel = entry.name
                rel = f".ai/{KIND_DIRS[kind]}/{name}{KIND_SUFFIX[kind]}"

                raw = document.read_text(encoding="utf-8", errors="replace")
                if kind in ("plugin", "hook"):
                    meta = _manifest_json(raw)
                else:
                    meta = frontmatter(raw)
                # Agents declare the skills they drive; skills declare the ones
                # they must load; a plugin declares its bundled members.
                if kind == "agent":
                    declared = _name_list(meta.get("skills"))
                elif kind == "plugin":
                    declared = _plugin_members(raw)
                else:
                    declared = _name_list(meta.get("requires"))
                item_id = make_id(source_name, name)
                if (kind, item_id) in seen:
                    _die(f"duplicate catalog id '{item_id}' for kind {kind} at {rel}")
                # Two sources can offer the same name (`refactor` exists in both
                # this template and awesome-copilot). They would install to the
                # same path, so the earlier source — the template — keeps the
                # name and the later one is shadowed. Dropping an item must never
                # be silent: it is recorded here and reported by the caller.
                key = (kind, name)
                previous = materialised.get(key)
                if previous and previous != item_id:
                    shadowed.append({"id": item_id, "by": previous, "path": rel})
                    continue
                materialised[key] = item_id
                seen.add((kind, item_id))
                sha, count = item_hash(entry)
                description = _one_line(meta.get("description"))
                category, tags = classify(source_name, name, meta, description)
                items.append({
                    "id": item_id,
                    "source": source_name,
                    "kind": kind,
                    "name": name,
                    "path": rel,
                    "sourcePath": source_rel,
                    "description": description,
                    "category": category,
                    "tags": tags,
                    "ref": ref,
                    "sha256": sha,
                    "files": count,
                    "license": license_name,
                    "upstream": f"https://github.com/{repo}/tree/{ref}/{source_rel}",
                    "_declared_requires": declared,
                })

    if shadowed:
        shown = ", ".join(entry["id"] for entry in shadowed[:5])
        more = "" if len(shadowed) <= 5 else f" and {len(shadowed) - 5} more"
        print(f"note: {len(shadowed)} item(s) shadowed by a same-named item from an "
              f"earlier source: {shown}{more}", file=sys.stderr)

    items.sort(key=lambda item: (item["id"], item["kind"]))

    # Dependencies are exposed as ids, resolved after the whole walk so the UI
    # and the validator never have to re-parse frontmatter. A dangling reference
    # stays visible here and only becomes fatal when the item is selected — the
    # index is read by status/browse constantly and must not die on bad data.
    ids_by_name = {(entry["kind"], entry["name"]): entry["id"] for entry in items}
    for entry in items:
        declared = entry.pop("_declared_requires", [])
        entry["requires"] = [
            ids_by_name.get(("skill", name), make_id(entry["source"], name))
            for name in declared
        ]

    return {
        "indexVersion": 1,
        "generatedBy": "python .ai/scripts/catalog.py index",
        "shadowed": shadowed,
        "items": items,
    }


def _index_map(index: dict) -> dict:
    return {(item["kind"], item["id"]): item for item in index.get("items") or []}


def _known_ids(index: dict) -> list:
    ids = []
    for item in index.get("items") or []:
        ids.append(display_id(item["id"]))
        ids.append(item["id"])
    return ids


def _load_presets() -> dict:
    """Presets ship with the template: project copy first, then this script's."""
    candidates = (
        _ai_dir() / "catalog" / "presets.json",
        _root() / "catalog" / "presets.json",
        Path(__file__).resolve().parent.parent.parent / "catalog" / "presets.json",
    )
    for candidate in candidates:
        if candidate.is_file():
            data = _read_json(candidate)
            return data if isinstance(data, dict) else {}
    return {}


_CATEGORIES_CACHE: dict = {}


def load_categories() -> dict:
    """The controlled vocabulary, plus per-source rules for items that declare none."""
    if "data" in _CATEGORIES_CACHE:
        return _CATEGORIES_CACHE["data"]
    candidates = (
        _ai_dir() / "catalog" / "categories.json",
        _root() / "catalog" / "categories.json",
        Path(__file__).resolve().parent.parent.parent / "catalog" / "categories.json",
    )
    data: dict = {"categories": [], "sources": {}}
    for candidate in candidates:
        if candidate.is_file():
            loaded = _read_json(candidate)
            if isinstance(loaded, dict):
                data = loaded
            break
    _CATEGORIES_CACHE["data"] = data
    return data


def category_ids() -> list:
    return [str(entry.get("id")) for entry in load_categories().get("categories") or []
            if entry.get("id")]


def classify(source_name: str, name: str, meta: dict, description: str) -> tuple:
    """Resolve one item's (category, tags).

    Our own items declare `category:` and are taken at their word — an unknown id
    is a data error the validator reports, not something to rewrite. A
    third-party source is not classified item by item (guessing from names left a
    fifth of a thousand items in the fallback bucket): everything it contributes
    lands in the single bucket that source is mapped to in categories.json, so
    the tree shows plainly what is ours and what is imported.
    """
    defined = set(category_ids())
    tags = _name_list(meta.get("tags"))

    # A source mapped to a bucket owns every item it contributes. We classify our
    # own catalog, not somebody else's — an upstream item may even carry a
    # `category:` that means something in its own taxonomy.
    rules = (load_categories().get("sources") or {}).get(source_name) or {}
    bucket = str(rules.get("category") or "").strip()
    if bucket:
        return bucket, tags

    declared = str(meta.get("category") or "").strip()
    if declared:
        return declared, [tag for tag in tags if tag != declared]
    # A tag naming a category is a category declaration too.
    for tag in tags:
        if tag in defined:
            return tag, [entry for entry in tags if entry != tag]

    return str(rules.get("default") or "uncategorised"), tags


PRESET_ALIASES = {"full": "Everything"}


def _canonical_preset_name(name) -> str:
    text = str(name).strip()
    return PRESET_ALIASES.get(text, text)


def _preset_names(manifest: dict) -> list:
    """The active presets, in order: legacy `preset` first, then `presets`.

    Presets compose. The old singleton field is still read so existing
    manifests keep working, and it is folded into the array on the first write.
    Names are canonicalised and de-duplicated, so both can coexist during that
    migration without selecting anything twice.
    """
    raw = []
    legacy = manifest.get("preset")
    if legacy:
        raw.append(legacy)
    for name in manifest.get("presets") or []:
        if isinstance(name, str) and name.strip():
            raw.append(name)
    names = []
    for name in raw:
        canonical = _canonical_preset_name(name)
        if canonical not in names:
            names.append(canonical)
    return names


def _known_presets() -> dict:
    return (_load_presets() or {}).get("presets") or {}


def _clients(manifest: dict) -> set:
    """The selected AI clients; a missing field means every legacy client."""
    raw = manifest.get("clients")
    if raw is None:
        return set(CLIENTS)
    if not isinstance(raw, list):
        _die("clients must be a list of 'claude', 'copilot' and/or 'reasonix'")
    names = {str(value).strip().lower() for value in raw if str(value).strip()}
    unknown = sorted(names - set(CLIENTS))
    if unknown:
        _die(f"unknown client(s): {', '.join(unknown)} — "
             f"choose from {', '.join(CLIENTS)}")
    return names


def _locked_ids(manifest: dict) -> set:
    """Selection ids the user froze: update and sync never overwrite them."""
    return {soft_canonical(raw) for raw in manifest.get("locked") or []
            if soft_canonical(raw)}


def _validate_preset_names(names, known: dict) -> list:
    """Canonicalise preset names, dying on an unknown one with a suggestion."""
    canonical = []
    for raw in names:
        name = _canonical_preset_name(raw)
        if name not in known:
            close = difflib.get_close_matches(name, list(known), n=3)
            hint = f" (did you mean: {', '.join(close)})" if close else ""
            _die(f"unknown preset '{name}'{hint} — see catalog/presets.json")
        if name not in canonical:
            canonical.append(name)
    return canonical


def _preset_selection(manifest: dict, index_by_key: dict, strict: bool = True) -> dict:
    """Expand every active preset into one {kind: [id, ...]} selection.

    A preset name is resolved against catalog/presets.json. An entry may be a
    bare id or {"all": true}, which pulls in every catalog item of that kind.
    Presets are additive: their ids are unioned with each other (and later with
    the manifest's explicit lists), never replaced.
    """
    names = _preset_names(manifest)
    if not names:
        return {}

    presets = _known_presets()
    expanded = {kind: [] for kind in KINDS}
    for name in _validate_preset_names(names, presets):
        body = presets[name] if isinstance(presets[name], dict) else {}
        for kind in KINDS:
            ids = expanded[kind]
            for entry in body.get(MANIFEST_KEYS[kind]) or []:
                if isinstance(entry, dict):
                    if not entry.get("all"):
                        _die(f"preset '{name}' has an unsupported entry in "
                             f"{MANIFEST_KEYS[kind]}: {entry!r}")
                    for (item_kind, item_id) in sorted(index_by_key):
                        if item_kind == kind and item_id not in ids:
                            ids.append(item_id)
                    continue
                item_id = canonical_id(entry)
                if strict and (kind, item_id) not in index_by_key:
                    _die(f"preset '{name}' references unknown {kind} '{entry}'")
                if item_id not in ids:
                    ids.append(item_id)
    return expanded


def _selection(manifest: dict, index_by_key: dict, strict: bool = True) -> list:
    """Expand the manifest into an ordered, de-duplicated [(kind, id)] list.

    strict=True fails on an id the catalog no longer offers (add/sync).
    strict=False keeps it, so status can report it as removed-upstream.
    """
    excludes = set()
    for raw in manifest.get("exclude") or []:
        normalised = soft_canonical(raw)
        if normalised:
            excludes.add(normalised)

    preset_ids = _preset_selection(manifest, index_by_key, strict=strict)
    selected = []
    for kind in KINDS:
        for raw in list(preset_ids.get(kind) or []) + list(manifest.get(MANIFEST_KEYS[kind]) or []):
            item_id = canonical_id(raw)
            if (kind, item_id) not in index_by_key and strict:
                other = "agent" if kind == "skill" else "skill"
                if (other, item_id) in index_by_key:
                    _die(
                        f"'{display_id(item_id)}' is an {other}, but it is listed under "
                        f"'{MANIFEST_KEYS[kind]}' in {_manifest_path().name}"
                    )
                _die(
                    f"unknown id '{raw}' — not in the catalog "
                    f"(run 'catalog.py index' if the tree changed)"
                )
            if item_id in excludes:
                continue
            if (kind, item_id) not in selected:
                selected.append((kind, item_id))
    return selected


def _closure(selected: list, index_by_key: dict) -> list:
    """The selection plus everything it requires, dependencies first.

    This is what actually gets installed. The declared set stays separate so
    `remove` can edit the manifest without writing dependencies into it.
    """
    ordered: list = []
    done: set = set()
    visiting: list = []
    missing: dict = {}

    def visit(kind: str, item_id: str, required_by) -> None:
        key = (kind, item_id)
        if key in done:
            return
        if key in visiting:
            chain = " -> ".join(display_id(entry[1]) for entry in visiting + [key])
            _die(f"dependency cycle in the catalog: {chain}")
        item = index_by_key.get(key)
        if item is None:
            if required_by is not None:
                # A dependency the catalog cannot satisfy is only fatal once
                # something actually asks for it.
                missing.setdefault(item_id, set()).add(required_by)
            return
        visiting.append(key)
        for dependency in item.get("requires") or []:
            # ids carry no kind, so prefer a skill and fall back to an agent —
            # that is what an agent's `skills:` means, and a plugin's members can
            # be either.
            visit("skill" if ("skill", dependency) in index_by_key else "agent",
                  dependency, item_id)
        visiting.pop()
        done.add(key)
        ordered.append(key)

    for kind, item_id in selected:
        visit(kind, item_id, None)

    if missing:
        detail = "; ".join(
            f"{display_id(dep)} (required by "
            f"{', '.join(display_id(who) for who in sorted(who_needs))})"
            for dep, who_needs in sorted(missing.items()))
        _die(f"the catalog declares dependencies that do not exist: {detail}")

    return ordered


def _effective_selection(manifest: dict, index_by_key: dict, strict: bool = True) -> list:
    """Manifest → declared selection → closure. What every read path uses."""
    return _closure(_selection(manifest, index_by_key, strict=strict), index_by_key)


# ─── Lock ────────────────────────────────────────────────────────────────────

def load_lock() -> dict:
    path = _lock_path()
    if not path.is_file():
        return {"lockVersion": 1, "resolved": []}
    data = _read_json(path)
    if not isinstance(data, dict):
        _die(f"lock {path} is not a JSON object")
    data.setdefault("lockVersion", 1)
    if not isinstance(data.get("resolved"), list):
        _die(f"lock {path} has no 'resolved' array")
    return data


def _lock_key(entry: dict) -> tuple:
    return (entry.get("kind") or "skill", entry.get("id") or "")


def _lock_sort_key(entry: dict) -> tuple:
    return (entry.get("kind") or "skill", entry.get("id") or "")


def lock_signature(lock: dict) -> str:
    entries = sorted(lock.get("resolved") or [], key=_lock_sort_key)
    return json.dumps(
        {"lockVersion": lock.get("lockVersion", 1), "resolved": entries},
        sort_keys=True,
        ensure_ascii=False,
    )


def save_lock(entries: list) -> None:
    _write_json(
        _lock_path(),
        {"lockVersion": 1, "resolved": sorted(entries, key=_lock_sort_key)},
    )


# ─── Markers ─────────────────────────────────────────────────────────────────

def remove_markers(kind: str, item_id: str) -> list:
    """Delete legacy marker files for one item (kept for prune/upgrade cleanup)."""
    removed = []
    directory = _installed_dir()
    if directory.is_dir():
        for candidate in directory.glob(f"{_slug(item_id)}.*.json"):
            candidate.unlink()
            removed.append(candidate)
    return removed


def legacy_markers(targets) -> list:
    """Marker files left by older versions: the lock already records this state.

    Shared `.ai/installed/` markers are all ours; a marker inside a target only
    counts for that target, so a stray project file with the same name is never
    touched elsewhere.
    """
    found = []
    directory = _installed_dir()
    if directory.is_dir():
        found.extend(sorted(path for path in directory.glob("*.json") if path.is_file()))
    for target in targets:
        target = Path(target)
        # A link resolves into the source tree; never delete a file there.
        if _is_link(target):
            continue
        marker = target / MARKER
        if marker.is_file():
            found.append(marker)
    return found


def remove_legacy_artifacts(targets) -> dict:
    """Delete state files older versions wrote (markers and the old index)."""
    removed = []
    paths = legacy_markers(targets)
    old_index = legacy_index()
    if old_index is not None:
        paths.append(old_index)
    for path in paths:
        try:
            path.unlink()
            removed.append(str(path.relative_to(_root())).replace(os.sep, "/"))
        except OSError:
            pass
    try:
        _installed_dir().rmdir()
    except OSError:
        pass
    return {"removed": removed}


# ─── Materialisation ─────────────────────────────────────────────────────────

def create_link(target: Path, source: Path) -> bool:
    """Per-item junction (Windows) or relative symlink (POSIX).

    Mirrors sync-platforms.py: `mklink /J` needs no administrator rights for
    directories. A single-file item (agents) uses a hardlink first, then a
    symlink, then a copy, because a junction cannot point at a file.
    """
    target.parent.mkdir(parents=True, exist_ok=True)
    if _platform_windows():
        if source.is_dir():
            return _run_command(["cmd", "/c", "mklink", "/J", str(target), str(source)]) == 0
        if _run_command(["cmd", "/c", "mklink", "/H", str(target), str(source)]) == 0:
            return True
        if _run_command(["cmd", "/c", "mklink", str(target), str(source)]) == 0:
            return True
        target.write_bytes(source.read_bytes())
        return True
    try:
        return _symlink(target, source)
    except OSError:
        return False


def materialize_item(kind: str, item: dict, source: Path, target: Path, mode: str) -> bool:
    """Place one item at its target path and write its marker."""
    if target.exists() or _is_link(target):
        _rmdir(target)
    target.parent.mkdir(parents=True, exist_ok=True)
    try:
        if mode == "link":
            if not create_link(target, source):
                print(f"ERROR: could not link {target} -> {source}", file=sys.stderr)
                return False
        elif source.is_dir():
            shutil.copytree(str(source), str(target))
        else:
            shutil.copy2(str(source), str(target))
    except OSError as exc:
        print(f"ERROR: could not materialize {target}: {exc}", file=sys.stderr)
        return False
    return True


def prune_entry(entry: dict) -> tuple:
    """Delete a lock-recorded path. Returns (deleted_path, removed_markers).

    Only paths recorded in the lock and living under .ai/skills/ or .ai/agents/
    are ever touched — everything else is refused.
    """
    kind = entry.get("kind") or "skill"
    item_id = entry.get("id") or ""
    rel = Path(entry.get("path") or "").as_posix()
    if not (rel.startswith(".ai/skills/") or rel.startswith(".ai/agents/")):
        return (None, [])
    target = _root() / rel
    deleted = None
    if target.exists() or _is_link(target):
        _rmdir(target)
        deleted = target
    # A copy-mode marker inside the item died with the directory; link-mode and
    # file markers live centrally and are removed here.
    markers = remove_markers(kind, item_id)
    if (target / MARKER).is_file():  # a copy-mode item whose directory survived
        (target / MARKER).unlink()
        markers.append(target / MARKER)
    return (deleted, markers)


# ─── Status evaluation ───────────────────────────────────────────────────────

def upstream_changed(item: dict | None, entry: dict | None) -> bool:
    """True when the catalog content differs from what the lock recorded.

    The index is built from the resolved source (a fetched snapshot when the
    source is `resolve: remote`), so comparing its hash with the locked hash is
    exactly "has upstream moved since we installed this". With a `local` source
    that reads the project's own tree, caller `same_dir` already short-circuits
    before this is consulted.
    """
    if not item or not entry:
        return False
    return bool(item.get("sha256")) and item.get("sha256") != entry.get("sha256")


def evaluate_status(kind: str, item: dict | None, target: Path, entry: dict | None,
                    same_dir: bool) -> str:
    """Status of one selected item, as one of the documented enum values."""
    if item is None:
        return STATUS_REMOVED_UPSTREAM
    if same_dir:
        return STATUS_OK
    exists = target.exists() or _is_link(target)
    if not exists:
        return STATUS_NOT_INSTALLED if entry is None else STATUS_MISSING
    if entry is None:
        return STATUS_NOT_INSTALLED
    current = installed_hash(item, target)
    local = current != entry.get("sha256")
    upstream = upstream_changed(item, entry)
    if local and upstream:
        return STATUS_BOTH
    if local:
        return STATUS_LOCAL_MODIFIED
    if upstream:
        return STATUS_UPSTREAM_CHANGED
    return STATUS_OK


# ─── Commands: index ─────────────────────────────────────────────────────────

def render_compact(index: dict, installed: int | None = None) -> str:
    """One line per available item: cheap to read, cheap to put in a prompt.

    Discovery is the budget that matters — an agent should search a small index
    file rather than be handed every skill description up front.
    """
    items = index.get("items") or []
    lines = [
        "# Catalog index",
        "",
        "Generated by `python .ai/scripts/catalog.py index --compact`. Do not edit.",
        f"{len(items)} item(s) available in the catalog"
        + (f"; this project installs {installed}" if installed is not None else "")
        + ". Add one with `catalog.py add <id>`, then `catalog.py sync`.",
        "",
    ]
    for item in items:
        description = " ".join(str(item.get("description") or "").split())
        if len(description) > 140:
            description = description[:137].rstrip() + "..."
        lines.append(f"- `{display_id(item['id'])}` ({item['kind']}) — {description}")
    return "\n".join(lines) + "\n"


def cmd_index(args) -> int:
    manifest = load_manifest() or {}
    index = build_index(manifest)
    out = Path(args.out) if args.out else _index_path()
    if not out.is_absolute():
        out = _root() / out

    existing = _read_json(out) if out.is_file() else None
    stale = existing != index

    compact_target = None
    compact_stale = False
    if args.compact:
        compact_target = Path(args.compact_out) if args.compact_out else out.with_name("INDEX.md")
        if not compact_target.is_absolute():
            compact_target = _root() / compact_target
        try:
            installed = len(_effective_selection(manifest, _index_map(index), strict=False))
        except SystemExit:  # a broken manifest must not block discovery
            installed = None
        compact_text = render_compact(index, installed)
        compact_stale = (not compact_target.is_file()
                         or compact_target.read_text(encoding="utf-8") != compact_text)

    if args.verify:
        if stale or compact_stale:
            message = f"index is stale: {out} (run 'python .ai/scripts/catalog.py index')"
            if args.json:
                print(_dumps({"command": "index", "stale": True, "path": str(out),
                              "items": len(index["items"])}))
            print(f"STALE: {message}", file=sys.stderr)
            return 1
        if args.json:
            print(_dumps({"command": "index", "stale": False, "path": str(out),
                          "items": len(index["items"])}))
        else:
            print(f"index is up to date: {out} ({len(index['items'])} item(s))")
        return 0

    if stale:
        _write_json(out, index)
    if compact_target is not None and compact_stale:
        compact_target.parent.mkdir(parents=True, exist_ok=True)
        compact_target.write_text(compact_text, encoding="utf-8")
    if args.json:
        print(_dumps({"command": "index", "written": stale, "path": str(out),
                      "items": len(index["items"]),
                      "compact": str(compact_target) if compact_target else None}))
    else:
        verb = "wrote" if stale else "unchanged"
        print(f"{verb}: {out} ({len(index['items'])} item(s))")
        if compact_target is not None:
            print(f"compact: {compact_target}")
    return 0


# ─── Commands: add / remove ──────────────────────────────────────────────────

def _resolve_add_targets(raw_ids: list, kind_flag: str | None, index_by_key: dict) -> list:
    """Turn CLI ids into [(kind, id)], failing on unknown or ambiguous ids."""
    resolved = []
    problems = []
    for raw in raw_ids:
        item_id = canonical_id(raw)
        matches = [kind for kind in KINDS if (kind, item_id) in index_by_key]
        if kind_flag:
            if (kind_flag, item_id) not in index_by_key:
                if matches:
                    problems.append(
                        f"'{display_id(item_id)}' is not a {kind_flag} (it is a {matches[0]})"
                    )
                else:
                    problems.append(_unknown_message(raw, index_by_key))
                continue
            resolution = kind_flag
        elif not matches:
            problems.append(_unknown_message(raw, index_by_key))
            continue
        elif len(matches) > 1:
            problems.append(
                f"'{display_id(item_id)}' exists as both a skill and an agent — "
                f"pass --kind skill or --kind agent"
            )
            continue
        else:
            resolution = matches[0]
        if (resolution, item_id) not in resolved:
            resolved.append((resolution, item_id))
    return resolved, problems


def _unknown_message(raw: str, index_by_key: dict) -> str:
    """Unknown-id error text, with the closest catalog names as suggestions."""
    import difflib
    wanted = soft_canonical(raw) or str(raw)
    bare = display_id(wanted)
    names = sorted({display_id(item["id"]) for item in index_by_key.values()})
    hints = difflib.get_close_matches(bare, names, n=3, cutoff=0.5)
    suffix = f" — did you mean: {', '.join(hints)}" if hints else ""
    return f"unknown id '{raw}': not in the catalog{suffix}"


def cmd_add(args) -> int:
    existing = load_manifest()
    manifest = existing or starter_manifest()
    index = build_index(manifest)
    index_by_key = _index_map(index)

    resolved, problems = _resolve_add_targets(args.ids, args.kind, index_by_key)
    if problems:
        for problem in problems:
            print(f"ERROR: {problem}", file=sys.stderr)
        return 2

    added = []
    for kind, item_id in resolved:
        key = MANIFEST_KEYS[kind]
        selection = manifest.setdefault(key, [])
        if not isinstance(selection, list):
            _die(f"manifest key '{key}' must be a list")
        bare = display_id(item_id)
        if bare not in selection and item_id not in selection:
            selection.append(bare)
            added.append({"id": item_id, "kind": kind})
        excludes = manifest.get("exclude")
        if isinstance(excludes, list):
            manifest["exclude"] = [
                value for value in excludes
                if soft_canonical(value) != item_id
            ]

    if added or existing is None:
        save_manifest(manifest)

    if args.json:
        print(_dumps({"command": "add", "added": added, "manifest": str(_manifest_path())}))
    else:
        if added:
            print(f"added {len(added)} item(s) to {_manifest_path()}:")
            for entry in added:
                print(f"  + {entry['kind']:<6} {display_id(entry['id'])}")
            print("run 'python .ai/scripts/catalog.py sync' to materialise them")
        else:
            print("nothing to add — already selected")
    return 0


def cmd_remove(args) -> int:
    manifest = load_manifest()
    if manifest is None:
        _die(f"no manifest at {_manifest_path()} — nothing to remove")

    removed = []
    for raw in args.ids:
        item_id = canonical_id(raw)
        for kind in KINDS:
            if args.kind and kind != args.kind:
                continue
            key = MANIFEST_KEYS[kind]
            selection = manifest.get(key) or []
            kept = [
                value for value in selection
                if soft_canonical(value) != item_id
            ]
            if len(kept) != len(selection):
                manifest[key] = kept
                removed.append({"id": item_id, "kind": kind, "via": "list"})

    # A preset can contribute an id the manifest never lists. Opting one out
    # is what `exclude` is for, so removing a preset-declared item records the
    # exclusion instead of failing with "not declared". Items that are only
    # present as a dependency keep the old, safer answer: remove the dependent.
    index_by_key = _index_map(build_index(manifest))
    declared = {
        (kind, item_id)
        for kind in KINDS
        for item_id in (_preset_selection(manifest, index_by_key, strict=False).get(kind) or [])
    }
    selected = _selection(manifest, index_by_key, strict=False)
    for raw in args.ids:
        item_id = canonical_id(raw)
        match = next(((kind, item_id) for kind, _id in selected
                      if _id == item_id and (kind, item_id) in declared), None)
        if match is None or (args.kind and match[0] != args.kind):
            continue
        kind = match[0]
        excludes = manifest.get("exclude")
        if not isinstance(excludes, list):
            excludes = []
        if all(soft_canonical(value) != item_id for value in excludes):
            excludes.append(display_id(item_id))
            manifest["exclude"] = excludes
            if not any(entry["id"] == item_id for entry in removed):
                removed.append({"id": item_id, "kind": kind, "via": "exclude"})

    if not removed:
        # The id may exist only as a dependency someone else pulled in: that is
        # not a manifest entry, so say why instead of "nothing to remove".
        installed = {item_id for _kind, item_id
                     in _closure(
                         _selection(manifest, index_by_key, strict=False), index_by_key)}
        requested = [canonical_id(raw) for raw in args.ids]
        pulled = [item_id for item_id in requested if item_id in installed]
        if pulled and not args.force:
            names = ", ".join(f"'{display_id(item_id)}'" for item_id in pulled)
            _die(f"{names} is not declared in {_manifest_path().name} — it is "
                 f"installed because something else requires it. Remove the "
                 f"dependent instead.")

    # Block orphaning: if something still selected requires one of these, the
    # closure would put it straight back on the next sync, so say that here
    # rather than reporting a removal that does not stick. An exclusion is not
    # put back by the closure, so check requirements directly for that case.
    if removed and not args.force:
        remaining = _closure(
            _selection(manifest, index_by_key, strict=False), index_by_key)
        required_by: dict = {}
        for kind, item_id in remaining:
            item = index_by_key.get((kind, item_id)) or {}
            for dependency in item.get("requires") or []:
                required_by.setdefault(("skill", dependency), []).append(item_id)
        blocked = [
            (entry, required_by.get((entry["kind"], entry["id"]), []))
            for entry in removed
            if (entry["kind"], entry["id"]) in remaining
            or required_by.get((entry["kind"], entry["id"]))
        ]
        if blocked:
            detail = "; ".join(
                f"{display_id(entry['id'])} ({entry['kind']}) is required by "
                f"{', '.join(display_id(who) for who in sorted(set(who))) or 'another selected item'}"
                for entry, who in blocked)
            forced = ("the dependent will be installed without it"
                      if any(entry.get("via") == "exclude" for entry, _who in blocked)
                      else "a later sync will restore it")
            _die(f"refusing to orphan — {detail}. Remove the dependent first, "
                 f"or pass --force to edit the manifest anyway ({forced})")

    if removed:
        # A lock on an item that is no longer selected has nothing to protect.
        locks = manifest.get("locked")
        if isinstance(locks, list):
            dropped = {entry["id"] for entry in removed}
            manifest["locked"] = [
                value for value in locks if soft_canonical(value) not in dropped
            ]
        save_manifest(manifest)

    pruned = []
    if args.prune:
        lock = load_lock()
        remaining = []
        drop = {(entry["kind"], entry["id"]) for entry in removed}
        for entry in lock.get("resolved") or []:
            if _lock_key(entry) in drop:
                deleted, markers = prune_entry(entry)
                pruned.append({
                    "id": entry.get("id"),
                    "kind": entry.get("kind") or "skill",
                    "path": entry.get("path"),
                    "deleted": str(deleted) if deleted else None,
                    "markers": len(markers),
                })
                continue
            remaining.append(entry)
        if len(remaining) != len(lock.get("resolved") or []):
            save_lock(remaining)

    if args.json:
        print(_dumps({"command": "remove", "removed": removed, "pruned": pruned}))
        return 0

    if not removed:
        print("nothing to remove — no such selection in the manifest")
        return 0
    for entry in removed:
        print(f"  - {entry['kind']:<6} {display_id(entry['id'])}")
    if args.prune:
        for entry in pruned:
            state = "deleted" if entry["deleted"] else "not on disk"
            print(f"  pruned {entry['path']} ({state})")
        if not pruned:
            print("  nothing lock-recorded to prune")
    else:
        print("installed files were left alone: add --prune to delete lock-recorded paths")
    return 0


def _edit_locks(args, adding: bool) -> int:
    """Freeze or thaw selected items in the manifest.

    A lock is a user decision, not a detected state: `update` and `sync` never
    overwrite a locked item that is already on disk, `--force` included. It is
    how an item you edited stops being treated as something to fix.
    """
    manifest = load_manifest()
    if manifest is None:
        _die(f"no manifest at {_manifest_path()} — nothing to lock")

    index_by_key = _index_map(build_index(manifest))
    selected = {item_id for _kind, item_id
                in _effective_selection(manifest, index_by_key, strict=False)}
    entries = manifest.get("locked")
    if not isinstance(entries, list):
        entries = []
    current = {soft_canonical(value) for value in entries}
    changed = []

    for raw in args.ids:
        item_id = canonical_id(raw)
        if adding:
            if item_id not in selected:
                _die(f"'{display_id(item_id)}' is not selected — lock an item "
                     f"this project installs")
            if item_id in current:
                continue
            entries.append(display_id(item_id))
            current.add(item_id)
            changed.append(item_id)
        else:
            kept = [value for value in entries
                    if soft_canonical(value) != item_id]
            if len(kept) != len(entries):
                entries = kept
                current.discard(item_id)
                changed.append(item_id)

    manifest["locked"] = entries
    save_manifest(manifest)

    if args.json:
        print(_dumps({"command": "lock" if adding else "unlock",
                      "locked": [display_id(soft_canonical(value)) for value in entries],
                      "changed": [display_id(item_id) for item_id in changed]}))
        return 0

    verb = "locked" if adding else "unlocked"
    if changed:
        print(f"{verb}: {', '.join(display_id(item_id) for item_id in changed)}")
    else:
        print(f"nothing to {('lock' if adding else 'unlock')} — "
              f"{'already locked' if adding else 'not locked'}")
    if adding:
        print("locked items are never overwritten by update or sync; "
              "unlock to resume updates")
    else:
        print("unlocked items follow the catalog again on the next update")
    return 0


def cmd_lock(args) -> int:
    return _edit_locks(args, True)


def cmd_unlock(args) -> int:
    return _edit_locks(args, False)


# ─── Commands: list ──────────────────────────────────────────────────────────

# ─── Commands: browse / presets / init / update ──────────────────────────────

def _preset_cards(index_by_key: dict) -> list:
    """Every preset with the counts it actually resolves to.

    Counts go through the dependency closure, not the declared list: a preset
    that names one agent installs that agent's skills too, and a card that said
    "1 item" would be lying.
    """
    presets = (_load_presets() or {}).get("presets") or {}
    cards = []
    # File order, not alphabetical: presets.json is the curated reading order.
    for name in presets:
        body = presets[name] if isinstance(presets[name], dict) else {}
        declared = _preset_selection({"preset": name}, index_by_key, strict=False)
        try:
            resolved = _closure(
                _selection({"preset": name}, index_by_key, strict=False), index_by_key)
        except SystemExit:
            resolved = []  # a broken preset is reported by the validator, not here
        if resolved:
            counts = {kind: sum(1 for item_kind, _id in resolved if item_kind == kind)
                      for kind in KINDS}
            ids = [item_id for _kind, item_id in resolved]
        else:
            counts = {kind: len(declared.get(kind) or []) for kind in KINDS}
            ids = [item_id for kind in KINDS for item_id in (declared.get(kind) or [])]
        cards.append({
            "name": name,
            "description": str(body.get("description") or ""),
            "skills": counts["skill"],
            "agents": counts["agent"],
            # The resolved ids let the UI say exactly what applying this preset
            # would add, instead of comparing counts that may hide a swap.
            "ids": ids,
            "recommended": bool(body.get("recommended")),
        })
    return cards


def _source_rows(manifest: dict) -> list:
    rows = []
    for name, entry in _sources(manifest):
        rows.append({
            "id": name,
            "repo": str(entry.get("repo") or DEFAULT_REPO),
            "ref": str(entry.get("ref") or DEFAULT_REF),
            "resolve": "remote" if is_remote(entry) else "local",
            "license": str(entry.get("license") or LICENSE),
            "allowHooks": bool(entry.get("allowHooks")),
            "path": entry.get("path"),
        })
    return rows


def _make_entry(kind: str, item: dict, mode: str) -> dict:
    return {
        "id": item["id"],
        "kind": kind,
        "source": item.get("source") or DEFAULT_SOURCE,
        "ref": item.get("ref") or DEFAULT_REF,
        "path": item["path"],
        "sha256": item["sha256"],
        "files": item["files"],
        "license": item.get("license") or LICENSE,
        "upstream": item.get("upstream") or "",
        "materialize": mode,
    }


def _category_rows(items: list) -> list:
    """The vocabulary in declared order, with live counts, plus any strays.

    Groups that hold at least one item of our own (the template source) come
    before buckets that only hold imported items. Without that, a repository
    whose own items carry no `category:` would list ~450 imported Copilot
    entries above its own 89 — the imported catalog is the guest here.
    """
    vocabulary = load_categories().get("categories") or []
    counts: dict = {}
    for item in items:
        counts[item["category"]] = counts.get(item["category"], 0) + 1

    rows = []
    for entry in vocabulary:
        category_id = str(entry.get("id"))
        rows.append({
            "id": category_id,
            "label": str(entry.get("label") or category_id),
            "description": str(entry.get("description") or ""),
            "count": counts.pop(category_id, 0),
        })
    # A category that is not in the vocabulary still has to appear somewhere:
    # hiding it would make items vanish from the tree.
    for stray, count in sorted(counts.items()):
        rows.append({"id": stray, "label": stray, "description": "", "count": count})

    own = {item["category"] for item in items
           if (item.get("source") or DEFAULT_SOURCE) == DEFAULT_SOURCE}
    return ([row for row in rows if row["id"] in own]
            + [row for row in rows if row["id"] not in own])


def cmd_browse(args) -> int:
    """One payload that paints the whole UI: catalog + selection + state."""
    manifest = load_manifest() or starter_manifest()
    index = build_index(manifest, refresh=bool(getattr(args, "refresh", False)))
    index_by_key = _index_map(index)
    selected = _effective_selection(manifest, index_by_key, strict=False)
    selected_keys = set(selected)
    sources = dict(_sources(manifest))
    roots = {name: _source_root_for(name, entry) for name, entry in sources.items()}
    lock = load_lock()
    locked = {_lock_key(entry): entry for entry in lock.get("resolved") or []}
    frozen = _locked_ids(manifest)
    mode = _materialize_mode(manifest)
    template_root = roots.get(DEFAULT_SOURCE, _root())

    items = []
    # Keyed by (kind, id): a skill and an agent may share a name, so an id alone
    # would make an agent look like it is required by itself.
    required_by: dict = {}
    for entry in index.get("items") or []:
        for dependency in entry.get("requires") or []:
            required_by.setdefault(("skill", dependency), []).append(entry["id"])

    summary = {"available": len(index.get("items") or []), "selected": len(selected),
               "installed": 0, "drift": 0, "updates": 0, "conflicts": 0}
    for item in index.get("items") or []:
        kind = item["kind"]
        item_id = item["id"]
        key = (kind, item_id)
        source_name = item.get("source") or DEFAULT_SOURCE
        source_root = roots.get(source_name, _root())
        same_dir = _same_path(source_root, _root())
        target = _target_path(kind, item["name"])
        entry = locked.get(key)
        is_selected = key in selected_keys

        if is_selected:
            state = evaluate_status(kind, item, target, entry, same_dir)
            if item_id in frozen and state in ("upstream-changed", "local-modified", "both"):
                # A lock says the local content is intentional; it is not drift.
                state = "locked"
        else:
            state = "available"
        exists = target.exists() or _is_link(target)
        local_modified = bool(entry) and exists and installed_hash(item, target) != entry.get("sha256")
        moved = bool(entry) and upstream_changed(item, entry)

        if state in ("ok", "locked"):
            summary["installed"] += 1
        elif state == "available":
            pass
        else:
            summary["drift"] += 1
        if state == "upstream-changed":
            summary["updates"] += 1
        if state == "both":
            summary["conflicts"] += 1

        items.append({
            "id": item_id,
            "displayId": display_id(item_id),
            "kind": kind,
            "name": item["name"],
            "path": item["path"],
            "description": item.get("description") or "",
            "category": item.get("category") or "uncategorised",
            "tags": item.get("tags") or [],
            "source": source_name,
            "ref": item.get("ref") or DEFAULT_REF,
            "license": item.get("license") or LICENSE,
            "upstream": item.get("upstream") or "",
            "sha256": item.get("sha256") or "",
            "files": item.get("files") or 0,
            "requires": item.get("requires") or [],
            "requiredBy": sorted(required_by.get((kind, item_id), [])),
            "selected": is_selected,
            "status": state,
            "installed": exists and is_selected,
            "locked": item_id in frozen,
            "localModified": local_modified,
            "upstreamChanged": moved,
        })

    payload = {
        "command": "browse",
        "source": {
            "id": DEFAULT_SOURCE,
            "root": str(template_root),
            "ref": _source_ref(manifest),
            "sameDir": _same_path(template_root, _root()),
            "remote": any(is_remote(entry) for _name, entry in _sources(manifest)),
        },
        "materialize": mode,
        "clients": [name for name in CLIENTS if name in _clients(manifest)],
        "summary": summary,
        "sources": _source_rows(manifest),
        "presets": _preset_cards(index_by_key),
        "categories": _category_rows(items),
        "items": items,
    }

    if args.json:
        print(_dumps(payload))
        return 0

    print(f"{summary['available']} available; {summary['selected']} selected; "
          f"{summary['drift']} need attention; {summary['updates']} update(s)")
    return 0


def cmd_preset(args) -> int:
    """Add or remove presets without touching the rest of the selection.

    `init --preset` replaces the selection: it is the bootstrap command. This
    one composes presets. Adding keeps every explicit item and every other
    preset; removing drops only that preset's contribution. Neither deletes
    files from disk — deselected items stay until `sync --prune` removes them.
    """
    adding = args.preset_action == "add"
    existing = load_manifest()
    if existing is None:
        if not adding:
            _die(f"no manifest at {_manifest_path()} — nothing to remove")
        manifest = starter_manifest()
    else:
        manifest = existing

    names = _preset_names(manifest)
    requested = _validate_preset_names(args.names, _known_presets())
    changed = []
    if adding:
        for name in requested:
            if name not in names:
                names.append(name)
                changed.append(name)
    else:
        for name in requested:
            if name in names:
                names.remove(name)
                changed.append(name)

    # Fold the legacy singleton into the additive list on the first write.
    manifest["presets"] = names
    manifest.pop("preset", None)
    save_manifest(manifest)

    if args.json:
        print(_dumps({"command": "preset", "action": args.preset_action,
                      "changed": changed, "presets": names,
                      "manifest": str(_manifest_path())}))
        return 0
    if adding:
        if changed:
            print(f"added preset(s): {', '.join(changed)}")
        else:
            print("nothing to add — already applied")
    else:
        if changed:
            print(f"removed preset(s): {', '.join(changed)}")
        else:
            print("nothing to remove — not applied")
        if changed:
            print("installed files were left alone: add --prune to the next sync "
                  "to delete lock-recorded paths")
    print(f"presets: {', '.join(names) or '(none)'}")
    if adding:
        print("run 'python .ai/scripts/catalog.py sync' to materialise the added items")
    return 0


def cmd_presets(args) -> int:
    manifest = load_manifest() or starter_manifest()
    index_by_key = _index_map(build_index(manifest))
    cards = _preset_cards(index_by_key)
    active = _preset_names(manifest)
    # `current` is the old singleton shape, kept for callers that only need
    # one name; `active` is the full additive list.
    current = active[0] if active else None

    if args.json:
        print(_dumps({"command": "presets", "current": current, "active": active,
                      "presets": cards}))
        return 0
    for card in cards:
        marker = "*" if card["name"] in active else " "
        print(f"{marker} {card['name']:<12} {card['skills']:>3} skill(s) "
              f"{card['agents']:>2} agent(s)  {card['description']}")
    return 0


def cmd_pin(args) -> int:
    """Point the template source at a release tag, keeping what is installed.

    This edits `.ai/ai.json` only. It rewrites the effective selection as an
    explicit list (so a `preset` cannot suddenly expand the set), switches the
    source to a remote fetch of the tag, and leaves the materialisation to
    `update --apply` — which is what actually replaces the files.
    """
    existing = load_manifest()
    manifest = existing or starter_manifest()
    entry = dict((manifest.get("sources") or {}).get(DEFAULT_SOURCE) or {})

    tag = args.release
    if not tag:
        row = release_row(DEFAULT_SOURCE, entry)
        tag = row.get("latest")
        if not tag:
            detail = row.get("error") or "no semantic-version tags were found"
            _die(f"no release to pin {DEFAULT_SOURCE} to: {detail}")

    # Freeze the current selection before the source moves: the new ref may
    # offer a different catalog, and a preset would expand against it.
    index = build_index(manifest)
    index_by_key = _index_map(index)
    selected = _effective_selection(manifest, index_by_key, strict=False)
    # Decide before the source moves: is the old catalog this very tree?
    was_local_tree = _same_path(_source_root(manifest), _root())
    lists = {key: [] for key in MANIFEST_KEYS.values()}
    for kind, item_id in selected:
        item = index_by_key.get((kind, item_id)) or {}
        if (item.get("source") or DEFAULT_SOURCE) != DEFAULT_SOURCE:
            continue
        lists[MANIFEST_KEYS[kind]].append(display_id(item_id))

    sources = manifest.setdefault("sources", {})
    entry["repo"] = args.repo or entry.get("repo") or DEFAULT_REPO
    entry["ref"] = tag
    entry["resolve"] = "remote"
    entry.pop("path", None)
    sources[DEFAULT_SOURCE] = entry
    manifest.update(lists)
    # Freeze the selection: pinning rewrites it as explicit ids, so no preset
    # may expand (or be added) against a catalog that just changed.
    manifest.pop("preset", None)
    manifest.pop("presets", None)
    # When the old source was this tree, its items are real files in the
    # repository. Linking them to the fetch cache would move the content out of
    # version control, so such a tree is copied instead.
    if was_local_tree:
        manifest["materialize"] = "copy"
    save_manifest(manifest)

    # Record the scripts this tree already carries as template-owned, so the
    # update-to-release step may refresh them. Nothing else is recorded: files
    # the template does not ship stay project-created, once.
    if was_local_tree:
        seed_tooling_record(_root())

    if args.json:
        print(_dumps({
            "command": "pin",
            "source": DEFAULT_SOURCE,
            "repo": entry["repo"],
            "ref": tag,
            "selected": sum(len(names) for names in lists.values()),
            "manifest": str(_manifest_path()),
        }))
    else:
        print(f"pinned {DEFAULT_SOURCE} to {tag} ({entry['repo']}) — "
              f"{sum(len(names) for names in lists.values())} selected item(s)")
        print("run 'python .ai/scripts/catalog.py update --apply' to replace them")
    return 0


def cmd_adopt(args) -> int:
    """Write `.ai/ai.json` describing what this repository already has.

    Persists the same implicit manifest `browse` paints — the repository's own
    items when it has a catalog, otherwise the template source — so the state is
    explicit and reviewable. Nothing is materialised and an existing manifest is
    never overwritten.
    """
    path = _manifest_path()
    if path.is_file():
        existing = load_manifest() or {}
        selected = sum(len(existing.get(key) or []) for key in MANIFEST_KEYS.values())
        if args.json:
            print(_dumps({"command": "adopt", "created": False,
                          "path": str(path), "selected": selected}))
        else:
            print(f"kept: {path} already exists")
        return 0

    manifest = starter_manifest()
    # A repository that already holds files keeps them as copies: its content
    # is real, not a link into the fetch cache.
    if has_local_catalog():
        manifest["materialize"] = "copy"
    save_manifest(manifest)
    selected = sum(len(manifest.get(key) or []) for key in MANIFEST_KEYS.values())
    if args.json:
        print(_dumps({"command": "adopt", "created": True,
                      "path": str(path), "selected": selected}))
    else:
        print(f"wrote: {path} ({selected} selected item(s))")
    return 0


def cmd_release(args) -> int:
    """Report newer release tags for sources that name a repository.

    Informational, so it always exits 0: an unreachable remote is a row with
    `checked: false`, not a failed command. Bumping the pin stays a manifest
    edit — this command only makes the edit worth making.
    """
    manifest = load_manifest() or starter_manifest()
    rows = []
    for name, entry in _sources(manifest):
        # A source with a `path` reads that checkout; its `repo` is provenance,
        # not a release channel, so there is nothing to compare.
        if entry.get("path"):
            continue
        rows.append(release_row(name, entry))

    newer = [row["id"] for row in rows if row["newer"]]
    if args.json:
        print(_dumps({"command": "release", "offline": offline(),
                      "newer": newer, "sources": rows}))
        return 0

    if not rows:
        print("no source names a remote repository")
        return 0
    for row in rows:
        if row["error"]:
            print(f"  {row['id']:<14} {row['repo']} @ {row['ref']} — "
                  f"cannot read tags: {row['error']}")
        elif row["newer"]:
            print(f"  {row['id']:<14} {row['repo']} @ {row['ref']} — "
                  f"{row['latest']} is available")
        elif row["latest"] and not row["pinnedTag"]:
            print(f"  {row['id']:<14} {row['repo']} @ {row['ref']} — "
                  f"pinned to a moving ref; latest release {row['latest']}")
        else:
            print(f"  {row['id']:<14} {row['repo']} @ {row['ref']} — up to date")
    return 0


def cmd_init(args) -> int:
    """Bootstrap a project's selection — the 'install everything' entry point."""
    manifest_path = _manifest_path()
    fresh = not manifest_path.is_file()
    if not fresh and not args.force:
        _die(f"{manifest_path.name} already exists — use --force to replace it")

    preset = "Everything" if args.all else args.preset
    if not preset and not (args.ids or args.agents):
        _die("init needs --preset <name>, --all, or explicit ids")

    existing = load_manifest() or {}
    # A project with no catalog of its own (a brand-new repo, or one that never
    # had this template) cannot read the template source locally — there is
    # nothing there. Point it at the repository and fetch instead.
    if has_local_catalog():
        ref = args.ref or DEFAULT_REF
        data = default_manifest(repo=args.repo or DEFAULT_REPO, ref=ref)
    else:
        ref = args.ref or "main"
        data = default_manifest(repo=args.repo or DEFAULT_REPO, ref=ref)
        data["sources"][DEFAULT_SOURCE]["resolve"] = "remote"
        if not args.ref and not args.repo:
            print("no local catalog — resolving the template from its repository", file=sys.stderr)
    # Third-party sources are offered without being typed by hand, but they are
    # *written into the manifest* rather than hidden: you can see the pin, change
    # it, or switch it off with "enabled": false.
    if not offline():
        for name, entry in BUILTIN_SOURCES.items():
            data["sources"].setdefault(name, dict(entry))
    # init sets the *selection*. It must not silently redefine where the catalog
    # is reached from, or a project pointed at a local checkout would be
    # repointed at a GitHub URL it cannot fetch.
    if existing.get("sources"):
        data["sources"].update(existing["sources"])
    if args.repo or args.ref or args.path:
        entry = dict(data["sources"].get(DEFAULT_SOURCE) or {})
        if args.repo:
            entry["repo"] = args.repo
        if args.ref:
            entry["ref"] = args.ref
        if args.path:
            entry["path"] = args.path
        data["sources"][DEFAULT_SOURCE] = entry
    data["preset"] = preset
    data["skills"] = list(args.ids or [])
    data["agents"] = list(args.agents or [])
    # Copy by default for a fetched source: a file hard link goes stale when the
    # cache is refreshed. A local source (the template itself) keeps link.
    default_mode = "copy" if is_remote(data["sources"].get(DEFAULT_SOURCE) or {}) else "link"
    data["materialize"] = args.materialize or default_mode
    _write_json(manifest_path, data)

    if fresh:
        # A brand-new project gets the supporting folders and agent files in
        # full. Existing files are never overwritten; this only fills in what
        # is absent, and nothing is ever pruned.
        source_root = _source_root(data)
        install_scaffold(source_root)
        install_agent_config(source_root, data)

    if not args.json:
        print(f"wrote: {manifest_path} (preset: {preset or 'explicit list'})")

    # A bootstrap that leaves the tree unsynced is half a bootstrap.
    sync_args = argparse.Namespace(check=False, frozen=False, prune=False,
                                   dry_run=False, materialize=data["materialize"],
                                   json=args.json)
    return cmd_sync(sync_args)


def cmd_update(args) -> int:
    """Report (and optionally apply) catalog changes since the lock was written."""
    manifest = load_manifest() or starter_manifest()
    # An update check is exactly the moment a cached snapshot must not be trusted.
    index = build_index(manifest, refresh=True)
    index_by_key = _index_map(index)
    sources = dict(_sources(manifest))
    selected = _effective_selection(manifest, index_by_key, strict=False)

    _hook_install_gate(manifest, selected, index_by_key)
    # Whether anything *selected* is remote. A local selection has no upstream,
    # even when another, unselected source (awesome-copilot) is remote — the UI
    # must not read that as "your template is up to date".
    remote_selected = any(
        is_remote(sources.get(((index_by_key.get((kind, item_id)) or {}).get("source")
                               or DEFAULT_SOURCE)) or {})
        for kind, item_id in selected
    )
    lock = load_lock()
    locked = {_lock_key(entry): entry for entry in lock.get("resolved") or []}
    mode = _materialize_mode(manifest)
    wanted = set(args.ids or [])

    UPDATABLE = ("upstream-changed", "not-installed", "missing")
    CONFLICT = ("local-modified", "both")
    frozen = _locked_ids(manifest)

    rows = []
    for kind, item_id in selected:
        if wanted and item_id not in wanted and display_id(item_id) not in wanted:
            continue
        item = index_by_key.get((kind, item_id))
        entry = locked.get((kind, item_id))
        source_name = (item or {}).get("source") or DEFAULT_SOURCE
        root = _source_root_for(source_name, sources.get(source_name) or {})
        target = _target_path(kind, (item or {}).get("name") or display_id(item_id))
        state = evaluate_status(kind, item, target, entry, _same_path(root, _root()))
        rows.append({
            "id": item_id,
            "displayId": display_id(item_id),
            "kind": kind,
            "status": state,
            "locked": item_id in frozen,
            "path": (item or {}).get("path") or (entry or {}).get("path") or "",
            "source": source_name,
        })

    # A locked item is never overwritten, `--force` included. The one exception
    # is content that is not there at all: installing it back is not an update,
    # and a fresh clone still needs its locked items materialised.
    ready = [row for row in rows
             if not row["locked"] and row["status"] in UPDATABLE]
    ready += [row for row in rows
              if row["locked"] and row["status"] in ("not-installed", "missing")]
    blocked = [row for row in rows
               if not row["locked"] and row["status"] in CONFLICT]
    held = [row for row in rows
            if row["locked"] and row["status"] not in ("not-installed", "missing")]
    chosen = ready + (blocked if args.force else [])

    if not args.apply:
        if args.json:
            print(_dumps({"command": "update", "applied": False, "dryRun": True,
                          "ready": ready, "blocked": blocked, "locked": held,
                          "remoteSelected": remote_selected,
                          "remote": any(is_remote(entry) for _n, entry in _sources(manifest))}))
        else:
            if not rows:
                print("nothing selected")
            for row in ready:
                print(f"  {'UPDATE' if row['status'] != 'not-installed' else 'INSTALL':<8} "
                      f"{row['displayId']} ({row['kind']}) — {row['status']}")
            for row in held:
                print(f"  {'LOCKED':<8} {row['displayId']} ({row['kind']}) — "
                      f"{row['status']}; unlock to let an update touch it")
            for row in blocked:
                print(f"  {'SKIP':<8} {row['displayId']} ({row['kind']}) — {row['status']}"
                      f"{'' if args.force else ' (use --force to overwrite local changes)'}")
            print(f"\n{len(ready)} ready, {len(blocked)} blocked, {len(held)} locked. "
                  f"Run with --apply to take the ready ones.")
        return 0

    entries = dict(locked)
    applied = []
    for row in chosen:
        item = index_by_key.get((row["kind"], row["id"]))
        if not item:
            continue
        source_name = item.get("source") or DEFAULT_SOURCE
        root = _source_root_for(source_name, sources.get(source_name) or {})
        target = _target_path(row["kind"], item["name"])
        ok = materialize_item(row["kind"], item,
                             root / (item.get("sourcePath") or item["path"]), target, mode)
        applied.append({"id": row["id"], "ok": ok, "status": row["status"]})
        if ok:
            entries[(row["kind"], row["id"])] = _make_entry(row["kind"], item, mode)

    template_root = _source_root_for(DEFAULT_SOURCE, _source_entry(manifest))
    if not _same_path(template_root, _root()):
        save_lock(list(entries.values()))

    if getattr(args, "tooling", False):
        # The explicit update-to-release path: the repository's resolver and
        # vocabulary follow the pin, so its own CLI and mirrors match the tree.
        migrate_catalog_dir()
        source_root = _source_root(manifest)
        install_tooling(source_root, overwrite=True, ref=_source_ref(manifest))
        install_agent_config(source_root, manifest)
        ensure_gitignore()
        sync_platform_mirrors(manifest)

    if args.json:
        print(_dumps({"command": "update", "applied": True, "dryRun": False,
                      "appliedItems": applied, "blocked": blocked, "locked": held,
                      "remoteSelected": remote_selected}))
    else:
        for entry in applied:
            print(f"  {'OK' if entry['ok'] else 'FAILED':<8} {display_id(entry['id'])}")
        for row in held:
            print(f"  {'LOCKED':<8} {row['displayId']} ({row['kind']}) — left untouched")
        print(f"\n{len(applied)} item(s) updated, {len(held)} locked and left untouched.")
    return 0 if all(entry["ok"] for entry in applied) else 1


def cmd_notice(args) -> int:
    """Write attribution for the third-party content this project installs."""
    manifest = load_manifest() or starter_manifest()
    index = build_index(manifest)
    index_by_key = _index_map(index)
    selected = _effective_selection(manifest, index_by_key, strict=False)

    out = Path(args.out)
    if not out.is_absolute():
        out = _root() / out

    rows = [index_by_key[(kind, item_id)] for kind, item_id in selected
            if (kind, item_id) in index_by_key]

    lines = [
        "# NOTICE",
        "",
        "Third-party skills and agents installed in this project, with their source,",
        "pinned ref, licence and upstream URL. Generated by",
        "`python .ai/scripts/catalog.py notice` — do not edit by hand.",
        "",
        "Review the upstream of anything you install. Content in this catalog may be",
        "sourced from third parties and is included as-is.",
        "",
        "| Item | Kind | Source | Ref | License | Upstream |",
        "|---|---|---|---|---|---|",
    ]
    for item in sorted(rows, key=lambda entry: (entry["source"], entry["id"], entry["kind"])):
        lines.append(
            f"| `{display_id(item['id'])}` | {item['kind']} | {item['source']} | "
            f"{item['ref']} | {item['license']} | {item['upstream']} |"
        )
    text = "\n".join(lines) + "\n"

    if args.verify:
        existing = out.read_text(encoding="utf-8") if out.is_file() else None
        if existing != text:
            if args.json:
                print(_dumps({"command": "notice", "stale": True, "path": str(out)}))
            print(f"STALE: {out} — run 'python .ai/scripts/catalog.py notice'", file=sys.stderr)
            return 1
        if args.json:
            print(_dumps({"command": "notice", "stale": False, "path": str(out)}))
        else:
            print(f"notice is up to date: {out} ({len(rows)} item(s))")
        return 0

    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(text, encoding="utf-8")

    if args.json:
        print(_dumps({"command": "notice", "path": str(out), "items": len(rows)}))
    else:
        print(f"wrote: {out} ({len(rows)} item(s))")
    return 0


def cmd_list(args) -> int:
    manifest = load_manifest() or starter_manifest()
    index = build_index(manifest)
    available = index.get("items") or []
    lock = load_lock() if _lock_path().is_file() else {"resolved": []}

    selected = set(_effective_selection(manifest, _index_map(index), strict=False))
    locked = {_lock_key(entry) for entry in lock.get("resolved") or []}
    installed_keys = selected | locked

    only_installed = args.installed and not args.available
    show_installed = args.installed or not args.available
    show_available = args.available or not args.installed

    rows = []
    for item in available:
        key = (item["kind"], item["id"])
        is_installed = key in installed_keys
        if only_installed and not is_installed:
            continue
        rows.append({
            "id": item["id"],
            "kind": item["kind"],
            "name": item["name"],
            "path": item["path"],
            "description": item["description"],
            "sha256": item["sha256"],
            "ref": item["ref"],
            "installed": is_installed,
            "state": ("selected" if key in selected else "locked") if is_installed else "",
        })

    # Locked entries the catalog no longer offers are still installed.
    catalog_keys = {(item["kind"], item["id"]) for item in available}
    if show_installed:
        for entry in lock.get("resolved") or []:
            if _lock_key(entry) in catalog_keys:
                continue
            rows.append({
                "id": entry.get("id"),
                "kind": entry.get("kind") or "skill",
                "name": display_id(entry.get("id") or ""),
                "path": entry.get("path") or "",
                "description": "(not in the catalog)",
                "sha256": entry.get("sha256") or "",
                "ref": entry.get("ref") or "",
                "installed": True,
                "state": "locked",
            })

    if args.json:
        payload = {
            "command": "list",
            "items": rows,
            "available": [row["id"] for row in rows if row["state"] != "locked"
                          or (row["kind"], row["id"]) in catalog_keys],
            "installed": [row["id"] for row in rows if row["installed"]],
        }
        if show_available and not show_installed:
            payload["items"] = [row for row in rows if not row["installed"]
                                or (row["kind"], row["id"]) in catalog_keys]
        print(_dumps(payload))
        return 0

    if not rows:
        print("no items")
        return 0
    width = max(len(display_id(row["id"] or "")) for row in rows)
    print(f"{'ID':<{width}}  {'KIND':<6}  {'INSTALLED':<9}  DESCRIPTION")
    for row in rows:
        flag = "yes" if row["installed"] else "-"
        description = row["description"]
        if len(description) > 70:
            description = description[:67] + "..."
        print(f"{display_id(row['id'] or ''):<{width}}  {row['kind']:<6}  "
              f"{flag:<9}  {description}")
    print(f"\n{len(rows)} item(s); {len(lock.get('resolved') or [])} in the lock")
    return 0


# ─── Commands: status ────────────────────────────────────────────────────────

def cmd_status(args) -> int:
    manifest = load_manifest() or starter_manifest()
    index = build_index(manifest)
    index_by_key = _index_map(index)
    source_root = _source_root(manifest)
    same_dir = _same_path(source_root, _root())
    lock = load_lock()
    locked = {_lock_key(entry): entry for entry in lock.get("resolved") or []}
    selected = _effective_selection(manifest, index_by_key, strict=False)
    mode = _materialize_mode(manifest)

    rows = []
    for kind, item_id in selected:
        item = index_by_key.get((kind, item_id))
        name = item["name"] if item else display_id(item_id)
        target = _target_path(kind, name)
        entry = locked.get((kind, item_id))
        status = evaluate_status(kind, item, target, entry, same_dir)
        rows.append({
            "id": item_id,
            "kind": kind,
            "path": (entry or {}).get("path") or (item or {}).get("path") or "",
            "status": status,
            "upstream": UPSTREAM_UNCHANGED,
            "selected": True,
            "materialize": (entry or {}).get("materialize") or mode,
        })

    # Locked but no longer selected: the files are still on disk and only
    # `sync --prune` may remove them. The status enum has no dedicated value,
    # so these are reported as removed-upstream (gone from the wanted set),
    # or missing when their path is already gone.
    for key, entry in sorted(locked.items(), key=lambda pair: _lock_sort_key(pair[1])):
        if key in selected:
            continue
        target = _root() / (entry.get("path") or "")
        exists = target.exists() or _is_link(target)
        rows.append({
            "id": entry.get("id"),
            "kind": entry.get("kind") or "skill",
            "path": entry.get("path") or "",
            "status": STATUS_REMOVED_UPSTREAM if exists else STATUS_MISSING,
            "upstream": UPSTREAM_UNCHANGED,
            "selected": False,
            "materialize": entry.get("materialize") or mode,
        })

    if args.json:
        print(_dumps({
            "command": "status",
            "source": {"id": DEFAULT_SOURCE, "root": str(source_root),
                       "ref": _source_ref(manifest),
                       "sameDir": same_dir},
            "items": rows,
        }))
        return 0

    if not rows:
        print("no selections")
        return 0
    width = max(len(display_id(row["id"])) for row in rows)
    print(f"{'ID':<{width}}  {'KIND':<6}  {'STATUS':<17}  PATH")
    for row in rows:
        print(f"{display_id(row['id']):<{width}}  {row['kind']:<6}  "
              f"{row['status']:<17}  {row['path']}")
    counts = {}
    for row in rows:
        counts[row["status"]] = counts.get(row["status"], 0) + 1
    summary = ", ".join(f"{count} {status}" for status, count in sorted(counts.items()))
    print(f"\n{len(rows)} item(s): {summary}")
    if same_dir:
        print("source == target: the catalog and the installed set are the same directory")
    return 0


# ─── Commands: sync ──────────────────────────────────────────────────────────

def _action(name: str, kind: str, item_id: str, path: str, detail: str = "") -> dict:
    return {"action": name, "kind": kind, "id": item_id, "path": path, "detail": detail}


PROJECT_INFO = ".ai/project/project.json"


def project_info_path() -> Path:
    return _root() / PROJECT_INFO


def detected_project_name() -> str:
    """The solution's name, or the workspace folder's."""
    for pattern in ("*.slnx", "*.sln"):
        solutions = sorted(_root().glob(pattern))
        if solutions:
            return solutions[0].stem
    return _root().name


def load_project_info() -> dict:
    path = project_info_path()
    if path.is_file():
        data = _read_json(path)
        if isinstance(data, dict):
            return data
    return {}


def _insert_after_title(text: str, lines: list) -> str:
    parts = text.splitlines()
    for index, line in enumerate(parts):
        if line.startswith("# "):
            return "\n".join(parts[:index + 1] + [""] + lines + parts[index + 1:]) + "\n"
    return "\n".join(lines + parts) + "\n"


def _scaffold_files() -> list:
    """The supporting `.ai/` files a project owns (not items, not scripts)."""
    files = [
        _root() / ".ai" / "session-context.md",
        _root() / "AGENTS.md",
        _root() / ".github" / "copilot-instructions.md",
    ]
    for name in TEMPLATE_DIRS:
        directory = _root() / ".ai" / name
        if directory.is_dir():
            files.extend(sorted(entry for entry in directory.rglob("*") if entry.is_file()))
    return files


def _fill_application_name(name: str) -> list:
    """Replace `{ApplicationName}` wherever the scaffold still carries it.

    Patterns, checklists, reference docs and code templates use the token in
    examples, so the project name belongs in them too — not only in
    session-context. Other placeholders (`{Domain}`, `{Entity}`, …) are left for
    the agent to fill per feature.
    """
    written = []
    for path in _scaffold_files():
        if path.suffix.lower() not in (".md", ".txt", ".feature", ".json"):
            continue
        try:
            text = path.read_text(encoding="utf-8")
        except (OSError, UnicodeDecodeError):
            continue
        if "{ApplicationName}" not in text:
            continue
        path.write_text(text.replace("{ApplicationName}", name),
                        encoding="utf-8", newline="\n")
        written.append(str(path.relative_to(_root())).replace(os.sep, "/"))
    return written


def _fill_template(path: Path, marker: str, replacements: list,
                   preamble: list | None = None) -> bool:
    """Fill a still-template document; a customized one is left alone."""
    if not path.is_file():
        return False
    text = path.read_text(encoding="utf-8")
    if marker not in text:
        return False
    for old, new in replacements:
        text = text.replace(old, new)
    if preamble:
        text = _insert_after_title(text, preamble)
    path.write_text(text, encoding="utf-8", newline="\n")
    return True


def _update_line(text: str, label: str, value: str) -> str:
    return re.sub(rf"^{re.escape(label)}.*$", f"{label} {value}", text,
                  count=1, flags=re.M)


def _describe_project(name: str, intention: str) -> dict:
    today = date.today().isoformat()
    purpose = intention or "[What this application does]"
    written, skipped = [], []

    session = _root() / ".ai" / "session-context.md"
    if _fill_template(session, "{ApplicationName}", [
        ("# Session Context Template (CUSTOMIZE THIS)\n\n"
         "**Instructions:** Copy this to `.ai/session-context.md` and customize for your project.\n\n", ""),
        ("# {ApplicationName} Session Context", f"# {name} Session Context"),
        ("[{ApplicationName}]", name),
        ("[What this application does]", purpose),
        ("[DateTime]", today),
        ("**Updated By:** [Claude Code | Copilot | Reasonix Code | User]",
         "**Updated By:** AI Workspace Manager"),
    ]):
        written.append(".ai/session-context.md")
    elif session.is_file():
        text = session.read_text(encoding="utf-8")
        # A document this form filled earlier: update the purpose in place.
        if ("**Updated By:** AI Manager" in text
            or "**Updated By:** AI Workspace Manager" in text):
            if intention:
                text = _update_line(text, "**Purpose:**", intention)
            text = _update_line(text, "**Last Updated:**", today)
            session.write_text(text, encoding="utf-8", newline="\n")
            written.append(".ai/session-context.md")
        else:
            skipped.append(".ai/session-context.md")

    for rel, title in ((".ai/project/architecture.md", "Project Architecture"),
                       (".ai/project/domains.md", "Business Domains")):
        path = _root() / rel
        if _fill_template(path, "(CUSTOMIZE THIS)", [
            (f"# {title} (CUSTOMIZE THIS)", f"# {name} — {title}"),
        ], preamble=[f"**Project:** {name}", f"**Intention:** {purpose}", ""]):
            written.append(rel)
        elif path.is_file():
            text = path.read_text(encoding="utf-8")
            if "**Project:**" in text and "**Intention:**" in text:
                text = _update_line(text, "**Project:**", name)
                if intention:
                    text = _update_line(text, "**Intention:**", intention)
                path.write_text(text, encoding="utf-8", newline="\n")
                written.append(rel)
            else:
                skipped.append(rel)

    info = {"version": 1, "name": name, "intention": intention, "updated": today}
    _write_json(project_info_path(), info)
    for rel in _fill_application_name(name):
        if rel not in written:
            written.append(rel)

    readme = _root() / "README.md"
    if not readme.is_file():
        readme.write_text(f"# {name}\n\n{intention or README_MARKER}\n",
                          encoding="utf-8", newline="\n")
        written.append("README.md")
    else:
        text = readme.read_text(encoding="utf-8")
        if README_MARKER in text:
            text = text.replace(README_MARKER, intention or README_MARKER)
            lines = text.splitlines()
            for index, line in enumerate(lines):
                if line.startswith("# "):
                    lines[index] = f"# {name}"
                    break
            readme.write_text("\n".join(lines) + "\n", encoding="utf-8", newline="\n")
            written.append("README.md")

    return {**info, "detectedName": detected_project_name(),
            "written": written, "skipped": skipped}


def cmd_project(args) -> int:
    """Read or write the project's basic description.

    The name defaults to the solution (`.sln`/`.slnx`) or the folder. Writing
    stores `.ai/project/project.json` and fills `session-context.md`,
    `architecture.md` and `domains.md` only while they still carry the template
    markers, so a customized document is never rewritten.
    """
    current = load_project_info()
    detected = detected_project_name()
    if args.name is None and args.intention is None:
        info = {
            "name": str(current.get("name") or detected),
            "intention": str(current.get("intention") or ""),
            "detectedName": detected,
            "written": [],
            "skipped": [],
        }
        if args.json:
            print(_dumps(info))
        else:
            print(f"name: {info['name']}")
            print(f"intention: {info['intention'] or '(not set)'}")
        return 0

    name = str(args.name if args.name is not None
               else current.get("name") or detected).strip()
    if not name:
        _die("a project name is required")
    intention = str(args.intention if args.intention is not None
                    else current.get("intention") or "").strip()
    info = _describe_project(name, intention)
    if args.json:
        print(_dumps(info))
    else:
        print(f"wrote: {project_info_path()} ({name})")
        for rel in info["written"]:
            print(f"  filled {rel}")
        for rel in info["skipped"]:
            print(f"  kept   {rel} (customized)")
    return 0


README_MARKER = "[Describe the project's purpose.]"


def ensure_readme() -> str:
    """Create a starter README when the project has none.

    The template's own README is template-specific, so a project gets a two-line
    starter instead; the project form fills the purpose when it is still there.
    """
    path = _root() / "README.md"
    if path.is_file():
        return "kept"
    path.write_text(f"# {detected_project_name()}\n\n{README_MARKER}\n",
                    encoding="utf-8", newline="\n")
    return "created"


# `AGENTS.md` is the source of truth; the Copilot file is generated from it.
AGENTS_ANCHOR = ".ai/session-context.md"
COPILOT_FILE = ".github/copilot-instructions.md"
# Copilot reads `.github/`, not Claude's `.claude/`. AGENTS.md is shared, so a
# platform path in it is rewritten only in the generated Copilot copy. The
# Claude settings file is a real Claude-only path and stays as written.
_CLAUDE_PATH = re.compile(r"\.claude/(?!settings\.json|settings\.local\.json)")


def _copilot_paths(text: str) -> str:
    return _CLAUDE_PATH.sub(".github/", text)
# HTML-comment markers that identify generated regions. The namespace was
# renamed for this release; LEGACY_MARKER_PREFIX below is the only place the old
# spelling survives, so files written by an older version can be migrated.
MARKER_PREFIX = "<!-- ai-workspace-manager:"
COPILOT_GENERATED = f"{MARKER_PREFIX}generated-from-agents -->"
COPILOT_REFERENCE_START = f"{MARKER_PREFIX}copilot-reference:start -->"
COPILOT_REFERENCE_END = f"{MARKER_PREFIX}copilot-reference:end -->"
INSTALLED_START = f"{MARKER_PREFIX}installed:start -->"
INSTALLED_END = f"{MARKER_PREFIX}installed:end -->"
# Pre-rename marker prefix, kept solely for backward compatibility: text read
# from disk is normalized with it before any anchor is matched, and the next
# regeneration writes only the current namespace. Removing it would make
# existing generated files look hand-written (and their reference block get
# duplicated).
LEGACY_MARKER_PREFIX = "<!-- ai-manager:"
# Copilot-only summaries that AGENTS.md does not repeat. They are carried over
# when the Copilot file is regenerated, so no shared rule is duplicated.
COPILOT_REFERENCE = (
    "Project Context",
    "Architecture",
    "Forbidden Technologies",
    "Patterns & Templates",
    "Naming Conventions",
    "Token Replacements",
    "Core Operational Rules",
    "Work-Type Context Mapping",
)
_ITEM_REF = re.compile(
    r"`(?P<path>\.ai/(?:skills/(?P<skill>[A-Za-z0-9._-]+)/SKILL\.md"
    r"|agents/(?P<agent>[A-Za-z0-9._-]+)\.md))`"
)
_INSTALLED_BLOCK = re.compile(
    r"## Installed skills and agents\s*\n.*?" + re.escape(INSTALLED_END),
    re.S,
)


def _normalize_markers(text: str) -> str:
    """Rewrite pre-rename markers to the current namespace (idempotent)."""
    return str(text or "").replace(LEGACY_MARKER_PREFIX, MARKER_PREFIX)


def _first_line(text: str) -> str:
    lines = [line.strip() for line in str(text or "").strip().splitlines() if line.strip()]
    return lines[0] if lines else ""


def installed_catalog() -> dict:
    """The skills and agents actually present, with their descriptions."""
    skills, agents = [], []
    skills_dir = _ai_dir() / "skills"
    if skills_dir.is_dir():
        for directory in sorted(skills_dir.iterdir()):
            skill = directory / "SKILL.md"
            if not directory.is_dir() or not skill.is_file():
                continue
            meta = frontmatter(skill.read_text(encoding="utf-8", errors="replace"))
            skills.append({"name": str(meta.get("name") or directory.name),
                           "description": _first_line(str(meta.get("description") or ""))})
    agents_dir = _ai_dir() / "agents"
    if agents_dir.is_dir():
        for file in sorted(agents_dir.glob("*.md")):
            meta = frontmatter(file.read_text(encoding="utf-8", errors="replace"))
            agents.append({"name": str(meta.get("name") or file.stem),
                           "description": _first_line(str(meta.get("description") or ""))})
    return {"skills": skills, "agents": agents}


def _installed_section(catalog: dict) -> str:
    lines = ["## Installed skills and agents", "", INSTALLED_START]
    for kind, title in (("skills", "Skills"), ("agents", "Agents")):
        if not catalog[kind]:
            continue
        lines.extend([f"### {title}"])
        for item in catalog[kind]:
            description = f" — {item['description']}" if item["description"] else ""
            lines.append(f"- `{item['name']}`{description}")
        lines.append("")
    lines.append(INSTALLED_END)
    return "\n".join(lines)


def _tidy_reference_line(line: str) -> str:
    while re.search(r",\s*,", line):
        line = re.sub(r",\s*,", ", ", line)
    line = re.sub(r"\|\s*,", "|", line)
    line = re.sub(r",\s*\|", " |", line)
    line = re.sub(r";\s*invocable as\s*$", "", line, flags=re.I)
    line = re.sub(r"\s+as\s*$", "", line, flags=re.I)
    line = re.sub(r"\s+and\s*\.?\s*$", "", line)
    line = re.sub(r"\s+or\s*\.?\s*$", "", line)
    line = re.sub(r";\s*$", "", line)
    line = re.sub(r"[ 	]{2,}", " ", line)
    return line.rstrip()


def _filter_item_refs(text: str, names: dict) -> str:
    """Drop references to skills/agents that are not installed.

    A table row that loses every `.ai/` reference disappears; a cell that keeps
    another path loses only the missing token.
    """
    kept = []
    for line in text.splitlines():
        missing = False

        def replace(match):
            nonlocal missing
            if match.group("skill"):
                if match.group("skill") in names["skills"]:
                    return match.group(0)
            elif match.group("agent") in names["agents"]:
                return match.group(0)
            missing = True
            return ""

        updated = _ITEM_REF.sub(replace, line)
        if not missing:
            kept.append(line)
            continue
        updated = _tidy_reference_line(updated)
        if updated.lstrip().startswith("|") and not re.search(r"`\.ai/", updated):
            continue
        if not updated.strip():
            continue
        kept.append(updated)
    return "\n".join(kept) + ("\n" if text.endswith("\n") else "")


def _section(text: str, title: str) -> str:
    pattern = re.compile(rf"^## {re.escape(title)}\s*$.*?(?=^## |\Z)", re.M | re.S)
    match = pattern.search(text)
    return match.group(0).strip() if match else ""


def _copilot_reference(text: str) -> str:
    """The Copilot-only sections, kept across regenerations."""
    if COPILOT_REFERENCE_START in text and COPILOT_REFERENCE_END in text:
        start = text.index(COPILOT_REFERENCE_START) + len(COPILOT_REFERENCE_START)
        end = text.index(COPILOT_REFERENCE_END)
        return text[start:end].strip()
    parts = [_section(text, title) for title in COPILOT_REFERENCE]
    return "\n\n".join(part for part in parts if part)


def _copilot_document(agents_text: str, existing: str | None, names: dict) -> str | None:
    """Build the Copilot file from AGENTS.md plus its Copilot-only sections.

    A hand-written Copilot file without the template's structure or the
    generated marker is left alone. Markers from before the namespace rename
    are normalized first, so such a file is recognized and migrated rather than
    treated as hand-written.
    """
    reference = ""
    if existing is not None:
        existing = _normalize_markers(existing)
        generated = COPILOT_GENERATED in existing
        template = "## Work-Type Context Mapping" in existing or "## Project Context" in existing
        if not generated and not template:
            return None
        reference = _filter_item_refs(_copilot_reference(existing), names).strip()
    parts = ["# GitHub Copilot Instructions", "", COPILOT_GENERATED,
             "<!-- Generated from AGENTS.md. Edit AGENTS.md, not this file. -->", "",
             _copilot_paths(agents_text.strip())]
    if reference:
        parts += ["", COPILOT_REFERENCE_START, _copilot_paths(reference),
                  COPILOT_REFERENCE_END]
    return "\n".join(parts).rstrip() + "\n"


def update_instruction_docs() -> dict:
    """Keep the agent instructions in step with what is installed.

    `AGENTS.md` is the single source of truth: references to skills/agents that
    are not installed are filtered out and the installed items are listed.
    `.github/copilot-instructions.md` is then regenerated from it (plus the
    Copilot-only quick reference), so the two files cannot drift apart.
    """
    manifest = load_manifest()
    if not manifest:
        return {}
    entry = _source_entry(manifest)
    if entry and not is_remote(entry):
        source_root = _source_root_for(DEFAULT_SOURCE, entry)
        if _same_path(source_root, _root()):
            # The template repository itself: its docs are the full catalog.
            return {}

    catalog = installed_catalog()
    names = {"skills": {item["name"] for item in catalog["skills"]},
             "agents": {item["name"] for item in catalog["agents"]}}
    section = _installed_section(catalog)
    results = {}

    agents_path = _root() / "AGENTS.md"
    agents_raw = agents_path.read_text(encoding="utf-8") if agents_path.is_file() else ""
    # Compare against the bytes on disk: a file that only differs by its legacy
    # markers still has to be rewritten, or the old namespace would survive.
    agents_text = _normalize_markers(agents_raw)
    if agents_text:
        original = agents_raw
        if AGENTS_ANCHOR in agents_text:
            agents_text = _filter_item_refs(agents_text, names)
            if _INSTALLED_BLOCK.search(agents_text):
                agents_text = _INSTALLED_BLOCK.sub(section, agents_text)
            else:
                agents_text = agents_text.rstrip() + "\n\n" + section + "\n"
        if agents_text != original:
            agents_path.write_text(agents_text, encoding="utf-8", newline="\n")
            results["AGENTS.md"] = "updated"
        else:
            results["AGENTS.md"] = "unchanged"

    copilot_path = _root() / COPILOT_FILE
    if "copilot" not in _clients(manifest):
        if copilot_path.is_file():
            existing = copilot_path.read_text(encoding="utf-8")
            if COPILOT_GENERATED in existing or LEGACY_MARKER_PREFIX in existing:
                copilot_path.unlink()
                try:
                    copilot_path.parent.rmdir()
                except OSError:
                    pass
                results[COPILOT_FILE] = "removed"
            else:
                results[COPILOT_FILE] = "kept"
        return results

    if agents_text:
        existing = copilot_path.read_text(encoding="utf-8") if copilot_path.is_file() else None
        document = _copilot_document(agents_text, existing, names)
        if document is not None and document != existing:
            copilot_path.parent.mkdir(parents=True, exist_ok=True)
            copilot_path.write_text(document, encoding="utf-8", newline="\n")
            results[COPILOT_FILE] = "updated"
        else:
            results[COPILOT_FILE] = "unchanged" if document is not None else "kept"
    return results


def cmd_clients(args) -> int:
    """Show or set which AI clients get platform folders.

    The selection is stored in `.ai/ai.json` so it is committed and shared.
    `--set` takes the complete list; an empty list disables every client while
    leaving the `.ai/` source untouched.
    """
    manifest = load_manifest() or starter_manifest()

    if args.set_clients is None:
        names = [name for name in CLIENTS if name in _clients(manifest)]
        if args.json:
            print(_dumps({"command": "clients", "clients": names}))
        else:
            print(f"clients: {', '.join(names) or '(none)'}")
        return 0

    names = []
    for raw in args.set_clients:
        name = str(raw).strip().lower()
        if name not in CLIENTS:
            _die(f"unknown client '{raw}' — choose from {', '.join(CLIENTS)}")
        if name not in names:
            names.append(name)
    manifest["clients"] = names
    save_manifest(manifest)

    if args.json:
        print(_dumps({"command": "clients", "clients": names}))
    else:
        print(f"clients: {', '.join(names) or '(none)'}")
        print("run 'python .ai/scripts/catalog.py sync' to add or remove the folders")
    return 0


def cmd_scaffold(args) -> int:
    """Copy any missing supporting `.ai/` files on request.

    A new project gets these during setup. This command exists for an existing
    project that wants a missing file back. Existing files are never changed or
    removed — only absent ones are written.
    """
    manifest = load_manifest() or starter_manifest()
    source_root = _source_root(manifest)

    summary = install_scaffold(source_root)
    install_agent_config(source_root, manifest)
    if args.json:
        print(_dumps({"command": "scaffold", **summary}))
    else:
        print(f"scaffold: {len(summary['copied'])} copied, "
              f"{len(summary['skipped'])} kept")
    return 0


def cmd_bootstrap(args) -> int:
    """Set a repository up from the template: manifest, resolver, scaffold.

    One call for a brand-new folder. It writes `.ai/ai.json` when missing,
    fetches the template, installs the resolver and vocabulary under `.ai/`,
    and — on a project that had no manifest yet — copies the supporting `.ai/`
    folders (plans, progress, patterns, reference, checklists, completed,
    analysis, project, session-context.md). Existing projects are not re-seeded;
    `catalog.py scaffold` does that on request. It never overwrites project
    content and never selects items — choosing skills is what presets are for.
    """
    existing = load_manifest()
    manifest = existing or starter_manifest()
    added_sources = []
    if existing is None:
        save_manifest(manifest)
    else:
        # A project that predates a built-in source never sees it otherwise:
        # browse only reads the sources its manifest declares.
        added_sources = merge_builtin_sources(manifest)
        if added_sources:
            save_manifest(manifest)

    migrate_catalog_dir()
    manifest = load_manifest() or manifest
    source_root = _source_root(manifest)
    # Activation is also the upgrade path: drop state files an older version
    # wrote, even if the user never runs sync.
    legacy = {"removed": []}
    if not _same_path(source_root, _root()):
        installed = [_root() / str(entry.get("path")) for entry
                     in (load_lock().get("resolved") or []) if entry.get("path")]
        legacy = remove_legacy_artifacts(installed)
    # Setup brings an existing project up to the files its pin (or the
    # extension bundle, for files the template no longer ships) provides.
    # A file the project edited no longer matches its recorded hash and is
    # protected by install_tooling; only untouched files are refreshed. This is
    # what makes a new preset or category appear without asking the user to
    # re-run anything.
    tooling = install_tooling(source_root, overwrite=True, ref=_source_ref(manifest))
    install_agent_config(source_root, manifest)
    # Seeding the supporting `.ai/` folders is a setup step, not something to
    # repeat on every activation: projects that predate this change keep what
    # they have, and `catalog.py scaffold` is there when they want the rest.
    scaffold = (install_scaffold(source_root) if existing is None
                else {"copied": [], "skipped": []})
    gitignore = ensure_gitignore()
    readme = ensure_readme()
    if is_remote(_source_entry(manifest)) and not project_info_path().is_file():
        # A project starts with its folder/solution name filled in; the form
        # supplies the intention later.
        _describe_project(detected_project_name(), "")
    sync_platform_mirrors(manifest)
    instructions = update_instruction_docs()

    selected = sum(len(manifest.get(key) or []) for key in MANIFEST_KEYS.values())
    if args.json:
        print(_dumps({
            "command": "bootstrap",
            "created": existing is None,
            "path": str(_manifest_path()),
            "selected": selected,
            "sourcesAdded": added_sources,
            "tooling": tooling,
            "scaffold": scaffold,
            "legacyRemoved": legacy["removed"],
            "gitignore": gitignore,
            "readme": readme,
            "instructions": instructions,
        }))
    else:
        print(f"wrote: {_manifest_path()} ({selected} selected item(s))")
        if added_sources:
            print(f"sources: added {', '.join(added_sources)}")
        print(f"tooling: {len(tooling['copied'])} copied, "
              f"{len(tooling['missing'])} missing")
        print(f"scaffold: {len(scaffold['copied'])} copied, "
              f"{len(scaffold['skipped'])} kept")
        if legacy["removed"]:
            print(f"legacy state: {len(legacy['removed'])} file(s) removed")
    return 0


def cmd_sync(args) -> int:
    migrate_catalog_dir()
    existing_manifest = load_manifest()
    fresh = existing_manifest is None
    manifest = existing_manifest or starter_manifest()
    mode = _materialize_mode(manifest, args.materialize)
    source_root = _source_root(manifest)
    same_dir = _same_path(source_root, _root())
    index = build_index(manifest)
    index_by_key = _index_map(index)
    selected = _effective_selection(manifest, index_by_key)

    _hook_install_gate(manifest, selected, index_by_key)
    lock = load_lock()
    locked = {_lock_key(entry): entry for entry in lock.get("resolved") or []}
    frozen = _locked_ids(manifest)
    ref = _source_ref(manifest)

    actions = []
    conflicts = []
    entries = []
    targets = []

    for kind, item_id in selected:
        item = index_by_key[(kind, item_id)]
        target = _target_path(kind, item["name"])
        targets.append(target)
        item_source = item.get("source") or DEFAULT_SOURCE
        item_source_root = _source_root_for(item_source,
                                            _source_entry_named(manifest, item_source))
        source = item_source_root / (item.get("sourcePath") or item["path"])
        entry = {
            "id": item_id,
            "kind": kind,
            "source": item_source,
            "ref": item.get("ref") or ref,
            "path": item["path"],
            "sha256": item["sha256"],
            "files": item["files"],
            "license": item["license"],
            "upstream": item["upstream"],
            "materialize": mode,
        }
        previous = locked.get((kind, item_id))

        if _same_path(item_source_root, _root()):
            entries.append(entry)
            actions.append(_action("no-op", kind, item_id, item["path"],
                                   "source == target"))
            continue

        exists = target.exists() or _is_link(target)
        if not exists:
            entries.append(entry)
            actions.append(_action("create", kind, item_id, item["path"]))
        elif previous is None:
            # Never touch a path the lock does not record — and never adopt it
            # into the lock either, so a stray directory stays untouchable.
            actions.append(_action("conflict", kind, item_id, item["path"],
                                   "untracked path exists"))
            conflicts.append(f"{item['path']} exists but is not recorded in the lock")
        elif item_id in frozen:
            # The user froze this content. Keep the previous lock entry so a
            # later unlock still compares against the pristine hash instead of
            # adopting the edit as the new baseline.
            entries.append(previous)
            actions.append(_action("ok", kind, item_id, item["path"], "locked"))
        elif _same_path(source, target):
            # A healthy link resolves to the source, so its content cannot drift.
            # Verify it instead of reporting a no-op: only source == target (this
            # template repository's own case) short-circuits.
            entries.append(entry)
            actions.append(_action("ok", kind, item_id, item["path"]))
        else:
            entries.append(entry)
            current = installed_hash(item, target)
            needs = (
                current != entry["sha256"]
                or previous.get("path") != entry["path"]
                or (previous.get("materialize") or "link") != mode
            )
            actions.append(_action("update" if needs else "ok", kind, item_id, item["path"]))

    # Legacy install markers are redundant with the lock. Report them as drift
    # so a `--check` does not call a tree clean while they are still there.
    marker_files = [] if same_dir else legacy_markers(targets)
    old_index = None if same_dir else legacy_index()

    selected_keys = set(selected)
    known_kinds = set(KINDS)
    pending_prune = []
    # A lock written by a newer resolver may contain a kind this one does not
    # know (the prompt kind, for example, when an older client opens a project
    # that selected prompts). Never treat an unknown kind as deselected: pruning
    # it would delete files this client cannot reason about. Keep the entry in
    # the lock untouched so a later client still owns it.
    preserved_unknown = []
    for key, entry in locked.items():
        if key in selected_keys:
            continue
        kind = entry.get("kind") or "skill"
        if kind in known_kinds:
            pending_prune.append(entry)
        else:
            preserved_unknown.append(entry)
    pending_prune.sort(key=_lock_sort_key)

    if not args.prune:
        # Keep the lock entries so a later --prune can still find the paths.
        entries.extend(pending_prune)
    entries.extend(preserved_unknown)

    new_lock = {"lockVersion": 1, "resolved": entries}
    # With source == target the installed content *is* the source, so the lock
    # would only ever restate hashes that cannot drift. Nothing to pin, nothing
    # to freeze — and no lock churn on every skill edit.
    lock_changed = (not same_dir) and lock_signature(new_lock) != lock_signature(lock)

    changes = [a for a in actions if a["action"] in ("create", "update", "conflict")]
    drift = (bool(changes) or bool(pending_prune) or lock_changed
             or bool(marker_files) or old_index is not None)

    if args.check:
        report = {
            "command": "sync",
            "mode": "check",
            "clean": not drift,
            "sameDir": same_dir,
            "actions": actions,
            "pendingPrune": [entry.get("path") for entry in pending_prune],
            "preservedUnknown": [entry.get("id") for entry in preserved_unknown],
            "legacyMarkers": [str(path.relative_to(_root())) for path in marker_files],
            "legacyIndex": str(old_index.relative_to(_root())) if old_index else None,
            "lockChanged": lock_changed,
        }
        if args.json:
            print(_dumps(report))
        else:
            _print_sync_actions(actions, pending_prune, same_dir)
            if marker_files:
                print(f"{len(marker_files)} legacy marker(s) to clean")
            if old_index:
                print(f"legacy index to remove: {old_index.relative_to(_root())}")
            if drift:
                print("drift detected")
            else:
                print("in sync")
        return 1 if drift else 0

    if args.frozen and lock_changed:
        if args.json:
            print(_dumps({"command": "sync", "mode": "frozen", "frozen": False,
                          "lockChanged": True, "actions": actions}))
        else:
            _print_sync_actions(actions, pending_prune, same_dir)
            print(f"frozen lock would need to change: {_lock_path()}")
        return 1

    performed = []
    markers_removed = 0
    if not args.dry_run:
        for marker in marker_files:
            try:
                marker.unlink()
                markers_removed += 1
            except OSError:
                # Already gone with a directory replaced by materialize.
                pass
        if old_index is not None:
            try:
                old_index.unlink()
            except OSError:
                pass
        if marker_files:
            try:
                # Remove `.ai/installed/` itself when nothing else lives there.
                _installed_dir().rmdir()
            except OSError:
                pass
        for action in actions:
            if action["action"] not in ("create", "update"):
                continue
            kind, item_id = action["kind"], action["id"]
            item = index_by_key[(kind, item_id)]
            target = _target_path(kind, item["name"])
            item_source = item.get("source") or DEFAULT_SOURCE
            source = _source_root_for(item_source,
                                      _source_entry_named(manifest, item_source)) \
                / (item.get("sourcePath") or item["path"])
            ok = materialize_item(kind, item, source, target, mode)
            performed.append({"action": action["action"], "kind": kind,
                              "id": item_id, "path": action["path"], "ok": ok})
            if not ok:
                conflicts.append(f"could not materialize {action['path']}")
        if args.prune:
            for entry in pending_prune:
                deleted, markers = prune_entry(entry)
                performed.append({
                    "action": "delete",
                    "kind": entry.get("kind") or "skill",
                    "id": entry.get("id"),
                    "path": entry.get("path"),
                    "ok": True,
                    "deleted": bool(deleted),
                    "markers": len(markers),
                })
        if lock_changed:
            save_lock(entries)

        # Bring the resolver itself along, not only the selected items: a repo
        # that installs anything should be able to run the catalog (and create
        # the platform mirrors) without the extension. Missing files only here;
        # an explicit update to a release refreshes existing ones.
        install_tooling(source_root, overwrite=False, ref=_source_ref(manifest))
        install_agent_config(source_root, manifest)
        if fresh:
            # A repository with no manifest is a new project: give it the full
            # supporting set. Once a manifest exists, sync leaves it alone.
            install_scaffold(source_root)
        ensure_gitignore()

        # Installing a kind for the first time creates `.ai/<kind>/`, which the
        # platform mirrors need to see.
        sync_platform_mirrors(manifest)

    update_instruction_docs()

    if args.json:
        print(_dumps({
            "command": "sync",
            "mode": "dry-run" if args.dry_run else "apply",
            "sameDir": same_dir,
            "actions": actions,
            "performed": performed,
            "pendingPrune": [entry.get("path") for entry in pending_prune],
            "preservedUnknown": [entry.get("id") for entry in preserved_unknown],
            "legacyMarkers": [str(path.relative_to(_root())) for path in marker_files],
            "markersRemoved": markers_removed,
            "legacyIndexRemoved": old_index is not None and not old_index.exists(),
            "pruned": bool(args.prune),
            "lockChanged": lock_changed,
            "conflicts": conflicts,
        }))
        return 1 if conflicts else 0

    _print_sync_actions(actions, pending_prune, same_dir)
    prefix = "[DRY RUN] " if args.dry_run else ""
    touched = len([a for a in actions if a["action"] in ("create", "update")])
    if pending_prune and not args.prune:
        print(f"{prefix}{len(pending_prune)} item(s) pending prune "
              f"(run 'catalog.py sync --prune')")
    if preserved_unknown:
        print(f"{prefix}{len(preserved_unknown)} lock entry/entries of an unknown "
              f"kind preserved (not pruned)")
    if marker_files:
        verb = "would be cleaned" if args.dry_run else f"cleaned ({markers_removed})"
        print(f"{prefix}{len(marker_files)} legacy marker(s) {verb}")
    if old_index is not None:
        verb = "would be removed" if args.dry_run else "removed"
        print(f"{prefix}legacy index {verb}: {old_index.relative_to(_root())}")
    if args.dry_run:
        print(f"{prefix}{touched} item(s) would change; nothing written")
    else:
        print(f"{prefix}{touched} item(s) changed; "
              f"{'lock updated' if lock_changed else 'lock unchanged'}")
    for conflict in conflicts:
        print(f"CONFLICT: {conflict}", file=sys.stderr)
    return 1 if conflicts else 0


def _print_sync_actions(actions: list, pending_prune: list, same_dir: bool) -> None:
    if same_dir:
        print("no-op: source == target")
    for action in actions:
        name = action["action"].upper()
        detail = f" ({action['detail']})" if action.get("detail") else ""
        print(f"  {name:<8} {action['path']}{detail}")
    for entry in pending_prune:
        print(f"  {'PRUNE':<8} {entry.get('path')} (not selected; needs --prune)")


# ─── CLI ─────────────────────────────────────────────────────────────────────

def _common(parser: argparse.ArgumentParser) -> None:
    parser.add_argument("--json", action="store_true", help="Emit machine-readable JSON")
    parser.add_argument("--version", action="version", version=f"catalog.py {VERSION}")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        prog="catalog.py",
        description="Per-repo skill/agent selection resolver (offline, Phase 0)",
    )
    parser.add_argument("--version", action="version", version=f"catalog.py {VERSION}")
    subparsers = parser.add_subparsers(dest="command", required=True)

    index_parser = subparsers.add_parser("index", help="Build the catalog index")
    index_parser.add_argument("--out", default=None,
                              help="Where to write the index (default: "
                                   ".ai/tmp/catalog-index.json, a gitignored cache)")
    index_parser.add_argument("--verify", action="store_true",
                              help="Exit non-zero if the generated index is stale")
    index_parser.add_argument("--compact", action="store_true",
                              help="Also write a one-line-per-item markdown index next to it")
    index_parser.add_argument("--compact-out", default=None,
                              help="Where to write the compact index (default: INDEX.md beside --out)")
    _common(index_parser)
    index_parser.set_defaults(handler=cmd_index)

    add_parser = subparsers.add_parser("add", help="Add ids to .ai/ai.json")
    add_parser.add_argument("ids", nargs="+", help="Selection ids (e.g. council or template:council)")
    add_parser.add_argument("--kind", choices=list(KINDS), default=None,
                            help="Disambiguate an id that exists as both a skill and an agent")
    _common(add_parser)
    add_parser.set_defaults(handler=cmd_add)

    remove_parser = subparsers.add_parser("remove", help="Remove ids from .ai/ai.json")
    remove_parser.add_argument("ids", nargs="+", help="Selection ids to remove")
    remove_parser.add_argument("--kind", choices=list(KINDS), default=None,
                              help="Only remove ids of this kind")
    remove_parser.add_argument("--prune", action="store_true",
                               help="Also delete lock-recorded materialised files")
    remove_parser.add_argument("--force", action="store_true",
                               help="Remove even if something selected still requires it "
                                    "(a later sync restores it as a dependency)")
    _common(remove_parser)
    remove_parser.set_defaults(handler=cmd_remove)

    lock_parser = subparsers.add_parser(
        "lock", help="Freeze selected items so update and sync never overwrite them")
    lock_parser.add_argument("ids", nargs="+", help="Selection ids to lock")
    _common(lock_parser)
    lock_parser.set_defaults(handler=cmd_lock)

    unlock_parser = subparsers.add_parser(
        "unlock", help="Let a locked item follow the catalog again")
    unlock_parser.add_argument("ids", nargs="+", help="Selection ids to unlock")
    _common(unlock_parser)
    unlock_parser.set_defaults(handler=cmd_unlock)

    list_parser = subparsers.add_parser("list", help="List installed and available items")
    list_parser.add_argument("--installed", action="store_true", help="Only selected items")
    list_parser.add_argument("--available", action="store_true", help="Only catalog items")
    _common(list_parser)
    list_parser.set_defaults(handler=cmd_list)

    notice_parser = subparsers.add_parser(
        "notice", help="Write NOTICE.md attribution for the installed set")
    notice_parser.add_argument("--out", default="NOTICE.md",
                               help="Where to write (default: NOTICE.md)")
    notice_parser.add_argument("--verify", action="store_true",
                               help="Exit 1 if the file is stale")
    _common(notice_parser)
    notice_parser.set_defaults(handler=cmd_notice)

    browse_parser = subparsers.add_parser(
        "browse", help="One payload for the UI: catalog + selection + state")
    browse_parser.add_argument("--refresh", action="store_true",
                               help="Re-fetch remote sources instead of using the cache")
    _common(browse_parser)
    browse_parser.set_defaults(handler=cmd_browse)

    presets_parser = subparsers.add_parser("presets", help="List presets with resolved counts")
    _common(presets_parser)
    presets_parser.set_defaults(handler=cmd_presets)

    preset_parser = subparsers.add_parser(
        "preset", help="Add or remove presets without replacing the selection")
    preset_sub = preset_parser.add_subparsers(dest="preset_action", required=True)
    preset_add = preset_sub.add_parser(
        "add", help="Add presets to the current selection (keeps existing items)")
    preset_add.add_argument("names", nargs="+", metavar="name")
    _common(preset_add)
    preset_add.set_defaults(handler=cmd_preset)
    preset_remove = preset_sub.add_parser(
        "remove", help="Remove presets from the current selection (keeps other items)")
    preset_remove.add_argument("names", nargs="+", metavar="name")
    _common(preset_remove)
    preset_remove.set_defaults(handler=cmd_preset)

    release_parser = subparsers.add_parser(
        "release", help="Report newer release tags for the declared sources")
    _common(release_parser)
    release_parser.set_defaults(handler=cmd_release)

    pin_parser = subparsers.add_parser(
        "pin", help="Point the template source at a release, keeping the selection")
    pin_parser.add_argument("--release", default=None,
                            help="Release tag to pin (default: the newest published)")
    pin_parser.add_argument("--repo", default=None,
                            help="Repository to pin (default: the manifest's current repo)")
    _common(pin_parser)
    pin_parser.set_defaults(handler=cmd_pin)

    adopt_parser = subparsers.add_parser(
        "adopt", help="Write .ai/ai.json from what is already in this repository")
    _common(adopt_parser)
    adopt_parser.set_defaults(handler=cmd_adopt)

    bootstrap_parser = subparsers.add_parser(
        "bootstrap", help="Set this repository up from the template in one call")
    _common(bootstrap_parser)
    bootstrap_parser.set_defaults(handler=cmd_bootstrap)

    scaffold_parser = subparsers.add_parser(
        "scaffold", help="Copy any missing template .ai/ folders and files")
    _common(scaffold_parser)
    scaffold_parser.set_defaults(handler=cmd_scaffold)

    clients_parser = subparsers.add_parser(
        "clients", help="Show or set the AI clients (Claude, Copilot, Reasonix)")
    clients_parser.add_argument(
        "--set", dest="set_clients", nargs="*", default=None, metavar="client",
        help="Set the complete list: claude, copilot, reasonix")
    _common(clients_parser)
    clients_parser.set_defaults(handler=cmd_clients)

    project_parser = subparsers.add_parser(
        "project", help="Read or write the project name and intention")
    project_parser.add_argument("--name", default=None,
                                help="Project name (default: solution or folder name)")
    project_parser.add_argument("--intention", default=None,
                                help="What this project is for")
    _common(project_parser)
    project_parser.set_defaults(handler=cmd_project)

    init_parser = subparsers.add_parser(
        "init", help="Create .ai/ai.json for a new project and sync it")
    init_parser.add_argument("--preset", default=None, help="Preset name to start from")
    init_parser.add_argument("--all", action="store_true",
                             help="Install everything the catalog offers (preset: Everything)")
    init_parser.add_argument("--ids", nargs="*", default=None, help="Explicit skill ids")
    init_parser.add_argument("--agents", nargs="*", default=None, help="Explicit agent ids")
    init_parser.add_argument("--repo", default=None, help="Source repository (owner/name)")
    init_parser.add_argument("--ref", default=None, help="Pinned source ref")
    init_parser.add_argument("--path", default=None,
                             help="Local checkout of the source to resolve from")
    init_parser.add_argument("--materialize", choices=["link", "copy"], default=None)
    init_parser.add_argument("--force", action="store_true", help="Replace an existing manifest")
    _common(init_parser)
    init_parser.set_defaults(handler=cmd_init)

    update_parser = subparsers.add_parser(
        "update", help="Report or apply catalog changes since the lock")
    update_parser.add_argument("--apply", action="store_true", help="Write the updates")
    update_parser.add_argument("--force", action="store_true",
                               help="Also overwrite items modified locally")
    update_parser.add_argument("--tooling", action="store_true",
                               help="Also refresh the resolver scripts and vocabulary "
                                    "from the pinned source")
    update_parser.add_argument("ids", nargs="*", help="Limit to these ids")
    _common(update_parser)
    update_parser.set_defaults(handler=cmd_update)

    sync_parser = subparsers.add_parser("sync", help="Reconcile the tree with manifest + lock")
    sync_parser.add_argument("--check", action="store_true",
                             help="Report drift and exit 1; writes nothing")
    sync_parser.add_argument("--frozen", action="store_true",
                             help="Exit 1 if the lock would need to change")
    sync_parser.add_argument("--prune", action="store_true",
                             help="Delete lock-recorded paths that are no longer selected")
    sync_parser.add_argument("--dry-run", action="store_true", help="Show changes without writing")
    sync_parser.add_argument("--materialize", choices=["copy", "link"], default=None,
                             help="Override the manifest's materialize mode for this run")
    _common(sync_parser)
    sync_parser.set_defaults(handler=cmd_sync)

    status_parser = subparsers.add_parser("status", help="Per-item drift status")
    _common(status_parser)
    status_parser.set_defaults(handler=cmd_status)

    return parser


def main(argv: list | None = None) -> None:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    if hasattr(sys.stderr, "reconfigure"):
        sys.stderr.reconfigure(encoding="utf-8", errors="replace")
    args = build_parser().parse_args(argv)
    sys.exit(args.handler(args))


if __name__ == "__main__":
    main()

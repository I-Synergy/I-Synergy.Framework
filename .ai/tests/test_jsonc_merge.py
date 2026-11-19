"""Tests for the comment-preserving JSONC merger and the .vscode / .claude merge policies.

Covers .ai/scripts/jsonc_merge.py (text surgery) and the merge functions in
.ai/scripts/upgrade-template.py that apply the rules in
.ai/plans/vscode-settings-merge.md. Every test works in a temporary directory.
"""

import importlib.util
import json
import sys
from pathlib import Path

import pytest

TESTS_DIR = Path(__file__).resolve().parent
TEMPLATE_ROOT = TESTS_DIR.parent.parent
SCRIPTS_DIR = TEMPLATE_ROOT / ".ai" / "scripts"

if str(SCRIPTS_DIR) not in sys.path:
    sys.path.append(str(SCRIPTS_DIR))
import jsonc_merge as jm  # noqa: E402

_spec = importlib.util.spec_from_file_location("upgrade_template", SCRIPTS_DIR / "upgrade-template.py")
upgrade = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(upgrade)


# ─── Fixtures: a small synthetic template ────────────────────────────────────

FRAGMENT = {
    "chat.useAgentsMdFile": False,
    "chat.useClaudeHooks": True,
    "editor.tabSize": 4,
    "terminal.integrated.env.windows": {
        "MSBUILDDISABLENODEREUSE": "1",
        "UseSharedCompilation": "false",
    },
}

TASKS = {
    "version": "2.0.0",
    "tasks": [
        {"label": "Link", "type": "shell", "command": "python", "args": [".ai/scripts/sync-platforms.py"]},
    ],
}

CLAUDE = {
    "plansDirectory": "./.ai/plans",
    "env": {"MSBUILDDISABLENODEREUSE": "1"},
    "hooks": {
        "SessionStart": [
            {"matcher": "startup", "hooks": [
                {"type": "command", "command": "bash .ai/scripts/hooks/py.sh session_start.py"},
            ]},
        ],
    },
    "permissions": {
        "allow": ["Bash(dotnet:*)"],
        "deny": ["Bash(rm -rf:*)"],
        "additionalDirectories": ["./.ai"],
    },
}

RULES = {
    "version": 1,
    "vscode": {"pinnedSettings": ["chat.useAgentsMdFile", "chat.useClaudeHooks"]},
    "claude": {"legacyHookCommands": ["python .ai/scripts/sync-platforms.py"]},
}


def _write(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


@pytest.fixture
def template(tmp_path: Path) -> Path:
    src = tmp_path / "template"
    _write(src / ".ai/reference/templates/vscode/settings.json", json.dumps(FRAGMENT, indent=4))
    _write(src / ".ai/reference/templates/vscode/tasks.json", json.dumps(TASKS, indent=2))
    _write(src / ".ai/reference/templates/merge-rules.json", json.dumps(RULES, indent=2))
    _write(src / ".claude/settings.json", json.dumps(CLAUDE, indent=2))
    return src


@pytest.fixture
def project(tmp_path: Path) -> Path:
    tgt = tmp_path / "project"
    tgt.mkdir()
    return tgt


def _merge_all(src: Path, tgt: Path, dry_run: bool = False):
    vscode = upgrade.merge_vscode_files(src, tgt, dry_run)
    claude = upgrade.merge_settings_json(src, tgt, dry_run)
    return {o.rel: o for o in vscode + ([claude] if claude else [])}


def _snapshot(root: Path) -> dict:
    return {p.relative_to(root).as_posix(): p.read_bytes() for p in root.rglob("*") if p.is_file()}


# ─── jsonc_merge: text surgery ───────────────────────────────────────────────

def test_comments_and_trailing_commas_preserved():
    text = '{\n  // keep me\n  "editor.tabSize": 2, /* inline */\n  "list": [1, 2,],\n}\n'
    out = jm.add_member(text, (), "new.key", True)
    assert "// keep me" in out
    assert "/* inline */" in out
    assert "[1, 2,]" in out
    loaded = jm.load(out)
    assert loaded == {"editor.tabSize": 2, "list": [1, 2], "new.key": True}


def test_same_line_comment_stays_with_its_entry():
    text = '{\n    "a": 1, // about a\n    "b": 2\n}\n'
    out = jm.add_member(text, (), "c", 3)
    assert '"a": 1, // about a\n' in out
    assert jm.load(out) == {"a": 1, "b": 2, "c": 3}


def test_empty_object_and_empty_array_get_members():
    assert jm.load(jm.add_member("{}\n", (), "k", {"n": [1]})) == {"k": {"n": [1]}}
    out = jm.append_element('{\n  "tasks": []\n}\n', ("tasks",), {"label": "x"})
    assert jm.load(out) == {"tasks": [{"label": "x"}]}


def test_array_append_and_remove_keep_the_rest():
    text = '{\n  "items": [\n    "a", // first\n    "b",\n  ],\n}\n'
    out = jm.append_element(text, ("items",), "c")
    assert "// first" in out
    assert jm.load(out)["items"] == ["a", "b", "c"]
    out = jm.remove_element(out, ("items",), 0)
    assert "// first" not in out  # the removed element's own comment goes with it
    assert jm.load(out)["items"] == ["b", "c"]


def test_crlf_newlines_are_kept():
    text = '{\r\n    "editor.tabSize": 2\r\n}\r\n'
    out = jm.add_member(text, (), "x", 1)
    assert out.count("\n") == out.count("\r\n")
    assert jm.load(out) == {"editor.tabSize": 2, "x": 1}


def test_set_member_replaces_value_only():
    text = '{\n    "chat.useClaudeHooks": false, // mine\n}\n'
    out = jm.set_member(text, (), "chat.useClaudeHooks", True)
    assert out == '{\n    "chat.useClaudeHooks": true, // mine\n}\n'


def test_invalid_json_is_rejected():
    for bad in ['{"a": 1', '{"a" 1}', "[1 2]", '{"a":1} x', "", '{"a": 1, "a": 2}']:
        with pytest.raises(jm.JsoncError):
            jm.load(bad)


# ─── .vscode/settings.json ───────────────────────────────────────────────────

def test_nested_env_keeps_pip_config_and_adds_msbuild_keys(template, project):
    _write(project / ".vscode/settings.json", (
        "{\n"
        '    "terminal.integrated.env.windows": {\n'
        '        "PIP_CONFIG_FILE": "${workspaceFolder}/pip.ini",\n'
        "    },\n"
        "}\n"
    ))
    outcomes = _merge_all(template, project)
    env = jm.load((project / ".vscode/settings.json").read_text(encoding="utf-8"))["terminal.integrated.env.windows"]
    assert env == {
        "PIP_CONFIG_FILE": "${workspaceFolder}/pip.ini",
        "MSBUILDDISABLENODEREUSE": "1",
        "UseSharedCompilation": "false",
    }
    assert outcomes[".vscode/settings.json"].status == "merged"


def test_project_wins_on_non_pinned_conflict(template, project):
    _write(project / ".vscode/settings.json", '{\n    "editor.tabSize": 2\n}\n')
    outcome = _merge_all(template, project)[".vscode/settings.json"]
    loaded = jm.load((project / ".vscode/settings.json").read_text(encoding="utf-8"))
    assert loaded["editor.tabSize"] == 2
    assert outcome.conflicts == ["editor.tabSize"]


def test_pinned_key_is_replaced_and_comment_kept(template, project):
    _write(project / ".vscode/settings.json", (
        "{\n"
        '    "chat.useClaudeHooks": false, // project choice\n'
        "}\n"
    ))
    outcome = _merge_all(template, project)[".vscode/settings.json"]
    text = (project / ".vscode/settings.json").read_text(encoding="utf-8")
    assert jm.load(text)["chat.useClaudeHooks"] is True
    assert "// project choice" in text
    assert outcome.pinned == ["chat.useClaudeHooks"]


def test_missing_settings_file_is_created_from_fragment(template, project):
    outcome = _merge_all(template, project)[".vscode/settings.json"]
    assert outcome.status == "created"
    assert jm.load((project / ".vscode/settings.json").read_text(encoding="utf-8")) == FRAGMENT


def test_bom_is_preserved(template, project):
    _write(project / ".vscode/settings.json", "")
    (project / ".vscode/settings.json").write_bytes(
        b"\xef\xbb\xbf" + b'{\n    "editor.tabSize": 2\n}\n')
    _merge_all(template, project)
    assert (project / ".vscode/settings.json").read_bytes().startswith(b"\xef\xbb\xbf")


# ─── .vscode/tasks.json ──────────────────────────────────────────────────────

def test_tasks_merged_by_label_without_duplicates(template, project):
    _write(project / ".vscode/tasks.json", (
        "{\n"
        '  // project tasks\n'
        '  "tasks": [\n'
        '    {"label": "Build", "type": "shell", "command": "dotnet build"},\n'
        "  ],\n"
        "}\n"
    ))
    _merge_all(template, project)
    text = (project / ".vscode/tasks.json").read_text(encoding="utf-8")
    loaded = jm.load(text)
    labels = [t["label"] for t in loaded["tasks"]]
    assert labels == ["Build", "Link"]
    assert loaded["version"] == "2.0.0"
    assert "// project tasks" in text

    again = _merge_all(template, project)[".vscode/tasks.json"]
    assert again.status == "unchanged"
    assert [t["label"] for t in jm.load((project / ".vscode/tasks.json").read_text(encoding="utf-8"))["tasks"]] == labels


def test_existing_task_with_same_label_is_kept(template, project):
    _write(project / ".vscode/tasks.json", json.dumps({
        "version": "2.0.0",
        "tasks": [{"label": "Link", "type": "shell", "command": "mine"}],
    }))
    _merge_all(template, project)
    loaded = jm.load((project / ".vscode/tasks.json").read_text(encoding="utf-8"))
    assert loaded["tasks"] == [{"label": "Link", "type": "shell", "command": "mine"}]


# ─── .claude/settings.json ───────────────────────────────────────────────────

def test_legacy_hook_removed_new_hook_added_project_hook_kept(template, project):
    _write(project / ".claude/settings.json", json.dumps({
        "hooks": {
            "SessionStart": [
                {"hooks": [{"type": "command", "command": "python .ai/scripts/sync-platforms.py"}]},
                {"hooks": [{"type": "command", "command": "make lint"}]},
            ],
        },
    }, indent=2))
    outcome = _merge_all(template, project)[".claude/settings.json"]
    loaded = jm.load((project / ".claude/settings.json").read_text(encoding="utf-8"))
    commands = [h["command"] for g in loaded["hooks"]["SessionStart"] for h in g["hooks"]]
    assert "python .ai/scripts/sync-platforms.py" not in commands
    assert "make lint" in commands
    assert "bash .ai/scripts/hooks/py.sh session_start.py" in commands
    assert outcome.removed and outcome.added


def test_permissions_are_unioned_in_project_order(template, project):
    _write(project / ".claude/settings.json", json.dumps({
        "permissions": {"allow": ["Bash(git:*)", "Bash(dotnet:*)"]},
    }, indent=2))
    _merge_all(template, project)
    perms = jm.load((project / ".claude/settings.json").read_text(encoding="utf-8"))["permissions"]
    assert perms["allow"] == ["Bash(git:*)", "Bash(dotnet:*)"]
    assert perms["deny"] == ["Bash(rm -rf:*)"]
    assert perms["additionalDirectories"] == ["./.ai"]


def test_claude_project_value_wins_on_conflict(template, project):
    _write(project / ".claude/settings.json", json.dumps({"plansDirectory": "./docs/plans"}))
    outcome = _merge_all(template, project)[".claude/settings.json"]
    loaded = jm.load((project / ".claude/settings.json").read_text(encoding="utf-8"))
    assert loaded["plansDirectory"] == "./docs/plans"
    assert outcome.conflicts == ["plansDirectory"]


def test_missing_claude_settings_is_copied(template, project):
    outcome = _merge_all(template, project)[".claude/settings.json"]
    assert outcome.status == "created"
    assert jm.load((project / ".claude/settings.json").read_text(encoding="utf-8"))["plansDirectory"] == "./.ai/plans"


# ─── Errors, dry run, idempotence ────────────────────────────────────────────

def test_invalid_json_writes_nothing(template, project):
    broken_settings = '{\n    "editor.tabSize": 2\n'
    _write(project / ".vscode/settings.json", broken_settings)
    _write(project / ".vscode/tasks.json", '{"tasks": []}\n')
    _write(project / ".claude/settings.json", '{"hooks": [')
    before = {rel: (project / rel).read_bytes() for rel in (".vscode/settings.json", ".claude/settings.json")}
    outcomes = _merge_all(template, project)
    assert outcomes[".vscode/settings.json"].status == "error"
    assert outcomes[".claude/settings.json"].status == "error"
    after = {rel: (project / rel).read_bytes() for rel in before}
    assert after == before


def test_dry_run_writes_nothing(template, project):
    before = _snapshot(project)
    outcomes = _merge_all(template, project, dry_run=True)
    assert all(o.status == "created" for o in outcomes.values())
    assert _snapshot(project) == before


def test_second_run_is_a_byte_identical_noop(template, project):
    _write(project / ".vscode/settings.json", (
        "{\n"
        "    // comment\n"
        '    "chat.useClaudeHooks": false,\n'
        '    "terminal.integrated.env.windows": {"PIP_CONFIG_FILE": "pip.ini"},\n'
        "}\n"
    ))
    _write(project / ".vscode/tasks.json", '{"tasks": []}\n')
    _write(project / ".claude/settings.json", json.dumps({
        "hooks": {"SessionStart": [{"hooks": [{"type": "command", "command": "python .ai/scripts/sync-platforms.py"}]}]},
    }, indent=2))

    _merge_all(template, project)
    first = _snapshot(project)
    second_outcomes = _merge_all(template, project)
    assert _snapshot(project) == first
    assert all(o.status == "unchanged" for o in second_outcomes.values())


def test_legacy_fallback_matches_shipped_merge_rules():
    rules = json.loads((TEMPLATE_ROOT / ".ai/reference/templates/merge-rules.json").read_text(encoding="utf-8"))
    assert frozenset(rules["claude"]["legacyHookCommands"]) == upgrade.LEGACY_HOOK_COMMANDS_FALLBACK


def test_shipped_template_merges_into_empty_project_and_is_idempotent(tmp_path):
    project = tmp_path / "project"
    project.mkdir()
    first = _merge_all(TEMPLATE_ROOT, project)
    assert {o.status for o in first.values()} == {"created"}
    for rel in (".vscode/settings.json", ".vscode/tasks.json", ".claude/settings.json"):
        jm.load((project / rel).read_text(encoding="utf-8"))
    snapshot = _snapshot(project)
    second = _merge_all(TEMPLATE_ROOT, project)
    assert {o.status for o in second.values()} == {"unchanged"}
    assert _snapshot(project) == snapshot


# ─── Parity with the extension: created files and duplicate keys ─────────────


def test_missing_claude_settings_is_copied_byte_for_byte_with_every_key(template, project):
    text = '{\r\n  "plansDirectory": "./.ai/plans",\r\n  "enabledPlugins": {"x@y": true}\r\n}\r\n'
    (template / ".claude/settings.json").write_bytes(text.encode("utf-8"))
    outcome = _merge_all(template, project)[".claude/settings.json"]
    assert outcome.status == "created"
    assert (project / ".claude/settings.json").read_bytes() == text.encode("utf-8")


def test_missing_vscode_files_are_created_from_the_fragments_verbatim(template, project):
    for name in ("settings", "tasks"):
        src = template / f".ai/reference/templates/vscode/{name}.json"
        src.write_bytes(b'{\r\n  "version": "2.0.0", "tasks": []\r\n}\r\n'
                        if name == "tasks" else b'{\r\n  "a": 1\r\n}\r\n')
    _merge_all(template, project)
    assert (project / ".vscode/settings.json").read_bytes() == b'{\r\n  "a": 1\r\n}\r\n'
    assert (project / ".vscode/tasks.json").read_bytes().startswith(b'{\r\n  "version": "2.0.0", "tasks"')


@pytest.mark.parametrize("rel", [".vscode/settings.json", ".vscode/tasks.json", ".claude/settings.json"])
def test_duplicate_key_is_invalid_and_nothing_is_written(template, project, rel):
    broken = '{\n  "a": 1,\n  "a": 2\n}\n'
    _write(project / rel, broken)
    outcome = _merge_all(template, project)[rel]
    assert outcome.status == "error"
    assert (project / rel).read_text(encoding="utf-8") == broken


def test_duplicate_key_in_a_nested_object_is_invalid():
    with pytest.raises(jm.JsoncError):
        jm.load('{"env": {"A": 1, "A": 2}}')


# ─── Layout rules shared with the extension's C# merge ───────────────────────


def test_add_member_keeps_trailing_comma_style():
    assert jm.add_member('{\n  "a": 1,\n}\n', (), "b", 2) == '{\n  "a": 1,\n  "b": 2,\n}\n'


def test_add_member_keeps_line_comment_with_the_member_it_trails():
    assert jm.add_member('{\n  "a": 1 // last\n}\n', (), "b", 2) == '{\n  "a": 1, // last\n  "b": 2\n}\n'


def test_add_member_into_comment_only_object_goes_after_the_comment():
    assert jm.add_member('{\n  // nothing yet\n}\n', (), "a", 1) == '{\n  // nothing yet\n  "a": 1\n}\n'


def test_remove_element_on_a_shared_line_takes_the_blanks_after_it():
    assert jm.remove_element('{"l": [1, 2]}', ("l",), 0) == '{"l": [2]}'

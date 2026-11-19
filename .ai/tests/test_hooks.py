"""Unit tests for the shared hook scripts in .ai/scripts/hooks/ and the rules generator.

Nothing here touches repo files: ROOT-derived paths are redirected into tmp_path.
"""

import importlib.util
import io
import subprocess
import sys
from pathlib import Path

import pytest

TEMPLATE_ROOT = Path(__file__).resolve().parent.parent.parent
HOOKS_DIR = TEMPLATE_ROOT / ".ai" / "scripts" / "hooks"
SCRIPTS_DIR = TEMPLATE_ROOT / ".ai" / "scripts"


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


@pytest.fixture(scope="module")
def common():
    sys.path.insert(0, str(HOOKS_DIR))
    try:
        return _load("_common", HOOKS_DIR / "_common.py")
    finally:
        sys.path.remove(str(HOOKS_DIR))


@pytest.fixture()
def post_edit(common):
    sys.path.insert(0, str(HOOKS_DIR))
    sys.modules["_common"] = common
    try:
        return _load("post_edit_sync_under_test", HOOKS_DIR / "post_edit_sync.py")
    finally:
        sys.path.remove(str(HOOKS_DIR))


@pytest.fixture()
def session_start(common):
    sys.path.insert(0, str(HOOKS_DIR))
    sys.modules["_common"] = common
    try:
        return _load("session_start_under_test", HOOKS_DIR / "session_start.py")
    finally:
        sys.path.remove(str(HOOKS_DIR))


@pytest.fixture()
def stop_build(common):
    sys.path.insert(0, str(HOOKS_DIR))
    sys.modules["_common"] = common
    try:
        return _load("stop_build_under_test", HOOKS_DIR / "stop_build.py")
    finally:
        sys.path.remove(str(HOOKS_DIR))


@pytest.fixture(scope="module")
def sync_platforms():
    return _load("sync_platforms_under_test", SCRIPTS_DIR / "sync-platforms.py")


# --- payload / path extraction ------------------------------------------------

def test_extract_path_claude_shape(post_edit):
    payload = {"tool_input": {"file_path": "D:/repo/.ai/skills/x/SKILL.md"}}
    assert post_edit.extract_path(payload) == "D:/repo/.ai/skills/x/SKILL.md"


def test_extract_path_copilot_toolargs_json_string_windows_path(post_edit):
    payload = {"toolArgs": '{"filePath": "D:\\\\repo\\\\.ai\\\\skills\\\\x\\\\SKILL.md"}'}
    assert post_edit.extract_path(payload) == "D:\\repo\\.ai\\skills\\x\\SKILL.md"


def test_extract_path_copilot_toolargs_unescaped_backslashes(post_edit):
    payload = {"toolArgs": '{"path": "D:\\repo\\.ai\\skills\\x"}'}
    assert post_edit.extract_path(payload).endswith("skills\\x")


def test_extract_path_copilot_toolinput_dict(post_edit):
    payload = {"toolInput": {"filePath": ".ai/agents/tester.md"}}
    assert post_edit.extract_path(payload) == ".ai/agents/tester.md"


def test_extract_path_missing_returns_empty(post_edit):
    assert post_edit.extract_path({}) == ""
    assert post_edit.extract_path({"toolArgs": "not json"}) == ""
    assert post_edit.extract_path({"tool_input": ["x"]}) == ""


def test_watched_and_ignored_decision(post_edit):
    assert post_edit.is_watched(post_edit.normalize(".ai/skills/x/SKILL.md"))
    assert post_edit.is_watched(post_edit.normalize("./.ai/agents/a.md"))
    assert post_edit.is_watched(post_edit.normalize(".ai/instructions/c.instructions.md"))
    assert not post_edit.is_watched(post_edit.normalize("src/Foo.cs"))


def test_normalize_absolute_inside_repo(post_edit, common):
    absolute = str(common.ROOT / ".ai" / "skills" / "x" / "SKILL.md")
    assert post_edit.normalize(absolute) == ".ai/skills/x/SKILL.md"


def test_normalize_absolute_outside_repo_is_empty(post_edit, tmp_path):
    assert post_edit.normalize(str(tmp_path / "other.txt")) == ""


@pytest.mark.parametrize("stdin_text", ["", "   \n", "{not json", "[1, 2]", '"str"'])
def test_read_payload_bad_input_gives_empty_dict(common, monkeypatch, stdin_text):
    class FakeStdin(io.StringIO):
        def isatty(self):
            return False

    monkeypatch.setattr(sys, "stdin", FakeStdin(stdin_text))
    assert common.read_payload() == {}


def test_read_payload_valid_json(common, monkeypatch):
    class FakeStdin(io.StringIO):
        def isatty(self):
            return False

    monkeypatch.setattr(sys, "stdin", FakeStdin('{"a": 1}'))
    assert common.read_payload() == {"a": 1}


def test_post_edit_main_ignored_path_does_not_sync(post_edit, common, monkeypatch):
    class FakeStdin(io.StringIO):
        def isatty(self):
            return False

    called = []
    monkeypatch.setattr(post_edit._common, "run_sync", lambda: called.append(1) or (0, "", ""))
    monkeypatch.setattr(sys, "stdin", FakeStdin('{"tool_input": {"file_path": "src/Foo.cs"}}'))
    assert post_edit.main() == 0
    assert called == []


def test_post_edit_main_watched_path_syncs(post_edit, monkeypatch):
    class FakeStdin(io.StringIO):
        def isatty(self):
            return False

    called = []
    monkeypatch.setattr(post_edit._common, "run_sync", lambda: called.append(1) or (0, "", ""))
    monkeypatch.setattr(sys, "stdin", FakeStdin('{"tool_input": {"file_path": ".ai/skills/x/SKILL.md"}}'))
    assert post_edit.main() == 0
    assert called == [1]


def test_post_edit_main_empty_stdin_no_crash(post_edit, monkeypatch):
    class FakeStdin(io.StringIO):
        def isatty(self):
            return False

    monkeypatch.setattr(sys, "stdin", FakeStdin(""))
    assert post_edit.main() == 0


# --- stop_build ----------------------------------------------------------------

def test_stop_build_without_claude_dir_returns_zero_and_never_runs_dotnet(stop_build, monkeypatch):
    monkeypatch.delenv("CLAUDE_PROJECT_DIR", raising=False)
    calls = []
    monkeypatch.setattr(subprocess, "run", lambda *a, **k: calls.append(a) or None)
    assert stop_build.main() == 0
    assert calls == []


def test_stop_build_without_solution_does_not_run_dotnet(stop_build, monkeypatch, tmp_path):
    monkeypatch.setenv("CLAUDE_PROJECT_DIR", str(tmp_path))
    monkeypatch.setattr(stop_build, "ROOT", tmp_path)
    calls = []
    monkeypatch.setattr(subprocess, "run", lambda *a, **k: calls.append(a) or None)
    assert stop_build.main() == 0
    assert calls == []


# --- session_start --------------------------------------------------------------

def _redirect_session(session_start, tmp_path, monkeypatch, content):
    session_file = tmp_path / "session-context.md"
    session_file.write_text(content, encoding="utf-8")
    completed = tmp_path / "completed"
    completed.mkdir()
    (completed / "done-task.md").write_text("x", encoding="utf-8")
    monkeypatch.setattr(session_start, "SESSION_FILE", session_file)
    monkeypatch.setattr(session_start, "COMPLETED_DIR", completed)


def test_session_start_skips_placeholder(session_start, tmp_path, monkeypatch, capsys):
    _redirect_session(session_start, tmp_path, monkeypatch, "# Session Context Template\n\nfill me\n")
    session_start.print_session_context()
    assert capsys.readouterr().out == ""


def test_session_start_prints_real_context(session_start, tmp_path, monkeypatch, capsys):
    _redirect_session(session_start, tmp_path, monkeypatch, "# Real Project\n\nNotes here\n")
    session_start.print_session_context()
    out = capsys.readouterr().out
    assert session_start.HEADER in out
    assert "Notes here" in out
    assert "done-task.md" in out


def test_session_start_caps_lines(session_start, tmp_path, monkeypatch, capsys):
    body = "# Real\n" + "".join(f"line {i}\n" for i in range(500))
    _redirect_session(session_start, tmp_path, monkeypatch, body)
    session_start.print_session_context()
    out = capsys.readouterr().out
    assert "line 100" in out
    assert "line 400" not in out


def test_session_start_main_never_touches_repo_files(session_start, tmp_path, monkeypatch, capsys):
    class FakeStdin(io.StringIO):
        def isatty(self):
            return False

    _redirect_session(session_start, tmp_path, monkeypatch, "# Real\n")
    monkeypatch.setattr(session_start._common, "PROBE_LOG", tmp_path / "probe.log")
    monkeypatch.setattr(session_start._common, "run_sync", lambda: (0, "", ""))
    monkeypatch.setattr(sys, "stdin", FakeStdin('{"hook_event_name": "SessionStart"}'))
    assert session_start.main() == 0
    log = (tmp_path / "probe.log").read_text(encoding="utf-8")
    assert "event=SessionStart" in log


# --- rules generator ------------------------------------------------------------

def _source(apply_to, description="Desc"):
    return f'---\ndescription: "{description}"\napplyTo: {apply_to}\n---\nBody text.\n'


def test_render_rule_single_glob_becomes_paths(sync_platforms):
    out = sync_platforms.render_rule("csharp", _source('"**/*.cs"'))
    assert 'paths:\n  - "**/*.cs"\n' in out
    assert 'description: "Desc"' in out
    assert out.endswith("Body text.\n")


def test_render_rule_comma_separated_globs(sync_platforms):
    out = sync_platforms.render_rule("multi", _source('"**/*.cs, **/*.csproj"'))
    assert '  - "**/*.cs"\n' in out
    assert '  - "**/*.csproj"\n' in out


def test_render_rule_match_everything_has_no_paths(sync_platforms):
    out = sync_platforms.render_rule("all", _source('"**/*"'))
    assert "paths:" not in out


def test_render_rule_has_generated_marker(sync_platforms):
    out = sync_platforms.render_rule("csharp", _source('"**/*.cs"'))
    assert sync_platforms._rule_marker("csharp") in out.split("\n")


@pytest.fixture()
def rules_sandbox(sync_platforms, tmp_path, monkeypatch):
    monkeypatch.setattr(sync_platforms, "SCRIPT_DIR", tmp_path)
    sources = tmp_path / sync_platforms.RULES_SOURCE
    sources.mkdir(parents=True)
    (sources / f"csharp{sync_platforms.RULE_SUFFIX}").write_text(_source('"**/*.cs"'), encoding="utf-8")
    return tmp_path


def test_sync_rules_writes_generated_file(sync_platforms, rules_sandbox):
    changed, conflicts, failures = sync_platforms.sync_rules(dry_run=False)
    target = rules_sandbox / sync_platforms.RULES_TARGET / f"csharp{sync_platforms.RULE_SUFFIX}"
    assert (changed, conflicts, failures) == (1, [], 0)
    assert sync_platforms._rule_marker("csharp") in target.read_text(encoding="utf-8")


def test_sync_rules_never_overwrites_hand_written_file(sync_platforms, rules_sandbox):
    target_dir = rules_sandbox / sync_platforms.RULES_TARGET
    target_dir.mkdir(parents=True)
    target = target_dir / f"csharp{sync_platforms.RULE_SUFFIX}"
    target.write_text("my own rule, no marker\n", encoding="utf-8")
    changed, conflicts, failures = sync_platforms.sync_rules(dry_run=False)
    assert changed == 0
    assert len(conflicts) == 1
    assert target.read_text(encoding="utf-8") == "my own rule, no marker\n"

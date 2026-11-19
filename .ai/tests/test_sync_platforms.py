"""Tests for the prompt-adapter rules in .ai/scripts/sync-platforms.py.

Prompt adapters (.claude/commands, .github/prompts) are linked to .ai/prompts/
only while that directory holds at least one file. Otherwise they are legacy:
removed when they are links or empty directories, and real content is reported
as CONFLICT and left in place. Every test works in a temporary directory; the
script's SCRIPT_DIR is redirected there, so the real repository is never touched.
"""

import importlib.util
import sys
from pathlib import Path

import pytest

TESTS_DIR = Path(__file__).resolve().parent
SCRIPT = TESTS_DIR.parent / "scripts" / "sync-platforms.py"

_spec = importlib.util.spec_from_file_location("sync_platforms_under_test", SCRIPT)
sp = importlib.util.module_from_spec(_spec)
sys.modules[_spec.name] = sp
_spec.loader.exec_module(sp)

CLAUDE_COMMANDS = ".claude/commands"
COPILOT_PROMPTS = ".github/prompts"


@pytest.fixture
def root(tmp_path, monkeypatch) -> Path:
    """A fake repository root. sync-platforms resolves every path against SCRIPT_DIR."""
    monkeypatch.setattr(sp, "SCRIPT_DIR", tmp_path)
    return tmp_path


def _add_prompt(root: Path, name: str = "sample.prompt.md") -> None:
    prompts = root / ".ai" / "prompts"
    prompts.mkdir(parents=True, exist_ok=True)
    (prompts / name).write_text("# sample prompt\n", encoding="utf-8")


def _legacy_link(root: Path, rel: str) -> None:
    """A link at rel pointing at a directory that holds nothing the template owns."""
    source = root / "legacy-source"
    source.mkdir(exist_ok=True)
    assert sp._create_link(root / rel, source), f"could not create link at {rel}"


def _is_link(path: Path) -> bool:
    return sp._is_link(path)


# ─── prompts_active ──────────────────────────────────────────────────────────

def test_prompts_inactive_when_dir_absent(root):
    assert sp.prompts_active() is False


def test_prompts_inactive_when_dir_empty(root):
    (root / ".ai" / "prompts").mkdir(parents=True)
    assert sp.prompts_active() is False


def test_prompts_inactive_when_only_subdirectories(root):
    (root / ".ai" / "prompts" / "empty-sub").mkdir(parents=True)
    assert sp.prompts_active() is False


def test_prompts_active_with_nested_file(root):
    nested = root / ".ai" / "prompts" / "group"
    nested.mkdir(parents=True)
    (nested / "deep.prompt.md").write_text("x", encoding="utf-8")
    assert sp.prompts_active() is True


# ─── no prompts: links are removed ───────────────────────────────────────────

@pytest.mark.parametrize("prompts_state", ["absent", "empty"])
def test_no_prompts_removes_prompt_links(root, prompts_state, capsys):
    if prompts_state == "empty":
        (root / ".ai" / "prompts").mkdir(parents=True)
    _legacy_link(root, CLAUDE_COMMANDS)
    _legacy_link(root, COPILOT_PROMPTS)

    assert sp.sync_all(dry_run=False, tools=("claude", "copilot")) == 0

    assert not _is_link(root / CLAUDE_COMMANDS)
    assert not (root / CLAUDE_COMMANDS).exists()
    assert not _is_link(root / COPILOT_PROMPTS)
    assert not (root / COPILOT_PROMPTS).exists()
    out = capsys.readouterr().out
    assert "prompt adapters are legacy" in out
    assert "REMOVE" in out


def test_no_prompts_removes_empty_prompt_directories(root):
    (root / CLAUDE_COMMANDS).mkdir(parents=True)
    (root / COPILOT_PROMPTS).mkdir(parents=True)

    assert sp.sync_all(dry_run=False, tools=("claude", "copilot")) == 0

    assert not (root / CLAUDE_COMMANDS).exists()
    assert not (root / COPILOT_PROMPTS).exists()


def test_no_prompts_still_removes_retired_instructions_link(root):
    _legacy_link(root, ".claude/instructions")
    _add_prompt(root)  # prompts state must not affect the always-legacy instructions path

    assert sp.sync_all(dry_run=False, tools=("claude",)) == 0

    assert not (root / ".claude" / "instructions").exists()
    assert not _is_link(root / ".claude" / "instructions")


# ─── prompts with files: links are created ───────────────────────────────────

def test_prompts_with_file_creates_both_links(root, capsys):
    _add_prompt(root)

    assert sp.sync_all(dry_run=False, tools=("claude", "copilot")) == 0

    prompts = root / ".ai" / "prompts"
    for rel in (CLAUDE_COMMANDS, COPILOT_PROMPTS):
        link = root / rel
        assert _is_link(link), f"{rel} should be a link"
        assert (link / "sample.prompt.md").is_file(), f"{rel} should expose the prompt file"
    assert "linking prompt adapters" in capsys.readouterr().out
    assert prompts.is_dir()


def test_prompts_link_is_idempotent(root, capsys):
    _add_prompt(root)
    assert sp.sync_all(dry_run=False, tools=("claude", "copilot")) == 0
    capsys.readouterr()

    assert sp.sync_all(dry_run=False, tools=("claude", "copilot")) == 0

    out = capsys.readouterr().out
    assert f"OK       {CLAUDE_COMMANDS} -> .ai/prompts" in out
    assert f"OK       {COPILOT_PROMPTS} -> .ai/prompts" in out


def test_prompts_respect_tool_selection(root):
    _add_prompt(root)

    assert sp.sync_all(dry_run=False, tools=("copilot",)) == 0

    assert _is_link(root / COPILOT_PROMPTS)
    assert not (root / CLAUDE_COMMANDS).exists()


def test_dry_run_creates_nothing(root):
    _add_prompt(root)

    assert sp.sync_all(dry_run=True, tools=("claude", "copilot")) == 0

    assert not (root / CLAUDE_COMMANDS).exists()
    assert not (root / COPILOT_PROMPTS).exists()


# ─── real content is never deleted ───────────────────────────────────────────

def test_real_prompts_dir_with_files_is_conflict_when_prompts_active(root, capsys):
    _add_prompt(root)
    real = root / COPILOT_PROMPTS
    real.mkdir(parents=True)
    (real / "hand-written.prompt.md").write_text("keep me", encoding="utf-8")

    assert sp.sync_all(dry_run=False, tools=("claude", "copilot")) == 0

    assert not _is_link(real), "a real directory must not be replaced by a link"
    assert (real / "hand-written.prompt.md").read_text(encoding="utf-8") == "keep me"
    assert f"CONFLICT {COPILOT_PROMPTS} -> .ai/prompts" in capsys.readouterr().out


def test_real_prompts_dir_with_files_is_conflict_when_prompts_inactive(root, capsys):
    real = root / COPILOT_PROMPTS
    real.mkdir(parents=True)
    (real / "hand-written.prompt.md").write_text("keep me", encoding="utf-8")

    assert sp.sync_all(dry_run=False, tools=("copilot",)) == 0

    assert (real / "hand-written.prompt.md").read_text(encoding="utf-8") == "keep me"
    assert "CONFLICT     .github/prompts" in capsys.readouterr().out


def test_real_claude_commands_with_files_is_conflict_and_kept(root):
    real = root / CLAUDE_COMMANDS
    real.mkdir(parents=True)
    (real / "old.md").write_text("keep me", encoding="utf-8")

    assert sp.sync_all(dry_run=False, tools=("claude",)) == 0

    assert (real / "old.md").read_text(encoding="utf-8") == "keep me"


def test_remove_legacy_targets_never_deletes_real_files(root):
    real = root / COPILOT_PROMPTS
    real.mkdir(parents=True)
    (real / "keep.md").write_text("x", encoding="utf-8")

    removed, conflicts, failures = sp.remove_legacy_targets(dry_run=False, prompts_on=False)

    assert (real / "keep.md").is_file()
    assert COPILOT_PROMPTS in conflicts
    assert failures == 0
    assert removed == 0

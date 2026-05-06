"""
Pytest wrappers for the Claude template validation suite.

Each test function runs one existing validation script as a subprocess and
fails with the script's output if it exits non-zero. This gives VS Code
Test Explorer one clickable entry per suite without rewriting the scripts.
"""

import subprocess
import sys
import shutil
from pathlib import Path
from typing import Optional

import pytest

TESTS_DIR = Path(__file__).parent
TEMPLATE_ROOT = TESTS_DIR.parent.parent


def _run_python(script: str) -> tuple[int, str]:
    result = subprocess.run(
        [sys.executable, str(TESTS_DIR / script)],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        cwd=str(TEMPLATE_ROOT),
    )
    return result.returncode, result.stdout + result.stderr


def _find_bash() -> Optional[str]:
    """Prefer Git Bash on Windows; WSL bash resolves drives at /mnt/d/ which
    breaks the SCRIPT_DIR-relative path logic inside the shell scripts."""
    if sys.platform == "win32":
        for candidate in [
            r"C:\Program Files\Git\bin\bash.exe",
            r"C:\Program Files\Git\usr\bin\bash.exe",
        ]:
            if Path(candidate).exists():
                return candidate
    return shutil.which("bash")


def _run_bash(script: str) -> tuple[int, str]:
    bash = _find_bash()
    if bash is None:
        pytest.skip("bash not available on PATH")
    # Pass script as a plain filename with cwd=TESTS_DIR so bash never
    # receives a Windows path with backslashes or drive letters.
    result = subprocess.run(
        [bash, script],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        cwd=str(TESTS_DIR),
    )
    return result.returncode, result.stdout + result.stderr


# ---------------------------------------------------------------------------
# Suite 1 — Directory Structure
# ---------------------------------------------------------------------------
def test_directory_structure():
    code, output = _run_bash("validate-structure.sh")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 2 — YAML Frontmatter
# ---------------------------------------------------------------------------
def test_yaml_frontmatter():
    code, output = _run_python("validate-skills.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 3 — File References
# ---------------------------------------------------------------------------
def test_file_references():
    code, output = _run_bash("validate-references.sh")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 4 — Content Quality
# ---------------------------------------------------------------------------
def test_content_quality():
    code, output = _run_python("validate-content.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 5 — Token Consistency
# ---------------------------------------------------------------------------
def test_token_consistency():
    code, output = _run_bash("validate-tokens.sh")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 6 — CLAUDE.md References
# ---------------------------------------------------------------------------
def test_claude_md_references():
    code, output = _run_python("validate-claude-md.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 7 — Settings & Structure
# ---------------------------------------------------------------------------
def test_settings_and_structure():
    code, output = _run_python("validate-settings.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 8 — Copilot Integration
# ---------------------------------------------------------------------------
def test_copilot_integration():
    code, output = _run_python("validate-copilot.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 9 — Integration Smoke Tests
# ---------------------------------------------------------------------------
def test_smoke():
    code, output = _run_python("smoke-test.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 10 — Upgrade Script
# ---------------------------------------------------------------------------
def test_upgrade_script():
    code, output = _run_python("validate-upgrade-script.py")
    assert code == 0, f"\n{output}"

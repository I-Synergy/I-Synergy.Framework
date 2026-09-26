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


# ---------------------------------------------------------------------------
# Suite 11 — Reasonix Integration
# ---------------------------------------------------------------------------
def test_reasonix_integration():
    code, output = _run_python("validate-reasonix.py")
    assert code == 0, f"\n{output}"

# ---------------------------------------------------------------------------
# Suite 12 — Pi Integration
# ---------------------------------------------------------------------------
def test_pi_integration():
    code, output = _run_python("validate-pi.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 13 — Traceability
# ---------------------------------------------------------------------------
def test_traceability():
    code, output = _run_python("validate-traceability.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 14 — Standards
# ---------------------------------------------------------------------------
def test_standards():
    code, output = _run_python("validate-standards.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Suite 15 — DeepSeek Parity
# ---------------------------------------------------------------------------
def test_deepseek_parity():
    code, output = _run_python("validate-deepseek-parity.py")
    assert code == 0, f"\n{output}"


# ---------------------------------------------------------------------------
# Fixture regression — validate-traceability.py's T1-T4, the source-link and document-scope
# checks, and the padded-child advisory
#
# Neither Wave-1 validator is exercised against real artifacts by anything here: this
# template ships no docs/ tree, no .cs and no .feature, so validate-traceability.py only
# ever runs its empty-repo path in the repository that owns it and could rot silently.
#
# So the fixture below is generated into tmp_path at runtime -- nothing is committed, no
# bash suite runs it, and no other validator sees it. The real script is copied into the
# fixture tree, which makes its repository root (`<root>/.ai/tests/../..`) the fixture:
# this tests the shipped script rather than a reimplementation of it, and it asserts the
# exit code, which is the evidence validate-standards.py computes a status from.
#
# The pytest wrapper is the only thing that runs this fixture, and `.github/workflows/`
# runs the wrapper as its own step -- without that step a regression here would leave CI
# green, which is the failure mode the fixture exists to prevent.
# ---------------------------------------------------------------------------

_TRACEABILITY_VALIDATOR = TESTS_DIR / "validate-traceability.py"

_FEATURE_FILE = """\
# File: docs/slices/Budget/Budget.Create/Budget.Create.feature

Feature: Record a budget

  @AC-Budget-014.1
  Scenario: Create a valid budget
    Given I am an authenticated user
    When I record a budget of 100.00
    Then the budget is stored

  @AC-Budget-014.2
  Scenario: Reject a non-positive amount
    Given I am an authenticated user
    When I record a budget of 0.00
    Then the creation fails with a validation error
"""

_BLUEPRINT = """\
{
  "slice": "Budget.Create",
  "bounded_context": "Budget",
  "operation_type": "Command",
  "entity": "Budget",
  "handler_class": "CreateBudgetCommandHandler",
  "source": {
    "use_cases": ["UC-Budget-007"],
    "user_stories": ["US-Budget-014"],
    "acceptance_criteria": ["AC-Budget-014.1", "AC-Budget-014.2"]
  }
}
"""

_SOURCE_LINK_LINES = (
    '    "use_cases": ["UC-Budget-007"],\n'
    '    "user_stories": ["US-Budget-014"],\n'
)

# The pre-convention blueprint shape: no `source` key at all.
_BLUEPRINT_NO_SOURCE = """\
{
  "slice": "Budget.Create",
  "bounded_context": "Budget",
  "operation_type": "Command",
  "entity": "Budget",
  "handler_class": "CreateBudgetCommandHandler"
}
"""

_USE_CASES = """\
# Use Cases — Budget

### UC-Budget-007: Record a budget

**Actor:** Household planner
"""

# T7 is scoped to the story and use-case documents. These two sit in the same bounded context,
# hold no stories, and used to be swept in by a bare `stor` substring match on the file stem --
# `history.md` and `storage-notes.md` both warn about "0 of 1 stories" under it.
_CONTEXT_NOTES = {
    "docs/bounded-contexts/Budget/history.md": """\
# Budget Context History

### 2026-01-01 — Extracted from the monolith

The budget context was carved out of the monolith in this change.
""",
    "docs/bounded-contexts/Budget/storage-notes.md": """\
# Storage Notes

### Serialization

Budgets are stored as JSON.
""",
}

# A real story document with no identifiers at all: the legacy mode T7 exists for.
_LEGACY_STORY = """\
# User Stories — Budget (pre-convention)

### Record a budget

**As a** household planner
**I want** to record a monthly budget
**So that** I can track spending against it
"""


def _padded(text: str) -> str:
    """`AC-Budget-014.1` -> `AC-Budget-014.01`: `{n}` is unpadded, and this is the drift."""
    return text.replace("AC-Budget-014.1", "AC-Budget-014.01").replace(
        "AC-Budget-014.2", "AC-Budget-014.02")


def _story(*, duplicate_criterion: bool) -> str:
    criteria = [
        "- [ ] AC-Budget-014.1: A budget is created and stored with a unique identifier.",
        "- [ ] AC-Budget-014.2: A non-positive amount is rejected with a validation error.",
    ]
    if duplicate_criterion:
        criteria.append(criteria[0])
    return "\n".join([
        "# User Stories — Budget",
        "",
        "### US-Budget-014: Record a budget",
        "",
        "**As a** household planner",
        "**I want** to record a monthly budget",
        "**So that** I can track spending against it",
        "",
        "**Acceptance Criteria:**",
        *criteria,
        "",
        "**Gherkin:**",
        "```gherkin",
        "Feature: Record a budget",
        "",
        "  @AC-Budget-014.1",
        "  Scenario: Create a valid budget",
        "    Given I am an authenticated user",
        "    When I record a budget of 100.00",
        "    Then the budget is stored",
        "",
        "  @AC-Budget-014.2",
        "  Scenario: Reject a non-positive amount",
        "    Given I am an authenticated user",
        "    When I record a budget of 0.00",
        "    Then the creation fails with a validation error",
        "```",
        "",
    ])


def _test_class(extra_methods: str) -> str:
    return (
        "// File: tests/Acme.Budget.Tests/Budget/CreateBudgetCommandHandlerTests.cs\n"
        "// Traces: US-Budget-014 / AC-Budget-014.1, AC-Budget-014.2\n"
        "\n"
        "using Microsoft.VisualStudio.TestTools.UnitTesting;\n"
        "\n"
        "namespace Acme.Budget.Tests.Budget;\n"
        "\n"
        "[TestClass]\n"
        "public class CreateBudgetCommandHandlerTests\n"
        "{\n"
        "    [TestMethod]\n"
        '    [TestCategory("AC-Budget-014.1")]\n'
        "    public void HandleAsync_ValidCommand_CreatesBudgetSuccessfully()\n"
        "    {\n"
        "    }\n"
        "\n"
        "    [TestMethod]\n"
        '    [TestCategory("AC-Budget-014.2")]\n'
        "    public void HandleAsync_NonPositiveAmount_ThrowsArgumentException()\n"
        "    {\n"
        "    }\n"
        f"{extra_methods}"
        "}\n"
    )


def _fixture_files(*, duplicate_criterion: bool = False, unknown_tag: str = "",
                   unknown_slice_ref: str = "", unknown_test_category: str = "",
                   unknown_story_ref: str = "", unknown_use_case_ref: str = "",
                   drop_source_links: bool = False, no_source_key: bool = False,
                   padded: bool = False, legacy_story_document: bool = False) -> dict:
    """The clean fixture, with at most one defect planted in it."""
    feature = _FEATURE_FILE
    if unknown_tag:
        feature += (
            f"\n  @{unknown_tag}\n"
            f"  Scenario: A tag no document declares\n"
            f"    Given I am an authenticated user\n"
            f"    Then nothing happens\n"
        )

    blueprint = _BLUEPRINT
    if unknown_slice_ref:
        blueprint = blueprint.replace(
            '["AC-Budget-014.1", "AC-Budget-014.2"]',
            f'["AC-Budget-014.1", "AC-Budget-014.2", "{unknown_slice_ref}"]',
        )
    if unknown_story_ref:
        blueprint = blueprint.replace(
            '["US-Budget-014"]', f'["US-Budget-014", "{unknown_story_ref}"]')
    if unknown_use_case_ref:
        blueprint = blueprint.replace(
            '["UC-Budget-007"]', f'["UC-Budget-007", "{unknown_use_case_ref}"]')
    if drop_source_links:
        # `source` still present, naming neither a story nor a use case: untraceable by
        # construction, which T3 reports rather than passing in silence.
        blueprint = blueprint.replace(_SOURCE_LINK_LINES, "")
    if no_source_key:
        # No `source` key at all: the pre-convention shape, read as a legacy slice.
        blueprint = _BLUEPRINT_NO_SOURCE

    extra_methods = ""
    if unknown_test_category:
        extra_methods = (
            "\n"
            "    [TestMethod]\n"
            f'    [TestCategory("{unknown_test_category}")]\n'
            "    public void HandleAsync_NoCriterionDeclaresThis()\n"
            "    {\n"
            "    }\n"
        )

    files = {
        "docs/bounded-contexts/Budget/user-stories.md": _story(
            duplicate_criterion=duplicate_criterion),
        "docs/bounded-contexts/Budget/use-cases.md": _USE_CASES,
        **_CONTEXT_NOTES,
        "docs/slices/Budget/Budget.Create/blueprint.json": blueprint,
        "docs/slices/Budget/Budget.Create/Budget.Create.feature": feature,
        "tests/Acme.Budget.Tests/Budget/CreateBudgetCommandHandlerTests.cs": _test_class(
            extra_methods),
    }
    if legacy_story_document:
        files["docs/bounded-contexts/Budget/old-stories.md"] = _LEGACY_STORY
    if padded:
        files = {rel: _padded(text) for rel, text in files.items()}
    return files


def _run_traceability_fixture(files: dict, root: Path) -> tuple[int, str]:
    """Write the fixture, copy the real validator into it, and run it."""
    for rel, text in files.items():
        path = root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("w", encoding="utf-8", newline="\n") as handle:
            handle.write(text)

    script = root / ".ai" / "tests" / "validate-traceability.py"
    script.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(_TRACEABILITY_VALIDATOR, script)

    result = subprocess.run(
        [sys.executable, str(script)],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        cwd=str(root),
    )
    return result.returncode, result.stdout + result.stderr


def _traceability_failures(output: str) -> list:
    return [line for line in output.splitlines() if line.startswith("❌ FAIL")]


def test_traceability_fixture_regression(tmp_path):
    """Every FAIL the validator owns fires on a planted defect each, and stays silent on the
    clean fixture; the padded-child advisory fires without turning into a failure."""
    code, output = _run_traceability_fixture(_fixture_files(), tmp_path / "clean")
    assert code == 0, f"the clean fixture did not pass:\n{output}"
    assert "❌ FAIL" not in output, f"the clean fixture reported a failure:\n{output}"
    assert "⚠️" not in output, f"the clean fixture reported an advisory warning:\n{output}"
    assert "ALL TRACEABILITY CHECKS PASSED" in output, f"\n{output}"

    defects = {
        "t1": (dict(duplicate_criterion=True), "AC-Budget-014.1 is declared 2 times"),
        "t2": (dict(unknown_tag="AC-Budget-099.1"),
               "@AC-Budget-099.1 resolves to no declared criterion"),
        "t3": (dict(unknown_slice_ref="AC-Budget-098.1"),
               "source.acceptance_criteria value AC-Budget-098.1 resolves to no declared "
               "criterion"),
        "t4": (dict(unknown_test_category="AC-Budget-097.1"),
               '[TestCategory("AC-Budget-097.1")] resolves to no declared criterion'),
        "t3-story-ref": (dict(unknown_story_ref="US-Budget-999"),
                         "source.user_stories value US-Budget-999 resolves to no declared "
                         "identifier"),
        "t3-use-case-ref": (dict(unknown_use_case_ref="UC-Budget-999"),
                            "source.use_cases value UC-Budget-999 resolves to no declared "
                            "identifier"),
        "t3-no-source-link": (dict(drop_source_links=True),
                              "names neither `use_cases` nor `user_stories` in `source`"),
    }

    for defect, (kwargs, expected) in defects.items():
        code, output = _run_traceability_fixture(_fixture_files(**kwargs), tmp_path / defect)
        failures = _traceability_failures(output)
        assert code == 1, f"{defect}: expected exit 1, got {code}:\n{output}"
        assert len(failures) == 1, (
            f"{defect}: expected exactly the planted defect to fail:\n{output}"
        )
        assert expected in failures[0], (
            f"{defect}: expected {expected!r}, got {failures[0]!r}"
        )

    # A blueprint with no `source` key at all is the pre-convention shape, which the skills read
    # as a legacy slice: it names no reference, so there is nothing to resolve and nothing fails.
    # Only a `source` that is *present* and names neither key is untraceable by construction.
    code, output = _run_traceability_fixture(
        _fixture_files(no_source_key=True), tmp_path / "legacy-slice")
    assert code == 0, f"a blueprint with no `source` key is legacy, not a defect:\n{output}"
    assert _traceability_failures(output) == [], f"\n{output}"

    # A padded child number resolves -- the tolerant match keeps both the declaration and its
    # references in the trace, so exit 0 -- and every carrier is reported as an advisory: two
    # declarations, two scenario tags, two blueprint values and two test categories.
    code, output = _run_traceability_fixture(_fixture_files(padded=True), tmp_path / "padded")
    assert code == 0, f"a padded child number must not fail resolution:\n{output}"
    assert _traceability_failures(output) == [], f"\n{output}"
    padded = [line for line in output.splitlines() if "padded child number" in line]
    assert len(padded) == 8, (
        f"expected the declaration and each reference to be reported, got {len(padded)}:\n"
        f"{output}"
    )
    assert any("user-stories.md" in line for line in padded), (
        f"the declaration is not reported:\n{output}"
    )
    assert any("CreateBudgetCommandHandlerTests.cs" in line for line in padded), (
        f"the test-attribute reference is not reported:\n{output}"
    )
    assert "AC-Budget-014.01 carries a padded child number; `{n}` is unpadded " \
           "(AC-Budget-014.1)" in output, f"\n{output}"

    # T7 is scoped to the story and use-case documents. A context history and a storage note in
    # the same context hold no stories and must stay out of it; an unlabelled *story* document
    # is still reported, so the scope narrowed without the legacy warning being lost.
    code, output = _run_traceability_fixture(
        _fixture_files(legacy_story_document=True), tmp_path / "legacy")
    assert code == 0, f"a legacy document is advisory, not a failure:\n{output}"
    assert "history.md" not in output, f"a context history was reported as a story file:\n{output}"
    assert "storage-notes.md" not in output, (
        f"a storage note was reported as a story file:\n{output}"
    )
    assert "old-stories.md -- legacy mode: 0 of 1 stories carry identifiers" in output, (
        f"a legacy story document is no longer reported:\n{output}"
    )

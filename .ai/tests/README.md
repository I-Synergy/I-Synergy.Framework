# Template Validation Tests

This directory contains a comprehensive test suite for validating the Claude template structure, content, and consistency.

## Quick Start

Run every bash suite:

```bash
bash .ai/tests/run-all-tests.sh
```

`run-all-tests.sh` is the canonical list of suites, in run order — read it to see exactly what executes. Its pytest twin runs the same scripts as individual, clickable tests in VS Code's Test Explorer:

```bash
python -m pytest .ai/tests/test_suite.py -v
```

The pytest wrapper also runs `validate-upgrade-script.py`, which the bash runner skips.

## Test Suites

Suites are listed below in the order `run-all-tests.sh` runs them.

### Structure Validation (`validate-structure.sh`)

Validates the directory structure and presence of all required files.

**What it checks:**
- Required directories exist (`.ai/skills`, `.ai/patterns`, etc.)
- Every directory in `.ai/skills/` has a `SKILL.md` file
- All pattern files exist
- Required reference files are present
- Template files exist
- The checklist file exists
- Main documentation files (`AGENTS.md`, `README.md`, `.ai/session-context.md`)

**Run individually:**
```bash
bash .ai/tests/validate-structure.sh
```

### YAML Frontmatter Validation (`validate-skills.py`)

Parses and validates YAML frontmatter in all skill files.

**What it checks:**
- YAML frontmatter is valid and parseable
- Required fields present: `name`, `description`
- Field types are correct (string, boolean, list)
- `name` matches directory name
- `description` is meaningful (>10 characters)
- Boolean flags are valid
- `allowed-tools` syntax is correct

**Run individually:**
```bash
python3 .ai/tests/validate-skills.py
```

### Reference Validation (`validate-references.sh`)

Validates file references and links throughout the documentation.

**What it checks:**
- File references in AGENTS.md exist
- Skill references are valid
- Pattern file references are valid
- Template file references exist
- Cross-references between files work
- Markdown links are not broken
- Skill directories have SKILL.md files

**Run individually:**
```bash
bash .ai/tests/validate-references.sh
```

### Content Quality Validation (`validate-content.py`)

Validates content quality in skills and patterns.

**What it checks:**
- Skills have meaningful content (>100 characters)
- Patterns have code examples
- Required sections are present
- Templates use proper token placeholders
- Content has quality indicators (examples, "when" descriptions)

**Run individually:**
```bash
python3 .ai/tests/validate-content.py
```

### Token Consistency Validation (`validate-tokens.sh`)

Validates template token usage and consistency.

**What it checks:**
- Template files use token placeholders
- Token definitions exist in `tokens.md`
- All expected tokens are defined
- No hardcoded project names in templates
- Consistent token usage across files

**Run individually:**
```bash
bash .ai/tests/validate-tokens.sh
```

### AGENTS.md References (`validate-agents-md.py`)

Validates every `.ai/` path referenced from `AGENTS.md`, and flags skills and patterns that exist on disk but are referenced by no task type.

**What it checks:**
- Skill references point at an existing `.ai/skills/{name}/SKILL.md`
- Pattern and other file references resolve on disk
- Orphaned skills and patterns (advisory warnings, not failures)
- Skill references use the `.md` form

**Run individually:**
```bash
python3 .ai/tests/validate-agents-md.py
```

### Trigger Lines (`validate-triggers.py`)

The frontmatter `description` is the primary mechanism a client uses to decide whether to load a skill or delegate to an agent, so it must state *when* to invoke it.

**What it checks:**
- Every `.ai/skills/*/SKILL.md` and `.ai/agents/*.md` carries parseable YAML frontmatter
- The `description` field exists and is non-empty
- The combined text of `description` plus (for skills) `when_to_use` contains an explicit trigger clause — one of `use when`, `use for`, `use before`, `use after`, `trigger when`, `trigger whenever`, `trigger on`, `call this when`, `invoke when`, or `when to use` (case-insensitive, on word boundaries)
- Fails on a missing frontmatter, a missing/empty description, or a description with no trigger clause

**Run individually:**
```bash
python3 .ai/tests/validate-triggers.py
```

### Prose To Skills (`validate-prose.py`)

`AGENTS.md` is the always-loaded entry point, so long prose must point at the skill or reference that carries the detail.

**What it checks:**
- Splits `AGENTS.md` on blank lines into blocks, skipping blocks that are only table rows or headings, and skipping everything inside fenced code blocks
- Any remaining block over 100 words must carry a `.ai/` pointer — a reference matching `.ai/skills/`, `.ai/reference/`, `.ai/patterns/` or `.ai/agents/`
- Fails when `AGENTS.md` is missing or an over-threshold block has no such pointer, naming the block's first line and its word count

**Run individually:**
```bash
python3 .ai/tests/validate-prose.py
```

### Settings & Structure (`validate-settings.py`)

Validates Claude Code's configuration and the overall directory layout.

**What it checks:**
- `.claude/settings.json` parses and sets `plansDirectory`
- `additionalDirectories` covers the `.ai/` subdirectories
- No stale `.claude/` content references in tracked docs
- `.ai/` directory structure is complete
- `.claude/` holds config only — no AI content

**Run individually:**
```bash
python3 .ai/tests/validate-settings.py
```

### Copilot Integration (`validate-copilot.py`)

Validates the GitHub Copilot integration.

**What it checks:**
- `.github/copilot-instructions.md` exists and carries no stale `.claude/` references
- `.github/skills/` is a folder-level junction into `.ai/skills/`
- `.claude/skills/` is a folder-level junction into `.ai/skills/`

**Run individually:**
```bash
python3 .ai/tests/validate-copilot.py
```

### Integration Smoke Tests (`smoke-test.py`)

Integration tests that verify the template works as a whole.

**What it checks:**
- All skills can be loaded and parsed
- Skill names are unique
- Descriptions are unique
- `allowed-tools` references are valid
- All patterns exist
- All templates exist
- Content is parseable

**Run individually:**
```bash
python3 .ai/tests/smoke-test.py
```

### Traceability (`validate-traceability.py`)

Validates the identifier chain that joins story and use-case documents to blueprints, feature files and test code.

**What it checks:**
- T1–T4 (failures) — duplicate `US-`/`UC-`/`AC-` identifiers, `@AC-…` scenario tags that resolve to no declared criterion, every identifier a blueprint's `source` names resolving — `use_cases` and `user_stories` as well as `acceptance_criteria`, with a `source` that names neither key failing as untraceable by construction — and `[TestCategory("AC-…")]` attributes that resolve
- T5–T6 (advisories) — a declared criterion with no tagged scenario, and a declared criterion with no slice or no test-side reference
- T7 (advisory) — a legacy or partly-labelled *story or use-case* document, reported once per document rather than once per entry; a context history, glossary or note beside them is out of scope
- A padded child number (`AC-Widget-014.02`) resolves and is reported as an advisory — `{n}` is unpadded
- Exit code `1` only when a reference is broken; advisory warnings do not change it

This tree ships no `docs/` tree and no test sources, so the suite passes here by finding nothing to check.

**Run individually:**
```bash
python3 .ai/tests/validate-traceability.py
```

### Standards (`validate-standards.py`)

Validates `.ai/reference/standards.md` under the parsing contract that file documents, and checks that a generated compatibility surface makes no claim the manifest cannot back.

**What it checks:**
- The status table is selected by the `## Compatibility` heading and carries the eight exact headers
- Every `Compatibility` cell holds one of `Evidence` / `Aligned` / `Organizational`, and every `Standard` key is unique
- Every path claim resolves; `{…}` patterns name a file a project will produce and are never existence-checked
- Every `Evidence` row's proving validator exits `0`, and the status computed from that run is the token the manifest carries
- An `Evidence` row whose `Evidence a project produces` patterns match nothing the project produced is downgraded to `Aligned`, with a note naming what was not found — the patterns are globbed with `*` in place of each `{…}`, and only a tree holding a solution file (`*.sln` or `*.slnx`) is asked the question (manifest rule 9)
- A generated surface (`COMPLIANCE.md`, or the marked block in `README.md`) agrees row for row, and the exit codes in its provenance line agree with a re-run
- The `## Validators` table's declared status agrees with the file on disk (advisory)

No solution file is present in this tree, so nothing is generated here, the evidence-pattern question is not asked, and the surface checks report a skip rather than a failure.

**Run individually:**
```bash
python3 .ai/tests/validate-standards.py
```

### Upgrade Script (`validate-upgrade-script.py`) — pytest only

`run-all-tests.sh` does not run this suite; `test_suite.py` does, which makes pytest the fuller run.

**What it checks:**
- `PROJECT_OWNED` and `TEMPLATE_OWNED` cover the expected paths, and classification is correct
- A dry run writes nothing; new template files are copied; project-owned files are never touched
- Three-tier skill completeness (`.ai/skills/` through to `.claude/skills/` and `.github/skills/`)

**Run individually:**
```bash
python3 .ai/tests/validate-upgrade-script.py
```

### Harness Health Score (`score-harness.py`)

A reporting layer over every suite above, not another suite. `score-harness.py` runs each
validator as a subprocess and weights the result into a single 0–100 score:

- `1.0` — exit `0`, no advisory lines
- `0.8` — exit `0`, at least one advisory line (`⚠️`)
- `0.0` — non-zero exit

The 17 weights sum to exactly 100 and are grouped into five categories — Integrity 40,
Platform 15, Quality 15, Standards 14, Release 16. Lines marked `⚠️ SKIP:` mean "not
applicable here" (the standards and traceability suites skip work in a tree with no solution
file and no test sources) and cost no points. The report gives the total, a letter grade
(`A+` ≥ 95, `A` ≥ 90, `B` ≥ 80, `C` ≥ 70, `D` ≥ 60, `F` below), a per-category breakdown,
and every deduction named with its reason.

Options: `--json PATH` writes the machine-readable document, `--badge PATH` writes a
shields.io SVG (label `harness`, message `<score>/100 <grade>`), `--from-json PATH` re-renders
a saved document without running a single check, `--fail-under N` exits `1` below a threshold,
`--timeout SECONDS` caps each check (default 300), `--list` prints the checks and weights, and
`--quiet` prints only `96.4/100 A+`-style output.

`run-all-tests.sh` does **not** run it, and `test_suite.py` has no wrapper for it: the scorer
re-invokes the same validators those runners already execute, so adding it would double the
runtime without adding a single new check. It is a convenience and reporting entry point, not
part of the pass/fail gate.

The release workflow (`.github/workflows/ci.yml`) does run it on every release: the score is
appended to the GitHub Release notes, and `harness-score.json` plus `harness-score.svg` are
uploaded as release assets. The README badges read the latest release's JSON, so a release
publishes its own score without committing anything to `main`.

**Run individually:**
```bash
python .ai/tests/score-harness.py
python .ai/tests/score-harness.py --json harness-score.json --badge harness-score.svg --fail-under 90
```

## Exit Codes

All test scripts follow standard Unix conventions:
- `0` - All tests passed
- `1` - One or more tests failed

## Requirements

### Bash Scripts
- Bash 4.0 or higher
- Standard Unix tools: `grep`, `find`, `wc`

### Python Scripts
- Python 3.10 or higher (CI runs 3.14)
- `PyYAML` and `pytest` (pinned in `requirements-dev.txt`)

Install Python dependencies:
```bash
PIP_CONFIG_FILE=pip.ini pip install -r requirements-dev.txt
```

## CI/CD Integration

### GitHub Actions

Create `.github/workflows/validate-template.yml`:

```yaml
name: Validate Template

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v7

      - name: Set up Python
        uses: actions/setup-python@v7
        with:
          python-version: '3.14'

      - name: Install dependencies
        run: pip install -r requirements-dev.txt

      - name: Make scripts executable
        run: chmod +x .ai/tests/*.sh

      - name: Run validation tests
        run: .ai/tests/run-all-tests.sh
```

### GitLab CI

Create `.gitlab-ci.yml`:

```yaml
validate:
  image: python:3.14
  script:
    - pip install -r requirements-dev.txt
    - chmod +x .ai/tests/*.sh
    - .ai/tests/run-all-tests.sh
```

## Test Output Examples

The runner counts suites at runtime, so the totals below are placeholders — this README does not restate them.

### Success

```
=========================================
  Claude Template Validation Suite
=========================================

Running comprehensive validation tests...

=========================================
TEST: 1. Directory Structure
=========================================
✅ Directory exists: .ai/skills
✅ Directory exists: .ai/patterns
...
✅ 1. Directory Structure PASSED

...

=========================================
  VALIDATION SUITE SUMMARY
=========================================

Total test suites: <n>
Passed: <n>
Failed: 0

✅✅✅ ALL TESTS PASSED ✅✅✅

The Claude template is valid and ready to use!
=========================================
```

### Failure

```
=========================================
TEST: 2. YAML Frontmatter
=========================================
✅ dotnet-engineer
❌ unit-tester
   - Missing required field: 'description'
✅ playwright-tester
...

❌ 2. YAML Frontmatter FAILED

...

=========================================
  VALIDATION SUITE SUMMARY
=========================================

Total test suites: <n>
Passed: <n>
Failed: 1

❌❌❌ SOME TESTS FAILED ❌❌❌

Please review the errors above and fix them.
=========================================
```

## Adding New Tests

To add a new test suite:

1. Create a new test script in `.ai/tests/`
2. Follow the naming convention: `validate-<feature>.sh` or `validate-<feature>.py`
3. Ensure it returns exit code 0 on success, 1 on failure
4. Add it to `run-all-tests.sh` with the `run_test` function (and a pytest wrapper in `test_suite.py` if it should appear in Test Explorer)
5. Document it here only when the script name doesn't tell the whole story — `run-all-tests.sh` is the list of record

Example:

```bash
# In run-all-tests.sh
run_test "Custom Feature" "bash '$SCRIPT_DIR/validate-custom.sh'"
```

## Troubleshooting

### "Permission denied" errors

Make scripts executable:
```bash
chmod +x .ai/tests/*.sh
```

### "PyYAML not found" errors

Install PyYAML:
```bash
pip install -r requirements-dev.txt
```

### Path issues on Windows

Use Git Bash or WSL to run the tests. The scripts use Unix-style paths.

## Maintenance

### When to Run Tests

- Before committing changes to the template
- After adding new skills or patterns
- After modifying AGENTS.md or other documentation
- Before creating a new template release
- In CI/CD pipeline on every commit

### Updating Tests

When adding new template features:
1. Update structure validation for new directories/files
2. Update content validation for new content requirements
3. Update token validation for new tokens
4. Add smoke tests for new functionality

## License

These tests are part of the Claude template project and follow the same license.

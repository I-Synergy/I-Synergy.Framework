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
- Main documentation files (`CLAUDE.md`, `README.md`, `.ai/session-context.md`)

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
- File references in CLAUDE.md exist
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

### CLAUDE.md References (`validate-claude-md.py`)

Validates every `.ai/` path referenced from `CLAUDE.md`, and flags skills and patterns that exist on disk but are referenced by no task type.

**What it checks:**
- Skill references point at an existing `.ai/skills/{name}/SKILL.md`
- Pattern and other file references resolve on disk
- Orphaned skills and patterns (advisory warnings, not failures)
- Skill references use the `.md` form

**Run individually:**
```bash
python3 .ai/tests/validate-claude-md.py
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

### Reasonix Integration (`validate-reasonix.py`)

Validates the Reasonix Code integration.

**What it checks:**
- `REASONIX.md` exists and references paths that resolve
- `.reasonix/skills/` and `.reasonix/agents/` are folder-level junctions into `.ai/`
- `.claude/settings.json` includes Reasonix in permissions and hooks
- The sync scripts create and repair the `.reasonix/` junctions
- Session-management docs mention Reasonix

**Run individually:**
```bash
python3 .ai/tests/validate-reasonix.py
```

### Pi Integration (`validate-pi.py`)

Validates the Pi (claude-pi) integration.

**What it checks:**
- `.pi/skills/`, `.pi/agents/`, and `.pi/chains/` are folder-level junctions into `.ai/`
- `.pi/settings.json` exists

**Run individually:**
```bash
python3 .ai/tests/validate-pi.py
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

## Exit Codes

All test scripts follow standard Unix conventions:
- `0` - All tests passed
- `1` - One or more tests failed

## Requirements

### Bash Scripts
- Bash 4.0 or higher
- Standard Unix tools: `grep`, `find`, `wc`

### Python Scripts
- Python 3.8 or higher
- `PyYAML` library

Install Python dependencies:
```bash
pip install pyyaml
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
      - uses: actions/checkout@v1

      - name: Set up Python
        uses: actions/setup-python@v2
        with:
          python-version: '3.11'

      - name: Install dependencies
        run: pip install pyyaml

      - name: Make scripts executable
        run: chmod +x .ai/tests/*.sh

      - name: Run validation tests
        run: .ai/tests/run-all-tests.sh
```

### GitLab CI

Create `.gitlab-ci.yml`:

```yaml
validate:
  image: python:3.11
  script:
    - pip install pyyaml
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
pip install pyyaml
```

### Path issues on Windows

Use Git Bash or WSL to run the tests. The scripts use Unix-style paths.

## Maintenance

### When to Run Tests

- Before committing changes to the template
- After adding new skills or patterns
- After modifying CLAUDE.md or other documentation
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

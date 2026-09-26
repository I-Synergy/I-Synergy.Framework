# Claude Template Testing Guide

## Overview

This comprehensive testing system validates the structure, content, and consistency of the Claude template to ensure it's ready for production use.

## Running the Suites

`run-all-tests.sh` is the canonical list of suites, in run order — read it to see exactly what executes. To run the same scripts as individual tests under pytest (one entry per suite in VS Code's Test Explorer):

```bash
python -m pytest .ai/tests/test_suite.py -v
```

The pytest wrapper also runs `validate-upgrade-script.py`, which the bash runner skips.

| Test Suite | Script | What it validates |
|-----------|--------|-------------------|
| Directory Structure | `validate-structure.sh` | Required directories and files, and a `SKILL.md` in every skill directory |
| YAML Frontmatter | `validate-skills.py` | Frontmatter in every `SKILL.md` |
| File References | `validate-references.sh` | File references in CLAUDE.md, `README.md` links, and templates |
| Content Quality | `validate-content.py` | Content quality in skills and patterns |
| Token Consistency | `validate-tokens.sh` | Token definitions and usage |
| CLAUDE.md References | `validate-claude-md.py` | `.ai/` paths in CLAUDE.md, plus orphaned skills and patterns |
| Settings & Structure | `validate-settings.py` | `.claude/settings.json`, `.ai/` layout, `.claude/` config-only |
| Copilot Integration | `validate-copilot.py` | Copilot instructions and the `.github/skills/` junction |
| Integration Smoke Tests | `smoke-test.py` | Skills loadable, names and descriptions unique, patterns and templates present |
| Reasonix Integration | `validate-reasonix.py` | REASONIX.md, `.reasonix/` junctions, settings, sync scripts |
| Pi Integration | `validate-pi.py` | `.pi/` junctions (`skills`, `agents`, `chains`) and `settings.json` |
| Upgrade Script (pytest only) | `validate-upgrade-script.py` | `upgrade-template.py` classification and integration |

No dated results snapshot is kept here — results change with every run.

## Quick Start

Run all tests:
```bash
bash .ai/tests/run-all-tests.sh
```

Run individual suite:
```bash
# Structure validation
bash .ai/tests/validate-structure.sh

# YAML frontmatter validation
python3 .ai/tests/validate-skills.py

# Reference validation
bash .ai/tests/validate-references.sh

# Content quality validation
python3 .ai/tests/validate-content.py

# Token consistency validation
bash .ai/tests/validate-tokens.sh

# CLAUDE.md reference validation
python3 .ai/tests/validate-claude-md.py

# Settings and structure validation
python3 .ai/tests/validate-settings.py

# Copilot integration validation
python3 .ai/tests/validate-copilot.py

# Integration smoke tests
python3 .ai/tests/smoke-test.py

# Reasonix integration validation
python3 .ai/tests/validate-reasonix.py

# Pi integration validation
python3 .ai/tests/validate-pi.py

# Upgrade script validation (pytest only — not run by run-all-tests.sh)
python3 .ai/tests/validate-upgrade-script.py
```

## Prerequisites

### Required Software

- **Bash** 4.0+ (Git Bash on Windows)
- **Python** 3.8+ with PyYAML

### Installation

```bash
# Install Python dependencies
pip install pyyaml

# Make scripts executable (Linux/Mac)
chmod +x .ai/tests/*.sh .ai/tests/*.py
```

## Test Suite Details

### Structure Validation (`validate-structure.sh`)

Validates the directory structure and file presence.

**Validates:**
- ✅ Required directories exist
- ✅ Every skill directory has a `SKILL.md`
- ✅ Pattern files exist
- ✅ Required reference files exist
- ✅ Project files exist
- ✅ The pre-submission checklist exists
- ✅ Code templates exist
- ✅ Main documentation files exist (`CLAUDE.md`, `README.md`, `.ai/session-context.md`)

### YAML Frontmatter Validation (`validate-skills.py`)

Parses and validates YAML frontmatter in all skill files.

**Validates:**
- ✅ YAML frontmatter is parseable
- ✅ Required fields present: `name`, `description`
- ✅ Field types are correct
- ✅ `name` matches directory name
- ✅ `description` is meaningful (>10 characters)
- ✅ Boolean flags are valid
- ✅ `allowed-tools` is a list (not string)
- ✅ Content is non-empty (>100 characters)

### File References Validation (`validate-references.sh`)

Validates all file references and cross-references.

**Validates:**
- ✅ Skill, pattern, and reference paths named in `CLAUDE.md` resolve
- ✅ Template file references resolve
- ✅ Pattern files used for cross-references exist
- ✅ Every skill directory is consistent

### Content Quality Validation (`validate-content.py`)

Validates content quality in skills and patterns.

**Validates:**
- ✅ Skills have meaningful content (>100 chars)
- ✅ Skills have code examples (``` blocks)
- ⚠️  Skills have quality indicators (reported as warnings)
- ✅ Patterns have code examples
- ✅ Patterns have sufficient content (>200 chars)

**Warnings (Non-Blocking):**
Quality warnings are reported per run — which skills and patterns lack quality indicators or code examples depends on the current content, so no list is kept here.

### Token Consistency Validation (`validate-tokens.sh`)

Validates template token usage and consistency.

**Validates:**
- ✅ Token definitions file exists
- ✅ Tokens are defined for every placeholder the templates use
- ✅ Skills use tokens
- ✅ `CLAUDE.md` uses tokens
- ✅ No hardcoded project names in templates

**Tokens Validated:**

The canonical token list is `.ai/reference/tokens.md`; this guide does not repeat it.

### CLAUDE.md References (`validate-claude-md.py`)

Validates that all skill and pattern paths referenced in CLAUDE.md exist on disk.

**Validates:**
- ✅ Skill references in `CLAUDE.md` point at an existing `.ai/skills/{name}/SKILL.md`
- ✅ Pattern and other file references in `CLAUDE.md` resolve on disk
- ✅ Skills and patterns that exist but are referenced by no task type are reported (advisory warnings, not failures)

### Settings & Structure (`validate-settings.py`)

Validates `.claude/settings.json` configuration and the overall directory layout.

**Validates:**
- ✅ `plansDirectory` set to `./.ai/plans`
- ✅ `additionalDirectories` includes all `.ai/` subdirectories
- ✅ No stale `.claude/` content references in key docs
- ✅ `.ai/` directory structure is complete
- ✅ `.claude/` contains only config files (no AI content)

### Copilot Integration (`validate-copilot.py`)

Validates the GitHub Copilot setup.

**Validates:**
- ✅ `.github/copilot-instructions.md` exists with no stale `.claude/` references
- ✅ `.github/skills/` directory exists
- ✅ `.github/skills/` is a folder-level junction into `.ai/skills/`
- ✅ `.claude/skills/` is a folder-level junction into `.ai/skills/`

### Integration Smoke Tests (`smoke-test.py`)

Integration tests verifying the template works as a whole.

**Validates:**
- ✅ Skills Loadable - every skill can be loaded and parsed
- ✅ Patterns Exist - every pattern file exists
- ✅ Templates Exist - every code template exists
- ✅ Unique Skill Names - All skill names are unique
- ✅ Unique Descriptions - All descriptions are unique
- ✅ Valid Tool References - All allowed-tools are valid
- ✅ Parseable Content - All content is parseable

The Reasonix, Pi, and Upgrade Script suites are described in `.ai/tests/README.md`.

## Files Tested

The suites discover files from disk at run time, so this guide keeps no inventory of them — an enumerated list here would only drift from `.ai/skills/`, `.ai/patterns/`, `.ai/reference/`, `.ai/reference/templates/`, and `.ai/project/`. Read those directories, or the suite output, for the current contents.

## CI/CD Integration

### GitHub Actions

The template includes a GitHub Actions workflow at `.github/workflows/validate-template.yml` that runs all tests automatically on:
- Push to main/master/develop branches
- Pull requests to main/master/develop branches
- Manual workflow dispatch

### Running in CI

The workflow:
1. Checks out code
2. Sets up Python 3.11
3. Installs PyYAML
4. Makes scripts executable
5. Runs every suite listed in `run-all-tests.sh`
6. Uploads test results (if any logs generated)

## Exit Codes

All test scripts follow Unix conventions:
- `0` - All tests passed
- `1` - One or more tests failed

## Maintenance

### When to Run Tests

Run tests:
- ✅ Before committing changes
- ✅ After adding new skills or patterns
- ✅ After modifying documentation
- ✅ Before creating releases
- ✅ In CI/CD on every commit

### Adding New Tests

To add a new test suite:

1. Create script in `.ai/tests/`
2. Follow naming: `validate-<feature>.sh` or `validate-<feature>.py`
3. Return exit code 0 on success, 1 on failure
4. Add to `run-all-tests.sh`:
   ```bash
   run_test "New Feature" "bash '$SCRIPT_DIR/validate-new-feature.sh'"
   ```
5. Document it in `.ai/tests/README.md` only when the script name doesn't tell the whole story

## Troubleshooting

### Common Issues

**"Permission denied"**
```bash
chmod +x .ai/tests/*.sh .ai/tests/*.py
```

**"PyYAML not found"**
```bash
pip install pyyaml
```

**"Path not found" on Windows**
- Use Git Bash or WSL
- Scripts use Unix-style paths

**Unicode errors in Python**
- Scripts automatically handle UTF-8 on Windows
- If issues persist, set `PYTHONIOENCODING=utf-8`

### Debugging Tests

Run with verbose output:
```bash
bash -x .ai/tests/validate-structure.sh
python3 -v .ai/tests/validate-skills.py
```

## Test Coverage

Coverage is whatever the suites discover on disk at run time. The suite table at the top of this guide lists what each one covers, and each suite prints its own passed/failed totals when it runs — no coverage figures are recorded here.

## Performance

Each suite runs in seconds, and the full bash run is dominated by the junction pre-step and the smoke test. Exact timings depend on the machine, so no per-suite figures are recorded here.

## Future Enhancements

Potential improvements:
- [ ] Add performance benchmarks
- [ ] Add security scanning (no hardcoded secrets)
- [ ] Add markdown link validation (external links)
- [ ] Add spelling/grammar checks
- [ ] Add complexity metrics
- [ ] Add test coverage reporting
- [ ] Add badge generation for README

## Support

For issues or questions:
1. Check this guide
2. Review test output for specific errors
3. Check `.ai/tests/README.md` for detailed documentation
4. Review individual test scripts for validation logic

## License

Part of the Claude Template project.

# Claude Template Testing Guide

## Overview

This comprehensive testing system validates the structure, content, and consistency of the Claude template to ensure it's ready for production use.

## Test Results Summary

**Last Run:** 2026-04-19
**Status:** ✅ ALL TESTS PASSED (9/9)

### Test Suites

| # | Test Suite | Status | Checks |
|---|-----------|--------|--------|
| 1 | Directory Structure | ✅ PASSED | 49 checks - all directories and files exist |
| 2 | YAML Frontmatter | ✅ PASSED | 17 skills validated - all frontmatter valid |
| 3 | File References | ✅ PASSED | 34 checks - all references valid |
| 4 | Content Quality | ✅ PASSED | 23 checks - content quality verified (8 warnings) |
| 5 | Token Consistency | ✅ PASSED | 19 checks - tokens properly defined |
| 6 | CLAUDE.md References | ✅ PASSED | validates CLAUDE.md skill/pattern paths |
| 7 | Settings & Structure | ✅ PASSED | settings.json paths, .ai/ layout, .claude/ config-only |
| 8 | Copilot Integration | ✅ PASSED | copilot-instructions.md + skills three-tier sync |
| 9 | Integration Smoke Tests | ✅ PASSED | 7 tests - all integration tests pass |

## Quick Start

Run all tests:
```bash
cd /d/Projects/Template
bash .ai/tests/run-all-tests.sh
```

Run individual test:
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

### 1. Structure Validation (`validate-structure.sh`)

Validates the directory structure and file presence.

**Validates:**
- ✅ 7 required directories exist
- ✅ 17 skill SKILL.md files exist
- ✅ 8 pattern files exist
- ✅ 5 reference files exist
- ✅ 5 project files exist
- ✅ 1 checklist file exists
- ✅ 6 template files exist
- ✅ 2 main documentation files exist

**Total:** 49 structure checks

### 2. YAML Frontmatter Validation (`validate-skills.py`)

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

**Total:** 17 skills validated

### 3. File References Validation (`validate-references.sh`)

Validates all file references and cross-references.

**Validates:**
- ✅ 11 references in CLAUDE.md
- ✅ 6 template file references
- ✅ 2 pattern cross-references
- ✅ 17 skill directory consistency checks

**Total:** 34 reference checks

### 4. Content Quality Validation (`validate-content.py`)

Validates content quality in skills and patterns.

**Validates:**
- ✅ Skills have meaningful content (>100 chars)
- ✅ Skills have code examples (``` blocks)
- ⚠️  Skills have quality indicators (8 warnings)
- ✅ Patterns have code examples
- ✅ Patterns have sufficient content (>200 chars)

**Total:** 23 content checks, 8 warnings (non-blocking)

**Warnings (Non-Blocking):**
- 5 skills lack quality indicators (when/example)
- 2 skills missing code examples
- 1 pattern lacks quality indicators

### 5. Token Consistency Validation (`validate-tokens.sh`)

Validates template token usage and consistency.

**Validates:**
- ✅ Token definitions file exists
- ✅ All 5 tokens are defined
- ✅ Skills use tokens (12 skills with 200+ occurrences)
- ✅ CLAUDE.md uses tokens (10 occurrences)
- ✅ No hardcoded project names in templates

**Total:** 19 token checks

**Tokens Validated:**
- `{ApplicationName}` - Application name
- `{Domain}` - Domain/bounded context
- `{Entity}` - Entity name (PascalCase)
- `{entity}` - Entity name (lowercase)
- `{entities}` - Entity plural (lowercase)

### 6. CLAUDE.md References (`validate-claude-md.py`)

Validates that all skill and pattern paths referenced in CLAUDE.md exist on disk.

**Validates:**
- ✅ All skills in Work-Type Context Mapping exist in `.ai/skills/`
- ✅ All patterns referenced exist in `.ai/patterns/`
- ✅ No broken file references in CLAUDE.md

### 7. Settings & Structure (`validate-settings.py`)

Validates `.claude/settings.json` configuration and the overall directory layout.

**Validates:**
- ✅ `plansDirectory` set to `./.ai/plans`
- ✅ `additionalDirectories` includes all `.ai/` subdirectories
- ✅ No stale `.claude/` content references in key docs
- ✅ `.ai/` directory structure is complete
- ✅ `.claude/` contains only config files (no AI content)

### 8. Copilot Integration (`validate-copilot.py`)

Validates the GitHub Copilot three-tier skills setup.

**Validates:**
- ✅ `.github/copilot-instructions.md` exists with no stale `.claude/` references
- ✅ `.github/skills/` directory exists
- ✅ All `.github/skills/` entries are full content copies matching `.ai/skills/`
- ✅ All `.claude/skills/` entries are thin wrappers (not full copies)

### 9. Integration Smoke Tests (`smoke-test.py`)

Integration tests verifying the template works as a whole.

**Validates:**
- ✅ Skills Loadable - All 17 skills can be loaded and parsed
- ✅ Patterns Exist - All 8 patterns exist
- ✅ Templates Exist - All 6 templates exist
- ✅ Unique Skill Names - All skill names are unique
- ✅ Unique Descriptions - All descriptions are unique
- ✅ Valid Tool References - All allowed-tools are valid
- ✅ Parseable Content - All content is parseable

**Total:** 7 integration tests

## Files Tested

### Skills (17 files in `.ai/skills/`)
- ✅ api-security
- ✅ architect
- ✅ blazor-specialist
- ✅ code-reviewer
- ✅ database-migration
- ✅ devops-engineer
- ✅ dotnet-engineer
- ✅ integration-specialist
- ✅ maui-specialist
- ✅ performance-engineer
- ✅ playwright-tester
- ✅ security
- ✅ software-security
- ✅ technical-writer
- ✅ unit-tester
- ✅ update-skills
- ✅ verify-config

### Patterns (8 files)
- ✅ api-patterns.md
- ✅ cqrs-patterns.md
- ✅ microservices.md
- ✅ mvvm.md
- ✅ object-oriented-programming.md
- ✅ service-oriented-architecture.md
- ✅ test-driven-development.md
- ✅ testing-patterns.md

### Templates (6 files)
- ✅ command-handler.cs.txt
- ✅ query-handler.cs.txt
- ✅ endpoint.cs.txt
- ✅ test-class.cs.txt
- ✅ feature-file.feature.txt

### Reference Files (5 files)
- ✅ critical-rules.md
- ✅ forbidden-tech.md
- ✅ glossary.md
- ✅ naming-conventions.md
- ✅ tokens.md

### Project Files (5 files)
- ✅ architecture.md
- ✅ domains.md
- ✅ preferences.md
- ✅ session-context.md
- ✅ tech-stack.md

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
5. Runs all 9 test suites
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
   run_test "7. New Feature" "bash '$SCRIPT_DIR/validate-new-feature.sh'"
   ```
5. Update documentation

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

| Category | Files | Tests | Coverage |
|----------|-------|-------|----------|
| Skills (.ai/) | 17 | 136+ | 100% |
| Skills (.claude/ wrappers) | 17 | - | via validate-copilot |
| Skills (.github/ copies) | 17 | - | via validate-copilot |
| Patterns | 8 | 24 | 100% |
| Templates | 6 | 18 | 100% |
| References | 5 | 15 | 100% |
| Project Files | 5 | 15 | 100% |
| Structure | 7 dirs | 49 | 100% |
| Settings/Config | 1 | 12+ | 100% |
| Copilot | 2 | 8+ | 100% |
| **Total** | **83+ files** | **277+ checks** | **100%** |

## Performance

Average test suite execution time:
- Structure validation: ~1 second
- YAML frontmatter: ~1 second
- References: ~2 seconds
- Content quality: ~2 seconds
- Token consistency: ~1 second
- Integration tests: ~1 second

**Total runtime:** ~8-10 seconds

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

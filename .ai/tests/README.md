# Template Validation Tests

This directory contains a comprehensive test suite for validating the Claude template structure, content, and consistency.

## Quick Start

Run all tests:

```bash
.ai/tests/run-all-tests.sh
```

## Test Suites

### 1. Structure Validation (`validate-structure.sh`)

Validates the directory structure and presence of all required files.

**What it checks:**
- Required directories exist (`.ai/skills`, `.ai/patterns`, etc.)
- All 15 skill directories have `SKILL.md` files
- All 8 pattern files exist
- All reference files are present
- Template files exist
- Checklist files exist
- Main documentation files (CLAUDE.md, README.md)

**Run individually:**
```bash
bash .ai/tests/validate-structure.sh
```

### 2. YAML Frontmatter Validation (`validate-skills.py`)

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

### 3. Reference Validation (`validate-references.sh`)

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

### 4. Content Quality Validation (`validate-content.py`)

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

### 5. Token Consistency Validation (`validate-tokens.sh`)

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

### 6. Integration Smoke Tests (`smoke-test.py`)

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

Total test suites: 6
Passed: 6
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

Total test suites: 6
Passed: 5
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
4. Add it to `run-all-tests.sh` with the `run_test` function
5. Update this README with documentation

Example:

```bash
# In run-all-tests.sh
run_test "7. Custom Feature" "bash '$SCRIPT_DIR/validate-custom.sh'"
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

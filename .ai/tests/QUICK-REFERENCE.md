# Claude Template Testing - Quick Reference

## One Command Test

```bash
bash .ai/tests/run-all-tests.sh
```

## Individual Tests

```bash
# Structure
bash .ai/tests/validate-structure.sh

# YAML
python3 .ai/tests/validate-skills.py

# References
bash .ai/tests/validate-references.sh

# Content
python3 .ai/tests/validate-content.py

# Tokens
bash .ai/tests/validate-tokens.sh

# CLAUDE.md references
python3 .ai/tests/validate-claude-md.py

# Settings & structure
python3 .ai/tests/validate-settings.py

# Copilot integration
python3 .ai/tests/validate-copilot.py

# Smoke Tests
python3 .ai/tests/smoke-test.py

# Reasonix integration
python3 .ai/tests/validate-reasonix.py

# Pi integration
python3 .ai/tests/validate-pi.py

# Upgrade script (pytest only — not run by run-all-tests.sh)
python3 .ai/tests/validate-upgrade-script.py
```

## Expected Results

```
✅✅✅ ALL TESTS PASSED ✅✅✅

Total test suites: <n>
Passed: <n>
Failed: 0
```

The runner computes its own totals — `run-all-tests.sh` is the list of record.

## Test Coverage

| Test | What It Checks |
|------|---------------|
| **Structure** | Directories + files exist, a `SKILL.md` per skill |
| **YAML** | Skill frontmatter valid |
| **References** | File refs + links valid |
| **Content** | Quality + examples |
| **Tokens** | Token definitions and usage |
| **CLAUDE.md** | Skill/pattern paths in CLAUDE.md |
| **Settings** | settings.json + `.ai/` layout + `.claude/` config-only |
| **Copilot** | copilot-instructions + `.github/skills/` junction |
| **Reasonix** | REASONIX.md + `.reasonix/` junctions |
| **Pi** | `.pi/` junctions + settings.json |
| **Smoke** | Integration tests |
| **Upgrade Script** | `upgrade-template.py` (pytest only) |

## Quick Fixes

### Permission Denied
```bash
chmod +x .ai/tests/*.sh .ai/tests/*.py
```

### PyYAML Missing
```bash
pip install pyyaml
```

### Test Fails

1. Read error message
2. Fix the issue
3. Re-run specific test
4. Run full suite to verify

## Files Validated

Everything the suites discover on disk:

- `.ai/skills/`, plus its `.claude/`, `.github/`, `.reasonix/`, and `.pi/` junctions
- `.ai/patterns/`
- `.ai/reference/` and `.ai/reference/templates/`
- `.ai/project/`
- `.ai/checklists/pre-submission.md`
- `CLAUDE.md`, `README.md`, and `.ai/session-context.md`
- `.claude/settings.json` and `.github/copilot-instructions.md`

## Success Criteria

All checks must pass:
- [x] Directory structure complete
- [x] All skill YAML valid
- [x] All file references exist
- [x] Content has examples
- [x] Tokens properly defined
- [x] CLAUDE.md paths point to existing files
- [x] settings.json paths correct, .claude/ is config-only
- [x] Copilot, Reasonix, and Pi skills wired through junctions into `.ai/`
- [x] Skills are loadable
- [x] No duplicate names

## Runtime

~8-10 seconds for full suite

## CI/CD

Runs automatically on:
- Push to main/master/develop
- Pull requests
- Manual trigger

## Exit Codes

- `0` = Pass
- `1` = Fail

## Documentation

- **Full Guide:** `.ai/tests/TESTING-GUIDE.md`
- **Detailed README:** `.ai/tests/README.md`
- **This Card:** `.ai/tests/QUICK-REFERENCE.md`

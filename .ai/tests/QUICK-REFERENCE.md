# Claude Template Testing - Quick Reference

## One Command Test

```bash
.ai/tests/run-all-tests.sh
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
```

## Expected Results

```
✅✅✅ ALL TESTS PASSED ✅✅✅

Total test suites: 9
Passed: 9
Failed: 0
```

## Test Coverage

| Test | What It Checks | Count |
|------|---------------|-------|
| **Structure** | Directories + files exist | 49 |
| **YAML** | Skill frontmatter valid | 17 |
| **References** | File refs + links valid | 34 |
| **Content** | Quality + examples | 23 |
| **Tokens** | Token consistency | 19 |
| **CLAUDE.md** | Skill/pattern paths in CLAUDE.md | 10+ |
| **Settings** | settings.json + .ai/ layout + .claude/ config-only | 12+ |
| **Copilot** | copilot-instructions + skills 3-tier sync | 8+ |
| **Smoke** | Integration tests | 7 |
| **TOTAL** | | **179+** |

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

- ✅ 17 skills (`.ai/skills/`)
- ✅ 17 skill wrappers (`.claude/skills/`)
- ✅ 17 skill copies (`.github/skills/`)
- ✅ 8 patterns
- ✅ 6 templates
- ✅ 5 reference docs
- ✅ 5 project files
- ✅ 2 main docs (CLAUDE.md, README.md)
- ✅ `.claude/settings.json`
- ✅ `.github/copilot-instructions.md`

**Total: 83+ files + 7 directories**

## Success Criteria

All checks must pass:
- [x] Directory structure complete
- [x] All skill YAML valid
- [x] All file references exist
- [x] Content has examples
- [x] Tokens properly defined
- [x] CLAUDE.md paths point to existing files
- [x] settings.json paths correct, .claude/ is config-only
- [x] Copilot skills in sync (.github/ = full copies, .claude/ = wrappers)
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

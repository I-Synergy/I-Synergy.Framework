# Release Tagging on Merge to Main

Status: DONE
Started: 2026-10-28

## Steps

- [x] Move versioning logic into `.github/workflows/ci.yml` as inline steps (no scripts)
- [x] Seed `CHANGELOG.md`
- [x] Update template ownership tables and upgrade script
- [x] Add `validate-release.py`, register it in the suite and CI
- [x] Update README and TEMPLATE-USAGE (mirror workflow was removed by the user)
- [x] Run validation suites and smoke-test the release flow end to end

## Notes

Approved plan: `.ai/plans/release-tagging.md`.
Revision 2026-10-28: consolidated into a single `ci.yml`; `release.yml` and the two
PowerShell scripts were removed at the user's request.
Revision 2: `main` is protected, so the workflow no longer commits `CHANGELOG.md`; it
generates release notes, pushes only the tag and publishes the release.
Verification: validation suites, pytest, two-release git simulation
(changelog commit → annotated tag → remote tag), `git diff --check`.

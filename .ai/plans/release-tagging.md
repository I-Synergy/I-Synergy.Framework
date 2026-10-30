# Release Tagging on Merge to Main

**Status:** IMPLEMENTED 2026-10-28
**Requested:** 2026-10-28
**Decision source:** User chose the build-number scheme (`YYYY.1MMdd.1HHmm`), tagged
`$BUILD_NUMBER` (no `v` prefix), with a GitHub Release and a committed `CHANGELOG.md`,
ported here without the .NET build/pack steps (this repository has no solution). The
user supplied both PowerShell scripts; this plan was adapted to their parameters.

## Goal

After a successful merge to `main`, the repository automatically:

1. Derives a build number `YYYY.1MMdd.1HHmm` (UTC unless a manual run supplies one).
2. Derives version variables from it (`VersionNumber`, `VersionInformational`,
   `VersionPackage`) through a single source of truth.
3. Prepends a release section to `CHANGELOG.md` listing commits since the last `v*` tag.
4. Commits and pushes that changelog update to `main`.
5. Creates and pushes the annotated tag `v$BUILD_NUMBER` on that commit.
6. Creates a GitHub Release named `$BUILD_NUMBER` with the generated notes.

No .NET `AssemblyVersion`/`PackageVersion` wiring is added, because this repository
contains no project to consume it. The scripts still emit those values so a downstream
project generated from this template can pass them to MSBuild.

## Files

### New

| File | Purpose |
|---|---|
| `scripts/Set-VersionVariables.ps1` | Single source of truth. Validates the build number, splits `yyyy.1MMdd.1HHmm[.SS]`, emits `BuildNumber`, `VersionNumber`, `VersionInformational`, `VersionPackage` to `$GITHUB_OUTPUT` or `##vso[task.setvariable]`. `-IsPullRequest` yields `.SS` seconds and a `-preview` package version. |
| `scripts/New-GitTag.ps1` | Creates the annotated tag `v$BuildNumber` and optionally pushes it. Refuses to overwrite an existing tag and reports an existing one idempotently. |
| `.github/workflows/release.yml` | Trigger `push` to `main` plus `workflow_dispatch` (optional `build_number` input). Job guarded against bot/`[skip release]` pushes and concurrent runs. Runs the two scripts, updates `CHANGELOG.md`, commits, tags, and calls `gh release create --verify-tag`. |
| `CHANGELOG.md` | Seed file with the standard header; the workflow prepends entries. |
| `.ai/tests/validate-release.py` | Focused validator: workflow triggers only on `main`, declares `contents: write`, calls both scripts, and `Set-VersionVariables.ps1` produces the exact expected outputs for the merge and PR cases. |

### Changed

| File | Change |
|---|---|
| `README.md` | Add `release.yml`, root `scripts/` and `CHANGELOG.md` to the file-structure tree; add a “Release Versioning” section documenting the scheme, tag format, permissions and branch-protection caveat. |
| `TEMPLATE-USAGE.md` | Add `release.yml` and the two scripts to the template-owned table; note the changelog is project-owned after the first run. |
| `.ai/scripts/upgrade-template.py` | Add `scripts/Set-VersionVariables.ps1`, `scripts/New-GitTag.ps1`, `.github/workflows/release.yml` and the seeded `CHANGELOG.md` handling to `TEMPLATE_OWNED`. |
| `.ai/tests/validate-upgrade-script.py` | Require the new template-owned paths. |
| `.ai/tests/run-all-tests.sh` | Register `validate-release.py` as suite 12. |
| `.github/workflows/validate-template.yml` | Run `validate-release.py` alongside the other validators. |

`AGENTS.md`, `.ai/project/*` and all skills remain untouched.

## Version rules

- Build number: `$yyyy.1$MMdd.1$HHmm`; the `1` prefix keeps every component non-zero
  and preserves leading zeros.
- Merge build → `VersionNumber = X.Y.Z.0`, `VersionInformational = X.Y.Z`,
  `VersionPackage = X.Y.Z`.
- PR build → build number carries `.SS` seconds,
  `VersionNumber = X.Y.Z.SS`, `VersionPackage = X.Y.Z-preview`.
- Tag is always `v$BUILD_NUMBER` (e.g. `v2026.11028.11435`); the Release title is the
  plain build number.

## Safety behaviour

- Idempotent: if `v$BUILD_NUMBER` already exists, the run logs a notice and exits
  without touching the changelog.
- No release loop: changelog commits use the workflow token (which does not re-trigger
  workflows), carry `[skip release]`, and the job skips `github-actions[bot]` actors.
- `concurrency: release-main` with `cancel-in-progress: false` serialises merges.
- `permissions: contents: write` only; no other scopes.
- Uses `RELEASE_TOKEN` when present, otherwise `GITHUB_TOKEN`. The README documents
  that branch protection on `main` must allow the release actor to push, otherwise
  `RELEASE_TOKEN` (a PAT) is required.

## Verification

1. `pwsh -NoProfile -File scripts/Set-VersionVariables.ps1 -BuildNumber ...` against a
   temporary `GITHUB_OUTPUT`, for both merge and PR inputs.
2. `python .ai/tests/validate-release.py`.
3. `python -m pytest .ai/tests/test_suite.py -q` and `bash .ai/tests/run-all-tests.sh`.
4. `actionlint` is unavailable locally; YAML is covered by a Python parse in the
   validator.

## Revision 2026-10-28

The versioning logic was consolidated into a single `.github/workflows/ci.yml` with
inline steps. `.github/workflows/release.yml`, `scripts/Set-VersionVariables.ps1` and
`scripts/New-GitTag.ps1` were removed; `validate-template.yml` remains the separate
validation workflow. The tag is still the plain build number (`$BUILD_NUMBER`).

## Revision 2 — 2026-10-28

`main` is protected and rejects direct pushes, so the workflow no longer commits
`CHANGELOG.md`. It generates release notes from the commits since the previous tag,
pushes only the annotated tag, and publishes the GitHub Release. `CHANGELOG.md` is a
normal, manually curated file.

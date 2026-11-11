# Fix the release tag push and pin the SDK to global.json

**Status:** IMPLEMENTED 2026-11-11
**Requested:** 2026-11-11
**Trigger:** production run (CI 14, `v2026.11027.11355`) failed in `Publish release packages`:

```
! [remote rejected] v2026.11027.11355 -> v2026.11027.11355
  (refusing to allow a GitHub App to create or update workflow
   `.github/workflows/ci.yml` without `workflows` permission)
```

## Root cause — tag push

`git push origin "$tag"` authenticates with the credential that `actions/checkout` persisted,
which is `GITHUB_TOKEN`. `GITHUB_TOKEN` is an installation token of the GitHub Actions App, and an
App token may only create or update a ref whose commit touches `.github/workflows/` when the App is
granted the `workflows` permission. `GITHUB_TOKEN` can never be granted it — `permissions:` in a
workflow has no `workflows` key — so any release whose commit changed a workflow file fails at the
tag step, after the packages are already on nuget.org.

## Root cause — SDK version

`global.json` is the single source of truth (`11.0.100`, `rollForward: disable`,
`allowPrerelease: true`), but the workflow delegated version resolution to
`actions/setup-dotnet`'s `global-json-file` input and never checked what was actually installed.
The build logs report `11.0.100-rc.3.26522.148` rather than the pinned `11.0.100`, and nothing in
the pipeline notices because the RC still builds `net11.0`.

## Changes

| File | Change |
|---|---|
| `.github/workflows/ci.yml` | Release job checks out with `RELEASE_TOKEN` (fallback `GITHUB_TOKEN`), passes it to `gh release create`, and documents why. Both jobs read `sdk.version` from `global.json`, pass it to `setup-dotnet` as an explicit `dotnet-version`, and fail when `dotnet --version` disagrees with the pin. |
| `scripts/New-GitTag.ps1` | Capture the `git push` output and turn the App/`workflows` refusal into an actionable error naming `RELEASE_TOKEN`. |
| `docs/MIGRATION_GUIDE.md` | Step 11.1 documents the `RELEASE_TOKEN` secret and its permissions, plus the failure it prevents. |
| `.ai/project/tech-stack.md` | SDK line moves from the stale `11.0.100-rc.2` to `11.0.100`. |

## Decisions

- **Fine-grained PAT (`RELEASE_TOKEN`) over a GitHub App installation token** — the user choice
  already recorded for the docs sync (`WIKI_TOKEN`): least privilege, scoped to one repository,
  no App installation to maintain. Required permissions: Contents: Read and write, Workflows:
  Read and write.
- **Fallback to `GITHUB_TOKEN`** — PR/acceptance runs never tag, and a repository without the
  secret still works for releases that do not touch a workflow file.
- **Tag after publishing is kept** — the published version is recoverable by re-running the job
  (`dotnet nuget push --skip-duplicate`), so the ordering does not need to change.

## Verification

1. `python .ai/tests/validate-release.py` (referenced scripts exist, release contract intact).
2. YAML parses; both jobs contain read → install → verify steps.
3. `pwsh -NoProfile -File scripts/New-GitTag.ps1` with an unreachable remote reproduces the
   generic failure path without touching a tag.

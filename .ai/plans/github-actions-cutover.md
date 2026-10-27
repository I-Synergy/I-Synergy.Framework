# Move CI/CD from Azure DevOps to GitHub Actions

Status: IMPLEMENTED (nuget.org policies and the first green release pending)
Started: 2026-10-27
Approved by: user (full cutover; Key Vault signing with OIDC, NuGet OIDC)

## Goal

Port `azure-pipelines.yml` (build → pack → verify → test → sign → artifacts → NuGet.org → tag)
to GitHub Actions so that GitHub is the only place code is built and released. No change to
`src/` or `tests/` is expected; the projects, TFMs and package layout stay as they are.

## Decisions

| Decision | Choice | Why |
|---|---|---|
| Migration shape | Full cutover | User choice; one pipeline owner |
| Repository | GitHub is the source of truth; ADO repo becomes read-only | Actions can only build what is on GitHub |
| Workflow layout | One workflow, two jobs: `build` and `release` | Mirrors BuildStage/Release |
| Publish lanes | PR → `acceptance`, signed `-preview` packages pushed to NuGet.org; `main` → `production` (approval), release packages, tag | Restores the ADO ACC/PROD split; both lanes sign and publish, only main tags |
| Runner | `windows-2025-vs2026` (build, matches ADO), `windows-2025` (release) | ADO image choice carried over; release needs no VS toolchain |
| Version scheme | Unchanged `yyyy.1MMdd.1HHmm`; PR → `-preview` | Keeps NuGet ordering and `v<version>` tags consistent with history |
| Signing identity | `DevOpsCodeSigning` (`6e6a6823-…`), certificate `CodeSign` in vault `I-Synergy` (access-policy model, permissions already granted) | The only I-Synergy signing identity with vault access; user confirmed the certificate |
| Key Vault signing | GitHub OIDC token written to `AZURE_FEDERATED_TOKEN_FILE` + `-act WorkloadIdentityCredential` | No client secret, no subscription (the identity has a vault access policy, not a role); `azure/login@v2` only authenticates the `az` CLI, so it cannot hand credentials to the signing tool |
| NuGet.org push | `NuGet/login@v1` trusted publishing + `--skip-duplicate` | No long-lived API key; retries are idempotent |
| Concurrency | `cancel-in-progress: false` | A run may be mid-push; only the pending run is replaced |
| `azure-pipelines.yml` | Kept with a deprecation header; delete after the first green GitHub release | Releases must not stop before the GitHub setup is proven |

## Steps

1. [x] Add `.github/workflows/ci.yml` (build + release).
2. [x] Make `scripts/Set-VersionVariables.ps1` write `GITHUB_OUTPUT` when present (still writes
   `##vso[...]` for ADO).
3. [x] Make `scripts/New-GitTag.ps1` emit `::error::` annotations on GitHub (still `##vso[...]`).
4. [x] Deprecation header in `azure-pipelines.yml`.
5. [x] README badge, `.ai/project/*.md`, `docs/MIGRATION_GUIDE.md` Part 11, `CHANGELOG.md`.
6. [x] GitHub environments `acceptance` (no gate) and `production` (approval, main only).
7. [x] Repository variables and the `NUGET_USER` secret (placeholder until nuget.org policy).
8. [x] Azure federated credentials `github-actions-acceptance` / `github-actions-production` on
   `DevOpsCodeSigning`; no role change needed (vault policy already grants certs get/list, keys sign).
9. [ ] nuget.org trusted-publishing policies for `ci.yml` — `acceptance` and `production`.
10. [ ] First PR run (publishes a signed preview) and first `main` run (release + tag).

## Manual prerequisites

1. ~~GitHub → Settings → Environments~~ — `production` created by the user; `acceptance` created.
2. ~~Variables/secrets~~ — done; `NUGET_USER` still a placeholder.
3. ~~Azure federated credentials~~ — done for both environments.
4. nuget.org → Trusted Publishing policies (owner `I-Synergy`, repository `I-Synergy.Framework`,
   workflow `ci.yml`):
   - environment `acceptance` — PR preview packages;
   - environment `production` — release packages.
   The `Environment` field must match the job's environment, so both policies are required.
5. ~~Replace the `NUGET_USER` secret~~ — set to the nuget.org account `I-Synergy`.
6. Push ADO history to GitHub (`git push github --all --tags --force`), set `main` as protected,
   then disable the ADO pipeline before deleting `azure-pipelines.yml`.

## Verification

- YAML parses (PyYAML), both scripts parse, and `Set-VersionVariables.ps1` writes the expected
  `GITHUB_OUTPUT` entries.
- GitHub API read-back of variables, secrets and environments.
- `az ad app federated-credential list` shows both subjects; vault access policy grants the identity.
- First PR run publishes a signed `-preview` package to NuGet.org; first `main` run publishes the
  release set and creates the tag.

## Rollback

`azure-pipelines.yml` is untouched apart from a comment; re-enable the ADO pipeline and revert the
GitHub workflow commit. Tag/version continuity is preserved because the scheme did not change.

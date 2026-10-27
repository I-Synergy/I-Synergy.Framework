# Move CI/CD from Azure DevOps to GitHub Actions
Status: IN PROGRESS
Started: 2026-10-27

## Steps
- [x] Add .github/workflows/ci.yml
- [x] Make the version and tag scripts GitHub-aware
- [x] Deprecate azure-pipelines.yml
- [x] Update README, project docs and CHANGELOG
- [x] Manual: GitHub environment `production` (done by the user) and `acceptance`
- [x] Manual: repository variables + `NUGET_USER` secret (`NUGET_USER=I-Synergy`)
- [x] Manual: Azure federated credentials for both environments (identity already had vault access)
- [x] Manual: nuget.org trusted publishing policies created for `acceptance` and `production`
- [x] Both nuget.org policies use `I-Synergy.Framework*` (covers all 48 package IDs)
- [ ] Manual: first PR run (signed preview to nuget.org), first release run, delete azure-pipelines.yml
   - [x] PR run #6: `build` green (43 min); `release` failed at `azure/login` — the identity has no
     subscription and `azure/login@v2` does not export the workload-identity env vars
   - [x] Fix `b2627afa`: fetch the GitHub OIDC token into `AZURE_FEDERATED_TOKEN_FILE` instead of
     `azure/login`
   - [ ] Confirm the PR run signs and pushes a `-preview` package to nuget.org

## Notes
Plan: `.ai/plans/github-actions-cutover.md`
PR: #171 (`development/main` → `main`).

Run #6 (`20e89cc5`) shape: `build` 43 min, all steps green. `release` job needs
`id-token: write` (present) and, after `b2627afa`, no subscription and no `az` CLI at all.
`AZURE_SUBSCRIPTION_ID` is no longer read; the repository variable can stay or be deleted.

Run #7 (`b2627afa`) was cancelled by run #8 (`a380d494`) — the build job's `cancel-in-progress: true`.
Every push to the PR branch starts a new run even when the push only touches `*.md`, because for
`pull_request` events the `paths-ignore` filter is evaluated against the PR's whole diff, not against
the pushed files. Do not push while a run is in flight unless superseding it is intended.

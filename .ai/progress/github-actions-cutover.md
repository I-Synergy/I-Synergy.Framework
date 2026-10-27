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
- [ ] Manual: nuget.org trusted publishing policies — `acceptance` and `production` — and replace `NUGET_USER`
- [ ] Manual: first PR run (signed preview to nuget.org), first release run, delete azure-pipelines.yml

## Notes
Plan: `.ai/plans/github-actions-cutover.md`

# Fix the release tag push and pin the SDK to global.json

Status: DONE
Completed: 2026-11-11
Started: 2026-11-11

## Steps
- [x] Push the release tag with `RELEASE_TOKEN` instead of `GITHUB_TOKEN`
- [x] Explain the App/`workflows` refusal in `New-GitTag.ps1`
- [x] Install the exact `global.json` SDK version and verify it
- [x] Document `RELEASE_TOKEN` and refresh the recorded SDK version
- [x] Run the release validator

## Notes
- Failure: CI 14 on 2026-10-27, `git push origin v2026.11027.11355` rejected by GitHub.
- Packages were already published when the tag step failed.

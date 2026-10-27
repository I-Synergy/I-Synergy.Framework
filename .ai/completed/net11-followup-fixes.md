# net11-followup-fixes
Status: DONE
Started: 2026-10-24

## Steps
- [x] Fix NETSDK1148: WindowsSdkPackageVersion .57 -> .87 in WinUI tests
- [x] Remove leftover dnceng dotnet9 feed from nuget.config
- [x] Document .NET 11 multi-targeting (README, CHANGELOG, tech-stack)
- [x] Add includePreviewVersions to azure-pipelines.yml SDK step
- [x] Untrack .ai/tmp scratch files + add .gitignore rule
- [x] Verify: full solution build + representative dotnet test runs

## Notes
Plan: .ai/plans/net11-followup-fixes.md

Verification results:
- `dotnet restore I-Synergy.Framework.slnx` -> exit 0 after removing the dnceng source.
- `dotnet build I-Synergy.Framework.slnx` -> 1 error, 10 warnings (was 2 errors, 17 warnings). The remaining error is `samples/Sample.Maui` `net11.0-ios`: "RuntimeIdentifier was changed too late", caused by the local, gitignored `Sample.Maui.csproj.user` pinning `ios-arm64` while the .NET 11 iOS SDK infers a simulator. Repo-side content is unaffected; deleting the `RuntimeIdentifier` block from that user file clears it.
- `dotnet test` -> Core 572 passed, CQRS 38 passed, UI.WinUI 52 passed / 8 skipped, 0 failed.

Deliberately unchanged:
- `SampleTargetFramework` default `net11.0` in `Directory.Build.props` (samples/performance runtime choice).
- No `net11.0` test legs (agreed scope: src libraries only).
- `.ai/completed/net11-multitarget.md` left as the historical record of the original work.

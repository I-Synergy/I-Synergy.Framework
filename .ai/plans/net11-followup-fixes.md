# .NET 11 follow-up fixes

Status: DONE
Scope: repair the gaps found when auditing the committed `net11-multitarget` work (`5edf29cc`).

## Findings driving the fixes

1. `tests/ISynergy.Framework.UI.WinUI.Tests` pins `WindowsSdkPackageVersion=10.0.26100.57`. Under SDK 10 the resulting Windows SDK projection conflict is a warning; SDK 11 turns it into `error NETSDK1148`. Verified: same project builds clean with `10.0.26100.87` (pack already in the local cache).
2. `nuget.config` carries a leftover dnceng `dotnet9` feed keyed by its own URL, mapped to `Aspire*` — which also strips `Aspire*` from the `nuget.org` mapping.
3. No .NET 11 mention in `README.md`, `CHANGELOG.md` or `.ai/project/tech-stack.md` (which still says SDK 10.0.300 / C# 14).
4. `azure-pipelines.yml` installs the SDK via `useGlobalJson: true` but never sets `includePreviewVersions`, and `global.json` now pins a prerelease SDK.
5. `.ai/tmp/` has no ignore rule and 29 scratch files (`dpp.bak`, `fix*.py`, `probe-*`, `*.orig`) are tracked in git.

## Changes (no new application types; no public API change)

1. **tests/ISynergy.Framework.UI.WinUI.Tests.csproj** — `WindowsSdkPackageVersion` 10.0.26100.57 → 10.0.26100.87.
2. **nuget.config** — delete the dnceng feed and its `packageSourceMapping` entry; `nuget.org` keeps pattern `*`.
3. **README.md** — framework blurb + `.NET` badge cover 10 (LTS) and 11 (preview).
4. **CHANGELOG.md** — `[Unreleased] → Added` entry for net10/net11 multi-targeting.
5. **.ai/project/tech-stack.md** — Runtime & Language: SDK 11 RC pin, C# 14, and the `net10.0` (LTS) + `net11.0` (preview) target matrix.
6. **azure-pipelines.yml** — `includePreviewVersions: true` on the `UseDotNet@2` step.
7. **.gitignore** + index — add `.ai/tmp/`, `git rm -r --cached .ai/tmp` (files stay on disk).

## Out of scope (deliberate)

- `SampleTargetFramework` defaulting to `net11.0` in `Directory.Build.props` — deliberate, runtime-only for samples/performance projects.
- No `net11.0` test legs — tests stay `net10.0` per the agreed scope (src libraries only).
- `samples/Sample.Maui` `net11.0-ios` failure comes from the gitignored, machine-local `Sample.Maui.csproj.user` (`RuntimeIdentifier=ios-arm64`), not from the repo.

## Verification

- `dotnet build I-Synergy.Framework.slnx` with `MSBUILDDISABLENODEREUSE=1 -nodeReuse:false`: expect zero errors apart from the known local `Sample.Maui` iOS user-file issue.
- `dotnet test` on representative test projects (Core, CQRS, WinUI) to confirm no regression.
- `dotnet restore` after the `nuget.config` edit to prove Aspire packages still resolve.

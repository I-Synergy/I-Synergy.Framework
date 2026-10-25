# Add .NET 11 (RC) multi-targeting to all src/ libraries

Status: DONE (option A)

## Goal
Every NuGet package under `src/` (48 projects) ships `net10.0` (LTS) **and** `net11.0` (preview/RC) assets.
`net11.0` builds must resolve the .NET 11 RC versions of the framework-aligned packages.

## Findings (investigation done, no code changed)
- All 48 libs are single-TFM `net10.0*`. Variants: WPF (`-windows7.0/8.0/10.0.26100.0`), WinUI (`-windows10.0.26100.0`), MAUI (`-android/-ios/-maccatalyst` + `-windows10.0.26100.0` on Windows).
- Git history: `Directory.Packages.props` used `<ItemGroup Condition="$(TargetFramework.StartsWith('net9.0'))">` (and net8.0) blocks with per-TFM versions until commit `c37b2a27` ("Refactor to target .NET 10.0") collapsed them into one group. `26be02e8` ("Update to .Net 9") introduced the net9 variant. I re-introduce the same pattern for net10 / net11.
- NuGet probe (`.ai/tmp/probe-out.txt`): `11.0.0-rc.2.26504.105` exists for all `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, `Microsoft.Extensions.*`; `Npgsql.EntityFrameworkCore.PostgreSQL 11.0.0-rc.2`; `Microsoft.Maui.Controls 11.0.0-rc.2.26505.5`.
  - **No 11.x for** `Microsoft.Extensions.Http.Resilience`, `Microsoft.Extensions.ServiceDiscovery` (stay 10.11.0 for both TFMs).
  - **`Microsoft.Maui.Controls.Compatibility`: only `11.0.0-preview.5.26304.4`** (no rc/preview.7) - version mismatch risk against Controls rc.2.

## Changes
1. **Directory.Packages.props** - split the framework-aligned versions (AspNetCore, EFCore, Extensions train 10.0.13, Maui.Controls*, Npgsql EFCore) into two groups:
   `Condition="!$(TargetFramework.StartsWith('net11.0'))"` -> 10.0.x (current) and `Condition="$(TargetFramework.StartsWith('net11.0'))"` -> 11.0.0-rc.2. Everything else (third-party, 10.11.0 train) stays unconditional. Your uncommitted version bumps in that file are preserved.
2. **48 csproj** - `<TargetFramework>net11.0</TargetFramework>` -> `<TargetFrameworks>net10.0;net11.0</TargetFrameworks>`; WPF/WinUI/MAUI get the matching `net11.0-*` twins (WPF becomes 6 TFMs). Existing platform conditions in the MAUI csproj extended for `net11.0`.
3. **global.json** - pin SDK `11.0.100-rc.2.26504.105` (`allowPrerelease: true`, `rollForward: latestFeature`), update `tools.dotnet`.
4. **azure-pipelines.yml** - already uses `useGlobalJson: true`; verify it installs the prerelease SDK (`includePreviewVersions`) and workloads for the new band.
5. Verify: `dotnet restore` + `dotnet build` of each TFM (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`), then `dotnet test` on `net10.0`. Delegated to the `developer` agent; progress in `.ai/progress/net11-multitarget.md`.

## Blocking issue - needs your decision
Pinning global.json to SDK 11 conflicts with the current Xcode 27 pin `"workloadVersion": "10.0.401.1"` (commit `c31bf08b`):
- `workloadVersion` is band-specific; a 10.0.401.1 set is invalid under SDK 11.
- This machine has **no `android`/`ios`/`maccatalyst`/`maui` manifests for `11.0.100-rc.2`** (only 10.0.x bands), so `net10.0-ios` etc. (and `net11.0-*` MAUI) would fail to build under an SDK-11 pin until the RC workloads are installed.
- Sample.Maui (net10, Xcode 27) would then also build on the SDK-11 band, with no verified Apple 27 workload pairing.

Options:
- **A (recommended):** keep global.json on SDK 10.0.402 + `workloadVersion`, and add `net11.0` TFMs; the RC SDK is opt-in (CI/dev machines that have it build all TFMs; set `<TargetFrameworks>` net11 only when the SDK is >= 11 via an MSBuild condition so SDK 10 still builds). Preserves the Xcode 27 setup.
- **B (your request):** pin SDK 11 RC, drop the `workloadVersion` pin, install the 11 RC workload set, re-verify Xcode 27 / Sample.Maui. Higher risk; MAUI/iOS/Android net10 builds depend on RC workloads.
- **C:** pin SDK 11 RC for everything except MAUI; keep MAUI at net10 only for now.

Also decide: MAUI `Compatibility` preview.5 for net11 (accept, or drop net11 for the MAUI lib until rc exists).

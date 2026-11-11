# Use the self-contained AzureSignTool.Cli and drop the .NET 10 install

**Status:** IMPLEMENTED 2026-11-11
**Requested:** 2026-11-11
**Context:** `D:\Projects\Github\AzureSignTool.Cli` (commits `8eec00f`, `46b44e0`, `844850e`) now
packs the tool self-contained per RID, so consumers install it without a matching .NET runtime.

## What the tool repo does

- `AzureSignTool.Cli.csproj`: `SelfContained` when a `RuntimeIdentifier` is set,
  `RuntimeIdentifiers=win-x64;win-arm64;linux-x64`, so `dotnet pack` emits one package per RID plus
  a pointer package that selects one by machine RID.
- `ci.yml` verifies each RID package bundles `System.Private.CoreLib.dll` and declares
  `includedFrameworks` (no `framework`).
- Its `release` job installs no .NET at all: signing runs the self-contained `win-x64` executable
  from the package built in the same run, and `dotnet nuget push` runs from `RUNNER_TEMP` so
  `global.json` is not consulted.

Published `AzureSignTool.Cli` versions (`api.nuget.org/v3-flatcontainer`): `2026.11027.11135`,
`2026.11027.11159`, `2026.11030.11358`, `2026.11030.11408`, then the self-contained
`2026.11111.11120` and `2026.11111.11200` — only those two have an `AzureSignTool.Cli.<rid>`
package beside the pointer, and the pin uses the latest, `2026.11111.11200`.

## What changes here

| File | Change |
|---|---|
| `.github/workflows/ci.yml` | Pin `AzureSignTool.Cli` to `2026.11111.11200`; remove the `Install .NET 10 SDK and runtime for AzureSignTool` step from `release` **and** from `build`; document why the tool needs no runtime. |
| `docs/MIGRATION_GUIDE.md` | Step 11.2 item 4 records the self-contained packaging and that the release job installs no .NET 10. |

## Decisions

- **`dotnet tool install --global` stays**: the tool still needs a .NET 10+ CLI to install, but no
  SDK is installed for it — the `windows-2025` image provides one, and both
  `dotnet tool install` and `dotnet nuget push` run from `RUNNER_TEMP` so the repository's
  `global.json` pin (which exists for building) is not consulted.
- **The `release` job installs no .NET at all**: it builds nothing, signs with the self-contained
  tool and tags/releases with `git` and `gh`.
- **`build` loses the step too**: that job never installs or runs AzureSignTool (`git grep` finds it
  only in `ci.yml`), all test projects target `net11.0`, and `global.json` (`rollForward: disable`)
  means SDK 10 was never selected — the step only added 10.x runtimes nobody executes.
- **Older tool versions are unusable without .NET 10**: they are framework-dependent, so the pin
  must not move backwards.

## Verification

1. Package inspection of `AzureSignTool.Cli.win-x64` `2026.11111.11200`: `AzureSignTool.exe`,
   `System.Private.CoreLib.dll`, `tools/any/win-x64/DotnetToolSettings.xml` present.
2. `ci.yml` parses; neither job contains `dotnet-version: '10.0.x'`.
3. `python .ai/tests/validate-release.py` passes.

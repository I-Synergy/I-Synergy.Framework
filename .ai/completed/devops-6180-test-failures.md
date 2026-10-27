# Fix the three failure classes of Azure DevOps build 6180

Status: DONE
Started: 2026-10-26
Plan: `.ai/plans/devops-6180-test-failures.md`

## Steps

- [x] Fix the nine float→decimal test expectations (3 test files)
- [x] Move the WPF and WinUI test projects to .NET 11
- [x] Add `coverlet.collector` to `Sample.Blazor.UI.Tests`
- [x] Verify: Financial / Globalization / Mathematics test runs, plus WPF + WinUI + Blazor test builds

## What changed

- `tests/ISynergy.Framework.Financial.Tests/PercentageTests.cs` — the expected value column is a
  `string` compared through `decimal.Parse(..., InvariantCulture)` in `CalcPercAmountOfAmountTest`
  and `CalcMarginPercentageTest`.
- `tests/ISynergy.Framework.Globalization.Tests/Formatters/CurrencyFormatterTests.cs` —
  `TryParse_ValidInput_ReturnsTrue(string input, string expected)`.
- `tests/ISynergy.Framework.Mathematics.Tests/Matrices/Matrix.Conversion.cs` — `TableToJaggedTest`
  compares against an explicit single-column `decimal[][]` instead of the
  `double[] → decimal[]` round-trip.
- `tests/ISynergy.Framework.UI.WPF.Tests/*.csproj` → `net11.0-windows`.
- `tests/ISynergy.Framework.UI.WinUI.Tests/*.csproj` → `net11.0-windows10.0.26100.0`.
- `tests/Sample.Blazor.UI.Tests/*.csproj` → `coverlet.collector` reference (version from
  `Directory.Packages.props`).

## Verification

- Financial 29/29, Globalization 122/122, Mathematics 954 passed + 1 skipped (was 6 / 2 / 1 failed).
- WPF: build clean, 20/20 tests green on `net11.0-windows` (the run used to abort for lack of a
  net10 runtime).
- WinUI: build succeeded on `net11.0-windows10.0.26100.0` (only the pre-existing CsWinRT1032 warning).
- `Sample.Blazor.UI.Tests`: builds, and `--collect:"XPlat Code Coverage"` now produces
  `coverage.cobertura.xml` instead of "Unable to find a datacollector".

## Open follow-up (not part of this fix)

Once the suite can start, 16 of its 36 tests fail because they still assert the FluentLayout DOM
that commit `cbd17689` deliberately removed from `samples/Sample.Blazor` (`.fluent-layout`,
`nav.fluent-nav`, `#main_menu`, hamburger drawer). The app serves HTTP 200 and renders the new
`.app-shell` / `.app-sidebar` / `nav.app-nav` shell; at `<=900px` the sidebar becomes a 72px icon
rail and there is no drawer at all. Resolved in the same session by adding the drawer and
re-baselining the suite — see `.ai/completed/blazor-shell-mobile-drawer.md`.

## CI outcome (PR 1685)

Three commits were pushed to `development/main` and watched through three pipeline runs:

- **6181** (31da603a) — the three original failure classes were gone; two new ones surfaced:
  `UI.Maui.Windows` `PageFramesField_ExistsAndIsCorrectType` (asserting a field `cbd17689` deleted)
  and `Sample.Blazor.UI.Tests` contrast, `a 'Blazor Sample' ratio=2.37` in the dark theme.
- **6182** (e49ce9c4) — fixed both: a text-safe `--app-accent-text` token (`#5ED3B0` in dark, 10:1
  measured) and `HostBodiesTable_ExistsAndIsCorrectType` guarding `NavigationHostView._bodies`.
  Test task **succeeded** (all 34 projects), but `Sign Nuget packages` failed: `AzureSignTool` is a
  .NET 10 tool and the pinned .NET 11 SDK step leaves only the 11 runtime on the agent.
- **6183** (ebff9ecd) — added a `UseDotNet@2` step installing the `10.0.x` runtime before signing.
  **Build succeeded end to end**: build, pack, verify net10/net11 assets, copy, **sign**, publish
  artifacts, deploy to NuGet, and every test project green (Sample.Blazor 36/36,
  UI.Maui.Windows 45 + 4 skipped, UI.WPF 20/20, UI.WinUI 52 + 8 skipped, ...).

Watch out: `scripts/Update-PackageVersions.ps1` rewrote all package versions in
`Directory.Packages.props` (Aspire/framework preview bumps) at some point during local runs. It was
reverted and is not part of these commits — check that file before committing if the script runs.

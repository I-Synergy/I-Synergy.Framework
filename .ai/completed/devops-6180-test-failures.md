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
rail and there is no drawer at all. Re-baselining those tests (or restoring a mobile drawer) is a
separate decision.

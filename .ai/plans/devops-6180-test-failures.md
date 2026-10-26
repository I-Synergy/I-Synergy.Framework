# Fix the three failure classes of Azure DevOps build 6180

**Status:** APPROVED (user: "fix everything")
**Source:** PR 1685 / build 6180, `chore(tools,perf): target .NET 11` (`286339dd`)
**Scope:** tests only — no production code changes.

## Problem

Build 6180 fails in three independent ways.

1. **Nine assertion failures caused by a .NET 11 breaking change.** Since .NET 11 Preview 7,
   `double` → `decimal` conversions round the *exact* source value once instead of keeping 15
   significant digits (`Decimal and BigInteger floating-point conversions are correctly rounded`).
   `(decimal)0.2d` is `0.2000000000000000111022302463`, not `0.2`. Tests that feed a `double`
   literal (DataRow `object`/`double` parameters) into a `decimal` assertion now fail:
   - `Financial.PercentageTests` — 6 (`CalcPercAmountOfAmountTest` x4, `CalcMarginPercentageTest` x2)
   - `Globalization.CurrencyFormatterTests.TryParse_ValidInput_ReturnsTrue` — 2
   - `Mathematics.MatrixTest.TableToJaggedTest` — 1 (`double[] → decimal[]` vs table→decimal)
   Reproduced locally on SDK 11.0.100-rc.2 with the same counts (6 / 2 / 1).
2. **WPF and WinUI test runs aborted.** `tests/ISynergy.Framework.UI.WPF.Tests` (`net10.0-windows`)
   and `tests/ISynergy.Framework.UI.WinUI.Tests` (`net10.0-windows10.0.26100.0`) were missed by
   `08268f7f test!: run the test projects on .NET 11`. `UseDotNet@2` installs only the 11.0.100-rc.2
   runtime (x64), so the net10 testhost cannot start.
3. **`Sample.Blazor.UI.Tests` aborted at `AssemblyInitialize`** (HTTP 500 → 240 s timeout). The
   missing `IThemeService` registration in `samples/Sample.Blazor/Program.cs` is already fixed by
   `cbd17689`; what remains is the missing `coverlet.collector` reference, which makes the pipeline's
   `--collect:"XPlat Code Coverage"` warn for this project only.

## Existing types reused (rule 12)

No new production types. Only existing test classes, `decimal.Parse(..., InvariantCulture)`, and the
`coverlet.collector` package already pinned in `Directory.Packages.props` (10.1.0).

## Approach

Test expectations never depend on binary floating point again; `.NET 11` conversion behaviour is
left untouched (it is the documented, correct behaviour).

1. `tests/ISynergy.Framework.Financial.Tests/PercentageTests.cs`
   - `CalcPercAmountOfAmountTest` / `CalcMarginPercentageTest`: expected value column becomes
     `string` and is compared as `decimal.Parse(result, CultureInfo.InvariantCulture)`.
     Input columns keep their current types (they are inputs, not expectations).
2. `tests/ISynergy.Framework.Globalization.Tests/Formatters/CurrencyFormatterTests.cs`
   - `TryParse_ValidInput_ReturnsTrue`: `double expected` → `string expected`, DataRows become
     `"123.45"`, `"0.99"`, `"1000.00"`, compared as `decimal.Parse(expected, InvariantCulture)`.
3. `tests/ISynergy.Framework.Mathematics.Tests/Matrices/Matrix.Conversion.cs`
   - `TableToJaggedTest`: replace `ea.GetColumn(1).ToDecimal().ToJagged()[i][j]` (a double→decimal
     round-trip that now carries the exact binary expansion) with an explicit `decimal[][]`
     expectation (`0.3m, 0.1m / 4.2m, 1.4m / 7.3m, 8.7m`) — the DataTable stores text, so the
     table→decimal conversion is exact.
4. `tests/ISynergy.Framework.UI.WPF.Tests/*.csproj` → `net11.0-windows`;
   `tests/ISynergy.Framework.UI.WinUI.Tests/*.csproj` → `net11.0-windows10.0.26100.0`
   (the libraries already multi-target net10 + net11).
5. `tests/Sample.Blazor.UI.Tests/*.csproj` → add the same `coverlet.collector` reference the other
   test projects use.
6. Out of scope, unchanged: `VATTests` (integral values, passes), `ISynergy.Framework.UI.Maui.Mac.Tests`
   (`net10.0-maccatalyst`, explicitly excluded from the Windows pipeline).

## Steps

- [ ] Fix the nine float→decimal test expectations (3 test files)
- [ ] Move the WPF and WinUI test projects to .NET 11
- [ ] Add `coverlet.collector` to `Sample.Blazor.UI.Tests`
- [ ] Verify: Financial / Globalization / Mathematics test runs, plus WPF + WinUI + Blazor test builds

## Verification

`dotnet test` on the three affected test projects (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`),
`dotnet build` on the WPF, WinUI and `Sample.Blazor.UI.Tests` test projects.

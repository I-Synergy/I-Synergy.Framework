# Fix build warnings and the SonarCloud analysis error

Status: DONE
Started: 2026-11-09

## Steps
- [x] Fix the SonarCloud `sonar.issue.ignore.allfile` failure in `ci.yml`
- [x] Bump deprecated GitHub Actions (Node 20 -> Node 24 majors)
- [x] Suppress the port's analyzer diagnostics in the Mathematics csproj
- [x] Silence analyzers for generated `obj/` sources (Strada-generated Razor file)
- [x] Fix Core / CQRS / Automations / Geography / IO / MessageBus warnings
- [x] Fix Mvvm / UI / UI.Maui / UI.WinUI / UI.WPF / UI.Blazor warnings
- [x] Fix Sample.WinUI warnings
- [x] Fix remaining sample / performance / tools warnings
- [x] Build the solution in Release with no warnings in the touched areas
- [x] Run the test suite
- [x] Review the final diff and update `.ai/progress/`

## Notes
- Branch `fix/build-warnings-and-sonar`.
- Warning inventory: 272 locations outside the Mathematics port, resolved one by one.
- Final Release build with the Sonar analyzer injected: **0** Sonar/compiler warnings in
  `src/`, `samples/`, `performance/` and `tools/`; the Mathematics port prints **0**.
  The only remaining warning is the machine-local `XA0033` (local JDK 25 version parsing),
  which the CI runner does not hit because it installs JDK 21.
- `Sample.Blazor.UI.Tests`: 36/36 pass. Every other test project passed in the full run.
- One behavioural regression found and fixed during review: implementing `IDisposable` on the
  two Blazor dialog components made Blazor dispose the view model on close, so the second open
  of the dialog never rendered (caught by
  `CircuitLivenessTests.Dialog_RepeatedOpenAndCloseCycles_LeaveTheCircuitInteractive`).
  The cleanup method is renamed to `DetachViewModel` instead, preserving the previous lifetime.
- The SonarCloud failure comes from an **organization-level** setting
  (`"parentOrigin":"INSTANCE"`), not from this repository; `ci.yml` now overrides it per project
  so the pipeline is unblocked, and the setting should also be corrected in SonarCloud.

## Follow-up (user decision): fix the Mathematics port too

The user chose to fix all 272 Mathematics warnings instead of suppressing them. Plan:
- 6 disjoint directory groups, fixed in two waves, no concurrent builds; central build + tests.
- Remove the project NoWarn afterwards and keep only genuinely unavoidable suppressions, if any.
- Public API changes are explicitly accepted (S107/S3241/S101/S3246/S3427/S4035/S4050).

## Follow-up outcome (2026-11-09)

The Mathematics port is now fixed rather than suppressed:
- All 272 analyzer locations resolved (overload adjacency, TODOs, parameter bundles,
  void returns, unsafe removal, deserialization callbacks, duplicate writes, naming).
- `<NoWarn>` in `ISynergy.Framework.Mathematics.csproj` is back to `CS8981;CS1591`.
- Release build of the project with the analyzer injected: **0 warnings, 0 errors**.
- `ISynergy.Framework.Mathematics.Tests`: 954 passed, 1 skipped.
- Full solution build: 0 errors; full test suite: all projects pass.
- Public API changes are listed in `CHANGELOG.md` (Cdf97, FieldNames method,
  MonteCarloIntegration RNG type, interface variance, sealed QuadraticObjectiveFunction,
  OptimizationStep, OctaveEnvironment field initialization).

## Pragma removal wave (2026-11-09)

User asked to remove the pre-existing blanket `#pragma warning disable` blocks too. Removing
1,412 pragma lines across 264 files exposed 1,974 findings. Status:

- Fixed: 775 mechanical findings (S1905, S1110, S125, S1117, S1066, S1854, S3626, S4136,
  S1764, S2234, S1135, S1939, S2292, S1116, S8969, S3427, S2245, S1450, S108, S101, S2346,
  S4456, S1133, S6640, S1871, S1264) across the Mathematics project.
- Also fixed the S8969 findings this exposed in Core (`RedBlackTree{T}.Balancing.cs`).
- Tests adapted to removed obsolete APIs (`Submatrix` -> `Get`, `Subgroups` -> `Classes.Separate`).
- Found and fixed a control-flow regression from the `goto L90` rewrite in
  `InfiniteAdaptiveGaussKronrod` (caught by `MultipleTests_Examples`).
- Remaining after this wave: **S2368 (582)**, **S907 (296)**, **S1244 (218)**, **S1199 (103)**.
  These are structural: public array-based API, goto-based Fortran ports, exact float
  semantics and nested scoping blocks.
- Verification: Release build 0 errors; Mathematics 954 passed / 1 skipped; full test suite green.

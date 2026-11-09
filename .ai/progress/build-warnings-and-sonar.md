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

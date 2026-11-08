# SonarCloud — known false positives and deliberate deferrals

Source: `I-Synergy_I-Synergy.Framework`, analysis `2026-10-30T13:12:50Z`.
This file records findings that were inspected and **must not be "fixed" by rote**.
It is evidence for a rule-exclusion decision, not a blanket suppression list.

## Analysis method: CI-based since 2026-11-08

Automatic analysis was tried and is **not** the method any more: it runs without the MSBuild
compilation (so it reports framework contracts as smells) and ignores `sonar-project.properties`
(so the Mathematics exclusions did not apply, and the backlog jumped from 0 to 606). The project
is analysed by `dotnet sonarscanner` inside `.github/workflows/ci.yml`: `begin` runs before
Restore/Build, `end` runs after Test so the OpenCover reports are uploaded with the analysis.
Automatic Analysis must stay **off** in SonarCloud — the two methods cannot coexist.

The 25 Mathematics `sonar.issue.ignore.multicriteria` entries live in `sonar-project.properties`
and are passed to the scanner as `/d:` arguments in the workflow (the .NET scanner rejects the
block from `SonarQube.Analysis.xml`). The CPD exclusion is passed the same way.

Findings the build-aware analysis still raises, and why they must not be "fixed" by rote:
S1172 (BindableProperty `propertyChanged` delegate signature), S2325 (bindable-property CLR
wrappers), S8970 (load-bearing null-forgiving operator; `Nullable` is enabled), S110 (fixed
`ViewModelBladeView` inheritance chain), S3011 (reflection over WinUI's protected
`ProtectedCursor`). The sites carry an inline `// NOSONAR` with the reason.

The 2026-11-08 triage also covered 14 findings outside those rules; all were false positives
caused by conditional compilation:

- S8969 — `MauiWinUIApplicationExtensions`: removing `!` from
  `IPlatformApplication.Current` produces CS8602, verified by building the Windows head.
- S3881 — the WPF/MAUI windows inherit `Dispose` from the framework `Window`, and the
  WinUI `View`/`Window` split the pattern across `#if WINDOWS` / iOS / Android branches.
- S3459 — `UpdateService` fields are assigned inside `#if WINDOWS`, which automatic
  analysis does not evaluate.
- S2589 — `DialogService._logger` is an optional constructor parameter.
- S1905 — the casts in `AggregateRepository` and `OpenBaoKeyVaultService` carry the
  nullable annotation (dictionary invariance), not a redundant conversion.

## S8970 — nullable-forgiving operator (229)

Sonar reports *"Remove this null-forgiving operator; nullable warnings are disabled
here."* That premise is false for this repository: `Directory.Build.props` sets
`<Nullable>enable</Nullable>`.

Probe: removing the `!` from
`src/ISynergy.Framework.IO/Watchers/Base/BaseWatcher.cs:143`
(`(TWatcher)Activator.CreateInstance(...)!`) produced:

```
warning CS8600: Converting null literal or possible null value to non-nullable type.
warning CS8602: Dereference of a possibly null reference.
```

The operator is load-bearing. A previous batch removal introduced the same warnings
and was reverted. Resolving these means redesigning each null flow (guard/throw or
`?? throw`), not deleting the operator.

**Handling:** leave open. Fix individually only where a compiler run proves the
operator redundant.

## S2325 — member could be static (221)

- 154 findings are `IValueConverter.Convert` / `ConvertBack` implementations. They
  implement an interface member and cannot be static.
- Most of the remainder are CLR wrappers around `BindableProperty` (MAUI/WPF/WinUI
  controls). XAML binding requires instance properties; making them static breaks the
  control contract.
- A handful of event handlers and private helpers are genuinely static-able; those
  were fixed where found.

**Handling:** candidates for a rule exclusion on converter implementations and
`BindableProperty` wrappers.

## S1172 — unused parameter (40)

Dominated by event-handler signatures (`object? sender, EventArgs e`), `override`
methods, and delegate contracts. Removing a parameter breaks the signature.

**Handling:** do not change. Exclude handler/override signatures.

## S4487 — unread private field (14)

Every flagged field is either a DI constructor parameter stored for lifetime reasons
or used only inside `#if WINDOWS` / `#elif ANDROID` / `#elif IOS` branches, so the
compiled TFM sees it as unused.

**Handling:** do not change. Exclude DI fields and conditional-compilation members.

## S1944 — impossible cast (7)

`(IViewModel)serviceProvider.GetRequiredService(type)` in the navigation services.
`GetRequiredService` returns `object`; the cast is required. The rule's premise
("no type extends '' and implements IViewModel") is wrong for DI-resolved types.

**Handling:** false positive.

## S2699 — test without assertions (7)

- Three are intentional empty placeholders (`InitialTests`, `MessageBus.Tests/UnitTest`,
  `Storage.Tests/UnitTest`) that keep zero-test projects discoverable; the comments
  explain why a tautological assertion is avoided (`MSTEST0032`).
- The other four were fixed with meaningful assertions (Sentry repeat registration,
  UUIDv4 `ToId25`, eigenvalue/jagged NaN construction).

**Handling:** placeholders are deliberate.

## S107 — too many parameters (19)

- 15 are ported Fortran/L-BFGS routines and numerical integration kernels whose
  signatures mirror the upstream source. Repackaging them into option objects hurts
  portability and adds allocations on hot paths.
- `Application` constructors are DI composition roots; an options object would be
  another service, not a simplification.

**Handling:** deliberate; exclude ported numerical routines.

## S3776 — cognitive complexity (9)

Two are `RedBlackTree` balancing algorithms (complexity 26 and 52) where the branches
are the algorithm. The rest are UI orchestration methods (navigation, dialog, theme,
host startup) 1–5 points over the limit. Splitting them is possible but behavior-
sensitive; each needs a dedicated review with UI tests.

**Handling:** deferred to a focused refactor; not a mechanical change.

## S4136 — overload adjacency (110 remaining)

Virtually all remaining findings are in `ISynergy.Framework.Mathematics`, whose
matrix/jagged implementations are a port with an intentionally parallel, generated
layout. Reordering them diverges from the upstream source and from the CPD exclusion
already documented in `sonar-project.properties`.

**Handling:** exclude the Mathematics port from S4136.

## S3241 — return value never used (15)

Ported numerical routines keep their original status-returning signatures (`qzhes`,
`qagi_`, `fwt2d`, …). The return value documents convergence/status and is part of the
port's fidelity.

**Handling:** deliberate.

## S1450 — field could be local (5)

The flagged fields are DI-injected services (`_logger`, `_serviceProvider`) or
MAUI control fields whose lifetime spans methods. Converting them changes public
constructors or control state.

**Handling:** do not change.

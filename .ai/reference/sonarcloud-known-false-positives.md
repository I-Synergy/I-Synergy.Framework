# SonarCloud — known false positives and deliberate deferrals

Source: `I-Synergy_I-Synergy.Framework`, analysis `2026-10-30T13:12:50Z`.
This file records findings that were inspected and **must not be "fixed" by rote**.
It is evidence for a rule-exclusion decision, not a blanket suppression list.

## Automatic analysis (since 2026-11-08)

SonarQube Cloud Automatic Analysis is the project's analysis method now. It ignores
`sonar-project.properties`, so the scanner-side exclusions no longer apply: the 25
`sonar.issue.ignore.multicriteria` Mathematics rules must be recreated in
**Administration > General Settings > Analysis scope > Ignore Issues on Multiple
Criteria**, and the CPD exclusion lives in `.sonarcloud.properties` (read from `main`).

Automatic analysis also runs without the MSBuild compilation, so it reports framework
patterns it cannot bind as smells. PR #174 surfaced 25 such findings. The genuinely
actionable ones were fixed — S3776 (extracted the activation handlers), S3267 (LINQ
loops) — and the contractual ones carry an inline `// NOSONAR` with the reason:
S1172 (BindableProperty `propertyChanged` delegate signature), S2325 (bindable-property
CLR wrappers), S8970 (`Border.Content` is non-nullable in MAUI), S110 (fixed
`ViewModelBladeView` inheritance chain), S3011 (reflection over WinUI's protected
`ProtectedCursor`). Do not "fix" those by changing the signatures.

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

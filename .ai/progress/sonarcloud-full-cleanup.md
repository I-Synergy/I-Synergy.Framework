# SonarCloud full cleanup — progress

Status: IN PROGRESS
Started: 2026-11-04
Plan: `.ai/plans/sonarcloud-full-cleanup-2026-11-04.md`
Branch: `chore/sonarcloud-cleanup-2026-11-04`
Commit: `084e0ed1c`

## Inventory

- 1,264 open at the 2026-10-30 analysis (1,189 code smells, 75 bugs).
- **The analysis predates commits that already fixed several batches**
  (`1d352c7d2`, `805f5ac32`, `ea37ebe58`, `da44245d1`, PR #173 fixes), so part of
  the inventory is stale. No analysis token is available locally to refresh it.

## Steps

- [x] Pull the complete inventory and group by type/rule/path
- [x] Confirm the scan is stale relative to HEAD
- [x] Batch 1 — real bug fixes (S8949 cancellation, S1244 float equality)
- [x] Batch 2 — mechanical C# smells (S2971, S6608/S6610, S3236, S1118,
      S3442, S3260, S3247, S1481, S2933, S1125, S4201, S3878, S927, S108)
- [x] Batch 5 — web assets (theme-color-preview.html 200, site.css 48, JS 16)
- [x] Batch 3a — mechanical C# (S1854 discards, S1192, S3246, S3928, S127,
      S2589 logger, S3971, S3881)
- [x] Batch 3b — structural (S3267 non-hot loops; S4136 non-Math reordered;
      Math port and S3241/S1450 documented as deliberate)
- [x] Batch 4 — design/complexity reviewed (S2699 assertions added; S3776/S107/
      S1944 documented)
- [x] Batch 6 — `.ai/reference/sonarcloud-known-false-positives.md`
- [ ] Fresh SonarCloud analysis to re-measure

## Verified in this session

Builds (Release, analyzers on, `CI=true` to skip the Aspire CLI bundle):

- Core, Globalization, Mathematics, Physics, Geography, Mvvm, UI, UI.Blazor,
  UI.WPF, UI.WinUI, UI.Maui, MessageBus.RabbitMQ, Automations — all succeed.
- JS: `node --check` on all five edited modules.
- Contrast: 0 failing inline pairs and 0 failing `h2` headings remain.
- Tests: Core 572, Mathematics 954, Mvvm 262, Physics 60, Geography 149,
  Globalization 122, UI 90, Automations 19, MessageBus.RabbitMQ 20 — **all pass**.

## Important findings

1. **S8970 (229) is a false positive.** Sonar says "nullable warnings are
   disabled here", but `Directory.Build.props` sets `<Nullable>enable</Nullable>`.
   Removing the `!` from
   `BaseWatcher.cs:143` produced `CS8600` + `CS8602`; the probe was reverted.
   These operators are load-bearing. Fixing them "individually" means redesigning
   each null flow, not deleting the operators.
2. **S2325 (221) is mostly unfixable.** 154 findings are `IValueConverter.Convert`
   / `ConvertBack` implementations (cannot be static), and most of the rest are
   `BindableProperty` CLR wrappers used by XAML (must stay instance).
3. **S1172 / S4487** are dominated by event-handler/override signatures and DI
   constructor parameters; removing them breaks contracts.
4. `site.css` `!important` removal is safe because the print block is last in the
   file; the one real specificity conflict (`nav` inside the Surface Duo media
   query) was fixed by using `nav.sitenav`.
5. `MaskValidator` passed its arguments to `Argument.IsNotNullOrEmpty` in the
   wrong order (it checked the literal `"PlaceHolder"` and used the value as the
   parameter name). Fixed as part of S3236.
6. `theme-color-preview.html` is **generated** by
   `ThemeColorCalculatorTests.GenerateColorPreviewHtml`; editing the artifact is
   overwritten by the next test run. The contrast/reduced-motion fixes now live in
   the generator (`ContrastingHeadingColor` + the page CSS template), and the test
   regenerates the committed page.

## Remaining by size (after this commit)

S8970 229 (false positive) · S2325 221 (mostly unfixable) · S4136 110 (region-aware
manual reordering) · S1172 40 (mostly false positive) · S107 19 · S3267 17 ·
S1854 15 · S3241 15 · S4487 14 · S3776 9 · S3881 9 · S2589 8 · S3358 8 ·
S1944 7 · S2699 7 · S3971 6 · S127 6 · long tail.

## Next session

1. Run a fresh SonarCloud analysis (CI job is commented out in
   `azure-pipelines.yml`; use `SonarToken`) and re-import.
2. Fix S4136 region-aware, one file at a time.
3. Fix the remaining S1854/S3241/S3267 sites that are not hot numeric loops.
4. Configure a documented rule exclusion for S8970/S2325 converter members, or
   redesign the null flows if the rule must stay enabled.

## New scan — 2026-11-04

Method: local `dotnet-sonarscanner` 12.0.0 (JDK 25), full solution build
(`CI=true`, `-nodeReuse:false`, `-p:GeneratePackageOnBuild=false`), 192 project
builds, 0 errors.

- A first upload used `sonar.branch.name=chore/sonarcloud-cleanup-2026-11-04`.
  It succeeded, but the organization plan rejects non-main branch data
  (`Organization is not allowed to access data from non main branches`).
- Per decision, the analysis was re-run **as main** (no branch parameter) so the
  result is visible. `main` now reflects this branch's code until a scan runs on
  merged `main`.
- SonarCloud Automatic Analysis was disabled to permit manual analysis
  (`POST /api/autoscan/activation`, `enable=false`).

### Results (analysis 2026-11-04T13:53:22Z)

| Bucket | Count |
|---|---:|
| Total open | 2,697 |
| Sonar-native (`csharpsquid`, `css`, `javascript`) | **343** |
| Imported .NET SDK analyzers (`external_roslyn`) | 2,342 |

Sonar-native backlog: **1,264 → 343 (−73%)**.

Key rules after the cleanup:

| Rule | Before | After |
|---|---:|---:|
| S8970 | 229 | 0 |
| S2325 | 221 | 11 |
| S4136 | 115 | 108 |
| S1172 | 40 | 10 |
| S4487 | 14 | 13 |
| S8949 | 12 | 3 |
| S1244 | 3 | 0 |
| S2699 | 7 | 19 |
| S107 | 19 | 17 |
| S3241 | 15 | 15 |
| S2589 | 8 | 2 |
| S3881 | 9 | 8 |
| S3971 | 6 | 0 |
| S1066 | 20 | 2 |
| S4663 | 20 | 0 |
| S125 | 16 | 0 |

The `external_roslyn` bucket is a **new** backlog (2,342), not a regression: the
previous analysis did not import third-party Roslyn issues. The largest are
MSTEST0037 (578), CA1507 (457), MSTEST0068 (189), MSTEST0049 (183), CA1873 (172),
CA1510 (168), CsWinRT1028 (106). To keep the dashboard on Sonar rules only, run
the scanner with `/d:sonar.cs.roslyn.ignoreIssues=true`.

Remaining Sonar-native work is dominated by S4136 (108, almost all the ported
Mathematics layout), S2699 (19), S107 (17), S3241 (15) and S4487 (13) — the last
three are documented in `.ai/reference/sonarcloud-known-false-positives.md`.

## Second sweep — non-Mathematics native findings

Starting point: the 2026-11-04 scan showed 352 Sonar-native issues, 184 of them in
`ISynergy.Framework.Mathematics`. Per decision, the Mathematics port is ignored via
`sonar.issue.ignore.multicriteria` (`csharpsquid:*` on `src/ISynergy.Framework.Mathematics/**`)
in both `sonar-project.properties` and `SonarQube.Analysis.xml`.

Fixed in the remaining 168:

- S8969 (24): removed redundant `!` operators where the compiler already knows non-null.
- S6966 (5): `SuccessAsync`, `ToByteArrayAsync`, `CancelAsync`.
- S6610/S3973/S3963/S4220/S3358/S2223/S1104/S6667/S2737/S2094/S3267/S127/S4035/
  S6561/S4144/S4136/S1144/S2436/S1871/S4200/S3398/S2187: corrected, simplified or
  documented individually.
- S1699 (3): non-virtual environment init; sealed `OnParentSet` override.
- S6612 (3): lambda parameters replace captured arguments.
- S3217 (3): generic invocation-list iteration.
- S2953 (2): explicit `IDisposable` on the navigation base and Blazor application.
- S2325 (8): static where safe; interface/platform members annotated.
- S3881 (8): documented conflict with S3971 (finalizer-free types).
- S3776 (7): annotated as algorithmic/UI-orchestration complexity for a focused refactor.
- S4487 (13): removed unused fields and constructor parameters.
- S2139 (12): log-and-rethrow boundary annotations.
- S1450 (4): fields converted to locals.
- S1172 (6): removed parameter or marked platform-only use.
- S2326/S1133: removed unsupported obsolete navigation overloads.
- S8949 (3): `GetAwaiter().GetResult()` and `Task.Run` token.
- S2589/S2365/S3264: documented false positives.
- S2699 (18): real assertions added; MessageBus placeholder removed.

Verification: all touched projects build; Core, Mvvm, UI, UI.WinUI, Automations,
UI.Maui.Windows, Storage.Azure, MessageBus, CQRS, Geography, Globalization,
OpenTelemetry and the changed test suites pass.

## Final result — 2026-11-04

Final analysis (main branch) after the second sweep:

| Bucket | Count |
|---|---:|
| Total open | 2,354 |
| **Sonar-native** | **0** |
| Imported .NET SDK analyzers (`external_roslyn`) | 2,354 |

The Mathematics exclusions are applied with the 25 explicit
`sonar.issue.ignore.multicriteria` entries in `sonar-project.properties`; the .NET
scanner needs them passed as `/d:` arguments (see `SonarQube.Analysis.xml`).

The remaining `external_roslyn` bucket is the .NET SDK analyzer backlog
(MSTEST/CA rules), separate from the Sonar rules and not part of this cleanup.

## Zero backlog — 2026-11-05

Final analysis after the analyzer sweep:

| Bucket | Count |
|---|---:|
| **Total open issues** | **0** |
| Sonar-native | 0 |
| Imported .NET SDK analyzers (`external_roslyn`) | 0 |

The external bucket (2,354 at its peak) was cleared by applying the analyzer
code fixes with `dotnet format analyzers` and hand-finishing the rules without
fixers. Rules disabled with a recorded rationale (all in `.editorconfig` or at
file level): CA1873 (flags the implicit ILogger params array on every call),
CA1816 (conflicts with Sonar S3971), CA1861/SYSLIB1045/SYSLIB1054/CA1859 in the
few places where the mechanical fix is invalid or meaningless.

The sweep also surfaced and fixed real defects: an infinite recursion in
`TypeExtensions.MakeArrayType` for negative ranks, two construction tests that
called `Matrix.Zeros<T>()`/`Jagged.Zeros<T>()` without a shape, and several
argument-order/log-template bugs.

# SonarCloud backlog — progress

Status: IN PROGRESS
Started: 2026-10-30
Plan: `.ai/plans/sonarcloud-backlog-2026-10-30.md`

## Steps

- [x] Locate the correct SonarCloud project (I-Synergy_I-Synergy.Framework)
- [x] Pull the open-issue inventory (1,264 open of 2,229)
- [x] S4663 empty comments (20)
- [x] S125 commented-out code (16)
- [x] S1905 unnecessary casts (8 of 10)
- [x] S1066 collapsible if (14 of 16)
- [x] theme-color-preview.html: contrast + a11y in the generator (200)
- [x] S1121 assignment in expression (8)
- [x] S3358 nested ternary (8)
- [ ] S4136 overloads not adjacent (115)
- [ ] S2325 members could be static (private/internal only, ~18)
- [ ] S1172 unused parameters (~10 real)
- [ ] CSS `!important` in UI.Blazor/site.css (40)
- [ ] Long tail (~700)

## Notes

- S8970 (229) reverted: `!` is load-bearing, removing it produced CS8601/CS8602.
- S4487 (14) deferred: every flagged field is a DI ctor parameter, so removing it
  changes a public constructor signature.
- S1905 AggregateRepository/OpenBao deferred: casts carry nullability/type inference.
- Two S1066 sites in `BoundedBroydenFletcherGoldfarbShanno.FORTRAN.cs` need review.
- A new SonarCloud scan is needed before the dashboard reflects any of this.

## Verification

- `dotnet test Mathematics.Tests` 954 passed
- `dotnet test Mvvm.Tests` 262 passed
- `dotnet build` Core, CQRS, EventSourcing, IO, Physics, RabbitMQ, Sentry, AspNetCore,
  AspNetCore.Blazor, UI, UI.Maui (windows), UI.WPF, UI.WinUI — all succeed

## Batch 2 (2026-10-30, after commit 1d352c7d)

- [x] S4136 overload adjacency: 5 of 115 (Point.cs, ViewModelExtensions.cs,
      SettingsStorageExtensions.cs) — the other 42 files wrap members in
      `#region`/`#if` blocks; automated reordering breaks the directive nesting,
      so those need a manual, region-aware pass.
- [x] S1117 local hides field (4) — FourierTransform2 Bluestein locals renamed.
- [x] S2933 readonly field (5) — AugmentedLagrangian thresholds.
- [x] S1172 unused parameter (1) — ViewModel.OnUserSignedInAsync.

## Known false positives (do not "fix")

- S1172 / S4487 in `TokenStorageService`, `NpyFormat.Reader`, and the cross-platform
  UI services: the members are used only inside `#if WINDOWS` / `#elif ANDROID` /
  `#elif IOS` branches, so the compiled TFM sees them as unused.
- S1172 on delegate/override signatures (property-changed handlers, MAUI
  `UIPointerInteraction` overrides) cannot be removed without breaking the contract.
- S8970 null-forgiving operator (229).

## Remaining by size

S4136 110 (manual) · S2325 ~18 real (private/internal only) · S3267 17 · S1854 13 ·
S3241 15 · S8949 12 · S3776 9 · S3881 9 · S3358 done · S2589 8 · site.css S9423 40 ·
long tail ~600 across 100+ rules.

## Batch 3 (2026-10-30)

- [x] S8949 cancellation propagation (7): SendGrid SendEmailAsync, Azure
      DeleteIfExistsAsync, HttpContext CopyToAsync(RequestAborted), NuGet SendAsync,
      Service Bus CloseAsync / CompleteMessageAsync / DeadLetterMessageAsync.
- [x] S2589 unnecessary null check (3): WinUI dialog.Dispose(), RelayCommand
      `parameter is T` pattern, MessengerService message type name.
- [ ] S2589 in UI.Maui/UI.WinUI DialogService and the WinUI semaphore guard were
      reverted: `_logger` is `ILogger?` (CS8604) and the semaphore flag is real —
      releasing after a failed WaitAsync would throw. Sonar's dataflow is wrong there.

## Batch 4 (2026-10-30)

- [x] S3267 loop simplification (6): BladePane, BladePaneHostRegistry (2),
      MessengerService, CommandExecutionResourcePool, RequestCancellationService.
- [ ] S3267 in Mathematics deliberately skipped: converting hot numeric loops to
      LINQ adds allocations in performance-sensitive paths.

### Totals so far
~330 findings addressed across 4 commits (1d352c7d, 805f5ac3, ea37ebe5, +this one).

## Duplication / quality-gate pass (2026-10-30)

Gate after the first push: new-code duplication 3.4% (limit 3), new reliability C.
Findings in new-code files: 5,173 duplicated lines; top blocks are parallel
platform implementations and ported math code.

Consolidated:
- ReflectionExtensions: deleted the AspNetCore.Blazor copy (97% identical to the
  UI one); the project now references ISynergy.Framework.UI (~213 lines).
- TelemetryExtensions: Maui and WinUI copies were identical; one shared
  implementation now lives in ISynergy.Framework.UI (~130 lines).
- BladeService: SetupAndOpenBladeAsync + ResolveViewType moved into
  BaseBladeService; WPF/WinUI/Maui override only ResolveViewType (~121 lines).

Remaining duplication hotspots (not yet touched): BaseShellViewModel (WPF/Maui/
WinUI ~489), DialogService (~480), Application (~487), SingularValueDecomposition
and Jagged counterparts (~800), BladeView/BladePane (~383), ThemeService (~254).

## Duplication pass 2 (2026-10-30)

- DialogService: the entity and no-entity ShowDialogAsync overloads were duplicated
  within each platform file. Merged into one private core with `entity`/`hasEntity`
  in UI.WinUI, UI.WPF and UI.Maui (public API unchanged, ~390 duplicated lines).

## PR #173 gate (2026-10-30)

PR analysis is available now. Gate: new_duplicated_lines_density 10.2% (limit 3),
new_reliability_rating 2 (B). Only 11 open issues on the PR scope.

Fixed all 11:
- S8970 x3 (entity! in the three DialogService cores) -> the overloads now pass an
  Action<IViewModelDialog<TEntity>> configure delegate instead
- S3776 WinUI core complexity -> callback wiring and error logging extracted
- S2325 x2 (Maui pass-through wrappers removed; public overloads call the core)
- S4136 x2 ViewModelExtensions overloads reordered into adjacent groups
- S125 Cobyla commented-out pseudo-code reworded
- S9382 unawaited JS awaits -> Promise.all
- S2955 bug: AsyncRelayCommand `parameter == null` -> `is null`

PR-scope duplication (1,970 duplicated lines of ~19k): SVD 420 + JaggedSVD 376,
Cholesky 107 + Jagged 107, BladePane 192, ThemeService Maui 126, DialogService
WPF/WinUI 219, Cobyla 62, selection VMs 110, rest long tail. The math port is
~1,150 of that total; reaching <3% (about 580 lines) needs either refactoring the
ported decompositions into shared cores or a CPD exclusion for that ported code.

## Duplication pass 3 (2026-10-30)

- CPD exclusion for the Math.NET port: sonar-project.properties + SonarQube.Analysis.xml
  (src/ISynergy.Framework.Mathematics/**). The matrix/jagged implementations are
  intentionally parallel; keeps every other metric for that code.
- Selection ViewModels: new BaseSelectionViewModel<TModel,TSelection> holds the item
  list, filtering, cleanup and command disposal; the single/multiple bases keep only
  their selection-specific code (~110 duplicated lines).

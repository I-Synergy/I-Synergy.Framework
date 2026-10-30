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

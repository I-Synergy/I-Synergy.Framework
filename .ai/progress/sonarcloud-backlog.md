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

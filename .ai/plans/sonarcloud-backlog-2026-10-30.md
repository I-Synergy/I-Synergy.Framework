# SonarCloud backlog cleanup

Status: IN PROGRESS
Started: 2026-10-30

## Source of truth

SonarCloud project: `https://sonarcloud.io/project/overview?id=I-Synergy_I-Synergy.Framework`
- organization `i-synergy-github`, key `I-Synergy_I-Synergy.Framework`
- analysis `2026-10-30T13:12:50Z`, version `2026.10607.11905-preview`
- issues at start: 2,229 total / 1,264 open (900 code smells C#, plus CSS/HTML/JS)

Note: the `.sonarlint/I-Synergy.Framework.json` file points at a different, stale project
(`I-Synergy.Framework`, organization `i-synergy`, last analysis 2026-04-08). Ignore it.

## Strategy

Work rule-by-rule, largest and safest first. Each batch is verified with
`dotnet build` (Debug disables analyzers, Release enables them) plus the relevant
`dotnet test` suite. Behavior-preserving changes only; no public-API breaks.

## Batches

- [x] S4663 empty comments (20)
- [x] S125 commented-out code (16)
- [x] S1905 unnecessary casts (8 of 10; 2 need design decisions)
- [x] S1066 collapsible if (14 of 16; 2 `BoundedBroydenFletcherGoldfarbShanno.FORTRAN.cs` need review)
- [ ] S2325 members could be static (221) — only private/internal, public API must stay instance
- [ ] S4136 overloads not adjacent (115) — pure reordering
- [ ] S1172 unused parameters (40) — event-handler signatures are false positives
- [ ] S3267 LINQ simplification (17)
- [ ] S1854 useless assignments (15)
- [ ] S3241 return value never used (15)
- [ ] S4487 unread private fields (14)
- [ ] S8949 cancellation token not propagated (12)
- [ ] S3776 cognitive complexity (9)
- [ ] Generated assets: `theme-color-preview.html` (200) + CSS (166) — needs a decision
- [ ] Remaining long tail (~600 across 100+ rules)

## Deliberately not fixed

- S8970 "remove null-forgiving operator": the `!` is load-bearing (`default!` for
  unconstrained `T`, non-null flow across repeated calls). Removing all 229 introduced
  CS8601/CS8602 warnings; batch reverted. Candidate for a SonarCloud false-positive rule.
- S1905 `AggregateRepository.cs` `.Cast<IDomainEvent>()` — required for nullability flow.
- S1905 `OpenBaoKeyVaultService.cs` `(string?)` — changes dictionary element type.

## Verification per batch

```
MSBUILDDISABLENODEREUSE=1 dotnet build <project> -nodeReuse:false
MSBUILDDISABLENODEREUSE=1 dotnet test <tests> -nodeReuse:false
```

# SonarCloud full cleanup — 2026-11-04

Status: IN PROGRESS (batches 1, 2 and 5 landed in `084e0ed1c`)
Started: 2026-11-04
Source: `https://sonarcloud.io/project/issues?id=I-Synergy_I-Synergy.Framework`
Project key: `I-Synergy_I-Synergy.Framework` (org `i-synergy-github`)

## Snapshot

- **1,264 open issues** (1,189 code smells, 75 bugs; no vulnerabilities)
- Severity: 16 BLOCKER, 46 CRITICAL, 454 MAJOR, 742 MINOR, 6 INFO
- Files: 997 C#, 200 HTML, 50 CSS, 16 JS, 1 PowerShell
- **Scan age:** latest analysis is `2026-10-30T13:12:50Z`; `HEAD` is `0190d8be1`
  (2026-11-04). Prior sessions fixed S4663/S125/S1066/S1905/S1121/S3358 and much of
  `theme-color-preview.html`, but **no analysis has run since**, so the inventory
  contains work that is already done. Batch 0 reconciles before fixing.
- 106 distinct rules. Full table below.

## Approach

Rule-by-rule ("grouped by type"), largest and safest first, behavior-preserving.
Each batch: apply → `MSBUILDDISABLENODEREUSE=1 dotnet build` (Release, analyzers on)
→ relevant `dotnet test`. A new SonarCloud scan is required at the end; the dashboard
cannot be queried for verification from here (anonymous API, no analysis token).

## Batches

### Batch 0 — Baseline and reconciliation
- [ ] Build baseline (Release, analyzers on).
- [ ] Re-check the six batches the prior session marks done; record leftovers.
- [ ] Regenerate the authoritative file:line work list from the current code.

### Batch 1 — Bugs (75)
- [ ] `Web:MouseEventWithoutKeyboardEquivalentCheck` 42 — all in `theme-color-preview.html`
- [ ] `csharpsquid:S8949` 12 — cancellation not propagated
- [ ] `csharpsquid:S4143` 6 — index overwritten (decompositions)
- [ ] `csharpsquid:S1244` 3 — float equality
- [ ] `csharpsquid:S2583` 3 — always-false condition
- [ ] `csharpsquid:S2955` 2 — `== null` on unconstrained generic
- [ ] `javascript:S9383` 2, `javascript:S1226` 2, `javascript:S930` 1
- [ ] `csharpsquid:S1751` 1, `css:S9424` 1

### Batch 2 — Mechanical C# smells (high confidence)
- [ ] `S1066` 20 collapsible if · `S4663` 20 empty comment · `S125` 16 dead code
- [ ] `S1854` 15 useless assignment · `S3241` 15 return value unused
- [ ] `S1905` 10 unnecessary cast · `S1121` 8 assignment in expression
- [ ] `S3358` 8 nested ternary · `S1481` 3 unused local · `S2933` 6 readonly
- [ ] `S1117` 4 local hides field · `S4201` 2 · `S6610` 7 · `S6608` 5
- [ ] `S1125` 2 · `S3878` 2 · `S3236` 2 · `S1118` 3 · `S3442` 3 · `S3260` 4
- [ ] `S101` 4 · `S927` 3 · `S108` 2 · `S3264` 1 · `S4200` 1 · `S2094` 1
- [ ] `S2737` 1 · `S1994` 1 · `S2223` 1 · `S1104` 1 · `S1939` 1 · `S2234` 1
- [ ] `javascript:S6582` 3 · `S7761` 2 · `S1126` 1 · `S1788` 1 · `S1481` 1
- [ ] `css:S4666` 4 duplicate selector · `css:S125` 2 · `S4667` 1 · `S4658` 1

### Batch 3 — Structural C# smells
- [x] `S2325` 221 could be static — private/internal event handlers fixed; converter
      implementations and BindableProperty wrappers are false positives (see reference doc)
- [x] `S4136` non-Mathematics sites reordered; the Mathematics port is excluded (see reference doc)
- [x] `S3267` remaining non-hot loops converted; Mathematics hot loops skipped
- [ ] `S1450` 5 field → local · `S1144`-family leftovers · `S4144` 4 duplicate bodies
- [ ] `S1192` 5 string literals · `S2971` 4 · `S2953` 3 · `S1699` 3 · `S3217` 3
- [ ] `S2326` 3 · `S3247` 2 · `S2365` 2 · `S6612` 3 · `S3427` 4 · `S3220` 4
- [ ] `S4035` 4 · `S2436` 4 · `S3246` 6 · `S1133` 4 · `S1135` 2 · `S6966` 3

### Batch 4 — Design / complexity / correctness judgement
- [x] `S3776` reviewed (2 algorithmic, 7 UI orchestration deferred) · `S107` ported/DI (see reference doc)
- [ ] `S3881` 9 IDisposable pattern · `S3971` 6 GC.SuppressFinalize
- [ ] `S2589` 8 unnecessary null check · `S127` 6 loop counter · `S3928` 6 param name
- [ ] `S1944` 7 impossible cast · `S1244` remaining · `S6561` 3 DateTime.Now
- [ ] `S6672` 2 logger type · `S2699` 7 missing assertions · `S2187` 1 · `S8969` 1
- [ ] `S2486` 1 · `S3963` 1 · `S4220` 1 · `S3398` 1 · `S4050` 1
- [ ] `powershelldre:S8677` 1

### Batch 5 — Web assets
- [ ] `theme-color-preview.html` — 200 issues (contrast 114, a11y 84, reduced motion 2)
  **pending scope decision**
- [ ] `site.css` — `S9423` 40 `!important`, `S4666` 4, `S9424` 1
- [ ] JS — `cache_storage_accessor.js`, `cookie_consent.js`, other `wwwroot/js`

### Batch 6 — Documented false positives (suppress with rationale, do not change)
- [ ] `S1172` 40 unused parameter — most are event-handler/override signatures
- [ ] `S4487` 14 unread field — DI ctor parameters / `#if`-only use
- [ ] `S8970` 229 null-forgiving — **pending scope decision** (prior removal attempt
      introduced CS8601/CS8602 and was reverted)
- [ ] `S2325`/`S1172` in cross-platform services used only in `#if WINDOWS/ANDROID/IOS`

## Full rule table (106 rules)

| Count | Rule | Type |
|---:|---|---|
| 229 | csharpsquid:S8970 | CODE_SMELL |
| 221 | csharpsquid:S2325 | CODE_SMELL |
| 115 | csharpsquid:S4136 | CODE_SMELL |
| 114 | css:S7924 | CODE_SMELL |
| 42 | Web:S6848 | CODE_SMELL |
| 42 | Web:MouseEventWithoutKeyboardEquivalentCheck | BUG |
| 40 | csharpsquid:S1172 | CODE_SMELL |
| 40 | css:S9423 | CODE_SMELL |
| 20 | csharpsquid:S1066 | CODE_SMELL |
| 20 | csharpsquid:S4663 | CODE_SMELL |
| 19 | csharpsquid:S107 | CODE_SMELL |
| 17 | csharpsquid:S3267 | CODE_SMELL |
| 16 | csharpsquid:S125 | CODE_SMELL |
| 15 | csharpsquid:S1854 | CODE_SMELL |
| 15 | csharpsquid:S3241 | CODE_SMELL |
| 14 | csharpsquid:S4487 | CODE_SMELL |
| 12 | csharpsquid:S8949 | BUG |
| 10 | csharpsquid:S1905 | CODE_SMELL |
| 9 | csharpsquid:S3776 | CODE_SMELL |
| 9 | csharpsquid:S3881 | CODE_SMELL |
| 8 | csharpsquid:S2589 | CODE_SMELL |
| 8 | csharpsquid:S3358 | CODE_SMELL |
| 8 | csharpsquid:S1121 | CODE_SMELL |
| 7 | csharpsquid:S1944 | CODE_SMELL |
| 7 | csharpsquid:S2699 | CODE_SMELL |
| 7 | csharpsquid:S6610 | CODE_SMELL |
| 6 | csharpsquid:S127 | CODE_SMELL |
| 6 | csharpsquid:S2933 | CODE_SMELL |
| 6 | csharpsquid:S3246 | CODE_SMELL |
| 6 | csharpsquid:S3928 | CODE_SMELL |
| 6 | csharpsquid:S3971 | CODE_SMELL |
| 6 | csharpsquid:S4143 | BUG |
| 5 | csharpsquid:S1192 | CODE_SMELL |
| 5 | csharpsquid:S1450 | CODE_SMELL |
| 5 | csharpsquid:S6608 | CODE_SMELL |
| 4 | csharpsquid:S101 | CODE_SMELL |
| 4 | csharpsquid:S1117 | CODE_SMELL |
| 4 | csharpsquid:S1133 | CODE_SMELL |
| 4 | csharpsquid:S2436 | CODE_SMELL |
| 4 | csharpsquid:S2971 | CODE_SMELL |
| 4 | csharpsquid:S3220 | CODE_SMELL |
| 4 | csharpsquid:S3260 | CODE_SMELL |
| 4 | csharpsquid:S3427 | CODE_SMELL |
| 4 | csharpsquid:S4035 | CODE_SMELL |
| 4 | csharpsquid:S4144 | CODE_SMELL |
| 4 | css:S4666 | CODE_SMELL |
| 3 | csharpsquid:S1118 | CODE_SMELL |
| 3 | csharpsquid:S1481 | CODE_SMELL |
| 3 | csharpsquid:S1699 | CODE_SMELL |
| 3 | csharpsquid:S2326 | CODE_SMELL |
| 3 | csharpsquid:S2583 | BUG |
| 3 | csharpsquid:S2953 | CODE_SMELL |
| 3 | csharpsquid:S3217 | CODE_SMELL |
| 3 | csharpsquid:S3442 | CODE_SMELL |
| 3 | csharpsquid:S6561 | CODE_SMELL |
| 3 | csharpsquid:S6612 | CODE_SMELL |
| 3 | csharpsquid:S6966 | CODE_SMELL |
| 3 | csharpsquid:S927 | CODE_SMELL |
| 3 | javascript:S6582 | CODE_SMELL |
| 2 | csharpsquid:S108 | CODE_SMELL |
| 2 | csharpsquid:S1125 | CODE_SMELL |
| 2 | csharpsquid:S2365 | CODE_SMELL |
| 2 | csharpsquid:S2955 | BUG |
| 2 | csharpsquid:S3236 | CODE_SMELL |
| 2 | csharpsquid:S3247 | CODE_SMELL |
| 2 | csharpsquid:S3459 | CODE_SMELL |
| 2 | csharpsquid:S3878 | CODE_SMELL |
| 2 | csharpsquid:S3973 | CODE_SMELL |
| 2 | csharpsquid:S4201 | CODE_SMELL |
| 2 | csharpsquid:S6672 | CODE_SMELL |
| 2 | css:S125 | CODE_SMELL |
| 2 | css:S9440 | CODE_SMELL |
| 2 | javascript:S1226 | BUG |
| 2 | javascript:S7761 | CODE_SMELL |
| 2 | javascript:S9383 | BUG |
| 1 | csharpsquid:S1104 | CODE_SMELL |
| 1 | csharpsquid:S1751 | BUG |
| 1 | csharpsquid:S1939 | CODE_SMELL |
| 1 | csharpsquid:S1994 | CODE_SMELL |
| 1 | csharpsquid:S2094 | CODE_SMELL |
| 1 | csharpsquid:S2187 | CODE_SMELL |
| 1 | csharpsquid:S2223 | CODE_SMELL |
| 1 | csharpsquid:S2234 | CODE_SMELL |
| 1 | csharpsquid:S2486 | CODE_SMELL |
| 1 | csharpsquid:S2737 | CODE_SMELL |
| 1 | csharpsquid:S3264 | CODE_SMELL |
| 1 | csharpsquid:S3398 | CODE_SMELL |
| 1 | csharpsquid:S3963 | CODE_SMELL |
| 1 | csharpsquid:S4050 | CODE_SMELL |
| 1 | csharpsquid:S4200 | CODE_SMELL |
| 1 | csharpsquid:S4220 | CODE_SMELL |
| 1 | csharpsquid:S8969 | CODE_SMELL |
| 1 | css:S4658 | CODE_SMELL |
| 1 | css:S4667 | CODE_SMELL |
| 1 | css:S9424 | BUG |
| 1 | css:S9449 | CODE_SMELL |
| 1 | javascript:S1121 | CODE_SMELL |
| 1 | javascript:S1126 | CODE_SMELL |
| 1 | javascript:S1226 | BUG |
| 1 | javascript:S1481 | CODE_SMELL |
| 1 | javascript:S1788 | CODE_SMELL |
| 1 | javascript:S1854 | CODE_SMELL |
| 1 | javascript:S2486 | CODE_SMELL |
| 1 | javascript:S930 | BUG |
| 1 | powershelldre:S8677 | CODE_SMELL |

## Verification

```
MSBUILDDISABLENODEREUSE=1 dotnet build I-Synergy.Framework.slnx -c Release -nodeReuse:false
MSBUILDDISABLENODEREUSE=1 dotnet test <affected test projects> -c Release -nodeReuse:false
```

End state: working tree diff limited to intended changes; a fresh SonarCloud analysis
is required to confirm the count drop.

## Decisions taken

1. `theme-color-preview.html` — fix properly (contrast, reduced motion). Done.
2. `S8970` — fix individually only where provably redundant. Sampled
   `BaseWatcher.cs:143`; removal raised `CS8600`/`CS8602`, so the operators are
   load-bearing. Those 229 sites need null-flow redesign, not deletion.
3. Overall scope — full sweep, bugs first. Batches 1, 2 and 5 are complete;
   3, 4 and 6 remain.

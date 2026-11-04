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
- [ ] Batch 3 — structural smells (S2325, S4136, S3267, S1192, S4144, …)
- [ ] Batch 4 — design/complexity (S3776, S107, S3881, S3971, S2699, …)
- [ ] Batch 6 — suppress documented false positives
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

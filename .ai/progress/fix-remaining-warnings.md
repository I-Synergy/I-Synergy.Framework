# Eliminate remaining 85 solution warnings (trim chain + MSTest + samples)
Status: IN PROGRESS
Started: 2026-07-15
Plan: .ai/plans/steady-cooking-storm.md (summary) + .ai/plans/steady-cooking-storm-agent-a65e43f7a23e5e373.md (verbatim code)

## Steps

### Part A — framework trim chain (ILLink)
- [x] A1: Core ReflectionExtensions — RUC on assembly walkers; relax DAM(All) on IsSingleton/IsScoped/IsFreeApplication
- [x] A2: UI ReflectionExtensions — RUC on To*Types/Get*Types/plural Register*s; DAM(Interfaces|PublicConstructors) on singular Register*; FindAbstraction helper
- [x] A3: UI ServiceCollectionExtensions — RUC on both RegisterAssemblies overloads
- [x] A4: WinUI/WPF/MAUI app builders — boundary suppression on ConfigureServices; remove #pragma blocks; manual binding for options; MAUI parity (DAM on generic params)
- [x] A5: Mvvm ViewModelExtensions.GetRelatedViewType — method-level suppression
- [x] A6: Core EnumExtensions GetLocalizedDescription/GetDescription — IL2075 suppression
- [x] A7: Core ObservableClassExtensions.GetProperties — IL2075 suppression
- [x] A8: OpenTelemetry OtlpTelemetryExtensions — manual binding replaces Bind
- [x] A9: Module-level XamlTypeInfo suppressions (UI.WinUI + Sample.WinUI)
- [x] A10: Sample.WinUI SettingsService — JsonSerializerContext for LocalSettings (fixes live publish crash)
- [x] A11: AspNetCore.Blazor duplicates — same treatment as A2/A3
- [x] A12: Trim .editorconfig obj/**.g.cs section if redundant after A9 (verify first) — VERIFIED STILL NEEDED for WPF .g.cs (IL2026/IL2067/IL2075 resurface without it); section KEPT with updated comment
- [x] A-verify: solution Release build + Sample.WinUI trimmed build → only 7x IL2104 residual (confirmed: solution build 47 warnings = 7 IL2104 + 40 MSTEST/BL0007/CA1416/CS8602 for Parts B/C; trimmed Sample.WinUI = 7 IL2104 + 1 pre-existing NETSDK1198, zero ISynergy IL warnings)

### Part B — MSTest cleanup
- [ ] B1: MSTEST0017 swapped args (14 sites)
- [ ] B2: MSTEST0065 arrays → AreSame/AreNotSame (6 sites, incl. overlapping 0017s)
- [ ] B3: MSTEST0032 always-true asserts (ArgumentTests, UtmGridTests, 3 placeholders, DomainEventBaseTests, 4 Eigen/SVD sites)
- [ ] B4: MSTEST0044 DataTestMethod → TestMethod (2 sites)
- [ ] B5: MSTEST0030 MailServiceTests — [TestClass] + class-level [Ignore] (never run: real Graph calls)
- [ ] B6: MSTEST0014 PointTest — add [TestMethod] + fix swapped args at line 50
- [ ] B7: CS8602 NavigationServiceRetentionCycleTests — null-forgiving ToString()
- [ ] B-verify: dotnet test all touched projects green; Mail tests 4 skipped

### Part C — samples
- [ ] C1: BL0007 BaseEmptyDialog/BaseModelDialog — auto-property + OnParametersSet lifecycle (fixes double-subscription bug)
- [ ] C2: CA1416 ApplicationMonitor — guard TotalProcessorTime outside lambda
- [ ] C-verify: solution Release build clean of BL0007/CA1416

## Notes
- ILLink ignores #pragma/.editorconfig — attribute-based mechanisms only.
- A1-A4+A11 must land atomically (interim RUC would create new analyzer warnings).
- Expected residual after all parts: exactly 7x IL2104 (third-party).

# Eliminate remaining 85 solution warnings (trim chain + MSTest + samples)

## Context

Commit f297f51 made the three UI library projects analyzer-clean, but a full Release solution build still shows 85 warnings in three buckets: (1) ~36 publish-time ILLink trim warnings surfaced through Sample.WinUI's `PublishTrimmed=true` (framework assembly-scanning chain, Bind calls, generated XamlTypeInfo, sample JSON), (2) ~37 MSTest analyzer warnings in tests, (3) 3 sample-app warnings (BL0007 ×2, CA1416). The user chose to fix all three buckets.

Key constraint discovered: **ILLink does not honor `#pragma` or `.editorconfig`** — only attributes (`[RequiresUnreferencedCode]`, `[UnconditionalSuppressMessage]`, incl. module-level). So the WinUI/WPF `Bind` pragmas and the `.editorconfig` `obj/**.g.cs` rule from f297f51 silence the build analyzer but resurface at trimmed publish — this plan replaces them with ILLink-visible mechanisms.

Bonus live-bug fix: `PublishTrimmed=true` sets `JsonSerializer.IsReflectionEnabledByDefault=false` (confirmed in Sample.runtimeconfig.json), so Sample.WinUI's `SettingsService` JSON calls **throw NotSupportedException at startup in published Release today**. The source-gen fix in Step A10 repairs this.

**Detailed plan with verbatim code per file: `.ai/plans/steady-cooking-storm-agent-a65e43f7a23e5e373.md` (581 lines) — the implementing agent MUST read it; steps below are the executive summary.**

## Part A — Framework trim chain (~36 ILLink warnings)

Design: honest `[RequiresUnreferencedCode]` on the fundamentally trim-unsafe scanning APIs, DAM annotations where flow is provable, `[UnconditionalSuppressMessage]` only at the `ConfigureServices` boundary (justification: view/viewmodel/window types are rooted by the app's XAML compilation) and for lambda-hosted patterns that can't be annotated. Replace all `ConfigurationBinder.Bind` on small option types with manual trim-safe binding via the `IConfigurationSection` string indexer.

1. **Core\ReflectionExtensions.cs** — RUC on `GetAllReferencedAssemblyNames` + `GetAllReferencedAssemblies` (Assembly.Load walkers). Remove unneeded `DAM(All)` from `IsSingleton`/`IsScoped`/`IsFreeApplication` (they only call `Attribute.GetCustomAttribute` — no DAM requirement; pure relaxation).
2. **UI\ReflectionExtensions.cs** — RUC on `ToViewModelTypes`/`ToViewTypes`/`ToWindowTypes` + `Get*Types` + plural `RegisterWindows/Views/ViewModels`; DAM `Interfaces | PublicConstructors` on singular `RegisterWindow/RegisterView/RegisterViewModel` Type params; drop DAM from private `Register`'s `abstraction` param (DI service type — unannotated in MS.Ext.DI); shared `FindAbstraction` helper (named method) carrying the IL2070 suppression for the `GetInterfaces()` predicate.
3. **UI\ServiceCollectionExtensions.cs** — RUC on both `RegisterAssemblies` overloads.
4. **App builders (WinUI `WindowsAppBuilderExtensions`, WPF `WPFAppBuilderExtensions`, MAUI `MauiAppBuilderExtensions`)** — boundary `UnconditionalSuppressMessage IL2026` on `ConfigureServices`; REMOVE the f297f51 `#pragma IL2026` blocks and replace `Configure<T>(...Bind)` with manual binding (ApplicationFeatures 1 prop, ClientApplicationOptions 3, UpdateOptions 4, AnalyticOptions 2). MAUI additionally gets the DAM annotations on its 4 generic params (currently missing) for parity.
5. **Mvvm\ViewModelExtensions.cs `GetRelatedViewType`** — method-level suppression (registry-first path is AOT-safe; reflection fallback only finds app-referenced types).
6. **Core\EnumExtensions.cs `GetLocalizedDescription`/`GetDescription`** — method-level IL2075 suppression (runtime enum type's members preserved via annotated T in practice; GetType() deliberate per existing comment).
7. **Core\ObservableClassExtensions.cs `GetProperties`** — method-level IL2075 suppression (no signature change — generic rewrite would change behavior for derived types).
8. **OpenTelemetry\OtlpTelemetryExtensions.cs** — replace `Bind` with manual binding (5 scalars + nested `BatchExportProcessorOptions`), following the file's existing reflection-free `Map` precedent.
9. **XamlTypeInfo IL2026 ×4** — module-level `[module: UnconditionalSuppressMessage(..., Scope="member", Target="M:...Activate_49_EventTriggerBehavior")]` files in ISynergy.Framework.UI.WinUI and Sample.WinUI (suppressions must live in the warning assembly; doc-ID format, no parens). Fragile to XAML index shifts but fails safe.
10. **Sample.WinUI SettingsService** — add `[JsonSerializable(typeof(LocalSettings))]` JsonSerializerContext and use it in Serialize/Deserialize. Verified byte-compatible: base-class public members already `[JsonIgnore]`, LocalSettings props all simple. **Fixes the live Release-publish startup crash.**
11. **AspNetCore.Blazor duplicates** (`ReflectionExtensions.cs`, `ServiceCollectionExtensions.cs`) — same treatment as steps 2–3.
12. Revert/trim the now-redundant `.editorconfig` `[**/obj/**.g.cs]` section ONLY if the module-level suppressions also satisfy the build-time analyzer (verify; keep if still needed for WPF `.g.cs` sites which are build-analyzer-only).

Steps 1–4+11 must land together (interim RUC additions would create new analyzer warnings in the three UI projects between steps).

Residual (accepted, documented): exactly 7× IL2104 from Syncfusion ×5, WinRT.Runtime, Microsoft.Windows.SDK.NET — third-party, not fixable here.

## Part B — MSTest cleanup (~37) — exact edits in the detailed plan

- **MSTEST0017** (14 sites): swap to `(expected, actual)` — IntPointTest:20, DistanceTest:481-482, Cholesky/JaggedCholesky Solve tests, Matrix.Conversion:203/216, Matrix.Multiply:524, Matrix.Selection:34.
- **MSTEST0065** (arrays, in-place-contract intent): `Assert.AreSame(B, actual)` / `AreNotSame` — preserves the reference-identity contract (CollectionAssert would silently weaken the tests); resolves overlapping 0017s on those lines.
- **MSTEST0032**: ArgumentTests:281 remove redundant assert; ArgumentTests:313 replace with `Assert.AreEqual(TestEnum.Value1, result)` (only assert in test); UtmGridTests:73 delete `IsTrue(true)`; 3 placeholder tests → empty method + comment (keep ≥1 discoverable test per project); DomainEventBaseTests:50/58 + 4 Eigen/SVD `AreSame(x.Prop, x.Prop)` sites → capture two locals then assert (preserves stability/caching contract).
- **MSTEST0044**: `[DataTestMethod()]` → `[TestMethod]` (UnitConversionServiceTests:49,91).
- **MSTEST0030 MailServiceTests**: add `[TestClass]` + class-level `[Ignore("Integration test — requires live Microsoft 365 credentials; sends real email via Graph API")]`. Never let it run in CI (verified: it calls real Graph with hardcoded recipient).
- **MSTEST0014 PointTest:45**: add missing `[TestMethod]` to `EqualityOperatorTest` + fix its swapped AreEqual (line 50).
- **CS8602**: `_cleanupMethod.ToString()!.Contains(...)`.

## Part C — Samples (3)

- **BL0007 ×2** (BaseEmptyDialog.razor, BaseModelDialog.razor): `Content` → auto-property; move event subscribe/unsubscribe to `OnParametersSet()` with `_previousViewModel` tracking; remove the duplicate subscription in `OnInitializedAsync` (fixes existing double-subscription bug); Dispose paths unchanged.
- **CA1416** (ApplicationMonitor.cs:68): capture `TotalProcessorTime` outside the gauge lambda (CA1416 guard analysis doesn't flow into lambdas) under `!OperatingSystem.IsIOS() && !OperatingSystem.IsTvOS()` guard, mirroring the existing memory-gauge capture pattern.

## Verification

1. `dotnet build I-Synergy.Framework.slnx -c Release --no-incremental` → expect only the 7 IL2104 third-party warnings (plus zero MSTEST/BL/CA/CS in the fixed set).
2. `dotnet build samples/Sample.WinUI/Sample.WinUI.csproj -c Release -r win-x64 --self-contained true` → ILLink surface: expect exactly 7× IL2104, zero ISynergy-originated IL warnings.
3. `dotnet test` for every touched test project (Core, Mathematics, Geography, EventSourcing, Physics, AspNetCore.Authentication, MessageBus, Storage, UI.Maui, Mvvm) — all green; Mail.Microsoft365 shows 4 skipped (not failed).
4. If feasible, launch published Sample.WinUI and round-trip settings (proves the JSON source-gen fix; old settings.json must still load).
5. Watch for the flagged uncertainty: method-level suppressions covering lambda-originated warnings (WPF ConfigureServices, GetRelatedViewType) — if a warning survives, apply the extract-to-attributed-method fallback specified in the detailed plan.

## Risk notes

- New RUC on `RegisterAssemblies`/`To*Types`/plural `Register*s` surfaces IL2026 for NuGet consumers calling them directly under trim analysis — intended honest contract (consistent with f297f51's SettingsStorageExtensions decision); `ConfigureServices` callers see no change. Release-notes mention required.
- `Bind` → manual binding: any config key added to those option types later must be bound explicitly (add a comment at each site).
- MailServiceTests now shows 4 skipped tests in CI output (previously silently undiscovered).
- Catalyst loses the CPU gauge (TotalProcessorTime unsupported there per analyzer).

## Execution

Delegate to `developer` subagent(s); progress file `.ai/progress/fix-remaining-warnings.md`. Suggested order/commits: (1) Part A steps 1-4+11 atomically, then 5-10+12; (2) Part B; (3) Part C. Full verification after each part.

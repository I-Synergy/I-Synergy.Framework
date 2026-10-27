# Fix trim-analysis warnings in UI.WinUI (+ WPF/MAUI parallels)
**Status:** DONE
Started: 2026-07-15
Completed: 2026-07-15
Plan: .ai/plans/steady-cooking-storm.md

## Steps
- [x] Step 1: Application.cs — replace GetValue<string?> with configuration indexer (IL2026)
- [x] Step 2: WindowsAppSdkHostBuilder.cs — DAM(PublicConstructors) on TApp + HostBuilder Write<T> pattern (IL2091 x2, IL2026)
- [x] Step 3: DialogService WinUI — DAM(PublicConstructors) on Type parameter (IL2067)
- [x] Step 4: DialogService WPF + MAUI (incl. ShowDialogAsyncInternal) — same annotation
- [x] Step 5: EnumCollection.cs (WinUI) + EnumCollectionExtension.cs (MAUI, property + ctor) — DAM(PublicFields) (IL2072)
- [x] Step 6: EnumConverters WinUI (3 methods) + MAUI (1 method) — UnconditionalSuppressMessage IL2057
- [x] Step 7: SettingsStorageExtensions.cs — RUC + RDC on 4 generic methods (IL2026 x4)
- [x] Step 8: Verification — Release builds of UI.WinUI, UI.WPF, UI.Maui + Sample.WinUI; zero IL2xxx warnings

## Notes
- EnumKeyValueCollection.cs and EnumToArrayConverter need NO change (Enum.GetValues(Type) has no DAM requirement).
- Step 5 risk: if XamlTypeInfo.g.cs setter triggers new IL2072, fall back to UnconditionalSuppressMessage("Trimming","IL2072",...) on ProvideValue.
- FALLBACK APPLIED (Step 5, WinUI only): annotating EnumCollection.EnumType produced IL2067 in generated XamlTypeInfo.g.cs (predicted risk, manifested as IL2067 instead of IL2072). Reverted property annotation; suppressed IL2072 on ProvideValue instead. MAUI EnumCollectionExtension annotation kept (no generated-code issue there).
- EXTRA (pre-existing warnings missed by plan inventory, confirmed via baseline stash build):
  - WinUI WindowsAppBuilderExtensions.cs: IL2091 x4 fixed with DAM(PublicConstructors) on TContext/TCommonServices/TExceptionHandlerService/TSettingsService; IL2026 x2 (ConfigurationBinder.Bind) suppressed with pragma (file precedent).
  - WPF Application.cs:149: same GetValue -> indexer fix as Step 1.
  - WPF WPFAppBuilderExtensions.cs: IL2091 x3 fixed with DAM; IL2026 x2 (Bind) suppressed with pragma.
  - Root .editorconfig: IL2026/IL2067/IL2075 severity=none for [**/obj/**.g.cs] (XAML compiler generated code: WinUI XamlTypeInfo activators for EventTriggerBehavior, WPF InitializeComponent/GeneratedInternalTypeHelper — not annotatable in source).
- Sample.WinUI: plain Release build fails with pre-existing NETSDK1102 (PublishTrimmed=true in Release without RID/self-contained — unrelated to this change). Built successfully with -r win-x64 --self-contained true: no new warnings from any changed API (no IL2091 on WindowsAppSdkHostBuilder<App>); 46 pre-existing app-level ILLink trim-analysis warnings across framework assemblies (ReflectionExtensions.Register*, ViewModelExtensions, EnumExtensions, OpenTelemetry Bind, Syncfusion/WinRT IL2104, sample's own JsonSerializer IL2026) — all out of scope, pre-date this task.

# Plan: Eliminate the remaining 85 solution-build warnings

Verified against source on 2026-07-15. Constraint honored throughout: ILLink (publish-time trimmer for Sample.WinUI, `PublishTrimmed=true` in Release) only honors `[UnconditionalSuppressMessage]` / `[RequiresUnreferencedCode]` (incl. module-level with `Scope`/`Target`); `#pragma` and `.editorconfig` only silence the Roslyn analyzer.

Key project facts found during verification:
- `EnableTrimAnalyzer=true` + `IsTrimmable=true` only in **UI.WinUI, UI.WPF, UI.Maui** csproj. Core, Mvvm, UI (shared), OpenTelemetry, AspNetCore.Blazor have neither — adding `[RequiresUnreferencedCode]` there produces no new build-time warnings inside those projects, but the three analyzer-enabled projects WILL flag their calls to newly-RUC methods, so the boundary suppressions in Step 4 must land together with Steps 1-3.
- Attribute-style precedent already in repo: `src\ISynergy.Framework.UI.WinUI\Converters\EnumConverters.cs` uses `[UnconditionalSuppressMessage("Trimming", "IL2057:TypeGetType", Justification = "...")]`.
- No `AssemblyInfo.cs`/`GlobalSuppressions.cs` exists yet in UI.WinUI or Sample.WinUI — new files created in Step 9.

---

## PART A — Bucket 1: framework trim chain (~36 ILLink warnings)

### Step 1 — `src\ISynergy.Framework.Core\Extensions\ReflectionExtensions.cs`

**1a. RUC on the assembly walkers** (fixes IL2026 at 365→381 and 388; same treatment for the uncalled sibling):

Above `GetAllReferencedAssemblies` (line 318) and `GetAllReferencedAssemblyNames` (line 363):

```csharp
[RequiresUnreferencedCode("Recursively loads referenced assemblies with Assembly.Load(AssemblyName); the trimmer cannot statically discover assemblies or types loaded this way.")]
```

**1b. Relax `IsFreeApplication` (281), `IsSingleton` (293), `IsScoped` (305): REMOVE `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]` from the `this Type _self` parameter on all three.**

Determination requested by the direction ("IsSingleton/IsScoped need?"): they only call `Attribute.GetCustomAttribute(Type, Type)`, which has NO DAM/RUC requirement in the BCL (custom attributes are preserved with the member they decorate). `DAM(All)` is over-annotation and is the root cause of IL2067 at UI `ReflectionExtensions.cs:197/203`. Removing it:
- is a pure relaxation — no consumer gains warnings; binary and source compatible;
- lets the singular `RegisterView/RegisterViewModel/RegisterWindow` params use `Interfaces | PublicConstructors` instead of a viral `All` (which would over-preserve every view's private members).

Resulting signatures: `public static bool IsSingleton(this Type _self)` etc.

### Step 2 — `src\ISynergy.Framework.UI\Extensions\ReflectionExtensions.cs`

**2a. RUC on all six discovery methods** (fixes IL2026/IL2065 at 26/27, 57/58, 91/92 incl. the IL2070 at 27 in the lambda — since .NET 7, ILLink attributes state-machine/lambda warnings to the user method, so RUC on the iterator covers the generated `MoveNext`):

Above `ToViewModelTypes` (23), `GetViewModelTypes` (39), `ToViewTypes` (54), `GetViewTypes` (71), `ToWindowTypes` (88), `GetWindowTypes` (104):

```csharp
[RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces(); types not statically referenced by the application may already have been removed by the trimmer.")]
```

**2b. RUC on the plural registrars** (`IEnumerable<Type>` elements cannot carry DAM) — `RegisterWindows` (119), `RegisterViews` (144), `RegisterViewModels` (169):

```csharp
[RequiresUnreferencedCode("Registers types produced by runtime assembly scanning. For trim-safe registration, call RegisterWindow/RegisterView/RegisterViewModel with statically-known (typeof) types instead.")]
```

**2c. DAM on the singular registrars** — exact member-type set, derived from the bodies:

| Requirement | Source |
|---|---|
| `Interfaces` | `.GetInterfaces()` at 132/157/182 |
| `PublicConstructors` | private `Register` param (194); DI `TryAddScoped(Type service, Type impl)` / `TryAddScoped(Type)` implementation-type params at 187/189 |
| ~~All~~ | no longer needed after Step 1b |

```csharp
public static void RegisterWindow(this IServiceCollection services,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] Type window)
```
(identically for `RegisterView(... Type view)` and `RegisterViewModel(... Type viewmodel)`).

**2d. Fix the abstraction-lookup lambdas** (IL2070 at 134/159/184: `q.GetInterfaces()` on an unannotated lambda parameter — not annotatable). Replace the three inline `Array.Find(...)` expressions with one shared private helper carrying a targeted suppression (a named method avoids any reliance on suppressions flowing into compiler-generated lambda classes):

```csharp
[UnconditionalSuppressMessage("Trimming", "IL2070:UnrecognizedReflectionPattern",
    Justification = "Interface types returned by GetInterfaces() are preserved together with the implementing type, and the marker interfaces (IView/IWindow/IViewModel) are statically referenced here, so their interface hierarchies survive trimming.")]
private static Type? FindAbstraction(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type,
    Type marker,
    string markerName) =>
    Array.Find(type.GetInterfaces(), q => q.GetInterfaces().Contains(marker) && !q.Name.StartsWith(markerName));
```

Call sites:
```csharp
var abstraction = FindAbstraction(window, typeof(IWindow), nameof(IWindow));       // RegisterWindow, replaces 132-134
var abstraction = FindAbstraction(view, typeof(IView), nameof(IView));             // RegisterView, replaces 157-159
var abstraction = FindAbstraction(viewmodel, typeof(IViewModel), nameof(IViewModel)); // RegisterViewModel, replaces 182-184
```

**2e. Private `Register` (192-195): keep `DAM(PublicConstructors)` on `type`; REMOVE it from `abstraction`.** `abstraction` is only ever used as a DI *service* type (`TryAddSingleton(abstraction, type)` etc.), whose parameter is unannotated in MS.Ext.DI; the current annotation is what produces IL2067/IL2072 at 136/161 (GetInterfaces() results can never satisfy it).

After 1b+2c+2e, IL2067/IL2072 at 136/161/187/189/197/203 are resolved by construction — no suppressions needed there.

### Step 3 — `src\ISynergy.Framework.UI\Extensions\ServiceCollectionExtensions.cs`

RUC on both overloads (covers the Assembly.Load IL2026s at 27/32 and the calls into RUC To*Types/Register*):

```csharp
[RequiresUnreferencedCode("Discovers and loads referenced assemblies at runtime and scans them for IView/IWindow/IViewModel implementors; not trim-compatible. Types are only found if the application still references them.")]
public static void RegisterAssemblies(this IServiceCollection services, Assembly assembly, Func<AssemblyName, bool> assemblyFilter)

[RequiresUnreferencedCode("Scans the supplied assemblies for IView/IWindow/IViewModel implementors; not trim-compatible.")]
public static void RegisterAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
```

### Step 4 — Boundary suppression + Bind replacement in the three app builders

**4a. `src\ISynergy.Framework.UI.WinUI\Extensions\WindowsAppBuilderExtensions.cs`**

On `ConfigureServices<...>` (line 44):

```csharp
[UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
    Justification = "View, ViewModel and Window types are rooted by the application XAML compilation (XamlTypeInfo) and direct references; RegisterAssemblies only rediscovers types the application already preserves. typeof(TResource) is statically known, so its ResourceManager is preserved at compile time.")]
```

Replace lines 64-67 (`#pragma warning disable IL2026` around the two `Configure(...Bind)` calls — pragma never reached ILLink) with reflection-free manual binding. Decision: **bind by hand** — ApplicationFeatures has 1 property, ClientApplicationOptions has 3. Use the raw `IConfigurationSection` string indexer (fully trim-safe); do NOT use `ConfigurationBinder.GetValue<T>` (it is RUC-annotated):

```csharp
var featuresSection = appBuilder.Configuration.GetSection(nameof(ApplicationFeatures));
appBuilder.Services.Configure<ApplicationFeatures>(options =>
    options.CheckForUpdatesInMicrosoftStore =
        bool.TryParse(featuresSection[nameof(ApplicationFeatures.CheckForUpdatesInMicrosoftStore)], out var check) && check);

var clientSection = appBuilder.Configuration.GetSection(nameof(ClientApplicationOptions));
appBuilder.Services.Configure<ClientApplicationOptions>(options =>
{
    options.ClientId = clientSection[nameof(ClientApplicationOptions.ClientId)];
    options.ClientSecret = clientSection[nameof(ClientApplicationOptions.ClientSecret)];
    options.Endpoint = clientSection[nameof(ClientApplicationOptions.Endpoint)];
});
```

(Config key lookup via the indexer is case-insensitive, same as Bind — behavior preserved.)

Also delete the now-dead `#pragma IL2026` at 76-79 around `AddResourceManager(typeof(TResource))` — the method-level attribute above (same warning id) covers it for both analyzer and ILLink.

**4b. `src\ISynergy.Framework.UI.WPF\Extensions\WPFAppBuilderExtensions.cs`** (class `WpfAppBuilderExtensions`)

- Same `[UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", ...)]` on `ConfigureServices<...>` (line 43). Note: `RegisterAssemblies` is called inside the `wpfAppBuilder.ConfigureServices((context, services) => {...})` lambda — since .NET 7 both ILLink and the analyzer attribute compiler-generated (lambda) warnings to the containing user method, so the attribute suppresses it. **Fallback if a warning survives the WPF Release build or the trimmed publish:** extract the lambda body into a private static method carrying the attribute.
- Replace `#pragma IL2026` at 64-66 with the 3-property ClientApplicationOptions manual bind (same snippet as 4a, using `context.Configuration`).
- Delete the `#pragma IL2026` at 68-71 (TResource) — covered by the method attribute.
- `AddUpdatesIntegration` (122-129): replace the pragma-wrapped `UpdateOptions` Bind with manual binding (verified 4 properties in `src\ISynergy.Framework.UI.WPF\Options\UpdateOptions.cs`):

```csharp
public static IServiceCollection AddUpdatesIntegration(this IServiceCollection services, IConfiguration configuration)
{
    var updateSection = configuration.GetSection(nameof(UpdateOptions));
    services.Configure<UpdateOptions>(options =>
    {
        options.Filename = updateSection[nameof(UpdateOptions.Filename)];
        options.VersionEndpoint = updateSection[nameof(UpdateOptions.VersionEndpoint)];
        options.DownloadEndpoint = updateSection[nameof(UpdateOptions.DownloadEndpoint)];
        if (int.TryParse(updateSection[nameof(UpdateOptions.ApplicationId)], out var applicationId))
            options.ApplicationId = applicationId;
    });
    services.TryAddSingleton<IUpdateService, UpdateService>();
    return services;
}
```

**4c. `src\ISynergy.Framework.UI.Maui\Extensions\MauiAppBuilderExtensions.cs` — bring to parity**

- Annotate the generic parameters at line 66 (currently completely unannotated; `TryAddSingleton<TContext>()` etc. require `PublicConstructors` — matches WinUI 46-49 / WPF 44-47):

```csharp
public static MauiAppBuilder ConfigureServices<
    TApplication,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TContext,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCommonServices,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TExceptionHandlerService,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSettingsService,
    TResource>( // NOSONAR
```

- Replace the two UNGUARDED Bind calls (79-80) with the manual-binding snippet from 4a.
- Add the same `[UnconditionalSuppressMessage("Trimming", "IL2026:...")]` on `ConfigureServices` (covers `RegisterAssemblies` at 129 and the pragma-wrapped TResource block at 89-92, which can then be deleted).
- Add `using System.Diagnostics.CodeAnalysis;` if missing.

### Step 5 — `src\ISynergy.Framework.Mvvm\Extensions\ViewModelExtensions.cs` — `GetRelatedViewType` (188)

IL2026 x2 at 199 (`Assembly.GetType`) and 205 (`GetExportedTypes`) — both id IL2026, so one attribute. Do NOT use RUC: it would cascade into NavigationService/BladeService/NavigationExtensions across WinUI/WPF/MAUI.

```csharp
[UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
    Justification = "Registry-first resolution: RegisterViewTypes provides the AOT/trim-safe path and is checked before any reflection. The reflection fallback can only locate view types the application still references; a type removed by the trimmer could never be a valid navigation target.")]
public static Type? GetRelatedViewType(this string name)
```

(Warnings originate in lambdas inside the method — same compiler-generated-attribution note as 4b; fallback is extracting the two LINQ queries into an attributed private helper.)

### Step 6 — `src\ISynergy.Framework.Core\Extensions\EnumExtensions.cs` (IL2075 at 57)

On `GetLocalizedDescription<T>` (line 48) — keep the existing DAM union on `T` and suppress the deliberate `GetType()` pattern (the comment at line 51-52 explains why `typeof(T)` cannot be used):

```csharp
[UnconditionalSuppressMessage("Trimming", "IL2075:UnrecognizedReflectionPattern",
    Justification = "GetType() is deliberate: typeof(T) resolves to System.Enum when called on an Enum-typed variable. The trimmer preserves an enum type together with its fields and their attributes whenever the enum is preserved, so GetMember on the value runtime type is safe.")]
```

Apply the identical attribute to the sibling `GetDescription<T>` (same `concreteType.GetMember` pattern at line 28) — not in the current warning list only because it is not reachable in the trimmed app today; attribute it now so it never regresses.

### Step 7 — `src\ISynergy.Framework.Core\Extensions\ObservableClassExtensions.cs` (IL2075 at 10)

Do NOT make it generic — callers (`ObservableClass.cs:67,72`, `ObservableValidatedClass.cs:75,80`) invoke it as `this.GetProperties(...)`, so `typeof(T)` would bind to the compile-time base type and change behavior. Suppress on the method and add `using System.Diagnostics.CodeAnalysis;`:

```csharp
[UnconditionalSuppressMessage("Trimming", "IL2075:UnrecognizedReflectionPattern",
    Justification = "Intentionally reflects over the runtime type of the ObservableClass-derived instance; a generic parameter would bind to the compile-time base type and change behavior. Public instance properties of these model types are rooted by the consuming application bindings/serialization; a property removed by the trimmer has no observable dirty/validation state to enumerate.")]
public static IEnumerable<PropertyInfo> GetProperties(this object observableClass, bool includeMemo = false, bool includeAudit = false, bool includeReadOnly = false)
```

### Step 8 — `src\ISynergy.Framework.OpenTelemetry\Extensions\OtlpTelemetryExtensions.cs` (IL2026 at 30)

Replace `telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection).Bind(options);` with manual binding, following the file's own `Map` precedent (lines 61-71). `HttpClientFactory` cannot come from config; bind the scalar keys plus the nested batch options to preserve Bind fidelity:

```csharp
var section = telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection);
var options = new OtlpExporterOptions();

if (Uri.TryCreate(section[nameof(OtlpExporterOptions.Endpoint)], UriKind.Absolute, out var endpoint))
    options.Endpoint = endpoint;
if (section[nameof(OtlpExporterOptions.Headers)] is { Length: > 0 } headers)
    options.Headers = headers;
if (int.TryParse(section[nameof(OtlpExporterOptions.TimeoutMilliseconds)], out var timeoutMilliseconds))
    options.TimeoutMilliseconds = timeoutMilliseconds;
if (Enum.TryParse<OtlpExportProtocol>(section[nameof(OtlpExporterOptions.Protocol)], ignoreCase: true, out var protocol))
    options.Protocol = protocol;
if (Enum.TryParse<ExportProcessorType>(section[nameof(OtlpExporterOptions.ExportProcessorType)], ignoreCase: true, out var exportProcessorType))
    options.ExportProcessorType = exportProcessorType;

var batchSection = section.GetSection(nameof(OtlpExporterOptions.BatchExportProcessorOptions));
if (batchSection.Exists())
{
    var batch = options.BatchExportProcessorOptions;
    if (int.TryParse(batchSection[nameof(batch.MaxQueueSize)], out var maxQueueSize)) batch.MaxQueueSize = maxQueueSize;
    if (int.TryParse(batchSection[nameof(batch.ScheduledDelayMilliseconds)], out var scheduledDelay)) batch.ScheduledDelayMilliseconds = scheduledDelay;
    if (int.TryParse(batchSection[nameof(batch.ExporterTimeoutMilliseconds)], out var exporterTimeout)) batch.ExporterTimeoutMilliseconds = exporterTimeout;
    if (int.TryParse(batchSection[nameof(batch.MaxExportBatchSize)], out var maxExportBatchSize)) batch.MaxExportBatchSize = maxExportBatchSize;
}
```

Add `using OpenTelemetry;` (namespace of `ExportProcessorType` — verify during implementation). Document in XML remarks that `Headers` keeps the OTLP `key1=value1,key2=value2` string format. Behavior note: unset config keys leave the same library defaults as Bind did, so the existing endpoint-not-configured skip logic (line 34-38) is unchanged.

### Step 9 — Module-level suppressions for generated XamlTypeInfo members (IL2026 x4)

ILLink matches Scope="member" Target strings using the XML documentation-comment ID format (M: prefix; no parentheses for parameterless methods). Verified generated member names from obj\Release\net10.0-windows10.0.26100.0\win-x64\XamlTypeInfo.g.cs:

- UI.WinUI: namespace `ISynergy.Framework.UI.ISynergy_Framework_UI_WinUI_XamlTypeInfo`, class `XamlTypeInfoProvider`, members `Activate_49_EventTriggerBehavior` (line 372) and `StaticInitializer_49_EventTriggerBehavior` (line 413).
- Sample.WinUI: namespace `Sample.Sample_WinUI_XamlTypeInfo`, class `XamlTypeInfoProvider`, members `Activate_120_EventTriggerBehavior` (736) and `StaticInitializer_120_EventTriggerBehavior` (885).

Suppressions must be compiled into the SAME assembly as the generated code, so each project gets its own file.

**New file** `src\ISynergy.Framework.UI.WinUI\Properties\GlobalSuppressions.Trimming.cs`:

```csharp
// Trim-warning suppressions for XAML-compiler-generated code (XamlTypeInfo.g.cs).
// ILLink does not honor #pragma or .editorconfig; a module-level UnconditionalSuppressMessage
// with Scope/Target is the only mechanism that reaches the publish-time trimmer.
//
// MAINTENANCE NOTE: the numeric member index (49) is assigned by the XAML compiler and can
// change when XAML content changes. A stale Target is harmless (the warning simply reappears);
// update the index if these IL2026 warnings resurface in a trimmed publish.
using System.Diagnostics.CodeAnalysis;

[module: UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
    Scope = "member",
    Target = "M:ISynergy.Framework.UI.ISynergy_Framework_UI_WinUI_XamlTypeInfo.XamlTypeInfoProvider.Activate_49_EventTriggerBehavior",
    Justification = "Microsoft.Xaml.Interactivity.EventTriggerBehavior is statically referenced by the generated XAML type table of this assembly and is therefore preserved.")]
[module: UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
    Scope = "member",
    Target = "M:ISynergy.Framework.UI.ISynergy_Framework_UI_WinUI_XamlTypeInfo.XamlTypeInfoProvider.StaticInitializer_49_EventTriggerBehavior",
    Justification = "RunClassConstructor on a statically referenced type preserved by the generated XAML type table.")]
```

**New file** `samples\Sample.WinUI\Properties\GlobalSuppressions.Trimming.cs`: identical shape with
Target = "M:Sample.Sample_WinUI_XamlTypeInfo.XamlTypeInfoProvider.Activate_120_EventTriggerBehavior" and ...StaticInitializer_120_EventTriggerBehavior.

Rejected alternatives: TrimmerSingleWarn=true collapses per-assembly and hides every future real warning; NoWarn IL2026 at publish is global and equally blunt. Targeted module suppression is precise, and index drift fails safe (warning returns rather than over-suppressing). If ILLink reports the origin member differently in the publish log, copy the member name exactly as printed (the log prints doc-ID-style names).

### Step 10 — Sample.WinUI JSON source generation (ALSO A LIVE RUNTIME-BUG FIX — call out in the PR)

PublishTrimmed=true sets `System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault=false` (confirmed in Sample.runtimeconfig.json), so `SettingsService.LoadLocalSettings`/`SaveLocalSettings` (lines 73/93) THROW NotSupportedException at startup in a published Release build today — and `LoadLocalSettings` only catches JsonException/FileNotFoundException, so the exception escapes the constructor and kills DI resolution. This step is a functional repair, not just warning hygiene.

Feasibility (verified): `LocalSettings` (samples\Sample.Shared\Models\LocalSettings.cs) declares only simple props (string/bool/int/List<string>/Themes enum/read-only IsLightThemeEnabled); every public member of the ObservableClass/ObservableValidatedClass bases (IsDisposed, Properties, IsDirty, AutomaticValidationTrigger, Validator, IsValid, Errors, Error, HasErrors) already carries [JsonIgnore] (System.Text.Json.Serialization — verified in both base files). Therefore plain [JsonSerializable(typeof(LocalSettings))] generates cleanly: the non-serializable Validator delegate and ConcurrentDictionary<string, IProperty> are excluded, and the emitted JSON is byte-compatible with the current reflection output (PascalCase, numeric enum, IsLightThemeEnabled written-and-ignored-on-read, no indentation). No DefaultIgnoreCondition or custom resolver needed. Existing persisted settings.json files round-trip unchanged.

**New file** `samples\Sample.WinUI\Serialization\LocalSettingsJsonContext.cs`:

```csharp
using Sample.Models;
using System.Text.Json.Serialization;

namespace Sample.Serialization;

/// <summary>
/// Source-generated serializer context for settings persistence.
/// Required because PublishTrimmed disables reflection-based System.Text.Json.
/// </summary>
[JsonSerializable(typeof(LocalSettings))]
internal sealed partial class LocalSettingsJsonContext : JsonSerializerContext;
```

**Edit** `samples\Sample.WinUI\Services\SettingsService.cs` (add `using Sample.Serialization;`):

```csharp
// line 73
_localSettings = JsonSerializer.Deserialize(json, LocalSettingsJsonContext.Default.LocalSettings);

// line 93 — the (object?, Type, JsonSerializerContext) overload avoids a CS8604 on the nullable field
string json = JsonSerializer.Serialize(_localSettings, typeof(LocalSettings), LocalSettingsJsonContext.Default);
```

Optional follow-up (out of scope): Sample.WPF/Maui/Blazor SettingsServices share the pattern but are never published trimmed; consider moving the context to Sample.Shared later.

### Step 11 — src\ISynergy.Framework.AspNetCore.Blazor parity (recommended: yes, same change set)

The same warnings will fire for any trimmed/AOT Blazor consumer; the copies were verified structurally identical.

- `Extensions\ReflectionExtensions.cs`: apply Steps 2a-2e verbatim — RUC on ToViewModelTypes (22), GetViewModelTypes (38), ToViewTypes (53), GetViewTypes (70), ToWindowTypes (87), GetWindowTypes (103); RUC on plural RegisterWindows (118) / RegisterViews (143) / RegisterViewModels (168); DAM Interfaces|PublicConstructors on singular (129/154/179); the FindAbstraction helper; drop DAM from the private Register abstraction param (188+).
- `Extensions\ServiceCollectionExtensions.cs`: RUC on both RegisterAssemblies (109, 132). On ConfigureServices<...> (44): add the same UnconditionalSuppressMessage for IL2026, add [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] to TContext/TCommonServices/TExceptionHandlerService/TSettingsService, and replace the two unguarded Bind calls (59-60) with manual binding: ClientApplicationOptions (3 props, snippet from 4a) and AnalyticOptions (verified 2 string props):

```csharp
var analyticSection = configuration.GetSection(nameof(AnalyticOptions));
services.Configure<AnalyticOptions>(options =>
{
    options.GoogleAnalyticsMeasurementId = analyticSection[nameof(AnalyticOptions.GoogleAnalyticsMeasurementId)];
    options.MicrosoftClarityProjectId = analyticSection[nameof(AnalyticOptions.MicrosoftClarityProjectId)];
});
```

### Bucket 1 residual (expected, accepted)

IL2104 x7 — "Assembly X produced trim warnings" for Syncfusion x5, WinRT.Runtime, Microsoft.Windows.SDK.NET. Not fixable from this repo (third-party binaries without trim annotations). Decision: ACCEPT and document (short note in the repo build docs or a comment in Sample.WinUI.csproj next to PublishTrimmed). Do NOT set TrimmerSingleWarn=false (explodes into hundreds of detailed third-party warnings) and do NOT NoWarn IL2104 (would hide any NEW problem assembly added later).

---

## PART B — Bucket 2: MSTest warnings (~37) — exact edits (all verified against source)

`tests\ISynergy.Framework.Core.Tests\Points\IntPointTest.cs`
- 20: `Assert.AreEqual(point.EuclideanNorm(), expectedNorm);` -> `Assert.AreEqual(expectedNorm, point.EuclideanNorm());`

`tests\ISynergy.Framework.Core.Tests\Points\PointTest.cs`
- Insert `[TestMethod]` on its own line above line 41 (`[DataRow(1.1f, 2.2f, 1.1f, 2.2f, true)]`) of `EqualityOperatorTest` (MSTEST0014 — the test currently never runs).
- 50: `Assert.AreEqual(point1 == point2, areEqual);` -> `Assert.AreEqual(areEqual, point1 == point2);`
  Verified the enabled test passes: trivial float == with literal-identical/distinct DataRows.
- Optional consistency (not warned): 63 `Assert.AreEqual(point1 != point2, areNotEqual);` -> `Assert.AreEqual(areNotEqual, point1 != point2);`

`tests\ISynergy.Framework.Mathematics.Tests\Common\DistanceTest.cs`
- 481: -> `Assert.AreEqual(expected, a);`   482: -> `Assert.AreEqual(expected, b);`

`tests\ISynergy.Framework.Mathematics.Tests\Decompositions\CholeskyDecompositionTest.cs` (in-place Solve reference-identity intent; fixes the overlapping MSTEST0017s on the same lines)
- 100: `Assert.AreNotEqual(actual, B);` -> `Assert.AreNotSame(B, actual);`
- 103: `Assert.AreEqual(actual, B);` -> `Assert.AreSame(B, actual);`
- 131: `Assert.AreEqual(actual, B);` -> `Assert.AreSame(B, actual);`

`tests\ISynergy.Framework.Mathematics.Tests\Decompositions\JaggedCholeskyDecompositionTest.cs`
- 100 -> `Assert.AreNotSame(B, actual);`   103 -> `Assert.AreSame(B, actual);`   130 -> `Assert.AreSame(B, actual);`

`tests\ISynergy.Framework.Mathematics.Tests\Matrices\Matrix.Conversion.cs`
- 203: -> `Assert.AreEqual(typeof(int[]), values.GetType());`
- 216: -> `Assert.AreEqual(typeof(int[,,]), matrix.GetType());`

`tests\ISynergy.Framework.Mathematics.Tests\Matrices\Matrix.Multiply.cs`
- 524: `Assert.AreEqual(actual, r);` -> `Assert.AreSame(r, actual);` (Matrix.Cross(u, v, r) returns the supplied buffer). Line 478 is already AreSame — leave untouched.

`tests\ISynergy.Framework.Mathematics.Tests\Matrices\Matrix.Selection.cs`
- 34: `Assert.AreEqual(actual, expected);` -> `Assert.AreEqual(expected, actual);` (both sides remain double after int-to-double widening; semantics unchanged).

`tests\ISynergy.Framework.Mathematics.Tests\Matrices\MatrixTest.cs`
- 2326: `Assert.AreEqual(matrix, actual);` -> `Assert.AreSame(matrix, actual);` (in-place transpose identity).

Self-comparison sites (MSTEST0032/AreSame(x,x)) — capture two locals to preserve the caching/stability contract under test:

`tests\ISynergy.Framework.Mathematics.Tests\Decompositions\EigenvalueDecompositionTest.cs` line 69 — and identically JaggedEigenvalueDecompositionTest.cs:67 (with `target`), JaggedSingularValueDecompositionTest.cs:388 and SingularValueDecompositionTest.cs:410 (with `target1`):

```csharp
// The decomposition caches DiagonalMatrix: repeated access must return the same instance.
var firstAccess = target.DiagonalMatrix;
var secondAccess = target.DiagonalMatrix;
Assert.AreSame(firstAccess, secondAccess);
```

`tests\ISynergy.Framework.EventSourcing.Tests\Events\DomainEventBaseTests.cs`
- 50:
```csharp
var firstRead = @event.EventId;
var secondRead = @event.EventId;
Assert.AreEqual(firstRead, secondRead);
```
- 58: same shape with `@event.OccurredAt`.

`tests\ISynergy.Framework.Core.Tests\Validation\ArgumentTests.cs`
- 281: delete `Assert.IsNotNull(result);` (the IsFalse(result == Guid.Empty) at 280 remains).
- 313: DEVIATION from the given direction — this is the ONLY assert in IsNotNullEnum_WithEnumValue_ReturnsTrue; deleting it leaves an assert-less test. Replace instead with the pass-through contract (verified: Argument.IsNotNullEnum<T> returns its input, Argument.cs:171):
  `Assert.AreEqual(TestEnum.Value1, result);`

`tests\ISynergy.Framework.Geography.Tests\Utm\UtmGridTests.cs`
- 73: delete the `Assert.IsTrue(true);` line (ValidateCorners(g) on line 74 carries the real assertions).

Placeholder tests — `tests\ISynergy.Framework.AspNetCore.Authentication.Tests\InitialTests.cs:17`, `tests\ISynergy.Framework.MessageBus.Tests\UnitTest.cs:17`, `tests\ISynergy.Framework.Storage.Tests\UnitTest.cs:17`: remove the `Assert.IsTrue(true);` line, leaving:

```csharp
[TestMethod]
public void Test()
{
    // Intentionally empty placeholder. Keeps the test project discoverable by the
    // runner (a zero-test run fails under Microsoft.Testing.Platform) without
    // asserting a tautology (MSTEST0032). Replace with real tests.
}
```

Rationale for keeping the method: deleting the class would leave these projects with zero tests, which modern test runners treat as a failed run; an empty method passes and raises no MSTest analyzer diagnostic.

`tests\ISynergy.Framework.Physics.Tests\Services\UnitConversionServiceTests.cs`
- 49 and 91: `[DataTestMethod()]` -> `[TestMethod]` (MSTEST0044; the DataRow attributes stay).

`tests\ISynergy.Framework.Mail.Microsoft365.Tests\Services\MailServiceTests.cs`
- Line 10 (verified: the class has NO [TestClass] today, so its [TestMethod]s never ran):

```csharp
[TestClass]
[Ignore("Integration test — requires live Microsoft 365 credentials and sends real email via Graph API. Run manually with valid Microsoft365MailOptions.")]
public class MailServiceTests
```

Resolves MSTEST0030 while keeping CI from sending mail / failing on the empty credentials set in the constructor. Net effect on CI: tests go from invisible to explicitly skipped.

`tests\ISynergy.Framework.UI.Maui.Tests\Services\NavigationServiceRetentionCycleTests.cs`
- 68: `_cleanupMethod.ToString().Contains(...)` -> `_cleanupMethod.ToString()!.Contains(...)`. Chosen over a pattern rewrite: line 65 already asserts IsNotNull(_cleanupMethod) and MethodInfo.ToString() does not return null in practice; the null-forgiving operator is the minimal, intention-preserving fix.

---

## PART C — Bucket 3: samples (3 warnings)

### C1. BL0007 — samples\Sample.Blazor\Components\Dialogs\Base\BaseEmptyDialog.razor and BaseModelDialog.razor

Verified in both files: the [Parameter] Content setter subscribes/unsubscribes PropertyChanged + Closed, AND OnInitializedAsync subscribes PropertyChanged AGAIN (double-subscription bug: duplicate StateHasChanged per property change; Closed is not duplicated). Identical @code structure apart from line offsets (BaseEmptyDialog: field 30, setter 42-67, OnInitializedAsync 69-82, Dispose 155-166; BaseModelDialog: field 53, setter 65-90, OnInitializedAsync 95-108, Dispose 181-192). Apply to BOTH files:

1. Delete the `private TViewModel _viewModel = default!;` field and the whole setter-property block. Replace with:

```csharp
private TViewModel? _previousViewModel;

[Parameter] public TViewModel Content { get; set; } = default!;
```

2. Add an OnParametersSet override — single subscription point; also handles viewmodel replacement including command re-subscription, which the old code never refreshed:

```csharp
protected override void OnParametersSet()
{
    base.OnParametersSet();

    if (ReferenceEquals(_previousViewModel, Content))
        return;

    if (_previousViewModel is not null)
    {
        _previousViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _previousViewModel.Closed -= OnViewModelClosed;
        UnsubscribeFromViewModelCommands();
    }

    _previousViewModel = Content;

    if (Content is not null)
    {
        Content.PropertyChanged += OnViewModelPropertyChanged;
        Content.Closed += OnViewModelClosed;
        SubscribeToViewModelCommands();
    }
}
```

3. Reduce OnInitializedAsync to initialization only (removes the duplicate `PropertyChanged +=` at BaseEmptyDialog 78 / BaseModelDialog 104 and the SubscribeToViewModelCommands() call, now owned by OnParametersSet):

```csharp
protected override async Task OnInitializedAsync()
{
    await base.OnInitializedAsync();

    if (Content is not null && !Content.IsInitialized)
        await Content.InitializeAsync();
}
```

Blazor lifecycle note: on first composition parameters are assigned, then OnInitializedAsync, then OnParametersSet — so InitializeAsync runs before subscription. Property changes raised DURING InitializeAsync no longer trigger StateHasChanged, which is fine because Blazor re-renders at the lifecycle boundary anyway.

4. Replace remaining `_viewModel` references with `Content`: in SubscribeToViewModelCommands (the null guard, `Content.GetType().GetProperties(...)`, `property.GetValue(Content)`) and in Dispose(bool):

```csharp
protected virtual void Dispose(bool disposing)
{
    if (disposing && Content is not null)
    {
        Content.PropertyChanged -= OnViewModelPropertyChanged;
        Content.Closed -= OnViewModelClosed;
        UnsubscribeFromViewModelCommands();
        Content.Dispose();
    }
}
```

(Content and _previousViewModel are identical by the time Dispose runs; unsubscribing handlers that were never subscribed is a no-op, so ordering is safe. Markup already uses Content — no .razor markup changes needed.)

### C2. CA1416 — samples\Sample.Maui\OpenTelemetry\ApplicationMonitor.cs:68

Process.TotalProcessorTime is [UnsupportedOSPlatform("ios")] / [UnsupportedOSPlatform("tvos")] — unsupported on ALL iOS versions, so the analyzer-recognized guard is the NEGATED OperatingSystem.IsIOS() check (NOT IsIOSVersionAtLeast, which is for version-gated APIs). Two critical details:
- CA1416 guard analysis does NOT flow into lambda bodies, so the property read must happen in the guarded method body, not inside the CreateObservableGauge callback.
- The surrounding method already uses exactly this capture pattern for the memory gauge (lines 62/65) — mirror it. (Re-creating gauges every timer tick is a pre-existing oddity; explicitly out of scope, do not redesign.)

Replace lines 67-68 with:

```csharp
// Monitor CPU usage — Process.TotalProcessorTime is unsupported on iOS/tvOS (CA1416).
if (!OperatingSystem.IsIOS() && !OperatingSystem.IsTvOS())
{
    var cpuMilliseconds = _currentProcess.TotalProcessorTime.TotalMilliseconds;
    _meter.CreateObservableGauge("maui.cpu.usage.percent", () => cpuMilliseconds, "%", "CPU usage");
}
```

Trade-off note: OperatingSystem.IsIOS() also returns true on Mac Catalyst (where the API IS supported via [SupportedOSPlatform("maccatalyst")]), so Catalyst loses this gauge — acceptable for a sample; mention in the comment if desired.

---

## Expected residual warnings after all fixes

- IL2104 x7 (Syncfusion x5, WinRT.Runtime, Microsoft.Windows.SDK.NET) in the Sample.WinUI trimmed publish — third-party, accepted and documented. Everything else: 0.
- Watch item: if the publish log still shows IL2026 for LanguageService.AddResourceManager(typeof(TResource)) (previously only pragma-silenced — pragmas never reached ILLink), it is already covered by the Step 4 method-level UnconditionalSuppressMessage (same warning id).

## Verification

```powershell
# 1. Full solution, clean Release build — expect zero warnings in the target set
dotnet build "D:\Projects\I-Synergy\I-Synergy.Framework\I-Synergy.Framework.sln" -c Release --no-incremental

# 2. Trimmed publish-style build of the WinUI sample (the ILLink surface) — expect exactly 7x IL2104
dotnet publish "D:\Projects\I-Synergy\I-Synergy.Framework\samples\Sample.WinUI\Sample.WinUI.csproj" -c Release -r win-x64 --self-contained
#    (if the packaged-app publish requires it, add -p:Platform=x64; count IL* lines in the log)

# 3. Launch the published Sample.WinUI exe and exercise settings:
#    change theme/language -> restart -> verify settings.json round-trips
#    (proves the NotSupportedException startup crash is fixed and old settings.json files still load).

# 4. Tests for every touched project
dotnet test tests\ISynergy.Framework.Core.Tests -c Release
dotnet test tests\ISynergy.Framework.Mathematics.Tests -c Release
dotnet test tests\ISynergy.Framework.Geography.Tests -c Release
dotnet test tests\ISynergy.Framework.Physics.Tests -c Release
dotnet test tests\ISynergy.Framework.EventSourcing.Tests -c Release
dotnet test tests\ISynergy.Framework.Mail.Microsoft365.Tests -c Release   # expect: skipped, not failed
dotnet test tests\ISynergy.Framework.UI.Maui.Tests -c Release
dotnet test tests\ISynergy.Framework.AspNetCore.Authentication.Tests -c Release
dotnet test tests\ISynergy.Framework.MessageBus.Tests -c Release
dotnet test tests\ISynergy.Framework.Storage.Tests -c Release
```

Also build samples: Sample.Maui (confirms CA1416 gone) and Sample.Blazor (confirms BL0007 gone).

## Risk notes for NuGet consumers

1. New [RequiresUnreferencedCode] on public APIs (RegisterAssemblies x2 in UI and in Blazor, To*Types/Get*Types x6 each, plural RegisterWindows/Views/ViewModels, Core GetAllReferencedAssemblyNames/GetAllReferencedAssemblies): consumers who call these directly AND have trim analysis enabled get new IL2026 warnings — intentional honesty; warnings, not errors, unless they opted into WarningsAsErrors. Consumers using ConfigureServices see no change.
2. New DAM on singular RegisterWindow/RegisterView/RegisterViewModel Type params: typeof(X) call sites (the normal pattern) are unaffected; passing an unannotated Type variable now warns IL2067 in analyzer-enabled consumer projects.
3. Removed DAM(All) from IsSingleton/IsScoped/IsFreeApplication: pure relaxation — no new warnings anywhere; consumers' own suppressions around these become redundant but harmless. DAM is metadata-only; not a binary break.
4. MAUI/Blazor ConfigureServices generic params gaining DAM(PublicConstructors): standard closed classes unaffected; a consumer forwarding through their own unannotated generic method would see IL2091 — this is parity with the existing WinUI/WPF signatures, so net-new only for MAUI/Blazor users.
5. ConfigurationBinder.Bind -> manual binding (ApplicationFeatures, ClientApplicationOptions, UpdateOptions, AnalyticOptions, OTLP section): binding of the enumerated keys is behavior-identical (case-insensitive keys, same defaults when absent). These are framework-owned closed option types, so no consumer-added properties can exist on them. The OTLP change additionally documents the supported keys (Endpoint, Headers, TimeoutMilliseconds, Protocol, ExportProcessorType, BatchExportProcessorOptions.{MaxQueueSize, ScheduledDelayMilliseconds, ExporterTimeoutMilliseconds, MaxExportBatchSize}).
6. XamlTypeInfo suppression fragility: generated member indices (49/120) shift when XAML changes; a stale Target fails safe (warning reappears, nothing is over-suppressed). Documented in the suppression file headers.

## Disagreements / refinements vs. the given direction

1. IsSingleton/IsScoped DAM determination: they currently demand DAM(All). Rather than propagating All up through the singular registrars (viral, over-preserving), relax the three Core methods to NO annotation — Attribute.GetCustomAttribute is trim-safe. This answers the "IsSingleton/IsScoped need?" question: after the relaxation they need nothing, and the singular registrar DAM set is exactly Interfaces | PublicConstructors.
2. ArgumentTests.cs:313: the direction said "remove redundant IsNotNull (other asserts remain)" — for this test NO other assert remains; replace with Assert.AreEqual(TestEnum.Value1, result) instead of deleting.
3. Placeholder tests: keep an empty, commented test method rather than deleting the file/class — zero-test projects fail modern dotnet test runs (Microsoft.Testing.Platform treats an empty run as an error).
4. Bind sites: chose manual binding over a suppressed helper for ALL option types (largest is 4 simple properties) — it removes the warnings truthfully instead of attesting them away, and matches the existing Map precedent in OtlpTelemetryExtensions. Only caveat: OTLP nested BatchExportProcessorOptions binding is now explicit (included to preserve Bind fidelity).
5. WPF/Mvvm lambda-suppression caveat: UnconditionalSuppressMessage on the containing method is expected to cover warnings originating in lambdas (compiler-generated attribution, .NET 7+), but this is the one mechanism in this plan with residual uncertainty — the verification publish must confirm; the specified fallback (extract lambda body / LINQ queries into an attributed private method) is mechanical.

## Suggested implementation order (commits)

1. Bucket 2 test fixes (independent, low-risk, immediately verifiable via dotnet test).
2. Bucket 3 samples (independent).
3. Bucket 1 Steps 1-8 + 11 as ONE commit (intermediate states raise new IL2026 in the three analyzer-enabled UI projects until the Step 4 boundary suppressions land), then Step 9 (suppression files) + Step 10 (Sample.WinUI JSON), then the full verification pass.

## Critical files for implementation

- D:\Projects\I-Synergy\I-Synergy.Framework\src\ISynergy.Framework.Core\Extensions\ReflectionExtensions.cs
- D:\Projects\I-Synergy\I-Synergy.Framework\src\ISynergy.Framework.UI\Extensions\ReflectionExtensions.cs
- D:\Projects\I-Synergy\I-Synergy.Framework\src\ISynergy.Framework.UI\Extensions\ServiceCollectionExtensions.cs
- D:\Projects\I-Synergy\I-Synergy.Framework\src\ISynergy.Framework.UI.WinUI\Extensions\WindowsAppBuilderExtensions.cs
- D:\Projects\I-Synergy\I-Synergy.Framework\samples\Sample.WinUI\Services\SettingsService.cs

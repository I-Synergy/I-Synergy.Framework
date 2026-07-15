# Fix trim-analysis warnings (IL2026/IL2091/IL2072/IL2057/IL2067) in UI.WinUI (+ WPF/MAUI parallels)

## Context

CI builds of `ISynergy.Framework.UI.WinUI` emit trim-analysis warnings because the project sets `<IsTrimmable>true</IsTrimmable>` + `<EnableTrimAnalyzer>true</EnableTrimAnalyzer>` (csproj lines 21-22) while several APIs use reflection-based patterns without annotation flow. `UI.WPF` and `UI.Maui` have the same analyzer settings and parallel copies of two of the patterns, so those are fixed in the same pass to prevent the identical warnings there.

Goal: zero IL2xxx warnings on build, using honest annotation propagation (`[DynamicallyAccessedMembers]` / `[RequiresUnreferencedCode]`) rather than blanket suppression, matching the existing annotation style in `ISynergy.Framework.Core` (e.g. `EnumExtensions.cs:75`, `TypeActivator.cs:31`).

Verified facts driving the design:
- `ActivatorUtilities.CreateInstance(IServiceProvider, Type, ...)` requires DAM **PublicConstructors** on the type param.
- `AddSingleton<TService>()` requires DAM **PublicConstructors** on `TService`.
- `DiagnosticSource.Write<T>` carries RUC + DAM(PublicProperties) on `T`; dotnet/runtime's `HostBuilder` fixes the identical private helper with DAM + `UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", ...)`.
- `Enum.GetValues(Type)` has **no** DAM requirement → `EnumKeyValueCollection.cs` and `EnumToArrayConverter` need **no change**.
- Core's `EnumExtensions.ToList` is already annotated `[DynamicallyAccessedMembers(PublicFields)]` — callers just need to flow it.
- `ShowDialogAsync<TEntity>(Type, ...)` is NOT on `IDialogService` (no IL2092/interface-mismatch risk); zero live callers pass a `Type`.
- No project sets `IsAotCompatible`, so `[RequiresDynamicCode]` is inert today but correct propagation for future/consumer AOT analysis.

Every touched file needs `using System.Diagnostics.CodeAnalysis;` if not already present.

## Steps

- [ ] **1. Application.cs — IL2026 (trim-safe replacement, no annotation)**
  `src\ISynergy.Framework.UI.WinUI\Application\Application.cs:267`
  Replace `configuration.GetValue<string?>(nameof(Environment), null)` with `configuration[nameof(Environment)]` (behavior-identical for string keys). Remove the `Microsoft.Extensions.Configuration` using only if it becomes unused.

- [ ] **2. WindowsAppSdkHostBuilder.cs — IL2091 x2 + IL2026**
  `src\ISynergy.Framework.UI.WinUI\Hosting\WindowsAppSdkHostBuilder.cs`
  - Line 18, annotate class generic parameter:
    ```csharp
    public sealed class WindowsAppSdkHostBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp> : IHostApplicationBuilder
        where TApp : Application, new()
    ```
  - Lines 131-132, apply the dotnet/runtime HostBuilder pattern:
    ```csharp
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
        Justification = "The values being passed into Write are being consumed by the application already.")]
    private static void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(DiagnosticSource diagnosticSource, string name, T value)
        => diagnosticSource.Write(name, value);
    ```
    Call sites (lines 105, 126) pass concrete types (`this`, `IHost`) — no new IL2091.

- [ ] **3. DialogService — IL2067 (three projects, same annotation)**
  Annotate the `Type` parameter: `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type`
  - `src\ISynergy.Framework.UI.WinUI\Services\DialogService.cs:308` (`ShowDialogAsync<TEntity>(Type, IViewModelDialog<TEntity>)`)
  - `src\ISynergy.Framework.UI.WPF\Services\DialogService.cs:159` (same signature)
  - `src\ISynergy.Framework.UI.Maui\Services\DialogService.cs:491` **and** the private `ShowDialogAsyncInternal<TEntity>` at line 686 (annotation must flow through the delegation).

- [ ] **4. Enum markup extensions — IL2072 (flow Core's existing annotation)**
  - `src\ISynergy.Framework.UI.WinUI\Markup\EnumCollection.cs:22`:
    ```csharp
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public Type? EnumType { get; set; }
    ```
    **Build-risk check:** `EnumCollection` is `[Bindable]`; if WinUI's generated `XamlTypeInfo.g.cs` setter triggers a new IL2072, fall back to `[UnconditionalSuppressMessage("Trimming", "IL2072", ...)]` on `ProvideValue` instead (XML remarks already document the limitation).
  - `src\ISynergy.Framework.UI.Maui\Markup\EnumCollectionExtension.cs` — annotate **both** the property (line 11) and the ctor parameter (line 18), or the ctor assignment produces a new IL2067.
  - `EnumKeyValueCollection.cs`: **no change** (`Enum.GetValues(Type)` has no DAM requirement).

- [ ] **5. EnumConverters — IL2057 (suppress; RUC not viable due to IValueConverter interface → IL2046)**
  Attribute for all sites:
  ```csharp
  [UnconditionalSuppressMessage("Trimming", "IL2057:TypeGetType",
      Justification = "The enum type name is a fully-qualified string set in XAML; the consuming application must preserve the enum type. Documented in XML remarks.")]
  ```
  - `src\ISynergy.Framework.UI.WinUI\Converters\EnumConverters.cs`: `EnumToBooleanConverter.Convert` (line 38), `EnumToBooleanConverter.ConvertBack` (line 73), `EnumToStringConverter.Convert` (line 173).
  - `src\ISynergy.Framework.UI.Maui\Converters\EnumConverters.cs`: `EnumToStringConverter.Convert` (line 118, `Type.GetType` at 121).
  - Changing the string `EnumType` property to `Type` is rejected: breaks consumer XAML (`EnumType="...Themes, ISynergy.Framework.Core"` in `Windows\ThemeWindow.xaml:18`).

- [ ] **6. SettingsStorageExtensions — IL2026 x4 (honest RUC/RDC propagation; public API surface — do last)**
  `src\ISynergy.Framework.UI.WinUI\Extensions\SettingsStorageExtensions.cs` — on all four generic methods (`SaveAsync<T>(StorageFolder,...)` line 32, `ReadAsync<T>(StorageFolder,...)` line 47, `SaveAsync<T>(ApplicationDataContainer,...)` line 70, `ReadAsync<T>(ApplicationDataContainer,...)` line 94):
  ```csharp
  [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved, or use a serializer configured with a JsonSerializerContext.")]
  [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
  ```
  Non-generic helpers (`SaveString`, `SaveFileAsync`, `ReadFileAsync`, `ReadBytesAsync`) need nothing. Suppression rejected (arbitrary `T` genuinely requires reflection serialization); `JsonTypeInfo` overloads impossible for open `T`. Zero callers repo-wide, so no downstream warning fallout inside the repo.

- [ ] **7. Verification**
  - `dotnet build` each of: `src/ISynergy.Framework.UI.WinUI`, `src/ISynergy.Framework.UI.WPF`, `src/ISynergy.Framework.UI.Maui` (Release) — assert zero `IL2` matches in output.
  - Check WinUI output specifically for new IL2072 in `XamlTypeInfo.g.cs` (step 4 fallback trigger).
  - Build `samples\Sample.WinUI` — confirms `WindowsAppSdkHostBuilder<App>` (Program.cs:23) produces no new warnings.
  - Run existing UI test suites if present; only step 1 touches runtime behavior and it is semantically equivalent.

## Risk notes (NuGet consumers)

- New `[RequiresUnreferencedCode]` on `SettingsStorageExtensions` surfaces IL2026 at consumer call sites with trim analysis on — intended honest contract; binary-compatible; mention in release notes.
- DAM additions are binary-compatible; consumers passing `typeof(X)` / concrete types see no change.
- Suppressions assert runtime safety the analyzer can't prove (enum type must be rooted by the app) — consistent with existing XML remarks.

## Execution

Delegate steps 1-6 to the `developer` subagent per CLAUDE.md orchestration rules; progress tracked in `.ai/progress/fix-trim-warnings-winui.md`. Order: 1 → 2 → 3 → 4 → 5 → 6 → 7 (cheapest first, XamlTypeInfo-risk step isolated, public-API RUC last).

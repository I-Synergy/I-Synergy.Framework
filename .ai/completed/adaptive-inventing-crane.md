# Dynamic Language/Culture Support

**Status:** PLANNING

## Context

The MAUI language selection system is hardcoded. A `Languages` enum defines English, Dutch, German, French as fixed values. RadioButtons in `LanguageWindow.xaml` are hand-authored — German and French are even hardcoded as `IsEnabled="False"`. Adding a new language requires: adding an enum member, updating the enum→culture dictionary in `LanguageExtensions`, adding a RadioButton in XAML, and updating the converter. This does not scale.

The goal is to let consumers configure supported cultures at startup via `MauiAppBuilderExtensions`, with English always available as the default and fallback. The existing `.resx` resource files (en, nl, de, fr in Core, Mvvm, UI, Blazor) continue to work transparently — they resolve via `CultureInfo.CurrentUICulture`, which `LanguageService.GetString()` already uses.

### Culture specificity: neutral and specific both supported

`CultureInfo` natively supports both forms — "nl" (neutral) and "nl-NL" / "nl-BE" (specific). The .NET resource fallback chain handles this automatically:

1. `Resources.nl-BE.resx` — specific culture (if it exists)
2. `Resources.nl.resx` — neutral parent fallback
3. `Resources.resx` — default (English)

This means consumers can register either form, or both. If only "nl-BE" is registered and no `nl-BE` `.resx` files exist, the framework falls back to the existing `Resources.nl.resx`. No new `.resx` files are needed unless region-specific overrides are desired. The saved culture name preserves the full specificity (e.g. `"nl-BE"`) so the preference survives restart.

## Approach

Replace the `Languages` enum with `CultureInfo` throughout the localization chain. The enum is deleted entirely — all usages are replaced by `CultureInfo`. Introduce `ISupportedCulturesService` to hold the configured culture list. Use `BindableLayout` + `RadioButtonGroup.SelectedValue` in MAUI for dynamic RadioButtons.

### What stays unchanged
- `.resx` files — work automatically via `CultureInfo.CurrentUICulture`
- `LanguageService.GetString()` — already uses `CultureInfo.CurrentUICulture`
- `ILanguageService` interface — no changes needed
- Resource manager registration in `ConfigureServices`

### Configuration API (consumer-facing)

Both neutral and specific cultures are accepted via `CultureInfo.GetCultureInfo()`:
```csharp
builder.ConfigureSupportedCultures(cultures =>
{
    cultures.AddCulture("nl");      // neutral — falls back to Resources.nl.resx
    cultures.AddCulture("nl-BE");   // specific — falls back to Resources.nl.resx if no nl-BE .resx exists
    cultures.AddCulture("de");
    cultures.AddCulture("fr");
    // English ("en") is always included automatically
});
```

## Files to Create (3)

| # | File | Purpose |
|---|------|---------|
| 1 | `src/ISynergy.Framework.Core/Abstractions/Services/ISupportedCulturesService.cs` | Service interface holding supported cultures |
| 2 | `src/ISynergy.Framework.Core/Services/SupportedCulturesService.cs` | Implementation — English always first, always present |
| 3 | `src/ISynergy.Framework.Core/Models/LanguageOption.cs` | Display model: `CultureInfo`, `Name` ("nl-NL"), `TwoLetterISOLanguageName` ("nl"), `NativeName` ("Nederlands (Nederland)"), `EnglishName` ("Dutch (Netherlands)") — all derived from `CultureInfo` so no manual localization needed |

## Files to Modify (18)

| # | File | Change |
|---|------|--------|
| 1 | `Core/Abstractions/Base/ILocalSettings.cs` | `Languages Language` → `string Language` |
| 2 | `Core/Enumerations/Languages.cs` | **Delete** — no longer needed |
| 3 | `UI/Extensions/LanguageExtensions.cs` | Replace `SetLocalizationLanguage(Languages)` with `SetLocalizationLanguage(CultureInfo)` and `SetLocalizationLanguage(string)` overloads |
| 4 | `Mvvm/Abstractions/ViewModels/ILanguageViewModel.cs` | `IViewModelDialog<Languages>` → `IViewModelDialog<CultureInfo>`; add `SupportedCultures` property |
| 5 | `UI/ViewModels/LanguageViewModel.cs` | Change base to `ViewModelDialog<CultureInfo>`; inject `ISupportedCulturesService`; build culture list |
| 6 | `UI.Maui/Windows/LanguageWindow.xaml` | Rewrite with `BindableLayout` + `RadioButtonGroup.SelectedValue` |
| 7 | `UI.Maui/ViewModels/Base/BaseShellViewModel.cs` | Handler signature: `SubmitEventArgs<Languages>` → `SubmitEventArgs<CultureInfo>`; save `e.Result.Name` |
| 8 | `UI.Maui/Application/Application.cs` | Line 96 update for string→CultureInfo conversion |
| 9 | `UI.Maui/Extensions/MauiAppBuilderExtensions.cs` | Add `ConfigureSupportedCultures()` method + builder class + TryAddSingleton fallback |
| 10 | `UI.Blazor/Extensions/ServiceCollectionExtensions.cs` | Add `ConfigureSupportedCultures()` on `IServiceCollection` + TryAddSingleton fallback |
| 11 | `UI.Blazor/Application/Application.cs` | Same init update as MAUI Application |
| 12 | `UI.WinUI/Windows/LanguageWindow.xaml` | Rewrite with `ItemsControl` + `LanguageOption.IsSelected` bindings |
| 13 | `UI.WinUI/ViewModels/Base/BaseShellViewModel.cs` | Handler: `SubmitEventArgs<CultureInfo>`; save `e.Result.Name` |
| 14 | `UI.WinUI/Application/Application.cs` | Line 131 update for string→CultureInfo conversion |
| 15 | `UI.WPF/ViewModels/Base/BaseShellViewModel.cs` | Same handler update |
| 16 | `UI.WPF/Application/Application.cs` | Same init update |
| 17 | `samples/Sample.Shared/Models/LocalSettings.cs` | `Languages Language` → `string Language` |
| 18 | `samples/Sample.Maui/MauiProgram.cs` | Add `ConfigureSupportedCultures()` call |

## Files to Delete (4)

| # | File | Reason |
|---|------|--------|
| 1 | `Core/Enumerations/Languages.cs` | Replaced by `CultureInfo` everywhere |
| 2 | `UI.Maui/Converters/RadioButtonToLanguageConverter.cs` | Replaced by `RadioButtonGroup.SelectedValue` |
| 3 | `UI.WinUI/Converters/RadioButtonToLanguageConverter.cs` | Replaced by `LanguageOption.IsSelected` binding |
| 4 | `UI.WPF/Converters/RadioButtonToLanguageConverter.cs` | Replaced by `LanguageOption.IsSelected` binding |

## Implementation Sequence

### Step 1: Core Layer — New service and ILocalSettings change
Create `ISupportedCulturesService`, `SupportedCulturesService`, `LanguageOption`. Change `ILocalSettings.Language` from `Languages` to `string`. Delete `Languages.cs` enum.

`SupportedCulturesService` constructor takes `IEnumerable<CultureInfo>`, ensures English is first/always present, exposes `DefaultCulture` ("en"), `SupportedCultures` (read-only list), `IsCultureSupported(string)`.

### Step 2: UI Extensions — LanguageExtensions + ILanguageViewModel + LanguageViewModel
- Replace `SetLocalizationLanguage(Languages)` with `SetLocalizationLanguage(CultureInfo)` (core) and `SetLocalizationLanguage(string)` (convenience). Remove the enum→culture dictionary entirely.
- Change `ILanguageViewModel : IViewModelDialog<CultureInfo>`, add `IReadOnlyList<LanguageOption> SupportedCultures`.
- Update `LanguageViewModel` to `ViewModelDialog<CultureInfo>`, inject `ISupportedCulturesService`, build `SupportedCultures` list, pre-select from saved culture name.

### Step 3: MAUI — LanguageWindow.xaml + Converter deletion + BaseShellViewModel
Rewrite `LanguageWindow.xaml`:
```xml
<VerticalStackLayout Margin="10" Spacing="10"
                     RadioButtonGroup.GroupName="Language"
                     RadioButtonGroup.SelectedValue="{Binding SelectedItem}">
    <BindableLayout.ItemsSource><Binding Path="SupportedCultures"/></BindableLayout.ItemsSource>
    <BindableLayout.ItemTemplate>
        <DataTemplate x:DataType="models:LanguageOption">
            <RadioButton Content="{Binding NativeName}" Value="{Binding CultureInfo}"/>
        </DataTemplate>
    </BindableLayout.ItemTemplate>
</VerticalStackLayout>
```

RadioButtons show only the native name: **English**, **Nederlands**, **Deutsch**, **français**. The culture code (`"en"`, `"nl"`, etc.) is stored in settings only — exposed on `LanguageOption` but not displayed.
Delete `RadioButtonToLanguageConverter.cs`. Update `BaseShellViewModel.LanguageVM_Submitted` for `SubmitEventArgs<CultureInfo>`.

### Step 4: MAUI — Application.cs + MauiAppBuilderExtensions
- Application.cs line 96: if string is not null/empty, call `SetLocalizationLanguage(string)`.
- Add `ConfigureSupportedCultures()` to `MauiAppBuilderExtensions` with builder pattern. Add English-only fallback registration in `ConfigureServices()`.

### Step 5: Blazor, WinUI, WPF — parallel updates
Same pattern: update Application.cs init, update BaseShellViewModels, update LanguageWindow XAML, delete converters.

### Step 6: Samples
Update `Sample.Shared/LocalSettings.cs` and `Sample.Maui/MauiProgram.cs`.

## Key Design Decisions

1. **`RadioButtonGroup.SelectedValue` for MAUI** — idiomatic MAUI pattern. No converter needed. `CultureInfo.Equals()` handles value comparison.
2. **`LanguageOption.IsSelected` for WinUI/WPF** — these platforms lack `RadioButtonGroup.SelectedValue`. Each option has `IsSelected` with `PropertyChanged`, wired to `SetSelectedItem()`.
3. **`string` in `ILocalSettings`** — we save `CultureInfo.Name` (e.g. `"en"`, `"nl"`, `"nl-NL"`). This is a plain string that serializes to JSON naturally and round-trips via `CultureInfo.GetCultureInfo(savedString)`. Specificity is preserved: if the user picked "nl-NL", it stays "nl-NL".
4. **`TryAddSingleton` fallback** — `ConfigureServices` registers English-only `ISupportedCulturesService` as fallback. If `ConfigureSupportedCultures()` was called first, its registration wins.
5. **Old JSON settings** — migration from `Languages` enum (int) to `string` means old settings files throw `JsonException` on deserialization. Existing `SettingsService` catches this and falls back to defaults (`"en"`). Users lose language preference once on first run after upgrade.

## Verification

1. Build the solution: `dotnet build` — all projects should compile without errors
2. Check obsolete warnings fire: external code using `Languages` enum should get compiler warning
3. Run Sample.Maui: verify language window shows dynamically generated RadioButtons
4. Switch language: verify culture changes, strings translate, setting is persisted
5. Restart app: verify language preference is restored
6. Verify fallback: remove `ConfigureSupportedCultures()` call — app should work with English only

# Add ISupportedCulturesService fallback and ConfigureSupportedCultures extensions

Status: DONE
Started: 2026-06-02

## Steps
- [x] Read all files to understand current state
- [x] Add fallback + ConfigureSupportedCultures to WinUI (`WindowsAppBuilderExtensions.cs`)
- [x] Add fallback + ConfigureSupportedCultures to WPF (`WPFAppBuilderExtensions.cs`)
- [x] Add fallback + ConfigureSupportedCultures to AspNetCore.Blazor (`ServiceCollectionExtensions.cs`)
- [x] Add ConfigureSupportedCultures call to Sample.WPF
- [x] Add ConfigureSupportedCultures call to Sample.Blazor
- [x] Add ConfigureSupportedCultures call to Sample.WinUI
- [x] Build to verify compilation (all 6 projects compiled with 0 errors)

## Notes
- ISupportedCulturesBuilder and SupportedCulturesBuilder are only defined in MauiAppBuilderExtensions.cs (MAUI project)
- Other platform projects do not reference MAUI, so builder types were defined locally as nested types in each platform extension class, matching the MAUI pattern
- All 3 framework projects (WinUI, WPF, AspNetCore.Blazor) and all 3 sample projects (WPF, Blazor, WinUI) compile with 0 errors

# Dynamic Language/Culture Support
Status: DONE
Started: 2026-06-02

## Steps
- [x] Step 1: Core Layer — ISupportedCulturesService, SupportedCulturesService, LanguageOption, ILocalSettings change, delete Languages enum
- [x] Step 2: UI Extensions — LanguageExtensions, ILanguageViewModel, LanguageViewModel
- [x] Step 3: MAUI — LanguageWindow.xaml, delete converter, BaseShellViewModel
- [x] Step 4: MAUI — Application.cs, MauiAppBuilderExtensions
- [x] Step 5: WinUI, WPF, Blazor platform updates
- [x] Step 6: Samples and build verification

## Notes
- Plan: .ai/plans/adaptive-inventing-crane.md
- Build: 0 errors
- English always default/fallback via SupportedCulturesService
- Both neutral ("nl") and specific ("nl-NL") cultures supported
- RadioButtons show NativeName only; culture code saved in LocalSettings.Language (string)

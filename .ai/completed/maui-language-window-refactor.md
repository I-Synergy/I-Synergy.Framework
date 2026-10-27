# MAUI Language Window Refactor
Status: DONE
Started: 2026-06-02

## Steps
- [x] Read all files to understand current state
- [x] Rewrite LanguageWindow.xaml with BindableLayout
- [x] Delete RadioButtonToLanguageConverter.cs
- [x] Update BaseShellViewModel.cs (LanguageVM_Submitted signature and body)
- [x] Update Application.cs (null check on Language before SetLocalizationLanguage)
- [x] Update MauiAppBuilderExtensions.cs (ConfigureSupportedCultures + builder types)
- [x] Verify all changes compile

## Notes
- Language is now stored as string (culture name), not Languages enum
- LanguageViewModel is ViewModelDialog<CultureInfo>, SelectedItem is CultureInfo
- ISupportedCulturesService already exists in ISynergy.Framework.Core
- Supporting infrastructure is already in place
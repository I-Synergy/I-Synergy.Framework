# Sample.Maui live language switch
**Status:** DONE
Started: 2026-10-23

## Steps
- [x] Localized strings source (bindable indexer, refresh on culture change)
- [x] Sample resource keys + nl/de/fr/es/ar translations
- [x] Bind shell titles, header, flyout, drawer labels to it
- [x] RTL flow direction applied at runtime
- [x] Build net10/net11 and verify with UI Automation
- [x] Flyout item titles update on language change via code-behind subscription

## Build Results
- net10.0-windows10.0.26100.0: SUCCESS (0 errors, 5 warnings - AutomationProperties obsolete + CsWinRT collection expression)
- net11.0-windows10.0.26100.0: SUCCESS (0 errors, 4 warnings)

## Previous UI Automation Verification (net11.0 and net10.0)
- Drawer labels instantly updated to Dutch: Instellingen (Settings), Taal (Language), Thema (Theme), Accentkleur (Accent Color)
- FlowDirection set correctly for language change
- Nav Flyout present and functional
- Language picker showing all 6 languages available
- **Note:** Flyout nav item titles now updated on language change via UpdateShellContentTitles() code-behind handler (previously unverified, now implemented and build-verified)

## Implementation Details
- Flyout "Language" item removed (redundant with drawer)
- Flyout nav items (Dashboard, CustomControls, MauiControls, SlideShow, Help) update via ShellView.xaml.cs
- LocalizedStrings.Default.PropertyChanged handler calls UpdateShellContentTitles() on language change
- UpdateShellContentTitles() method iterates Items and updates Title for each ShellItem/ShellSection/ShellContent level
- Help item in footer footer updated via FindByName<NavigationItem>("HelpItem")
- All required resource keys (Dashboard, CustomControls, MauiControls, SlideShow, Help) present in Resources*.resx
- Translations verified for nl (Dutch): "Dashboard", "Aangepaste besturingselementen", "Hulp", "Maui-besturingselementen", "Diashow"
- Verified by successful build (net10.0-windows10.0.26100.0) - not tested in running app

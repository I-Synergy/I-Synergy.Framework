# WPF
- validation behavior in wpf behaves weird. Seems not to update correctly on input
- the theme switcher in wpf does not work

# WinUI
- closing the settings pane causes an exception in winui
- the help icon is dimmed and not aligned with the menu icons causing it to be clipped.
- the breadcrumb is set in the title which does not change

# Blazor
- The UI is using fluentUI and does not resemble the maui sample ui. Take a look at web app in D:\Projects\International Intelligence\The Quran

In winui we have this concept of blades (panes) which we have not in wpf or maui. Is it possible to recreate it? Make sure it respects also the rtl positioning

# WPF 
- wrong title (Maui sample > WPF sample)
- The system (close, min, max) buttons are padded too much too the left
- if maximized or minimized no main window rounding. Only between.  
- we have 2 kind of maximized. In kiosk mode the main window is maximized full covering the windows systembar. In normal mode (default) its maximized but the windows system bar is visible.

# WinUI
- The system buttons don't respect dark/light theme. In dark the buttons are light with dark icons
- we have 2 kind of maximized. In kiosk mode the main window is maximized full covering the windows systembar. In normal mode (default) its maximized but the windows system bar is visible. The current state is between. Maximized like kiosk with gaps at the left and right. See screenshots.

Also fix and validate all design inconsistencies

dotnet tool update -g Aspire.Cli --version 17.0.0-preview.1.26525.1 --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet9/nuget/v3/index.json
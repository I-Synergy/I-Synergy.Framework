# Theme ViewModel ColorOption Updates

Status: DONE
Started: 2026-06-19

## Steps
- [x] Read all 5 files requiring modification
- [x] Change 1: Update ThemeViewModel.cs with SelectedColorOption property and TwoWay binding support
- [x] Change 2: Update WinUI ThemeWindow.xaml to bind to ColorOptions and SelectedColorOption
- [x] Change 3: Update WPF ThemeWindow.xaml to bind to ColorOptions and SelectedColorOption
- [x] Change 4: Add UseThemeColors to Sample.WinUI Program.cs
- [x] Change 5: Add UseThemeColors to Sample.WPF App.xaml.cs
- [x] Build all 5 projects and verify no errors

## Notes
- ThemeViewModel already has ColorOption class defined
- Current GridView/ListBox bind to ThemeColors.Colors (static) - needs to change to ColorOptions (ViewModel property)
- Need to add SelectedColorOption property for TwoWay binding
- Sample apps need UseThemeColors extension chained with existing builder/hostBuilder

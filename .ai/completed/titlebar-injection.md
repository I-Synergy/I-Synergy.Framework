# TitleBar Injection via Virtual CreateTitleBar
**Status:** DONE
Started: 2026-07-12

## Steps
- [x] Replace readonly _titleBar field with virtual CreateTitleBar() method in framework Application.cs
- [x] Update CreateWindow to call CreateTitleBar() instead of using private field
- [x] Update SetClock to reference _mainWindow.TitleBar instead of private _titleBar field
- [x] Override CreateTitleBar in Sample.Maui App.xaml.cs to use custom XAML TitleBar control
- [x] Build and verify (0 errors both projects)

## Notes
Framework: `src/ISynergy.Framework.UI.Maui/Application/Application.cs`
- Line 52: `TitleBar? _titleBar` (nullable, no inline init)
- Lines 57-60: `protected virtual CreateTitleBar()` returns `new TitleBar()`
- Line 236: `var titleBar = CreateTitleBar();` called in CreateWindow
- Line 281: SetClock uses `_mainWindow?.TitleBar is TitleBar titleBar`

Sample: `samples/Sample.Maui/App.xaml.cs`
- Overrides CreateTitleBar to return `new Sample.Controls.TitleBar()` (XAML-based with SearchBar + ImageButton)

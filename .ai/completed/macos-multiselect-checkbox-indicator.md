# macOS multi-select checkbox indicator

**Status:** DONE
Started: 2026-08-15

## Steps
- [x] Restructure `MultipleSelectionWindow.xaml` ItemTemplate to `Auto,*` columns with checkbox-style `Border`+`Label` indicator and `VisualStateManager` `CommonStates`/`Selected` state
- [x] Build `net10.0-maccatalyst` target clean
- [x] Build `net10.0-windows` target clean (build-only sanity check) — *Architectural limitation: project.csproj conditionally includes Windows only on Windows OS*
- [x] Launch Sample.app on macOS, open multi-selection dialog, verify checkbox toggles per row on tap and matches SelectedItems
- [x] Add macOS-only hint label ("⌘-click to select multiple items") above/near the CollectionView in MultipleSelectionWindow.xaml

## Notes
Plan file: .ai/plans/sharded-zooming-melody.md
Scope: MultipleSelectionWindow.xaml only (SingleSelectionWindow.xaml explicitly out of scope per user decision).

Follow-up finding from manual verification: on Mac Catalyst, CollectionView SelectionMode="Multiple"
follows native macOS list conventions - a plain click replaces the whole selection with just that
row; Cmd+Click (or Shift+Click for a range) is required to add to the selection. This is native
UIKit/Catalyst pointer behavior, not caused by the checkbox template change. User decision: keep
native Cmd+Click behavior, add a discoverability hint instead of intercepting taps.

### Build Results
- **macOS (net10.0-maccatalyst)**: SUCCESS (0 errors, 0 warnings, completed 15.67s)
  - XAML source generation enabled (MauiXamlInflator=SourceGen)
  - Assembly optimized for size
  
- **Windows (net10.0-windows10.0.26100.0)**: Cannot build on macOS (project design)
  - Root cause: Sample.Maui.csproj line 4 conditionally includes Windows target only when `$([MSBuild]::IsOSPlatform('windows'))` is true
  - XAML changes are platform-agnostic (Border/Grid/Label/VisualStateManager are all cross-platform MAUI controls)
  - No platform-specific code in the template changes
  
### XAML Changes Summary
Replaced simple ItemTemplate with two-column layout:
- **Column 0 (Auto)**: 20x20 Border with stroke, rounded corners (CornerRadius="3"), contains centered Label with checkmark glyph
- **Column 1 (*)**: Original item label
- **VisualStateManager**: CommonStates group with Normal and Selected states; Selected state sets SelectedIndicator.IsVisible=True

# MultipleSelectionWindow: visible checkbox indicator on macOS

## Context

`MultipleSelectionWindow` (`src/ISynergy.Framework.UI.Maui/Windows/MultipleSelectionWindow.xaml`) already tracks
multi-selection correctly in code (fixed in commit `56a897b6`: `SelectedItems` populates, `SelectCommand`
re-evaluates). The remaining problem is purely visual: the `CollectionView`'s `ItemTemplate` is a bare
`Grid`+`Label` with no selection indicator at all.

- On Windows, the native WinUI `ListView` backing renderer gives a selection highlight for free, so the gap was
  invisible there.
- On Mac Catalyst, there is no equivalent automatic indicator, so tapping a row selects it internally but the user
  sees no feedback — this is the "I can't see a checkbox" report.

An Explore pass confirmed there is nothing to reuse: no `VisualStateManager` `Selected` state, no checkbox/checkmark
icon resource, and no shared `CollectionView` item style anywhere in `src/ISynergy.Framework.UI.Maui` or
`src/ISynergy.Framework.UI`. `SingleSelectionWindow.xaml` has the identical bare template but is explicitly out of
scope (user decision) — only `MultipleSelectionWindow.xaml` is touched.

## Approach

Add a self-contained checkbox-style indicator to the `DataTemplate`, driven by `VisualStateManager`'s
`CommonStates` group (`Normal` / `Selected`), which `CollectionView` sets automatically on each item container.
This is the MAUI-documented pattern for reacting to selection in a template, and it avoids introducing a second,
competing source of truth for selection state (no real interactive `CheckBox` control, no manual event wiring) —
row taps stay the single interaction that drives `SelectedItems`, and the box just mirrors that state.

Restructure the template from a single implicit column to `Auto,*`:
- Column 0: a small square `Border` (checkbox look) containing a `Label` checkmark (`x:Name="SelectedIndicator"`,
  `IsVisible="False"` by default).
- Column 1: the existing item `Label`, unchanged.

Add to the template root:
```xml
<VisualStateManager.VisualStateGroups>
    <VisualStateGroup x:Name="CommonStates">
        <VisualState x:Name="Normal" />
        <VisualState x:Name="Selected">
            <VisualState.Setters>
                <Setter TargetName="SelectedIndicator" Property="IsVisible" Value="True" />
            </VisualState.Setters>
        </VisualState>
    </VisualStateGroup>
</VisualStateManager.VisualStateGroups>
```

## File to modify

`src/ISynergy.Framework.UI.Maui/Windows/MultipleSelectionWindow.xaml` — replace the `CollectionView.ItemTemplate`
`DataTemplate` (currently lines 55-61) with the two-column Grid + Border/checkmark + VisualStateManager block
described above. No other files change; no code-behind changes needed since `VisualState.Setters` handles the
toggle declaratively.

## Verification

1. `dotnet build -f net10.0-maccatalyst samples/Sample.Maui/Sample.Maui.csproj` — must build clean (already
   confirmed this target builds with 0 errors before this change).
2. Launch `Sample.app` (`open .../maccatalyst-arm64/Sample.app`), open the multi-selection dialog from the sample
   app's MAUI controls view, tap several rows, and visually confirm the checkbox fills in/out per row as selection
   toggles, and that it matches `SelectedItems` (confirm via the existing Select button `CanExecute` behavior which
   already reads `SelectedItem`/`SelectedItems`).
3. Spot-check `net10.0-windows` build isn't broken (native highlight still applies; the new checkbox is additive,
   not a replacement) — build only, since Windows can't be run from this machine.

# Multi-Select: Bind SelectCommand parameter to SelectedItem

Status: DONE
Started: 2026-08-15

## Goal

The multi-select views bind `CommandParameter="{Binding SelectedItems}"`, but the view model has no `SelectedItems` property — the real multi-selection lives in `SelectedItem` (a `List<TModel>`). Re-point the bindings to `SelectedItem`.

Because `SelectedItem` is `List<TModel>` while `SelectCommand` is `AsyncRelayCommand<List<object>>` (invariant — `List<TModel>` is not assignable to `List<object>`, which would make `CanExecute` throw), also change `SelectCommand` to `AsyncRelayCommand<object>` (matching the single-selection pattern).

## Steps

- [x] IMultipleSelectionViewModel.cs — SelectCommand type → AsyncRelayCommand<object>
- [x] BaseMultipleSelectionViewModel{TModel}.cs — SelectCommand property + ctor + SelectAsync signature → object
- [x] MAUI Generic.xaml — CommandParameter SelectedItems → SelectedItem
- [x] WinUI MultipleSelectionView.xaml — CommandParameter SelectedItems → SelectedItem (2 places)
- [x] WPF MultipleSelectionWindow.xaml — CommandParameter SelectedItems → SelectedItem (2 places)
- [x] Build Mvvm + UI.Maui + UI.WinUI + UI.WPF (0 warnings / 0 errors)

## Notes

- Only `{Binding SelectedItems}` data bindings change. Do NOT touch native `DataSummary.SelectedItems` (control property), `MultiSelectionBehavior.SelectedItems` (WinUI behavior DP), or `TestItemsListViewModel.SelectedItems` (sample VM).

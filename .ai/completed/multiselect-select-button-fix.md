# Multi-Select: Enable Select Button After Selection

Status: DONE
Started: 2026-08-15

## Root cause

Two bugs keep the multi-select "Select" button disabled:

1. `BaseMultipleSelectionViewModel.SelectCommand` `canExecute` depends on the command parameter `s`, which every platform's view binds to a non-existent `SelectedItems` property → always null → button never enabled.
2. The platform code-behind `SelectionChanged` handlers mutate `viewModel.SelectedItem` through the explicit interface property `IMultipleSelectionViewModel.SelectedItem`, which returns a COPY (`SelectedItem?.Cast<object>().ToList()`), so the real `SelectedItem` (`List<TModel>`) is never populated.

## Fix

- Make `canExecute` check `SelectedItem.Count` directly (ignore the parameter).
- Change each platform's handler to call `SetSelectedItem(selectedItems)` instead of mutating the copy.

## Steps

- [x] BaseMultipleSelectionViewModel.cs — canExecute checks SelectedItem.Count
- [x] MAUI MultipleSelectionWindow.xaml.cs — SetSelectedItem(DataSummary.SelectedItems)
- [x] WinUI MultipleSelectionView.xaml.cs — SetSelectedItem(DataSummary.SelectedItems)
- [x] WPF MultipleSelectionWindow.xaml.cs — SetSelectedItem(DataSummary.SelectedItems.Cast<object>())
- [x] Build Mvvm + UI.Maui + UI.WinUI + UI.WPF (0 warnings / 0 errors)

## Notes

- Vestigial `CommandParameter="{Binding SelectedItems}"` bindings remain harmless after the canExecute fix (parameter is ignored); not removed in this change.

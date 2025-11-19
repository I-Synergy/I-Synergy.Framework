# ViewModelDialog<TModel> Unit Tests

## Overview
Comprehensive test suite for `ViewModelDialog<TModel>` covering PropertyChanged subscription, command re-evaluation, and cleanup scenarios.

## Test Categories

### 1. **PropertyChanged Subscription Tests**
Tests that verify automatic subscription to `INotifyPropertyChanged` on `SelectedItem`:

| Test | Description | Expected Behavior |
|------|-------------|-------------------|
| `SelectedItem_WhenSetWithObservableEntity_SubscribesToPropertyChanged` | Sets an observable entity and changes a property | PropertyChanged count increases |
| `SelectedItem_WhenNestedPropertyChanges_RaisesPropertyChangedForSelectedItem` | Changes a property on the nested entity | SelectedItem PropertyChanged is raised |
| `SelectedItem_WhenReplacedWithNewObservableEntity_UnsubscribesFromOld` | Replaces SelectedItem with a new entity | Old entity changes don't trigger events |
| `SelectedItem_WhenSetToNull_UnsubscribesFromPrevious` | Sets SelectedItem to null | Previous entity changes don't trigger events |
| `SelectedItem_WithNonObservableEntity_DoesNotSubscribe` | Sets a non-observable entity | No exception, command works normally |

### 2. **Command Re-evaluation Tests**
Tests that verify `SubmitCommand.CanExecute` behavior:

| Test | Description | Expected Behavior |
|------|-------------|-------------------|
| `SubmitCommand_WhenSelectedItemIsNull_CannotExecute` | SelectedItem is null | CanExecute returns false |
| `SubmitCommand_WhenSelectedItemIsNotNull_CanExecute` | SelectedItem is set | CanExecute returns true |
| `SubmitCommand_WhenObservableEntityPropertyChanges_CanExecuteRemainsTrue` | Nested property changes | CanExecute remains true |
| `SubmitCommand_WhenSelectedItemSet_NotifyCanExecuteChangedIsCalled` | SetSelectedItem is called | CanExecuteChanged event is raised |
| `SubmitCommand_WhenObservableEntityPropertyChanges_NotifyCanExecuteChangedIsCalled` | Nested property changes | CanExecuteChanged event is raised |

### 3. **Cleanup and Disposal Tests**
Tests that verify proper event unsubscription:

| Test | Description | Expected Behavior |
|------|-------------|-------------------|
| `Cleanup_WithObservableEntity_UnsubscribesFromPropertyChanged` | Cleanup is called | PropertyChanged stops firing |
| `Dispose_WithObservableEntity_UnsubscribesFromPropertyChanged` | Dispose is called | PropertyChanged stops firing |

### 4. **ThemeStyle-like Scenario Tests**
Tests that simulate the ThemeWindow use case:

| Test | Description | Expected Behavior |
|------|-------------|-------------------|
| `ThemeStyleScenario_WhenMultiplePropertiesChange_RaisesPropertyChangedForEach` | Multiple properties change (Theme + Color) | PropertyChanged raised for each change |
| `ThemeStyleScenario_CommandCanExecute_AlwaysTrueAfterInitialization` | Properties change after initialization | SubmitCommand remains executable |

## Test Entities

### `TestEntity`
A simple POCO entity (no INotifyPropertyChanged):
- Used to test scenarios where SelectedItem is not observable
- Verifies the base class handles non-observable types gracefully

### `TestObservableEntity`
An observable entity inheriting from `ObservableClass`:
- Simulates `ThemeStyle` behavior
- Properties: `Name` (string), `Value` (int)
- Used to test PropertyChanged subscription scenarios

### `TestObservableDialogViewModel`
A test ViewModel that tracks PropertyChanged event counts:
- Inherits from `ViewModelDialog<TestObservableEntity>`
- Exposes `PropertyChangedCallCount` for assertion
- Used to verify event propagation

## Running the Tests

```bash
dotnet test --filter "FullyQualifiedName~ViewModelDialogTests"
```

## Coverage Summary

? **PropertyChanged Subscription**: Verifies automatic subscription when SelectedItem is set  
? **Event Propagation**: Verifies nested property changes raise PropertyChanged for SelectedItem  
? **Unsubscription on Replace**: Verifies old items are unsubscribed when SelectedItem changes  
? **Unsubscription on Cleanup**: Verifies proper cleanup prevents memory leaks  
? **Command Re-evaluation**: Verifies SubmitCommand.CanExecute re-evaluates correctly  
? **Non-Observable Entities**: Verifies base class handles POCOs without errors  
? **ThemeStyle Scenario**: Verifies real-world usage patterns work as expected  

## Key Assertions

### For OK Button Visual State Issue
The tests verify that:
1. When `SelectedItem` is set ? `SubmitCommand.CanExecute(SelectedItem)` returns `true`
2. When nested properties change ? `SubmitCommand.CanExecuteChanged` event is raised
3. The OK button can re-evaluate its visual state based on these events

### For Memory Leaks
The tests verify that:
1. When `SelectedItem` is replaced ? old entity is unsubscribed
2. When `Cleanup()` is called ? all subscriptions are removed
3. When `Dispose()` is called ? all resources are released

## Related Issues

This test suite validates the fix for:
- **Issue**: ThemeWindow OK button appears disabled despite being clickable
- **Root Cause**: `SubmitCommand.CanExecute` not re-evaluating when ThemeStyle properties changed
- **Solution**: Automatic PropertyChanged subscription in `ViewModelDialog<TModel>`

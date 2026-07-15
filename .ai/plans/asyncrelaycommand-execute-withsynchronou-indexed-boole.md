# Fix: Event Handler Exception Handling Gaps

## Context

Three groups of tests are failing because event invocation code and tests are out of sync after the ServiceLocator removal refactoring (commit `eb8b19df`). Each group has a different problem:

### Failing Tests Summary

| Test | Why it fails |
|------|-------------|
| `ViewModelDialog_OnSubmitted_CallsExceptionHandlerService` | Test expects exception to propagate, but `OnSubmitted` intentionally catches + delegates to `IExceptionHandlerService` |
| `ViewModelDialog_OnSubmitted_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled` | Same — expects both propagation AND handler isolation |
| `ViewModelNavigation_OnSubmitted_CallsExceptionHandlerService` | Same as dialog version |
| `RaisePropertyChanged_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled` | `RaisePropertyChanged` has NO try/catch — handler2's exception stops the loop before handler3 fires |

## Root Cause Analysis

### ViewModel.OnSubmitted — code is correct, tests are wrong

`ViewModelSelectable<TModel>.OnSubmitted` already has the right design:
```csharp
foreach (EventHandler<SubmitEventArgs<TModel>> handler in handlers)
{
    try
    {
        handler.Invoke(this, e);
    }
    catch (Exception ex)
    {
        _commonServices.ExceptionHandlerService?.HandleException(ex); // delegate, continue loop
    }
}
```

Exceptions are caught per-handler so subsequent handlers still fire. They're delegated to `IExceptionHandlerService`, not re-thrown. The tests need to verify `HandleException` was called, not expect exceptions to propagate.

### ObservableClass.RaisePropertyChanged — code is missing error handling

The comment says "Invoke each handler individually to prevent one exception from stopping others" but there's no `try/catch`:
```csharp
foreach (var h in handlers)
{
    h.Invoke(this, new PropertyChangedEventArgs(propertyName));  // no try/catch!
}
```

`ObservableClass` (in Core) has no reference to `IExceptionHandlerService`. Needs try/catch + debug logging.

## Plan

### Step 1: Fix `ObservableClass.RaisePropertyChanged` — add try/catch per handler

File: `src/ISynergy.Framework.Core/Base/ObservableClass.cs` (line ~224)

```csharp
foreach (var h in handlers)
{
    try
    {
        h.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Exception in PropertyChanged handler: {ex.Message}");
        // Continue to next handler — don't let one broken handler stop others
    }
}
```

No `IExceptionHandlerService` dependency — `ObservableClass` is in Core where that pattern isn't available. Exceptions are logged via `Debug.WriteLine` and execution continues to the next handler.

### Step 2: Fix `ViewModelExceptionHandlingTests` — verify HandleException was called

File: `tests/ISynergy.Framework.Mvvm.Tests/ViewModels/ViewModelExceptionHandlingTests.cs`

The tests need to:
1. Mock `IExceptionHandlerService` on `_mockCommonServices`
2. Verify `HandleException(ex)` was called instead of expecting exception to propagate
3. For the multiple-handlers test: verify all handlers fire (or at least the handler after the throwing one fires)

Updated test pattern:
```csharp
[TestMethod]
public void ViewModelDialog_OnSubmitted_CallsExceptionHandlerService()
{
    // Setup mock
    var mockExceptionHandler = new Mock<IExceptionHandlerService>();
    _mockCommonServices.SetupGet(s => s.ExceptionHandlerService).Returns(mockExceptionHandler.Object);

    var viewModel = new TestViewModelDialog(_mockCommonServices.Object, _mockLogger.Object);
    var expectedException = new InvalidOperationException("Handler exception");
    var submitEventArgs = new SubmitEventArgs<string>("test");
    viewModel.Submitted += (s, e) => throw expectedException;

    viewModel.TestSubmit(submitEventArgs);  // Should NOT throw

    mockExceptionHandler.Verify(
        s => s.HandleException(It.Is<InvalidOperationException>(ex => ex == expectedException)),
        Times.Once);
}
```

Same pattern for:
- `OnSubmitted_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled` — verify HandleException called once AND other handlers fired
- `Navigation_OnSubmitted_CallsExceptionHandlerService` — same as dialog version
- `OnSubmitted_WithException_WhenHandlerServiceNotAvailable_LogsToDebug` — already passes, verify no throw
- `OnSubmitted_WithException_WhenHandlerServiceThrows_DoesNotCrash` — verify other handlers still fire even when handler service throws

### Step 3: Fix `ObservableClassExceptionHandlingTests` — reflect corrected behavior

File: `tests/ISynergy.Framework.Core.Tests/Base/ObservableClassExceptionHandlingTests.cs`

After the `RaisePropertyChanged` fix (Step 1), exceptions are caught and logged, not propagated. Tests need updating:

- `RaisePropertyChanged_WithExceptionInHandler_CallsExceptionHandlerService` — remove `Assert.Throws`, verify no exception propagates
- `RaisePropertyChanged_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled` — remove `Assert.Throws`, verify handler1 and handler3 both fire
- `RaisePropertyChanged_WithException_WhenHandlerServiceNotAvailable_LogsToDebug` — remove `Assert.Throws`, verify no exception (already passes logic-wise)
- `RaisePropertyChanged_WithException_WhenHandlerServiceThrows_DoesNotCrash` — remove `Assert.Throws`, verify no exception

## Files to Modify

1. `src/ISynergy.Framework.Core/Base/ObservableClass.cs` — Add try/catch in `RaisePropertyChanged`
2. `tests/ISynergy.Framework.Mvvm.Tests/ViewModels/ViewModelExceptionHandlingTests.cs` — Rewrite tests to verify `HandleException` calls
3. `tests/ISynergy.Framework.Core.Tests/Base/ObservableClassExceptionHandlingTests.cs` — Remove `Assert.Throws`, verify handler isolation

## Verification

```bash
dotnet build src/ISynergy.Framework.Core
dotnet build tests/ISynergy.Framework.Core.Tests
dotnet test tests/ISynergy.Framework.Core.Tests --filter "ClassName=ObservableClassExceptionHandlingTests"
dotnet test tests/ISynergy.Framework.Mvvm.Tests --filter "ClassName=ViewModelExceptionHandlingTests"
```

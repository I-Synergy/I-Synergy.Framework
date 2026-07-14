# Fix: RaisePropertyChanged Exception Handling

**Status:** DONE
**Started:** 2026-07-14

## Context

The test `RaisePropertyChanged_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled` fails because when multiple `PropertyChanged` handlers are attached and one throws, subsequent handlers are never invoked.

This was already fixed for the `OnSubmitted` method in `ViewModelSelectable<TModel>` in commit `fb6fa00c` ("fix: continue invoking OnSubmitted handlers after one throws"). The user explicitly requested the same approach be applied to `RaisePropertyChanged`.

### Root Cause

`ObservableClass.RaisePropertyChanged` (line 215-229) iterates over `GetInvocationList()` but does **not** wrap each `h.Invoke()` in try/catch. When a handler throws, the exception exits the loop immediately, skipping remaining handlers.

### Fix Pattern (from OnSubmitted)

```csharp
Exception? capturedException = null;
foreach (var handler in handlers)
{
    try { handler.Invoke(...); }
    catch (Exception ex)
    {
        // log + capture, then continue
        capturedException = ex;
    }
}
if (capturedException is not null) throw capturedException;
```

## Changes

### 1. `src/ISynergy.Framework.Core/Base/ObservableClass.cs` — `RaisePropertyChanged` method

Wrap each handler invocation in try/catch. Capture the first exception, log via `Debug.WriteLine`, and rethrow after all handlers have run.

Since `ObservableClass` is in the Core library and does not have an `ILogger`, use `System.Diagnostics.Debug.WriteLine` for logging (same approach as the pre-fix OnSubmitted).

### No test changes needed

The test `RaisePropertyChanged_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled` already asserts the correct contract:
- `Assert.Throws<InvalidOperationException>` — exception is still surfaced
- `handler1Called == true` — first handler fires
- `handler3Called == true` — third handler fires even though second threw

## Verification

1. Build the Core project: `dotnet build src/ISynergy.Framework.Core/`
2. Run the specific test: `dotnet test tests/ISynergy.Framework.Core.Tests/ --filter "RaisePropertyChanged_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled"`
3. Run all ObservableClass exception handling tests to confirm no regressions

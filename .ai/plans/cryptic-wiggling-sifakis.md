# Remove ServiceLocator & Command-Level Exception Handling

**Status:** PLAN
**Started:** 2026-07-14

## Context

All RelayCommand and AsyncRelayCommand classes use `ServiceLocator.Default` to resolve `IExceptionHandlerService` in their catch blocks. This is an anti-pattern because:

1. **ServiceLocator is already deleted** (`ServiceLocator.cs` shows as `D` in git status) but the imports and usages remain — code is broken
2. **Duplicate exception handling** — the applications already wire up global exception handlers (`DispatcherUnhandledException`, `UnobservedTaskException`, `app.UnhandledException`, `GlobalExceptionHandler`) that route through `IExceptionHandlerService`
3. **Commands should be thin wrappers** — they execute delegates, not handle errors. Exception handling belongs at the application boundary
4. **ErrorHandlingStrategy enum is dead code** — defined but never checked in any logic path

## Plan

### Files to modify (7 total):

#### 1. `src/ISynergy.Framework.Mvvm/Commands/Base/BaseAsyncRelayCommand.cs`

- Remove `using ISynergy.Framework.Core.Locators;`
- Remove `using ISynergy.Framework.Core.Abstractions.Services;`
- Remove `_exceptionHandlerService` field (line 109)
- Remove `exceptionHandlerService` parameter from constructor (simplify signature)
- Remove `ErrorHandlingStrategy` property (line 103)
- Remove `ErrorHandlingStrategy` enum (lines 930-944)
- Change `AwaitAndThrowIfFailed` to simply `await executionTask;` without try/catch — let the `async void` propagate exceptions to the SynchronizationContext/global handler

#### 2. `src/ISynergy.Framework.Mvvm/Commands/AsyncRelayCommand.cs`

- Remove `using ISynergy.Framework.Core.Locators;`
- Remove `using ISynergy.Framework.Core.Abstractions.Services;`
- Remove `bool exceptionAlreadyHandled = false;` variable declaration
- Remove all `catch (TimeoutException ex)` blocks (lines 223-246 in first try, lines 289-312 in second try)
- Remove all `catch (Exception ex)` blocks (lines 247-270 in first try, lines 313-336 in second try)
- Remove `when (!exceptionAlreadyHandled)` clauses from remaining catch blocks
- **Keep** `catch (OperationCanceledException)` blocks (control flow, not error handling)
- Simplify the inner try/catch structure

#### 3. `src/ISynergy.Framework.Mvvm/Commands/AsyncRelayCommand{T}.cs`

- Same changes as AsyncRelayCommand.cs (mirror structure)
- Remove 4 ServiceLocator sites, remove exceptionAlreadyHandled, keep OperationCanceledException

#### 4. `src/ISynergy.Framework.Mvvm/Commands/RelayCommand.cs`

- Remove `using ISynergy.Framework.Core.Locators;`
- Remove `using ISynergy.Framework.Core.Abstractions.Services;`
- Remove the entire try/catch around `_execute?.Invoke();` (lines 48-77) — let exception propagate directly
- `Execute()` becomes: `_execute?.Invoke();` (one line)

#### 5. `src/ISynergy.Framework.Mvvm/Commands/RelayCommand{T}.cs`

- Remove `using ISynergy.Framework.Core.Locators;`
- Remove `using ISynergy.Framework.Core.Abstractions.Services;`
- Remove the entire try/catch around `_execute(parameter);` (lines 66-96)
- `Execute(T? parameter)` becomes: `_execute(parameter);` (one line)

#### 6. `tests/ISynergy.Framework.Mvvm.Tests/Commands/ExceptionHandlingTests.cs`

All tests in this file validate the ServiceLocator-based exception handling pattern being removed. They need to be rewritten to test the **new behavior**:

- **Remove**: Tests that verify `_mockExceptionHandler` was called (no longer happens in commands)
- **Remove**: Test that verifies ServiceLocator fallback when handler is null
- **Add**: Test that synchronous exceptions in `Execute()` propagate directly (not caught)
- **Add**: Test that `ExecuteAsync()` returns a faulted task when the delegate throws
- **Add**: Test that `OperationCanceledException` is handled (swallowed unless `FlowExceptionsToTaskScheduler` is set)
- **Remove**: All `ServiceLocator.SetLocatorProvider()` setup — no longer needed
- **Remove**: `using ISynergy.Framework.Core.Locators;` import
- **Remove**: Mock setup for `IServiceProvider`, `IServiceScope`, `IServiceScopeFactory` (no longer needed)

#### 7. `tests/ISynergy.Framework.Mvvm.Tests/Commands/AsyncRelayCommandTests.cs`

- Check for and remove any ServiceLocator references or exception handling assertions that reference the old pattern

### What stays unchanged:
- `IExceptionHandlerService` interface — still used by global handlers in Application classes
- `BaseExceptionHandlerService` — still used by global handlers in Application classes
- `ICommonServices.ExceptionHandlerService` property — still valid for injection into global handlers
- `AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler` — still controls whether sync `Execute()` awaits the task
- `OperationCanceledException` handling in async commands — control flow, not error handling
- Global exception handlers in WPF/WinUI/MAUI/Blazor/ASP.NET Application classes
- All constructors in `AsyncRelayCommand` and `AsyncRelayCommand{T}` (no API change)

### Exception flow after changes:

| Path | Before | After |
|------|--------|-------|
| `ExecuteAsync()` called by binding | Exception caught, handled via ServiceLocator, swallowed | Exception propagates through Task, platform handles |
| `Execute()` (sync ICommand) | Exception caught in ExecuteAsync, swallowed. `AwaitAndThrowIfFailed` sees clean task | Exception propagates through Task, `AwaitAndThrowIfFailed` awaits faulted task, exception propagates via async void to SynchronizationContext → global handler |
| `FlowExceptionsToTaskScheduler` | Exception caught, handled, then re-thrown to TaskScheduler | Exception propagates directly to TaskScheduler.UnobservedTaskException → global handler |

## Verification

1. **Build**: `dotnet build` the entire solution — must compile clean (ServiceLocator imports removed)
2. **Tests**: Run `dotnet test tests/ISynergy.Framework.Mvvm.Tests/` — update ExceptionHandlingTests to match new behavior, ensure all other tests pass
3. **Manual check**: `grep -r "ServiceLocator" src/ISynergy.Framework.Mvvm/` returns zero results
4. **Manual check**: `grep -r "ErrorHandlingStrategy" src/` returns zero results
5. **Manual check**: `grep -r "exceptionAlreadyHandled" src/` returns zero results

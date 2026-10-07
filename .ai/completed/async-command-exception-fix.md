# Async RelayCommand Exception Propagation Fix

Status: DONE
Started: 2026-07-14

## Steps
- [x] Step 1: Fix Execute() in AsyncRelayCommand.cs (flip condition + add LogIfFailed for None path)
- [x] Step 2: Fix Execute(T?) in AsyncRelayCommand{T}.cs (same change)
- [x] Step 3: Replace AwaitAndThrowIfFailed with synchronous re-throw in BaseAsyncRelayCommand.cs
- [x] Step 4: Add LogIfFailed method in BaseAsyncRelayCommand.cs
- [x] Step 5: Remove ErrorHandlingStrategy dead code in BaseAsyncRelayCommand.cs
- [x] Step 6: Fix tests in ExceptionHandlingTests.cs
- [x] Step 7: Build and test

## Notes
- Default option (FlowExceptionsToTaskScheduler) should propagate exceptions via Execute()
- None option should log via Debug, not propagate
- Need to flip condition from == 0 to != 0 in both Execute methods

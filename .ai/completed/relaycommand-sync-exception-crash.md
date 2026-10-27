# RelayCommand synchronous exception crash fix

**Status:** DONE
Started: 2026-08-14
Completed: 2026-08-14

## Post-completion note
A full (unfiltered) `dotnet test` run of `ISynergy.Framework.Mvvm.Tests` surfaced
another pre-existing test file, `tests/ISynergy.Framework.Mvvm.Tests/Commands/ExceptionHandlingTests.cs`,
not caught by the initial `grep` sweep, which also asserted the old
synchronous-rethrow-and-crash behavior for `RelayCommand`, `RelayCommand<T>`,
and `AsyncRelayCommand`. Updated its 4 affected tests to assert the new
behavior (`Execute()` does not throw; the exception is observed via
`CommandExceptionDispatcher.UnhandledCommandException` instead), same pattern
as the other 3 test files. One unrelated pre-existing failure remains in the
full run: `FileResultTests.Constructor_InitializesProperties` fails on macOS
because it asserts Windows path-parsing behavior (`C:\test\file.txt` ->
`file.txt`) — confirmed unrelated to this change (fails identically in
isolation, has nothing to do with commands).

## Context
Confirmed live (crash repro + macOS .ips crash report analysis, see
`.ai/completed/nativeaot-maui-buttons.md` session for method) that clicking
"Throw Not Implemented error" in Sample.Maui's Custom Controls view
(`CustomControlsViewModel.NotImplementedErrorCommand`, a plain
`RelayCommand`) crashes the app with `SIGABRT`.

Root cause: `RelayCommand.Execute()` (src/ISynergy.Framework.Mvvm/Commands/RelayCommand.cs:40-48)
has no try/catch around `_execute?.Invoke()`. A synchronous throw from a
Button-bound command becomes a genuine Mono/CLR unhandled exception on the
main thread. Crash report showed the exact chain:
`abort() <- xamarin_unhandled_exception_handler (runtime.m:1010) <-
mono_invoke_unhandled_exception_hook (exception.c:1219)`.

The framework's global exception plumbing (`AppDomain.UnhandledException` /
`MauiExceptions.UnhandledException` -> `Application.CurrentDomain_UnhandledException`
-> `IExceptionHandlerService.HandleException`, which already special-cases
`NotImplementedException` to show a friendly dialog via
`BaseExceptionHandlerService.cs:62-65`) DOES run, but the native Xamarin
runtime layer calls `abort()` unconditionally afterward regardless — that
event is not cancelable, so the dialog can never actually prevent the crash
via this path.

`AsyncRelayCommand` doesn't have this problem for genuinely-async faults:
its `Execute()` fires `ExecuteAsync()` and (with the default
`FlowExceptionsToTaskScheduler` option) only rethrows synchronously if the
task is *already* faulted when checked — a real async fault (after an
`await`) becomes an unobserved task exception instead, which .NET does not
abort the process for, and gets routed to `IExceptionHandlerService` via
each platform's `TaskScheduler.UnobservedTaskException` handler.
BUT: `AsyncNotImplementedErrorCommand`'s lambda
(`() => throw new NotImplementedException()`) throws *before any await*, so
it likely hits the same synchronous-rethrow-and-crash path — needs
verification/fix too.

## User's requirement
Fix must route through the existing exception handler and show the dialog
(same UX as async command failures / `BaseExceptionHandlerService`) —
NOT just avoid crashing silently.

## Architecture note (found during investigation, not yet decided)
- `IExceptionHandlerService` lives in `ISynergy.Framework.Core.Abstractions.Services`.
  `ISynergy.Framework.Mvvm` already references `ISynergy.Framework.Core`
  (see its .csproj), so `RelayCommand` CAN reference the interface type.
  But there is no existing service-locator/static-DI-accessor pattern in
  Mvvm or Core to actually **obtain an instance** of it from a command
  constructed via `new RelayCommand(...)` (no DI injection point).
- The existing pattern for `AsyncRelayCommand` does NOT call
  `IExceptionHandlerService` directly either — it relies on
  `TaskScheduler.UnobservedTaskException`, which each platform's
  `Application` class subscribes to and forwards to
  `IExceptionHandlerService.HandleException` (confirmed wired in all 4
  platform heads: `ISynergy.Framework.UI.Maui/Application/Application.cs`,
  `ISynergy.Framework.UI.WinUI/Application/Application.cs`,
  `ISynergy.Framework.UI.WPF/Application/Application.cs`,
  `ISynergy.Framework.UI.Blazor/Application/Application.cs`).
- Recommended direction (not mandatory — use judgment): mirror that existing
  pattern instead of coupling Mvvm to a DI container. Add a static event
  (e.g. on `BaseRelayCommand` or a shared static helper in
  `ISynergy.Framework.Mvvm.Commands`) that `RelayCommand`/`RelayCommand<T>`
  raise from a catch block in `Execute()`, and subscribe to it from all 4
  platform `Application` classes alongside their existing
  `TaskScheduler_UnobservedTaskException`/`CurrentDomain_UnhandledException`
  handlers, forwarding to `_exceptionHandlerService.HandleException(ex)`.
  This keeps Mvvm UI-framework-agnostic and reuses the dialog-showing logic
  that already exists and already special-cases `NotImplementedException`.

## Steps
- [x] Design/confirm the exception-routing mechanism for sync `RelayCommand`
      failures (static event mirroring existing UnobservedTaskException
      pattern, or an alternative — document the choice and why)
      -> Implemented `ISynergy.Framework.Mvvm.Commands.Base.CommandExceptionDispatcher`,
      a static class with a static `EventHandler<CommandExceptionEventArgs>
      UnhandledCommandException` event (event args in
      `ISynergy.Framework.Mvvm.Events.CommandExceptionEventArgs`), mirroring the
      recommended direction exactly. `RelayCommand`/`RelayCommand<T>.Execute` now
      catch and call `CommandExceptionDispatcher.Raise(this, ex)` instead of letting
      the exception propagate synchronously.
- [x] Fix `RelayCommand.Execute()` (src/ISynergy.Framework.Mvvm/Commands/RelayCommand.cs)
- [x] Fix `RelayCommand<T>.Execute()` (src/ISynergy.Framework.Mvvm/Commands/RelayCommand{T}.cs)
- [x] Check/fix the synchronous-throw path in `AsyncRelayCommand`/`AsyncRelayCommand<T>`
      (`AwaitAndThrowIfFailed` rethrows synchronously if the task is already
      faulted when checked, which happens for a lambda that throws before
      any `await` — e.g. `AsyncNotImplementedErrorCommand` in the sample)
      -> Confirmed the bug: `ExecuteAsync` is itself an `async Task` method, so a
      synchronous throw inside its body never throws synchronously to `Execute`'s
      caller — it's captured into the returned Task, which is already `IsFaulted`
      by the time `AwaitAndThrowIfFailed` checks it. Previously that method did
      `ExceptionDispatchInfo.Capture(exception).Throw()`, i.e. rethrew synchronously
      on the UI thread -> same SIGABRT crash path as plain `RelayCommand`. Changed
      `AwaitAndThrowIfFailed` to route through `CommandExceptionDispatcher.Raise`
      instead of rethrowing (accessing `.Exception` also marks the task observed,
      so it can't double-report via `TaskScheduler.UnobservedTaskException` later).
      Genuinely-async faults (after a real `await`) are untouched by this method and
      still flow to `TaskScheduler.UnobservedTaskException` as before.
- [x] Wire the new mechanism into all 4 platform `Application` classes
      (Maui, WinUI, WPF, Blazor) so the dialog shows on every platform, not
      just Maui
      -> All 4 `Application` classes now subscribe to
      `CommandExceptionDispatcher.UnhandledCommandException` alongside their
      existing `TaskScheduler.UnobservedTaskException`/`CurrentDomain_UnhandledException`
      subscriptions (constructor), forward to a new
      `CommandExceptionDispatcher_UnhandledCommandException` handler that calls
      `_exceptionHandlerService.HandleException(e.Exception)` (mirroring each
      platform's existing `TaskScheduler_UnobservedTaskException` handler style,
      including Blazor's `_isDisposing` guard/try-catch), and unsubscribe in
      `Dispose(bool)`.
- [x] Build the solution, verify 0 errors/warnings
      -> Built `ISynergy.Framework.Mvvm`, `ISynergy.Framework.Mvvm.Tests`,
      `ISynergy.Framework.UI.WPF`, `ISynergy.Framework.UI.Blazor`,
      `ISynergy.Framework.UI.Maui` (net10.0-maccatalyst),
      `ISynergy.Framework.UI.Tests`, `ISynergy.Framework.UI.Maui.Mac.Tests`,
      and `Sample.Maui` (net10.0-maccatalyst) individually — all 0
      Warning(s)/0 Error(s). `ISynergy.Framework.UI.WinUI` fails to build on
      this Mac with a pre-existing, unrelated environment error
      (`XamlCompiler.exe: cannot execute binary file` — it's a Windows-only
      native tool; confirmed no C# (CSxxxx) compiler errors were produced).
      A full `dotnet build I-Synergy.Framework.slnx` also fails to *restore*
      on this Mac for an unrelated, pre-existing reason: one test project
      (`ISynergy.Framework.UI.Maui.Windows.Tests`) targets
      `net10.0-windows10.0.26100`, which isn't buildable/restorable on macOS.
      Neither failure is caused by or related to this change.
- [x] Manually verify in Sample.Maui: click "Throw Not Implemented error" ->
      app does NOT crash, friendly dialog appears (same as the existing
      NotImplementedException dialog behavior); same for the Asynchronous
      variant
      -> Built and launched `Sample.app` (net10.0-maccatalyst) directly, used
      `osascript`/System Events UI automation (accessibility) to open the
      flyout menu, navigate to "Custom Controls", and click both "Throw Not
      Implemented error" and "Throw Not Implemented error (Asynchronous)".
      Both clicks produced an `AXSheet` alert with title "Features" and
      message "This feature is not available at this moment in this
      version." (the existing `BaseExceptionHandlerService` NotImplementedException
      dialog), and the process (`ps -p <pid>`) remained alive and responsive
      after each click and after dismissing each dialog with "Ok" — no
      SIGABRT/crash, matching the async command's already-working behavior.
- [x] Update this file to DONE and move to `.ai/completed/`

Progress file: .ai/progress/relaycommand-sync-exception-crash.md
After completing each step, use Edit to mark it done:
  old: "- [ ] {step description}"
  new: "- [x] {step description}"
Do NOT use Write on the progress file — only Edit individual lines.

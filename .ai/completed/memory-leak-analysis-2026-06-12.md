# Memory Leak Analysis — 2026-06-12

**Status:** DONE

## Summary

Systematic analysis of ISynergy.Framework across 4 dimensions: event handler subscriptions, IDisposable chains, WeakReference usage, and async state machine captures.

## Findings Count

| Severity | Count |
|----------|-------|
| Critical | 10 |
| High | 9 |
| Medium | 9 |
| Low | 4 |

## Top 5 Highest-Impact Fixes

1. **Static event leaks in WPF telemetry** — `RenderingPerformanceMonitor` and `ApplicationMonitor` subscribe to static events without cleanup, preventing instance GC for app lifetime.
2. **RequestCancellationService.CancellationTokenSource disposal** — CTS objects removed from dictionary but never `.Dispose()`'d, leaking `ManualResetEvent` handles.
3. **ProcessInterface stream disposal** — `StreamWriter`/`TextReader` set to null without calling `.Dispose()`, leaking OS pipe handles.
4. **DialogService/BladeService async void closures** — Singletons subscribe to short-lived ViewModel events with `async void` lambdas that capture DI scope; if `Closed` doesn't fire, scope leaks.
5. **Property\<T\> not IDisposable** — `ValueChanged` subscribers can never be cleaned up by `ObservableClass.Dispose`.

## Positive Patterns

- MessengerService uses `WeakAction<T>` internally
- Complete weak event infrastructure (`WeakEventSource`, `WeakEventListener`, `WeakEventHandler`)
- BaseAsyncRelayCommand uses pooled `WeakReference` for continuations
- BladeView stores VM as `WeakReference<IViewModel>`
- ViewModel.Dispose properly unsubscribes from PropertyChanged, Messenger, cancellation events
- ItemObservableCollection cleans up CollectionChanged and per-item PropertyChanged

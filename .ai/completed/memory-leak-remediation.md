# Memory Leak Remediation
**Status:** DONE
**Started:** 2026-06-12
**Completed:** 2026-06-12
**Plan:** .ai/plans/memory-leak-remediation-plan.md

## Phases
- [x] Phase 1: Static Event Leaks + Handle Leaks (Critical - 5 fixes)
- [x] Phase 3: IDisposable Chain Fixes (High - 5 fixes)
- [x] Phase 4: Event Handler Cleanup (High - 3 fixes)
- [x] Phase 2: Async State Machine Fixes (Critical - 4 fixes)
- [x] Phase 5: Medium Severity Fixes (5 fixes—P5.1/P5.2 already done in P1)
- [x] Phase 6: Low Severity Fixes (4 fixes)

## Notes
Executed in order: 1 → 3 → 4 → 2 → 5 → 6
All 6 phases verified with `dotnet build` — 0 errors across all affected projects.

## Files Changed (by phase)

### Phase 1
- `src/ISynergy.Framework.UI.WPF/OpenTelemetry/RenderingPerformanceMonitor.cs` — Added IDisposable, static event unsubscribe
- `src/ISynergy.Framework.UI.WPF/OpenTelemetry/ApplicationMonitor.cs` — Named handler replaces anonymous lambda, _currentProcess disposal
- `src/ISynergy.Framework.UI.WinUI/Controls/ImageView/ImageView.xaml.cs` — Track/subscribe named ImageOpened handler
- `src/ISynergy.Framework.Core/Services/RequestCancellationService.cs` — CTS Dispose on RemoveRequest/CancelAllRequests
- `src/ISynergy.Framework.UI.WPF/Processes/ProcessInterface.cs` — Stream disposal, Process.Exited unsubscribe

### Phase 3
- `src/ISynergy.Framework.Core/Base/Property.cs` — Added IDisposable, clears ValueChanged event
- `src/ISynergy.Framework.UI.Maui/Controls/TabbedView.cs` — Removed ViewModel.Dispose() direct call, added scope-based DI
- `src/ISynergy.Framework.UI.Maui/Controls/View.cs` — Added IServiceScope for ViewModel resolution
- `src/ISynergy.Framework.Mvvm/Commands/Base/BaseAsyncRelayCommand.cs` — ProcessExit hooks CleanupStaticResources
- `src/ISynergy.Framework.Core/Base/ObservableClass.cs` — DisposeAsync calls Dispose(disposing: true)

### Phase 4
- `src/ISynergy.Framework.UI/ViewModels/LanguageViewModel.cs` — Named handler with Cleanup unsubscribe
- `src/ISynergy.Framework.UI.Maui/Application/Application.cs` — _clockTimer stopped in Dispose
- `src/ISynergy.Framework.Core/Locators/ServiceLocator.cs` — IServiceProvider disposed in Dispose

### Phase 2
- `src/ISynergy.Framework.Automations/BackgroundServices/ActionQueuingBackgroundService.cs` — async void → async Task
- `src/ISynergy.Framework.Mvvm/ViewModels/ViewModel.cs` — OnUserSignedIn async void → async Task
- `src/ISynergy.Framework.Core/Helpers/DelayTimer.cs` — async void → async Task with CancellationToken
- `src/ISynergy.Framework.UI/Services/Base/BaseBladeService.cs` — async void closures → SafeFireAndForget
- `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs` — async void closures → SafeFireAndForget
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` — async void closures → SafeFireAndForget + Window Dispose
- `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs` — async void closures → SafeFireAndForget

### Phase 5
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs` — Window disposal after ShowMessageAsync
- `src/ISynergy.Framework.UI.WinUI/Common/DeferredNavigation.cs` — try/catch around async void
- `src/ISynergy.Framework.UI.Maui/Controls/ImageBrowser/ImageBrowser.xaml.cs` — try/catch on async void handlers
- `src/ISynergy.Framework.UI.WinUI/Controls/ImageBrowser/ImageBrowser.xaml.cs` — try/catch on async void handlers

### Phase 6
- `src/ISynergy.Framework.UI.WPF/Services/BladeService.cs` — Loaded handler removed on timeout
- `src/ISynergy.Framework.UI.WinUI/Services/BladeService.cs` — Loaded handler removed on timeout
- `src/ISynergy.Framework.Core/Helpers/NullScope.cs` — Removed GC.SuppressFinalize
- `src/ISynergy.Framework.Core/Events/WeakEventSource{T}.cs` — Added Cleanup() method
- `src/ISynergy.Framework.UI/Services/Base/BaseNavigationService.cs` — Batch dead handler collection

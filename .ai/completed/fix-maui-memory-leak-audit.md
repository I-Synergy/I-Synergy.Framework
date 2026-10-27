# Fix MAUI Memory Leak Audit Findings

**Status:** DONE
Started: 2026-07-02
Completed: 2026-07-02

## Steps

- [x] Phase 1.1: Add [Lifetime(Lifetimes.Singleton)] to ShellView
- [x] Phase 1.2: Add CleanupPage + old page disposal to NavigationService.NavigateModalAsync
- [x] Phase 1.3: Add IServiceScopeFactory + scoped page/ViewModel resolution to NavigationService and NavigationExtensions (deferred item implemented)
- [x] Phase 2.1: Fix Windows CursorExtensions delegate accumulation (ConditionalWeakTable + fix line 45 bug)
- [x] Phase 2.2: Add RemoveCustomCursor to iOS CursorExtensions
- [x] Phase 2.3: Add RemoveCustomCursor to MacCatalyst CursorExtensions
- [x] Phase 2.4: Add RemoveCustomCursor no-op to Android CursorExtensions (cross-platform compilation)
- [x] Phase 2.5: Add Unloaded handler to CursorBehavior
- [x] Phase 3: Write NavigationService unit tests (CleanupPage guard clauses, signature verification, constructor update)
- [x] Phase 4: Write CursorBehavior unit tests (signature verification, subscription guard pattern, RemoveCustomCursor existence)
- [x] Phase 5: Build verification (framework all 4 platforms + sample + tests)

## Files Modified (10 source + 3 test)

### Source — Finding 1 (Root page replacement)
- `samples/Sample.Maui/Views/ShellView.xaml.cs` — [Lifetime(Singleton)]
- `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs` — CleanupPage, IServiceScopeFactory, scoped resolution, _modalScope, _pageScopes, Dispose
- `src/ISynergy.Framework.UI.Maui/Extensions/NavigationExtensions.cs` — new CreatePage(IServiceProvider, ...) overload

### Source — Finding 2 (Cursor callbacks)
- `src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs` — ConditionalWeakTable<UIElement, CursorHandlers>, RemoveCustomCursor
- `src/ISynergy.Framework.UI.Maui/Platforms/iOS/Extensions/CursorExtensions.cs` — RemoveCustomCursor
- `src/ISynergy.Framework.UI.Maui/Platforms/MacCatalyst/Extensions/CursorExtensions.cs` — RemoveCustomCursor
- `src/ISynergy.Framework.UI.Maui/Platforms/Android/Extensions/CursorExtensions.cs` — RemoveCustomCursor (no-op)
- `src/ISynergy.Framework.UI.Maui/Behaviors/CursorBehavior.cs` — Unloaded handler, PropertyChanged teardown

### Tests
- `tests/ISynergy.Framework.UI.Maui.Tests/Services/NavigationServiceCleanupTests.cs` — CleanupPage null/signature tests
- `tests/ISynergy.Framework.UI.Maui.Tests/Services/NavigationServiceRetentionCycleTests.cs` — idempotency/signature tests
- `tests/ISynergy.Framework.UI.Maui.Tests/Services/NavigationServiceExceptionHandlingTests.cs` — updated constructor call
- `tests/ISynergy.Framework.UI.Maui.Tests/Behaviors/CursorBehaviorTests.cs` — signature/guard pattern tests

## Build & Test Results

- Framework build: 0 warnings, 0 errors (Android, iOS, Mac Catalyst, Windows)
- Sample build: 0 warnings, 0 errors (Windows)
- Unit tests: 41 passed, 4 skipped, 0 failures

## Notes

Plan: `.ai/plans/wild-shimmying-llama.md`
Cross-platform analysis: WinUI and WPF are not affected (use BaseNavigationService with IServiceScopeFactory). Blazor not applicable.
Android CursorExtensions needed a no-op RemoveCustomCursor (not in original plan) because CursorBehavior.cs is shared across all target frameworks and needs the method to exist on every platform.

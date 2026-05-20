# Cross-Platform Quality Plan Implementation
**Status:** DONE
**Started:** 2026-05-20
**Plan:** .ai/plans/cross-platform-quality-plan.md

## Steps
- [x] C-13: Properties dictionary → ConcurrentDictionary
- [x] C-12: GetHashCode → RuntimeHelpers.GetHashCode
- [x] C-09: ObservableValidatedClass double-dispose guard
- [x] C-10: DisposeAsyncCore errorsChanged ordering
- [x] M-09: Redundant _disposed removed
- [x] M-11: PropertyChanged concurrent dispose hardening
- [x] C-01: LoadingView anonymous lambda → named method (WinUI + MAUI)
- [x] C-07: WaitForLoadingCompleteAsync race (WinUI + MAUI)
- [x] C-02: ThemeService _window → nullable + guard
- [x] H-11: ThemeService hard cast → TryGetValue
- [x] C-03: WinUI Exiting event → nullable + Dispose null
- [x] C-04: WinUI DialogService Closed self-removing handler
- [x] C-05: MAUI DialogService → IDisposable
- [x] C-06: MAUI cross-thread Task.Run removed
- [x] M-10: ValidateApplicationStateAsync removed
- [x] C-08 + M-08: WPF Application dispose fix + event cleanup
- [x] C-11: WPF SelectionWindow OnClosed unsubscribe
- [x] H-04 + H-05: ViewModelSelectable<TModel> + migrate 5 ViewModels
- [x] H-06: WizardViewModelBase<TModel> extraction
- [x] H-07: MAUI font registration deduplication
- [x] H-08: MAUI MigrationService → Scoped
- [x] H-09: WinUI BuildServiceProvider removed
- [x] H-10: backStackChanged null assignment removed
- [x] H-12: ClipboardService → single #if WINDOWS file
- [x] H-14: MAUI ThemeService → Scoped
- [x] H-15: WPF builder missing registrations added
- [x] H-17: WinUI DialogService dual-registration
- [x] H-01: BaseNavigationService extraction (resolves M-02, M-03)
- [x] H-02: BaseDialogService.ResolveGreeting()
- [x] H-03: DownloadFileService moved to shared UI
- [x] M-01: BusyService lock added
- [x] M-04: DispatcherService docs
- [x] M-05: MAUI NavigationService recursion fix
- [x] M-06: Window_Unloaded deleted
- [x] WPF test project created
- [x] Regression tests written (Core + Mvvm)

## Notes
- UpdateService intentionally left platform-specific (Store vs HTTP update mechanisms)
- H-13 resolved automatically by H-05 (ViewModelSummary inherits PropertyChanged from base)
- H-16 resolved automatically by H-08 + H-15
- M-02 resolved by H-01 (InitializeViewAndViewModel now called from navigation paths)
- M-03 resolved by H-01 (ServiceLocator replaced with injected _exceptionHandlerService)
- M-07 resolved by H-05 (Cleanup now single unsubscribe path in base)
- All 3 Core test projects pass. 1 pre-existing Core test failure (Dispose_ShouldClearErrors).

# MAUI + WinUI Single-Instance Activation Tests
Status: IN PROGRESS
Started: 2026-05-31

## Steps
- [x] Read source files (MauiWinUIApplicationExtensions, AppActivationExtensions, AppWindowExtensions, WindowsAppBuilderExtensions)
- [x] Read existing tests and project files for patterns
- [x] Write MAUI test: SingleInstanceActivationTests.cs
- [x] Write WinUI test: AppActivationExtensionsTests.cs
- [x] Verify compilation — both projects build 0 errors, 0 warnings
- [x] Run tests — 6 new pass, 4 correctly skipped (WinRT constraints)

## Notes
- Source classes use `internal static` methods; test classes access via reflection (consistent with NavigationServiceExceptionHandlingTests pattern)
- No InternalsVisibleTo exists; reflection is the correct approach given strong-name signing
- WinRT types (LaunchActivatedEventArgs, ProtocolActivatedEventArgs) may not be constructable in test context; use try/catch with Assert.Inconclusive as instructed

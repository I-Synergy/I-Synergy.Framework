# MAUI DevFlow Onboarding

**Status:** DONE
Started: 2026-10-25
Completed: 2026-10-25

## Steps
- [x] Add `Microsoft.Maui.DevFlow.Agent` to Central Package Management
- [x] Reference the package from `samples/Sample.Maui`
- [x] Register `AddMauiDevFlowAgent()` in `MauiProgram` (Debug only)
- [x] Allow the in-app server through the Mac Catalyst entitlements
- [x] Build the app and verify the agent package resolves

## Notes
- Standard MAUI app (no Blazor WebView, no GTK), so only the base Agent package applies.
- Package version matches the installed `maui` CLI: `0.1.0-preview.13.26512.5`.
- Verified: Debug `net10.0-windows10.0.26100.0` build succeeded; running the app logged
  `[Microsoft.Maui.DevFlow] Agent started on port 9223`;
  `maui devflow agent status -ap 9223` reported app `Maui Sample` (WinUI) and
  `maui devflow ui tree --depth 1 -ap 9223` returned the window node.
- The CLI broker is not running on this host, so `maui devflow diagnose` does not auto-discover
  the agent; pass `-ap 9223` until the broker is started.

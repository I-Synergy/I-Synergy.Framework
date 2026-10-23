# net11-multitarget
Status: DONE
Started: 2026-10-23

## Steps
- [x] Directory.Packages.props: split into net10 / net11 conditional groups
- [x] Convert 44 plain net10.0 csproj to conditional multi-target
- [x] WPF, WinUI, MAUI csproj twins
- [x] Verify build net10 (SDK 10) and net11 (temporary SDK 11 global.json)
- [x] Restore global.json, update docs/pipeline notes

## Notes

- MAUI net11 not buildable here: NETSDK1147, no maui-android workload for the 11.0.100-rc.2 band.
- CI (global.json on SDK 10) will not produce net11.0 assets until an SDK 11 is installed there.

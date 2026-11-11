# Use the self-contained AzureSignTool.Cli and drop the .NET 10 install

Status: DONE
Started: 2026-11-11
Completed: 2026-11-11

## Steps
- [x] Verify the published tool package is self-contained per RID
- [x] Pin `AzureSignTool.Cli` to `2026.11111.11200`
- [x] Remove the .NET 10 install from build and release
- [x] Document the packaging in the migration guide
- [x] Re-run the release validator

## Notes
- Release job signs with the self-contained tool; no .NET 10 runtime needed.

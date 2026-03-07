# Bulk Refactor Skill
1. Read the reference file the user provides to understand the target pattern
2. Use Grep to find ALL files matching the old pattern across the entire solution
3. Apply the transformation to every file found
4. After all edits, grep again to verify ZERO remaining instances of the old pattern (check string literals, logger calls, exception messages, and all other contexts)
5. If any remain, fix them
6. Delete any obsolete files that were replaced
7. Run `dotnet build` to verify no compilation errors

---
name: refactor
description: Bulk refactor specialist. Use for large-scale find-and-replace transformations, renaming patterns across the entire solution, or systematically migrating from one pattern to another. Trigger whenever the user says "rename X to Y everywhere", "change all X to Y", "migrate from X to Y", or needs to apply a transformation to many files at once — even if they don't use the word "refactor".
---

# Bulk Refactor Skill
1. Read the reference file the user provides to understand the target pattern
2. Use Grep to find ALL files matching the old pattern across the entire solution
3. Apply the transformation to every file found
4. After all edits, grep again to verify ZERO remaining instances of the old pattern (check string literals, logger calls, exception messages, and all other contexts)
5. If any remain, fix them
6. Delete any obsolete files that were replaced
7. Run `dotnet build` to verify no compilation errors

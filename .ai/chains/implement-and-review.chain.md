---
name: implement-and-review
description: Implement a feature then review until clean — worker implements, reviewer checks, worker applies fixes
---

## developer
phase: Implementation
label: Implement feature
as: implementation

Implement: {task}

Follow .ai/reference/critical-rules.md strictly. After every file edit, run `dotnet build --nologo --verbosity minimal` to verify.

## reviewer
phase: Review
label: Review implementation
reads: {outputs.implementation}

Review the implementation from the previous step. Check against:
- .ai/reference/critical-rules.md (all 13 rules)
- .ai/patterns/cqrs-patterns.md
- .ai/patterns/testing-patterns.md

Report issues with file paths and line numbers. Do NOT edit code — only report findings.

## developer
phase: Fixes
label: Apply review fixes
reads: {outputs.implementation}

Apply the review feedback from the previous step. Fix every issue reported. Run `dotnet build` and `dotnet test` after fixes.

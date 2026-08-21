---
name: scout-plan-implement
description: Full feature workflow — scout the codebase, plan implementation, implement, then review
---

## scout
phase: Context
label: Map codebase
as: context
model: anthropic/claude-haiku-4-5

Analyze the codebase for: {task}

Identify:
- Relevant existing files and patterns
- Entry points and dependencies
- Risks and edge cases
- Where new code should be placed

Write findings to a structured summary.

## planner
phase: Planning
label: Implementation plan
reads: {outputs.context}
model: anthropic/claude-sonnet-4

Create a concrete implementation plan based on the scout's analysis. Include:
- File-by-file list of changes (create, modify, delete)
- Class names, method signatures
- Which existing types to reuse (search first!)
- Acceptance criteria for each step

## developer
phase: Implementation
label: Implement
reads: {outputs.context}
model: anthropic/claude-haiku-4-5

Implement the plan from the previous step. Follow .ai/reference/critical-rules.md strictly.
After each file edit, run `dotnet build --nologo --verbosity minimal`.
After all changes, run `dotnet test`.

## reviewer
phase: Review
label: Review implementation
model: anthropic/claude-sonnet-4

Review all changes against the plan and .ai/reference/critical-rules.md.
Report issues with file paths and line numbers.

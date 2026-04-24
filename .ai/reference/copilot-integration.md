# GitHub Copilot Integration Guide

This template supports **both** Claude AI and GitHub Copilot working together in the same repository.

## Architecture

```
Template/
├── CLAUDE.md                          # Claude orchestration (agentic workflows)
├── .github/
│   ├── copilot-instructions.md        # Copilot workspace guidance
│   └── skills/                        # Full content copies for Copilot
├── .claude/
│   └── skills/                        # Thin wrappers for Claude Code dynamic injection
└── .ai/                               # Shared knowledge base (source of truth)
    ├── reference/                     # Standards (both systems)
    ├── patterns/                      # Patterns (both systems)
    ├── scripts/                       # sync-skills.py, migrate-to-ai.py
    ├── skills/                        # Skills source — edit here
    └── project/                       # Shared project context
```

### Skills Three-Tier Architecture

Skills have a single source of truth in `.ai/skills/` and are synced to two targets automatically:

| Layer | Path | Format | Who uses it |
|-------|------|--------|-------------|
| Source | `.ai/skills/<name>/SKILL.md` | Full content | Edit here |
| Claude Code | `.claude/skills/<name>/SKILL.md` | Thin wrapper: `!`cat .ai/skills/...`` | Claude Code executes at runtime |
| GitHub Copilot | `.github/skills/<name>/SKILL.md` | Full content copy | Copilot reads directly |

**Sync:** A PostToolUse hook fires `sync-skills.py --from-hook` on every `.ai/skills/` write. Run `/update-skills` (or `python .ai/scripts/sync-skills.py`) manually to sync all at once.

## System Comparison

| Feature | Claude AI | GitHub Copilot |
|---------|-----------|----------------|
| **Primary Use** | Agentic workflows, architecture, planning | Code completion, inline suggestions |
| **Config File** | `CLAUDE.md` | `.github/copilot-instructions.md` |
| **Context Management** | Session-based, progress tracking | Workspace-level, file-based |
| **Orchestration** | Multi-agent, structured tasks | Single completion context |
| **Skills** | 17 specialized agents (via `.claude/skills/` wrappers) | 17 skills in `.github/skills/` (full copies) |
| **Session State** | Persistent (session-context.md) | Ephemeral |

## Shared Resources

Both systems can reference:

**Standards & Rules:**
- `.ai/reference/critical-rules.md` - CQRS/DDD patterns
- `.ai/reference/forbidden-tech.md` - Technology standards
- `.ai/reference/glossary.md` - Terminology definitions
- `.ai/reference/naming-conventions.md` - Naming standards

**Patterns:**
- `.ai/patterns/cqrs-patterns.md` - CQRS implementation
- `.ai/patterns/api-patterns.md` - API endpoint patterns
- `.ai/patterns/testing-patterns.md` - Testing approaches
- `.ai/patterns/mvvm.md` - MVVM for Blazor/MAUI
- And 4 more...

**Templates:**
- `.ai/reference/templates/*.txt` - Code templates for copy-paste

## Recommended Workflow

### Use Claude AI For:
- ✅ Multi-file feature implementations
- ✅ Architecture decisions and planning
- ✅ Complex refactoring across multiple files
- ✅ Creating test suites (unit + integration + BDD)
- ✅ Code reviews with checklist validation
- ✅ Database migrations
- ✅ Security reviews
- ✅ Performance optimization
- ✅ Documentation generation

### Use GitHub Copilot For:
- ✅ Inline code completion while typing
- ✅ Single-method implementations
- ✅ Quick boilerplate generation
- ✅ Simple test cases
- ✅ Comment-to-code generation
- ✅ Code suggestions during active development

### Use Both Together:
1. **Claude:** Plan architecture, create structure, implement core handlers
2. **Copilot:** Fill in method bodies, add validation, write simple tests
3. **Claude:** Review, verify against checklists, create integration tests
4. **Copilot:** Add XML docs, refine edge cases
5. **Claude:** Final verification and PR creation

## Configuration Updates

### When You Update Patterns

If you modify `.ai/patterns/` or `.ai/reference/`:

1. **No action needed for Claude** - Loads on-demand automatically
2. **Copilot:** Restart VS Code or reload window for changes to take effect

### When You Add New Standards

1. Add to `.ai/reference/`
2. Update `CLAUDE.md` Work-Type Context Mapping (if needed)
3. Reference in `.github/copilot-instructions.md` (if applicable)

## Key Differences

### Claude's Advantages
- **Structured workflows** - Session management, progress tracking
- **Multi-agent orchestration** - 17 specialized skills
- **Long-running tasks** - Can work for hours with state preservation
- **Quality gates** - Pre-submission checklists
- **Architecture planning** - Can design before implementing

### Copilot's Advantages
- **Real-time suggestions** - As you type
- **Fast completions** - Immediate feedback
- **IDE integration** - Native VS Code experience
- **Context awareness** - Understands current file deeply
- **No setup** - Works immediately in editor

## Migration Path

If you're currently using only Copilot:

1. ✅ Keep `.github/copilot-instructions.md` (already created)
2. ✅ Copilot can now reference `.ai/reference/` and `.ai/patterns/`
3. ✅ No breaking changes to your workflow
4. ✅ Optionally add Claude for complex tasks

If you're currently using only Claude:

1. ✅ Add `.github/copilot-instructions.md` (already created)
2. ✅ Copilot can leverage your existing patterns
3. ✅ Claude workflows unchanged
4. ✅ Gain inline completion benefits

## Best Practices

### For Consistency
- ✅ Both systems reference same standards (`.ai/reference/`)
- ✅ Both systems follow same patterns (`.ai/patterns/`)
- ✅ Both systems use same token replacements
- ✅ Both systems respect forbidden technologies

### For Efficiency
- ✅ Use Claude for architecture and orchestration
- ✅ Use Copilot for rapid coding and completions
- ✅ Use Claude for final validation and PR creation
- ✅ Let both systems complement each other

### For Quality
- ✅ Claude enforces pre-submission checklist
- ✅ Copilot suggests standards-compliant code
- ✅ Both systems reference critical-rules.md
- ✅ Consistent patterns across all generated code

## Example Workflow

**Scenario:** Implement CRUD for new "Product" entity

```
1. Claude (via CLAUDE.md):
   - Read session context
   - Load .ai/patterns/cqrs-patterns.md
   - Load .ai/skills/dotnet-engineer.md
   - Create Product entity structure
   - Generate command/query/handler files
   - Create endpoint scaffolding

2. Copilot (via .github/copilot-instructions.md):
   - Fill in validation logic in commands
   - Add guard clauses to handlers
   - Generate XML documentation
   - Suggest error handling patterns

3. Claude:
   - Create comprehensive test suite
   - Verify against .ai/checklists/pre-submission.md
   - Update session-context.md with learnings
   - Create PR with structured description
```

## Validation

Run the Copilot integration test to verify everything is in sync:

```bash
python3 .ai/tests/validate-copilot.py
```

This checks:
1. `.github/copilot-instructions.md` exists with no stale `.claude/` references
2. `.github/skills/` directory exists
3. All `.github/skills/` entries are full content copies matching `.ai/skills/`
4. All `.claude/skills/` entries are thin wrappers (not full copies)

Run the full test suite to validate both Claude and Copilot setup:

```bash
bash .ai/tests/run-all-tests.sh
```

## Troubleshooting

### Copilot Not Respecting Standards
- ✅ Ensure `.github/copilot-instructions.md` exists
- ✅ Reload VS Code window
- ✅ Check that file references in copilot-instructions.md are correct

### Claude Not Using Copilot Patterns
- ✅ Claude doesn't read `.github/copilot-instructions.md` (by design)
- ✅ Both systems independently reference `.ai/` resources
- ✅ No cross-contamination by design

### Conflicting Suggestions
- ✅ Claude's rules in `CLAUDE.md` take precedence for agentic workflows
- ✅ Copilot's suggestions are just that - suggestions (you approve)
- ✅ Both should align because they reference same `.ai/reference/`

## Summary

**✅ YES** - You can use this template with Copilot!

- Both systems coexist peacefully
- Both reference the same standards and patterns
- Each system has its strengths
- No configuration conflicts
- Complementary workflows

The template is now **dual-mode**: Claude for orchestration, Copilot for completion. 🚀

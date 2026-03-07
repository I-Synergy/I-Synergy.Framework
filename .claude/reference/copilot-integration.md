# GitHub Copilot Integration Guide

This template supports **both** Claude AI and GitHub Copilot working together in the same repository.

## Architecture

```
Template/
├── CLAUDE.md                          # Claude orchestration (agentic workflows)
├── .github/copilot-instructions.md    # Copilot workspace guidance
└── .claude/                           # Shared knowledge base
    ├── reference/                     # Standards (both systems)
    ├── patterns/                      # Patterns (both systems)
    ├── skills/                        # Claude-only (agentic)
    └── project/                       # Shared project context
```

## System Comparison

| Feature | Claude AI | GitHub Copilot |
|---------|-----------|----------------|
| **Primary Use** | Agentic workflows, architecture, planning | Code completion, inline suggestions |
| **Config File** | `CLAUDE.md` | `.github/copilot-instructions.md` |
| **Context Management** | Session-based, progress tracking | Workspace-level, file-based |
| **Orchestration** | Multi-agent, structured tasks | Single completion context |
| **Skills** | 15 specialized agents | N/A |
| **Session State** | Persistent (session-context.md) | Ephemeral |

## Shared Resources

Both systems can reference:

**Standards & Rules:**
- `.claude/reference/critical-rules.md` - CQRS/DDD patterns
- `.claude/reference/forbidden-tech.md` - Technology standards
- `.claude/reference/glossary.md` - Terminology definitions
- `.claude/reference/naming-conventions.md` - Naming standards

**Patterns:**
- `.claude/patterns/cqrs-patterns.md` - CQRS implementation
- `.claude/patterns/api-patterns.md` - API endpoint patterns
- `.claude/patterns/testing-patterns.md` - Testing approaches
- `.claude/patterns/mvvm.md` - MVVM for Blazor/MAUI
- And 4 more...

**Templates:**
- `.claude/reference/templates/*.txt` - Code templates for copy-paste

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

If you modify `.claude/patterns/` or `.claude/reference/`:

1. **No action needed for Claude** - Loads on-demand automatically
2. **Copilot:** Restart VS Code or reload window for changes to take effect

### When You Add New Standards

1. Add to `.claude/reference/`
2. Update `CLAUDE.md` Work-Type Context Mapping (if needed)
3. Reference in `.github/copilot-instructions.md` (if applicable)

## Key Differences

### Claude's Advantages
- **Structured workflows** - Session management, progress tracking
- **Multi-agent orchestration** - 15 specialized skills
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
2. ✅ Copilot can now reference `.claude/reference/` and `.claude/patterns/`
3. ✅ No breaking changes to your workflow
4. ✅ Optionally add Claude for complex tasks

If you're currently using only Claude:

1. ✅ Add `.github/copilot-instructions.md` (already created)
2. ✅ Copilot can leverage your existing patterns
3. ✅ Claude workflows unchanged
4. ✅ Gain inline completion benefits

## Best Practices

### For Consistency
- ✅ Both systems reference same standards (`.claude/reference/`)
- ✅ Both systems follow same patterns (`.claude/patterns/`)
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
   - Load .claude/patterns/cqrs-patterns.md
   - Load .claude/skills/dotnet-engineer.md
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
   - Verify against .claude/checklists/pre-submission.md
   - Update session-context.md with learnings
   - Create PR with structured description
```

## Troubleshooting

### Copilot Not Respecting Standards
- ✅ Ensure `.github/copilot-instructions.md` exists
- ✅ Reload VS Code window
- ✅ Check that file references in copilot-instructions.md are correct

### Claude Not Using Copilot Patterns
- ✅ Claude doesn't read `.github/copilot-instructions.md` (by design)
- ✅ Both systems independently reference `.claude/` resources
- ✅ No cross-contamination by design

### Conflicting Suggestions
- ✅ Claude's rules in `CLAUDE.md` take precedence for agentic workflows
- ✅ Copilot's suggestions are just that - suggestions (you approve)
- ✅ Both should align because they reference same `.claude/reference/`

## Summary

**✅ YES** - You can use this template with Copilot!

- Both systems coexist peacefully
- Both reference the same standards and patterns
- Each system has its strengths
- No configuration conflicts
- Complementary workflows

The template is now **dual-mode**: Claude for orchestration, Copilot for completion. 🚀

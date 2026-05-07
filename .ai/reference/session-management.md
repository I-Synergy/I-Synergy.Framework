# Session Management

## Every Session

1. Read `.ai/session-context.md`
2. Read `.ai/completed/` (relevant tasks)
3. Work with real-time progress reporting
4. Write structured handoff before ending using `.ai/reference/templates/session-handoff.md.txt`

## Agent Delegation

- All agents have full repository access
- All agents report progress in real-time to `.ai/progress/`
- All agents use structured output (not free-form prose)

## Session Switching

Start new session when:
- Context nears the model's limit (session context grows large enough that the next task won't fit without compaction)
- Switching projects or domains
- Changing work types
- Session reached completion

## Session Handoff

Before ending: Use `.ai/reference/templates/session-handoff.md.txt` template. Write to `.ai/session-context.md`. Always set **Written By: Claude Code** in the handoff.

The session context is shared — GitHub Copilot reads and writes the same `.ai/session-context.md`. When picking up after a Copilot session:
- Read `.ai/session-context.md` for full context
- Check `.ai/progress/` for in-progress tasks
- Check `.ai/plans/` for approved plans not yet executed
- No re-setup needed — all context is in `.ai/`

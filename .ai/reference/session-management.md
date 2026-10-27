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

Before ending: Use `.ai/reference/templates/session-handoff.md.txt` template. Write to `.ai/session-context.md`. Always set **Written By: [assistant name]** in the handoff (e.g. "Claude Code", "Reasonix Code", or "Copilot").

The session context is shared — all assistants (Claude Code, GitHub Copilot, Reasonix Code) read and write the same `.ai/session-context.md`. When picking up after another assistant's session:
- Read `.ai/session-context.md` for full context
- Check `.ai/progress/` for in-progress tasks
- Check `.ai/plans/` for approved plans not yet executed
- No re-setup needed — all context is in `.ai/`

## Blocked Paths (Do Not Retry)

Record empirically-disproven approaches in `.ai/session-context.md` under the `Blocked Paths (Do Not Retry)` section, with a "Do NOT re-attempt" note and the reason:

```markdown
### {Approach name}

- **Do NOT re-attempt:** {what was tried}
- **Reason:** {why it is a dead end}
- **Use instead:** {the working alternative}
```

Why this matters: a dead end discovered in one session gets rediscovered — and its time re-wasted — in a later session unless it is written down. Future sessions read `session-context.md` first and skip the blocked path.

## Progress File Status

- `IN PROGRESS` — active work
- `DONE` — completed; move the file to `.ai/completed/`
- `SUPERSEDED` — the approach in this progress file was replaced by a later approach. Mark it (do not delete it) so future sessions know why the earlier approach was abandoned.

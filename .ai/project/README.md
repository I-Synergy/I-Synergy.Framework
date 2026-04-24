# Project Configuration Files

This directory contains **project-specific configuration files** that you must customize for your project.

## Clear Separation of Concerns

Each file has a **distinct, non-overlapping purpose**. No duplication between files.

| File | Purpose | Key Question |
|------|---------|--------------|
| [preferences.md](preferences.md) | Personal workflow & style | **HOW** do you prefer to work? |
| [tech-stack.md](tech-stack.md) | Technology choices & versions | **WHAT** technologies do you use? |
| [architecture.md](architecture.md) | System design & patterns | **HOW** is your system structured? |
| [domains.md](domains.md) | Business context & entities | **WHAT** are you building? |
| [session-context.md](session-context.md) | Session memory | Track decisions across sessions |

## File Details

### [preferences.md](preferences.md) - HOW You Work

**Contains:**
- Communication style (direct/verbose, timeline estimates)
- Problem-solving approach (shortcuts vs correct fixes)
- Autonomy level (when to ask vs proceed)
- Code style preferences (immutability, expression-bodied members)
- File organization preferences
- Review & quality gates

**Does NOT contain:**
- Technology choices → See tech-stack.md
- Architectural patterns → See architecture.md
- Business rules → See domains.md

---

### [tech-stack.md](tech-stack.md) - WHAT You Use

**Contains:**
- Runtime & language versions (.NET, C#)
- All frameworks & libraries with versions
- Forbidden technologies table
- Package management rules
- Cloud services
- Development tools

**Does NOT contain:**
- How to structure code → See architecture.md
- Personal preferences → See preferences.md
- Business logic → See domains.md

---

### [architecture.md](architecture.md) - HOW It's Structured

**Contains:**
- Architectural patterns (Clean Architecture, CQRS, DDD)
- Layer definitions and dependencies
- Project structure template
- CQRS implementation patterns with code examples
- Data access patterns
- Mapping strategy (manual — no library)
- Domain events structure
- Integration patterns
- Security architecture
- ADRs (Architectural Decision Records)

**Does NOT contain:**
- Specific technology versions → See tech-stack.md
- Business domain details → See domains.md
- Personal workflow → See preferences.md

---

### [domains.md](domains.md) - WHAT You're Building

**Contains:**
- Business domain definitions
- Entity lists per domain
- Business rules and validation
- Domain events (when and why)
- Cross-domain relationships
- Domain maturity tracking
- Ubiquitous language glossary

**Does NOT contain:**
- How to implement (architecture) → See architecture.md
- Technology choices → See tech-stack.md
- Personal preferences → See preferences.md

---

### [session-context.md](session-context.md) - Session Memory

**Contains:**
- Project overview
- Architectural decisions made
- User preferences in action
- Established patterns
- Blocked paths and learnings
- Session handoffs

**Purpose:** Claude's memory across sessions. Update at end of each session.

---

## Customization Workflow

### 1. Before Starting Development

Customize all four configuration files:

```bash
cd .ai/project/

# Edit each file with your project specifics
edit preferences.md      # Your workflow preferences
edit tech-stack.md       # Your technology choices
edit architecture.md     # Your architecture decisions
edit domains.md          # Your business domains
```

### 2. Replace Tokens

Throughout your customized files, replace:
- `{ApplicationName}` → Your application name
- `{Domain}` → Your domain names
- `{Entity}` → Your entity names

### 3. Initialize Session Context

```bash
cp session-context.md ../.ai/session-context.md
# Edit with initial project state
```

## Benefits of This Structure

✅ **No duplication** - Information appears in exactly one place
✅ **Easy to find** - Clear question → clear file
✅ **Maintainable** - Update one file, not multiple
✅ **Composable** - Files reference each other as needed
✅ **Focused** - Each file has single responsibility

## Cross-References

Each file includes cross-references at the bottom:

**preferences.md** → Points to tech-stack.md and architecture.md
**tech-stack.md** → Points to architecture.md and preferences.md
**architecture.md** → Points to tech-stack.md and domains.md
**domains.md** → Points to architecture.md and tech-stack.md

## Questions?

- **"Where do I document X?"** → See the table above
- **"Can I add custom sections?"** → Yes, but maintain separation
- **"What if something fits multiple files?"** → Put it where it belongs primarily, reference from others

---

**Remember:** Customize these files BEFORE starting development. They form the foundation for all Claude AI interactions with your project.

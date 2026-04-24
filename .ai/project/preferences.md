# Project Development Preferences (CUSTOMIZE THIS)

**Instructions:** Copy this template and customize for your specific project preferences.

**Purpose:** This file defines HOW you prefer to work - your personal workflow, communication style, and development approach.

## Working Relationship

### Communication Style
- **Formality level:** [Direct and concise / Detailed explanations / etc.]
- **Sycophancy:** [No sycophancy / Be encouraging / etc.]
- **Challenge my thinking:** [Yes / No / Only when critical]
- **Timeline estimates:** [Never include / Always include / Include when asked]
- **Git co-authorship:** [Don't add Claude / Add Claude / etc.]

### Problem-Solving Approach
- **Shortcuts vs. Correct fixes:** [Always correct fix / Quick fixes acceptable for prototypes / etc.]
- **Bug fixing:** [Fix immediately / Create ticket / Ask first]
- **Technical debt:** [Never acceptable / Acceptable with documentation / etc.]
- **Assumption handling:** [Always verify / Ask when uncertain / Proceed with reasonable assumptions]
- **"Good enough" threshold:** [Production-ready only / Prototype-acceptable / etc.]
- **Decision-making:** [User decides all tradeoffs / Agent autonomy for technical details / etc.]

## Development Workflow

### Autonomy Level
- **Agent access:** [Full repository access / Read-only / Specific directories only]
- **Execution model:** [Execute without asking / Ask before major changes / Ask for everything]
- **Progress tracking:** [Real-time automatic updates / Status on request / etc.]

### Documentation Preferences
- **XML docs:** [All public APIs / Public interfaces only / Optional]
- **Code comments:** [For complex logic only / Extensively / Minimal]
- **Architecture docs:** [Mermaid diagrams required / Text only / Optional]
- **README updates:** [With every feature / Major changes only / Manual]

## Code Style

### Language Features
- **Immutability:** [Records preferred / Classes with init / Mixed]
- **Null handling:** [Nullable reference types enabled / Optional / Disabled]
- **Expression-bodied members:** [Use extensively / Use sparingly / Avoid]
- **Pattern matching:** [Use modern C# features / Conservative approach]

### Organization
- **File organization:** [One class per file / Multiple if related / No preference]
- **Namespace structure:** [Match folder structure / Flat / Custom]
- **Using statements:** [Implicit global usings / Explicit / Minimal]

## Review & Quality Gates

### Pre-Submission Requirements
- **Code review:** [Automated checklist / Manual review / Both]
- **Build status:** [0 errors, 0 warnings / 0 errors only / Build succeeds]
- **Test results:** [All pass / Critical pass / Optional]
- **Documentation:** [Complete / API docs only / Optional]

### Quality Metrics
- **Code coverage:** [80%+ / 70%+ / 60%+]
- **Cyclomatic complexity:** [< 10 / < 15 / No limit]
- **Method length:** [< 50 lines / < 100 lines / No limit]

## Session Context Integration

**How this integrates with session-context.md:**

When starting a new session, Claude should:
1. Read this preferences file
2. Apply these preferences to all work
3. Document preference-based decisions in session-context.md
4. Never re-ask questions answered here

---

**Remember:** This file defines your personal working style. For technology choices, see [tech-stack.md](tech-stack.md). For system design, see [architecture.md](architecture.md).

# Claude Development Documentation

This documentation is split into three focused documents for better clarity and reduced context window pressure.

## Document Structure

### 1. **claude-core.md** (Primary - Start Here)
**Purpose:** Architecture, CQRS patterns, and critical rules  
**Size:** ~400 lines  
**When to read:** Always read this first, reference throughout development

**Contains:**
- ⚡ Critical Rules Quick Reference
- Common Gotchas & Pitfalls
- Architecture Overview
- Core Stack & Technologies
- CQRS Implementation
- Data Access Patterns
- Mapping Strategy (Mapster)
- Forbidden Technologies
- Domain Events
- Reference Implementation

**Read this when:**
- Starting any new development task
- Implementing CQRS handlers
- Choosing technologies
- Debugging common issues

---

### 2. **claude-standards.md** (Supporting)
**Purpose:** Code quality, testing, security, and conventions  
**Size:** ~500 lines  
**When to read:** Reference as needed for specific topics

**Contains:**
- Code Quality Principles (SOLID, Clean Code)
- Naming Conventions
- Async Patterns
- Error Handling & Resilience
- Logging Standards
- Testing Strategy (MSTest, Reqnroll)
- Security Best Practices
- Performance Guidelines
- Validation Patterns
- Progress Tracking

**Read this when:**
- Writing tests
- Implementing logging
- Handling errors
- Applying security measures
- Optimizing performance
- Following naming conventions

---

### 3. **claude-reference.md** (Templates & Examples)
**Purpose:** Complete code templates and implementation examples  
**Size:** ~800 lines  
**When to read:** Copy/paste starting point for new features

**Contains:**
- Complete Handler Templates (Create, Update, Delete, Query)
- Complete Endpoint Template
- Complete Mapping Configuration
- Complete Test Templates (MSTest, Reqnroll)
- Service Registration Template
- Pre-Submission Checklist
- Reference Implementation Paths

**Read this when:**
- Implementing a new domain/entity
- Creating handlers from scratch
- Setting up endpoints
- Writing tests
- Before submitting any work (use checklist)

---

## Quick Decision Tree

```
┌─────────────────────────────────────┐
│   What do you need?                 │
└─────────────────────────────────────┘
                │
                ├─ Starting any task → Delegate to agent with full access (MANDATORY)
                │
                ├─ Critical rules & gotchas → claude-core.md (⚡ section)
                │
                ├─ Agent workflow requirements → README or claude-core.md (Agent Workflow)
                │
                ├─ CQRS implementation guide → claude-core.md (CQRS section)
                │
                ├─ Which tech to use/avoid → claude-core.md (Stack & Forbidden)
                │
                ├─ Code templates to copy → claude-reference.md
                │
                ├─ Testing examples → claude-reference.md (Tests section)
                │
                ├─ Naming convention → claude-standards.md (Naming section)
                │
                ├─ How to log properly → claude-standards.md (Logging section)
                │
                ├─ Security guidance → claude-standards.md (Security section)
                │
                └─ Final checklist → claude-reference.md (Checklist section)
```

---

## Typical Workflow

### Starting a New Feature

1. **Delegate:** Assign task to agent with full repository access
2. **Agent starts:** Creates progress file immediately
3. **Agent reads:** claude-core.md → ⚡ Critical Rules + Common Gotchas
4. **Agent references:** claude-reference.md → Relevant template
5. **Agent implements:** Following patterns from template
6. **Agent reports:** Real-time progress updates automatically
7. **If more agents created:** ALL additional agents must have full access + real-time reporting
8. **Agent checks:** claude-standards.md → Logging/Testing as needed
9. **Agent verifies:** claude-reference.md → Pre-Submission Checklist
10. **Agent completes:** Moves progress file to completed folder

**Key:** Agent reports progress automatically throughout - you never ask for status.

### Agent Organization Patterns

Whether agents work in:
- **Sequential chain:** Agent 1 → Agent 2 → Agent 3
- **Parallel execution:** Agent 1 + Agent 2 + Agent 3 (simultaneously)
- **Hierarchical:** Main Agent → Sub-Agent → Helper Agent
- **Any other pattern:** All combinations

**ALL agents** must have full repository access and report real-time progress. No exceptions based on agent role or organization pattern.

---

1. **Check:** claude-core.md → Common Gotchas
2. **Check:** claude-core.md → Critical Rules
3. **Review:** claude-core.md → Data Access Pattern
4. **Verify:** Your code matches the Reference Implementation

### Writing Tests

1. **Reference:** claude-reference.md → Test Templates
2. **Follow:** claude-standards.md → Testing Strategy
3. **Copy:** Template and adapt for your entity
4. **Verify:** Tests follow MSTest/Reqnroll patterns

### Code Review

1. **Use:** claude-reference.md → Pre-Submission Checklist
2. **Verify:** All items checked
3. **Confirm:** No forbidden technologies used
4. **Ensure:** Progress file updated and moved to completed/

---

## Document Maintenance

### When to Update These Documents

- **New pattern discovered:** Add to claude-core.md → Common Gotchas
- **New technology adopted:** Update claude-core.md → Core Stack table
- **Template improvement:** Update claude-reference.md templates
- **Convention change:** Update claude-standards.md → Naming Conventions

### Keeping Documents in Sync

All three documents reference each other. When updating:
1. Update the primary location of the information
2. Verify cross-references in other documents still make sense
3. Test that examples still compile and work

---

## Template Tokens

Throughout all documents, placeholders are used as template tokens. Replace them when implementing:

| Token | Replace With | Example |
|-------|--------------|---------|
| `{ApplicationName}` | Your application name | `BudgetTracker` |
| `{Domain}` | Domain/bounded context name | `Budgets`, `Goals`, `Debts` |
| `{Entity}` | Entity name (PascalCase) | `Budget`, `Goal`, `Debt` |
| `{entity}` | Entity name (lowercase) | `budget`, `goal`, `debt` |
| `{entities}` | Entity plural (lowercase) | `budgets`, `goals`, `debts` |

**Example transformation:**
- **Template:** `{ApplicationName}.Domain.{Domain}.Features.{Entity}`
- **Actual:** `BudgetTracker.Domain.Budgets.Features.Budget`

---

## 🤖 Agent Workflow Requirements (CRITICAL)

### Hard Requirements - No Exceptions

**ALL work must be delegated to agents. This is non-negotiable.**

1. **Full Repository Access**
   - **EVERY agent** must have **complete access and all permissions** to the repository
   - No file or directory restrictions
   - Applies to ALL agents created for ANY reason:
     - First agent, second agent, third agent...
     - Sequential agents, parallel agents
     - Main agents, sub-agents, helper agents, specialist agents
     - **NO EXCEPTIONS - EVERY SINGLE AGENT**

2. **Real-Time Automatic Progress Reporting**
   - **EVERY agent** must report progress **automatically** as work progresses
   - **I should never need to ask** "what's the status?" or "how's it going?"
   - Progress updates are continuous, not just at task start/end
   - Progress files are updated in real-time with each completed step
   - This applies to ALL agents, regardless of their role or scope

3. **Universal Agent Requirements**
   - When ANY agent is created for ANY reason, it must:
     - Have full repository access
     - Report progress in real-time
     - Create/update progress files
     - Follow all workflow requirements
   - Agent organization doesn't matter (sequential, parallel, nested) - requirements are the same
   - No agent is exempt based on role, task size, or complexity

4. **Mandatory for ALL Tasks, ALL Agents**
   - Small fixes, large features, documentation updates - everything requires agents
   - Main agents, sub-agents, helper agents - all follow the same rules
   - No task is "too small" to skip this workflow
   - No agent is "too minor" to skip progress reporting
   - **Absolutely no exceptions, no shortcuts**

### Why This Matters

- **Transparency:** Real-time visibility into all work being done by ALL agents
- **Accountability:** Every agent is responsible for progress tracking
- **Continuity:** Any agent can pick up where others left off
- **Efficiency:** No time wasted asking for status updates
- **Completeness:** No work happens in the dark - all agents visible

**Failure to follow this workflow is a critical violation of working principles.**

---

## Benefits of This Structure

### Reduced Context Window Pressure
- Each document is focused and smaller
- Claude can load just what's needed for the task
- Less scrolling, faster reference

### Clear Separation of Concerns
- **Core:** What you must know (architecture, patterns)
- **Standards:** How to write quality code (testing, logging, security)
- **Reference:** Copy-paste templates and checklists

### Easier Maintenance
- Update templates without touching architecture docs
- Add new conventions without modifying CQRS patterns
- Clear ownership of content areas

### Better Learning Curve
- Start with Core (critical rules)
- Reference Standards as needed
- Use Reference as implementation guide

### Agent Workflow Enforcement
- **Real-time visibility** into ALL agent work through progress files
- **Automatic progress reporting** from EVERY agent eliminates status request overhead
- **Universal agent requirements** ensure consistency across all agent types and patterns
- **Mandatory delegation** creates accountability and transparency for ALL agents
- **Complete coverage** - main agents, sub-agents, helper agents, parallel agents - all visible

---

## File Locations

Recommended placement in your repository:

```
.claude/
├── README.md                    # This file - start here
├── claude-core.md              # Primary - Architecture & CQRS
├── claude-standards.md         # Supporting - Quality & Testing
├── claude-reference.md         # Templates & Checklists
├── progress/                   # ⚠️ CRITICAL - Active agent progress files
│   ├── [task-name]-progress.md
│   └── [agent-2-task]-progress.md
└── completed/                  # Completed task progress archive
    ├── [finished-task]-progress.md
    └── [old-task]-progress.md
```

**Critical:** The `progress/` and `completed/` directories are mandatory for agent workflow tracking. Every agent must create and update progress files here in real-time.

---

## Version History

- **v2.0** - Split into three focused documents
- **v1.0** - Single monolithic claude.md file

---

**Next Steps:**
1. Place documents in `.claude/` directory of your repository
2. Test with a small task to verify the new structure works
3. Share with team and gather feedback
4. Update template tokens when implementing features
5. Iterate and update patterns as they evolve

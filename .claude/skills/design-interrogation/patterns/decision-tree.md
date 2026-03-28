# Decision Tree Mapping

How to map and traverse the decision tree during a grill-me session.

---

## What Is the Decision Tree

Every plan or design is a tree of decisions where some decisions depend on others.
The tree is not known upfront — it emerges as the conversation progresses.

```
Root: What are we building?
├── Event-driven or direct calls?
│   ├── [If event-driven] Which broker?
│   │   ├── Azure Service Bus → Topic or Queue?
│   │   └── RabbitMQ → Exchange type?
│   ├── [If event-driven] Schema format?
│   └── [If direct calls] Sync or async (Fire-and-forget)?
├── Which database?
│   ├── SQL → ORM or Dapper?
│   └── NoSQL → Document or Key-Value?
└── Deployment target?
    ├── Azure Container Apps → Scaling rules?
    └── On-premise → HA strategy?
```

Branches at the same level are siblings. Children depend on their parent's decision.
Resolve children before moving to siblings.

---

## Tracking Branch Status

Maintain a mental model throughout the session. You do not need to write this out
explicitly unless the tree is large — but you must be aware of it.

| Status | Meaning |
|--------|---------|
| **Open** | Decision not yet reached |
| **Resolved** | Decision made, no open sub-questions |
| **Deferred** | User consciously chose to decide later |
| **Blocked** | Cannot decide without external input |

When a branch is **Blocked**, name the blocker explicitly:
> "We can't decide the broker until you know whether the cloud team supports
> Azure Service Bus in the target subscription. I'll mark this as Blocked for now."

---

## Detecting Dependencies

A decision has a dependency when:
- The answer to one question determines which sub-questions are relevant
- Changing the parent decision would make the child decision irrelevant
- Two questions share the same underlying variable

**Example of a dependency:**
> "Should we use event sourcing?" → if Yes → "Which event store?" is now relevant
> If No → event store question is irrelevant

**Example of independent questions (siblings, not parent-child):**
> "Which database?" and "Which auth provider?" are independent — either can be
> asked first, and the answer to one does not change the other.

---

## Handling "It Depends" Branches

When the user says "it depends", the branch has not been resolved — it has been
turned into a conditional. Treat this as two sub-branches:

```
Decision: Caching strategy?
User: "It depends on whether we expect heavy read load."

→ Branch A: Heavy read load → Redis distributed cache
→ Branch B: Low read load   → In-memory cache per instance

Follow-up: "What's the expected read volume at peak?
  My recommendation: Assume heavy (design for Redis), so we don't have to revisit
  this decision later. Premature optimization in the other direction is harder to
  undo."
```

---

## Large Trees — Progressive Disclosure

For complex plans, do not try to map the entire tree upfront. Reveal the tree
progressively:

1. Start with the root question (what are we building?)
2. After each answer, determine the next most important sub-question
3. Only surface sibling topics once the current branch is fully resolved

This keeps the conversation focused and prevents the user from feeling overwhelmed.

---

## When the Tree Is Complete

The tree is complete when:
- All top-level dimensions (see SKILL.md) are in Resolved, Deferred, or Skipped status
- No branch has open children

At this point, deliver the closing summary (see `checklists/dod.md`).

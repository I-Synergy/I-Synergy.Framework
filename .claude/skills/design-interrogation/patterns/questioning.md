# Questioning Patterns

How to formulate questions and recommendations during a grill-me session.

---

## Question Format

Every question must follow this exact structure:

```
---

**Question N — [Topic]**

[The question, clearly stated in one or two sentences.]

**My recommendation:** [Concrete recommendation + reasoning. Cite codebase patterns
or industry best practices where applicable.]

---
```

Never omit the recommendation. The point of grill-me is not just to ask — it is to
think alongside the user and surface the best path forward.

---

## What Makes a Good Question

### Specific and answerable
Bad: "Have you thought about scalability?"
Good: "Should this service scale horizontally behind a load balancer, or is a single
instance with vertical scaling sufficient for the projected load?"

### One decision per question
Bad: "What database will you use and how will you handle migrations?"
Good: "Which database engine will back this service — PostgreSQL, SQL Server, or
something else?" (Migrations come in a follow-up question.)

### Dependency-aware
Ask about foundations before asking about details that depend on them.
Example: Resolve the "event-driven vs. direct call" question before asking about
the event schema — the schema question is meaningless without knowing if events exist.

---

## Formulating the Recommendation

A recommendation is not a guess — it is your informed position. Structure it as:

1. **The choice** — name the concrete option you recommend
2. **The reasoning** — one or two sentences explaining why
3. **The trade-off** — what you give up with this choice (optional but valuable)

**Example:**

> **My recommendation:** Use PostgreSQL with EF Core. It aligns with the existing
> stack (see `Directory.Packages.props`), avoids a new operational dependency, and
> the team already has query optimization experience here. The trade-off is that you
> lose native document-store flexibility — but the domain model does not appear to
> need it.

---

## Handling Vague Answers

If the user gives a vague or non-committal answer, do not move on. Acknowledge the
answer and probe for specifics:

> "I hear you — 'it depends' is a valid starting point. What specific condition
> would push you toward option A vs. option B?"

Common vague answers and how to handle them:

| Vague answer | How to probe |
|--------------|--------------|
| "It depends" | "What does it depend on? Let's name the condition." |
| "We'll figure it out later" | "Is this a conscious deferral or an unknown? If deferred, what would need to be true before deciding?" |
| "The usual approach" | "In this codebase, 'the usual approach' for X is Y — is that what you mean?" |
| "Whatever is standard" | "Standard in this project means Z — confirm you want that?" |

---

## Depth-First vs. Breadth-First

Always work **depth-first**. When a decision opens sub-questions, resolve those
sub-questions before moving to the next top-level topic.

**Example — depth-first (correct):**
1. Question: "Event-driven or direct API calls?"
2. User: "Event-driven."
3. Next question: "Which broker — Azure Service Bus or RabbitMQ?" ← sub-question
4. Resolve broker, schema, retry policy...
5. Only then move to: "Data Flow — how are read models updated?"

**Example — breadth-first (incorrect):**
1. Question: "Event-driven or direct API calls?"
2. User: "Event-driven."
3. Next question: "What database?" ← skipped sub-questions, jumped to sibling topic

**Exception 1 — Phase 0 Anchor:**
The four anchor questions (A1–A4) are always presented together as a single opening
round. This is the only place where multiple questions appear at once outside of
parallel BC interrogation. After Phase 0, revert strictly to one question at a time.

**Exception 2 — Parallel BC Interrogation:**
When Phase 0 identifies 2+ bounded contexts, each BC runs its own depth-first thread
(one question at a time within the BC). The coordination agent may present one question
per BC per round, clearly labelled by BC name. This is not a violation of the
depth-first rule — each BC's thread remains depth-first internally.

---

## When to Stop Asking

Stop asking about a dimension when:
- The decision is fully resolved (choice made, implications understood)
- The user explicitly defers ("we'll decide later — move on")
- The codebase already answers the question

Do not ask questions just to fill the session. The goal is shared understanding,
not exhaustive coverage for its own sake.

---

## Codebase-First Rule

Before asking the user about something that might already be answered in the code,
check the codebase. Use Read, Grep, or Glob to inspect relevant files.

Announce what you found before moving on:

> I checked `Directory.Packages.props` — the project already uses Azure Service Bus
> 7.x. I'll assume that's the broker unless you tell me otherwise.

This prevents the user from having to answer questions whose answers are already
embedded in their own code.

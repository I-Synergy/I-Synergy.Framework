---
name: ubiquitous-language
description: Captures and maintains the domain vocabulary (ubiquitous language) for a bounded context. Use when building a domain model, running a design interrogation session, or whenever domain terms need to be defined and documented. Trigger whenever the user says "define domain terms", "glossary", "ubiquitous language", or when terms emerge during design sessions. Outputs a structured glossary to UBIQUITOUS_LANGUAGE.md.
---

# Ubiquitous Language

Captures domain terms as they emerge during design sessions and produces a structured, canonical vocabulary for the bounded context.

## Output Format

All terms go to `UBIQUITOUS_LANGUAGE.md` at the project root.

```markdown
## [DRAFT] Ubiquitous Language — {SolutionName}

| Term | Definition | Bounded Context | Notes |
|------|-----------|-----------------|-------|
| {Term} | {Definition} | {BC} | [DRAFT] |
```

Mark the file `[DRAFT]` until finalization. Remove all `[DRAFT]` markers only during the finalization pass.

## Mode 0 — Extraction from Documents

Use when existing documents (PRD, business case, functional spec) are provided.

1. Read all input documents
2. Extract every domain noun, verb, and concept used with specific meaning
3. For each term, draft a definition based on how it is used in the documents
4. Identify the bounded context each term belongs to (if known)
5. Log all extracted terms as `[DRAFT]` to `UBIQUITOUS_LANGUAGE.md`
6. Mark gaps — terms used but not defined — as `[OPEN]` in the Notes column
7. Announce: "Extracted N terms — M gaps marked [OPEN]."

## Mode 1 — Progressive Capture (During Sessions)

Use during a design interrogation or domain modeling session, as terms emerge.

When a domain term is agreed upon:
1. Immediately append it to `UBIQUITOUS_LANGUAGE.md` (create the file if it does not exist, with `[DRAFT]` header)
2. Include the bounded context and any disambiguation notes
3. If the term conflicts with another term, note the conflict in the Notes column
4. Announce: "📝 Term logged to `UBIQUITOUS_LANGUAGE.md`."

## Finalization Pass

Run when the interrogation is complete and all decisions are made.

1. Read the full `UBIQUITOUS_LANGUAGE.md` draft
2. Resolve all `[OPEN]` gaps — ask the user or infer from context
3. Eliminate synonyms — choose one canonical term per concept, note the synonym
4. Check consistency — ensure each term is used with the same meaning across all bounded contexts
5. Remove all `[DRAFT]` markers
6. Write the finalized file

Announce:
> ✅ `UBIQUITOUS_LANGUAGE.md` finalized — N terms defined, M ambiguities resolved.

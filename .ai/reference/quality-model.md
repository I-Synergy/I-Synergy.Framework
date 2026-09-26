# Quality Model — ISO/IEC 25010:2023

> ISO/IEC 25010 gives this template the vocabulary for naming what a review is about. Every finding
> a reviewer reports is attributed to one of its nine product quality characteristics and carries one
> severity from the taxonomy below, so a review becomes citable rather than a list of opinions. This
> document is the source of truth read by `.ai/agents/reviewer.md`, the `code-reviewer` skill,
> Dimension 7 of the `design-interrogation` skill, and the Quality Characteristics gate in
> `.ai/checklists/pre-submission.md`.

## Purpose and Scope

**25010 is an evaluation model, not a certifiable standard, and nothing here measures anything.** It
structures *vocabulary and attribution*: it gives nine agreed names for the kinds of quality a change
can affect, so that "this is a security problem" and "this is a reliability problem" stop being the
same sentence, and so that a finding can be cited by name in a review, a plan or a retrospective. It
does not define how to measure a characteristic, and it does not define how to evaluate a system
against one.

Two companion standards cover what this document deliberately does not, and neither is implemented
here:

| Standard | Covers | Status in this template |
|---|---|---|
| ISO/IEC 25023 | Measurement of product quality — the metrics behind a characteristic | **Out of scope** — no metric is defined, collected or reported |
| ISO/IEC 25040 | Evaluation process — how a system is evaluated against the model | **Out of scope** — no evaluation is conducted |

The honest description of this file is therefore: a **review vocabulary**. A project using it can say
*which* characteristic a finding concerns and *how much it matters*; it cannot say its security "is
25010-compliant", because no such claim is meaningful. Measurement and evaluation are named here so
that their absence is visible, not so that this file can be read as a substitute for them.

**Edition.** This document follows **ISO/IEC 25010:2023**, which replaced the 2011 edition. The
characteristic set changed, and the changes are load-bearing for anyone reading an older review:

| Change | 2011 | 2023 |
|---|---|---|
| Renamed | `usability` | **`interaction capability`** |
| Renamed | `portability` | **`flexibility`** |
| Renamed | `maturity` (under reliability) | **`faultlessness`** |
| Added | — | **`safety`** (a ninth characteristic) |
| Added | — | **`resistance`** (under security) |

A project whose review vocabulary still says "usability" or "portability" is using the 2011 model.
That is a naming drift rather than a defect, but it makes reviews non-comparable across projects,
which is the one thing an agreed vocabulary is for.

## The Nine Characteristics

Each row carries the characteristic's name as the 2023 edition spells it, what it means for a
codebase built on this template's stack, and the question a reviewer asks of a change. The question is
the operative content: it is what turns a characteristic into a review, and it is deliberately
answerable from a diff and its surrounding code.

| # | Characteristic | What it means for this stack | The review question |
|---|---|---|---|
| 1 | **Functional suitability** | The handler does what the slice blueprint and its acceptance criteria say — including the edge cases those criteria name — and nothing in the design is unaccounted for | Does this change do what its traced acceptance criteria say, no more and no less? |
| 2 | **Performance efficiency** | Query shape, result-set bounds, allocations, and whether work sits on a request path that should not carry it | Is there an unbounded query, an N+1, or a hot-path cost the design did not account for? |
| 3 | **Compatibility** | Types shared through `{ApplicationName}.Common`, the EF Core schema, API response shapes, the Kiota client, and anything another project links against | Does this break, or force a change in, something outside the slice it belongs to? |
| 4 | **Interaction capability** | Blazor and MAUI surfaces, and — for a headless API — the status codes and error bodies a client has to interpret | Can the caller tell what happened and what to do next without reading the source? |
| 5 | **Reliability** | Failure paths: null checks, cancellation, retry and idempotency, partial writes, disposal, transaction boundaries | What happens when this fails, and is calling it twice safe? |
| 6 | **Security** | The exposure surface: new endpoints, serialization, logging, secrets, authorization | Does this widen who can do something, or see something? |
| 7 | **Maintainability** | File and type organization, layer boundaries, coupling, naming, and whether the changed code can be tested | Will the next change to this behaviour be local, or does it cut across layers? |
| 8 | **Flexibility** | Configuration over code, provider coupling, statelessness, and how replaceable the infrastructure pieces are | Can this be scaled, replaced or re-hosted without editing it? |
| 9 | **Safety** | Whether the feature can cause harm outside the system — to a person, a device, or data that cannot be recreated — and whether it fails safe | Can a wrong answer or an outage here hurt something that cannot be undone? |

**Characteristics that do not apply are not reported.** A pure refactor of a query handler may
implicate only maintainability; a Markdown change implicates none. Nothing in this model requires an
answer for all nine.

## Subcharacteristics

**Enumerated only where sourced.** 25010 decomposes each characteristic further. The lists below are
given only for the characteristics where the 2023 decomposition is settled enough to repeat. For the
other two, the characteristic is named and its subcharacteristics are **not enumerated here** — a list
assembled from plausibility would be indistinguishable in this file from one taken from the standard,
which is precisely the claim this template refuses to make.

The **Basis** column says where a list comes from. `2011 list, no 2023 change reported` means the list
is the well-established decomposition of the characteristic in the 2011 edition and this document
found no report that the 2023 revision altered it. It is not a transcription of the 2023 text.

| Characteristic | Subcharacteristics | Basis |
|---|---|---|
| Functional suitability | functional completeness, functional correctness, functional appropriateness | 2011 list, no 2023 change reported |
| Performance efficiency | time behaviour, resource utilization, capacity | 2011 list, no 2023 change reported |
| Compatibility | co-existence, interoperability | 2011 list, no 2023 change reported |
| Interaction capability | **not enumerated here** | Decomposition changed in 2023; available summaries disagree |
| Reliability | faultlessness, availability, fault tolerance, recoverability | `maturity` renamed to `faultlessness` in 2023; the rest unchanged |
| Security | confidentiality, integrity, non-repudiation, accountability, authenticity, resistance | `resistance` added in 2023; the rest unchanged |
| Maintainability | modularity, reusability, analysability, modifiability, testability | 2011 list, no 2023 change reported |
| Flexibility | adaptability, scalability, installability, replaceability | Replaced `portability` in 2023; `scalability` is the addition |
| Safety | **not enumerated here** | New in 2023; available summaries disagree |

**Using a characteristic marked "not enumerated here".** The review question in the table above still
applies in full — attribution does not depend on the decomposition. What is missing is only the finer
level of naming: a finding is reported against *interaction capability* or *safety*, never against a
subcharacteristic this document cannot source.

**The product quality model only.** 25010 also defines a quality-in-use model describing a system in
use rather than the product itself. This document covers the product quality model and does not
enumerate the quality-in-use model.

## Severity Taxonomy

Three levels, defined once here and referenced everywhere else. **Confidence is the entry bar;
severity is the label on what is reported** — they are different axes, and confidence is never a
severity.

| Severity | Means | Effect on the change |
|---|---|---|
| **Blocker** | Correctness, security, a public contract or the build is broken by this change | Cannot merge as it stands |
| **Major** | A rule in `.ai/reference/critical-rules.md` is violated, or a defect is latent under conditions the design already anticipates — an unbounded query, a missing cancellation token, an unhandled failure path, a domain entity exposed in a response | Fix before merge, or record it as a follow-up the user explicitly accepted |
| **Minor** | High-confidence and low-impact: correct today, measurably more expensive later | Non-blocking; recorded so that it is citable |

**Minor is not a licence to report nits.** The bar for every severity is unchanged: a finding is
high-confidence and a reviewer would defend it as a defect, or it is not reported. A style preference,
a naming taste, or a suggestion the reviewer would not act on themselves is not a Minor finding — it
is not a finding. The `reviewer` agent's instruction not to flag style preferences and minor nits
survives this taxonomy, and **the taxonomy adds attribution and severity, never volume**.

**Every finding carries exactly one characteristic and one severity**, written as:

```text
[Major] Performance efficiency — GetOrdersQueryHandler materializes the whole Orders table
(OrdersQueryHandler.cs:31); the table grows with every order. Add a filter or paging.
```

A characteristic with nothing to report is simply absent from the review. `Not applicable` is a
legitimate outcome, recorded once per characteristic when the quality attributes are agreed (see the
`design-interrogation` Dimension 7 question set) — not repeated as an empty heading in every review.

## Consumers

| Consumer | Uses this document for |
|---|---|
| `.ai/agents/reviewer.md` | Attributing every finding to one characteristic and one severity |
| `.ai/skills/code-reviewer/SKILL.md` | Mapping its Security and Performance checklists, and naming the four characteristics it has no checklist for |
| `.ai/skills/design-interrogation/SKILL.md` | Dimension 7 — the question set that brings quality attributes into a project |
| `.ai/checklists/pre-submission.md` | The Quality Characteristics gate |

The vocabulary lives here and the four consumers reference it. A characteristic is renamed here or
nowhere — two names for one characteristic is the drift this arrangement exists to prevent.

## What This Does Not Do

- **It does not certify anything.** It is an evaluation model, and this template asserts no conformity
  with it or with any other standard.
- **It does not measure.** No metric, threshold or report is defined here; that is ISO/IEC 25023, and
  it is out of scope.
- **It does not evaluate.** It does not describe how a system is assessed against the model; that is
  ISO/IEC 25040, and it is out of scope.
- **It does not enumerate subcharacteristics for interaction capability or safety**, for the reason
  given above.
- **It does not cover quality in use**, only the product quality model.
- **It does not make any characteristic mandatory to report on.** A clean review stays clean; the
  rubric is a naming scheme, not a coverage quota.
- **It does not turn quality attributes into acceptance criteria.** Performance, security and
  reliability targets are not numbered requirements — `.ai/reference/traceability.md` records that
  decision and the reason for it.
- **It does not replace the critical rules.** A rule violation is reported whether or not the reviewer
  can agree on which characteristic it belongs to.

## Quick Checklist

- [ ] Every review finding names one characteristic, spelled as the 2023 edition spells it
- [ ] Every review finding carries one severity: `Blocker`, `Major` or `Minor` — no others
- [ ] Every finding clears the high-confidence bar; no style preferences and no nits, at any severity
- [ ] A characteristic the change does not implicate is absent from the review, not present and empty
- [ ] A characteristic marked *not enumerated here* is still attributed by name when it applies
- [ ] No claim of certification, conformity or measurement is made from this vocabulary

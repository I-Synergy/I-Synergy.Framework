# Grill Me — Violations

Common mistakes during a grill-me session — and how to correct them.

---

## Violation 1: Asking Multiple Questions at Once

**What it looks like:**
```
What database will you use? How will you handle migrations? And what's your
caching strategy? Also, have you thought about failure modes?
```

**Why it's wrong:**
The user cannot give focused answers to four questions simultaneously. The session
becomes a form-filling exercise rather than a structured exploration of dependencies.
Important sub-branches get glossed over because the user is pattern-matching to a list.

**Correct approach:**
Ask exactly one question per turn. Follow the answer before asking the next question.
The session should feel like a conversation, not a questionnaire.

---

## Violation 2: Not Providing a Recommendation

**What it looks like:**
```
**Question 3 — Deployment Target**

Where will this service be deployed?
```

**Why it's wrong:**
The user came here to stress-test their thinking with a knowledgeable counterpart.
A question without a recommendation forces them to think in a vacuum. The recommendation
is often the most valuable part — it surfaces assumptions, trade-offs, and codebase
context they may not have considered.

**Correct approach:**
Every question must include a concrete recommendation with reasoning:
```
**My recommendation:** Azure Container Apps. The existing services in this solution
(see `Rfh.Laboratory.AppHost`) all deploy to ACA, and the Aspire integration already
handles the container manifests. Diverging to a different platform without a specific
reason adds operational overhead.
```

---

## Violation 3: Asking About Something Already in the Codebase

**What it looks like:**
```
**Question 5 — Message Broker**

Which broker should we use for domain events — Azure Service Bus or RabbitMQ?
```

*(Meanwhile, `Directory.Packages.props` already references `Azure.Messaging.ServiceBus 7.18.2`)*

**Why it's wrong:**
Asking the user to make a decision that is already made wastes their time and
signals that the codebase was not consulted. It can also lead to inconsistency if
the user says "RabbitMQ" without realizing Service Bus is already in use.

**Correct approach:**
Check the codebase before asking. Announce what you found:
```
I checked `Directory.Packages.props` — the project already has Azure.Messaging.ServiceBus
7.18.2 referenced. I'll treat Service Bus as the assumed broker. Does that align with
what you have in mind?
```

---

## Violation 4: Accepting Vague Answers and Moving On

**What it looks like:**
```
User: "We'll handle failure with retries and stuff."
Claude: "Got it. Next question — what's the deployment strategy?"
```

**Why it's wrong:**
"Retries and stuff" is not a decision — it is a non-answer. Moving on leaves a
branch unresolved and allows the user to ship without actually thinking through
failure modes. The whole point of the session is to surface these gaps.

**Correct approach:**
Acknowledge and probe:
```
"Retries make sense — let's be specific. Retry on transient failures only (network,
timeout) or also on business errors? And is there a dead-letter queue for messages
that exhaust retries, or do we alert-and-discard?

My recommendation: Retry transient failures with exponential backoff (3 attempts),
dead-letter after exhaustion. Business errors should NOT be retried — they need
explicit handling."
```

---

## Violation 5: Closing Without a Summary

**What it looks like:**
```
Great, I think we've covered everything. Good luck with the implementation!
```

**Why it's wrong:**
Without a summary, the decisions made during the session exist only in the chat
history. The user has no structured record of what was decided, what was deferred,
and what still needs resolution. The session's value is lost the moment the chat closes.

**Correct approach:**
Always close with a structured summary using the template in SKILL.md:
```
## Interrogation Summary

### Decisions Made
- Broker: Azure Service Bus, Queue topology, dead-letter enabled
- Database: PostgreSQL with EF Core, schema-per-domain
- Deployment: Azure Container Apps via Aspire

### Deferred Decisions
- Caching: Deferred — requires load test results before choosing strategy

### Recommended Next Steps
1. Spike the event schema contract with the consuming service team
2. Add the dead-letter handler to the backlog before launch
```

---

## Violation 6: Skipping a Dimension Without Stating Why

**What it looks like:**
The session covers Purpose, Domain Model, and Data Flow — then jumps straight to
Deployment without mentioning Failure Modes or Non-Functionals.

**Why it's wrong:**
Failure Modes and Non-Functionals are the dimensions most likely to be missed in
early design. Skipping them silently means the plan has unexamined gaps that will
surface in production.

**Correct approach:**
If a dimension is truly not relevant, say so explicitly:
```
"Failure Modes: For a read-only reporting service with no side effects, failure
recovery is straightforward — we just retry the request. I'll skip a deep dive here
unless you see complexity I'm missing."
```

---

## Violation 7: Treating Deferral as Resolution

**What it looks like:**
```
## Interrogation Summary

### Decisions Made
- Caching strategy: TBD
- Auth provider: TBD
- Rollout: TBD
```

**Why it's wrong:**
Listing "TBD" under Decisions Made misrepresents the outcome. These are not decisions
— they are deferrals. Presenting them as decisions gives the user false confidence
that the design is further along than it is.

**Correct approach:**
List TBDs under "Deferred Decisions" with an explicit reason and impact:
```
### Deferred Decisions
- Caching strategy: Deferred — requires load test data. Risk: service may
  experience slow responses under peak load if this is not addressed before launch.
```

---

## Violation 8: SQL DDL in Architecture Document

**What it looks like:**
```markdown
### 4.3 Projections

```sql
CREATE TABLE sample_mgmt.sample_list_projection (
    tenant_id VARCHAR(200) NOT NULL,
    sample_id UUID NOT NULL,
    barcode VARCHAR(50) NOT NULL,
    PRIMARY KEY (tenant_id, sample_id)
);
CREATE INDEX ix_sample_list_status ON sample_mgmt.sample_list_projection(tenant_id, status);
```
```

**Why it's wrong:**
SQL DDL in an architecture document creates a dangerous mismatch: agents that generate
code will reference the document and emit raw SQL instead of EF Core `IEntityTypeConfiguration`
classes. This bypasses Code-First migrations, breaks type safety, and violates
`entityframework-core.instructions.md`. The schema diverges from the C# model the moment
anyone runs a migration.

**Correct approach:**
Replace every SQL DDL block with an EF Core `IEntityTypeConfiguration<T>` example and a
pattern reference:

```markdown
**Entity Class:**
```csharp
public sealed class SampleListProjection
{
    public string TenantId { get; set; } = null!;
    public Guid SampleId { get; set; }
    public string Barcode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTimeOffset ReceivedDate { get; set; }
}
```

**Configuration Class:**
```csharp
public sealed class SampleListProjectionConfiguration
    : IEntityTypeConfiguration<SampleListProjection>
{
    public void Configure(EntityTypeBuilder<SampleListProjection> builder)
    {
        builder.ToTable("sample_list_projection", "sample_mgmt");
        builder.HasKey(e => new { e.TenantId, e.SampleId });
        builder.Property(e => e.Barcode).HasColumnName("barcode").HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("ix_sample_list_status");
    }
}
```

**Pattern References:**
- See `.github/instructions/event-sourcing-projections.instructions.md`
- See `.github/instructions/entityframework-core.instructions.md`
```

---

## Violation 9: Hardcoded Connection Strings in Architecture Document

**What it looks like:**
```markdown
### Appendix D: Connection Strings

```json
{
  "ConnectionStrings": {
    "SampleDatabase": "Host=localhost;Port=5432;Database=laboratory_db;Username=sample_writer;Password=WriterPass123!"
  }
}
```
```

**Why it's wrong:**
Publishing connection strings with credentials — even "development" credentials — in a
committed architecture document is a security violation (`security-patterns.instructions.md`).
More importantly, it teaches agents and developers to hardcode connection strings instead of
using Aspire's `.WithReference()` pattern, undermining the entire Aspire service discovery model.

**Correct approach:**
Replace with the Aspire connection management pattern:

```markdown
### Connection String Management

**Strategy:** .NET Aspire service references — NO hardcoded connection strings

**AppHost registration:**
```csharp
var postgres = builder.AddPostgres("postgres").AddDatabase("laboratory-db");
builder.AddProject<Projects.SystemNT_Laboratory_Services_Samples>("sample-service")
    .WithReference(postgres);
```

**Service consumption (in Program.cs):**
```csharp
builder.AddNpgsqlDbContext<SamplesDataContext>("laboratory-db");
```

**Development secrets:** Managed via `dotnet user-secrets` — NEVER committed to source control

**Pattern Reference:** See `.github/instructions/aspire-orchestration.instructions.md`
```

---

## Violation 10: Pattern Sections Without Instruction File References

**What it looks like:**
```markdown
### 3.2 Mailbox Flow

1. UnitOfWork persists Result + writes IntegrationEvent to mailboxes table
2. Worker polls mailboxes table (PeriodicTimer, every 10s)
3. Worker claims message (FOR UPDATE SKIP LOCKED)
4. Worker deserializes event → calls handler
```

**Why it's wrong:**
The architecture document is consumed by implementation agents. Without a `**Pattern Reference:**`
line, an agent reading this section has no authoritative source to consult for exact method names,
SDK choices (`Microsoft.NET.Sdk.Worker` not `Microsoft.NET.Sdk`), and patterns like
`IServiceScopeFactory`. The agent will improvise — and get it wrong.

**Correct approach:**
Every implementation section must close with pattern references:

```markdown
### 3.2 Mailbox Flow

1. `UnitOfWork` persists domain aggregate + writes `IntegrationEvent` to `event_store.mailboxes`
2. `Worker.{Domain}` (`Microsoft.NET.Sdk.Worker`) polls via `PeriodicTimer` every 10s
3. Claims batch: `SELECT ... FOR UPDATE SKIP LOCKED`
4. Deserializes payload → dispatches to typed `I{EventName}Handler`
5. On success: `Status = Processed`; on transient failure: `RetryCount++`; after 3 failures: `Status = Failed`

**Pattern References:**
- See `.github/instructions/integration-event-worker.instructions.md` — SDK, PeriodicTimer, IServiceScopeFactory rules
- See `.github/instructions/background-jobs.instructions.md` — BackgroundService lifecycle
```

---

## Violation 11: Missing Mandatory Sections

**What it looks like:**
The architecture document covers Purpose, Domain Model, Data Architecture, Auth, and Resilience —
but has no Testing Strategy, no Multi-Tenancy section, no Naming Conventions section, and no
"Related Skills" section at the end.

**Why it's wrong:**
Each missing section represents a gap that implementation agents will fill incorrectly:
- Without **Testing Strategy**: agents create test projects with wrong structure or wrong frameworks
- Without **Multi-Tenancy**: agents forget `ICurrentTenant` and introduce cross-tenant data leaks
- Without **Naming Conventions**: agents use `{Domain}DbContext` (WRONG) instead of `{Domain}DataContext`
- Without **Related Skills**: the team doesn't know which skills to invoke for implementation

**Correct approach:**
Before closing the session, verify that the architecture document contains all mandatory sections
listed in `checklists/dod.md` → "Mandatory Sections Present" block.
Use `patterns/document-template.md` as the structural scaffold for every generated document.

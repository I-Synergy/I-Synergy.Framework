# Design Interrogation Skill Improvement

**Date:** 2026-01-09  
**Issue:** Architecture documents contain SQL DDL instead of EF Core patterns  
**Root Cause:** Missing verification step against instruction files  

---

## Problem Statement

When generating Architecture-Document.md, the design-interrogation skill produces implementation details that violate instruction files:

**Gap Example:**
```sql
-- ❌ Generated in Architecture-Document.md
CREATE TABLE sample_mgmt.sample_list_projection (
    tenant_id VARCHAR(200) NOT NULL,
    sample_id UUID NOT NULL,
    barcode VARCHAR(50) NOT NULL,
    ...
);
```

**Should Reference (per infrastructure-layer.instructions.md):**
```csharp
// ✅ Should reference EF Core pattern
public sealed class SampleListProjectionConfiguration : IEntityTypeConfiguration<SampleListProjection>
{
    public void Configure(EntityTypeBuilder<SampleListProjection> builder)
    {
        builder.ToTable("sample_list_projection", "sample_mgmt");
        // ... configuration
    }
}
```

---

## Test Cases

### Test 1: Event Sourcing + Projections
**Input:** "Design a LIMS with event sourcing and projections for fast queries"

**Current Output (WRONG):**
- Architecture doc contains SQL CREATE TABLE statements
- SQL schema definitions for projections

**Expected Output (CORRECT):**
- Architecture doc references `IEntityTypeConfiguration` pattern
- Links to `.github/instructions/event-sourcing-projections.instructions.md`
- NO SQL DDL statements

---

### Test 2: Microservices with Aspire
**Input:** "Design microservices with .NET Aspire orchestration"

**Current Output (WRONG):**
- Hardcoded connection strings in architecture doc

**Expected Output (CORRECT):**
- References `.WithReference()` pattern from aspire-orchestration.instructions.md
- Connection string management via Aspire service discovery

---

### Test 3: CQRS with Custom Handlers
**Input:** "Design CQRS without MediatR"

**Current Output (WRONG):**
- Architecture doc might show MediatR patterns

**Expected Output (CORRECT):**
- Direct handler injection pattern from cqrs-implementation.instructions.md
- Individual parameters, not model objects

---

## Proposed Improvement

Add **Verification Step** to design-interrogation workflow:

### New Section in SKILL.md: "Architecture Document Verification"

Before finalizing the architecture document:

1. **Load relevant instruction files** based on technology choices:
   - Event sourcing → `event-sourcing-projections.instructions.md`
   - EF Core → `infrastructure-layer.instructions.md`, `entityframework-core.instructions.md`
   - Aspire → `aspire-orchestration.instructions.md`
   - CQRS → `cqrs-implementation.instructions.md`

2. **Check for violations:**
   - ❌ SQL DDL statements (CREATE TABLE, ALTER TABLE, etc.)
   - ❌ Hardcoded connection strings
   - ❌ MediatR references when custom CQRS is chosen
   - ❌ DateTime instead of DateTimeOffset

3. **Replace with pattern references:**
   - ✅ Link to relevant instruction file section
   - ✅ Show entity class + configuration class
   - ✅ Reference the DoD from instruction files

4. **Document rationale:**
   - Why EF Core Code-First instead of SQL DDL
   - Why this aligns with the instruction files
   - How agents will implement this

---

## Success Criteria

After improvement, the skill should:

1. **Never generate SQL DDL** in architecture documents
2. **Always reference instruction patterns** instead
3. **Include links** to relevant instruction files
4. **Pass DoD verification** against instruction files

---

## Evaluation Plan

Run 3 test cases (above) with:
- Baseline: Current design-interrogation skill
- Improved: With verification step added

Measure:
- Number of SQL DDL violations
- Number of correct pattern references
- Alignment with instruction files (0-100%)

Target: 100% alignment with instruction files

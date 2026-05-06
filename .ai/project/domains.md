# Business Domains (CUSTOMIZE THIS)

**Instructions:** Document your project's business domains and entities.

**Purpose:** This file defines WHAT you're building - business contexts, entities, rules, and domain relationships.

## Domain Overview

**Total Domains:** [Number]

**Organization Strategy:** [How domains are organized - by business capability / by team / by module]

## Domain List

| Domain | Purpose | Status | Team/Owner |
|--------|---------|--------|------------|
| [{Domain1}] | [Business purpose] | [Active / Planned / Deprecated] | [Team/Person] |
| [{Domain2}] | [Business purpose] | [Active / Planned / Deprecated] | [Team/Person] |

**Example:**
| Domain | Purpose | Status | Team/Owner |
|--------|---------|--------|------------|
| Budgets | Financial planning and tracking | Active | Finance Team |
| Customers | Customer relationship management | Active | Sales Team |
| Orders | Sales order processing | Planned | Operations Team |

---

## Domain Details

### {Domain1} Domain

**Purpose:** [What this domain does in business terms]

**Bounded Context:** [What this domain is responsible for - its boundaries]

**Entities:**

| Entity | Description | Aggregate Root? | Key Properties |
|--------|-------------|-----------------|----------------|
| [{Entity1}] | [What it represents] | [Yes / No] | [Key properties] |
| [{Entity2}] | [What it represents] | [Yes / No] | [Key properties] |

**Example:**
| Entity | Description | Aggregate Root? | Key Properties |
|--------|-------------|-----------------|----------------|
| Budget | Financial budget period | Yes | Name, Amount, StartDate, EndDate |
| Goal | Savings goal within budget | No | Name, TargetAmount, CurrentAmount |
| Debt | Debt to pay off | No | Creditor, TotalAmount, OutstandingAmount |

**Domain Events:**

- [{Entity}Created] - [When fired and why it matters]
- [{Entity}Updated] - [When fired and why it matters]
- [{Entity}Deleted] - [When fired and why it matters]

**Example:**
- [BudgetCreated] - When a new budget is successfully created; triggers notification to user
- [GoalAchieved] - When a goal reaches its target amount; triggers celebration notification
- [DebtPaidOff] - When debt outstanding amount reaches zero; triggers credit score update

**Business Rules:**

1. [Rule 1] - [Description and rationale]
2. [Rule 2] - [Description and rationale]
3. [Rule 3] - [Description and rationale]

**Example:**
1. Budget amount must be positive - Cannot create negative budgets
2. Goals total cannot exceed budget amount - Prevents over-allocation of funds
3. Debt payment date cannot be in the past - Ensures valid payment scheduling
4. Budget dates cannot overlap for same user - Prevents ambiguous period assignment

**Validation Rules:**

| Field | Validation | Error Message |
|-------|-----------|---------------|
| [Field] | [Rule] | [Message] |

**Example:**
| Field | Validation | Error Message |
|-------|-----------|---------------|
| Budget.Name | Required, Max 100 chars | "Budget name is required and must be less than 100 characters" |
| Budget.Amount | > 0, < 1,000,000 | "Budget amount must be between 0 and 1,000,000" |
| Goal.TargetAmount | > 0 | "Goal target amount must be positive" |

**Domain-Specific Patterns:**

- [Pattern 1] - [Why used in this domain]
- [Pattern 2] - [Why used in this domain]

**Example:**
- **Soft Delete** - Budget entities use soft delete (IsDeleted flag) to maintain historical data for reporting
- **Cascade Archiving** - Deleting a budget marks all child goals/debts as archived, not deleted
- **Temporal Data** - Budget has StartDate/EndDate for period-based queries and overlapping validation

**Project Structure:**

```
{ApplicationName}.Contracts.{Domain}/       # Service contracts, interfaces
{ApplicationName}.Entities.{Domain}/        # EF Core entity classes
{ApplicationName}.Models.{Domain}/          # DTOs, view models
{ApplicationName}.Domain.{Domain}/          # CQRS handlers, domain logic
{ApplicationName}.Services.{Domain}/        # API endpoints
{ApplicationName}.ViewModels.{Domain}/      # MVVM ViewModels (if applicable)
```

**Dependencies:**

- **Depends On:** [Other domains this one depends on]
- **Depended On By:** [Other domains that depend on this one]

**Example:**
- **Depends On:** None (core domain)
- **Depended On By:** Reports (reads budget data), Analytics (aggregates budget metrics)

---

### {Domain2} Domain

[Repeat the same structure for each domain]

---

## Cross-Domain Relationships

**Document how domains interact:**

```
[Visual representation of domain dependencies]

Example:
Orders ----reads----→ Customers (get customer details)
Orders ----creates---→ Invoices (create invoice from order)
Payments ----updates---→ Invoices (mark invoice as paid)
Reports ----reads----→ All Domains (aggregate reporting data)
```

**Relationship Details:**

| Source Domain | Relationship | Target Domain | Method | Notes |
|--------------|--------------|---------------|--------|-------|
| [Domain1] | [reads/writes/creates] | [Domain2] | [How communicated] | [Why needed] |

**Example:**
| Source Domain | Relationship | Target Domain | Method | Notes |
|--------------|--------------|---------------|--------|-------|
| Orders | reads | Customers | Direct reference | Get customer details for order |
| Orders | creates | Invoices | Domain event | Create invoice when order completed |
| Payments | updates | Invoices | API call | Mark invoice as paid |
| Reports | reads | All | Read-only queries | Aggregate data for reporting |

## Shared Concepts

**Concepts used across multiple domains:**

| Concept | Domains | Approach | Location |
|---------|---------|----------|----------|
| [Concept] | [Domain1, Domain2] | [Shared / Duplicated / Referenced] | [Where defined] |

**Example:**
| Concept | Domains | Approach | Location |
|---------|---------|----------|----------|
| Address | Customers, Suppliers, Invoices | Shared value object | Common.ValueObjects |
| Money | Orders, Invoices, Payments | Shared value object with currency | Common.ValueObjects |
| Person | Customers, Users, Contacts | Duplicated (different contexts) | Each domain |
| AuditFields | All domains | Base entity | Common.Entities.BaseEntity |

## Domain Maturity

**Implementation maturity per domain:**

| Domain | Maturity | Coverage | Technical Debt | Notes |
|--------|----------|----------|----------------|-------|
| [{Domain}] | [Initial / Growing / Mature] | [% complete] | [Low / Medium / High] | [Notes] |

**Maturity Levels:**
- **Initial:** Basic CRUD, minimal testing, incomplete features
- **Growing:** Most features implemented, good test coverage, some edge cases missing
- **Mature:** Complete implementation, comprehensive testing, production-ready, well-documented

**Example:**
| Domain | Maturity | Coverage | Technical Debt | Notes |
|--------|----------|----------|----------------|-------|
| Budgets | Mature | 100% | Low | Complete CRUD, full testing, comprehensive documentation |
| Customers | Growing | 60% | Medium | Missing bulk operations, needs performance optimization |
| Orders | Initial | 20% | Low | Basic create/read only, needs update/delete and validation |

## Reference Implementation

**Which domain serves as the template:**

**Primary Reference:** [{Domain}] - [Why it's the reference]

**What to copy from the reference:**
- [Aspect 1] - [What to learn from it]
- [Aspect 2] - [What to learn from it]
- [Aspect 3] - [What to learn from it]

**Example:**

**Primary Reference:** [Budgets] - Complete implementation with all CRUD operations, comprehensive testing, full documentation

**What to copy from the reference:**
- CQRS handler structure and naming conventions
- API endpoint organization
- Unit test structure with BDD scenarios
- XML documentation style

## Domain Evolution History

**Track how domains have evolved over time:**

### {Domain} Evolution

**Version 1.0 (Date):**
- [Feature 1]
- [Feature 2]

**Version 2.0 (Date):**
- [Feature added]
- [Refactoring done]
- [Breaking change]

**Example:**

### Budgets Evolution

**Version 1.0 (2025-01-01):**
- Basic budget CRUD operations
- Simple goal tracking
- Basic validation

**Version 2.0 (2025-02-01):**
- Added debt management
- Implemented recurring transactions
- Added budget templates
- Enhanced validation with business rules

**Version 2.1 (2025-02-15):**
- Performance optimization for large budget lists
- Added budget category filtering
- Implemented soft delete for historical data

## Future Domains

**Planned domains not yet implemented:**

| Future Domain | Purpose | Priority | Dependencies | Estimated Effort |
|---------------|---------|----------|--------------|------------------|
| [Domain] | [Purpose] | [High / Medium / Low] | [Required domains] | [Small / Medium / Large] |

**Example:**
| Future Domain | Purpose | Priority | Dependencies | Estimated Effort |
|---------------|---------|----------|--------------|------------------|
| Analytics | Reporting and insights | Medium | Budgets, Orders | Medium |
| Notifications | User notifications | Low | All domains | Small |
| Workflows | Automated business workflows | High | Orders, Invoices | Large |

## Domain Ubiquitous Language

**Key terms used in each domain:**

### {Domain} Terms

| Term | Definition | Usage |
|------|------------|-------|
| [Term] | [Business definition] | [How used in code] |

**Example:**

### Budgets Terms

| Term | Definition | Usage |
|------|------------|-------|
| Budget | A financial plan for a specific time period | Budget entity, BudgetModel DTO |
| Goal | A savings target within a budget | Goal entity, child of Budget |
| Allocation | Assignment of budget amount to categories | Property on Budget entity |
| Rollover | Carrying unused budget to next period | Feature flag on Budget |

---

**Remember:** Keep this file updated as domains evolve. Reference it in session-context.md when working on specific domains. For architectural patterns, see [architecture.md](architecture.md). For technology choices, see [tech-stack.md](tech-stack.md).

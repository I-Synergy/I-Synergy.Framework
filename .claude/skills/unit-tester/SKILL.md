---
name: unit-tester
description: MSTest unit testing specialist. Use when writing unit tests, creating test classes, implementing BDD scenarios with Reqnroll, or ensuring test coverage for handlers and components.
---

# Unit Tester Skill

Specialized agent for MSTest, Moq, and Reqnroll testing.

> **Context:** This skill covers two scenarios. Identify which applies before writing tests:
> - **Framework library tests** (`tests/ISynergy.Framework.*.Tests`) — test the framework's own classes (extensions, CQRS dispatchers, validators, etc.) using standard DI or direct instantiation. No `DataContext`, no `TenantContext`.
> - **Application tests** (projects built on top of I-Synergy Framework) — test CQRS handlers against a real `DataContext` with `UseInMemoryDatabase`. The `DataContext`/`TenantContext`/`TestDataHelper` patterns below apply here.

## Expertise Areas

- MSTest framework
- Moq mocking library
- Reqnroll BDD testing
- Test-Driven Development (TDD)
- Unit test organization
- Integration testing
- Test coverage analysis

## Responsibilities

1. **Create Unit Tests**
   - Test all CQRS handlers
   - Test business logic thoroughly
   - Test edge cases and error conditions
   - Use descriptive test names

2. **Write BDD Scenarios**
   - Create Gherkin feature files
   - Implement step definitions
   - Cover happy paths and error cases
   - Make scenarios readable for non-technical stakeholders

3. **Use Real DataContext with InMemory Provider**
   - NEVER mock DataContext — use real `DataContext` with `UseInMemoryDatabase`
   - ALWAYS call `TenantContext.Set(TestDataHelper.TestTenantId)` BEFORE `new DataContext(options)`
   - Seed test data via `await _dataContext.{DbSet}.AddAsync(entity); await _dataContext.SaveChangesAsync()`
   - Mock only external service dependencies (non-DataContext), never the DataContext itself

4. **Ensure Coverage**
   - Aim for 80%+ code coverage
   - Cover all public methods
   - Test validation logic
   - Test error handling

## Test Frameworks (REQUIRED)

- **Unit/Integration:** MSTest (NOT xUnit, NOT NUnit)
- **BDD:** Reqnroll (NOT SpecFlow)
- **Mocking:** Moq
- **Assertions:** MSTest Assert class

## Test Naming Convention

```
MethodName_Scenario_ExpectedResult

Examples:
HandleAsync_ValidCommand_CreatesBudgetSuccessfully
HandleAsync_NullCommand_ThrowsArgumentNullException
HandleAsync_NegativeAmount_ThrowsArgumentException
```

## Test Structure (AAA Pattern)

```csharp
[TestMethod]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var command = new Create{Entity}Command(...);

    // Act
    var result = await _handler.HandleAsync(command);

    // Assert
    Assert.IsNotNull(result);
    _mockContext.Verify(x => x.Method(...), Times.Once);
}
```

## Gherkin Scenario Template

```gherkin
Feature: {Entity} Management
  As a user
  I want to manage {entities}
  So that I can achieve business goal

  Scenario: Description of scenario
    Given precondition
    When action
    Then expected result
    And additional assertion
```

## Templates to Use

- [`test-class.cs.txt`](../../reference/templates/test-class.cs.txt)
- [`feature-file.feature.txt`](../../reference/templates/feature-file.feature.txt)

## Patterns to Follow

- [`testing-patterns.md`](../../patterns/testing-patterns.md)

## FORBIDDEN Patterns — Will Cause Silent Test Failures

**Never use any of the following:**

- `Mock<DataContext>` — EF Core global query filters are bypassed; tests pass against empty sets
- `InMemoryDataContext` (custom subclass) — never create DataContext subclasses for tests
- `MockDbSetHelper` — creates fake `DbSet<T>` that ignores tenant filter
- `new DataContext(options)` without `TenantContext.Set(TestDataHelper.TestTenantId)` first

**Why this matters:** `BaseDbContext` applies `e.TenantId == TenantContext.TenantId` to all `ITenantEntity` types via global EF query filters. `TenantContext.TenantId` defaults to `Guid.Empty`. All test entities have a real TenantId set by `TestDataHelper`. If you forget `TenantContext.Set(...)`, every query returns an empty result — tests that check "entity exists" will silently fail to validate anything.

## Correct DataContext Setup (Required in Every Test That Uses DataContext)

```csharp
[TestInitialize]
public void Setup()
{
    var options = new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    TenantContext.Set(TestDataHelper.TestTenantId); // MUST come before new DataContext(...)
    _dataContext = new DataContext(options);
}

[TestCleanup]
public void Cleanup() => _dataContext?.Dispose();
```

## Checklist Before Completion

- [ ] All handlers have unit tests
- [ ] All tests use MSTest (not xUnit/NUnit)
- [ ] Descriptive test names following convention
- [ ] AAA pattern used consistently
- [ ] DataContext uses real InMemory provider (NOT `Mock<DataContext>`)
- [ ] `TenantContext.Set(TestDataHelper.TestTenantId)` called before `new DataContext(options)` in every test class
- [ ] Test data seeded via `AddAsync` / `AddRangeAsync` + `SaveChangesAsync` on real DataContext
- [ ] State verified by querying the real DataContext, not via `mock.Verify`
- [ ] BDD scenarios for complex workflows
- [ ] All tests pass
- [ ] Coverage meets 80%+ target

# Testing Patterns

Complete patterns for MSTest, Moq, and Reqnroll testing.

## MSTest Unit Test Pattern

```csharp
// File: tests/{ApplicationName}.{Domain}.Tests/Handlers/Create{Entity}CommandHandlerTests.cs

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace {ApplicationName}.{Domain}.Tests.Handlers;

[TestClass]
public class Create{Entity}CommandHandlerTests
{
    private Mock<DataContext> _dataContextMock;
    private Mock<ILogger<Create{Entity}CommandHandler>> _loggerMock;
    private Create{Entity}CommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _dataContextMock = new Mock<DataContext>();
        _loggerMock = new Mock<ILogger<Create{Entity}CommandHandler>>();
        _handler = new Create{Entity}CommandHandler(
            _dataContextMock.Object,
            _loggerMock.Object);
    }

    [TestMethod]
    public async Task HandleAsync_ValidCommand_Creates{Entity}Successfully()
    {
        // Arrange
        var command = new Create{Entity}Command(
            Property1: "Test Value",
            Property2: 100.50m,
            Property3: DateTimeOffset.UtcNow);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(Guid.Empty, result.{Entity}Id);

        _dataContextMock.Verify(
            x => x.{Entities}.Add(It.IsAny<{Entity}Model>()),
            Times.Once);
        _dataContextMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task HandleAsync_NullCommand_ThrowsArgumentNullException()
    {
        // Act
        await _handler.HandleAsync(null!);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task HandleAsync_ValidCommand_LogsInformation()
    {
        // Arrange
        var command = new Create{Entity}Command(
            Property1: "Test Value",
            Property2: 100.50m,
            Property3: DateTimeOffset.UtcNow);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }
}
```

## Reqnroll Feature File Pattern

```gherkin
# File: tests/{ApplicationName}.{Domain}.Tests/Features/{Entity}Management.feature

Feature: {Entity} Management
  As a user
  I want to manage {entities}
  So that I can track my data

  @AC-{BC}-{NNN}.1
  Scenario: Create a new {entity}
    Given I am an authenticated user
    When I create a {entity} with the following details:
      | Property1  | Property2 | Property3  |
      | Test Value | 100.50    | 2025-12-31 |
    Then the {entity} should be created successfully
    And the {entity} should have a unique identifier
    And the {entity} should be retrievable by its identifier

  @AC-{BC}-{NNN}.2
  Scenario: Cannot create {entity} with invalid data
    Given I am an authenticated user
    When I attempt to create a {entity} with negative Property2
    Then the creation should fail with a validation error
    And the error message should indicate Property2 must be positive

  @AC-{BC}-{NNN}.3
  Scenario: Update an existing {entity}
    Given I am an authenticated user
    And a {entity} exists with Property1 "Original Value"
    When I update the {entity} Property1 to "Updated Value"
    Then the {entity} should be updated successfully
    And the {entity} Property1 should be "Updated Value"

  @AC-{BC}-{NNN}.4
  Scenario: Delete an existing {entity}
    Given I am an authenticated user
    And a {entity} exists
    When I delete the {entity}
    Then the {entity} should be deleted successfully
    And the {entity} should not be retrievable

  @AC-{BC}-{NNN}.5
  Scenario: Get list of {entities} with pagination
    Given I am an authenticated user
    And 25 {entities} exist
    When I request page 1 with page size 10
    Then I should receive 10 {entities}
    When I request page 3 with page size 10
    Then I should receive 5 {entities}
```

## Traceability

Every scenario states which acceptance criterion it is evidence for, as a tag on the line directly
above `Scenario:` — `@AC-{BC}-{NNN}.{n}`, where `{BC}` is the bounded context, `{NNN}` the story and
`{n}` the criterion within it. The full convention, including how identifiers are allocated and
retired, is `.ai/reference/traceability.md`.

- Exactly one traceability tag per scenario. A criterion may be satisfied by several scenarios; a
  scenario satisfies exactly one criterion.
- Other tags (`@smoke`, `@e2e`) are allowed alongside the traceability tag.
- `Feature:` and `Background:` carry no traceability tag.

The MSTest side carries the same link: one `[TestCategory("AC-…")]` per test method, and a
`// Traces: US-… / AC-…` header listing the file's story and criterion identifiers — see
`.ai/reference/templates/test-class.cs.txt`.

## Reqnroll Step Definitions Pattern

```csharp
// File: tests/{ApplicationName}.{Domain}.Tests/Steps/{Entity}ManagementSteps.cs

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace {ApplicationName}.{Domain}.Tests.Steps;

[Binding]
public class {Entity}ManagementSteps
{
    private readonly ScenarioContext _scenarioContext;
    private Create{Entity}Response? _createResponse;
    private {Entity}Response? _retrievedEntity;
    private Exception? _exception;

    public {Entity}ManagementSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"I am an authenticated user")]
    public void GivenIAmAnAuthenticatedUser()
    {
        // Set up authentication context
    }

    [When(@"I create a {entity} with the following details:")]
    public async Task WhenICreateEntityWithDetails(Table table)
    {
        var row = table.Rows[0];
        var command = new Create{Entity}Command(
            Property1: row["Property1"],
            Property2: decimal.Parse(row["Property2"]),
            Property3: DateTimeOffset.Parse(row["Property3"]));

        try
        {
            _createResponse = await _handler.HandleAsync(command);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the {entity} should be created successfully")]
    public void ThenEntityShouldBeCreatedSuccessfully()
    {
        Assert.IsNotNull(_createResponse);
        Assert.AreNotEqual(Guid.Empty, _createResponse.{Entity}Id);
    }
}
```

## Test Naming Conventions

```csharp
// Pattern: MethodName_Scenario_ExpectedResult

[TestMethod]
public async Task HandleAsync_ValidCommand_CreatesBudgetSuccessfully()

[TestMethod]
public async Task HandleAsync_NullCommand_ThrowsArgumentNullException()

[TestMethod]
public async Task HandleAsync_NegativeAmount_ThrowsArgumentException()
```

## Moq Patterns

```csharp
// ─── EF Core DbSet Mock Helpers ─────────────────────────────────────
// Place these in a test infrastructure project or base test class.
// They enable FirstOrDefaultAsync/ToListAsync on mocked DbSet properties.

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;
    public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
    public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    public T Current => _inner.Current;
}

internal class TestAsyncQueryProvider<T> : IQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
    public IQueryable CreateQuery(Expression expression) =>
        new TestAsyncEnumerable<T>(expression);
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
        new TestAsyncEnumerable<TElement>(expression);
    public object? Execute(Expression expression) => _inner.Execute(expression);
    public TResult Execute<TResult>(Expression expression) =>
        _inner.Execute<TResult>(expression);
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal static class DbSetMockHelper
{
    public static Mock<DbSet<T>> CreateMock<T>(List<T> source) where T : class
    {
        var queryable = source.AsQueryable();
        var mock = new Mock<DbSet<T>>();
        mock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
        mock.As<IQueryable<T>>()
            .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
        mock.As<IQueryable<T>>()
            .Setup(m => m.Expression).Returns(queryable.Expression);
        mock.As<IQueryable<T>>()
            .Setup(m => m.ElementType).Returns(queryable.ElementType);
        mock.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return mock;
    }
}

// ─── Setup DbSet to return data from FirstOrDefaultAsync ───
var expectedBudget = new BudgetModel { BudgetId = Guid.NewGuid(), Description = "Test" };
var budgetsMock = DbSetMockHelper.CreateMock(new List<BudgetModel> { expectedBudget });
_dataContextMock.Setup(x => x.Budgets).Returns(budgetsMock.Object);

// ─── Verify Add + SaveChanges (Create) ───
_dataContextMock.Verify(
    x => x.Budgets.Add(It.Is<BudgetModel>(m => m.Description == "Test")),
    Times.Once);
_dataContextMock.Verify(
    x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
    Times.Once);

// ─── Verify Remove + SaveChanges (Delete) ───
_dataContextMock.Verify(
    x => x.Budgets.Remove(It.IsAny<BudgetModel>()),
    Times.Once);
_dataContextMock.Verify(
    x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
    Times.Once);

// ─── Setup empty DbSet (not found => FirstOrDefaultAsync returns null) ───
var emptyBudgetsMock = DbSetMockHelper.CreateMock(new List<BudgetModel>());
_dataContextMock.Setup(x => x.Budgets).Returns(emptyBudgetsMock.Object);
```

## Test Organization

```
tests/
  {ApplicationName}.{Domain}.Tests/
    Features/
      {Entity}Management.feature
    Steps/
      {Entity}ManagementSteps.cs
    Handlers/
      Create{Entity}CommandHandlerTests.cs
      Update{Entity}CommandHandlerTests.cs
      Delete{Entity}CommandHandlerTests.cs
      Get{Entity}ByIdQueryHandlerTests.cs
      Get{Entities}ListQueryHandlerTests.cs
    Integration/
      {Entity}EndpointsTests.cs
```

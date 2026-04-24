# Testing Patterns

Complete patterns for MSTest, Moq, and Reqnroll testing.

## MSTest Unit Test Pattern

```csharp
// File: tests/{ApplicationName}.{Domain}.Tests/Handlers/Create{Entity}HandlerTests.cs

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace {ApplicationName}.{Domain}.Tests.Handlers;

[TestClass]
public class Create{Entity}HandlerTests
{
    private Mock<DataContext> _dataContextMock;
    private Mock<ILogger<Create{Entity}Handler>> _loggerMock;
    private Create{Entity}Handler _handler;

    [TestInitialize]
    public void Setup()
    {
        _dataContextMock = new Mock<DataContext>();
        _loggerMock = new Mock<ILogger<Create{Entity}Handler>>();
        _handler = new Create{Entity}Handler(
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
            x => x.AddItemAsync<{Entity}, {Entity}Model>(
                It.IsAny<{Entity}Model>(),
                It.IsAny<CancellationToken>()),
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

  Scenario: Create a new {entity}
    Given I am an authenticated user
    When I create a {entity} with the following details:
      | Property1  | Property2 | Property3  |
      | Test Value | 100.50    | 2025-12-31 |
    Then the {entity} should be created successfully
    And the {entity} should have a unique identifier
    And the {entity} should be retrievable by its identifier

  Scenario: Cannot create {entity} with invalid data
    Given I am an authenticated user
    When I attempt to create a {entity} with negative Property2
    Then the creation should fail with a validation error
    And the error message should indicate Property2 must be positive

  Scenario: Update an existing {entity}
    Given I am an authenticated user
    And a {entity} exists with Property1 "Original Value"
    When I update the {entity} Property1 to "Updated Value"
    Then the {entity} should be updated successfully
    And the {entity} Property1 should be "Updated Value"

  Scenario: Delete an existing {entity}
    Given I am an authenticated user
    And a {entity} exists
    When I delete the {entity}
    Then the {entity} should be deleted successfully
    And the {entity} should not be retrievable
```

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
// Setup method return
_dataContextMock
    .Setup(x => x.GetItemByIdAsync<Budget, BudgetModel, Guid>(
        It.IsAny<Guid>(),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(new BudgetModel { BudgetId = Guid.NewGuid() });

// Verify method called
_dataContextMock.Verify(
    x => x.AddItemAsync<Budget, BudgetModel>(
        It.IsAny<BudgetModel>(),
        It.IsAny<CancellationToken>()),
    Times.Once);

// Verify method never called
_dataContextMock.Verify(
    x => x.RemoveItemAsync<Budget, Guid>(
        It.IsAny<Guid>(),
        It.IsAny<CancellationToken>()),
    Times.Never);

// Setup method to throw
_dataContextMock
    .Setup(x => x.GetItemByIdAsync<Budget, BudgetModel, Guid>(
        It.IsAny<Guid>(),
        It.IsAny<CancellationToken>()))
    .ThrowsAsync(new KeyNotFoundException());
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
      Create{Entity}HandlerTests.cs
      Update{Entity}HandlerTests.cs
      Delete{Entity}HandlerTests.cs
      Get{Entity}ByIdHandlerTests.cs
      Get{Entity}ListHandlerTests.cs
    Integration/
      {Entity}EndpointsTests.cs
```

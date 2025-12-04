using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace ISynergy.Framework.Core.Tests.Models.Results;

[TestClass]
public class ResultGenericTests
{
    #region Success Factory Methods

    [TestMethod]
    public void Success_WithNoParameters_ReturnsSucceededResult()
    {
        // Act
        var result = Result<string>.Success();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.IsNull(result.StatusCode);
        Assert.IsNull(result.Data);
    }

    // Note: Result<T>.Success(string message) is intentionally NOT available to avoid ambiguity when T is string.
    // For message-only success without data, use Result<T>.Success() or the base Result.Success(message).

    [TestMethod]
    public void Success_WithData_ReturnsSucceededResultWithData()
    {
        // Arrange
        var data = "Test data";

        // Act
        var result = Result<string>.Success(data);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.AreEqual(data, result.Data);
    }

    [TestMethod]
    public void Success_WithDataAndMessage_ReturnsSucceededResultWithBoth()
    {
        // Arrange
        var data = "Test data";
        var message = "Data retrieved successfully";

        // Act
        var result = Result<string>.Success(data, message);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.AreEqual(data, result.Data);
    }

    [TestMethod]
    public void Success_WithDataAndMessages_ReturnsSucceededResultWithAll()
    {
        // Arrange
        var data = 42;
        var messages = new List<string> { "Message 1", "Message 2" };

        // Act
        var result = Result<int>.Success(data, messages);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(2, result.Messages);
        CollectionAssert.AreEqual(messages, result.Messages);
        Assert.AreEqual(data, result.Data);
    }

    [TestMethod]
    public void Success_WithDataAndStatusCode_ReturnsSucceededResultWithBoth()
    {
        // Arrange
        var data = new TestDto { Id = 1, Name = "Test" };
        var statusCode = HttpStatusCode.Created;

        // Act
        var result = Result<TestDto>.Success(data, statusCode);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(statusCode, result.StatusCode);
        Assert.AreEqual(data, result.Data);
    }

    [TestMethod]
    public void Success_WithDataMessageAndStatusCode_ReturnsCompleteResult()
    {
        // Arrange
        var data = "Created entity";
        var message = "Entity created successfully";
        var statusCode = HttpStatusCode.Created;

        // Act
        var result = Result<string>.Success(data, message, statusCode);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.AreEqual(statusCode, result.StatusCode);
        Assert.AreEqual(data, result.Data);
    }

    #endregion

    #region Fail Factory Methods

    [TestMethod]
    public void Fail_WithNoParameters_ReturnsFailedResult()
    {
        // Act
        var result = Result<string>.Fail();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.IsNull(result.StatusCode);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public void Fail_WithMessage_ReturnsFailedResultWithMessage()
    {
        // Arrange
        var message = "Operation failed";

        // Act
        var result = Result<string>.Fail(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public void Fail_WithMessages_ReturnsFailedResultWithMessages()
    {
        // Arrange
        var messages = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result<int>.Fail(messages);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.HasCount(3, result.Messages);
        CollectionAssert.AreEqual(messages, result.Messages);
    }

    [TestMethod]
    public void Fail_WithMessageAndStatusCode_ReturnsFailedResultWithBoth()
    {
        // Arrange
        var message = "Internal server error";
        var statusCode = HttpStatusCode.InternalServerError;

        // Act
        var result = Result<object>.Fail(message, statusCode);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    [TestMethod]
    public void Fail_WithStatusCodeOnly_ReturnsFailedResultWithStatusCode()
    {
        // Arrange
        var statusCode = HttpStatusCode.Forbidden;

        // Act
        var result = Result<string>.Fail(statusCode);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    #endregion

    #region Async Factory Methods

    [TestMethod]
    public async Task SuccessAsync_WithNoParameters_ReturnsSucceededResult()
    {
        // Act
        var result = await Result<string>.SuccessAsync();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsNull(result.Data);
    }

    // Note: Result<T>.SuccessAsync(string message) is intentionally NOT available to avoid ambiguity when T is string.
    // For message-only success without data, use Result<T>.SuccessAsync() or the base Result.SuccessAsync(message).

    [TestMethod]
    public async Task SuccessAsync_WithData_ReturnsSucceededResultWithData()
    {
        // Arrange
        var data = "Async data";

        // Act
        var result = await Result<string>.SuccessAsync(data);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(data, result.Data);
    }

    [TestMethod]
    public async Task SuccessAsync_WithDataAndMessage_ReturnsSucceededResultWithBoth()
    {
        // Arrange
        var data = 100;
        var message = "Data loaded";

        // Act
        var result = await Result<int>.SuccessAsync(data, message);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(data, result.Data);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public async Task SuccessAsync_WithDataAndStatusCode_ReturnsSucceededResultWithBoth()
    {
        // Arrange
        var data = new TestDto { Id = 5, Name = "Async Test" };
        var statusCode = HttpStatusCode.OK;

        // Act
        var result = await Result<TestDto>.SuccessAsync(data, statusCode);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(data, result.Data);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    [TestMethod]
    public async Task FailAsync_WithNoParameters_ReturnsFailedResult()
    {
        // Act
        var result = await Result<string>.FailAsync();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task FailAsync_WithMessage_ReturnsFailedResultWithMessage()
    {
        // Arrange
        var message = "Async failure";

        // Act
        var result = await Result<string>.FailAsync(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public async Task FailAsync_WithMessages_ReturnsFailedResultWithMessages()
    {
        // Arrange
        var messages = new List<string> { "Error A", "Error B" };

        // Act
        var result = await Result<int>.FailAsync(messages);

        // Assert
        Assert.IsFalse(result.Succeeded);
        CollectionAssert.AreEqual(messages, result.Messages);
    }

    [TestMethod]
    public async Task FailAsync_WithMessageAndStatusCode_ReturnsFailedResultWithBoth()
    {
        // Arrange
        var message = "Gateway timeout";
        var statusCode = HttpStatusCode.GatewayTimeout;

        // Act
        var result = await Result<object>.FailAsync(message, statusCode);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    #endregion

    #region HTTP Convenience Methods

    [TestMethod]
    public void Unauthorized_WithNoMessage_ReturnsUnauthorizedResult()
    {
        // Act
        var result = Result<string>.Unauthorized();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual("Unauthorized", result.Messages[0]);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public void Unauthorized_WithMessage_ReturnsUnauthorizedResultWithCustomMessage()
    {
        // Arrange
        var message = "Invalid token";

        // Act
        var result = Result<TestDto>.Unauthorized(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void NotFound_WithNoMessage_ReturnsNotFoundResult()
    {
        // Act
        var result = Result<TestDto>.NotFound();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("Not found", result.Messages[0]);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public void NotFound_WithMessage_ReturnsNotFoundResultWithCustomMessage()
    {
        // Arrange
        var message = "User with ID 123 not found";

        // Act
        var result = Result<object>.NotFound(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void BadRequest_WithNoMessage_ReturnsBadRequestResult()
    {
        // Act
        var result = Result<string>.BadRequest();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Bad request", result.Messages[0]);
    }

    [TestMethod]
    public void BadRequest_WithMessage_ReturnsBadRequestResultWithCustomMessage()
    {
        // Arrange
        var message = "Invalid input parameters";

        // Act
        var result = Result<int>.BadRequest(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void NoContent_ReturnsNoContentResult()
    {
        // Act
        var result = Result<string>.NoContent();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        Assert.IsEmpty(result.Messages);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public void Cancelled_WithNoMessage_ReturnsCancelledResult()
    {
        // Act
        var result = Result<object>.Cancelled();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.StatusCode);
        Assert.AreEqual("Operation cancelled", result.Messages[0]);
    }

    [TestMethod]
    public void Cancelled_WithMessage_ReturnsCancelledResultWithCustomMessage()
    {
        // Arrange
        var message = "User cancelled the operation";

        // Act
        var result = Result<string>.Cancelled(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void ServiceUnavailable_WithNoMessage_ReturnsServiceUnavailableResult()
    {
        // Act
        var result = Result<TestDto>.ServiceUnavailable();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
        Assert.AreEqual("Service unavailable", result.Messages[0]);
    }

    [TestMethod]
    public void ServiceUnavailable_WithMessage_ReturnsServiceUnavailableResultWithCustomMessage()
    {
        // Arrange
        var message = "Database connection unavailable";

        // Act
        var result = Result<string>.ServiceUnavailable(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    #endregion

    #region Interface Implementation and Inheritance

    [TestMethod]
    public void ResultOfT_ImplementsIResultOfT()
    {
        // Act
        var result = new Result<string>();

        // Assert
        Assert.IsInstanceOfType<IResult<string>>(result);
    }

    [TestMethod]
    public void ResultOfT_InheritsFromResult()
    {
        // Act
        var result = new Result<string>();

        // Assert
        Assert.IsInstanceOfType<Result>(result);
    }

    [TestMethod]
    public void ResultOfT_DefaultConstructor_InitializesWithDefaultValues()
    {
        // Act
        var result = new Result<int>();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsNotNull(result.Messages);
        Assert.IsEmpty(result.Messages);
        Assert.IsNull(result.StatusCode);
        Assert.AreEqual(default(int), result.Data);
    }

    [TestMethod]
    public void ResultOfT_ConstructorWithData_SetsDataProperty()
    {
        // Arrange
        var data = new TestDto { Id = 10, Name = "Constructor Test" };

        // Act
        var result = new Result<TestDto>(data);

        // Assert
        Assert.AreEqual(data, result.Data);
    }

    #endregion

    #region Complex Type Tests

    [TestMethod]
    public void Success_WithComplexType_ReturnsSucceededResultWithComplexData()
    {
        // Arrange
        var data = new List<TestDto>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };

        // Act
        var result = Result<List<TestDto>>.Success(data);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsNotNull(result.Data);
        Assert.HasCount(2, result.Data);
        Assert.AreEqual("First", result.Data[0].Name);
    }

    [TestMethod]
    public void Success_WithNullableValueType_ReturnsCorrectResult()
    {
        // Arrange
        int? data = 42;

        // Act
        var result = Result<int?>.Success(data);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(42, result.Data);
    }

    [TestMethod]
    public void Success_WithNullData_ReturnsSucceededResultWithNullData()
    {
        // Arrange
        TestDto? data = null;

        // Act
        var result = Result<TestDto?>.Success(data);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsNull(result.Data);
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void Fail_WithEmptyMessage_ReturnsFailedResultWithEmptyMessage()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var result = Result<string>.Fail(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(string.Empty, result.Messages[0]);
    }

    [TestMethod]
    public void Fail_WithEmptyMessageList_ReturnsFailedResultWithNoMessages()
    {
        // Arrange
        var messages = new List<string>();

        // Act
        var result = Result<int>.Fail(messages);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsEmpty(result.Messages);
    }

    [TestMethod]
    public void Success_AllHttpStatusCodes_CanBeUsedWithData()
    {
        // Test various HTTP status codes to ensure they work correctly with data
        var successCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted };

        foreach (var code in successCodes)
        {
            var result = Result<string>.Success("test data", code);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(code, result.StatusCode);
            Assert.AreEqual("test data", result.Data);
        }
    }

    [TestMethod]
    public void Fail_AllHttpStatusCodes_CanBeUsed()
    {
        // Test various HTTP error status codes
        var errorCodes = new[]
        {
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Forbidden,
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        foreach (var code in errorCodes)
        {
            var result = Result<object>.Fail(code);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(code, result.StatusCode);
            Assert.IsNull(result.Data);
        }
    }

    [TestMethod]
    public void Result_CanBeUsedInPatternMatching()
    {
        // Arrange
        var successResult = Result<int>.Success(42);
        var failResult = Result<int>.Fail("Error");

        // Act & Assert
        var successValue = successResult.Succeeded switch
        {
            true => successResult.Data,
            false => -1
        };
        Assert.AreEqual(42, successValue);

        var failValue = failResult.Succeeded switch
        {
            true => failResult.Data,
            false => -1
        };
        Assert.AreEqual(-1, failValue);
    }

    #endregion

    #region HTTP API Scenario Tests

    [TestMethod]
    public void ApiGetScenario_EntityFound_ReturnsOkWithData()
    {
        // Arrange - Simulating a GET request that finds an entity
        var entity = new TestDto { Id = 1, Name = "Found Entity" };

        // Act
        var result = Result<TestDto>.Success(entity, HttpStatusCode.OK);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("Found Entity", result.Data.Name);
    }

    [TestMethod]
    public void ApiGetScenario_EntityNotFound_ReturnsNotFound()
    {
        // Act - Simulating a GET request that doesn't find an entity
        var result = Result<TestDto>.NotFound("Entity with ID 999 was not found");

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Entity with ID 999 was not found", result.Messages[0]);
    }

    [TestMethod]
    public void ApiPostScenario_EntityCreated_ReturnsCreatedWithData()
    {
        // Arrange - Simulating a POST request that creates an entity
        var createdEntity = new TestDto { Id = 5, Name = "New Entity" };

        // Act
        var result = Result<TestDto>.Success(createdEntity, "Entity created successfully", HttpStatusCode.Created);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("Entity created successfully", result.Messages[0]);
    }

    [TestMethod]
    public void ApiPostScenario_ValidationFailed_ReturnsBadRequest()
    {
        // Act - Simulating a POST request with invalid data
        var result = Result<TestDto>.BadRequest("Name is required");

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Name is required", result.Messages[0]);
    }

    [TestMethod]
    public void ApiDeleteScenario_EntityDeleted_ReturnsNoContent()
    {
        // Act - Simulating a DELETE request that successfully deletes an entity
        var result = Result<TestDto>.NoContent();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public void ApiScenario_Unauthorized_ReturnsUnauthorized()
    {
        // Act - Simulating an unauthorized request
        var result = Result<TestDto>.Unauthorized("Invalid or expired token");

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual("Invalid or expired token", result.Messages[0]);
    }

    [TestMethod]
    public void ApiScenario_ServiceDown_ReturnsServiceUnavailable()
    {
        // Act - Simulating a service unavailable scenario
        var result = Result<TestDto>.ServiceUnavailable("External service is temporarily unavailable");

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
        Assert.AreEqual("External service is temporarily unavailable", result.Messages[0]);
    }

    [TestMethod]
    public void ApiScenario_RequestCancelled_ReturnsCancelled()
    {
        // Act - Simulating a cancelled request (e.g., user navigated away)
        var result = Result<TestDto>.Cancelled("Request was cancelled by the user");

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.StatusCode); // Cancellation typically doesn't have HTTP status
        Assert.AreEqual("Request was cancelled by the user", result.Messages[0]);
    }

    #endregion

    #region Helper Classes

    private class TestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}

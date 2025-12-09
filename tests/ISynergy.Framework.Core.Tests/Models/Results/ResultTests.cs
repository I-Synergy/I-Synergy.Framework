using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace ISynergy.Framework.Core.Tests.Models.Results;

[TestClass]
public class ResultTests
{
    #region Success Factory Methods

    [TestMethod]
    public void Success_WithNoParameters_ReturnsSucceededResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.IsNull(result.StatusCode);
    }

    [TestMethod]
    public void Success_WithMessage_ReturnsSucceededResultWithMessage()
    {
        // Arrange
        var message = "Operation completed successfully";

        // Act
        var result = Result.Success(message);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.IsNull(result.StatusCode);
    }

    [TestMethod]
    public void Success_WithStatusCode_ReturnsSucceededResultWithStatusCode()
    {
        // Arrange
        var statusCode = HttpStatusCode.OK;

        // Act
        var result = Result.Success(statusCode);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    [TestMethod]
    public void Success_WithMessageAndStatusCode_ReturnsSucceededResultWithBoth()
    {
        // Arrange
        var message = "Created successfully";
        var statusCode = HttpStatusCode.Created;

        // Act
        var result = Result.Success(message, statusCode);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    #endregion

    #region Fail Factory Methods

    [TestMethod]
    public void Fail_WithNoParameters_ReturnsFailedResult()
    {
        // Act
        var result = Result.Fail();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsEmpty(result.Messages);
        Assert.IsNull(result.StatusCode);
    }

    [TestMethod]
    public void Fail_WithMessage_ReturnsFailedResultWithMessage()
    {
        // Arrange
        var message = "Operation failed";

        // Act
        var result = Result.Fail(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
        Assert.IsNull(result.StatusCode);
    }

    [TestMethod]
    public void Fail_WithMessages_ReturnsFailedResultWithMessages()
    {
        // Arrange
        var messages = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result.Fail(messages);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.HasCount(3, result.Messages);
        CollectionAssert.AreEqual(messages, result.Messages);
        Assert.IsNull(result.StatusCode);
    }

    [TestMethod]
    public void Fail_WithMessageAndStatusCode_ReturnsFailedResultWithBoth()
    {
        // Arrange
        var message = "Internal server error";
        var statusCode = HttpStatusCode.InternalServerError;

        // Act
        var result = Result.Fail(message, statusCode);

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
        var result = Result.Fail(statusCode);

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
        var result = await Result.SuccessAsync();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsEmpty(result.Messages);
    }

    [TestMethod]
    public async Task SuccessAsync_WithMessage_ReturnsSucceededResultWithMessage()
    {
        // Arrange
        var message = "Async success";

        // Act
        var result = await Result.SuccessAsync(message);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(1, result.Messages);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public async Task SuccessAsync_WithStatusCode_ReturnsSucceededResultWithStatusCode()
    {
        // Arrange
        var statusCode = HttpStatusCode.Accepted;

        // Act
        var result = await Result.SuccessAsync(statusCode);

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(statusCode, result.StatusCode);
    }

    [TestMethod]
    public async Task FailAsync_WithNoParameters_ReturnsFailedResult()
    {
        // Act
        var result = await Result.FailAsync();

        // Assert
        Assert.IsFalse(result.Succeeded);
    }

    [TestMethod]
    public async Task FailAsync_WithMessage_ReturnsFailedResultWithMessage()
    {
        // Arrange
        var message = "Async failure";

        // Act
        var result = await Result.FailAsync(message);

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
        var result = await Result.FailAsync(messages);

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
        var result = await Result.FailAsync(message, statusCode);

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
        var result = Result.Unauthorized();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual("Unauthorized", result.Messages[0]);
    }

    [TestMethod]
    public void Unauthorized_WithMessage_ReturnsUnauthorizedResultWithCustomMessage()
    {
        // Arrange
        var message = "Invalid token";

        // Act
        var result = Result.Unauthorized(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void NotFound_WithNoMessage_ReturnsNotFoundResult()
    {
        // Act
        var result = Result.NotFound();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("Not found", result.Messages[0]);
    }

    [TestMethod]
    public void NotFound_WithMessage_ReturnsNotFoundResultWithCustomMessage()
    {
        // Arrange
        var message = "User with ID 123 not found";

        // Act
        var result = Result.NotFound(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void BadRequest_WithNoMessage_ReturnsBadRequestResult()
    {
        // Act
        var result = Result.BadRequest();

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
        var result = Result.BadRequest(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void NoContent_ReturnsNoContentResult()
    {
        // Act
        var result = Result.NoContent();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        Assert.IsEmpty(result.Messages);
    }

    [TestMethod]
    public void Cancelled_WithNoMessage_ReturnsCancelledResult()
    {
        // Act
        var result = Result.Cancelled();

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
        var result = Result.Cancelled(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsNull(result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    [TestMethod]
    public void ServiceUnavailable_WithNoMessage_ReturnsServiceUnavailableResult()
    {
        // Act
        var result = Result.ServiceUnavailable();

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
        var result = Result.ServiceUnavailable(message);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
        Assert.AreEqual(message, result.Messages[0]);
    }

    #endregion

    #region Interface Implementation

    [TestMethod]
    public void Result_ImplementsIResult()
    {
        // Act
        var result = new Result();

        // Assert
        Assert.IsInstanceOfType<IResult>(result);
    }

    [TestMethod]
    public void Result_MessagesProperty_IsInitializedAsEmptyList()
    {
        // Act
        var result = new Result();

        // Assert
        Assert.IsNotNull(result.Messages);
        Assert.IsEmpty(result.Messages);
    }

    [TestMethod]
    public void Result_SucceededProperty_IsFalseByDefault()
    {
        // Act
        var result = new Result();

        // Assert
        Assert.IsFalse(result.Succeeded);
    }

    [TestMethod]
    public void Result_StatusCodeProperty_IsNullByDefault()
    {
        // Act
        var result = new Result();

        // Assert
        Assert.IsNull(result.StatusCode);
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void Fail_WithEmptyMessage_ReturnsFailedResultWithEmptyMessage()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var result = Result.Fail(message);

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
        var result = Result.Fail(messages);

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsEmpty(result.Messages);
    }

    [TestMethod]
    public void Success_AllHttpStatusCodes_CanBeUsed()
    {
        // Test various HTTP status codes to ensure they work correctly
        var successCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted };

        foreach (var code in successCodes)
        {
            var result = Result.Success(code);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(code, result.StatusCode);
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
            var result = Result.Fail(code);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(code, result.StatusCode);
        }
    }

    #endregion
}

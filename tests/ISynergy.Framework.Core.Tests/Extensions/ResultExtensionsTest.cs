using ISynergy.Framework.Core.Models.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace ISynergy.Framework.Core.Extensions.Tests;

[TestClass]
public class ResultExtensionsTest
{
    #region Match Method Tests

    [TestMethod]
    public void Match_OnSuccess_ReturnsSuccessResult()
    {
        // Arrange - Success without data just sets Succeeded=true
        var result = Result<string>.Success();

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            () => "Failure"
        );

        // Assert - Data is null when using Success() without data
        Assert.AreEqual($"Success: {null}", output);
    }

    [TestMethod]
    public void Match_OnSuccess_ReturnsSuccessResultWithData()
    {
        // Arrange
        var result = Result<string>.Success("sample", "Success");

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            () => "Failure"
        );

        // Assert
        Assert.AreEqual("Success: sample", output);
    }

    [TestMethod]
    public void Match_OnFailure_ReturnsFailureResult()
    {
        // Arrange
        var result = Result<string>.Fail("Error");

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            () => "Failure"
        );

        // Assert
        Assert.AreEqual("Failure", output);
    }

    [TestMethod]
    public void Match_OnFailureWithStatusCode_ReturnsFailureResult()
    {
        // Arrange
        var result = Result<string>.NotFound("Entity not found");

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            () => "Failure"
        );

        // Assert
        Assert.AreEqual("Failure", output);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public void Match_WithComplexType_ReturnsTransformedResult()
    {
        // Arrange
        var testData = new TestDto { Id = 42, Name = "Test" };
        var result = Result<TestDto>.Success(testData);

        // Act
        var output = result.Match(
            dto => dto?.Id ?? 0,
            () => -1
        );

        // Assert
        Assert.AreEqual(42, output);
    }

    [TestMethod]
    public void Match_WithNullData_ReturnsSuccessHandlerWithNull()
    {
        // Arrange
        var result = Result<string>.Success();

        // Act
        var output = result.Match(
            value => value ?? "null value",
            () => "Failure"
        );

        // Assert
        Assert.AreEqual("null value", output);
    }

    [TestMethod]
    public void Match_CanReturnDifferentType()
    {
        // Arrange
        var result = Result<int>.Success(100);

        // Act
        var output = result.Match(
            value => value > 50,
            () => false
        );

        // Assert
        Assert.IsTrue(output);
    }

    [TestMethod]
    public void Match_CanReturnComplexType()
    {
        // Arrange
        var result = Result<int>.Success(5);

        // Act
        var output = result.Match(
            value => new TestDto { Id = value, Name = $"Item {value}" },
            () => new TestDto { Id = 0, Name = "Default" }
        );

        // Assert
        Assert.AreEqual(5, output.Id);
        Assert.AreEqual("Item 5", output.Name);
    }

    [TestMethod]
    public void Match_OnFailure_CanProvideDefaultComplexType()
    {
        // Arrange
        var result = Result<int>.Fail("Error");

        // Act
        var output = result.Match(
            value => new TestDto { Id = value, Name = $"Item {value}" },
            () => new TestDto { Id = -1, Name = "Error" }
        );

        // Assert
        Assert.AreEqual(-1, output.Id);
        Assert.AreEqual("Error", output.Name);
    }

    #endregion

    #region PaginatedResult Match Tests

    [TestMethod]
    public void Match_PaginatedResult_OnSuccess_ReturnsSuccessResult()
    {
        // Arrange
        var data = new List<string> { "Item1", "Item2", "Item3" };
        var result = PaginatedResult<string>.Success(data, 3, 1, 10);

        // Act
        var output = result.Match(
            items => items?.Count() ?? 0,
            () => -1
        );

        // Assert
        Assert.AreEqual(3, output);
    }

    [TestMethod]
    public void Match_PaginatedResult_OnFailure_ReturnsFailureResult()
    {
        // Arrange
        var result = PaginatedResult<string>.Failure(["Error retrieving data"]);

        // Act
        var output = result.Match(
            items => items?.Count() ?? 0,
            () => -1
        );

        // Assert
        Assert.AreEqual(-1, output);
    }

    #endregion

    #region HTTP Status Code Specific Tests

    [TestMethod]
    public void Match_UnauthorizedResult_CanBeHandledProperly()
    {
        // Arrange
        var result = Result<string>.Unauthorized("Token expired");

        // Act
        var (isSuccess, statusCode) = result.Match(
            value => (true, (HttpStatusCode?)null),
            () => (false, result.StatusCode)
        );

        // Assert
        Assert.IsFalse(isSuccess);
        Assert.AreEqual(HttpStatusCode.Unauthorized, statusCode);
    }

    [TestMethod]
    public void Match_ServiceUnavailableResult_CanBeHandledProperly()
    {
        // Arrange
        var result = Result<TestDto>.ServiceUnavailable("Database offline");

        // Act
        var shouldRetry = result.Match(
            _ => false,
            () => result.StatusCode == HttpStatusCode.ServiceUnavailable
        );

        // Assert
        Assert.IsTrue(shouldRetry);
    }

    [TestMethod]
    public void Match_NoContentResult_IsSuccessWithNoData()
    {
        // Arrange
        var result = Result<string>.NoContent();

        // Act
        var output = result.Match(
            value => value ?? "empty",
            () => "failure"
        );

        // Assert
        Assert.AreEqual("empty", output);
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
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

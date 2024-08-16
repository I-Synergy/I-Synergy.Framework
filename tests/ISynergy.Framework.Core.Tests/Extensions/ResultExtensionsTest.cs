using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests;

[TestClass]
public class ResultExtensionsTest
{
    [TestMethod]
    public void Match_OnSuccess_ReturnsSuccessResult()
    {
        // Arrange
        var result = Result<string>.Success("Success");

        // Act
        var output = result.Match(
            value => $"Success: {value}",
            () => "Failure"
        );

        // Assert
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
}

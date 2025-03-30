using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Core.Extensions.Tests;

[TestClass]
public class ContextExtensionsTests
{
    private Mock<IContext> _contextMock;
    private Mock<IProfile> _profileMock;

    private Token _token;

    private const string TestRefreshToken = "test-refresh-token";

    public ContextExtensionsTests()
    {
        _contextMock = new Mock<IContext>();
        _profileMock = new Mock<IProfile>();

        _token = new Token();
        _token.RefreshToken = TestRefreshToken;

        _profileMock.Setup(p => p.Token).Returns(_token);
        _contextMock.Setup(c => c.Profile).Returns(_profileMock.Object);
    }

    [TestMethod]
    public void ToEnvironmentalRefreshToken_Production_ReturnsOriginalToken()
    {
        // Arrange
        _contextMock.Setup(c => c.Environment).Returns(SoftwareEnvironments.Production);

        // Act
        var result = _contextMock.Object.ToEnvironmentalRefreshToken();

        // Assert
        Assert.AreEqual(TestRefreshToken, result);
    }

    [TestMethod]
    public void ToEnvironmentalRefreshToken_Test_ReturnsPrefixedToken()
    {
        // Arrange
        _contextMock.Setup(c => c.Environment).Returns(SoftwareEnvironments.Test);
        var expected = $"{GenericConstants.UsernamePrefixTest}{TestRefreshToken}";

        // Act
        var result = _contextMock.Object.ToEnvironmentalRefreshToken();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToEnvironmentalRefreshToken_Local_ReturnsPrefixedToken()
    {
        // Arrange
        _contextMock.Setup(c => c.Environment).Returns(SoftwareEnvironments.Local);
        var expected = $"{GenericConstants.UsernamePrefixLocal}{TestRefreshToken}";

        // Act
        var result = _contextMock.Object.ToEnvironmentalRefreshToken();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToEnvironmentalRefreshToken_NullProfile_ReturnsEmptyString()
    {
        // Arrange
        _contextMock.Setup(c => c.Profile).Returns((IProfile)null!);
        _contextMock.Setup(c => c.Environment).Returns(SoftwareEnvironments.Production);

        // Act
        var result = _contextMock.Object.ToEnvironmentalRefreshToken();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToEnvironmentalRefreshToken_NullToken_ReturnsEmptyString()
    {
        // Arrange
        _profileMock.Setup(p => p.Token).Returns((Token)null!);
        _contextMock.Setup(c => c.Environment).Returns(SoftwareEnvironments.Production);

        // Act
        var result = _contextMock.Object.ToEnvironmentalRefreshToken();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }
}

using ISynergy.Framework.AspNetCore.Globalization.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace ISynergy.Framework.AspNetCore.Globalization.Providers.Tests;

[TestClass]
public class RouteDataRequestCultureProviderTests
{
    private Mock<IOptions<GlobalizationOptions>> _mockOptions;
    private RouteDataRequestCultureProvider _provider;

    public RouteDataRequestCultureProviderTests()
    {
        _mockOptions = new Mock<IOptions<GlobalizationOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(new GlobalizationOptions
        {
            DefaultCulture = "en-US",
            SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" }
        });

        _provider = new RouteDataRequestCultureProvider(_mockOptions.Object);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithSupportedCulture_ReturnsCultureResult()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/nl-NL/home";

        // Act
        var result = await _provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("nl-NL", result.Cultures[0].Value);
        Assert.AreEqual("nl-NL", result.UICultures[0].Value);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithUnsupportedCulture_ReturnsDefaultCulture()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/de-DE/home";

        // Act
        var result = await _provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("en-US", result.Cultures[0].Value);
        Assert.AreEqual("en-US", result.UICultures[0].Value);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithEmptyPath_ReturnsDefaultCulture()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "";

        // Act
        var result = await _provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("en-US", result.Cultures[0].Value);
        Assert.AreEqual("en-US", result.UICultures[0].Value);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithRootPath_ReturnsDefaultCulture()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/";

        // Act
        var result = await _provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("en-US", result.Cultures[0].Value);
        Assert.AreEqual("en-US", result.UICultures[0].Value);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithNoSegments_ReturnsDefaultCulture()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "///";

        // Act
        var result = await _provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("en-US", result.Cultures[0].Value);
        Assert.AreEqual("en-US", result.UICultures[0].Value);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithCaseInsensitiveCulture_ReturnsCultureResult()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/fr-fr/home"; // lowercase

        // Act
        var result = await _provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("fr-fr", result.Cultures[0].Value);
        Assert.AreEqual("fr-fr", result.UICultures[0].Value);
    }

    [TestMethod]
    public async Task DetermineProviderCultureResult_WithExceptionThrown_ReturnsDefaultCulture()
    {
        // Arrange
        var mockOptions = new Mock<IOptions<GlobalizationOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new GlobalizationOptions
        {
            DefaultCulture = "en-US",
            SupportedCultures = null! // This will cause an exception when Contains is called
        });

        var provider = new RouteDataRequestCultureProvider(mockOptions.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/fr-FR/home";

        // Act
        var result = await provider.DetermineProviderCultureResult(httpContext);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("en-US", result.Cultures[0].Value);
        Assert.AreEqual("en-US", result.UICultures[0].Value);
    }

    [TestMethod]
    public Task DetermineProviderCultureResult_WithNullHttpContext_ThrowsArgumentNullException()
    {
        // Assert is handled by ExpectedException attribute
        return Assert.ThrowsAsync<ArgumentNullException>(() => _provider.DetermineProviderCultureResult(null!));
    }
}
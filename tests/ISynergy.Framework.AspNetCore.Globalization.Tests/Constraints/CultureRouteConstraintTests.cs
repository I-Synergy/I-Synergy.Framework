using ISynergy.Framework.AspNetCore.Globalization.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.AspNetCore.Globalization.Constraints.Tests;

[TestClass]
public class CultureRouteConstraintTests
{
    private Mock<IOptions<GlobalizationOptions>> _mockOptions;
    private CultureRouteConstraint _constraint;

    public CultureRouteConstraintTests()
    {
        _mockOptions = new Mock<IOptions<GlobalizationOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(new GlobalizationOptions
        {
            DefaultCulture = "en-US",
            SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" }
        });

        _constraint = new CultureRouteConstraint(_mockOptions.Object);
    }

    [TestMethod]
    public void Match_WithSupportedCulture_ReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var route = Mock.Of<IRouter>();
        var routeKey = "culture";
        var values = new RouteValueDictionary(new Dictionary<string, object?>
            {
                { routeKey, "nl-NL" }
            });
        var routeDirection = RouteDirection.IncomingRequest;

        // Act
        var result = _constraint.Match(httpContext, route, routeKey, values, routeDirection);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Match_WithUnsupportedCulture_ReturnsFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var route = Mock.Of<IRouter>();
        var routeKey = "culture";
        var values = new RouteValueDictionary(new Dictionary<string, object?>
            {
                { routeKey, "de-DE" }
            });
        var routeDirection = RouteDirection.IncomingRequest;

        // Act
        var result = _constraint.Match(httpContext, route, routeKey, values, routeDirection);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Match_WithMissingRouteValue_ReturnsFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var route = Mock.Of<IRouter>();
        var routeKey = "culture";
        var values = new RouteValueDictionary();
        var routeDirection = RouteDirection.IncomingRequest;

        // Act
        var result = _constraint.Match(httpContext, route, routeKey, values, routeDirection);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Match_WithNullRouteValue_ReturnsFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var route = Mock.Of<IRouter>();
        var routeKey = "culture";
        var values = new RouteValueDictionary(new Dictionary<string, object?>
            {
                { routeKey, null }
            });
        var routeDirection = RouteDirection.IncomingRequest;

        // Act
        var result = _constraint.Match(httpContext, route, routeKey, values, routeDirection);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Match_WithCaseInsensitiveCulture_ReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var route = Mock.Of<IRouter>();
        var routeKey = "culture";
        var values = new RouteValueDictionary(new Dictionary<string, object?>
            {
                { routeKey, "en-us" } // lowercase
            });
        var routeDirection = RouteDirection.IncomingRequest;

        // Act
        var result = _constraint.Match(httpContext, route, routeKey, values, routeDirection);

        // Assert
        Assert.IsTrue(result);
    }
}
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Tests verifying that root page replacement properly disposes old pages and ViewModels.
/// Note: <see cref="Microsoft.Maui.Controls.Page"/> cannot be mocked in unit tests
/// because its static constructors require MAUI platform initialization. These tests
/// verify the guard clauses and null-safety of <c>CleanupPage</c>.
///
/// For full disposal-chain verification, run the Sample.Maui app and exercise the
/// sign-in/sign-out cycle (see verification section in the plan).
/// </summary>
[TestClass]
public class NavigationServiceCleanupTests
{
    private MethodInfo _cleanupMethod = null!;

    [TestInitialize]
    public void Setup()
    {
        // Initialize ServiceLocator with minimal mock for NavigationService constructor
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockExceptionHandler = new Mock<IExceptionHandlerService>();
        var mockLocalSettings = new Mock<ILocalSettings>();
        var mockLogger = new Mock<ILogger<NavigationService>>();

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(mockExceptionHandler.Object);

        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var mockServiceScope = new Mock<IServiceScope>();
        mockServiceScope.SetupGet(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockServiceScopeFactory.Object);

        ServiceLocator.SetLocatorProvider(mockServiceProvider.Object);

        _cleanupMethod = typeof(NavigationService)
            .GetMethod("CleanupPage", BindingFlags.NonPublic | BindingFlags.Static)!;
    }

    [TestCleanup]
    public void Cleanup()
    {
        ServiceLocator.Default.Dispose();
    }

    /// <summary>
    /// Verifies that CleanupPage is safe to call with a null page (no-op).
    /// </summary>
    [TestMethod]
    public void CleanupPage_WithNullPage_DoesNotThrow()
    {
        // Act & Assert — should not throw
        _cleanupMethod.Invoke(null, new object?[] { null });
    }

    /// <summary>
    /// Verifies that CleanupPage is callable — confirms the method exists and
    /// has the expected signature (private static void CleanupPage(Page? page)).
    /// </summary>
    [TestMethod]
    public void CleanupPage_HasExpectedSignature()
    {
        Assert.IsNotNull(_cleanupMethod, "CleanupPage method should exist");
        Assert.IsTrue(_cleanupMethod.IsPrivate, "Should be private");
        Assert.IsTrue(_cleanupMethod.IsStatic, "Should be static");

        var parameters = _cleanupMethod.GetParameters();
        Assert.AreEqual(1, parameters.Length, "Should take one parameter");
        Assert.AreEqual(typeof(Page), parameters[0].ParameterType, "Parameter should be Page?");
    }
}

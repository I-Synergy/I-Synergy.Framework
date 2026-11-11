using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Coverage-focused tests for the guard / early-return paths of
/// <see cref="NavigationService"/> that do not require a live MAUI window:
/// <list type="bullet">
///   <item><c>NavigateModalAsync</c> returns immediately when there is no
///         <c>Application.Current</c>,</item>
///   <item><c>PresentModalPageAsync</c> disposes the freshly created scope and throws
///         when the main page is unavailable.</item>
/// </list>
/// In the headless test host <c>Application.Current</c> is always <c>null</c>, which is
/// exactly the branch these tests assert on.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class NavigationServiceGuardCoverageTests
{
    private NavigationService _navigationService = null!;

    [TestInitialize]
    public void Setup()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockExceptionHandler = new Mock<IExceptionHandlerService>();
        var mockCommonServices = new Mock<ICommonServices>();
        var mockLocalSettings = new Mock<ILocalSettings>();
        var mockLogger = new Mock<ILogger<NavigationService>>();

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(mockExceptionHandler.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        mockScope.SetupGet(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

        _navigationService = new NavigationService(
            mockExceptionHandler.Object,
            mockCommonServices.Object,
            mockLocalSettings.Object,
            mockScopeFactory.Object,
            mockLogger.Object);
    }

    [TestMethod]
    public async Task NavigateModalAsync_WithoutApplication_ReturnsWithoutThrowing()
    {
        // Application.Current is null in the headless host -> early return.
        Exception? observed = null;
        try
        {
            await _navigationService.NavigateModalAsync<IViewModel>();
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsNull(observed);
    }

    [TestMethod]
    public async Task PresentModalPageAsync_WithoutMainPage_DisposesScopeAndThrows()
    {
        var method = typeof(NavigationService).GetMethod(
            "PresentModalPageAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "PresentModalPageAsync should exist");

        var scope = new Mock<IServiceScope>();

        // GetMainPage() returns null (no Application.Current), so the page argument is
        // never dereferenced before the guard throws.
        var task = (Task)method.Invoke(_navigationService, new object?[] { null!, scope.Object })!;

        Exception? observed = null;
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            observed = ex;
        }

        Assert.IsInstanceOfType<InvalidOperationException>(observed);
        scope.Verify(x => x.Dispose(), Times.Once,
            "The orphaned scope must be disposed before the guard throws.");
    }

    [TestMethod]
    public void PresentModalPageAsync_HasExpectedSignature()
    {
        var method = typeof(NavigationService).GetMethod(
            "PresentModalPageAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        Assert.IsTrue(method!.IsPrivate);
        Assert.AreEqual(typeof(Task), method.ReturnType);
        Assert.HasCount(2, method.GetParameters());
    }
}

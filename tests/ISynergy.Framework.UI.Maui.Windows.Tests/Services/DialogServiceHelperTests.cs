using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Tests for the helper methods extracted from <see cref="DialogService"/> during the
/// error-handling refactor: exception classification
/// (<c>IsRecoverableDialogException</c> / <c>IsCriticalDialogException</c>) and the
/// safe exception-routing helper (<c>HandleExceptionSafely</c>).
///
/// These members are private, so they are reached via reflection — the same approach
/// used elsewhere in this test project (see <c>NavigationServiceCleanupTests</c>).
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DialogServiceHelperTests
{
    private Mock<IExceptionHandlerService> _mockExceptionHandler = null!;
    private Mock<ILogger<DialogService>> _mockLogger = null!;
    private DialogService _dialogService = null!;

    private static readonly MethodInfo s_isRecoverable =
        typeof(DialogService).GetMethod("IsRecoverableDialogException", BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly MethodInfo s_isCritical =
        typeof(DialogService).GetMethod("IsCriticalDialogException", BindingFlags.NonPublic | BindingFlags.Static)!;

    [TestInitialize]
    public void Setup()
    {
        var mockLocalSettings = new Mock<ILocalSettings>();
        var mockLanguageService = new Mock<ILanguageService>();
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
        _mockLogger = new Mock<ILogger<DialogService>>();

        mockLanguageService
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(s => s);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        mockScope.SetupGet(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

        _dialogService = new DialogService(
            mockScopeFactory.Object,
            mockLocalSettings.Object,
            _mockExceptionHandler.Object,
            mockLanguageService.Object,
            _mockLogger.Object);
    }

    private static bool InvokeClassifier(MethodInfo method, Exception exception) =>
        (bool)method.Invoke(null, new object?[] { exception })!;

    // ── IsRecoverableDialogException ─────────────────────────────────────────

    [TestMethod]
    public void IsRecoverableDialogException_IsPrivateStaticWithSignature()
    {
        Assert.IsNotNull(s_isRecoverable, "IsRecoverableDialogException should exist");
        Assert.IsTrue(s_isRecoverable.IsPrivate);
        Assert.IsTrue(s_isRecoverable.IsStatic);
        Assert.AreEqual(typeof(bool), s_isRecoverable.ReturnType);
        Assert.HasCount(1, s_isRecoverable.GetParameters());
        Assert.AreEqual(typeof(Exception), s_isRecoverable.GetParameters()[0].ParameterType);
    }

    [TestMethod]
    public void IsRecoverableDialogException_UiThreadInvalidOperation_ReturnsTrue()
    {
        var ex = new InvalidOperationException("Cannot update off the UI thread");
        Assert.IsTrue(InvokeClassifier(s_isRecoverable, ex));
    }

    [TestMethod]
    public void IsRecoverableDialogException_MainPageInvalidOperation_ReturnsTrue()
    {
        var ex = new InvalidOperationException("MainPage was not set");
        Assert.IsTrue(InvokeClassifier(s_isRecoverable, ex));
    }

    [TestMethod]
    public void IsRecoverableDialogException_Timeout_ReturnsTrue()
    {
        Assert.IsTrue(InvokeClassifier(s_isRecoverable, new TimeoutException("slow")));
    }

    [TestMethod]
    public void IsRecoverableDialogException_OtherInvalidOperation_ReturnsFalse()
    {
        var ex = new InvalidOperationException("Something else entirely");
        Assert.IsFalse(InvokeClassifier(s_isRecoverable, ex));
    }

    [TestMethod]
    public void IsRecoverableDialogException_Argument_ReturnsFalse()
    {
        Assert.IsFalse(InvokeClassifier(s_isRecoverable, new ArgumentException("bad")));
    }

    // ── IsCriticalDialogException ────────────────────────────────────────────

    [TestMethod]
    public void IsCriticalDialogException_IsPrivateStaticWithSignature()
    {
        Assert.IsNotNull(s_isCritical, "IsCriticalDialogException should exist");
        Assert.IsTrue(s_isCritical.IsPrivate);
        Assert.IsTrue(s_isCritical.IsStatic);
        Assert.AreEqual(typeof(bool), s_isCritical.ReturnType);
        Assert.HasCount(1, s_isCritical.GetParameters());
    }

    [TestMethod]
    public void IsCriticalDialogException_OutOfMemory_ReturnsTrue()
    {
        Assert.IsTrue(InvokeClassifier(s_isCritical, new OutOfMemoryException()));
    }

    [TestMethod]
    public void IsCriticalDialogException_AccessViolation_ReturnsTrue()
    {
        Assert.IsTrue(InvokeClassifier(s_isCritical, new AccessViolationException()));
    }

    [TestMethod]
    public void IsCriticalDialogException_AppDomainUnloaded_ReturnsTrue()
    {
        Assert.IsTrue(InvokeClassifier(s_isCritical, new AppDomainUnloadedException()));
    }

    [TestMethod]
    public void IsCriticalDialogException_WinRtCurrentState_ReturnsTrue()
    {
        var ex = new Exception("The object is in the current state")
        {
            Source = "WinRT.Runtime"
        };

        Assert.IsTrue(InvokeClassifier(s_isCritical, ex));
    }

    [TestMethod]
    public void IsCriticalDialogException_WinRtOtherMessage_ReturnsFalse()
    {
        var ex = new Exception("Some other failure")
        {
            Source = "WinRT.Runtime"
        };

        Assert.IsFalse(InvokeClassifier(s_isCritical, ex));
    }

    [TestMethod]
    public void IsCriticalDialogException_OrdinaryException_ReturnsFalse()
    {
        Assert.IsFalse(InvokeClassifier(s_isCritical, new InvalidOperationException("ordinary")));
    }

    // ── HandleExceptionSafely ────────────────────────────────────────────────

    [TestMethod]
    public void HandleExceptionSafely_WithWorkingHandler_RoutesException()
    {
        var method = typeof(DialogService).GetMethod(
            "HandleExceptionSafely", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.IsNotNull(method, "HandleExceptionSafely should exist");

        var ex = new InvalidOperationException("boom");

        // Act — must not throw.
        method.Invoke(_dialogService, new object?[] { ex, "handler failed", "handler not available" });

        _mockExceptionHandler.Verify(x => x.HandleException(ex), Times.Once);
    }

    [TestMethod]
    public void HandleExceptionSafely_WhenHandlerThrows_SwallowsAndDoesNotRethrow()
    {
        var method = typeof(DialogService).GetMethod(
            "HandleExceptionSafely", BindingFlags.NonPublic | BindingFlags.Instance)!;

        _mockExceptionHandler
            .Setup(x => x.HandleException(It.IsAny<Exception>()))
            .Throws(new InvalidOperationException("handler crashed"));

        Exception? observed = null;
        try
        {
            method.Invoke(_dialogService, new object?[] { new Exception("boom"), "handler failed", "handler not available" });
        }
        catch (TargetInvocationException tie)
        {
            observed = tie.InnerException;
        }

        // The helper must never propagate — app must not crash on a failed handler.
        Assert.IsNull(observed);

        // The handler is still attempted.
        _mockExceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>()), Times.Once);
    }
}

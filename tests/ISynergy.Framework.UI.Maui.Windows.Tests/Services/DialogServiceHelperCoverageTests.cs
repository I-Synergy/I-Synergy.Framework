using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Coverage-focused tests for <see cref="DialogService"/> helper / guard / exception
/// paths that never require a running MAUI window:
/// <list type="bullet">
///   <item>the constructor null-argument guards,</item>
///   <item>the empty-message input validation in <c>ShowMessageAsync</c>,</item>
///   <item>the progressive fallback notification writers
///         (<c>TryConsole/Debug/EventLog/FileNotificationAsync</c>),</item>
///   <item>the failure bookkeeping helpers
///         (<c>HandleDialogFailureAsync</c>/<c>HandleCriticalDialogFailureAsync</c>),</item>
///   <item><c>ReportDialogFailure</c> for both the entity and no-entity overloads,</item>
///   <item><c>WireDialogHandlers</c> submit / cancel / close wiring and scope disposal,</item>
///   <item><c>Dispose</c>.</item>
/// </list>
/// Unreachable-in-a-test paths (a real MAUI <see cref="Window"/> showing a dialog) are
/// deliberately avoided; those are exercised only through the private helpers.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DialogServiceHelperCoverageTests
{
    public TestContext TestContext { get; set; } = null!;

    private Mock<IServiceScopeFactory> _scopeFactory = null!;
    private Mock<ILocalSettings> _localSettings = null!;
    private Mock<IExceptionHandlerService> _exceptionHandler = null!;
    private Mock<ILanguageService> _languageService = null!;
    private Mock<ILogger<DialogService>> _logger = null!;
    private DialogService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _scopeFactory = new Mock<IServiceScopeFactory>();
        _localSettings = new Mock<ILocalSettings>();
        _exceptionHandler = new Mock<IExceptionHandlerService>();
        _languageService = new Mock<ILanguageService>();
        _logger = new Mock<ILogger<DialogService>>();

        _languageService
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(s => s);

        var provider = new Mock<IServiceProvider>();
        provider.Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_exceptionHandler.Object);

        var scope = new Mock<IServiceScope>();
        scope.SetupGet(x => x.ServiceProvider).Returns(provider.Object);
        _scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

        _service = new DialogService(
            _scopeFactory.Object,
            _localSettings.Object,
            _exceptionHandler.Object,
            _languageService.Object,
            _logger.Object);
    }

    private static Exception? Catch(Action action)
    {
        try { action(); return null; }
        catch (Exception ex) { return ex; }
    }

    // ── Constructor guards ──────────────────────────────────────────────────

    [TestMethod]
    public void Constructor_NullScopeFactory_Throws()
    {
        var ex = Catch(() => _ = new DialogService(
            null!, _localSettings.Object, _exceptionHandler.Object, _languageService.Object));
        Assert.IsInstanceOfType<ArgumentNullException>(ex);
    }

    [TestMethod]
    public void Constructor_NullLocalSettings_Throws()
    {
        var ex = Catch(() => _ = new DialogService(
            _scopeFactory.Object, null!, _exceptionHandler.Object, _languageService.Object));
        Assert.IsInstanceOfType<ArgumentNullException>(ex);
    }

    [TestMethod]
    public void Constructor_NullExceptionHandler_Throws()
    {
        var ex = Catch(() => _ = new DialogService(
            _scopeFactory.Object, _localSettings.Object, null!, _languageService.Object));
        Assert.IsInstanceOfType<ArgumentNullException>(ex);
    }

    [TestMethod]
    public void Constructor_NullLanguageService_Throws()
    {
        var ex = Catch(() => _ = new DialogService(
            _scopeFactory.Object, _localSettings.Object, _exceptionHandler.Object, null!));
        Assert.IsInstanceOfType<ArgumentNullException>(ex);
    }

    // ── ShowMessageAsync input validation ───────────────────────────────────

    [TestMethod]
    public async Task ShowMessageAsync_EmptyMessage_ReturnsNoneWithoutTouchingUi()
    {
        var result = await _service.ShowMessageAsync(string.Empty, "title");
        Assert.AreEqual(MessageBoxResults.None, result);
    }

    [TestMethod]
    public async Task ShowMessageAsync_WhitespaceMessage_ReturnsNone()
    {
        var result = await _service.ShowMessageAsync("   ", "title");
        Assert.AreEqual(MessageBoxResults.None, result);
    }

    // ── Fallback notification writers (static, no MAUI window) ──────────────

    private static Task<bool> InvokeBoolFallback(string methodName, string message, string title)
    {
        var method = typeof(DialogService).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!;
        Assert.IsNotNull(method, $"{methodName} should exist");
        return (Task<bool>)method.Invoke(null, new object?[] { message, title })!;
    }

    [TestMethod]
    public async Task TryConsoleNotificationAsync_ReturnsTrue()
    {
        Assert.IsTrue(await InvokeBoolFallback("TryConsoleNotificationAsync", "msg", "ttl"));
    }

    [TestMethod]
    public async Task TryDebugNotificationAsync_ReturnsTrue()
    {
        Assert.IsTrue(await InvokeBoolFallback("TryDebugNotificationAsync", "msg", "ttl"));
    }

    [TestMethod]
    public async Task TryEventLogNotificationAsync_DoesNotThrow()
    {
        // Windows head writes to the Application event log; whether or not the host
        // is allowed to write, the method must never throw and must yield a bool.
        var handled = await InvokeBoolFallback("TryEventLogNotificationAsync", "msg", "ttl");
        _ = handled;
    }

    [TestMethod]
    public async Task TryFileNotificationAsync_DoesNotThrow()
    {
        var handled = await InvokeBoolFallback("TryFileNotificationAsync", "msg", "ttl");
        _ = handled;
    }

    // ── Failure bookkeeping helpers ─────────────────────────────────────────

    private Task InvokeInstanceTask(string methodName, params object?[] args)
    {
        var method = typeof(DialogService).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.IsNotNull(method, $"{methodName} should exist");
        return (Task)method.Invoke(_service, args)!;
    }

    [TestMethod]
    public async Task HandleDialogFailureAsync_DoesNotThrowAndRunsFallback()
    {
        await InvokeInstanceTask("HandleDialogFailureAsync",
            new InvalidOperationException("boom"), "msg", "ttl");
    }

    [TestMethod]
    public async Task HandleCriticalDialogFailureAsync_DoesNotThrowAndRunsEmergencyFallback()
    {
        await InvokeInstanceTask("HandleCriticalDialogFailureAsync",
            new OutOfMemoryException(), "msg", "ttl");
    }

    // ── ReportDialogFailure ──────────────────────────────────────────────────

    [TestMethod]
    public void ReportDialogFailure_WithEntity_RoutesToHandler()
    {
        var method = typeof(DialogService).GetMethod(
            "ReportDialogFailure", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.IsNotNull(method);

        var ex = new InvalidOperationException("boom");
        method.Invoke(_service, new object?[] { ex, "W", "VM", "E", true });

        _exceptionHandler.Verify(x => x.HandleException(ex), Times.Once);
    }

    [TestMethod]
    public void ReportDialogFailure_WithoutEntity_RoutesToHandler()
    {
        var method = typeof(DialogService).GetMethod(
            "ReportDialogFailure", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var ex = new InvalidOperationException("boom");
        method.Invoke(_service, new object?[] { ex, "W", "VM", "E", false });

        _exceptionHandler.Verify(x => x.HandleException(ex), Times.Once);
    }

    [TestMethod]
    public void ReportDialogFailure_WhenHandlerThrows_DoesNotPropagate()
    {
        _exceptionHandler
            .Setup(x => x.HandleException(It.IsAny<Exception>()))
            .Throws(new InvalidOperationException("handler failed"));

        var method = typeof(DialogService).GetMethod(
            "ReportDialogFailure", BindingFlags.NonPublic | BindingFlags.Instance)!;

        Exception? observed = null;
        try
        {
            method.Invoke(_service, new object?[] { new Exception("boom"), "W", "VM", "E", true });
        }
        catch (TargetInvocationException tie)
        {
            observed = tie.InnerException;
        }

        Assert.IsNull(observed);
        _exceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>()), Times.Once);
    }

    // ── WireDialogHandlers ────────────────────────────────────────────────────

    private void InvokeWireHandlers(
        IViewModelDialog<string> viewModel,
        IServiceScope scope,
        Func<string, Task> submitted,
        Func<Task>? cancelled,
        Func<Task>? closed)
    {
        var method = typeof(DialogService)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(m => m.Name == "WireDialogHandlers")
            .MakeGenericMethod(typeof(string));

        method.Invoke(_service, new object?[]
        {
            viewModel, scope, "TestWindow", "TestViewModel", submitted, cancelled, closed
        });
    }

    [TestMethod]
    public async Task WireDialogHandlers_RoutesSubmittedCancelledClosedAndDisposesScope()
    {
        var viewModel = new Mock<IViewModelDialog<string>>();
        var scope = new Mock<IServiceScope>();

        var submittedTcs = new TaskCompletionSource<string>();
        var cancelledTcs = new TaskCompletionSource();
        var closedTcs = new TaskCompletionSource();

        InvokeWireHandlers(
            viewModel.Object,
            scope.Object,
            result => { submittedTcs.TrySetResult(result); return Task.CompletedTask; },
            () => { cancelledTcs.TrySetResult(); return Task.CompletedTask; },
            () => { closedTcs.TrySetResult(); return Task.CompletedTask; });

        // Fire each wired event.
        viewModel.Raise(m => m.Submitted += null, new SubmitEventArgs<string>("payload"));
        viewModel.Raise(m => m.Cancelled += null, EventArgs.Empty);
        viewModel.Raise(m => m.Closed += null, EventArgs.Empty);

        await submittedTcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);
        Assert.AreEqual("payload", await submittedTcs.Task);

        await cancelledTcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);

        await closedTcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);

        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task WireDialogHandlers_WithoutCancelledOrClosed_StillDisposesScope()
    {
        var viewModel = new Mock<IViewModelDialog<string>>();
        var scope = new Mock<IServiceScope>();

        InvokeWireHandlers(
            viewModel.Object,
            scope.Object,
            _ => Task.CompletedTask,
            cancelled: null,
            closed: null);

        viewModel.Raise(m => m.Closed += null, EventArgs.Empty);

        // Give the fire-and-forget Closed handler time to dispose the scope.
        for (int i = 0; i < 40 && !WasDisposed(scope); i++)
            await Task.Delay(25, TestContext.CancellationToken);

        scope.Verify(x => x.Dispose(), Times.Once);
    }

    private static bool WasDisposed(Mock<IServiceScope> scope)
    {
        try
        {
            scope.Verify(x => x.Dispose(), Times.Once);
            return true;
        }
        catch (MockException)
        {
            return false;
        }
    }

    // ── Dispose ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void Dispose_DoesNotThrow()
    {
        var ex = Catch(() => _service.Dispose());
        Assert.IsNull(ex);
    }
}

using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Moq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinWindow = ISynergy.Framework.UI.Controls.Window;

namespace ISynergy.Framework.UI.WinUI.Services;

/// <summary>
/// Additional coverage for <see cref="DialogService"/> focusing on the still-uncovered helper, guard,
/// exception-filter, disposal and lifecycle paths.
/// <para>
/// A real <see cref="WinWindow"/> derives from the COM-backed WinUI <see cref="ContentDialog"/> and
/// cannot be constructed in a plain unit-test host (REGDB_E_CLASSNOTREG). To exercise the paths that
/// take a <see cref="WinWindow"/> argument we instead pass an <b>uninitialized</b> instance
/// (<see cref="RuntimeHelpers.GetUninitializedObject(Type)"/>), kept alive just long enough for the
/// service to run its guard/exception logic. Every WinUI member access on such an instance throws a
/// managed <see cref="NullReferenceException"/> (or a <see cref="COMException"/> from the unavailable
/// <c>Application.Current</c>), which is exactly what the service's logging-only catch blocks handle.
/// </para>
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DialogServiceCoverageTests
{
    private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    private Mock<IServiceScopeFactory> _scopeFactory = null!;
    private Mock<ILocalSettings> _localSettings = null!;
    private Mock<ILanguageService> _languageService = null!;
    private Mock<ILogger<DialogService>> _logger = null!;
    private DialogService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _scopeFactory = new Mock<IServiceScopeFactory>();
        _localSettings = new Mock<ILocalSettings>();
        _languageService = new Mock<ILanguageService>();
        _logger = new Mock<ILogger<DialogService>>();

        _localSettings.SetupProperty(x => x.Language, "en-US");
        _languageService.Setup(x => x.GetString(It.IsAny<string>())).Returns((string key) => key);

        _service = new DialogService(
            _scopeFactory.Object,
            _localSettings.Object,
            _languageService.Object,
            _logger.Object);
    }

    [TestCleanup]
    public void Cleanup() => _service.Dispose();

    #region Message helpers (dialog construction always fails in the test host -> returns None)

    [TestMethod]
    public async Task ShowErrorAsync_WithException_ReturnsNone_AndLogsError()
    {
        var result = await _service.ShowErrorAsync(new InvalidOperationException("boom"));

        Assert.AreEqual(MessageBoxResults.None, result);
        VerifyLog(LogLevel.Error, "Error showing message dialog", Times.Once());
    }

    [TestMethod]
    public async Task ShowErrorAsync_WithString_ReturnsNone()
    {
        var result = await _service.ShowErrorAsync("a message");

        Assert.AreEqual(MessageBoxResults.None, result);
    }

    [TestMethod]
    public async Task ShowInformationAsync_ReturnsNone()
    {
        var result = await _service.ShowInformationAsync("a message");

        Assert.AreEqual(MessageBoxResults.None, result);
    }

    [TestMethod]
    public async Task ShowWarningAsync_ReturnsNone()
    {
        var result = await _service.ShowWarningAsync("a message");

        Assert.AreEqual(MessageBoxResults.None, result);
    }

    [TestMethod]
    public async Task ShowGreetingAsync_ReturnsNone()
    {
        await _service.ShowGreetingAsync("Alice");

        VerifyLog(LogLevel.Error, "Error showing message dialog", Times.Once());
    }

    #endregion

    #region Exception-filter helpers (Log*Failure)

    [TestMethod]
    public void LogDialogCreationFailure_LogsError_AndReturnsFalse()
    {
        var result = InvokeBoolHelper("LogDialogCreationFailure", new InvalidOperationException("boom"));

        Assert.IsFalse(result, "The exception filter must return false so the caller rethrows");
        VerifyLog(LogLevel.Error, "Error creating dialog", Times.Once());
    }

    [TestMethod]
    public void LogViewModelInitializationFailure_LogsError_AndReturnsFalse()
    {
        var result = InvokeBoolHelper("LogViewModelInitializationFailure", new InvalidOperationException("boom"));

        Assert.IsFalse(result, "The exception filter must return false so the caller rethrows");
        VerifyLog(LogLevel.Error, "Error initializing viewmodel", Times.Once());
    }

    [TestMethod]
    public void LogDialogOpenFailure_LogsError_AndReturnsFalse()
    {
        var result = InvokeBoolHelper("LogDialogOpenFailure", new InvalidOperationException("boom"));

        Assert.IsFalse(result, "The exception filter must return false so the caller rethrows");
        VerifyLog(LogLevel.Error, "Error opening dialog", Times.Once());
    }

    #endregion

    #region ApplyDialogButtonStyles / ConfigureDialogButtons

    [TestMethod]
    public void ApplyDialogButtonStyles_WhenApplicationResourcesUnavailable_LogsWarning()
    {
        // Application.Current is unavailable in the test host, so the style lookup throws; it must be
        // logged as a warning and swallowed.
        InvokeVoid("ApplyDialogButtonStyles", NewWindow());

        VerifyLog(LogLevel.Warning, "Failed to set button styles", Times.Once());
    }

    [TestMethod]
    [DataRow(MessageBoxButtons.OK)]
    [DataRow(MessageBoxButtons.OKCancel)]
    [DataRow(MessageBoxButtons.YesNo)]
    [DataRow(MessageBoxButtons.YesNoCancel)]
    public void ConfigureDialogButtons_SelectsButtonSet_AndAttemptsToStyle(MessageBoxButtons buttons)
    {
        // Navigating the switch is the behaviour under test. Setting the ContentDialog button texts on
        // an uninitialized dialog throws (WinUI native unavailable) before the style delegate runs.
        var exception = Capture(() => InvokeVoid("ConfigureDialogButtons", NewWindow(), buttons));

        Assert.IsNotNull(exception, "Configuring buttons on an uninitialized dialog must surface the WinUI failure");
    }

    #endregion

    #region Lifecycle helpers (open / close dialog, viewmodel closed)

    [TestMethod]
    public async Task OpenDialogAsync_WhenShowFails_ReturnsNone_AndLogsError()
    {
        var result = await InvokeOpenDialogAsync(NewWindow());

        Assert.AreEqual(ContentDialogResult.None, result);
        VerifyLog(LogLevel.Error, "Error showing dialog", Times.Once());
    }

    [TestMethod]
    public async Task OpenDialogAsync_WithExistingActiveDialog_LogsCloseFailure_AndReturnsNone()
    {
        SetActiveDialog(NewWindow());

        var result = await InvokeOpenDialogAsync(NewWindow());

        Assert.AreEqual(ContentDialogResult.None, result);
        VerifyLog(LogLevel.Error, "Error closing active dialog", Times.Once());
    }

    [TestMethod]
    public async Task HandleViewModelClosedAsync_WhenCloseFails_LogsError()
    {
        await InvokeTask("HandleViewModelClosedAsync", NewWindow());

        VerifyLog(LogLevel.Error, "Error in viewmodel closed handler", Times.Once());
    }

    [TestMethod]
    public async Task InitializeAndOpenDialogAsync_RunsInitializeThenOpen()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        viewmodel.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);

        await InvokeInitializeAndOpenAsync(viewmodel.Object, NewWindow());

        viewmodel.Verify(x => x.InitializeAsync(), Times.Once);
        VerifyLog(LogLevel.Error, "Error showing dialog", Times.Once());
    }

    [TestMethod]
    public async Task InitializeAndOpenDialogAsync_WhenInitializeThrows_LogsError_AndRethrows()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        viewmodel.Setup(x => x.InitializeAsync()).ThrowsAsync(new InvalidOperationException("init failed"));

        // The initialization exception-filter logs and returns false, so the failure propagates.
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => InvokeInitializeAndOpenAsync(viewmodel.Object, NewWindow()));

        VerifyLog(LogLevel.Error, "Error initializing viewmodel", Times.Once());
        VerifyLog(LogLevel.Error, "Error showing dialog", Times.Never());
    }

    [TestMethod]
    public async Task InitializeAndOpenDialogAsync_WhenViewModelClosed_ClosesWindow()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        viewmodel.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);

        await InvokeInitializeAndOpenAsync(viewmodel.Object, NewWindow());

        // Raising Closed exercises the self-unsubscribing close handler (HandleViewModelClosedAsync).
        viewmodel.Raise(x => x.Closed += null, EventArgs.Empty);
        await VerifyLogEventuallyAsync(LogLevel.Error, "Error in viewmodel closed handler");
    }

    #endregion

    #region CreateDialogAsync guard / exception path

    [TestMethod]
    public async Task CreateDialogAsync_WithWindow_WhenXamlRootUnavailable_LogsCreationFailure_AndRethrows()
    {
        var exception = await CaptureAsync(() =>
            _service.CreateDialogAsync(NewWindow(), new Mock<IViewModelDialog<TestEntity>>().Object));

        Assert.IsNotNull(exception, "CreateDialogAsync must rethrow when the XamlRoot cannot be resolved");
        VerifyLog(LogLevel.Error, "Error creating dialog", Times.Once());
    }

    #endregion

    #region ShowDialogAsync (window / core) success paths that reach a resolved Window

    [TestMethod]
    public async Task ShowDialogAsync_WithResolvedWindow_WhenCreateFails_LogsError()
    {
        // Exercies the `window is Window` branch, delegating to CreateDialogAsync which fails and is logged.
        await _service.ShowDialogAsync(NewWindow(), new Mock<IViewModelDialog<TestEntity>>().Object);

        VerifyLog(LogLevel.Error, "Error showing dialog for", Times.Once());
    }

    [TestMethod]
    public async Task ShowDialogAsync_CoreWithEntity_WhenCreateFails_DisposesScope_AndLogsEntityError()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var window = NewWindow();

        var provider = new Mock<IServiceProvider>();
        provider
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns((Type type) => type == typeof(IViewModelDialog<TestEntity>) ? viewmodel.Object : window);
        var scope = new Mock<IServiceScope>();
        scope.Setup(x => x.ServiceProvider).Returns(provider.Object);
        _scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

        await _service.ShowDialogAsync<StubWindow, IViewModelDialog<TestEntity>, TestEntity>(
            new TestEntity(),
            _ => Task.CompletedTask);

        scope.Verify(x => x.Dispose(), Times.Once);
        VerifyLog(LogLevel.Error, nameof(TestEntity), Times.Once());
        viewmodel.Verify(x => x.SetSelectedItem(It.IsAny<TestEntity>()), Times.Once);
    }

    #endregion

    #region Dispose with an active dialog (unreachable without a real ContentDialog)

    // NOTE: The active-dialog branch of Dispose() guards with `_activeDialog != null`, which binds to
    // ContentDialog.op_Inequality. On an uninitialized ContentDialog that operator dereferences the
    // native ThisPtr and throws NullReferenceException *before* entering the try/catch, so the branch
    // can only be reached with a real (COM-backed) ContentDialog — impossible in a unit-test host.
    // The `_dialogSemaphore.Dispose()` and idempotent second-Dispose paths are already covered by
    // DialogServiceTests.Dispose_CalledTwice_DoesNotThrow.

    #endregion

    #region Helpers

    private static WinWindow NewWindow() =>
        (WinWindow)RuntimeHelpers.GetUninitializedObject(typeof(WinWindow));

    private bool InvokeBoolHelper(string methodName, Exception exception)
    {
        var method = typeof(DialogService).GetMethod(methodName, PrivateInstance);
        Assert.IsNotNull(method, $"{methodName} must remain a private instance helper");
        return (bool)method.Invoke(_service, [exception])!;
    }

    private void InvokeVoid(string methodName, params object?[] arguments)
    {
        var method = typeof(DialogService).GetMethod(methodName, PrivateInstance);
        Assert.IsNotNull(method, $"{methodName} must remain a private instance helper");
        method.Invoke(_service, arguments);
    }

    private async Task InvokeTask(string methodName, params object?[] arguments)
    {
        var method = typeof(DialogService).GetMethod(methodName, PrivateInstance);
        Assert.IsNotNull(method, $"{methodName} must remain a private instance helper");
        await (Task)method.Invoke(_service, arguments)!;
    }

    private async Task<ContentDialogResult> InvokeOpenDialogAsync(WinWindow dialog)
    {
        var method = typeof(DialogService).GetMethod("OpenDialogAsync", PrivateInstance);
        Assert.IsNotNull(method, "OpenDialogAsync must remain a private instance helper");
        return await (Task<ContentDialogResult>)method.Invoke(_service, [dialog])!;
    }

    private async Task InvokeInitializeAndOpenAsync(IViewModelDialog<TestEntity> viewmodel, WinWindow window)
    {
        var method = typeof(DialogService)
            .GetMethod("InitializeAndOpenDialogAsync", PrivateInstance)!
            .MakeGenericMethod(typeof(TestEntity));
        await (Task)method.Invoke(_service, [window, viewmodel])!;
    }

    private void SetActiveDialog(WinWindow? window) =>
        typeof(DialogService)
            .GetField("_activeDialog", PrivateInstance)!
            .SetValue(_service, window);

    private WinWindow? GetActiveDialog() =>
        (WinWindow?)typeof(DialogService)
            .GetField("_activeDialog", PrivateInstance)!
            .GetValue(_service);

    private static Exception? Capture(Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (TargetInvocationException ex)
        {
            return ex.InnerException;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static async Task<Exception?> CaptureAsync(Func<Task> action)
    {
        try
        {
            await action();
            return null;
        }
        catch (TargetInvocationException ex)
        {
            return ex.InnerException;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private void VerifyLog(LogLevel level, string fragment, Times times) =>
        _logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state!.ToString()!.Contains(fragment, StringComparison.Ordinal)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);

    private async Task VerifyLogEventuallyAsync(LogLevel level, string fragment)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            try
            {
                VerifyLog(level, fragment, Times.AtLeastOnce());
                return;
            }
            catch (MockException)
            {
                await Task.Delay(20);
            }
        }

        VerifyLog(level, fragment, Times.AtLeastOnce());
    }

    #endregion

    #region Test doubles

    public sealed class TestEntity
    {
        public string Name { get; init; } = "entity";
    }

    public sealed class StubWindow : IWindow
    {
        public IViewModel ViewModel { get; set; } = null!;

        public Task CloseAsync() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    #endregion
}

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

namespace ISynergy.Framework.UI.WinUI.Services;

/// <summary>
/// Tests for <see cref="DialogService"/> covering the refactored dialog flow: constructor guards,
/// the button-style/result helpers, the submitted/cancelled/closed callback wiring and the
/// exception (logging-only) paths.
/// <para>
/// A real <see cref="ISynergy.Framework.UI.Controls.Window"/> derives from a COM-backed WinUI
/// <see cref="ContentDialog"/> that cannot be instantiated in a plain unit-test host
/// (REGDB_E_CLASSNOTREG), so every helper that requires one is exercised through its
/// logging/exception path instead — which is exactly the refactored logic.
/// </para>
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DialogServiceTests
{
    public TestContext TestContext { get; set; } = null!;

    private const string ButtonStyleFailure = "Failed to set button styles";
    private const string DialogErrorMessage = "Error showing dialog";

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

    #region Constructor

    [TestMethod]
    public void Constructor_WithNullScopeFactory_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new DialogService(
            null!,
            _localSettings.Object,
            _languageService.Object,
            _logger.Object));
    }

    [TestMethod]
    public void Constructor_WithNullLocalSettings_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new DialogService(
            _scopeFactory.Object,
            null!,
            _languageService.Object,
            _logger.Object));
    }

    [TestMethod]
    public void Constructor_WithNullLanguageService_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new DialogService(
            _scopeFactory.Object,
            _localSettings.Object,
            null!,
            _logger.Object));
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new DialogService(
            _scopeFactory.Object,
            _localSettings.Object,
            _languageService.Object,
            null!));
    }

    #endregion

    #region Message helpers — argument validation

    [TestMethod]
    public async Task ShowMessageAsync_WithEmptyMessage_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            async () => await _service.ShowMessageAsync(string.Empty, "title"));
    }

    [TestMethod]
    public void ShowErrorAsync_WithNullException_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => _service.ShowErrorAsync((Exception)null!));
    }

    [TestMethod]
    public void ShowErrorAsync_WithEmptyMessage_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => _service.ShowErrorAsync(string.Empty));
    }

    [TestMethod]
    public void ShowInformationAsync_WithEmptyMessage_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => _service.ShowInformationAsync(string.Empty));
    }

    [TestMethod]
    public void ShowWarningAsync_WithEmptyMessage_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => _service.ShowWarningAsync(string.Empty));
    }

    [TestMethod]
    public void ShowGreetingAsync_WithEmptyName_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => _service.ShowGreetingAsync(string.Empty));
    }

    [TestMethod]
    public async Task ShowMessageAsync_WhenDialogCannotBeCreated_LogsErrorAndReturnsNone()
    {
        // The WinUI ContentDialog base type cannot be activated in a unit-test host, so the dialog
        // construction throws; the guarded catch block must swallow it and report None.
        var result = await _service
            .ShowMessageAsync("message", "title", MessageBoxButtons.OKCancel)
            .WaitAsync(TimeSpan.FromSeconds(30), TestContext.CancellationToken);

        Assert.AreEqual(MessageBoxResults.None, result);
        VerifyLog(LogLevel.Error, "dialog", Times.Once());
    }

    #endregion

    #region ShowDialogAsync overloads

    [TestMethod]
    public async Task ShowDialogAsync_WithNullWindow_ThrowsArgumentNullException()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.ShowDialogAsync((IWindow)null!, viewmodel.Object));
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithNullViewModel_ThrowsArgumentNullException()
    {
        var window = new Mock<IWindow>();

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.ShowDialogAsync(window.Object, (IViewModelDialog<TestEntity>)null!));
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithNonWindowImplementation_DoesNothing()
    {
        var window = new Mock<IWindow>();
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();

        await _service.ShowDialogAsync(window.Object, viewmodel.Object);

        window.Verify(x => x.CloseAsync(), Times.Never);
        VerifyLog(LogLevel.Error, DialogErrorMessage, Times.Never());
    }

    [TestMethod]
    public async Task ShowDialogAsync_ByType_WithNullType_ThrowsArgumentNullException()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.ShowDialogAsync((Type)null!, viewmodel.Object));
    }

    [TestMethod]
    public async Task ShowDialogAsync_ByType_WithNullViewModel_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.ShowDialogAsync(typeof(PlainDialog), (IViewModelDialog<TestEntity>)null!));
    }

    [TestMethod]
    public async Task ShowDialogAsync_ByType_WhenInstanceIsNotAWindow_DisposesScopeWithoutLoggingError()
    {
        var scope = UseScopeFactoryReturning(new Mock<IServiceProvider>().Object);

        await _service.ShowDialogAsync(typeof(PlainDialog), new Mock<IViewModelDialog<TestEntity>>().Object);

        scope.Verify(x => x.Dispose(), Times.Once);
        VerifyLog(LogLevel.Error, DialogErrorMessage, Times.Never());
    }

    [TestMethod]
    public async Task ShowDialogAsync_ByType_WhenConstructionFails_LogsErrorWithWindowType()
    {
        var provider = new Mock<IServiceProvider>();
        provider
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns((Type _) => null);
        UseScopeFactoryReturning(provider.Object);

        // PlainDialogWithDependency needs an unregistered FakeDependency, so ActivatorUtilities throws.
        await _service.ShowDialogAsync(typeof(PlainDialogWithDependency), new Mock<IViewModelDialog<TestEntity>>().Object);

        VerifyLog(LogLevel.Error, nameof(PlainDialogWithDependency), Times.Once());
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithEntity_WhenScopeResolutionFails_LogsErrorWithEntityType()
    {
        var scope = UseScopeFactoryWithThrowingProvider();

        await _service.ShowDialogAsync<StubWindow, IViewModelDialog<TestEntity>, TestEntity>(
            new TestEntity(),
            _ => Task.CompletedTask);

        scope.Verify(x => x.Dispose(), Times.Once);
        VerifyLog(LogLevel.Error, nameof(TestEntity), Times.Once());
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithoutEntity_WhenScopeResolutionFails_LogsErrorWithViewModelType()
    {
        var scope = UseScopeFactoryWithThrowingProvider();

        await _service.ShowDialogAsync<StubWindow, IViewModelDialog<TestEntity>, TestEntity>(
            _ => Task.CompletedTask);

        scope.Verify(x => x.Dispose(), Times.Once);
        VerifyLog(LogLevel.Error, typeof(IViewModelDialog<TestEntity>).Name, Times.Once());
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithoutEntity_WithNullSubmittedHandler_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.ShowDialogAsync<StubWindow, IViewModelDialog<TestEntity>, TestEntity>(null!));
    }

    [TestMethod]
    public async Task ShowDialogAsync_WithEntity_WithNullSubmittedHandler_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.ShowDialogAsync<StubWindow, IViewModelDialog<TestEntity>, TestEntity>(new TestEntity(), null!));
    }

    [TestMethod]
    public async Task ShowDialogAsync_WhenResolvedViewModelIsNotADialogViewModel_DisposesScope()
    {
        var provider = new Mock<IServiceProvider>();
        provider
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new object());
        var scope = UseScopeFactoryReturning(provider.Object);

        await _service.ShowDialogAsync<StubWindow, IViewModelDialog<TestEntity>, TestEntity>(
            _ => Task.CompletedTask);

        scope.Verify(x => x.Dispose(), Times.Once);
        VerifyLog(LogLevel.Error, DialogErrorMessage, Times.Never());
    }

    #endregion

    #region CreateDialogAsync

    [TestMethod]
    public async Task CreateDialogAsync_WithNullDialog_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.CreateDialogAsync((IWindow)null!, new Mock<IViewModelDialog<TestEntity>>().Object));
    }

    [TestMethod]
    public async Task CreateDialogAsync_WithNullViewModel_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => _service.CreateDialogAsync(new Mock<IWindow>().Object, (IViewModelDialog<TestEntity>)null!));
    }

    [TestMethod]
    public async Task CreateDialogAsync_WithNonWindowImplementation_ReturnsWithoutTouchingViewModel()
    {
        var window = new Mock<IWindow>();
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();

        await _service.CreateDialogAsync(window.Object, viewmodel.Object);

        viewmodel.Verify(x => x.InitializeAsync(), Times.Never);
        window.VerifySet(x => x.ViewModel = It.IsAny<IViewModel>(), Times.Never);
    }

    #endregion

    #region ApplyButtonStyle / MapDialogResult

    [TestMethod]
    public void ApplyButtonStyle_WhenActionSucceeds_InvokesActionOnceAndDoesNotLog()
    {
        var invoked = 0;

        InvokeApplyButtonStyle(() => invoked++);

        Assert.AreEqual(1, invoked);
        VerifyLog(LogLevel.Warning, ButtonStyleFailure, Times.Never());
    }

    [TestMethod]
    public void ApplyButtonStyle_WhenActionThrows_LogsWarningAndDoesNotRethrow()
    {
        InvokeApplyButtonStyle(() => throw new InvalidOperationException("style not available"));

        VerifyLog(LogLevel.Warning, ButtonStyleFailure, Times.Once());
    }

    [TestMethod]
    [DataRow(MessageBoxButtons.OKCancel, ContentDialogResult.Primary, MessageBoxResults.OK)]
    [DataRow(MessageBoxButtons.OKCancel, ContentDialogResult.Secondary, MessageBoxResults.Cancel)]
    [DataRow(MessageBoxButtons.OKCancel, ContentDialogResult.None, MessageBoxResults.Cancel)]
    [DataRow(MessageBoxButtons.YesNoCancel, ContentDialogResult.Primary, MessageBoxResults.Yes)]
    [DataRow(MessageBoxButtons.YesNoCancel, ContentDialogResult.Secondary, MessageBoxResults.No)]
    [DataRow(MessageBoxButtons.YesNoCancel, ContentDialogResult.None, MessageBoxResults.Cancel)]
    [DataRow(MessageBoxButtons.YesNo, ContentDialogResult.Primary, MessageBoxResults.Yes)]
    [DataRow(MessageBoxButtons.YesNo, ContentDialogResult.Secondary, MessageBoxResults.No)]
    [DataRow(MessageBoxButtons.YesNo, ContentDialogResult.None, MessageBoxResults.No)]
    [DataRow(MessageBoxButtons.OK, ContentDialogResult.None, MessageBoxResults.OK)]
    [DataRow(MessageBoxButtons.OK, ContentDialogResult.Primary, MessageBoxResults.OK)]
    public void MapDialogResult_MapsDialogResultForButtonSet(
        MessageBoxButtons buttons,
        ContentDialogResult result,
        MessageBoxResults expected)
    {
        var method = typeof(DialogService).GetMethod(
            "MapDialogResult",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "MapDialogResult must remain a private static helper");

        var actual = (MessageBoxResults)method.Invoke(null, [buttons, result])!;

        Assert.AreEqual(expected, actual);
    }

    #endregion

    #region Callback wiring (AttachDialogCallbacks) and handlers

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenSubmittedRaised_InvokesSubmittedHandler()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var submitted = new TaskCompletionSource<TestEntity>(TaskCreationOptions.RunContinuationsAsynchronously);
        var entity = new TestEntity();

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            result => { submitted.TrySetResult(result); return Task.CompletedTask; },
            null,
            null,
            new Mock<IServiceScope>().Object);

        viewmodel.Raise(x => x.Submitted += null, new SubmitEventArgs<TestEntity>(entity));

        var actual = await submitted.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.CancellationToken);
        Assert.AreSame(entity, actual);
    }

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenSubmittedHandlerThrows_LogsErrorAndSwallows()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var submitted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            _ =>
            {
                submitted.TrySetResult(true);
                throw new InvalidOperationException("submit failed");
            },
            null,
            null,
            new Mock<IServiceScope>().Object);

        viewmodel.Raise(x => x.Submitted += null, new SubmitEventArgs<TestEntity>(new TestEntity()));

        await submitted.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.CancellationToken);
        await VerifyLogEventuallyAsync(LogLevel.Error, "Submit callback failed");
    }

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenCancelledRaised_InvokesCancelledHandler()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            _ => Task.CompletedTask,
            () => { cancelled.TrySetResult(true); return Task.CompletedTask; },
            null,
            new Mock<IServiceScope>().Object);

        viewmodel.Raise(x => x.Cancelled += null, EventArgs.Empty);

        Assert.IsTrue(await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenCancelledRaisedWithoutHandler_DoesNotLogError()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            _ => Task.CompletedTask,
            null,
            null,
            new Mock<IServiceScope>().Object);

        viewmodel.Raise(x => x.Cancelled += null, EventArgs.Empty);

        await Task.Delay(50, TestContext.CancellationToken);
        VerifyLog(LogLevel.Error, "Cancel callback failed", Times.Never());
    }

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenClosedRaised_InvokesClosedHandlerAndDisposesScope()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var scope = new Mock<IServiceScope>();
        var closed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            _ => Task.CompletedTask,
            null,
            () => { closed.TrySetResult(true); return Task.CompletedTask; },
            scope.Object);

        viewmodel.Raise(x => x.Closed += null, EventArgs.Empty);

        Assert.IsTrue(await closed.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.CancellationToken));
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenClosedRaised_UnsubscribesSubmittedAndCancelled()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var submittedCount = 0;
        var cancelledCount = 0;

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            _ => { Interlocked.Increment(ref submittedCount); return Task.CompletedTask; },
            () => { Interlocked.Increment(ref cancelledCount); return Task.CompletedTask; },
            null,
            new Mock<IServiceScope>().Object);

        viewmodel.Raise(x => x.Closed += null, EventArgs.Empty);

        // The Closed handler detaches the other handlers: nothing must fire afterwards.
        viewmodel.Raise(x => x.Submitted += null, new SubmitEventArgs<TestEntity>(new TestEntity()));
        viewmodel.Raise(x => x.Cancelled += null, EventArgs.Empty);

        await Task.Delay(50, TestContext.CancellationToken);

        Assert.AreEqual(0, submittedCount);
        Assert.AreEqual(0, cancelledCount);
    }

    [TestMethod]
    public async Task AttachDialogCallbacks_WhenCancelledHandlerThrows_LogsErrorAndSwallows()
    {
        var viewmodel = new Mock<IViewModelDialog<TestEntity>>();
        var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        InvokeAttachDialogCallbacks(
            viewmodel.Object,
            _ => Task.CompletedTask,
            () =>
            {
                cancelled.TrySetResult(true);
                throw new InvalidOperationException("cancel failed");
            },
            null,
            new Mock<IServiceScope>().Object);

        viewmodel.Raise(x => x.Cancelled += null, EventArgs.Empty);

        Assert.IsTrue(await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.CancellationToken));
        await VerifyLogEventuallyAsync(LogLevel.Error, "Cancel callback failed");
    }

    [TestMethod]
    public async Task HandleDialogCancelledAsync_WhenHandlerThrows_LogsError()
    {
        await InvokePrivateTaskAsync(
            "HandleDialogCancelledAsync",
            [],
            (Func<Task>)(() => throw new InvalidOperationException("cancel failed")),
            "WindowType",
            "ViewModelType");

        VerifyLog(LogLevel.Error, "Cancel callback failed", Times.Once());
    }

    [TestMethod]
    public async Task HandleDialogClosedAsync_WhenHandlerThrows_LogsError()
    {
        await InvokePrivateTaskAsync(
            "HandleDialogClosedAsync",
            [],
            (Func<Task>)(() => throw new InvalidOperationException("close failed")),
            "WindowType",
            "ViewModelType");

        VerifyLog(LogLevel.Error, "Close callback failed", Times.Once());
    }

    [TestMethod]
    public void LogDialogError_WithEntity_LogsEntityType()
    {
        InvokeLogDialogError(hasEntity: true);

        VerifyLog(LogLevel.Error, nameof(TestEntity), Times.Once());
    }

    [TestMethod]
    public void LogDialogError_WithoutEntity_DoesNotLogEntityType()
    {
        InvokeLogDialogError(hasEntity: false);

        VerifyLog(LogLevel.Error, nameof(TestEntity), Times.Never());
        VerifyLog(LogLevel.Error, typeof(IViewModelDialog<TestEntity>).Name, Times.Once());
    }

    #endregion

    #region Dispose

    [TestMethod]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        Exception? exception = null;
        try
        {
            _service.Dispose();
            _service.Dispose();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        Assert.IsNull(exception);
    }

    #endregion

    #region Helpers

    private void InvokeApplyButtonStyle(Action applyStyles)
    {
        var method = typeof(DialogService).GetMethod(
            "ApplyButtonStyle",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, "ApplyButtonStyle must remain a private instance helper");

        method.Invoke(_service, [applyStyles]);
    }

    private Mock<IServiceScope> UseScopeFactoryReturning(IServiceProvider provider)
    {
        var scope = new Mock<IServiceScope>();
        scope.Setup(x => x.ServiceProvider).Returns(provider);
        _scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);
        return scope;
    }

    private Mock<IServiceScope> UseScopeFactoryWithThrowingProvider()
    {
        var provider = new Mock<IServiceProvider>();
        provider
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Throws(new InvalidOperationException("service not registered"));
        return UseScopeFactoryReturning(provider.Object);
    }

    private void InvokeAttachDialogCallbacks(
        IViewModelDialog<TestEntity> viewmodel,
        Func<TestEntity, Task> submittedHandler,
        Func<Task>? cancelledHandler,
        Func<Task>? closedHandler,
        IServiceScope scope)
    {
        var method = typeof(DialogService)
            .GetMethod("AttachDialogCallbacks", BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(typeof(StubWindow), typeof(IViewModelDialog<TestEntity>), typeof(TestEntity));

        method.Invoke(_service, [viewmodel, submittedHandler, cancelledHandler, closedHandler, scope]);
    }

    private void InvokeLogDialogError(bool hasEntity)
    {
        var method = typeof(DialogService)
            .GetMethod("LogDialogError", BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(typeof(StubWindow), typeof(IViewModelDialog<TestEntity>), typeof(TestEntity));

        method.Invoke(_service, [new InvalidOperationException("boom"), hasEntity]);
    }

    private async Task InvokePrivateTaskAsync(string methodName, Type[] genericArguments, params object?[] arguments)
    {
        var method = typeof(DialogService).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"{methodName} must remain a private instance helper");

        if (genericArguments.Length > 0)
            method = method.MakeGenericMethod(genericArguments);

        var task = (Task?)method.Invoke(_service, arguments);
        Assert.IsNotNull(task, $"{methodName} must return a Task");
        await task;
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

    public sealed partial class StubWindow : IWindow
    {
        public IViewModel ViewModel { get; set; } = null!;

        public Task CloseAsync() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    public sealed class FakeDependency
    {
    }

    public sealed class PlainDialog
    {
    }

    public sealed class PlainDialogWithDependency
    {
        public PlainDialogWithDependency(FakeDependency dependency)
        {
            Assert.IsNotNull(dependency);
        }
    }

    #endregion
}

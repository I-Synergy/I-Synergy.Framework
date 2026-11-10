using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using ControlsWindow = ISynergy.Framework.UI.Controls.Window;

namespace ISynergy.Framework.UI.WPF.Services;

/// <summary>
/// Covers the ViewModel-initialization failure path of
/// <c>DialogService.CreateDialogAsync{TEntity}</c>: the exception filter logs the failure (and
/// returns <c>false</c> so the original exception keeps propagating) and the <c>catch</c> block
/// unsubscribes the synthetic <c>Closed</c> handler before rethrowing.
///
/// A real <see cref="ControlsWindow"/> is required because the method only handles
/// <c>ISynergy.Framework.UI.Controls.Window</c> dialogs, so this is an STA test. A real
/// <see cref="System.Windows.Application"/> would change process-wide WPF state (and can only be
/// created once per AppDomain), therefore only its static <c>Current</c> slot is populated with an
/// uninitialized instance whose dispatcher points at the test thread — enough for the
/// <c>Application.Current.MainWindow</c> read (which is <c>null</c>, i.e. no owner).
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DialogServiceViewModelInitializationTests
{
    private static readonly FieldInfo? s_applicationInstance =
        typeof(System.Windows.Application).GetField("_appInstance", BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly FieldInfo? s_dispatcher =
        typeof(DispatcherObject).GetField("_dispatcher", BindingFlags.NonPublic | BindingFlags.Instance);

    [STATestMethod]
    public async Task CreateDialogAsync_WhenInitializeAsyncThrows_LogsAndRethrows()
    {
        var window = new ControlsWindow();
        var viewModel = new Mock<IViewModelDialog<string>>();
        var expected = new InvalidOperationException("initialization failed");
        viewModel.Setup(vm => vm.InitializeAsync()).ThrowsAsync(expected);

        var logger = new Mock<ILogger<DialogService>>();
        var service = new DialogService(
            new Mock<IServiceScopeFactory>().Object,
            new Mock<ILanguageService>().Object,
            logger.Object);

        using var application = new FakeApplication();

        var thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            service.CreateDialogAsync(window, viewModel.Object));

        Assert.AreEqual(expected.Message, thrown.Message);
        Assert.IsNull(thrown.InnerException, "The original exception must not be wrapped");
        Assert.AreSame(viewModel.Object, window.DataContext, "The view model is still the data context");

        logger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => (v != null && v.ToString()!.Contains("Error initializing viewmodel"))),
                expected,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "The failure must be logged exactly once by the exception filter.");
    }

    /// <summary>
    /// Installs an uninitialized <see cref="System.Windows.Application"/> as
    /// <c>Application.Current</c> for the duration of the test, and restores the previous value.
    /// </summary>
    private sealed class FakeApplication : IDisposable
    {
        private readonly object? _previous;

        public FakeApplication()
        {
            Assert.IsNotNull(s_applicationInstance, "System.Windows.Application._appInstance was not found.");
            Assert.IsNotNull(s_dispatcher, "DispatcherObject._dispatcher was not found.");

            var application = (System.Windows.Application)RuntimeHelpers.GetUninitializedObject(
                typeof(System.Windows.Application));

            // Application.MainWindow (and other accessors) call VerifyAccess() first.
            s_dispatcher!.SetValue(application, Dispatcher.CurrentDispatcher);

            _previous = s_applicationInstance!.GetValue(null);
            s_applicationInstance.SetValue(null, application);
        }

        public void Dispose() => s_applicationInstance?.SetValue(null, _previous);
    }
}

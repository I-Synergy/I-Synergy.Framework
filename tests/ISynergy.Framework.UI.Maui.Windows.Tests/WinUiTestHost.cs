using Microsoft.UI.Dispatching;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace ISynergy.Framework.UI.Maui;

/// <summary>
/// Hosts a WinUI (Windows App SDK) application with a running message loop for the whole test
/// session. Real MAUI controls — <c>ContentPage</c>, <c>FlyoutPage</c>, <c>Shell</c>, … — can only
/// be constructed once the WinUI runtime is initialized on a thread that pumps messages, which a
/// headless test host does not provide by default.
///
/// Initialization is best-effort: when the Windows App SDK runtime is unavailable the host simply
/// stays "not available" and the tests that need it report <see cref="Assert.Inconclusive(string)"/>.
/// </summary>
internal static class WinUiTestHost
{
    /// <summary>Windows App SDK 1.8 (0x00010008).</summary>
    private const uint WindowsAppSdkVersion = 0x00010008;

    private static readonly ManualResetEventSlim s_started = new(false);
    private static DispatcherQueue? s_dispatcherQueue;
    private static Microsoft.UI.Xaml.Application? s_application;
    private static Exception? s_startupFailure;

    public static bool IsAvailable => s_dispatcherQueue is not null;

    public static void Start()
    {
        var thread = new Thread(() =>
        {
            try
            {
                if (!Bootstrap.TryInitialize(WindowsAppSdkVersion, null, new PackageVersion(), out _))
                    throw new InvalidOperationException("The Windows App SDK runtime is not installed.");

                Microsoft.UI.Xaml.Application.Start(_ =>
                {
                    s_application = new Microsoft.UI.Xaml.Application();
                    s_dispatcherQueue = DispatcherQueue.GetForCurrentThread();

                    // Force the MAUI/Controls static initializers (Element, VisualElement,
                    // ViewHandler) to run on this UI thread. Their type initializers touch WinUI
                    // statics; running them first from a non-UI thread would fail permanently
                    // (type initializers are cached) and break every later page creation.
                    GC.KeepAlive(new Microsoft.Maui.Controls.ContentPage());

                    s_started.Set();
                });
            }
            catch (Exception exception)
            {
                s_startupFailure = exception;
                s_started.Set();
            }
        })
        {
            IsBackground = true,
            Name = "WinUI test host"
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        if (!s_started.Wait(TimeSpan.FromSeconds(60)))
            throw new TimeoutException("The WinUI test host did not start within 60 seconds.");
    }

    /// <summary>
    /// Runs <paramref name="action"/> on the WinUI UI thread and rethrows its exception (if any)
    /// on the calling thread.
    /// </summary>
    public static void Run(Action action)
    {
        if (s_startupFailure is not null)
            Assert.Inconclusive($"The WinUI test host is unavailable: {s_startupFailure.GetType().Name}.");

        var dispatcherQueue = s_dispatcherQueue
            ?? throw new InvalidOperationException("The WinUI test host has not been started.");

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    action();
                    completion.SetResult();
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            }))
        {
            throw new InvalidOperationException("Could not dispatch the action onto the WinUI UI thread.");
        }

        try
        {
            completion.Task.GetAwaiter().GetResult();
        }
        catch (Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }

    public static void Stop()
    {
        if (!IsAvailable || s_application is null)
            return;

        try
        {
            Run(s_application.Exit);
        }
        catch
        {
            // Best-effort teardown.
        }
    }
}

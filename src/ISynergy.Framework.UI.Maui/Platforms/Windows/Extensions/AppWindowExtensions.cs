using ISynergy.Framework.Core.Validation;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;

public static class AppWindowExtensions
{
    public static AppWindow GetAppWindow(this Page page)
    {
        Argument.IsNotNull(page);

        var window = page.GetParentWindow().Handler.PlatformView as MauiWinUIWindow;
        var appWindow = window!.GetAppWindow();
        return appWindow;
    }

    public static AppWindow GetAppWindow(this Microsoft.UI.Xaml.Window window)
    {
        Argument.IsNotNull(window);

        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = AppWindow.GetFromWindowId(id);
        return appWindow;
    }

    public static void ToggleFullScreen(this AppWindow window)
    {
        Argument.IsNotNull(window);

        if (window?.Presenter is OverlappedPresenter overlappedPresenter)
        {
            if (overlappedPresenter.State == OverlappedPresenterState.Maximized)
                window.ToNormalScreen();
            else
                window.ToFullScreen();
        }
    }

    public static void ToFullScreen(this AppWindow window)
    {
        Argument.IsNotNull(window);

        if (window?.Presenter is OverlappedPresenter overlappedPresenter)
        {
            overlappedPresenter.SetBorderAndTitleBar(false, false);
            overlappedPresenter.Maximize();
        }
    }

    public static void ToNormalScreen(this AppWindow window)
    {
        Argument.IsNotNull(window);

        if (window?.Presenter is OverlappedPresenter overlappedPresenter)
        {
            overlappedPresenter.SetBorderAndTitleBar(true, true);
            overlappedPresenter.Restore();
        }
    }

    // ──────────────────────────────────────────────
    //  Activation argument handling
    // ──────────────────────────────────────────────

    /// <summary>
    /// Registers callbacks that are invoked when a second instance redirects its
    /// activation (protocol URI, command-line args, etc.) to this instance.
    /// <para>
    /// Separate from <see cref="MauiWinUIApplicationExtensions.InitializeSingleInstanceApp"/>
    /// which handles the single-instance enforcement (redirect + kill).
    /// This method only handles WHAT to do with the activation arguments.
    /// </para>
    /// </summary>
    /// <param name="builder">The MAUI app builder.</param>
    /// <param name="defaultAction">Called with command-line arguments (fallback).</param>
    /// <param name="protocolAction">Called for protocol URI activations (myapp://...).</param>
    /// <param name="launchAction">Called for standard launch activations.</param>
    public static MauiAppBuilder ConfigureActivationHandlers(this MauiAppBuilder builder, Action<string[]> defaultAction, Action<ProtocolActivatedEventArgs> protocolAction, Action<LaunchActivatedEventArgs> launchAction) // NOSONAR
    {
#if WINDOWS
        // Store callbacks — invoked directly by OnAppInstanceActivated when a
        // redirected activation arrives at the first instance.
        MauiWinUIApplicationExtensions.SetActivationCallbacks(defaultAction, protocolAction, launchAction);
#endif

        return builder;
    }

    // ──────────────────────────────────────────────
    //  Single-instance enforcement (opt-in)
    // ──────────────────────────────────────────────

    /// <summary>
    /// Enables single-instance behavior. When a second instance is launched, its
    /// activation is redirected to the first instance and the second process exits.
    /// <para>
    /// Call this BEFORE <see cref="ConfigureActivationHandlers"/> to ensure the
    /// redirect happens for every launch. Without this call, multiple instances
    /// can run simultaneously.
    /// </para>
    /// </summary>
    public static MauiAppBuilder ConfigureSingleInstance(this MauiAppBuilder builder) // NOSONAR
    {
#if WINDOWS
        builder.ConfigureLifecycleEvents(configureDelegate =>
        {
            configureDelegate.AddWindows(windows =>
            {
                windows.OnLaunched((app, args) =>
                {
                    Debug.WriteLine("[SingleInstance] OnLaunched — checking for existing instances.");
                    MauiWinUIApplicationExtensions.RedirectAndExitIfNotFirst();

                    // If we reach here, this is the first instance.
                    // Subscribe so future redirects dispatch activation callbacks
                    // and bring the window to foreground.
                    AppInstance.GetCurrent().Activated += MauiWinUIApplicationExtensions.OnAppInstanceActivated;
                    Debug.WriteLine("[SingleInstance] Subscribed to Activated for redirects.");

                    // Process the INITIAL launch activation args (from launchSettings.json
                    // commandLineArgs, protocol URIs, etc.). Defer with low priority so the
                    // MAUI window has been created before callbacks fire — callbacks may
                    // try to show UI (dialogs, etc.) which need a XamlRoot.
                    var initialArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                    var queue = DispatcherQueue.GetForCurrentThread();
                    queue.TryEnqueue(DispatcherQueuePriority.Low, () =>
                    {
                        Debug.WriteLine($"[SingleInstance] Processing initial activation: Kind={initialArgs.Kind}");
                        MauiWinUIApplicationExtensions.InvokeActivationCallbacksDirectly(initialArgs);
                    });
                });
            });
        });
#endif

        return builder;
    }
}

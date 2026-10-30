using ISynergy.Framework.Core.Validation;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Activation;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;

public static class AppWindowExtensions
{
    public static AppWindow? GetAppWindow(this Page page)
    {
        if (page is null)
            return null;

        var window = page.GetParentWindow()?.Handler?.PlatformView as MauiWinUIWindow;
        return window?.GetAppWindow();
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

    /// <summary>
    /// Configures the native Windows title bar to support right-to-left layout.
    /// Adjusts the caption button colors to blend with the app theme so the
    /// buttons integrate cleanly when the MAUI layout flows RTL beneath them.
    /// The MAUI-level <c>FlowDirection.RightToLeft</c> handles content layout;
    /// the native title bar itself cannot be mirrored via public API.
    /// </summary>
    /// <param name="appWindow">The WinUI AppWindow instance.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar").</param>
    public static void ApplyFlowDirectionToTitleBar(this AppWindow appWindow, string cultureName)
    {
        if (appWindow?.TitleBar is null)
            return;

        // Apply WS_EX_LAYOUTRTL to mirror the native title bar chrome
        // (caption buttons and title text) for RTL languages.
        ApplyNativeFlowDirection(appWindow, cultureName);
    }

    // ──────────────────────────────────────────────
    //  Native window layout direction (RTL/LTR)
    // ──────────────────────────────────────────────

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYOUTRTL = 0x00400000;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    /// <summary>
    /// Applies or clears the <c>WS_EX_LAYOUTRTL</c> extended window style on the
    /// native HWND that backs the given <see cref="AppWindow"/>. This mirrors the
    /// title bar chrome including caption buttons (minimize, maximize, close) and
    /// the native title text for RTL languages.
    /// </summary>
    private static void ApplyNativeFlowDirection(AppWindow appWindow, string cultureName)
    {
        var isRightToLeft = CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft;
        var windowId = appWindow.Id;
        var hWnd = Microsoft.UI.Win32Interop.GetWindowFromWindowId(windowId);

        if (hWnd == nint.Zero)
            return;

        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

        if (isRightToLeft)
            exStyle |= WS_EX_LAYOUTRTL;
        else
            exStyle &= ~WS_EX_LAYOUTRTL;

        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
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
    public static MauiAppBuilder ConfigureActivationHandlers(this MauiAppBuilder builder,
        Action<string[]> defaultAction,
        Action<ProtocolActivatedEventArgs> protocolAction,
        Action<LaunchActivatedEventArgs> launchAction) // NOSONAR
    {
        // Store callbacks — invoked directly by OnAppInstanceActivated when a
        // redirected activation arrives at the first instance.
        MauiWinUIApplicationExtensions.SetActivationCallbacks(defaultAction, protocolAction, launchAction);

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
        builder.ConfigureLifecycleEvents(configureDelegate =>
        {
            configureDelegate.AddWindows(windows =>
            {
                windows.OnLaunched((app, args) =>
                {
                    Debug.WriteLine("[SingleInstance] OnLaunched — first instance.");

                    // The redirect/kill for second instances already happened in the
                    // WinUI App constructor (InitializeSingleInstanceApp). Here we
                    // just ensure Activated is subscribed (safety net) and handle
                    // the initial launch activation args.

                    // Safety-net subscription in case InitializeSingleInstanceApp's
                    // subscription was removed or failed.
                    AppInstance.GetCurrent().Activated += MauiWinUIApplicationExtensions.OnAppInstanceActivated;

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

        return builder;
    }
}

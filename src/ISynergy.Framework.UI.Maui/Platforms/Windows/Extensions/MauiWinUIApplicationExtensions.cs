using Microsoft.Maui.LifecycleEvents;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Activation;

#pragma warning disable IDE0130, S1200
#nullable enable

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Provides single-instance enforcement for WinUI/MAUI applications.
/// When a second instance is launched, its activation is redirected to the first
/// instance and the second process is terminated.
/// </summary>
public static partial class MauiWinUIApplicationExtensions
{
    private const int SW_SHOWNORMAL = 1;

    // Activation callbacks — set by ConfigureActivationHandlers.
    // Invoked when a redirected activation arrives at the first instance.
    private static Action<string[]>? s_activationDefault;
    private static Action<ProtocolActivatedEventArgs>? s_activationProtocol;
    private static Action<LaunchActivatedEventArgs>? s_activationLaunch;

    [LibraryImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    // ──────────────────────────────────────────────
    //  Activated event subscription
    // ──────────────────────────────────────────────

    /// <summary>
    /// Call from the WinUI App constructor before InitializeComponent().
    /// Checks for existing instances and, if found, redirects activation to the
    /// first instance and terminates the current process — BEFORE the MAUI window
    /// is created, so no flash is visible.
    /// <para>
    /// On the first instance, subscribes to AppInstance.Activated so that future
    /// redirects bring the window to the foreground and dispatch activation callbacks.
    /// </para>
    /// </summary>
    public static void InitializeSingleInstanceApp(this MauiWinUIApplication app)
    {
        Debug.WriteLine("[SingleInstance] Checking for existing instances...");

        var instances = AppInstance.GetInstances();
        Debug.WriteLine($"[SingleInstance] GetInstances returned {instances.Count} instance(s).");

        // Find an existing instance that isn't the current process
        AppInstance? existing = null;
        foreach (var instance in instances)
        {
            if (!instance.IsCurrent)
            {
                existing = instance;
                break;
            }
        }

        if (existing is not null)
        {
            // Second (or later) instance — redirect activation and exit BEFORE
            // the window is created. No SynchronizationContext yet, so .Wait() is safe.
            Debug.WriteLine("[SingleInstance] Existing instance found. Redirecting activation...");

            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            Debug.WriteLine($"[SingleInstance] Activation kind: {args.Kind}");

            try
            {
                existing.RedirectActivationToAsync(args).AsTask().Wait();
                Debug.WriteLine("[SingleInstance] Redirect completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SingleInstance] Redirect failed: {ex}");
            }

            Debug.WriteLine("[SingleInstance] Exiting second instance.");
            Process.GetCurrentProcess().Kill();
            return;
        }

        // First instance — subscribe to future redirected activations
        Debug.WriteLine("[SingleInstance] First instance. Subscribing to Activated event.");
        AppInstance.GetCurrent().Activated += OnAppInstanceActivated;
        Debug.WriteLine("[SingleInstance] Ready to receive redirected activations.");
    }

    /// <summary>
    /// Checks for existing instances and, if found, redirects activation to the
    /// first instance and terminates the current process.
    /// Called by ConfigureSingleInstance from the OnLaunched handler as a fallback.
    /// </summary>
    internal static void RedirectAndExitIfNotFirst()
    {
        var instances = AppInstance.GetInstances();
        Debug.WriteLine($"[SingleInstance] GetInstances returned {instances.Count} instance(s).");

        AppInstance? existing = null;
        foreach (var instance in instances)
        {
            if (!instance.IsCurrent)
            {
                existing = instance;
                break;
            }
        }

        if (existing is not null)
        {
            Debug.WriteLine("[SingleInstance] Existing instance found. Redirecting activation...");

            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            Debug.WriteLine($"[SingleInstance] Activation kind: {args.Kind}");

            // Redirect on a dedicated STA thread to avoid UI-thread deadlock
            // from SynchronizationContext capture.
            Exception? redirectError = null;
            var redirectThread = new Thread(() =>
            {
                try
                {
                    existing.RedirectActivationToAsync(args).AsTask().Wait();
                    Debug.WriteLine("[SingleInstance] Redirect completed.");
                }
                catch (Exception ex)
                {
                    redirectError = ex;
                    Debug.WriteLine($"[SingleInstance] Redirect failed: {ex}");
                }
            });
            redirectThread.SetApartmentState(ApartmentState.STA);
            redirectThread.Start();
            redirectThread.Join();

            if (redirectError is not null)
                Debug.WriteLine($"[SingleInstance] Redirect error (instance will still exit): {redirectError}");

            Debug.WriteLine("[SingleInstance] Exiting second instance.");
            Process.GetCurrentProcess().Kill();
        }
    }

    // ──────────────────────────────────────────────
    //  Activation callback storage (set by ConfigureActivationHandlers)
    // ──────────────────────────────────────────────

    /// <summary>
    /// Stores the activation argument callbacks so the static Activated handler
    /// can invoke them directly, bypassing the MAUI lifecycle event system.
    /// Called by <see cref="AppWindowExtensions.ConfigureActivationHandlers"/>.
    /// </summary>
    internal static void SetActivationCallbacks(
        Action<string[]> defaultAction,
        Action<ProtocolActivatedEventArgs> protocolAction,
        Action<LaunchActivatedEventArgs> launchAction)
    {
        s_activationDefault = defaultAction;
        s_activationProtocol = protocolAction;
        s_activationLaunch = launchAction;
    }

    // ──────────────────────────────────────────────
    //  Activated event handler
    // ──────────────────────────────────────────────

    /// <summary>
    /// Handles the AppInstance.Activated event when a second instance redirects
    /// its activation to this (the first) instance.
    /// </summary>
    internal static void OnAppInstanceActivated(object? sender, AppActivationArguments e)
    {
        Debug.WriteLine($"[SingleInstance] Activated event fired: Kind={e.Kind}");

        // Primary route: invoke the stored callbacks directly.
        // This is the reliable path — MAUI's InvokeEvents may silently find
        // zero handlers even when ServiceLocator resolution succeeds.
        InvokeActivationCallbacksDirectly(e);

        // Also dispatch through MAUI lifecycle for any handlers registered
        // externally via windows.OnAppInstanceActivated (outside of ConfigureActivationHandlers).
        try
        {
            var lifecycleService = IPlatformApplication.Current!.Services.GetRequiredService<ILifecycleEventService>();
            lifecycleService.OnAppInstanceActivated(sender, e);
            Debug.WriteLine("[SingleInstance] MAUI lifecycle dispatch completed.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SingleInstance] MAUI lifecycle dispatch failed: {ex}");
        }

        // Bring the existing window to the foreground
        BringWindowToForeground();
    }

    // ──────────────────────────────────────────────
    //  Direct callback invocation (fallback)
    // ──────────────────────────────────────────────

    internal static void InvokeActivationCallbacksDirectly(AppActivationArguments e)
    {
        Debug.WriteLine($"[SingleInstance] Direct callback: Kind={e.Kind}");

        if (e.Kind == ExtendedActivationKind.Launch)
        {
            if (e.Data is LaunchActivatedEventArgs launchArgs && s_activationLaunch is not null)
            {
                Debug.WriteLine($"[SingleInstance] Direct: Invoking launch callback.");
                s_activationLaunch.Invoke(launchArgs);
            }
        }
        else if (e.Kind == ExtendedActivationKind.Protocol)
        {
            if (e.Data is ProtocolActivatedEventArgs protocolArgs && s_activationProtocol is not null)
            {
                Debug.WriteLine($"[SingleInstance] Direct: Invoking protocol callback.");
                s_activationProtocol.Invoke(protocolArgs);
            }
        }
        else if (e.Data is LaunchActivatedEventArgs fallbackArgs
                 && !string.IsNullOrEmpty(fallbackArgs.Arguments)
                 && s_activationDefault is not null)
        {
            Debug.WriteLine($"[SingleInstance] Direct: Invoking default callback.");
            s_activationDefault.Invoke(fallbackArgs.Arguments.Split(' '));
        }
        else
        {
            Debug.WriteLine($"[SingleInstance] Direct: No matching callback for Kind={e.Kind}.");
        }
    }

    // ──────────────────────────────────────────────
    //  Window management
    // ──────────────────────────────────────────────

    private static void BringWindowToForeground()
    {
        if (Microsoft.Maui.Controls.Application.Current?.Windows.Count == 0)
            return;

        var window = (Microsoft.UI.Xaml.Window?)Microsoft.Maui.Controls.Application.Current!.Windows[0].Page?.Window?.Handler?.PlatformView;

        if (window is not null)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            ShowWindow(windowHandle, SW_SHOWNORMAL);
            SetForegroundWindow(windowHandle);
        }
    }
}

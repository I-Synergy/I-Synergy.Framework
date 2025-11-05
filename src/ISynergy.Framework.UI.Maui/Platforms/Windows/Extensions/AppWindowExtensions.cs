using ISynergy.Framework.Core.Validation;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Windowing;
using Microsoft.Windows.AppLifecycle;
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

    public static MauiAppBuilder ConfigureSingleInstanceApp(this MauiAppBuilder builder, Action<string[]> defaultAction, Action<ProtocolActivatedEventArgs> protocolAction, Action<LaunchActivatedEventArgs> launchAction)
    {
#if WINDOWS
        builder.ConfigureLifecycleEvents(configureDelegate =>
        {
            configureDelegate.AddWindows(windows =>
            {
                windows.OnAppInstanceActivated((sender, e) =>
                {
                    if (e.Kind == ExtendedActivationKind.Launch)
                    {
                        launchAction.Invoke(e.Data as LaunchActivatedEventArgs);
                    }
                    else if (e.Kind == ExtendedActivationKind.Protocol)
                    {
                        protocolAction.Invoke(e.Data as ProtocolActivatedEventArgs);
                    }
                    else if (Environment.GetCommandLineArgs().Length > 1)
                    {
                        defaultAction.Invoke(Environment.GetCommandLineArgs());
                    }
                });
            });
        });
#endif

        return builder;
    }
}

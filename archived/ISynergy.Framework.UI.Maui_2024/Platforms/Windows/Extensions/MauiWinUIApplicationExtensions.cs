using ISynergy.Framework.Core.Locators;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Runtime.InteropServices;

#nullable enable

namespace ISynergy.Framework.UI.Extensions;

public static partial class MauiWinUIApplicationExtensions
{
    private const int SW_SHOWNORMAL = 1;

    [LibraryImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void InitializeSingleInstanceApp(this MauiWinUIApplication app)
    {
        var singleInstance = AppInstance.FindOrRegisterForKey("SingleInstanceApp");

        if (!singleInstance.IsCurrent)
        {
            // this is another instance

            // 1. activate the first instance
            var currentInstance = AppInstance.GetCurrent();
            var args = currentInstance.GetActivatedEventArgs();
            singleInstance.RedirectActivationToAsync(args).GetAwaiter().GetResult();

            // 2. close this instance
            Process.GetCurrentProcess().Kill();
            return;
        }

        // this is the first instance

        // 1. register for future activation
        singleInstance.Activated += OnAppInstanceActivated;
    }

    private static void OnAppInstanceActivated(object? sender, AppActivationArguments e)
    {
        ServiceLocator.Default.GetInstance<ILifecycleEventService>().OnAppInstanceActivated(sender, e);

        var window = (Microsoft.UI.Xaml.Window)Microsoft.Maui.Controls.Application.Current!.MainPage!.Window!.Handler!.PlatformView!;

        // handle the old app being loaded.
        if (window is not null)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            // restores the window in case it was minimized.
            ShowWindow(windowHandle, SW_SHOWNORMAL);
            // brings the window to the foreground.
            SetForegroundWindow(windowHandle);
        }
    }
}

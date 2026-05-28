using ISynergy.Framework.Core.Enumerations;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Window extensions class
/// </summary>
public static partial class WindowExtensions
{
    /// <summary>
    /// Get handle from main window.
    /// </summary>
    /// <returns></returns>
    public static IntPtr GetCurrentProcMainWindowHandle()
    {
        using var process = Process.GetCurrentProcess();
        return process.MainWindowHandle;
    }

    public static Microsoft.UI.Xaml.Window SetTheme(this Microsoft.UI.Xaml.Window window, Themes theme)
    {
        if (window.Content is FrameworkElement frameworkElement)
        {
            switch (theme)
            {
                case Themes.Light:
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                    break;
                case Themes.Dark:
                    frameworkElement.RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    frameworkElement.RequestedTheme = ElementTheme.Default;
                    break;
            }
        }

        window.SetTitleBar(theme);

        return window;
    }

    public static Microsoft.UI.Xaml.Window SetTitleBar(this Microsoft.UI.Xaml.Window window, Themes theme)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        var iconPath = Path.Combine(Environment.CurrentDirectory, "icon.ico");

        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            if (theme == Themes.Dark)
            {
                appWindow.TitleBar.ForegroundColor = Colors.White;
                appWindow.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                appWindow.TitleBar.InactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);

                appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);

                appWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 51, 51, 51);
                appWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }
            else
            {
                appWindow.TitleBar.ForegroundColor = Colors.Black;
                appWindow.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);
                appWindow.TitleBar.InactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);

                appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);
                appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);

                appWindow.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);
                appWindow.TitleBar.ButtonPressedForegroundColor = Colors.Black;
                appWindow.TitleBar.ButtonPressedBackgroundColor = Colors.White;
            }

            appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
        }

        return window;
    }
}

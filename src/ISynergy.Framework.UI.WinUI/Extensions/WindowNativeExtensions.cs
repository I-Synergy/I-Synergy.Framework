using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Globalization;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for applying native window layout direction (RTL/LTR)
/// using the Win32 WS_EX_LAYOUTRTL extended window style.
/// </summary>
public static class WindowNativeExtensions
{
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
    /// <param name="appWindow">The WinUI AppWindow instance.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    public static void ApplyNativeFlowDirection(this AppWindow appWindow, string cultureName)
    {
        if (appWindow is null)
            return;

        var isRightToLeft = CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft;

        var hWnd = GetWindowHandle(appWindow);

        if (hWnd == nint.Zero)
            return;

        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

        if (isRightToLeft)
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle |= WS_EX_LAYOUTRTL);
        else
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle &= ~WS_EX_LAYOUTRTL);
    }

    /// <summary>
    /// Applies or clears the <c>WS_EX_LAYOUTRTL</c> extended window style on the
    /// native HWND that backs the given WinUI <see cref="Microsoft.UI.Xaml.Window"/>.
    /// </summary>
    /// <param name="window">The WinUI Window instance.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    public static void ApplyNativeFlowDirection(this Microsoft.UI.Xaml.Window window, string cultureName)
    {
        if (window is null)
            return;

        var hWnd = WindowNative.GetWindowHandle(window);

        if (hWnd == nint.Zero)
            return;

        var isRightToLeft = CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft;
        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

        if (isRightToLeft)
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle |= WS_EX_LAYOUTRTL);
        else
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle &= ~WS_EX_LAYOUTRTL);
    }

    /// <summary>
    /// Gets the native window handle (HWND) for an <see cref="AppWindow"/>.
    /// </summary>
    private static nint GetWindowHandle(AppWindow appWindow)
    {
        var windowId = appWindow.Id;
        var hWnd = Win32Interop.GetWindowFromWindowId(windowId);
        return hWnd;
    }

    public static AppWindow GetAppWindowForCurrentWindow(Microsoft.UI.Xaml.Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }
}

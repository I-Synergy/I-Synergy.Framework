using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using WinRT.Interop;

namespace ISynergy.Framework.UI.Helpers;

// Helper class to allow the app to find the Window that contains an
// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
// of all active Windows.  The app code must call WindowHelper.CreateWindow
// rather than "new Window" so we can keep track of all the relevant
// windows.  In the future, we would like to support this in platform APIs.
public class WindowHelper
{
    static public List<Microsoft.UI.Xaml.Window> ActiveWindows { get { return _activeWindows; } }

    static private List<Microsoft.UI.Xaml.Window> _activeWindows = new List<Microsoft.UI.Xaml.Window>();

    static public Microsoft.UI.Xaml.Window CreateWindow()
    {
        var newWindow = new Microsoft.UI.Xaml.Window
        {
#if WINDOWS
            SystemBackdrop = new MicaBackdrop()
#endif
        };

        TrackWindow(newWindow);
        return newWindow;
    }

    static public void TrackWindow(Microsoft.UI.Xaml.Window window)
    {
        window.Closed += (sender, args) =>
        {
            _activeWindows.Remove(window);
        };

        _activeWindows.Add(window);
    }

#if WINDOWS
    static public AppWindow GetAppWindow(Microsoft.UI.Xaml.Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }
#endif

    static public Microsoft.UI.Xaml.Window GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (var window in _activeWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }


    static public async Task<StorageFolder> GetAppLocalFolderAsync()
    {
        if (!NativeHelper.IsAppPackaged)
            return await StorageFolder.GetFolderFromPathAsync(System.AppContext.BaseDirectory);

        return ApplicationData.Current.LocalFolder;
    }
}

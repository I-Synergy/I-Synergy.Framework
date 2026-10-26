using Foundation;
using UIKit;

namespace Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    /// <summary>
    /// Hides the native window title so the shell header is the only title bar on macOS.
    /// The traffic-light buttons stay in place.
    /// </summary>
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var result = base.FinishedLaunching(application, launchOptions);

        foreach (var window in UIApplication.SharedApplication.Windows)
        {
            if (window.WindowScene?.Titlebar is { } titlebar)
            {
                titlebar.TitleVisibility = UITitlebarTitleVisibility.Hidden;
                titlebar.Toolbar = null;
            }
        }

        return result;
    }
}

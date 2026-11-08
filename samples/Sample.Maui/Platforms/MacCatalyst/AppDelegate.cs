using Foundation;
using UIKit;

namespace Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    /// <summary>
    /// Uses the standard opaque title bar so Mac Catalyst matches the native macOS window.
    /// </summary>
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        var result = base.FinishedLaunching(application, launchOptions);

        foreach (var window in UIApplication.SharedApplication.Windows)
        {
            if (window.WindowScene?.Titlebar is { } titlebar)
            {
                titlebar.TitleVisibility = UITitlebarTitleVisibility.Visible;
                titlebar.Toolbar = null;
            }
        }

        return result;
    }
}

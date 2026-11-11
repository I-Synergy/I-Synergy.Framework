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

        // CA1422: UIApplication.Windows is obsolete on MacCatalyst 15+ in favour of per-scene
        // UIWindowScene.Windows. This single-window sample keeps the existing enumeration so the
        // title-bar styling applies to the one window exactly as before.
#pragma warning disable CA1422 // obsolete API kept for behaviour parity in a single-window sample
        foreach (var window in UIApplication.SharedApplication.Windows)
#pragma warning restore CA1422
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

using Foundation;

namespace Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.Default.CreateMauiAppAsync().GetAwaiter().GetResult();
}

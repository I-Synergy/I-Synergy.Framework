using Foundation;
using Microsoft.Testing.Platform.Builder;
using UIKit;

namespace ISynergy.Framework.UI.Maui;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        _ = RunTestsAsync(Environment.GetCommandLineArgs().Skip(1).ToArray());
        return true;
    }

    private static async Task RunTestsAsync(string[] args)
    {
        int exitCode;

        try
        {
            ITestApplicationBuilder builder = await TestApplication.CreateBuilderAsync(args);
            SelfRegisteredExtensions.AddSelfRegisteredExtensions(builder, args);

            using ITestApplication app = await builder.BuildAsync();
            exitCode = await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            exitCode = 1;
        }

        Environment.Exit(exitCode);
    }
}

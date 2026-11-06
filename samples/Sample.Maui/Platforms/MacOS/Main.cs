#if MACOS
using AppKit;

namespace Sample;

/// <summary>
/// Entry point of the native macOS (AppKit) target.
/// </summary>
/// <remarks>
/// The Mac Catalyst target starts through <c>UIApplication</c> (Platforms/MacCatalyst/Program.cs);
/// AppKit has no <c>UIApplication</c>, so the application object and delegate are started here.
/// </remarks>
public static class MainClass
{
    public static void Main(string[] args)
    {
        NSApplication.Init();
        NSApplication.SharedApplication.Delegate = new MauiMacOSApp();
        NSApplication.Main(args);
    }
}
#endif

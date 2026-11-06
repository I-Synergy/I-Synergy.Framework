#if MACOS
using Foundation;
using Microsoft.Maui.Platforms.MacOS.Platform;

namespace Sample;

/// <summary>
/// AppKit application delegate for the native macOS target.
/// </summary>
/// <remarks>
/// The AppKit backend replaces the Mac Catalyst <c>MauiUIApplicationDelegate</c> with
/// <see cref="MacOSMauiApplication"/>: the delegate owns the <c>NSApplication</c> lifetime and
/// creates the MAUI app through <see cref="MauiProgram.CreateMauiApp"/>.
/// </remarks>
[Register(nameof(MauiMacOSApp))]
public class MauiMacOSApp : MacOSMauiApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
#endif

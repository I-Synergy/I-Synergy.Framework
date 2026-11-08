using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Cross-head entry point for the Windows activation plumbing.
/// </summary>
/// <remarks>
/// Only WinUI has single-instance activation, protocol/launch arguments and the ability to surface them
/// in a dialog; every other head compiles the no-op below, so an application can call
/// <see cref="ConfigureActivationDiagnostics"/> without conditional compilation. The Windows
/// implementation lives in <c>Platforms/Windows/Extensions/ActivationExtensions.Windows.cs</c>.
/// </remarks>
public static partial class ActivationExtensions
{
#if !WINDOWS
    /// <summary>
    /// No-op on heads without single-instance activation.
    /// </summary>
    /// <param name="appBuilder">The MAUI app builder.</param>
    /// <returns>The same builder.</returns>
    public static MauiAppBuilder ConfigureActivationDiagnostics(this MauiAppBuilder appBuilder) => appBuilder;
#endif
}

using Microsoft.Maui.LifecycleEvents;
using Microsoft.Windows.AppLifecycle;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;

public delegate void OnAppInstanceActivated(object? sender, AppActivationArguments e);

public static class WindowsLifeCycleExtensions
{
    public static IWindowsLifecycleBuilder OnAppInstanceActivated(this IWindowsLifecycleBuilder builder, OnAppInstanceActivated handler)
    {
        builder.AddEvent(nameof(OnAppInstanceActivated), handler);
        return builder;
    }

    public static void OnAppInstanceActivated(this ILifecycleEventService lifecycle, object? sender, AppActivationArguments e)
    {
        lifecycle.InvokeEvents<OnAppInstanceActivated>(nameof(OnAppInstanceActivated), del => del(sender, e));
    }
}
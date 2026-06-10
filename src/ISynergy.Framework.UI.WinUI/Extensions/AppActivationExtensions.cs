using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Stores activation argument callbacks and dispatches redirected activations to them.
/// Separate from single-instance enforcement — this only handles WHAT to do with the
/// activation arguments, not the redirect/kill mechanics.
/// </summary>
internal static class AppActivationExtensions
{
    // Single-instance flag — set by ConfigureSingleInstance()
    internal static bool IsSingleInstanceEnabled;

    private static Action<string[]>? s_defaultAction;
    private static Action<ProtocolActivatedEventArgs>? s_protocolAction;
    private static Action<LaunchActivatedEventArgs>? s_launchAction;

    /// <summary>
    /// Stores the activation argument callbacks.
    /// Called by <see cref="WindowsAppBuilderExtensions.ConfigureActivationHandlers"/>.
    /// </summary>
    internal static void SetCallbacks(
        Action<string[]> defaultAction,
        Action<ProtocolActivatedEventArgs> protocolAction,
        Action<LaunchActivatedEventArgs> launchAction)
    {
        s_defaultAction = defaultAction;
        s_protocolAction = protocolAction;
        s_launchAction = launchAction;
    }

    /// <summary>
    /// Dispatches a redirected activation to the registered callbacks.
    /// Called from the AppInstance.Activated event handler in WindowsAppSdkHost.
    /// </summary>
    internal static void Dispatch(object? sender, AppActivationArguments e)
    {
        Debug.WriteLine($"[Activation] Dispatch: Kind={e.Kind}");

        if (e.Kind == ExtendedActivationKind.Launch)
        {
            if (e.Data is LaunchActivatedEventArgs launchArgs && s_launchAction is not null)
            {
                Debug.WriteLine($"[Activation] Invoking launch callback. Args={launchArgs.Arguments ?? "(null)"}");
                s_launchAction.Invoke(launchArgs);
            }
        }
        else if (e.Kind == ExtendedActivationKind.Protocol)
        {
            if (e.Data is ProtocolActivatedEventArgs protocolArgs && s_protocolAction is not null)
            {
                Debug.WriteLine($"[Activation] Invoking protocol callback. URI={protocolArgs.Uri}");
                s_protocolAction.Invoke(protocolArgs);
            }
        }
        else if (e.Data is LaunchActivatedEventArgs fallbackArgs
                 && !string.IsNullOrEmpty(fallbackArgs.Arguments)
                 && s_defaultAction is not null)
        {
            Debug.WriteLine($"[Activation] Invoking default callback. Args={fallbackArgs.Arguments}");
            s_defaultAction.Invoke(fallbackArgs.Arguments.Split(' '));
        }
        else
        {
            Debug.WriteLine($"[Activation] No matching callback for Kind={e.Kind}.");
        }
    }
}

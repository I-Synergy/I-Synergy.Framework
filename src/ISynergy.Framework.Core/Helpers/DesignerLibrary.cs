using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Helpers;

/// <summary>
/// Helper class for platform detection.
/// </summary>
internal static class DesignerLibrary
{
    /// <summary>
    /// Gets the detected designer library.
    /// </summary>
    /// <value>The detected designer library.</value>
    internal static DesignerPlatformLibrary DetectedDesignerLibrary
    {
        [RequiresUnreferencedCode("Calls GetCurrentPlatform(), which probes for optional third-party assemblies via Type.GetType(string).")]
        get
        {
            if (_detectedDesignerPlatformLibrary is null)
            {
                _detectedDesignerPlatformLibrary = GetCurrentPlatform();
            }
            return _detectedDesignerPlatformLibrary.Value;
        }
    }

    /// <summary>
    /// Gets the current platform.
    /// </summary>
    /// <returns>DesignerPlatformLibrary.</returns>
    /// <remarks>
    /// Type.GetType(string) here is a best-effort probe for optional third-party assemblies
    /// (WPF's PresentationFramework, WinRT's Windows.ApplicationModel) that are never part of
    /// this app's own compiled closure. It correctly returns null rather than throwing when
    /// they aren't present (the 1-arg overload defaults to throwOnError: false), including
    /// under NativeAOT/trimming -- annotated for correctness per .NET's trim-analysis rules,
    /// not because it's known to fail here.
    /// </remarks>
    [RequiresUnreferencedCode("Type.GetType(string) probes for optional third-party assemblies not part of this app's compiled closure.")]
    private static DesignerPlatformLibrary GetCurrentPlatform()
    {
        // We check Silverlight first because when in the VS designer, the .NET libraries will resolve
        // If we can resolve the SL libs, then we're in SL or WP
        // Then we check .NET because .NET will load the WinRT library (even though it can't really run it)
        // When running in WinRT, it will not load the PresentationFramework lib

        // Check .NET 
        var cmdm = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        if (cmdm is not null) // loaded the assembly, could be .net 
        {
            return DesignerPlatformLibrary.Net;
        }

        // check WinRT next
        var wadm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
        if (wadm is not null)
        {
            return DesignerPlatformLibrary.WinRt;
        }

        return DesignerPlatformLibrary.Unknown;
    }


    /// <summary>
    /// The detected designer platform library
    /// </summary>
    private static DesignerPlatformLibrary? _detectedDesignerPlatformLibrary;

    /// <summary>
    /// The is in design mode
    /// </summary>
    private static bool? _isInDesignMode;

    /// <summary>
    /// Gets a value indicating whether this instance is in design mode.
    /// </summary>
    /// <value><c>true</c> if this instance is in design mode; otherwise, <c>false</c>.</value>
    public static bool IsInDesignMode
    {
        get
        {
            if (!_isInDesignMode.HasValue)
            {
                _isInDesignMode = false;
            }

            return _isInDesignMode.Value;
        }
    }
}

/// <summary>
/// Enum DesignerPlatformLibrary
/// </summary>
internal enum DesignerPlatformLibrary
{
    /// <summary>
    /// The unknown
    /// </summary>
    Unknown,
    /// <summary>
    /// The net
    /// </summary>
    Net,
    /// <summary>
    /// The win rt
    /// </summary>
    WinRt,
    /// <summary>
    /// The silverlight
    /// </summary>
    Silverlight
}

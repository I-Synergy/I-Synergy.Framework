namespace ISynergy.Framework.UI.Exceptions;

public static partial class MauiExceptions
{
    /// <summary>
    /// Registers the native exception hooks of the current head (WinUI, Android, iOS/Catalyst).
    /// </summary>
    static partial void RegisterPlatformHandlers();

    // We'll route all unhandled exceptions through this one event.
    public static event UnhandledExceptionEventHandler? UnhandledException;

    static MauiExceptions() // NOSONAR
    {
        // This is the normal event expected, and should still be used.
        // It will fire for exceptions from iOS and Mac Catalyst,
        // and for exceptions on background threads from WinUI 3.

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            UnhandledException?.Invoke(sender, args);
        };

        // Each head registers its native handlers in Platforms/<head>/Exceptions/MauiExceptions.<head>.cs.
        RegisterPlatformHandlers();
    }
}

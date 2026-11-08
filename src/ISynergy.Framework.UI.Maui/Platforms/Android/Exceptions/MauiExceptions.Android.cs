namespace ISynergy.Framework.UI.Exceptions;

/// <summary>
/// Android exception hook: all exceptions flow through
/// <c>AndroidEnvironment.UnhandledExceptionRaiser</c>, never through AppDomain.
/// </summary>
public static partial class MauiExceptions
{
    /// <inheritdoc />
    static partial void RegisterPlatformHandlers()
    {
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
        {
            UnhandledException?.Invoke(
                sender ?? AppDomain.CurrentDomain,
                new UnhandledExceptionEventArgs(args.Exception, true));
        };
    }
}

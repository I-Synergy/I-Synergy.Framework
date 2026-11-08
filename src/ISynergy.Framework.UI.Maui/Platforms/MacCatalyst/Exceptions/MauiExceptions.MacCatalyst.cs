namespace ISynergy.Framework.UI.Exceptions;

/// <summary>
/// Mac Catalyst exception hook: managed exceptions must unwind native code for
/// <c>AppDomain.UnhandledException</c> to see them.
/// </summary>
public static partial class MauiExceptions
{
    /// <inheritdoc />
    static partial void RegisterPlatformHandlers()
    {
        ObjCRuntime.Runtime.MarshalManagedException += (_, args) =>
        {
            args.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
        };
    }
}

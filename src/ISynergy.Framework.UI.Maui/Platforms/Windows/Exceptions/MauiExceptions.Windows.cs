namespace ISynergy.Framework.UI.Exceptions;

/// <summary>
/// WinUI exception hooks: background-thread exceptions surface on AppDomain, while
/// <c>Microsoft.UI.Xaml.Application.UnhandledException</c> drops the stack, so first-chance
/// exceptions are remembered to recover it.
/// </summary>
public static partial class MauiExceptions
{
    private static Exception? _lastFirstChanceException;

    /// <inheritdoc />
    static partial void RegisterPlatformHandlers()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, args) => _lastFirstChanceException = args.Exception;

        Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, args) =>
        {
            var exception = args.Exception;

            if (exception.StackTrace is null)
                exception = _lastFirstChanceException ?? exception;

            UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(exception, true));
        };
    }
}

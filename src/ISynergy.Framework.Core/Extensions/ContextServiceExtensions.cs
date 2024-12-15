using ISynergy.Framework.Core.Services;

namespace ISynergy.Framework.Core.Extensions;

public static class ContextServiceExtensions
{
    public static async Task ExecuteInContextAsync(this IServiceProvider services, Func<ScopedContextService, Task> action)
    {
        using var context = new ScopedContextService(services);
        await action(context);
    }

    public static void ExecuteInContext(this IServiceProvider services, Action<ScopedContextService> action)
    {
        using var context = new ScopedContextService(services);
        action(context);
    }
}

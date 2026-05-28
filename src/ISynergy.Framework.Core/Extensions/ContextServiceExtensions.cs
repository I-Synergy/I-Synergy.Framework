using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Core.Extensions;

public static class ContextServiceExtensions
{
    public static async Task ExecuteInContextAsync(
        this IServiceProvider services,
        Func<IServiceProvider, Task> action)
    {
        using var scope = services.CreateScope();
        await action(scope.ServiceProvider);
    }

    public static void ExecuteInContext(
        this IServiceProvider services,
        Action<IServiceProvider> action)
    {
        using var scope = services.CreateScope();
        action(scope.ServiceProvider);
    }
}

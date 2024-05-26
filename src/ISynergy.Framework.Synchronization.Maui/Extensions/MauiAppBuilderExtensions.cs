using ISynergy.Framework.Synchronization.Abstractions.Services;
using ISynergy.Framework.Synchronization.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Synchronization.Extensions;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureOfflineSynchronization(this MauiAppBuilder builder, Action<MauiAppBuilder> action)
    {
        action.Invoke(builder);
        builder.Services.TryAddScoped<ISynchronizationService, SynchronizationService>();
        return builder;
    }
}

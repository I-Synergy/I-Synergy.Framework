using ISynergy.Framework.Synchronization.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Services;

namespace Sample.Extensions;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureOfflineSynchronization(this MauiAppBuilder builder, Action<MauiAppBuilder> action)
    {
        action.Invoke(builder);
        builder.Services.TryAddScoped<ISynchronizationService, SynchronizationService>();
        return builder;
    }
}


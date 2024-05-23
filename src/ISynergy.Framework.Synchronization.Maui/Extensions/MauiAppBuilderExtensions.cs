using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Synchronization.Extensions;
public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureOfflineSynchronization(this MauiAppBuilder builder)
    {
        builder.Services.TryAddScoped<ISynchronizationService, SynchronizationService>();
        return builder;
    }
}

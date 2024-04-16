using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Options;
using ISynergy.Framework.Synchronization.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Synchronization.Extensions;

public static class ServiceCollectionExtensions
{
    public static MauiAppBuilder ConfigureSynchronization(this MauiAppBuilder builder)
    {
        builder.Services.Configure<SynchronizationOptions>(builder.Configuration.GetSection(nameof(SynchronizationOptions)).BindWithReload);
        builder.Services.TryAddSingleton<ISynchronizationService, SynchronizationService>();
        return builder;
    }
}

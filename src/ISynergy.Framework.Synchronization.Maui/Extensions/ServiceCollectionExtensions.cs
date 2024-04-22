using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Synchronization.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSynchronization(this IServiceCollection services, Func<IServiceProvider, ISynchronizationSettingsService> implementation)
    {
        services.TryAddScoped<IBaseApplicationSettingsService>(implementation);
        services.TryAddScoped<ISynchronizationSettingsService>(implementation);

        services.TryAddScoped<ISynchronizationService, SynchronizationService>();
        return services;
    }

    public static IServiceCollection ConfigureFakeSynchronization(this IServiceCollection services, Func<IServiceProvider, ISynchronizationSettingsService> implementation)
    {
        services.TryAddScoped<IBaseApplicationSettingsService>(implementation);
        services.TryAddScoped<ISynchronizationSettingsService>(implementation);

        services.TryAddScoped<ISynchronizationService, FakeSynchronizationService>();
        return services;
    }
}

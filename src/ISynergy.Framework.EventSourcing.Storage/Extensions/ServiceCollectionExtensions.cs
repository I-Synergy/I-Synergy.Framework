using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.Configuration;
using ISynergy.Framework.EventSourcing.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.EventSourcing.Storage.Extensions;

/// <summary>
/// Extension methods for registering event archive services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IEventArchiver"/> and <see cref="IEventArchiveReader"/> with the
    /// dependency injection container.
    /// </summary>
    /// <remarks>
    /// An <see cref="IEventArchiveStorage"/> implementation must be registered separately
    /// (e.g. via <c>AddAzureEventArchiveStorage()</c> from
    /// <c>ISynergy.Framework.EventSourcing.Storage.Azure</c>).
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Used to bind <see cref="EventArchiveSettings"/>.</param>
    /// <returns>The <paramref name="services"/> for chaining.</returns>
    [RequiresUnreferencedCode("EventArchiver uses reflection to instantiate aggregate types.")]
    [RequiresDynamicCode("EventArchiver uses Activator.CreateInstance at runtime.")]
    public static IServiceCollection AddEventArchiving(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EventArchiveSettings>(
            configuration.GetSection(EventArchiveSettings.SectionName));

        services.AddScoped<IEventArchiver, EventArchiver>();
        services.AddScoped<IEventArchiveReader, EventArchiveReader>();

        return services;
    }
}

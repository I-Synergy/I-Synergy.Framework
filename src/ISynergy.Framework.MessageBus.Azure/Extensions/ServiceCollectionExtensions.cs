using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Azure.Options.Queue;
using ISynergy.Framework.MessageBus.Azure.Services.Queue;
using ISynergy.Framework.MessageBus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.MessageBus.Azure.Extensions;

/// <summary>
/// Service collection extensions for azure messagebus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds azure messagebus publish integration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddMessageBusAzurePublishIntegration<TQueuePublishMessage>(this IServiceCollection services, IConfiguration configuration)
        where TQueuePublishMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<PublisherOptions>(configuration.GetSection(nameof(PublisherOptions)).BindWithReload);
        services.AddPublishingQueueMessageBus<TQueuePublishMessage, PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>();
        return services;
    }

    /// <summary>
    /// Adds azure messagebus subscribe integration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddMessageBusAzureSubscribeIntegration<TQueueSubscribeMessage>(this IServiceCollection services, IConfiguration configuration)
        where TQueueSubscribeMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<SubscriberOptions>(configuration.GetSection(nameof(SubscriberOptions)).BindWithReload);
        services.AddSubscribingQueueMessageBus<TQueueSubscribeMessage, SubscriberServiceBus<TQueueSubscribeMessage, SubscriberOptions>>();
        return services;
    }
}

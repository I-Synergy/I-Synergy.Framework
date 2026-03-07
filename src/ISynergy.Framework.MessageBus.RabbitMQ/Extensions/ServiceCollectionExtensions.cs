using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Extensions;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Extensions;

/// <summary>
/// Service collection extensions for RabbitMQ message bus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ message bus publish integration.
    /// </summary>
    /// <typeparam name="TQueuePublishMessage">The type of the queue publish message.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessageBusRabbitMQPublishIntegration<TQueuePublishMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
        where TQueuePublishMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<PublisherOptions>(configuration.GetSection($"{prefix}{nameof(PublisherOptions)}").BindWithReload);
        services.AddPublishingQueueMessageBus<TQueuePublishMessage, PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>();
        return services;
    }

    /// <summary>
    /// Adds RabbitMQ message bus subscribe integration.
    /// </summary>
    /// <typeparam name="TQueueSubscribeMessage">The type of the queue subscribe message.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessageBusRabbitMQSubscribeIntegration<TQueueSubscribeMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
        where TQueueSubscribeMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<SubscriberOptions>(configuration.GetSection($"{prefix}{nameof(SubscriberOptions)}").BindWithReload);
        services.AddSubscribingQueueMessageBus<TQueueSubscribeMessage, SubscriberServiceBus<TQueueSubscribeMessage, SubscriberOptions>>();
        return services;
    }
}

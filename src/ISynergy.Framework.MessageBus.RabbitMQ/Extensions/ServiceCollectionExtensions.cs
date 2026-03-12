using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using ISynergy.Framework.MessageBus.Services.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization.Metadata;

namespace ISynergy.Framework.MessageBus.Extensions;

/// <summary>
/// Service collection extensions for RabbitMQ message bus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ message bus publish integration using AOT-safe source-generated JSON serialization.
    /// </summary>
    /// <typeparam name="TQueuePublishMessage">The type of the queue publish message.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="jsonTypeInfo">
    /// The <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TQueuePublishMessage"/>, obtained from a
    /// <c>[JsonSerializable]</c>-attributed <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>.
    /// Required for Native AOT publishing.
    /// </param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessageBusRabbitMQPublishIntegration<TQueuePublishMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        JsonTypeInfo<TQueuePublishMessage> jsonTypeInfo,
        string prefix = "")
        where TQueuePublishMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<PublisherOptions>(configuration.GetSection($"{prefix}{nameof(PublisherOptions)}").BindWithReload);
        services.AddPublishingQueueMessageBus<TQueuePublishMessage, PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>(
            sp => new PublisherServiceBus<TQueuePublishMessage, PublisherOptions>(
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PublisherOptions>>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>>(),
                jsonTypeInfo));
        return services;
    }

    /// <summary>
    /// Adds RabbitMQ message bus publish integration using reflection-based JSON serialization.
    /// </summary>
    /// <typeparam name="TQueuePublishMessage">The type of the queue publish message.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// This overload uses the reflection-based <see cref="System.Text.Json.JsonSerializer"/> path and is not
    /// compatible with Native AOT publishing. Use the overload that accepts a <see cref="JsonTypeInfo{T}"/>
    /// for AOT scenarios.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Serialization uses reflection. Pass a JsonTypeInfo<T> for AOT compatibility.")]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Serialization uses dynamic code generation. Pass a JsonTypeInfo<T> for AOT compatibility.")]
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
    /// Adds RabbitMQ message bus subscribe integration using AOT-safe source-generated JSON deserialization.
    /// </summary>
    /// <typeparam name="TQueueSubscribeMessage">The type of the queue subscribe message.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="jsonTypeInfo">
    /// The <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TQueueSubscribeMessage"/>, obtained from a
    /// <c>[JsonSerializable]</c>-attributed <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>.
    /// Required for Native AOT publishing.
    /// </param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessageBusRabbitMQSubscribeIntegration<TQueueSubscribeMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        JsonTypeInfo<TQueueSubscribeMessage> jsonTypeInfo,
        string prefix = "")
        where TQueueSubscribeMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<SubscriberOptions>(configuration.GetSection($"{prefix}{nameof(SubscriberOptions)}").BindWithReload);
        services.AddSubscribingQueueMessageBus<TQueueSubscribeMessage, SubscriberServiceBus<TQueueSubscribeMessage, SubscriberOptions>>();
        return services;
    }

    /// <summary>
    /// Adds RabbitMQ message bus subscribe integration using reflection-based JSON deserialization.
    /// </summary>
    /// <typeparam name="TQueueSubscribeMessage">The type of the queue subscribe message.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// This overload uses the reflection-based <see cref="System.Text.Json.JsonSerializer"/> path and is not
    /// compatible with Native AOT publishing. Use the overload that accepts a <see cref="JsonTypeInfo{T}"/>
    /// for AOT scenarios.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Deserialization uses reflection. Pass a JsonTypeInfo<T> for AOT compatibility.")]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Deserialization uses dynamic code generation. Pass a JsonTypeInfo<T> for AOT compatibility.")]
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

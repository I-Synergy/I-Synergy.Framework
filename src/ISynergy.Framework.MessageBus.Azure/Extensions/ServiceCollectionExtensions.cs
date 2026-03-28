using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Azure.Options.Queue;
using ISynergy.Framework.MessageBus.Azure.Services.Queue;
using ISynergy.Framework.MessageBus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace ISynergy.Framework.MessageBus.Azure.Extensions;

/// <summary>
/// Service collection extensions for azure messagebus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds azure messagebus publish integration using AOT-safe source-generated JSON serialization.
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
    [RequiresUnreferencedCode("Configuration binding via BindWithReload uses reflection and is not AOT-safe.")]
    [RequiresDynamicCode("Configuration binding via BindWithReload requires dynamic code generation.")]
    public static IServiceCollection AddAzureMessageBusPublishIntegration<TQueuePublishMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        JsonTypeInfo<TQueuePublishMessage> jsonTypeInfo,
        string prefix = "")
        where TQueuePublishMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<AzurePublisherOptions>(configuration.GetSection($"{prefix}{nameof(AzurePublisherOptions)}").BindWithReload);
        services.AddPublishingQueueMessageBus<TQueuePublishMessage, PublisherServiceBus<TQueuePublishMessage, AzurePublisherOptions>>(
            sp => new PublisherServiceBus<TQueuePublishMessage, AzurePublisherOptions>(
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AzurePublisherOptions>>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<PublisherServiceBus<TQueuePublishMessage, AzurePublisherOptions>>>(),
                jsonTypeInfo));
        return services;
    }

    /// <summary>
    /// Adds azure messagebus publish integration using reflection-based JSON serialization.
    /// </summary>
    /// <typeparam name="TQueuePublishMessage">The type of the queue publish message.</typeparam>
    /// <param name="services">The service collection.</param>
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
    public static IServiceCollection AddAzureMessageBusPublishIntegration<TQueuePublishMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
        where TQueuePublishMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<AzurePublisherOptions>(configuration.GetSection($"{prefix}{nameof(AzurePublisherOptions)}").BindWithReload);
        services.AddPublishingQueueMessageBus<TQueuePublishMessage, PublisherServiceBus<TQueuePublishMessage, AzurePublisherOptions>>();
        return services;
    }

    /// <summary>
    /// Adds azure messagebus subscribe integration using AOT-safe source-generated JSON deserialization.
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
    [RequiresUnreferencedCode("Configuration binding via BindWithReload uses reflection and is not AOT-safe.")]
    [RequiresDynamicCode("Configuration binding via BindWithReload requires dynamic code generation.")]
    public static IServiceCollection AddAzureMessageBusSubscribeIntegration<TQueueSubscribeMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        JsonTypeInfo<TQueueSubscribeMessage> jsonTypeInfo,
        string prefix = "")
        where TQueueSubscribeMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<AzureSubscriberOptions>(configuration.GetSection($"{prefix}{nameof(AzureSubscriberOptions)}").BindWithReload);
        services.AddSubscribingQueueMessageBus<TQueueSubscribeMessage, SubscriberServiceBus<TQueueSubscribeMessage, AzureSubscriberOptions>>();
        return services;
    }

    /// <summary>
    /// Adds azure messagebus subscribe integration using reflection-based JSON deserialization.
    /// </summary>
    /// <typeparam name="TQueueSubscribeMessage">The type of the queue subscribe message.</typeparam>
    /// <param name="services">The service collection.</param>
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
    public static IServiceCollection AddAzureMessageBusSubscribeIntegration<TQueueSubscribeMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
        where TQueueSubscribeMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<AzureSubscriberOptions>(configuration.GetSection($"{prefix}{nameof(AzureSubscriberOptions)}").BindWithReload);
        services.AddSubscribingQueueMessageBus<TQueueSubscribeMessage, SubscriberServiceBus<TQueueSubscribeMessage, AzureSubscriberOptions>>();
        return services;
    }
}

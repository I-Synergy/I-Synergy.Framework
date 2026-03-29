using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.MessageBus.Extensions;

/// <summary>
/// Class ServiceCollectionExtensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the publishing queue message bus.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the t model.</typeparam>
    /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
    /// <param name="_self">The self.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddPublishingQueueMessageBus<TQueueMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection _self)
        where TQueueMessage : class, IBaseMessage
        where TImplementation : class, IPublisherServiceBus<TQueueMessage>
    {
        _self.TryAddSingleton<TImplementation>();
        _self.TryAddSingleton<IPublisherServiceBus<TQueueMessage>, TImplementation>();

        return _self;
    }

    /// <summary>
    /// Adds the publishing queue message bus using a factory delegate.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the queue message.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    /// <param name="_self">The service collection.</param>
    /// <param name="factory">A factory delegate that creates the implementation instance.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddPublishingQueueMessageBus<TQueueMessage, TImplementation>(
        this IServiceCollection _self,
        Func<IServiceProvider, TImplementation> factory)
        where TQueueMessage : class, IBaseMessage
        where TImplementation : class, IPublisherServiceBus<TQueueMessage>
    {
        _self.TryAddSingleton(factory);
        _self.TryAddSingleton<IPublisherServiceBus<TQueueMessage>>(sp => sp.GetRequiredService<TImplementation>());

        return _self;
    }

    /// <summary>
    /// Adds the subscribing queue message bus.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the t queue message.</typeparam>
    /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
    /// <param name="_self">The self.</param>
    /// <param name="isScoped">if set to <c>true</c> [is scoped].</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddSubscribingQueueMessageBus<TQueueMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection _self, bool isScoped = false)
        where TQueueMessage : class, IBaseMessage
        where TImplementation : class, ISubscriberServiceBus<TQueueMessage>
    {
        if (isScoped)
        {
            _self.TryAddScoped<TImplementation>();
            _self.TryAddScoped<ISubscriberServiceBus<TQueueMessage>, TImplementation>();
        }
        else
        {
            _self.TryAddSingleton<TImplementation>();
            _self.TryAddSingleton<ISubscriberServiceBus<TQueueMessage>, TImplementation>();
        }

        return _self;
    }

    /// <summary>
    /// Adds the subscribing queue message bus using a factory delegate.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the queue message.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    /// <param name="_self">The service collection.</param>
    /// <param name="factory">A factory delegate that creates the implementation instance.</param>
    /// <param name="isScoped">if set to <c>true</c>, registers as scoped; otherwise singleton.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddSubscribingQueueMessageBus<TQueueMessage, TImplementation>(
        this IServiceCollection _self,
        Func<IServiceProvider, TImplementation> factory,
        bool isScoped = false)
        where TQueueMessage : class, IBaseMessage
        where TImplementation : class, ISubscriberServiceBus<TQueueMessage>
    {
        if (isScoped)
        {
            _self.TryAddScoped(factory);
            _self.TryAddScoped<ISubscriberServiceBus<TQueueMessage>>(sp => sp.GetRequiredService<TImplementation>());
        }
        else
        {
            _self.TryAddSingleton(factory);
            _self.TryAddSingleton<ISubscriberServiceBus<TQueueMessage>>(sp => sp.GetRequiredService<TImplementation>());
        }

        return _self;
    }

    /// <summary>
    /// Adds the publishing topic message bus.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the t model.</typeparam>
    /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
    /// <param name="_self">The self.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddPublishingTopicMessageBus<TQueueMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection _self)
        where TQueueMessage : class, IBaseMessage
        where TImplementation : class, IPublisherServiceBus<TQueueMessage>
    {
        _self.TryAddSingleton<TImplementation>();
        _self.TryAddSingleton<IPublisherServiceBus<TQueueMessage>, TImplementation>();

        return _self;
    }

    /// <summary>
    /// Adds the subscribing topic message bus.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the t model.</typeparam>
    /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
    /// <param name="_self">The self.</param>
    /// <param name="isScoped">if set to <c>true</c> [is scoped].</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddSubscribingTopicMessageBus<TQueueMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection _self, bool isScoped = false)
        where TQueueMessage : class, IBaseMessage
        where TImplementation : class, ISubscriberServiceBus<TQueueMessage>
    {
        if (isScoped)
        {
            _self.TryAddScoped<TImplementation>();
            _self.TryAddScoped<ISubscriberServiceBus<TQueueMessage>, TImplementation>();
        }
        else
        {
            _self.TryAddSingleton<TImplementation>();
            _self.TryAddSingleton<ISubscriberServiceBus<TQueueMessage>, TImplementation>();
        }

        return _self;
    }
}

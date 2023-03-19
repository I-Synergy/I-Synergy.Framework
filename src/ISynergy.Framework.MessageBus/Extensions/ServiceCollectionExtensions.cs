using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.MessageBus.Extensions
{
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
        public static IServiceCollection AddPublishingQueueMessageBus<TQueueMessage, TImplementation>(this IServiceCollection _self)
            where TQueueMessage : class, IBaseMessage
            where TImplementation : class, IPublisherServiceBus<TQueueMessage>
        {
            _self.TryAddSingleton<TImplementation>();
            _self.TryAddSingleton<IPublisherServiceBus<TQueueMessage>, TImplementation>();

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
        public static IServiceCollection AddSubscribingQueueMessageBus<TQueueMessage, TImplementation>(this IServiceCollection _self, bool isScoped = false)
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
        /// Adds the publishing topic message bus.
        /// </summary>
        /// <typeparam name="TQueueMessage">The type of the t model.</typeparam>
        /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
        /// <param name="_self">The self.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddPublishingTopicMessageBus<TQueueMessage, TImplementation>(this IServiceCollection _self)
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
        public static IServiceCollection AddSubscribingTopicMessageBus<TQueueMessage, TImplementation>(this IServiceCollection _self, bool isScoped = false)
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
}

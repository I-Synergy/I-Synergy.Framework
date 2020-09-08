using ISynergy.Framework.MessageBus.Abstractions;

namespace ISynergy.Framework.MessageBus.Models
{
    /// <summary>
    /// Class HubMessage.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public class HubMessage<TEntity> : IHubMessage<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HubMessage{TEntity}" /> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="event">The event.</param>
        /// <param name="data">The data.</param>
        public HubMessage(string channel, string @event, TEntity data = default)
        {
            Channel = channel;
            Event = @event;
            Data = data;
            ContentType = data.GetType().Name;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel { get; }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>The event.</value>
        public string Event { get; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public TEntity Data { get; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }
    }
}

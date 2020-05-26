using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Enumerations;

namespace ISynergy.Framework.MessageBus.Azure.Models
{
    /// <summary>
    /// Class QueueMessage.
    /// Implements the <see cref="T:ISynergy.Framework.MessageBus.Abstractions.IQueueMessage{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public abstract class QueueMessage<TEntity> : IQueueMessage<TEntity>
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public TEntity Data { get; }
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public QueueMessageActions Action { get; }
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }
        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        public string To { get; }
        /// <summary>
        /// Gets the reply to.
        /// </summary>
        /// <value>The reply to.</value>
        public string ReplyTo { get; }
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMessage{TEntity}" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="data">The data.</param>
        protected QueueMessage(QueueMessageActions action, TEntity data)
        {
            Action = action;
            Data = data;
            ContentType = data.GetType().Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMessage{TEntity}" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="data">The data.</param>
        /// <param name="tag">The tag.</param>
        protected QueueMessage(QueueMessageActions action, TEntity data, string tag)
            : this(action, data)
        {
            Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMessage{TEntity}" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="data">The data.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="to">To.</param>
        protected QueueMessage(QueueMessageActions action, TEntity data, string tag, string to)
            : this(action, data, tag)
        {
            To = to;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMessage{TEntity}" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="data">The data.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="to">To.</param>
        /// <param name="replyTo">The reply to.</param>
        protected QueueMessage(QueueMessageActions action, TEntity data, string tag, string to, string replyTo)
            : this(action, data, tag, to)
        {
            ReplyTo = replyTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMessage{TEntity}" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="data">The data.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="to">To.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <param name="contentType">Type of the content.</param>
        protected QueueMessage(QueueMessageActions action, TEntity data, string tag, string to, string replyTo, string contentType)
            : this(action, data, tag, to, replyTo)
        {
            ContentType = contentType;
        }
    }
}

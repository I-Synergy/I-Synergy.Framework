using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class ItemSelectedMessage.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="EventMessage" />
    public class ItemSelectedMessage<T> : EventMessage
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSelectedMessage{T}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public ItemSelectedMessage(object sender, T value)
            : base(sender)
        {
            Value = value;
        }
    }
}

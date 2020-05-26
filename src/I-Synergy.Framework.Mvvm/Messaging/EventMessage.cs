namespace ISynergy.Framework.Mvvm.Messaging
{
    /// <summary>
    /// Base class for all messages broadcasted by the Messenger.
    /// You can create your own message types by extending this class.
    /// </summary>
    public abstract class EventMessage
    {
        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        protected EventMessage()
        {
            Handled = false;
        }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        protected EventMessage(object sender)
            : this()
        {
            Sender = sender;
        }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, and may be null.</param>
        protected EventMessage(object sender, object target)
            : this(sender)
        {
            Target = target;
        }

        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public object Sender { get; }

        /// <summary>
        /// Gets or sets the message's intended target. This property can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, and may be null.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Property to mark message as handled or not.
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}

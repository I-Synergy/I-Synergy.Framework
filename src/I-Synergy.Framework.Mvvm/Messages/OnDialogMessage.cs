using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class OnDialogMessage.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class OnDialogMessage : EventMessage
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public NotificationTypes Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDialogMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public OnDialogMessage(object sender, string message, NotificationTypes type)
            : base(sender)
        {
            Message = message;
            Type = type;
            Handled = false;
        }
    }
}

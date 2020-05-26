using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class OnlineModeChangedMessage.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class OnlineModeChangedMessage : EventMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineModeChangedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public OnlineModeChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}

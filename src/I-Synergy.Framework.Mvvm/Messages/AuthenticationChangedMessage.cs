using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class AuthenticationChangedMessage.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class AuthenticationChangedMessage : EventMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationChangedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public AuthenticationChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}

using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class ExceptionHandledMessage.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class ExceptionHandledMessage : EventMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandledMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public ExceptionHandledMessage(object sender)
            : base(sender)
        {
        }
    }
}

using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class AuthenticateUserMessageResult.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class AuthenticateUserMessageResult : EventMessage
    {
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public object Property { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateUserMessageResult"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="property">The property.</param>
        /// <param name="authenticated">if set to <c>true</c> [authenticated].</param>
        public AuthenticateUserMessageResult(object sender, object property, bool authenticated)
            : base(sender)
        {
            Property = property;
            IsAuthenticated = authenticated;
        }
    }
}

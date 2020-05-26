using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class AuthenticateUserMessageRequest.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class AuthenticateUserMessageRequest : EventMessage
    {
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public object Property { get; }
        /// <summary>
        /// Gets a value indicating whether [enable login].
        /// </summary>
        /// <value><c>true</c> if [enable login]; otherwise, <c>false</c>.</value>
        public bool EnableLogin { get; }
        /// <summary>
        /// Gets a value indicating whether [show login].
        /// </summary>
        /// <value><c>true</c> if [show login]; otherwise, <c>false</c>.</value>
        public bool ShowLogin { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateUserMessageRequest"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="showLogin">if set to <c>true</c> [show login].</param>
        /// <param name="property">The property.</param>
        /// <param name="enableLogin">if set to <c>true</c> [enable login].</param>
        public AuthenticateUserMessageRequest(object sender, bool showLogin, object property = null, bool enableLogin = true)
            : base(sender)
        {
            Property = property;
            EnableLogin = enableLogin;
            ShowLogin = showLogin;
        }
    }
}

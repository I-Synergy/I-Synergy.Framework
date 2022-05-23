using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using System.Collections.Generic;

namespace ISynergy.Framework.MessageBus.Abstractions.Messages
{
    /// <summary>
    /// Interface IEmailMessage
    /// </summary>
    public interface IEmailMessage : IBaseMessage
    {
        /// <summary>
        /// Gets the email address from.
        /// </summary>
        /// <value>The email address from.</value>
        string EmailAddressFrom { get; }
        /// <summary>
        /// Gets the email addresses to.
        /// </summary>
        /// <value>The email addresses to.</value>
        List<string> EmailAddressesTo { get; }

        /// <summary>
        /// Gets or sets the email addresses cc.
        /// </summary>
        /// <value>The email addresses cc.</value>
        List<string> EmailAddressesCc { get; set; }
        /// <summary>
        /// Gets or sets the email addresses BCC.
        /// </summary>
        /// <value>The email addresses BCC.</value>
        List<string> EmailAddressesBcc { get; set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        string Subject { get; }
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; }
        /// <summary>
        /// Gets a value indicating whether [send copy].
        /// </summary>
        /// <value><c>true</c> if [send copy]; otherwise, <c>false</c>.</value>
        bool SendCopy { get; }
    }
}

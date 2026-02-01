namespace ISynergy.Framework.Mail.Models;

/// <summary>
/// Mail message model.
/// </summary>
public class MailMessage
{
    /// <summary>
    /// Gets the email address from.
    /// </summary>
    /// <value>The email address from.</value>
    public string? EmailAddressFrom { get; }
    /// <summary>
    /// Gets the email addresses to.
    /// </summary>
    /// <value>The email addresses to.</value>
    public List<string> EmailAddressesTo { get; }

    /// <summary>
    /// Gets or sets the email addresses cc.
    /// </summary>
    /// <value>The email addresses cc.</value>
    public List<string> EmailAddressesCc { get; set; } = new List<string>();
    /// <summary>
    /// Gets or sets the email addresses BCC.
    /// </summary>
    /// <value>The email addresses BCC.</value>
    public List<string> EmailAddressesBcc { get; set; } = new List<string>();

    /// <summary>
    /// Gets the subject.
    /// </summary>
    /// <value>The subject.</value>
    public string Subject { get; }
    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; }
    /// <summary>
    /// Gets a value indicating whether [send copy].
    /// </summary>
    /// <value><c>true</c> if [send copy]; otherwise, <c>false</c>.</value>
    public bool SendCopy { get; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>The tag.</value>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Gets the type of the content.
    /// </summary>
    /// <value>The type of the content.</value>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Constructor of MailMessage.
    /// </summary>
    /// <param name="emailAddressesTo">The email addresses to.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="message">The message.</param>
    /// <param name="sendCopy">if set to <c>true</c> [send copy].</param>
    public MailMessage(List<string> emailAddressesTo, string subject, string message, bool sendCopy)
    {
        EmailAddressesTo = emailAddressesTo;
        Subject = subject;
        Message = message;
        SendCopy = sendCopy;
    }
}

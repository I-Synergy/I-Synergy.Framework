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
    public List<string> EmailAddressesCc { get; set; } = [];

    /// <summary>
    /// Gets or sets the email addresses BCC.
    /// </summary>
    /// <value>The email addresses BCC.</value>
    public List<string> EmailAddressesBcc { get; set; } = [];

    /// <summary>
    /// Gets the subject.
    /// </summary>
    /// <value>The subject.</value>
    public string Subject { get; }

    /// <summary>
    /// Gets the message body.
    /// </summary>
    /// <value>The message.</value>
    /// <remarks>
    /// <strong>Security notice:</strong> When <see cref="ContentType"/> is set to an HTML MIME type,
    /// callers are responsible for sanitizing user-supplied content before assigning it here.
    /// Unsanitized HTML may enable XSS or content-injection attacks in email clients that render HTML.
    /// </remarks>
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
    /// <param name="emailAddressesTo">The email addresses to. Each address is validated to reject header-injection characters.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="message">The message.</param>
    /// <param name="sendCopy">if set to <c>true</c> [send copy].</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="emailAddressesTo"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when any address in <paramref name="emailAddressesTo"/> contains header-injection characters (<c>\r</c>, <c>\n</c>, or <c>;</c>).</exception>
    public MailMessage(List<string> emailAddressesTo, string subject, string message, bool sendCopy)
    {
        ArgumentNullException.ThrowIfNull(emailAddressesTo);

        foreach (var address in emailAddressesTo)
            ValidateEmailAddress(address);

        EmailAddressesTo = emailAddressesTo;
        Subject = subject;
        Message = message;
        SendCopy = sendCopy;
    }

    /// <summary>
    /// Validates that an email address does not contain SMTP header-injection characters.
    /// </summary>
    /// <param name="address">The email address to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="address"/> contains <c>\r</c>, <c>\n</c>, or <c>;</c>.
    /// </exception>
    public static void ValidateEmailAddress(string address)
    {
        if (address is null)
            return;

        if (address.Contains('\r', StringComparison.Ordinal) ||
            address.Contains('\n', StringComparison.Ordinal) ||
            address.Contains(';', StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Email address '{address}' contains invalid characters (CR, LF, or semicolon) that could be used for header injection.",
                nameof(address));
        }
    }
}

using ISynergy.Framework.Core.Options;

namespace ISynergy.Framework.Mail.Options.Base;

/// <summary>
/// Mail service options.
/// </summary>
public abstract class BaseMailOptions : ClientApplicationOptions
{
    /// <summary>
    /// Gets or sets the email address of the sender.
    /// </summary>
    /// <value>The email address.</value>
    public string EmailAddress { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the description or name of the sender.
    /// </summary>
    /// <value>The email sender.</value>
    public string Sender { get; set; } = string.Empty;
}

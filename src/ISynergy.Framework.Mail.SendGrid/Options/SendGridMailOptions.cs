using ISynergy.Framework.Mail.Options.Base;

namespace ISynergy.Framework.Mail.SendGrid.Options;

/// <summary>
/// Sendgrid options.
/// </summary>
internal class SendGridMailOptions : BaseMailOptions
{
    /// <summary>
    /// Sendgrid Api key
    /// </summary>
    public string Key { get; set; } = string.Empty;
}

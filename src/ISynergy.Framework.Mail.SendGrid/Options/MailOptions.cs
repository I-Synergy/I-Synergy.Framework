using ISynergy.Framework.Mail.Options.Base;

namespace ISynergy.Framework.Mail.Options;

/// <summary>
/// Sendgrid options.
/// </summary>
internal class MailOptions : BaseMailOptions
{
    /// <summary>
    /// Sendgrid Api key
    /// </summary>
    public string Key { get; set; } = string.Empty;
}

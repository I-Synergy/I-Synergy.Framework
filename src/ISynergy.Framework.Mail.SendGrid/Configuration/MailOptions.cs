namespace ISynergy.Framework.Mail.Configuration;

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

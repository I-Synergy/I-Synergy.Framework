namespace ISynergy.Framework.Mail.Options;

internal class MailOptions : BaseMailOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = new string[] { "https://graph.microsoft.com/.default" };
}

using ISynergy.Framework.Mail.Options.Base;

namespace ISynergy.Framework.Mail.Options;

internal class MailOptions : BaseMailOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];
}

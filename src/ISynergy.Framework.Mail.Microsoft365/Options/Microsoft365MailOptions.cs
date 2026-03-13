using ISynergy.Framework.Mail.Options.Base;

namespace ISynergy.Framework.Mail.Microsoft365.Options;

internal class Microsoft365MailOptions : BaseMailOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];
}

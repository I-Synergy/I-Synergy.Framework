using ISynergy.Framework.Mail.Options.Base;

namespace ISynergy.Framework.Mail.Microsoft365.Options;

internal class Microsoft365MailOptions : BaseMailOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];

    /// <summary>
    /// Gets or sets a value indicating whether the mail service authenticates with a managed identity
    /// instead of a client secret. When <c>true</c>, identity takes precedence even if
    /// <see cref="ISynergy.Framework.Core.Options.ClientApplicationOptions.ClientSecret"/> is set.
    /// </summary>
    /// <remarks>
    /// Identity is also used implicitly when no client secret is configured. The identity (system- or
    /// user-assigned managed identity, or the developer credential resolved by
    /// <c>DefaultAzureCredential</c>) must hold the Microsoft Graph <c>Mail.Send</c> application
    /// permission for the configured sender mailbox.
    /// </remarks>
    public bool UseManagedIdentity { get; set; }

    /// <summary>
    /// Gets or sets the client identifier of a user-assigned managed identity.
    /// When empty, the system-assigned identity (or another <c>DefaultAzureCredential</c> source)
    /// is used.
    /// </summary>
    public string? ManagedIdentityClientId { get; set; }
}

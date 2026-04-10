namespace ISynergy.Framework.Core.Options;

/// <summary>
/// Base configuration options for client applications.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Security notice:</strong> Properties that hold credentials (<see cref="ClientSecret"/>)
/// must <em>never</em> be populated from plain-text configuration files such as
/// <c>appsettings.json</c> or <c>appsettings.Production.json</c>.
/// Source secrets exclusively from a Key Vault provider (Azure Key Vault, OpenBao) or
/// environment variables injected by the hosting platform.
/// </para>
/// </remarks>
public class ClientApplicationOptions
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>The client identifier.</value>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the OAuth 2.0 client secret used for client-credentials authentication flows.
    /// </summary>
    /// <value>The client secret.</value>
    /// <remarks>
    /// <para>
    /// <strong>Security warning:</strong> This value is a sensitive credential.
    /// It must <em>never</em> be stored in source control or in plain-text configuration files.
    /// Retrieve it at runtime from a Key Vault service (e.g. <c>IKeyVaultService</c>) or from a
    /// platform-provided secret store (environment variable, Kubernetes secret, managed identity).
    /// </para>
    /// <para>
    /// Where the identity platform supports it (Azure AD, Entra ID), prefer Managed Identity or
    /// Workload Identity Federation over client-secret credentials entirely.
    /// </para>
    /// </remarks>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the network endpoint URI used to connect to the service.
    /// </summary>
    public string? Endpoint { get; set; }
}

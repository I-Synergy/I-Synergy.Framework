namespace ISynergy.Framework.Core.Options;

/// <summary>
/// Base configuration options
/// </summary>
public class ClientApplicationOptions
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>The client identifier.</value>
    public string? ClientId { get; set; }
    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>The client secret.</value>
    public string? ClientSecret { get; set; }
    /// <summary>
    /// Gets or sets the network endpoint URI used to connect to the service.
    /// </summary>
    public string? Endpoint { get; set; }
}

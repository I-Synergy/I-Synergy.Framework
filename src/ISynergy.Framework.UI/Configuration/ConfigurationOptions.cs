namespace ISynergy.Framework.UI.Configuration;

/// <summary>
/// Base configuration options
/// </summary>
public class ConfigurationOptions
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>The client identifier.</value>
    public string ClientId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>The client secret.</value>
    public string ClientSecret { get; set; } = string.Empty;
}

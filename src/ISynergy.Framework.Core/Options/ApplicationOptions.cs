namespace ISynergy.Framework.Core.Options;

/// <summary>
/// Base configuration options
/// </summary>
public class ApplicationOptions
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
}

namespace ISynergy.Framework.AspNetCore.Options;

/// <summary>
/// CORS (Cross-Origin Resource Sharing) configuration options.
/// </summary>
public class CORSOptions // NOSONAR
{
    /// <summary>
    /// Gets or sets the list of allowed origins for CORS requests.
    /// </summary>
    public string[] AllowedOrigins { get; set; } = [];
}

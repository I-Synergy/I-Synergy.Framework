#pragma warning disable S101 // CORSOptions uses established CORS acronym; renaming would break public API

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

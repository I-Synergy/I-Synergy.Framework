using ISynergy.Framework.Core.Options;

namespace ISynergy.Framework.AspNetCore.Options;

/// <summary>
/// Class KeyVaultOptions.
/// </summary>
public class KeyVaultOptions : ApplicationOptions
{
    /// <summary>
    /// Gets or sets the key vault URI.
    /// </summary>
    /// <value>The key vault URI.</value>
    public Uri? KeyVaultUri { get; set; }
}
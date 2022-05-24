using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Models
{
    /// <summary>
    /// Class Token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>The access token.</value>
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
        /// <summary>
        /// Gets or sets the identifier token.
        /// </summary>
        /// <value>The identifier token.</value>
        [JsonPropertyName("id_token")] public string IdToken { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>The expires in.</value>
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        [JsonPropertyName("token_type")] public string TokenType { get; set; }
    }
}

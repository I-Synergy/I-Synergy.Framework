namespace ISynergy.Framework.Payment.Mollie.Models.Connect
{
    /// <summary>
    /// Class TokenRequest.
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRequest" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        public TokenRequest(string code, string redirectUri)
        {
            if (code.StartsWith("refresh_"))
            {
                GrantType = "refresh_token";
                RefreshToken = code;
            }
            else
            {
                GrantType = "authorization_code";
                Code = code;
            }
            RedirectUri = redirectUri;
        }

        /// <summary>
        /// If you wish to exchange your auth code for an access token, use grant type authorization_code. If you wish to renew
        /// your access token with your refresh token, use grant type refresh_token.
        /// Possible values: authorization_code refresh_token
        /// </summary>
        /// <value>The type of the grant.</value>
        public string GrantType { get; }

        /// <summary>
        /// Optional – The auth code you've received when creating the authorization. Only use this field when using grant type
        /// authorization_code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>
        /// Optional – The refresh token you've received when creating the authorization. Only use this field when using grant
        /// type refresh_token.
        /// </summary>
        /// <value>The refresh token.</value>
        public string RefreshToken { get; }

        /// <summary>
        /// The URL the merchant is sent back to once the request has been authorized. It must match the URL you set when
        /// registering your app.
        /// </summary>
        /// <value>The redirect URI.</value>
        public string RedirectUri { get; }
    }
}

namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Class AuthenticationEndpointOptions.
    /// </summary>
    public class AuthenticationEndpointOptions
    {
        /// <summary>
        /// Gets or sets the token endpoint path.
        /// </summary>
        /// <value>The token endpoint path.</value>
        public string TokenEndpointPath { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the authorization endpoint path.
        /// </summary>
        /// <value>The authorization endpoint path.</value>
        public string AuthorizationEndpointPath { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the logout endpoint path.
        /// </summary>
        /// <value>The logout endpoint path.</value>
        public string LogoutEndpointPath { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the userinfo endpoint path.
        /// </summary>
        /// <value>The userinfo endpoint path.</value>
        public string UserinfoEndpointPath { get; set; } = string.Empty;
    }
}
